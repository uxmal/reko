#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Dock.Model.ReactiveUI.Controls;
using Reko.Gui;
using System;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents
{
    /// <summary>
    /// Avalonia specific implementation of the <see cref="IWindowFrame"/> 
    /// interface for "documents"; that is, views that live in the document
    /// dock.
    /// </summary>
    public class DocumentFrameViewModel : Document, IWindowFrame
    {
        public DocumentFrameViewModel(string documentType, IWindowPane pane, string title)
        {
            this.DocumentType = documentType;
            this.Pane = pane;
            this.Title = title;
        }

        public DocumentFrameViewModel(string documentType, object docItem, IWindowPane pane, string title)
        {
            this.DocumentType = documentType;
            this.DocumentItem = docItem;
            this.Pane = pane;
            this.Title = title;
        }

        public object? DocumentItem { get; }
        
        public string DocumentType { get; }

        public IWindowPane Pane { get; }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
        }
    }
}
