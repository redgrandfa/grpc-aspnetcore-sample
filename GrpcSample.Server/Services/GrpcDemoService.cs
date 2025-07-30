using Grpc.Core;
using GrpcSample.Server;
using static System.Net.Mime.MediaTypeNames;

namespace GrpcSample.Server.Services;

public class GrpcDemoService : GrpcDemo.GrpcDemoBase
{
    private readonly ILogger<GrpcDemoService> _logger;
    public GrpcDemoService(ILogger<GrpcDemoService> logger)
    {
        _logger = logger;
    }


    public override async Task<Message> Echo(Message request, ServerCallContext context)
    {
        Console.WriteLine(">>>>>>>>GrpcDemoService.Echo>>>>>>>>");
        Console.WriteLine($"Echo.Request: {request.Text}");
        await Task.Delay(2); // Simulate

        Console.WriteLine("<<<<<<<<GrpcDemoService.Echo<<<<<<<<");
        return new Message
        {
            Text = $"Echo: {request.Text}",
        };

    }

    public override async Task CountDown(
        CountDownRequest request,
        IServerStreamWriter<Message> responseStream,
        ServerCallContext context)
    {
        Console.WriteLine(">>>>>>>>GrpcDemoService.CountDown>>>>>>>>");
        Console.WriteLine($"CountDown.Request: None");

        for (int sec = 3; sec >= 0; sec--)
        {
            var text = $"{sec}...";
            Console.WriteLine($"CountDown.responseStream.Write: {text}");

            await responseStream.WriteAsync(
                new Message
                {
                    Text = text,
                }
            );
            await Task.Delay(1000); // Simulate
        }

        Console.WriteLine("<<<<<<<<GrpcDemoService.CountDown<<<<<<<<");
    }

    public override async Task EchoChat(
        IAsyncStreamReader<Message> requestStream,
        IServerStreamWriter<Message> responseStream,
        ServerCallContext context)
    {
        Console.WriteLine(">>>>>>>>GrpcDemoService.EchoChat>>>>>>>>");
        
        await foreach (var msg in requestStream.ReadAllAsync(context.CancellationToken))
        {

            Console.WriteLine($"EchoChat.requestStream.Read: {msg.Text}");
            var text = $"Echo: {msg.Text}";

            Console.WriteLine($"EchoChat.responseStream.Write: {text}");

            await responseStream.WriteAsync(
                new Message
                {
                    Text = text,
                }
            );
        }

        Console.WriteLine("<<<<<<<<GrpcDemoService.EchoChat<<<<<<<<");
    }
}
