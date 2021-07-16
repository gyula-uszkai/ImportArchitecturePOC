using DC360.Import.Api.Import.Base;
using DC360.Import.Api.Import.Models;
using DC360.Import.Api.Import.Sender;
using System.Collections.Generic;

namespace DC360.Import.Api.Import.Mappers
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Not sure if this abstract mapper base is needed</remarks>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TU"></typeparam>
    public abstract class MapperBase<T, TU> : DispatcherBase, IFlowItem<T, TU>
         where T : class
         where TU : class
    {
        public MapperBase(IHubDispatcher dispatcher) : base(dispatcher)
        {
        }

        public abstract List<ImportFlowModel<T, TU>> Execute(List<ImportFlowModel<T, TU>> input);
    }
}
