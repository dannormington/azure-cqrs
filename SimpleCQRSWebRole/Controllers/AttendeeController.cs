using Newtonsoft.Json;
using SimpleCQRS.Commands;
using SimpleCQRS.Infrastructure;
using SimpleCQRS.Infrastructure.Exceptions;
using SimpleCQRS.Infrastructure.Query;
using SimpleCQRSWebRole.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        [Route("api/attendee/register")]
        public async Task<HttpResponseMessage> RegisterAttendeeAsync([FromBody]RegisterAttendee command)
        {
            return await ProcessRequestAsync(command);
        }

        [HttpPost]
        [Route("api/attendee/{attendeeId}/email")]
        public async Task<HttpResponseMessage> ChangeEmailAsync(Guid? attendeeId, [FromBody]ChangeEmailAddress command)
        {
            if (command == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);

            command.AttendeeId = attendeeId.Value;
            return await ProcessRequestAsync(command);
        }

        [HttpPost]
        [Route("api/attendee/{attendeeId}/email/{confirmationId}")]
        public async Task<HttpResponseMessage> ConfirmChangeEmailAsync(Guid? attendeeId, Guid? confirmationId)
        {
            var command = new ConfirmChangeEmailAddress()
            {
                AttendeeId = attendeeId.Value,
                ConfirmationId = confirmationId.Value
            };

            return await ProcessRequestAsync(command);
        }

        [HttpDelete]
        [Route("api/attendee/{attendeeId}")]
        public async Task<HttpResponseMessage> UnregisterAttendeeAsync(Guid? attendeeId, [FromBody]UnregisterAttendee command)
        {
            if (command == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);

            command.AttendeeId = attendeeId.Value;
            return await ProcessRequestAsync(command);
        }

        private async Task<HttpResponseMessage> ProcessRequestAsync(ICommand command)
        {
            try
            {
                if (command == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);

                await _bus.SendAsync(command);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (EventCollisionException x)
            {
                return BuildExceptionResponse(HttpStatusCode.Conflict, x);
            }
            catch (AggregateNotFoundException x)
            {
                return BuildExceptionResponse(HttpStatusCode.NotFound, x);
            }
            catch (HydrationException x)
            {
                return BuildExceptionResponse(HttpStatusCode.InternalServerError, x);
            }
            catch (InvalidOperationException x)
            {
                return BuildExceptionResponse(HttpStatusCode.BadRequest, x);
            }
            catch (ArgumentException x)
            {
                return BuildExceptionResponse(HttpStatusCode.BadRequest, x);
            }
            catch (Exception x)
            {
                return BuildExceptionResponse(HttpStatusCode.InternalServerError, x);
            }
        }

        private HttpResponseMessage BuildExceptionResponse(HttpStatusCode status, Exception exception)
        {
            var error = new ExceptionResponse(status, exception.Message);
            var json = JsonConvert.SerializeObject(error);

            return new HttpResponseMessage(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        }
    }
}
