using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Extensions;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

using Perfolizer.Horology;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Tools;

namespace ToolTest
{
    internal partial class Program
    {
        public static void Boxing()
        {
            while(true)
            {
                var stopWatch = new Stopwatch();
                stopWatch.Restart();
                for (int i = 0; i < 100000;i++)
                {
                    var s = $"{i}";
                }
                Console.WriteLine(stopWatch.Elapsed);
                stopWatch.Restart();
                for (int i = 0; i < 100000; i++)
                {
                    var s = $"{i.ToString()}";
                }
                Console.WriteLine(stopWatch.Elapsed);

                Console.WriteLine("-----------------");
                Thread.Sleep(1000);
            }
        }

        static async Task Main(string[] args)
        {
            Boxing();
            while (true)
            {
                await Task.Delay(1000);
            }
        }

    }
}
