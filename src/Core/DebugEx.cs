#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    public static class DebugEx
    {
        [Conditional("DEBUG")]
        public static void PrintIf(bool trace, string message, params object[] args)
        {
            if (trace)
            {
                Debug.Print(message, args);
            }
        }

        [Conditional("DEBUG")]
        public static void Error(TraceSwitch trace, string message, params object[] args)
        {
            if (trace.TraceError)
            {
                Debug.Print(message, args);
            }
        }

        [Conditional("DEBUG")]
        public static void Warn(TraceSwitch trace, string message, params object[] args)
        {
            if (trace.TraceWarning)
            {
                Debug.Print(message, args);
            }
        }

        [Conditional("DEBUG")]
        public static void Info(TraceSwitch trace, string message, params object[] args)
        {
            if (trace.TraceInfo)
            {
                Debug.Print(message, args);
            }
        }

        [Conditional("DEBUG")]
        public static void Verbose(TraceSwitch trace, string message, params object[]args)
        {
            if (trace.TraceVerbose)
            {
                Debug.Print(message, args);
            }
        }
    }
}