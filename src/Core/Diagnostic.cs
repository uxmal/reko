/* 
 * Copyright (C) 1999-2009 John Källén.
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

using System;

namespace Decompiler.Core
{
    [Obsolete("Use the subclasses of Diagnostic instead.")]
    public enum DiagnosticOld
    {
        FatalError,
        Error,
        Warning,
        Info
    }

    public class Diagnostic
    {
        private Address addr;
        private string message;

        public Diagnostic(Address addr, string messageFormat, params object[] args)
        {
            this.addr = addr;
            this.message = string.Format(messageFormat, args);
        }

        public Address Address
        {
            get { return addr; }
        }

        public string Message
        {
            get { return message; }
        }
    }

    public class ErrorDiagnostic : Diagnostic
    {
        public ErrorDiagnostic(Address addr, string messageFormat, params object[] args)
            : base(addr, messageFormat, args)
        {
        }

    }


    public class WarningDiagnostic : Diagnostic
    {
        public WarningDiagnostic(Address addr, string messageFormat, params object[] args)
            : base(addr, messageFormat, args)
        {
        }

    }
}

