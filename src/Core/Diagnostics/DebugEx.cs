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

using System.Diagnostics;
using System.Threading;

namespace Reko.Core.Diagnostics
{
    /// <summary>
    /// Extension methods for the <see cref="TraceSwitch"/> class.
    /// </summary>
    public static class DebugEx
    {
        /// <summary>
        /// Prints a message to the debug output if the trace switch is set to 
        /// <see cref="TraceLevel.Error"/>.
        /// </summary>
        /// <param name="trace"><see cref="TraceSwitch"/> controlling the output.</param>
        /// <param name="format">Format string.</param>
        /// <param name="args">Arguments to be interpolated into the format string.
        /// </param>
        [Conditional("DEBUG")]
        public static void Error(this TraceSwitch trace, string format, params object[] args)
        {
            if (trace is not null && trace.TraceError)
            {
                Debug.Print(format, args);
            }
        }

        /// <summary>
        /// Prints a message to the debug output if the trace switch is set to 
        /// <see cref="TraceLevel.Warning"/>.
        /// </summary>
        /// <param name="trace"><see cref="TraceSwitch"/> controlling the output.</param>
        /// <param name="format">Format string.</param>
        /// <param name="args">Arguments to be interpolated into the format string.
        /// </param>
        [Conditional("DEBUG")]
        public static void Warn(this TraceSwitch trace, string format, params object[] args)
        {
            if (trace is not null && trace.TraceWarning)
            {
                Debug.Print(format, args);
            }
        }

        /// <summary>
        /// Prints a message to the debug output if the trace switch is set to 
        /// <see cref="TraceLevel.Info"/>.
        /// </summary>
        /// <param name="trace"><see cref="TraceSwitch"/> controlling the output.</param>
        /// <param name="format">Format string.</param>
        /// <param name="args">Arguments to be interpolated into the format string.
        /// </param>
        [Conditional("DEBUG")]
        public static void Inform(this TraceSwitch trace, string format, params object[] args)
        {
            if (trace is not null && trace.TraceInfo)
            {
                Debug.Print(format, args);
            }
        }

        /// <summary>
        /// Prints a message to the debug output if the trace switch is set to 
        /// <see cref="TraceLevel.Verbose"/>.
        /// </summary>
        /// <param name="trace"><see cref="TraceSwitch"/> controlling the output.</param>
        /// <param name="message">Message to write.</param>
        [Conditional("DEBUG")]
        public static void Verbose(this TraceSwitch trace, string message)
        {
            if (trace is not null && trace.TraceVerbose)
            {
                Debug.WriteLine(message);
            }
        }

        /// <summary>
        /// Prints a message to the debug output if the trace switch is set to 
        /// <see cref="TraceLevel.Verbose"/>.
        /// </summary>
        /// <param name="trace"><see cref="TraceSwitch"/> controlling the output.</param>
        /// <param name="format">Format string.</param>
        /// <param name="args">Arguments to be interpolated into the format string.
        /// </param>
        [Conditional("DEBUG")]
        public static void Verbose(this TraceSwitch trace, string format, params object[]args)
        {
            if (trace is not null && trace.TraceVerbose)
            {
                Debug.Print(format, args);
            }
        }

        /// <summary>
        /// Launch the Debugger and Break
        /// </summary>
        /// <param name="launchIfTrue"></param>
        /// <remarks>
        /// Example usage: start reko with Ctrl+F5.
        /// When this function is hit, the debugger will be started if <paramref name="launchIfTrue"/> is true.
        /// Can be used with the result of a boolean expression as an alternative
        /// to conventional breakpoints when debugging large executables
        /// </remarks>
        [Conditional("DEBUG")]
        public static void Break(bool launchIfTrue = true)
        {
            // should we break?
            if (!launchIfTrue) return;

            if (Debugger.IsAttached) {
                // yes, but we're already attached. break instead
                Debugger.Break();
                return;
            }

            // yes, but we're not attached. launch & break
            if (!Debugger.Launch())
            {
                // user clicked Cancel or this operation is not supported (e.g. Linux)
                return;
            }

            while (!Debugger.IsAttached)
            {
                Thread.Sleep(200);
            }
        }
    }
}