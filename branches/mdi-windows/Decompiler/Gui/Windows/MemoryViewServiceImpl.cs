/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Gui.Windows
{
    public class MemoryViewServiceImpl : ViewService, IMemoryViewService
    {
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        private MemoryViewInteractor mvi;
        private IServiceProvider sp;

        public const string ViewWindowType = "memoryViewWindow";


        public MemoryViewServiceImpl(IServiceProvider sp) : base(sp)
        {
        }

        #region IMemoryViewService Members

        public void InvalidateWindow()
        {
            mvi.InvalidateControl();
        }

        public void ViewImage(ProgramImage image)
        {
            ShowWindow();
            mvi.ProgramImage = image;
        }

        public void ShowMemoryAtAddress(Address addr)
        {
            ShowWindow();
        }


        public AddressRange GetSelectedAddressRange()
        {
            return mvi.GetSelectedAddressRange();
        }

        public void ShowWindow()
        {
            if (mvi == null)
            {
                mvi = new MemoryViewInteractor();
                mvi.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(mvi_SelectionChanged);
            }
            ShowWindow(ViewWindowType, mvi);
        }
        #endregion

        void mvi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }
    }
}
