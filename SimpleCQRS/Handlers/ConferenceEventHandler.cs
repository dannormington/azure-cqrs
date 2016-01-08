using SimpleCQRS.Events;
using SimpleCQRS.Infrastructure;
using SimpleCQRS.Infrastructure.Query;
using System.Threading.Tasks;

namespace SimpleCQRS.Handlers
{
    /// <summary>
    /// Class to handle conference attendee events
    /// </summary>
    public class ConferenceEventHandler : 
        IHandles<AttendeeRegistered>,
        IHandles<AttendeeChangeEmailConfirmed>,
        IHandles<AttendeeEmailChanged>,
        IHandles<AttendeeUnregistered>
    {
        private readonly IAttendeeDataAccess _dataAccess;

        public ConferenceEventHandler(IAttendeeDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        async Task IHandles<AttendeeRegistered>.HandleAsync(AttendeeRegistered message)
        {
            var attendee = _dataAccess.GetById(message.Id);

            if (attendee == null)
            {
                attendee = new AttendeeEntity(message.Id, message.Email);
                await _dataAccess.InsertAsync(attendee);
            }
        }

        async Task IHandles<AttendeeUnregistered>.HandleAsync(AttendeeUnregistered message)
        {
            var attendee = _dataAccess.GetById(message.Id);

            if (attendee != null)
            {
                attendee.IsActiveRegistration = false;
                attendee.ReasonForUnregistration = message.Reason;
                await _dataAccess.UpdateAsync(attendee);
            }
        }

        async Task IHandles<AttendeeChangeEmailConfirmed>.HandleAsync(AttendeeChangeEmailConfirmed message)
        {
            var attendee = _dataAccess.GetById(message.Id);

            if (attendee != null)
            {
                attendee.Email = message.Email;
                await _dataAccess.UpdateAsync(attendee);
            }
        }

        Task IHandles<AttendeeEmailChanged>.HandleAsync(AttendeeEmailChanged message)
        {
            //nothing to do yet
            return Task.FromResult(0);
        }
    }
}
