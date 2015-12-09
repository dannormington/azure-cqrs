using SimpleCQRS.Infrastructure;
using System;

namespace SimpleCQRS.Events
{
    public class AttendeeRegistered : Event
    {
        public readonly string Email;

        public AttendeeRegistered(Guid id, string email)
            : base(id)
        {
            this.Email = email;
        }
    }
}
