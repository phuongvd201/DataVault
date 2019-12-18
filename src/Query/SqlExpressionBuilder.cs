using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DataVault.Common.Extensions;

namespace DataVault.Query
{
    public class SqlExpressionBuilder
    {
        public static Dictionary<QueryOperator, string> QueryOperatorToSqlOperators = new Dictionary<QueryOperator, string>()
        {
            { QueryOperator.Equal, "=" },
            { QueryOperator.GreaterThan, ">" },
            { QueryOperator.GreaterThanOrEqual, ">=" },
            { QueryOperator.LessThan, "<" },
            { QueryOperator.LessThanOrEqual, "<=" },
            { QueryOperator.Like, "LIKE" },
            { QueryOperator.In, "IN" },
            { QueryOperator.NotEqual, "!=" },
        };

        public static string BuildCondition(QueryCondition queryCondition)
        {
            var field = BuildField(queryCondition.Field);
            var @operator = QueryOperatorToSqlOperators.GetValueOrDefault(queryCondition.Operator);
            var value = BuildValue(queryCondition.Value);

            return $"{field} {@operator} {value}";
        }

        private static string BuildValue(QueryValue value)
        {
            object constValue = value.Value;

            switch (constValue)
            {
                case null:
                    return "NULL";

                case string s:
                    return BuildValue(s);

                case IEnumerable e:
                    return BuildValue(e);

                default:
                    return Convert.ToString(constValue, CultureInfo.InvariantCulture);
            }
        }

        private static string BuildValue(IEnumerable values)
        {
            return values.OfType<string>().Select(BuildValue).JoinNotEmpty(",").WrapCurlyBracket();
        }

        protected static string BuildValue(string value)
        {
            if (value.Contains("UNHEX"))
            {
                return value;
            }

            return "'" + value.Replace(@"'", @"\'") + "'";
        }

        private static string BuildField(QueryField field)
        {
            return field.TableName.IsNullOrWhiteSpace() ? $"`{field.FieldName}`" : $"`{field.TableName}.{field.FieldName}`";
        }
    }
}