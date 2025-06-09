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
using Reko.Gui;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UserInterfaces.WindowsForms
{
    public class DisassemblyViewServiceImpl : ViewService, IDisassemblyViewService
    {
        private DisassemblyViewInteractor dvi;

        public const string ViewWindowType = "disassemblyViewWindow";

        public DisassemblyViewServiceImpl(IServiceProvider services)
            : base(services)
        {
        }

        #region IDisassemblyViewService Members

        public void DisassembleStartingAtAddress(Program program, Address addr)
        {
            ShowWindow(program);
            dvi.StartAddress = addr;
            dvi.Program = program;
            dvi.DumpAssembler();
        }

        public void Clear()
        {
            dvi.ClearText();
        }

        public void ShowWindow(Program program)
        {
            if (dvi is null)
            {
                dvi = new DisassemblyViewInteractor();
            }
            base.ShowWindow(ViewWindowType, "Disassembly", program, dvi);
        }

        #endregion
    }
}
