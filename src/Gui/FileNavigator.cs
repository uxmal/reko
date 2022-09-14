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
using Reko.Core.Services;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui
{
    /// <summary>
    /// Used to navigate to a text file.
    /// </summary>
    public class FileNavigator : ICodeLocation
    {
        private readonly IServiceProvider sp;
        private readonly string fileName;
        private readonly int? line;

        public FileNavigator(string fileName, int? line, IServiceProvider sp)
        {
            this.fileName = fileName;
            this.line = line;
            this.sp = sp;
            this.ShowFileName = true;
        }

        public bool ShowFileName { get; set; }

        #region ICodeLocation Members

        public string Text
        {
            get
            {
                if (!ShowFileName)
                    return "";
                return GetText(Path.GetFileName(fileName), line);
            }
        }

        public ValueTask NavigateTo()
        {
            var editorSvc = sp.GetService<ITextFileEditorService>();
            editorSvc?.DisplayFile(fileName, line);
            return ValueTask.CompletedTask;
        }

        #endregion

        public override string ToString()
        {
            return GetText(fileName, line);
        }

        private static string GetText(string fileName, int? line)
        {
            return line.HasValue ? $"{fileName}:{line}" : fileName;
        }
    }
}
