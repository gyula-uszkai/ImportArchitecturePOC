using DC360.Import.Api.Import.Base;
using DC360.Import.Api.Import.Models;
using DC360.Import.Api.Import.Sender;
using Npoi.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DC360.Import.Api.Import
{
    /*
     * NOTE
     * 
     * sheet name is important for the reader
     * we also need to translate the error messages
     * 
     * if we want to update the excel at the end, then we should send all records to the end step (not just the one without errors)
     * 
     * consider how we would identify the records to update them? consider adding a "nr-crt" or "id" column to identify the records
     * (this could also be the database id from the export, but if needed to be added manually then it would break the premise that exports can be directly imported)
     * 
     * add global exceptions
     */

    /// <summary>
    /// Responsible for executing the import flow
    /// </summary>
    public class ImportProcess
    {
        private readonly IHubDispatcher dispatcher;

        public ImportProcess(IHubDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public void Start(string type, string path)
        {
            Console.WriteLine("-------new run -----------");
            if (type == "user")
            {
                StartImport<IUserStringModel, UserTypedModel>(type, path);
            }

            if (type == "userde")
            {
                StartImport<IUserStringModel, UserTypedModel>(type, path);
            }

            if (type == "not user but a contract")
            {
                StartImport<UserTypedModel, UserTypedModel>(type, path);
            }
        }

        private void StartImport<T, TU>(string type, string path)
            where T : class
            where TU : class
        {

            var flow = FlowCreator.CreateFlow<T, TU>(this.dispatcher, type, path);

            // execute flow
            var partialResult = new List<ImportFlowModel<T, TU>>();
            foreach (var item in flow)
            {
                partialResult = item.Execute(partialResult);
            }
        }
    }

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


    public class DispatcherBase
    {
        internal IHubDispatcher Dispatcher { get; init; }

        public DispatcherBase(IHubDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
        }

        public void Dispatch<T, TU>(List<ImportFlowModel<T, TU>> input)
            where T : class
            where TU : class
        {
            this.Dispatcher.Send(input);
        }
    }

    public class Reader<T, TU> : DispatcherBase, IFlowItem<T, TU>
         where T : class
         where TU : class
    {
        internal string Path { get; init; }

        public Reader(IHubDispatcher dispatcher, string path) : base(dispatcher)
        {
            this.Path = path;
        }

        public virtual List<ImportFlowModel<T, TU>> Execute(List<ImportFlowModel<T, TU>> input)
        {
            var mapper = new Mapper(this.Path);
            var rowInfos = GetRowInfos(mapper);

            // we could also create a new list and not use the input
            input.Clear();

            input.AddRange(rowInfos.Select(rowInfo => new ImportFlowModel<T, TU>
            {
                StringModel = rowInfo.Value,
                Status = ImportStatus.Processing
            }));

            this.Dispatch(input);

            return input;
        }

        protected virtual IEnumerable<RowInfo<T>> GetRowInfos(Mapper mapper)
        {
            //NOTE that there can be errors here that we are not handling
            return mapper.Take<T>("sheet1");
        }

    }

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

    public class UserReaderEN : Reader<IUserStringModel, UserTypedModel>
    {
        public UserReaderEN(IHubDispatcher dispatcher, string path) : base(dispatcher, path)
        {
        }

        protected override IEnumerable<RowInfo<IUserStringModel>> GetRowInfos(Mapper mapper)
        {
            var rowInfos = mapper.Take<UserStringModel>("sheet1");

            //NOTE that there can be errors here that we are not handling
            return rowInfos.Select(i => new RowInfo<IUserStringModel>(i.RowNumber, i.Value, i.ErrorColumnIndex, i.ErrorMessage));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Not sure if this abstract mapper base is needed</remarks>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TU"></typeparam>
    public abstract class Mapper<T, TU> : DispatcherBase, IFlowItem<T, TU>
         where T : class
         where TU : class
    {
        public Mapper(IHubDispatcher dispatcher) : base(dispatcher)
        {
        }

        public abstract List<ImportFlowModel<T, TU>> Execute(List<ImportFlowModel<T, TU>> input);
    }

    public class UserMapper : Mapper<IUserStringModel, UserTypedModel>
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

            this.Dispatch(processedItems.Where(i => i.Status != ImportStatus.Processing).ToList());

            return processedItems.Where(i => i.Status == ImportStatus.Processing).ToList();
        }
    }


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

            this.Dispatch(input.Where(i => i.Status != ImportStatus.Processing).ToList());

            return input.Where(i => i.Status == ImportStatus.Processing).ToList();
        }
    }


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

            this.Dispatch(input);

            return input;
        }
    }
}
