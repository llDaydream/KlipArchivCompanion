using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace GmnaoCompanion.Services;

public class SettingsService
{
    public static readonly SettingsService Instance = new();

    private const string AppName = "GmnaoCompanion";
    private const string RegistryRunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GmnaoCompanion", "settings.json");

    private AppSettings _data = new();

    private SettingsService() { }

    public void Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
                _data = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath)) ?? new();
        }
        catch { _data = new(); }
    }

    public bool AutoStart
    {
        get
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey);
            return key?.GetValue(AppName) != null;
        }
        set
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey, writable: true);
            if (key == null) return;
            if (value)
            {
                var exe = Process.GetCurrentProcess().MainModule?.FileName;
                if (exe != null) key.SetValue(AppName, $"\"{exe}\"");
            }
            else
            {
                key.DeleteValue(AppName, throwOnMissingValue: false);
            }
        }
    }

    public bool ShadowPlayNotification
    {
        get => _data.ShadowPlayNotification;
        set { _data.ShadowPlayNotification = value; Save(); }
    }

    private void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(_data,
                new JsonSerializerOptions { WriteIndented = true }));
        }
        catch { }
    }

    private class AppSettings
    {
        public bool ShadowPlayNotification { get; set; } = true;
    }
}
