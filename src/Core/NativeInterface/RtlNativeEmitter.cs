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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using System.Threading;
using Reko.Core.Types;

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
        private RtlEmitter m;
        private NativeTypeFactory ntf;
        private IRewriterHost host;
        private List<Expression> handles;
        private List<Expression> args;
        private Address address;
        private InstrClass rtlClass;
        private int instrLength;

        public NativeRtlEmitter(RtlEmitter m, NativeTypeFactory ntf, IRewriterHost host)
        {
            this.m = m;
            this.ntf = ntf;
            this.host = host;
            this.handles = new List<Expression>();
            this.args = new List<Expression>();
            this.address = null;
            this.rtlClass = InstrClass.Invalid;
            this.instrLength = 0;
        }

        //$TODO: arch-specific: IProcessorARchitecture?
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

        public RtlInstructionCluster ExtractCluster()
        {
            if (this.address == null || this.instrLength == 0)
                throw new InvalidOperationException();

            var rtlc = new RtlInstructionCluster(address, instrLength, m.Instructions.ToArray());
            rtlc.Class = this.rtlClass;

            address = null;
            instrLength = 0;

            return rtlc;
        }


        #region Factory methods

        public void Assign(HExpr dst, HExpr src)
        {
            m.Assign(GetExpression(dst), GetExpression(src));
        }

        public void Branch(HExpr exp, HExpr dst, InstrClass rtlClass)
        {
            m.Branch(GetExpression(exp), (Address)GetExpression(dst), rtlClass);
        }

        public void BranchInMiddleOfInstruction(HExpr exp, HExpr dst, InstrClass rtlClass)
        {
            m.BranchInMiddleOfInstruction(GetExpression(exp), (Address)GetExpression(dst), rtlClass);
        }

        public void Call(HExpr dst, int bytesOnStack)
        {
            m.Call(GetExpression(dst), (byte)bytesOnStack);
        }

        public void Goto(HExpr dst)
        {
            m.Goto(GetExpression(dst));
        }

        public void Invalid()
        {
            m.Invalid();
        }

        public void Nop()
        {
            m.Nop();
        }

        public void Return(int returnAddressBytes, int extraBytesPopped)
        {
            m.Return(returnAddressBytes, extraBytesPopped);
        }

        public void SideEffect(HExpr exp)
        {
            m.SideEffect(GetExpression(exp));
        }

        public void FinishCluster(InstrClass rtlClass, ulong address, int mcLength)
        {
            this.address = CreateAddress(address);
            this.rtlClass = rtlClass;
            this.instrLength = mcLength;
            this.handles.Clear();
        }

        public HExpr And(HExpr a, HExpr b)
        {
            return MapToHandle(m.And(GetExpression(a), GetExpression(b)));
        }

        public HExpr Cast(BaseType type, HExpr a)
        {
            return MapToHandle(m.Cast(Interop.DataTypes[type], GetExpression(a)));
        }

        public HExpr Comp(HExpr a)
        {
            return MapToHandle(m.Comp(GetExpression(a)));
        }

        public HExpr Cond(HExpr a)
        {
            return MapToHandle(m.Cond(GetExpression(a)));
        }

        public HExpr Dpb(HExpr dst, HExpr src, int pos)
        {
            return MapToHandle(m.Dpb(GetExpression(dst), GetExpression(src), pos));
        }

        public HExpr IAdc(HExpr a, HExpr b, HExpr c)
        {
            throw new NotImplementedException();
        }

        public HExpr IAdd(HExpr a, HExpr b)
        {
            return MapToHandle(m.IAdd(GetExpression(a), GetExpression(b)));
        }

        public HExpr IMul(HExpr a, HExpr b)
        {
            return MapToHandle(m.IMul(GetExpression(a), GetExpression(b)));
        }

        public HExpr ISub(HExpr a, HExpr b)
        {
            return MapToHandle(m.ISub(GetExpression(a), GetExpression(b)));
        }

        public HExpr FAdd(HExpr a, HExpr b)
        {
            return MapToHandle(m.FAdd(GetExpression(a), GetExpression(b)));
        }

        public HExpr FSub(HExpr a, HExpr b)
        {
            return MapToHandle(m.FSub(GetExpression(a), GetExpression(b)));
        }

        public HExpr FMul(HExpr a, HExpr b)
        {
            return MapToHandle(m.FMul(GetExpression(a), GetExpression(b)));
        }

        public HExpr FDiv(HExpr a, HExpr b)
        {
            return MapToHandle(m.FDiv(GetExpression(a), GetExpression(b)));
        }


        public HExpr Mem(BaseType dt, HExpr ea)
        {
            return MapToHandle(m.Mem(Interop.DataTypes[dt], GetExpression(ea)));
        }

        public HExpr Mem8(HExpr ea)
        {
            return MapToHandle(m.Mem8(GetExpression(ea)));
        }

        public HExpr Mem16(HExpr ea)
        {
            return MapToHandle(m.Mem16(GetExpression(ea)));
        }

        public HExpr Mem32(HExpr ea)
        {
            return MapToHandle(m.Mem32(GetExpression(ea)));
        }

        public HExpr Mem64(HExpr ea)
        {
            return MapToHandle(m.Mem64(GetExpression(ea)));
        }

        public HExpr Not(HExpr a)
        {
            return MapToHandle(m.Not(GetExpression(a)));
        }

        public HExpr Or(HExpr a, HExpr b)
        {
            return MapToHandle(m.Or(GetExpression(a), GetExpression(b)));
        }

        public HExpr Ror(HExpr a, HExpr b)
        {
            var aa = GetExpression(a);
            var bb = GetExpression(b);
            return MapToHandle(host.PseudoProcedure(PseudoProcedure.Ror, PrimitiveType.Word32, aa, bb));
        }

        public HExpr Rrc(HExpr a, HExpr b)
        {
            var aa = GetExpression(a);
            var bb = GetExpression(b);
            return MapToHandle(host.PseudoProcedure(PseudoProcedure.Ror, PrimitiveType.Word32, aa, bb));
        }

        public HExpr Sar(HExpr a, HExpr b)
        {
            return MapToHandle(m.Sar(GetExpression(a), GetExpression(b)));
        }

        public HExpr SDiv(HExpr a, HExpr b)
        {
            return MapToHandle(m.SDiv(GetExpression(a), GetExpression(b)));
        }

        public HExpr Shl(HExpr a, HExpr b)
        {
            return MapToHandle(m.Shl(GetExpression(a), GetExpression(b)));
        }

        public HExpr Shr(HExpr a, HExpr b)
        {
            return MapToHandle(m.Shr(GetExpression(a), GetExpression(b)));
        }

        public HExpr Slice(HExpr a, int pos, int bits)
        {
            return MapToHandle(m.Slice(GetExpression(a), pos, bits));
        }

        public HExpr SMul(HExpr a, HExpr b)
        {
            return MapToHandle(m.SMul(GetExpression(a), GetExpression(b)));
        }

        public HExpr Test(ConditionCode cc, HExpr exp)
        {
            return MapToHandle(m.Test(cc, GetExpression(exp)));
        }

        public HExpr UDiv(HExpr a, HExpr b)
        {
            return MapToHandle(m.UDiv(GetExpression(a), GetExpression(b)));
        }

        public HExpr UMul(HExpr a, HExpr b)
        {
            return MapToHandle(m.UMul(GetExpression(a), GetExpression(b)));
        }

        public HExpr Xor(HExpr a, HExpr b)
        {
            return MapToHandle(m.Xor(GetExpression(a), GetExpression(b)));
        }


        public HExpr Eq0(HExpr e)
        {
            return MapToHandle(m.Eq0(GetExpression(e)));
        }
        public HExpr Ne0(HExpr e)
        {
            return MapToHandle(m.Ne0(GetExpression(e)));
        }


        public HExpr Byte(byte b)
        {
            return MapToHandle(Constant.Byte(b));
        }

        public HExpr Int16(short s)
        {
            return MapToHandle(Constant.Int16(s));
        }

        public HExpr Int32(int i)
        {
            return MapToHandle(Constant.Int32(i));
        }

        public HExpr Int64(long l)
        {
            return MapToHandle(Constant.Int64(l));
        }

        public HExpr Ptr16(ushort p)
        {
            return MapToHandle(Address.Ptr16(p));
        }

        public HExpr Ptr32(uint p)
        {
            return MapToHandle(Address.Ptr32(p));
        }

        public HExpr Ptr64(ulong p)
        {
            return MapToHandle(Address.Ptr64(p));
        }

        public HExpr UInt16(ushort us)
        {
            return MapToHandle(Constant.UInt16(us));
        }

        public HExpr UInt32(uint u)
        {
            return MapToHandle(Constant.UInt32(u));
        }

        public HExpr UInt64(ulong ul)
        {
            return MapToHandle(Constant.UInt64(ul));
        }

        public HExpr Word16(ushort us)
        {
            return MapToHandle(Constant.Word16(us));
        }

        public HExpr Word32(uint u)
        {
            return MapToHandle(Constant.Word32(u));
        }

        public HExpr Word64(ulong ul)
        {
            return MapToHandle(Constant.Word64(ul));
        }

        public void AddArg(HExpr a)
        {
            this.args.Add(GetExpression(a));
        }

        /// <summary>
        /// Pops all the args and makes an Application instance.
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public HExpr Fn(HExpr fn)
        {
            var appl = m.Fn(GetExpression(fn), this.args.ToArray());
            this.args.Clear();
            return MapToHandle(appl);
        }

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
