#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Services;
using Reko.Gui.ViewModels.Documents;
using System;
using System.Diagnostics;
using System.Linq;

namespace Reko.Gui.Services
{
    public sealed class LowLevelViewService : ViewService, ILowLevelViewService
    {
        private readonly ISelectedAddressService selAddrSvc;

        public LowLevelViewService(IServiceProvider sp) : base(sp)
        {
            this.selAddrSvc = sp.RequireService<ISelectedAddressService>();
        }

        #region IMemoryViewService Members


        public void ViewImage(Program program)
        {
            Debug.Assert(program is not null);
            var llvi = ShowWindowImpl(program);
            var addr = program.SegmentMap.Segments.Values
                .Where(s => s.MemoryArea is not null)
                .Select(s => Address.Max(s.Address, s.MemoryArea.BaseAddress))
                .FirstOrDefault();
            llvi.SelectedAddress = addr;
        }

        public void ShowWindow(Program program)
        {
            ShowWindowImpl(program);
        }

        public void ShowMemoryAtAddress(IReadOnlyProgram? program, Address addr)
        {
            if (program is null)
                return;
            var llvi = ShowWindowImpl((Program)program);
            llvi.SelectedAddress = addr;
        }

        private ILowLevelViewInteractor ShowWindowImpl(Program program)
        {
            var llvi = CreateMemoryViewInteractor(program);
            llvi.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(mvi_SelectionChanged);
            var frame = base.ShowWindow(ILowLevelViewService.ViewWindowType, "Memory View", program, llvi);
            llvi = (ILowLevelViewInteractor)frame.Pane ?? llvi;
            //llvi.Program = program;
            return llvi;
        }

        #endregion

        public ILowLevelViewInteractor CreateMemoryViewInteractor(Program program)
        {
            var wpf = Services.RequireService<IWindowPaneFactory>();
            return wpf.CreateLowLevelViewPane(program);
        }

        private void mvi_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var llvi = (ILowLevelViewInteractor) sender!;
            var addrRange = llvi.GetSelectedAddressRange();
            var par = ProgramAddressRange.ClosedRange(llvi.Program!, addrRange.Begin, addrRange.End);
            selAddrSvc.SelectedAddressRange = par;
        }
    }
}
