using DC360.Import.Api.Import.Base;
using DC360.Import.Api.Import.Models;
using DC360.Import.Api.Import.Sender;
using System.Collections.Generic;
using System.Linq;

namespace DC360.Import.Api.Import.Validators
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Note sure if we need a common base for the validators or not</remarks>
    public class UserValidatorController : DispatcherBase, IFlowItem<IUserStringModel, UserTypedModel>
    {
        public UserValidatorController(IHubDispatcher dispatcher) : base(dispatcher)
        {
        }

        public List<ImportFlowModel<IUserStringModel, UserTypedModel>> Execute(List<ImportFlowModel<IUserStringModel, UserTypedModel>> input)
        {
            // TODO the controller should delegate the actual validation to "specialized" validators (see schema)
            foreach (var row in input)
            {
                // do validation
                if (row.TypedModel.BirthDate.Month > 3)
                {
                    row.Status = ImportStatus.Error;
                    row.Message += "Person too young"; //TODO translate
                }
            }

            Dispatch(input.Where(i => i.Status != ImportStatus.Processing).ToList());

            return input.Where(i => i.Status == ImportStatus.Processing).ToList();
        }
    }
}
