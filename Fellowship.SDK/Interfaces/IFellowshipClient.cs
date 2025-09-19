namespace Fellowship.SDK.Interfaces;

/// <summary>
/// Main client interface for accessing The One API
/// </summary>
public interface IFellowshipClient
{
    /// <summary>
    /// Client for movie-related operations
    /// </summary>
    IMoviesClient Movies { get; set;}

    /// <summary>
    /// Client for quote-related operations
    /// </summary>
    IQuotesClient Quotes { get; set;}
}
