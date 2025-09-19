using Fellowship.SDK.Filters;
using Fellowship.SDK.Interfaces;
using Fellowship.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Fellowship.SDK;

public class MoviesClient : IMoviesClient
{
    private readonly HttpClient _http;
    private readonly ILogger<MoviesClient> _logger;

    public MoviesClient(string apiKey, ILogger<MoviesClient> logger)
    {
        _logger = logger;
        _http = new HttpClient
        {
            BaseAddress = new Uri("https://the-one-api.dev/v2/")
        };
        _http.DefaultRequestHeaders.Authorization = new("Bearer", apiKey);
        
        _logger.LogDebug("MoviesClient initialized with base URL: {BaseUrl}", _http.BaseAddress);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(
        int? limit = null,
        int? page = null,
        IEnumerable<Filter<Movie>>? filters = null,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Getting all movies with limit={Limit}, page={Page}", 
            limit, page);

        var result = await HttpHelper.GetAsync(_http, "movie", limit, page,  filters, ct, _logger);
        
        _logger.LogInformation("Retrieved {MovieCount} movies", result.Docs.Count);
        return result.Docs;
    }

    public async Task<Movie?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        _logger.LogInformation("Getting movie by ID: {MovieId}", id);
        
        var result = (await HttpHelper.GetAsync<Movie>(_http, $"movie/{id}", ct: ct, logger: _logger)).Docs.FirstOrDefault();
        
        if (result != null)
            _logger.LogDebug("Found movie: {MovieName} ({MovieId})", result.Name, result.Id);
        else
            _logger.LogWarning("Movie not found: {MovieId}", id);
            
        return result;
    }

    public async Task<IEnumerable<Quote>> GetQuotesAsync(
        string movieId,
        int? limit = null,
        int? page = null,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Getting quotes for movie: {MovieId} with limit={Limit}, page={Page}", 
            movieId, limit, page);
            
        var result = await HttpHelper.GetAsync<Quote>(_http, $"movie/{movieId}/quote", limit, page, null, ct, _logger);
        
        _logger.LogInformation("Retrieved {QuoteCount} quotes for movie {MovieId}", result.Docs.Count, movieId);
        return result.Docs;
    }
}
