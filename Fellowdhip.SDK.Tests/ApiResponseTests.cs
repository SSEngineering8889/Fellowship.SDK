using System.Text.Json;
using Fellowship.SDK;
using Fellowship.SDK.Models;
using FluentAssertions;

namespace Fellowdhip.SDK.Tests
{
    [TestFixture]
    public class ApiResponseTests
    {
        private JsonSerializerOptions _jsonOptions;

        [SetUp]
        public void Setup()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Test]
        public void ApiResponse_DefaultConstructor_ShouldInitializeWithDefaults()
        {
            // Act
            var response = new ApiResponse<Movie>();

            // Assert
            response.Docs.Should().NotBeNull();
            response.Docs.Should().BeEmpty();
            response.Total.Should().Be(0);
            response.Limit.Should().Be(0);
            response.Offset.Should().Be(0);
            response.Page.Should().Be(0);
            response.Pages.Should().Be(0);
        }

        [Test]
        public void ApiResponse_Deserialization_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var json = """
            {
                "docs": [
                    {
                        "_id": "5cd95395de30eff6ebccde5c",
                        "name": "The Fellowship of the Ring",
                        "runtimeInMinutes": 178,
                        "rottenTomatoesScore": 91.5
                    }
                ],
                "total": 1,
                "limit": 10,
                "offset": 0,
                "page": 1,
                "pages": 1
            }
            """;

            // Act
            var response = JsonSerializer.Deserialize<ApiResponse<Movie>>(json, _jsonOptions);

            // Assert
            response.Should().NotBeNull();
            response!.Docs.Should().HaveCount(1);
            response.Docs.First().Name.Should().Be("The Fellowship of the Ring");
            response.Total.Should().Be(1);
            response.Limit.Should().Be(10);
            response.Offset.Should().Be(0);
            response.Page.Should().Be(1);
            response.Pages.Should().Be(1);
        }

        [Test]
        public void ApiResponse_Deserialization_WithEmptyDocs_ShouldWork()
        {
            // Arrange
            var json = """
            {
                "docs": [],
                "total": 0,
                "limit": 10,
                "offset": 0,
                "page": 1,
                "pages": 1
            }
            """;

            // Act
            var response = JsonSerializer.Deserialize<ApiResponse<Movie>>(json, _jsonOptions);

            // Assert
            response.Should().NotBeNull();
            response!.Docs.Should().BeEmpty();
            response.Total.Should().Be(0);
        }

        [Test]
        public void ApiResponse_Deserialization_WithQuotes_ShouldWork()
        {
            // Arrange
            var json = """
            {
                "docs": [
                    {
                        "_id": "5cd96e05de30eff6ebcce7e9",
                        "dialog": "Deagol!",
                        "character": "5cd99d4bde30eff6ebccfe9e",
                        "movie": "5cd95395de30eff6ebccde5d"
                    },
                    {
                        "_id": "5cd96e05de30eff6ebcce7ea",
                        "dialog": "Give us that! Deagol my love",
                        "character": "5cd99d4bde30eff6ebccfe9e",
                        "movie": "5cd95395de30eff6ebccde5d"
                    }
                ],
                "total": 2,
                "limit": 1000,
                "offset": 0,
                "page": 1,
                "pages": 1
            }
            """;

            // Act
            var response = JsonSerializer.Deserialize<ApiResponse<Quote>>(json, _jsonOptions);

            // Assert
            response.Should().NotBeNull();
            response!.Docs.Should().HaveCount(2);
            response.Docs.First().Dialog.Should().Be("Deagol!");
            response.Docs.Last().Dialog.Should().Be("Give us that! Deagol my love");
            response.Total.Should().Be(2);
            response.Limit.Should().Be(1000);
        }

        [Test]
        public void ApiResponse_Deserialization_WithMissingProperties_ShouldUseDefaults()
        {
            // Arrange
            var json = """
            {
                "docs": []
            }
            """;

            // Act
            var response = JsonSerializer.Deserialize<ApiResponse<Movie>>(json, _jsonOptions);

            // Assert
            response.Should().NotBeNull();
            response!.Docs.Should().BeEmpty();
            response.Total.Should().Be(0);
            response.Limit.Should().Be(0);
            response.Offset.Should().Be(0);
            response.Page.Should().Be(0);
            response.Pages.Should().Be(0);
        }
    }
}
