#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Graphs;
using Reko.Core.Services;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.Concurrent
{
    /// <summary>
    /// This class coordinates the execution of work units on the .NET
    /// thread pool. The work units are obtained from the strongly connected
    /// components (SCC)s of a directed graph.
    /// </summary>
    public class SccWorkerCoordinator<TItem>
        where TItem : notnull
    {
        private readonly Dictionary<int, Worker> workers;
        private readonly Condensation<TItem> condensation;
        private readonly Action<TItem[]> workAction;
        private readonly IEventListener listener;

        public SccWorkerCoordinator(
            Condensation<TItem> condensation,
            IEventListener listener,
            Action<TItem[]> workAction)
        {
            this.condensation = condensation;
            this.workAction = workAction;
            this.workers = condensation.Graph.Nodes.ToDictionary(m => m, MakeWorker);
            this.listener = listener;
        }

        /// <summary>
        /// Starts processing the nodes of the condensed SCC DAG, starting 
        /// from the leaves (nodes with no successors) towards the roots 
        /// (nodes with no predecessors). The result is a "swarm" of workers
        /// running on the thread pool. When this method returns, the swarm
        /// is most likely still executing, so callers need to await the
        /// completion of the swarm.
        /// </summary>
        /// <returns>A <see cref="Task"/> which can be awaited until the
        /// swarm of workers have processed the whole graph.
        /// </returns>
        public Task RunAsync()
        {
            var leaves = new List<Worker>(); // these workers are the start of the processs.
            var roots = new List<Task>();
            foreach (var node in condensation.Graph.Nodes)
            {
                if (condensation.Graph.Successors(node).Count == 0)
                    leaves.Add(this.workers[node]);
                if (condensation.Graph.Predecessors(node).Count == 0)
                    roots.Add(this.workers[node].ComputationDone);
            }
            foreach (var leaf in leaves)
            {
                Task.Run(() => RunTask(leaf));
            }
            return Task.WhenAll(roots);
        }

        /// <summary>
        /// Represents the worker for a particular SCC. It will not
        /// start working until it has been poked using the 
        /// <see cref="SuccessorCompleted"/> method once for each
        /// of its successors.
        /// </summary>
        private class Worker
        {
            private readonly TaskCompletionSource tcs;
            private int successorsLeft;

            public Worker(int sccId, int successors)
            {
                this.SccId = sccId;
                this.tcs = new TaskCompletionSource();
                this.successorsLeft = successors;
            }

            public int SccId { get; }

            public Task ComputationDone => tcs.Task;

            /// <summary>
            /// This method should be called each time a successor has 
            /// completed execution. 
            /// </summary>
            /// <returns>Returns true if all successors of this SCC
            /// have been completed.
            /// </returns>
            /// <remarks>
            /// Since this method could be called from many threads,
            /// we shared mutable state <see cref="successorsLeft"/>
            /// us mutated in a thread-safe manner.
            /// </remarks>
            public bool SuccessorCompleted()
            {
                var n = Interlocked.Decrement(ref successorsLeft);
                return n == 0;
            }

            public void SetResult()
            {
                tcs.SetResult();
            }
        }

        private Worker MakeWorker(int sccId)
        {
            var csucc = condensation.Graph.Successors(sccId).Count;
            return new Worker(sccId, csucc);
        }


        private void RunTask(Worker leaf)
        {
            var members = condensation.Members[leaf.SccId];
            workAction.Invoke(members);
            leaf.SetResult();
            WakenPredecessors(leaf.SccId);
        }

        private void WakenPredecessors(int sccId)
        {
            foreach (var pred in condensation.Graph.Predecessors(sccId))
            {
                if (listener.IsCanceled())
                    return;
                var w = this.workers[pred];
                if (w.SuccessorCompleted())
                    Task.Run(() => RunTask(w));
            }
        }
    }
}
