#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Various helper methods that don't belong to any particular class.
    /// </summary>
    public class UiTools
    {
        //$REVIEW: this is dependent on the English locale.
        private const Keys CtrlA = Keys.A | Keys.Control;

        public static void AddSelectAllHandler(ToolStripTextBox textBox)
        {
            textBox.Control.KeyDown += TextBox_SelectAll;
        }

        public static void AddSelectAllHandler(TextBox textBox)
        {
            textBox.KeyDown += TextBox_SelectAll;
        }

        private static void TextBox_SelectAll(object sender, KeyEventArgs e)
        {
            if (e.KeyData == CtrlA)
            {
                var t = (TextBox) sender;
                t.SelectAll();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
