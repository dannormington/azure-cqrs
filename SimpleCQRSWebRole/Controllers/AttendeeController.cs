using SimpleCQRS.Commands;
using SimpleCQRS.Infrastructure;
using SimpleCQRS.Infrastructure.Query;
using SimpleCQRSWebRole.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace SimpleCQRSWebRole.Controllers
{
    public class AttendeeController : ApiController
    {
        private readonly IMessageBus _bus;
        private readonly IAttendeeDataAccess _dataAccess;

        public AttendeeController(IMessageBus bus, IAttendeeDataAccess dataAccess) {
            _bus = bus;
            _dataAccess = dataAccess;
        }

        [HttpGet]
        [ResponseType(typeof(Attendee))]
        [Route("api/attendee/{attendeeId}")]
        public HttpResponseMessage GetAttendee(Guid? attendeeId)
        {
            if (attendeeId.HasValue)
            {
                var attendee = _dataAccess.GetById(attendeeId.Value);
              
                if (attendee != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new Attendee()
                    {
                        AttendeeId = Guid.Parse(attendee.PartitionKey),
                        Email = attendee.Email
                    });
                }
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public HttpResponseMessage RegisterAttendee([FromBody]RegisterAttendee value)
        {
            try
            {
                _bus.Send(value);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                //log the exception
                Trace.WriteLine(ex.Message);

                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("api/attendee/{attendeeId}/email")]
        public HttpResponseMessage ChangeEmail(Guid? attendeeId, [FromBody]ChangeEmailAddress command)
        {
            if (!attendeeId.HasValue || command == null || string.IsNullOrEmpty(command.Email))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            try
            {
                command.AttendeeId = attendeeId.Value;
                
                _bus.Send(command);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                //log the exception
                Trace.WriteLine(ex.Message);

                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("api/attendee/{attendeeId}/email/{confirmationId}")]
        public HttpResponseMessage ConfirmChangeEmail(Guid? attendeeId, Guid? confirmationId)
        {

            if (!attendeeId.HasValue || !confirmationId.HasValue)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var command = new ConfirmChangeEmailAddress()
            {
                AttendeeId = attendeeId.Value,
                ConfirmationId = confirmationId.Value
            };

            try
            {
                _bus.Send(command);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                //log the exception
                Trace.WriteLine(ex.Message);

                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        [Route("api/attendee/{attendeeId}")]
        public HttpResponseMessage UnregisterAttendee(Guid? attendeeId, [FromBody]UnregisterAttendee value)
        {
            try
            {
                _bus.Send(value);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                //log the exception
                Trace.WriteLine(ex.Message);

                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
