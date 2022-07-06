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

using Reko.Gui.Controls;
using Reko.Gui.Services;
using System;
using System.IO;

namespace Reko.Gui
{
    public class TextFileViewerServiceImpl : ViewService, ITextFileEditorService
    {
        public TextFileViewerServiceImpl(IServiceProvider sp) : base(sp)
        {
        }

        public void DisplayFile(string fileName, int? line)
        {
            var pane = new TextFileEditorInteractor(fileName);
            var windowType = typeof(TextFileEditorInteractor).Name;
            var title = Path.GetFileName(fileName);
            var frame = ShowWindow(windowType, title, fileName, pane);
            pane = ((TextFileEditorInteractor) frame.Pane);
            pane.DisplayFile();
            if (line.HasValue)
            {
                pane.GoToLine(line.Value);
            }
        }
    }
}
