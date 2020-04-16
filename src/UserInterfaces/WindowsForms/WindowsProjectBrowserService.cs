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
using Reko.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class WindowsProjectBrowserService : ProjectBrowserService
    {
        public WindowsProjectBrowserService(IServiceProvider services, ITabPage tabPage, ITreeView treeView) : base(services, tabPage, treeView)
        {
            this.tree.DragEnter += tree_DragEnter;
            this.tree.DragOver += tree_DragOver;
            this.tree.DragDrop += tree_DragDrop;
            this.tree.DragLeave += tree_DragLeave;
            this.tree.MouseWheel += tree_MouseWheel;
        }

        void tree_DragEnter(object sender, Gui.Controls.DragEventArgs e)
        {
            if (((IDataObject) e.Data).GetDataPresent(DataFormats.FileDrop))
                e.Effect = e.AllowedEffect & Gui.Controls.DragDropEffects.Copy;
            else
                e.Effect = Gui.Controls.DragDropEffects.None;
        }

        void tree_DragOver(object sender, Gui.Controls.DragEventArgs e)
        {
            if (((IDataObject) e.Data).GetDataPresent(DataFormats.FileDrop))
                e.Effect = e.AllowedEffect & Gui.Controls.DragDropEffects.Copy;
            else
                e.Effect = Gui.Controls.DragDropEffects.None;
        }

        void tree_DragLeave(object sender, EventArgs e)
        {
        }

        void tree_DragDrop(object sender, Gui.Controls.DragEventArgs e)
        {
            if (((IDataObject)e.Data).GetDataPresent(DataFormats.FileDrop))
            {
                var filename = (string) ((IDataObject) e.Data).GetData(DataFormats.FileDrop);
                OnFileDropped(new FileDropEventArgs(filename));
            }
        }

        private void tree_MouseWheel(object sender, Gui.Controls.MouseEventArgs e)
        {
            //model.MoveTo(model.CurrentPosition, (e.Delta < 0 ? 1 : -1));
            //RecomputeLayout();
            //OnScroll();
            //tree,Invalidate();
        }
    }
}
