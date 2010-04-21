/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Output;
using Decompiler.Core.Code;
using System;
using System.IO;

namespace Decompiler.Core
{
    /// <summary>
    /// Models a procedure in an API, whose signature is known, but whose code is 
    /// irrelevant to the decompilation.
    /// </summary>
	public class ExternalProcedure : ProcedureBase
	{
		private ProcedureSignature signature;

		public ExternalProcedure(string name, ProcedureSignature signature) : base(name)
		{
			this.signature = signature;
		}

		public override ProcedureSignature Signature
		{
			get { return signature; }
			set { signature = value; }
		}

		public override string ToString()
		{
			StringWriter sw = new StringWriter();
            Formatter fmt = new Formatter(sw);
            CodeFormatter cf = new CodeFormatter(fmt);
            TypeFormatter tf = new TypeFormatter(fmt, false);
			signature.Emit(Name, ProcedureSignature.EmitFlags.ArgumentKind, fmt, cf, tf);
			return sw.ToString();
		}
	}
}
