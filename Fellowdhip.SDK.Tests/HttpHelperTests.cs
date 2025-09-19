using System.Net;
using System.Text;
using System.Text.Json;
using Fellowship.SDK;
using Fellowship.SDK.Filters;
using Fellowship.SDK.Models;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace Fellowdhip.SDK.Tests
{
    [TestFixture]
    public class HttpHelperTests
    {
        private Mock<HttpMessageHandler> _mockMessageHandler;
        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            _mockMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockMessageHandler.Object)
            {
                BaseAddress = new Uri("https://api.test.com/")
            };
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }

        [Test]
        public async Task GetAsync_WithSuccessfulResponse_ShouldReturnDeserializedData()
        {
            // Arrange
            var responseData = new
            {
                docs = new[]
                {
                    new { _id = "1", name = "Test Movie", runtimeInMinutes = 120, rottenTomatoesScore = 85.0 }
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

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await HttpHelper.GetAsync<Movie>(_httpClient, "movie");

            // Assert
            result.Should().NotBeNull();
            result.Docs.Should().HaveCount(1);
            result.Docs.First().Id.Should().Be("1");
            result.Docs.First().Name.Should().Be("Test Movie");
            result.Total.Should().Be(1);
        }

        [Test]
        public async Task GetAsync_WithErrorResponse_ShouldThrowApiException()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Resource not found", Encoding.UTF8, "text/plain")
            };

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var act = async () => await HttpHelper.GetAsync<Movie>(_httpClient, "movie");
            await act.Should().ThrowAsync<ApiException>()
                .Where(ex => ex.StatusCode == 404);
        }

        [Test]
        public async Task GetAsync_WithLimit_ShouldIncludeLimitInUrl()
        {
            // Arrange
            var responseData = new { docs = new object[0], total = 0, limit = 5, offset = 0, page = 1, pages = 1 };
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
            await HttpHelper.GetAsync<Movie>(_httpClient, "movie", limit: 5);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.RequestUri!.Query.Should().Contain("limit=5");
        }

        [Test]
        public async Task GetAsync_WithPage_ShouldIncludePageInUrl()
        {
            // Arrange
            var responseData = new { docs = new object[0], total = 0, limit = 10, offset = 10, page = 2, pages = 1 };
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
            await HttpHelper.GetAsync<Movie>(_httpClient, "movie", page: 2);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.RequestUri!.Query.Should().Contain("page=2");
        }

        [Test]
        public async Task GetAsync_WithFilters_ShouldIncludeFiltersInUrl()
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
                new Filter<Movie>(m => m.Name, FilterOperator.Match, "Fellowship"),
                new Filter<Movie>(m => m.RuntimeInMinutes, FilterOperator.GreaterThan, "120")
            };

            // Act
            await HttpHelper.GetAsync<Movie>(_httpClient, "movie", filters: filters);

            // Assert
            capturedRequest.Should().NotBeNull();
            var query = Uri.UnescapeDataString(capturedRequest!.RequestUri!.Query);
            query.Should().Contain("name=Fellowship");
            query.Should().Contain("runtimeInMinutes>120");
        }

        [Test]
        public async Task GetAsync_WithAllParameters_ShouldIncludeAllInUrl()
        {
            // Arrange
            var responseData = new { docs = new object[0], total = 0, limit = 5, offset = 5, page = 2, pages = 2 };
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

            var filters = new[] { new Filter<Movie>(m => m.Name, FilterOperator.Match, "Test") };

            // Act
            await HttpHelper.GetAsync<Movie>(_httpClient, "movie", limit: 5, page: 2, filters: filters);

            // Assert
            capturedRequest.Should().NotBeNull();
            var query = capturedRequest!.RequestUri!.Query;
            query.Should().Contain("limit=5");
            query.Should().Contain("page=2");
            query.Should().Contain("name=Test");
        }
    }
}
