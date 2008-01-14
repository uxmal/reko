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

namespace Decompiler.Arch.Intel.Assembler
{
	public class ParsedOperand
	{
		private Operand op;
		private Symbol sym;
		private bool longJmp;

		public ParsedOperand(Operand op, Symbol sym, bool l)
		{
			this.op = op;
			this.sym = sym;
			this.longJmp = l;
		}

		public ParsedOperand(Operand op, Symbol sym)
		{
			this.op = op;
			this.sym = sym;
		}

		public ParsedOperand(Operand op)
		{
			this.op = op;
			this.sym = null;
		}

		public bool Long
		{
			get { return longJmp; }
		}

		public Operand Operand
		{
			get { return op; }
		}

		public Symbol Symbol
		{
			get { return sym; }
		}
	}
}
