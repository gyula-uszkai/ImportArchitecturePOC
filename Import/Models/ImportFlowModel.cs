using DC360.Import.Api.Import.Base;

namespace DC360.Import.Api.Import.Models
{
    public class ImportFlowModel<T, TU>
        where T : class
        where TU : class
    {
        public T StringModel { get; set; }

        public TU TypedModel { get; set; }

        public ImportStatus Status { get; set; }

        public string Message { get; set; }
    }
}
