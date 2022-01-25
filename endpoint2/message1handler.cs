using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class Message1Handler :
    IHandleMessages<Command>
{
    static ILog log = LogManager.GetLogger<Message1Handler>();

    public Task Handle(Command message, IMessageHandlerContext context)
    {
        log.Info("Hello from CommandMessageHandler");
        Task reply;
        if (message.Id % 2 == 0)
        {
            reply = context.Reply(1);
        }
        else
        {
            reply = context.Reply(0);
        }
        return reply;
    }
}