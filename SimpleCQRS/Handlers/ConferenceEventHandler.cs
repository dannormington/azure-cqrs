using SimpleCQRS.Events;
using SimpleCQRS.Infrastructure;
using System.Diagnostics;

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
