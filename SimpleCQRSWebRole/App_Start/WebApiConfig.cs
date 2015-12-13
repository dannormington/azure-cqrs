using Microsoft.Azure;
using Microsoft.Practices.Unity;
using SimpleCQRS.Commands;
using SimpleCQRS.Domain;
using SimpleCQRS.Events;
using SimpleCQRS.Handlers;
using SimpleCQRS.Infrastructure;
using SimpleCQRS.Infrastructure.Persistence;
using SimpleCQRS.Infrastructure.Query;
using System.Web.Http;

namespace SimpleCQRSWebRole
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();

            ////register the commands
            IMessageBus messageBus = new MessageBus(container);
            messageBus.RegisterHandler<ChangeEmailAddress, ConferenceCommandHandler>();
            messageBus.RegisterHandler<ConfirmChangeEmailAddress, ConferenceCommandHandler>();
            messageBus.RegisterHandler<RegisterAttendee, ConferenceCommandHandler>();
            messageBus.RegisterHandler<UnregisterAttendee, ConferenceCommandHandler>();

            container.RegisterInstance(messageBus);
            container.RegisterType<IEventStore, EventStore>();
            container.RegisterType<IRepository<Attendee>, Repository<Attendee>>();
            container.RegisterType<IAttendeeDataAccess, AttendeeDataAccess>();
            
            config.DependencyResolver = new DependencyResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

           
        }
    }
}
