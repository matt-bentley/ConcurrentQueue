using ConcurrentQueue.Queues.Abstract;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ConcurrentQueue.Queues
{
    class WaitPulseQueue : PCQueue
    {
        private readonly object _locker = new object();
        private Thread[] _workers;
        private Queue<Action> _itemQueue = new Queue<Action>();

        public WaitPulseQueue(int workerCount)
        {
            _workers = new Thread[workerCount];

            // Create and start a separate thread for each worker
            for (int i = 0; i < workerCount; i++)
            {
                (_workers[i] = new Thread(Consume)).Start();
            }
        }

        public void Dispose()
        {
            // Enqueue one null item per worker to make each exit.
            foreach (Thread worker in _workers)
            {
                EnqueueItem(null);
            }

            foreach (Thread worker in _workers)
            {
                worker.Join();
            }
        }

        public void EnqueueItem(Action item)
        {
            lock (_locker)
            {
                _itemQueue.Enqueue(item);           // We must pulse because we're
                Monitor.Pulse(_locker);             // changing a blocking condition.
            }
        }

        public void Consume()
        {
            while (true)                        // Keep consuming until
            {                                   // told otherwise.
                Action item;
                lock (_locker)
                {
                    while (_itemQueue.Count == 0)
                    {
                        Monitor.Wait(_locker);
                    }
                    item = _itemQueue.Dequeue();
                }
                if (item == null)
                {
                    return;         // This signals our exit.
                }
                item();             // Execute item.
            }
        }
    }
}
