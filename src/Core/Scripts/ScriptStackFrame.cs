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
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Scripts
{
    /// <summary>
    /// Represents a stack frame in a script trace.
    /// </summary>
    public class ScriptStackFrame
    {
        /// <summary>
        /// Constructs an instance of <see cref="ScriptStackFrame"/>.
        /// </summary>
        /// <param name="file">Name of the script file.</param>
        /// <param name="line">Line of the script file.</param>
        /// <param name="method">Method name.</param>
        public ScriptStackFrame(string file, int line, string method)
        {
            this.FileName = file;
            this.LineNumber = line;
            this.MethodName = method;
        }

        /// <summary>
        /// Name of the script file.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Line number in the script file.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Name of the method in the script file.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Returns a string representation of the stack frame.
        /// </summary>
        public override string ToString()
        {
            return $"File \"{FileName}\", line {LineNumber}, in {MethodName}";
        }
    }
}
