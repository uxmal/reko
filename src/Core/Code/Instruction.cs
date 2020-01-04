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
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.IO;
using System.Text;

namespace Reko.Core.Code
{
    /// <summary>
    /// Base class for intermediate-level instructions. These are generated 
    /// from the low-level, register transfer instructions, and will in turn
    /// be converted to Abstract syntax statements by the latter stages of the
    /// decompilation.
    /// </summary>
	public abstract class Instruction
	{
		public abstract Instruction Accept(InstructionTransformer xform);

		public abstract void Accept(InstructionVisitor v);

        public abstract T Accept<T>(InstructionVisitor<T> visitor);

		public abstract bool IsControlFlow { get; }

        /// <summary>
        /// Utility function to simplify code that first checks the type of an instruction
        /// and then casts to it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ass"></param>
        /// <returns></returns>
        public bool As<T>(out T value) where T : Instruction
        {
            value = this as T;
            return value != null;
        }

		public override string ToString()
		{
			StringWriter sw = new StringWriter();
            TextFormatter f = new TextFormatter(sw);
			f.Terminator = "";
			f.Indentation = 0;
			CodeFormatter fmt = new CodeFormatter(f);
			Accept(fmt);
			return sw.ToString();
		}

    }

    /// <summary>
    /// This class is used in the SSA generation stage to simulate incoming parameters. DefInstructions are generated
    /// in the entry block.
    /// </summary>
	public class DefInstruction : Instruction
	{
		public DefInstruction(Identifier e)
		{
			Identifier = e;
		}

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformDefInstruction(this);
        }

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitDefInstruction(this);
        }


        public override void Accept(InstructionVisitor v)
		{
			v.VisitDefInstruction(this);
		}

        public Identifier Identifier { get; set; }

		public override bool IsControlFlow
		{
			get { return false; }
		}

	}	
	
	public class ReturnInstruction : Instruction
	{
        public ReturnInstruction()
            : this(null)
        {
        }

        public ReturnInstruction(Expression exp)
        {
            this.Expression = exp;
        }

        public Expression Expression { get; set; }
        public override bool IsControlFlow { get { return true; } }

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformReturnInstruction(this);
		}

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitReturnInstruction(this);
        }

		public override void Accept(InstructionVisitor v)
		{
			v.VisitReturnInstruction(this);
		}

	}

	/// <summary>
	/// Used to force an expression to be live and to model out arguments.
	/// </summary>
	public class UseInstruction : Instruction
	{
		public UseInstruction(Expression e)
		{
			this.Expression = e;
		}

		public UseInstruction(Expression e, Identifier argument)
		{
			this.Expression = e;
			this.OutArgument = argument;
		}

        public Expression Expression { get;  set; }
        public override bool IsControlFlow { get { return false; } }
        public Identifier OutArgument { get; set; }

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformUseInstruction(this);
		}

        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitUseInstruction(this);
        }

		public override void Accept(InstructionVisitor v)
		{
			v.VisitUseInstruction(this);
		}
	}

}
