#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Core;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.Common
{
    public abstract class PICInstruction : MachineInstruction
    {

        public const InstructionClass CondLinear = InstructionClass.Conditional | InstructionClass.Linear;
        public const InstructionClass CondTransfer = InstructionClass.Conditional | InstructionClass.Transfer;
        public const InstructionClass LinkTransfer = InstructionClass.Call | InstructionClass.Transfer;
        public const InstructionClass Transfer = InstructionClass.Transfer;

        public readonly MachineOperand op1;
        public readonly MachineOperand op2;
        public readonly MachineOperand op3;

        private static Dictionary<Opcode, InstructionClass> classOf = new Dictionary<Opcode, InstructionClass>()
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
                { Opcode.CONFIG,    InstructionClass.None },
                { Opcode.DA,        InstructionClass.None },
                { Opcode.DB,        InstructionClass.None },
                { Opcode.DE,        InstructionClass.None },
                { Opcode.DT,        InstructionClass.None },
                { Opcode.DTM,       InstructionClass.None },
                { Opcode.DW,        InstructionClass.None },
                { Opcode.__CONFIG,  InstructionClass.None },
                { Opcode.__IDLOCS,  InstructionClass.None },
                { Opcode.invalid,   InstructionClass.Invalid },
                { Opcode.unaligned, InstructionClass.Invalid },
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
        /// Returns true if the instruction is valid.
        /// </summary>
        /// <value>
        /// True if this instruction is valid, false if not.
        /// </value>
        public override bool IsValid => (Opcode != Opcode.invalid && Opcode != Opcode.unaligned);

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

        /// <summary>
        /// The control-flow kind of the instruction.
        /// </summary>
        public override InstructionClass InstructionClass
        {
            get
            {
                if (!classOf.TryGetValue(Opcode, out InstructionClass il))
                    il = InstructionClass.Linear;
                return il;
            }
        }

        /*
    public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
    {
        writer.WriteOpcode(Opcode.ToString());
        if (op1 is null)
            return;
        if (op1 is IOperandShadow opshad1)
        {
            if (opshad1.IsPresent)
            {
                writer.WriteString(",");
                writer.Tab();
                op1.Write(writer, options);
            }
            return;
        }
        if (op1 is PICFastOperand)
        {
            op1.Write(writer, options);
            return;
        }
        writer.Tab();
        op1.Write(writer, options);
        if (op2 is null)
            return;
        if (op2 is IOperandShadow opshad2)
        {
            if (opshad2.IsPresent)
            {
                writer.WriteString(",");
                op2.Write(writer, options);
            }
            return;
        }
        if (op2 is PICFastOperand)
        {
            op2.Write(writer, options);
            return;
        }
        writer.WriteString(",");
        op2.Write(writer, options);
    }
*/
    }

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

    public class PICInstructionImmedByte : PICInstruction
    {
        public PICInstructionImmedByte(Opcode opcode, ushort imm)
            : base(opcode,
                   new PICOperandImmediate(imm, PrimitiveType.Byte))
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
            : base(opcode,
                   new PICOperandImmediate(imm, PrimitiveType.SByte))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            op1.Write(writer, options);
        }

    }

    public class PICInstructionImmedUShort : PICInstruction
    {
        public PICInstructionImmedUShort(Opcode opcode, ushort imm)
            : base(opcode,
                   new PICOperandImmediate(imm, PrimitiveType.UInt16))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            op1.Write(writer, options);
        }

    }

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

    public class PICInstructionMem : PICInstruction
    {
        public PICInstructionMem(Opcode opcode, ushort memaddr)
            : base(opcode,
                   new PICOperandOffsetBankedMemory(Constant.Byte((byte)memaddr)))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var bankmem = op1 as PICOperandOffsetBankedMemory ?? throw new InvalidOperationException($"Invalid banked memory operand: {op1}");

            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();

            if (PICRegisters.TryGetAlwaysAccessibleRegister(bankmem.Offset.ToUInt16(), out var reg))
            {
                writer.WriteString($"{reg.Name}");
            }
            else
            {
                op1.Write(writer, options);
            }
        }

    }

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
                   new PICOperandFSRIndexation(Constant.Byte(srcidx), FSRIndexedMode.FSR2INDEXED),
                   new PICOperandDataMemoryAddress(dstaddr))
        {
        }

        public PICInstructionMem2Mem(Opcode opcode, byte srcidx, byte dstidx)
            : base(opcode,
                   new PICOperandFSRIndexation(Constant.Byte(srcidx), FSRIndexedMode.FSR2INDEXED),
                   new PICOperandFSRIndexation(Constant.Byte(dstidx), FSRIndexedMode.FSR2INDEXED))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();

            switch (op1)
            {
                case PICOperandDataMemoryAddress srcmem:
                    var asrcmem = PICMemoryDescriptor.RemapDataAddress(srcmem.DataTarget);
                    if (PICRegisters.TryGetRegister(asrcmem, out var srcreg))
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
                    writer.WriteString($"[{srcidx.Offset}]");
                    break;
            }

            writer.WriteString(",");

            switch (op2)
            {
                case PICOperandDataMemoryAddress dstmem:
                    var adstmem = PICMemoryDescriptor.RemapDataAddress(dstmem.DataTarget);
                    if (PICRegisters.TryGetRegister(adstmem, out var dstreg))
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
                    writer.WriteString($"[{dstidx.Offset}]");
                    break;
            }

        }

    }

    public class PICInstructionMemAccess : PICInstruction
    {
        public PICInstructionMemAccess(Opcode opcode, ushort memaddr, ushort acc)
            : base(opcode,
                   new PICOperandOffsetBankedMemory(Constant.Byte((byte)memaddr)),
                   new PICOperandIsAccessBank(acc))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var bankmem = op1 as PICOperandOffsetBankedMemory ?? throw new InvalidOperationException($"Invalid banked memory operand: {op1}");
            var acc = op2 as PICOperandIsAccessBank ?? throw new InvalidOperationException($"Invalid access flag operand: {op2}");

            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();

            if (acc.IsAccessBank.ToBoolean())
            {
                var remapAdr = PICMemoryDescriptor.RemapDataAddress(bankmem.Offset.ToUInt16());
                if (PICRegisters.TryGetRegister(remapAdr, out var areg, 8))
                {
                    writer.WriteString($"{areg.Name},");
                }
                else
                {
                    bankmem.Write(writer, options);
                }
            }
            else if (PICRegisters.TryGetAlwaysAccessibleRegister(bankmem.Offset.ToUInt16(), out var reg))
            {
                writer.WriteString($"{reg.Name}");
            }
            else
            {
                bankmem.Write(writer, options);
            }
            acc.Write(writer, options);
        }

    }

    public class PICInstructionMemWregDest : PICInstruction
    {
        public PICInstructionMemWregDest(Opcode opcode, ushort memaddr, ushort dest)
            : base(opcode,
                   new PICOperandOffsetBankedMemory(Constant.Byte((byte)memaddr)),
                   new PICOperandWregDest(dest))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var bankmem = op1 as PICOperandOffsetBankedMemory ?? throw new InvalidOperationException($"Invalid banked memory operand: {op1}");
            var dest = op2 as PICOperandWregDest ?? throw new InvalidOperationException($"Invalid W/F destination operand: {op1}");

            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();

            if (PICRegisters.TryGetAlwaysAccessibleRegister(bankmem.Offset.ToUInt16(), out var reg))
            {
                writer.WriteString($"{reg.Name}");
            }
            else
            {
                op1.Write(writer, options);
            }
            op2.Write(writer, options);
        }

    }

    public class PICInstructionMemWregDestAccess : PICInstruction
    {
        public PICInstructionMemWregDestAccess(Opcode opcode, ushort memaddr, ushort dest, ushort acc)
            : base(opcode,
                   new PICOperandOffsetBankedMemory(Constant.Byte((byte)memaddr)),
                   new PICOperandWregDest(dest),
                   new PICOperandIsAccessBank(acc))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var bankmem = op1 as PICOperandOffsetBankedMemory ?? throw new InvalidOperationException($"Invalid banked memory operand: {op1}");
            var dest = op2 as PICOperandWregDest ?? throw new InvalidOperationException($"Invalid W/F destination operand: {op1}");
            var acc = op3 as PICOperandIsAccessBank ?? throw new InvalidOperationException($"Invalid access flag operand: {op3}");

            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();

            if (acc.IsAccessBank.ToBoolean())
            {
                var remapAdr = PICMemoryDescriptor.RemapDataAddress(bankmem.Offset.ToUInt16());
                if (PICRegisters.TryGetRegister(remapAdr, out var areg, 8))
                {
                    writer.WriteString($"{areg.Name},");
                }
                else
                {
                    bankmem.Write(writer, options);
                }
            }
            else if (PICRegisters.TryGetAlwaysAccessibleRegister(bankmem.Offset.ToUInt16(), out var reg))
            {
                writer.WriteString($"{reg.Name}");
            }
            else
            {
                bankmem.Write(writer, options);
            }
            dest.Write(writer, options);
            acc.Write(writer, options);
        }

    }

    public class PICInstructionMemBit : PICInstruction
    {
        public PICInstructionMemBit(Opcode opcode, ushort memaddr, byte bitno)
            : base(opcode,
                   new PICOperandOffsetBankedMemory(Constant.Byte((byte)memaddr)),
                   new PICOperandImmediate(bitno, PrimitiveType.Byte))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var bankmem = op1 as PICOperandOffsetBankedMemory ?? throw new InvalidOperationException($"Invalid banked memory operand: {op1}");
            var bitno = op2 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid bit number operand: {op2}");

            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();

            if (PICRegisters.TryGetAlwaysAccessibleRegister(bankmem.Offset.ToUInt16(), out var reg))
            {
                if (PICRegisters.TryGetBitField(reg, out var fld, bitno.ImmediateValue.ToByte(), 1))
                {
                    writer.WriteString($"{reg.Name},{fld.Name}");
                    return;
                }
                writer.WriteString($"{reg.Name}");
            }
            else
            {
                bankmem.Write(writer, options);
            }
            writer.WriteString(",");
            bitno.Write(writer, options);
        }

    }

    public class PICInstructionMemBitAccess : PICInstruction
    {
        public PICInstructionMemBitAccess(Opcode opcode, ushort memaddr, ushort bitno, ushort acc)
            : base(opcode,
                   new PICOperandOffsetBankedMemory(Constant.Byte((byte)memaddr)),
                   new PICOperandImmediate(bitno, PrimitiveType.Byte),
                   new PICOperandIsAccessBank(acc))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var bankmem = op1 as PICOperandOffsetBankedMemory ?? throw new InvalidOperationException($"Invalid banked memory operand: {op1}");
            var bitno = op2 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid bit number operand: {op2}");
            var acc = op3 as PICOperandIsAccessBank ?? throw new InvalidOperationException($"Invalid access flag operand: {op3}");

            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();

            if (acc.IsAccessBank.ToBoolean())
            {
                var remapAdr = PICMemoryDescriptor.RemapDataAddress(bankmem.Offset.ToUInt16());
                if (PICRegisters.TryGetRegister(remapAdr, out var areg, 8))
                {
                    writer.WriteString($"{areg.Name},");
                    if (PICRegisters.TryGetBitField(areg, out var fld, bitno.ImmediateValue.ToByte(), 1))
                    {
                        writer.WriteString($"{fld.Name}");
                    }
                    else
                    {
                        bitno.Write(writer, options);
                    }
                }
                else
                {
                    bankmem.Write(writer, options);
                    writer.WriteString(",");
                    bitno.Write(writer, options);

                }
            }
            else if (PICRegisters.TryGetAlwaysAccessibleRegister(bankmem.Offset.ToUInt16(), out var reg))
            {
                writer.WriteString($"{reg.Name},");
                if (PICRegisters.TryGetBitField(reg, out var fld, bitno.ImmediateValue.ToByte(), 1))
                {
                    writer.WriteString($"{fld.Name}");
                }
                else
                {
                    bitno.Write(writer, options);
                }
            }
            else
            {
                bankmem.Write(writer, options);
                writer.WriteString(",");
                bitno.Write(writer, options);
            }
            acc.Write(writer, options);
        }

    }

    public class PICInstructionProgTarget : PICInstruction
    {
        public PICInstructionProgTarget(Opcode opcode, uint progAdr)
            : base(opcode,
                   new PICOperandProgMemoryAddress(progAdr))
        {
        }

        public PICInstructionProgTarget(Opcode opcode, short progOff, Address instrAdr)
            : base(opcode,
                   new PICOperandProgMemoryAddress(progOff, instrAdr))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            op1.Write(writer, options);
        }

    }

    public class PICInstructionFast : PICInstruction
    {
        public PICInstructionFast(Opcode opcode, ushort fast, bool wTab)
            : base(opcode,
                   new PICOperandFast(fast, wTab))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
            op1.Write(writer, options);
        }

    }

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
            op1.Write(writer, options);
            op2.Write(writer, options);
        }

    }

    public class PICInstructionWithFSR : PICInstruction
    {
        public PICInstructionWithFSR(Opcode opcode, PICRegisterStorage fsr, ushort value)
            : base(opcode,
                   new PICOperandRegister(fsr),
                   new PICOperandImmediate(value, PrimitiveType.UInt16))
        {
        }

        public PICInstructionWithFSR(Opcode opcode, PICRegisterStorage fsr, short value)
            : base(opcode,
                   new PICOperandRegister(fsr),
                   new PICOperandImmediate(value, PrimitiveType.Int16))
        {
        }

        public PICInstructionWithFSR(Opcode opcode, PICRegisterStorage fsr, short value, FSRIndexedMode mode)
            : base(opcode,
                   new PICOperandRegister(fsr),
                   new PICOperandFSRIndexation(Constant.Create(PrimitiveType.SByte, value), mode))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var fsrreg = op1 as PICOperandRegister ?? throw new InvalidOperationException($"Invalid FSR operand: {op1}");

            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();

            switch (Opcode)
            {
                case Opcode.ADDFSR:
                case Opcode.LFSR:
                case Opcode.SUBFSR:
                    var imm = op2 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {op2}");
                    fsrreg.Write(writer, options);
                    writer.WriteString(",");
                    imm.Write(writer, options);
                    return;

                case Opcode.ADDULNK:
                case Opcode.SUBULNK:
                    var immu = op2 as PICOperandImmediate ?? throw new InvalidOperationException($"Invalid immediate operand: {op2}");
                    immu.Write(writer, options);
                    return;

                case Opcode.MOVIW:
                case Opcode.MOVWI:
                    var idx = op2 as PICOperandFSRIndexation ?? throw new InvalidOperationException($"Invalid indexation operand: {op2}");
                    var fsrname = fsrreg.Register.Name;
                    switch (idx.Mode)
                    {
                        case FSRIndexedMode.INDEXED:
                            writer.WriteString($"{idx.Offset}[{fsrname}]");
                            return;
                        case FSRIndexedMode.POSTDEC:
                            writer.WriteString($"{fsrname}--");
                            return;
                        case FSRIndexedMode.POSTINC:
                            writer.WriteString($"{fsrname}++");
                            return;
                        case FSRIndexedMode.PREDEC:
                            writer.WriteString($"--{fsrname}");
                            return;
                        case FSRIndexedMode.PREINC:
                            writer.WriteString($"++{fsrname}");
                            return;
                    }
                    throw new InvalidOperationException($"Invalid indexation '{idx}' for MOVI instruction.");
            }

            throw new InvalidOperationException($"Invalid opcode '{Opcode}' for FSR-related instruction.");
        }

    }

    public class PICInstructionTbl : PICInstruction
    {

        public PICInstructionTbl(Opcode opcode, ushort mode)
            : base(opcode,
                   new PICOperandTBLRW(mode))
        {
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteOpcode(Opcode.ToString());
            writer.Tab();
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
