using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCQRS.Infrastructure.Exceptions
{
    public class AggregateNotFoundException : AggregateException
    {
        private const string ERROR_TEXT = "The aggregate you requested cannot be found.";

        public AggregateNotFoundException(Guid aggregateId) : base(aggregateId, ERROR_TEXT) { }
    }
}
