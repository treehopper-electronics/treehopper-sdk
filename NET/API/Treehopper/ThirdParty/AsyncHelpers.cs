using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Treehopper.ThirdParty
{
    // from https://blogs.msdn.microsoft.com/pfxteam/2012/02/12/building-async-coordination-primitives-part-6-asynclock/

    /// <summary>
    ///     An Async-compatible semaphore lock
    /// </summary>
    public class AsyncSemaphore
    {
        private static readonly Task Completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> _waiters = new Queue<TaskCompletionSource<bool>>();
        private int _currentCount;

        /// <summary>
        ///     Create a new lock
        /// </summary>
        /// <param name="initialCount">The initial count the semaphore should use</param>
        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0) throw new ArgumentOutOfRangeException(nameof(initialCount));
            _currentCount = initialCount;
        }

        /// <summary>
        ///     Wait for the semaphore to release
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public Task WaitAsync()
        {
            lock (_waiters)
            {
                if (_currentCount > 0)
                {
                    --_currentCount;
                    return Completed;
                }
                var waiter = new TaskCompletionSource<bool>();
                _waiters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        /// <summary>
        ///     Release the semaphore
        /// </summary>
        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (_waiters)
            {
                if (_waiters.Count > 0)
                    toRelease = _waiters.Dequeue();
                else
                    ++_currentCount;
            }

            toRelease?.SetResult(true);
        }
    }

    /// <summary>
    ///     A mutual exclusion lock that is compatible with async. Note that this lock is <b>not</b> recursive!
    /// </summary>
    public class AsyncLock
    {
        private readonly Task<Releaser> _releaser;
        private readonly AsyncSemaphore _semaphore;

        /// <summary>
        ///     Creates a new async-compatible mutual exclusion lock.
        /// </summary>
        public AsyncLock()
        {
            _semaphore = new AsyncSemaphore(1);
            _releaser = Task.FromResult(new Releaser(this));
        }

        /// <summary>
        ///     Asynchronously acquires the lock. Returns a disposable that releases the lock when disposed.
        /// </summary>
        /// <returns>A disposable that releases the lock when disposed.</returns>
        public Task<Releaser> LockAsync()
        {
            var wait = _semaphore.WaitAsync();
            return wait.IsCompleted
                ? _releaser
                : wait.ContinueWith(
                    (_, state) => new Releaser(
                        (AsyncLock) state),
                    this,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
        }

        /// <summary>
        ///     The disposable which releases the lock.
        /// </summary>
        public struct Releaser : IDisposable
        {
            private readonly AsyncLock _toRelease;

            internal Releaser(AsyncLock toRelease)
            {
                _toRelease = toRelease;
            }

            /// <summary>
            ///     Release the lock.
            /// </summary>
            public void Dispose()
            {
                _toRelease?._semaphore.Release();
            }
        }
    }
}