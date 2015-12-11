using SimpleCQRS.Infrastructure;
using System;

namespace SimpleCQRS.Commands
{
    public class RegisterAttendee : ICommand
    {
        public Guid AttendeeId { get; set; }
        public string Email { get; set; }
    }
}