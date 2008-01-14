/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core.Code;
using System;
using System.IO;

namespace Decompiler.Core
{
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
			signature.Emit(Name, ProcedureSignature.EmitFlags.ArgumentKind, sw);
			return sw.ToString();
		}
	}
}
