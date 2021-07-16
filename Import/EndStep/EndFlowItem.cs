using DC360.Import.Api.Import.Base;
using DC360.Import.Api.Import.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DC360.Import.Api.Import.EndStep
{
    public class EndFlowItem<T, TU> : IFlowItem<T, TU>
         where T : class
        where TU : class
    {
        public List<ImportFlowModel<T, TU>> Execute(List<ImportFlowModel<T, TU>> input)
        {
            Console.WriteLine("You reached the end step");

            foreach (var item in input)
            {
                Console.WriteLine($"Item: { JsonSerializer.Serialize(item.StringModel)} - Status:{item.Status} - Message: {item.Message}");
            }
            return input;
        }
    }
}
