using System;

using DataVault.Entities;
using DataVault.Entities.Dv;

namespace DataVault
{
    /// <summary>
    /// Provide internal APIs (Tier 0)
    /// </summary>
    public interface IDataVaultContext : IDisposable
    {
        /// <summary>
        /// ReadRecord[HUB_NAME](IN HUB_DATA, IN SAT_DATA, OUT RESULT, OUT STATUS_CODE)
        /// </summary>
        /// <typeparam name="TEntity">Type of hub (The name of the record, for example, HUB_CUSTOMER, we extract the word CUSTOMER.)</typeparam>
        /// <param name="hubData">The business/primary keys values if available, JSON object.</param>
        /// <param name="satData">This can have multiple conditions
        /// NULL | {} | empty | []  = No satellite data
        /// *  =  All satellite data
        /// JSON Array(Example below) = Specific satellite data</param>
        /// <returns></returns>
        TEntity ReadRecord<TEntity>(object hubData, object satData) where TEntity : IDvHubEntity;

        /// <summary>
        /// CreateRecord[HUB_NAME](IN HUB_DATA, IN SAT_DATA, OUT RESULT, OUT STATUS_CODE)
        /// </summary>
        /// <typeparam name="TEntity">Type of hub (The name of the record, for example, HUB_CUSTOMER, we extract the word CUSTOMER.)</typeparam>
        /// <param name="hubData">The business keys values if available, JSON array.</param>
        /// <param name="satData">The attributes of the satellite and the function will map accordingly. only insert in the satellites that have attributes,
        /// if there is a satellite with no attributes in the array, then no rows are inserted into that satellite. JSON array.
        /// Data is a key/value pair.</param>
        /// <returns></returns>
        string CreateRecord<TEntity>(object hubData, object satData) where TEntity : IDvHubEntity;

        /// <summary>
        /// LinkRecord[LINK_NAME](IN FK[], IN SAT_DATA, OUT RESULT, OUT STATUS_CODE)
        /// </summary>
        /// <typeparam name="TEntity">Type of hub (The name of the record, for example, HUB_CUSTOMER, we extract the word CUSTOMER.)</typeparam>
        /// <param name="foreignKeys"> Array of foreign key of HUBs. it's json format include key and value.</param>
        /// <param name="satData">The attributes of the satellite and the function will map accordingly.
        /// only insert in the satellites that have attributes,
        /// if there is a satellite with no attributes in the array, then no rows are inserted into that satellite. JSON array.</param>
        /// <returns></returns>
        string LinkRecord<TEntity>(object foreignKeys, object satData) where TEntity : IDvLinkEntity;

        /// <summary>
        /// UpdateRecord[HUB_NAME](IN HUB_PK, IN SAT_DATA, OUT RESULT, OUT STATUS_CODE)
        /// </summary>
        /// <typeparam name="TEntity">Type of hub (The name of the record, for example, HUB_CUSTOMER, we extract the word CUSTOMER.)</typeparam>
        /// <param name="hubPk">The primary key of HUB. (UPPERCASE, HEX(UUID) - We will pass the VARCHAR(32) version of the UUID)</param>
        /// <param name="satData">The attributes of the satellite and the function will map accordingly.
        /// only insert in the satellites that have attributes,
        /// if there is a satellite with no attributes in the array, then no rows are inserted into that satellite. JSON array.
        /// Data is a key/value pair.</param>
        /// <returns></returns>
        string UpdateRecord<TEntity>(string hubPk, object satData) where TEntity : IDvHubEntity;

        /// <summary>
        /// UpdateLink([LINK_NAME](IN LINK_PK, IN SAT_DATA, OUT STATUS_CODE)
        /// </summary>
        /// <typeparam name="TEntity">Type of link entity (The name of the record, for example, HUB_CUSTOMER_USER, we extract the word CUSTOMER and USER.)</typeparam>
        /// <param name="linkPk">The primary key of LINK.</param>
        /// <param name="satData">The attributes of the satellite and the function will map accordingly.
        /// only update in the satellites that have attributes,
        /// if there is a satellite with no attributes in the array, then no rows are updated into that satellite. JSON array</param>
        void UpdateLink<TEntity>(string linkPk, object satData) where TEntity : IDvLinkEntity;

        /// <summary>
        /// DeleteRecord[HUB_NAME](IN HUB_PK, OUT STATUS_CODE)
        /// </summary>
        /// <typeparam name="TEntity">Type of hub (The name of the record, for example, HUB_CUSTOMER, we extract the word CUSTOMER.)</typeparam>
        /// <param name="hubPk">The primary key of HUB.</param>
        void DeleteRecord<TEntity>(string hubPk) where TEntity : IDvHubEntity;

