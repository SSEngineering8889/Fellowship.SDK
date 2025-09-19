using Fellowship.SDK.Filters;
using Fellowship.SDK.Models;

namespace Fellowship.SDK.Interfaces;

/// <summary>
/// Interface for movie-related operations
/// </summary>
public interface IMoviesClient
{
    /// <summary>
    /// Gets all movies with optional filtering, sorting, and pagination
    /// </summary>
    /// <param name="limit">Maximum number of results to return</param>
    /// <param name="page">Page number for pagination</param>
    /// <param name="filters">Optional filters to apply</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Collection of movies</returns>
    Task<IEnumerable<Movie>> GetAllAsync(
        int? limit = null,
        int? page = null,
        IEnumerable<Filter<Movie>>? filters = null,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a specific movie by ID
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Movie if found, null otherwise</returns>
    Task<Movie?> GetByIdAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Gets all quotes for a specific movie
    /// </summary>
    /// <param name="movieId">Movie ID</param>
    /// <param name="limit">Maximum number of results to return</param>
    /// <param name="page">Page number for pagination</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Collection of quotes for the movie</returns>
    Task<IEnumerable<Quote>> GetQuotesAsync(
        string movieId,
        int? limit = null,
        int? page = null,
        CancellationToken ct = default);
}
