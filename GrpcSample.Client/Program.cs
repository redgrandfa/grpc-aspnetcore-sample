using Microsoft.Extensions.DependencyInjection;
using GrpcSample.ClientSDK;

Console.WriteLine( "Client started.");

var services = new ServiceCollection(); 
services.AddGrpcSDK();
var provider = services.BuildServiceProvider();

var grpcService = provider.GetRequiredService<GrpcDemoClientService>();

await grpcService.RunEchoAsync("Orz", CancellationToken.None);
await Task.Delay(11111);
await grpcService.RunCountDownAsync(CancellationToken.None);
await Task.Delay(11111);
await grpcService.RunEchoChatAsync(CancellationToken.None);


Console.ReadLine();