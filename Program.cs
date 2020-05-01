using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static IQueueClient subscriptionClient;

        static void Main(string[] args)
        {
            Console.WriteLine("Listner started.");
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            const string ServiceBusConnectionString = "Endpoint=sb://studentservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=O5BCsQW4K2UvesE/JzS7WOMI5UdbsAC3MKzRfb+qIik="; // enter primary key name here
            const string QueueName = "studentqqueue"; // enter queue name here
            

            subscriptionClient = new QueueClient(ServiceBusConnectionString, QueueName);

            // Register subscription message handler and receive messages in a loop.
            RegisterOnMessageHandlerAndReceiveMessages();

            Console.ReadKey();

            await subscriptionClient.CloseAsync();
        }

        static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler);

            // Register the function that processes messages.
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var messageBody = Encoding.UTF8.GetString(message.Body);

            var serviceBusMessage = JsonConvert.DeserializeObject<ServiceBusMessage>(messageBody);

            Console.WriteLine($"Received message: UserInfo:{Encoding.UTF8.GetString(message.Body)}");
            Console.WriteLine("\n");

            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var exception = exceptionReceivedEventArgs.Exception;

            return Task.CompletedTask;
        }

        public class ServiceBusMessage
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Content { get; set; }
        }
    }
}