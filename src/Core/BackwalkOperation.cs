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

using Reko.Core.Expressions;
using System;

namespace Reko.Core
{
	public class BackwalkOperation
	{
		private BackwalkOperator op; 
		protected int operand;

		public BackwalkOperation(BackwalkOperator op, int operand)
		{
			this.op = op; this.operand = operand;
		}

		public virtual int Apply(int n)
		{
			switch (op)
			{
			case BackwalkOperator.cmp: return operand;
			case BackwalkOperator.add: return n + operand;
			case BackwalkOperator.sub: return n - operand;
			case BackwalkOperator.mul: return n * operand;
			case BackwalkOperator.and: return n & operand;
			case BackwalkOperator.deref: return n;
			default: throw new NotImplementedException();
			}
		}

		private string OperatorToString(BackwalkOperator op)
		{
			switch (op)
			{
			case BackwalkOperator.add: return "+";
			case BackwalkOperator.sub: return "-";
			case BackwalkOperator.mul: return "*";
			case BackwalkOperator.and: return "&";
			case BackwalkOperator.cmp: return "cmp";
			case BackwalkOperator.deref: return "deref";
			default: throw new NotImplementedException("NYI");
			}
			
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", OperatorToString(op), operand);
		}
	}

	public class BackwalkDereference : BackwalkOperation
	{
		private int entrySize;

		public BackwalkDereference(int tableOffset, int tableEntrySize) : base(BackwalkOperator.deref, tableOffset)
		{
			this.entrySize = tableEntrySize;
		}

		public int TableOffset
		{
			get { return operand; }
		}

		public override string ToString()
		{
			return string.Format("deref {0:X8} {1}", operand, entrySize);
		}
	}

	public class BackwalkBranch : BackwalkOperation
	{
		private ConditionCode cc;

		public BackwalkBranch(ConditionCode cc) : base(BackwalkOperator.branch, 0)
		{
			this.cc = cc;
		}

		public override int Apply(int limit)
		{
			switch (cc)
			{
				case ConditionCode.UGT:
				case ConditionCode.GT:
					return limit + 1;
				case ConditionCode.ULE:
					return limit + 1;
				default:
					throw new NotImplementedException();
			}
		}

		public override string ToString()
		{
			return string.Format("branch {0}", cc);
		}
	}

	public class BackwalkError : BackwalkOperation
	{
		private string errorMsg;

		public BackwalkError(string errorMsg) : base(BackwalkOperator.err, 0)
		{
			this.errorMsg = errorMsg;
		}

		public string ErrorMessage
		{
			get { return errorMsg; }
		}

		public override string ToString()
		{
			return string.Format("err {0}", errorMsg);
		}
	}

	public enum BackwalkOperator
	{
		add, mul, sub, and, cmp, deref, err, branch
	}
}
