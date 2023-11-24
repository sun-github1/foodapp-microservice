using Azure.Messaging.ServiceBus;
using Food.MessageBus;
using Food.Services.OrderAPI.Messages;
using Food.Services.OrderAPI.Models;
using Food.Services.OrderAPI.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Food.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly AutoMapper.IMapper _mapper;
        private readonly OrderRepository _orderRepository;
        private readonly string serviceBusConnectionString;
        private readonly string subscriptionCheckOut;
        private readonly string checkoutMessageTopic;
        private readonly string orderUpdatePaymentMessageTopic;
        private readonly string orderPaymentMessageTopic;
        private readonly IConfiguration _configuration;

        private ServiceBusProcessor checkOutProcessor;
        private ServiceBusProcessor orderUpdatePaymentStatusProcessor;
        private readonly ILogger<AzureServiceBusConsumer> _logger;
        private readonly IMessageBus _messageBus;
        public AzureServiceBusConsumer(ILogger<AzureServiceBusConsumer> logger, AutoMapper.IMapper mapper, OrderRepository orderRepository,
            IConfiguration configuration, IMessageBus messageBus)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _configuration = configuration;
            _logger = logger;
            _messageBus = messageBus;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            subscriptionCheckOut = _configuration.GetValue<string>("SubscriptionCheckout");
            orderPaymentMessageTopic = _configuration.GetValue<string>("OrderPaymentMessageTopic");
            orderUpdatePaymentMessageTopic = _configuration.GetValue<string>("OrderUpdatePaymentTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);
            checkOutProcessor = client.CreateProcessor(checkoutMessageTopic, subscriptionCheckOut);
            orderUpdatePaymentStatusProcessor = client.CreateProcessor(orderUpdatePaymentMessageTopic, subscriptionCheckOut);
        }

        public async Task Start()
        {
            checkOutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            checkOutProcessor.ProcessErrorAsync += ErrorHandler;
            await checkOutProcessor.StartProcessingAsync();
            orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrdrPaymentUpdateMessageReceived;
            orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
        }

        private async Task OnOrdrPaymentUpdateMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
            await args.CompleteMessageAsync(args.Message);
        }

        private async Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception.ToString());
        }

        public async Task Stop()
        {
            await checkOutProcessor.StopProcessingAsync();
            await checkOutProcessor.DisposeAsync();
        }

        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);
            OrderHeader orderHeader = _mapper.Map<OrderHeader>(checkoutHeaderDto);
            orderHeader.OrderTime = DateTime.Now;
            orderHeader.OrderDetails = new List<OrderDetails>();
            foreach (var detailList in checkoutHeaderDto.CartDetails)
            {
                OrderDetails orderDetails = _mapper.Map<OrderDetails>(detailList);
                //OrderDetails orderDetails = new()
                //{
                //ProductId = detailList.ProductId,
                orderDetails.ProductName = detailList.Product.Name;
                orderDetails.Price = detailList.Product.Price;
                //Count = detailList.Count
                //};
                orderHeader.CartTotalItems += detailList.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            }
            var newordr = await _orderRepository.AddOrder(orderHeader);
            PaymentRequestMessage paymentRequestMessage = new()
            {
                Name = newordr.FirstName + " " + newordr.LastName,
                CardNumber = newordr.CardNumber,
                CVV = newordr.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderId = newordr.OrderHeaderId,
                OrderTotal = newordr.OrderTotal,
                Email = newordr.Email
            };
            try
            {
                await _messageBus.PublishMessage(paymentRequestMessage, orderPaymentMessageTopic);
                // complete the message
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());
                throw;
            }
        }
    }
}
