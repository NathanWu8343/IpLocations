using IpLocations.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace IpLocations.IntegrationTests.Abstractions
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly RedisContainer _redisContainer = new RedisBuilder()
            .WithImage("redis:7")
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                //redis
                services.RemoveAll(typeof(IConnectionMultiplexer));
                var redisConnection = _redisContainer.GetConnectionString()
                                      ?? throw new ArgumentNullException("RedisConnection");

                services.AddSingleton<IConnectionMultiplexer>(
                    ConnectionMultiplexer.Connect(redisConnection));
            });

            //builder.UseEnvironment("Test");
        }

        public Task InitializeAsync()
        {
            return Task.WhenAll(
                 _redisContainer.StartAsync());
        }

        public new Task DisposeAsync()
        {
            return Task.WhenAll(
                 _redisContainer.DisposeAsync().AsTask()
                 );
        }
    }
}