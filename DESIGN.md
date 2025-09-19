# Fellowship SDK - Design Specification

## Overview

The Fellowship SDK is a .NET 8 client library designed to provide type-safe, async access to [The One API](https://the-one-api.dev/). The SDK abstracts the complexity of HTTP communication and provides a fluent, strongly-typed interface for accessing Lord of the Rings data including movies, quotes, characters, and more.

## Architecture

### High-Level Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  Client App     │───▶│ Fellowship SDK  │───▶│  The One API    │
│                 │    │                 │    │                 │
│ - lotrRunner    │    │ - FellowshipClient    │ - REST Endpoints│
│ - User Apps     │    │ - MoviesClient  │    │ - JSON Responses│
│                 │    │ - QuotesClient  │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

### Component Architecture

```
FellowshipClient
├── MoviesClient
│   ├── GetAllAsync()
│   ├── GetByIdAsync()
│   └── GetQuotesAsync()
└── QuotesClient
    ├── GetAllAsync()
    └── GetByIdAsync()

Supporting Components:
├── HttpHelper (Internal)
├── Filter System
├── Sorting System
├── Models
└── Exception Handling
```

## Core Components

### 1. FellowshipClient (Entry Point)

**Purpose**: Main client facade that provides access to all API endpoints.

**Design Principles**:
- Single point of entry for all API operations
- Encapsulates authentication (API key)
- Provides strongly-typed sub-clients

**Implementation**:
```csharp
public class FellowshipClient
{
    public MoviesClient Movies { get; }
    public QuotesClient Quotes { get; }
    
    public FellowshipClient(string apiKey)
    {
        Movies = new MoviesClient(apiKey);
        Quotes = new QuotesClient(apiKey);
    }
}
```

**Key Design Decisions**:
- Composition over inheritance for sub-clients
- Constructor injection of API key
- Immutable client instances

### 2. API Clients (MoviesClient, QuotesClient)

**Purpose**: Specialized clients for specific API endpoints.

**Design Principles**:
- Single Responsibility: Each client handles one resource type
- Consistent method signatures across clients
- Full async/await support with cancellation tokens

**Common Interface Pattern**:
```csharp
Task<IEnumerable<TModel>> GetAllAsync(
    int? limit = null,
    int? page = null,
    string? sortField = null,
    string? sortOrder = null,
    IEnumerable<Filter<TModel>>? filters = null,
    CancellationToken ct = default)

Task<TModel?> GetByIdAsync(string id, CancellationToken ct = default)
```

**Key Design Decisions**:
- Optional parameters for flexibility
- Generic return types for type safety
- Nullable return types for not-found scenarios
- CancellationToken support for responsive applications

### 3. HttpHelper (Internal Utility)

**Purpose**: Centralized HTTP communication logic.

**Design Principles**:
- Internal visibility (implementation detail)
- Generic design for reusability
- Comprehensive error handling
- URL building abstraction

**Key Features**:
- JSON serialization/deserialization with case-insensitive options
- Query parameter building
- HTTP status code to exception mapping
- Stream-based deserialization for memory efficiency

**Implementation Details**:
```csharp
internal static class HttpHelper
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public static async Task<ApiResponse<TModel>> GetAsync<TModel>(...)
}
```

### 4. Filter System

**Purpose**: Type-safe, fluent filtering of API results.

**Design Principles**:
- Compile-time type safety using Expression trees
- Fluent API design
- Comprehensive operator support
- JSON property name mapping

**Architecture**:
```
Filter<TModel>
├── Expression<Func<TModel, object>> fieldSelector
├── FilterOperator operator
├── string? value
└── Validation Logic
    ├── Regex validation
    └── JSON property name resolution
```

**Supported Operators**:
- **Equality**: Match, NotMatch
- **Set Operations**: In, NotIn
- **Existence**: Exists, NotExists  
- **Pattern Matching**: Regex, NotRegex
- **Comparison**: GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual

**Key Design Decisions**:
- Expression trees for compile-time field validation
- Automatic JSON property name resolution via reflection
- Regex validation with flag support
- String-based query parameter generation

### 5. Sorting System

**Purpose**: Consistent sorting across all API endpoints.

**Design Principles**:
- Constants for type safety
- Consistent naming with API
- Extensible design

**Implementation**:
```csharp
public static class SortOrder
{
    public const string Ascending = "asc";
    public const string Descending = "desc";
}

public static class MovieSort
{
    public const string Name = "name";
    public const string RuntimeInMinutes = "runtimeInMinutes";
}
```

### 6. Data Models

**Purpose**: Strongly-typed representations of API responses.

**Design Principles**:
- Immutable by convention (init-only setters)
- JSON attribute mapping
- Non-nullable reference types where appropriate

**Model Design Pattern**:
```csharp
public class Movie
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = default!;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
    
    [JsonPropertyName("runtimeInMinutes")]
    public int RuntimeInMinutes { get; set; }
    
    [JsonPropertyName("rottenTomatoesScore")]
    public double RottenTomatoesScore { get; set; }
}
```

**Key Design Decisions**:
- JsonPropertyName attributes for API compatibility
- Default values for required properties
- Primitive types for API-provided data
- Nullable reference type annotations

### 7. Exception Handling

**Purpose**: Structured error handling for API failures.

**Design Principles**:
- Custom exception types for API-specific errors
- HTTP status code preservation
- Meaningful error messages

**Implementation**:
```csharp
public class ApiException : Exception
{
    public int StatusCode { get; }
    public ApiException(int statusCode, string message)
        : base($"API Error {statusCode}: {message}")
    {
        StatusCode = statusCode;
    }
}
```

## API Response Handling

### Response Structure

All API responses follow a consistent structure:

```csharp
internal class ApiResponse<T>
{
    public List<T> Docs { get; set; } = new();
    public int Total { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
    public int Page { get; set; }
    public int Pages { get; set; }
}
```

### Pagination Strategy

- **Client-side**: Optional limit and page parameters
- **Server-side**: API handles pagination logic
- **Response**: Full pagination metadata returned
- **Usage**: Consumers can implement their own pagination UI

## Security Considerations

### Authentication
- **Method**: Bearer token authentication
- **Storage**: API key stored in HttpClient headers
- **Scope**: Per-client instance (no global state)

### Data Validation
- **Input**: Filter validation (especially regex patterns)
- **Output**: JSON deserialization with type checking
- **Errors**: Structured exception handling

## Performance Considerations

### HTTP Client Management
- **Reuse**: Single HttpClient instance per API client
- **Configuration**: Proper base address and default headers
- **Disposal**: Managed by .NET garbage collection

### Memory Efficiency
- **Streaming**: Stream-based JSON deserialization
- **Async**: Non-blocking I/O operations
- **Cancellation**: CancellationToken support throughout

### Caching Strategy
- **Current**: No caching (stateless design)
- **Future**: Could implement response caching at HttpClient level

## Testing Strategy

### Unit Testing
- **Framework**: NUnit with FluentAssertions
- **Mocking**: Moq for HTTP dependencies
- **Coverage**: Comprehensive test coverage for all public APIs

### Test Categories
- **API Clients**: HTTP behavior and error handling
- **Models**: Serialization/deserialization
- **Filters**: Expression parsing and query building
- **Integration**: End-to-end API communication

## Extensibility

### Adding New Endpoints
1. Create new client class following existing pattern
2. Add to FellowshipClient as property
3. Implement standard async methods
4. Add corresponding model classes
5. Write comprehensive tests

### Adding New Filter Operators
1. Add enum value to FilterOperator
2. Update Filter.ToString() method
3. Add validation logic if needed
4. Update documentation

### Adding New Models
1. Create model class with JsonPropertyName attributes
2. Add to appropriate namespace
3. Update API clients to support new model
4. Add unit tests for serialization

## Deployment and Packaging

### NuGet Package Structure
```
Fellowship.SDK.1.0.0.nupkg
├── lib/net8.0/
│   ├── Fellowship.SDK.dll
│   └── Fellowship.SDK.xml (documentation)
├── Fellowship.SDK.nuspec
└── README.md
```

### Versioning Strategy
- **Semantic Versioning**: Major.Minor.Patch
- **Breaking Changes**: Major version increment
- **New Features**: Minor version increment
- **Bug Fixes**: Patch version increment

## Future Enhancements

### Planned Features
1. **Response Caching**: Implement configurable caching
2. **Retry Policies**: Add automatic retry with exponential backoff
3. **Rate Limiting**: Implement client-side rate limiting
4. **Additional Endpoints**: Support for characters, chapters, books
5. **Bulk Operations**: Support for batch requests

### Technical Debt
1. **HttpHelper Sorting**: Clean up hardcoded sort field mapping
2. **Error Messages**: Improve error message localization
3. **Documentation**: Generate XML documentation for IntelliSense

## Dependencies

### Runtime Dependencies
- **.NET 8.0**: Target framework
- **System.Text.Json**: JSON serialization
- **System.Net.Http**: HTTP communication
- **System.Linq.Expressions**: Filter expression trees

### Development Dependencies
- **NUnit**: Testing framework
- **Moq**: Mocking framework
- **FluentAssertions**: Assertion library
- **Coverlet**: Code coverage analysis

## Compliance and Standards

### Coding Standards
- **C# Conventions**: Microsoft C# coding conventions
- **Nullable Reference Types**: Enabled throughout
- **Async/Await**: Proper async patterns
- **SOLID Principles**: Applied where appropriate

### API Compatibility
- **REST**: RESTful API design principles
- **HTTP**: Standard HTTP status codes and methods
- **JSON**: Standard JSON serialization

This design specification serves as the technical blueprint for the Fellowship SDK, documenting architectural decisions, design patterns, and implementation details for current and future development.
