#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Gui.Windows
{
    /// <summary>
    /// Service supporting the display and navigation of intermediate code.
    /// </summary>
    public class CodeViewerServiceImpl : ViewService, ICodeViewerService
    {
        public CodeViewerServiceImpl(IServiceProvider sp) : base (sp)
        {
        }

        public void DisplayProcedure(Program program, Procedure proc)
        {
            if (proc == null)
                return;
            var pane = new CodeViewerPane();
            var frame = ShowWindow("codeViewerWindow", proc.Name, proc, pane);
            pane.FrameWindow = frame;
            pane.DisplayProcedure(program, proc);
        }

        public void DisplayDataType(Program program, DataType dt)
        {
            if (dt == null)
                return;
            var pane = new CodeViewerPane();
            var frame = ShowWindow("codeViewerWindow", dt.Name, dt, pane);
            pane.FrameWindow = frame;
            pane.DisplayDataType(program, dt);
        }
    }
}
