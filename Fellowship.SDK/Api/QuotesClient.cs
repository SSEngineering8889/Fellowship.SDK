using Fellowship.SDK.Filters;
using Fellowship.SDK.Interfaces;
using Fellowship.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Fellowship.SDK;

public class QuotesClient : IQuotesClient
{
    private readonly HttpClient _http;
    private readonly ILogger<QuotesClient> _logger;

    public QuotesClient(string apiKey, ILogger<QuotesClient> logger)
    {
        _logger = logger;
        _http = new HttpClient
        {
            BaseAddress = new Uri("https://the-one-api.dev/v2/")
        };
        _http.DefaultRequestHeaders.Authorization = new("Bearer", apiKey);
        
        _logger.LogDebug("QuotesClient initialized with base URL: {BaseUrl}", _http.BaseAddress);
    }

    public async Task<IEnumerable<Quote>> GetAllAsync(
        int? limit = null,
        int? page = null,
        IEnumerable<Filter<Quote>>? filters = null,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Getting all quotes with limit={Limit}, page={Page}", 
            limit, page);

        var result = await HttpHelper.GetAsync<Quote>(_http, "quote", limit, page, filters, ct, _logger);
        
        _logger.LogInformation("Retrieved {QuoteCount} quotes", result.Docs.Count);
        return result.Docs;
    }

    public async Task<Quote?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        _logger.LogInformation("Getting quote by ID: {QuoteId}", id);
        
        var result = (await HttpHelper.GetAsync<Quote>(_http, $"quote/{id}", ct: ct, logger: _logger)).Docs.FirstOrDefault();
        
        if (result != null)
            _logger.LogDebug("Found quote: {Dialog} ({QuoteId})", result.Dialog, result.Id);
        else
            _logger.LogWarning("Quote not found: {QuoteId}", id);
            
        return result;
    }
}
