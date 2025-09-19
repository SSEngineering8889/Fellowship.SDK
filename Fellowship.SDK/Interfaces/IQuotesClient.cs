using Fellowship.SDK.Filters;
using Fellowship.SDK.Models;

namespace Fellowship.SDK.Interfaces;

/// <summary>
/// Interface for quote-related operations
/// </summary>
public interface IQuotesClient
{
    /// <summary>
    /// Gets all quotes with optional filtering, sorting, and pagination
    /// </summary>
    /// <param name="limit">Maximum number of results to return</param>
    /// <param name="page">Page number for pagination</param>
    /// <param name="filters">Optional filters to apply</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Collection of quotes</returns>
    Task<IEnumerable<Quote>> GetAllAsync(
        int? limit = null,
        int? page = null,
        IEnumerable<Filter<Quote>>? filters = null,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a specific quote by ID
    /// </summary>
    /// <param name="id">Quote ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Quote if found, null otherwise</returns>
    Task<Quote?> GetByIdAsync(string id, CancellationToken ct = default);
}
