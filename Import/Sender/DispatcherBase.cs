using DC360.Import.Api.Import.Models;
using System.Collections.Generic;

namespace DC360.Import.Api.Import.Sender
{
    public class DispatcherBase
    {
        internal IHubDispatcher Dispatcher { get; init; }

        public DispatcherBase(IHubDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public void Dispatch<T, TU>(List<ImportFlowModel<T, TU>> input)
            where T : class
            where TU : class
        {
            Dispatcher.Send(input);
        }
    }
}
