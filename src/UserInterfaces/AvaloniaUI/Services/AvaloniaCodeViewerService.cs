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
using Reko.Core.Loading;
using Reko.Core.Types;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaCodeViewerService : ICodeViewerService
    {
        public void DisplayDataType(Program program, DataType dt)
        {
            throw new NotImplementedException();
        }

        public void DisplayGlobals(Program program, ImageSegment segment)
        {
            throw new NotImplementedException();
        }

        public void DisplayProcedure(Program program, Procedure proc, bool mixedMode)
        {
            //$TODO:display the code of this procedure.
        }

        public void DisplayProcedureControlGraph(Program program, Procedure proc)
        {
            throw new NotImplementedException();
        }

        public void DisplayStatement(Program program, Statement statement)
        {
            throw new NotImplementedException();
        }
    }
}
