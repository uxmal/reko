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
using Reko.Core.Services;
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
            if (!program.User.Procedures.TryGetValue(address, out var userproc))
                userproc = new UserProcedure(address, procedure.Name);
            using (IDialog<UserProcedure?> dlg = dlgFactory.CreateProcedureDialog(program, userproc))
            {
                var newProc = await uiSvc.ShowModalDialog(dlg);
                if (newProc is { })
                {
                    program.User.Procedures[address] = newProc;
                    if (procedure is not null)
                        procedure.Name = newProc.Name;
                    if (newProc.Signature is { })
                    {
                        // Edit Signature could happen before scanning has been done,
                        // so there are no program.Procedures yet.
                        if (!program.Procedures.TryGetValue(address, out var proc))
                            return;
                        var ser = program.CreateProcedureSerializer();
                        proc.Signature = ser.Deserialize(newProc.Signature, proc.Frame)!;
                    }
                }
            }
        }
    }
}
