Azure CQRS with Event Sourcing
==============================

CQRS pattern implemented using C# on Azure. The example code includes both a base set of classes as well as a sample domain model, events, and set of endpoints to interact.

This implementation is inspired by Greg Young's Simple CQRS C# example
located at https://github.com/gregoryyoung/m-r

Why?
----
Similar to my node example the purpose of the repo was to learn Azure while applying patterns I enjoy working with. 

Application Domain
------------------
The domain for this particular example is conference registration. Since this sample application is supposed to be "simple" the only behaviors at this point are:
- Register
- Unregister
- Change your email
- Confirm your new email

Simple?
-------
Well, maybe not as simple as I intended. The domain model is simple that is for sure. As I mentioned above the purpose of this repo was to learn Azure. More specifically it was created to understand how Azure Service Bus works. There are two main components of this repository.

- Web Role
  - Handles all web requests to register, unregister, change email, confirm changed email
- Worker Role
  - Processes all events sent to the Azure Service Bus Queue

The basic workflow is as follows:

1. Command is received by Web API
2. The command is sent through a simple message bus implementation and is processed by a handler
3. The handler receives the command and makes the necessary state changes through the domain model
4. The repository persists the changes through an event store implementation
5. If the event store successfully persists the new event(s), it then sends the event(s) to an Azure Service Bus Queue
6. The worker role receives the message and processes it by sending the events to their appropriate event handlers
7. The event handlers process the event(s) where typically a read model is created or updated
