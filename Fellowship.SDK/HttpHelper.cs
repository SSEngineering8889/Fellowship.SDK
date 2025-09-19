using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Fellowship.SDK;

internal static class HttpHelper
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public static async Task<ApiResponse<TModel>> GetAsync<TModel>(
        HttpClient http,
        string path,
        int? limit = null,
        int? page = null,
        IEnumerable<Filters.Filter<TModel>>? filters = null,
        CancellationToken ct = default,
        ILogger? logger = null)
    {
        var url = BuildUrl(path, limit, page, filters);
        
        logger?.LogDebug("Making HTTP request to: {Url}", url);
        var startTime = DateTime.UtcNow;
        
        var res = await http.GetAsync(url, ct);
        var duration = DateTime.UtcNow - startTime;

        logger?.LogDebug("HTTP request completed in {Duration}ms with status: {StatusCode}", 
            duration.TotalMilliseconds, (int)res.StatusCode);

        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync(ct);
            logger?.LogError("API request failed: {StatusCode} {Error}", (int)res.StatusCode, error);
            throw new ApiException((int)res.StatusCode, error);
        }

        var stream = await res.Content.ReadAsStreamAsync(ct);
        var result = await JsonSerializer.DeserializeAsync<ApiResponse<TModel>>(stream, JsonOpts, ct);

        logger?.LogDebug("Successfully deserialized {ItemCount} items", result?.Docs?.Count ?? 0);

        return result ?? new ApiResponse<TModel> { Docs = new() };
    }

    private static string BuildUrl<TModel>(
        string path,
        int? limit,
        int? page,
        IEnumerable<Filters.Filter<TModel>>? filters)
    {
        var query = new List<string>();

        if (limit.HasValue) query.Add($"limit={limit}");
        if (page.HasValue) query.Add($"page={page}");
      
        if (filters != null)
        {
            foreach (var f in filters)
                query.Add(f.ToString());
        }

        return query.Count > 0 ? $"{path}?{string.Join("&", query)}" : path;
    }
}
