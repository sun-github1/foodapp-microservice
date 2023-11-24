using Azure.Messaging.ServiceBus;
using Food.MessageBus;
using Food.Services.PaymentAPI.Messages;
using Newtonsoft.Json;
using PaymentProcessor;
using System.Text;

namespace Food.Services.PaymentAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        //private readonly AutoMapper.IMapper _mapper;
        //private readonly OrderRepository _orderRepository;
        private readonly string serviceBusConnectionString;
        //private readonly string subscriptionCheckOut;
        private readonly string orderUpdatePaymentMessageTopic;
        private readonly string subscriptionOrderPayment;
        private readonly string orderPaymentMessageTopic;
        private readonly IConfiguration _configuration;
        private readonly IProcessPayment _processPayment;

        private ServiceBusProcessor ordeerPaymentProcessor;
        private readonly ILogger<AzureServiceBusConsumer> _logger;
        private readonly IMessageBus _messageBus;
        public AzureServiceBusConsumer(ILogger<AzureServiceBusConsumer> logger, IProcessPayment processPayment,
            IConfiguration configuration, IMessageBus messageBus)
        {
            //_mapper = mapper;
            //_orderRepository = orderRepository;
            _configuration = configuration;
            _logger = logger;
            _messageBus = messageBus;
            _processPayment = processPayment;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            orderPaymentMessageTopic = _configuration.GetValue<string>("OrderPaymentMessageTopic");
            subscriptionOrderPayment = _configuration.GetValue<string>("SubscriptionOrderPayment");
            orderUpdatePaymentMessageTopic = _configuration.GetValue<string>("OrderUpdatePaymentTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);
            ordeerPaymentProcessor = client.CreateProcessor(orderPaymentMessageTopic, subscriptionOrderPayment);
        }

        public async Task Start()
        {
            ordeerPaymentProcessor.ProcessMessageAsync += OnPaymentProcesssMessageReceived;
            ordeerPaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await ordeerPaymentProcessor.StartProcessingAsync();
        }

        private async Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception.ToString());
        }

        public async Task Stop()
        {
            await ordeerPaymentProcessor.StopProcessingAsync();
            await ordeerPaymentProcessor.DisposeAsync();
        }

        private async Task OnPaymentProcesssMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            PaymentRequestMessage paymentReqMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                Status=result,
                OrderId= paymentReqMessage.OrderId,
                Email= paymentReqMessage.Email
            };
            try
            {
                await _messageBus.PublishMessage(updatePaymentResultMessage, orderUpdatePaymentMessageTopic);
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
