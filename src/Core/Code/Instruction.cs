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
using Reko.Core.Output;
using System.IO;

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
        /// <summary>
        /// Accept an <see cref="InstructionTransformer"/> visitor.
        /// </summary>
        /// <param name="xform">Typeless visitor.</param>
		public abstract Instruction Accept(InstructionTransformer xform);

        /// <summary>
        /// Accept a visitor.
        /// </summary>
        /// <param name="visitor">Typeless visitor.</param>
		public abstract void Accept(InstructionVisitor visitor);

        /// <summary>
        /// Accept a visitor returning an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="visitor">Visitor returning <typeparamref name="T"/>.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        public abstract T Accept<T>(InstructionVisitor<T> visitor);

        /// <summary>
        /// Accept a visitor with context, returning an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="visitor">Visitor expecting <paramref name="ctx"/> and 
        /// returning <typeparamref name="T"/>.
        /// </param>
        /// <param name="ctx">Contextual information.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        public abstract T Accept<T, C>(InstructionVisitor<T, C> visitor, C ctx);

        /// <summary>
        /// Returns true if this instruction is a control flow instruction.
        /// </summary>
		public abstract bool IsControlFlow { get; }

        /// <inheritdoc/>
		public override string ToString()
		{
			var sw = new StringWriter();
            var f = new TextFormatter(sw);
			f.Terminator = "";
			f.Indentation = 0;
			var fmt = new CodeFormatter(f);
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
        /// <summary>
        /// Construct a <see cref="DefInstruction"/> instance.
        /// </summary>
        /// <param name="e">Instruction being defined.</param>
		public DefInstruction(Identifier e)
		{
			Identifier = e;
		}

        /// <inheritdoc/>
		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformDefInstruction(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitDefInstruction(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(InstructionVisitor<T, C> visitor, C ctx)
        {
            return visitor.VisitDefInstruction(this, ctx);
        }

        /// <inheritdoc/>
        public override void Accept(InstructionVisitor v)
		{
			v.VisitDefInstruction(this);
		}

        /// <summary>
        /// The identifier being defined.
        /// </summary>
        public Identifier Identifier { get; }

        /// <inheritdoc/>
        public override bool IsControlFlow => false;
	}


    /// <summary>
    /// Models a return out of the current procedure.
    /// </summary>
	public class ReturnInstruction : Instruction
	{
        /// <summary>
        /// Constructs a return instruction with no return value.
        /// </summary>
        public ReturnInstruction()
            : this(null)
        {
        }

        /// <summary>
        /// Constructs a return instruction with an optional return value.
        /// </summary>
        /// <param name="exp"></param>
        public ReturnInstruction(Expression? exp)
        {
            this.Expression = exp;
        }

        /// <summary>
        /// Optional return value.
        /// </summary>
        public Expression? Expression { get; set; }

        /// <inheritdoc/>
        public override bool IsControlFlow => true;

        /// <inheritdoc/>
		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformReturnInstruction(this);
		}

        /// <inheritdoc/>
        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitReturnInstruction(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(InstructionVisitor<T, C> visitor, C ctx)
        {
            return visitor.VisitReturnInstruction(this, ctx);
        }

        /// <inheritdoc/>
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
        /// <summary>
        /// Constructs a <see cref="UseInstruction"/> instance.
        /// </summary>
        /// <param name="e">Expression being used.</param>
		public UseInstruction(Expression e)
		{
			this.Expression = e;
		}

        /// <summary>
        /// Constructs a <see cref="UseInstruction"/> instance,
        /// associating it with an out argument.
        /// </summary>
        /// <param name="e">Expression being used.</param>
        /// <param name="argument">Out argument to associate with.</param>
		public UseInstruction(Expression e, Identifier? argument)
		{
			this.Expression = e;
			this.OutArgument = argument;
		}

        /// <summary>
        /// Expression being used.
        /// </summary>
        public Expression Expression { get; set; }

        /// <inheritdoc/>
        public override bool IsControlFlow => false;

        /// <summary>
        /// Optional out argument to this procedure associated
        /// with the expression.
        /// </summary>
        public Identifier? OutArgument { get; set; }

        /// <inheritdoc/>
		public override Instruction Accept(InstructionTransformer xform)
		{
			return xform.TransformUseInstruction(this);
		}

        /// <inheritdoc/>
        public override T Accept<T>(InstructionVisitor<T> visitor)
        {
            return visitor.VisitUseInstruction(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(InstructionVisitor<T, C> visitor, C ctx)
        {
            return visitor.VisitUseInstruction(this, ctx);
        }

        /// <inheritdoc/>
        public override void Accept(InstructionVisitor v)
		{
			v.VisitUseInstruction(this);
		}
	}
}
