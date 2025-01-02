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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Arch.i8051
{
    public class i8051Architecture : ProcessorArchitecture
    {
        public i8051Architecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            this.CarryFlag = Registers.CFlag;
            this.Endianness = EndianServices.Big;
            this.StackRegister = Registers.SP;
            this.WordWidth = PrimitiveType.Byte;
            this.PointerType = PrimitiveType.Ptr16;
            this.FramePointerType = PrimitiveType.Byte; // tiny stack pointer!
            this.InstructionBitSize = 8;

            this.DataMemory = new RegisterStorage("__data", 300, 0, PrimitiveType.SegmentSelector); 
        }

        /// <summary>
        /// This pseudo-register is not architectural, but is required to represent
        /// data memory accesses, which don't occupy the same address space as code
        /// memory accesses.
        /// </summary>
        public RegisterStorage DataMemory { get; }

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader rdr)
        {
            return new i8051Disassembler(this, rdr);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new i8051State(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new i8051Rewriter(this, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            var grfStg = new FlagGroupStorage(Registers.PSW, grf, GrfToString(flagRegister, "", grf));
            return grfStg;
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetMnemonicNumber(string name)
        {
            throw new NotImplementedException();
        }

        public override RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
        {
            return Registers.GetRegister(domain - StorageDomain.Register);
        }

        public override RegisterStorage[] GetRegisters()
        {
            return Registers.GetRegisters();
        }

        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            uint grf = flags.FlagGroupBits;
            if ((grf & Registers.CFlag.FlagGroupBits) != 0) yield return Registers.CFlag;
            if ((grf & Registers.AFlag.FlagGroupBits) != 0) yield return Registers.AFlag;
            if ((grf & Registers.OFlag.FlagGroupBits) != 0) yield return Registers.OFlag;
            if ((grf & Registers.PFlag.FlagGroupBits) != 0) yield return Registers.PFlag;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            var sb = new StringBuilder();
            if ((grf & (uint)FlagM.C) != 0)
                sb.Append("C");
            if ((grf & (uint)FlagM.AC) != 0)
                sb.Append("A");
            if ((grf & (uint)FlagM.OV) != 0)
                sb.Append("O");
            if ((grf & (uint)FlagM.P) != 0)
                sb.Append("P");
            return sb.ToString();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            return Address.Ptr16(c.ToUInt16());
        }

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            if (!rdr.TryReadBeUInt16(out ushort uAddr))
                return null;
            return Address.Ptr16(uAddr);
        }

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            return Registers.TryGetRegister(name, out reg);
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse16(txtAddr, out addr);
        }

        /*
         * The Keil C compiler emits a call to the following subroutine if 
         * a switch statement is sparse.
         */
        private readonly static byte[] sparseSwitchSubroutine = new byte[]
        {
            0xD0, 0x83, 0xD0, 0x82,  0xF8, 0xE4, 0x93, 0x70,
            0x12, 0x74, 0x01, 0x93,  0x70, 0x0D, 0xA3, 0xA3,
            0x93, 0xF8, 0x74, 0x01,  0x93, 0xF5, 0x82, 0x88,
            0x83, 0xE4, 0x73, 0x74,  0x02, 0x93, 0x68, 0x60,
            0xEF, 0xA3, 0xA3, 0xA3,  0x80, 0xDF
        };

        public override void PostprocessProgram(Program program)
        {
            // This feels a little hacky, but we will allow it for now.
            // Scan image to find Keil subroutine for sparse switch 
            // statements.

            var sparseSwitchePatches = MakeSparseSwitchPatches(program);
            foreach (var sparseSwitchPatch in sparseSwitchePatches)
            {
                program.User.Patches.Add(sparseSwitchPatch.Address, sparseSwitchPatch);
            }
        }

        /// <summary>
        /// Given a <paramref name="program" /> returns a list of locations
        /// where patches need to be applied.
        /// </summary>
        /// <remarks>
        /// Scans through all loaded segments, looking for a LCALL instruction
        /// to the Keil sparse switch subroutine.
        /// </remarks>
        /// <param name="program"></param>
        /// <returns></returns>
        private IEnumerable<CodePatch> MakeSparseSwitchPatches(Program program)
        {
            const byte LCall_Opcode = 0x12;

            foreach (var seg in program.SegmentMap.Segments.Values)
            {
                var rdr = seg.MemoryArea.CreateBeReader(0);
                while (rdr.TryReadByte(out byte b))
                {
                    if (b != LCall_Opcode)
                        continue;
                    var addrCall = rdr.Address - 1;
                    if (!rdr.TryReadBeUInt16(out ushort uAddrSwitchSubroutine))
                        break;
                    var addrSwitchSubroutine = Address.Ptr16(uAddrSwitchSubroutine);
                    if (!program.SegmentMap.TryFindSegment(addrSwitchSubroutine, out var segment))
                        continue;
                    var bmem = (ByteMemoryArea) segment.MemoryArea;
                    var offset = (int) (addrSwitchSubroutine - bmem.BaseAddress);
                    if (!ByteMemoryArea.CompareArrays(bmem.Bytes, offset, sparseSwitchSubroutine, sparseSwitchSubroutine.Length))
                        continue;

                    // We found the sparse switch subroutine. Now we parse the sparse switch
                    // data.

                    var cluster = MakeCluster(addrCall, rdr);
                    yield return new CodePatch(cluster);
                }
            }
        }

        /// <summary>
        /// Given an image reader positioned at a Keil sparse switch table, generates
        /// an <see cref="RtlInstructionCluster" /> that implements the table as a 
        /// sequence of RTL if-instructions.
        /// </summary>
        private RtlInstructionCluster MakeCluster(Address addrPatch, ImageReader rdr)
        {
            var binder = new StorageBinder();
            var areg = binder.EnsureRegister(Registers.A);
            var m = new RtlEmitter(new List<RtlInstruction>());
            while (rdr.TryReadBeUInt16(out ushort uAddrDst))
            {
                if (uAddrDst == 0)
                {
                    if (!rdr.TryReadBeUInt16(out uAddrDst))
                        break;
                    m.Goto(Address.Ptr16(uAddrDst));
                    break;
                }
                if (!rdr.TryReadByte(out byte bValue))
                    break;
                var addrDst = Address.Ptr16(uAddrDst);
                m.BranchInMiddleOfInstruction(m.Eq(areg, bValue), addrDst, InstrClass.ConditionalTransfer);
            }
            int length = (int) (rdr.Address - addrPatch);
            var cluster = m.MakeCluster(addrPatch, length, InstrClass.Transfer);
            return cluster;
        }
    }
}
