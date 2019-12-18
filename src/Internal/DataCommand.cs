using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DataVault.Common.Extensions;

namespace DataVault.Internal
{
    public class DataCommand : DisposableBase, IDataCommand
    {
        private readonly Queue<DataCallback> _callbacks;
        private readonly IDataSession _dataSession;
        private readonly DbCommand _command;

        public DataCommand(IDataSession dataSession, DbTransaction transaction)
        {
            _callbacks = new Queue<DataCallback>();
            _dataSession = dataSession ?? throw new ArgumentNullException(nameof(dataSession));
            _command = dataSession.Connection.CreateCommand();
            _command.Transaction = transaction;
        }

        public DbCommand Command => _command;

        public IDataCommand Sql(string sql)
        {
            _command.CommandText = sql;
            _command.CommandType = CommandType.Text;
            return this;
        }

        public IDataCommand StoredProcedure(string storedProcedure)
        {
            _command.CommandText = storedProcedure;
            _command.CommandType = CommandType.StoredProcedure;
            return this;
        }

        public IDataCommand CommandTimeout(int timeout)
        {
            _command.CommandTimeout = timeout;
            return this;
        }

        public IDataCommand Parameter(DbParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            _command.Parameters.Add(parameter);
            return this;
        }

        public IDataCommand RegisterCallback<TParameter>(DbParameter parameter, Action<TParameter> callback)
        {
            var dataCallback = new DataCallback
            {
                Callback = callback,
                Type = typeof(TParameter),
                Parameter = parameter
            };
            _callbacks.Enqueue(dataCallback);

            return this;
        }

        public int Execute()
        {
            return ExecuteCommand(command => command.ExecuteNonQuery());
        }

        public TValue QueryValue<TValue>(Func<object, TValue> convert)
        {
            return ExecuteCommand(
                command =>
                {
                    var result = _command.ExecuteScalar();
                    return result.ConvertValue(convert);
                });
        }

        public TEntity[] Query<TEntity>(Func<IDataReader, Dictionary<string, int>, TEntity> createEntity)
            where TEntity : class
        {
            if (createEntity == null)
                throw new ArgumentNullException(nameof(createEntity));

            return ExecuteCommand(
                command =>
                {
                    var result = Enumerable.Empty<TEntity>();
                    using (var reader = _command.ExecuteReader())
                    {
                        var columns = Enumerable.Range(0, reader.FieldCount).ToDictionary((index) => reader.GetName(index), StringComparer.InvariantCultureIgnoreCase);
                        while (reader.Read())
                        {
                            var entity = createEntity(reader, columns);
                            result = result.Append(entity);
                        }

                        return result.ToArray();
                    }
                });
        }

        public TEntity QuerySingle<TEntity>(Func<IDataReader, Dictionary<string, int>, TEntity> createEntity)
            where TEntity : class
        {
            if (createEntity == null)
                throw new ArgumentNullException(nameof(createEntity));

            return ExecuteCommand(
                command =>
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var columns = Enumerable.Range(0, reader.FieldCount).ToDictionary((index) => reader.GetName(index), StringComparer.InvariantCultureIgnoreCase);
                        return reader.Read()
                            ? createEntity(reader, columns)
                            : default;
                    }
                });
        }

        public T ExecuteCommand<T>(Func<IDbCommand, T> action)
        {
            AssertDisposed();

            var sw = Stopwatch.StartNew();

            try
            {
                _dataSession.EnsureConnection();

                var result = action(_command);

                LogCommand(sw.ElapsedMilliseconds);

                TriggerCallbacks();

                return result;
            }
            catch (Exception exception)
            {
                throw this.CreateDataException(exception);
            }
            finally
            {
                _dataSession.ReleaseConnection();
                Dispose();
            }
        }

        protected override void DisposeManagedResources()
        {
            _command?.Dispose();
        }

        internal void TriggerCallbacks()
        {
            if (_callbacks.Count == 0)
                return;

            while (_callbacks.Count > 0)
            {
                var dataCallback = _callbacks.Dequeue();
                dataCallback.Invoke();
            }
        }

        private void LogCommand(long executeTimeInMs)
        {
            LogCommand(_dataSession.WriteLog, _command, executeTimeInMs);
        }

        private static void LogCommand(Action<string> writer, IDbCommand command, long executeTimeInMs)
        {
            if (writer == null)
                return;

            var buffer = new StringBuilder($"Execute query: [{command.Connection.Database}].[{command.CommandText}] took {executeTimeInMs} ms. ");
            buffer.AppendLine();

            foreach (IDataParameter parameter in command.Parameters)
            {
                int precision = 0;
                int scale = 0;
                int size = 0;

                if (parameter is IDbDataParameter dataParameter)
                {
                    precision = dataParameter.Precision;
                    scale = dataParameter.Scale;
                    size = dataParameter.Size;
                }

                buffer.AppendFormat(

                    //"-- {0}: {1} {2} (Size = {3}; Precision = {4}; Scale = {5}) [{6}]",
                    "'{0}': {1}",
                    parameter.ParameterName,

                    //parameter.DbType,
                    //size,
                    //precision,
                    //scale,
                    parameter.Value);

                buffer.AppendLine();
            }

            writer(buffer.ToString());
        }
    }
}