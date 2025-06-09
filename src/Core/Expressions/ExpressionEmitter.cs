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

using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#pragma warning disable CA1822

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Factory class that has the extra benefit of reducing the verbosity of the code.
    /// </summary>
    public class ExpressionEmitter
    {
        /// <summary>
        /// Generate an array access.
        /// </summary>
        /// <param name="elemType">The element type of the array.</param>
        /// <param name="arrayPtr">Pointer to beginning of array.</param>
        /// <param name="index">Index into array.</param>
        /// <returns></returns>
        public ArrayAccess Array(DataType elemType, Expression arrayPtr, Expression index)
        {
            return new ArrayAccess(elemType, arrayPtr, index);
        }

        /// <summary>
        /// Generates an integer two's complement addition-with-carry expression.
        /// </summary>
        /// <param name="left">Augend</param>
        /// <param name="right">Addend</param>
        /// <param name="carry">Carry flag</param>
        /// <returns>A binary expression for the add-with-carry.</returns>
        public BinaryExpression IAddC(Expression left, Expression right, Expression carry)
        {
            return IAdd(IAdd(left, right), carry);
        }

        /// <summary>
        /// Generates an integer two's complement addition expression. Signedness
        /// doesn't matter here.
        /// </summary>
        /// <param name="left">Augend</param>
        /// <param name="right">Addend</param>
        /// <returns>A binary expression for the sum.</returns>
        public BinaryExpression IAdd(Expression left, Expression right)
        {
            var bitSize = left.DataType.BitSize;
            var dtResult = bitSize > 0 ? PrimitiveType.CreateWord(bitSize) : (DataType) new UnknownType();
            return new BinaryExpression(Operator.IAdd, dtResult, left, right);
        }

        /// <summary>
        /// Convenience method for addition. The addend is converted to a Constant
        /// of the same size as the augend.
        /// </summary>
        /// <param name="left">Augend</param>
        /// <param name="right">Addend</param>
        /// <returns>A binary expression for the sum.</returns>
        public BinaryExpression IAdd(Expression left, long right)
        {
            return IAdd(left, Word(left.DataType.BitSize, right));
        }

        /// <summary>
        /// Convenience method for addition. The addend is converted to a
        /// signed integer Constant of the same size as the augend.
        /// </summary>
        /// <param name="left">Augend</param>
        /// <param name="right">Addend</param>
        /// <returns>A binary expression for the sum.</returns>
        public BinaryExpression IAddS(Expression left, long right)
        {
            return IAdd(left, Constant.Int(left.DataType, right));
        }

        /// <summary>
        /// Creates an offset sum of <paramref name="e"/> and the
        /// signed integer <paramref name="c"/>
        /// </summary>
        /// <param name="e">Expression forming the base of offset sum.</param>
        /// <param name="c">Signed offset</param>
        /// <returns>
        /// Return addition if <paramref name="c"/> is positive
        /// Return subtraction if <paramref name="c"/> is negative
        /// </returns>
        public Expression AddSubSignedInt(Expression e, long c)
        {
            if (c == 0)
            {
                return e;
            }
            else if (c > 0)
            {
                return IAddS(e, c);
            }
            else
            {
                return ISubS(e, -c);
            }
        }

        /// <summary>
        /// Takes the address of the expression (which must be an l-value).
        /// </summary>
        /// <param name="ptType">Type of the resulting pointer.</param>
        /// <param name="e">L-value</param>
        /// <returns>A unary expresssion representing the address-of operation.</returns>
        public UnaryExpression AddrOf(DataType ptType, Expression e)
        {
            return new UnaryExpression(Operator.AddrOf, ptType, e);
        }

        /// <summary>
        /// Convenience method for generating the bitwise logical AND of the two factors. The 
        /// second parameter is converted to a Constant of the appropriate width.
        /// </summary>
        /// <param name="left">Logical factor</param>
        /// <param name="right"></param>
        /// <returns>The conjunction of the factors.</returns>
        public BinaryExpression And(Expression left, ulong right)
        {
            return And(left, Constant.Create(left.DataType, right));
        }

        /// <summary>
        /// Generates the bitwise logical AND of the two factors.
        /// </summary>
        /// <param name="left">Logical factor</param>
        /// <param name="right"></param>
        /// <returns>The conjunction of the factors.</returns>
        public BinaryExpression And(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.And, left.DataType, left, right);
        }

        /// <summary>
        /// Generates an array access of the array <paramref name="array"/> at
        /// element # <paramref name="index"/>. The type of the referenced array
        /// element 
        /// </summary>
        /// <param name="elementType">The data type of the accessed element.</param>
        /// <param name="array">Expression representing the array, typically a pointer.</param>
        /// <param name="index">Expression for the index, which should be integral.</param>
        /// <returns>An array access.</returns>
        public ArrayAccess ARef(DataType elementType, Expression array, Expression index)
        {
            return new ArrayAccess(elementType, array, index);
        }

        /// <summary>
        /// Convenience method that generates a <see cref="BinaryExpression"/> using
        /// the provided <see cref="BinaryOperator"/> and assuming the returned 
        /// data type is the same as the one of its first operand.
        /// </summary>
        /// <param name="op">The binary operator of the resulting expression.</param>
        /// <param name="left">The first operand of the resulting expression.</param>
        /// <param name="right">The second operand of the resulting expression.</param>
        /// <returns>A binary expression.</returns>
        public BinaryExpression Bin(BinaryOperator op, Expression left, Expression right)
        {
            return new BinaryExpression(op, left.DataType, left, right);
        }

        /// <summary>
        /// Convenience method that generates a <see cref="BinaryExpression"/> using
        /// the provided <see cref="BinaryOperator"/> and returning a value of 
        /// the given data type <paramref name="dt"/>.
        /// </summary>
        /// <param name="op">The binary operator of the resulting expression.</param>
        /// <param name="dt">The <see cref="DataType"/> of the result of the 
        /// binary expression.</param>
        /// <param name="left">The first operand of the resulting expression.</param>
        /// <param name="right">The second operand of the resulting expression.</param>
        /// <returns>A binary expression.</returns>
        public BinaryExpression Bin(BinaryOperator op, DataType dt, Expression left, Expression right)
        {
            return new BinaryExpression(op, dt, left, right);
        }

        /// <summary>
        /// Convenience method that generates a <see cref="BinaryExpression"/> using
        /// the provided <see cref="BinaryOperator"/> and assuming the returned 
        /// data type is the same as the one of its first operand.
        /// </summary>
        /// <param name="op">The binary operator of the resulting expression.</param>
        /// <param name="left">The first operand of the resulting expression.</param>
        /// <param name="right">The second operand of the resulting expression.</param>
        /// <param name="carryFlag">The expression holding the carry flag.</param>
        /// <returns>A binary expression.</returns>
        public BinaryExpression BinC(BinaryOperator op, Expression left, Expression right, Expression carryFlag)
        {
            return new BinaryExpression(
                op,
                left.DataType,
                new BinaryExpression(op, left.DataType, left, right),
                carryFlag);
        }

        /// <summary>
        /// Short-circuiting 'and' ('&amp;&amp;' in C family of languages)
        /// </summary>
        /// <param name="a">Antecedent expression.</param>
        /// <param name="b">Subsequent expression.</param>
        /// <returns>Short-circuiting 'and' expression.</returns>
        public Expression Cand(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Cand, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Generates the bitwise complement of <paramref name="expr"/> (the '~' 
        /// operator in the C language family).
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public Expression Comp(Expression expr)
        {
            return new UnaryExpression(Operator.Comp, expr.DataType, expr); 
        }

        /// <summary>
        /// Generate a set of condition codes for the expression <paramref name="expr"/>.
        /// </summary>
        /// <remarks>
        /// This is used to model condition condes on architectures where these
        /// are used. For instance, a typical `add eax,4` instruction on a machine with
        /// condition codes would be coded:
        /// <code>
        /// m.Assign(eax, m.Add(eax, 4))
        /// m.Assign(SZCO, m.Cond(eax))
        /// </code>
        /// </remarks>
        /// <param name="expr">Expression whose value is the source of the condition 
        /// code</param>
        /// <returns></returns>
        public ConditionOf Cond(Expression expr)
        {
            return new ConditionOf(expr);
        }

        /// <summary>
        /// Expresses a ternary conditional expression (in C syntax, expressed with
        /// ? and :).
        /// </summary>
        /// <param name="dt">data type of the result of the conditional.</param>
        /// <param name="cond">the condition that is tested.</param>
        /// <param name="th">the consequent expression.</param>
        /// <param name="el">the alternative expression.</param>
        /// <returns>Ternary conditional expression.</returns>
        public ConditionalExpression Conditional(DataType dt, Expression cond, Expression th, Expression el)
        {
            return new ConditionalExpression(dt, cond, th, el);
        }

        /// <summary>
        /// Short-circuiting 'or' ('||' in C family of languages).
        /// </summary>
        /// <param name="a">Antecedent expression.</param>
        /// <param name="b">Subsequent expression.</param>
        /// <returns>Short-circuiting 'or' expression.</returns>
        public Expression Cor(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Cor, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Generates a <see cref="Conversion"/> expression which converts the <paramref name="expr"/> from 
        /// the data type <paramref name="dataTypeFrom" /> to the data type <paramref name="dataTypeTo"/>.
        /// </summary>
        /// <param name="expr">Value to convert.</param>
        /// <param name="dataTypeFrom">Type to convert from.</param>
        /// <param name="dataTypeTo">Type to convert to.</param>
        /// <returns>A <see cref="Conversion"/> expression.</returns>
        public Conversion Convert(Expression expr, DataType dataTypeFrom, DataType dataTypeTo)
        {
            return new Conversion(expr, dataTypeFrom, dataTypeTo);
        }

        /// <summary>
        /// Generates a simple Dereference operation ('<c>*a</c>' in the C language
        /// family). If you don't know the exact data type of <paramref name="a"/>,
        /// use one of the <c>Mem...</c> methods instead.
        /// </summary>
        /// <param name="a">Expression to dereference.</param>
        /// <returns>A C-style dereference.</returns>
        public Dereference Deref(Expression a)
        {
            return new Dereference(a.DataType, a);
        }

        /// <summary>
        /// Generate a sequence of expressions which model the act
        /// of updating a subsequence of bits inside of a wider value.
        /// </summary>
        /// <param name="dst">The wider value, expected to be a <see cref="Identifier"/> or a
        /// <see cref="Constant"/>.</param>
        /// <param name="src">The smaller value to deposit.</param>
        /// <param name="offset">The bit offset at which to deposit the value.</param>
        /// <returns>A deposit-bits expression.</returns>
        public MkSequence Dpb(Expression dst, Expression src, int offset)
        {
            Debug.Assert(dst is Identifier || dst is Constant);
            var exps = new List<Expression>();
            if (offset > 0)
            {
                exps.Add(Slice(dst, PrimitiveType.CreateWord(offset)));
            }
            var msb = Math.Min(dst.DataType.BitSize, offset + src.DataType.BitSize);
            var dtInserted = PrimitiveType.CreateWord(msb - offset);
            if (dtInserted.BitSize < src.DataType.BitSize)
            {
                exps.Add(Slice(src, dtInserted));
            }
            else
            {
                exps.Add(src);
            }
            if (msb < dst.DataType.BitSize)
            {
                exps.Add(Slice(dst, PrimitiveType.CreateWord(dst.DataType.BitSize - msb), msb));
            }
            exps.Reverse();
            return new MkSequence(dst.DataType, exps.ToArray());
        }

        /// <summary>
        /// Generate an equality comparison ('==' in the C language family).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Comparison expression.</returns>
        public BinaryExpression Eq(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Eq, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Convenience method for equality comparison. The second parameter
        /// is converted to a <see cref="Constant"/> first.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Comparison expression.</returns>
        public BinaryExpression Eq(Expression a, int b)
        {
            return new BinaryExpression(Operator.Eq, PrimitiveType.Bool, a, Constant.Create(a.DataType, b));
        }

        /// <summary>
        /// Equality comparison with zero.
        /// </summary>
        /// <param name="exp">Expression to compare with zero.</param>
        /// <returns>Comparison expression.</returns>
        public BinaryExpression Eq0(Expression exp)
        {
            return new BinaryExpression(Operator.Eq, PrimitiveType.Bool, exp, Constant.Create(exp.DataType, 0));
        }

        /// <summary>
        /// Sign extend the expression <paramref name="exp"/> to the same size as 
        /// <paramref name="newSize"/>.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        public Conversion ExtendS(Expression exp, DataType newSize)
        {
            var dtDst = PrimitiveType.Create(Domain.SignedInt, newSize.BitSize);
            return new Conversion(exp, exp.DataType, dtDst);
        }

        /// <summary>
        /// Zero extend the expression <paramref name="exp"/> to the same size as 
        /// <paramref name="newSize"/>.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        public Conversion ExtendZ(Expression exp, DataType newSize)
        {
            var dtDst = PrimitiveType.CreateWord(newSize.BitSize);
            return new Conversion(exp, exp.DataType, dtDst);
        }


        // Floating point operations.

        /// <summary>
        /// Generate a floating point addition.
        /// </summary>
        /// <param name="a">Augend</param>
        /// <param name="b">Addend</param>
        /// <returns>A floating point sum expression.</returns>
        public Expression FAdd(Expression a, Expression b)
        {
            var dtSum = PrimitiveType.Create(Domain.Real, a.DataType.BitSize);
            return new BinaryExpression(Operator.FAdd, dtSum, a, b);
        }

        /// <summary>
        /// Generate a floating point division.
        /// </summary>
        /// <param name="a">Dividend.</param>
        /// <param name="b">Divisor.</param>
        /// <returns>A floating point division expression.</returns>
        public Expression FDiv(Expression a, Expression b)
        {
            var dtSum = PrimitiveType.Create(Domain.Real, a.DataType.BitSize);
            return new BinaryExpression(Operator.FDiv, dtSum, a, b);
        }

        /// <summary>
        /// Generate a floating point modulus operation.
        /// </summary>
        /// <param name="a">Dividend.</param>
        /// <param name="b">Divisor.</param>
        /// <returns>A floating point modulies operation.</returns>
        public Expression FMod(Expression a, Expression b)
        {
            var dtMod = PrimitiveType.Create(Domain.Real, a.DataType.BitSize);
            return new BinaryExpression(Operator.FMod, dtMod, a, b);
        }

        /// <summary>
        /// Generate a floating point multiplication.
        /// </summary>
        /// <param name="a">Multiplicand.</param>
        /// <param name="b">Multiplier.</param>
        /// <returns>A floating point multiplication expression.</returns>
        public Expression FMul(Expression a, Expression b)
        {
            var dtSum = PrimitiveType.Create(Domain.Real, a.DataType.BitSize);
            return new BinaryExpression(Operator.FMul, dtSum, a, b);
        }

        /// <summary>
        /// Generate a floating point multiplication where the product differs
        /// in size from the operands.
        /// </summary>
        /// <param name="dtProduct">Datatype of the result.</param>
        /// <param name="a">Multiplicand.</param>
        /// <param name="b">Multiplier.</param>
        /// <returns>A floating point multiplication expression.</returns>
        public Expression FMul(DataType dtProduct, Expression a, Expression b)
        {
            return new BinaryExpression(Operator.FMul, dtProduct, a, b);
        }

        /// <summary>
        /// Generates a floating point unary negation (implicit subtraction
        /// from floating point zero).
        /// </summary>
        /// <param name="a">Floating point expression to be negated.</param>
        /// <returns>A floating point negation.</returns>
        public Expression FNeg(Expression a)
        {
            return new UnaryExpression(Operator.FNeg, a.DataType, a);
        }

        /// <summary>
        /// Generates a floating point equality comparison ('==' in the C
        /// language family).
        /// </summary>
        /// <returns>A floating point comparison expression.</returns>
        public Expression FEq(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Feq, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Generates a floating point inequality comparison ('!=' in the C
        /// language family).
        /// </summary>
        /// <returns>A floating point comparison expression.</returns>
        public Expression FNe(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Fne, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Generates a floating point greater-or-equal comparison ('>=' in the C
        /// language family).
        /// </summary>
        /// <returns>A floating point inequality comparison.</returns>
        public Expression FGe(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Fge, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Generates a floating point greater-than comparison ('>' in the C
        /// language family).
        /// </summary>
        /// <returns>A floating point inequality comparison.</returns>
        public Expression FGt(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Fgt, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Generates a floating point less-or-equal comparison ('&lt;=' in the C
        /// language family).
        /// </summary>
        /// <returns>A floating point inequality comparison.</returns>
        public Expression FLe(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Fle, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Generates a floating point less-than comparison ('&lt;' in the C
        /// language family).
        /// </summary>
        /// <returns>A floating point inequality comparison.</returns>
        public Expression FLt(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Flt, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Generates a floating point subtraction expression.
        /// </summary>
        /// <param name="a">Minuend.</param>
        /// <param name="b">Subtrahend.</param>
        /// <returns>A floating point subtraction expression.</returns>
        public BinaryExpression FSub(Expression a, Expression b)
        {
            var dtSum = PrimitiveType.Create(Domain.Real, a.DataType.BitSize);
            return new BinaryExpression(Operator.FSub, dtSum, a, b);
        }

        /// <summary>
        /// Generate a field access ('foo.bar' in the C language family).
        /// </summary>
        /// <param name="dt">Data type of the field access.</param>
        /// <param name="e">Structured type whose field we're accessing.</param>
        /// <param name="field">The field being accessed.</param>
        /// <returns>A field-access expression.</returns>
        public FieldAccess Field(DataType dt, Expression e, Field field)
        {
            return new FieldAccess(dt, e, field);
        }

        // Function applications

        /// <summary>
        /// Convenience method to generate a function application that applies
        /// the function-valued expression <paramref name="fn"/> to
        /// the arguments <paramref name="args" /> and returns a 32-bit value. Used 
        /// primarily in unit tests.
        /// </summary>
        /// <param name="fn">The function to apply.</param>
        /// <param name="args">The arguments of the function.</param>
        /// <returns>A function application</returns>
        public Application Fn(Expression fn, params Expression[] args)
        {
            return Fn(fn, PrimitiveType.Word32, args);
        }

        /// <summary>
        /// Generate a function application that applies the function-valued
        /// expression <paramref name="fn"/> to
        /// the arguments <paramref name="exps" /> and returns a value of type 
        /// <param name="retType"/>.
        /// </summary>
        /// <param name="fn">The function to apply.</param>
        /// <param name="exps">The arguments of the function.</param>
        /// <returns>A function application</returns>
        public Application Fn(Expression fn, DataType retType, params Expression[] exps)
        {
            return new Application(fn, retType, exps);
        }

        /// <summary>
        /// Generate a function application that applies the external 
        /// procedure <paramref name="ep"/> to
        /// the arguments <paramref name="args" /> and returns a value of the
        /// return type of the external procedure.
        /// </summary>
        /// <param name="ep">The external procedure to apply.</param>
        /// <param name="args">The arguments of the function.</param>
        /// <returns>A function application</returns>
        public Application Fn(ExternalProcedure ep, params Expression[] args)
        {
            var retType = ep.Signature.ReturnValue is not null
                ? ep.Signature.ReturnValue.DataType
                : VoidType.Instance;
            return new Application(
                new ProcedureConstant(PrimitiveType.Ptr32, ep), 
                retType,
                args);
        }

        /// <summary>
        /// Generates a function application that applies the intrinsic 
        /// procedure <paramref name="intrinsic"/> to
        /// the arguments <paramref name="args" /> and returns a value of the
        /// return type of the external procedure. Use this when modelling
        /// processor-specific intrinsic functions that cannot be expressed
        /// in RTL any other way.
        /// </summary>
        /// <param name="intrinsic">The instrinsic function to apply.</param>
        /// <param name="args">The arguments of the function.</param>
        /// <returns>A function application</returns>
        public Application Fn(IntrinsicProcedure intrinsic,  params Expression[] args)
        {
            if (intrinsic.IsGeneric && !intrinsic.IsConcreteGeneric)
            {
                var types = new DataType[args.Length];
                for (int i = 0; i < types.Length; ++i)
                {
                    types[i] = args[i].DataType;
                }
                intrinsic = intrinsic.MakeInstance(types);
            }
            var sig = intrinsic.Signature;
            if (sig is not null && !HasCorrectNumberOfParameters(sig, args))
                throw new InvalidOperationException(
                    $"Parameter count for {intrinsic.Name} must match argument count. " +
                    $"Expected: {sig.Parameters!.Length}. " +
                    $"But was: {args.Length}.");
            return new Application(new ProcedureConstant(PrimitiveType.Ptr32, intrinsic), intrinsic.ReturnType, args);
        }

        private static bool HasCorrectNumberOfParameters(FunctionType sig, Expression[] args)
        {
            if (sig.IsVariadic)
                return sig.Parameters!.Length <= args.Length;
            else
                return sig.Parameters!.Length == args.Length;
        }

        /// <summary>
        /// Generates a function application that applies the intrinsic 
        /// procedure <paramref name="intrinsic"/> to
        /// the arguments <paramref name="args" /> and returns a value of the
        /// return type of the external procedure. Use this when modelling
        /// processor-specific intrinsic functions that cannot be expressed
        /// in RTL any other way. The called procedure <paramref name="intrinsic"/>
        /// is considered variadic, so no attempt is made to validate the number
        /// of parameters.
        /// </summary>
        /// <param name="intrinsic">The instrinsic function to apply.</param>
        /// <param name="args">The arguments of the function.</param>
        /// <returns>A function application</returns>
        public Application FnVariadic(IntrinsicProcedure intrinsic, params Expression[] args)
        {
            if (intrinsic.IsGeneric && !intrinsic.IsConcreteGeneric)
            {
                var types = new DataType[args.Length];
                for (int i = 0; i < types.Length; ++i)
                {
                    types[i] = args[i].DataType;
                }
                intrinsic = intrinsic.MakeInstance(types);
            }
            return new Application(new ProcedureConstant(PrimitiveType.Ptr32, intrinsic), intrinsic.ReturnType, args);
        }

        /// <summary>
        /// Generates a signed integer greater-or-equal comparison
        /// ('>=' in the C language family).
        /// </summary>
        /// <returns>Signed integer comparison.</returns>
        public Expression Ge(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ge, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Convenience function that generates a signed integer greater-or-equal comparison
        /// ('>=' in the C language family). The second parameter is first converted
        /// to a <see cref="Constant" />.
        /// </summary>
        /// <returns>Signed integer comparison.</returns>
        public Expression Ge(Expression a, int b)
        {
            return Ge(a, Constant.Create(a.DataType, b));
        }

        /// <summary>
        /// Generates a signed integer greater-or-equal-to-0 comparison
        /// ('>= 0' in the C language family).
        /// </summary>
        /// <returns>Signed integer comparison.</returns>
        public BinaryExpression Ge0(Expression exp)
        {
            return new BinaryExpression(Operator.Ge, PrimitiveType.Bool, exp, Constant.Zero(exp.DataType));
        }

        /// <summary>
        /// Generates a signed integer greater-than comparison
        /// ('>' in the C language family).
        /// </summary>
        /// <returns>Signed integer comparison.</returns>
        public BinaryExpression Gt(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Gt, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Convenience function that generates a signed integer greater-than comparison
        /// ('>' in the C language family). The second parameter is first converted
        /// to a <see cref="Constant" />.
        /// </summary>
        /// <returns>Signed integer comparison.</returns>
        public BinaryExpression Gt(Expression a, int b)
        {
            return Gt(a, Int32(b));
        }

        /// <summary>
        /// Generates a signed integer greater-than-0 comparison
        /// ('> 0' in the C language family).
        /// </summary>
        /// <returns>Signed integer comparison.</returns>
        public BinaryExpression Gt0(Expression exp)
        {
            return new BinaryExpression(Operator.Gt, PrimitiveType.Bool, exp, Constant.Zero(exp.DataType));
        }

        /// <summary>
        /// Generates a signed 8-bit integer constant.
        /// </summary>
        /// <param name="n">8-bit signed integer.</param>
        /// <returns>Constant representing a signed 8-bit integer.</returns>
        public Constant Int8(int n)
        {
            return Constant.SByte((sbyte)n);
        }

        /// <summary>
        /// Generates a signed 16-bit integer constant.
        /// </summary>
        /// <param name="n">16-bit signed integer.</param>
        /// <returns>Constant representing a signed 16-bit integer.</returns>
        public Constant Int16(short n)
        {
            return Constant.Int16(n);
        }

        /// <summary>
        /// Generates a signed 16-bit integer constant.
        /// </summary>
        /// <param name="n">16-bit unsigned integer, which will be cast to a signed value.</param>
        /// <returns>Constant representing a signed 16-bit integer.</returns>
        public Constant Int16(uint n)
        {
            return Constant.Int16((short) n);
        }

        /// <summary>
        /// Generates a signed 32-bit integer constant.
        /// </summary>
        /// <param name="n">32-bit unsigned integer, which will be cast to a signed value.</param>
        /// <returns>Constant representing a signed 32-bit integer.</returns>
        public Constant Int32(uint n)
        {
            return Constant.Int32((int)n);
        }

        /// <summary>
        /// Generates a signed 32-bit integer constant.
        /// </summary>
        /// <param name="n">32-bit signed integer.</param>
        /// <returns>Constant representing a signed 32-bit integer.</returns>
        public Constant Int32(int n)
        {
            return Constant.Int32(n);
        }

        /// <summary>
        /// Generates a memory access to the specified effective address, 
        /// <paramref name="ea"/>.
        /// </summary>
        /// <param name="dt">Data type of the memory access.</param>
        /// <param name="ea">The address of the memory being accessed.</param>
        /// <returns>A memory access expression.</returns>
        public MemoryAccess Mem(DataType dt, Expression ea)
        {
            return Mem(MemoryStorage.GlobalMemory, dt, ea);
        }

        /// <summary>
        /// Generates a memory access of the data of type <paramref name="dt"/> at the specified effective address
        /// <paramref name="ea"/> in the address space identified by <paramref name="mid" />.
        /// </summary>
        /// <param name="mid">The memory identifier to use for this access.</param>
        /// <param name="dt">The data type of the memory access.</param>
        /// <param name="ea">The address of the memory being accessed.</param>
        /// <returns>A memory access expression.</returns>
        public virtual MemoryAccess Mem(
            Identifier mid,
            DataType dt,
            Expression ea)
        {
            return new MemoryAccess(mid, ea, dt);
        }

        /// <summary>
        /// Generates a memory access of the byte at the specified effective address
        /// <paramref name="ea"/>.
        /// </summary>
        /// <param name="ea">The address of the memory being accessed.</param>
        /// <returns>A memory access expression.</returns>
        public virtual MemoryAccess Mem8(Expression ea)
        {
            return new MemoryAccess(MemoryStorage.GlobalMemory, ea, PrimitiveType.Byte);
        }

        /// <summary>
        /// Generates a memory access of the 16-bit word at the specified effective address
        /// <paramref name="ea"/>.
        /// </summary>
        /// <param name="ea">The address of the memory being accessed.</param>
        /// <returns>A memory access expression.</returns>
        public virtual MemoryAccess Mem16(Expression ea)
        {
            return new MemoryAccess(MemoryStorage.GlobalMemory, ea, PrimitiveType.Word16);
        }

        /// <summary>
        /// Generates a memory access of the 32-bit word at the specified effective address
        /// <paramref name="ea"/>.
        /// </summary>
        /// <param name="ea">The address of the memory being accessed.</param>
        /// <returns>A memory access expression.</returns>
        public virtual MemoryAccess Mem32(Expression ea)
        {
            return new MemoryAccess(MemoryStorage.GlobalMemory, ea, PrimitiveType.Word32);
        }

        /// <summary>
        /// Generates a memory access of the 64-bit word at the specified effective address
        /// <paramref name="ea"/>.
        /// </summary>
        /// <param name="ea">The address of the memory being accessed.</param>
        /// <returns>A memory access expression.</returns>
        public virtual MemoryAccess Mem64(Expression ea)
        {
            return new MemoryAccess(MemoryStorage.GlobalMemory, ea, PrimitiveType.Word64);
        }

        /// <summary>
        /// Generates a signed integer less-or-equal comparison
        /// ('&lt;=' in the C language family).
        /// </summary>
        /// <returns>Signed integer comparison.</returns>
        public BinaryExpression Le(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Le, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Convenience function that generates a signed integer less-or-equal comparison
        /// ('&lt;=' in the C language family). The second parameter is first converted
        /// to a Constant.
        /// </summary>
        /// <returns>Signed integer comparison.</returns>
        public BinaryExpression Le(Expression a, int b)
        {
            return Le(a, Constant.Create(a.DataType, b));
        }

        /// <summary>
        /// Generates a signed integer less--or-equal-0 comparison
        /// ('&lt;= 0' in the C language family).
        /// </summary>
        /// <returns>Signed integer comparison.</returns>
        public BinaryExpression Le0(Expression exp)
        {
            return new BinaryExpression(Operator.Le, PrimitiveType.Bool, exp, Constant.Zero(exp.DataType));
        }

        /// <summary>
        /// Generates a signed integer less-than comparison
        /// ('&lt;' in the C language family).
        /// </summary>
        /// <returns>Signed integer comparison.</returns>
        public BinaryExpression Lt(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Lt, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Convenience function that generates a signed integer less-than comparison
        /// ('&lt;' in the C language family). The second parameter is first converted
        /// to a <see cref="Constant" />.
        /// </summary>
        /// <returns>Signed integer comparison.</returns>
        public BinaryExpression Lt(Expression a, int b)
        {
            return Lt(a, Constant.Create(a.DataType, b));
        }

        /// <summary>
        /// Generates a signed integer less--or-equal-0 comparison
        /// ('&lt;= 0' in the C language family).
        /// </summary>
        /// <returns>Signed integer comparison.</returns>
        public BinaryExpression Lt0(Expression exp)
        {
            return new BinaryExpression(Operator.Lt, PrimitiveType.Bool, exp, Constant.Zero(exp.DataType));
        }

        /// <summary>
        /// Generates a member pointer dereference (similar to the C++ ptr->*membPtr) to an 8-bit field.
        /// Used only in unit tests.
        /// </summary>
        /// <param name="ptr">Base pointer</param>
        /// <param name="membPtr">Offset pointer</param>
        /// <returns>A member pointer dereference.</returns>
        public MemberPointerSelector MembPtr8(Expression ptr, Expression membPtr)
        {
            return new MemberPointerSelector(PrimitiveType.Byte, new Dereference(PrimitiveType.Ptr32, ptr), membPtr);
        }

        /// <summary>
        /// Generates a member pointer dereference (similar to the C++ ptr->*membPtr) to an 16-bit field.
        /// Used only in unit tests.
        /// </summary>
        /// <param name="ptr">Base pointer</param>
        /// <param name="membPtr">Offset pointer</param>
        /// <returns>A member pointer dereference.</returns>
        public MemberPointerSelector MembPtr16(Expression ptr, Expression membPtr)
        {
            return new MemberPointerSelector(PrimitiveType.Word16, new Dereference(PrimitiveType.Ptr32, ptr), membPtr);
        }

        /// <summary>
        /// Generate the integer modulus ('%' in the C language family).
        /// </summary>
        /// <param name="opLeft">Dividend.</param>
        /// <param name="opRight">Divisor.</param>
        /// <returns>A modulus expression.</returns>
        public Expression Mod(Expression opLeft, Expression opRight)
        {
            return new BinaryExpression(Operator.IMod, opLeft.DataType, opLeft, opRight);
        }

        /// <summary>
        /// Generate the integer modulus ('%' in the C language family) where the 
        /// size of the modulus is not the same as the dividend.
        /// </summary>
        /// <param name="dt">Size of modulus.</param>
        /// <param name="opLeft">Dividend.</param>
        /// <param name="opRight">Divisor.</param>
        /// <returns>A modulus expression.</returns>
        public Expression Mod(DataType dt, Expression opLeft, Expression opRight)
        {
            return new BinaryExpression(Operator.IMod, dt, opLeft, opRight);
        }

        /// <summary>
        /// Generate the modulus ('%' in the C language family) of signed integers.
        /// </summary>
        /// <param name="opLeft">Dividend.</param>
        /// <param name="opRight">Divisor.</param>
        /// <returns>A modulus expression.</returns>
        public Expression SMod(Expression opLeft, Expression opRight)
        {
            return new BinaryExpression(Operator.SMod, opLeft.DataType, opLeft, opRight);
        }

        /// <summary>
        /// Generate the modulus ('%' in the C language family) of unsigned integers.
        /// </summary>
        /// <param name="opLeft">Dividend.</param>
        /// <param name="opRight">Divisor.</param>
        /// <returns>A modulus expression.</returns>
        public Expression UMod(Expression opLeft, Expression opRight)
        {
            return new BinaryExpression(Operator.UMod, opLeft.DataType, opLeft, opRight);
        }

        /// <summary>
        /// Generate the modulus ('%' in the C language family) of signed integers 
        /// where the size of the modulus is not the same as the dividend.
        /// </summary>
        /// <param name="dt">Size of modulus.</param>
        /// <param name="opLeft">Dividend.</param>
        /// <param name="opRight">Divisor.</param>
        /// <returns>A modulus expression.</returns>
        public Expression SMod(DataType dt, Expression opLeft, Expression opRight)
        {
            return new BinaryExpression(Operator.SMod, dt, opLeft, opRight);
        }

        /// <summary>
        /// Generate the modulus ('%' in the C language family) of unsigned integers 
        /// where the size of the modulus is not the same as the dividend.
        /// </summary>
        /// <param name="dt">Size of modulus.</param>
        /// <param name="opLeft">Dividend.</param>
        /// <param name="opRight">Divisor.</param>
        /// <returns>A modulus expression.</returns>
        public Expression UMod(DataType dt, Expression opLeft, Expression opRight)
        {
            return new BinaryExpression(Operator.UMod, dt, opLeft, opRight);
        }

        /// <summary>
        /// Generates an integer inequality comparison ('!=' in the C language family).
        /// </summary>
        /// <returns>An integer comparison expression.</returns>
        public BinaryExpression Ne(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ne, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Convenience method to generate an integer inequality comparison ('!=' in the
        /// C language family). The second argument is converted to a Constant.
        /// </summary>
        /// <returns>An integer comparison expression.</returns>
        public BinaryExpression Ne(Expression a, long n)
        {
            return new BinaryExpression(Operator.Ne, PrimitiveType.Bool, a, Constant.Create(a.DataType, n));
        }

        /// <summary>
        /// Generates an integer inequality comparison ('!= 0' in the C language family).
        /// </summary>
        /// <returns>An integer comparison expression.</returns>
        public BinaryExpression Ne0(Expression expr)
        {
            return new BinaryExpression(
                Operator.Ne, PrimitiveType.Bool, expr, Constant.Create(expr.DataType, 0));
        }


        /// <summary>
        /// Generates an integer unary negation expression ('-expr' in the C language family).
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>A negated integer expression.</returns>
        public UnaryExpression Neg(Expression expr)
        {
            return new UnaryExpression(
                Operator.Neg,
                expr.DataType,
                expr);
        }

        /// <summary>
        /// Generates an out argument reference in a function application. Used when
        /// calling functions that return more than one value.
        /// </summary>
        /// <param name="dt">Data type of the outgoing value.</param>
        /// <param name="expr">Expression to overwrite with the outgoing value.</param>
        /// <returns>Out-expression.</returns>
        public OutArgument Out(DataType dt, Expression expr)
        {
            return new OutArgument(dt, expr);
        }

        /// <summary>
        /// Generate a 16-bit pointer.
        /// </summary>
        /// <param name="ptr">Pointer value</param>
        /// <returns>A 16-bit <see cref="Address"/> instance.</returns>
        public Address Ptr16(ushort ptr)
        {
            return Address.Ptr16(ptr);
        }

        /// <summary>
        /// Generate a 32-bit pointer.
        /// </summary>
        /// <param name="ptr">Pointer value</param>
        /// <returns>A 32-bit <see cref="Address"/> instance.</returns>
        public Address Ptr32(uint ptr)
        {
            return Address.Ptr32(ptr);
        }

        /// <summary>
        /// Generate a 64-bit pointer.
        /// </summary>
        /// <param name="ptr">Pointer value</param>
        /// <returns>A 64-bit <see cref="Address"/> instance.</returns>
        public Address Ptr64(ulong ptr)
        {
            return Address.Ptr64(ptr);
        }

        /// <summary>
        /// Generate a signed integer division.
        /// </summary>
        /// <param name="a">Dividend.</param>
        /// <param name="b">Divisor.</param>
        /// <returns>A signed division expression.</returns>
        public BinaryExpression SDiv(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.SDiv, b.DataType, a, b);
        }

        /// <summary>
        /// Generate a signed integer division, returning a value of the 
        /// specified type <paramref name="dt"/>.
        /// </summary>
        /// <param name="dt">Data type of the quotient.</param>
        /// <param name="a">Dividend.</param>
        /// <param name="b">Divisor.</param>
        /// <returns>A signed division expression.</returns>
        public BinaryExpression SDiv(DataType dt, Expression a, Expression b)
        {
            return new BinaryExpression(Operator.SDiv, dt, a, b);
        }

        /// <summary>
        /// Generate a concatenated sequence of values. Use this to express
        /// values that are too long to fit in a machine register, or to model
        /// segmented pointers on architectures like the x86.
        /// </summary>
        /// <remarks>
        /// This method is used for the very common case of a two-element
        /// sequence, especially in contexts where x86-style segment:offset
        /// pairs exist.</remarks>
        /// <param name="head">Most significant part of value.</param>
        /// <param name="tail">Least significant part of value.</param>
        /// <returns>A value sequence.</returns>
        public MkSequence Seq(Expression head, Expression tail)
        {
            int totalBitSize = head.DataType.BitSize + tail.DataType.BitSize;
            Domain dom = (head.DataType == PrimitiveType.SegmentSelector)
                ? Domain.Pointer
                : ((PrimitiveType)head.DataType).Domain;
            return new MkSequence(PrimitiveType.Create(dom, totalBitSize), new[] { head, tail });
        }

        /// <summary>
        /// Generate an ordered sequence of values. Use this when expressing
        /// values that are too long to fit in a machine registers.
        /// </summary>
        /// <param name="exprs"></param>
        /// <returns>A sequence whose DataType is the weak "word" type of the 
        /// combined sizes of the expressions.</returns>
        public MkSequence Seq(params Expression [] exprs)
        {
            int totalBitSize = exprs.Sum(e => e.DataType.BitSize);
            var dt = PrimitiveType.CreateWord(totalBitSize);
            return new MkSequence(dt, exprs);
        }

        /// <summary>
        /// Generate an ordered sequence of values of type <paramref name="dtSeq"/>.
        /// Use this when expressing values that are too long to fit in a machine registers.
        /// </summary>
        /// <param name="dtSeq">The data type of the resulting sequence.</param>
        /// <param name="exprs">The constituent elements of the sequence.</param>
        /// <returns>A sequence whose DataType is the weak "word" type of the 
        /// combined sizes of the expressions.</returns>
        public MkSequence Seq(DataType dtSeq, params Expression [] exprs)
        {
            return new MkSequence(dtSeq, exprs);
        }

        /// <summary>
        /// Generate a segmented access to memory, using <paramref name="basePtr"/>
        /// as the base pointer and the <paramref name="offset"/> as the offset.
        /// </summary>
        /// <param name="mid">The memory identifier used in this access.</param>
        /// <param name="dt">Data type of the memory access.</param>
        /// <param name="basePtr">Base pointer or segment selector.</param>
        /// <param name="offset">Offset from base pointer.</param>
        /// <returns>A segmented memory access expression.</returns>
        public virtual MemoryAccess SegMem(Identifier mid, DataType dt, Expression basePtr, Expression offset)
        {
            int segptrBitsize = basePtr.DataType.BitSize + offset.DataType.BitSize;
            return new MemoryAccess(mid, 
                new SegmentedPointer(PrimitiveType.Create(Domain.SegPointer, segptrBitsize), basePtr, offset),
                dt);
        }

        /// <summary>
        /// Generate a segmented access to memory, using <paramref name="basePtr"/>
        /// as the base pointer and the <paramref name="offset"/> as the offset.
        /// </summary>
        /// <param name="dt">Data type of the memory access.</param>
        /// <param name="basePtr">Base pointer or segment selector.</param>
        /// <param name="offset">Offset from base pointer.</param>
        /// <returns>A segmented memory access expression.</returns>
        public virtual MemoryAccess SegMem(DataType dt, Expression basePtr, Expression offset)
        {
            return SegMem(MemoryStorage.GlobalMemory, dt, basePtr, offset);
        }

        /// <summary>
        /// Generate a segmented access to an 8-bit value in memory, using <paramref name="basePtr"/>
        /// as the base pointer and the <paramref name="offset"/> as the offset.
        /// </summary>
        /// <param name="basePtr">Base pointer or segment selector.</param>
        /// <param name="offset">Offset from base pointer.</param>
        /// <returns>A segmented memory access expression.</returns>
        public MemoryAccess SegMem8(Expression basePtr, Expression offset)
        {
            return SegMem(PrimitiveType.Byte, basePtr, offset);
        }

        /// <summary>
        /// Generate a segmented access to a 16-bit value in memory, using <paramref name="basePtr"/>
        /// as the base pointer and the <paramref name="offset"/> as the offset.
        /// </summary>
        /// <param name="basePtr">Base pointer or segment selector.</param>
        /// <param name="offset">Offset from base pointer.</param>
        /// <returns>A segmented memory access expression.</returns>
        public MemoryAccess SegMem16(Expression basePtr, Expression offset)
        {
            return SegMem(PrimitiveType.Word16, basePtr, offset);
        }

        /// <summary>
        /// Generates a <see cref="SegmentedPointer"/>.
        /// </summary>
        /// <param name="basePtr">Base or segment of the pointer.</param>
        /// <param name="offset">Offset of the pointer.</param>
        /// <returns>A segmented pointer expression.</returns>
        public SegmentedPointer SegPtr(Expression basePtr, Expression offset)
        {
            return SegmentedPointer.Create(basePtr, offset);
        }

        /// <summary>
        /// Generates a <see cref="SegmentedPointer"/>.
        /// </summary>
        /// <param name="dt">Data type of the resulting pointer.</param>
        /// <param name="basePtr">Base or segment of the pointer.</param>
        /// <param name="offset">Offset of the pointer.</param>
        /// <returns>A segmented pointer expression.</returns>
        public SegmentedPointer SegPtr(DataType dt, Expression basePtr, Expression offset)
        {
            return new SegmentedPointer(dt, basePtr, offset);
        }

        /// <summary>
        /// Generates an integer multiplication. No assumption about signedness is made.
        /// </summary>
        /// <param name="left">Multiplicand.</param>
        /// <param name="right">Multiplier.</param>
        /// <returns>An integer multiplication expression</returns>
        public Expression IMul(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.IMul, left.DataType, left, right);
        }

        /// <summary>
        /// Generates an integer multiplication where the product differs in size from
        /// the operands. No assumption about signedness is made.
        /// </summary>
        /// <param name="dtProduct">Resulting size of the product.</param>
        /// <param name="left">Multiplicand.</param>
        /// <param name="right">Multiplier.</param>
        /// <returns>An integer multiplication expression</returns>
        public Expression IMul(PrimitiveType dtProduct, Expression left, Expression right)
        {
            return new BinaryExpression(Operator.IMul, dtProduct, left, right);
        }

        /// <summary>
        /// Convenience method to generate an integer multiplication. The second parameter
        /// is converted to an integer constant. No assumption about signedness is made.
        /// </summary>
        /// <param name="left">Multiplicand.</param>
        /// <param name="c">Multiplier, which is first converted to a <see cref="Constant"/>.</param>
        /// <returns>An integer multiplication expression</returns>
        public Expression IMul(Expression left, int c)
        {
            return IMul(left, Constant.Create(left.DataType, c));
        }

        /// <summary>
        /// Generates a signed integer multiplication.
        /// </summary>
        /// <param name="left">Multiplicand.</param>
        /// <param name="right">Multiplier.</param>
        /// <returns>A signed integer multiplication expression</returns>
        public Expression SMul(Expression left, Expression right)
        {
            return new BinaryExpression(
                Operator.SMul, 
                PrimitiveType.Create(Domain.SignedInt, left.DataType.BitSize), 
                left, right);
        }

        /// <summary>
        /// Generates a signed integer multiplication where the product type
        /// differs from the type of the multiplicand and the multiplier.
        /// </summary>
        /// <param name="dtProduct">Data type of product.</param>
        /// <param name="left">Multiplicand.</param>
        /// <param name="right">Multiplier.</param>
        /// <returns>A signed integer multiplication expression.</returns>
        public Expression SMul(PrimitiveType dtProduct, Expression left, Expression right)
        {
            return new BinaryExpression(Operator.SMul, dtProduct, left, right);
        }

        /// <summary>
        /// Convenience method to generate a signed integer multiplication. The
        /// second parameter is converted to a signed integer constant.
        /// </summary>
        /// <param name="left">Multiplicand.</param>
        /// <param name="c">Multiplier, which is first converted to a signed <see cref="Constant"/>.</param>
        /// <returns>A signed integer multiplication expression</returns>
        public Expression SMul(Expression left, long c)
        {
            return new BinaryExpression(
                Operator.SMul, 
                PrimitiveType.Create(Domain.SignedInt, left.DataType.BitSize), 
                left, Constant.Create(left.DataType, c));
        }

        /// <summary>
        /// Generates an unsigned integer multiplication.
        /// </summary>
        /// <param name="left">Multiplicand.</param>
        /// <param name="right">Multiplier.</param>
        /// <returns>An unsigned integer multiplication expression</returns>

        public Expression UMul(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.UMul, left.DataType, left, right);
        }

        /// <summary>
        /// Generates an unsigned integer multiplication.
        /// </summary>
        /// <param name="dtProduct">Data type of product.</param>
        /// <param name="left">Multiplicand.</param>
        /// <param name="right">Multiplier.</param>
        /// <returns>An unsigned integer multiplication expression</returns>
        public Expression UMul(DataType dtProduct, Expression left, Expression right)
        {
            return new BinaryExpression(Operator.UMul, dtProduct, left, right);
        }

        /// <summary>
        /// Convenience method to generate an unsigned integer multiplication. The
        /// second parameter is converted to an unsigned integer constant.
        /// </summary>
        /// <param name="left">Multiplicand.</param>
        /// <param name="c">Multiplier, which is first converted to an unsigned <see cref="Constant"/>.</param>
        /// <returns>An unsigned integer multiplication expression</returns>
        public Expression UMul(Expression left, ulong c)
        {
            return new BinaryExpression(
                Operator.UMul, 
                PrimitiveType.Create(Domain.UnsignedInt, left.DataType.BitSize),
                left, Constant.Create(left.DataType, c));
        }

        /// <summary>
        /// Generates a logical not operation ('!exp' in the C language family).
        /// </summary>
        /// <param name="exp">Expression to logically negate.</param>
        /// <returns>Logical negation expression.</returns>
        public UnaryExpression Not(Expression exp)
        {
            return new UnaryExpression(Operator.Not, PrimitiveType.Bool, exp);
        }

        /// <summary>
        /// Generates a bitwise OR operation ('a | b' in the C language family).
        /// </summary>
        /// <returns>Bitwise disjunction expression.</returns>
        public BinaryExpression Or(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Or, a.DataType, a, b);
        }

        /// <summary>
        /// Convenience method to generate a bitwise OR operation ('a | b' in the C language family).
        /// The second parameter is converted to a Constant.
        /// </summary>
        /// <returns>Bitwise disjunction expression.</returns>
        public BinaryExpression Or(Expression a, ulong b)
        {
            return new BinaryExpression(Operator.Or, a.DataType, a, Constant.Create(a.DataType, b));
        }

        /// <summary>
        /// Generates a signed Shift arithmetic right expression ('a >> b' in the C language family). 
        /// </summary>
        /// <param name="e">Value to shift.</param>
        /// <param name="sh">Shift amount.</param>
        /// <returns>Arithmetic right shift expression.</returns>
        public BinaryExpression Sar(Expression e, Expression sh)
        {
            return new BinaryExpression(Operator.Sar, e.DataType, e, sh);
        }

        /// <summary>
        /// Convenience method to generate a signed Shift arithmetic right expression ('a >> b' in the C language family). 
        /// The shift amount is converted to a <see cref="Constant" />.
        /// </summary>
        /// <param name="e">Value to shift.</param>
        /// <param name="sh">Shift amount.</param>
        /// <returns>Arithmetic right shift expression.</returns>
        public BinaryExpression Sar(Expression e, byte sh)
        {
            return new BinaryExpression(Operator.Sar, e.DataType, e, Constant.Byte(sh));
        }

        /// <summary>
        /// Convenience method to generate a signed Shift arithmetic right
        /// expression ('a >> b' in the C language family). 
        /// The shift amount is converted to a <see cref="Constant" />.
        /// </summary>
        /// <param name="e">Value to shift.</param>
        /// <param name="sh">Shift amount.</param>
        /// <returns>Arithmetic right shift expression.</returns>
        public BinaryExpression Sar(Expression e, int sh)
        {
            return new BinaryExpression(Operator.Sar, e.DataType, e, Constant.Create(e.DataType, sh));
        }

        /// <summary>
        /// Generates a shift left expression ('a &lt;&lt; b' in the C language family). 
        /// </summary>
        /// <param name="e">Value to shift.</param>
        /// <param name="sh">Shift amount.</param>
        /// <returns>Left shift expression.</returns>

        public BinaryExpression Shl(Expression e, Expression sh)
        {
            return new BinaryExpression(Operator.Shl, e.DataType, e, sh);
        }

        /// <summary>
        /// Convenience method to generate a shift left expression ('a &lt;&lt; b' in the C language family). 
        /// The second argument is converted to a Constant.
        /// </summary>
        /// <param name="e">Value to shift.</param>
        /// <param name="sh">Shift amount.</param>
        /// <returns>Left shift expression.</returns>
        public BinaryExpression Shl(Expression e, int sh)
        {
            return new BinaryExpression(Operator.Shl, e.DataType, e, Constant.Byte((byte)sh));
        }

        /// <summary>
        /// Convenience method to generate a shift left expression ('a &lt;&lt; b' in the C language family). 
        /// The first argument is converted to a 32-bit constant.
        /// </summary>
        /// <param name="c">Value to shift.</param>
        /// <param name="sh">Shift amount.</param>
        /// <returns>Left shift expression.</returns>
        public BinaryExpression Shl(int c, Expression sh)
        {
            Constant cc = Constant.Word32(c);
            return new BinaryExpression(Operator.Shl, cc.DataType, cc, sh);
        }

        /// <summary>
        /// Generates an unsigned shift logical right expression ('a >> b' in the C language family). 
        /// </summary>
        /// <param name="exp">Value to shift.</param>
        /// <param name="sh">Shift amount.</param>
        /// <returns>Logical right shift expression.</returns>
        public BinaryExpression Shr(Expression exp, Expression sh)
        {
            return new BinaryExpression(Operator.Shr, exp.DataType, exp, sh);
        }

        /// <summary>
        /// Convenience method to generate an unsigned shift logical right
        /// expression ('a >> b' in the C language family). 
        /// </summary>
        /// <param name="exp">Value to shift.</param>
        /// <param name="sh">Shift amount.</param>
        /// <returns>Logical right shift expression.</returns>
        public BinaryExpression Shr(Expression exp, byte sh)
        {
            Constant cc = Constant.Byte(sh);
            return new BinaryExpression(Operator.Shr, exp.DataType, exp, cc);
        }

        /// <summary>
        /// Convenience method to generate an unsigned shift logical right
        /// expression ('a >> b' in the C language family). 
        /// </summary>
        /// <param name="e">Value to shift.</param>
        /// <param name="sh">Shift amount.</param>
        /// <returns>Logical right shift expression.</returns>
        public BinaryExpression Shr(Expression e, int sh)
        {
            return new BinaryExpression(Operator.Shr, e.DataType, e, Constant.Byte((byte) sh));
        }

        /// <summary>
        /// Generates an integer two's complement subtraction-with-borow expression.
        /// </summary>
        /// <param name="left">Augend</param>
        /// <param name="right">Addend</param>
        /// <param name="borrow">Carry flag</param>
        /// <returns>A binary expression for the subtraction-with-borrow.</returns>
        public BinaryExpression ISubB(Expression left, Expression right, Expression borrow)
        {
            return ISub(ISub(left, right), borrow);
        }

        /// <summary>
        /// Generates an integer two's complement subtraction expression. Signedness
        /// doesn't matter here.
        /// </summary>
        /// <param name="left">Minuend.</param>
        /// <param name="right">Subtrahend</param>
        /// <returns>An integer subtraction expression.</returns>
        public BinaryExpression ISub(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.ISub, left.DataType, left, right);
        }

        /// <summary>
        /// Convenience method to generate an integer subtraction expression. 
        /// The subtrahend is converted to a Constant of the same size as the augend.
        /// </summary>
        /// <param name="left">Minuend.</param>
        /// <param name="right">Subtrahend</param>
        /// <returns>An integer subtraction expression.</returns>
        public BinaryExpression ISub(Expression left, long right)
        {
            return ISub(left, Word(left.DataType.BitSize, right));
        }

        /// <summary>
        /// Generates an integer two's complement subtraction-with-carry expression.
        /// </summary>
        /// <param name="left">Augend</param>
        /// <param name="right">Addend</param>
        /// <param name="carry">Carry flag</param>
        /// <returns>A binary expression for the subtract-with-carry.</returns>
        public BinaryExpression ISubC(Expression left, Expression right, Expression carry)
        {
            return ISub(ISub(left, right), carry);
        }

        /// <summary>
        /// Convenience method to generate an integer subtraction expression. 
        /// The subtrahend is converted to a signed integer Constant of the same 
        /// size as the minuend.
        /// </summary>
        /// <param name="left">Minuend.</param>
        /// <param name="right">Subtrahend</param>
        /// <returns>An integer subtraction expression.</returns>
        public BinaryExpression ISubS(Expression left, long right)
        {
            return ISub(left, Constant.Int(left.DataType, right));
        }

        /// <summary>
        /// Generates a bit-slice of type <paramref name="dataType"/> of 
        /// an expression <paramref name="value"/>, starting at bit position 0.
        /// </summary>
        /// <param name="value">The value being sliced</param>
        /// <param name="dataType">The type of the bit slice</param>
        /// <returns>A bit-slice expression.</returns>
        public Slice Slice(Expression value, DataType dataType)
        {
            return new Slice(dataType, value, 0);
        }

        /// <summary>
        /// Generates a bit-slice of type <paramref name="dataType"/> of 
        /// an expression <paramref name="value"/>, starting at bit position
        /// <paramref name="bitOffset"/>.
        /// </summary>
        /// <param name="value">The value being sliced</param>
        /// <param name="dataType">The type of the bit slice</param>
        /// <param name="bitOffset">Slice offset from least significant bit.</param>
        /// <returns>A bit-slice expression.</returns>
        public Slice Slice(Expression value, DataType dataType, int bitOffset)
        {
            return new Slice(dataType, value, bitOffset);
        }

        /// <summary>
        /// Generates a bit-slice of bit length <paramref name="bitLength"/> of 
        /// an expression <paramref name="value"/>, starting at bit position
        /// <paramref name="bitOffset"/>.
        /// </summary>
        /// <param name="value">The expression to slice.</param>
        /// <param name="bitOffset">The bit offset at which the slice begins.</param>
        /// <param name="bitLength">The bit length of the slice.</param>
        /// <returns>A bit-slice expression.</returns>
        public Slice Slice(Expression value, int bitOffset, int bitLength)
        {
            var type = PrimitiveType.CreateBitSlice(bitLength);
            return new Slice(type, value, bitOffset);
        }

        /// <summary>
        /// Generates a Test expression which models the way processors
        /// use condition code flags to implement comparison results.
        /// </summary>
        /// <param name="cc">Condition code being implemented.</param>
        /// <param name="expr">Flag bits used to generate condition code.</param>
        /// <returns>A <see cref="TestCondition"/> instance.</returns>
        public TestCondition Test(ConditionCode cc, Expression expr)
        {
            return new TestCondition(cc, expr);
        }

        /// <summary>
        /// Convenience method that generates a <see cref="UnaryExpression"/> using
        /// the provided <see cref="UnaryOperator"/> and assuming the returned 
        /// data type is the same as the one of its operand.
        /// </summary>
        /// <param name="op">The unary operator of the resulting expression.</param>
        /// <param name="exp">The operand of the expression.</param>
        /// <returns>A unary expression.</returns>
        public UnaryExpression Unary(UnaryOperator op, Expression exp)
        {
            return new UnaryExpression(op, exp.DataType, exp);
        }

        /// <summary>
        /// Convenience method that generates a <see cref="UnaryExpression"/> using
        /// the provided <see cref="UnaryOperator"/> and assuming the returned 
        /// data type is the same as the one of its operand.
        /// </summary>
        /// <param name="op">The unary operator of the resulting expression.</param>
        /// <param name="dt">The data type of the result of the operation.</param>
        /// <param name="exp">The operand of the expression.</param>
        /// <returns>A unary expression.</returns>
        public UnaryExpression Unary(UnaryOperator op, DataType dt, Expression exp)
        {
            return new UnaryExpression(op, dt, exp);
        }

        /// <summary>
        /// Generates an unsigned integer subtraction.
        /// </summary>
        /// <param name="left">Minuend.</param>
        /// <param name="right">Subtrahend.</param>
        /// <returns>An unsigned integer subtraction expression.</returns>
        public BinaryExpression USub(Expression left, Expression right)
        {
            return new BinaryExpression(Operator.USub, left.DataType, left, right);
        }

        /// <summary>
        /// Generates an unsigned division expression.
        /// </summary>
        /// <param name="a">Dividend</param>
        /// <param name="b">Divisor</param>
        /// <returns>An unsigned division expression.</returns>
        public BinaryExpression UDiv(Expression a, Expression b)
        {
            return new BinaryExpression(
                Operator.UDiv, b.DataType, a, b);
        }

        /// <summary>
        /// Generates an unsigned division expression where the quotient
        /// has a different size than the dividend or divisor.
        /// /// </summary>
        /// <param name="dt">Quotient size</param>
        /// <param name="a">Dividend</param>
        /// <param name="b">Divisor</param>
        /// <returns>An unsigned division expression.</returns>
        public BinaryExpression UDiv(DataType dt, Expression a, Expression b)
        {
            return new BinaryExpression(Operator.UDiv, dt, a, b);
        }

        /// <summary>
        /// Generates an unsigned integer greater-or-equal comparison ('>=' in the C
        /// language family).
        /// </summary>
        /// <returns>An unsigned integer point inequality comparison.</returns>
        public BinaryExpression Uge(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Uge, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Convenience method to generate an unsigned integer greater-or-equal comparison ('>=' in the C
        /// language family). The second argument is converted to a Constant.
        /// </summary>
        /// <returns>An unsigned integer point inequality comparison.</returns>
        public BinaryExpression Uge(Expression a, int n)
        {
            return new BinaryExpression(Operator.Uge, PrimitiveType.Bool, a, Constant.Create(a.DataType, n));
        }

        /// <summary>
        /// Generates an unsigned integer greater-than comparison ('>' in the C
        /// language family).
        /// </summary>
        /// <returns>An unsigned integer point inequality comparison.</returns>
        public BinaryExpression Ugt(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ugt, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Generates an unsigned integer greater-than-0 comparison
        /// ('>= 0' in the C language family).
        /// </summary>
        /// <returns>An unsigned integer point inequality comparison.</returns>
        public BinaryExpression Ugt0(Expression exp)
        {
            return new BinaryExpression(Operator.Ugt, PrimitiveType.Bool, exp, Constant.Zero(exp.DataType));
        }

        /// <summary>
        /// Generates an unsigned integer less-or-equal comparison ('&lt;=' in the C
        /// language family).
        /// </summary>
        /// <returns>An unsigned integer point inequality comparison.</returns>
        public BinaryExpression Ule(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ule, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Convenience method to generate an unsigned integer less-than-or-
        /// equal comparison ('>=' in the C language family). The second
        /// argument is converted to a Constant.
        /// </summary>
        /// <returns>An unsigned integer point inequality comparison.</returns>
        public BinaryExpression Ule(Expression a, int n)
        {
            return new BinaryExpression(Operator.Ule, PrimitiveType.Bool, a, Constant.Create(a.DataType, n));
        }

        /// <summary>
        /// Generates an unsigned integer less-than comparison ('&lt;' in the C
        /// language family).
        /// </summary>
        /// <returns>An unsigned integer point inequality comparison.</returns>
        public Expression Ult(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ult, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Convenience method to generate an unsigned integer less-than comparison ('&lt;' in the C
        /// language family). The second argument is converted into a Constant.
        /// </summary>
        /// <returns>An unsigned integer point inequality comparison.</returns>
        public Expression Ult(Expression a, ulong b)
        {
            return Ult(a, Word(a.DataType.BitSize, b));
        }

        /// <summary>
        /// Generates an unsigned integer less-than-0 comparison
        /// ('&lt;= 0' in the C language family).
        /// </summary>
        /// <returns>An unsigned integer point inequality comparison.</returns>
        public BinaryExpression Ult0(Expression exp)
        {
            return new BinaryExpression(Operator.Ult, PrimitiveType.Bool, exp, Constant.Zero(exp.DataType));
        }

        /// <summary>
        /// Generates an 8-bit constant from a bit pattern.
        /// </summary>
        /// <param name="b">8 bits</param>
        /// <returns>8-bit constant</returns>
        public Constant Byte(byte b)
        {
            return Constant.Byte(b);
        }

        /// <summary>
        /// Generates an 16-bit constant from a bit pattern.
        /// </summary>
        /// <param name="n">16 bits</param>
        /// <returns>16-bit constant</returns>
        public Constant Word16(ushort n)
        {
            return Constant.Word16(n);
        }

        /// <summary>
        /// Generates an 16-bit constant from a bit pattern.
        /// </summary>
        /// <param name="n">Bits, of which the least significant 16 are used.</param>
        /// <returns>16-bit constant</returns>
        public Constant Word16(uint n)
        {
            return Constant.Word16((ushort) n);
        }

        /// <summary>
        /// Generates an 32-bit constant from a bit pattern.
        /// </summary>
        /// <param name="n">32 bits</param>
        /// <returns>32-bit constant</returns>
        public Constant Word32(int n)
        {
            return Constant.Word32(n);
        }

        /// <summary>
        /// Generates an 32-bit constant from a bit pattern.
        /// </summary>
        /// <param name="n">32 bits</param>
        /// <returns>32-bit constant</returns>
        public Constant Word32(uint n)
        {
            return Constant.Word32(n);
        }

        /// <summary>
        /// Generates an 64-bit constant from a bit pattern.
        /// </summary>
        /// <param name="n">64 bits</param>
        /// <returns>64-bit constant</returns>
        public Constant Word64(ulong n)
        {
            return Constant.Word64(n);
        }

        /// <summary>
        /// Generates an bit-vector of length <paramref name="bitSize" /> bits
        /// from the bit pattern <pararef name="n"/>.
        /// </summary>
        /// <param name="bitSize">
        /// Size, in bits, of the resulting <see cref="Constant"/>.
        /// </param>
        /// <param name="n">
        /// Value to encode as a <see cref="Constant"/>.
        /// </param>
        /// <returns>Bit vector constant</returns>
        public Constant Word(int bitSize, long n)
        {
            return Constant.Word(bitSize, n);
        }

        /// <summary>
        /// Generates an bit-vector of length <paramref name="bitSize"/> bits
        /// from the bit pattern <pararef name="n"/>.
        /// </summary>
        /// <param name="bitSize">Bitsize of the resulting constant.</param>
        /// <param name="n">Value to wrap in a <see cref="Constant"/> object.</param>
        /// <returns>Bit vector constant</returns>
        public Constant Word(int bitSize, ulong n)
        {
            return Constant.Word(bitSize, n);
        }

        /// <summary>
        /// Generates the bitwise exclusive OR of the two arguments.
        /// </summary>
        /// <returns>Bitwise XOR expression.</returns>
        public BinaryExpression Xor(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Xor, a.DataType, a, b);
        }

        /// <summary>
        /// Convenience method to generate the bitwise exclusive OR of the two arguments.
        /// The second argument is converted to a Constant.
        /// </summary>
        /// <returns>Bitwise XOR expression.</returns>
        public BinaryExpression Xor(Expression a, ulong b)
        {
            return new BinaryExpression(Operator.Xor, a.DataType, a, Constant.Create(a.DataType, b));
        }
    }
}
