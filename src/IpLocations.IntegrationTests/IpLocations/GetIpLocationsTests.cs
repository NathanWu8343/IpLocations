using FluentAssertions;
using IpLocations.IntegrationTests.Abstractions;
using System.Net;

namespace IpLocations.IntegrationTests.IpLocations
{
    public class GetIpLocationsTests : BaseIntegrationTest
    {
        public GetIpLocationsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("1.160.0.0", "TW")]
        public async Task Create_Should_ReturnOk_WhenRequesIsValid(string ip, string expected)
        {
            // Arrange

            // Act
            HttpResponseMessage response = await HttpClientStub.GetAsync(requestUri: $"api/IpLocations?ip={ip}");

            // Assert (HTTP)
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert (HTTP Content Response)
            var result = await response.Content.ReadAsStringAsync();
            result.Should().Be(expected);
        }
    }
}