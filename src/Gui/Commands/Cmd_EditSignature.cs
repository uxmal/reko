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

#nullable enable

using Reko.Core;
using Reko.Gui.Forms;
using Reko.Gui.Services;
using System;
using System.Threading.Tasks;

namespace Reko.Gui.Commands
{
    public class Cmd_EditSignature : Command
    {
        private readonly Program program;
        private readonly Procedure procedure;
        private readonly Address address;

        public Cmd_EditSignature(IServiceProvider services, Program program, Procedure procedure, Address addr)
            : base(services)
        {
            this.program = program;
            this.procedure = procedure;
            this.address = addr;
        }

        public async override ValueTask DoItAsync()
        {
            var dlgFactory = Services.RequireService<IDialogFactory>();
            var uiSvc = Services.RequireService<IDecompilerShellUiService>();
            if (!program.User.Procedures.TryGetValue(address, out var proc))
                proc = new UserProcedure(address, procedure.Name);
            using (IProcedureDialog dlg = dlgFactory.CreateProcedureDialog(program, proc))
            {
                if (DialogResult.OK == await uiSvc.ShowModalDialog(dlg))
                {
                    //$TODO: move applychanges out!
                    dlg.ApplyChanges();
                    program.User.Procedures[address] = proc;
                    if (procedure != null)
                        procedure.Name = proc.Name;
                }
            }
        }
    }
}
