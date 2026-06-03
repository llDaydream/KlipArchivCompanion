using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace GmnaoCompanion.Services;

public class UpdateService
{
    public static readonly UpdateService Instance = new();

    private const string ApiUrl =
        "https://api.github.com/repos/llDaydream/KlipArchivCompanion/releases/latest";

    private static readonly HttpClient _http = new()
    {
        DefaultRequestHeaders = { { "User-Agent", "GmnaoCompanion" } }
    };

    private UpdateService() { }

    public static Version CurrentVersion =>
        Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);

    public async Task<ReleaseInfo?> CheckForUpdateAsync()
    {
        try
        {
            var release = await _http.GetFromJsonAsync<GitHubRelease>(ApiUrl);
            if (release is null || string.IsNullOrEmpty(release.TagName)) return null;

            var tag = release.TagName.TrimStart('v');
            if (!Version.TryParse(tag, out var latest)) return null;

            if (latest <= CurrentVersion) return null;

            var asset = release.Assets.FirstOrDefault(a =>
                a.Name.StartsWith("GmnaoCompanion_Setup") && a.Name.EndsWith(".exe"));

            if (asset is null) return null;

            return new ReleaseInfo(latest, asset.BrowserDownloadUrl);
        }
        catch
        {
            return null;
        }
    }

    public async Task DownloadAndInstallAsync(string downloadUrl, Action<int>? onProgress = null)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), "GmnaoCompanion_Update.exe");

        using var response = await _http.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var total = response.Content.Headers.ContentLength ?? -1;
        await using var stream = await response.Content.ReadAsStreamAsync();
        await using var file = File.Create(tempPath);

        var buffer = new byte[81920];
        long downloaded = 0;
        int read;

        while ((read = await stream.ReadAsync(buffer)) > 0)
        {
            await file.WriteAsync(buffer.AsMemory(0, read));
            downloaded += read;
            if (total > 0)
                onProgress?.Invoke((int)(downloaded * 100 / total));
        }

        file.Close();

        Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });
    }

    public record ReleaseInfo(Version Version, string DownloadUrl);

    private class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = "";

        [JsonPropertyName("assets")]
        public List<GitHubAsset> Assets { get; set; } = [];
    }

    private class GitHubAsset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; } = "";
    }
}
