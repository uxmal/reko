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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Gui
{
    public interface IProjectBrowserService : ICommandTarget
    {
        event EventHandler<FileDropEventArgs> FileDropped;

        Program CurrentProgram { get; }
        bool ContainsFocus { get; }

        /// <summary>
        /// The currently selected object in the project browser tree.
        /// </summary>
        object SelectedObject { get; set; }


        /// <summary>
        /// Loads a project into the project browser and starts listening to changes. 
        /// Loading a null project clears the project browser.
        /// </summary>
        /// <param name="project"></param>
        void Load(Project project);

        void Clear();

        void Reload();

        void Show();
    }
}
