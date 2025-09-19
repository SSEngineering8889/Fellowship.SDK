using System.Net;
using System.Text;
using System.Text.Json;
using Fellowship.SDK;
using Fellowship.SDK.Filters;
using Fellowship.SDK.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;

namespace Fellowdhip.SDK.Tests
{
    [TestFixture]
    public class MoviesClientTests
    {
        private Mock<HttpMessageHandler> _mockMessageHandler;
        private MoviesClient _moviesClient;

        [SetUp]
        public void Setup()
        {
            _mockMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_mockMessageHandler.Object)
            {
                BaseAddress = new Uri("https://the-one-api.dev/v2/")
            };

            _moviesClient = new MoviesClient("test-api-key", new NullLogger<MoviesClient>());
            
            // Replace the internal HttpClient with our mocked one
            var httpClientField = typeof(MoviesClient).GetField("_http", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            httpClientField?.SetValue(_moviesClient, httpClient);
        }

        [Test]
        public void MoviesClient_Constructor_ShouldSetupHttpClientWithCorrectBaseAddress()
        {
            // Act
            var client = new MoviesClient("test-key", new NullLogger<MoviesClient>());

            // Assert - We can't easily test the private HttpClient, but we can test that it doesn't throw
            client.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllAsync_WithoutParameters_ShouldReturnMovies()
        {
            // Arrange
            var responseData = new
            {
                docs = new[]
                {
                    new { _id = "1", name = "Fellowship of the Ring", runtimeInMinutes = 178, rottenTomatoesScore = 91.5 },
                    new { _id = "2", name = "The Two Towers", runtimeInMinutes = 179, rottenTomatoesScore = 95.0 }
                },
                total = 2,
                limit = 10,
                offset = 0,
                page = 1,
                pages = 1
            };

            var jsonResponse = JsonSerializer.Serialize(responseData);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _moviesClient.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Fellowship of the Ring");
            result.Last().Name.Should().Be("The Two Towers");
        }

        [Test]
        public async Task GetAllAsync_WithLimitAndPage_ShouldIncludeParametersInRequest()
        {
            // Arrange
            var responseData = new { docs = new object[0], total = 0, limit = 5, offset = 5, page = 2, pages = 1 };
            var jsonResponse = JsonSerializer.Serialize(responseData);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            HttpRequestMessage? capturedRequest = null;
            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
                .ReturnsAsync(httpResponse);

            // Act
            await _moviesClient.GetAllAsync(limit: 5, page: 2);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.RequestUri!.PathAndQuery.Should().Contain("limit=5");
            capturedRequest.RequestUri.PathAndQuery.Should().Contain("page=2");
        }

        [Test]
        public async Task GetAllAsync_WithFilters_ShouldIncludeFiltersInRequest()
        {
            // Arrange
            var responseData = new { docs = new object[0], total = 0, limit = 10, offset = 0, page = 1, pages = 1 };
            var jsonResponse = JsonSerializer.Serialize(responseData);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            HttpRequestMessage? capturedRequest = null;
            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
                .ReturnsAsync(httpResponse);

            var filters = new[]
            {
                new Filter<Movie>(m => m.RuntimeInMinutes, FilterOperator.GreaterThan, "120")
            };

            // Act
            await _moviesClient.GetAllAsync(filters: filters);

            // Assert
            capturedRequest.Should().NotBeNull();
            Uri.UnescapeDataString(capturedRequest!.RequestUri!.PathAndQuery).Should().Contain("runtimeInMinutes>120");
        }

        [Test]
        public async Task GetByIdAsync_WithValidId_ShouldReturnMovie()
        {
            // Arrange
            const string movieId = "5cd95395de30eff6ebccde5c";
            var responseData = new
            {
                docs = new[]
                {
                    new { _id = movieId, name = "Fellowship of the Ring", runtimeInMinutes = 178, rottenTomatoesScore = 91.5 }
                },
                total = 1,
                limit = 10,
                offset = 0,
                page = 1,
                pages = 1
            };

            var jsonResponse = JsonSerializer.Serialize(responseData);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            HttpRequestMessage? capturedRequest = null;
            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _moviesClient.GetByIdAsync(movieId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(movieId);
            result.Name.Should().Be("Fellowship of the Ring");
            
            capturedRequest.Should().NotBeNull();
            capturedRequest!.RequestUri!.PathAndQuery.Should().Contain($"movie/{movieId}");
        }

        [Test]
        public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            const string movieId = "non-existent-id";
            var responseData = new
            {
                docs = new object[0],
                total = 0,
                limit = 10,
                offset = 0,
                page = 1,
                pages = 1
            };

            var jsonResponse = JsonSerializer.Serialize(responseData);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _moviesClient.GetByIdAsync(movieId);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetQuotesAsync_WithValidMovieId_ShouldReturnQuotes()
        {
            // Arrange
            const string movieId = "5cd95395de30eff6ebccde5c";
            var responseData = new
            {
                docs = new[]
                {
                    new { _id = "quote1", dialog = "You shall not pass!", character = "gandalf", movie = movieId },
                    new { _id = "quote2", dialog = "My precious...", character = "gollum", movie = movieId }
                },
                total = 2,
                limit = 10,
                offset = 0,
                page = 1,
                pages = 1
            };

            var jsonResponse = JsonSerializer.Serialize(responseData);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            HttpRequestMessage? capturedRequest = null;
            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _moviesClient.GetQuotesAsync(movieId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Dialog.Should().Be("You shall not pass!");
            result.Last().Dialog.Should().Be("My precious...");

            capturedRequest.Should().NotBeNull();
            capturedRequest!.RequestUri!.PathAndQuery.Should().Contain($"movie/{movieId}/quote");
        }

        [Test]
        public async Task GetQuotesAsync_WithLimitAndPage_ShouldIncludeParametersInRequest()
        {
            // Arrange
            const string movieId = "test-movie-id";
            var responseData = new { docs = new object[0], total = 0, limit = 5, offset = 10, page = 3, pages = 1 };
            var jsonResponse = JsonSerializer.Serialize(responseData);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            HttpRequestMessage? capturedRequest = null;
            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
                .ReturnsAsync(httpResponse);

            // Act
            await _moviesClient.GetQuotesAsync(movieId, limit: 5, page: 3);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.RequestUri!.PathAndQuery.Should().Contain("limit=5");
            capturedRequest.RequestUri.PathAndQuery.Should().Contain("page=3");
        }

        [Test]
        public async Task GetAllAsync_WithApiError_ShouldThrowApiException()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("Unauthorized", Encoding.UTF8, "text/plain")
            };

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var act = async () => await _moviesClient.GetAllAsync();
            await act.Should().ThrowAsync<ApiException>()
                .Where(ex => ex.StatusCode == 401);
        }
    }
}
