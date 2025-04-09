using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;

namespace Reko.Arch.Oki.NX8_200;

public class NX8_200Disassembler : DisassemblerBase<NX8_200Instruction, Mnemonic>
{
    private static readonly Decoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction> rootDecoder;
    private static readonly Decoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction>[] prefixDecoders;
    private static readonly Bitfield bf0L7 = new Bitfield(0, 7);

    private readonly NX8_200Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;
    private bool ddSet;
    private bool? ddSetThisInstruction;

    public NX8_200Disassembler(NX8_200Architecture arch, EndianImageReader rdr)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = [];
    }

    public override NX8_200Instruction CreateInvalidInstruction()
    {
        return new NX8_200Instruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid,
        };
    }

    public override NX8_200Instruction? DisassembleInstruction()
    {
        this.addr = rdr.Address;
        long offset = rdr.Offset;
        if (!rdr.TryReadByte(out var b))
            return null;
        ddSetThisInstruction = null;
        var instr = rootDecoder.Decode(b, this);
        ops.Clear();
        instr.Address = this.addr;
        instr.Length = (int) (rdr.Offset - offset);
        return instr;
    }

    public override NX8_200Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new NX8_200Instruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Operands = this.ops.ToArray(),
        };
    }

    public override NX8_200Instruction NotYetImplemented(string message)
    {
        var testGenSvc = arch.Services.GetService<ITestGenerationService>();
        testGenSvc?.ReportMissingDecoder("Nx8Dasm", this.addr, this.rdr, message);
        return new NX8_200Instruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.nyi,
        };
    }

    private bool IsDDflagSet()
    {
        if (this.ddSetThisInstruction.HasValue)
            return ddSetThisInstruction.Value;
        return ddSet;
    }

    private static bool ClearDDflag(uint uInstr, NX8_200Disassembler dasm)
    {
        dasm.ddSet = false;
        return true;
    }

    private static bool SetDDflag(uint uInstr, NX8_200Disassembler dasm)
    {
        dasm.ddSet = true;
        return true;
    }

    /// <summary>
    /// Operand decoder for specific registers.
    /// </summary>
    /// <param name="reg">Specific register to add to the operands list.</param>
    private static Mutator<NX8_200Disassembler> Register(RegisterStorage reg)
    {
        return (u, d) =>
        {
            d.ops.Add(reg);
            return true;
        };
    }
    private static readonly Mutator<NX8_200Disassembler> a = Register(Registers.Acc);
    private static readonly Mutator<NX8_200Disassembler> ER0 = Register(Registers.ERegisters[0]);
    private static readonly Mutator<NX8_200Disassembler> ER1 = Register(Registers.ERegisters[1]);
    private static readonly Mutator<NX8_200Disassembler> ER2 = Register(Registers.ERegisters[2]);
    private static readonly Mutator<NX8_200Disassembler> ER3 = Register(Registers.ERegisters[3]);

    private static readonly Mutator<NX8_200Disassembler> PR0 = Register(Registers.X1);
    private static readonly Mutator<NX8_200Disassembler> PR1 = Register(Registers.X2);
    private static readonly Mutator<NX8_200Disassembler> PR2 = Register(Registers.Dp);
    private static readonly Mutator<NX8_200Disassembler> PR3 = Register(Registers.Usp);

    private static readonly Mutator<NX8_200Disassembler> X1 = Register(Registers.X1);
    private static readonly Mutator<NX8_200Disassembler> X2 = Register(Registers.X2);
    private static readonly Mutator<NX8_200Disassembler> DP = Register(Registers.Dp);
    private static readonly Mutator<NX8_200Disassembler> SSP = Register(Registers.Ssp);
    private static readonly Mutator<NX8_200Disassembler> USP = Register(Registers.Usp);
    private static readonly Mutator<NX8_200Disassembler> LRB = Register(Registers.Lrb);
    private static readonly Mutator<NX8_200Disassembler> PSW = Register(Registers.Psw);
    private static readonly Mutator<NX8_200Disassembler> PSWH = Register(Registers.Pswh);
    private static readonly Mutator<NX8_200Disassembler> PSWL = Register(Registers.Pswl);
    private static readonly Mutator<NX8_200Disassembler> R0 = Register(Registers.BRegisters[0]);
    private static readonly Mutator<NX8_200Disassembler> R1 = Register(Registers.BRegisters[1]);
    private static readonly Mutator<NX8_200Disassembler> R2 = Register(Registers.BRegisters[2]);
    private static readonly Mutator<NX8_200Disassembler> R3 = Register(Registers.BRegisters[3]);
    private static readonly Mutator<NX8_200Disassembler> R4 = Register(Registers.BRegisters[4]);
    private static readonly Mutator<NX8_200Disassembler> R5 = Register(Registers.BRegisters[5]);
    private static readonly Mutator<NX8_200Disassembler> R6 = Register(Registers.BRegisters[6]);
    private static readonly Mutator<NX8_200Disassembler> R7 = Register(Registers.BRegisters[7]);

    private static Mutator<NX8_200Disassembler> rr(int index)
    {
        return (u, d) =>
        {
            var regs = d.IsDDflagSet() ? Registers.ERegisters : Registers.BRegisters;
            if ((uint)index >= (uint) regs.Length)
                return false;
            d.ops.Add(regs[index]);
            return true;
        };
    }
    private static readonly Mutator<NX8_200Disassembler> rr0 = rr(0);
    private static readonly Mutator<NX8_200Disassembler> rr1 = rr(1);
    private static readonly Mutator<NX8_200Disassembler> rr2 = rr(2);
    private static readonly Mutator<NX8_200Disassembler> rr3 = rr(3);
    private static readonly Mutator<NX8_200Disassembler> rr4 = rr(4);
    private static readonly Mutator<NX8_200Disassembler> rr5 = rr(5);
    private static readonly Mutator<NX8_200Disassembler> rr6 = rr(6);
    private static readonly Mutator<NX8_200Disassembler> rr7 = rr(7);

    private static Mutator<NX8_200Disassembler> FlagGroup(FlagGroupStorage grf)
    {
        return (u, d) =>
        {
            d.ops.Add(grf);
            return true;
        };
    }
    private static readonly Mutator<NX8_200Disassembler> c = FlagGroup(Registers.C);

    private static bool fix8(uint uInstr, NX8_200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte loFix))
            return false;
        dasm.ops.Add(MemoryOperand.Direct(Registers.Dsr, 0x200 + loFix));
        return true;
    }

    private static bool off8(uint uInstr, NX8_200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte loOff))
            return false;
        dasm.ops.Add(MemoryOperand.Direct(Registers.Dsr, 0x000 + loOff));
        return true;
    }


    private static bool sfr8(uint uInstr, NX8_200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte sfrOff))
            return false;
        dasm.ops.Add(MemoryOperand.Direct(Registers.Dsr, 0x000 + sfrOff));
        return true;
    }

    private static bool dir(uint uInstr, NX8_200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadLeUInt16(out ushort uAddr))
            return false;
        dasm.ops.Add(MemoryOperand.Direct(Registers.Dsr, uAddr));
        return true;
    }

    private static bool Dir8(uint uInstr, NX8_200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte b))
            return false;
        dasm.ops.Add(MemoryOperand.Direct(null, b));
        return true;
    }

    private static bool Dir16(uint uInstr, NX8_200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadLeUInt16(out ushort n16))
            return false;
        dasm.ops.Add(Constant.Word16(n16));
        return true;
    }

    private static bool Ndd(uint uInstr, NX8_200Disassembler dasm)
    {
        MachineOperand op;
        if (dasm.IsDDflagSet())
        {
            if (!dasm.rdr.TryReadLeUInt16(out ushort n16))
                return false;
            op = Constant.Word16(n16);
        }
        else
        {
            if (!dasm.rdr.TryReadByte(out byte n))
                return false;
            op = Constant.Byte(n);
        }
        dasm.ops.Add(op);
        return true;
    }

    private static bool Mdp_n7(uint uInstr, NX8_200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte n))
            return false;
        int offset = bf0L7.ReadSigned(uInstr);
        dasm.ops.Add(MemoryOperand.Indirect(Registers.Dsr, Registers.Dp, offset));
        return true;
    }

    private static Mutator<NX8_200Disassembler> Mindexed(RegisterStorage @base, RegisterStorage index)
    {
        return (u, d) =>
        {
            d.ops.Add(MemoryOperand.Indexed(Registers.Dsr, @base, index));
            return true;
        };
    }
    private static readonly Mutator<NX8_200Disassembler> Mx1_a = Mindexed(Registers.X1, Registers.Acc);
    private static readonly Mutator<NX8_200Disassembler> Mx1_r0 = Mindexed(Registers.X1, Registers.BRegisters[0]);

    private static Mutator<NX8_200Disassembler> Indirect(RegisterStorage seg, RegisterStorage reg)
    {
        return (u, d) =>
        {
            var mem = MemoryOperand.Indirect(seg, reg, 0);
            d.ops.Add(mem);
            return true;
        };
    }
    private static readonly Mutator<NX8_200Disassembler> Mdp = Indirect(Registers.Dsr, Registers.Dp);
    private static readonly Mutator<NX8_200Disassembler> Mx1 = Indirect(Registers.Dsr, Registers.X1);

    private static Mutator<NX8_200Disassembler> IndirectOffset8(RegisterStorage seg, RegisterStorage reg)
    {
        return (u, d) =>
        {
            if (!d.rdr.TryReadByte(out byte offset))
                return false;
            var mem = MemoryOperand.Indirect(seg, reg, (sbyte)offset);
            d.ops.Add(mem);
            return true;
        };
    }
    private static readonly Mutator<NX8_200Disassembler> Md8_usp = IndirectOffset16(Registers.Dsr, Registers.Usp);

    private static Mutator<NX8_200Disassembler> IndirectOffset16(RegisterStorage seg, RegisterStorage reg)
    {
        return (u, d) =>
        {
            if (!d.rdr.TryReadLeUInt16(out ushort offset))
                return false;
            var mem = MemoryOperand.Indirect(seg, reg, offset);
            d.ops.Add(mem);
            return true;
        };
    }
    private static readonly Mutator<NX8_200Disassembler> Md16_X1 = IndirectOffset16(Registers.Dsr, Registers.X1);
    private static readonly Mutator<NX8_200Disassembler> Md16_X2 = IndirectOffset16(Registers.Dsr, Registers.X2);


    private static bool Mdp_plus(uint uInstr, NX8_200Disassembler dasm)
    {
        var mem = MemoryOperand.PostIncremented(Registers.Dsr, Registers.Dp);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool Mdp_minus(uint uInstr, NX8_200Disassembler dasm)
    {
        var mem = MemoryOperand.PostDecremented(Registers.Dsr, Registers.Dp);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool Addr16(uint uInstr, NX8_200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadLeUInt16(out ushort addr16))
            return false;
        dasm.ops.Add(Address.Ptr16(addr16));
        return true;
    }

    private static Mutator<NX8_200Disassembler> Addr(uint uAddr)
    {
        return (u, d) =>
        {
            d.ops.Add(Address.Ptr16((ushort)uAddr));
            return true;
        };
    }

    private static bool pcdisp8(uint uInstr, NX8_200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte b))
            return false;
        var target = dasm.rdr.Address + (sbyte)b;
        dasm.ops.Add(target);
        return true;
    }

    private static Mutator<NX8_200Disassembler> Imm(byte n)
    {
        return (u, d) =>
        {
            var imm = Constant.Byte(n);
            d.ops.Add(imm);
            return true;
        };
    }
    private static readonly Mutator<NX8_200Disassembler> imm1 = Imm(1);
    private static readonly Mutator<NX8_200Disassembler> imm2 = Imm(2);
    private static readonly Mutator<NX8_200Disassembler> imm3 = Imm(3);
    private static readonly Mutator<NX8_200Disassembler> imm4 = Imm(4);

    private static bool imm8(uint uInstr, NX8_200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte b))
            return false;
        var imm = Constant.Byte(b);
        dasm.ops.Add(imm);
        return true;
    }

    private static bool imm16(uint uInstr, NX8_200Disassembler dasm)
    {
        if (!dasm.rdr.TryReadLeUInt16(out ushort imm16))
            return false;
        var imm = Constant.Word16(imm16);
        dasm.ops.Add(imm);
        return true;
    }

    private static bool ClearOperands(uint uInstr, NX8_200Disassembler dasm)
    {
        dasm.ops.Clear();
        return true;
    }


    private static bool SwapOperands(uint uInstr, NX8_200Disassembler dasm)
    {
        var t = dasm.ops[1];
        dasm.ops[1] = dasm.ops[0];
        dasm.ops[0] = t;
        return true;
    }

    private static Mutator<NX8_200Disassembler> Bit(int n)
    {
        return (u, d) =>
        {
            var bo = new BitOperand(d.ops[^1], n);
            d.ops[^1] = bo;
            return true;
        };
    }
    private static readonly Mutator<NX8_200Disassembler> bit0 = Bit(0);
    private static readonly Mutator<NX8_200Disassembler> bit1 = Bit(1);
    private static readonly Mutator<NX8_200Disassembler> bit2 = Bit(2);
    private static readonly Mutator<NX8_200Disassembler> bit3 = Bit(3);
    private static readonly Mutator<NX8_200Disassembler> bit4 = Bit(4);
    private static readonly Mutator<NX8_200Disassembler> bit5 = Bit(5);
    private static readonly Mutator<NX8_200Disassembler> bit6 = Bit(6);
    private static readonly Mutator<NX8_200Disassembler> bit7 = Bit(7);

    private static Decoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<NX8_200Disassembler>[] mutators)
    {
        return new InstrDecoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction>(iclass, mnemonic, mutators);
    }

    private static Decoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction> Instr(Mnemonic mnemonic, params Mutator<NX8_200Disassembler>[] mutators)
    {
        return new InstrDecoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction>(InstrClass.Linear, mnemonic, mutators);
    }

    /// <summary>
    /// Creates an instruction decoder that is sensitive to the state of the
    /// DD status bit.
    /// </summary>
    /// <param name="mnemonicB">Mnemonic to use when DD=0</param>
    /// <param name="mnemonicW">Mnemonic to use when DD=1</param>
    /// <param name="mutators">Operand decoders</param>
    /// <returns>An instance of <see cref="DdInstrDecoder"/></returns>
    private static Decoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction> InstrDD(Mnemonic mnemonicB, Mnemonic mnemonicW,  params Mutator<NX8_200Disassembler>[] mutators)
    {
        return new DdInstrDecoder(mnemonicB, mnemonicW, InstrClass.Linear, mutators);
    }

    private static Decoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction> InstrDD(Mnemonic mnemonicB, Mnemonic mnemonicW, InstrClass iclass, params Mutator<NX8_200Disassembler>[] mutators)
    {
        return new DdInstrDecoder(mnemonicB, mnemonicW, iclass, mutators);
    }


    private static Decoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction> Prefix(Mutator<NX8_200Disassembler> mutator)
    {
        return new PrefixDecoder(mutator, null);
    }

    private static Decoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction> BPrefix(Mutator<NX8_200Disassembler> mutator)
    {
        return new PrefixDecoder(mutator, false);
    }

    private static Decoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction> WPrefix(Mutator<NX8_200Disassembler> mutator)
    {
        return new PrefixDecoder(mutator, true);
    }


    private class DdInstrDecoder : Decoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction>
    {
        private readonly Mnemonic mnemonicB;
        private readonly Mnemonic mnemonicW;
        private readonly InstrClass iclass;
        private readonly Mutator<NX8_200Disassembler>[] mutators;

        internal DdInstrDecoder(Mnemonic mnemonicB, Mnemonic mnemonicW, InstrClass iclass, Mutator<NX8_200Disassembler>[] mutators)
        {
            this.mnemonicB = mnemonicB;
            this.mnemonicW = mnemonicW;
            this.iclass = iclass;
            this.mutators = mutators;
        }

        public override NX8_200Instruction Decode(uint uInstr, NX8_200Disassembler dasm)
        {
            foreach (var m in mutators)
            {
                if (!m(uInstr, dasm))
                    return dasm.CreateInvalidInstruction();
            }
            var mnemonic = dasm.IsDDflagSet() ? mnemonicW : mnemonicB;
            return dasm.MakeInstruction(iclass, mnemonic);
        }
    }

    private class PrefixDecoder : Decoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction>
    {
        private readonly Mutator<NX8_200Disassembler> mutator;
        private readonly bool? setDd;

        public PrefixDecoder(Mutator<NX8_200Disassembler> mutator, bool? setDd)
        {
            this.mutator = mutator;
            this.setDd = setDd;
        }

        public override NX8_200Instruction Decode(uint uInstr, NX8_200Disassembler dasm)
        {
            dasm.ddSetThisInstruction = this.setDd;
            if (!mutator(uInstr, dasm))
                return dasm.CreateInvalidInstruction();
            if (!dasm.rdr.TryReadByte(out byte b))
                return dasm.CreateInvalidInstruction();
            return prefixDecoders[b].Decode(uInstr, dasm);
        }
    }

    static NX8_200Disassembler()
    {
        var nyi = new NyiDecoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction>("nyi");
        
        prefixDecoders = new Decoder<NX8_200Disassembler, Mnemonic, NX8_200Instruction>[] {
            // 0x00
            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.rb, bit0),
            Instr(Mnemonic.rb, bit1),
            Instr(Mnemonic.rb, bit2),
            Instr(Mnemonic.rb, bit3),

            Instr(Mnemonic.rb, bit4),
            Instr(Mnemonic.rb, bit5),
            Instr(Mnemonic.rb, bit6),
            Instr(Mnemonic.rb, bit7),

            // 0x10
            InstrDD(Mnemonic.xchgb, Mnemonic.xchg, a, SwapOperands),
            Instr(Mnemonic.sbr),
            Instr(Mnemonic.rbr),
            Instr(Mnemonic.tbr),

            nyi,
            Instr(Mnemonic.clr),
            InstrDD(Mnemonic.inc, Mnemonic.incb),
            InstrDD(Mnemonic.dec, Mnemonic.decb),

            Instr(Mnemonic.sb, bit0),
            Instr(Mnemonic.sb, bit1),
            Instr(Mnemonic.sb, bit2),
            Instr(Mnemonic.sb, bit3),

            Instr(Mnemonic.sb, bit4),
            Instr(Mnemonic.sb, bit5),
            Instr(Mnemonic.sb, bit6),
            Instr(Mnemonic.sb, bit7),

            // 0x20
            Instr(Mnemonic.mbr, c),
            Instr(Mnemonic.mbr, c, SwapOperands),
            Instr(Mnemonic.j),
            Instr(Mnemonic.cal),

            nyi,
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.mb, bit0, c, SwapOperands),
            Instr(Mnemonic.mb, bit1, c, SwapOperands),
            Instr(Mnemonic.mb, bit2, c, SwapOperands),
            Instr(Mnemonic.mb, bit3, c, SwapOperands),

            Instr(Mnemonic.mb, bit4, c, SwapOperands),
            Instr(Mnemonic.mb, bit5, c, SwapOperands),
            Instr(Mnemonic.mb, bit6, c, SwapOperands),
            Instr(Mnemonic.mb, bit7, c, SwapOperands),

            // 0x30
            nyi,
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.mulb, ClearOperands),
            Instr(Mnemonic.mul, ClearOperands),
            Instr(Mnemonic.divb, ClearOperands),
            Instr(Mnemonic.div, ClearOperands),

            Instr(Mnemonic.mb, bit0),
            Instr(Mnemonic.mb, bit1),
            Instr(Mnemonic.mb, bit2),
            Instr(Mnemonic.mb, bit3),

            Instr(Mnemonic.mb, bit4),
            Instr(Mnemonic.mb, bit5),
            Instr(Mnemonic.mb, bit6),
            Instr(Mnemonic.mb, bit7),

            // 0x40
            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.mov, rr0),
            Instr(Mnemonic.mov, rr1),
            Instr(Mnemonic.mov, rr2),
            Instr(Mnemonic.mov, rr3),

            Instr(Mnemonic.mov, rr4),
            Instr(Mnemonic.mov, rr5),
            Instr(Mnemonic.mov, rr6),
            Instr(Mnemonic.mov, rr7),

            // 0x50
            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0x60
            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0x70
            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.mov, X1, SwapOperands),
            Instr(Mnemonic.mov, X2, SwapOperands),
            Instr(Mnemonic.mov, DP, SwapOperands),
            Instr(Mnemonic.mov, USP, SwapOperands),

            Instr(Mnemonic.mov, off8, SwapOperands),
            Instr(Mnemonic.mov, PSW, SwapOperands),
            Instr(Mnemonic.mov, SSP, SwapOperands),
            Instr(Mnemonic.mov, LRB, SwapOperands),

            // 0x80
            InstrDD(Mnemonic.addb, Mnemonic.add, Ndd),
            InstrDD(Mnemonic.addb, Mnemonic.add, a),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, SwapOperands),
            InstrDD(Mnemonic.addb, Mnemonic.add, off8),

            nyi,
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.mov, PSWL, SwapOperands),
            Instr(Mnemonic.mov, PSWH, SwapOperands),
            Instr(Mnemonic.mov, a),
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0x90
            nyi,
            nyi,
            InstrDD(Mnemonic.adcb, Mnemonic.adc, a, SwapOperands),
            nyi,

            nyi,
            InstrDD(Mnemonic.movb, Mnemonic.mov, a, SwapOperands),
            nyi,
            nyi,

            InstrDD(Mnemonic.movb, Mnemonic.mov, Ndd),
            InstrDD(Mnemonic.movb, Mnemonic.mov, a, SwapOperands),
            nyi,
            nyi,

            Instr(Mnemonic.lc, ClearOperands, a, Dir16),
            Instr(Mnemonic.lcb, ClearOperands, a, Dir16),
            Instr(Mnemonic.cmpc, ClearOperands, a, Dir16),
            nyi,

            // 0xA0
            InstrDD(Mnemonic.subb, Mnemonic.sub, Ndd),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, SwapOperands),
            InstrDD(Mnemonic.subb, Mnemonic.sub, off8),

            nyi,
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.lc, a, SwapOperands),
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.cmpc, a, SwapOperands),
            nyi,        //$TODO CMPC -- offset
            nyi,
            nyi,

            // 0xB0
            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, Ndd),
            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, a),
            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, a, SwapOperands),
            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, off8, SwapOperands),

            InstrDD(Mnemonic.rolb, Mnemonic.rol),
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0xC0
            InstrDD(Mnemonic.cmpb, Mnemonic.cmp, Ndd),
            InstrDD(Mnemonic.cmpb, Mnemonic.cmp, a),
            InstrDD(Mnemonic.cmpb, Mnemonic.cmp, a, SwapOperands),
            InstrDD(Mnemonic.cmpb, Mnemonic.cmp, off8),

            nyi,
            nyi,
            nyi,
            InstrDD(Mnemonic.rorb, Mnemonic.ror),

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0xD0
            InstrDD(Mnemonic.andb, Mnemonic.and, Ndd),
            InstrDD(Mnemonic.andb, Mnemonic.and, a),
            InstrDD(Mnemonic.andb, Mnemonic.and, a, SwapOperands),
            InstrDD(Mnemonic.andb, Mnemonic.and, off8),

            nyi,
            nyi,
            nyi,
            InstrDD(Mnemonic.sllb, Mnemonic.sll),

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0xE0
            InstrDD(Mnemonic.orb, Mnemonic.or, Ndd),
            InstrDD(Mnemonic.orb, Mnemonic.or, a),
            InstrDD(Mnemonic.orb, Mnemonic.or, a, SwapOperands),
            InstrDD(Mnemonic.orb, Mnemonic.or, off8),

            nyi,
            nyi,
            nyi,
            InstrDD(Mnemonic.srlb, Mnemonic.srl),

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0xF0
            InstrDD(Mnemonic.xorb, Mnemonic.xor, Ndd),
            InstrDD(Mnemonic.xorb, Mnemonic.xor, a),
            InstrDD(Mnemonic.xorb, Mnemonic.xor, a, SwapOperands),
            InstrDD(Mnemonic.xorb, Mnemonic.xor, off8),

            nyi,
            nyi,
            nyi,
            InstrDD(Mnemonic.srab, Mnemonic.sra),

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi
        };
        rootDecoder = Mask(0, 8, "NX8",
            // 0x00
            Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding|InstrClass.Zero),
            Instr(Mnemonic.rt, InstrClass.Transfer|InstrClass.Return),
            Instr(Mnemonic.rti, InstrClass.Transfer|InstrClass.Return),
            Instr(Mnemonic.j, Addr16),

            nyi,
            nyi,
            nyi,
            nyi,

            InstrDD(Mnemonic.addb, Mnemonic.add, a, rr0),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, rr1),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, rr2),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, rr3),

            InstrDD(Mnemonic.addb, Mnemonic.add, a, rr4),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, rr5),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, rr6),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, rr7),

            // 0x10
            Instr(Mnemonic.vcal, Addr(0x0028)),
            Instr(Mnemonic.vcal, Addr(0x002A)),
            Instr(Mnemonic.vcal, Addr(0x002C)),
            Instr(Mnemonic.vcal, Addr(0x002E)),

            Instr(Mnemonic.vcal, Addr(0x0030)),
            Instr(Mnemonic.vcal, Addr(0x0032)),
            Instr(Mnemonic.vcal, Addr(0x0034)),
            Instr(Mnemonic.vcal, Addr(0x0036)),

            InstrDD(Mnemonic.adcb, Mnemonic.adc, a, rr0),
            InstrDD(Mnemonic.adcb, Mnemonic.adc, a, rr1),
            InstrDD(Mnemonic.adcb, Mnemonic.adc, a, rr2),
            InstrDD(Mnemonic.adcb, Mnemonic.adc, a, rr3),

            InstrDD(Mnemonic.adcb, Mnemonic.adc, a, rr4),
            InstrDD(Mnemonic.adcb, Mnemonic.adc, a, rr5),
            InstrDD(Mnemonic.adcb, Mnemonic.adc, a, rr6),
            InstrDD(Mnemonic.adcb, Mnemonic.adc, a, rr7),

            // 0x20
            Prefix(rr0),
            Prefix(rr1),
            Prefix(rr2),
            Prefix(rr3),

            Prefix(rr4),
            Prefix(rr5),
            Prefix(rr6),
            Prefix(rr7),

            InstrDD(Mnemonic.subb, Mnemonic.sub, a, rr0),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, rr1),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, rr2),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, rr3),

            InstrDD(Mnemonic.subb, Mnemonic.sub, a, rr4),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, rr5),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, rr6),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, rr7),

            // 0x30
            Instr(Mnemonic.jrnz, InstrClass.ConditionalTransfer, DP, pcdisp8),
            Instr(Mnemonic.scal, InstrClass.Transfer|InstrClass.Call, pcdisp8),
            Instr(Mnemonic.cal, InstrClass.Transfer|InstrClass.Call, Addr16),
            InstrDD(Mnemonic.rolb, Mnemonic.rol, a),

            nyi,
            nyi,
            nyi,
            nyi,

            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, a, rr0),
            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, a, rr1),
            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, a, rr2),
            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, a, rr3),

            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, a, rr4),
            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, a, rr5),
            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, a, rr6),
            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, a, rr7),

            // 0x40
            Instr(Mnemonic.l, a, X1, SetDDflag),
            Instr(Mnemonic.l, a, X2, SetDDflag),
            Instr(Mnemonic.l, a, DP, SetDDflag),
            InstrDD(Mnemonic.rorb, Mnemonic.ror, a),

            Prefix(ER0),
            Prefix(ER1),
            Prefix(ER2),
            Prefix(ER3),

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0x50
            Instr(Mnemonic.mov, X1, a),
            Instr(Mnemonic.mov, X2, a),
            Instr(Mnemonic.mov, DP, a),
            InstrDD(Mnemonic.sllb, Mnemonic.sll, a),

            Instr(Mnemonic.pushs, LRB),
            Instr(Mnemonic.pushs, a),
            nyi,
            Instr(Mnemonic.mov, LRB, imm16),

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0x60
            nyi,
            nyi,
            Instr(Mnemonic.mov, DP, imm16),
            InstrDD(Mnemonic.srlb, Mnemonic.srl, a),

            Instr(Mnemonic.pops, LRB),
            Instr(Mnemonic.pops, a, SetDDflag),
            nyi,
            Instr(Mnemonic.l, a, Ndd, SetDDflag),

            Instr(Mnemonic.xor, a, ER0),
            Instr(Mnemonic.xor, a, ER1),
            Instr(Mnemonic.xor, a, ER2),
            Instr(Mnemonic.xor, a, ER3),

            nyi,
            nyi,
            nyi,
            nyi,

            // 0x70
            Instr(Mnemonic.inc, X1),
            Instr(Mnemonic.inc, X2),
            Instr(Mnemonic.inc, DP),
            InstrDD(Mnemonic.srab, Mnemonic.sra, a),

            nyi,
            nyi,
            nyi,
            Instr(Mnemonic.lb, a, imm8, ClearDDflag),

            Instr(Mnemonic.lb, a, R0, ClearDDflag),
            Instr(Mnemonic.lb, a, R1, ClearDDflag),
            Instr(Mnemonic.lb, a, R2, ClearDDflag),
            Instr(Mnemonic.lb, a, R3, ClearDDflag),

            nyi,
            nyi,
            nyi,
            nyi,

            // 0x80
            Instr(Mnemonic.dec, X1),
            Instr(Mnemonic.dec, X2),
            Instr(Mnemonic.dec, DP),
            InstrDD(Mnemonic.swapb, Mnemonic.swap),

            nyi,
            Instr(Mnemonic.sc),
            Instr(Mnemonic.add, a, Ndd),
            nyi,

            InstrDD(Mnemonic.stb, Mnemonic.st, rr0),
            InstrDD(Mnemonic.stb, Mnemonic.st, rr1),
            InstrDD(Mnemonic.stb, Mnemonic.st, rr2),
            InstrDD(Mnemonic.stb, Mnemonic.st, rr3),

            InstrDD(Mnemonic.stb, Mnemonic.st, rr4),
            InstrDD(Mnemonic.stb, Mnemonic.st, rr5),
            InstrDD(Mnemonic.stb, Mnemonic.st, rr6),
            InstrDD(Mnemonic.stb, Mnemonic.st, rr7),

            // 0x90
            WPrefix(X1),
            WPrefix(X2),
            WPrefix(DP),
            Instr(Mnemonic.daa),

            Instr(Mnemonic.das),
            Instr(Mnemonic.rc),
            InstrDD(Mnemonic.adcb, Mnemonic.adc, a, Ndd),
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0xA0
            WPrefix(SSP),
            WPrefix(USP),
            BPrefix(PSWH),
            BPrefix(PSWL),

            WPrefix(LRB),
            nyi,
            InstrDD(Mnemonic.subb, Mnemonic.sub, Ndd),
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0xB0
            WPrefix(Md16_X1),
            WPrefix(Md16_X2),
            WPrefix(Mdp),
            Instr(Mnemonic.xnbl, off8),

            nyi,
            nyi,
            InstrDD(Mnemonic.sbcb, Mnemonic.sbc, Ndd),
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0xC0
            Prefix(Md16_X1),
            Prefix(Md16_X2),
            Prefix(Mdp),
            nyi,

            nyi,
            Prefix(Dir8),
            InstrDD(Mnemonic.cmpb, Mnemonic.cmp, a, Ndd),
            nyi,

            Instr(Mnemonic.jgt, InstrClass.ConditionalTransfer,  pcdisp8),
            Instr(Mnemonic.jeq, InstrClass.ConditionalTransfer,  pcdisp8),
            Instr(Mnemonic.jlt, InstrClass.ConditionalTransfer,  pcdisp8),
            Instr(Mnemonic.sj,  InstrClass.Transfer, pcdisp8),

            nyi,
            Instr(Mnemonic.jge, InstrClass.ConditionalTransfer, pcdisp8),
            Instr(Mnemonic.jne, InstrClass.ConditionalTransfer, pcdisp8),
            Instr(Mnemonic.jle, InstrClass.ConditionalTransfer, pcdisp8),

            // 0xD0
            InstrDD(Mnemonic.stb, Mnemonic.st, a, Md16_X1),
            InstrDD(Mnemonic.stb, Mnemonic.st, a, Md16_X2),
            InstrDD(Mnemonic.stb, Mnemonic.st, a, Mdp),
            InstrDD(Mnemonic.stb, Mnemonic.st, a, Md8_usp),

            InstrDD(Mnemonic.stb, Mnemonic.st, a, off8),
            InstrDD(Mnemonic.stb, Mnemonic.st, a, Dir8),
            InstrDD(Mnemonic.andb, Mnemonic.and, a, Ndd),
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0xE0
            Instr(Mnemonic.l, a, Md16_X1, SetDDflag),
            Instr(Mnemonic.l, a, Md16_X2, SetDDflag),
            Instr(Mnemonic.l, a, Mdp, SetDDflag),
            Instr(Mnemonic.l, a, Md8_usp, SetDDflag),

            Instr(Mnemonic.l, a, off8, SetDDflag),
            Instr(Mnemonic.l, a, Dir8, SetDDflag),
            InstrDD(Mnemonic.orb, Mnemonic.or, a, Ndd),
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0xF0
            Instr(Mnemonic.lb, a, Md16_X1, ClearDDflag),
            Instr(Mnemonic.lb, a, Md16_X2, ClearDDflag),
            Instr(Mnemonic.l, a, Mdp, ClearDDflag),
            Instr(Mnemonic.lb, a, Md8_usp, ClearDDflag),

            Instr(Mnemonic.lb, a, off8, ClearDDflag),
            Instr(Mnemonic.l, a, Dir8, ClearDDflag),
            InstrDD(Mnemonic.xorb, Mnemonic.xor, a, Ndd),
            nyi,

            Instr(Mnemonic.extnd, SetDDflag),
            Instr(Mnemonic.clr, a, SetDDflag),
            Instr(Mnemonic.clrb, a, ClearDDflag),
            nyi,

            nyi,
            Instr(Mnemonic.inc, LRB),
            Instr(Mnemonic.dec, LRB),
            Instr(Mnemonic.brk, InstrClass.Terminates|InstrClass.Padding));
    }
}
