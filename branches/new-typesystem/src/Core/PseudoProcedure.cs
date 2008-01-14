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

using System;
using System.IO;
using Decompiler.Core;

namespace Decompiler.Core
{
	/// <summary>
	/// Represents predefined functions or processor instructions that don't have a 
	/// C/C++ equivalent (like rotate operations).
	/// </summary>
	public class PseudoProcedure : ProcedureBase
	{
		private int arity;
		private ProcedureSignature sig;

		public PseudoProcedure(string name, int arity) : base(name)
		{
			this.arity = arity;
		}

		public PseudoProcedure(string name, ProcedureSignature sig) : base(name)
		{
			this.sig = sig;
		}

		public int Arity
		{
			get { return sig != null ? sig.Arguments.Length : arity; }
		}

		public override ProcedureSignature Signature
		{
			get { return sig; }
			set { throw new InvalidOperationException("Not allowed to change the signature of a PseudoProcedure"); }
		}

		public override string ToString()
		{
			if (Signature != null)
			{
				StringWriter sw = new StringWriter();
				sig.Emit(this.Name, ProcedureSignature.EmitFlags.ArgumentKind, sw);
				return sw.ToString();
			}
			else
			{
				return string.Format("{0}({1} args)", Name, arity);
			}
		}

	}
}
