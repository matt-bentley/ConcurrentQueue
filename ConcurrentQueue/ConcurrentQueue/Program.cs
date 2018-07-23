using ConcurrentQueue.Queues;
using ConcurrentQueue.Queues.Abstract;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentQueue
{
    class Program
    {
        private static int _completed = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting applications...");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            using (PCQueue queue = new WaitPulseQueue(4))
            {
                for(int i = 0; i < 40; i++)
                {
                    queue.EnqueueItem(LongRunningTask);
                }

                Console.WriteLine("Press any key to stop the queue...");
                Console.ReadKey();
            }

            sw.Stop();

            Console.WriteLine($"Completed {_completed} tasks in {sw.Elapsed.TotalSeconds}s. Press any key to exit...");
            Console.ReadKey();
        }

        private static void LongRunningTask()
        {            
            Console.WriteLine($"Starting task on thread: {Thread.CurrentThread.ManagedThreadId}");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Thread.Sleep(100);
            sw.Stop();
            var i = Interlocked.Increment(ref _completed);
            Console.WriteLine($"Finished task {_completed} on thread: {Thread.CurrentThread.ManagedThreadId} in {sw.Elapsed.TotalSeconds}");          
        }
    }
}
