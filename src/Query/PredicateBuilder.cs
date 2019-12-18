using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DataVault.Common.Extensions;

using Newtonsoft.Json;

namespace DataVault.Query
{
    public class PredicateBuilder
    {
        private readonly Dictionary<QueryCondition, QueryLogicOperator> _queryConditions;

        public PredicateBuilder(Dictionary<QueryCondition, QueryLogicOperator> queryConditions)
        {
            _queryConditions = queryConditions;
        }

        public static PredicateBuilder New<T>(Expression<Func<T, object>> expression, QueryOperator @operator, object value)
        {
            var queryCondition = BuildQueryCondition(expression, @operator, value);

            return new PredicateBuilder(
                new Dictionary<QueryCondition, QueryLogicOperator>
                {
                    { queryCondition, QueryLogicOperator.None }
                });
        }

        public PredicateBuilder And<T>(Expression<Func<T, object>> expression, QueryOperator @operator, object value)
        {
            var queryCondition = BuildQueryCondition(expression, @operator, value);

            _queryConditions.Add(queryCondition, QueryLogicOperator.And);

            return this;
        }

        public PredicateBuilder Or<T>(Expression<Func<T, object>> expression, QueryOperator @operator, object value)
        {
            var queryCondition = BuildQueryCondition(expression, @operator, value);

            _queryConditions.Add(queryCondition, QueryLogicOperator.Or);

            return this;
        }

        public string Build()
        {
            return _queryConditions.Select(x => $"{(x.Value == QueryLogicOperator.None ? string.Empty : x.Value.ToString().ToUpper())} {SqlExpressionBuilder.BuildCondition(x.Key)}  ").JoinNotEmpty();
        }

        private static QueryCondition BuildQueryCondition<T>(Expression<Func<T, object>> expression, QueryOperator @operator, object value)
        {
            var queryCondition = new QueryCondition
            {
                Field = new QueryField
                {
                    TableName = typeof(T).Name.ToUnderscoreCase().ToUpper(),
                    FieldName = ReflectionHelper.GetAttribute<JsonPropertyAttribute, T>(expression)?.PropertyName
                },
                Operator = @operator,
                Value = new QueryValue(value),
            };

            return queryCondition;
        }
    }
}