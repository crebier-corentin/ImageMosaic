using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SharedClasses
{
    public class ProgressBar
    {
        private static object _consoleLock = new object();

        private const int BarLenght = 30;

        private string _label;
        private int _maximum;
        private int _current;
        private readonly bool _noPercentage;

        private readonly int _originalTop;
        private int _lastPrintLength;

        public bool Done;

        public string Label => _label;
        public int Current => _current;
        public int Maximum => _maximum;
        public int Percentage => (int) ((double) _current / _maximum * 100);

        public int Fraction => Helpers.Ratio(_current, _maximum, BarLenght);

        public ProgressBar(int maximum, string label = "")
        {
            _maximum = maximum;
            _label = label;

            if (_maximum <= 0)
            {
                _noPercentage = true;
            }

            _originalTop = Console.CursorTop;

            //Move cursor to the next line
            Console.WriteLine();

            //PrintTimer
            ProgressBarManager.TimerElapsed += ProgressBarManagerOnTimerElapsed;
        }


        public void Tick()
        {
            if (Done) return;

            Interlocked.Increment(ref _current);

            //Done
            if (_current == _maximum)
            {
                Done = true;
                ProgressBarManager.TimerElapsed -= ProgressBarManagerOnTimerElapsed;
                PrintAsync();
            }
        }

        public void Tick(string label)
        {
            Tick();

            //Update label
            _label = label;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            //Label
            if (!string.IsNullOrWhiteSpace(_label))
            {
                builder.Append(_label);
                builder.Append(' ');
            }

            //Ratio and Percentage
            builder.Append($"{_current}");

            if (!_noPercentage)
            {
                builder.Append($"/{_maximum} - {Percentage}% ");

                //Bar
                var emptyPart = BarLenght - Fraction;

                builder.Append(
                    $"[{Helpers.RepeatChar('#', Fraction)}{Helpers.RepeatChar('-', emptyPart)}]");
            }

            return builder.ToString();
        }

        public void Print()
        {
            lock (_consoleLock)
            {
                //Remember console cursor position
                var consoleTop = Console.CursorTop;
                var consoleLeft = Console.CursorLeft;

                //Clear line and print new progress
                Console.SetCursorPosition(0, _originalTop);

                var progressOutput = ToString();

                //Pad right with space to clear previous progress
                Console.Write(progressOutput.PadRight(_lastPrintLength));

                _lastPrintLength = progressOutput.Length;

                //Restore console cursor position
                Console.SetCursorPosition(consoleLeft, consoleTop);
            }
        }

        public async Task PrintAsync()
        {
            await Task.Run(() => { Print(); });
        }

        private void ProgressBarManagerOnTimerElapsed(object sender, EventArgs e)
        {
            Print();
        }
    }
}