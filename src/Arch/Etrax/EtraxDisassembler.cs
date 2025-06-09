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

#pragma warning disable IDE1006

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Etrax
{
    using Decoder = Decoder<EtraxDisassembler, Mnemonic, EtraxInstruction>;

    public class EtraxDisassembler : DisassemblerBase<EtraxInstruction, Mnemonic>
    {
        private static readonly Decoder rootDecoder;
        private static readonly PrimitiveType?[] dataWidths;

        private readonly EtraxArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private PrimitiveType? dataWidth;
        private MachineOperand? prefix;
        private uint swapBits;

        public EtraxDisassembler(EtraxArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
        }

        public override EtraxInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            this.ops.Clear();
            this.dataWidth = null;
            this.prefix = null;
            this.swapBits = 0;
            if (!rdr.TryReadLeUInt16(out ushort uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = addr;
            instr.Length = (int) (rdr.Address - addr);
            return instr;
        }

        public override EtraxInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            var instr = new EtraxInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray(),
                DataWidth = dataWidth,
                SwapBits = swapBits,
            };
            return instr;
        }

        public override EtraxInstruction CreateInvalidInstruction()
        {
            return new EtraxInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.Invalid,
            };
        }

        #region Mutators

        private static readonly Bitfield low5 = new Bitfield(0, 5);
        private static readonly Bitfield low6 = new Bitfield(0, 6);
        private static readonly Bitfield[] bitBccOffset = Bf((0, 1), (1, 7));

        /// <summary>
        /// Register.
        /// </summary>
        private static Mutator<EtraxDisassembler> Reg(int bitpos)
        {
            var regField = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var ireg = regField.Read(u);
                var reg = Registers.GpRegisters[ireg];
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<EtraxDisassembler> R0 = Reg(0);
        private static readonly Mutator<EtraxDisassembler> R12 = Reg(12);

        /// <summary>
        /// Scaled source register.
        /// </summary>
        private static bool ScR(uint uInstr, EtraxDisassembler dasm)
        {
            var ireg = (int) Bits.ZeroExtend(uInstr, 4);
            var scale = dataWidths[(uInstr >> 4) & 3];
            if (scale is null)
                return false;
            var reg = Registers.GpRegisters[ireg];
            dasm.ops.Add(new ScaledRegisterOperand(reg, scale));
            return true;
        }

        /// <summary>
        /// Special register encoded at bit offset 0.
        /// </summary>
        private static bool P0(uint uInstr, EtraxDisassembler dasm)
        {
            var ireg = (int) Bits.ZeroExtend(uInstr, 4);
            var reg = Registers.GpRegisters[ireg];
            dasm.ops.Add(reg);
            return true;
        }

        /// <summary>
        /// Special register encoded at bit offset 12.
        /// </summary>
        private static bool P12(uint uInstr, EtraxDisassembler dasm)
        {
            var ireg = (int) Bits.ZeroExtend(uInstr >> 12, 4);
            var reg = Registers.SystemRegisters[ireg];
            dasm.ops.Add(reg);
            return true;
        }

        /// <summary>
        /// Destination register, except PC.
        /// </summary>
        private static bool NpcRd0(uint uInstr, EtraxDisassembler dasm)
        {
            var ireg = (int) Bits.ZeroExtend(uInstr, 4);
            if (ireg == 0xF)    // PC is not valid
                return false;
            var reg = Registers.GpRegisters[ireg];
            dasm.ops.Add(reg);
            return true;
        }

        /// <summary>
        /// Destination register, except PC.
        /// </summary>
        private static bool NpcRd(uint uInstr, EtraxDisassembler dasm)
        {
            var ireg = (int) Bits.ZeroExtend(uInstr >> 12, 4);
            if (ireg == 0xF)    // PC is not valid
                return false;
            var reg = Registers.GpRegisters[ireg];
            dasm.ops.Add(reg);
            return true;
        }

        /// <summary>
        /// Unsigned immediate (6 bits)
        /// </summary>
        private static bool j(uint uInstr, EtraxDisassembler dasm)
        {
            var n = low6.Read(uInstr);
            var imm = Constant.Word32(n);
            dasm.ops.Add(imm);
            return true;
        }

        /// <summary>
        /// Signed immediate(6 bits)
        /// </summary>
        private static bool i(uint uInstr, EtraxDisassembler dasm)
        {
            var n = low6.ReadSigned(uInstr);
            var imm = Constant.Word32(n);
            dasm.ops.Add(imm);
            return true;
        }

        /// <summary>
        /// Shift amount (5 bits)
        /// </summary>
        private static bool c(uint uInstr, EtraxDisassembler dasm)
        {
            var n = (int)low5.Read(uInstr);
            var imm = Constant.Int32(n);
            dasm.ops.Add(imm);
            return true;
        }

        /// <summary>
        /// Size modifier (one bit).
        /// </summary>
        private static bool z(uint uInstr, EtraxDisassembler dasm)
        {
            dasm.dataWidth = dataWidths[(uInstr >> 4) & 1];
            return true;
        }

        /// <summary>
        /// Size modifier (two bits).
        /// </summary>
        private static bool zz(uint uInstr, EtraxDisassembler dasm)
        {
            dasm.dataWidth = dataWidths[(uInstr >> 4) & 3];
            return true;
        }

        /// <summary>
        /// Indirect operator.
        /// </summary>
        private static Mutator<EtraxDisassembler> Memory(bool pcRelative)
        {
            return (uint uInstr, EtraxDisassembler dasm) =>
            {
                if (dasm.prefix is not null)
                {
                    dasm.ops.Add(dasm.prefix);
                    return true;
                }
                else
                {
                    var iReg = uInstr & 0xF;
                    bool postInc = Bits.IsBitSet(uInstr, 10);
                    MachineOperand? op = null;
                    if (postInc && iReg == 0xF)
                    {
                        op = ReadImmediate(pcRelative, dasm);
                    }
                    else
                    {
                        var mem = new MemoryOperand(dasm.dataWidth ?? PrimitiveType.Word32);
                        var reg = Registers.GpRegisters[iReg];
                        mem.Base = reg;
                        mem.PostIncrement = postInc;
                        op = mem;
                    }
                    if (op is null)
                        return false;
                    dasm.ops.Add(op);
                    return true;
                }
            };
        }

        private static MachineOperand? ReadImmediate(bool pcRelative, EtraxDisassembler dasm)
        {
            // Immediate mode.
            var dt = dasm.dataWidth ?? PrimitiveType.Word32;
            var addrImm = dasm.rdr.Address;
            if (!dasm.rdr.TryReadLe(dt, out var imm))
                return null;
            if (dt.BitSize == 8)
                dasm.rdr.Seek(1);
            if (pcRelative)
            {
                addrImm += imm.ToInt32();
                return addrImm;
            }
            else
            {
                return imm;
            }
        }

        private static readonly Mutator<EtraxDisassembler> Mem = Memory(false);
        private static readonly Mutator<EtraxDisassembler> MemJ = Memory(true);
        
        private static bool Jb(uint uInstr, EtraxDisassembler dasm)
        {
            var offset = Bitfield.ReadSignedFields(bitBccOffset, uInstr) << 1;
            var dst = dasm.rdr.Address + offset;
            dasm.ops.Add(dst);
            return true;
        }

        private static bool Jw(uint uInstr, EtraxDisassembler dasm)
        {
            if (!dasm.rdr.TryReadLeInt16(out short offset))
                return false;
            var dst = dasm.rdr.Address + offset;
            dasm.ops.Add(dst);
            return true;
        }

        private static Mutator<EtraxDisassembler> AllFlags(uint mask)
        {
            return (u, d) =>
            {
                u &= mask;
                var uFlag = ((u >> 8) & 0xF0u) | (u & 0xF);
                d.ops.Add(d.arch.GetFlagGroup(Registers.dccr, uFlag)!);
                return true;
            };
        }

        private static bool SwapBits(uint uInstr, EtraxDisassembler dasm)
        {
            dasm.swapBits = uInstr >> 12;
            return dasm.swapBits != 0;
        }

        private static bool skip4(uint uInstr, EtraxDisassembler dasm)
        {
            return dasm.rdr.TryReadUInt32(out uint _);
        }

        #endregion

        public override EtraxInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("EtraxDis", this.addr, rdr, message);
            var instr = CreateInvalidInstruction();
            instr.Mnemonic = Mnemonic.nyi;
            return instr;
        }

        private static InstrDecoder<EtraxDisassembler, Mnemonic, EtraxInstruction> Instr(Mnemonic mnemonic, params Mutator<EtraxDisassembler> [] mutators)
        {
            return new InstrDecoder<EtraxDisassembler, Mnemonic, EtraxInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static InstrDecoder<EtraxDisassembler, Mnemonic, EtraxInstruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<EtraxDisassembler>[] mutators)
        {
            return new InstrDecoder<EtraxDisassembler, Mnemonic, EtraxInstruction>(iclass, mnemonic, mutators);
        }

        private static Decoder<EtraxDisassembler, Mnemonic, EtraxInstruction> SizedInstr(Decoder variableSize, Decoder fixedSize)
        {
            return Mask(4, 2,
                variableSize,
                variableSize,
                variableSize,
                fixedSize);
        }

        private static NyiDecoder<EtraxDisassembler,Mnemonic,EtraxInstruction> Nyi(string message)
        {
            return new NyiDecoder<EtraxDisassembler, Mnemonic, EtraxInstruction>(message);
        }

        private static PrefixDecoder Prefix(Mutator<EtraxDisassembler> mutator)
        {
            return new PrefixDecoder(mutator);
        }

        private class PrefixDecoder : Decoder
        {
            private readonly Mutator<EtraxDisassembler> mutator;

            public PrefixDecoder(Mutator<EtraxDisassembler> mutator)
            {
                this.mutator = mutator;
            }

            public override EtraxInstruction Decode(uint wInstr, EtraxDisassembler dasm)
            {
                if (mutator(wInstr, dasm) &&
                    dasm.rdr.TryReadUInt16(out ushort uNext))
                {
                    return rootDecoder.Decode(uNext, dasm);
                }
                else
                {
                    return dasm.CreateInvalidInstruction();
                }
            }
        }

        static EtraxDisassembler()
        {
            dataWidths = new[]
            {
                PrimitiveType.Byte,
                PrimitiveType.Word16,
                PrimitiveType.Word32,
                null
            };

            var reserved = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            var bcc = Mask(12, 4, "  Bcc",
                Instr(Mnemonic.bcc, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.bcs, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.beq, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.bvc, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.bvs, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.bpl, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.bmi, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.bls, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.bhi, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.bge, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.blt, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.bgt, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.ble, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.ba,  InstrClass.Transfer | InstrClass.Delay, Jb),
                Instr(Mnemonic.bwf, InstrClass.ConditionalTransfer | InstrClass.Delay, Jb));

            var bccW = Mask(12, 4, "  BccW",
                Instr(Mnemonic.bcc, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.bcs, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.bne, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.beq, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.bvc, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.bvs, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.bpl, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.bmi, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.bls, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.bhi, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.bge, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.blt, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.bgt, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.ble, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.ba, InstrClass.Transfer | InstrClass.Delay, Jw),
                Instr(Mnemonic.bwf, InstrClass.ConditionalTransfer | InstrClass.Delay, Jw));


            var scc = Mask(12, 4, "  Scc",
                Instr(Mnemonic.scc, NpcRd0),
                Instr(Mnemonic.scs, NpcRd0),
                Instr(Mnemonic.sne, NpcRd0),
                Instr(Mnemonic.seq, NpcRd0),
                Instr(Mnemonic.svc, NpcRd0),
                Instr(Mnemonic.svs, NpcRd0),
                Instr(Mnemonic.spl, NpcRd0),
                Instr(Mnemonic.smi, NpcRd0),
                Instr(Mnemonic.sls, NpcRd0),
                Instr(Mnemonic.shi, NpcRd0),
                Instr(Mnemonic.sge, NpcRd0),
                Instr(Mnemonic.slt, NpcRd0),
                Instr(Mnemonic.sgt, NpcRd0),
                Instr(Mnemonic.sle, NpcRd0),
                Instr(Mnemonic.sa,  NpcRd0),
                Instr(Mnemonic.swf, NpcRd0));


            Nyi("bcc - Offset(7 bits) s.");

            var dip = Prefix((u, d) =>
            {
                // Double indirect.
                var iReg = u & 0xF;
                var postInc = Bits.IsBitSet(u, 10);
                MachineOperand? inner;
                if (postInc && iReg == 0xF)
                {
                    inner = ReadImmediate(false, d);
                    if (inner is null)
                        return false;
                }
                else
                {
                    var baseReg = Registers.GpRegisters[u & 0xF];
                    inner = new MemoryOperand(baseReg.DataType)
                    {
                        Base = baseReg,
                        PostIncrement = postInc,
                    };
                }
                d.prefix = new MemoryOperand(PrimitiveType.Word32)
                {
                    Base = inner,
                };
                return true;
            });

            var bdap = Prefix((u, d) =>
            {
                // Offset Addressing Mode Prefix Word
                var baseReg = Registers.GpRegisters[(u >> 12) & 0xF];
                var dt = dataWidths[(u >> 4) & 3];

                //$REFACTOR: so similar to Memory method.
                var iReg = u & 0xF;
                bool postInc = Bits.IsBitSet(u, 10);
                MachineOperand? op = null;
                if (postInc && iReg == 0xF)
                {
                    op = ReadImmediate(false, d);
                }
                else
                {
                    var mem = new MemoryOperand(dt ?? PrimitiveType.Word32);
                    var reg = Registers.GpRegisters[iReg];
                    mem.Base = reg;
                    mem.PostIncrement = postInc;
                    op = mem;
                }
                if (op is null)
                    return false;
                d.prefix = new MemoryOperand(PrimitiveType.Word32)
                {
                    Base = baseReg,
                    Offset = op,
                };
                return true;
            });

            /// <summary>
            /// Sets the prefix to the register to use 8-bit signed displacement.
            /// </summary>
            var bdapImm = Prefix((u, d) =>
            {
                Decoder.DumpMaskedInstruction(32, u, 0xFF, "  bdap prefix");
                // Offset Addressing Mode Prefix Word, Immediate byte Offset
                var baseReg = Registers.GpRegisters[(u >> 12) & 0xF];
                d.prefix = new MemoryOperand(PrimitiveType.Byte)
                {
                    Base = baseReg,
                    Offset = Constant.Int32((int)Bits.SignExtend(u, 8)),
                };
                return true;
            });

            var biap = Prefix((u, d) =>
            {
                // Indexed Addressing Mode Prefix Word.
                var baseReg = Registers.GpRegisters[u & 0xF];
                var idxReg = Registers.GpRegisters[(u >> 12) & 0xF];
                var scale = dataWidths[(u >> 4) & 3];
                d.prefix = new MemoryOperand(PrimitiveType.Byte)
                {
                    Base = baseReg,
                    Offset = idxReg,
                    IndexScale = scale,
                };
                return true;
            });

            var mode00 = Mask(6, 4, "  mode 0",
                bcc,
                bcc,
                bcc,
                bcc,

                bdapImm,
                bdapImm,
                bdapImm,
                bdapImm,

                Instr(Mnemonic.addq, j, R12),
                Instr(Mnemonic.moveq, i, R12),
                Instr(Mnemonic.subq, j, R12),
                Instr(Mnemonic.cmpq, i, R12),

                Instr(Mnemonic.andq, i, R12),
                Instr(Mnemonic.orq, i, R12),
                Mask(5, 1,
                    Instr(Mnemonic.btstq, c, R12),
                    Instr(Mnemonic.asrq, c, R12)),
                Mask(5, 1,
                    Instr(Mnemonic.lslq, c, NpcRd),
                    Instr(Mnemonic.lsrq, c, NpcRd)));

            var mode01 = Mask(6, 4, "  mode 1",
                Mask(5, 1, " 0b0000",
                    Instr(Mnemonic.addu, z, R0, R12),
                    Instr(Mnemonic.adds, z, R0, R12)),
                Mask(5, 1, " 0b0001",
                    Instr(Mnemonic.movu, z, R0, R12),
                    Instr(Mnemonic.movs, z, R0, R12)),
                Mask(5, 1, " 0b0010",
                    Instr(Mnemonic.subu, z, R0, R12),
                    Instr(Mnemonic.subs, z, R0, R12)),
                SizedInstr(
                    Instr(Mnemonic.lsl, zz, R0, NpcRd),
                    Instr(Mnemonic.btst, R0,R12)),

                Select(u => u == 0x050F,
                    Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),
                    SizedInstr(
                        Instr(Mnemonic.addi, ScR, NpcRd),
                        scc)),
                SizedInstr(
                    biap,      //   0 1  0 1 0 1 z z Base note7
                    reserved),
                SizedInstr(
                    Instr(Mnemonic.neg, zz, R0, NpcRd),
                    Instr(Mnemonic.setf, AllFlags(0xFFFF))),
                SizedInstr(
                    Instr(Mnemonic.bound, zz, R0, NpcRd), // Index    0 1  0 1 1 1 z z Bound
                    Instr(Mnemonic.clearf, AllFlags(0x7FFF))),

                SizedInstr(
                    Instr(Mnemonic.add, zz, R0, R12),
                    Instr(Mnemonic.move, R0,P12)),
                SizedInstr(
                    Instr(Mnemonic.move, R0, R12),
                    Select((0, 4), u => u == 0b1111,
                        // Assignments to PC are jumps.
                        Select((12, 4), u => u == 0b1011,
                            Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return),
                            Instr(Mnemonic.move, InstrClass.Transfer, P0, R12)),
                        Instr(Mnemonic.move, P0, R12))),
                SizedInstr(
                    Instr(Mnemonic.sub, zz, R0, R12),
                    Instr(Mnemonic.abs, R0,R12)),
                SizedInstr(
                    Instr(Mnemonic.cmp, zz, R0, R12),
                    Instr(Mnemonic.dstep, R0, NpcRd)),

                SizedInstr(
                    Instr(Mnemonic.and, zz, R0, R12),
                    Instr(Mnemonic.lz, R0, R12)),
                SizedInstr(
                    Instr(Mnemonic.or, zz, R0, R12),
                    Instr(Mnemonic.swap, SwapBits, NpcRd0)),
                SizedInstr(
                    Instr(Mnemonic.asr, zz, R0, R12),
                    Instr(Mnemonic.xor, R0,R12)),
                SizedInstr(
                    Instr(Mnemonic.lsr, zz, R0, NpcRd),
                    Instr(Mnemonic.mstep, R0, NpcRd)));

            var mode1x = Mask(6, 4, "  mode1x",
                Mask(5, 1,
                    Instr(Mnemonic.addu,z,Mem, R12),        // Dest.reg. 1 m 0 0 0 0 0 z Source
                    Instr(Mnemonic.adds,z,Mem, R12)),       // Dest.reg. 1 m 0 0 0 0 1 z Source
                Mask(5, 1,
                    Instr(Mnemonic.movu,z,Mem, R12),        // Dest.reg. 1 m 0 0 0 1 0 z Source
                    Instr(Mnemonic.movs,z,Mem, R12)),       // Dest.reg. 1 m 0 0 0 1 1 z Source

                Mask(5, 1,
                    Instr(Mnemonic.subu,z,Mem, R12),        // Dest.reg. 1 m 0 0 1 0 0 z Source
                    Instr(Mnemonic.subs,z,Mem, R12)),       // Dest.reg. 1 m 0 0 1 0 1 z Source
                Mask(5, 1,
                    Instr(Mnemonic.cmpu,z,Mem, R12),        // Dest.reg. 1 m 0 0 1 1 0 z Source
                    Instr(Mnemonic.cmps,z,Mem, R12)),       // Dest.reg. 1 m 0 0 1 1 1 z Source

                SizedInstr(
                    Mask(10, 1, 
                        Instr(Mnemonic.muls,zz, R0, NpcRd),    // Dest.reg. 1 0  0 1 0 0 z z Source reg.
                        Instr(Mnemonic.mulu,zz, R0, NpcRd)),   // Dest.reg. 1 1  0 1 0 0 z z Source reg.
                    Sparse(12, 4, "  jbrc/jsrc", reserved, // specreg -8 1 m 0 1 0 0 1 1 Source
                        (0x0, Instr(Mnemonic.jump, InstrClass.Transfer, MemJ)),    // 0 0 0 0 1 m 0 1 0 0 1 1 Source
                        (0x2, Instr(Mnemonic.jirc, InstrClass.Transfer, MemJ, skip4)),    // 0 0 0 0 1 m 0 1 0 0 1 1 Source
                        (0x3, Instr(Mnemonic.jsrc, InstrClass.Transfer|InstrClass.Call, MemJ)),    // 0 0 0 0 1 m 0 1 0 0 1 1 Source
                        (0x6, Instr(Mnemonic.jbrc, InstrClass.Transfer, MemJ, skip4)),    // 0 1 1 0 1 m 0 1 0 0 1 1 Source
                        (0x8, Instr(Mnemonic.jmpu, InstrClass.Transfer, MemJ, skip4)),
                        (0xA, Instr(Mnemonic.jir, InstrClass.Transfer, MemJ)),    // 0 0 0 0 1 m 0 1 0 0 1 1 Source
                        (0xB, Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, MemJ)),    // 0 0 0 0 1 m 0 1 0 0 1 1 Source
                        (0xF, reserved))),             //         1 0 0 0 1 m 0 1 0 0 1 1 Source
                        //Nyi("jsr / jir[ ]         "),     //    Special reg. 1 m 0 1 0 0 1 1 Source
                        //Nyi("break                "),     //       n 1 1 1 0 1 0 0 1 0 0 1 1 n
                SizedInstr(
                    bdap,                                   //         Base   1 m  0 1 0 1 z z Source note9
                    dip),                                   //         0 0 0 0 1 m 0 1 0 1 1 1 Source note10
                SizedInstr(
                    reserved,
                    Sparse(12, 4, reserved,
                        (0x0, Instr(Mnemonic.jump, InstrClass.Transfer, R0)),   // 0 0 0 0 1 m 0 1 0 0 1 1 Source
                        (0x2, Instr(Mnemonic.jirc, InstrClass.Transfer, R0, skip4)),    // 0 0 0 0 1 m 0 1 0 0 1 1 Source
                        (0x3, Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, R0, skip4)),     // 1 0 1 1 1 m 0 1 0 0 1 1 Source
                        (0x6, Instr(Mnemonic.jbrc, InstrClass.Transfer, R0, skip4)),    // 0 1 1 0 1 m 0 1 0 0 1 1 Source
                        (0xA, Instr(Mnemonic.jir, InstrClass.Transfer, R0)),    // 0 0 0 0 1 m 0 1 0 0 1 1 Source
                        (0xB, Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, R0))     // 1 0 1 1 1 m 0 1 0 0 1 1 Source
                    )),
                SizedInstr(
                    Instr(Mnemonic.bound,zz, Mem, R12),  // index     1 m 0 1 1 1 z z Bound
                    bccW),      //           1 m 0 1 1 1 1 1 1 1 1 1

                SizedInstr(
                    Instr(Mnemonic.add,zz, Mem, R12),           // Dest.reg. 1 m 1 0 0 0 z z Source
                    Instr(Mnemonic.move, Mem, P12)),            // Spec reg. 1 m 1 0 0 0 1 1 Source
                SizedInstr(
                    Instr(Mnemonic.move, zz, Mem, R12),         // Dest.reg. 1 m 1 0 0 1 z z Source
                    Instr(Mnemonic.move, P12,Mem)),             // Spec reg. 1 m 1 0 0 1 1 1 Dest.note11
                SizedInstr(
                    Instr(Mnemonic.sub,zz, Mem, R12),    // Dest.reg. 1 m 1 0 1 0 z z Source
                    reserved),                          // Dest.reg. 1 m 1 0 1 0 1 1 Source
                SizedInstr(
                    Instr(Mnemonic.cmp,zz, Mem, R12),    // Dest.reg. 1 m 1 0 1 1 z z Source
                    reserved),                          // Dest.reg. 1 m 1 0 1 1 1 1 Source

                SizedInstr(
                    Instr(Mnemonic.and,zz, Mem, R12),   // Dest.reg. 1 m 1 1 0 0 z z Source
                    Select((12, 4), u => u == 3,
                        Instr(Mnemonic.rbf, Mem),       //   0 0 1 1 1 m 1 1 0 0 1 1 Source
                        reserved)),
                SizedInstr(
                    Instr(Mnemonic.or,zz, Mem, R12),    // Dest.reg. 1 m 1 1 0 1 z z Source
                    Select((12, 4), u => u == 3,        //   0 0 1 1 1 m 1 1 0 1 1 1 Dest.
                        Instr(Mnemonic.sbfs, Mem),
                        reserved)),
                SizedInstr(
                    Instr(Mnemonic.test,zz, Mem),       //   0 0 0 0 1 m 1 1 1 0 z z Source
                    Instr(Mnemonic.movem, Mem,R12)),                // Dest.reg. 1 m 1 1 1 0 1 1 Source
                SizedInstr(
                    Instr(Mnemonic.move,zz, R0,Mem),    // Src reg.  1 m 1 1 1 1 z z Dest
                    Instr(Mnemonic.movem, R12, Mem)));             // Src reg.  1 m 1 1 1 1 1 1 Dest


            rootDecoder = Mask(10, 2, "Etrax mode",
                mode00,
                mode01,
                mode1x,
                mode1x);
        }
    }
}