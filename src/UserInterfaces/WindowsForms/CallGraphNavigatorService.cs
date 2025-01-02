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

using Reko.Core;
using Reko.Core.Graphs;
using Reko.Core.Services;
using Reko.Gui.Services;
using Reko.Gui.ViewModels;
using Reko.Gui.ViewModels.Tools;
using Reko.UserInterfaces.WindowsForms.Forms;
using System;

#nullable enable

namespace Reko.UserInterfaces.WindowsForms
{
    public class CallGraphNavigatorService : ICallGraphNavigatorService, IWindowPane
    {
        private readonly IServiceProvider services;
        private readonly CallGraphNavigatorView view;
        private ISelectedAddressService? selSvc;
        private ICodeViewerService codeViewerSvc;
        private Program? program;

        public CallGraphNavigatorService(IServiceProvider services, CallGraphNavigatorView callGraphNavigatorView)
        {
            this.services = services;
            this.selSvc = services.RequireService<ISelectedAddressService>();
            this.codeViewerSvc = services.RequireService<ICodeViewerService>();
            this.selSvc.SelectedProcedureChanged += SelSvc_SelectedProcedureChanged;
            this.view = callGraphNavigatorView;
            this.view.ViewModel = new CallGraphNavigatorViewModel(program, new CallGraph(), codeViewerSvc);
        }

        public IWindowFrame? Frame { get; set; }

        public void Close()
        {
        }

        public object CreateControl()
        {
            return view;
        }

        public void SetSite(IServiceProvider services)
        {

        }

        public void Show(Program? program, Procedure? proc)
        {
            if (this.program != program)
            {
                this.program = program;
                var callGraph = (program is not null)
                    ? program.CallGraph
                    : new CallGraph();
                this.view.ViewModel = new CallGraphNavigatorViewModel(program, callGraph, proc, codeViewerSvc);
            }
            else
            {
                this.view.ViewModel.NavigateTo(proc);
            }
        }

        private void SelSvc_SelectedProcedureChanged(object? sender, EventArgs e)
        {
            if (selSvc is null)
                return;
            Show(selSvc.SelectedProgram, selSvc.SelectedProcedure);
        }
    }
}
