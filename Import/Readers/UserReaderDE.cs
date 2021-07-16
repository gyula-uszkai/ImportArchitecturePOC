using DC360.Import.Api.Import.Models;
using DC360.Import.Api.Import.Sender;
using Npoi.Mapper;
using System.Collections.Generic;
using System.Linq;

namespace DC360.Import.Api.Import.Readers
{
    public class UserReaderDE : Reader<IUserStringModel, UserTypedModel>
    {
        public UserReaderDE(IHubDispatcher dispatcher, string path) : base(dispatcher, path)
        {
        }

        protected override IEnumerable<RowInfo<IUserStringModel>> GetRowInfos(Mapper mapper)
        {
            var rowInfos = mapper.Take<UserDeStringModel>("sheet1");

            //NOTE that there can be errors here that we are not handling
            return rowInfos.Select(i => new RowInfo<IUserStringModel>(i.RowNumber, i.Value, i.ErrorColumnIndex, i.ErrorMessage));
        }
    }
}
