using SimpleCQRS.Events;
using SimpleCQRS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCQRS.Handlers
{
    public class ConferenceEventHandler : 
        IHandles<AttendeeRegistered>,
        IHandles<AttendeeChangeEmailConfirmed>,
        IHandles<AttendeeEmailChanged>
    {
        void IHandles<AttendeeRegistered>.Handle(AttendeeRegistered message)
        {
            Trace.WriteLine("Processing Service Bus message: " + message.GetType().AssemblyQualifiedName);
        }

        void IHandles<AttendeeChangeEmailConfirmed>.Handle(AttendeeChangeEmailConfirmed message)
        {
            Trace.WriteLine("Processing Service Bus message: " + message.GetType().AssemblyQualifiedName);
        }

        void IHandles<AttendeeEmailChanged>.Handle(AttendeeEmailChanged message)
        {
            Trace.WriteLine("Processing Service Bus message: " + message.GetType().AssemblyQualifiedName);
        }
    }
}
