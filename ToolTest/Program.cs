using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
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




        public class DisposeSample : IDisposable
        {
            public Stream A;
            public Stream B;
            public Stream C;
            private bool disposedValue;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        A.SafeDispose();
                        B.SafeDispose();
                        C.SafeDispose();
                        // TODO: 處置受控狀態 (受控物件)
                    }

                    // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                    // TODO: 將大型欄位設為 Null
                    disposedValue = true;
                }
            }

            // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
            // ~DisposeSample()
            // {
            //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
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
