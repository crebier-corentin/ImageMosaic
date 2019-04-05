using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharedClasses
{
    internal static class ProgressBarManager
    {
        private static readonly ConcurrentBag<ProgressBar> Bars = new ConcurrentBag<ProgressBar>();
        private static readonly CancellationTokenSource Source = new CancellationTokenSource();

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

                        Print();
                    }
                },
                cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        internal static void Add(ProgressBar progressBar)
        {
            Bars.Add(progressBar);
        }

        private static void Print()
        {
            foreach (var bar in Bars)
            {
                //Ignore if bar at 100%
                if (bar.Done) continue;

                bar.Print();

                if (bar.Current == bar.Maximum)
                {
                    bar.Done = true;
                }
            }
        }

        public static void Stop()
        {
            Source.Cancel();
        }
    }
}