using SimpleCQRS.Events;
using System;

namespace SimpleCQRS.Domain
{
    public class Attendee : AggregateRoot
    {
        private string _unconfirmedEmail;
        private Guid? _confirmationId;

        public Attendee() { }

        public Attendee(Guid id, string email) 
        {
            this.ApplyChange(new AttendeeRegistered(id, email));
        }

        public void ChangeEmailAddress(string email) 
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                return;

            this.ApplyChange(new AttendeeEmailChanged(this.Id, Guid.NewGuid(), email.Trim()));
        }

        public void ConfirmChangeEmail(Guid confirmationId) 
        {
            if (confirmationId == this._confirmationId) 
            {
                this.ApplyChange(new AttendeeChangeEmailConfirmed(this.Id, confirmationId, this._unconfirmedEmail));
            }
        }

        public void Unregister(string reason)
        {
            this.ApplyChange(new AttendeeUnregistered(this.Id, reason));
        }

        private void Apply(AttendeeRegistered @event) 
        {
            this.Id = @event.Id;
        }

        private void Apply(AttendeeEmailChanged @event) 
        {
            _unconfirmedEmail = @event.Email;
            _confirmationId = @event.ConfirmationId;
        }

        private void Apply(AttendeeChangeEmailConfirmed @event) 
        {
            _confirmationId = null;
            _unconfirmedEmail = null;
        }
    }
}
