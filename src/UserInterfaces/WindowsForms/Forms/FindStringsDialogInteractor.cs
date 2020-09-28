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
using System.Text;
using Reko.Core;
using Reko.Core.Types;
using Reko.Scanning;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    internal class FindStringsDialogInteractor
    {
        private FindStringsDialog dlg;

        public FindStringsDialogInteractor()
        {
        }

        public void Attach(FindStringsDialog dlg)
        {
            this.dlg = dlg;
            dlg.Load += Dialog_Load;
        }

        private void Dialog_Load(object sender, EventArgs e)
        {
            dlg.StringKindList.SelectedIndex = 0;
            dlg.CharacterSizeList.SelectedIndex = 0;
        }

        public StringFinderCriteria GetCriteria()
        {
            Encoding encoding;
            PrimitiveType charType;
            Func<MemoryArea, Address, Address, EndianImageReader> rdrCreator;
            switch (dlg.CharacterSizeList.SelectedIndex)
            {
            default:
                encoding = Encoding.ASCII;
                charType = PrimitiveType.Char;
                rdrCreator = (m, a, b) => new LeImageReader(m, a, b);
                break;
            case 1:
                encoding = Encoding.GetEncoding("utf-16LE");
                charType = PrimitiveType.WChar;
                rdrCreator = (m, a, b) => new LeImageReader(m, a, b);
                break;
            case 2:
                encoding = Encoding.GetEncoding("utf-16BE");
                charType = PrimitiveType.WChar;
                rdrCreator = (m, a, b) => new BeImageReader(m, a, b);
                break;
            }

            StringType strType;
            switch (dlg.StringKindList.SelectedIndex)
            {
            default: strType = StringType.NullTerminated(charType); break;
            case 1: strType = StringType.LengthPrefixedStringType(charType, PrimitiveType.Byte); break;
            case 2: case 3: strType = StringType.LengthPrefixedStringType(charType, PrimitiveType.UInt16); break;
            }

            return new StringFinderCriteria
            {
                StringType = strType,
                Encoding = encoding,
                MinimumLength = dlg.MinLength,
                CreateReader = rdrCreator,
            };
        }
    }
}