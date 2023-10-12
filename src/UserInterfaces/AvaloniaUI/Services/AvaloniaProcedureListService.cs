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
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Services;
using Reko.Gui.ViewModels.Tools;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaProcedureListService : IProcedureListService
    {
        private readonly IServiceProvider services;
        private readonly ProcedureListToolViewModel procedureVm;

        public AvaloniaProcedureListService(IServiceProvider services, ProcedureListToolViewModel procedureVm)
        {
            this.services = services;
            this.procedureVm = procedureVm;

            this.procedureVm.PropertyChanged += ProcedureVm_PropertyChanged;
        }

        private void ProcedureVm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProcedureListViewModel.SelectedProcedure))
            {
                var pp = procedureVm.ProcedureList?.SelectedProcedure;
                if (pp is null)
                    return;
                services.RequireService<ISelectedAddressService>().SelectedProcedure = pp.Procedure;
                services.RequireService<ICodeViewerService>().DisplayProcedure(pp.Program, pp.Procedure, pp.Program.NeedsScanning);
            }
        }

        public bool ContainsFocus
        {
            get
            {
                return false; //$TODO:
            }
        }

        public void Clear()
        {
            procedureVm.ProcedureList?.Procedures.Clear();
        }

        public ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            throw new NotImplementedException();
        }

        public void Load(Project project)
        {
            procedureVm.ProcedureList = new ProcedureListViewModel(services);
            procedureVm.ProcedureList?.LoadProcedures(project.Programs.SelectMany(program =>
                program.Procedures.Values.Select(p => (program, p))));
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            procedureVm.Show();
        }
    }
}