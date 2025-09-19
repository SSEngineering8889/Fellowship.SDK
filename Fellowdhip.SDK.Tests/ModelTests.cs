using System.Text.Json;
using Fellowship.SDK.Models;
using FluentAssertions;

namespace Fellowdhip.SDK.Tests
{
    [TestFixture]
    public class ModelTests
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
        public void Movie_Serialization_ShouldMapJsonPropertiesCorrectly()
        {
            // Arrange
            var json = """
            {
                "_id": "5cd95395de30eff6ebccde5c",
                "name": "The Fellowship of the Ring",
                "runtimeInMinutes": 178,
                "rottenTomatoesScore": 91.5
            }
            """;

            // Act
            var movie = JsonSerializer.Deserialize<Movie>(json, _jsonOptions);

            // Assert
            movie.Should().NotBeNull();
            movie!.Id.Should().Be("5cd95395de30eff6ebccde5c");
            movie.Name.Should().Be("The Fellowship of the Ring");
            movie.RuntimeInMinutes.Should().Be(178);
            movie.RottenTomatoesScore.Should().Be(91.5);
        }

        [Test]
        public void Movie_Deserialization_ShouldHandleDefaultValues()
        {
            // Arrange
            var json = """
            {
                "_id": "test-id",
                "name": "Test Movie"
            }
            """;

            // Act
            var movie = JsonSerializer.Deserialize<Movie>(json, _jsonOptions);

            // Assert
            movie.Should().NotBeNull();
            movie!.Id.Should().Be("test-id");
            movie.Name.Should().Be("Test Movie");
            movie.RuntimeInMinutes.Should().Be(0);
            movie.RottenTomatoesScore.Should().Be(0.0);
        }

        [Test]
        public void Quote_Serialization_ShouldMapJsonPropertiesCorrectly()
        {
            // Arrange
            var json = """
            {
                "_id": "5cd96e05de30eff6ebcce7e9",
                "dialog": "Deagol!",
                "character": "5cd99d4bde30eff6ebccfe9e",
                "movie": "5cd95395de30eff6ebccde5d"
            }
            """;

            // Act
            var quote = JsonSerializer.Deserialize<Quote>(json, _jsonOptions);

            // Assert
            quote.Should().NotBeNull();
            quote!.Id.Should().Be("5cd96e05de30eff6ebcce7e9");
            quote.Dialog.Should().Be("Deagol!");
            quote.Character.Should().Be("5cd99d4bde30eff6ebccfe9e");
            quote.Movie.Should().Be("5cd95395de30eff6ebccde5d");
        }

        [Test]
        public void Quote_Deserialization_ShouldHandleEmptyValues()
        {
            // Arrange
            var json = """
            {
                "_id": "test-quote-id",
                "dialog": "",
                "character": "",
                "movie": ""
            }
            """;

            // Act
            var quote = JsonSerializer.Deserialize<Quote>(json, _jsonOptions);

            // Assert
            quote.Should().NotBeNull();
            quote!.Id.Should().Be("test-quote-id");
            quote.Dialog.Should().Be("");
            quote.Character.Should().Be("");
            quote.Movie.Should().Be("");
        }
    }
}