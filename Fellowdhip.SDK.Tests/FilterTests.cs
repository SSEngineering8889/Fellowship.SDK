using Fellowship.SDK.Filters;
using Fellowship.SDK.Models;
using FluentAssertions;

namespace Fellowdhip.SDK.Tests
{
    [TestFixture]
    public class FilterTests
    {
        [Test]
        public void Filter_Constructor_ShouldExtractJsonPropertyName()
        {
            // Act
            var filter = new Filter<Movie>(m => m.Name, FilterOperator.Match, "test");

            // Assert
            filter.Field.Should().Be("name");
            filter.Operator.Should().Be(FilterOperator.Match);
            filter.Value.Should().Be("test");
        }

        [Test]
        public void Filter_Constructor_ShouldHandleIdProperty()
        {
            // Act
            var filter = new Filter<Movie>(m => m.Id, FilterOperator.Match, "test-id");

            // Assert
            filter.Field.Should().Be("_id");
        }

        [Test]
        public void Filter_ToString_Match_ShouldFormatCorrectly()
        {
            // Arrange
            var filter = new Filter<Movie>(m => m.Name, FilterOperator.Match, "Fellowship");

            // Act
            var result = filter.ToString();

            // Assert
            result.Should().Be("name=Fellowship");
        }

        [Test]
        public void Filter_ToString_NotMatch_ShouldFormatCorrectly()
        {
            // Arrange
            var filter = new Filter<Movie>(m => m.Name, FilterOperator.NotMatch, "Hobbit");

            // Act
            var result = filter.ToString();

            // Assert
            result.Should().Be("name!=Hobbit");
        }

        [Test]
        public void Filter_ToString_GreaterThan_ShouldFormatCorrectly()
        {
            // Arrange
            var filter = new Filter<Movie>(m => m.RuntimeInMinutes, FilterOperator.GreaterThan, "120");

            // Act
            var result = filter.ToString();

            // Assert
            result.Should().Be("runtimeInMinutes>120");
        }

        [Test]
        public void Filter_ToString_GreaterThanOrEqual_ShouldFormatCorrectly()
        {
            // Arrange
            var filter = new Filter<Movie>(m => m.RuntimeInMinutes, FilterOperator.GreaterThanOrEqual, "120");

            // Act
            var result = filter.ToString();

            // Assert
            result.Should().Be("runtimeInMinutes>=120");
        }

        [Test]
        public void Filter_ToString_LessThan_ShouldFormatCorrectly()
        {
            // Arrange
            var filter = new Filter<Movie>(m => m.RuntimeInMinutes, FilterOperator.LessThan, "180");

            // Act
            var result = filter.ToString();

            // Assert
            result.Should().Be("runtimeInMinutes<180");
        }

        [Test]
        public void Filter_ToString_LessThanOrEqual_ShouldFormatCorrectly()
        {
            // Arrange
            var filter = new Filter<Movie>(m => m.RuntimeInMinutes, FilterOperator.LessThanOrEqual, "180");

            // Act
            var result = filter.ToString();

            // Assert
            result.Should().Be("runtimeInMinutes<=180");
        }

        [Test]
        public void Filter_ToString_Exists_ShouldFormatCorrectly()
        {
            // Arrange
            var filter = new Filter<Movie>(m => m.Name, FilterOperator.Exists);

            // Act
            var result = filter.ToString();

            // Assert
            result.Should().Be("name");
        }

        [Test]
        public void Filter_ToString_NotExists_ShouldFormatCorrectly()
        {
            // Arrange
            var filter = new Filter<Movie>(m => m.Name, FilterOperator.NotExists);

            // Act
            var result = filter.ToString();

            // Assert
            result.Should().Be("!name");
        }

        [Test]
        public void Filter_ToString_Regex_ShouldFormatCorrectly()
        {
            // Arrange
            var filter = new Filter<Quote>(q => q.Dialog, FilterOperator.Regex, "/ring/i");

            // Act
            var result = filter.ToString();

            // Assert
            result.Should().Be("dialog=/ring/i");
        }

        [Test]
        public void Filter_ToString_NotRegex_ShouldFormatCorrectly()
        {
            // Arrange
            var filter = new Filter<Quote>(q => q.Dialog, FilterOperator.NotRegex, "/ring/i");

            // Act
            var result = filter.ToString();

            // Assert
            result.Should().Be("dialog!=/ring/i");
        }

        [Test]
        public void Filter_Constructor_WithValidRegex_ShouldNotThrow()
        {
            // Act & Assert
            var act = () => new Filter<Quote>(q => q.Dialog, FilterOperator.Regex, "/test/i");
            act.Should().NotThrow();
        }

        [Test]
        public void Filter_Constructor_WithInvalidRegex_ShouldThrowArgumentException()
        {
            // Act & Assert
            var act = () => new Filter<Quote>(q => q.Dialog, FilterOperator.Regex, "[invalid");
            act.Should().Throw<ArgumentException>()
               .WithMessage("Invalid regex pattern: [invalid*");
        }

        [Test]
        public void Filter_Constructor_WithRegexWithoutSlashes_ShouldWork()
        {
            // Act & Assert
            var act = () => new Filter<Quote>(q => q.Dialog, FilterOperator.Regex, "test.*");
            act.Should().NotThrow();
        }

        [Test]
        public void Filter_ToString_In_ShouldFormatCorrectly()
        {
            // Arrange
            var filter = new Filter<Movie>(m => m.Name, FilterOperator.In, "Fellowship,Two Towers");

            // Act
            var result = filter.ToString();

            // Assert
            result.Should().Be("name=Fellowship,Two Towers");
        }

        [Test]
        public void Filter_ToString_NotIn_ShouldFormatCorrectly()
        {
            // Arrange
            var filter = new Filter<Movie>(m => m.Name, FilterOperator.NotIn, "Hobbit");

            // Act
            var result = filter.ToString();

            // Assert
            result.Should().Be("name!=Hobbit");
        }

        [Test]
        public void Filter_Constructor_WithInvalidFieldSelector_ShouldThrowArgumentException()
        {
            // Act & Assert
            var act = () => new Filter<Movie>(m => "constant", FilterOperator.Match, "test");
            act.Should().Throw<ArgumentException>()
               .WithMessage("Invalid field selector*");
        }
    }
}
