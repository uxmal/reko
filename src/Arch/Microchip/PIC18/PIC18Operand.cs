#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet
 * inspired by work of
 * Copyright (C) 1999-2017 John Källén.
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

using Microchip.Crownking;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Microchip.PIC18
{
    /// <summary>
    /// Interface for PIC18 Operands' visitors.
    /// </summary>
    public interface IPIC18kOperand
    {
        T Accept<T>(IPIC18OperandVisitor<T> visitor);
    }

    /// <summary>
    /// Interface defining the permitted visitors on PIC18 Operands.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IPIC18OperandVisitor<T>
    {
        T Visit(PIC18Immed4Operand imm4);
        T Visit(PIC18Immed6Operand imm6);
        T Visit(PIC18Immed8Operand imm8);
        T Visit(PIC18Immed12Operand imm12);
        T Visit(PIC18Immed14Operand imm14);
        T Visit(PIC18FSR2IdxOperand imm8);
        T Visit(PIC18Memory12bitAbsAddrOperand addr12);
        T Visit(PIC18Memory14bitAbsAddrOperand addr14);
        T Visit(PIC18DataBitAccessOperand bitno);
        T Visit(PIC18DataBankAccessOperand mem);
        T Visit(PIC18DataByteAccessWithDestOperand mem);
        T Visit(PIC18ProgRel8AddrOperand roff8);
        T Visit(PIC18ProgRel11AddrOperand roff11);
        T Visit(PIC18ProgAbsAddrOperand addr20);
        T Visit(PIC18FSRNumOperand fsrnum);
        T Visit(PIC18ShadowOperand shad);
        T Visit(PIC18TableReadWriteOperand tblmode);

    }

    /// <summary>
    /// A PIC18 operand with visitor mechanism interface.
    /// </summary>
    public abstract class PIC18OperandImpl : MachineOperand, IPIC18kOperand
    {
        public PICExecMode ExecMode { get; }

        /// <summary>
        /// Specialised constructor for use only by derived class.
        /// </summary>
        /// <param name="dataWidth">Width of the data.</param>
        protected PIC18OperandImpl(PrimitiveType dataWidth, PICExecMode execmod)
            : base(dataWidth)
        {
            ExecMode = execmod;
        }

        public abstract T Accept<T>(IPIC18OperandVisitor<T> visitor);

        public virtual bool IsVisible => true;

    }

    /// <summary>
    /// A PIC18 4-bit unsigned immediate operand. Used by MOVLB instruction.
    /// </summary>
    public class PIC18Immed4Operand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 4-bit immediate constant.
        /// </summary>
        public Constant Imm4 { get; }

        /// <summary>
        /// Instantiates a 4-bit unsigned immediate operand. Used by MOVLB instruction.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18Immed4Operand(PICExecMode mode, byte b) : base(PrimitiveType.Byte, mode)
        {
            Imm4 = Constant.Byte(b);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{Imm4.ToByte():X2}");
        }

    }

    /// <summary>
    /// A PIC18 6-bit unsigned immediate operand. Used by ADDFSR, SUBFSR, MOVLB instructions.
    /// </summary>
    public class PIC18Immed6Operand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 6-bit immediate constant.
        /// </summary>
        public Constant Imm6 { get; }

        /// <summary>
        /// Instantiates a 6-bit unsigned immediate operand. Used by ADDFSR, SUBFSR instructions.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18Immed6Operand(PICExecMode mode, byte b) : base(PrimitiveType.Byte, mode)
        {
            Imm6 = Constant.Byte(b);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{Imm6.ToByte():X2}");
        }

    }

    /// <summary>
    /// A PIC18 8-bit unsigned immediate operand. Used by immediate instructions (ADDLW, SUBLW, RETLW, ...)
    /// </summary>
    public class PIC18Immed8Operand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 8-bit immediate constant.
        /// </summary>
        public Constant Imm8 { get; }

        /// <summary>
        /// Instantiates a 8-bit unsigned immediate operand. Used by immediate instructions (ADDLW, SUBLW, RETLW, ...)
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18Immed8Operand(PICExecMode mode, byte b) : base(PrimitiveType.Byte, mode)
        {
            Imm8 = Constant.Byte(b);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"{Imm8}");
        }

    }

    /// <summary>
    /// A PIC18 12-bit unsigned immediate operand. Used by LFSR instruction.
    /// </summary>
    public class PIC18Immed12Operand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 12-bit immediate constant.
        /// </summary>
        public Constant Imm12 { get; }

        /// <summary>
        /// Instantiates a 12-bit unsigned immediate operand. Used by LFSR instruction.
        /// </summary>
        /// <param name="w">An ushort to process.</param>
        public PIC18Immed12Operand(PICExecMode mode, ushort w) : base(PrimitiveType.UInt16, mode)
        {
            Imm12 = Constant.Word16(w);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{Imm12.ToUInt16():X3}");
        }

    }

    /// <summary>
    /// A PIC18 14-bit unsigned immediate operand. Uused by LFSR instruction - PIC18 Enhanced.
    /// </summary>
    public class PIC18Immed14Operand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 14-bit immediate constant.
        /// </summary>
        public Constant Imm14 { get; }

        /// <summary>
        /// Instantiates a 14-bit unsigned immediate operand. Used by LFSR instruction - PIC18 Enhanced.
        /// </summary>
        /// <param name="w">An ushort to process.</param>
        public PIC18Immed14Operand(PICExecMode mode, ushort w) : base(PrimitiveType.UInt16, mode)
        {
            Imm14 = Constant.Word16(w);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{Imm14.ToUInt16():X4}");
        }

    }

    /// <summary>
    /// A PIC18 7-bit unsigned offset operand to FSR2. Used by MOVSF, MOVSFL, MOVSS instructions.
    /// </summary>
    public class PIC18FSR2IdxOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 7-bit unsigned offset constant.
        /// </summary>
        public Constant Offset { get; }

        /// <summary>
        /// Instantiates a 7-bit unsigned offset operand. Used by MOVSF, MOVSFL, MOVSS instructions.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18FSR2IdxOperand(PICExecMode mode, byte b) : base(PrimitiveType.Byte, mode)
        {
            Offset = Constant.Byte(b);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"[0x{Offset.ToByte():X2}]");
        }

    }

    /// <summary>
    /// A PIC18 12-bit data memory absolute address operand (used by MOVFF, MOVSF instruction)
    /// </summary>
    public class PIC18Memory12bitAbsAddrOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 12-bit data memory address.
        /// </summary>
        public AddressOperand DataTarget { get; }

        public PIC18Memory12bitAbsAddrOperand(PICExecMode mode, ushort w) : base(PrimitiveType.UInt16, mode)
        {
            DataTarget = AddressOperand.Ptr16(w);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{DataTarget.Address.ToUInt16():X4}");
        }

    }

    /// <summary>
    /// A PIC18 14-bit data memory absolute address operand (used by MOVFFL, MOVSFL instruction)
    /// </summary>
    public class PIC18Memory14bitAbsAddrOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 14-bit data memory address.
        /// </summary>
        public AddressOperand DataTarget { get; }

        /// <summary>
        /// Instantiates a 14-bit data memory address operand (used by MOVFFL, MOVSFL instruction)
        /// </summary>
        /// <param name="w">An ushort to process.</param>
        public PIC18Memory14bitAbsAddrOperand(PICExecMode mode, ushort w) : base(PrimitiveType.UInt16, mode)
        {
            DataTarget = AddressOperand.Ptr16(w);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{DataTarget.Address.ToUInt16():X4}");
        }

    }

    /// <summary>
    /// A PIC18 Registers Shadowing flag operand. Used by instructions RETFIE, RETURN, CALL.
    /// </summary>
    public class PIC18ShadowOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the indication if a Shadow Register(s) are used. If false, no Shadow Register(s) used.
        /// </summary>
        public Constant IsShadow { get; }

        /// <summary>
        /// Instantiates a Registers Shadowing flag operand. Used by instructions RETFIE, RETURN, CALL.
        /// </summary>
        /// <param name="s">An int to process.</param>
        public PIC18ShadowOperand(PICExecMode mode, uint s) : base(PrimitiveType.Bool, mode)
        {
            IsShadow = Constant.Bool(s == 1);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write(IsShadow.ToBoolean() ? "S" : "");
        }

        public override bool IsVisible => IsShadow.ToBoolean();

    }

    /// <summary>
    /// A PIC18 Bank/Access Data Memory Address operand (like "f,a").
    /// </summary>
    public class PIC18DataBankAccessOperand : PIC18OperandImpl
    {

        /// <summary>
        /// Gets the 8-bit memory address.
        /// </summary>
        public AddressOperand MemAddr { get; }

        /// <summary>
        /// Gets the indication if RAM location is in Access RAM or specified by BSR register.
        /// </summary>
        public Constant IsAccessRAM { get; }

        public PIC18DataBankAccessOperand(PICExecMode mode, byte addr, int a) : base(PrimitiveType.Byte, mode)
        {
            MemAddr = AddressOperand.Ptr16(addr);
            IsAccessRAM = Constant.Bool(a == 0);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (ExecMode == PICExecMode.Extended && IsAccessRAM.ToBoolean() && (MemAddr.Address.ToUInt16() < 0x60))
            {
                writer.Write($"[0x{MemAddr.Address.ToUInt16():X2}]");
            }
            else
            {
                writer.Write($"0x{MemAddr.Address.ToUInt16():X2}{(IsAccessRAM.ToBoolean() ? ",ACCESS" : "")}");
            }
        }

    }

    /// <summary>
    /// A PIC18 data memory bit access with destination operand (like "f,b,a").
    /// Used by Bit-oriented instructions (BCF, BTFSS, ...)
    /// </summary>
    public class PIC18DataBitAccessOperand : PIC18DataBankAccessOperand
    {
         /// <summary>
        /// Gets the bit number (value between 0 and 7).
        /// </summary>
        /// <value>
        /// The bit number.
        /// </value>
        public Constant BitNumber { get; }

        /// <summary>
        /// Instantiates a bit number operand. Used by Bit-oriented instructions (BCF, BTFSS, ...).
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18DataBitAccessOperand(PICExecMode mode, byte addr, int a, byte b) : base(mode,addr, a)
        {
            BitNumber = Constant.Byte(b);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (ExecMode == PICExecMode.Extended && IsAccessRAM.ToBoolean() && (MemAddr.Address.ToUInt16() < 0x60))
            {
                writer.Write($"[0x{MemAddr.Address.ToUInt16():X2}],{BitNumber.ToByte()}");
            }
            else
            {
                writer.Write($"0x{MemAddr.Address.ToUInt16():X2},{BitNumber.ToByte()}{(IsAccessRAM.ToBoolean() ? ",ACCESS" : "")}");
            }

        }

    }

    /// <summary>
    /// A PIC18 Bank Data Memory Address with destination operand (like "f,d,a").
    /// </summary>
    public class PIC18DataByteAccessWithDestOperand : PIC18DataBankAccessOperand
    {

        /// <summary>
        /// Gets the indication if the Working Register is the destination of the operation. If false, the File Register is the destination.
        /// </summary>
        public Constant WregIsDest { get; }

        public PIC18DataByteAccessWithDestOperand(PICExecMode mode, byte addr, int a, int d) : base(mode, addr, a)
        {
            WregIsDest = Constant.Bool(d == 0);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (ExecMode == PICExecMode.Extended && IsAccessRAM.ToBoolean() && (MemAddr.Address.ToUInt16() < 0x60))
            {
                writer.Write($"[0x{MemAddr.Address.ToUInt16():X2}]{(WregIsDest.ToBoolean() ? ",W" : "")}");
            }
            else
            {
                writer.Write($"0x{MemAddr.Address.ToUInt16():X2}{(WregIsDest.ToBoolean() ? ",W" : "")}{(IsAccessRAM.ToBoolean() ? ",ACCESS" : "")}");
            }

        }

    }

    /// <summary>
    /// A PIC18 8-bit relative code offset operand.
    /// </summary>
    public class PIC18ProgRel8AddrOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the relative offset. This a word offset.
        /// </summary>
        public Constant RelativeWordOffset { get; }

        /// <summary>
        /// Gets the target absolute code address. This is a byte address.
        /// </summary>
        public AddressOperand CodeTarget { get; }

        /// <summary>
        /// Instantiates a 8-bit relative code offset operand.
        /// </summary>
        /// <param name="off">The 8-bit signed offset value. (word offset)</param>
        /// <param name="instrAddr">The instruction address.</param>
        public PIC18ProgRel8AddrOperand(PICExecMode mode, sbyte off, Address instrAddr) : base(PrimitiveType.SByte, mode)
        {
            RelativeWordOffset = Constant.SByte(off);
            uint absaddr = (uint)((long)instrAddr.ToUInt32() + 1 + off) & 0xFFFFFU;
            CodeTarget = AddressOperand.Ptr32(absaddr << 1); 
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{CodeTarget.Address.ToUInt32():X6}");
        }

    }

    /// <summary>
    /// A PIC18 11-bit code relative offset operand.
    /// </summary>
    public class PIC18ProgRel11AddrOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the relative offset. This a word offset.
        /// </summary>
        public Constant RelativeWordOffset { get; }

        /// <summary>
        /// Gets the target absolute code address. This is a byte address.
        /// </summary>
        public AddressOperand CodeTarget { get; }

        /// <summary>
        /// Instantiates a 11-bit code relative offset operand.
        /// </summary>
        /// <param name="off">The off.</param>
        /// <param name="instrAddr">The instruction address.</param>
        public PIC18ProgRel11AddrOperand(PICExecMode mode, short off, Address instrAddr) : base(PrimitiveType.Int16, mode)
        {
            RelativeWordOffset = Constant.Int16(off);
            uint absaddr = (uint)((long)instrAddr.ToUInt32() + 1 + off) & 0xFFFFFU;
            CodeTarget = AddressOperand.Ptr32(absaddr << 1);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{CodeTarget.Address.ToUInt32():X6}");
        }

    }

    /// <summary>
    /// A PIC18 Absolute Code Address operand. Used by GOTO, CALL instructions.
    /// </summary>
    public class PIC18ProgAbsAddrOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the Code Memory Address. This is a byte address (21-bit, bit 0 = 0).
        /// </summary>
        public AddressOperand CodeTarget { get; }

        /// <summary>
        /// Instantiates a Absolute Code Address operand. Used by GOTO, CALL instructions.
        /// </summary>
        /// <param name="addr">The program word address.</param>
        public PIC18ProgAbsAddrOperand(PICExecMode mode, uint addr) : base(PrimitiveType.Pointer32, mode)
        {
            CodeTarget = AddressOperand.Ptr32(addr << 1);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{(CodeTarget.Address.ToUInt32()):X6}");
        }

    }

    /// <summary>
    /// A PIC18 FSRn register operand. Used by LFSR, ADDFSR, SUBFSR instructions.
    /// </summary>
    public class PIC18FSRNumOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the FSR register number.
        /// </summary>
        public Constant FSRNum { get; }

        /// <summary>
        /// Instantiates a FSRn register operand. Used by LFSR, ADDFSR, SUBFSR instructions.
        /// </summary>
        /// <param name="fsrnum">Gets the FSR register number.</param>
        public PIC18FSRNumOperand(PICExecMode mode, byte fsrnum) : base(PrimitiveType.Byte, mode)
        {
            FSRNum = Constant.Byte(fsrnum);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            byte num = FSRNum.ToByte();
            writer.Write($"FSR{num}");
        }

    }

    /// <summary>
    /// A PIC18 TBLRD/TBLWT Increment Change mode.
    /// </summary>
    public class PIC18TableReadWriteOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the TBL increment mode.
        /// </summary>
        public Constant TBLIncrMode { get; }

        /// <summary>
        /// Instantiates a TBLRD/TBLWT Increment Change mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public PIC18TableReadWriteOperand(PICExecMode mode, uint incrmode) : base(PrimitiveType.Byte, mode)
        {
            TBLIncrMode = Constant.Byte((byte)(incrmode & 3));
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            byte mode = TBLIncrMode.ToByte();
            switch (mode)
            {
                case 0:
                    writer.Write("*");
                    break;
                case 1:
                    writer.Write("*+");
                    break;
                case 2:
                    writer.Write("*-");
                    break;
                case 3:
                    writer.Write("+*");
                    break;
                default:
                    writer.Write($"Invalid TBLRD/TBLWT mode: {mode}");
                    break;
            }
        }

    }

}
