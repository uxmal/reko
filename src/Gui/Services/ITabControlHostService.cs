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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Gui.ViewModels;

namespace Reko.Gui.Services
{
    public interface ITabControlHostService : ICommandTarget
    {
        IWindowFrame ActiveFrame { get; set; }
        bool ContainsFocus { get; }

        /// <summary>
        /// Attaches a pane to an existing tab page.
        /// </summary>
        /// <param name="pane"></param>
        /// <param name="tabPage"></param>
        /// <returns></returns>
        IWindowFrame Attach(IWindowPane pane, object tabPage);

        IWindowFrame Add(IWindowPane pane, string tabCaption);
    }
}
