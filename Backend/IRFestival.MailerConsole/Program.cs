using Azure.Messaging.ServiceBus;

Console.WriteLine("Hello, I'm a Mailer Console Application!");

var connectionString = "Endpoint=sb://irservicebusjvr.servicebus.windows.net/;SharedAccessKeyName=Listener;SharedAccessKey=yoYpthTIbyE27Nmpp5BZTniynXKD/4txutk1IRsRW6M=;EntityPath=mails";
var queueName = "mails";
await using (var client = new ServiceBusClient(connectionString))
{
    // create a processor that we can use to process the messages
    var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

    // add handler to process messages or errors
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;

    // start processing
    await processor.StartProcessingAsync();

    Console.WriteLine("Wait for a minute and then press any key to end the processing");
    Console.ReadKey();

    // stop processing
    Console.WriteLine("\nStopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}

static async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Mail to send: {body}");

    //complete the message. Messages is deleted from the queue.
    await args.CompleteMessageAsync(args.Message);
}

static Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}
