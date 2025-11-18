namespace UrlShortener.Api.Models;

public class UrlEntry
{
    public int Id { get; set; }
    public string Shortcode { get; set; } = "";
    public string OriginalUrl { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public long ClickCount { get; set; }
    public string? QrBase64 { get; set; }
}
