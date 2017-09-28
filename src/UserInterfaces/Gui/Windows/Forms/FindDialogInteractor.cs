/* 
* Copyright (C) 1999-2017 John Källén.
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Reko.Gui.Windows.Forms
{
    public class FindDialogInteractor
    {
        private FindDialog dlg;

        public FindDialog CreateDialog()
        {
            dlg = new FindDialog();
            dlg.FindText.TextChanged += new EventHandler(FindText_TextChanged);
            UpdateUI();
            return dlg;
        }

        private void UpdateUI()
        {
            dlg.FindButton.Enabled = ToHexadecimal(dlg.FindText.Text) != null;
        }

        protected void FindText_TextChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        public byte[] ToHexadecimal(string str)
        {
            List<byte> bytes = new List<byte>();
            str = str.Replace(" ", "").Replace("\t", "");
            if (str.Length == 0)
                return null;
            if (str.Length % 2 == 1)
                return null;                // input must have even # of hex digits.
            for (int i = 0; i < str.Length; i += 2)
            {
                int n = HexDigit(str[i]);
                if (n < 0)
                    return null;
                int nn = HexDigit(str[i + 1]);
                if (nn < 0)
                    return null;

                bytes.Add((byte) (n * 16 + nn));
            }
            return bytes.ToArray();
        }

        private int HexDigit(char p)
        {
            if ('0' <= p && p <= '9')
                return p - '0';
            else if ('A' <= p && p <= 'F')
                return p - 'A' + 10;
            else if ('a' <= p && p <= 'f')
                return p - 'a' + 10;
            else
                return -1;
        }
    }
}
