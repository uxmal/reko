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

using Decompiler.Core.Output;
using Decompiler.Core.Types;
using System;
using System.IO;
using System.Text;

namespace Decompiler.Core.Code
{
	public abstract class Instruction
	{
		public abstract Instruction Accept(InstructionTransformer xform);

		public abstract void Accept(InstructionVisitor v);
		
		public abstract bool IsControlFlow { get; }

		public override string ToString()
		{
			StringWriter sw = new StringWriter();
            Formatter f = new Formatter(sw);
			f.Terminator = "";
			f.Indentation = 0;
			CodeFormatter fmt = new CodeFormatter(f);
			Accept(fmt);
			return sw.ToString();
		}

	}

	public class DefInstruction : Instruction
	{
		private Expression expr;

		public DefInstruction(Expression e)
		{
			expr = e;
		}

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformDefInstruction(this);
		}

		public override void Accept(InstructionVisitor v)
		{
			v.VisitDefInstruction(this);
		}

		public Expression Expression
		{
			get { return expr; }
			set { expr = value; }
		}

		public override bool IsControlFlow
		{
			get { return false; }
		}

	}	
	
	public class ReturnInstruction : Instruction
	{
        private Expression expr;

        public Expression Expression
        {
            get { return expr; }
            set { expr = value; }
        }

		public ReturnInstruction(Expression v)
		{
			expr = v;
		}

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformReturnInstruction(this);
		}


		public override void Accept(InstructionVisitor v)
		{
			v.VisitReturnInstruction(this);
		}

		public override bool IsControlFlow
		{
			get { return true; }
		}
	}


	public class SwitchInstruction : Instruction
	{
        private Expression expr;
		public Block [] targets;

		public SwitchInstruction(Expression expr, Block [] targets)
		{
			this.expr = expr;
			this.targets = targets;
		}

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformSwitchInstruction(this);
		}

		public override void Accept(InstructionVisitor v)
		{
			v.VisitSwitchInstruction(this);
		}

        public Expression Expression
        {
            get { return expr; }
            set { expr = value; }
        }
        
        public override bool IsControlFlow
		{
			get { return true; }
		}
	}

	/// <summary>
	/// Used to force an expression to be live and to model out arguments.
	/// </summary>
	public class UseInstruction : Instruction
	{
		private Expression expr;
		private Identifier argOut;

		public UseInstruction(Identifier id)
		{
			this.expr = id;
		}

		public UseInstruction(Identifier id, Identifier argument)
		{
			this.expr = id;
			this.argOut = argument;
		}

		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformUseInstruction(this);
		}

		public override void Accept(InstructionVisitor v)
		{
			v.VisitUseInstruction(this);
		}

		public Expression Expression
		{
			get { return expr; }
			set { expr = value; }
		}

		public override bool IsControlFlow
		{
			get { return false; }
		}

		public Identifier OutArgument
		{
			get { return argOut; }
		}
	}

}
