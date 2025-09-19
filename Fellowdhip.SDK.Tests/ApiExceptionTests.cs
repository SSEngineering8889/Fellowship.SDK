using Fellowship.SDK;
using FluentAssertions;

namespace Fellowdhip.SDK.Tests
{
    [TestFixture]
    public class ApiExceptionTests
    {
        [Test]
        public void ApiException_Constructor_ShouldSetStatusCodeAndMessage()
        {
            // Arrange
            const int statusCode = 404;
            const string errorMessage = "Not Found";

            // Act
            var exception = new ApiException(statusCode, errorMessage);

            // Assert
            exception.StatusCode.Should().Be(statusCode);
            exception.Message.Should().Be("API Error 404: Not Found");
        }

        [Test]
        public void ApiException_Constructor_ShouldHandleEmptyMessage()
        {
            // Arrange
            const int statusCode = 500;
            const string errorMessage = "";

            // Act
            var exception = new ApiException(statusCode, errorMessage);

            // Assert
            exception.StatusCode.Should().Be(statusCode);
            exception.Message.Should().Be("API Error 500: ");
        }

        [Test]
        public void ApiException_Constructor_ShouldHandleNullMessage()
        {
            // Arrange
            const int statusCode = 401;
            const string? errorMessage = null;

            // Act
            var exception = new ApiException(statusCode, errorMessage!);

            // Assert
            exception.StatusCode.Should().Be(statusCode);
            exception.Message.Should().Be("API Error 401: ");
        }

        [Test]
        public void ApiException_ShouldInheritFromException()
        {
            // Arrange & Act
            var exception = new ApiException(400, "Bad Request");

            // Assert
            exception.Should().BeAssignableTo<Exception>();
        }

        [Test]
        public void ApiException_ShouldBeThrowable()
        {
            // Arrange
            var exception = new ApiException(403, "Forbidden");

            // Act & Assert
            Action act = () => throw exception;
            act.Should().Throw<ApiException>()
               .Which.StatusCode.Should().Be(403);
        }
    }
}
