using Orleans.Runtime.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
