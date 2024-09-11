#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

using DynamicData;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Gui.Reactive;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Reko.Gui.ViewModels.Tools
{
    public class CallGraphNavigatorViewModel : ChangeNotifyingObject
    {
        private readonly Program? program;
        private readonly CallGraph graph;
        private readonly ICodeViewerService codeViewerSvc;
        private readonly HashSet<ProcedureBase> visited;
        private CallGraphViewModelItem? currentNode;

        public CallGraphNavigatorViewModel(
            Program? program,
            CallGraph graph,
            ICodeViewerService codeViewerSvc)
        {
            this.program = program;
            this.graph = graph;
            this.codeViewerSvc = codeViewerSvc;
            this.visited = new HashSet<ProcedureBase>();
            Predecessors = new ObservableCollection<CallGraphViewModelItem>(CollectRoots());
            Successors = new ObservableCollection<CallGraphViewModelItem>();
            NavigateTo((Procedure?) null);
        }

        public CallGraphNavigatorViewModel(
            Program? program,
            CallGraph graph,
            Procedure? proc,
            ICodeViewerService codeViewerSvc)
        {
            this.program = program;
            this.graph = graph;
            this.codeViewerSvc = codeViewerSvc;
            visited = new HashSet<ProcedureBase>();
            Predecessors = new ObservableCollection<CallGraphViewModelItem>();
            Successors = new ObservableCollection<CallGraphViewModelItem>();
            NavigateTo(proc);
        }

        public ObservableCollection<CallGraphViewModelItem> Predecessors { get; }
        public ObservableCollection<CallGraphViewModelItem> Successors { get; }

        public CallGraphViewModelItem? NodeObject => currentNode;

        public string? NodeTitle
        {
            get => nodeTitle;
            set
            {
                RaiseAndSetIfChanged(ref nodeTitle, value);
            }
        }
        private string? nodeTitle;

        public string? NodeDescription
        {
            get => nodeDescription;
            set
            {
                RaiseAndSetIfChanged(ref nodeDescription, value);
            }
        }
        private string? nodeDescription;

        public string? NodeDetails
        {
            get => nodeDetails;
            set
            {
                RaiseAndSetIfChanged(ref nodeDetails, value);
            }
        }
        private string? nodeDetails;

        private IEnumerable<CallGraphViewModelItem> CollectRoots()
        {
            return MakeViewModelItems(graph.Procedures.Nodes
                .Where(p => graph.Procedures.Predecessors(p).Count == 0));
        }

        private IEnumerable<CallGraphViewModelItem> MakeViewModelItems(IEnumerable<ProcedureBase> procs)
        {
            return procs
                .Select(p => (p, p.Name, p is Procedure proc ? proc.EntryAddress : null))
                .DistinctBy(p => p.Name)
                .OrderBy(p => p.Name)
                .Select(p => MakeViewModelItem(p.p));
        }

        private CallGraphViewModelItem MakeViewModelItem(ProcedureBase? p)
        {
            bool isVisited = p is not null
                ? visited.Contains(p)
                : false;
            return p switch
            {
                Procedure proc => new CallGraphViewModelItem(
                    proc.Name, proc.EntryAddress.ToString(), proc, isVisited),
                ExternalProcedure ep => new CallGraphViewModelItem(
                    ep.Name, "", ep, isVisited),
                ProcedureBase callable => new CallGraphViewModelItem(
                    callable.Name, "", null!, isVisited),
                _ => CallGraphViewModelItem.Empty(),
            };
        }

        private IEnumerable<CallGraphViewModelItem> GetPredecessors(CallGraphViewModelItem item)
        {
            return MakeViewModelItems(graph.CallerProcedures(item.NodeObject));
        }

        private IEnumerable<CallGraphViewModelItem> GetSuccessors(CallGraphViewModelItem item)
        {
            switch (item.NodeObject)
            {
            case Procedure proc:
                var calleeCollector = new CalleeCollector(this, proc);
                return calleeCollector.FindCallees();
            default:
                return Enumerable.Empty<CallGraphViewModelItem>();
            }
        }

        public void NavigateTo(ProcedureBase? proc)
        {
            if (proc is null)
            {
                if (currentNode is null)
                    return;
                NavigateTo(CallGraphViewModelItem.Empty());
            }
            else
            {
                if (currentNode is not null && currentNode.NodeObject == proc)
                    return;
                NavigateTo(MakeViewModelItem(proc));
            }
        }

        public void NavigateTo(CallGraphViewModelItem? item)
        {
            currentNode = item;
            if (item is null || item.NodeObject is not Procedure &&
                item.NodeObject is not ExternalProcedure)
            {
                NodeTitle = "(No selection)";
                NodeDescription = "";
                NodeDetails = "";
                Predecessors.Clear();
                Successors.Clear();
                return;
            }
            visited.Add(item.NodeObject);
            var preds = GetPredecessors(item);
            Predecessors.Clear();
            Predecessors.AddRange(preds);
            var succs = GetSuccessors(item);
            Successors.Clear();
            Successors.AddRange(succs);

            NodeTitle = item.Title;
            NodeDescription = item.Description;
            NodeDetails = item.Details;
        }

        public void ShowProcedure(ProcedureBase callable)
        {
            if (program is null)
                return;
            if (callable is not Procedure proc)
                return;
            codeViewerSvc.DisplayProcedure(program, proc, program.NeedsScanning);
        }

        public class CalleeCollector : InstructionVisitorBase
        {
            private readonly CallGraphNavigatorViewModel outer;
            private Procedure proc;
            private List<ProcedureBase> callees;

            public CalleeCollector(CallGraphNavigatorViewModel outer, Procedure proc)
            {
                this.outer = outer;
                this.proc = proc;
                callees = new List<ProcedureBase>();
            }

            public IEnumerable<CallGraphViewModelItem> FindCallees()
            {
                foreach (var stm in proc.Statements)
                {
                    stm.Instruction.Accept(this);
                }
                return outer.MakeViewModelItems(callees);
            }

            public override void VisitProcedureConstant(ProcedureConstant pc)
            {
                callees.Add(pc.Procedure);
            }
        }
    }
}
