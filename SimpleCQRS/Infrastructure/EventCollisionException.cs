using System;

namespace SimpleCQRS.Infrastructure
{
    public class EventCollisionException : Exception
    {
        private const string ERROR_TEXT = "Data has been changed between loading and state changes.";

        public EventCollisionException() : base(ERROR_TEXT) { }
    }
}
