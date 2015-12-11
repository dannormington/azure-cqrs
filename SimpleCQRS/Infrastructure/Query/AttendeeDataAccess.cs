using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCQRS.Infrastructure.Query
{
    public class AttendeeDataAccess : IDataAccess<AttendeeEntity>
    {
        private CloudTable _attendees;

        public AttendeeDataAccess()
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.WindowsAzure.Storage.ConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _attendees = tableClient.GetTableReference("attendee");
            _attendees.CreateIfNotExists();
        }

        public AttendeeEntity GetById(Guid id)
        {
            var query = (from attendee in _attendees.CreateQuery<AttendeeEntity>()
                         where attendee.PartitionKey == id.ToString()
                         select attendee).ToList();

            return query.FirstOrDefault();
        }

        public TableResult Insert(AttendeeEntity attendee)
        {
            var operation = TableOperation.Insert(attendee);
            return _attendees.Execute(operation);
        }

        public TableResult Update(AttendeeEntity attendee)
        {
            var operation = TableOperation.Merge(attendee);
            return _attendees.Execute(operation);
        }
    }
}
