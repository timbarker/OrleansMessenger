using Orleans.Runtime.Host;

namespace OrleansMessenger
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WindowsServerHost();
            host.ParseArguments(args);

            host.Init();
            host.Run();   
        }
    }
}
