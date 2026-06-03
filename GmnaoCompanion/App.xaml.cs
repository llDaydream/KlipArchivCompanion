using System.Windows;
using GmnaoCompanion.Services;
using Hardcodet.Wpf.TaskbarNotification;

namespace GmnaoCompanion;

public partial class App : Application
{
    private TaskbarIcon? _trayIcon;
    private ProcessWatcher? _processWatcher;
    private MainWindow? _settingsWindow;
    private UpdateService.ReleaseInfo? _pendingUpdate;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        SettingsService.Instance.Load();

        _trayIcon = new TaskbarIcon
        {
            Icon = System.Drawing.SystemIcons.Application,
            ToolTipText = "Gmnao Companion"
        };

        var contextMenu = new System.Windows.Controls.ContextMenu();

        var settingsItem = new System.Windows.Controls.MenuItem { Header = "Einstellungen" };
        settingsItem.Click += (_, _) => OpenSettings();

        var checkUpdateItem = new System.Windows.Controls.MenuItem { Header = "Auf Updates prüfen" };
        checkUpdateItem.Click += async (_, _) => await CheckForUpdatesAsync(manual: true);

        var separator = new System.Windows.Controls.Separator();

        var exitItem = new System.Windows.Controls.MenuItem { Header = "Beenden" };
        exitItem.Click += (_, _) => Shutdown();

        contextMenu.Items.Add(settingsItem);
        contextMenu.Items.Add(checkUpdateItem);
        contextMenu.Items.Add(separator);
        contextMenu.Items.Add(exitItem);
        _trayIcon.ContextMenu = contextMenu;
        _trayIcon.TrayMouseDoubleClick += (_, _) => OpenSettings();
        _trayIcon.TrayBalloonTipClicked += OnBalloonClicked;

        _processWatcher = new ProcessWatcher("bf6");
        _processWatcher.ProcessStarted += OnBattlefieldStarted;
        _processWatcher.Start();

        // Update-Check 5 Sekunden nach Start
        _ = Task.Delay(5000).ContinueWith(_ => CheckForUpdatesAsync(manual: false));
    }

    private async Task CheckForUpdatesAsync(bool manual)
    {
        var update = await UpdateService.Instance.CheckForUpdateAsync();

        await Dispatcher.InvokeAsync(() =>
        {
            if (update is not null)
            {
                _pendingUpdate = update;
                _trayIcon!.ShowBalloonTip(
                    "Update verfügbar",
                    $"Version {update.Version} ist verfügbar — klicken zum Installieren.",
                    BalloonIcon.Info);
            }
            else if (manual)
            {
                _trayIcon!.ShowBalloonTip(
                    "Kein Update verfügbar",
                    $"Du verwendest die aktuelle Version ({UpdateService.CurrentVersion}).",
                    BalloonIcon.Info);
            }
        });
    }

    private async void OnBalloonClicked(object sender, RoutedEventArgs e)
    {
        if (_pendingUpdate is null) return;

        var update = _pendingUpdate;
        _pendingUpdate = null;

        _trayIcon!.ToolTipText = "Gmnao Companion — Update wird heruntergeladen...";

        await UpdateService.Instance.DownloadAndInstallAsync(
            update.DownloadUrl,
            progress => Dispatcher.Invoke(() =>
                _trayIcon.ToolTipText = $"Gmnao Companion — Download: {progress}%"));

        _trayIcon.ToolTipText = "Gmnao Companion";
    }

    private void OpenSettings()
    {
        if (_settingsWindow == null || !_settingsWindow.IsLoaded)
            _settingsWindow = new MainWindow();

        _settingsWindow.Show();
        _settingsWindow.Activate();

        if (_settingsWindow.WindowState == WindowState.Minimized)
            _settingsWindow.WindowState = WindowState.Normal;
    }

    private void OnBattlefieldStarted(object? sender, EventArgs e)
    {
        if (!SettingsService.Instance.ShadowPlayNotification) return;

        Dispatcher.Invoke(() =>
        {
            var overlay = new ShadowplayOverlay();
            overlay.Show();
        });
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _processWatcher?.Stop();
        _trayIcon?.Dispose();
        base.OnExit(e);
    }
}
