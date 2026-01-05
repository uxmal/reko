#region License
/* 
 * Copyright (C) 1999-2026 Pavel Tomin.
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
using System.ComponentModel;

namespace Reko.Core.Scripts
{
    /// <summary>
    /// Base class for execution of user-defined scripts.
    /// </summary>
    [Designer("Reko.Gui.Design.ScriptFileDesigner,Reko.Gui")]
    public abstract class ScriptFile
    {
        /// <summary>
        /// Constructs an instance of <see cref="ScriptFile"/>.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> instance.</param>
        /// <param name="scriptLocation">Location of the script file.</param>
        /// <param name="bytes">Raw bytes of the script file.</param>
        public ScriptFile(IServiceProvider services, ImageLocation scriptLocation, byte[] bytes)
        {
            this.Location = scriptLocation;
        }

        /// <summary>
        /// The location from which the script was loaded.
        /// </summary>
        public ImageLocation Location { get; }

        /// <summary>
        /// Evaluate script. Reset event handlers.
        /// </summary>
        public abstract void Evaluate(string script);

        /// <summary>
        /// Call handlers of specified event, pass program as parameter.
        /// </summary>
        /// <param name="event">Fired event.</param>
        /// <param name="program">Program.</param>
        public abstract void FireEvent(ScriptEvent @event, Program program);
    }
}
