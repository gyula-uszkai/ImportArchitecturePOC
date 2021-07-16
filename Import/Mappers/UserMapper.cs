using DC360.Import.Api.Import.Base;
using DC360.Import.Api.Import.Models;
using DC360.Import.Api.Import.Sender;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DC360.Import.Api.Import.Mappers
{
    public class UserMapper : MapperBase<IUserStringModel, UserTypedModel>
    {
        public UserMapper(IHubDispatcher dispatcher) : base(dispatcher)
        {
        }

        public override List<ImportFlowModel<IUserStringModel, UserTypedModel>> Execute(List<ImportFlowModel<IUserStringModel, UserTypedModel>> input)
        {
            var processedItems = new List<ImportFlowModel<IUserStringModel, UserTypedModel>>();
            foreach (var row in input)
            {
                var record = new ImportFlowModel<IUserStringModel, UserTypedModel>
                {
                    StringModel = row.StringModel,
                    TypedModel = new UserTypedModel
                    {
                        FirstName = row.StringModel.FirstName
                    },
                    Status = ImportStatus.Processing
                };

                // do conversion
                if (DateTime.TryParse(row.StringModel.BirthDate, out var bDate))
                {
                    record.TypedModel.BirthDate = bDate;
                }
                else
                {
                    record.Status = ImportStatus.Error;
                    record.Message += "Some problems with the b-date"; //TODO translate
                }
                processedItems.Add(record); // consider using 2 list instead
            }

            Dispatch(processedItems.Where(i => i.Status != ImportStatus.Processing).ToList());

            return processedItems.Where(i => i.Status == ImportStatus.Processing).ToList();
        }
    }
}
