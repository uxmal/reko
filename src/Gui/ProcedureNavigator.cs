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
using System.Text;

namespace Reko.Gui
{
    /// <summary>
    /// Used to navigate to a procedure.
    /// </summary>
    public class ProcedureNavigator : ICodeLocation
    {
        private IServiceProvider sp;
        private Program program;

        public ProcedureNavigator(Program program, Procedure proc, IServiceProvider sp)
        {
            this.program = program;
            this.Procedure = proc;
            this.sp = sp;
        }

        public Procedure Procedure { get; private set; }

        #region ICodeLocation Members

        public string Text
        {
            get { return Procedure.Name; }
        }

        public void NavigateTo()
        {
            var codeSvc = sp.GetService<ICodeViewerService>();
            if (codeSvc != null)
                codeSvc.DisplayProcedure(program, Procedure, program.NeedsScanning);
        }
        #endregion

        public override string ToString()
        {
            return Procedure.Name;
        }
    }
}
