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

using Reko.Gui;
using Reko.Gui.Services;
using Reko.Gui.ViewModels;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    internal class AvaloniaTabControlHostService : ITabControlHostService
    {
        private IServiceProvider services;

        public AvaloniaTabControlHostService(IServiceProvider services)
        {
            this.services = services;
        }

        public IWindowFrame ActiveFrame { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool ContainsFocus
        {
            get
            {
                return false;   //$TODO
            }
        }

        public IWindowFrame Add(IWindowPane pane, string tabCaption)
        {
            throw new NotImplementedException();
        }

        public IWindowFrame Attach(IWindowPane pane, object tabPage)
        {
            throw new NotImplementedException();
        }
        public ValueTask<bool> ExecuteAsync(CommandID cmdId)
        {
            throw new NotImplementedException();
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            throw new NotImplementedException();
        }
    }
}