namespace Treehopper.ThirdParty
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    // from https://blogs.msdn.microsoft.com/pfxteam/2012/02/12/building-async-coordination-primitives-part-6-asynclock/

    /// <summary>
    /// An Async-compatible semaphore lock
    /// </summary>
    public class AsyncSemaphore
    {
        private static readonly Task Completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> waiters = new Queue<TaskCompletionSource<bool>>();
        private int currentCount;

        /// <summary>
        /// Create a new lock
        /// </summary>
        /// <param name="initialCount">The initial count the semaphore should use</param>
        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount");
            currentCount = initialCount;
        }

        /// <summary>
        /// Wait for the semaphore to release
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public Task WaitAsync()
        {
            lock (waiters)
            {
                if (currentCount > 0)
                {
                    --currentCount;
                    return Completed;
                }
                else
                {
                    var waiter = new TaskCompletionSource<bool>();
                    waiters.Enqueue(waiter);
                    return waiter.Task;
                }
            }
        }

        /// <summary>
        /// Release the semaphore
        /// </summary>
        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (waiters)
            {
                if (waiters.Count > 0)
                    toRelease = waiters.Dequeue();
                else
                    ++currentCount;
            }

            if (toRelease != null)
                toRelease.SetResult(true);
        }
    }

    /// <summary>
    /// A mutual exclusion lock that is compatible with async. Note that this lock is <b>not</b> recursive!
    /// </summary>
    public class AsyncLock
    {
        private readonly AsyncSemaphore semaphore;
        private readonly Task<Releaser> releaser;

        /// <summary>
        /// Creates a new async-compatible mutual exclusion lock.
        /// </summary>
        public AsyncLock()
        {
            semaphore = new AsyncSemaphore(1);
            releaser = Task.FromResult(new Releaser(this));
        }

        /// <summary>
        /// Asynchronously acquires the lock. Returns a disposable that releases the lock when disposed.
        /// </summary>
        /// <returns>A disposable that releases the lock when disposed.</returns>
        public Task<Releaser> LockAsync()
        {
            var wait = semaphore.WaitAsync();
            return wait.IsCompleted ?
                releaser :
                wait.ContinueWith(
                    (_, state) => new Releaser(
                    (AsyncLock)state),
                    this, 
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, 
                    TaskScheduler.Default);
        }

        /// <summary>
        /// The disposable which releases the lock.
        /// </summary>
        public struct Releaser : IDisposable
        {
            private readonly AsyncLock toRelease;

            internal Releaser(AsyncLock toRelease)
            {
                this.toRelease = toRelease;
            }

            /// <summary>
            /// Release the lock.
            /// </summary>
            public void Dispose()
            {
                if (toRelease != null)
                    toRelease.semaphore.Release();
            }
        }
    }
}
