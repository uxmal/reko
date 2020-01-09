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
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.CmdLine
{
    class CmdLineDiagnosticsService : IDiagnosticsService
    {
        private TextWriter writer;

        public CmdLineDiagnosticsService(TextWriter textWriter)
        {
            this.writer = textWriter;
        }

        public void AddDiagnostic(ICodeLocation location, Diagnostic diagnostic)
        {
            throw new NotImplementedException();
        }

        public void Error(string message)
        {
            writer.WriteLine("error: {0}", message);
        }

        public void Error(string message, params object[] args)
        {
            Error(string.Format(message, args));
        }

        public void Error(Exception ex, string message)
        {
            writer.WriteLine("error: {0} {1}", message, ex);
        }

        public void Error(ICodeLocation location, string message)
        {
            writer.WriteLine("{0}: error: {1}", location, message);
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            Error(location, string.Format(message, args));
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            writer.WriteLine("{0}: error: {1} {2}", location, message, GetExceptionMessage(ex));
            writer.WriteLine(ex.StackTrace);
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            writer.WriteLine("{0}: error: {1} {2}", location, string.Format(message, args), GetExceptionMessage(ex));
            writer.WriteLine(ex.StackTrace);
        }

        public void Warn(string message)
        {
            writer.WriteLine("warning: {0}", message);
        }

        public void Warn(string message, params object[] args)
        {
            writer.WriteLine("warning: {0}", string.Format(message, args));
        }

        public void Warn(ICodeLocation location, string message)
        {
            writer.WriteLine("{0}: warning: {1}", location, message);
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            writer.WriteLine("{0}: warning: {1}", location, string.Format(message, args));
        }

        public void Inform(string message)
        {
            writer.WriteLine(message);
        }

        public void Inform(string message, params object[] args)
        {
            writer.WriteLine(message, args);
        }

        public void Inform(ICodeLocation location, string message)
        {
            writer.WriteLine("{0}: {1}", location, message);
        }

        public void Inform(ICodeLocation location, string message, params object[] args)
        {
            Inform(location, string.Format(message, args));
        }

        public void ClearDiagnostics()
        {
            throw new NotImplementedException();
        }

        private StringBuilder GetExceptionMessage(Exception ex)
        {
            var sb = new StringBuilder();
            var fmt = "{0}";
            while (ex != null)
            {
                sb.AppendFormat(fmt, ex.Message);
                fmt = " {0}";
                ex = ex.InnerException;
            }
            return sb;
        }
    }
}
