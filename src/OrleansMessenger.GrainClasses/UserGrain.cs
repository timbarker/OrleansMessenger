using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using OrleansMessenger.GrainInterfaces;

namespace OrleansMessenger.GrainClasses
{
    public class UserGrain : Grain, IUserGrain
    {
        private IAsyncStream<string> _clientMessageStream;

        public override Task OnActivateAsync()
        {
            _clientMessageStream = GetStreamProvider("SMSProvider")
                .GetStream<string>(Guid.Parse("FED26B31-9D86-4F30-8128-01BA23880066"), this.GetPrimaryKeyString());
            return base.OnActivateAsync();
        }

        public async Task ReceiveMessage(string message, string from)
        {
            Console.WriteLine("Reveive message");
            await _clientMessageStream.OnNextAsync(string.Format("m:{0}:{1}", from, message));
        }

        public async Task SendMessage(string message, string to)
        {
            Console.WriteLine("Send Message");
            await GrainFactory.GetGrain<IUserGrain>(to).ReceiveMessage(message, this.GetPrimaryKeyString());            
        }
    }
}
