using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.ASBS.SendReply.Endpoint1";

        #region config

        var endpointConfiguration = new EndpointConfiguration("Samples.ASBS.SendReply.Endpoint1");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.MakeInstanceUniquelyAddressable("1");
        endpointConfiguration.EnableCallbacks();

        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();

        var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus_ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Could not read the 'AzureServiceBus_ConnectionString' environment variable. Check the sample prerequisites.");
        }
        transport.ConnectionString(connectionString);

        #endregion

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press 'enter' to send a message");
        Console.WriteLine("Press any other key to exit");

        //IMessageSession

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key != ConsoleKey.Enter)
            {
                break;
            }

            var orderId = Guid.NewGuid();
            var message = new Message1
            {
                Property = "Hello from Endpoint1"
            };

            var command = new Command
            {
                Id = 1
            };

            var sendOptions = new SendOptions();
            sendOptions.SetDestination("Samples.ASBS.SendReply.Endpoint2");
            var code = await endpointInstance.Request<ErrorCodes>(command, sendOptions);
            var text = Enum.GetName(typeof(ErrorCodes), code);
            Console.WriteLine($"Responce received: code { code }, text { text }");
        }
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}