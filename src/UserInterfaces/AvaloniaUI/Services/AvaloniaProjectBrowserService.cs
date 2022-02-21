#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System;
using System.ComponentModel.Design;

namespace Reko.UserInterfaces.AvaloniaUI.Services
{
    public class AvaloniaProjectBrowserService : IProjectBrowserService
    {
        public event EventHandler<FileDropEventArgs>? FileDropped;

        private IServiceProvider services;

        public AvaloniaProjectBrowserService(IServiceProvider services)
        {
            this.services = services;
        }

        public Program CurrentProgram => throw new NotImplementedException();

        public bool ContainsFocus
        {
            get
            {
                return false; //$TODO
            }
        }

        public object SelectedObject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Clear()
        {
            //$TODO
        }

        public bool Execute(CommandID cmdId)
        {
            throw new NotImplementedException();
        }

        public void Load(Project project)
        {
            throw new NotImplementedException();
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            throw new NotImplementedException();
        }

        public void Reload()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }
    }
}