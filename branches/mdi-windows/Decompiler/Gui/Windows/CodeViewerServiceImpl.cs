/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Gui.Windows
{
    /// <summary>
    /// Service supporting the display and navigation of intermediate code.
    /// </summary>
    public class CodeViewerServiceImpl  : ViewService, ICodeViewerService
    {
        private CodeViewerPane pane;

        public CodeViewerServiceImpl(IServiceProvider sp) : base (sp)
        {
            pane = new CodeViewerPane();
        }

        public void DisplayProcedure(Procedure proc)
        {
            ShowWindow("codeViewerWindow", "Code Viewer", pane);
            pane.DisplayProcedure(proc);
        }
    }
}
