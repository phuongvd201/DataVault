using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DataVault.Internal
{
    public interface IDataCommand
    {
        DbCommand Command { get; }

        IDataCommand Sql(string sql);

        TEntity[] Query<TEntity>(Func<IDataReader, Dictionary<string, int>, TEntity> create) where TEntity : class;

        TEntity QuerySingle<TEntity>(Func<IDataReader, Dictionary<string, int>, TEntity> createEntity) where TEntity : class;

        TValue QueryValue<TValue>(Func<object, TValue> convert);

        IDataCommand StoredProcedure(string storedProcedure);

        IDataCommand CommandTimeout(int timeout);

        IDataCommand Parameter(DbParameter parameter);

        IDataCommand RegisterCallback<TParameter>(DbParameter parameter, Action<TParameter> callback);

        int Execute();
    }
}