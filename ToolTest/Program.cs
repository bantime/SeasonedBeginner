using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;

using Tools;

namespace ToolTest
{

    internal class Program
    {


        static async Task Main(string[] args)
        {

            while (true)
                await Task.Delay(1000);
        }
        public class EasyLockerSampleItem
        {
            public int Value;
        }
        public static async Task EasyLockSample()
        {
            var dic = new ConcurrentDictionary<int, EasyLocker<EasyLockerSampleItem>>();
            dic.TryAdd(0, new EasyLockerSampleItem().CreateLocker());
            dic.TryAdd(1, new EasyLockerSampleItem().CreateLocker());
            bool start = false;
            for (int i = 0; i < 10; i++)
            {
                _ = Task.Run(async () =>
                {
                    while (!start)
                    {
                        await Task.Delay(1);
                    }
                    for (int j = 0; j < 100; j++)
                    {
                        if (dic.TryGetValue(0, out var item))
                        {
                            item.Execute(x =>
                            {
                                x.Value++;
                            });
                        }
                        await Task.Delay(1);
                    }
                    {
                        if (dic.TryGetValue(0, out var item))
                        {
                            Console.WriteLine(item.Execute(x => x.Value));
                        }
                    }

                });
            }
            await Task.Delay(1000);
            start = true;
        }
    }
}
