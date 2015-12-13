using System;


namespace SimpleCQRS.Infrastructure.Query
{
    /// <summary>
    /// Interface to support data access to attendee entities
    /// </summary>
    public interface IAttendeeDataAccess : IDataAccess<AttendeeEntity>
    {
        /// <summary>
        /// Get an attendee by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AttendeeEntity GetById(Guid id);
    }
}
