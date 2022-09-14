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

using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;
using Reko.Core;
using Reko.Core.Services;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools
{
    public class DiagnosticsViewModel : Tool
    {
        private List<(ICodeLocation, Diagnostic)> diagnosticItems;
        private List<KeyValuePair<ICodeLocation, Diagnostic>> pending;
        private SynchronizationContext syncCtx;

        public DiagnosticsViewModel(SynchronizationContext syncCtx, IServiceProvider services)
        {
            this.diagnosticItems = new();
            this.pending = new();
            this.FilteredDiagnostics = new();
            this.syncCtx = syncCtx;
            var settings = services.RequireService<ISettingsService>();
            this.Filter = (DiagnosticFilters) settings.Get(IDiagnosticsService.FilterSetting, -1);
        }

        public ObservableCollection<DiagnosticItem> FilteredDiagnostics { get; }

        public DiagnosticFilters Filter 
        {
            get { return filter; }
            set { this.RaiseAndSetIfChanged(ref this.filter, value); }
        }
        private DiagnosticFilters filter;

        public void ClearItems()
        {
            this.diagnosticItems.Clear();
            this.FilteredDiagnostics.Clear();
        }


        private bool AllowVisibleItem(Diagnostic diagnostic)
        {
            switch (diagnostic)
            {
            case ErrorDiagnostic _:
                return this.Filter.HasFlag(DiagnosticFilters.Errors);
            case WarningDiagnostic _:
                return this.Filter.HasFlag(DiagnosticFilters.Warnings);
            case InformationalDiagnostic _:
                return this.Filter.HasFlag(DiagnosticFilters.Information);
            default:
                return true;
            }
        }

        public void AddDiagnostic(ICodeLocation location, Diagnostic d)
        {
            this.diagnosticItems.Add((location, d));
            if (!AllowVisibleItem(d))
                return;
            this.FilteredDiagnostics.Add(CreateDiagnosticItem(location, d));
        }

        private DiagnosticItem CreateDiagnosticItem(ICodeLocation location, Diagnostic d)
        {
            return new DiagnosticItem
            {
                Location = location.Text,
                Description = d.Message
            };
        }
    }

    public class DiagnosticItem
    {
        public string? Location { get; set; }
        public string? Description { get; set; }
    }
}
