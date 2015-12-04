using Orleans;
using Orleans.Providers;
using Orleans.Streams;
using OrleansMessenger.GrainInterfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Orleans.Concurrency;

namespace OrleansMessenger.GrainClasses
{
    [StorageProvider(ProviderName = "UserGrainStore")]
    [Reentrant]
    public class UserGrain : Grain<UserState>, IUserGrain
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
            var command = string.Format("m:{0}:{1}", from, message);
            State.History.Add(command);
            await WriteStateAsync();
            await _clientMessageStream.OnNextAsync(command);
        }

        public async Task SendMessage(string message, string to)
        {
            State.History.Add(string.Format("s:{0}:{1}", to, message));
            await WriteStateAsync();
            await GrainFactory.GetGrain<IUserGrain>(to)
                .ReceiveMessage(message, this.GetPrimaryKeyString());
        }

        public Task<string[]> GetHistoricalMessages(int count = int.MaxValue)
        {
            return Task.FromResult(State.History.Skip(Math.Max(0, State.History.Count - count)).ToArray());
        }
    }
}
