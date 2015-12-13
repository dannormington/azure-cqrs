using System;

namespace SimpleCQRS.Infrastructure.Query
{
    /// <summary>
    /// The purpose of this class is to support data access to attendee entities
    /// </summary>
    public class AttendeeDataAccess : DataAccess<AttendeeEntity>, IAttendeeDataAccess
    {
        public AttendeeDataAccess() : base("attendee") { }

        public AttendeeEntity GetById(Guid id)
        {
            return this.Get(id.ToString(), "0");
        }
    }
}
