using System.Diagnostics;
using System.Timers;

namespace GmnaoCompanion.Services;

public class ProcessWatcher
{
    private readonly string _processName;
    private readonly System.Timers.Timer _timer;
    private bool _wasRunning;

    public event EventHandler? ProcessStarted;

    public ProcessWatcher(string processName, double intervalMs = 2000)
    {
        _processName = processName;
        _timer = new System.Timers.Timer(intervalMs);
        _timer.Elapsed += OnTimerElapsed;
    }

    public void Start()
    {
        _wasRunning = IsRunning();
        _timer.Start();
    }

    public void Stop() => _timer.Stop();

    private bool IsRunning() =>
        Process.GetProcessesByName(_processName).Length > 0;

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        bool isRunning = IsRunning();
        if (isRunning && !_wasRunning)
            ProcessStarted?.Invoke(this, EventArgs.Empty);
        _wasRunning = isRunning;
    }
}
