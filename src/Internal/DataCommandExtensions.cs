using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.ExceptionServices;

using DataVault.Common.Extensions;

namespace DataVault.Internal
{
    public static class DataCommandExtensions
    {
        public static IDataCommand Parameter(this IDataCommand dataCommand, IEnumerable<DbParameter> parameters)
        {
            foreach (var parameter in parameters)
                dataCommand.Parameter(parameter);

            return dataCommand;
        }

        public static IDataCommand Parameter<TParameter>(this IDataCommand dataCommand, string name, TParameter value)
        {
            // convert to object
            object innerValue = value;

            // handle value type by using actual value
            var valueType = value != null ? value.GetType() : typeof(TParameter);

            var parameter = dataCommand.Command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = innerValue ?? DBNull.Value;
            parameter.DbType = valueType.GetUnderlyingType().ToDbType();
            parameter.Direction = ParameterDirection.Input;

            return dataCommand.Parameter(parameter);
        }

        public static IDataCommand Parameter(this IDataCommand dataCommand, string name, object value)
        {
            // convert to object
            object innerValue = value;

            // handle value type by using actual value
            //var valueType = value != null ? value.GetType() : typeof(TParameter);

            var parameter = dataCommand.Command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = innerValue ?? DBNull.Value;
            parameter.DbType = value != null ? value.GetType().GetUnderlyingType().ToDbType() : DbType.Object;
            parameter.Direction = ParameterDirection.Input;

            return dataCommand.Parameter(parameter);
        }

        public static IDataCommand ParameterOut<TParameter>(this IDataCommand dataCommand, string name, Action<TParameter> callback)
        {
            var parameter = dataCommand.Command.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = typeof(TParameter).GetUnderlyingType().ToDbType();
            parameter.Direction = ParameterDirection.Output;

            // output parameters must have a size, default to MAX
            parameter.Size = -1;

            dataCommand.RegisterCallback(parameter, callback);
            dataCommand.Parameter(parameter);

            return dataCommand;
        }

        public static IDataCommand ParameterOut<TParameter>(this IDataCommand dataCommand, string name, TParameter value, Action<TParameter> callback)
        {
            object innerValue = value;

            var parameter = dataCommand.Command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = innerValue ?? DBNull.Value;
            parameter.DbType = typeof(TParameter).GetUnderlyingType().ToDbType();
            parameter.Direction = ParameterDirection.InputOutput;

            dataCommand.RegisterCallback(parameter, callback);
            dataCommand.Parameter(parameter);

            return dataCommand;
        }

        public static IDataCommand Return<TParameter>(this IDataCommand dataCommand, Action<TParameter> callback)
        {
            const string parameterName = "@ReturnValue";

            var parameter = dataCommand.Command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = typeof(TParameter).GetUnderlyingType().ToDbType();
            parameter.Direction = ParameterDirection.ReturnValue;

            dataCommand.RegisterCallback(parameter, callback);
            dataCommand.Parameter(parameter);

            return dataCommand;
        }

        public static TValue QueryValue<TValue>(this IDataCommand dataQuery)
        {
            return dataQuery.QueryValue<TValue>(null);
        }

        public static DataException CreateDataException(this IDataCommand dataCommand, Exception inner)
        {
            var cmd = dataCommand.Command;

            // Failed to generate a command. Throw original exception.
            if (string.IsNullOrEmpty(cmd.CommandText))
            {
                ExceptionDispatchInfo.Capture(inner).Throw();
            }

            var exceptionToAnalyze = inner;
            if (exceptionToAnalyze is AggregateException)
                exceptionToAnalyze = inner.InnerException;

            var pos = exceptionToAnalyze.Message.IndexOfAny(new[] { '\r', '\n' });
            var innerMsg = pos == -1 ? exceptionToAnalyze.Message : exceptionToAnalyze.Message.Substring(0, pos);

            var str = innerMsg + dataCommand.GetCommandString();

            return new DataException(str, inner);
        }

        public static string GetCommandString(this IDataCommand dataCommand)
        {
            var cmd = dataCommand.Command;

            var str = Environment.NewLine + "Query: " + $"[{cmd.Connection.Database}].[{cmd.CommandText}]";

            foreach (IDbDataParameter parameter in cmd.Parameters)
            {
                str += Environment.NewLine + parameter.ParameterName + ": " + (parameter.Value ?? "NULL");
            }

            return str;
        }
    }
}