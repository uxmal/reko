#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools
{
    /// <summary>
    /// The view model for backing the procedure list view.
    /// </summary>
    public class ProcedureListViewModel : Tool
    {
        private List<ProcedureItem> modelProcedures;

        public ProcedureListViewModel()
        {
            this.searchCriterion = "";
            this.modelProcedures = new List<ProcedureItem>();
            this.procedures = new ObservableCollection<ProcedureItem>(modelProcedures);
        }

        public string SearchCriterion
        {
            get { return searchCriterion; }
            set { 
                if (string.IsNullOrEmpty(searchCriterion))
                {
                    this.Procedures = new ObservableCollection<ProcedureItem>(this.modelProcedures);
                }
                else
                {
                    var criterion = searchCriterion.Trim();
                    this.Procedures = new ObservableCollection<ProcedureItem>(
                        modelProcedures.Where(p => p.Name.Contains(criterion)));
                }
                this.RaiseAndSetIfChanged(ref searchCriterion, value, nameof(SearchCriterion));
            }
        }
        private string searchCriterion;

        public ObservableCollection<ProcedureItem> Procedures
        {
            get { return procedures; }
            set { this.RaiseAndSetIfChanged(ref procedures, value, nameof(Procedures)); }
        }
        private ObservableCollection<ProcedureItem> procedures;


        public void LoadProcedures(IEnumerable<Procedure> procedures)
        {
            this.modelProcedures.Clear();
            this.modelProcedures.AddRange(procedures.Select(CreateProcedureItem));
            this.Procedures = new(modelProcedures.Where(p => p.Name.Contains(searchCriterion.Trim())));
        }

        private ProcedureItem CreateProcedureItem(Procedure proc)
        {
            return new ProcedureItem(proc.Name, proc.EntryAddress.ToString());
        }

        public void Show()
        {
            var ow = this.Owner;
            if (ow is ToolDock toolDock)
            {
                toolDock.ActiveDockable = this;
                toolDock.FocusedDockable = this;
            }
        }

        public class ProcedureItem
        {
            public ProcedureItem(string name, string address)
            {
                this.Name = name; this.Address = address;
            }
            public string Name { get; }
            public string Address { get; }
        }
    }
}
