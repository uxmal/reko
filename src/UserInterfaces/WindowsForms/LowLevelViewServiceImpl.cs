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

using Reko.Core;
using Reko.Gui;
using System;
using System.Diagnostics;
using System.Linq;

namespace Reko.UserInterfaces.WindowsForms
{
    public class LowLevelViewServiceImpl : ViewService, ILowLevelViewService
    {

        public const string ViewWindowType = "memoryViewWindow";

        public LowLevelViewServiceImpl(IServiceProvider sp) : base(sp)
        {
        }

        #region IMemoryViewService Members


        public void ViewImage(Program program)
        {
            Debug.Assert(program != null);
            var llvi = ShowWindowImpl(program);
            var addr = program.SegmentMap.Segments.Values
                .Where(s => s.MemoryArea != null)
                .Select(s => Address.Max(s.Address, s.MemoryArea.BaseAddress))
                .FirstOrDefault();
            if (addr != null)
            {
                llvi.SelectedAddress = addr;
            }
        }

        public void ShowWindow(Program program)
        {
            ShowWindowImpl(program);
        }

        public void ShowMemoryAtAddress(Program program, Address addr)
        {
            var llvi = ShowWindowImpl(program);
            llvi.SelectedAddress = addr;
        }

        private LowLevelViewInteractor ShowWindowImpl(Program program)
        {
            var llvi = CreateMemoryViewInteractor();
            llvi.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(mvi_SelectionChanged);
            var frame = base.ShowWindow(ViewWindowType, "Memory View", program, llvi);
            llvi = (LowLevelViewInteractor)frame.Pane ?? llvi;
            llvi.Program = program;
            return llvi;
        }

        #endregion

        public virtual LowLevelViewInteractor CreateMemoryViewInteractor()
        {
            return new LowLevelViewInteractor();
        }

        void mvi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
