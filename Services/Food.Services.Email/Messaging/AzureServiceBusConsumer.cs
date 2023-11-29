using Azure.Messaging.ServiceBus;
using Food.Services.Email.Messages;
using Food.Services.Email.Models;
using Food.Services.Email.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Food.Services.Email.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly EmailRepository _emailRepository;
        private readonly string serviceBusConnectionString;
        private readonly string subscriptionEmail;
        private readonly string orderUpdatePaymentMessageTopic;
        private readonly IConfiguration _configuration;

        private ServiceBusProcessor checkOutProcessor;
        private ServiceBusProcessor orderUpdatePaymentStatusProcessor;
        private readonly ILogger<AzureServiceBusConsumer> _logger;
        public AzureServiceBusConsumer(ILogger<AzureServiceBusConsumer> logger,  EmailRepository emailRepository,
            IConfiguration configuration)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;
            _logger = logger;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            subscriptionEmail = _configuration.GetValue<string>("SubscriptionEmail");
            orderUpdatePaymentMessageTopic = _configuration.GetValue<string>("OrderUpdatePaymentTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);
            orderUpdatePaymentStatusProcessor = client.CreateProcessor(orderUpdatePaymentMessageTopic, subscriptionEmail);
        }

        public async Task Start()
        {
            orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrdrPaymentUpdateMessageReceived;
            orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
        }

        private async Task OnOrdrPaymentUpdateMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject
                <UpdatePaymentResultMessage>(body);

            await _emailRepository.SendAndLogEmail(paymentResultMessage);
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
    }
}
