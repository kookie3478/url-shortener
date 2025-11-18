namespace UrlShortener.Api.Models;

public class Click
{
    public int Id { get; set; }
    public int UrlId { get; set; }
    public string? RemoteIp { get; set; }
    public string? UserAgent { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
