using DC360.Import.Api.Import;
using DC360.Import.Api.Import.Sender;
using System;

namespace ImportArchitecture
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var dispatcher = new HubDispatcher();
            var x = new ImportProcess(dispatcher);

            x.Start("user");
        }
    }
}
