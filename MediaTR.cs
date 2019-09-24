//Install-Package MediatR
//Install-Package MediatR.Extensions.Microsoft.DependencyInjection
public void ConfigureServices(IServiceCollection services)
{
	//register all of MediatR’s dependencies
    services.AddMediatR(Assembly.GetExecutingAssembly());		
	services.AddTransient<INotifierMediatorService, NotifierMediatorService>();
    //Other injected services. 
}

//INotification: An inbuilt type of MediatR
public class NotificationMessage : INotification
{
    public string NotifyText { get; set; }
}

//INotificationHandler: An inbuilt type of MediatR
//don’t need to register these handlers
//services.AddMediatR(Assembly.GetExecutingAssembly()); => 
//MediatR finds all handlers within the assembly and registers them correctly.
public class Notifier1 : INotificationHandler<NotificationMessage>
{
    public Task Handle(NotificationMessage notification, CancellationToken cancellationToken)
    {
        Debug.WriteLine($"Debugging from Notifier 1. Message  : {notification.NotifyText} ");
        return Task.CompletedTask;
    }
}
 
 
//INotificationHandler: An inbuilt type of MediatR
//don’t need to register these handlers
//services.AddMediatR(Assembly.GetExecutingAssembly()); => 
//MediatR finds all handlers within the assembly and registers them correctly.
public class Notifier2 : INotificationHandler<NotificationMessage>
{
    public Task Handle(NotificationMessage notification, CancellationToken cancellationToken)
    {
        Debug.WriteLine($"Debugging from Notifier 2. Message  : {notification.NotifyText} ");
        return Task.CompletedTask;
    }
}

public interface INotifierMediatorService
{
    void Notify(string notifyText);
}
 
/*
We *can* just inject in the inbuilt IMediator interface everywhere and publish messages directly. 
that wrong because we are basically telling everywhere “by the way, we use MediatR”. 
we wrap it into NotifierMediatorService 
*/
public class NotifierMediatorService : INotifierMediatorService
{
    private readonly IMediator _mediator;
 
    public NotifierMediatorService(IMediator mediator)
    {
        _mediator = mediator;
    }
 
    public void Notify(string notifyText)
    {
        _mediator.Publish(new NotificationMessage { NotifyText = notifyText });
    }
}

// We *can* just inject in the inbuilt IMediator interface everywhere and publish messages directly.
public class HomeController : ControllerBase
{
    private readonly INotifierMediatorService _notifierMediatorService;
 
    public HomeController(INotifierMediatorService notifierMediatorService)
    {
        _notifierMediatorService = notifierMediatorService;
    }
 
    [HttpGet("")]
    public ActionResult<string> NotifyAll()
    {
        _notifierMediatorService.Notify("This is a test notification");
        return "Completed";
    }
}

/*
“In Process Messaging” is “implementation” of the mediator pattern where “mediator” class passes data back and forth  
- It’s an object that encapsulates how objects interact. It can obviously handle passing on “messages” between objects.

- It promotes loose coupling by not having objects refer to each other, but instead to the mediator. 
  So they pass the messages to the mediator, who will pass it on to the right person.
  
- The caller doesn’t need to know how things are being handled (By whom and by how many handlers)
*/
