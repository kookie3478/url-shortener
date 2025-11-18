using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;
using UrlShortener.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Read DATABASE_URL from environment (Supabase)
var connString = Environment.GetEnvironmentVariable("DATABASE_URL") // db.hqydxyctqgtmgxszquel.supabase.co
                 ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Register services
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connString));

builder.Services.AddScoped<QrService>();
builder.Services.AddScoped<ShortcodeService>();

// Allow frontend (Vercel) to call API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");

// ----------------------
// API ENDPOINTS
// ----------------------

// CREATE SHORT URL
// CREATE SHORT URL
app.MapPost("/api/shorten",
async (CreateRequest req, HttpContext ctx, AppDbContext db, QrService qr, ShortcodeService codeGen) =>
{
    // Get client IP
    var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

    // Rate limit check
    bool allowed = await RateLimitEntry.CheckRateLimitAsync(ip, db);
    if (!allowed) return Results.StatusCode(429);

    // Validate URL
    if (!Uri.IsWellFormedUriString(req.Url, UriKind.Absolute))
        return Results.BadRequest("Invalid URL");

    // Generate unique shortcode or use custom alias
    string shortcode = string.IsNullOrEmpty(req.CustomAlias)
        ? await codeGen.GenerateUniqueCode(db)
        : req.CustomAlias;

    var entry = new UrlEntry
    {
        OriginalUrl = req.Url,
        Shortcode = shortcode,
        ExpiresAt = req.ExpiryDays > 0
            ? DateTime.UtcNow.AddDays(req.ExpiryDays)
            : null
    };

    // ---- FIX: Use environment BASE_URL instead of localhost ----
    var envBase = Environment.GetEnvironmentVariable("BASE_URL");
    var baseUrl = !string.IsNullOrWhiteSpace(envBase)
                    ? envBase.TrimEnd('/')
                    : $"{ctx.Request.Scheme}://{ctx.Request.Host}";

    // If QR requested
    if (req.GenerateQr)
    {
        entry.QrBase64 = qr.GenerateQr($"{baseUrl}/{shortcode}");
    }

    // Save to DB
    db.Urls.Add(entry);
    await db.SaveChangesAsync();

    // ---- FIXED shortUrl ----
    var responseUrl = $"{baseUrl}/{shortcode}";

    return Results.Ok(new
    {
        shortUrl = responseUrl,
        qrBase64 = entry.QrBase64 != null
            ? $"data:image/png;base64,{entry.QrBase64}"
            : null
    });
});

// REDIRECT
// REDIRECT
app.MapGet("/{code}", async (string code, HttpContext ctx, AppDbContext db) =>
{
    var entry = await db.Urls.FirstOrDefaultAsync(u => u.Shortcode == code);

    if (entry == null)
        return Results.NotFound("<h1>Short URL not found</h1>");

    if (entry.ExpiresAt.HasValue && entry.ExpiresAt.Value < DateTime.UtcNow)
        return Results.Content("<h1>Link expired</h1>", "text/html");

    // Count click + store user agent / IP
    entry.ClickCount++;
    db.Clicks.Add(new Click
    {
        UrlId = entry.Id,
        RemoteIp = ctx.Connection.RemoteIpAddress?.ToString(),
        UserAgent = ctx.Request.Headers["User-Agent"].ToString()
    });

    await db.SaveChangesAsync();

    // FIX — prevent newline in redirect target
    return Results.Redirect(entry.OriginalUrl.Trim());
});


app.Run();

public record CreateRequest(
    string Url,
    bool GenerateQr,
    int ExpiryDays,
    string? CustomAlias,
    string? BaseUrl
);
