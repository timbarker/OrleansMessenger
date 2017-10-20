﻿using Orleans;
using Orleans.Streams;
using OrleansMessenger.GrainInterfaces;
using System;
using System.Threading.Tasks;

namespace OrleansMessenger.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrainClient.Initialize();

            Console.Write("Enter user name: ");
            var user = Console.ReadLine();

            var userGrain = GrainClient.GrainFactory.GetGrain<IUserGrain>(user);

            var subscription = await GrainClient.GetStreamProvider("SMSProvider")
                .GetStream<string>(Guid.Parse("FED26B31-9D86-4F30-8128-01BA23880066"), user)
                .SubscribeAsync(new IncommingMessageObserver());

            foreach (var message in await userGrain.GetHistoricalMessages(10))
            {
                Console.WriteLine(message);
            }

            Console.WriteLine("commands:");
            Console.WriteLine("\tq - quit");
            Console.WriteLine("\th - get historical messages");
            Console.WriteLine("\ts:to:body - send a message");

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
                    await userGrain.SendMessage(commandArgs[2], commandArgs[1]);
                }
            }

            await subscription.UnsubscribeAsync();

            GrainClient.Uninitialize();
        }

        class IncommingMessageObserver : IAsyncObserver<string>
        {
            public Task OnCompletedAsync()
            {
                return Task.CompletedTask;
            }

            public Task OnErrorAsync(Exception ex)
            {
                return Task.CompletedTask;
            }

            public Task OnNextAsync(string message, StreamSequenceToken token = null)
            {
                Console.WriteLine(message);
                return Task.CompletedTask;
            }
        }
    }
}
