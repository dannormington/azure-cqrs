using SimpleCQRS.Infrastructure;
using System;

namespace SimpleCQRS.Commands
{
    public class ConfirmChangeEmailAddress : ICommand
    {
        public Guid AttendeeId { get; set; }
        public Guid ConfirmationId { get; set; }
    }
}