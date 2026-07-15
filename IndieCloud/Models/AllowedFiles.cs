namespace IndieCloud.AllowedFiles;

public static class AllowedFiles
{
    public static readonly IReadOnlyDictionary<string, string> MimeMap = new Dictionary<string, string>
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".pdf"] = "application/pdf",
        [".wav"] = "audio/wav",
        [".mp3"] = "audio/mpeg",
    };
}
