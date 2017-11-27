#region License
/* 
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

using System;
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
        T Visit(PIC18FSRIdxOperand imm8);
        T Visit(PIC18BitNumberOperand bitno);
        T Visit(PIC18Memory12bitAddrOperand addr12);
        T Visit(PIC18Memory14bitAddrOperand addr14);
        T Visit(PIC18BankMemoryOperand bmem);
        T Visit(PIC18AccessRAMOperand acc);
        T Visit(PIC18WdestOperand wdest);
        T Visit(PIC18Rel8Operand roff8);
        T Visit(PIC18Rel11Operand roff11);
        T Visit(PIC18CodeAddr20Operand addr20);
        T Visit(PIC18FSRNumOperand fsrnum);
        T Visit(PIC18ShadowOperand shad);
        T Visit(PIC18TBLOperand tblmode);

    }

    /// <summary>
    /// A PIC18 operand with visitor mechanism interface.
    /// </summary>
    public abstract class PIC18OperandImpl : MachineOperand, IPIC18kOperand
    {
        /// <summary>
        /// Specialised constructor for use only by derived class.
        /// </summary>
        /// <param name="dataWidth">Width of the data.</param>
        protected PIC18OperandImpl(PrimitiveType dataWidth)
            : base(dataWidth)
        {
        }

        public abstract T Accept<T>(IPIC18OperandVisitor<T> visitor);
    }

    /// <summary>
    /// A PIC18 4-bit unsigned immediate operand. Used by MOVLB instruction.
    /// </summary>
    public class PIC18Immed4Operand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 4-bit immediate constant.
        /// </summary>
        public Constant Imm4 { get; private set; }

        /// <summary>
        /// Instantiates a 4-bit unsigned immediate operand. Used by MOVLB instruction.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18Immed4Operand(byte b) : base(PrimitiveType.Byte)
        {
            Imm4 = Constant.Byte(b);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"{Imm4.ToByte()}");
        }
    }

    /// <summary>
    /// A PIC18 6-bit unsigned immediate operand. Used by ADDFSR, SUBFSR instructions.
    /// </summary>
    public class PIC18Immed6Operand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 6-bit immediate constant.
        /// </summary>
        public Constant Imm6 { get; private set; }

        /// <summary>
        /// Instantiates a 6-bit unsigned immediate operand. Used by ADDFSR, SUBFSR instructions.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18Immed6Operand(byte b) : base(PrimitiveType.Byte)
        {
            Imm6 = Constant.Byte(b);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"{Imm6.ToByte()}");
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
        public Constant Imm8 { get; private set; }

        /// <summary>
        /// Instantiates a 8-bit unsigned immediate operand. Used by immediate instructions (ADDLW, SUBLW, RETLW, ...)
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18Immed8Operand(byte b) : base(PrimitiveType.Byte)
        {
            Imm8 = Constant.Byte(b);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"{Imm8:2X}h");
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
        public Constant Imm12 { get; private set; }

        /// <summary>
        /// Instantiates a 12-bit unsigned immediate operand. Used by LFSR instruction.
        /// </summary>
        /// <param name="w">An ushort to process.</param>
        public PIC18Immed12Operand(ushort w) : base(PrimitiveType.UInt16)
        {
            Imm12 = Constant.Word16(w);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"{Imm12:3X}h");
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
        public Constant Imm14 { get; private set; }

        /// <summary>
        /// Instantiates a 14-bit unsigned immediate operand. Used by LFSR instruction - PIC18 Enhanced.
        /// </summary>
        /// <param name="w">An ushort to process.</param>
        public PIC18Immed14Operand(ushort w) : base(PrimitiveType.UInt16)
        {
            Imm14 = Constant.Word16(w);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"{Imm14:4X}h");
        }
    }

    /// <summary>
    /// A PIC18 7-bit unsigned offset operand. Used by MOVSF, MOVSFL, MOVSS instructions.
    /// </summary>
    public class PIC18FSRIdxOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 7-bit unsigned offset constant.
        /// </summary>
        public Constant Offset { get; private set; }

        /// <summary>
        /// Instantiates a 7-bit unsigned offset operand. Used by MOVSF, MOVSFL, MOVSS instructions.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18FSRIdxOperand(byte b) : base(PrimitiveType.Byte)
        {
            Offset = Constant.Byte(b);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"[{Offset:2X}h]");
        }
    }

    /// <summary>
    /// A PIC18 bit number operand. Used by Bit-oriented instructions (BCF, BTFSS, ...)
    /// </summary>
    public class PIC18BitNumberOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the bit number (value between 0 and 7).
        /// </summary>
        /// <value>
        /// The bit number.
        /// </value>
        public Constant BitNumber { get; private set; }

        /// <summary>
        /// Instantiates a bit number operand. Used by Bit-oriented instructions (BCF, BTFSS, ...).
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18BitNumberOperand(byte b) : base(PrimitiveType.Byte)
        {
            BitNumber = Constant.Byte(b);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write(BitNumber.ToByte().ToString());
        }
    }

    /// <summary>
    /// A PIC18 12-bit data memory address operand (used by MOVFF, MOVSF instruction)
    /// </summary>
    public class PIC18Memory12bitAddrOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 12-bit data memory address.
        /// </summary>
        public Constant Addr12 { get; private set; }

        public PIC18Memory12bitAddrOperand(ushort w) : base(PrimitiveType.UInt16)
        {
            Addr12 = Constant.Word16(w);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"{Addr12:3X}h");
        }
    }

    /// <summary>
    /// A PIC18 14-bit data memory address operand (used by MOVFFL, MOVSFL instruction)
    /// </summary>
    public class PIC18Memory14bitAddrOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 14-bit data memory address.
        /// </summary>
        public Constant Addr14 { get; private set; }

        /// <summary>
        /// Instantiates a 14-bit data memory address operand (used by MOVFFL, MOVSFL instruction)
        /// </summary>
        /// <param name="w">An ushort to process.</param>
        public PIC18Memory14bitAddrOperand(ushort w) : base(PrimitiveType.UInt16)
        {
            Addr14 = Constant.Word16(w);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"{Addr14:4X}h");
        }
    }

    /// <summary>
    /// A PIC18 Access RAM flag operand. Used by instructions with "f,a" operands, "f,b,a" operands or "f,d,a" operands.
    /// </summary>
    public class PIC18AccessRAMOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the indication if RAM location is in Access RAM or specified by BSR register.
        /// </summary>
        public Constant IsAccessRAM { get; private set; }

        public PIC18AccessRAMOperand(int d) : base(PrimitiveType.Bool)
        {
            IsAccessRAM = Constant.Bool(d == 0);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write(IsAccessRAM.ToBoolean() ? "ACCESS" : "BANKED");
        }
    }

    /// <summary>
    /// A PIC18 Working Register Destination flag operand. Used by instructions with "f,d,a" operands.
    /// </summary>
    public class PIC18WdestOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the indication if the Working Register is the destination of the operation. If false, the File Register is the destination.
        /// </summary>
        public Constant WregIsDest { get; private set; }

        /// <summary>
        /// Instantiates a Working Register Destination flag operand. Used by instructions with "f,d,a" operands.
        /// </summary>
        /// <param name="d">An int to process.</param>
        public PIC18WdestOperand(int d) : base(PrimitiveType.Bool)
        {
            WregIsDest = Constant.Bool(d == 0);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write(WregIsDest.ToBoolean() ? 'W' : 'F');
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
        public Constant IsShadow { get; private set; }

        /// <summary>
        /// Instantiates a Registers Shadowing flag operand. Used by instructions RETFIE, RETURN, CALL.
        /// </summary>
        /// <param name="s">An int to process.</param>
        public PIC18ShadowOperand(uint s) : base(PrimitiveType.Bool)
        {
            IsShadow = Constant.Bool(s == 0);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write(IsShadow.ToBoolean() ? "S" : "");
        }
    }

    /// <summary>
    /// A PIC18 Bank Data Memory Address operand ("f" in the operands like "f,a", "f,b,a" or "f,d,a").
    /// </summary>
    public class PIC18BankMemoryOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 8-bit memory address.
        /// </summary>
        public Constant MemAddr { get; private set; }

        /// <summary>
        /// Instantiates a Bank Data Memory Address operand ("f" in the operands like "f,a", "f,b,a" or "f,d,a").
        /// </summary>
        /// <param name="addr">The address.</param>
        public PIC18BankMemoryOperand(byte addr) : base(PrimitiveType.Byte)
        {
            MemAddr = Constant.Byte(addr);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"{MemAddr}");
        }
    }

    /// <summary>
    /// A PIC18 8-bit relative code offset operand.
    /// </summary>
    public class PIC18Rel8Operand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the relative offset. This a word offset.
        /// </summary>
        public Constant RelativeOffset { get; private set; }

        /// <summary>
        /// Gets the target absolute code address. This is a word address.
        /// </summary>
        public Constant AbsAddress { get; }

        /// <summary>
        /// Instantiates a 8-bit relative code offset operand.
        /// </summary>
        /// <param name="off">The 8-bit signed offset value.</param>
        /// <param name="instrAddr">The instruction address.</param>
        public PIC18Rel8Operand(sbyte off, Address instrAddr) : base(PrimitiveType.SByte)
        {
            RelativeOffset = Constant.SByte(off);
            AbsAddress = Constant.UInt32((uint)((long)instrAddr.ToUInt32() + 1 + off));
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (options.HasFlag(MachineInstructionWriterOptions.ResolvePcRelativeAddress))
                writer.Write($"{(AbsAddress.ToUInt32()*2):X6}h");
            else
            {
                int off = RelativeOffset.ToInt16() * 2;
                if (off < 0)
                {
                    writer.Write($"$-{-off:X3}h");
                }
                else
                    writer.Write($"$+{off:X3}h");
            }
        }
    }

    /// <summary>
    /// A PIC18 11-bit code relative offset operand.
    /// </summary>
    public class PIC18Rel11Operand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the relative offset. This a word offset.
        /// </summary>
        public Constant RelativeOffset { get; private set; }

        /// <summary>
        /// Gets the target absolute code address. This is a word address.
        /// </summary>
        public Constant AbsAddress { get; }

        /// <summary>
        /// Instantiates a 11-bit code relative offset operand.
        /// </summary>
        /// <param name="off">The off.</param>
        /// <param name="instrAddr">The instruction address.</param>
        public PIC18Rel11Operand(short off, Address instrAddr) : base(PrimitiveType.Int16)
        {
            RelativeOffset = Constant.Int16(off);
            AbsAddress = Constant.UInt32((uint)((long)instrAddr.ToUInt32() + 1 + off));
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (options.HasFlag(MachineInstructionWriterOptions.ResolvePcRelativeAddress))
                writer.Write($"{(AbsAddress.ToUInt32() * 2):X6}h");
            else
            {
                int off = RelativeOffset.ToInt16() * 2;
                if (off < 0)
                {
                    writer.Write($"$-{-off:X3}h");
                }
                else
                    writer.Write($"$+{off:X3}h");
            }
        }
    }

    /// <summary>
    /// A PIC18 20-bit Code Address operand. Used by GOTO, CALL instructions.
    /// </summary>
    public class PIC18CodeAddr20Operand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the 20-bit Code Memory Address. This is a word address.
        /// </summary>
        public Constant CodeAddr { get; private set; }

        /// <summary>
        /// Instantiates a 20-bit Code Address operand. Used by GOTO, CALL instructions.
        /// </summary>
        /// <param name="addr">The address.</param>
        public PIC18CodeAddr20Operand(uint addr) : base(PrimitiveType.Pointer32)
        {
            CodeAddr = Constant.UInt32(addr);
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var addr = CodeAddr.ToUInt32();
            writer.Write($"{(addr * 2):X6}h");
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
        public Constant FSRNum { get; private set; }

        /// <summary>
        /// Instantiates a FSRn register operand. Used by LFSR, ADDFSR, SUBFSR instructions.
        /// </summary>
        /// <param name="fsrnum">Gets the FSR register number.</param>
        public PIC18FSRNumOperand(byte fsrnum) : base(PrimitiveType.Byte)
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
    public class PIC18TBLOperand : PIC18OperandImpl
    {
        /// <summary>
        /// Gets the TBL increment mode.
        /// </summary>
        public Constant TBLMode { get; private set; }

        /// <summary>
        /// Instantiates a TBLRD/TBLWT Increment Change mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public PIC18TBLOperand(uint mode) : base(PrimitiveType.Byte)
        {
            TBLMode = Constant.Byte((byte)(mode & 3));
        }

        public override T Accept<T>(IPIC18OperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            byte mode = TBLMode.ToByte();
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
