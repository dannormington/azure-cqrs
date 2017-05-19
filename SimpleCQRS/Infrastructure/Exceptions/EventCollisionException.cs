using System;

namespace SimpleCQRS.Infrastructure.Exceptions
{
    public class EventCollisionException : AggregateException
    {
        private const string ERROR_TEXT = "Data has been changed between loading and state changes.";

        public EventCollisionException(Guid aggregateId) : base(aggregateId, ERROR_TEXT) { }

        public EventCollisionException(Guid aggregateId, string message) : base(aggregateId, message) { }
    }
}
