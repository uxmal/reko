#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.UserInterfaces.AvaloniaUI.ViewModels.Documents;
using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Docks
{
    public class CustomDocumentDock : DocumentDock
    {
        public CustomDocumentDock()
        {
            CreateDocument = ReactiveCommand.Create(CreateNewDocument);
        }

        private void CreateNewDocument()
        {
            if (!CanCreateDocument)
            {
                return;
            }

            var index = VisibleDockables?.Count + 1;
            var document = new DocumentViewModel { Id = $"Document{index}", Title = $"Document{index}"};

            Factory?.AddDockable(this, document);
            Factory?.SetActiveDockable(document);
            Factory?.SetFocusedDockable(this, document);
        }
    }
}
