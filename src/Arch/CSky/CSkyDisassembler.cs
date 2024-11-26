#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Reko.Arch.CSky
{
    using Decoder = Decoder<CSkyDisassembler, Mnemonic, CSkyInstruction>;

    public class CSkyDisassembler : DisassemblerBase<CSkyInstruction, Mnemonic>
    {
        private static Decoder<CSkyDisassembler, Mnemonic, CSkyInstruction> rootDecoder;

        private readonly CSkyArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<MachineOperand> ops;
        private Address addr;
        private long offset;

        public CSkyDisassembler(CSkyArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            this.addr = default!;
        }

        public override CSkyInstruction? DisassembleInstruction()
        {
            this.addr = rdr.Address;
            this.offset = rdr.Offset;
            if (!rdr.TryReadUInt16(out var uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            ops.Clear();
            instr.Address = addr;
            instr.Length = (int) (rdr.Offset - this.offset);
            return instr;
        }

        public override CSkyInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new CSkyInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Operands = ops.ToArray()
            };
        }

        public override CSkyInstruction CreateInvalidInstruction()
        {
            return new CSkyInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.invalid
            };
        }

        public override CSkyInstruction NotYetImplemented(string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("CSkyDis", this.addr, rdr, message);
            return new CSkyInstruction
            {
                InstructionClass = InstrClass.Invalid,
                Mnemonic = Mnemonic.nyi
            };
        }

        private static readonly Bitfield bf0_2 = new Bitfield(0, 2);
        private static readonly Bitfield bf0_4 = new Bitfield(0, 4);
        private static readonly Bitfield bf0_5 = new Bitfield(0, 5);
        private static readonly Bitfield bf0_12 = new Bitfield(0, 12);
        private static readonly Bitfield bf0_16 = new Bitfield(0, 16);
        private static readonly Bitfield bf2_3 = new Bitfield(2, 3);
        private static readonly Bitfield bf2_4 = new Bitfield(2, 4);
        private static readonly Bitfield bf4_4 = new Bitfield(4, 4);
        private static readonly Bitfield bf5_2 = new Bitfield(5, 2);
        private static readonly Bitfield bf5_3 = new Bitfield(5, 3);
        private static readonly Bitfield bf6_4 = new Bitfield(6, 4);
        private static readonly Bitfield bf8_2 = new Bitfield(8, 2);
        private static readonly Bitfield bf8_3 = new Bitfield(8, 3);
        private static readonly Bitfield bf16_5 = new Bitfield(16, 5);
        private static readonly Bitfield bf16_10 = new Bitfield(16, 10);
        private static readonly Bitfield bf21_4 = new Bitfield(21, 4);
        private static readonly Bitfield bf21_5 = new Bitfield(21, 5);
        private static readonly Bitfield[] bf8_0 = Bf((8, 2), (0, 5));

        private static Mutator<CSkyDisassembler> Register(Bitfield bf)
        {
            return (u, d) =>
            {
                var ireg = bf.Read(u);
                d.ops.Add(Registers.GpRegs[ireg]);
                return true;
            };
        }

        // Conventions: 3-bit fields are indicated by lower-case 'r' and a '_3' suffix
        // 4-bit fields are indicated by lower-case 'r', 5-bit fields by 
        // upper-case 'R'.
        private static readonly Mutator<CSkyDisassembler> R0 = Register(bf0_5);
        private static readonly Mutator<CSkyDisassembler> R16 = Register(bf16_5);
        private static readonly Mutator<CSkyDisassembler> R21 = Register(bf21_5);
        private static readonly Mutator<CSkyDisassembler> r2 = Register(bf2_4);
        private static readonly Mutator<CSkyDisassembler> r6 = Register(bf6_4);
        private static readonly Mutator<CSkyDisassembler> r2_3 = Register(bf2_3);
        private static readonly Mutator<CSkyDisassembler> r5_3 = Register(bf5_3);
        private static readonly Mutator<CSkyDisassembler> r8_3 = Register(bf8_3);


        private static Mutator<CSkyDisassembler> ControlRegister(Bitfield bf)
        {
            return (u, d) =>
            {
                var ireg = bf.Read(u);
                d.ops.Add(Registers.ControlRegister(ireg));
                return true;
            };
        }
        private static readonly Mutator<CSkyDisassembler> CR16 = ControlRegister(bf16_10);

        private static Mutator<CSkyDisassembler> VR(int bitpos)
        {
            var bf = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var ireg = bf.Read(u);
                d.ops.Add(Registers.VrRegs[ireg]);
                return true;
            };
        }
        private static readonly Mutator<CSkyDisassembler> VR0 = VR(0);
        private static readonly Mutator<CSkyDisassembler> VR16 = VR(16);
        private static readonly Mutator<CSkyDisassembler> VR21 = VR(21);


        /// <summary>
        /// A range of VR registers, encoded as a start register and a count.
        /// </summary>
        private static bool vrRange(uint uInstr, CSkyDisassembler dasm)
        {
            var ireg = (int)bf0_4.Read(uInstr);
            var imm4 = (int)bf21_4.Read(uInstr);
            if (ireg + imm4 >= 0x10)
                return false;
            var regs = new RegisterListOperand((uint)Bits.Mask(ireg, imm4 + 1), Registers.VrRegs);
            dasm.ops.Add(regs);
            return true;
        }

        private static Mutator<CSkyDisassembler> Register(RegisterStorage reg)
        {
            return (_, d) =>
            {
                d.ops.Add(reg);
                return true;
            };
        }
        private static readonly Mutator<CSkyDisassembler> reg28 = Register(Registers.GpRegs[28]);
        private static readonly Mutator<CSkyDisassembler> regsp = Register(Registers.GpRegs[14]);

        private static Mutator<CSkyDisassembler> uimm(int bitpos, int length)
        {
            var bf = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var imm = ImmediateOperand.Word32(bf.Read(u));
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<CSkyDisassembler> uimm5 = uimm(0, 5);
        private static readonly Mutator<CSkyDisassembler> uimm5_5 = uimm(5, 5);
        private static readonly Mutator<CSkyDisassembler> uimm8 = uimm(0, 8);
        private static readonly Mutator<CSkyDisassembler> uimm10_2 = uimm(10, 2);
        private static readonly Mutator<CSkyDisassembler> uimm12 = uimm(0, 12);
        private static readonly Mutator<CSkyDisassembler> uimm16 = uimm(0, 16);
        private static readonly Mutator<CSkyDisassembler> uimm21_4 = uimm(21, 4);
        private static readonly Mutator<CSkyDisassembler> uimm21_5 = uimm(21, 5);


        private static Mutator<CSkyDisassembler> uimmp1(int bitpos, int length)
        {
            var bf = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var value = bf.Read(u);
                var imm = ImmediateOperand.Word32(value + 1);
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<CSkyDisassembler> uimm2_3p1 = uimmp1(2, 3);
        private static readonly Mutator<CSkyDisassembler> uimm5p1 = uimmp1(0, 5);
        private static readonly Mutator<CSkyDisassembler> uimm5_5p1 = uimmp1(5, 5);
        private static readonly Mutator<CSkyDisassembler> uimm8p1 = uimmp1(0, 8);
        private static readonly Mutator<CSkyDisassembler> uimm12p1 = uimmp1(0, 12);
        private static readonly Mutator<CSkyDisassembler> uimm16p1 = uimmp1(0, 16);
        private static readonly Mutator<CSkyDisassembler> uimm18p1 = uimmp1(0, 18);

        private static Mutator<CSkyDisassembler> uimmsh(int bitpos, int length, int shift)
        {
            var bf = new Bitfield(bitpos, length);
            return (u, d) =>
            {
                var value = bf.Read(u);
                var imm = ImmediateOperand.Word32(value << shift);
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<CSkyDisassembler> uimm8_sh2 = uimmsh(0, 8, 2);

        private static Mutator<CSkyDisassembler> uimmsh(Bitfield[] fields, int shift)
        {
            return (u, d) =>
            {
                var value = Bitfield.ReadFields(fields, u);
                var imm = ImmediateOperand.Word32(value << shift);
                d.ops.Add(imm);
                return true;
            };
        }
        private static readonly Mutator<CSkyDisassembler> uimm8_2_0_5_sh2 = uimmsh(bf8_0, 2);

        private static Mutator<CSkyDisassembler> shamt(int bitposition, int length)
        {
            var bf = new Bitfield(bitposition, length);
            return (u, d) =>
            {
                var value = (int) bf.Read(u);
                d.ops.Add(ImmediateOperand.Int32(value));
                return true;
            };
        }
        private static readonly Mutator<CSkyDisassembler> shamt0 = shamt(0, 5);
        private static readonly Mutator<CSkyDisassembler> shamt21 = shamt(21, 5);


        private static Mutator<CSkyDisassembler> oimm(int bitposition, int length)
        {
            var bf = new Bitfield(bitposition, length);
            return (u, d) =>
            {
                var value = (int) bf.Read(u) + 1;
                d.ops.Add(ImmediateOperand.Int32(value));
                return true;
            };
        }
        private static readonly Mutator<CSkyDisassembler> oimm0 = oimm(0, 5);
        private static readonly Mutator<CSkyDisassembler> oimm21 = oimm(21, 5);


        private static bool bitpos(uint uInstr, CSkyDisassembler dasm)
        {
            var bitPos = (int)(uInstr & 0x1F);
            dasm.ops.Add(ImmediateOperand.Int32(bitPos));
            return true;
        }

        /// <summary>
        /// Displacement from PC+instruction length.
        /// </summary>
        private static Mutator<CSkyDisassembler> pcdisp(int length, int shift)
        {
            var bf = new Bitfield(0, length);
            return (u, d) =>
            {
                var displacement = bf.ReadSigned(u);
                var addrOp = d.addr + (displacement << shift);
                d.ops.Add(addrOp);
                return true;
            };
        }
        private static readonly Mutator<CSkyDisassembler> pcdisp10 = pcdisp(10, 1);
        private static readonly Mutator<CSkyDisassembler> pcdisp16_1 = pcdisp(16, 1);
        private static readonly Mutator<CSkyDisassembler> pcdisp18_0 = pcdisp(18, 0);
        private static readonly Mutator<CSkyDisassembler> pcdisp18_1 = pcdisp(18, 1);
        private static readonly Mutator<CSkyDisassembler> pcdisp18_2 = pcdisp(18, 2);
        private static readonly Mutator<CSkyDisassembler> pcdisp26 = pcdisp(26, 1);

        /// <summary>
        /// The last 2 bits of the instruction encode the scale factor for the
        /// indexed jump.
        /// </summary>
        /// <returns></returns>
        private static bool jmpix2(uint uInstr, CSkyDisassembler dasm)
        {
            var i = bf0_2.Read(uInstr);
            var imm = ImmediateOperand.UInt32(jmpix_scales[i]);
            dasm.ops.Add(imm);
            return true;
        }
        private static readonly uint[] jmpix_scales = new[] { 16u, 24u, 32u, 40u };

        /// <summary>
        /// Compact PC-relative load with "negate" offset. The manual is unclear,
        /// but it seems the offset is unsigned.
        /// </summary>
        /// <param name="uInstr"></param>
        /// <param name="dasm"></param>
        /// <returns></returns>
        private static bool pcReln(uint uInstr, CSkyDisassembler dasm)
        {
            var offset = (uint)(byte) ~Bitfield.ReadFields(bf8_0, uInstr) << 2;
            var uAddr = (int)((dasm.addr.Offset + offset) & ~3u);
            dasm.ops.Add(MemoryOperand.Displacement(PrimitiveType.Word32, null, uAddr));
            return true;
        }

        private static Mutator<CSkyDisassembler> M(PrimitiveType dt, Bitfield bfBase, Bitfield bfOffset, int shift)
        {
            return (u, d) =>
            {
                var ireg = bfBase.Read(u);
                var offset = ((int) bfOffset.Read(u)) << shift;
                var mem = MemoryOperand.Displacement(dt, Registers.GpRegs[ireg], offset);
                d.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Indirect memory access 
        /// </summary>
        private static Mutator<CSkyDisassembler> Mind(PrimitiveType dt, Bitfield bfBase)
        {
            return (u, d) =>
            {
                var ireg = bfBase.Read(u);
                var mem = MemoryOperand.Displacement(dt, Registers.GpRegs[ireg], 0);
                d.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Memory access using stack pointer as base register.
        /// </summary>
        private static Mutator<CSkyDisassembler> Msp(PrimitiveType dt, Bitfield[] bfsOffset)
        {
            return (u, d) =>
            {
                var offset = (int)Bitfield.ReadFields(bfsOffset, u) << 2;
                var mem = MemoryOperand.Displacement(dt, d.arch.StackRegister, offset);
                d.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// PC-relative offset, aligned to 4-byte boundary.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static Mutator<CSkyDisassembler> Mpc(PrimitiveType dt, params Bitfield[] bfs)
        {
            return (u, d) =>
            {
                var offset = Bitfield.ReadFields(bfs, u) << 2;
                var uAddr = (d.addr.Offset + offset) & ~3ul;
                var mem = MemoryOperand.Direct(dt, uAddr);
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<CSkyDisassembler> Mpc_offs5_2 = Mpc(PrimitiveType.Word32, bf8_2, bf0_5);
        private static readonly Mutator<CSkyDisassembler> Mpc_offs16 = Mpc(PrimitiveType.Word32, bf0_16);


        /// <summary>
        /// Memory access used for floating point loads and stores.
        /// </summary>
        private static Mutator<CSkyDisassembler> Mfp(PrimitiveType dt, int shift)
        {
            return (u, d) =>
            {
                var imm4h = bf21_4.Read(u) << 4;
                var imm4l = bf4_4.Read(u);
                var offset = (imm4h | imm4l) << shift;
                var ibase = bf16_5.Read(u);
                var rbase = Registers.GpRegs[ibase];
                var mem = MemoryOperand.Displacement(dt, rbase, (int)offset);
                d.ops.Add(mem);
                return true;
            };
        }

        private static Mutator<CSkyDisassembler> Midx(PrimitiveType dt, Bitfield bfShift)
        {
            return (u, d) =>
            {
                var ix = (int) bf16_5.Read(u);
                var iy = (int) bf21_5.Read(u);
                var shift = (int) bfShift.Read(u);
                var mem = MemoryOperand.Indexed(
                    dt,
                    Registers.GpRegs[ix],
                    Registers.GpRegs[iy],
                    shift);
                d.ops.Add(mem);
                return true;
            };
        }

        private static readonly uint[] decodePushPopArgs = new uint[]
        {
            0x0000,
            0x0010,
            0x0030,
            0x0070,

            0x00F0,
            0x01F0,
            0x03F0,
            0x07F0,

            0x0FF0,
            ~0u,
            ~0u,
            ~0u,

            ~0u,
            ~0u,
            ~0u,
            ~0u,
        };

        private static bool PushPopArgs(uint uInstr, CSkyDisassembler dasm)
        {
            var encodedList = bf0_4.Read(uInstr);
            var list = decodePushPopArgs[encodedList];
            if (list == ~0u)
                return false;
            list |= (uInstr & 0x10) << 11;
            dasm.ops.Add(new RegisterListOperand(list, Registers.GpRegs));
            return true; 
        }

        /// <summary>
        /// Memory access with base and index register.
        /// </summary>
        private static Mutator<CSkyDisassembler> Mx(PrimitiveType dt, int shift)
        {
            return (u, d) =>
            {
                var iBase = bf16_5.Read(u);
                var iIndex = bf21_5.Read(u);
                var mem = MemoryOperand.Indexed(dt, Registers.GpRegs[iBase], Registers.GpRegs[iIndex], shift);
                d.ops.Add(mem);
                return true;
            };
        }

        private class NextUInt16Decoder : Decoder
        {
            private readonly Decoder decoder;

            public NextUInt16Decoder(Decoder decoder)
            {
                this.decoder = decoder;
            }

            public override CSkyInstruction Decode(uint wInstr, CSkyDisassembler dasm)
            {
                if (!dasm.rdr.TryReadUInt16(out ushort loPart))
                    return dasm.CreateInvalidInstruction();
                uint uInstr = (wInstr << 16) | loPart;
                return decoder.Decode(uInstr, dasm);
            }
        }

        private static Decoder Instr(Mnemonic mnemonic, params Mutator<CSkyDisassembler>[] mutators)
        {
            return new InstrDecoder<CSkyDisassembler, Mnemonic, CSkyInstruction>(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<CSkyDisassembler>[] mutators)
        {
            return new InstrDecoder<CSkyDisassembler, Mnemonic, CSkyInstruction>(iclass, mnemonic, mutators);
        }


        static CSkyDisassembler()
        {
            var nyi = new NyiDecoder<CSkyDisassembler, Mnemonic, CSkyInstruction>("nyi");

            var bitop_16 = Mask(5, 3, "  bitop",
                Instr(Mnemonic.cmphsi, r8_3, uimm5p1),
                Instr(Mnemonic.cmplti, r8_3, uimm5p1),
                Instr(Mnemonic.cmpnei, r8_3, uimm5p1),
                nyi,

                Instr(Mnemonic.bclri, r8_3, bitpos),
                Instr(Mnemonic.bseti, r8_3, bitpos),
                Instr(Mnemonic.btsti, r8_3, bitpos),
                Mask(2, 3,
                    Instr(Mnemonic.jmpix, InstrClass.Transfer, r8_3, jmpix2),
                    nyi,
                    nyi,
                    nyi,

                    nyi,
                    nyi,
                    nyi,
                    nyi));

            var decoder00 = Mask(10, 4, "  16-bit 00",
                Select((0, 10), u => u == 0, "  0000",
                    Instr(Mnemonic.bkpt, InstrClass.Linear|InstrClass.Zero),
                    Instr(Mnemonic.lrw, r5_3, Mpc_offs5_2)),
                Instr(Mnemonic.br, InstrClass.Transfer, pcdisp10),
                Instr(Mnemonic.bt, InstrClass.ConditionalTransfer, pcdisp10),
                Instr(Mnemonic.bf, InstrClass.ConditionalTransfer, pcdisp10),

                Instr(Mnemonic.lrw, r5_3, pcReln),
                Mask(5, 3, "  5..3",
                    Instr(Mnemonic.addi, regsp, regsp, uimm8_2_0_5_sh2),
                    Instr(Mnemonic.subi, regsp, regsp, uimm8_2_0_5_sh2),
                    nyi,
                    Mask(0, 2, "  0..2",
                        Sparse(2, 3, "  ...00", nyi,
                            (0, Instr(Mnemonic.nie, InstrClass.Privileged |InstrClass.Linear))),
                        Sparse(2, 3, "  ...01", nyi,
                            (0, Instr(Mnemonic.nir, InstrClass.Privileged | InstrClass.Transfer | InstrClass.Return))),
                        Instr(Mnemonic.ipush),
                        Instr(Mnemonic.ipop)),

                    Instr(Mnemonic.pop, PushPopArgs),
                    Mask(0, 2, "  0..2",
                        Instr(Mnemonic.bpop_h, r2_3),
                        nyi,
                        Instr(Mnemonic.bpop_w, r2_3),
                        nyi),
                    Instr(Mnemonic.push, PushPopArgs),
                    Mask(0, 2, "  0..2",
                        Instr(Mnemonic.bpush_h, r2_3),
                        nyi,
                        Instr(Mnemonic.bpush_w, r2_3),
                        nyi)),
                Instr(Mnemonic.addi, r8_3, regsp, uimm8_sh2),
                Instr(Mnemonic.addi, r8_3, regsp, uimm8_sh2),

                Instr(Mnemonic.addi, r5_3, uimm8p1),
                Instr(Mnemonic.addi, r5_3, uimm8p1),
                Instr(Mnemonic.subi, r5_3, uimm8p1),
                Instr(Mnemonic.subi, r5_3, uimm8p1),

                Instr(Mnemonic.movi, r8_3, uimm8),
                Instr(Mnemonic.movi, r8_3, uimm8),
                bitop_16,
                bitop_16);

            var dec01_011 = Mask(0, 2, "  dec01 011",
                Instr(Mnemonic.addu, r5_3, r8_3, r2_3),
                Instr(Mnemonic.subu, r5_3, r8_3, r2_3),
                Instr(Mnemonic.addi, r5_3, r8_3, uimm2_3p1),
                Instr(Mnemonic.subi, r5_3, r8_3, uimm2_3p1));

            var asri_16 = Instr(Mnemonic.asri, r5_3, r8_3, shamt0);
            var lsli_16 = Instr(Mnemonic.lsli, r5_3, r8_3, shamt0);
            var lsri_16 = Instr(Mnemonic.lsri, r5_3, r8_3, shamt0);

            var decoder01 = Mask(10, 4, "  16-bit 01",
                lsli_16,
                lsli_16,
                lsri_16,
                lsri_16,

                asri_16,
                asri_16,
                dec01_011,
                dec01_011,

                Mask(0, 2, "  1000",
                    Instr(Mnemonic.addu, r6, r2),
                    Instr(Mnemonic.addc, r6, r2),
                    Instr(Mnemonic.subu, r6, r2),
                    Instr(Mnemonic.subc, r6, r2)),
                Mask(0, 2, "  1001",
                    Instr(Mnemonic.cmphs, r2, r6),
                    Instr(Mnemonic.cmplt, r2, r6),
                    Instr(Mnemonic.cmpne, r2, r6),
                    Instr(Mnemonic.mvcv, r6)),
                Mask(0, 2, "  1010",
                    Instr(Mnemonic.and, r6, r2),
                    Instr(Mnemonic.andn, r6, r2),
                    Instr(Mnemonic.tst, r6, r2),
                    Instr(Mnemonic.tstnbz, r2)),
                Mask(0, 2, "  1011",
                    Instr(Mnemonic.or, r6, r2),
                    Instr(Mnemonic.xor, r6, r2),
                    Instr(Mnemonic.nor, r6, r2),
                    Instr(Mnemonic.mov, r6, r2)),
                
                Mask(0, 2, "  1100",
                    Instr(Mnemonic.lsl, r6, r2),
                    Instr(Mnemonic.lsr, r6, r2),
                    Instr(Mnemonic.asr, r6, r2),
                    Instr(Mnemonic.rotl, r6, r2)),
                Mask(0, 2, "  1101",
                    Instr(Mnemonic.zextb, r6, r2),
                    Instr(Mnemonic.zexth, r6, r2),
                    Instr(Mnemonic.sextb, r6, r2),
                    Instr(Mnemonic.sexth, r6, r2)),
                Mask(0, 2, "  1110",
                    Instr(Mnemonic.jmp, InstrClass.Transfer, r2),
                    Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, r2),
                    Instr(Mnemonic.revb, r6, r2),
                    Instr(Mnemonic.revh, r6, r2)),
                Mask(0, 2, "  1111",
                    Instr(Mnemonic.mult, r6, r2),
                    Instr(Mnemonic.mulsh, r6, r2),
                    nyi,
                    nyi));

            var decoder10 = Mask(11, 3, "  10",
                Instr(Mnemonic.ld_b, r5_3, M(PrimitiveType.Byte, bf8_3, bf0_5, 0)),
                Instr(Mnemonic.ld_h, r5_3, M(PrimitiveType.Word16, bf8_3, bf0_5, 1)),
                Instr(Mnemonic.ld_w, r5_3, M(PrimitiveType.Word32, bf8_3, bf0_5, 2)),
                Instr(Mnemonic.ld_w, r5_3, Msp(PrimitiveType.Word32, bf8_0)),

                Instr(Mnemonic.st_b, r5_3, M(PrimitiveType.Byte, bf8_3, bf0_5, 0)),
                Instr(Mnemonic.st_h, r5_3, M(PrimitiveType.Word16, bf8_3, bf0_5, 0)),
                Instr(Mnemonic.st_w, r5_3, M(PrimitiveType.Word32, bf8_3, bf0_5, 2)),
                Instr(Mnemonic.st_w, r5_3, Msp(PrimitiveType.Word32, bf8_0)));

            var decoder11_1001 = Sparse(12, 4, "  1001", nyi,
                (0x0, Instr(Mnemonic.addi, R21, R16, uimm12)),
                (0x1, Instr(Mnemonic.subi, R21, R16, uimm12)),
                (0x2, Instr(Mnemonic.andi, R21, R16, uimm12)),
                (0x3, Instr(Mnemonic.andni, R21, R16, uimm12)),
                (0x4, Instr(Mnemonic.xori, R21, R16, uimm12)));

            var decoder11_1101 = Sparse(12, 4, "  1101", nyi,
                (0x0, Sparse(4, 8, "  0000", nyi,
                    (0x00, Instr(Mnemonic.fadds, VR0, VR16, VR21)),
                    (0x02, Instr(Mnemonic.fsubs, VR0, VR16, VR21)),
                    (0x08, Instr(Mnemonic.fmovs, VR0, VR16)),
                    (0x0C, Instr(Mnemonic.fabss, VR0, VR16)),
                    (0x0E, Instr(Mnemonic.fnegs, VR0, VR16)),
                    (0x10, Instr(Mnemonic.fcmpzhss, VR16, VR21)),
                    (0x12, Instr(Mnemonic.fcmpzlss, VR16, VR21)),
                    (0x14, Instr(Mnemonic.fcmpznes, VR16, VR21)),
                    (0x16, Instr(Mnemonic.fcmpzuos, VR16)),
                    (0x18, Instr(Mnemonic.fcmphss, VR16, VR21)),
                    (0x1A, Instr(Mnemonic.fcmplts, VR16, VR21)),
                    (0x1C, Instr(Mnemonic.fcmpnes, VR16, VR21)),
                    (0x1E, Instr(Mnemonic.fcmpuos, VR16, VR21)),
                    (0x20, Instr(Mnemonic.fmuls, VR0, VR16, VR21)),
                    (0x22, Instr(Mnemonic.fnmuls, VR0, VR16, VR21)),
                    (0x28, Instr(Mnemonic.fmacs, VR0, VR16, VR21)),
                    (0x2A, Instr(Mnemonic.fmscs, VR0, VR16, VR21)),
                    (0x2C, Instr(Mnemonic.fnmacs, VR0, VR16, VR21)),
                    (0x2E, Instr(Mnemonic.fnmscs, VR0, VR16, VR21)),
                    (0x30, Instr(Mnemonic.fdivs, VR0, VR16, VR21)),
                    (0x32, Instr(Mnemonic.frecips, VR0, VR16)),
                    (0x34, Instr(Mnemonic.fsqrts, VR0, VR16)),

                    (0x80, Instr(Mnemonic.faddd, VR0, VR16, VR21)),
                    (0x82, Instr(Mnemonic.fsubd, VR0, VR16, VR21)),
                    (0x88, Instr(Mnemonic.fmovd, VR0, VR16)),
                    (0x8C, Instr(Mnemonic.fabsd, VR0, VR16)),
                    (0x8E, Instr(Mnemonic.fnegd, VR0, VR16)),
                    (0x90, Instr(Mnemonic.fcmpzhsd, VR16, VR21)),
                    (0x92, Instr(Mnemonic.fcmpzlsd, VR16, VR21)),
                    (0x94, Instr(Mnemonic.fcmpzned, VR16, VR21)),
                    (0x96, Instr(Mnemonic.fcmpzuod, VR16)),
                    (0x98, Instr(Mnemonic.fcmphsd, VR16, VR21)),
                    (0x9A, Instr(Mnemonic.fcmpltd, VR16, VR21)),
                    (0x9C, Instr(Mnemonic.fcmpned, VR16, VR21)),
                    (0x9E, Instr(Mnemonic.fcmpuod, VR16, VR21)),
                    (0xA0, Instr(Mnemonic.fmuld, VR0, VR16, VR21)),
                    (0xA2, Instr(Mnemonic.fnmuld, VR0, VR16, VR21)),
                    (0xA8, Instr(Mnemonic.fmacd, VR0, VR16, VR21)),
                    (0xAA, Instr(Mnemonic.fmscd, VR0, VR16, VR21)),
                    (0xAC, Instr(Mnemonic.fnmacd, VR0, VR16, VR21)),
                    (0xAE, Instr(Mnemonic.fnmscd, VR0, VR16, VR21)),
                    (0xB0, Instr(Mnemonic.fdivd, VR0, VR16, VR21)),
                    (0xB2, Instr(Mnemonic.frecipd, VR0, VR16)),
                    (0xB4, Instr(Mnemonic.fsqrtd, VR0, VR16)))),
                (0x1, Sparse(4, 8, "  0001", nyi,
                    (0x00, Instr(Mnemonic.faddm, VR0, VR16, VR21)),
                    (0x02, Instr(Mnemonic.fsubm, VR0, VR16, VR21)),
                    (0x08, Instr(Mnemonic.fmovm, VR0, VR16)),
                    (0x0C, Instr(Mnemonic.fabsm, VR0, VR16)),
                    (0x0E, Instr(Mnemonic.fnegm, VR0, VR16)),
                    (0x20, Instr(Mnemonic.fmulm, VR0, VR16, VR21)),
                    (0x22, Instr(Mnemonic.fnmulm, VR0, VR16, VR21)),
                    (0x28, Instr(Mnemonic.fmacm, VR0, VR16, VR21)),
                    (0x2A, Instr(Mnemonic.fmscm, VR0, VR16, VR21)),
                    (0x2C, Instr(Mnemonic.fnmacm, VR0, VR16, VR21)),
                    (0x2E, Instr(Mnemonic.fnmscs, VR0, VR16, VR21)),


                    (0x80, Instr(Mnemonic.fstosi_rn, VR0, VR16)),
                    (0x82, Instr(Mnemonic.fstosi_rz, VR0, VR16)),
                    (0x84, Instr(Mnemonic.fstosi_rpi, VR0, VR16)),
                    (0x86, Instr(Mnemonic.fstosi_rni, VR0, VR16)),
                    (0x88, Instr(Mnemonic.fstoui_rn, VR0, VR16)),
                    (0x8A, Instr(Mnemonic.fstoui_rz, VR0, VR16)),
                    (0x8C, Instr(Mnemonic.fstoui_rpi, VR0, VR16)),
                    (0x8E, Instr(Mnemonic.fstoui_rni, VR0, VR16)),
                    (0x90, Instr(Mnemonic.fdtosi_rn, VR0, VR16)),
                    (0x92, Instr(Mnemonic.fdtosi_rz, VR0, VR16)),
                    (0x94, Instr(Mnemonic.fdtosi_rpi, VR0, VR16)),
                    (0x96, Instr(Mnemonic.fdtosi_rni, VR0, VR16)),
                    (0x98, Instr(Mnemonic.fdtoui_rn, VR0, VR16)),
                    (0x9A, Instr(Mnemonic.fdtoui_rz, VR0, VR16)),
                    (0x9C, Instr(Mnemonic.fdtoui_rpi, VR0, VR16)),
                    (0x9E, Instr(Mnemonic.fdtoui_rni, VR0, VR16)),
                    (0xA0, Instr(Mnemonic.fsitos, VR0, VR16)),
                    (0xA2, Instr(Mnemonic.fuitos, VR0, VR16)),
                    (0xA8, Instr(Mnemonic.fsitod, VR0, VR16)),
                    (0xAA, Instr(Mnemonic.fuitod, VR0, VR16)),
                    (0xAC, Instr(Mnemonic.fdtos, VR0, VR16)),
                    (0xAE, Instr(Mnemonic.fstod, VR0, VR16)),
                    (0xB0, Instr(Mnemonic.fmfvrh, R0, VR16)),
                    (0xB2, Instr(Mnemonic.fmfvrl, R0, VR16)),
                    (0xB4, Instr(Mnemonic.fmtvrh, VR0, R16)),
                    (0xB6, Instr(Mnemonic.fmtvrl, VR0, R16)))),
                (0x2, Sparse(8, 4, "  0010", nyi,
                    (0x0, Instr(Mnemonic.flds, VR0, Mfp(PrimitiveType.Real32, 2))),
                    (0x1, Instr(Mnemonic.fldd, VR0, Mfp(PrimitiveType.Real64, 2))),
                    (0x2, Instr(Mnemonic.fldm, VR0, Mfp(PrimitiveType.Real64, 3))),
                    (0x4, Instr(Mnemonic.fsts, VR0, Mfp(PrimitiveType.Real64, 2))),
                    (0x5, Instr(Mnemonic.fstd, VR0, Mfp(PrimitiveType.Real64, 2))),
                    (0x6, Instr(Mnemonic.fstm, VR0, Mfp(PrimitiveType.Real64, 3))),
                    (0x8, Instr(Mnemonic.fldrs, VR0, Midx(PrimitiveType.Real32, bf5_2))),
                    (0x9, Instr(Mnemonic.fldrd, VR0, Midx(PrimitiveType.Real64, bf5_2))),
                    (0xA, Instr(Mnemonic.fldrm, VR0, Midx(PrimitiveType.Real64, bf5_2))),
                    (0xC, Instr(Mnemonic.fstrs, VR0, Midx(PrimitiveType.Real64, bf5_2))),
                    (0xD, Instr(Mnemonic.fstrd, VR0, Midx(PrimitiveType.Real64, bf5_2))),
                    (0xE, Instr(Mnemonic.fstrm, VR0, Midx(PrimitiveType.Real64, bf5_2)))
                    )),
                (0x3, Sparse(8, 4, "  0011", nyi,
                    (0x0, Instr(Mnemonic.fldms, vrRange, Mind(PrimitiveType.Real64, bf16_5))),
                    (0x1, Instr(Mnemonic.fldmd, vrRange, Mind(PrimitiveType.Real64, bf16_5))),
                    (0x2, Instr(Mnemonic.fldmd, VR0, Mfp(PrimitiveType.Real64, 3))))));

            var decoder11 = Mask(26, 4, "  11",
                Sparse(10, 6, "  0000", nyi,
                    (0x01, Sparse(5, 5, "  000100", nyi,
                        (0x01, Instr(Mnemonic.sync, uimm21_5)))),
                    (0x04, Sparse(5, 5, "  000100", nyi,
                        (0x01, Instr(Mnemonic.bmset)))),
                    (0x05, Sparse(5, 5, "  000100", nyi,
                        (0x01, Instr(Mnemonic.bmclr)))),
                    (0x06, Sparse(5, 5, "  000100", nyi,
                        (0x01, Instr(Mnemonic.sce, uimm21_4)))),
                    (0x07, Sparse(5, 5, "  000100", nyi,
                        (0x01, Instr(Mnemonic.idly, uimm21_5)))),
                    (0x08, Sparse(5, 5, "  001000", nyi,
                        (0x01, Instr(Mnemonic.trap, InstrClass.Transfer|InstrClass.Call, uimm10_2)))),
                    (0x09, Sparse(5, 5, "  001001", nyi,
                        (0x01, Instr(Mnemonic.trap, InstrClass.Transfer | InstrClass.Call, uimm10_2)))),
                    (0x0A, Sparse(5, 5, "  001010", nyi,
                        (0x01, Instr(Mnemonic.trap, InstrClass.Transfer | InstrClass.Call, uimm10_2)))),
                    (0x0B, Sparse(5, 5, "  001011", nyi,
                        (0x01, Instr(Mnemonic.trap, InstrClass.Transfer | InstrClass.Call, uimm10_2)))),
                    (0x10, Sparse(5, 5, "  010000", nyi,
                        (0x01, Instr(Mnemonic.rte, InstrClass.Privileged | InstrClass.Transfer | InstrClass.Return)))),
                    (0x11, Sparse(5, 5, "  010001", nyi,
                        (0x01, Instr(Mnemonic.rfi, InstrClass.Privileged | InstrClass.Transfer | InstrClass.Return)))),
                    (0x12, Sparse(5, 5, "  010010", nyi,
                        (0x01, Instr(Mnemonic.stop, InstrClass.Privileged | InstrClass.Linear)))),
                    (0x13, Sparse(5, 5, "  010011", nyi,
                        (0x01, Instr(Mnemonic.wait, InstrClass.Privileged | InstrClass.Linear)))),
                    (0x14, Sparse(5, 5, "  010100", nyi,
                        (0x01, Instr(Mnemonic.doze, InstrClass.Privileged | InstrClass.Linear)))),
                    (0x15, Sparse(5, 5, "  010101", nyi,
                        (0x01, Instr(Mnemonic.we, InstrClass.Privileged | InstrClass.Linear)))),
                    (0x16, Sparse(5, 5, "  010110", nyi,
                        (0x01, Instr(Mnemonic.se, InstrClass.Linear)))),
                    (0x18, Sparse(5, 5, "  0110000", nyi,
                        (0x01, Instr(Mnemonic.mfcr, InstrClass.Privileged|InstrClass.Linear, R0, CR16)))),
                    (0x19, Sparse(5, 5, "  0110000", nyi,
                        (0x01, Instr(Mnemonic.mtcr, InstrClass.Privileged | InstrClass.Linear, CR16, R0)))),
                    (0x1E, Sparse(5, 5, "  0110000", nyi,
                        (0x01, Instr(Mnemonic.strap, InstrClass.Privileged | InstrClass.Linear, CR16, R0)))),
                    (0x1F, Sparse(5, 5, "  0110000", nyi,
                        (0x01, Instr(Mnemonic.srte, InstrClass.Privileged | InstrClass.Transfer|InstrClass.Return))))),
                Sparse(10, 6, "  0001", nyi,
                    (0x00, Sparse(5, 5, "  000000", nyi,
                        (0x01, Instr(Mnemonic.addu, R0, R16, R21)),
                        (0x02, Instr(Mnemonic.addc, R0, R16, R21)),
                        (0x04, Instr(Mnemonic.subu, R0, R16, R21)),
                        (0x08, Instr(Mnemonic.subc, R0, R16, R21)),
                        (0x10, Instr(Mnemonic.abs, R0, R16)))),
                    (0x01, Sparse(5, 5, "  000001", nyi,
                        (0x02, Instr(Mnemonic.cmplt, R16, R21)),
                        (0x04, Instr(Mnemonic.cmpne, R16, R21)),
                        (0x08, Instr(Mnemonic.mvc, R0)),
                        (0x10, Instr(Mnemonic.mvcv, R0)))),
                    (0x02, Sparse(5, 5, "  000010", nyi,
                        (0x01, Instr(Mnemonic.ixh, R0, R16, R21)),
                        (0x02, Instr(Mnemonic.ixw, R0, R16, R21)),
                        (0x04, Instr(Mnemonic.ixd, R0, R16, R21)))),
                    (0x03, Sparse(5, 5, "  000011", nyi,
                        (0x01, Instr(Mnemonic.incf, R21, R16, uimm5)),
                        (0x02, Instr(Mnemonic.inct, R21, R16, uimm5)),
                        (0x04, Instr(Mnemonic.decf, R21, R16, uimm5)),
                        (0x08, Instr(Mnemonic.dect, R21, R16, uimm5)))),
                    (0x04, Sparse(5, 5, "  000100", nyi,
                        (0x01, Instr(Mnemonic.decgt, R0, R16, uimm21_5)),
                        (0x02, Instr(Mnemonic.declt, R0, R16, uimm21_5)),
                        (0x04, Instr(Mnemonic.decne, R0, R16, uimm21_5)))),
                    (0x08, Sparse(5, 5, "  001000", nyi,
                        (0x01, Instr(Mnemonic.and, R0, R16, R21)),
                        (0x02, Instr(Mnemonic.andn, R0, R16, R21)),
                        (0x04, Instr(Mnemonic.tst, R16, R21)),
                        (0x08, Instr(Mnemonic.tstnbz, R16)))),
                    (0x09, Sparse(5, 5, "  001001", nyi,
                        (0x01, Instr(Mnemonic.or, R0, R16, R21)),
                        (0x02, Instr(Mnemonic.xor, R0, R16, R21)),
                        (0x04, Instr(Mnemonic.nor, R0, R16, R21)))),
                    (0x0A, Sparse(5, 5, "  001010", nyi,
                        (0x01, Instr(Mnemonic.bclri, R0, R16, bitpos)),
                        (0x02, Instr(Mnemonic.bseti, R0, R16, bitpos)),
                        (0x04, Instr(Mnemonic.btsti, R0, R16, bitpos)))),
                    (0x0B, Sparse(5, 5, "  001011", nyi,
                        (0x01, Instr(Mnemonic.clrf, R21)),
                        (0x02, Instr(Mnemonic.clrt, R21)))),
                    (0x10, Sparse(5, 5, "  010000", nyi,
                        (0x01, Instr(Mnemonic.lsl, R0, R16, R21)),
                        (0x02, Instr(Mnemonic.lsr, R0, R16, R21)),
                        (0x04, Instr(Mnemonic.asr, R0, R16, R21)),
                        (0x08, Instr(Mnemonic.rotl, R0, R16, R21)))),
                    (0x12, Sparse(5, 5, "  010010", nyi,
                        (0x01, Select((21,5), w => w == 0,
                            Instr(Mnemonic.mov, R0, R16),
                            Instr(Mnemonic.lsli, R0, R16, shamt21))),
                        (0x02, Instr(Mnemonic.lsri, R0, R16, shamt21)),
                        (0x04, Instr(Mnemonic.asri, R0, R16, shamt21)),
                        (0x08, Instr(Mnemonic.rotli, R0, R16, shamt21)))),
                    (0x13, Sparse(5, 5, "  010011", nyi,
                        (0x01, Instr(Mnemonic.lslc, R0, R16, oimm21)),
                        (0x02, Instr(Mnemonic.lsrc, R0, R16, oimm21)),
                        (0x04, Instr(Mnemonic.asrc, R0, R16, oimm21)),
                        (0x08, Instr(Mnemonic.xsr, R0, R16, oimm21)))),
                    (0x14, Sparse(5, 5, "  010100", nyi,
                        (0x01, Instr(Mnemonic.bmaski, R0, oimm21)),
                        (0x02, Instr(Mnemonic.bgenr, R0, R16)))),
                    (0x15, Instr(Mnemonic.zext, R21, R16, uimm21_5, uimm5_5)),
                    (0x16, Instr(Mnemonic.sext, R21, R16, uimm21_5, uimm5_5)),
                    (0x17, Instr(Mnemonic.ins, R21, R16, uimm5_5p1, uimm5)),
                    (0x18, Sparse(5, 5, "  011000", nyi,
                        (0x04, Instr(Mnemonic.revb, R0, R16)),
                        (0x08, Instr(Mnemonic.revh, R0, R16)),
                        (0x10, Instr(Mnemonic.brev, R0, R16)))),
                    (0x1C, Sparse(5, 5, "  011100", nyi,
                        (0x01, Instr(Mnemonic.xtb0, R0, R16)),
                        (0x02, Instr(Mnemonic.xtb1, R0, R16)),
                        (0x04, Instr(Mnemonic.xtb2, R0, R16)),
                        (0x08, Instr(Mnemonic.xtb3, R0, R16)))),
                    (0x1F, Sparse(5, 5, "  011111", nyi,
                        (0x01, Instr(Mnemonic.ff0, R0, R16)),
                        (0x02, Instr(Mnemonic.ff1, R0, R16)))),
                    (0x20, Sparse(5, 5, "  100000", nyi,
                        (0x01, Instr(Mnemonic.divu, R0, R16, R21)),
                        (0x02, Instr(Mnemonic.divs, R0, R16, R21)))),
                    (0x21, Sparse(5, 5, "  100001", nyi,
                        (0x01, Instr(Mnemonic.mult, R0, R16, R21)))),
                    (0x22, Sparse(5, 5, "  100001", nyi,
                        (0x01, Instr(Mnemonic.mulu, R16, R21)),
                        (0x02, Instr(Mnemonic.mulua, R16, R21)),
                        (0x04, Instr(Mnemonic.mulus, R16, R21)))),
                    (0x23, Sparse(5, 5, "  100011", nyi,
                        (0x01, Instr(Mnemonic.muls, R16, R21)),
                        (0x02, Instr(Mnemonic.mulsa, R16, R21)),
                        (0x04, Instr(Mnemonic.mulss, R16, R21)))),
                    (0x24, Sparse(5, 5, "  100100", nyi,
                        (0x01, Instr(Mnemonic.mulsh, R0, R16, R21)),
                        (0x02, Instr(Mnemonic.mulsha, R16, R21)),
                        (0x04, Instr(Mnemonic.mulshs, R16, R21)))),
                    (0x25, Sparse(5, 5, "  100101", nyi,
                        (0x01, Instr(Mnemonic.mulsw, R0, R16, R21)),
                        (0x02, Instr(Mnemonic.mulw, R16, R21)),
                        (0x04, Instr(Mnemonic.mulswa, R16, R21)),
                        (0x08, Instr(Mnemonic.mulsws, R16, R21)))),
                    (0x26, Sparse(5, 5, "  100111", nyi,
                        (0x01, Instr(Mnemonic.mfhis, R0)),
                        (0x04, Instr(Mnemonic.mflos, R0)),
                        (0x10, Instr(Mnemonic.mvtc, R0)))),
                    (0x27, Sparse(5, 5, "  100111", nyi,
                        (0x01, Instr(Mnemonic.mfhi, R0)),
                        (0x02, Instr(Mnemonic.mthi, R0)),
                        (0x04, Instr(Mnemonic.mflo, R0)),
                        (0x08, Instr(Mnemonic.mtlo, R0)))),
                    (0x2C, Sparse(5, 5, "  101100", nyi,   
                        (0x01, Instr(Mnemonic.vmulsh, R16, R21)),
                        (0x02, Instr(Mnemonic.vmulsha, R16, R21)),
                        (0x04, Instr(Mnemonic.vmulshs, R16, R21)))),
                    (0x2D, Sparse(5, 5, "  101100", nyi,
                        (0x01, Instr(Mnemonic.vmulsw, R16, R21)),
                        (0x02, Instr(Mnemonic.vmulswa, R16, R21)),
                        (0x04, Instr(Mnemonic.vmulsws, R16, R21))))),

                nyi,
                Mask(18, 3, "  0011",
                    Instr(Mnemonic.lrs_b, R21, reg28, pcdisp18_0),
                    Instr(Mnemonic.lrs_h, R21, reg28, pcdisp18_1),
                    Instr(Mnemonic.lrs_w, R21, reg28, pcdisp18_2),
                    Instr(Mnemonic.grs, R21, pcdisp18_1),
                    Instr(Mnemonic.srs_b, R21, reg28, pcdisp18_0),
                    Instr(Mnemonic.srs_h, R21, reg28, pcdisp18_1),
                    Instr(Mnemonic.srs_w, R21, reg28, pcdisp18_2),
                    Instr(Mnemonic.addi, R21, reg28, uimm18p1)),

                Sparse(10, 6, "  0100", nyi,
                    (0x00, Sparse(5, 5, "  ldr.b", nyi,
                        (0x01, Instr(Mnemonic.ldr_b, R0, Mx(PrimitiveType.Byte, 0))),
                        (0x02, Instr(Mnemonic.ldr_b, R0, Mx(PrimitiveType.Byte, 1))),
                        (0x04, Instr(Mnemonic.ldr_b, R0, Mx(PrimitiveType.Byte, 2))),
                        (0x08, Instr(Mnemonic.ldr_b, R0, Mx(PrimitiveType.Byte, 3))))),
                    (0x01, Sparse(5, 5, "  ldr.h", nyi,
                        (0x01, Instr(Mnemonic.ldr_h, R0, Mx(PrimitiveType.Word16, 0))),
                        (0x02, Instr(Mnemonic.ldr_h, R0, Mx(PrimitiveType.Word16, 1))),
                        (0x04, Instr(Mnemonic.ldr_h, R0, Mx(PrimitiveType.Word16, 2))),
                        (0x08, Instr(Mnemonic.ldr_h, R0, Mx(PrimitiveType.Word16, 3))))),
                    (0x02, Sparse(5, 5, "  ldr.w", nyi,
                        (0x01, Instr(Mnemonic.ldr_w, R0, Mx(PrimitiveType.Word32, 0))),
                        (0x02, Instr(Mnemonic.ldr_w, R0, Mx(PrimitiveType.Word32, 1))),
                        (0x04, Instr(Mnemonic.ldr_w, R0, Mx(PrimitiveType.Word32, 2))),
                        (0x08, Instr(Mnemonic.ldr_w, R0, Mx(PrimitiveType.Word32, 3))))),
                    (0x04, Sparse(5, 5, "  ldr.bs", nyi,
                        (0x01, Instr(Mnemonic.ldr_bs, R0, Mx(PrimitiveType.Int8, 0))),
                        (0x02, Instr(Mnemonic.ldr_bs, R0, Mx(PrimitiveType.Int8, 1))),
                        (0x04, Instr(Mnemonic.ldr_bs, R0, Mx(PrimitiveType.Int8, 2))),
                        (0x08, Instr(Mnemonic.ldr_bs, R0, Mx(PrimitiveType.Int8, 3))))),
                    (0x05, Sparse(5, 5, "  ldr.hs", nyi,
                        (0x01, Instr(Mnemonic.ldr_hs, R0, Mx(PrimitiveType.Int16, 0))),
                        (0x02, Instr(Mnemonic.ldr_hs, R0, Mx(PrimitiveType.Int16, 1))),
                        (0x04, Instr(Mnemonic.ldr_hs, R0, Mx(PrimitiveType.Int16, 2))),
                        (0x08, Instr(Mnemonic.ldr_hs, R0, Mx(PrimitiveType.Int16, 3)))))),
                Sparse(10, 6, "  0101", nyi,
                    (0x00, Sparse(5, 5, "  str.b", nyi,
                        (0x01, Instr(Mnemonic.str_b, R0, Mx(PrimitiveType.Byte, 0))),
                        (0x02, Instr(Mnemonic.str_b, R0, Mx(PrimitiveType.Byte, 1))),
                        (0x04, Instr(Mnemonic.str_b, R0, Mx(PrimitiveType.Byte, 2))),
                        (0x08, Instr(Mnemonic.str_b, R0, Mx(PrimitiveType.Byte, 3))))),
                    (0x01, Sparse(5, 5, "  str.h", nyi,
                        (0x01, Instr(Mnemonic.str_h, R0, Mx(PrimitiveType.Word16, 0))),
                        (0x02, Instr(Mnemonic.str_h, R0, Mx(PrimitiveType.Word16, 1))),
                        (0x04, Instr(Mnemonic.str_h, R0, Mx(PrimitiveType.Word16, 2))),
                        (0x08, Instr(Mnemonic.str_h, R0, Mx(PrimitiveType.Word16, 3))))),
                    (0x02, Sparse(5, 5, "  str.w", nyi,
                        (0x01, Instr(Mnemonic.str_w, R0, Mx(PrimitiveType.Word32, 0))),
                        (0x02, Instr(Mnemonic.str_w, R0, Mx(PrimitiveType.Word32, 1))),
                        (0x04, Instr(Mnemonic.str_w, R0, Mx(PrimitiveType.Word32, 2))),
                        (0x08, Instr(Mnemonic.str_w, R0, Mx(PrimitiveType.Word32, 3)))))),
                Mask(12, 4, "  0110",
                    Instr(Mnemonic.ld_b, R21, M(PrimitiveType.Byte, bf16_5, bf0_12, 0)),
                    Instr(Mnemonic.ld_h, R21, M(PrimitiveType.Word16, bf16_5, bf0_12, 1)),
                    Instr(Mnemonic.ld_w, R21, M(PrimitiveType.Word32, bf16_5, bf0_12, 2)),
                    Instr(Mnemonic.ld_d, R21, M(PrimitiveType.Word64, bf16_5, bf0_12, 3)),

                    Instr(Mnemonic.ld_bs, R21, M(PrimitiveType.Int8, bf16_5, bf0_12, 0)),
                    Instr(Mnemonic.ld_hs, R21, M(PrimitiveType.Int16, bf16_5, bf0_12, 1)),
                    Instr(Mnemonic.pldr, M(PrimitiveType.Word32, bf16_5, bf0_12, 2)),
                    Instr(Mnemonic.ldex_w, R21, M(PrimitiveType.Word32, bf16_5, bf0_12, 2)),

                    nyi,
                    nyi,
                    nyi,
                    nyi,

                    nyi,
                    nyi,
                    nyi,
                    nyi),
                Mask(12, 4, "  0111",
                    Instr(Mnemonic.st_b, R21, M(PrimitiveType.Byte, bf16_5, bf0_12, 0)),
                    Instr(Mnemonic.st_h, R21, M(PrimitiveType.Word16, bf16_5, bf0_12, 1)),
                    Instr(Mnemonic.st_w, R21, M(PrimitiveType.Word32, bf16_5, bf0_12, 2)),
                    Instr(Mnemonic.st_d, R21, M(PrimitiveType.Word64, bf16_5, bf0_12, 3)),

                    nyi,
                    nyi,
                    Instr(Mnemonic.pldrw, M(PrimitiveType.Word32, bf16_5, bf0_12, 2)),
                    Instr(Mnemonic.stex_w, R21, M(PrimitiveType.Word32, bf16_5, bf0_12, 2)),

                    nyi,
                    nyi,
                    nyi,
                    nyi,

                    nyi,
                    nyi,
                    nyi,
                    nyi),

                Instr(Mnemonic.bsr, InstrClass.Transfer|InstrClass.Call, pcdisp26),
                decoder11_1001,
                Sparse(21, 5, "  1010", nyi,
                    (0x00, Instr(Mnemonic.br, InstrClass.Transfer, pcdisp16_1)),
                    (0x02, Instr(Mnemonic.bf, InstrClass.Transfer|InstrClass.Conditional, pcdisp16_1)),
                    (0x03, Instr(Mnemonic.bt, InstrClass.Transfer|InstrClass.Conditional, pcdisp16_1)),
                    (0x06, If(u => (u & 0xFFFFu) == 0,
                        Instr(Mnemonic.jmp, InstrClass.Transfer, R16))),
                    (0x07, If(u => (u & 0xFFFFu) == 0,
                        Instr(Mnemonic.jsr, InstrClass.Transfer|InstrClass.Call, R16))),
                    (0x08, Instr(Mnemonic.bez, InstrClass.Transfer|InstrClass.Conditional, R16, pcdisp16_1)),
                    (0x09, Instr(Mnemonic.bnez, InstrClass.Transfer|InstrClass.Conditional, R16, pcdisp16_1)),
                    (0x0A, Instr(Mnemonic.bhz, InstrClass.Transfer | InstrClass.Conditional, R16, pcdisp16_1)),
                    (0x0B, Instr(Mnemonic.blsz, InstrClass.Transfer | InstrClass.Conditional, R16, pcdisp16_1)),
                    (0x0C, Instr(Mnemonic.blz, InstrClass.Transfer | InstrClass.Conditional, R16, pcdisp16_1)),
                    (0x0D, Instr(Mnemonic.bhsz, InstrClass.Transfer | InstrClass.Conditional, R16, pcdisp16_1)),
                    (0x0F, If(u => (u & 0xFFFC) == 0,
                        Instr(Mnemonic.jmpix, InstrClass.Transfer, R16, jmpix2))),
                    (0x10, Instr(Mnemonic.movi, R16, uimm16)),
                    (0x11, Instr(Mnemonic.movih, R16, uimm16)),
                    (0x14, Instr(Mnemonic.lrw, R16, Mpc_offs16)),
                    (0x16, Instr(Mnemonic.jmpi, InstrClass.Transfer, pcdisp16_1)),
                    (0x17, Instr(Mnemonic.jsri, InstrClass.Transfer | InstrClass.Call, pcdisp16_1)),
                    (0x18, Instr(Mnemonic.cmphsi, R16, uimm16p1)),
                    (0x19, Instr(Mnemonic.cmplti, R16, uimm16p1)),
                    (0x1A, Instr(Mnemonic.cmpnei, R16, uimm16p1))),
                Instr(Mnemonic.ori, R21, R16, uimm16),

                nyi,
                decoder11_1101,
                nyi,
                nyi);

            rootDecoder = Mask(14, 2, "CSky",
                decoder00,
                decoder01,
                decoder10,
                new NextUInt16Decoder(decoder11));
        }
    }
}
