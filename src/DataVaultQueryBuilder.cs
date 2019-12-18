using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DataVault.Common.Extensions;
using DataVault.Query;

using Newtonsoft.Json;

namespace DataVault
{
    public class DataVaultQueryBuilder
    {
        public List<PredicateBuilder> FilterQueries { get; private set; }

        public List<string> SortQueries { get; private set; }

        public int RowCount { get; private set; }

        public int RowNumber { get; private set; }

        public static DataVaultQueryBuilder New()
        {
            return new DataVaultQueryBuilder
            {
                FilterQueries = new List<PredicateBuilder>(),
                SortQueries = new List<string>(),
                RowCount = 100,
                RowNumber = 0,
            };
        }

        public DataVaultQueryBuilder Filter(PredicateBuilder filter)
        {
            FilterQueries.Add(filter);

            return this;
        }

        public DataVaultQueryBuilder SetRowCount(int rowCount)
        {
            RowCount = rowCount;

            return this;
        }

        public DataVaultQueryBuilder SetRowNumber(int rowNumber)
        {
            RowNumber = rowNumber;

            return this;
        }

        public object BuildViewQuery()
        {
            return new
            {
                filter = new[] { FilterQueries.Select(x => x.Build()).JoinNotEmpty(", ") },
                order = SortQueries,
                row_number = RowNumber,
                row_count = RowCount,
            };
        }

        public DataVaultQueryBuilder SortAscending<T>(Expression<Func<T, object>> expression)
        {
            var tableName = typeof(T).Name.ToUnderscoreCase().ToUpper();
            var fieldName = ReflectionHelper.GetAttribute<JsonPropertyAttribute, T>(expression)?.PropertyName;

            var sort = $"`{tableName}.{fieldName}` {SortDirection.Asc.ToString().ToUpper()}";

            SortQueries.Add(sort);

            return this;
        }

        public DataVaultQueryBuilder SortDescending<T>(Expression<Func<T, object>> expression)
        {
            var tableName = typeof(T).Name.ToUnderscoreCase().ToUpper();
            var fieldName = ReflectionHelper.GetAttribute<JsonPropertyAttribute, T>(expression)?.PropertyName;

            var sort = $"`{tableName}.{fieldName}` {SortDirection.Desc.ToString().ToUpper()}";

            SortQueries.Add(sort);

            return this;
        }
    }
}