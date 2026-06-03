using System.Windows;
using System.Windows.Threading;

namespace GmnaoCompanion;

public partial class ShadowplayOverlay : Window
{
    private readonly DispatcherTimer _countdown;
    private int _secondsLeft = 30;

    public ShadowplayOverlay()
    {
        InitializeComponent();

        _countdown = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _countdown.Tick += OnCountdownTick;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        PositionBottomRight();
        UpdateCountdownText();
        _countdown.Start();
    }

    private void PositionBottomRight()
    {
        var area = SystemParameters.WorkArea;
        Left = area.Right - ActualWidth - 20;
        Top = area.Bottom - ActualHeight - 20;
    }

    private void OnCountdownTick(object? sender, EventArgs e)
    {
        _secondsLeft--;
        UpdateCountdownText();
        if (_secondsLeft <= 0)
            Close();
    }

    private void UpdateCountdownText() =>
        CountdownText.Text = $"Schließt in {_secondsLeft}s automatisch";

    private void OnYes_Click(object sender, RoutedEventArgs e) => Close();

    private void OnNo_Click(object sender, RoutedEventArgs e) => Close();

    private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) =>
        DragMove();

    protected override void OnClosed(EventArgs e)
    {
        _countdown.Stop();
        base.OnClosed(e);
    }
}
