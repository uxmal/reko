#region License
/* 
 * Copyright (C) 1999-2022 Pavel Tomin.
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
using Reko.Core.Scripts;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui
{
    public class ScriptErrorNavigator : ICodeLocation
    {
        private readonly ScriptError error;
        private readonly IServiceProvider sp;

        public ScriptErrorNavigator(ScriptError error, IServiceProvider sp)
        {
            this.error = error;
            this.sp = sp;
        }

        #region ICodeLocation Members

        public string Text => Path.GetFileName(error.FileName);

        public ValueTask NavigateTo()
        {
            var editorSvc = sp.GetService<ITextFileEditorService>();
            editorSvc?.DisplayFile(error.FileName, error.LineNumber);
            var stackTraceSvc = sp.GetService<IStackTraceService>();
            stackTraceSvc?.DisplayStackTrace(error.StackFrames);
            return ValueTask.CompletedTask;

        }

        #endregion
    }
}
