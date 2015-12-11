using SimpleCQRS.Commands;
using SimpleCQRS.Infrastructure;
using SimpleCQRS.Infrastructure.Query;
using SimpleCQRSWebRole.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace SimpleCQRSWebRole.Controllers
{
    public class AttendeeController : ApiController
    {
        private IMessageBus _bus;

        public AttendeeController(IMessageBus bus) {
            _bus = bus;
        }

        // GET: api/Attendee
        public IEnumerable<Attendee> Get()
        {
            //don't support getting back the entire list
            return new Attendee[] { };
        }

        // GET: api/Attendee/5
        [ResponseType(typeof(Attendee))]
        public HttpResponseMessage Get(Guid? id)
        {
            if (id.HasValue)
            {
                IDataAccess<AttendeeEntity> data = new AttendeeDataAccess();
                var attendee = data.GetById(id.Value);

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

        // POST: api/Attendee
        public HttpResponseMessage Post([FromBody]RegisterAttendee value)
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

        // PUT: api/Attendee
        public HttpResponseMessage Put([FromBody]ChangeEmailAddress value)
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

        // PUT: Confirm change email address
        [Route("api/Attendee/{attendeeId}/email/{confirmationId}")]
        public HttpResponseMessage Put(Guid? attendeeId, Guid? confirmationId)
        {

            if (!attendeeId.HasValue || !confirmationId.HasValue)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            //build the command
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

        // DELETE: api/Attendee/5
        public HttpResponseMessage Delete([FromBody]UnregisterAttendee value)
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
