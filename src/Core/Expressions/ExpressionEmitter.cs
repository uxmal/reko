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

using Reko.Core.Code;
using Reko.Core.Operators;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public BinaryExpression IAdd(Expression left, int right)
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
        public BinaryExpression IAddS(Expression left, int right)
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
        public Expression AddSubSignedInt(Expression e, int c)
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
        /// <param name="e">L-value</param>
        /// <param name="ptType">Type of the resulting pointer.</param>
        /// <returns>A unary expresssion representing the address-of operation.</returns>
        public UnaryExpression AddrOf(DataType ptType, Expression e)
        {
            return new UnaryExpression(UnaryOperator.AddrOf, ptType, e);
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
        /// Short-circuiting 'and' ('&&' in C family of languages)
        /// </summary>
        /// <param name="a">Antecedent expression.</param>
        /// <param name="b">Subsequent expression.</param>
        /// <returns>Short-circuiting 'and' expression.</returns>
        public Expression Cand(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Cand, PrimitiveType.Bool, a, b);
        }

        /// <summary>
        /// Generates a cast expression which coerces the <paramref name="expr"/> to
        /// the data type <paramref name="dataType"/>.
        /// </summary>
        /// <param name="dataType">Type to coerce to.</param>
        /// <param name="expr">Value to coerce.</param>
        /// <returns>A cast expression.</returns>
        public Cast Cast(DataType dataType, Expression expr)
        {
            return new Cast(dataType, expr);
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
        /// Generates a simple Dereference operation ('*a' in the C language
        /// family). If you don't know the exact data type of <paramref name="a"/>,
        /// use one of the `Mem...` methods instead.
        /// </summary>
        /// <param name="a"></param>
        /// <returns>A C-stype dereference.</returns>
        public Dereference Deref(Expression a)
        {
            return new Dereference(a.DataType, a);
        }

        /// <summary>
        /// Generate a deposit-bits expression, which models the act
        /// of updating a subsequence of bits inside of a wider register.
        /// </summary>
        /// <param name="dst">The wider register.</param>
        /// <param name="src">The smaller value to deposit.</param>
        /// <param name="offset">The bit offset at which to deposit the value.</param>
        /// <returns>A deposit-bits expression.</returns>
        public DepositBits Dpb(Expression dst, Expression src, int offset)
        {
            return new DepositBits(dst, src, offset);
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
        /// is converted to a Constant first.
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
        /// <param name="a">Expression to compare with zero.</param>
        /// <returns>Comparison expression.</returns>
        public BinaryExpression Eq0(Expression exp)
        {
            return new BinaryExpression(Operator.Eq, PrimitiveType.Bool, exp, Constant.Create(exp.DataType, 0));
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

        // Field access: "point.x"

        /// <summary>
        /// Generate a field access ('foo.bar' in the C language family
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
        /// the arguments <paramref name="args" /> and returns a value of type 
        /// <param name="retType"/>.
        /// </summary>
        /// <param name="fn">The function to apply.</param>
        /// <param name="args">The arguments of the function.</param>
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
            var retType = ep.Signature.ReturnValue != null
                ? ep.Signature.ReturnValue.DataType
                : VoidType.Instance;
            return new Application(
                new ProcedureConstant(PrimitiveType.Ptr32, ep), 
                retType,
                args);
        }

        /// <summary>
        /// Generates a function application that applies the intrinsic 
        /// procedure <paramref name="ep"/> to
        /// the arguments <paramref name="args" /> and returns a value of the
        /// return type of the external procedure. Use this when modelling
        /// processor-specific intrinsic functions that cannot be expressed
        /// in RTL any other way.
        /// </summary>
        /// <param name="ep">The instrinsic function to apply.</param>
        /// <param name="args">The arguments of the function.</param>
        /// <returns>A function application</returns>
        public Application Fn(PseudoProcedure ppp, params Expression[] args)
        {
            return new Application(new ProcedureConstant(PrimitiveType.Ptr32, ppp), ppp.ReturnType, args);
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
        /// to a Constant.
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
        /// to a Constant.
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
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, dt);
        }

        /// <summary>
        /// Generates a memory access of the data of type <paramref name="dt"/> at the specified effective address
        /// <paramref name="ea"/> in the address space identified by <paramref name="mid" />.
        /// </summary>
        /// <param name="ea">The address of the memory being accessed.</param>
        /// <returns>A memory access expression.</returns>
        public virtual MemoryAccess Mem(
            MemoryIdentifier mid,
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
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, PrimitiveType.Byte);
        }

        /// <summary>
        /// Generates a memory access of the 16-bit word at the specified effective address
        /// <paramref name="ea"/>.
        /// </summary>
        /// <param name="ea">The address of the memory being accessed.</param>
        /// <returns>A memory access expression.</returns>
        public virtual MemoryAccess Mem16(Expression ea)
        {
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, PrimitiveType.Word16);
        }

        /// <summary>
        /// Generates a memory access of the 32-bit word at the specified effective address
        /// <paramref name="ea"/>.
        /// </summary>
        /// <param name="ea">The address of the memory being accessed.</param>
        /// <returns>A memory access expression.</returns>
        public virtual MemoryAccess Mem32(Expression ea)
        {
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, PrimitiveType.Word32);
        }

        /// <summary>
        /// Generates a memory access of the 64-bit word at the specified effective address
        /// <paramref name="ea"/>.
        /// </summary>
        /// <param name="ea">The address of the memory being accessed.</param>
        /// <returns>A memory access expression.</returns>
        public MemoryAccess Mem64(Expression ea)
        {
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, ea, PrimitiveType.Word64);
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
        /// to a Constant.
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
        public MemberPointerSelector MembPtrW(Expression ptr, Expression membPtr)
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
        public BinaryExpression Ne(Expression a, int n)
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
        /// Generate the integer remainder ('%' in the C language family).
        /// </summary>
        /// <param name="opLeft">Dividend.</param>
        /// <param name="opRight">Divisor.</param>
        /// <returns>A modulus expression.</returns>
        public Expression Remainder(Expression a, Expression b)
        {
            return new BinaryExpression(
                Operator.IMod,
                b.DataType,
                a, b);
        }

        /// <summary>
        /// Generate a signed integer division.
        /// </summary>
        /// <param name="a">Dividend.</param>
        /// <param name="b">Divisor.</param>
        /// <returns>A signed division expression.</returns>
        public BinaryExpression SDiv(Expression a, Expression b)
        {
            return new BinaryExpression(
                Operator.SDiv, b.DataType, a, b);
        }

        /// <summary>
        /// Generate a concatenated sequence of values. Use this to express
        /// values that are too long to fit in a machine register, or to model
        /// segmented pointers on architectures like the x86.
        /// </summary>
        /// <remarks>
        /// This method is ised for the very common case of a two-element
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
        /// <param name="exprs"></param>
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
        /// <param name="dt">Data type of the memory access.</param>
        /// <param name="basePtr">Base pointer or segment selector.</param>
        /// <param name="offset">Offset from base pointer.</param>
        /// <returns>A segmented memory access expression.</returns>
        public virtual SegmentedAccess SegMem(DataType dt, Expression basePtr, Expression offset)
        {
            return new SegmentedAccess(MemoryIdentifier.GlobalMemory, basePtr, offset, dt);
        }

        /// <summary>
        /// Generate a segmented access to an 8-bit value in memory, using <paramref name="basePtr"/>
        /// as the base pointer and the <paramref name="offset"/> as the offset.
        /// </summary>
        /// <param name="basePtr">Base pointer or segment selector.</param>
        /// <param name="offset">Offset from base pointer.</param>
        /// <returns>A segmented memory access expression.</returns>
        public SegmentedAccess SegMem8(Expression basePtr, Expression ptr)
        {
            return SegMem(PrimitiveType.Byte, basePtr, ptr);
        }

        /// <summary>
        /// Generate a segmented access to a 16-bit value in memory, using <paramref name="basePtr"/>
        /// as the base pointer and the <paramref name="offset"/> as the offset.
        /// </summary>
        /// <param name="basePtr">Base pointer or segment selector.</param>
        /// <param name="offset">Offset from base pointer.</param>
        /// <returns>A segmented memory access expression.</returns>
        public SegmentedAccess SegMem16(Expression basePtr, Expression ptr)
        {
            return SegMem(PrimitiveType.Word16, basePtr, ptr);
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
        /// Convenience method to generate an integer multiplication. The second parameter
        /// is converted to an integer constant. No assumption about signedness is made.
        /// </summary>
        /// <param name="left">Multiplicand.</param>
        /// <param name="right">Multiplier.</param>
        /// <returns>An integer multiplication expression</returns>
        public Expression IMul(Expression left, int c)
        {
            return new BinaryExpression(Operator.IMul, left.DataType, left, Constant.Create(left.DataType, c));
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
        /// Convenience method to generate a signed integer multiplication. The
        /// second parameter is converted to a signed integer constant.
        /// </summary>
        /// <param name="left">Multiplicand.</param>
        /// <param name="right">Multiplier.</param>
        /// <returns>A signed integer multiplication expression</returns>
        public Expression SMul(Expression left, int c)
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
        /// Convenience method to generate an unsigned integer multiplication. The
        /// second parameter is converted to an unsigned integer constant.
        /// </summary>
        /// <param name="left">Multiplicand.</param>
        /// <param name="right">Multiplier.</param>
        /// <returns>An unsigned integer multiplication expression</returns>
        public Expression UMul(Expression left, int c)
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
        public BinaryExpression Or(Expression a, int b)
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
        /// The shift amount is converted to a Constant.
        /// </summary>
        /// <param name="e">Value to shift.</param>
        /// <param name="sh">Shift amount.</param>
        /// <returns>Arithmetic right shift expression.</returns>
        public BinaryExpression Sar(Expression e, byte sh)
        {
            return new BinaryExpression(Operator.Sar, e.DataType, e, Constant.Byte(sh));
        }

        /// <summary>
        /// Convenience method to generate a signed Shift arithmetic right expression ('a >> b' in the C language family). 
        /// The shift amount is converted to a Constant.
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
        /// <param name="e">Value to shift.</param>
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
        /// <param name="e">Value to shift.</param>
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
        /// <param name="e">Value to shift.</param>
        /// <param name="sh">Shift amount.</param>
        /// <returns>Logical right shift expression.</returns>
        public BinaryExpression Shr(Expression exp, byte c)
        {
            Constant cc = Constant.Byte(c);
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
        public BinaryExpression ISub(Expression left, int right)
        {
            return ISub(left, Word(left.DataType.BitSize, right));
        }

        /// <summary>
        /// Convenience method to generate an integer subtraction expression. 
        /// The subtrahend is converted to a signed integer Constant of the same 
        /// size as the minuend.
        /// </summary>
        /// <param name="left">Minuend.</param>
        /// <param name="right">Subtrahend</param>
        /// <returns>An integer subtraction expression.</returns>
        public BinaryExpression ISubS(Expression left, int right)
        {
            return ISub(left, Constant.Int(left.DataType, right));
        }

        /// <summary>
        /// Generates a bit-slice of type <paramref name="dataType"/> of 
        /// an expression <paramref name="value"/>, starting at bit position
        /// <paramref name="bitOffset"/>.
        /// </summary>
        /// <param name="primitiveType">The type of the bit slice</param>
        /// <param name="value">The value being sliced</param>
        /// <param name="bitOffset">Slice offset from least significant bit.</param>
        /// <returns>A bit-slice expression.</returns>
        public Slice Slice(DataType dataType, Expression value, int bitOffset)
        {
            return new Slice(dataType, value, bitOffset);
        }

        /// <summary>
        /// Generates a bit-slice of type <paramref name="primitiveType"/> of 
        /// an expression <paramref name="value"/>, starting at bit position
        /// <paramref name="bitOffset"/>.
        /// </summary>
        /// <param name="primitiveType"></param>
        /// <param name="value"></param>
        /// <param name="bitOffset"></param>
        /// <returns>A bit-slice expression.</returns>
        public Slice Slice(Expression value, int bitOffset, int bitlength)
        {
            var type = PrimitiveType.CreateBitSlice(bitlength);
            return new Slice(type, value, bitOffset);
        }

        /// <summary>
        /// Generates a Test expression which models the way processors
        /// use condition code flags to implement comparison results.
        /// </summary>
        /// <param name="cc">Condition code being implemented.</param>
        /// <param name="expr">Flag bits used to generate condition code.</param>
        /// <returns></returns>
        public TestCondition Test(ConditionCode cc, Expression expr)
        {
            return new TestCondition(cc, expr);
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
        /// Generates an unsigned integer less-or-equal comparison ('&lt;=' in the C
        /// language family).
        /// </summary>
        /// <returns>An unsigned integer point inequality comparison.</returns>
        public BinaryExpression Ule(Expression a, Expression b)
        {
            return new BinaryExpression(Operator.Ule, PrimitiveType.Bool, a, b);
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
        public Expression Ult(Expression a, int b)
        {
            return Ult(a, Word(a.DataType.BitSize, b));
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
        /// Generates an bit-vector of length <paramref name="bitSize"> bits
        /// from the bit patter <pararef name="n"/>.
        /// </summary>
        /// <param name="b">32 bits</param>
        /// <returns>Bit vector constant</returns>
        public Constant Word(int bitSize, long n)
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
        public BinaryExpression Xor(Expression a, int b)
        {
            return new BinaryExpression(Operator.Xor, a.DataType, a, Constant.Create(a.DataType, b));
        }
    }
}
