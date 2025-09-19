namespace Fellowship.SDK;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public ApiException(int statusCode, string message)
        : base($"API Error {statusCode}: {message}")
    {
        StatusCode = statusCode;
    }
}
