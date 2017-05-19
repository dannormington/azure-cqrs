using System.Net;

namespace SimpleCQRSWebRole.Models
{
    public class ExceptionResponse
    {
        public ExceptionResponse(HttpStatusCode status, string message)
        {
            Status = status;
            Message = message;
        }

        public HttpStatusCode Status { get; }

        public string Message { get; }
    }
}