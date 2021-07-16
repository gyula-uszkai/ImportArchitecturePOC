using DC360.Import.Api.Import.Base;
using DC360.Import.Api.Import.Models;
using DC360.Import.Api.Import.Sender;
using Npoi.Mapper;
using System.Collections.Generic;
using System.Linq;

namespace DC360.Import.Api.Import.Readers
{
    public class Reader<T, TU> : DispatcherBase, IFlowItem<T, TU>
         where T : class
         where TU : class
    {
        internal string Path { get; init; }

        public Reader(IHubDispatcher dispatcher, string path) : base(dispatcher)
        {
            Path = path;
        }

        public virtual List<ImportFlowModel<T, TU>> Execute(List<ImportFlowModel<T, TU>> input)
        {
            var mapper = new Mapper(Path);
            var rowInfos = GetRowInfos(mapper);

            // we could also create a new list and not use the input
            input.Clear();

            input.AddRange(rowInfos.Select(rowInfo => new ImportFlowModel<T, TU>
            {
                StringModel = rowInfo.Value,
                Status = ImportStatus.Processing
            }));

            Dispatch(input);

            return input;
        }

        protected virtual IEnumerable<RowInfo<T>> GetRowInfos(Mapper mapper)
        {
            //NOTE that there can be errors here that we are not handling
            return mapper.Take<T>("sheet1");
        }

    }
}
