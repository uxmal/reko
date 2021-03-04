#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public class RegisterValuesInteractor
    {
        private RegisterValuesDialog dlg;

        public RegisterValuesInteractor()
        {
        }

        internal void Attach(RegisterValuesDialog dlg)
        {
            this.dlg = dlg;
            dlg.Load += dlg_Load;
            dlg.Grid.CellValidating += Grid_CellValidating;
            dlg.Grid.DataError += Grid_DataError;
            dlg.OkButton.Click += OkButton_Click;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            var items = new Dictionary<Storage, Constant>();
            foreach (DataGridViewRow row in dlg.Grid.Rows)
            {
                Storage stg = null;
                if (row.Cells[0].Value is string sRegister)
                {
                    if (dlg.Architecture.TryGetRegister(sRegister, out var reg))
                    {
                        stg = reg;
                    }
                    else
                    {
                        stg = dlg.Architecture.GetFlagGroup(sRegister);
                    }
                }
                if (stg != null && row.Cells[1].Value is string sValue)
                {
                    var value = sValue != "*"
                        ? Constant.Create(stg.DataType, Convert.ToUInt64(sValue, 16))
                        : Constant.Invalid;
                    items[stg] = value;
                }
            }
            var list = items.Select(de => new UserRegisterValue(de.Key, de.Value)).ToList();
            dlg.RegisterValues = list;
        }

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = false;
        }

        private void Grid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            e.Cancel = false;
        }

        private void dlg_Load(object sender, EventArgs e)
        {
            var registers = dlg.Architecture.GetRegisters()
                .OrderBy(r => r.Name)
                .Cast<Storage>()
                .Concat(dlg.Architecture.GetFlags())
                .Select(r => r.Name)
                .ToArray();
            dlg.RegisterColumn.Items.AddRange(registers);
            if (dlg.RegisterValues != null)
            {
                foreach (var rv in dlg.RegisterValues)
                {
                    var sValue = rv.Value.IsValid
                        ? string.Format($"{{0:X{rv.Register.DataType.Size * 2}}}", rv.Value.ToUInt64())
                        : "*";
                    dlg.Grid.Rows.Add(
                        rv.Register.Name,
                        sValue);
                }
            }
        }
    }
}