using System;
using System.Collections.Generic;

namespace SharedClasses
{
    public class Helpers
    {
        public static int Ratio(int value, int maximum, int unit)
        {
            return Ratio((double)value, (double)maximum, (double)unit);
        }

        public static int Ratio(double value, double maximum, double unit)
        {
            return Ratio(value, 0, maximum, unit);
        }

        public static int Ratio(double value, double minimum, double maximum, double unit)
        {
            return (int)Math.Floor((value - minimum) * unit / (maximum - minimum));
        }

        public static string RepeatChar(char c, int count)
        {
            if (count <= 0)
            {
                return "";
            }

            return new string(c, count);
        }
    }
    
    public static class LinkedListExtensions
    {
        public static IEnumerable<LinkedListNode<T>> EnumerateNodes<T>(this LinkedList<T> list)
        {
            var node = list.First;
            while(node != null)
            {
                yield return node;
                node = node.Next;
            }
        }
    }
}