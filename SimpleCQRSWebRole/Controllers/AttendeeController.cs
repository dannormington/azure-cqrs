using SimpleCQRS.Commands;
using SimpleCQRS.Infrastructure;
using SimpleCQRS.Infrastructure.Query;
using SimpleCQRSWebRole.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
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
            command.AttendeeId = attendeeId.Value;
            return await ProcessRequestAsync(command);
        }

        private async Task<HttpResponseMessage> ProcessRequestAsync(ICommand command)
        {
            try
            {
                await _bus.SendAsync(command);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (InvalidOperationException x) 
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(x.Message) };
            }
            catch (ArgumentException x)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(x.Message) };
            }
            catch (Exception x)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(x.Message) };
            }
        }
    }
}
