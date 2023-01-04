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
using Reko.Gui;
using Reko.Gui.Services;
using Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaProcedureListService : IProcedureListService
    {
        private IServiceProvider services;
        private ProcedureListViewModel procedureVm;

        public AvaloniaProcedureListService(IServiceProvider services, ProcedureListViewModel procedureVm)
        {
            this.services = services;
            this.procedureVm = procedureVm;
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
            procedureVm.Procedures.Clear();
        }

        public ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            throw new NotImplementedException();
        }

        public void Load(Project project)
        {
            procedureVm.LoadProcedures(project.Programs.SelectMany(program => program.Procedures.Values));
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