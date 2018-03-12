#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core.Types;
using Reko.Gui;
using System;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Forms
{
    public partial class FindStringsDialog : Form, IFindStringsDialog
    {
        public FindStringsDialog()
        {
            InitializeComponent();
            new FindStringsDialogInteractor().Attach(this);
        }

        public int MinLength { get {  return Convert.ToInt32(this.numericUpDown1.Value); } set { } }

        public ComboBox CharacterSizeList {  get { return ddlCharSize; } }

        public ComboBox StringKindList { get { return ddlStringKind; } }

        public StringType GetStringType ()
        {
            var charType = ddlCharSize.SelectedIndex > 0
                ? PrimitiveType.WChar
                : PrimitiveType.Char;
            switch (ddlStringKind.SelectedIndex)
            {
            default: return StringType.NullTerminated(charType);
            case 1: return StringType.LengthPrefixedStringType(charType, PrimitiveType.Byte);
            case 2: return StringType.LengthPrefixedStringType(charType, PrimitiveType.UInt16);
            case 3: return StringType.LengthPrefixedStringType(charType, PrimitiveType.UInt32);
            }
        }
    }
}
