using SimpleCQRS.Infrastructure;
using System;

namespace SimpleCQRS.Events
{
    public class AttendeeChangeEmailConfirmed : Event
    {
        public readonly string Email;
        public readonly Guid ConfirmationId;

        public AttendeeChangeEmailConfirmed(Guid id, Guid confirmationId, string email)
            : base(id) 
        {
            this.Email = email;
            this.ConfirmationId = confirmationId;
        }
    }
}
