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

using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.ViewModels
{
    public class ProgramPropertiesViewModel : ReactiveObject
    {
        public ProgramPropertiesViewModel(ISet<string> heuristics)
        {
            this.ScanHeuristics = new ObservableCollection<ScanHeuristic>
            {
                new ScanHeuristic(
                    Scanning.ScannerHeuristics.Shingle,
                    "Shingle heuristic",
                    "Tries to discover basic blocks of executable code in regions not reached by " +
                        "ordinary recursive scanning."),
                new ScanHeuristic(
                    Scanning.ScannerHeuristics.UserMode,
                    "Only allow user mode instructions",
                    "Treats privileged or system instructions as invalid when scanning."),
                new ScanHeuristic(
                    Scanning.ScannerHeuristics.Unlikely,
                    "Treat 'unlikely' instructions as invalid",
                    "Treats validly dcode machine instructions as invalid if they are considered " +
                        "'unlikely'. For example, performing multiplications involving the stack register " +
                        "would be considered unlikely in most architectures.")
            };
            foreach (var sch in this.ScanHeuristics)
            {
                sch.IsChecked = heuristics.Contains(sch.Id);
            }
            this.scanHeuristicDescription = "";
        }

        //$TODO
        public List<string> OutputFileDispositions { get; set; } = new List<string>();

        public ObservableCollection<ScanHeuristic> ScanHeuristics { get; }


        public string ScanHeuristicDescription {
            get => scanHeuristicDescription;
            set => this.RaiseAndSetIfChanged(ref scanHeuristicDescription, value);
        }
        private string scanHeuristicDescription;
    }


    public class ScanHeuristic : ReactiveObject
    {
        public ScanHeuristic(string id, string text, string description)
        {
            this.Id = id;
            this.text = text;
            this.description = description;
        }

        public string Id { get; set; }

        public string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
        }
        private string text;

        public string Description
        {
            get => description;
            set => this.RaiseAndSetIfChanged(ref description, value);
        }
        private string description;


        public bool IsChecked
        {
            get => isChecked;
            set => this.RaiseAndSetIfChanged(ref isChecked, value);
        }
        private bool isChecked;
    }
}
