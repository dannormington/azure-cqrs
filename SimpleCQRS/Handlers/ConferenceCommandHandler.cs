using SimpleCQRS.Infrastructure;
using SimpleCQRS.Commands;
using System;
using SimpleCQRS.Domain;
using SimpleCQRS.Infrastructure.Persistence;
using System.Threading.Tasks;

namespace SimpleCQRS.Handlers
{
    /// <summary>
    /// Class to handle conference attendee commands
    /// </summary>
    public class ConferenceCommandHandler : 
        IHandles<RegisterAttendee>, 
        IHandles<ChangeEmailAddress>, 
        IHandles<ConfirmChangeEmailAddress>,
        IHandles<UnregisterAttendee>
    {
        private readonly IRepository<Attendee> _repository;

        public ConferenceCommandHandler(IRepository<Attendee> repository)
        {
            _repository = repository;
        }

        Task IHandles<ConfirmChangeEmailAddress>.HandleAsync(ConfirmChangeEmailAddress command)
        {
            var attendee = _repository.GetById(command.AttendeeId);
            attendee.ConfirmChangeEmail(command.ConfirmationId);
            return _repository.SaveAsync(attendee);
        }

        Task IHandles<UnregisterAttendee>.HandleAsync(UnregisterAttendee command)
        {
            var attendee = _repository.GetById(command.AttendeeId);
            attendee.Unregister(command.Reason);
            return _repository.SaveAsync(attendee);
        }

        Task IHandles<ChangeEmailAddress>.HandleAsync(ChangeEmailAddress command)
        {
            var attendee = _repository.GetById(command.AttendeeId);
            attendee.ChangeEmailAddress(command.Email);
            return _repository.SaveAsync(attendee);
        }

        Task IHandles<RegisterAttendee>.HandleAsync(RegisterAttendee command)
        {
            var attendee = new Attendee(command.AttendeeId, command.Email);
            return _repository.SaveAsync(attendee);
        }
    }
}
