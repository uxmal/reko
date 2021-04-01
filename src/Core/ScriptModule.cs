#region License
/* 
 * Copyright (C) 1999-2021 Pavel Tomin.
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
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Base class for execution of user-defined scripts.
    /// </summary>
    public abstract class ScriptModule
    {
        public ScriptModule(IServiceProvider services, string filename, byte[] bytes)
        {
            this.Filename = filename;
        }

        public readonly string Filename;

        /// <summary>
        /// Call specified function, pass program as parameter.
        /// </summary>
        /// <param name="funcName">Function name.</param>
        /// <param name="program">Program.</param>
        public abstract void CallFunction(string funcName, Program program);
    }
}
