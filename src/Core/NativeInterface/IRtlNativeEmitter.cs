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

using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface
{
    [NativeInterop]
    public enum HExpr { }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("412E748A-3181-4DBE-AD2A-AE23B5ECEAC2")]
    [ComVisible(true)]
    [ComImport]
    [NativeInterop]
    public interface INativeRtlEmitter
    {
        [PreserveSig] void Assign(HExpr dst, HExpr src);
        [PreserveSig] void Branch(HExpr exp, HExpr dst, InstrClass rtlClass);
        [PreserveSig] void BranchInMiddleOfInstruction(HExpr exp, HExpr dst, InstrClass rtlClass);
        [PreserveSig] void Call(HExpr dst, int bytesOnStack);
        [PreserveSig] void Goto(HExpr dst);
        [PreserveSig] void Invalid();
        [PreserveSig] void Nop();
        [PreserveSig] void Return(int x, int y);
        [PreserveSig] void FinishCluster(InstrClass rtlClass, ulong address, int mcLength);
        [PreserveSig] void SideEffect(HExpr exp);

        [PreserveSig] HExpr And(HExpr a, HExpr b);
        [PreserveSig] HExpr Cast(BaseType type, HExpr a);
        [PreserveSig] HExpr Comp(HExpr a);
        [PreserveSig] HExpr Cond(HExpr a);
        [PreserveSig] HExpr Dpb(HExpr dst, HExpr src, int pos);
        [PreserveSig] HExpr IAdc(HExpr a, HExpr b, HExpr c);
        [PreserveSig] HExpr IAdd(HExpr a, HExpr b);
        [PreserveSig] HExpr IMul(HExpr a, HExpr b);
        [PreserveSig] HExpr ISub(HExpr a, HExpr b);
        [PreserveSig] HExpr FAdd(HExpr a, HExpr b);
        [PreserveSig] HExpr FSub(HExpr a, HExpr b);
        [PreserveSig] HExpr FMul(HExpr a, HExpr b);
        [PreserveSig] HExpr FDiv(HExpr a, HExpr b);
        [PreserveSig] HExpr Mem(BaseType dt, HExpr ea);
        [PreserveSig] HExpr Mem8(HExpr ea);
        [PreserveSig] HExpr Mem16(HExpr ea);
        [PreserveSig] HExpr Mem32(HExpr ea);
        [PreserveSig] HExpr Mem64(HExpr ea);
        [PreserveSig] HExpr Not(HExpr a);
        [PreserveSig] HExpr Or(HExpr a, HExpr b);
        [PreserveSig] HExpr Ror(HExpr a, HExpr b);
        [PreserveSig] HExpr Rrc(HExpr a, HExpr b);
        [PreserveSig] HExpr Sar(HExpr a, HExpr b);
        [PreserveSig] HExpr SDiv(HExpr a, HExpr b);
        [PreserveSig] HExpr Shl(HExpr a, HExpr b);
        [PreserveSig] HExpr Shr(HExpr a, HExpr b);
        [PreserveSig] HExpr Slice(HExpr a, int pos, int bits);
        [PreserveSig] HExpr SMul(HExpr a, HExpr b);
        [PreserveSig] HExpr Test(Core.Expressions.ConditionCode cc, HExpr exp);
        [PreserveSig] HExpr UDiv(HExpr a, HExpr b);
        [PreserveSig] HExpr UMul(HExpr a, HExpr b);
        [PreserveSig] HExpr Xor(HExpr a, HExpr b);

        [PreserveSig] HExpr Eq0(HExpr e);
    	[PreserveSig] HExpr Ne0(HExpr e);

        [PreserveSig] HExpr Byte(byte b);
        [PreserveSig] HExpr Int16(short s);
        [PreserveSig] HExpr Int32(int i);
        [PreserveSig] HExpr Int64(long l);
        [PreserveSig] HExpr Ptr16(ushort p);
        [PreserveSig] HExpr Ptr32(uint p);
        [PreserveSig] HExpr Ptr64(ulong p);
        [PreserveSig] HExpr UInt16(ushort us);
        [PreserveSig] HExpr UInt32(uint u);
        [PreserveSig] HExpr UInt64(ulong ul);
        [PreserveSig] HExpr Word16(ushort us);
        [PreserveSig] HExpr Word32(uint u);
        [PreserveSig] HExpr Word64(ulong ul);

        // Add expressions to the "expression buffer".
        [PreserveSig] void AddArg(HExpr a);
        // Collect all expressions in the "expression buffer"
        // and create a function application.
    	[PreserveSig] HExpr Fn(HExpr fn);
        // Collect all expressions in the "expression buffer"
        // and create a sequence.
        [PreserveSig] HExpr Seq(HExpr dt);
    }
}
