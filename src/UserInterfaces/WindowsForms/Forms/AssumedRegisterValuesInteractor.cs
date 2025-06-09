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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class AssumedRegisterValuesInteractor
    {
        private AssumedRegisterValuesDialog dlg;

        public void Attach(AssumedRegisterValuesDialog dlg)
        {
            this.dlg = dlg;
            dlg.Load += dlg_Load;
            dlg.FormClosing += dlg_FormClosing;
            dlg.RegisterGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);
        }

        void dlg_Load(object sender, EventArgs e)
        {
            var col = (DataGridViewComboBoxColumn)dlg.RegisterGrid.Columns[0];
            col.Items.Clear();
            col.Items.AddRange(dlg.Architecture.GetRegisters()
                .Select(r => r.Name)
                .OrderBy(r => r)
                .ToArray());

            foreach (var de in dlg.Values.OrderBy(de => de.Key.Name))
            {
                dlg.RegisterGrid.Rows.Add(de.Key.Name, de.Value);
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != 1)
                return;
            var value = (string) e.FormattedValue;
            if (string.IsNullOrWhiteSpace(value))
                return;
            var oRegName = dlg.RegisterGrid[0, e.RowIndex].Value;
            RegisterStorage reg;
            if (!dlg.Architecture.TryGetRegister((string)oRegName, out reg))
                return;
            var pattern = string.Format(@"\*|([0-9a-f]{0}1,{1}{2})",
                "{",
                reg.DataType.Size * 2,
                "}");
            var re = new Regex(pattern);
            if (!re.IsMatch(value))
                e.Cancel = true;
        }

        private void dlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            dlg.Values = dlg.RegisterGrid.Rows
                .OfType<DataGridViewRow>()
                .Where(row => row.Cells[0].Value is not null)
                .ToDictionary(
                   row => dlg.Architecture.GetRegister((string)row.Cells[0].Value),
                   row => (string)row.Cells[1].Value);
        }
    }
}
