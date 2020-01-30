#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Arch.MicrochipPIC.Common
{
    public abstract class PICInstruction : MachineInstruction
    {
        public const InstrClass CondLinear = InstrClass.Conditional | InstrClass.Linear;
        public const InstrClass CondTransfer = InstrClass.Conditional | InstrClass.Transfer;
        public const InstrClass LinkTransfer = InstrClass.Call | InstrClass.Transfer;
        public const InstrClass Transfer = InstrClass.Transfer;

        private static readonly Dictionary<Mnemonic, InstrClass> classOf = new Dictionary<Mnemonic, InstrClass>()
        {
                { Mnemonic.ADDULNK,   Transfer },
                { Mnemonic.BRA,       Transfer },
                { Mnemonic.BRW,       Transfer },
                { Mnemonic.GOTO,      Transfer },
                { Mnemonic.RESET,     Transfer },
                { Mnemonic.RETFIE,    Transfer },
                { Mnemonic.RETLW,     Transfer },
                { Mnemonic.RETURN,    Transfer },
                { Mnemonic.SUBULNK,   Transfer },
                { Mnemonic.BC,        CondTransfer },
                { Mnemonic.BN,        CondTransfer },
                { Mnemonic.BNC,       CondTransfer },
                { Mnemonic.BNN,       CondTransfer },
                { Mnemonic.BNOV,      CondTransfer },
                { Mnemonic.BNZ,       CondTransfer },
                { Mnemonic.BOV,       CondTransfer },
                { Mnemonic.BZ,        CondTransfer },
                { Mnemonic.BTFSC,     CondLinear },
                { Mnemonic.BTFSS,     CondLinear },
                { Mnemonic.CPFSEQ,    CondLinear },
                { Mnemonic.CPFSGT,    CondLinear },
                { Mnemonic.CPFSLT,    CondLinear },
                { Mnemonic.DCFSNZ,    CondLinear },
                { Mnemonic.DECFSZ,    CondLinear },
                { Mnemonic.INCFSZ,    CondLinear },
                { Mnemonic.INFSNZ,    CondLinear },
                { Mnemonic.TSTFSZ,    CondLinear },
                { Mnemonic.CALL,      LinkTransfer },
                { Mnemonic.CALLW,     LinkTransfer },
                { Mnemonic.RCALL,     LinkTransfer },
                { Mnemonic.CONFIG,    InstrClass.None },
                { Mnemonic.DA,        InstrClass.None },
                { Mnemonic.DB,        InstrClass.None },
                { Mnemonic.DE,        InstrClass.None },
                { Mnemonic.DT,        InstrClass.None },
                { Mnemonic.DTM,       InstrClass.None },
                { Mnemonic.DW,        InstrClass.None },
                { Mnemonic.__CONFIG,  InstrClass.None },
                { Mnemonic.__IDLOCS,  InstrClass.None },
                { Mnemonic.invalid,   InstrClass.Invalid },
                { Mnemonic.unaligned, InstrClass.Invalid },
        };


        /// <summary>
        /// Instantiates a new <see cref="PICInstruction"/> with given <see cref="Mnemonic"/> and operands.
        /// Throws an <see cref="ArgumentException"/> in more than 3 operands are provided.
        /// </summary>
        /// <param name="mnemonic">The PIC mnemonic.</param>
        /// <param name="ops">Zero, one, two or three instruction's operands ops.</param>
        /// <exception cref="ArgumentException">Thrown if more than 3 operands provided.</exception>
        public PICInstruction(Mnemonic mnemonic, params MachineOperand[] ops)
        {
            if (ops.Length >= 4)
                throw new ArgumentException(nameof(ops), "Too many PIC instruction operands.");
            Mnemonic = mnemonic;

            if (!classOf.TryGetValue(Mnemonic, out this.InstructionClass))
                this.InstructionClass = InstrClass.Linear;
            Operands = ops;
        }

        /// <summary>
        /// Gets the mnemonic.
        /// </summary>
        public Mnemonic Mnemonic { get; }

        /// <summary>
        /// Each different supported mnemonic should have a different numerical value, exposed here.
        /// </summary>
        /// <value>
        /// The mnemonic as integer.
        /// </value>
        public override int MnemonicAsInteger => (int)Mnemonic;

        /// <summary>
        /// Gets the number of operands of this instruction.
        /// </summary>
        /// <value>
        /// The number of operands as an integer.
        /// </value>
        public byte NumberOfOperands
        {
            get
            {
                if (Operands[0] is null)
                    return 0;
                if (Operands[1] is null)
                    return 1;
                if (Operands[2] is null)
                    return 2;
                return 3;
            }
        }
    }

    /// <summary>
    /// A PIC instruction of the form 'instr'.
    /// </summary>
    public class PICInstructionNoOpnd : PICInstruction
    {
        public PICInstructionNoOpnd(Mnemonic mnemonic) : base(mnemonic)
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString());
        }
    }

    /// <summary>
    /// A PIC instruction of the form 'instr immbyte'.
    /// </summary>
    public class PICInstructionImmedByte : PICInstruction
    {
        public PICInstructionImmedByte(Mnemonic mnemonic, ushort imm)
            : base(mnemonic, new PICOperandImmediate(imm, PrimitiveType.Byte))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();
            Operands[0].Write(writer, options);
        }

    }

    public class PICInstructionImmedSByte : PICInstruction
    {
        public PICInstructionImmedSByte(Mnemonic mnemonic, short imm)
            : base(mnemonic, new PICOperandImmediate(imm, PrimitiveType.SByte))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();
            Operands[0].Write(writer, options);
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr immushort'.
    /// </summary>
    public class PICInstructionImmedUShort : PICInstruction
    {
        public PICInstructionImmedUShort(Mnemonic mnemonic, ushort imm)
            : base(mnemonic, new PICOperandImmediate(imm, PrimitiveType.UInt16))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();
            Operands[0].Write(writer, options);
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr +/-immshort'.
    /// </summary>
    public class PICInstructionImmedShort : PICInstruction
    {
        public PICInstructionImmedShort(Mnemonic mnemonic, short imm)
            : base(mnemonic,
                   new PICOperandImmediate(imm, PrimitiveType.Int16))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();
            Operands[0].Write(writer, options);
        }
    }

    /// <summary>
    /// A PIC instruction of the form 'instr fs,fd'.
    /// </summary>
    public class PICInstructionMem2Mem : PICInstruction
    {
        public PICInstructionMem2Mem(Mnemonic mnemonic, uint srcaddr, uint dstaddr)
            : base(mnemonic,
                   new PICOperandDataMemoryAddress(srcaddr),
                   new PICOperandDataMemoryAddress(dstaddr))
        {
        }

        public PICInstructionMem2Mem(Mnemonic mnemonic, byte srcidx, uint dstaddr)
            : base(mnemonic,
                   new PICOperandFSRIndexation(Constant.Byte(srcidx)),
                   new PICOperandDataMemoryAddress(dstaddr))
        {
        }

        public PICInstructionMem2Mem(Mnemonic mnemonic, byte srcidx, byte dstidx)
            : base(mnemonic,
                   new PICOperandFSRIndexation(Constant.Byte(srcidx)),
                   new PICOperandFSRIndexation(Constant.Byte(dstidx)))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();

            switch (Operands[0])
            {
                case PICOperandDataMemoryAddress srcmem:
                    if (PICRegisters.TryGetRegister(srcmem.DataTarget, out var srcreg))
                    {
                        writer.WriteString($"{srcreg.Name}");
                    }
                    else
                    {
                        srcmem.Write(writer, options);
                    }
                    break;

                case PICOperandFSRIndexation srcidx:
                    if (srcidx.Mode != FSRIndexedMode.FSR2INDEXED)
                        throw new InvalidOperationException($"Invalid FSR2 indexing mode: {srcidx.Mode}.");
                    writer.WriteString($"[{srcidx.Offset:X2}]");
                    break;
            }

            writer.WriteString(",");

            switch (Operands[1])
            {
                case PICOperandDataMemoryAddress dstmem:
                    if (PICRegisters.TryGetRegister(dstmem.DataTarget, out var dstreg))
                    {
                        writer.WriteString($"{dstreg.Name}");
                    }
                    else
                    {
                        dstmem.Write(writer, options);
                    }
                    break;

                case PICOperandFSRIndexation dstidx:
                    if (dstidx.Mode != FSRIndexedMode.FSR2INDEXED)
                        throw new InvalidOperationException($"Invalid FSR2 indexing mode: {dstidx.Mode}.");
                    writer.WriteString($"[{dstidx.Offset:X2}]");
                    break;
            }

        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr f'.
    /// </summary>
    public class PICInstructionMemF : PICInstruction
    {
        public PICInstructionMemF(Mnemonic mnemonic, ushort memaddr)
            : base(mnemonic, new PICOperandBankedMemory(memaddr))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var memop = Operands[0] as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {Operands[0]}");

            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();

            var bankmem = PICMemoryDescriptor.CreateBankedAddr(memop);

            if (PICRegisters.TryGetAlwaysAccessibleRegister(bankmem, out var reg))
            {
                writer.WriteString($"{reg.Name}");
            }
            else
            {
                Operands[0].Write(writer, options);
            }
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr f,b'.
    /// </summary>
    public class PICInstructionMemFB : PICInstruction
    {
        public PICInstructionMemFB(Mnemonic mnemonic, ushort memaddr, byte bitno)
            : base(mnemonic, new PICOperandBankedMemory(memaddr), new PICOperandMemBitNo(bitno))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var bankmem = Operands[0] as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {Operands[0]}");
            var bitno = Operands[1] as PICOperandMemBitNo ?? throw new InvalidOperationException($"Invalid bit number operand: {Operands[1]}");

            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();

            if (PICRegisters.TryGetAlwaysAccessibleRegister(bankmem.Offset, out var reg))
            {
                writer.WriteString($"{reg.Name}");
                writer.WriteString(",");
                if (PICRegisters.TryGetBitField(reg, out var fld, bitno.BitNo, 1))
                {
                    writer.WriteString($"{fld.Name}");
                }
                else
                {
                    bitno.Write(writer, options);
                }
                return;
            }
            bankmem.Write(writer, options);
            writer.WriteString(",");
            bitno.Write(writer, options);
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr f,d'.
    /// </summary>
    public class PICInstructionMemFD : PICInstruction
    {
        public PICInstructionMemFD(Mnemonic mnemonic, ushort memaddr, ushort dest)
            : base(mnemonic, new PICOperandBankedMemory(memaddr), new PICOperandMemWRegDest(dest))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var bankmem = Operands[0] as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {Operands[0]}");
            var wregdest = Operands[1] as PICOperandMemWRegDest ?? throw new InvalidOperationException($"Invalid destination operand: {Operands[1]}");

            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();

            if (PICRegisters.TryGetAlwaysAccessibleRegister(bankmem.Offset, out var reg))
            {
                writer.WriteString($"{reg.Name}");
            }
            else
            {
                bankmem.Write(writer, options);
            }
            wregdest.Write(writer, options);
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr f,a'.
    /// </summary>
    public class PICInstructionMemFA : PICInstruction
    {
        public PICInstructionMemFA(Mnemonic mnemonic, ushort memaddr, ushort acc)
            : base(mnemonic, new PICOperandBankedMemory(memaddr, acc))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var memop = Operands[0] as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {Operands[0]}");

            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();

            var bankmem = PICMemoryDescriptor.CreateBankedAddr(memop);

            if (PICMemoryDescriptor.CanBeFSR2IndexAddress(bankmem))
            {
                writer.WriteString($"[{bankmem.BankOffset:X2}]");
                return;
            }

            string sAcc = bankmem.IsAccessRAMAddr ? ",ACCESS" : "";
            if (PICMemoryDescriptor.TryGetAbsDataAddress(bankmem, out var absaddr))
            {
                if (PICRegisters.TryGetRegister(absaddr, out var areg, 8))
                {
                    writer.WriteString($"{areg.Name}{sAcc}");
                    return;
                }
                writer.WriteString($"0x{absaddr.Offset:X2}{sAcc}");
                return;
            }

            writer.WriteString($"{bankmem.BankOffset:X2}{sAcc}");
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr f,b,a'.
    /// </summary>
    public class PICInstructionMemFBA : PICInstruction
    {
        public PICInstructionMemFBA(Mnemonic mnemonic, ushort memaddr, ushort bitno, ushort acc)
            : base(mnemonic, new PICOperandBankedMemory(memaddr, acc), new PICOperandMemBitNo(bitno))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var memop = Operands[0] as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {Operands[0]}");
            var bitno = Operands[1] as PICOperandMemBitNo ?? throw new InvalidOperationException($"Invalid bit number operand: {Operands[1]}");

            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();

            var bankmem = PICMemoryDescriptor.CreateBankedAddr(memop);

            if (PICMemoryDescriptor.CanBeFSR2IndexAddress(bankmem))
            {
                writer.WriteString($"[{bankmem.BankOffset:X2}],");
                bitno.Write(writer, options);
                return;
            }

            string sAcc = bankmem.IsAccessRAMAddr ? ",ACCESS" : "";
            if (PICMemoryDescriptor.TryGetAbsDataAddress(bankmem, out var absaddr))
            {
                if (PICRegisters.TryGetRegister(absaddr, out var areg, 8))
                {
                    if (PICRegisters.TryGetBitField(areg, out var fld, bitno.BitNo, 1))
                    {
                        writer.WriteString($"{areg.Name},{fld.Name}{sAcc}");
                        return;
                    }
                    writer.WriteString($"{areg.Name},{bitno.BitNo}{sAcc}");
                    return;
                }
                writer.WriteString($"0x{absaddr.Offset:X2},{bitno.BitNo}{sAcc}");
                return;
            }

            writer.WriteString($"{bankmem.BankOffset:X2},{bitno.BitNo}{sAcc}");
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr f,d,a'.
    /// </summary>
    public class PICInstructionMemFDA : PICInstruction
    {
        public PICInstructionMemFDA(Mnemonic mnemonic, ushort memaddr, ushort dest, ushort acc)
            : base(mnemonic, new PICOperandBankedMemory(memaddr, acc), new PICOperandMemWRegDest(dest))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var memop = Operands[0] as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {Operands[0]}");
            var wregdest = Operands[1] as PICOperandMemWRegDest ?? throw new InvalidOperationException($"Invalid destination operand: {Operands[1]}");

            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();

            var bankmem = PICMemoryDescriptor.CreateBankedAddr(memop);

            if (PICMemoryDescriptor.CanBeFSR2IndexAddress(bankmem))
            {
                writer.WriteString($"[{bankmem.BankOffset:X2}]");
                wregdest.Write(writer, options);
                return;
            }

            string sAcc = bankmem.IsAccessRAMAddr ? ",ACCESS" : "";
            if (PICMemoryDescriptor.TryGetAbsDataAddress(bankmem, out var absaddr))
            {
                if (PICRegisters.TryGetRegister(absaddr, out var areg, 8))
                {
                    writer.WriteString($"{areg.Name}");
                    wregdest.Write(writer, options);
                    writer.WriteString(sAcc);
                    return;
                }
                writer.WriteString($"0x{absaddr.Offset:X2}");
                wregdest.Write(writer, options);
                writer.WriteString(sAcc);
                return;
            }

            writer.WriteString($"{bankmem.BankOffset:X2}");
            wregdest.Write(writer, options);
            writer.WriteString(sAcc);
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'LFSR fsr,k'.
    /// </summary>
    public class PICInstructionLFSRLoad : PICInstruction
    {
        public PICInstructionLFSRLoad(Mnemonic mnemonic, ushort fsrnum, ushort imm)
            : base(mnemonic,
                   new PICOperandFSRNum(fsrnum),
                   new PICOperandImmediate(imm, PrimitiveType.UInt16))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var fsrnum = Operands[0] as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR number operand: {Operands[0]}");
            var imm = Operands[1] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {Operands[1]}");

            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();
            fsrnum.Write(writer, options);
            writer.WriteString(",");
            imm.Write(writer, options);
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'ADDFSR fsr,+/-k'.
    /// </summary>
    public class PICInstructionFSRIArith : PICInstruction
    {
        public PICInstructionFSRIArith(Mnemonic mnemonic, ushort fsrnum, sbyte imm)
            : base(mnemonic,
                   new PICOperandFSRNum(fsrnum),
                   new PICOperandImmediate(imm, PrimitiveType.SByte))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var fsrnum = Operands[0] as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR number operand: {Operands[0]}");
            var imm = Operands[1] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {Operands[1]}");

            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();
            fsrnum.Write(writer, options);
            writer.WriteString(",");
            imm.Write(writer, options);
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'ADDFSR, SUBFSR fsr,k'.
    /// </summary>
    public class PICInstructionFSRUArith : PICInstruction
    {
        public PICInstructionFSRUArith(Mnemonic mnemonic, ushort fsrnum, byte imm)
            : base(mnemonic,
                   new PICOperandFSRNum(fsrnum),
                   new PICOperandImmediate(imm, PrimitiveType.Byte))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var fsrnum = Operands[0] as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR number operand: {Operands[0]}");
            var imm = Operands[1] as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {Operands[1]}");

            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();
            fsrnum.Write(writer, options);
            writer.WriteString(",");
            imm.Write(writer, options);
        }

    }

    /// <summary>
    /// A PIC instruction program memory target.
    /// </summary>
    public class PICInstructionProgTarget : PICInstruction
    {
        public PICInstructionProgTarget(Mnemonic mnemonic, uint progAdr)
            : base(mnemonic, new PICOperandProgMemoryAddress(progAdr))
        {
        }

        public PICInstructionProgTarget(Mnemonic mnemonic, short progOff, Address instrAdr)
            : base(mnemonic, new PICOperandProgMemoryAddress(progOff, instrAdr))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();
            if (Operands[0] is PICOperandProgMemoryAddress target)
            {
                writer.WriteString($"0x{target}");
            }
            else
            {
                Operands[0].Write(writer, options);
            }
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr [s]'.
    /// </summary>
    public class PICInstructionFast : PICInstruction
    {
        public PICInstructionFast(Mnemonic mnemonic, ushort fast, bool wTab)
            : base(mnemonic, new PICOperandFast(fast, wTab))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var fast = Operands[0] as PICOperandFast ?? throw new InvalidOperationException($"Invalid FAST operand: {Operands[1]}");
            writer.WriteMnemonic(Mnemonic.ToString());
            if (fast.IsFast)
            {
                writer.WriteString(",");
                writer.Tab();
                writer.WriteString("FAST");
            }
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr target,[s]'.
    /// </summary>
    public class PICInstructionProgTargetFast : PICInstruction
    {
        public PICInstructionProgTargetFast(Mnemonic mnemonic, uint progAdr, ushort fast, bool wTab)
            : base(mnemonic,
                   new PICOperandProgMemoryAddress(progAdr),
                   new PICOperandFast(fast, wTab))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();
            if (Operands[0] is PICOperandProgMemoryAddress target)
            {
                writer.WriteString($"0x{target}");
            }
            else
            {
                Operands[0].Write(writer, options);
            }
            if (Operands[1] is PICOperandFast fast)
            {
                writer.WriteString(fast.IsFast ? ",FAST" : "");
            }
        }

    }

    public class PICInstructionWithFSR : PICInstruction
    {
        public PICInstructionWithFSR(Mnemonic mnemonic, ushort fsrnum, sbyte value, FSRIndexedMode mode)
            : base(mnemonic, new PICOperandFSRIndexation(fsrnum, Constant.SByte(value), mode))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var fsridx = Operands[0] as PICOperandFSRIndexation ?? throw new InvalidOperationException($"Invalid FSR index operand: {Operands[0]}");
            var fsrnum = fsridx.FSRNum;

            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();

            switch (fsridx.Mode)
            {
                case FSRIndexedMode.INDEXED:
                    writer.WriteString($"{fsridx.Offset}[{fsrnum}]");
                    break;
                case FSRIndexedMode.POSTDEC:
                    writer.WriteString($"FSR{fsrnum}--");
                    break;
                case FSRIndexedMode.POSTINC:
                    writer.WriteString($"FSR{fsrnum}++");
                    break;
                case FSRIndexedMode.PREDEC:
                    writer.WriteString($"--FSR{fsrnum}");
                    break;
                case FSRIndexedMode.PREINC:
                    writer.WriteString($"++FSR{fsrnum}");
                    break;
                default:
                    throw new InvalidOperationException($"Invalid indexation '{fsridx}' for MOVI instruction.");
            }
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'tblinstr mode'.
    /// </summary>
    public class PICInstructionTbl : PICInstruction
    {

        public PICInstructionTbl(Mnemonic mnemonic, ushort mode)
            : base(mnemonic, new PICOperandTBLRW(mode))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString());
            Operands[0].Write(writer, options);
        }

    }

    public class PICInstructionTris : PICInstruction
    {

        public PICInstructionTris(Mnemonic mnemonic, byte trisnum)
            : base(mnemonic,
                   new PICOperandTris(trisnum))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();
            Operands[0].Write(writer, options);
        }
    }

    public class PICInstructionPseudo : PICInstruction
    {

        public PICInstructionPseudo(Mnemonic mnemonic, PICOperandPseudo pseudoOperand)
            : base(mnemonic, pseudoOperand)
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteMnemonic(Mnemonic.ToString());
            writer.Tab();
            Operands[0].Write(writer, options);
        }
    }
}
