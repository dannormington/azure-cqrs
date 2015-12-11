using SimpleCQRS.Infrastructure;
using System;

namespace SimpleCQRS.Events
{
    public class AttendeeUnregistered : Event
    {
        public readonly string Reason;

        public AttendeeUnregistered(Guid attendeeId, string reason) : base(attendeeId)
        {
            this.Reason = reason;
        }    
    }
}
