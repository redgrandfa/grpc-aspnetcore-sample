using Microsoft.Extensions.DependencyInjection;

namespace GrpcSample.ClientSDK
{
    public static class ServiceCollectionExtension
    {
        public static void AddGrpcSDK(this IServiceCollection services)
        {
            // IServiceCollection.AddGrpcClient comes from the Client Factory package.
            // Behind the scenes, it adds an HttpClient via the HttpClient Factory to call the server.
            services.AddGrpcClient<GrpcDemo.GrpcDemoClient>(client =>
            {
                client.Address = new Uri("https://localhost:7227"); // grpc server url: port
            });
            services.AddScoped<GrpcDemoClientService, GrpcDemoClientService>();
        }
    }
}
