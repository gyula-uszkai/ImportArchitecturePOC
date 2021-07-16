using DC360.Import.Api.Import.Models;
using System.Collections.Generic;

namespace DC360.Import.Api.Import.Base
{
    public interface IFlowItem<T, TU>
        where T : class
        where TU : class
    {
        public List<ImportFlowModel<T, TU>> Execute(List<ImportFlowModel<T, TU>> input);
    }
}
