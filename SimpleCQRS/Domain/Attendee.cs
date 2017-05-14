using SimpleCQRS.Events;
using System;

namespace SimpleCQRS.Domain
{
    public class Attendee : AggregateRoot
    {
        private string _unconfirmedEmail;
        private Guid? _confirmationId;
        private bool _isUnregistered;

        public Attendee() { }

        public Attendee(Guid id, string email) 
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("email");
            }

            this.ApplyChange(new AttendeeRegistered(id, email));
        }

        public void ChangeEmailAddress(string email) 
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("email");
            }
                
            this.ApplyChange(new AttendeeEmailChanged(this.Id, Guid.NewGuid(), email.Trim()));
        }

        public void ConfirmChangeEmail(Guid confirmationId) 
        {
            if (confirmationId == this._confirmationId) 
            {
                this.ApplyChange(new AttendeeChangeEmailConfirmed(this.Id, confirmationId, this._unconfirmedEmail));
            }

            throw new ArgumentException("confirmation Id does not match.", "confirmationId");
        }

        public void Unregister(string reason)
        {
            if (_isUnregistered) throw new InvalidOperationException("Attendee is already unregistered.");

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

        private void Apply(AttendeeUnregistered @event)
        {
            _isUnregistered = true;
        }
    }
}
