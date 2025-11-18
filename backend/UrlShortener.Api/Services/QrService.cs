using QRCoder;

namespace UrlShortener.Api.Services;

public class QrService
{
    public string GenerateQr(string url)
    {
        using var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        using var qr = new PngByteQRCode(data);
        var bytes = qr.GetGraphic(20);
        return Convert.ToBase64String(bytes);
    }
}
