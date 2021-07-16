using DC360.Import.Api.Import.Base;
using DC360.Import.Api.Import.Models;
using DC360.Import.Api.Import.Sender;
using System;
using System.Collections.Generic;

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
                //StartImport<ContractStringModel, ContractTypedModel>(type, path);
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
}
