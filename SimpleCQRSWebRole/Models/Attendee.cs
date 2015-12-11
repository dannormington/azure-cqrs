using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleCQRSWebRole.Models
{
    public class Attendee
    {
        public Guid AttendeeId { get; set; }
        public string Email { get; set; }
    }
}