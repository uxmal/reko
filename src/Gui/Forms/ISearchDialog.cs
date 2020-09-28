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

using Reko.Gui.Controls;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Gui.Forms
{
    public interface ISearchDialog : IDialog
    {
        event EventHandler Load;
        event EventHandler Closed;

        IServiceProvider Services { get; }
        string InitialPattern { get; set; }

        IComboBox Patterns { get; }
        ICheckBox RegexCheckbox { get; }
        IComboBox Encodings { get; }
        IComboBox Scopes { get; }
        ITextBox StartAddress { get; }
        ITextBox EndAddress { get; }
        IButton SearchButton { get; }
        ICheckBox ScannedMemory { get; }
        ICheckBox UnscannedMemory { get; }

        StringSearch<byte> ImageSearcher { get; set; }
    }
}
