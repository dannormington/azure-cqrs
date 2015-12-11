using SimpleCQRS.Infrastructure;
using System;

namespace SimpleCQRS.Commands
{
    public class UnregisterAttendee : ICommand
    {
        public Guid AttendeeId { get; set; }
        public string Reason { get; set; }
    }
}