        /// <summary>
        /// UnLinkRecord[LINK_NAME](IN LINK_PK, OUT STATUS_CODE)
        /// </summary>
        /// <typeparam name="TEntity">Type of link entity (The name of the record, for example, HUB_CUSTOMER_USER, we extract the word CUSTOMER and USER.)</typeparam>
        /// <param name="linkPk">The primary key of LINK.</param>
        void UnLinkRecord<TEntity>(string linkPk) where TEntity : IDvLinkEntity;

        /// <summary>
        /// ReadRecords[HUB_NAME](IN SAT_QUERY, OUT RESULT, OUT STATUS_CODE)
        /// </summary>
        /// <typeparam name="TEntity">Type of hub (The name of the record, for example, HUB_CUSTOMER, we extract the word CUSTOMER.)</typeparam>
        /// <param name="satQuery">includes elements for selection, filter, ordering and pagination of results.
        /// select=The list of satellite data to include in the response, EMPTY = Means only the HUB UUIDs+Business Keys+Meta data no other.
        /// filter=The filtering of the data.
        /// order= the column to use to filter the results.
        /// row_number R = Start from row R.
        /// row_count N = Include N rows
        /// </param>
        /// <returns></returns>
        TEntity[] ReadRecords<TEntity>(object satQuery) where TEntity : IDvHubEntity;

        /// <summary>
        /// ReadLinks[LINK_NAME](IN QUERY, OUT RESULT, OUT STATUS_CODE)
        /// </summary>
        /// <typeparam name="TEntity">Type of link entity (The name of the record, for example, HUB_CUSTOMER_USER, we extract the word CUSTOMER and USER.)</typeparam>
        /// <param name="satQuery"> includes elements for selection of links filter by hubs involved and selection, filter, ordering and pagination of results in the satellite of the link.
        /// related_hubs =The list of hubs which need to find links to relate them, EMPTY=Means the list contain every hub
        /// select=The list of satellite data to include in the response, EMPTY = Means only the LINK UUIDs+FK+MetaData no other.
        /// filter=The filtering of the data.
        /// order= order by the column to use to filter the results.
        /// row_number R = Start from row R.
        /// row_count N = Include N rows
        /// </param>
        /// <returns></returns>
        TEntity[] ReadLinks<TEntity>(object satQuery) where TEntity : IDvLinkEntity;

        TEntity[] ReadView<TEntity>(string viewName, object satQuery) where TEntity : IDvEntity;

        TEntity[] ReadView<TEntity>(object satQuery) where TEntity : IDvEntity;

        /// <summary>
        /// Check lookup value is exist in lookup table
        /// Throws <see cref="InvalidLookupException"/> if can not find value in lookup table.
        /// </summary>
        /// <typeparam name="TLookup">Type of lookup</typeparam>
        /// <param name="value">lookup value to check</param>
        void CheckLookupValue<TLookup>(string value);

        /// <summary>
        /// Validate lookup value is exist in lookup table
        /// If is valid return true, else return false;
        /// </summary>
        /// <typeparam name="TLookup">Type of lookup</typeparam>
        /// <param name="value">lookup value to check</param>
        bool ValidateLookupValue<TLookup>(string value);

        bool ValidateLookupValue(string lookupTableName, string value);

        bool ValidateLookupValue(Type lookupType, string value);

        /// <summary>
        /// Get all lookup table names
        /// </summary>
        /// <returns>Array of lookup table names in current database</returns>
        LookupItem[] ReadLookups();

        LookupItem[] ReadLookup(Type lookupType);

        /// <summary>
        /// Get all lookup item with given lookup table type
        /// </summary>
        /// <typeparam name="TLookup">Type of lookup</typeparam>
        /// <returns></returns>
        LookupItem[] ReadLookup<TLookup>();

        /// <summary>
        ///  Get all lookup item with given lookup table name
        /// </summary>
        /// <param name="lookupTableName">Lookup table name (case insensitive)</param>
        /// <returns></returns>
        LookupItem[] ReadLookup(string lookupTableName);

        /// <summary>
        /// Generate unique sequence for business key of data vault entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        string GenerateEntityUniqueSequence<TEntity>() where TEntity : IDvHubEntity;

        /// <summary>
        /// Commit all changes associated with this <see cref="DataVaultContext"/> to database.
        /// </summary>
        void SaveChanges();
    }
}