#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Environments.Ps3
{
    /// <summary>
    /// Platform support for Sony Playstation 3 (PS3).
    /// </summary>
    /// <remarks>
    /// A particularity of this platform is that even though the PowerPC 
    /// processor is a 64-bit CPU, all pointers are by convention 32-bit.
    /// This means that special care has to be taken to make sure no
    /// 64-bit pointers or addresses are introduced into the 
    /// decompilation process.
    /// </remarks>
    public class Ps3Platform : Platform
    {
        public Ps3Platform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "ps3") 
        {
            this.StructureMemberAlignment = 4;
            //$TODO: find out what registers are always trashed
        }

        public override string DefaultCallingConvention => "";

        public override PrimitiveType PointerType { get { return PrimitiveType.Ptr32; } }

        public override CallingConvention GetCallingConvention(string? ccName)
        {
            throw new NotImplementedException();
        }

        public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
        {
            throw new NotImplementedException();
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.WChar_t: return 16;  //$REVIEW: Does PS/3 support wchar_t?
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 32;
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble: return 64;
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> rdr, IRewriterHost host)
        {
            var dasm = rdr.Take(8).ToArray();
            if (dasm.Length < 8)
                return null;
             
            //ImmediateOperand immOp;
            //MemoryOperand memOp;

            throw new NotImplementedException();
            /*
            //addi r12,r0,0000
            instr = dasm[0].Instructions[0];
            if (instr.Mnemonic != Mnemonic.addi)
                return null;

            //oris r12,r12,0006
            instr = dasm.DisassembleInstruction();
            if (instr.Mnemonic != Mnemonic.oris)
                return null;
            immOp = (ImmediateOperand) instr.op3;
            uint aFuncDesc = immOp.Value.ToUInt32() << 16;

            //lwz r12,nnnn(r12)
            instr = dasm.DisassembleInstruction();
            if (instr.Mnemonic != Mnemonic.lwz)
                return null;
            memOp = (MemoryOperand)instr.op2;
            int offset = memOp.Offset.ToInt32();
            aFuncDesc = (uint)(aFuncDesc + offset);

            //std r2,40(r1)
            instr = dasm.DisassembleInstruction();
            if (instr.Mnemonic != Mnemonic.std)
                return null;

            //lwz r0,0(r12)
            // Have a pointer to a trampoline
            instr = dasm.DisassembleInstruction();
            if (instr.Mnemonic != Mnemonic.lwz)
                return null;

            //lwz r2,4(r12)
            instr = dasm.DisassembleInstruction();
            if (instr.Mnemonic != Mnemonic.lwz)
                return null;

            // mtctr r0
            instr = dasm.DisassembleInstruction();
            if (instr.Mnemonic != Mnemonic.mtctr)
                return null;

            // bcctr 14,00
            instr = dasm.DisassembleInstruction();
            if (instr.Mnemonic != Mnemonic.bcctr)
                return null;

            // Read the function pointer from the function descriptor.

            offset = (int)aFuncDesc - (int)rdr.Address.ToUInt32();
            rdr.Offset = rdr.Offset + offset;
            var aFn = rdr.ReadUInt32();
            return null;
            */
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            // Bizarrely, pointers are 32-bit on this 64-bit platform.
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr32(uAddr);
        }

        public override Address MakeAddressFromLinear(ulong uAddr, bool codeAlign)
        {
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr32((uint)uAddr);
        }

        public override bool TryParseAddress(string? sAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(sAddress, out addr);
        }
    }
}
