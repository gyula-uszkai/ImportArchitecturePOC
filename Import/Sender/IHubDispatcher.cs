using DC360.Import.Api.Import.Models;
using System.Collections.Generic;

namespace DC360.Import.Api.Import.Sender
{
    public interface IHubDispatcher
    {
        void Send<T, TU>(List<ImportFlowModel<T, TU>> input)
            where T : class
            where TU : class;
    }
}