using SimpleCQRS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCQRS.Events
{
    public class AttendeeEmailChanged : Event
    {
        public readonly string Email;
        public readonly Guid ConfirmationId;

        public AttendeeEmailChanged(Guid id, Guid confirmationId, string email)
            : base(id)
        {
            this.Email = email;
            this.ConfirmationId = confirmationId;
        }
    }
}
