using FluentAssertions;
using IpLocations.Api.Misc;

namespace IpLocations.UnitTests
{
    public class IpConverterTests
    {
        [Theory]
        [InlineData("192.168.0.1", 3232235521)]
        [InlineData("127.0.0.1", 2130706433)]
        [InlineData("10.0.0.1", 167772161)]
        [InlineData("0.0.0.0", 0)]
        [InlineData("255.255.255.255", 4294967295)]
        public void IpToInt_Should_ReturnExpectedValue_WhenIpIsValid(string ip, long expected)
        {
            // Act
            var result = IpConverter.IpToInt(ip);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("192.168.0")]
        [InlineData("127.0")]
        [InlineData("10")]
        [InlineData("127.0.0.-1")]
        [InlineData("127.0.-5.1")]
        [InlineData("127.-3.0.1")]
        [InlineData("-127.3.0.1")]
        [InlineData("255.255.256.255")]
        public void IpToInt_Should_ReturnNegativeOne_WhenIpFormatIsInvalid(string ip)
        {
            // Arrange
            var expected = -1;

            // Act
            var result = IpConverter.IpToInt(ip);

            // Assert
            result.Should().Be(expected);
        }
    }
}