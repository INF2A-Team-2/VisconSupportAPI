namespace VisconSupportAPI;

public static class Utils
{
    public static string? GetFileExtension(string mimeType)
    {
        Dictionary<string, string> extensions = new Dictionary<string, string>()
        {
            { "image/png", ".png" },
            { "image/jpeg", ".jpeg" },
            { "image/jpg", ".jpg" },
            { "video/mp4", ".mp4" },
            { "application/pdf", ".pdf" },
        };

        extensions.TryGetValue(mimeType, out string? ext);

        return ext;
    }
}