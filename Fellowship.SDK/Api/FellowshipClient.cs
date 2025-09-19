using Fellowship.SDK.Interfaces;

namespace Fellowship.SDK.Api;

public class FellowshipClient : IFellowshipClient
{
    public IMoviesClient Movies { get; set; }
    public IQuotesClient Quotes { get; set; }

    public FellowshipClient(IMoviesClient moviesClient, IQuotesClient quotesClient)
    {
        Movies = moviesClient;
        Quotes = quotesClient;
    }
}
