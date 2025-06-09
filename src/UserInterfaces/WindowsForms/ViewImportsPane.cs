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
using Reko.Core.Services;
using Reko.Gui.Services;
using Reko.Gui.ViewModels;
using Reko.Services;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    public class ViewImportsPane : IWindowPane
    {
        private ViewImportsControl control;
        private Comparer comparer;
        private Program program;
        private IServiceProvider services;
        private ListViewItem.ListViewSubItem mSelected;
        private IUiPreferencesService uiPrefsSvc;

        public ViewImportsPane(Program program)
        {
            this.program = program;
        }

        public IWindowFrame Frame { get; set; }

        public void Close()
        {
            if (control is not null)
                control.Dispose();
            control = null;
        }

        public object CreateControl()
        {
            this.control = new ViewImportsControl();
            this.comparer = new Comparer();

            this.control.Load += Control_Load;
            this.control.Imports.MouseMove += Imports_MouseMove;
            this.control.Imports.ColumnClick += Imports_ColumnClick;
            this.control.Imports.ListViewItemSorter = comparer;
            this.control.Imports.MouseClick += Imports_MouseClick;
            return control;
        }

        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }

        private void Control_Load(object sender, EventArgs e)
        {
            this.uiPrefsSvc = services.RequireService<IUiPreferencesService>();
            var items = program.ImportReferences.Values
                .Select(CreateListItem)
                .ToArray();
            control.Imports.Items.Clear();
            control.Imports.Items.AddRange(items);
        }

        private ListViewItem CreateListItem(ImportReference imp)
        {
            var item = new ListViewItem();
            item.Tag = imp;
            item.Text = imp.ReferenceAddress.ToString();
            item.UseItemStyleForSubItems = false;
            item.SubItems[0].Tag = imp.ReferenceAddress;
            item.SubItems.Add(imp.ModuleName);
            switch (imp)
            {
            case OrdinalImportReference ord:
                var refText = new StringBuilder(ord.Ordinal.ToString());
                string importName = TryGetImportName(ord);
                if (importName is not null)
                {
                    refText.AppendFormat(" ({0})", importName);
                }
                item.SubItems.Add(refText.ToString());
                break;
            case NamedImportReference nam:
                item.SubItems.Add(nam.ImportName);
                break;
            }
            item.SubItems[0].ForeColor = ((SolidBrush)uiPrefsSvc.Styles["link"].Foreground).Color;
            return item;
        }

        private string TryGetImportName(OrdinalImportReference ord)
        {
            var dec = services.RequireService<IDecompilerService>();
            var project = dec.Decompiler.Project;
            var dynLinker = new DynamicLinker(project, program, new NullEventListener());
            var e = dynLinker.ResolveService(ord.ModuleName, ord.Ordinal);
            string importName = e?.Name;
            return importName;
        }

        private void Imports_MouseMove(object sender, MouseEventArgs e)
        {
            var info = control.Imports.HitTest(e.Location);
            if (info.SubItem == mSelected) return;
            if (mSelected is not null)
                mSelected.Font = control.Imports.Font;
            mSelected = null;
            this.control.Imports.Cursor = Cursors.Default;
            if (info.SubItem is not null && info.Item.SubItems[0] == info.SubItem)
            {
                info.SubItem.Font = new Font(info.SubItem.Font, FontStyle.Underline);
                this.control.Imports.Cursor = Cursors.Hand;
                mSelected = info.SubItem;
            }
        }

        private void Imports_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == comparer.SortColumn)
            {
                // Same column as before, invert the sort order.
                comparer.Order = comparer.Order == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                comparer.SortColumn = e.Column;
                comparer.Order = SortOrder.Ascending;
            }
            control.Imports.Sort();
        }

        private void Imports_MouseClick(object sender, MouseEventArgs e)
        {
            var info = control.Imports.HitTest(e.X, e.Y);
            var row = info.Item.Index;
            if (info.SubItem is not null)
            {
                var addr = info.SubItem.Tag as Address?;
                services.RequireService<ILowLevelViewService>()
                    .ShowMemoryAtAddress(program, addr.Value);
            }
        }

        private class Comparer : IComparer
        {
            public Comparer()
            {
                this.SortColumn = 0;
                this.Order = SortOrder.Ascending;
            }

            public int SortColumn { get; set; }
            public SortOrder Order { get; set; }

            public int Compare(object x, object y)
            {
                var itemX = (ListViewItem)x;
                var itemY = (ListViewItem)y;
                var importX = (ImportReference)itemX.Tag;
                var importY = (ImportReference)itemY.Tag;
                int cmp;
                switch (SortColumn)
                {
                case 0:
                    cmp = importX.ReferenceAddress.CompareTo(importY.ReferenceAddress);
                    break;
                case 1:
                    cmp = CompareModuleNames(importX, importY);
                    break;
                case 2:
                    cmp = importX.CompareTo(importY);
                    break;
                default:
                    cmp = 0;
                    break;
                }
                if (Order == SortOrder.Descending)
                    cmp = -cmp;
                return cmp;
            }

            private static int CompareModuleNames(ImportReference importX, ImportReference importY)
            {
                if (importX.ModuleName is null)
                {
                    return (importY.ModuleName is null) ? 0 : -1;
                }
                else if (importY.ModuleName is null)
                {
                    Debug.Assert(importX.ModuleName is not null);
                    return 1;
                }
                else
                {
                    return importX.ModuleName.CompareTo(importY.ModuleName);
                }
            }
        }
    }
}