#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Intrinsics;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// This class exposes the INativeRtlEmitter interface so that native code
    /// can call the factory methods to build sequences of RTL instructions 
    /// that are the result of translating a machine code instruction.
    /// </summary>
    /// <remarks>
    /// Once the instructions have been added, the (native) caller calls 
    /// FinishCluster() and returns control to the .NET side. Finally, the 
    /// completed cluster is extracted with ExtractCluster
    /// </remarks>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class NativeRtlEmitter : INativeRtlEmitter
    {
        private readonly RtlEmitter m;
        private readonly NativeTypeFactory ntf;
        private readonly IRewriterHost host;
        private readonly List<Expression> handles;
        private readonly List<Expression> args;
        private Address address;
        private InstrClass rtlClass;
        private int instrLength;

        /// <summary>
        /// Creates an instance of the <see cref="NativeRtlEmitter"/> class.
        /// </summary>
        /// <param name="m">Emitter to write RTL instructions to.</param>
        /// <param name="ntf">Native type factory.</param>
        /// <param name="host"><see cref="IRewriterHost"/> to use when lifting instructions.
        /// </param>
        public NativeRtlEmitter(RtlEmitter m, NativeTypeFactory ntf, IRewriterHost host)
        {
            this.m = m;
            this.ntf = ntf;
            this.host = host;
            this.handles = new List<Expression>();
            this.args = new List<Expression>();
            this.address = default;
            this.rtlClass = InstrClass.Invalid;
            this.instrLength = 0;
        }

        //$TODO: arch-specific: IProcessorARchitecture?

        /// <summary>
        /// Creates an address from a linear address.
        /// </summary>
        /// <param name="linear">Value of the address.</param>
        /// <returns><see cref="Address"/> instance.</returns>
        public virtual Address CreateAddress(ulong linear)
        {
            return Address.Ptr32((uint)linear);
        }

        /// <summary>
        /// Retrieves the expression of a particular handle.
        /// </summary>
        /// <param name="hExp"></param>
        /// <returns></returns>
        public Expression GetExpression(HExpr hExp)
        {
            return handles[(int)hExp - 4711];
        }

        /// <summary>
        /// Creates a handle for the given expression.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public HExpr MapToHandle(Expression exp)
        {
            var hExp = (HExpr)(handles.Count + 4711);
            handles.Add(exp);
            return hExp;
        }

        /// <summary>
        /// Extracts the RTL instruction cluster that was built by the native code.
        /// </summary>
        /// <returns>RTL instruction cluster.</returns>
        public RtlInstructionCluster ExtractCluster()
        {
            if (this.instrLength == 0)
                throw new InvalidOperationException();

            var cluster = m.MakeCluster(address, instrLength, this.rtlClass);

            address = default;
            instrLength = 0;
            return cluster;
        }

        #region Factory methods

        /// <inheritdoc/>
        public void Assign(HExpr dst, HExpr src)
        {
            m.Assign(GetExpression(dst), GetExpression(src));
        }

        /// <inheritdoc/>
        public void Branch(HExpr exp, HExpr dst, InstrClass rtlClass)
        {
            m.Branch(GetExpression(exp), (Address)GetExpression(dst), rtlClass);
        }

        /// <inheritdoc/>
        public void BranchInMiddleOfInstruction(HExpr exp, HExpr dst, InstrClass rtlClass)
        {
            m.BranchInMiddleOfInstruction(GetExpression(exp), (Address)GetExpression(dst), rtlClass);
        }

        /// <inheritdoc/>
        public void Call(HExpr dst, int bytesOnStack)
        {
            m.Call(GetExpression(dst), (byte)bytesOnStack);
        }

        /// <inheritdoc/>
        public void Goto(HExpr dst)
        {
            m.Goto(GetExpression(dst));
        }

        /// <inheritdoc/>
        public void Invalid()
        {
            m.Invalid();
        }

        /// <inheritdoc/>
        public void Nop()
        {
            m.Nop();
        }

        /// <inheritdoc/>
        public void Return(int returnAddressBytes, int extraBytesPopped)
        {
            m.Return(returnAddressBytes, extraBytesPopped);
        }

        /// <inheritdoc/>
        public void SideEffect(HExpr exp)
        {
            m.SideEffect(GetExpression(exp));
        }

        /// <inheritdoc/>
        public void FinishCluster(InstrClass rtlClass, ulong address, int mcLength)
        {
            this.address = CreateAddress(address);
            this.rtlClass = rtlClass;
            this.instrLength = mcLength;
            this.handles.Clear();
        }

        /// <inheritdoc/>
        public HExpr And(HExpr a, HExpr b)
        {
            return MapToHandle(m.And(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr Cast(BaseType type, HExpr a)
        {
            var src = GetExpression(a);
            return MapToHandle(m.Convert(src, src.DataType, Interop.DataTypes[type]));
        }

        /// <inheritdoc/>
        public HExpr Comp(HExpr a)
        {
            return MapToHandle(m.Comp(GetExpression(a)));
        }

        /// <inheritdoc/>
        public HExpr Cond(HExpr a)
        {
            return MapToHandle(m.Cond(PrimitiveType.Byte, GetExpression(a)));
        }

        /// <inheritdoc/>
        public HExpr Dpb(HExpr dst, HExpr src, int pos)
        {
            var eDst = GetExpression(dst);
            return MapToHandle(m.Dpb((Identifier)eDst, GetExpression(src), pos));
        }

        /// <inheritdoc/>
        public HExpr IAdc(HExpr a, HExpr b, HExpr c)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public HExpr IAdd(HExpr a, HExpr b)
        {
            return MapToHandle(m.IAdd(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr IMul(HExpr a, HExpr b)
        {
            return MapToHandle(m.IMul(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr ISub(HExpr a, HExpr b)
        {
            return MapToHandle(m.ISub(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr FAdd(HExpr a, HExpr b)
        {
            return MapToHandle(m.FAdd(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr FSub(HExpr a, HExpr b)
        {
            return MapToHandle(m.FSub(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr FMul(HExpr a, HExpr b)
        {
            return MapToHandle(m.FMul(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr FDiv(HExpr a, HExpr b)
        {
            return MapToHandle(m.FDiv(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr Mem(BaseType dt, HExpr ea)
        {
            return MapToHandle(m.Mem(Interop.DataTypes[dt], GetExpression(ea)));
        }

        /// <inheritdoc/>
        public HExpr Mem8(HExpr ea)
        {
            return MapToHandle(m.Mem8(GetExpression(ea)));
        }

        /// <inheritdoc/>
        public HExpr Mem16(HExpr ea)
        {
            return MapToHandle(m.Mem16(GetExpression(ea)));
        }

        /// <inheritdoc/>
        public HExpr Mem32(HExpr ea)
        {
            return MapToHandle(m.Mem32(GetExpression(ea)));
        }

        /// <inheritdoc/>
        public HExpr Mem64(HExpr ea)
        {
            return MapToHandle(m.Mem64(GetExpression(ea)));
        }

        /// <inheritdoc/>
        public HExpr Not(HExpr a)
        {
            return MapToHandle(m.Not(GetExpression(a)));
        }

        /// <inheritdoc/>
        public HExpr Or(HExpr a, HExpr b)
        {
            return MapToHandle(m.Or(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr Ror(HExpr a, HExpr b)
        {
            var aa = GetExpression(a);
            var bb = GetExpression(b);
            var ror = CommonOps.Ror.MakeInstance(aa.DataType, bb.DataType);
            return MapToHandle(m.Fn(ror, aa, bb));
        }

        /// <inheritdoc/>
        public HExpr Rrc(HExpr a, HExpr b, HExpr c)
        {
            var aa = GetExpression(a);
            var bb = GetExpression(b);
            var cc = GetExpression(c);
            var rorc = CommonOps.RorC.MakeInstance(aa.DataType, bb.DataType, cc.DataType);
            return MapToHandle(m.Fn(CommonOps.RorC, aa, bb, cc));
        }

        /// <inheritdoc/>
        public HExpr Sar(HExpr a, HExpr b)
        {
            return MapToHandle(m.Sar(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr SDiv(HExpr a, HExpr b)
        {
            return MapToHandle(m.SDiv(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr Shl(HExpr a, HExpr b)
        {
            return MapToHandle(m.Shl(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr Shr(HExpr a, HExpr b)
        {
            return MapToHandle(m.Shr(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr Slice(HExpr a, int pos, int bits)
        {
            return MapToHandle(m.Slice(GetExpression(a), pos, bits));
        }

        /// <inheritdoc/>
        public HExpr SMul(HExpr a, HExpr b)
        {
            return MapToHandle(m.SMul(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr Test(ConditionCode cc, HExpr exp)
        {
            return MapToHandle(m.Test(cc, GetExpression(exp)));
        }

        /// <inheritdoc/>
        public HExpr UDiv(HExpr a, HExpr b)
        {
            return MapToHandle(m.UDiv(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr UMul(HExpr a, HExpr b)
        {
            return MapToHandle(m.UMul(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr Xor(HExpr a, HExpr b)
        {
            return MapToHandle(m.Xor(GetExpression(a), GetExpression(b)));
        }

        /// <inheritdoc/>
        public HExpr Eq0(HExpr e)
        {
            return MapToHandle(m.Eq0(GetExpression(e)));
        }

        /// <inheritdoc/>
        public HExpr Ne0(HExpr e)
        {
            return MapToHandle(m.Ne0(GetExpression(e)));
        }


        /// <inheritdoc/>
        public HExpr Byte(byte b)
        {
            return MapToHandle(Constant.Byte(b));
        }

        /// <inheritdoc/>
        public HExpr Int16(short s)
        {
            return MapToHandle(Constant.Int16(s));
        }

        /// <inheritdoc/>
        public HExpr Int32(int i)
        {
            return MapToHandle(Constant.Int32(i));
        }

        /// <inheritdoc/>
        public HExpr Int64(long l)
        {
            return MapToHandle(Constant.Int64(l));
        }

        /// <inheritdoc/>
        public HExpr Ptr16(ushort p)
        {
            return MapToHandle(Address.Ptr16(p));
        }

        /// <inheritdoc/>
        public HExpr Ptr32(uint p)
        {
            return MapToHandle(Address.Ptr32(p));
        }

        /// <inheritdoc/>
        public HExpr Ptr64(ulong p)
        {
            return MapToHandle(Address.Ptr64(p));
        }

        /// <inheritdoc/>
        public HExpr UInt16(ushort us)
        {
            return MapToHandle(Constant.UInt16(us));
        }

        /// <inheritdoc/>
        public HExpr UInt32(uint u)
        {
            return MapToHandle(Constant.UInt32(u));
        }

        /// <inheritdoc/>
        public HExpr UInt64(ulong ul)
        {
            return MapToHandle(Constant.UInt64(ul));
        }

        /// <inheritdoc/>
        public HExpr Word16(ushort us)
        {
            return MapToHandle(Constant.Word16(us));
        }

        /// <inheritdoc/>
        public HExpr Word32(uint u)
        {
            return MapToHandle(Constant.Word32(u));
        }

        /// <inheritdoc/>
        public HExpr Word64(ulong ul)
        {
            return MapToHandle(Constant.Word64(ul));
        }

        /// <inheritdoc/>
        public void AddArg(HExpr a)
        {
            this.args.Add(GetExpression(a));
        }

        /// <summary>
        /// Pops all the args and makes an Application instance.
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        /// <inheritdoc/>
        public HExpr Fn(HExpr fn)
        {
            var appl = m.Fn(GetExpression(fn), this.args.ToArray());
            this.args.Clear();
            return MapToHandle(appl);
        }

        /// <inheritdoc/>
        public HExpr Seq(HExpr dt)
        {
            var dataType = ntf.GetRekoType(dt);
            var seq = m.Seq(dataType, this.args.ToArray());
            this.args.Clear();
            return MapToHandle(seq);
        }

        #endregion
    }
}
