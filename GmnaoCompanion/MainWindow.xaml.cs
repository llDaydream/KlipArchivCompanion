using System.Windows;
using GmnaoCompanion.Services;

namespace GmnaoCompanion;

public partial class MainWindow : Window
{
    private bool _loading = true;

    public MainWindow()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        _loading = true;
        AutoStartToggle.IsChecked = SettingsService.Instance.AutoStart;
        ShadowPlayToggle.IsChecked = SettingsService.Instance.ShadowPlayNotification;
        _loading = false;
    }

    private void AutoStart_Changed(object sender, RoutedEventArgs e)
    {
        if (_loading) return;
        SettingsService.Instance.AutoStart = AutoStartToggle.IsChecked == true;
    }

    private void ShadowPlay_Changed(object sender, RoutedEventArgs e)
    {
        if (_loading) return;
        SettingsService.Instance.ShadowPlayNotification = ShadowPlayToggle.IsChecked == true;
    }

    private void OnMinimize(object sender, RoutedEventArgs e) =>
        WindowState = WindowState.Minimized;

    private void OnClose(object sender, RoutedEventArgs e) => Hide();

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}
