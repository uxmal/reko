using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using System.Runtime.Intrinsics.Arm;

namespace Reko.Arch.Oki;

public class NX8Disassembler : DisassemblerBase<NX8Instruction, Mnemonic>
{
    private static readonly Decoder<NX8Disassembler, Mnemonic, NX8Instruction> rootDecoder;
    private static readonly Decoder<NX8Disassembler, Mnemonic, NX8Instruction>[] prefixDecoders;
    private static readonly Bitfield bf0L7 = new Bitfield(0, 7);

    private readonly NX8Architecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;
    private bool ddSet;
    private bool? ddSetThisInstruction;

    public NX8Disassembler(NX8Architecture arch, EndianImageReader rdr)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = [];
    }

    public override NX8Instruction CreateInvalidInstruction()
    {
        return new NX8Instruction
        {
            InstructionClass = InstrClass.Invalid,
            Mnemonic = Mnemonic.Invalid,
        };
    }

    public override NX8Instruction? DisassembleInstruction()
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

    public override NX8Instruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new NX8Instruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Operands = this.ops.ToArray(),
        };
    }

    public override NX8Instruction NotYetImplemented(string message)
    {
        var testGenSvc = arch.Services.GetService<ITestGenerationService>();
        testGenSvc?.ReportMissingDecoder("Nx8Dasm", this.addr, this.rdr, message);
        return new NX8Instruction
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

    private static bool ClearDDflag(uint uInstr, NX8Disassembler dasm)
    {
        dasm.ddSet = false;
        return true;
    }

    private static bool SetDDflag(uint uInstr, NX8Disassembler dasm)
    {
        dasm.ddSet = true;
        return true;
    }

    /// <summary>
    /// Operand decoder for specific registers.
    /// </summary>
    /// <param name="reg">Specific register to add to the operands list.</param>
    private static Mutator<NX8Disassembler> Register(RegisterStorage reg)
    {
        return (u, d) =>
        {
            d.ops.Add(reg);
            return true;
        };
    }
    private static readonly Mutator<NX8Disassembler> a = Register(Registers.Acc);
    private static readonly Mutator<NX8Disassembler> ER0 = Register(Registers.ERegisters[0]);
    private static readonly Mutator<NX8Disassembler> ER1 = Register(Registers.ERegisters[1]);
    private static readonly Mutator<NX8Disassembler> ER2 = Register(Registers.ERegisters[2]);
    private static readonly Mutator<NX8Disassembler> ER3 = Register(Registers.ERegisters[3]);

    private static readonly Mutator<NX8Disassembler> PR0 = Register(Registers.X1);
    private static readonly Mutator<NX8Disassembler> PR1 = Register(Registers.X2);
    private static readonly Mutator<NX8Disassembler> PR2 = Register(Registers.Dp);
    private static readonly Mutator<NX8Disassembler> PR3 = Register(Registers.Usp);

    private static readonly Mutator<NX8Disassembler> X1 = Register(Registers.X1);
    private static readonly Mutator<NX8Disassembler> X2 = Register(Registers.X2);
    private static readonly Mutator<NX8Disassembler> DP = Register(Registers.Dp);
    private static readonly Mutator<NX8Disassembler> USP = Register(Registers.Usp);
    private static readonly Mutator<NX8Disassembler> R0 = Register(Registers.BRegisters[0]);
    private static readonly Mutator<NX8Disassembler> R1 = Register(Registers.BRegisters[1]);
    private static readonly Mutator<NX8Disassembler> R2 = Register(Registers.BRegisters[2]);
    private static readonly Mutator<NX8Disassembler> R3 = Register(Registers.BRegisters[3]);
    private static readonly Mutator<NX8Disassembler> R4 = Register(Registers.BRegisters[4]);
    private static readonly Mutator<NX8Disassembler> R5 = Register(Registers.BRegisters[5]);
    private static readonly Mutator<NX8Disassembler> R6 = Register(Registers.BRegisters[6]);
    private static readonly Mutator<NX8Disassembler> R7 = Register(Registers.BRegisters[7]);
    private static readonly Mutator<NX8Disassembler> pswl = Register(Registers.Pswl);

    private static bool fix8(uint uInstr, NX8Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte loFix))
            return false;
        dasm.ops.Add(MemoryOperand.Direct(Registers.Dsr, 0x200 + loFix));
        return true;
    }

    private static bool off8(uint uInstr, NX8Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte loOff))
            return false;
        dasm.ops.Add(MemoryOperand.Direct(Registers.Dsr, 0x000 + loOff));
        return true;
    }


    private static bool sfr8(uint uInstr, NX8Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte sfrOff))
            return false;
        dasm.ops.Add(MemoryOperand.Direct(Registers.Dsr, 0x000 + sfrOff));
        return true;
    }

    private static bool dir(uint uInstr, NX8Disassembler dasm)
    {
        if (!dasm.rdr.TryReadLeUInt16(out ushort uAddr))
            return false;
        dasm.ops.Add(MemoryOperand.Direct(Registers.Dsr, uAddr));
        return true;
    }

    private static bool N8(uint uInstr, NX8Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte b))
            return false;
        dasm.ops.Add(Constant.Byte(b));
        return true;
    }

    private static bool N16(uint uInstr, NX8Disassembler dasm)
    {
        if (!dasm.rdr.TryReadLeUInt16(out ushort n16))
            return false;
        dasm.ops.Add(Constant.Word16(n16));
        return true;
    }

    private static bool Ndd(uint uInstr, NX8Disassembler dasm)
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

    private static bool Mdp_n7(uint uInstr, NX8Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte n))
            return false;
        int offset = bf0L7.ReadSigned(uInstr);
        dasm.ops.Add(MemoryOperand.Indirect(Registers.Dsr, Registers.Dp, offset));
        return true;
    }

    private static Mutator<NX8Disassembler> Mindexed(RegisterStorage @base, RegisterStorage index)
    {
        return (u, d) =>
        {
            d.ops.Add(MemoryOperand.Indexed(Registers.Dsr, @base, index));
            return true;
        };
    }
    private static readonly Mutator<NX8Disassembler> Mx1_a = Mindexed(Registers.X1, Registers.Acc);
    private static readonly Mutator<NX8Disassembler> Mx1_r0 = Mindexed(Registers.X1, Registers.BRegisters[0]);

    private static Mutator<NX8Disassembler> Indirect(RegisterStorage seg, RegisterStorage reg)
    {
        return (u, d) =>
        {
            var mem = MemoryOperand.Indirect(seg, reg, 0);
            d.ops.Add(mem);
            return true;
        };
    }
    private static readonly Mutator<NX8Disassembler> Mdp = Indirect(Registers.Dsr, Registers.Dp);
    private static readonly Mutator<NX8Disassembler> Mx1 = Indirect(Registers.Dsr, Registers.X1);

    private static Mutator<NX8Disassembler> IndirectOffset(RegisterStorage seg, RegisterStorage reg)
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
    private static readonly Mutator<NX8Disassembler> Md16_X1 = IndirectOffset(Registers.Dsr, Registers.X1);
    private static readonly Mutator<NX8Disassembler> Md16_X2 = IndirectOffset(Registers.Dsr, Registers.X2);


    private static bool Mdp_plus(uint uInstr, NX8Disassembler dasm)
    {
        var mem = MemoryOperand.PostIncremented(Registers.Dsr, Registers.Dp);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool Mdp_minus(uint uInstr, NX8Disassembler dasm)
    {
        var mem = MemoryOperand.PostDecremented(Registers.Dsr, Registers.Dp);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool Addr16(uint uInstr, NX8Disassembler dasm)
    {
        if (!dasm.rdr.TryReadLeUInt16(out ushort addr16))
            return false;
        dasm.ops.Add(Address.Ptr16(addr16));
        return true;
    }

    private static bool rdiff8(uint uInstr, NX8Disassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out byte b))
            return false;
        var target = dasm.rdr.Address + b;
        dasm.ops.Add(target);
        return true;
    }

    private static Mutator<NX8Disassembler> Imm(byte n)
    {
        return (u, d) =>
        {
            var imm = Constant.Byte(n);
            d.ops.Add(imm);
            return true;
        };
    }
    private static readonly Mutator<NX8Disassembler> imm1 = Imm(1);
    private static readonly Mutator<NX8Disassembler> imm2 = Imm(2);
    private static readonly Mutator<NX8Disassembler> imm3 = Imm(3);
    private static readonly Mutator<NX8Disassembler> imm4 = Imm(4);

    private static bool SwapOperands(uint uInstr, NX8Disassembler dasm)
    {
        var t = dasm.ops[1];
        dasm.ops[1] = dasm.ops[0];
        dasm.ops[0] = t;
        return true;
    }

    private static Decoder<NX8Disassembler, Mnemonic, NX8Instruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<NX8Disassembler>[] mutators)
    {
        return new InstrDecoder<NX8Disassembler, Mnemonic, NX8Instruction>(iclass, mnemonic, mutators);
    }

    private static Decoder<NX8Disassembler, Mnemonic, NX8Instruction> Instr(Mnemonic mnemonic, params Mutator<NX8Disassembler>[] mutators)
    {
        return new InstrDecoder<NX8Disassembler, Mnemonic, NX8Instruction>(InstrClass.Linear, mnemonic, mutators);
    }

    /// <summary>
    /// Creates an instruction decoder that is sensitive to the state of the
    /// DD status bit.
    /// </summary>
    /// <param name="mnemonicB">Mnemonic to use when DD=0</param>
    /// <param name="mnemonicW">Mnemonic to use when DD=1</param>
    /// <param name="mutators">Operand decoders</param>
    /// <returns>An instance of <see cref="DdInstrDecoder"/></returns>
    private static Decoder<NX8Disassembler, Mnemonic, NX8Instruction> InstrDD(Mnemonic mnemonicB, Mnemonic mnemonicW,  params Mutator<NX8Disassembler>[] mutators)
    {
        return new DdInstrDecoder(mnemonicB, mnemonicW, InstrClass.Linear, mutators);
    }

    private static Decoder<NX8Disassembler, Mnemonic, NX8Instruction> InstrDD(Mnemonic mnemonicB, Mnemonic mnemonicW, InstrClass iclass, params Mutator<NX8Disassembler>[] mutators)
    {
        return new DdInstrDecoder(mnemonicB, mnemonicW, iclass, mutators);
    }


    private static Decoder<NX8Disassembler, Mnemonic, NX8Instruction> Prefix(Mutator<NX8Disassembler> mutator)
    {
        return new PrefixDecoder(mutator, null);
    }

    private static Decoder<NX8Disassembler, Mnemonic, NX8Instruction> BPrefix(Mutator<NX8Disassembler> mutator)
    {
        return new PrefixDecoder(mutator, false);
    }

    private static Decoder<NX8Disassembler, Mnemonic, NX8Instruction> WPrefix(Mutator<NX8Disassembler> mutator)
    {
        return new PrefixDecoder(mutator, true);
    }


    private class DdInstrDecoder : Decoder<NX8Disassembler, Mnemonic, NX8Instruction>
    {
        private readonly Mnemonic mnemonicB;
        private readonly Mnemonic mnemonicW;
        private readonly InstrClass iclass;
        private readonly Mutator<NX8Disassembler>[] mutators;

        internal DdInstrDecoder(Mnemonic mnemonicB, Mnemonic mnemonicW, InstrClass iclass, Mutator<NX8Disassembler>[] mutators)
        {
            this.mnemonicB = mnemonicB;
            this.mnemonicW = mnemonicW;
            this.iclass = iclass;
            this.mutators = mutators;
        }

        public override NX8Instruction Decode(uint uInstr, NX8Disassembler dasm)
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

    private class PrefixDecoder : Decoder<NX8Disassembler, Mnemonic, NX8Instruction>
    {
        private readonly Mutator<NX8Disassembler> mutator;
        private readonly bool? setDd;

        public PrefixDecoder(Mutator<NX8Disassembler> mutator, bool? setDd)
        {
            this.mutator = mutator;
            this.setDd = setDd;
        }

        public override NX8Instruction Decode(uint uInstr, NX8Disassembler dasm)
        {
            dasm.ddSetThisInstruction = this.setDd;
            if (!mutator(uInstr, dasm))
                return dasm.CreateInvalidInstruction();
            if (!dasm.rdr.TryReadByte(out byte b))
                return dasm.CreateInvalidInstruction();
            return prefixDecoders[b].Decode(uInstr, dasm);
        }
    }

    static NX8Disassembler()
    {
        var nyi = new NyiDecoder<NX8Disassembler, Mnemonic, NX8Instruction>("nyi");
        
        prefixDecoders = new Decoder<NX8Disassembler, Mnemonic, NX8Instruction>[] {
            // 0x00
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

            // 0x10
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

            // 0x20
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

            // 0x30
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

            // 0x40
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

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0x80
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

            InstrDD(Mnemonic.sllb, Mnemonic.sll, a, imm1),
            InstrDD(Mnemonic.sllb, Mnemonic.sll, a, imm2),
            InstrDD(Mnemonic.sllb, Mnemonic.sll, a, imm3),
            InstrDD(Mnemonic.sllb, Mnemonic.sll, a, imm3),

            // 0x90
            Instr(Mnemonic.cmp, fix8),
            Instr(Mnemonic.cmp, off8),
            Instr(Mnemonic.cmp, sfr8),
            Instr(Mnemonic.cmp, N16),

            Instr(Mnemonic.cmp, a),
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

            // 0xA0
            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.div),
            Instr(Mnemonic.mul),
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            // 0xB0
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

            // 0xC0
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

            // 0xD0
            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            InstrDD(Mnemonic.decb, Mnemonic.dec),
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

            // 0xF0
            nyi,
            nyi,
            nyi,
            InstrDD(Mnemonic.adcb, Mnemonic.adc, Ndd),

            nyi,
            InstrDD(Mnemonic.adcb, Mnemonic.adc, a, SwapOperands),
            nyi,
            nyi,

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
            Instr(Mnemonic.nop, InstrClass.Linear | InstrClass.Zero | InstrClass.Padding),
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.sj, InstrClass.ConditionalTransfer, rdiff8),
            nyi,
            nyi,
            nyi,

            InstrDD(Mnemonic.subb, Mnemonic.sub, a, ER0),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, ER1),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, ER2),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, ER3),

            InstrDD(Mnemonic.subb, Mnemonic.sub, a, PR0),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, PR1),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, PR2),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, PR3),

            // 0x10
            Instr(Mnemonic.movb, R0, N8),
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.cmp, a, ER0),
            Instr(Mnemonic.cmp, a, ER1),
            Instr(Mnemonic.cmp, a, ER2),
            Instr(Mnemonic.cmp, a, ER3),

            Instr(Mnemonic.cmp, a, X1),
            Instr(Mnemonic.cmp, a, X2),
            Instr(Mnemonic.cmp, a, DP),
            Instr(Mnemonic.cmp, a, USP),

            // 0x20
            Instr(Mnemonic.mov, X1, N16),
            Instr(Mnemonic.mov, X2, N16),
            Instr(Mnemonic.mov, DP, N16),
            Instr(Mnemonic.mov, USP, N16),

            Instr(Mnemonic.mov, ER0, N16),
            Instr(Mnemonic.mov, ER1, N16),
            Instr(Mnemonic.mov, ER2, N16),
            Instr(Mnemonic.mov, ER3, N16),

            InstrDD(Mnemonic.addb, Mnemonic.add, a, ER0),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, ER1),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, ER2),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, ER3),

            InstrDD(Mnemonic.addb, Mnemonic.add, a, X1),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, X2),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, DP),
            InstrDD(Mnemonic.addb, Mnemonic.add, a, USP),

            // 0x30
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
            InstrDD(Mnemonic.stb, Mnemonic.st, a,Mx1),
            nyi,
            nyi,

            // 0x40
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
            WPrefix(PR0),
            WPrefix(PR1),
            WPrefix(PR2),
            WPrefix(PR3),

            WPrefix(ER0),
            WPrefix(ER1),
            WPrefix(ER2),
            WPrefix(ER3),

            BPrefix(R0),
            BPrefix(R1),
            BPrefix(R2),
            BPrefix(R3),

            BPrefix(R4),
            BPrefix(R5),
            BPrefix(R6),
            BPrefix(R7),

            // 0x70
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

            // 0x80
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
            Prefix(pswl),
            WPrefix(Mdp_n7),

            InstrDD(Mnemonic.subb, Mnemonic.sub, a, fix8),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, off8),
            InstrDD(Mnemonic.subb, Mnemonic.sub, a, Ndd),
            InstrDD(Mnemonic.sllb, Mnemonic.sll, a),

            // 0x90
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
            Prefix(pswl),
            BPrefix(Mdp_n7),

            InstrDD(Mnemonic.cmpb, Mnemonic.cmp, a, fix8),
            InstrDD(Mnemonic.cmpb, Mnemonic.cmp, a, off8),
            InstrDD(Mnemonic.cmpb, Mnemonic.cmp, a, N16),
            InstrDD(Mnemonic.srlb, Mnemonic.srl, a),

            // 0xA0
            WPrefix(Mx1),
            WPrefix(Mdp),
            WPrefix(Mdp_plus),
            WPrefix(Mdp_minus),

            WPrefix(fix8),
            WPrefix(off8),
            WPrefix(sfr8),
            WPrefix(dir),

            WPrefix(Md16_X1),
            WPrefix(Md16_X2),
            WPrefix(Mx1_a),
            WPrefix(Mx1_r0),

            nyi,
            nyi,
            nyi,
            nyi,

            // 0xB0
            BPrefix(Mx1),
            BPrefix(Mdp),
            BPrefix(Mdp_plus),
            BPrefix(Mdp_minus),

            BPrefix(fix8),
            BPrefix(off8),
            BPrefix(sfr8),
            BPrefix(dir),

            BPrefix(Md16_X1),
            BPrefix(Md16_X2),
            BPrefix(Mx1_a),
            BPrefix(Mx1_r0),

            Prefix(a),
            nyi,
            nyi,
            nyi,

            // 0xC0
            nyi,
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.cmp, fix8, N16),
            Instr(Mnemonic.cmp, off8, N16),
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

            // 0x0D0
            InstrDD(Mnemonic.decb, Mnemonic.dec, R0),
            InstrDD(Mnemonic.decb, Mnemonic.dec, R1),
            InstrDD(Mnemonic.decb, Mnemonic.dec, R2),
            InstrDD(Mnemonic.decb, Mnemonic.dec, R3),

            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            Instr(Mnemonic.sdd, SetDDflag),
            Instr(Mnemonic.di),
            nyi,

            InstrDD(Mnemonic.decb, Mnemonic.dec, a),
            nyi,
            nyi,
            Instr(Mnemonic.swap),

            // 0xE0
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

            // 0xF0
            nyi,
            nyi,
            nyi,
            nyi,

            nyi,
            nyi,
            nyi,
            nyi,

            Instr(Mnemonic.l, N16, SetDDflag),
            Instr(Mnemonic.lb, N16, ClearDDflag),
            Instr(Mnemonic.clr, a),
            nyi,

            nyi,
            nyi,
            Instr(Mnemonic.cal, InstrClass.Transfer|InstrClass.Call, Addr16),
            Instr(Mnemonic.brk, InstrClass.Terminates|InstrClass.Padding));
    }
}
