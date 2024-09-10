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

using Reko.Core;
using Reko.Core.Services;
using Reko.Gui.Reactive;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Reko.Gui.ViewModels.Tools
{
    /// <summary>
    /// The view model for backing the procedure list view.
    /// </summary>
    public class ProcedureListViewModel : ChangeNotifyingObject
    {
        private readonly IServiceProvider services;
        private readonly List<ProcedureItem> modelProcedures;

        public ProcedureListViewModel(IServiceProvider services)
        {
            this.services = services;
            this.searchCriterion = "";
            this.baseFilter = ProcedureBaseFilter.All;
            this.modelProcedures = new List<ProcedureItem>();
            this.procedures = new ObservableCollection<ProcedureItem>(modelProcedures);
        }

        public string SearchCriterion
        {
            get { return searchCriterion; }
            set {
                bool changed = this.searchCriterion != value;
                this.RaiseAndSetIfChanged(ref searchCriterion, value, nameof(SearchCriterion));
                if (changed) 
                    Procedures = this.ApplyFilterCriteria();
            }
        }
        private string searchCriterion;

        public ObservableCollection<ProcedureItem> Procedures
        {
            get { return procedures; }
            set { this.RaiseAndSetIfChanged(ref procedures, value, nameof(Procedures)); }
        }
        private ObservableCollection<ProcedureItem> procedures;

        public ProcedureItem? SelectedProcedure
        {
            get { return procSelected; }
            set { this.RaiseAndSetIfChanged(ref procSelected, value); }
        }
        private ProcedureItem? procSelected;


        public ProcedureBaseFilter BaseFilter
        {
            get { return baseFilter; }
            set {
                bool changed = baseFilter != value;
                this.RaiseAndSetIfChanged(ref baseFilter, value);
                if (changed)
                    Procedures = ApplyFilterCriteria();
            }
        }
        private ProcedureBaseFilter baseFilter;



        public void LoadProcedures(IEnumerable<(Program, Procedure)> procedures)
        {
            this.modelProcedures.Clear();
            this.modelProcedures.AddRange(procedures.Select(CreateProcedureItem));
            this.Procedures = ApplyFilterCriteria();
        }

        private ObservableCollection<ProcedureItem> ApplyFilterCriteria()
        {
            var filteredProcs = new ObservableCollection<ProcedureItem>();
            if (modelProcedures.Count > 0)
            {
                var program = modelProcedures[0].Program;
                foreach (var proc in modelProcedures)
                {
                    var c = searchCriterion.Trim();
                    if (!string.IsNullOrWhiteSpace(c) &&
                        !proc.Name.Contains(c) &&
                        !proc.Address.Contains(c))
                        continue;
                    switch (this.baseFilter)
                    {
                    case ProcedureBaseFilter.Roots:
                        if (!program.CallGraph.IsRootProcedure(proc.Procedure))
                            continue;
                        break;
                    case ProcedureBaseFilter.Leaves:
                        if (!program.CallGraph.IsLeafProcedure(proc.Procedure))
                            continue;
                        break;
                    }
                    filteredProcs.Add(proc);
                }
            }
            return filteredProcs;
        }

        private ProcedureItem CreateProcedureItem((Program program, Procedure proc) pp)
        {
            return new ProcedureItem(
                pp.proc.Name, 
                GenerateDecoratedName(pp.program, pp.proc),
                pp.proc.EntryAddress.ToString(),
                pp.program,
                pp.proc);
        }

        public void GotFocus()
        {
            var plSvc = this.services.GetService<IProcedureListService>();
            if (plSvc is { })
            {
                this.services.RequireService<ICommandRouterService>().ActiveCommandTarget = plSvc;
            }
        }

        public class ProcedureItem : ChangeNotifyingObject
        {
            public ProcedureItem(string name, string decoratedName, string address, Program program, Procedure proc)
            {
                this.Name = name;
                this.decoratedName = decoratedName;
                this.Address = address;
                this.Program = program;
                this.Procedure = proc;
            }

            public string Name { get; }
            public string DecoratedName
            {
                get => decoratedName;
                set
                {
                    base.RaiseAndSetIfChanged(ref decoratedName, value);
                }
            }
            private string decoratedName;
            public string Address { get; }
            public Program Program { get; }
            public Procedure Procedure { get; }
        }

        public static string GenerateDecoratedName(Program program, Procedure procedure)
        {
            var name = procedure.Name;
            if (program.User.DebugTraceProcedures.Contains(name))
                name += " (D)";
            return name;
        }

    }

}
