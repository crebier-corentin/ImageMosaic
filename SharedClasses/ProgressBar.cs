using System;
using System.Text;
using System.Threading;

namespace SharedClasses
{
    public class ProgressBar
    {
        private const int BarLenght = 20;

        private string _label;
        private int _maximum;
        private int _current;
        private bool _noPercentage;

        private readonly int _originalTop;
        private int _lastPrintLength;

        internal bool Done;

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

            ProgressBarManager.Add(this);
        }

        public void Tick()
        {
            if (Done) return;

            Interlocked.Increment(ref _current);
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
}