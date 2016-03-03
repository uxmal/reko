#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Gui.Windows.Controls;
using System;
using System.Windows.Forms;

namespace Reko.Gui.Windows
{
    public class MixedCodeDataViewInteractor : IWindowPane
    {
        private IServiceProvider services;
        private MixedCodeDataControl ctrl;

        public MixedCodeDataViewInteractor()
        {
        }

        public Address TopAddress { get { return addrTop; } set { addrTop = value; } }
        private Address addrTop;

        public void Close()
        {
            if (ctrl != null)
                ctrl.Dispose();
        }

        public Control CreateControl()
        {
            this.ctrl = new MixedCodeDataControl();
            return ctrl;
        }

        public void SetSite(IServiceProvider sp)
        {
            this.services = sp;
        }
    }
}
