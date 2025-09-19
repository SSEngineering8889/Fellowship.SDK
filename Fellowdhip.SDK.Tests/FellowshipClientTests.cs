using Fellowship.SDK.Api;
using Fellowship.SDK.Interfaces;
using FluentAssertions;
using Moq;

namespace Fellowdhip.SDK.Tests
{
    [TestFixture]
    public class FellowshipClientTests
    {
        private Mock<IMoviesClient> _mockMoviesClient;
        private Mock<IQuotesClient> _mockQuotesClient;

        [SetUp]
        public void Setup()
        {
            _mockMoviesClient = new Mock<IMoviesClient>();
            _mockQuotesClient = new Mock<IQuotesClient>();
        }

        [Test]
        public void FellowshipClient_ShouldInitializeClients()
        {
            // Act
            var client = new FellowshipClient(_mockMoviesClient.Object, _mockQuotesClient.Object);

            // Assert
            client.Movies.Should().NotBeNull();
            client.Movies.Should().Be(_mockMoviesClient.Object);
            client.Quotes.Should().NotBeNull();
            client.Quotes.Should().Be(_mockQuotesClient.Object);
        }
    }
}
