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
     * 
     */

    /// <summary>
    /// Responsible for executing the import flow
    /// </summary>
    public class ImportProcess
    {
        public void Start(string type, string path = "UserTest.xlsx")
        {
            if (type == "user")
            {
                StartImport<UserStringModel, UserTypedModel>(path);
            }

            if (type == "not user but a contract")
            {
                StartImport<UserTypedModel, UserTypedModel>(path);
            }
        }

        private void StartImport<T, TU>(string path)
            where T : class
            where TU : class
        {

            var flow = FlowCreator.CreateFlow<T, TU>(path, typeof(T));

            // execute flow
            var partialResult = new List<ImportFlowModel<T, TU>>();
            foreach (var item in flow)
            {
                partialResult = item.Execute(partialResult);
            }
        }
    }



    /*
     * Flow
     * 
     * UserStringModel
     * UserTypeModel
     * Error
     * 
     * Read - path - UserStringModel
     * Map - UserStringModel
     * 
     * 
     */

    public class FlowCreator
    {
        public static List<IFlowItem<T, TU>> CreateFlow<T, TU>(string path, Type type)
            where T : class
            where TU : class
        {
            if (typeof(UserStringModel) == type)
            {
                return new List<IFlowItem<T, TU>>
                {
                    new UserReader(path) as IFlowItem<T, TU>,
                    new UserTest() as IFlowItem<T, TU>,
                };
            }

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

    public abstract class Reader<T, TU> : DispatcherBase, IFlowItem<T, TU>
         where T : class
         where TU : class
    {
        internal string Path { get; init; }

        public Reader(string path, IHubDispatcher dispatcher) : base(dispatcher)
        {
            this.Path = path;
        }

        public abstract List<ImportFlowModel<T, TU>> Execute(List<ImportFlowModel<T, TU>> input);
    }

    public class UserReader : Reader<UserStringModel, UserTypedModel>
    {
        public UserReader(string path, IHubDispatcher dispatcher) : base(path, dispatcher)
        {
        }

        public override List<ImportFlowModel<UserStringModel, UserTypedModel>> Execute(List<ImportFlowModel<UserStringModel, UserTypedModel>> input)
        {
            var mapper = new Mapper(this.Path);
            var rowInfos = mapper.Take<UserStringModel>("sheet1").ToList();

            // we could also create a new list and not use the input
            input.Clear();

            input.AddRange(rowInfos.Select(rowInfo => new ImportFlowModel<UserStringModel, UserTypedModel>
            {
                StringModel = rowInfo.Value,
                Status = ImportStatus.Processing
            }));

            this.Dispatch(input);

            return input;
        }
    }

    public abstract class Mapper<T, TU> : DispatcherBase, IFlowItem<T, TU>
         where T : class
         where TU : class
    {
        public Mapper(IHubDispatcher dispatcher) : base(dispatcher)
        {
        }

        public abstract List<ImportFlowModel<T, TU>> Execute(List<ImportFlowModel<T, TU>> input);
    }

    public class UserReader : Reader<UserStringModel, UserTypedModel>
    {
        public UserReader(string path, IHubDispatcher dispatcher) : base(path, dispatcher)
        {
        }

        public override List<ImportFlowModel<UserStringModel, UserTypedModel>> Execute(List<ImportFlowModel<UserStringModel, UserTypedModel>> input)
        {
            var mapper = new Mapper(this.Path);
            var rowInfos = mapper.Take<UserStringModel>("sheet1").ToList();

            // we could also create a new list and not use the input
            input.Clear();

            input.AddRange(rowInfos.Select(rowInfo => new ImportFlowModel<UserStringModel, UserTypedModel>
            {
                StringModel = rowInfo.Value,
                Status = ImportStatus.Processing
            }));

            this.Dispatch(input);

            return input;
        }
    }





}
