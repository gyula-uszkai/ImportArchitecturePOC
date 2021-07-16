using DC360.Import.Api.Import.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DC360.Import.Api.Import.Sender
{
    public class HubDispatcher : IHubDispatcher
    {
        public void Send<T, TU>(List<ImportFlowModel<T, TU>> input)
            where T : class
            where TU : class
        {
            //Do some sending
            if (!input.Any())
            {
                Console.WriteLine("Empty list, nothing to do...");
            }
            else
            {
                foreach (var item in input)
                {
                    Console.WriteLine($"Item: { JsonSerializer.Serialize(item.StringModel)} - Status:{item.Status} - Message: {item.Message}");
                }
            }

        }
    }
}
