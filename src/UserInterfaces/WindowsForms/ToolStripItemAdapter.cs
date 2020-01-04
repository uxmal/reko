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

using Reko.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class ToolStripItemAdapter : IMenuAdapter
    {
        private ToolStripItemCollection items;

        public ToolStripItemAdapter(ToolStripItemCollection items)
        {
            this.items = items;
        }

        public int Count
        {
            get { return items.Count; }
        }

        public CommandID GetCommandID(int i)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int i)
        {
            throw new NotImplementedException();
        }

        public bool IsTemporary(int i)
        {
            throw new NotImplementedException();
        }

        public bool IsSeparator(int i)
        {
            throw new NotImplementedException();
        }

        public void SetStatus(int i, MenuStatus s)
        {
            throw new NotImplementedException();
        }

        public bool IsDynamic(int i)
        {
            throw new NotImplementedException();
        }

        public void SetText(int i, string p)
        {
            throw new NotImplementedException();
        }

        public void InsertAt(int p, object itemNew)
        {
            throw new NotImplementedException();
        }
    }
}
