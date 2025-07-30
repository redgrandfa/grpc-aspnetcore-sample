using Grpc.Core;

namespace GrpcSample.ClientSDK
{
    public class GrpcDemoClientService
    {
        /// <summary>
        /// Get a Client instance via Dipendency Injection.
        /// </summary>
        private readonly GrpcDemo.GrpcDemoClient grpcClient;

        public GrpcDemoClientService(GrpcDemo.GrpcDemoClient grpcClient)
        {
            this.grpcClient = grpcClient;
        }

        /// <summary>
        /// Get a Client instance Manually.
        /// </summary>
        public static GrpcDemo.GrpcDemoClient NewClient()
        {
            var channel = Grpc.Net.Client.GrpcChannel.ForAddress("https://localhost:7227"); // grpc server url: port
            var client = new GrpcDemo.GrpcDemoClient(channel);
            return client;
        }

        public async Task RunEchoAsync(string text, CancellationToken cancellationToken)
        {
            Console.WriteLine(">>>>>>>>GrpcDemoClientService.RunEchoAsync>>>>>>>>");

            try
            {
                var response = await grpcClient.EchoAsync(
                    new Message
                    {
                        Text = text,
                    }
                    , cancellationToken: cancellationToken
                );
                Console.WriteLine(response.Text);
            }
            catch(Exception ex){
                // SocketException
                // RpcException
                throw;
            }

            Console.WriteLine("<<<<<<<<GrpcDemoClientService.RunEchoAsync<<<<<<<<");
        }

        public async Task RunCountDownAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine(">>>>>>>>GrpcDemoClientService.RunCountDownAsync>>>>>>>>");

            using var call = grpcClient.CountDown(new CountDownRequest());

            while (await call.ResponseStream.MoveNext(cancellationToken))
            {
                var msg = call.ResponseStream.Current;
                Console.WriteLine( msg.Text);
            }

            Console.WriteLine("<<<<<<<<GrpcDemoClientService.RunCountDownAsync<<<<<<<<");
        }

        public async Task RunEchoChatAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine(">>>>>>>>GrpcDemoClientService.RunEchoChatAsync>>>>>>>>");

            using var call = grpcClient.EchoChat();

            var sendTask = Task.Run(async () =>
            {
                for (int i = 3; i >= 0; i--)
                {
                    await call.RequestStream.WriteAsync(new Message
                    {
                        Text = $"{i}",
                    });
                    Console.WriteLine($"[Client --> Server]: {i}");

                    await Task.Delay(500); // simulate
                }
                await call.RequestStream.CompleteAsync();
            });

            var receiveTask = Task.Run(async () =>
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine($"[Client <-- Server]: {response.Text}");
                }
            });

            await Task.WhenAll(sendTask, receiveTask);

            Console.WriteLine("<<<<<<<<GrpcDemoClientService.RunEchoChatAsync<<<<<<<<");
        }
    }
}
