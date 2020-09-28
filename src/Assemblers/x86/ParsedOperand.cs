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
using Reko.Core.Assemblers;
using Reko.Core.Machine;
using System;

namespace Reko.Assemblers.x86
{
	public class ParsedOperand
	{
		private MachineOperand op;
		private bool longJmp;

		public ParsedOperand(MachineOperand op, Symbol sym, bool longJmp)
		{
			this.op = op;
			this.Symbol = sym;
			this.longJmp = longJmp;
		}

		public ParsedOperand(MachineOperand op, Symbol sym)
		{
			this.op = op;
			this.Symbol = sym;
		}

		public ParsedOperand(MachineOperand op)
		{
			this.op = op;
			this.Symbol = null;
		}

		public bool Long
		{
			get { return longJmp; }
		}

		public MachineOperand Operand
		{
			get { return op; }
		}

		public Symbol Symbol { get; set; }
	}
}
