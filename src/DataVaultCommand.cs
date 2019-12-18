using System.Data;

using DataVault.Common.Extensions;
using DataVault.Common.Json;
using DataVault.Entities.Audit;
using DataVault.Internal;

using Newtonsoft.Json.Linq;

namespace DataVault
{
    internal class DataVaultCommand
    {
        internal IDataCommand DataCommand { get; set; }

        internal DataVaultResult DataVaultResult { get; set; } = new DataVaultResult();

        internal DataVaultCommand(IDataCommand dataCommand)
        {
            DataCommand = dataCommand;
        }

        internal static DataVaultCommand ReadOnlyStoredProcedure(IDataSession dataSession, string stpName)
        {
            var dataVaultCommandBuilder = new DataVaultCommand(dataSession.StoredProcedure(stpName));

            dataVaultCommandBuilder.DataCommand
                .ParameterOut<string>(DataVaultParams.StatusCode, p => dataVaultCommandBuilder.DataVaultResult.StatusCode = p.ToEnumOrDefault(DataVaultBusinessErrorCode.Failed))
                .ParameterOut<string>(DataVaultParams.Result, p => dataVaultCommandBuilder.DataVaultResult.JsonResult = p);

            return dataVaultCommandBuilder;
        }

        internal static DataVaultCommand UpdateStoredProcedure(IDataSession dataSession, IAuditContext auditContext, string stpName)
        {
            dataSession.BeginTransaction(IsolationLevel.Unspecified);
            return ReadOnlyStoredProcedure(dataSession, stpName).WithMd(auditContext.AuditInfo);
        }

        internal DataVaultCommand WithHubData(object data)
        {
            DataCommand.Parameter(DataVaultParams.HubData, data.ToJsonString());

            return this;
        }

        internal DataVaultCommand WithSatData(object data)
        {
            DataCommand.Parameter(DataVaultParams.SatData, data.ToJsonString());

            return this;
        }

        internal DataVaultCommand WithSatQuery(object data)
        {
            DataCommand.Parameter(DataVaultParams.SatQuery, data.ToJsonString());

            return this;
        }

        internal DataVaultCommand WithForeignKeys(object data)
        {
            DataCommand.Parameter(DataVaultParams.Fk, data.ToJsonString());

            return this;
        }

        internal DataVaultCommand WithHubPk(string hubPk)
        {
            DataCommand.Parameter(DataVaultParams.HubPk, hubPk);

            return this;
        }

        internal DataVaultCommand WithLinkPk(string linkPk)
        {
            DataCommand.Parameter(DataVaultParams.LinkPk, linkPk);

            return this;
        }

        internal DataVaultCommand WithMd(AuditInfo auditJson)
        {
            DataCommand.Parameter(DataVaultParams.Md, auditJson.ToJsonString());

            return this;
        }

        internal DataVaultCommand WithLookupTable(string lkpTable)
        {
            DataCommand.Parameter(DataVaultParams.LkpTable, lkpTable);

            return this;
        }

        internal DataVaultCommand WithLookupCode(string lkpCode)
        {
            DataCommand.Parameter(DataVaultParams.LkpValue, lkpCode);

            return this;
        }

        internal DataVaultCommand WithParameter(string parameterName, string value)
        {
            DataCommand.Parameter(parameterName, value);

            return this;
        }

        internal DataVaultResult Execute()
        {
            DataCommand.Execute();

            return DataVaultResult;
        }

        internal string ExecuteWithErrorHandling()
        {
            var result = Execute();

            if (result.StatusCode != DataVaultBusinessErrorCode.Success)
            {
                string message = ParseMessage(result.JsonResult);

                throw new DataVaultBusinessException(
                    result.StatusCode,
                    message,
                    DataCommand.GetCommandString());
            }

            return result.JsonResult;
        }

        internal TEntity ExecuteWithErrorHandling<TEntity>()
        {
            var jsonResult = ExecuteWithErrorHandling();

            return jsonResult.HasJsonValue() ? jsonResult.ToNestedJson().FromJsonString<TEntity>() : default;
        }

        private static string ParseMessage(string jsonResult)
        {
            if (!jsonResult.ValidJson())
            {
                return jsonResult;
            }

            string message = jsonResult;

            if (JObject.Parse(jsonResult).TryGetValue("message", out var messageJson))
            {
                message = messageJson.Value<string>();
            }

            return message;
        }
    }
}