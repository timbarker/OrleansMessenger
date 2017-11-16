using System;
using System.Threading.Tasks;
using Orleans.Hosting;
using Microsoft.Extensions.Logging;

namespace OrleansMessenger
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var siloHost = new SiloHostBuilder()
                .LoadClusterConfiguration()
                .AddApplicationPart(typeof(Orleans.Providers.MemoryStreamProvider).Assembly)
                .AddApplicationPart(typeof(Orleans.EventSourcing.LogStorage.LogConsistencyProvider).Assembly)
                .AddApplicationPart(typeof(OrleansMessenger.GrainClasses.UserGrain).Assembly)
                .AddApplicationPart(typeof(OrleansMessenger.GrainInterfaces.IUserGrain).Assembly)
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await siloHost.StartAsync();

            Console.ReadLine();

            await siloHost.StopAsync();
        }
    }
}
