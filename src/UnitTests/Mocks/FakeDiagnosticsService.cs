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
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Mocks
{
    public class FakeDiagnosticsService : IDiagnosticsService
    {
        public void Error(string message)
        {
            Debug.Print(message);
        }

        public void Error(Exception ex, string message)
        {
            Error(new NullCodeLocation(""), ex, message);
        }
    
        public void Error(string message, params object[] args)
        {
            Error(string.Format(message, args));
        }

        public void Error(ICodeLocation location, string message)
        {
            Debug.Print("{0}: error: {1}", location, message);
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            Error(location, string.Format(message, args));
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            Debug.Print("{0}: error: {1} {2}", location, message, ex.Message);
            Debug.Print(ex.StackTrace);
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            Debug.Print("{0}: error: {1} {2}", location, string.Format(message, args), ex.Message);
            Debug.Print(ex.StackTrace);
        }

        public void Warn(string message)
        {
            Warn(new NullCodeLocation(""), message);
        }

        public void Warn(string message, params object[] args)
        {
            Warn(new NullCodeLocation(""), string.Format(message, args));
        }

        public void Warn(ICodeLocation location, string message)
        {
            Debug.Print("{0}: warning: {1}", location, message);
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            Warn(location, string.Format(message, args));
        }

        public void Inform(string message)
        {
            Debug.WriteLine(message);
        }

        public void Inform(string message, params object[] args)
        {
            Debug.WriteLine(message, args);
        }

        public void Inform(ICodeLocation location, string message)
        {
            Debug.Print("{0}: {1}", location, message);
        }

        public void Inform(ICodeLocation location, string message, params object[] args)
        {
            Inform(location, string.Format(message, args));
        }

        public void ClearDiagnostics()
        {
        }

    }
}
