using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace SimpleCQRS.Infrastructure.Query
{
    /// <summary>
    /// Interface to get/set data in table storage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataAccess<T>
        where T : TableEntity
    {
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Get(string partitionKey, string rowKey);

        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TableResult> InsertAsync(T entity);

        /// <summary>
        /// Update an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TableResult> UpdateAsync(T entity);
    }
}
