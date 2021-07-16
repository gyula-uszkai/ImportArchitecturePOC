using DC360.Import.Api.Import.Mappers;
using DC360.Import.Api.Import.Models;
using DC360.Import.Api.Import.Processors;
using DC360.Import.Api.Import.Readers;
using DC360.Import.Api.Import.Sender;
using DC360.Import.Api.Import.Validators;
using System.Collections.Generic;

namespace DC360.Import.Api.Import.Base
{
    public class FlowCreator
    {
        public static List<IFlowItem<T, TU>> CreateFlow<T, TU>(IHubDispatcher dispatcher, string type, string path)
            where T : class
            where TU : class
        {
            if ("user" == type)
            {
                return new List<IFlowItem<T, TU>>
                {
                    new UserReaderEN(dispatcher, path) as IFlowItem<T, TU>,
                    new UserMapper(dispatcher) as IFlowItem<T, TU>,
                    new UserValidatorController(dispatcher) as IFlowItem<T, TU>,
                    new ProcessorController<IUserStringModel, UserTypedModel>(dispatcher) as IFlowItem<T, TU>,
                    new EndStep.EndFlowItem<IUserStringModel, UserTypedModel>() as IFlowItem<T, TU>
                };
            }

            if ("userde" == type)
            {
                return new List<IFlowItem<T, TU>>
                {
                    new UserReaderDE(dispatcher, path) as IFlowItem<T, TU>, //only this line is different so we could merge the to (but not now)
                    new UserMapper(dispatcher) as IFlowItem<T, TU>,
                    new UserValidatorController(dispatcher) as IFlowItem<T, TU>,
                    new ProcessorController<IUserStringModel, UserTypedModel>(dispatcher) as IFlowItem<T, TU>,
                    new EndStep.EndFlowItem<IUserStringModel, UserTypedModel>() as IFlowItem<T, TU>
                };
            }

            //TODO add a new flow for contracts

            return null;
        }
    }
}
