#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Text;
using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;
using Reko.Gui;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools
{
    public class VisualizerViewModel : Tool
    {

        public List<ListOption> Visualizers { get; } = new List<ListOption>()
        {
            new ListOption("V1", "vis-1"),
            new ListOption("V2", "vis-2"),
            new ListOption("V3", "vis-3"),
        };

        public VisualizerViewModel()
        {
            this.selectedVisualizer = Visualizers[0];
        }


        public ListOption SelectedVisualizer {
            get => selectedVisualizer;
            set { this.RaiseAndSetIfChanged(ref selectedVisualizer, value, nameof(SelectedVisualizer)); }
        }
        private ListOption selectedVisualizer;
    }
}
