#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Types;
using Reko.Gui.Reactive;
using Reko.Scanning;
using System;
using System.Text;

namespace Reko.Gui.ViewModels.Dialogs
{
    public class FindStringsViewModel : ChangeNotifyingObject
    {
        public FindStringsViewModel()
        {
            this.minLength = 5;
        }

        public int CharacterType
        {
            get => characterType;
            set => base.RaiseAndSetIfChanged(ref characterType, value);
        }
        private int characterType;

        public int StringKind
        {
            get => stringKind;
            set => base.RaiseAndSetIfChanged(ref stringKind, value);
        }
        private int stringKind;

        public int MinLength
        {
            get => minLength;
            set => base.RaiseAndSetIfChanged(ref minLength, value);
        }
        private int minLength;

        public StringFinderCriteria GetCriteria()
        {
            return GetCriteria(this.CharacterType, this.StringKind, this.MinLength);
        }

        public StringFinderCriteria GetCriteria(int characterType, int stringKind, int minLength)
        {
            Encoding encoding;
            PrimitiveType charType;
            Func<ByteMemoryArea, Address, Address, EndianImageReader> rdrCreator;
            switch (characterType)
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
            switch (stringKind)
            {
            default: strType = StringType.NullTerminated(charType); break;
            case 1: strType = StringType.LengthPrefixedStringType(charType, PrimitiveType.Byte); break;
            case 2: case 3: strType = StringType.LengthPrefixedStringType(charType, PrimitiveType.UInt16); break;
            }

            return new StringFinderCriteria(
                StringType: strType,
                Encoding: encoding,
                MinimumLength: minLength,
                CreateReader: rdrCreator);
        }
    }
}
