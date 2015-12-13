using SimpleCQRS.Infrastructure;
using SimpleCQRS.Commands;
using System;
using SimpleCQRS.Domain;
using SimpleCQRS.Infrastructure.Persistence;

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

        void IHandles<ConfirmChangeEmailAddress>.Handle(ConfirmChangeEmailAddress command)
        {
            var attendee = _repository.GetById(command.AttendeeId);
            attendee.ConfirmChangeEmail(command.ConfirmationId);
            _repository.Save(attendee);
        }

        void IHandles<UnregisterAttendee>.Handle(UnregisterAttendee command)
        {
            var attendee = _repository.GetById(command.AttendeeId);
            attendee.Unregister(command.Reason);
            _repository.Save(attendee);
        }

        void IHandles<ChangeEmailAddress>.Handle(ChangeEmailAddress command)
        {
            var attendee = _repository.GetById(command.AttendeeId);
            attendee.ChangeEmailAddress(command.Email);
            _repository.Save(attendee);
        }

        void IHandles<RegisterAttendee>.Handle(RegisterAttendee command)
        {
            var attendee = new Attendee(command.AttendeeId, command.Email);
            _repository.Save(attendee);
        }
    }
}
