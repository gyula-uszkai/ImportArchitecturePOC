using System;
using System.Collections.Generic;

namespace DC360.Import.Api.Import
{
    public class ImportProcess
    {
        // responsibility execute flow
        public void Start(string type)
        {
            if (type == "user")
            {
                StartImport<UserStringModel>();
            }

            if (type == "not user but a contract")
            {
                StartImport<UserTypedModel>();
            }
        }

        private void StartImport<T>() where T : class
        {
            //get flow
            var flow = new FlowCreator().CreateFlow<T>(typeof(T));

            //execute flow
            var lastResult = new List<Trio<T>>();
            foreach (var item in flow)
            {
                lastResult = item.Execute(lastResult);
            }
        }
    }

    public class Trio<T>
    {
        public T Entity { get; set; }
        public string Whatever { get; set; }
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
        public List<IFlowItem<T>> CreateFlow<T>(Type type) where T : class
        {
            if (1 < 2)//This is for user
            {
                return new List<IFlowItem<T>>
                {
                    //new UserReader(),
                    //new UserMapper()
                    new Test<T>(),
                    new UserTest() as IFlowItem<T>,
                };
            }

            return null;
        }
    }

    public interface IFlowItem<T>
    {
        public List<Trio<T>> Execute(List<Trio<T>> input);
    }

    public interface IReader<T>
        : IFlowItem<T>
        where T : new()

    {
    }

    public abstract class Reader<T> : IReader<T> where T : new()
    {
        public Reader(string path)
        {

        }

        public abstract List<Trio<T>> Execute(List<Trio<T>> input);

        public void SendToSender(List<Trio<T>> input)
        {

        }
    }

    public class UserReader : Reader<UserStringModel>
    {
        public UserReader(string path) : base(path)
        {

        }

        public override List<Trio<UserStringModel>> Execute(List<Trio<UserStringModel>> input)
        {
            var a = new UserStringModel();
            a.MyProperty = "ceva";
            return null;
        }
    }


    public class Test<T> : IFlowItem<T> where T : class
    {
        public virtual List<Trio<T>> Execute(List<Trio<T>> input)
        {
            return new List<Trio<T>> { new Trio<UserStringModel>() as Trio<T> };
        }
    }

    public class UserTest : Test<UserStringModel>
    {
        public override List<Trio<UserStringModel>> Execute(List<Trio<UserStringModel>> input)
        {
            return new List<Trio<UserStringModel>> { new Trio<UserStringModel>() };
        }
    }

    public abstract class Mapper<T> : IFlowItem<T> where T : new()
    {

        public abstract List<Trio<T>> Execute(List<Trio<T>> input);

        public void SendToSender(List<Trio<T>> input)
        {

        }
    }

    public class UserMapper : Mapper<UserStringModel>
    {
        public override List<Trio<UserStringModel>> Execute(List<Trio<UserStringModel>> input)
        {
            var a = new UserStringModel();
            a.MyProperty = "ceva";
            return null;
        }
    }


    public class UserStringModel
    {
        public string MyProperty { get; set; }
    }
    public class UserTypedModel { }


}
