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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui
{
    /// <summary>
    /// This tracks, application-wide, the currently selected "thing".
    /// Controls that support tracking a selection shou
    /// </summary>
    public class SelectionService : ISelectionService
    {
        private ICollection selection;

        public SelectionService()
        {
            selection = new object[0];
        }

        public object PrimarySelection =>
            selection.Cast<object>().FirstOrDefault();

        public int SelectionCount =>
            selection.Count;

        public event EventHandler SelectionChanged;
        public event EventHandler SelectionChanging;

        public bool GetComponentSelected(object component)
        {
            if (component == null)
                return false;
            return selection.Cast<object>().Any(s => component.Equals(s));
        }

        public ICollection GetSelectedComponents()
        {
            return selection;
        }

        public void SetSelectedComponents(ICollection components)
        {
            SelectionChanging?.Invoke(this, EventArgs.Empty);
            this.selection = components;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetSelectedComponents(ICollection components, SelectionTypes selectionType)
        {
            throw new NotSupportedException();
        }
    }
}
