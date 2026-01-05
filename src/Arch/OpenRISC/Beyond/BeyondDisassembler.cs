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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Arch.OpenRISC.Beyond;

public class BeyondDisassembler : DisassemblerBase<BeyondInstruction, Mnemonic>
{
    private static readonly WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> rootDecoder;
    
    private readonly BeyondArchitecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;

    public BeyondDisassembler(BeyondArchitecture arch, EndianImageReader rdr)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = [];
    }

    public override BeyondInstruction? DisassembleInstruction()
    {
        var offset = rdr.Offset;
        this.addr = rdr.Address;
        if (!rdr.TryReadUInt16(out ushort uInstr))
            return null;
        ops.Clear();
        var instr = rootDecoder.Decode(uInstr, this);
        instr.Address = this.addr;
        instr.Length = (int) (rdr.Offset - offset);
        if (uInstr == 0)
            instr.InstructionClass |= InstrClass.Zero;
        return instr;
    }

    public override BeyondInstruction CreateInvalidInstruction()
    {
        return new BeyondInstruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid,
        };
    }

    public override BeyondInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new BeyondInstruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Operands = ops.ToArray(),
        };
    }

    public override BeyondInstruction NotYetImplemented(string message)
    {
        var testGenSvc = arch.Services.GetService<ITestGenerationService>();
        testGenSvc?.ReportMissingDecoder("BeyondDis", this.addr, this.rdr, message);
        return new BeyondInstruction
        {
            InstructionClass = InstrClass.Linear,
            Mnemonic = Mnemonic.Nyi,
        };
    }

    private static WideMutator<BeyondDisassembler> r(int bitpos, int bitlength)
    {
        var field = new Bitfield(bitpos, bitlength);
        return (u, d) =>
        {
            var ireg = field.Read(u);
            d.ops.Add(Registers.GpRegs[ireg]);
            return true;
        };
    }
    private static readonly WideMutator<BeyondDisassembler> r0 = r(0, 5);
    private static readonly WideMutator<BeyondDisassembler> r3 = r(3, 5);
    private static readonly WideMutator<BeyondDisassembler> r5 = r(5, 5);
    private static readonly WideMutator<BeyondDisassembler> r8 = r(8, 5);
    private static readonly WideMutator<BeyondDisassembler> r12 = r(12, 5);
    private static readonly WideMutator<BeyondDisassembler> r13 = r(13, 5);
    private static readonly WideMutator<BeyondDisassembler> r16 = r(16, 5);
    private static readonly WideMutator<BeyondDisassembler> r17 = r(17, 5);
    private static readonly WideMutator<BeyondDisassembler> r21 = r(21, 5);
    private static readonly WideMutator<BeyondDisassembler> r28 = r(28, 5);
    private static readonly WideMutator<BeyondDisassembler> r32 = r(32, 5);
    private static readonly WideMutator<BeyondDisassembler> r33 = r(33, 5);
    private static readonly WideMutator<BeyondDisassembler> r37 = r(37, 5);

    private static WideMutator<BeyondDisassembler> uimm(int bitpos, int length, PrimitiveType dt)
    {
        var field = new Bitfield(bitpos, length);
        return (u, d) =>
            {
                var imm = field.Read(u);
                d.ops.Add(Constant.Create(dt, imm));
                return true;
            };
    }
    private static readonly WideMutator<BeyondDisassembler> uimm32_0_4 = uimm(0, 4, PrimitiveType.Word32);
    private static readonly WideMutator<BeyondDisassembler> uimm32_0_8 = uimm(0, 8, PrimitiveType.Word32);
    private static readonly WideMutator<BeyondDisassembler> uimm32_3_5 = uimm(3, 5, PrimitiveType.Word32);
    private static readonly WideMutator<BeyondDisassembler> uimm32_8_4 = uimm(8, 4, PrimitiveType.Word32);
    private static readonly WideMutator<BeyondDisassembler> uimm32_13_3 = uimm(13, 3, PrimitiveType.Word32);

    private static WideMutator<BeyondDisassembler> simm(int bitpos, int length, PrimitiveType dt)
    {
        var field = new Bitfield(bitpos, length);
        return (u, d) =>
        {
            var imm = field.ReadSigned(u);
            d.ops.Add(Constant.Create(dt, imm));
            return true;
        };
    }
    private static readonly WideMutator<BeyondDisassembler> simm32_17_5 = simm(17, 5, PrimitiveType.Int32);
    private static readonly WideMutator<BeyondDisassembler> simm32_0_8 = uimm(0, 8, PrimitiveType.Int32);
    private static readonly WideMutator<BeyondDisassembler> wsimm32_0_8 = simm(0, 8, PrimitiveType.Word32);
    private static readonly WideMutator<BeyondDisassembler> wsimm32_0_16 = simm(0, 16, PrimitiveType.Word32);
    private static readonly WideMutator<BeyondDisassembler> wsimm32_17_5 = simm(17, 5, PrimitiveType.Word32);
    private static readonly WideMutator<BeyondDisassembler> wsimm32_0_32 = simm(0, 32, PrimitiveType.Word32);
    private static readonly WideMutator<BeyondDisassembler> wsimm32_33_5 = simm(33, 5, PrimitiveType.Word32);

    private static WideMutator<BeyondDisassembler> disp(int bitpos, int length)
    {
        var field = new Bitfield(bitpos, length);
        return (u, d) =>
        {
            var imm = field.ReadSigned(u);
            d.ops.Add(d.addr + imm);
            return true;
        };
    }
    private static readonly WideMutator<BeyondDisassembler> disp0_8 = disp(0, 8);
    private static readonly WideMutator<BeyondDisassembler> disp0_10 = disp(0, 10);
    private static readonly WideMutator<BeyondDisassembler> disp0_12 = disp(0, 12);
    private static readonly WideMutator<BeyondDisassembler> disp0_16 = disp(0, 16);
    private static readonly WideMutator<BeyondDisassembler> disp0_18 = disp(0, 18);
    private static readonly WideMutator<BeyondDisassembler> disp0_28 = disp(0, 28);
    private static readonly WideMutator<BeyondDisassembler> disp0_32 = disp(0, 32);

    private static bool abs0_32(ulong uInstr, BeyondDisassembler dasm)
    {
        var addr = Address.Ptr32((uint) uInstr);
        dasm.ops.Add(addr);
        return true;
    }

    private static WideMutator<BeyondDisassembler> MemBaseSignedOffset(int baseReg, int offset, int shift)
    {
        var fieldBase = new Bitfield(baseReg, 5);
        var fieldOffset = new Bitfield(0, offset);
        return (u, d) =>
        {
            var baseReg = fieldBase.Read(u);
            var offset = (int) fieldOffset.ReadSigned(u);
            d.ops.Add(new MemoryOperand(Registers.GpRegs[baseReg], offset));
            return true;
        };
    }
    private static readonly WideMutator<BeyondDisassembler> Mbs32_29_sh3 = MemBaseSignedOffset(32, 29, 3);
    private static readonly WideMutator<BeyondDisassembler> Mbs32_30_sh2 = MemBaseSignedOffset(32, 30, 2);
    private static readonly WideMutator<BeyondDisassembler> Mbs13_8 = MemBaseSignedOffset(13, 8, 0);
    private static readonly WideMutator<BeyondDisassembler> Mbs13_8_sh2 = MemBaseSignedOffset(13, 8, 2);
    private static readonly WideMutator<BeyondDisassembler> Mbs32_32 = MemBaseSignedOffset(32, 32, 0);

    private static WideMutator<BeyondDisassembler> MemBaseUnsignedOffset(int baseReg, int offset, int shift)
    {
        var fieldBase = new Bitfield(baseReg, 5);
        var fieldOffset = new Bitfield(0, offset);
        return (u, d) =>
        {
            var baseReg = fieldBase.Read(u);
            var offset = (int) fieldOffset.Read(u);
            d.ops.Add(new MemoryOperand(Registers.GpRegs[baseReg], offset));
            return true;
        };
    }

    private static readonly WideMutator<BeyondDisassembler> Mbu8_5_sh3 = MemBaseUnsignedOffset(8, 5, 3);
    private static readonly WideMutator<BeyondDisassembler> Mbu8_6_sh3 = MemBaseUnsignedOffset(8, 6, 3);

    private static bool Mabs0_32(ulong uInstr, BeyondDisassembler dasm)
    {
        var mem = new MemoryOperand(null, (int) uInstr);
        dasm.ops.Add(mem);
        return true;
    }

    private static WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> Instr(
        Mnemonic instr,
        InstrClass iclass,
        params WideMutator<BeyondDisassembler>[] mutators)
    {
        return new WideInstrDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction>(iclass, instr, mutators);
    }

    private static WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> Instr(
        Mnemonic instr,
        params WideMutator<BeyondDisassembler>[] mutators)
    {
        return new WideInstrDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction>(InstrClass.Linear, instr, mutators);
    }

    private static WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> Instr24(
        Mnemonic mnemonic,
        InstrClass iclass,
        params WideMutator<BeyondDisassembler>[] mutators)
    {
        return new Extend24Decoder(Instr(mnemonic, iclass, mutators));
    }


    private static WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> Instr24(
        Mnemonic instr,
        params WideMutator<BeyondDisassembler>[] mutators)
    {
        return new Extend24Decoder(Instr(instr, InstrClass.Linear, mutators));
    }

    private static WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> Widen24(
        WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> decoder)
    {
        return new Extend24Decoder(decoder);
    }

    private static WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> Widen32(
        WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> decoder)
    {
        return new Extend32Decoder(decoder);
    }

    private static WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> Widen48(
      WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> decoder)
    {
        return new Extend48Decoder(decoder);
    }

    private class Extend24Decoder : WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction>
    {
        private readonly WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> decoder;

        public Extend24Decoder(WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> decoder)
        {
            this.decoder = decoder;
        }

        public override BeyondInstruction Decode(ulong ulInstr, BeyondDisassembler dasm)
        {
            if (!dasm.rdr.TryReadByte(out byte lsb))
                return dasm.CreateInvalidInstruction();
            ulInstr = (ulInstr << 8) | lsb;
            return decoder.Decode(ulInstr, dasm);
        }
    }

    private class Extend32Decoder : WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction>
    {
        private readonly WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> decoder;

        public Extend32Decoder(WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> decoder)
        {
            this.decoder = decoder;
        }

        public override BeyondInstruction Decode(ulong ulInstr, BeyondDisassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt16(out ushort lsb))
                return dasm.CreateInvalidInstruction();
            ulInstr = (ulInstr << 16) | lsb;
            return decoder.Decode(ulInstr, dasm);
        }
    }

    private class Extend48Decoder : WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction>
    {
        private readonly WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> decoder;

        public Extend48Decoder(WideDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction> decoder)
        {
            this.decoder = decoder;
        }

        public override BeyondInstruction Decode(ulong ulInstr, BeyondDisassembler dasm)
        {
            if (!dasm.rdr.TryReadUInt32(out uint lsw))
                return dasm.CreateInvalidInstruction();
            ulInstr = (ulInstr << 32) | lsw;
            return decoder.Decode(ulInstr, dasm);
        }
    }


    static BeyondDisassembler()
    {
        var nyi = new WideNyiDecoder<BeyondDisassembler, Mnemonic, BeyondInstruction>("nyi");
        var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

        rootDecoder = WideSparse(10, 6, "BA2 Beyond", nyi,
            (0b000000, WideMask(4, 1, "  000000",
                WideSelect((5, 5), u => u == 0, "  0",
                    Instr(Mnemonic.bt_trap, uimm32_0_4),
                    Instr(Mnemonic.bt_movi, r5, uimm32_0_4)),
                WideSelect((5, 5), u => u == 0, "  0",
                    Instr(Mnemonic.bt_nop, InstrClass.Linear|InstrClass.Padding),
                    Instr(Mnemonic.bt_addi, r5, uimm32_0_4)))), // manual unclear if signed or not. Alias nop.
            (0b000001, WideSelect((5, 5), u => u == 0, "  000001", 
                WideSparse(0, 5, "  0", nyi,
                    (0b00001, Instr(Mnemonic.b_ei)),
                    (0b00010, Instr(Mnemonic.b_di))),
                Instr(Mnemonic.bt_mov, r5, r0))),
            (0b000010, Instr(Mnemonic.bt_add, r5, r0)),
            (0b000011, Instr(Mnemonic.bt_j, disp0_10)),
            (0b000110, Widen24(nyi)),
            (0b001000, Instr24(Mnemonic.bn_sb, Mbs13_8, r13)),
            (0b001001, Instr24(Mnemonic.bn_lbz, r13, Mbs13_8)),
            (0b001010, Instr24(Mnemonic.bn_sb, Mbs13_8, r13)),
            (0b001011, Widen24(WideMask(5, 3, "  001011",
                Instr(Mnemonic.bn_sw, Mbu8_6_sh3, r13),
                Instr(Mnemonic.bn_sw, Mbu8_6_sh3, r13),
                Instr(Mnemonic.bn_lwz, r13, Mbu8_6_sh3),
                Instr(Mnemonic.bn_lwz, r13, Mbu8_6_sh3),
                Instr(Mnemonic.bn_lws, r13, Mbu8_6_sh3),
                Instr(Mnemonic.bn_lws, r13, Mbu8_6_sh3),
                Instr(Mnemonic.bn_sd, Mbu8_5_sh3, r13),
                Instr(Mnemonic.bn_ld, r13, Mbu8_5_sh3)))),
            (0b001100, Instr24(Mnemonic.bn_addi, r13, r8, wsimm32_0_8)),
            (0b001101, Instr24(Mnemonic.bn_andi, r13, r8, wsimm32_0_8)),
            (0b001110, Instr24(Mnemonic.bn_ori, r13, r8, uimm32_0_8)),
            (0b001111, Widen24(WideMask(16, 2, "  001111",
                WideSparse(13, 3, "  00", nyi,
                    (0b000, Instr(Mnemonic.bn_sfeqi, r8, wsimm32_0_8)),
                    (0b001, Instr(Mnemonic.bn_sfnei, r8, wsimm32_0_8)),
                    (0b100, Instr(Mnemonic.bn_sfgtsi, r8, simm32_0_8)),
                    (0b101, Instr(Mnemonic.bn_sfgtui, r8, wsimm32_0_8))),
                nyi,
                WideMask(13, 1, "  10",
                    WideMask(0, 3, "  0",
                        Instr(Mnemonic.bn_extbz, r3, r8),
                        Instr(Mnemonic.bn_extbs, r3, r8),
                        Instr(Mnemonic.bn_exthz, r3, r8),
                        Instr(Mnemonic.bn_exths, r3, r8),
                        Instr(Mnemonic.bn_ff1, r3, r8),
                        Instr(Mnemonic.bn_clz, r3, r8),
                        Instr(Mnemonic.bn_bitrev, r3, r8),
                        nyi),
                    nyi),
                nyi))),
            // 01000110
            (0b010000, Widen24(WideMask(16, 2, "  010000",
                Instr(Mnemonic.bn_beqi, InstrClass.ConditionalTransfer, r8, uimm32_13_3, disp0_8),
                Instr(Mnemonic.bn_bnei, InstrClass.ConditionalTransfer, r8, uimm32_13_3, disp0_8),
                Instr(Mnemonic.bn_bgesi, InstrClass.ConditionalTransfer, r8, uimm32_13_3, disp0_8),
                Instr(Mnemonic.bn_bgtsi, InstrClass.ConditionalTransfer, r8, uimm32_13_3, disp0_8)
                ))),
            (0b010001, Widen24(WideSparse(16, 2, "  010001", nyi,
                (0b00, Instr(Mnemonic.bn_blesi, r8, uimm32_13_3, disp0_8)),
                (0b01, Instr(Mnemonic.bn_bltsi, r8, uimm32_13_3, disp0_8)),
                (0b10, Instr(Mnemonic.bn_j, disp0_16)),
                (0b11, WideSparse(12, 4, "  11", nyi,
                    (0b0010, Instr(Mnemonic.bn_bf, InstrClass.ConditionalTransfer, disp0_12)),
                    (0b0011, Instr(Mnemonic.bn_bnf, InstrClass.ConditionalTransfer, disp0_12)),
                    (0b0100, Instr(Mnemonic.bn_bo, InstrClass.ConditionalTransfer, disp0_12)),
                    (0b0101, Instr(Mnemonic.bn_bno, InstrClass.ConditionalTransfer, disp0_12)),
                    (0b0110, Instr(Mnemonic.bn_bc, InstrClass.ConditionalTransfer, disp0_12)),
                    (0b0111, Instr(Mnemonic.bn_bnc, InstrClass.ConditionalTransfer, disp0_12)),
                    (0b1010, Instr(Mnemonic.bn_entri, uimm32_8_4, uimm32_0_8)),
                    (0b1011, Instr(Mnemonic.bn_reti, uimm32_8_4, uimm32_0_8)),
                    (0b1100, Instr(Mnemonic.bn_rtnei, uimm32_8_4, uimm32_0_8)),
                    (0b1101, WideMask(8, 2, "  01000111_1101",
                        Instr(Mnemonic.bn_return, InstrClass.Transfer | InstrClass.Return),
                        Instr(Mnemonic.bn_jalr, InstrClass.Transfer | InstrClass.Call, r3),
                        Instr(Mnemonic.bn_jr, InstrClass.Transfer, r3),
                        nyi))))))),
            (0b010010, Instr24(Mnemonic.bn_jal, disp0_18)),
            (0b011000, Widen24(WideMask(0, 3, "  011000",
                Instr(Mnemonic.bn_and, r13, r8, r3),
                Instr(Mnemonic.bn_or, r13, r8, r3),
                Instr(Mnemonic.bn_xor, r13, r8, r3),
                Instr(Mnemonic.bn_nand, r13, r8, r3),           // b.not alias
                Instr(Mnemonic.bn_add, r13, r8, r3),
                Instr(Mnemonic.bn_sub, r13, r8, r3),
                Instr(Mnemonic.bn_sll, r13, r8, r3),
                Instr(Mnemonic.bn_srl, r13, r8, r3)
                ))),
            (0b011001, Widen24(WideMask(0, 3, "  011001",
                Instr(Mnemonic.bn_sra, r13, r8, r3),
                Instr(Mnemonic.bn_ror, r13, r8, r3),
                Instr(Mnemonic.bn_cmov, r13, r8, r3),
                Instr(Mnemonic.bn_mul, r13, r8, r3),            // signed multiply
                Instr(Mnemonic.bn_div, r13, r8, r3),            // signed divide
                Instr(Mnemonic.bn_divu, r13, r8, r3),           // unsigned divide
                WideSparse(8, 5, "  110", nyi,
                    (0b00000, Instr(Mnemonic.bn_mac, r8, r3)),
                    (0b00001, Instr(Mnemonic.bn_macs, r8, r3))),
                Instr(Mnemonic.bn_addc, r13, r8, r3)))),
            (0b011010, Widen24(WideMask(0, 3, "  011010",
                Instr(Mnemonic.bn_subb, r13, r8, r3),
                Instr(Mnemonic.bn_flb, r13, r8, r3),
                nyi,
                Instr(Mnemonic.bn_mulh, r13, r8, r3),
                Instr(Mnemonic.bn_mod, r13, r8, r3),
                Instr(Mnemonic.bn_modu, r13, r8, r3),
                Instr(Mnemonic.bn_aadd, r13, r8, r3),
                Instr(Mnemonic.bn_cmpxchg, r13, r8, r3)))),
            (0b011011, Widen24(WideMask(0, 2, "  011011",
                Instr(Mnemonic.bn_slli, r13, r8, uimm32_3_5),
                Instr(Mnemonic.bn_srli, r13, r8, uimm32_3_5),
                Instr(Mnemonic.bn_srai, r13, r8, uimm32_3_5),
                Instr(Mnemonic.bn_rori, r13, r8, uimm32_3_5)))),
            (0b011100, Widen24(WideMask(0, 3, "  011100",
                Instr(Mnemonic.fb_add_s, r13, r8, r3),
                Instr(Mnemonic.fb_sub_s, r13, r8, r3),
                Instr(Mnemonic.fb_mul_s, r13, r8, r3),
                Instr(Mnemonic.fb_div_s, r13, r8, r3),
                nyi,
                nyi,
                nyi,
                nyi))),
            (0b011101, Widen24(WideMask(0, 3, "  011101",
                Instr(Mnemonic.bn_adds, r13, r8, r3),
                Instr(Mnemonic.bn_subs, r13, r8, r3),
                nyi,
                nyi,
                nyi,
                nyi,
                nyi,
                nyi))),
            (0b100001, Widen48(Instr(Mnemonic.bw_lbz, r37, Mbs32_32))),
            (0b100011, Widen48(WideSparse(29, 3, "  100011", nyi,
                (0b000, Instr(Mnemonic.bw_sw, r37, Mbs32_30_sh2)),
                (0b001, Instr(Mnemonic.bw_sw, r37, Mbs32_30_sh2)),
                (0b110, Instr(Mnemonic.bw_ld, r37, Mbs32_29_sh3)),
                (0b111, Instr(Mnemonic.bw_ld, r37, Mbs32_29_sh3))))),
            (0b100100, Widen48(Instr(Mnemonic.bw_addi, r37, r32, wsimm32_0_32))),
            (0b100101, Widen48(Instr(Mnemonic.bw_andi, r37, r32, wsimm32_0_32))),
            (0b100110, Widen48(Instr(Mnemonic.bw_ori, r37, r32, wsimm32_0_32))),
            (0b100111, Widen48(WideSparse(38, 4, "  100111", nyi,
                (0b0110, Instr(Mnemonic.bw_sfeqi, r32, wsimm32_0_32)),
                (0b0111, Instr(Mnemonic.bw_sfnei, r32, wsimm32_0_32)),
                (0b1000, Instr(Mnemonic.bw_sfgesi, r32, wsimm32_0_32)),
                (0b1001, Instr(Mnemonic.bw_sfgeui, r32, wsimm32_0_32)),
                (0b1010, Instr(Mnemonic.bw_sfgtsi, r32, wsimm32_0_32)),
                (0b1011, Instr(Mnemonic.bw_sfgtui, r32, wsimm32_0_32)),
                (0b1100, Instr(Mnemonic.bw_sflesi, r32, wsimm32_0_32)),
                (0b1101, Instr(Mnemonic.bw_sfleui, r32, wsimm32_0_32)),
                (0b1110, Instr(Mnemonic.bw_sfltsi, r32, wsimm32_0_32)),
                (0b1111, Instr(Mnemonic.bw_sfltui, r32, wsimm32_0_32))))),


            (0b101000, Widen48(WideSparse(38, 4, "  101000", nyi,
                (0b0000, Instr(Mnemonic.bw_beqi, InstrClass.Conditional, r28, wsimm32_33_5, disp0_28))))),
            (0b101001, Widen48(WideSparse(38, 4, "  101000", nyi,

                (0b0000, Instr(Mnemonic.bw_jal, InstrClass.Transfer | InstrClass.Call, disp0_32)),
                (0b0001, Instr(Mnemonic.bw_j, InstrClass.Transfer, disp0_32)),
                (0b0010, Instr(Mnemonic.bw_bf, InstrClass.ConditionalTransfer, disp0_32)),
                (0b0011, Instr(Mnemonic.bw_bnf, InstrClass.ConditionalTransfer, disp0_32)),
                (0b0100, Instr(Mnemonic.bw_ja, InstrClass.Transfer, abs0_32)),
                (0b0101, WideMask(32, 1, "  0110", 
                    Instr(Mnemonic.bw_jma, InstrClass.Transfer, Mabs0_32),
                    Instr(Mnemonic.bw_jmal, InstrClass.Transfer | InstrClass.Call, Mabs0_32))),
                (0b0110, WideMask(32, 1, "  0110",
                    Instr(Mnemonic.bw_lma, r33,Mabs0_32),
                    Instr(Mnemonic.bw_sma, Mabs0_32, r33)))))),

            (0b110100, Widen32(WideSparse(22, 4, "  110100", nyi,
                (0b0000, Instr(Mnemonic.bg_beqi, InstrClass.ConditionalTransfer, r12, wsimm32_17_5, disp0_12)),
                (0b0001, Instr(Mnemonic.bg_bnei, InstrClass.ConditionalTransfer, r12, wsimm32_17_5, disp0_12)),
                (0b0011, Instr(Mnemonic.bg_bgtsi, InstrClass.ConditionalTransfer, r12, simm32_17_5, disp0_12)),
                (0b0100, Instr(Mnemonic.bg_bgeui, InstrClass.ConditionalTransfer, r12, simm32_17_5, disp0_12)),
                (0b1000, Instr(Mnemonic.bg_bleui, InstrClass.ConditionalTransfer, r12, simm32_17_5, disp0_12)),
                (0b1001, Instr(Mnemonic.bg_bltui, InstrClass.ConditionalTransfer, r12, simm32_17_5, disp0_12)),
                (0b1010, Instr(Mnemonic.bg_beq, InstrClass.ConditionalTransfer, r12, r12, disp0_12)),
                (0b1011, Instr(Mnemonic.bg_bne, InstrClass.ConditionalTransfer, r12, r12, disp0_12)),
                (0b1100, Instr(Mnemonic.bg_bges, InstrClass.ConditionalTransfer, r17, r12, disp0_12)),
                (0b1101, Instr(Mnemonic.bw_bgts, InstrClass.ConditionalTransfer, r17, r12, disp0_12)),
                (0b1110, Instr(Mnemonic.bw_bgeu, InstrClass.ConditionalTransfer, r17, r12, disp0_12)),
                (0b1111, Instr(Mnemonic.bw_bgtu, InstrClass.ConditionalTransfer, r17, r12, disp0_12))))),
            (0b110110, Widen32(Instr(Mnemonic.bg_addi, r21, r16, wsimm32_0_16))));
    }
}
