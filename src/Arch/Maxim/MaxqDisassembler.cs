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
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Maxim;

public class MaxqDisassembler : DisassemblerBase<MaxqInstruction, Mnemonic>
{
    private static readonly Decoder<MaxqDisassembler, Mnemonic, MaxqInstruction> rootDecoder;
    private static readonly Bitfield bf0l8 = new Bitfield(0, 8);
    private static readonly Bitfield bf4l4 = new Bitfield(4, 4);

    private readonly IProcessorArchitecture arch;
    private readonly Word16ImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;

    public MaxqDisassembler(MaxqArchitecture arch, EndianImageReader rdr)
    {
        this.arch = arch;
        this.rdr = (Word16ImageReader) rdr;
        this.ops = [];
    }

    public override MaxqInstruction? DisassembleInstruction()
    {
        this.ops.Clear();
        var offset = rdr.Offset;
        if (!rdr.TryReadBeUInt16(out ushort uInstr))
            return null;
        var instr = rootDecoder.Decode(uInstr, this);
        
        instr.Address = addr;
        instr.Length = (int) (rdr.Offset - offset);
        if (uInstr == 0)
            instr.InstructionClass |= InstrClass.Zero;
        return instr;
    }

    public override MaxqInstruction CreateInvalidInstruction()
    {
        return new MaxqInstruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid,
        };
    }

    public override MaxqInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new MaxqInstruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Operands = ops.ToArray(),
        };
    }

    public override MaxqInstruction NotYetImplemented(string message)
    {
        var testGenSvc = arch.Services.GetService<ITestGenerationService>();
        testGenSvc?.ReportMissingDecoder("MaxqDis", this.addr, (EndianImageReader) this.rdr, message);
        return CreateInvalidInstruction();
    }

    private static Mutator<MaxqDisassembler> Register(RegisterStorage reg)
    {
        return (u, d) =>
        {
            d.ops.Add(reg);
            return true;
        };
    }
    private static readonly Mutator<MaxqDisassembler> acc = Register(Registers.Acc);
    private static readonly Mutator<MaxqDisassembler> BP = Register(Registers.BP);
    private static readonly Mutator<MaxqDisassembler> AP = Register(Registers.AP);
    private static readonly Mutator<MaxqDisassembler> A_AP = Register(Registers.A_AP);
    private static readonly Mutator<MaxqDisassembler> APC = Register(Registers.APC);
    private static readonly Mutator<MaxqDisassembler> CKCN = Register(Registers.CKCN);
    private static readonly Mutator<MaxqDisassembler> DPC = Register(Registers.DPC);
    private static readonly Mutator<MaxqDisassembler> FP = Register(Registers.FP);
    private static readonly Mutator<MaxqDisassembler> GR  = Register(Registers.GR);
    private static readonly Mutator<MaxqDisassembler> GRH = Register(Registers.GRH);
    private static readonly Mutator<MaxqDisassembler> GRL = Register(Registers.GRL);
    private static readonly Mutator<MaxqDisassembler> GRS = Register(Registers.GRS);
    private static readonly Mutator<MaxqDisassembler> GRXL= Register(Registers.GRXL);
    private static readonly Mutator<MaxqDisassembler> IC = Register(Registers.IC);
    private static readonly Mutator<MaxqDisassembler> IP = Register(Registers.IP);
    private static readonly Mutator<MaxqDisassembler> IV = Register(Registers.IV);
    private static readonly Mutator<MaxqDisassembler> IIR = Register(Registers.IIR);
    private static readonly Mutator<MaxqDisassembler> IMR = Register(Registers.IMR);
    private static readonly Mutator<MaxqDisassembler> OFFS = Register(Registers.OFFS);
    private static readonly Mutator<MaxqDisassembler> PSF = Register(Registers.PSF);
    private static readonly Mutator<MaxqDisassembler> SC = Register(Registers.SC);
    private static readonly Mutator<MaxqDisassembler> SP = Register(Registers.SP);
    private static readonly Mutator<MaxqDisassembler> WDCN = Register(Registers.WDCN);

    private static Mutator<MaxqDisassembler> IndexedReg(RegisterStorage[] regs, int n)
    {
        var reg = regs[n];
        return (u, d) =>
        {
            d.ops.Add(reg);
            return true;
        };
    }

    private static Mutator<MaxqDisassembler> A(int n) => IndexedReg(Registers.Accumulators, n);
    private static Mutator<MaxqDisassembler> PFX(int n) => IndexedReg(Registers.Prefixes, n);
    private static Mutator<MaxqDisassembler> MDP(int n) => IndexedReg(Registers.Prefixes, n);
    private static Mutator<MaxqDisassembler> LC(int n) => IndexedReg(Registers.Prefixes, n);

    private static bool NUL(uint u, MaxqDisassembler d)
    {
        d.ops.Add(new LiteralOperand("NUL"));
        return true;
    }

    private static bool An(int n, uint u, MaxqDisassembler d)
    {
        d.ops.Add(Registers.Accumulators[n]);
        return true;
    }

    private static Decoder<MaxqDisassembler, Mnemonic, MaxqInstruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<MaxqDisassembler>[] mutators)
    {
        return new InstrDecoder<MaxqDisassembler, Mnemonic, MaxqInstruction>(iclass, mnemonic, mutators);
    }

    private static Decoder<MaxqDisassembler, Mnemonic, MaxqInstruction> Instr(Mnemonic mnemonic, params Mutator<MaxqDisassembler>[] mutators)
    {
        return new InstrDecoder<MaxqDisassembler, Mnemonic, MaxqInstruction>(InstrClass.Linear, mnemonic, mutators);
    }

    private static bool MNn(uint u, MaxqDisassembler d)
    {
        //MN[n] 1 nnnn 0NNN 8 / 16 nnnn Selects One of First 16 Registers in Module NNN;
        // where NNN = 0 to 5.Access to Second 16 Using PFX[n].
        var n = u & 0b111;
        if (n > 5)
            return false;
        d.ops.Add(new ModuleRegister(n, bf4l4.Read(u)));
        return true;
    }

    private static bool Src(uint u, MaxqDisassembler d)
    {
        if ((u & 0x8000) == 0)
        {
            // 0 kkkk kkkk 8 kkkkkkkk = Immediate (Literal) Data
            d.ops.Add(Constant.Byte((byte) u));
            return true;
        }
        if ((u & 0b1000) == 0)
        {
            //MN[n] 1 nnnn 0NNN 8 / 16 nnnn Selects One of First 16 Registers in Module NNN;
            // where NNN = 0 to 5.Access to Second 16 Using PFX[n].
            return MNn(u, d);
        }
        var src = bf0l8.Read(u);
        // Source specifiers:
        switch (src)
        {
case 0b0000_1000: return AP(u, d);          // AP             1 0000 1000 8 Accumulator Pointer
case 0b0001_1000: return APC(u, d);         // APC            1 0001 1000 8 Accumulator Pointer Control
case 0b0100_1000: return PSF(u, d);         // PSF            1 0100 1000 8 Processor Status Flag Register
case 0b0101_1000: return IC(u, d);          // IC             1 0101 1000 8 Interrupt and Control Register
case 0b0110_1000: return IMR(u, d);         // IMR            1 0110 1000 8 Interrupt Mask Register
case 0b1000_1000: return SC(u, d);          // SC             1 1000 1000 8 System Control Register
case 0b1011_1000: return IIR(u, d);         // IIR            1 1011 1000 8 Interrupt Identification Register
case 0b1110_1000: return CKCN(u, d);        // CKCN           1 1110 1000 8 Clock Control Register
case 0b1111_1000: return WDCN(u, d);        // WDCN           1 1111 1000 8 Watchdog Control Register

case 0b0000_1001: return An(0, u, d);       // A[n]           1 nnnn 1001 8/16 nnnn Selects One of 16 Accumulators
case 0b0001_1001: return An(1, u, d);
case 0b0010_1001: return An(2, u, d);
case 0b0011_1001: return An(3, u, d);
case 0b0100_1001: return An(4, u, d);
case 0b0101_1001: return An(5, u, d);
case 0b0110_1001: return An(6, u, d);
case 0b0111_1001: return An(7, u, d);
case 0b1000_1001: return An(8, u, d);
case 0b1001_1001: return An(9, u, d);
case 0b1010_1001: return An(10, u, d);
case 0b1011_1001: return An(11, u, d);
case 0b1100_1001: return An(12, u, d);
case 0b1101_1001: return An(13, u, d);
case 0b1110_1001: return An(14, u, d);
case 0b1111_1001: return An(15, u, d);

case 0b0000_1010: return acc(u, d);         // Acc            1 0000 1010 8/16 Active Accumulator = A[AP]. Update AP per APC
case 0b0001_1010: return A_AP(u, d);        // A[AP]          1 0001 1010 8/16 Active Accumulator = A[AP]. No change to AP
case 0b0000_1100: return IP(u, d);          // IP             1 0000 1100 16 Instruction Pointer
case 0b0000_1101: return MSPpostdec(u, d);  // @SP--          1 0000 1101 16 16-Bit Word @SP, Post-Decrement SP
case 0b0001_1101: return SP(u, d);          // SP             1 0001 1101 16 Stack Pointer
case 0b0010_1101: return IV(u, d);          // IV             1 0010 1101 16 Interrupt Vector
case 0b0110_1101: return LCn_src(0, u, d);  // LC[n]          1 011n 1101 16 n Selects 1 of 2 Loop Counter Registers
case 0b0111_1101: return LCn_src(1, u, d);  // LC[n]          1 011n 1101 16 n Selects 1 of 2 Loop Counter Registers
case 0b1000_1101: return MSPPostInc(u, d);  // @SPI--         1 1000 1101 16 16-bit word @SP, Post-Decrement SP, INS=0
case 0b0000_1110: return MBP_Offs(u, d);     // @BP[Offs]      1 0000 1110 8/16 Data Memory @BP[Offs]
case 0b0001_1110: return MBP_OffsPostdec(u,d);// @BP[Offs++]    1 0001 1110 8/16 Data memory @BP[Offs]; Post Increment OFFS
case 0b0010_1110: return MBP_OffsPostdec(u,d);// @BP[Offs--]    1 0010 1110 8/16 Data Memory @BP[Offs]; Post Decrement OFFS
case 0b0011_1110: return OFFS(u, d);       // OFFS           1 0011 1110 8 Frame Pointer Offset from Base Pointer (BP)
case 0b0100_1110: return DPC(u, d);        // DPC            1 0100 1110 16 Data Pointer Control Register
case 0b0101_1110: return GR(u, d);         // GR             1 0101 1110 16 General Register
case 0b0110_1110: return GRL(u, d);        // GRL            1 0110 1110 8 Low Byte of GR Register
case 0b0111_1110: return BP(u, d);         // BP             1 0111 1110 16 Frame Pointer Base Pointer (BP)
case 0b1000_1110: return GRS(u, d);        // GRS            1 1000 1110 16 Byte-Swapped GR Register
case 0b1001_1110: return GRH(u, d);        // GRH            1 1001 1110 8 High Byte of GR Register
case 0b1010_1110: return GRXL(u, d);       // GRXL           1 1010 1110 16 Sign Extended Low Byte of GR Register
case 0b1011_1110: return FP(u, d);         // FP             1 1011 1110 16 Frame Pointer (BP[Offs])
case 0b0000_1111: return MDPn(0, u, d);     // @DP[n]         1 0n00 1111 8/16 Data Memory @DP[n]
case 0b0100_1111: return MDPn(1, u, d);     // @DP[n]         1 0n00 1111 8/16 Data Memory @DP[n]
case 0b0001_1111: return MDPpostinc(0, u, d);   // @DP[n]++       1 0n01 1111 8/16 Data Memory @DP[n], Post-Increment DP[n]
case 0b0101_1111: return MDPpostinc(1, u, d);   // @DP[n]++       1 0n01 1111 8/16 Data Memory @DP[n], Post-Increment DP[n]
case 0b0010_1111: return MDPpostdec(0, u, d);   // @DP[n]--       1 0n10 1111 8/16 Data Memory @DP[n], Post-Decrement DP[n]
case 0b0110_1111: return MDPpostdec(1, u, d);   // @DP[n]--       1 0n10 1111 8/16 Data Memory @DP[n], Post-Decrement DP[n]
case 0b0011_1111: return DPn(0, u, d);       // DP[n]          1 0n11 1111 16 n Selects 1 of 2 Data Pointer
case 0b0111_1111: return DPn(1, u, d);       // DP[n]          1 0n11 1111 16 n Selects 1 of 2 Data Pointer

        }
        return false;
    }

    private static bool DPn(int v, uint u, MaxqDisassembler d)
    {
        throw new NotImplementedException();
    }

    private static bool MDPpostdec(int v, uint u, MaxqDisassembler d)
    {
        throw new NotImplementedException();
    }

    private static bool MDPpostinc(int v, uint u, MaxqDisassembler d)
    {
        throw new NotImplementedException();
    }

    private static bool MDPn(int v, uint u, MaxqDisassembler d)
    {
        throw new NotImplementedException();
    }

    private static bool MSPpostdec(uint u, MaxqDisassembler d)
    {
        throw new NotImplementedException();
    }

    private static bool LCn_src(int v, uint u, MaxqDisassembler d)
    {
        throw new NotImplementedException();
    }

    private static bool MSPPostInc(uint u, MaxqDisassembler d)
    {
        throw new NotImplementedException();
    }

    private static bool MBP_Offs(uint u, MaxqDisassembler d)
    {
        var mem = MemoryOperand.Create(PrimitiveType.Byte, Registers.BP, Registers.OFFS, IncrementMode.None);
        d.ops.Add(mem);
        return true;
    }

    private static bool MBP_preincOffs(uint u, MaxqDisassembler d)
    {
        var mem = MemoryOperand.Create(PrimitiveType.Byte, Registers.BP, Registers.OFFS, IncrementMode.PreIncrement);
        d.ops.Add(mem);
        return true;
    }

    private static bool MBP_predecOffs(uint u, MaxqDisassembler d)
    {
        var mem = MemoryOperand.Create(PrimitiveType.Byte, Registers.BP, Registers.OFFS, IncrementMode.PreDecrement);
        d.ops.Add(mem);
        return true;
    }

    private static bool MBP_OffsPostdec(uint u, MaxqDisassembler d)
    {
        var mem = MemoryOperand.Create(PrimitiveType.Byte, Registers.BP, Registers.OFFS, IncrementMode.PostDecrement);
        d.ops.Add(mem);
        return true;
    }

    private static Mutator<MaxqDisassembler> MpreincDP(int v)
    {
        return (u, d) =>
        {
            var reg = Registers.DataPointers[v];
            var mem = MemoryOperand.Create(PrimitiveType.UInt16, reg, 0, IncrementMode.PreIncrement);
            d.ops.Add(mem);
            return true;
        };
    }

    private static Mutator<MaxqDisassembler> MpredecDP(int v)
    {
        return (u, d) =>
        {
            var reg = Registers.DataPointers[v];
            var mem = MemoryOperand.Create(PrimitiveType.UInt16, reg, 0, IncrementMode.PreIncrement);
            d.ops.Add(mem);
            return true;
        };
    }

    private static bool MpreincSP(uint uInstr, MaxqDisassembler dasm)
    {
        var reg = Registers.SP;
        var mem = MemoryOperand.Create(PrimitiveType.UInt16, reg, 0, IncrementMode.PreIncrement);
        dasm.ops.Add(mem);
        return true;
    }

    private static Mutator<MaxqDisassembler> DP(int v)
    {
        return (u, d) =>
        {
            var reg = Registers.DataPointers[v];
            d.ops.Add(reg);
            return true;
        };
    }

    static MaxqDisassembler()
    {
        var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);
        var others = Sparse(8, 8, "  others", invalid,
(0b0001_1010, Instr(Mnemonic.and, acc, Src)), // AND src Acc ← Acc AND src f001 1010 ssss ssss S, Z Y 1
(0b1001_1010, Instr(Mnemonic.and, acc, Src)), // AND src Acc ← Acc AND src f001 1010 ssss ssss S, Z Y 1
/*
(0bf010_1010, Instr(Mnemonic.or)), // OR src Acc ← Acc OR src f010 1010 ssss ssss S, Z Y 1
(0bf011_1010, Instr(Mnemonic.xor)), // XOR src Acc ← Acc XOR src f011 1010 ssss ssss S, Z Y 1
(0b1000_1010, Instr(Mnemonic.cpl)), // CPL Acc ← ~Acc 1000 1010 0001 1010 S, Z Y
*/
(0b1000_1010, Sparse(0, 8, "  1000_1010", invalid,
    (0b1001_1010, Instr(Mnemonic.neg, acc)), // NEG Acc ← ~Acc + 1 1000 1010 1001 1010 S, Z Y
    (0b0010_1010, Instr(Mnemonic.sla, acc)), // SLA Shift Acc left arithmetically               1000 1010  C, S, Z Y
    (0b0011_1010, Instr(Mnemonic.sla2, acc)), // SLA2 Shift Acc left arithmetically twice       1000 1010  C, S, Z Y
    (0b0110_1010, Instr(Mnemonic.sla4, acc)), // SLA4 Shift Acc left arithmetically four times  1000 1010  C, S, Z Y
    (0b0100_1010, Instr(Mnemonic.rl, acc)), // RL Rotate Acc left (w/o C)                       1000 1010  S Y
    (0b0101_1010, Instr(Mnemonic.rlc, acc)), // RLC Rotate Acc left (through C)                 1000 1010  C, S, Z Y
    (0b1111_1010, Instr(Mnemonic.sra, acc)), // SRA Shift Acc right arithmetically              1000 1010  C, Z Y
    (0b1110_1010, Instr(Mnemonic.sra2, acc)), // SRA2 Shift Acc right arithmetically twice      1000 1010  C, Z Y
    (0b1011_1010, Instr(Mnemonic.sra4, acc)), // SRA4 Shift Acc right arithmetically four times 1000 1010  C, Z Y
    (0b1010_1010, Instr(Mnemonic.sr, acc)), // SR Shift Acc right (0 → msbit)                   1000 1010  C, S, Z Y
    (0b1100_1010, Instr(Mnemonic.rr, acc)), // RR Rotate Acc right (w/o C)                      1000 1010  S Y
    (0b1101_1010, Instr(Mnemonic.rrc, acc)))), // RRC Rotate Acc right (though C)               1000 1010  C, S, Z Y
/*
(0b1110_1010, Instr(Mnemonic.move)), // MOVE C, Acc.<b> C ← Acc.<b> 1110 1010 bbbb 1010 C
(0b1101_1010, Instr(Mnemonic.move)), // MOVE C, #0 C ← 0 1101 1010 0000 1010 C
(0b1101_1010, Instr(Mnemonic.move)), // MOVE C, #1 C ← 1 1101 1010 0001 1010 C
(0b1101_1010, Instr(Mnemonic.cpl)), // CPL C C ← ~C 1101 1010 0010 1010 C
(0b1111_1010, Instr(Mnemonic.move)), // MOVE Acc.<b>, C Acc.<b> ← C 1111 1010 bbbb 1010 S, Z
(0b1001_1010, Instr(Mnemonic.and)), // AND Acc.<b> C ← C AND Acc.<b> 1001 1010 bbbb 1010 C
(0b1010_1010, Instr(Mnemonic.or)), // OR Acc.<b> C ← C OR Acc.<b> 1010 1010 bbbb 1010 C
(0b1011_1010, Instr(Mnemonic.xor)), // XOR Acc.<b> C ← C XOR Acc.<b> 1011 1010 bbbb 1010 C
MOVE dst.<b>, #1 dst.<b> ← 1 1ddd dddd 1bbb 0111 C, S, E, Z 2
MOVE dst.<b>, #0 dst.<b> ← 0 1ddd dddd 0bbb 0111 C, S, E, Z 2
BIT OPERATIONS
MOVE C, src.<b> C ← src.<b> fbbb 0111 ssss ssss C
*/
(0b0100_1010, Instr(Mnemonic.add, acc, Src)), // ADD src Acc ← Acc + src f100 1010 ssss ssss C, S, Z, OV Y 1
(0b1100_1010, Instr(Mnemonic.add, acc, Src)), // ADD src Acc ← Acc + src f100 1010 ssss ssss C, S, Z, OV Y 1
(0b0110_1010, Instr(Mnemonic.addc, acc, Src)), // ADDC src Acc ← Acc + (src + C) f110 1010 ssss ssss C, S, Z, OV Y 1
(0b1110_1010, Instr(Mnemonic.addc, acc, Src)), // ADDC src Acc ← Acc + (src + C) f110 1010 ssss ssss C, S, Z, OV Y 1
(0b0101_1010, Instr(Mnemonic.sub, acc, Src)), // SUB src Acc ← Acc – src f101 1010 ssss ssss C, S, Z, OV Y 1
(0b1101_1010, Instr(Mnemonic.sub, acc, Src)), // SUB src Acc ← Acc – src f101 1010 ssss ssss C, S, Z, OV Y 1
(0b0111_1010, Instr(Mnemonic.subb, acc, Src)), // SUBB src Acc ← Acc – (src + C) f111 1010 ssss ssss C, S, Z, OV Y 1
(0b1111_1010, Instr(Mnemonic.subb, acc, Src)), // SUBB src Acc ← Acc – (src + C) f111 1010 ssss ssss C, S, Z, OV Y 1
/*
{L/S}JUMP src IP ← IP + src or src f000 1100 ssss ssss 6
{L/S}JUMP C, src If C=1, IP ← (IP + src) or src f010 1100 ssss ssss 6
{L/S}JUMP NC, src If C=0, IP ← (IP + src) or src f110 1100 ssss ssss 6
{L/S}JUMP Z, src If Z=1, IP ← (IP + src) or src f001 1100 ssss ssss 6
{L/S}JUMP NZ, src If Z=0, IP ← (IP + src) or src f101 1100 ssss ssss 6
{L/S}JUMP E, src If E=1, IP ← (IP + src) or src 0011 1100 ssss ssss 6
{L/S}JUMP NE, src If E=0, IP ← (IP + src) or src 0111 1100 ssss ssss 6
{L/S}JUMP S, src If S=1, IP ← (IP + src) or src f100 1100 ssss ssss 6
{L/S}DJNZ LC[n], src If --LC[n] <> 0, IP← (IP + src) or src f10n 1101 ssss ssss 6
*/
(0b0100_1101, Instr(Mnemonic.ldjnz, InstrClass.ConditionalTransfer, LC(0), Src)),
(0b0101_1101, Instr(Mnemonic.ldjnz, InstrClass.ConditionalTransfer, LC(1), Src)),
(0b1100_1101, Instr(Mnemonic.ldjnz, InstrClass.ConditionalTransfer, LC(0), Src)),
(0b1101_1101, Instr(Mnemonic.ldjnz, InstrClass.ConditionalTransfer, LC(1), Src)),
/*
(0b0011_1101, Instr(Mnemonic.lcall, InstrClass.Transfer | InstrClass.Call, Src)), //  { L/S}CALL src @++SP ← IP+1; IP ← (IP+src) or src f011 1101 ssss ssss 6,7
(0b1011_1101, Instr(Mnemonic.scall, InstrClass.Transfer | InstrClass.Call, Src)), //  { L/S}CALL src @++SP ← IP+1; IP ← (IP+src) or src f011 1101 ssss ssss 6,7
(0b1000_1100, Instr(Mnemonic.ret, InstrClass.Transfer| InstrClass.Return)), // RET IP ← @SP-- 1000 1100 0000 1101
/*
(0b1010_1100, Instr(Mnemonic.ret)), // RET C If C=1, IP ← @SP-- 1010 1100 0000 1101
(0b1110_1100, Instr(Mnemonic.ret)), // RET NC If C=0, IP ← @SP-- 1110 1100 0000 1101
(0b1001_1100, Instr(Mnemonic.ret)), // RET Z If Z=1, IP ← @SP-- 1001 1100 0000 1101
(0b1101_1100, Instr(Mnemonic.ret)), // RET NZ If Z=0, IP ← @SP-- 1101 1100 0000 1101
(0b1100_1100, Instr(Mnemonic.ret)), // RET S If S=1, IP ← @SP-- 1100 1100 0000 1101
(0b1000_1100, Instr(Mnemonic.reti)), // RETI IP ← @SP-- ; INS← 0 1000 1100 1000 1101
(0b1010_1100, Instr(Mnemonic.reti)), // RETI C If C=1, IP ← @SP-- ; INS← 0 1010 1100 1000 1101
(0b1110_1100, Instr(Mnemonic.reti)), // RETI NC If C=0, IP ← @SP-- ; INS← 0 1110 1100 1000 1101
(0b1001_1100, Instr(Mnemonic.reti)), // RETI Z If Z=1, IP ← @SP-- ; INS← 0 1001 1100 1000 1101
(0b1101_1100, Instr(Mnemonic.reti)), // RETI NZ If Z=0, IP ← @SP-- ; INS← 0 1101 1100 1000 1101
(0b1100_1100, Instr(Mnemonic.reti)), // RETI S If S=1, IP ← @SP-- ; INS← 0 1100 1100 1000 1101
(0b1000_1010, Instr(Mnemonic.xch)), // XCH (MAXQ20 only) Swap Acc bytes 1000 1010 1000 1010 S Y
(0b1000_1010, Instr(Mnemonic.xchn)), // XCHN Swap nibbles in each Acc byte 1000 1010 0111 1010 S Y
//MOVE dst, src dst ← src fddd dddd ssss ssss C, S, Z, E (Note 8) 7, 8
(0bf000_1101, Instr(Mnemonic.push)), // PUSH src @++SP ← src f000 1101 ssss ssss 7
(0b0000_1101, Instr(Mnemonic.pop)), // POP dst dst ← @SP-- 1ddd dddd 0000 1101 C, S, Z, E 7
(0b1000_1101, Instr(Mnemonic.popi)), // POPI dst dst ← @SP-- ; INS ← 0 1ddd dddd 1000 1101 C, S, Z, E 7
*/
(0b0111_1000, Instr(Mnemonic.cmp, Src)), // CMP src E ← (Acc = src) f111 1000 ssss ssss E
(0b1111_1000, Instr(Mnemonic.cmp, Src)) // CMP src E ← (Acc = src) f111 1000 ssss ssss E
// (0b1101_1010, Instr(Mnemonic.nop)) // NOP No operation 1101 1010 0011 1010

        );
        var specificMoves = Sparse(8, 7, "  Specific moves",
            others,
        // Weird encoding. Assume that bits 14..8 are a move destination.
        // if they are not, decode using the "others" decoder.
(0b000_1000, Instr(Mnemonic.move, AP, Src)),             // 000 1000 8 Accumulator Pointer
(0b001_1000, Instr(Mnemonic.move, APC, Src)),            // 001 1000 8 Accumulator Pointer Control
(0b100_1000, Instr(Mnemonic.move, PSF, Src)),            // 100 1000 8 Processor Status Flag Register
(0b101_1000, Instr(Mnemonic.move, IC, Src)),             // 101 1000 8 Interrupt and Control Register
(0b110_1000, Instr(Mnemonic.move, IMR, Src)),            // 110 1000 8 Interrupt Mask Register

(0b000_1001, Instr(Mnemonic.move, A(0), Src)),           // nnn 1001 8/16 nnn Selects 1 of First 8 Accumulators: A[0]..A[7]
(0b001_1001, Instr(Mnemonic.move, A(1), Src)),
(0b010_1001, Instr(Mnemonic.move, A(2), Src)),
(0b011_1001, Instr(Mnemonic.move, A(3), Src)),
(0b100_1001, Instr(Mnemonic.move, A(4), Src)),
(0b101_1001, Instr(Mnemonic.move, A(5), Src)),
(0b110_1001, Instr(Mnemonic.move, A(6), Src)),
(0b111_1001, Instr(Mnemonic.move, A(7), Src)),

(0b000_1010, Instr(Mnemonic.move, acc, Src)),            // 000 1010 8/16 Active Accumulator = A[AP]

(0b000_1011, Instr(Mnemonic.move, PFX(0), Src)),         // nnn 1011 8 nnn Selects One of 8 Prefix Registers
(0b001_1011, Instr(Mnemonic.move, PFX(1), Src)),
(0b010_1011, Instr(Mnemonic.move, PFX(2), Src)),
(0b011_1011, Instr(Mnemonic.move, PFX(3), Src)),
(0b100_1011, Instr(Mnemonic.move, PFX(4), Src)),
(0b101_1011, Instr(Mnemonic.move, PFX(5), Src)),
(0b110_1011, Instr(Mnemonic.move, PFX(6), Src)),
(0b111_1011, Instr(Mnemonic.move, PFX(7), Src)),

(0b000_1101, Instr(Mnemonic.move, MpreincSP, Src)),      // 000 1101 16 16-Bit Word @SP, Pre-Increment SP
(0b001_1101, Instr(Mnemonic.move, SP, Src)),             // 001 1101 16 Stack Pointer
(0b010_1101, Instr(Mnemonic.move, IV, Src)),             // 010 1101 16 Interrupt Vector

(0b110_1101, Instr(Mnemonic.move, LC(0), Src)),          // 11n 1101 16 n Selects 1 of 2 Loop Counter Registers
(0b111_1101, Instr(Mnemonic.move, LC(1), Src)),          // 11n 1101 16 n Selects 1 of 2 Loop Counter Registers

(0b000_1110, Instr(Mnemonic.move, MBP_Offs, Src)),      // 000 1110 8/16 Data Memory @BP[Offs]
(0b001_1110, Instr(Mnemonic.move, MBP_preincOffs, Src)),    // 001 1110 8/16 Data Memory @BP[Offs]; Pre-Increment OFFS
(0b010_1110, Instr(Mnemonic.move, MBP_predecOffs, Src)),    // 010 1110 8/16 Data Memory @BP[Offs]; Pre-Decrement OFFS
(0b011_1110, Instr(Mnemonic.move, OFFS, Src)),           // 011 1110 8 Frame Pointer Offset from Base Pointer (BP)
(0b100_1110, Instr(Mnemonic.move, DPC, Src)),            // 100 1110 16 Data Pointer Control Register
(0b101_1110, Instr(Mnemonic.move, GR, Src)),             // 101 1110 16 General Register
(0b110_1110, Instr(Mnemonic.move, GRL, Src)),            // 110 1110 8 Low Byte of GR Register
(0b111_1110, Instr(Mnemonic.move, BP, Src)),             // 111 1110 16 Frame Pointer Base Pointer (BP)

(0b000_1111, Instr(Mnemonic.move, MDP(0), Src)),         // n00 1111 8/16 Data Memory @DP[n]
(0b100_1111, Instr(Mnemonic.move, MDP(1), Src)),

(0b001_1111, Instr(Mnemonic.move, MpreincDP(0), Src)),       // n01 1111 8/16 Data Memory @DP[n], Pre-Increment DP[n]
(0b101_1111, Instr(Mnemonic.move, MpreincDP(1), Src)),       // n01 1111 8/16 Data Memory @DP[n], Pre-Increment DP[n]

(0b010_1111, Instr(Mnemonic.move, MpredecDP(0), Src)),       // n10 1111 8/16 Data Memory @DP[n], Pre-Decrement DP[n]
(0b110_1111, Instr(Mnemonic.move, MpredecDP(1), Src)),       // n10 1111 8/16 Data Memory @DP[n], Pre-Decrement DP[n]

(0b011_1111, Instr(Mnemonic.move, DP(0), Src)),          // n11 1111 16 n Selects 1 of 2 Data Pointers
(0b111_1111, Instr(Mnemonic.move, DP(1), Src))           // n11 1111 16 n Selects 1 of 2 Data Pointers

//2-CYCLE DESTINATION ACCESS USING PFX[n] REGISTER (See Special Notes)
//SC             000 1000 8 System Control Register
//(0b110_1000, Instr(Mnemonic.move, CKCN, Src)),           // 110 1000 8 Clock Control Register             // Crashes b/c reg values overlap
//(0b111_1000, Instr(Mnemonic.move, WDCN, Src)),           // 111 1000 8 Watchdog Control Register

//(0b000_1001, Instr(Mnemonic.move, A(8), Src)),           // nnn 1001 16 nnn Selects 1 of Second 8 Accumulators A[8]..A[15]
//(0b001_1001, Instr(Mnemonic.move, A(9), Src)),
//(0b010_1001, Instr(Mnemonic.move, A(10), Src)),
//(0b011_1001, Instr(Mnemonic.move, A(11), Src)),
//(0b100_1001, Instr(Mnemonic.move, A(12), Src)),
//(0b101_1001, Instr(Mnemonic.move, A(13), Src)),
//(0b110_1001, Instr(Mnemonic.move, A(14), Src)),
//(0b111_1001, Instr(Mnemonic.move, A(15), Src)),

// (0b001_1110, Instr(Mnemonic.move, GRH, Src))            // 001 1110 8 High Byte of GR Register
);
        rootDecoder = Mask(11, 1, "MAXQx0",
            Sparse(8, 7, "  Dst bit 11 = 0",
                Instr(Mnemonic.move, MNn, Src),   // nnn 0NNN 8/16 nnnn Selects One of First 8 Registers in Module NNN; where
                                                    // NNN= 0 to 5. Access to Next 24 Using PFX[n].
            (0b111_0110, Instr(Mnemonic.move, NUL, Src))),      // 111 0110 8/16 Null (Virtual) Destination. Intended as a bit bucket to assist
                                                                // software with pointer increments/decrements.
            specificMoves);
    }
}