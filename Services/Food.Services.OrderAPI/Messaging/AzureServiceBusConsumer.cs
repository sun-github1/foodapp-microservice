using Azure.Messaging.ServiceBus;
using Food.Services.OrderAPI.Messages;
using Food.Services.OrderAPI.Models;
using Food.Services.OrderAPI.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Food.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly AutoMapper.IMapper _mapper;
        private readonly OrderRepository _orderRepository;
        private readonly string serviceBusConnectionString;
        private readonly string subscriptionCheckOut;
        private readonly string checkoutMessageTopic;
        private readonly IConfiguration _configuration;

        private ServiceBusProcessor checkOutProcessor;
        private readonly ILogger<AzureServiceBusConsumer> _logger;
        public AzureServiceBusConsumer(ILogger<AzureServiceBusConsumer> logger, AutoMapper.IMapper mapper, OrderRepository orderRepository, 
            IConfiguration configuration)
        {
            _mapper= mapper;
            _orderRepository= orderRepository;
            _configuration= configuration;
            _logger= logger;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            subscriptionCheckOut = _configuration.GetValue<string>("SubscriptionCheckout");

            var client= new ServiceBusClient(serviceBusConnectionString);
            checkOutProcessor=client.CreateProcessor(checkoutMessageTopic, subscriptionCheckOut);
        }

        public async Task Start()
        {
            checkOutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            checkOutProcessor.ProcessErrorAsync += ErrorHandler;
            await checkOutProcessor.StartProcessingAsync();
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
            var message= args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);
            OrderHeader orderHeader= _mapper.Map<OrderHeader>(checkoutHeaderDto);
            orderHeader.OrderTime=DateTime.Now;
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
            await _orderRepository.AddOrder(orderHeader);
            // complete the message
            await args.CompleteMessageAsync(args.Message);

        }
    }
}
