#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
 *
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.Gui
{
    /// <summary>
    /// This class is used to register interest in events happening on a different thread.
    /// The different entry points support different modes of 
    /// </summary>
    public class EventBus
    {
        /// <summary>
        /// Registers to an event using the <paramref name="register"/> delegate.
        /// The EventHandler <paramref name="e"/> will be called on the current synchronization
        /// context. If more events arrive, their arrival is noted, and once the call
        /// on thethat listense to frequent event source. The 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="register">
        /// A delegate that registers the event you wish to listen to. You should provide
        /// a small function that just registers to en event and returns:
        /// <code>
        /// bus.RegisterSingleEventMailBox(h => object_to_listen_to.EventToListenTo += h, ...)
        /// </code>
        /// </param>
        /// <param name="e">
        /// Event handler that gets called on the current thread. It will be repeatedly
        /// called as long as pending events have arrived from other threads.
        /// </param>
        /// <returns>A </returns>
        public void RegisterSingleEventMailbox(Action<EventHandler> register, EventHandler e)
        {
            var sem = new SingleEventMailbox(SynchronizationContext.Current, e);
            register(sem.HandleEvent);
        }


        public class SingleEventMailbox
        {
            int moreWork;
            private EventHandler guiHandler;
            private SynchronizationContext ctx;
            private const bool useCtx = true;

            public SingleEventMailbox(SynchronizationContext ctx, EventHandler guiHandler)
            {
                this.ctx = ctx;
                this.guiHandler = guiHandler;
                if (!useCtx)
                System.Windows.Forms.Application.Idle += Application_Idle;
            }

            // An event has happened in the worker thread. We now have to dispatch 
            // the event to the gui thread
            public void HandleEvent(object sender, EventArgs e)
            {
                Debug.Print("HandleEvent: on thread {0}{1}",
                    Thread.CurrentThread.Name,
                    Thread.CurrentThread.ManagedThreadId);

                int newWork = Interlocked.Increment(ref this.moreWork);
                if (newWork == 1)
                {
                    // The UI thread wasn't busy, now it is. Start notification on the receiving thread. 
                    if (useCtx)
                    {
                        ctx.Post(new SendOrPostCallback(Worker), sender);
                    }
                    else
                    {
                        Debug.Print("Waiting for the Application.Idle event");
                    }
                }
            }

            private void Application_Idle(object sender, EventArgs e)
            {
                int wasMoreWork = Interlocked.Exchange(ref this.moreWork, 0);
                if (wasMoreWork > 0)
                {
                    Debug.Print("Worker: on thread {0}{1}", Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId);
                    this.guiHandler(sender, EventArgs.Empty);
                }
            }


            private void Worker(object sender)
            {
                Debug.Print("Worker: on thread {0}{1}", Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId);

                this.guiHandler(sender, EventArgs.Empty);
                int wasMoreWork = Interlocked.Exchange(ref this.moreWork, 0);
                if (wasMoreWork > 0)
                {
                    // more work arrived. We need to start again, but we want the UI thread 
                    // to have a chance to pump some messages. So we post again.
                    ctx.Post(new SendOrPostCallback(Worker), sender);
                }
            }
        }
    }
}
