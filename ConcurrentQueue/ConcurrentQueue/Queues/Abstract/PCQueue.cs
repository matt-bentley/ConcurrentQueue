using System;

namespace ConcurrentQueue.Queues.Abstract
{
    interface PCQueue : IDisposable
    {
        void EnqueueItem(Action item);
        void Consume();
    }
}
