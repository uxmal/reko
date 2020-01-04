#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Gui
{
    public class JumpVectorNavigator : ICodeLocation
    {
        private readonly IServiceProvider services;

        public JumpVectorNavigator(Program program, Address addrInstr, Address addrVector, int stride, IServiceProvider services)
        {
            this.Program = program;
            this.IndirectJumpAddress = addrInstr;
            this.VectorAddress = addrVector;
            this.Stride = stride;
            this.services = services;
        }

        public Address IndirectJumpAddress { get; private set; }
        public Address VectorAddress { get; private set; }
        public Program Program { get; private set; }
        public int Stride { get; private set; }

        public string Text { get { return IndirectJumpAddress.ToString();  } }

        public void NavigateTo()
        {
            var svc = services.RequireService<ILowLevelViewService>();
            svc.ShowMemoryAtAddress(Program, IndirectJumpAddress);

            var dlgSvc = services.RequireService<IDialogFactory>();
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            var instr = Program.CreateDisassembler(Program.Architecture, IndirectJumpAddress).First();
            using (var dlg = dlgSvc.CreateJumpTableDialog(Program, instr, VectorAddress, Stride))
            {
                if (DialogResult.OK == uiSvc.ShowModalDialog(dlg))
                {
                    var ujmp = dlg.GetResults();
                    this.Program.User.JumpTables[ujmp.Address] = ujmp.Table;
                    
                    ///$TODO: register
                    //$TODO: prevent user from proceeding, in effect forcing 
                    // a restart.
                }
            }
        }
    }
}
