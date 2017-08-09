using System;
using System.Diagnostics;

namespace GitNStats.Core
{
    public static class Tooling
    {
        public static void WithStopWatch(Action action, String formatString)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            action();
            
            stopwatch.Stop();
            Console.WriteLine(formatString, stopwatch.ElapsedMilliseconds);
        }
    }
}