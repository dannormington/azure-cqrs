using SimpleCQRS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
