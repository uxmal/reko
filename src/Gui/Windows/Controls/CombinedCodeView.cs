#region License
/* 
 * Copyright (C) 1999-2016 Pavel Tomin.
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
using Reko.Gui.Controls;
using System;
using System.Windows.Forms;

namespace Reko.Gui.Windows.Controls
{
    public partial class CombinedCodeView : UserControl, INavigableControl<Address>
    {
        public CombinedCodeView()
        {
            InitializeComponent();
            this.Back = new ToolStripButtonWrapper(btnBack);
            this.Forward = new ToolStripButtonWrapper(btnForward);
        }

        public IButton Back { get; set; }
        public IButton Forward { get; set; }

        IButton INavigableControl<Address>.BackButton { get { return Back; } }
        IButton INavigableControl<Address>.ForwardButton { get { return Forward; } }

        public MixedCodeDataControl MixedCodeDataView { get { return this.mixedCodeDataControl; } }
        public TextView CodeView { get { return this.codeTextView; } }

        public Address CurrentAddress
        {
            get { return addrCurrent; }
            set { addrCurrent = value; CurrentAddressChanged.Fire(this); }
        }
        public event EventHandler CurrentAddressChanged;
        private Address addrCurrent;
    }
}
