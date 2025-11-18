using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Api.Models;

public class RateLimitEntry
{
    public int Id { get; set; }
    public string RemoteIp { get; set; } = "";
    public string Window { get; set; } = ""; // minute window
    public int Attempts { get; set; }

    public static async Task<bool> CheckRateLimitAsync(string ip, DbContext db)
    {
        var window = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");

        var existing = await db.Set<RateLimitEntry>()
            .FirstOrDefaultAsync(r => r.RemoteIp == ip && r.Window == window);

        if (existing == null)
        {
            db.Add(new RateLimitEntry
            {
                RemoteIp = ip,
                Window = window,
                Attempts = 1
            });
            await db.SaveChangesAsync();
            return true;
        }

        // Limit: 5 requests per minute per IP
        if (existing.Attempts >= 5)
            return false;

        existing.Attempts++;
        await db.SaveChangesAsync();
        return true;
    }
}
