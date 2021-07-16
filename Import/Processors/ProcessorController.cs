using DC360.Import.Api.Import.Base;
using DC360.Import.Api.Import.Models;
using DC360.Import.Api.Import.Sender;
using System.Collections.Generic;

namespace DC360.Import.Api.Import.Processors
{
    public class ProcessorController<T, TU> : DispatcherBase, IFlowItem<T, TU>
         where T : class
         where TU : class
    {

        public ProcessorController(IHubDispatcher dispatcher) : base(dispatcher)
        {
        }

        public virtual List<ImportFlowModel<T, TU>> Execute(List<ImportFlowModel<T, TU>> input)
        {
            //TODO I need a bit more time to implement this

            Dispatch(input);

            return input;
        }
    }
}
