using SimpleCQRS.Infrastructure;
using System;

namespace SimpleCQRS.Events
{
    public class AttendeeRegistered : Event
    {
        public readonly string Email;

        public AttendeeRegistered(Guid attendeeId, string email)
            : base(attendeeId)
        {
            this.Email = email;
        }
    }
}
