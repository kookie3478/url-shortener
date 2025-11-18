using UrlShortener.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Api.Services;

public class ShortcodeService
{
    private const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public async Task<string> GenerateUniqueCode(AppDbContext db, int length = 6)
    {
        var rng = Random.Shared;

        while (true)
        {
            string code = new string(
                Enumerable.Range(0, length)
                    .Select(_ => chars[rng.Next(chars.Length)])
                    .ToArray()
            );

            if (!await db.Urls.AnyAsync(u => u.Shortcode == code))
                return code;
        }
    }
}
