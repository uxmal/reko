#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

namespace Reko.Scanning
{
    /// <summary>
    /// Abstraction of the operations that can be performed by the
    /// <see cref="Backwalker{TBlock, TInstr}"/> class.
    /// </summary>
	public class BackwalkOperation
	{
		private BackwalkOperator op;
        /// <summary>
        /// Operand value.
        /// </summary>
		protected int operand;

        /// <summary>
        /// Constructs a backwalk operation with the given operator and operand.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="operand"></param>
		public BackwalkOperation(BackwalkOperator op, int operand)
		{
			this.op = op; this.operand = operand;
		}

        /// <summary>
        /// Applies the operation to the given value.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
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

        /// <inheritdoc/>
		public override string ToString()
		{
			return string.Format("{0} {1}", OperatorToString(op), operand);
		}
	}

    /// <summary>
    /// Models backwalking over a memory dereference.
    /// </summary>
	public class BackwalkDereference : BackwalkOperation
	{
		private readonly int entrySize;

        /// <summary>
        /// Constructs an instance of <see cref="BackwalkDereference"/>.
        /// </summary>
        /// <param name="tableOffset">Offset of the table.</param>
        /// <param name="tableEntrySize">Size of a table entry in addressable units.</param>
		public BackwalkDereference(int tableOffset, int tableEntrySize) : base(BackwalkOperator.deref, tableOffset)
		{
			this.entrySize = tableEntrySize;
		}

        /// <summary>
        /// Offset of the table in the memory.
        /// </summary>
        public int TableOffset
		{
			get { return operand; }
		}

        /// <inheritdoc />
		public override string ToString()
		{
			return string.Format("deref {0:X8} {1}", operand, entrySize);
		}
	}

    /// <summary>
    /// Models backwalking over a branch instruction.
    /// </summary>
	public class BackwalkBranch : BackwalkOperation
	{
		private readonly ConditionCode cc;

        /// <summary>
        /// Constructs an instance of <see cref="BackwalkBranch"/>.
        /// </summary>
        /// <param name="cc">Condition code for the branch.</param>
		public BackwalkBranch(ConditionCode cc) : base(BackwalkOperator.branch, 0)
		{
			this.cc = cc;
		}

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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

        /// <inheritdoc/>
		public override string ToString()
		{
			return string.Format("branch {0}", cc);
		}
	}

    /// <summary>
    /// Models an error condition during backwalking.
    /// </summary>
	public class BackwalkError : BackwalkOperation
	{
		private readonly string errorMsg;

        /// <summary>
        /// Constructs an instance of <see cref="BackwalkError"/>.
        /// </summary>
        /// <param name="errorMsg">Error message describing the error.</param>
		public BackwalkError(string errorMsg) : base(BackwalkOperator.err, 0)
		{
			this.errorMsg = errorMsg;
		}

        /// <summary>
        /// The error message describing the error condition.
        /// </summary>
		public string ErrorMessage
		{
			get { return errorMsg; }
		}

        /// <inheritdoc/>
		public override string ToString()
		{
			return string.Format("err {0}", errorMsg);
		}
	}

    /// <summary>
    /// The operators that can be used in backwalking operations.
    /// </summary>
	public enum BackwalkOperator
	{
        /// <summary>
        /// Addition operation.
        /// </summary>
		add,

        /// <summary>
        /// Multiplication operation.
        /// </summary>
        mul,

        /// <summary>
        /// Subtraction operation.
        /// </summary>
        sub,

        /// <summary>
        /// Bitwise AND operation.
        /// </summary>
        and,

        /// <summary>
        /// Comparison operation.
        /// </summary>
        cmp,

        /// <summary>
        /// Memory dereference operation.
        /// </summary>
        deref,

        /// <summary>
        /// Error operation.
        /// </summary>
        err,

        /// <summary>
        /// Branch operation.
        /// </summary>
        branch
    }
}
