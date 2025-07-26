#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 .
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.Workers
{
    /// <summary>
    /// A worker class modeled after Erlang's processes. Subclasses should expose
    /// methods that allow clients to enqueue requests, and implement
    /// <see cref="ProcessMessage(TMessage)"/> to execute requests. The queue
    /// provides the synchronization mechanism.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public abstract class AbstractTaskWorker<TMessage>
    {
        private const int StateIdle = 0;
        private const int StateRunning = 1;
        private const int StateFinishing = 2;

        private ConcurrentQueue<TMessage> queue;
        private volatile int isTaskRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTaskWorker{TMessage}"/> class.
        /// </summary>
        protected AbstractTaskWorker()
        {
            this.queue = new ConcurrentQueue<TMessage>();
        }

        /// <summary>
        /// Subclasses should call this method to serialize requests and 
        /// put them in a queue.
        /// </summary>
        /// <param name="message"></param>
        protected void SendMessage(TMessage message)
        {
            queue.Enqueue(message);
            if (IsTaskNeeded())
            {
                // It wasn't running but is now: we need a new task to process the queue.
                Task.Run(ProcessQueue);
            }
        }

        private void ProcessQueue()
        {
            Debug.Assert(isTaskRunning == StateRunning , "Should be processing the queue.");
            do
            {
                while (queue.TryDequeue(out var message))
                {
                    ProcessMessage(message);
                }
            } while (TryFinishing());
        }

        /// <summary>
        /// After enqueuing a <typeparamref name="TMessage"/> we ensure that someone is working on it.
        /// </summary>
        /// <returns>True if there is no need to start a task.</returns>
        private bool IsTaskNeeded()
        {
            for (; ; )
            {
                switch (Interlocked.CompareExchange(ref isTaskRunning, StateRunning, StateIdle))
                {
                case StateIdle: return true;
                case StateFinishing: continue; // busy wait until other task has left 
                default: return false;
                }
            }
        }


        private bool TryFinishing()
        {
            // Indicate that we're trying to stop. We have to guard against a race
            // condition.
            // Other threads will busy wait until we either resume this thread because 
            // a message arrived in the queue, or because no new message arrived.
            var result = Interlocked.CompareExchange(ref isTaskRunning, StateFinishing, StateRunning);
            Debug.Assert(result == StateFinishing);
            if (queue.IsEmpty)
            {
                // No new message arrived; we definitely quit this thread.
                result = Interlocked.CompareExchange(ref isTaskRunning, StateIdle, StateFinishing);
                Debug.Assert(result == StateIdle);
                return true;
            }
            else
            {
                // A message has arrived while we were finishing;
                // restart  the thread.
                result = Interlocked.CompareExchange(ref isTaskRunning, StateRunning, StateFinishing);
                Debug.Assert(result == StateRunning);
                return false;
            }
        }

        /// <summary>
        /// Handles a message that has been sent to this worker.
        /// </summary>
        /// <param name="message">Message to process.</param>
        protected abstract void ProcessMessage(TMessage message);
    }
}
