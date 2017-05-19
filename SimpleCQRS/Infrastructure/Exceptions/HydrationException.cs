using System;

namespace SimpleCQRS.Infrastructure.Exceptions
{
    public class HydrationException : AggregateException
    {
        private const string ERROR_TEXT = "Loading the data failed.";

        public HydrationException(Guid aggregateId) : base(aggregateId, ERROR_TEXT) { }
    }
}
