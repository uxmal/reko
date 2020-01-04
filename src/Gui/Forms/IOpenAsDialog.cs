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

using Reko.Core.Configuration;
using Reko.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Gui.Forms
{
    public interface IOpenAsDialog : IDialog
    {
        event EventHandler Load;

        ITextBox AddressTextBox { get; }
        ITextBox FileName { get; }
        IServiceProvider Services { get; set; }
        IComboBox RawFileTypes { get; }
        IComboBox Architectures { get; }
        IComboBox Platforms { get; }
        IPropertyGrid PropertyGrid { get; }
        IButton BrowseButton { get; }
        IButton OkButton { get; }

        Dictionary<string, object> ArchitectureOptions { get; set; }

        ArchitectureDefinition GetSelectedArchitecture();
        PlatformDefinition GetSelectedEnvironment();
        void SetPropertyGrid(Dictionary<string, object> architectureOptions, List<PropertyOption> options);
    }
}
