using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FindTimeQuant
{
    class Program
    {
        static Stopwatch stopwatch = new Stopwatch();
        static int working = 1;
        private static long start;
        private static bool[] wroten = new[]  { false, false };
        private static List<double> times = new List<double>();
        static void Main(string[] args)
        {
            var processorNum = args.Length > 0 ? int.Parse(args[0]) - 1 : 1;
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << processorNum);
            stopwatch.Start();
            start = Stopwatch.GetTimestamp();
            var th = new Thread(() => { Go(0); });
            var th2 = new Thread(() => { Go(1); });
            th.Start();
            th2.Start();
            th.Join();
            th2.Join();
            Console.WriteLine(times.Skip(1).Select(x => x - times[times.IndexOf(x)-1]).Sum());
        }

        static void Go(int i)
        {
            int j = 0;
            while (j < 1000000000)
            {
                if (!wroten[i])
                {
                    var time = stopwatch.ElapsedMilliseconds;
                    Console.WriteLine(time + " " +i);
                    times.Add(time);
                    wroten[i] = true;
                    wroten[1 - i] = false;
                }
                j++;
            }
        }
    }
}