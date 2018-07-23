using ConcurrentQueue.Queues.Abstract;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentQueue.Queues
{
    class BlockingCollectionQueue : PCQueue
    {
        private BlockingCollection<Action> _taskQueue = new BlockingCollection<Action>();

        public BlockingCollectionQueue(int workerCount)
        {
            // Create and start a separate Task for each consumer:
            for (int i = 0; i < workerCount; i++)
            {
                Task.Factory.StartNew(Consume, TaskCreationOptions.LongRunning);
            }
        }

        public void Dispose()
        {
            _taskQueue.CompleteAdding();
        }

        public void EnqueueItem(Action action)
        {
            _taskQueue.Add(action);
        }

        public void Consume()
        {
            Console.WriteLine($"Starting worker on thread: {Thread.CurrentThread.ManagedThreadId}");
            // This sequence that we’re enumerating will block when no elements
            // are available and will end when CompleteAdding is called. 
            foreach (Action action in _taskQueue.GetConsumingEnumerable())
            {
                action();     // Perform task.
            }

            Console.WriteLine($"Stopping worker on thread: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
