namespace Fellowship.SDK;

internal class ApiResponse<T>
{
    public List<T> Docs { get; set; } = new();
    public int Total { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
    public int Page { get; set; }
    public int Pages { get; set; }
}
