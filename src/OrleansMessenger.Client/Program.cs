using Orleans;
using Orleans.Streams;
using OrleansMessenger.GrainInterfaces;
using System;
using System.Threading.Tasks;

namespace OrleansMessenger.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            GrainClient.Initialize();

            Console.Write("Enter user name: ");
            var user = Console.ReadLine();

            var userGrain = GrainClient.GrainFactory.GetGrain<IUserGrain>(user);

            var subscription = GrainClient.GetStreamProvider("SMSProvider")
                .GetStream<string>(Guid.Parse("FED26B31-9D86-4F30-8128-01BA23880066"), user)
                .SubscribeAsync(new IncommingMessageObserver())
                .Result;

            foreach (var message in userGrain.GetHistoricalMessages(10).Result)
            {
                Console.WriteLine(message);
            }

            var command = "";
            while (true)
            {
                command = Console.ReadLine();

                if (command == "q") break;
                if (command == "h")
                {
                    foreach (var message in userGrain.GetHistoricalMessages().Result)
                    {
                        Console.WriteLine(message);
                    }
                }
                if (command.StartsWith("s", StringComparison.Ordinal))
                {
                    var commandArgs = command.Split(':');
                    userGrain.SendMessage(commandArgs[2], commandArgs[1]);
                }
            }

            subscription.UnsubscribeAsync().Wait();

            GrainClient.Uninitialize();
        }

        class IncommingMessageObserver : IAsyncObserver<string>
        {
            public Task OnCompletedAsync()
            {
                return TaskDone.Done;
            }

            public Task OnErrorAsync(Exception ex)
            {
                return TaskDone.Done;
            }

            public Task OnNextAsync(string message, StreamSequenceToken token = null)
            {
                Console.WriteLine(message);
                return TaskDone.Done;
            }
        }
    }
}
