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

using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.Gui
{
    /// <summary>
    /// This tracks, application-wide, the currently selected "thing".
    /// Controls that support tracking a selection should register
    /// events from this service.
    /// </summary>
    public class SelectionService : ISelectionService
    {
        public event EventHandler? SelectionChanged;
        public event EventHandler? SelectionChanging;

        private ICollection selection;

        public SelectionService()
        {
            selection = Array.Empty<object>();
        }

        public object PrimarySelection =>
            selection.Cast<object>().FirstOrDefault()!;

        public int SelectionCount =>
            selection.Count;

        public bool GetComponentSelected(object component)
        {
            if (component is null)
                return false;
            return selection.Cast<object>().Any(s => component.Equals(s));
        }

        public ICollection GetSelectedComponents()
        {
            return selection;
        }

        public void SetSelectedComponents(ICollection? components)
        {
            components ??= Array.Empty<object>();
            SelectionChanging?.Invoke(this, EventArgs.Empty);
            this.selection = components;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetSelectedComponents(ICollection? components, SelectionTypes selectionType)
        {
            throw new NotSupportedException();
        }
    }
}
