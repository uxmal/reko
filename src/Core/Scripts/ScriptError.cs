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
using System.Linq;

namespace Reko.Core.Scripts
{
    /// <summary>
    /// Represents an error encountered when running a script.
    /// </summary>
    public class ScriptError
    {
        /// <summary>
        /// Constructs a new instance of <see cref="ScriptError"/>.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="ex">Exception.</param>
        /// <param name="message">Error message.</param>
        /// <param name="stackFrames">List of stack frames.</param>
        public ScriptError(
            string fileName,
            Exception ex,
            string message,
            IEnumerable<ScriptStackFrame> stackFrames)
        {
            this.FileName = fileName;
            this.Exception = ex;
            this.Message = message;
            this.StackFrames = stackFrames.ToList();
            this.LineNumber = FindFileLine(FileName, stackFrames);
        }

        /// <summary>
        /// File name to which the error refers.
        /// </summary>
        public readonly string FileName;

        /// <summary>
        /// Line number in the file to which the error refers.
        /// </summary>
        public readonly int? LineNumber;

        /// <summary>
        /// Exception that was thrown.
        /// </summary>
        public readonly Exception Exception;

        /// <summary>
        /// Message describing the error.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Stack frames.
        /// </summary>
        public readonly IList<ScriptStackFrame> StackFrames;

        private static int? FindFileLine(
            string fileName,
            IEnumerable<ScriptStackFrame> stackFrames)
        {
            foreach (var stackFrame in stackFrames)
            {
                if (stackFrame.FileName == fileName)
                    return stackFrame.LineNumber;
            }
            return null;
        }
    }
}
