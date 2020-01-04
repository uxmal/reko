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

using System;
using System.Text;

namespace Reko.Core
{
    public class Diagnostic
    {
        public Diagnostic(string message)
        {
            this.Message = message;
        }

        public string Message { get; private set; }

        public virtual string ImageKey
        {
            get { return ""; }
        }
    }

    public class ErrorDiagnostic : Diagnostic
    {
        public ErrorDiagnostic(string message) : base(message)
        {
        }

        private static string GetExceptionString(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            Exception e = ex;
            while (e != null)
            {
                sb.Append(e.Message);
                sb.Append(" ");
                e = e.InnerException;
            }
            return sb.ToString();
        }

        public ErrorDiagnostic(string message, Exception ex) : base(message +  " " + GetExceptionString(ex))
        {
        }

        public override string ImageKey
        {
            get { return "Error"; }
        }
    }


    public class WarningDiagnostic : Diagnostic
    {
        public WarningDiagnostic(string message) : base(message) { }

        public override string ImageKey
        {
            get { return "Warning"; }
        }
    }

    public class InformationalDiagnostic : Diagnostic
    {
        public InformationalDiagnostic(string message) : base(message) { }

        public override string ImageKey
        {
            get { return "Info"; }
        }
    }
}
