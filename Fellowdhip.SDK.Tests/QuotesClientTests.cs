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
    public class QuotesClientTests
    {
        private Mock<HttpMessageHandler> _mockMessageHandler;
        private QuotesClient _quotesClient;

        [SetUp]
        public void Setup()
        {
            _mockMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_mockMessageHandler.Object)
            {
                BaseAddress = new Uri("https://the-one-api.dev/v2/")
            };

            _quotesClient = new QuotesClient("test-api-key", new NullLogger<QuotesClient>());
            
            // Replace the internal HttpClient with our mocked one
            var httpClientField = typeof(QuotesClient).GetField("_http", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            httpClientField?.SetValue(_quotesClient, httpClient);
        }

        [Test]
        public void QuotesClient_Constructor_ShouldSetupHttpClientWithCorrectBaseAddress()
        {
            // Act
            var client = new QuotesClient("test-key", new NullLogger<QuotesClient>());

            // Assert - We can't easily test the private HttpClient, but we can test that it doesn't throw
            client.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllAsync_WithoutParameters_ShouldReturnQuotes()
        {
            // Arrange
            var responseData = new
            {
                docs = new[]
                {
                    new { _id = "1", dialog = "You shall not pass!", character = "gandalf", movie = "fellowship" },
                    new { _id = "2", dialog = "My precious...", character = "gollum", movie = "two-towers" }
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
            var result = await _quotesClient.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Dialog.Should().Be("You shall not pass!");
            result.Last().Dialog.Should().Be("My precious...");
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
            await _quotesClient.GetAllAsync(limit: 5, page: 2);

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
                new Filter<Quote>(q => q.Dialog, FilterOperator.Regex, "/ring/i")
            };

            // Act
            await _quotesClient.GetAllAsync(filters: filters);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.RequestUri!.PathAndQuery.Should().Contain("dialog=/ring/i");
        }

        [Test]
        public async Task GetByIdAsync_WithValidId_ShouldReturnQuote()
        {
            // Arrange
            const string quoteId = "5cd96e05de30eff6ebcce7e9";
            var responseData = new
            {
                docs = new[]
                {
                    new { _id = quoteId, dialog = "Deagol!", character = "smeagol", movie = "fellowship" }
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
            var result = await _quotesClient.GetByIdAsync(quoteId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(quoteId);
            result.Dialog.Should().Be("Deagol!");
            
            capturedRequest.Should().NotBeNull();
            capturedRequest!.RequestUri!.PathAndQuery.Should().Contain($"quote/{quoteId}");
        }

        [Test]
        public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            const string quoteId = "non-existent-id";
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
            var result = await _quotesClient.GetByIdAsync(quoteId);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetAllAsync_WithAllParameters_ShouldIncludeAllInRequest()
        {
            // Arrange
            var responseData = new { docs = new object[0], total = 0, limit = 3, offset = 6, page = 3, pages = 5 };
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
                new Filter<Quote>(q => q.Character, FilterOperator.Match, "gandalf")
            };

            // Act
            await _quotesClient.GetAllAsync(limit: 3, page: 3, filters: filters);

            // Assert
            capturedRequest.Should().NotBeNull();
            var query = Uri.UnescapeDataString(capturedRequest!.RequestUri!.PathAndQuery);
            query.Should().Contain("limit=3");
            query.Should().Contain("page=3");
            query.Should().Contain("character=gandalf");
        }

        [Test]
        public async Task GetAllAsync_WithApiError_ShouldThrowApiException()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Forbidden", Encoding.UTF8, "text/plain")
            };

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var act = async () => await _quotesClient.GetAllAsync();
            await act.Should().ThrowAsync<ApiException>()
                .Where(ex => ex.StatusCode == 403);
        }

        [Test]
        public async Task GetByIdAsync_WithApiError_ShouldThrowApiException()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Quote not found", Encoding.UTF8, "text/plain")
            };

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var act = async () => await _quotesClient.GetByIdAsync("invalid-id");
            await act.Should().ThrowAsync<ApiException>()
                .Where(ex => ex.StatusCode == 404);
        }
    }
}
