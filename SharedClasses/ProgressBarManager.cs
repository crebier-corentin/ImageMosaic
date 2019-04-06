using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharedClasses
{
    internal static class ProgressBarManager
    {
        private static readonly CancellationTokenSource Source = new CancellationTokenSource();

        public static event EventHandler TimerElapsed;

        static ProgressBarManager()
        {
            PrintTimer(Source.Token);
        }

        private static async Task PrintTimer(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(
                async () =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(100, cancellationToken);

                        RaiseTimerElapsed();
                    }
                },
                cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public static void RaiseTimerElapsed()
        {
            TimerElapsed?.Invoke(typeof(ProgressBarManager), EventArgs.Empty);
        }

        public static void Stop()
        {
            Source.Cancel();
        }
    }
}