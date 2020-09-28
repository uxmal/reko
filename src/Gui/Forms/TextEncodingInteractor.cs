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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Gui.Forms
{
    public class TextEncodingInteractor
    {
        private ITextEncodingDialog dlg;

        public void Attach(ITextEncodingDialog dlg)
        {
            this.dlg = dlg;
            dlg.EncodingList.AddItems(
                Encoding.GetEncodings()
                .OrderBy(e => e.DisplayName)
                .Select(e => new ListOption {
                    Text = string.Format("{0} - {1}", e.DisplayName, e.Name),
                    Value = e.Name })) ;
        }

        public Encoding GetSelectedTextEncoding()
        {
            var item = (ListOption)dlg.EncodingList.SelectedItem;
            if (item == null)
                return null;
            else
                return Encoding.GetEncoding((string)item.Value);
        }
    }
}
