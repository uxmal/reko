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

using Reko.Gui;
using System;
using System.ComponentModel.Design;
using System.Windows.Input;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels
{
    /// <summary>
    /// Adapter class that funnels <see cref="ICommand"/> methods to their corresponding
    /// <see cref="ICommandTarget"/> methods.
    /// </summary>
    public class CommandAdapter : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        
        private readonly ICommandTarget target;
        private readonly CommandStatus status;
        private readonly CommandText text;

        public CommandAdapter(ICommandTarget target)
        {
            this.target = target;
            this.status = new CommandStatus();
            this.text = new CommandText("_");
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter is CommandID cmdid)
            {
                return target.QueryStatus(cmdid, status, text) &&
                    status.Status.HasFlag(MenuStatus.Enabled);
            }
            return false;
        }

        public void Execute(object? parameter)
        {
            if (parameter is CommandID cmdid)
            {
                target.Execute(cmdid);
            }
        }
    }
}