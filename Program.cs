using DC360.Import.Api.Import;
using System;

namespace ImportArchitecture
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var x = new ImportProcess();

            x.Start("user");
        }
    }
}
