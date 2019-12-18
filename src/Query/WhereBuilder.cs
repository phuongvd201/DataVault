using System.Collections.Generic;
using System.Linq;

using DataVault.Common.Extensions;

namespace DataVault.Query
{
    public class WhereBuilder
    {
        public Dictionary<QueryCondition, QueryLogicOperator> QueryConditions { get; }

        public WhereBuilder(Dictionary<QueryCondition, QueryLogicOperator> queryConditions)
        {
            QueryConditions = queryConditions;
        }

        public static WhereBuilder New(string field, QueryOperator @operator, object value)
        {
            var queryCondition = BuildQueryCondition(field, @operator, value);

            return new WhereBuilder(
                new Dictionary<QueryCondition, QueryLogicOperator>
                {
                    { queryCondition, QueryLogicOperator.None }
                });
        }

        public WhereBuilder And(string field, QueryOperator @operator, object value)
        {
            var queryCondition = BuildQueryCondition(field, @operator, value);

            QueryConditions.Add(queryCondition, QueryLogicOperator.And);

            return this;
        }

        private WhereBuilder Or(string field, QueryOperator @operator, object value)
        {
            var queryCondition = BuildQueryCondition(field, @operator, value);

            QueryConditions.Add(queryCondition, QueryLogicOperator.Or);

            return this;
        }

        public string Build()
        {
            return QueryConditions.Select(x => $"{(x.Value == QueryLogicOperator.None ? string.Empty : x.Value.ToString().ToUpper())} {SqlExpressionBuilder.BuildCondition(x.Key)}  ").JoinNotEmpty();
        }

        private static QueryCondition BuildQueryCondition(string field, QueryOperator @operator, object value)
        {
            var queryCondition = new QueryCondition
            {
                Field = new QueryField
                {
                    FieldName = field
                },
                Operator = @operator,
                Value = new QueryValue(value),
            };

            return queryCondition;
        }
    }
}