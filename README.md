# Fellowship SDK - Lord of the Rings API Client

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET 8 SDK for accessing [The One API](https://the-one-api.dev/) - a comprehensive Lord of the Rings database containing information about movies, quotes, characters, and more.

## Features

- **Movies API**: Retrieve information about Lord of the Rings movies
- **Quotes API**: Access memorable quotes from the films
- **Advanced Filtering**: Support for complex filtering with multiple operators
- **Pagination**: Handle large datasets with built-in pagination support
- **Type Safety**: Strongly typed models with proper JSON serialization
- **Async/Await**: Full async support with cancellation tokens

## Installation

### From MyGet Package Feed

The Fellowship SDK is available as a NuGet package on MyGet. To install:

1. Add the MyGet package source to your project:
   ```bash
   dotnet nuget add source https://www.myget.org/F/fellowship-sdk/api/v3/index.json -n "Fellowship SDK"
   ```

2. Install the package:
   ```bash
   dotnet add package Fellowship.SDK
   ```

### From Source

1. Clone this repository:
   ```bash
   git clone <repository-url>
   cd Fellowship.SDK
   ```

2. Build the solution:
   ```bash
   dotnet build
   ```

3. Add a project reference to your application:
   ```xml
   <ProjectReference Include="path/to/Fellowship.SDK/Fellowship.SDK.csproj" />
   ```

## Quick Start

### 1. Get an API Key

Visit [The One API](https://the-one-api.dev/) to register and obtain your API key.

### 2. Basic Usage

#### Option A: Dependency Injection (Recommended)

```csharp
using Fellowship.SDK.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Setup DI container
var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole());
services.AddFellowshipSdk("your-api-key-here");

var serviceProvider = services.BuildServiceProvider();

// Get client from DI (with automatic logging)
var client = serviceProvider.GetRequiredService<IFellowshipClient>();

// Use the client
var movies = await client.Movies.GetAllAsync();
var quotes = await client.Quotes.GetAllAsync();
```

#### Option B: Manual Instantiation

```csharp
using Fellowship.SDK.Api;
using Fellowship.SDK.Filters;
using Fellowship.SDK.Models;

// Initialize the client with your API key
var client = new FellowshipClient("your-api-key-here");

// Get all movies
var movies = await client.Movies.GetAllAsync();

// Get all quotes
var quotes = await client.Quotes.GetAllAsync();

// Get a specific movie by ID
var movie = await client.Movies.GetByIdAsync("5cd95395de30eff6ebccde5c");

// Get quotes for a specific movie
var movieQuotes = await client.Movies.GetQuotesAsync("5cd95395de30eff6ebccde5c");
```

### 3. Advanced Filtering

The SDK supports powerful filtering capabilities:

```csharp
// Movies with runtime >= 160 minutes
var longMovies = await client.Movies.GetAllAsync(
    filters: new[] {
        new Filter<Movie>(m => m.RuntimeInMinutes, FilterOperator.GreaterThanOrEqual, "160")
    }
);

// Quotes mentioning "ring" (case-insensitive regex)
var ringQuotes = await client.Quotes.GetAllAsync(
    filters: new[] {
        new Filter<Quote>(q => q.Dialog, FilterOperator.Regex, "/ring/i")
    }
);

// Movies with specific Rotten Tomatoes score
var highRatedMovies = await client.Movies.GetAllAsync(
    filters: new[] {
        new Filter<Movie>(m => m.RottenTomatoesScore, FilterOperator.GreaterThan, "90")
    }
);
```

### 4. Pagination

```csharp

// Paginated results
var pagedQuotes = await client.Quotes.GetAllAsync(
    limit: 10,
    page: 1
);
```

## Available Filter Operators

- `Match` - Exact match
- `NotMatch` - Does not match
- `In` - Value is in a list
- `NotIn` - Value is not in a list
- `Exists` - Field exists
- `NotExists` - Field does not exist
- `Regex` - Regular expression match
- `NotRegex` - Regular expression does not match
- `GreaterThan` - Greater than
- `GreaterThanOrEqual` - Greater than or equal
- `LessThan` - Less than
- `LessThanOrEqual` - Less than or equal

## Models

### Movie
- `Id` - Unique identifier
- `Name` - Movie title
- `RuntimeInMinutes` - Duration in minutes
- `RottenTomatoesScore` - Critical rating

### Quote
- `Id` - Unique identifier
- `Dialog` - The quote text
- `Character` - Character who said the quote
- `Movie` - Movie the quote is from

## Running the Sample Application

The repository includes a sample console application (`lotrRunner`) that demonstrates the SDK usage:

1. Navigate to the runner directory:
   ```bash
   cd lotrRunner
   ```

2. Update the API key in `Program.cs` with your own key

3. Run the application:
   ```bash
   dotnet run
   ```

The sample application demonstrates:
- Filtering movies by runtime
- Searching quotes with regex

## Running Tests

The SDK includes comprehensive unit tests using NUnit, Moq, and FluentAssertions.

### Run All Tests
```bash
dotnet test
```

### Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run Specific Test Project
```bash
dotnet test Fellowdhip.SDK.Tests/Fellowdhip.SDK.Tests.csproj
```

### Test Categories

The test suite covers:
- **API Client Tests**: HTTP client behavior and error handling
- **Model Tests**: JSON serialization and deserialization
- **Filter Tests**: Filter construction and validation
- **Integration Tests**: End-to-end API communication

## Error Handling

The SDK includes custom exception handling:

```csharp
try
{
    var movies = await client.Movies.GetAllAsync();
}
catch (ApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Network Error: {ex.Message}");
}
```

## Development

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or Visual Studio Code

### Project Structure
```
Fellowship.SDK/
├── Fellowship.SDK/          # Main SDK library
│   ├── Api/                 # API clients
│   ├── Models/              # Data models
│   ├── Filters/             # Filtering system
├── Fellowdhip.SDK.Tests/    # Unit tests
├── lotrRunner/              # Sample application
└── Fellowship.SDK.sln       # Solution file
```

### Building from Source
```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Create NuGet package
dotnet pack Fellowship.SDK/Fellowship.SDK.csproj -c Release
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## API Reference

For complete API documentation, visit [The One API Documentation](https://the-one-api.dev/documentation).

## Support

For issues, questions, or contributions, please visit the project repository or contact the maintainer.
