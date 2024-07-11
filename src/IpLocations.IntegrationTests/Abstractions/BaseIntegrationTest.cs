using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IpLocations.IntegrationTests.Abstractions
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly HttpClient HttpClientStub;

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            // basic Arrange
            var serviceScope = factory.Services.CreateScope();

            // TODO
            _serviceProvider = serviceScope.ServiceProvider;
            HttpClientStub = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}