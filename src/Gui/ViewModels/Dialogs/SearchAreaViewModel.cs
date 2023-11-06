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

using Reko.Core;
using Reko.Gui.Reactive;
using Reko.Gui.Services;
using Reko.Gui.ViewModels.Documents;
using Reko.Scanning;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Gui.ViewModels.Dialogs
{
    public class SearchAreaViewModel : ChangeNotifyingObject
    {
        private readonly Program program;

        public SearchAreaViewModel(Program program, List<SearchArea> searchArea, IUiPreferencesService uiPreferencesSvc)
        {
            this.program = program;
            this.SegmentList = LoadSegmentDetails(program);
            this.UiPreferencesSvc = uiPreferencesSvc;
            this.freeFormAreas = "";
            this.freeFormError = "";
            this.Areas = new();
        }

        private ObservableCollection<SegmentListItemViewModel> LoadSegmentDetails(Program program)
        {
            return new ObservableCollection<SegmentListItemViewModel>(
                program.SegmentMap.Segments.Values.Select(
                    SegmentListItemViewModel.FromImageSegment));
        }

        public ObservableCollection<SegmentListItemViewModel> SegmentList { get; }

        public IUiPreferencesService UiPreferencesSvc { get; }

        public string FreeFormAreas
        {
            get { return freeFormAreas; }
            set {
                RaiseAndSetIfChanged(ref freeFormAreas, value);
                ParseFreeFormAreas();
            }
        }
        private string freeFormAreas;

        public List<SearchArea> Areas { get; set; }

        public string FreeFormError
        {
            get { return freeFormError; }
            set { RaiseAndSetIfChanged(ref freeFormError, value); }
        }
        private string freeFormError;


        private void ParseFreeFormAreas()
        {
            this.Areas.Clear();
            if (!SearchArea.TryParse(program, freeFormAreas, out var sa))
            {
                this.Areas.Clear();
                FreeFormError = "Invalid range syntax.";
            }
            else
            {
                this.Areas = sa;
                FreeFormError = "";
            }
        }

        public void ChangeAreas(IEnumerable selectedItems)
        {
            // Because user checked in the list box, we clear the 
            // free form text area.
            this.FreeFormAreas = "";
            this.Areas.Clear();
            foreach (SegmentListItemViewModel sitem in selectedItems)
            {
                var segment = sitem.Segment;
                this.Areas.Add(SearchArea.FromSegment(program, segment!));
            }
        }
    }
}
