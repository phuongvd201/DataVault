using System;
using System.Collections.Generic;

using DataVault.Common.Json;
using DataVault.Internal;

namespace DataVault.Query
{
    public abstract class DataCommandBuilder
    {
        public string Table { get; private set; }

        protected List<string> Columns { get; } = new List<string>();

        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        private WhereBuilder _whereBuilder;

        protected string WhereString => _whereBuilder == null ? string.Empty : " WHERE " + _whereBuilder.Build();

        protected IDataContext Context { get; }

        protected DataCommandBuilder(IDataContext context)
        {
            Context = context;
        }

        public DataCommandBuilder From(string table)
        {
            Table = table;

            return this;
        }

        public DataCommandBuilder Column(string name, object value)
        {
            Columns.Add(name);

            Parameters.AddIfNotContains(name, value);

            return this;
        }

        public DataCommandBuilder Where(Func<WhereBuilder> factory)
        {
            _whereBuilder = factory();

            return this;
        }

        public DataCommandBuilder Where(WhereBuilder builder)
        {
            _whereBuilder = builder;

            return this;
        }

        public DataCommandBuilder WhereId(int id)
        {
            _whereBuilder = WhereBuilder.New("id", QueryOperator.Equal, id);

            return this;
        }

        protected IDataCommand AddWhereParams(IDataCommand command)
        {
            string prefix = "@where";

            foreach (var condition in _whereBuilder.QueryConditions.Keys)
            {
                command.Parameter(prefix + condition.Field.FieldName, condition.Value);
            }

            return command;
        }

        public abstract IDataCommand BuildCommand();
    }
}