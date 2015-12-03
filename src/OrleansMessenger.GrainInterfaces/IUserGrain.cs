using System.Threading.Tasks;
using Orleans;

namespace OrleansMessenger.GrainInterfaces
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task SendMessage(string message, string to);
        Task ReceiveMessage(string message, string from);
    }
}
