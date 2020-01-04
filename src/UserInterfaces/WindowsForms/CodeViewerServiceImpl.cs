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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Reko.Gui;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Service supporting the display and navigation of intermediate code.
    /// </summary>
    public class CodeViewerServiceImpl : ViewService, ICodeViewerService
    {
        public CodeViewerServiceImpl(IServiceProvider sp) : base (sp)
        {
        }

        public void DisplayProcedure(Program program, Procedure proc, bool mixedMode)
        {
            if (proc == null)
                return;
            if (mixedMode)
            {
                var pane = new CombinedCodeViewInteractor();
                var windowType = typeof(CombinedCodeViewInteractor).Name;
                var frame = ShowWindow(windowType, proc.Name, proc, pane);
                ((CombinedCodeViewInteractor)frame.Pane).DisplayProcedure(program, proc);
            }
            else
            {
                var pane = new CodeViewInteractor();
                var windowType = typeof(CombinedCodeViewInteractor).Name;
                var frame = ShowWindow(windowType, proc.Name, proc, pane);
                ((CodeViewInteractor)frame.Pane).DisplayProcedure(program, proc);
            }
        }

        public void DisplayStatement(Program program, Statement stm)
        {
            var pane = new CombinedCodeViewInteractor();
            var windowType = typeof(CombinedCodeViewInteractor).Name;
            var proc = stm.Block.Procedure;
            var frame = ShowWindow(windowType, proc.Name, proc, pane);
            ((CombinedCodeViewInteractor)frame.Pane).DisplayStatement(program, stm);
        }

        public void DisplayGlobals(Program program, ImageSegment segment)
        {
            var pane = new CombinedCodeViewInteractor();
            var windowType = typeof(CombinedCodeViewInteractor).Name;
            var label = string.Format(Resources.SegmentGlobalsFmt, segment.Name);
            var frame = ShowWindow(windowType, label, segment, pane);
            ((CombinedCodeViewInteractor)frame.Pane).DisplayGlobals(program, segment);
        }

        public void DisplayDataType(Program program, DataType dt)
        {
            throw new NotImplementedException();
        }
    }
}
