using System;

namespace SimpleCQRS.Infrastructure.Exceptions
{
    public abstract class AggregateException : Exception
    {
        public AggregateException(Guid aggregateId, string message) : base(message)
        {
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }

        public override string Message
        {
            get
            {
                return $"Aggregate Id : {AggregateId} - {base.Message}";
            }
        }
    }
}
