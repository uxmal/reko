#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Core
{
    /// <summary>
    /// Interface for script interpreters.
    /// </summary>
    public interface IScriptInterpreter
    {
        /// <summary>
        /// Loads a script from the specified file.
        /// </summary>
        /// <param name="path">File path to the script file.</param>
        /// <param name="program">Program being analyzed.</param>
        /// <param name="currentDir">Used as the current directory.</param>
        void LoadFromFile(string path, Program program, string currentDir);

        /// <summary>
        /// Loads a script directly from a string.
        /// </summary>
        /// <param name="script">Script source code.
        /// </param>
        /// <param name="program">Program being analyzed.</param>
        /// <param name="currentDir">Used as the current directory.</param>
        void LoadFromString(string script, Program program, string currentDir);

        /// <summary>
        /// Executes the script.
        /// </summary>
        void Run();
    }
}
