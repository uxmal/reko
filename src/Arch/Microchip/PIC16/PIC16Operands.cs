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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Linq;

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    /// <summary>
    /// A PIC16 Bank Data Memory Address operand (like "f").
    /// </summary>
    public class PIC16BankedOperand : MachineOperand, IOperand
    {

        /// <summary>
        /// Gets the 7-bit memory address.
        /// </summary>
        public readonly byte BankAddr;

        public PIC16BankedOperand(byte addr) : base(PrimitiveType.Byte)
        {
            BankAddr = (byte)(addr & 0x7F);
        }

        public virtual void Accept(IPIC16OperandVisitor visitor) => visitor.VisitDataBanked(this);
        public virtual T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitDataBanked(this);
        public virtual T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitDataBanked(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            ushort uaddr = BankAddr;

            var aaddr = PICMemoryDescriptor.RemapDataAddress(uaddr);
            if (PICRegisters.TryGetRegister(aaddr, out var sfr))
            {
                writer.WriteString($"{sfr.Name}");
                return;
            }
            writer.WriteString($"0x{uaddr:X2}");
        }

    }

    /// <summary>
    /// A PIC16 data memory bit  with destination operand (like "f,b").
    /// Used by Bit-oriented instructions (BCF, BTFSS, ...)
    /// </summary>
    public class PIC16DataBitOperand : PIC16BankedOperand
    {
        /// <summary>
        /// Gets the bit number (value between 0 and 7).
        /// </summary>
        /// <value>
        /// The bit number.
        /// </value>
        public readonly Constant BitNumber;

        /// <summary>
        /// Instantiates a bit number operand. Used by Bit-oriented instructions (BCF, BTFSS, ...).
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC16DataBitOperand(byte addr, byte b) : base(addr)
        {
            BitNumber = Constant.Byte(b);
        }

        public override void Accept(IPIC16OperandVisitor visitor) => visitor.VisitDataBit(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitDataBit(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitDataBit(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            byte uaddr = BankAddr;
            byte bitpos = BitNumber.ToByte();

            base.Write(writer, options);

            var aaddr = PICMemoryDescriptor.RemapDataAddress(uaddr);
            if (PICRegisters.TryGetRegister(aaddr, out var sfr))
            {
                if (PICRegisters.TryGetBitField(aaddr, out var bitname, bitpos, 1))
                {
                    writer.WriteString($",{bitname.Name}");
                    return;
                }
                writer.WriteString($",{bitpos}");
                return;
            }

            writer.WriteString($",{bitpos}");

        }

    }

    /// <summary>
    /// A PIC16 Bank Data Memory Address with destination operand (like "f,d").
    /// </summary>
    public class PIC16DataByteWithDestOperand : PIC16BankedOperand
    {

        /// <summary>
        /// Gets the indication if the Working Register is the destination of the operation.
        /// If false, the File Register designated by the address is the destination.
        /// </summary>
        public readonly Constant WregIsDest;

        public PIC16DataByteWithDestOperand(byte addr, int d) : base(addr)
        {
            WregIsDest = Constant.Bool(d == 0);
        }

        public override void Accept(IPIC16OperandVisitor visitor) => visitor.VisitDataByte(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitDataByte(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitDataByte(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            base.Write(writer, options);
            writer.WriteString((WregIsDest.ToBoolean() ? ",W" : ",F"));
        }

    }

    public abstract class PIC16FSROperand : MachineOperand, IOperand
    {
        /// <summary>
        /// Gets the FSR register number.
        /// </summary>
        public readonly Constant FSRNum;

        public PIC16FSROperand(byte fsrnum) : base(PrimitiveType.Byte)
        {
            FSRNum = Constant.Byte(fsrnum);
        }

        public abstract void Accept(IPIC16OperandVisitor visitor);
        public abstract T Accept<T>(IOperandVisitor<T> visitor);
        public abstract T Accept<T, C>(IOperandVisitor<T, C> visitor, C context);

    }

    /// <summary>
    /// A PIC16 FSRn arithmetic operand. Used by ADDFSR instructions.
    /// </summary>
    public class PIC16FSRArithOperand : PIC16FSROperand
    {
        /// <summary>
        /// Gets the 6-bit signed offset [-32..31].
        /// </summary>
        public readonly Constant Offset;

        /// <summary>
        /// Instantiates a FSRn immediate operand. Used by ADDFSR instructions.
        /// </summary>
        /// <param name="fsrnum">The FSR register number [0, 1].</param>
        public PIC16FSRArithOperand(byte fsrnum, sbyte off) : base(fsrnum)
        {
            Offset = Constant.SByte(off);
        }

        public override void Accept(IPIC16OperandVisitor visitor) => visitor.VisitFSRArith(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitFSRArith(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitFSRArith(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            byte num = FSRNum.ToByte();
            short off = Offset.ToInt16();
            if (off < 0)
                writer.WriteString($"FSR{num},-0x{-off:X}");
            else
                writer.WriteString($"FSR{num},0x{off:X}");
        }

    }

    /// <summary>
    /// A PIC16 FSRn indirect indexed operand. Used by MOVIW, MOVWI instructions.
    /// </summary>
    public class PIC16FSRIndexedOperand : PIC16FSROperand
    {
        /// <summary>
        /// Gets the 6-bit signed offset [-32..31].
        /// </summary>
        public readonly Constant Offset;

        /// <summary>
        /// Instantiates a FSRn indirect indexed operand. Used by MOVIW, MOVWI instructions.
        /// </summary>
        /// <param name="fsrnum">The FSR register number [0, 1].</param>
        public PIC16FSRIndexedOperand(byte fsrnum, sbyte off) : base(fsrnum)
        {
            Offset = Constant.SByte(off);
        }

        public override void Accept(IPIC16OperandVisitor visitor) => visitor.VisitFSRIndexed(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitFSRIndexed(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitFSRIndexed(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            byte num = FSRNum.ToByte();
            short off = Offset.ToInt16();
            if (off < 0)
                writer.WriteString($"-0x{-off:X}[{num}]");
            else
                writer.WriteString($"0x{off:X}[{num}]");
        }

    }

    /// <summary>
    /// Values that represent the PIC16 FSR Increment/Decrement modes.
    /// </summary>
    public enum FSRIncDecMode : byte
    {
        PreInc = 0,
        PreDec = 1,
        PostInc = 2,
        PostDec = 3
    };

    /// <summary>
    /// A PIC16 FSR register increment/decrement operand. Used by MOVIW, MOVWI instructions.
    /// </summary>
    public class PIC16FSRIncDecOperand : PIC16FSROperand
    {
        /// <summary>
        /// Gets the FSR increment/decrement mode.
        /// </summary>
        public readonly FSRIncDecMode FSRIncDecMode;

        /// <summary>
        /// Instantiates a FSRn register operand. Used by LFSR, ADDFSR, SUBFSR instructions.
        /// </summary>
        /// <param name="incdecmode">The FSR register number [0, 1, 2].</param>
        public PIC16FSRIncDecOperand(byte fsrnum, byte incdecmode) : base(fsrnum)
        {
            FSRIncDecMode = (FSRIncDecMode)incdecmode;
        }

        public override void Accept(IPIC16OperandVisitor visitor) => visitor.VisitIncDecFSR(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitIncDecFSR(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitIncDecFSR(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            string fsr = $"FSR{FSRNum.ToByte()}";
            switch (FSRIncDecMode)
            {
                case FSRIncDecMode.PreInc:
                    writer.WriteString($"++{fsr}");
                    break;
                case FSRIncDecMode.PreDec:
                    writer.WriteString($"--{fsr}");
                    break;
                case FSRIncDecMode.PostInc:
                    writer.WriteString($"{fsr}++");
                    break;
                case FSRIncDecMode.PostDec:
                    writer.WriteString($"{fsr}--");
                    break;
            }
        }

    }

    /// <summary>
    /// A PIC16 TRIS register index. Used by TRIS instruction.
    /// </summary>
    public class PIC16TrisNumOperand : MachineOperand, IOperand
    {
        /// <summary>
        /// Gets the TRIS register index [5-7].
        /// </summary>
        public readonly Constant TrisNum;

        /// <summary>
        /// Instantiates a TRIS register operand. Used by TRIS instruction.
        /// </summary>
        /// <param name="trisnum">The TRIS register number [5, 6, 7].</param>
        public PIC16TrisNumOperand(byte trisnum) : base(PrimitiveType.Byte)
        {
            TrisNum = Constant.Byte(trisnum);
        }

        public virtual void Accept(IPIC16OperandVisitor visitor) => visitor.VisitTrisNum(this);
        public virtual T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitTrisNum(this);
        public virtual T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitTrisNum(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            switch (TrisNum.ToByte())
            {
                case 5:
                    writer.WriteString("A");
                    break;
                case 6:
                    writer.WriteString("B");
                    break;
                case 7:
                    writer.WriteString("C");
                    break;
            }
        }

    }

}
