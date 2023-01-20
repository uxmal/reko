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
using Reko.Gui;
using Reko.Gui.Services;
using Reko.Gui.ViewModels.Documents;
using System;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaMemoryViewService : LowLevelViewService
    {
        public AvaloniaMemoryViewService(IServiceProvider services) :
            base(services)
        {
        }

        public override ILowLevelViewInteractor CreateMemoryViewInteractor()
        {
            return new Dummy();
        }

        private class Dummy : Avalonia.Controls.Control, ILowLevelViewInteractor
        {
            public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

            public Program? Program { get; set; }
            public Address? SelectedAddress { get; set; }
            public IWindowFrame? Frame { get; set; }


            public void Close()
            {
            }

            public object CreateControl()
            {
                throw new NotImplementedException();
            }

            public AddressRange GetSelectedAddressRange()
            {
                throw new NotImplementedException();
            }

            public void SetSite(IServiceProvider services)
            {
            }
        }
    }
}