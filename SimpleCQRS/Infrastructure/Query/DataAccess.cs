using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure;

namespace SimpleCQRS.Infrastructure.Query
{
    public class DataAccess<T> : IDataAccess<T>
        where T : TableEntity, new()
    {
        protected readonly string _connectionString;
        protected readonly string _tableName;
        protected CloudTable _table;

        public DataAccess(string tableName)
        {
            _connectionString = CloudConfigurationManager.GetSetting("Microsoft.WindowsAzure.Storage.ConnectionString");
            _tableName = tableName;
        }

        private void Initialize()
        {
            if (_table != null)
            {
                return;
            }

            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            _table = tableClient.GetTableReference(_tableName);
            _table.CreateIfNotExists();
        }

        public T Get(string partitionKey, string rowKey)
        {
            Initialize();

            var query = (from element in _table.CreateQuery<T>()
                         where element.PartitionKey == partitionKey && element.RowKey == rowKey
                         select element).ToList();


            return query.FirstOrDefault();
        }

        public TableResult Insert(T entity)
        {
            Initialize();

            var operation = TableOperation.Insert(entity);
            return _table.Execute(operation);
        }

        public TableResult Update(T entity)
        {
            Initialize();

            var operation = TableOperation.Merge(entity);
            return _table.Execute(operation);
        }
    }
}
