#region License
/* 
 * Copyright (C) 2017-2019 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2019 John Källén.
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

        public readonly MachineOperand op1;
        public readonly MachineOperand op2;
        public readonly MachineOperand op3;

        private static readonly Dictionary<Opcode, InstrClass> classOf = new Dictionary<Opcode, InstrClass>()
        {
                { Opcode.ADDULNK,   Transfer },
                { Opcode.BRA,       Transfer },
                { Opcode.BRW,       Transfer },
                { Opcode.GOTO,      Transfer },
                { Opcode.RESET,     Transfer },
                { Opcode.RETFIE,    Transfer },
                { Opcode.RETLW,     Transfer },
                { Opcode.RETURN,    Transfer },
                { Opcode.SUBULNK,   Transfer },
                { Opcode.BC,        CondTransfer },
                { Opcode.BN,        CondTransfer },
                { Opcode.BNC,       CondTransfer },
                { Opcode.BNN,       CondTransfer },
                { Opcode.BNOV,      CondTransfer },
                { Opcode.BNZ,       CondTransfer },
                { Opcode.BOV,       CondTransfer },
                { Opcode.BZ,        CondTransfer },
                { Opcode.BTFSC,     CondLinear },
                { Opcode.BTFSS,     CondLinear },
                { Opcode.CPFSEQ,    CondLinear },
                { Opcode.CPFSGT,    CondLinear },
                { Opcode.CPFSLT,    CondLinear },
                { Opcode.DCFSNZ,    CondLinear },
                { Opcode.DECFSZ,    CondLinear },
                { Opcode.INCFSZ,    CondLinear },
                { Opcode.INFSNZ,    CondLinear },
                { Opcode.TSTFSZ,    CondLinear },
                { Opcode.CALL,      LinkTransfer },
                { Opcode.CALLW,     LinkTransfer },
                { Opcode.RCALL,     LinkTransfer },
                { Opcode.CONFIG,    InstrClass.None },
                { Opcode.DA,        InstrClass.None },
                { Opcode.DB,        InstrClass.None },
                { Opcode.DE,        InstrClass.None },
                { Opcode.DT,        InstrClass.None },
                { Opcode.DTM,       InstrClass.None },
                { Opcode.DW,        InstrClass.None },
                { Opcode.__CONFIG,  InstrClass.None },
                { Opcode.__IDLOCS,  InstrClass.None },
                { Opcode.invalid,   InstrClass.Invalid },
                { Opcode.unaligned, InstrClass.Invalid },
        };


        /// <summary>
        /// Instantiates a new <see cref="PICInstruction"/> with given <see cref="Opcode"/> and operands.
        /// Throws an <see cref="ArgumentException"/> in more than 3 operands are provided.
        /// </summary>
        /// <param name="opc">The PIC opcode.</param>
        /// <param name="ops">Zero, one, two or three instruction's operands ops.</param>
        /// <exception cref="ArgumentException">Thrown if more than 3 operands provided.</exception>
        public PICInstruction(Opcode opc, params MachineOperand[] ops)
        {
            Opcode = opc;

            if (!classOf.TryGetValue(Opcode, out this.InstructionClass))
                this.InstructionClass = InstrClass.Linear;

            if (ops.Length >= 1)
            {
                op1 = ops[0];
                if (ops.Length >= 2)
                {
                    op2 = ops[1];
                    if (ops.Length >= 3)
                    {
                        op3 = ops[2];
                        if (ops.Length >= 4)
                            throw new ArgumentException("Too many PIC instruction's operands.", nameof(ops));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the opcode.
        /// </summary>
        public Opcode Opcode { get; }

        /// <summary>
        /// Each different supported opcode should have a different numerical value, exposed here.
        /// </summary>
        /// <value>
        /// The opcode as integer.
        /// </value>
        public override int OpcodeAsInteger => (int)Opcode;

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
                if (op1 is null)
                    return 0;
                if (op2 is null)
                    return 1;
                if (op3 is null)
                    return 2;
                return 3;
            }
        }

        /// <summary>
        /// Retrieves the nth operand, or null if there is none at that position.
        /// </summary>
        /// <param name="n">Operand's index..</param>
        /// <returns>
        /// The designated operand or null.
        /// </returns>
        public override MachineOperand GetOperand(int n)
        {
            switch (n)
            {
                case 0:
                    return op1;
                case 1:
                    return op2;
                case 2:
                    return op3;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// A PIC instruction of the form 'instr'.
    /// </summary>
    public class PICInstructionNoOpnd : PICInstruction
    {
        public PICInstructionNoOpnd(Opcode opcode) : base(opcode)
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr immbyte'.
    /// </summary>
    public class PICInstructionImmedByte : PICInstruction
    {
        public PICInstructionImmedByte(Opcode opcode, ushort imm)
            : base(opcode, new PICOperandImmediate(imm, PrimitiveType.Byte))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            op1.Write(writer, options);
        }

    }

    public class PICInstructionImmedSByte : PICInstruction
    {
        public PICInstructionImmedSByte(Opcode opcode, short imm)
            : base(opcode, new PICOperandImmediate(imm, PrimitiveType.SByte))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            op1.Write(writer, options);
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr immushort'.
    /// </summary>
    public class PICInstructionImmedUShort : PICInstruction
    {
        public PICInstructionImmedUShort(Opcode opcode, ushort imm)
            : base(opcode, new PICOperandImmediate(imm, PrimitiveType.UInt16))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            op1.Write(writer, options);
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr +/-immshort'.
    /// </summary>
    public class PICInstructionImmedShort : PICInstruction
    {
        public PICInstructionImmedShort(Opcode opcode, short imm)
            : base(opcode,
                   new PICOperandImmediate(imm, PrimitiveType.Int16))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            op1.Write(writer, options);
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr fs,fd'.
    /// </summary>
    public class PICInstructionMem2Mem : PICInstruction
    {
        public PICInstructionMem2Mem(Opcode opcode, uint srcaddr, uint dstaddr)
            : base(opcode,
                   new PICOperandDataMemoryAddress(srcaddr),
                   new PICOperandDataMemoryAddress(dstaddr))
        {
        }

        public PICInstructionMem2Mem(Opcode opcode, byte srcidx, uint dstaddr)
            : base(opcode,
                   new PICOperandFSRIndexation(Constant.Byte(srcidx)),
                   new PICOperandDataMemoryAddress(dstaddr))
        {
        }

        public PICInstructionMem2Mem(Opcode opcode, byte srcidx, byte dstidx)
            : base(opcode,
                   new PICOperandFSRIndexation(Constant.Byte(srcidx)),
                   new PICOperandFSRIndexation(Constant.Byte(dstidx)))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();

            switch (op1)
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

            switch (op2)
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
        public PICInstructionMemF(Opcode opcode, ushort memaddr)
            : base(opcode, new PICOperandBankedMemory(memaddr))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var memop = op1 as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {op1}");

            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();

            var bankmem = PICMemoryDescriptor.CreateBankedAddr(memop);

            if (PICRegisters.TryGetAlwaysAccessibleRegister(bankmem, out var reg))
            {
                writer.WriteString($"{reg.Name}");
            }
            else
            {
                op1.Write(writer, options);
            }
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr f,b'.
    /// </summary>
    public class PICInstructionMemFB : PICInstruction
    {
        public PICInstructionMemFB(Opcode opcode, ushort memaddr, byte bitno)
            : base(opcode, new PICOperandBankedMemory(memaddr), new PICOperandMemBitNo(bitno))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var bankmem = op1 as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {op1}");
            var bitno = op2 as PICOperandMemBitNo ?? throw new InvalidOperationException($"Invalid bit number operand: {op2}");

            writer.WriteOpcode(Opcode.ToString());
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
        public PICInstructionMemFD(Opcode opcode, ushort memaddr, ushort dest)
            : base(opcode, new PICOperandBankedMemory(memaddr), new PICOperandMemWRegDest(dest))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var bankmem = op1 as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {op1}");
            var wregdest = op2 as PICOperandMemWRegDest ?? throw new InvalidOperationException($"Invalid destination operand: {op2}");

            writer.WriteOpcode(Opcode.ToString());
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
        public PICInstructionMemFA(Opcode opcode, ushort memaddr, ushort acc)
            : base(opcode, new PICOperandBankedMemory(memaddr, acc))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var memop = op1 as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {op1}");

            writer.WriteOpcode(Opcode.ToString());
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
        public PICInstructionMemFBA(Opcode opcode, ushort memaddr, ushort bitno, ushort acc)
            : base(opcode, new PICOperandBankedMemory(memaddr, acc), new PICOperandMemBitNo(bitno))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var memop = op1 as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {op1}");
            var bitno = op2 as PICOperandMemBitNo ?? throw new InvalidOperationException($"Invalid bit number operand: {op2}");

            writer.WriteOpcode(Opcode.ToString());
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
        public PICInstructionMemFDA(Opcode opcode, ushort memaddr, ushort dest, ushort acc)
            : base(opcode, new PICOperandBankedMemory(memaddr, acc), new PICOperandMemWRegDest(dest))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var memop = op1 as PICOperandBankedMemory ?? throw new InvalidOperationException($"Invalid memory operand: {op1}");
            var wregdest = op2 as PICOperandMemWRegDest ?? throw new InvalidOperationException($"Invalid destination operand: {op2}");

            writer.WriteOpcode(Opcode.ToString());
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
        public PICInstructionLFSRLoad(Opcode opcode, ushort fsrnum, ushort imm)
            : base(opcode,
                   new PICOperandFSRNum(fsrnum),
                   new PICOperandImmediate(imm, PrimitiveType.UInt16))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var fsrnum = op1 as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR number operand: {op1}");
            var imm = op2 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {op2}");

            writer.WriteOpcode(Opcode.ToString());
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
        public PICInstructionFSRIArith(Opcode opcode, ushort fsrnum, sbyte imm)
            : base(opcode,
                   new PICOperandFSRNum(fsrnum),
                   new PICOperandImmediate(imm, PrimitiveType.SByte))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var fsrnum = op1 as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR number operand: {op1}");
            var imm = op2 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {op2}");

            writer.WriteOpcode(Opcode.ToString());
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
        public PICInstructionFSRUArith(Opcode opcode, ushort fsrnum, byte imm)
            : base(opcode,
                   new PICOperandFSRNum(fsrnum),
                   new PICOperandImmediate(imm, PrimitiveType.Byte))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var fsrnum = op1 as PICOperandFSRNum ?? throw new InvalidOperationException($"Invalid FSR number operand: {op1}");
            var imm = op2 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {op2}");

            writer.WriteOpcode(Opcode.ToString());
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
        public PICInstructionProgTarget(Opcode opcode, uint progAdr)
            : base(opcode, new PICOperandProgMemoryAddress(progAdr))
        {
        }

        public PICInstructionProgTarget(Opcode opcode, short progOff, Address instrAdr)
            : base(opcode, new PICOperandProgMemoryAddress(progOff, instrAdr))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            if (op1 is PICOperandProgMemoryAddress target)
            {
                writer.WriteString($"0x{target}");
            }
            else
            {
                op1.Write(writer, options);
            }
        }

    }

    /// <summary>
    /// A PIC instruction of the form 'instr [s]'.
    /// </summary>
    public class PICInstructionFast : PICInstruction
    {
        public PICInstructionFast(Opcode opcode, ushort fast, bool wTab)
            : base(opcode, new PICOperandFast(fast, wTab))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var fast = op1 as PICOperandFast ?? throw new InvalidOperationException($"Invalid FAST operand: {op2}");
            writer.WriteOpcode(Opcode.ToString());
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
        public PICInstructionProgTargetFast(Opcode opcode, uint progAdr, ushort fast, bool wTab)
            : base(opcode,
                   new PICOperandProgMemoryAddress(progAdr),
                   new PICOperandFast(fast, wTab))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            if (op1 is PICOperandProgMemoryAddress target)
            {
                writer.WriteString($"0x{target}");
            }
            else
            {
                op1.Write(writer, options);
            }
            if (op2 is PICOperandFast fast)
            {
                writer.WriteString(fast.IsFast ? ",FAST" : "");
            }
        }

    }

    public class PICInstructionWithFSR : PICInstruction
    {
        public PICInstructionWithFSR(Opcode opcode, ushort fsrnum, sbyte value, FSRIndexedMode mode)
            : base(opcode, new PICOperandFSRIndexation(fsrnum, Constant.SByte(value), mode))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var fsridx = op1 as PICOperandFSRIndexation ?? throw new InvalidOperationException($"Invalid FSR index operand: {op1}");
            var fsrnum = fsridx.FSRNum;

            writer.WriteOpcode(Opcode.ToString());
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

        public PICInstructionTbl(Opcode opcode, ushort mode)
            : base(opcode, new PICOperandTBLRW(mode))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            op1.Write(writer, options);
        }

    }

    public class PICInstructionTris : PICInstruction
    {

        public PICInstructionTris(Opcode opcode, byte trisnum)
            : base(opcode,
                   new PICOperandTris(trisnum))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            op1.Write(writer, options);
        }

    }

    public class PICInstructionPseudo : PICInstruction
    {

        public PICInstructionPseudo(Opcode opcode, PICOperandPseudo pseudoOperand)
            : base(opcode, pseudoOperand)
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            op1.Write(writer, options);
        }

    }

}
