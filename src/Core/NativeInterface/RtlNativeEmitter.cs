#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// This class exposes the IRtlNativeEmitter interface so that native code
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
    public class RtlNativeEmitter : IRtlNativeEmitter
    {
        private RtlEmitter m;
        private List<Expression> handles;
        private Address address;
        private RtlClass rtlClass;
        private int instrLength;

        public RtlNativeEmitter(RtlEmitter m)
        {
            this.m = m;
            this.handles = new List<Expression>();
            this.address = null;
            this.rtlClass = RtlClass.Invalid;
            this.instrLength = 0;
        }

        //$TODO: arch-specific: IProcessorARchitecture?
        public virtual Address CreateAddress(ulong linear)
        {
            return Address.Ptr32((uint)linear);
        }

        /// <summary>
        /// Retrieves the expression ofa particular handle.
        /// </summary>
        /// <param name="hExp"></param>
        /// <returns></returns>
        public Expression GetExpression(HExpr hExp)
        {
            return handles[(int)hExp];
        }

        /// <summary>
        /// Creates a handle for the given expression.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public HExpr MapToHandle(Expression exp)
        {
            var hExp = (HExpr)handles.Count;
            handles.Add(exp);
            return hExp;
        }

        public RtlInstructionCluster ExtractCluster()
        {
            if (this.address == null || this.instrLength == 0)
                throw new InvalidOperationException();

            var rtlc = new RtlInstructionCluster(address, instrLength, m.Instructions);
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

        public void Branch(HExpr exp, HExpr dst, RtlClass rtlClass)
        {
            throw new NotImplementedException();
        }

        public void BranchInMiddleOfInstruction(HExpr exp, HExpr dst, RtlClass rtlClass)
        {
            throw new NotImplementedException();
        }

        public void Call(HExpr dst, int bytesOnStack)
        {
            throw new NotImplementedException();
        }

        public void Goto(HExpr dst)
        {
            throw new NotImplementedException();
        }

        public void Invalid()
        {
            throw new NotImplementedException();
        }

        public void Nop()
        {
            m.Nop();
        }

        public void Return(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void SideEffect(HExpr exp)
        {
            throw new NotImplementedException();
        }

        public void FinishCluster(RtlClass rtlClass, ulong address, int mcLength)
        {
            this.address = CreateAddress(address);
            this.rtlClass = rtlClass;
            this.instrLength = mcLength;
            this.handles.Clear();
        }

        public HExpr And(HExpr a, int n)
        {
            throw new NotImplementedException();
        }

        public HExpr And(HExpr a, HExpr b)
        {
            throw new NotImplementedException();
        }

        public HExpr Cast(BaseType type, HExpr a)
        {
            throw new NotImplementedException();
        }

        public HExpr Comp(HExpr a)
        {
            throw new NotImplementedException();
        }

        public HExpr Cond(HExpr a)
        {
            throw new NotImplementedException();
        }

        public HExpr Dpb(HExpr dst, HExpr src, int pos)
        {
            throw new NotImplementedException();
        }

        public HExpr IAdc(HExpr a, HExpr b, HExpr c)
        {
            throw new NotImplementedException();
        }

        public HExpr IAdd(HExpr a, int n)
        {
            return MapToHandle(m.IAdd(GetExpression(a), n));
        }

        public HExpr IAdd(HExpr a, HExpr b)
        {
            return MapToHandle(m.IAdd(GetExpression(a), GetExpression(b)));
        }

        public HExpr IMul(HExpr a, int n)
        {
            throw new NotImplementedException();
        }

        public HExpr IMul(HExpr a, HExpr b)
        {
            throw new NotImplementedException();
        }

        public HExpr ISub(HExpr a, int n)
        {
            throw new NotImplementedException();
        }

        public HExpr ISub(HExpr a, HExpr b)
        {
            throw new NotImplementedException();
        }

        public HExpr Mem(BaseType dt, HExpr ea)
        {
            throw new NotImplementedException();
        }

        public HExpr Mem8(HExpr ea)
        {
            throw new NotImplementedException();
        }

        public HExpr Mem16(HExpr ea)
        {
            return MapToHandle(m.LoadW(GetExpression(ea)));
        }

        public HExpr Mem32(HExpr ea)
        {
            return MapToHandle(m.LoadDw(GetExpression(ea)));
        }

        public HExpr Mem64(HExpr ea)
        {
            throw new NotImplementedException();
        }

        public HExpr Not(HExpr a)
        {
            throw new NotImplementedException();
        }

        public HExpr Or(HExpr a, HExpr b)
        {
            throw new NotImplementedException();
        }

        public HExpr Ror(HExpr a, int n)
        {
            throw new NotImplementedException();
        }

        public HExpr Ror(HExpr a, HExpr b)
        {
            throw new NotImplementedException();
        }

        public HExpr Rrc(HExpr a, int n)
        {
            throw new NotImplementedException();
        }

        public HExpr Rrc(HExpr a, HExpr b)
        {
            throw new NotImplementedException();
        }

        public HExpr Sar(HExpr a, int n)
        {
            throw new NotImplementedException();
        }

        public HExpr Sar(HExpr a, HExpr b)
        {
            throw new NotImplementedException();
        }
 
        public HExpr Shl(HExpr a, int n)
        {
            throw new NotImplementedException();
        }

        public HExpr Shl(HExpr a, HExpr b)
        {
            throw new NotImplementedException();
        }

        public HExpr Shr(HExpr a, int n)
        {
            throw new NotImplementedException();
        }

        public HExpr Shr(HExpr a, HExpr b)
        {
            throw new NotImplementedException();
        }

        public HExpr Slice(HExpr a, int pos, int bits)
        {
            throw new NotImplementedException();
        }

        public HExpr SMul(HExpr a, HExpr b)
        {
            throw new NotImplementedException();
        }

        public HExpr Test(ConditionCode cc, HExpr exp)
        {
            throw new NotImplementedException();
        }

        public HExpr UMul(HExpr a, HExpr b)
        {
            throw new NotImplementedException();
        }

        public HExpr Xor(HExpr a, HExpr b)
        {
            throw new NotImplementedException();
        }







        public HExpr Byte(byte b)
        {
            throw new NotImplementedException();
        }

        public HExpr Int16(short s)
        {
            throw new NotImplementedException();
        }

        public HExpr Int32(int i)
        {
            return MapToHandle(Constant.Int32(i));
        }

        public HExpr Int64(long l)
        {
            throw new NotImplementedException();
        }

        public HExpr Ptr16(ushort p)
        {
            throw new NotImplementedException();
        }

        public HExpr Ptr32(uint p)
        {
            return MapToHandle(Address.Ptr32(p));
        }

        public HExpr Ptr64(ulong p)
        {
            throw new NotImplementedException();
        }

        public HExpr UInt16(ushort us)
        {
            return MapToHandle(Constant.UInt16(us));
        }

        public HExpr UInt32(uint u)
        {
            throw new NotImplementedException();
        }

        public HExpr UInt64(ulong ul)
        {
            throw new NotImplementedException();
        }

        public HExpr Word16(ushort us)
        {
            throw new NotImplementedException();
        }

        public HExpr Word32(uint u)
        {
            throw new NotImplementedException();
        }

        public HExpr Word64(ulong ul)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
