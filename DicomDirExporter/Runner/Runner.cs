using System.Timers;

namespace DicomDirExporter.Runner
{
    public abstract class Runner
    {
        protected readonly Timer _timer;

        protected Runner()
        {
            _timer = new Timer { Interval = 1000 };
            _timer.Elapsed += OnTimedEvent;
            _timer.Start();
        }

        protected abstract void OnTimedEvent(object sender, ElapsedEventArgs e);

        protected void Start()
        {
            _timer.Start();
        }

        protected void Stop()
        {
            _timer.Stop();
        }
    }
}