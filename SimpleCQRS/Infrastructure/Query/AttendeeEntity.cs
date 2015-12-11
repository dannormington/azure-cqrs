using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace SimpleCQRS.Infrastructure.Query
{
    /// <summary>
    /// Entity representation of an attendee
    /// </summary>
    public class AttendeeEntity : TableEntity
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AttendeeEntity() { }

        public AttendeeEntity(Guid attendeeId, string email)
        {
            this.PartitionKey = attendeeId.ToString();
            this.Email = email;
            this.IsActiveRegistration = true;
            this.RowKey = "0";
        }

        public bool IsActiveRegistration { get; set; }

        public string Email { get; set; }

        public string ReasonForUnregistration { get; set; }
    }
}
