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
    /// The notion of PIC16 immediate operand. Must be inherited.
    /// </summary>
    public abstract class PIC16ImmediateOperand : MachineOperand, IOperand
    {
        /// <summary>
        /// The immediate value.
        /// </summary>
        public readonly Constant ImmediateValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="immValue">The immediate constant value.</param>
        /// <param name="dataWidth">The constant data width.</param>
        public PIC16ImmediateOperand(Constant immValue, PrimitiveType dataWidth) : base(dataWidth)
        {
            ImmediateValue = immValue;
        }

        /// <summary>
        /// Accepts the given visitor method.
        /// </summary>
        /// <param name="visitor">The visitor method.</param>
        public abstract void Accept(IOperandVisitor visitor);

        /// <summary>
        /// Accepts the given visitor function.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="visitor">The visitor function.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IOperandVisitor<T> visitor);

        /// <summary>
        /// Accepts the given visitor function with context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="visitor">The visitor function.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IOperandVisitor<T, C> visitor, C context);

    }

    /// <summary>
    /// A PIC16 5-bit unsigned immediate operand. Used by MOVLB instruction.
    /// </summary>
    public class PIC16Immed5Operand : PIC16ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 5-bit unsigned immediate operand. Used by MOVLB instruction.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC16Immed5Operand(byte b) : base(Constant.Byte(b), PrimitiveType.Byte)
        {
        }

        public override void Accept(IOperandVisitor visitor) => visitor.VisitImm5(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitImm5(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitImm5(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString($"0x{ImmediateValue.ToByte():X2}");
        }

    }

    /// <summary>
    /// A PIC16 6-bit signed immediate operand. Used by ADDFSR instruction.
    /// </summary>
    public class PIC16Signed6Operand : PIC16ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 5-bit unsigned immediate operand. Used by MOVLB instruction.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC16Signed6Operand(short s) : base(Constant.SByte((sbyte)s), PrimitiveType.Byte)
        {
        }

        public override void Accept(IOperandVisitor visitor) => visitor.VisitSigned6(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitSigned6(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitSigned6(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString($"{ImmediateValue}");
        }

    }

    /// <summary>
    /// A PIC16 6-bit unsigned immediate operand. Used by MOVLP instruction.
    /// </summary>
    public class PIC16Immed6Operand : PIC16ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 6-bit unsigned immediate operand. Used by MOVLP instruction.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC16Immed6Operand(byte b) : base(Constant.Byte(b), PrimitiveType.Byte)
        {
        }

        public override void Accept(IOperandVisitor visitor) => visitor.VisitImm6(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitImm6(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitImm6(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString($"0x{ImmediateValue.ToByte():X2}");
        }

    }

    /// <summary>
    /// A PIC16 7-bit unsigned immediate operand. Used by MOVLP instruction.
    /// </summary>
    public class PIC16Immed7Operand : PIC16ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 7-bit unsigned immediate operand. Used by MOVLP instruction.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC16Immed7Operand(byte b) : base(Constant.Byte(b), PrimitiveType.Byte)
        {
        }

        public override void Accept(IOperandVisitor visitor) => visitor.VisitImm7(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitImm7(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitImm7(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString($"0x{ImmediateValue.ToByte():X2}");
        }

    }

    /// <summary>
    /// A PIC16 8-bit unsigned immediate operand. Used by immediate instructions (ADDLW, SUBLW, RETLW, ...)
    /// </summary>
    public class PIC16Immed8Operand : PIC16ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 8-bit unsigned immediate operand. Used by immediate instructions (ADDLW, SUBLW, RETLW, ...)
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC16Immed8Operand(byte b) : base(Constant.Byte(b), PrimitiveType.Byte)
        {
        }

        public override void Accept(IOperandVisitor visitor) => visitor.VisitImm8(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitImm8(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitImm8(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString($"{ImmediateValue}");
        }

    }

    /// <summary>
    /// A PIC16 Bank Data Memory Address operand (like "f").
    /// </summary>
    public class PIC16BankedOperand : MachineOperand, IOperand
    {

        /// <summary>
        /// Gets the 7-bit memory address.
        /// </summary>
        public readonly Constant BankAddr;

        public PIC16BankedOperand(byte addr) : base(PrimitiveType.Byte)
        {
            BankAddr = Constant.Byte((byte)(addr & 0x7F));
        }

        public virtual void Accept(IOperandVisitor visitor) => visitor.VisitDataBanked(this);
        public virtual T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitDataBanked(this);
        public virtual T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitDataBanked(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            ushort uaddr = BankAddr.ToByte();

            var aaddr = PIC16MemoryDescriptor.TranslateDataAddress(uaddr);
            if (PICRegisters.TryGetRegister(aaddr, 8, out var sfr))
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

        public override void Accept(IOperandVisitor visitor) => visitor.VisitDataBit(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitDataBit(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitDataBit(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            byte uaddr = BankAddr.ToByte();
            byte bitpos = BitNumber.ToByte();

            base.Write(writer, options);

            var aaddr = PIC16MemoryDescriptor.TranslateDataAddress(uaddr);
            if (PICRegisters.TryGetRegister(aaddr, 8, out var sfr))
            {
                var bitname = PICRegisters.PeekBitFieldFromRegister(aaddr, bitpos, 1);
                if (bitname != null)
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

        public override void Accept(IOperandVisitor visitor) => visitor.VisitDataByte(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitDataByte(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitDataByte(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            byte uaddr = BankAddr.ToByte();
            string wdest = (WregIsDest.ToBoolean() ? ",W" : ",F");

            base.Write(writer, options);
            writer.WriteString(wdest);

        }

    }

    /// <summary>
    /// The notion of PIC16 program address operand. Must be inherited.
    /// </summary>
    public abstract class PIC16ProgAddrOperand : MachineOperand, IOperand
    {

        /// <summary>
        /// Gets the target absolute code target address. This is a word-aligned address.
        /// </summary>
        public uint CodeTarget { get; protected set; }

        public PIC16ProgAddrOperand() : base(PrimitiveType.Ptr32)
        {
        }

        public abstract void Accept(IOperandVisitor visitor);
        public abstract T Accept<T>(IOperandVisitor<T> visitor);
        public abstract T Accept<T, C>(IOperandVisitor<T, C> visitor, C context);

    }

    /// <summary>
    /// A PIC16 9-bit code relative offset operand.
    /// </summary>
    public class PIC16ProgRel9AddrOperand : PIC16ProgAddrOperand
    {
        /// <summary>
        /// Gets the relative offset. This a word offset.
        /// </summary>
        public readonly short RelativeWordOffset;

        /// <summary>
        /// Instantiates a 9-bit code relative offset operand.
        /// </summary>
        /// <param name="off">The off.</param>
        /// <param name="instrAddr">The instruction address.</param>
        public PIC16ProgRel9AddrOperand(short off, Address instrAddr)
        {
            RelativeWordOffset = off;
            CodeTarget = (uint)((long)instrAddr.ToUInt32() + 2 + (off * 2)) & PICProgAddress.MAXPROGBYTADDR;
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString($"0x{CodeTarget:X6}");
        }

        public override void Accept(IOperandVisitor visitor) => visitor.VisitProgRel9(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitProgRel9(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitProgRel9(this, context);

    }

    /// <summary>
    /// A PIC16 Absolute Code Address operand. Used by GOTO, CALL instructions.
    /// </summary>
    public class PIC16ProgAbsAddrOperand : PIC16ProgAddrOperand
    {

        /// <summary>
        /// Instantiates a Absolute Code Address operand. Used by GOTO, CALL instructions.
        /// </summary>
        /// <param name="absaddr">The program word address.</param>
        public PIC16ProgAbsAddrOperand(uint absaddr)
        {
            CodeTarget = (absaddr << 1) & PICProgAddress.MAXPROGBYTADDR;
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString($"0x{CodeTarget:X6}");
        }

        public override void Accept(IOperandVisitor visitor) => visitor.VisitProgAbs(this);
        public override T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitProgAbs(this);
        public override T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitProgAbs(this, context);

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

        public abstract void Accept(IOperandVisitor visitor);
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

        public override void Accept(IOperandVisitor visitor) => visitor.VisitFSRArith(this);
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

        public override void Accept(IOperandVisitor visitor) => visitor.VisitFSRIndexed(this);
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

        public override void Accept(IOperandVisitor visitor) => visitor.VisitIncDecFSR(this);
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
    /// A PIC16 data EEPROM series of bytes.
    /// </summary>
    public class PIC16DataEEPROMOperand : PseudoDataOperand, IOperand
    {

        public PIC16DataEEPROMOperand(params byte[] eedata) : base(eedata)
        {
        }

        public void Accept(IOperandVisitor visitor) => visitor.VisitEEPROM(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitEEPROM(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitEEPROM(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            string s = string.Join(",", Values.Select(b => $"0x{b:X2}"));
            writer.WriteString(s);
        }

    }

    /// <summary>
    /// A PIC16 declare ASCII bytes operand.
    /// </summary>
    public class PIC16DataASCIIOperand : PseudoDataOperand, IOperand
    {

        public PIC16DataASCIIOperand(params byte[] bytes) : base(bytes)
        {
        }

        public void Accept(IOperandVisitor visitor) => visitor.VisitASCII(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitASCII(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitASCII(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            string s = string.Join(",", Values.Select(b => $"0x{b:X2}"));
            writer.WriteString(s);
        }

    }

    /// <summary>
    /// A PIC16 declare data bytes operand.
    /// </summary>
    public class PIC16DataByteOperand : PseudoDataOperand, IOperand
    {

        public PIC16DataByteOperand(params byte[] bytes) : base(bytes)
        {
        }

        public void Accept(IOperandVisitor visitor) => visitor.VisitDB(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitDB(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitDB(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            string s = string.Join(",", Values.Select(b => $"0x{b:X2}"));
            writer.WriteString(s);
        }

    }

    /// <summary>
    /// A PIC16 declare data words operand.
    /// </summary>
    public class PIC16DataWordOperand : PseudoDataOperand, IOperand
    {

        public PIC16DataWordOperand(params ushort[] words) : base(words)
        {
        }

        public void Accept(IOperandVisitor visitor) => visitor.VisitDW(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitDW(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitDW(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            string s = string.Join(",", Values.Select(w => $"0x{w:X4}"));
            writer.WriteString(s);
        }

    }

    /// <summary>
    /// A PIC16 set processor ID locations operand (used for __IDLOCS).
    /// </summary>
    public class PIC16IDLocsOperand : PseudoDataOperand, IOperand
    {

        private Address addr;

        public PIC16IDLocsOperand(Address addr, ushort idlocs) : base(idlocs)
        {
            this.addr = addr;
        }

        public void Accept(IOperandVisitor visitor) => visitor.VisitIDLocs(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitIDLocs(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitIDLocs(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString($"{addr}, 0x{Values[0]:X3}");
        }

    }

    /// <summary>
    /// A PIC16 processor configuration bits operand (used for __CONFIG).
    /// </summary>
    public class PIC16ConfigOperand : PseudoDataOperand, IOperand
    {

        private PICArchitecture arch;
        private Address addr;

        public PIC16ConfigOperand(PICArchitecture arch, Address addr, ushort config) : base(config)
        {
            this.arch = arch;
            this.addr = addr;
        }

        public void Accept(IOperandVisitor visitor) => visitor.VisitConfig(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitConfig(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitConfig(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var fuse = Values[0] & 0x3FFF;
            var s = arch.DeviceConfigDefinitions.Render(addr, fuse);
            writer.WriteString(s);
        }

    }

}
