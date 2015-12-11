using Microsoft.WindowsAzure.Storage.Table;
using System;

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
        T GetById(Guid id);

        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TableResult Insert(T entity);

        /// <summary>
        /// Update an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TableResult Update(T entity);
    }
}
