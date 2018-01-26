#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
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
using Reko.Arch.Microchip.Common;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;

namespace Reko.Arch.Microchip.PIC18
{


    /// <summary>
    /// The notion of PIC18 immediate operand. Must be inherited.
    /// </summary>
    public abstract class PIC18ImmediateOperand : MachineOperand, IPIC18kOperand
    {
        /// <summary>
        /// The immediate value.
        /// </summary>
        public readonly Constant ImmediateValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mode">The PIC execution mode.</param>
        /// <param name="immValue">The immediate constant value.</param>
        /// <param name="datawidth">The constant data width.</param>
        public PIC18ImmediateOperand(Constant immValue, PrimitiveType datawidth) : base(datawidth)
        {
            ImmediateValue = immValue;
        }

        /// <summary>
        /// Accepts the given visitor method.
        /// </summary>
        /// <param name="visitor">The visitor method.</param>
        public abstract void Accept(IPIC18OperandVisitor visitor);

        /// <summary>
        /// Accepts the given visitor function.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="visitor">The visitor function.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IPIC18OperandVisitor<T> visitor);

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
        public abstract T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context);

    }

    /// <summary>
    /// A PIC18 4-bit unsigned immediate operand. Used by MOVLB instruction.
    /// </summary>
    public class PIC18Immed4Operand : PIC18ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 4-bit unsigned immediate operand. Used by MOVLB instruction.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18Immed4Operand(byte b) : base(Constant.Byte(b), PrimitiveType.Byte)
        {
        }

        public override void Accept(IPIC18OperandVisitor visitor) => visitor.VisitImm4(this);
        public override T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitImm4(this);
        public override T Accept<T, C>(IPIC18OperandVisitor<T,C > visitor, C context) => visitor.VisitImm4(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{ImmediateValue.ToByte():X2}");
        }

    }

    /// <summary>
    /// A PIC18 6-bit unsigned immediate operand. Used by ADDFSR, SUBFSR, MOVLB instructions.
    /// </summary>
    public class PIC18Immed6Operand : PIC18ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 6-bit unsigned immediate operand. Used by ADDFSR, SUBFSR instructions.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18Immed6Operand(byte b) : base(Constant.Byte(b), PrimitiveType.Byte)
        {
        }

        public override void Accept(IPIC18OperandVisitor visitor) => visitor.VisitImm6(this);
        public override T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitImm6(this);
        public override T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitImm6(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"0x{ImmediateValue.ToByte():X2}");
        }

    }

    /// <summary>
    /// A PIC18 8-bit unsigned immediate operand. Used by immediate instructions (ADDLW, SUBLW, RETLW, ...)
    /// </summary>
    public class PIC18Immed8Operand : PIC18ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 8-bit unsigned immediate operand. Used by immediate instructions (ADDLW, SUBLW, RETLW, ...)
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18Immed8Operand(byte b) : base(Constant.Byte(b), PrimitiveType.Byte)
        {
        }

        public override void Accept(IPIC18OperandVisitor visitor) => visitor.VisitImm8(this);
        public override T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitImm8(this);
        public override T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitImm8(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"{ImmediateValue}");
        }

    }

    /// <summary>
    /// A PIC18 12-bit unsigned immediate operand. Used by LFSR instruction.
    /// </summary>
    public class PIC18Immed12Operand : PIC18ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 12-bit unsigned immediate operand. Used by LFSR instruction.
        /// </summary>
        /// <param name="w">An unsigned 16-bit integer to process.</param>
        public PIC18Immed12Operand(ushort w) : base(Constant.UInt16(w), PrimitiveType.UInt16)
        {
        }

        public override void Accept(IPIC18OperandVisitor visitor) => visitor.VisitImm12(this);
        public override T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitImm12(this);
        public override T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitImm12(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            ushort immAddr = ImmediateValue.ToUInt16();
            RegisterStorage sfr = PIC18Registers.GetRegister(immAddr);
            if (sfr != RegisterStorage.None)
                writer.Write($"{sfr.Name}");
            else
                writer.Write($"0x{immAddr:X4}");
        }

    }

    /// <summary>
    /// A PIC18 14-bit unsigned immediate operand. Used by LFSR instruction - PIC18 Enhanced.
    /// </summary>
    public class PIC18Immed14Operand : PIC18ImmediateOperand
    {
        /// <summary>
        /// Instantiates a 14-bit unsigned immediate operand. Used by LFSR instruction - PIC18 Enhanced.
        /// </summary>
        /// <param name="w">An unsigned 16-bit integer to process.</param>
        public PIC18Immed14Operand(ushort w) : base(Constant.UInt16(w), PrimitiveType.UInt16)
        {
        }

        public override void Accept(IPIC18OperandVisitor visitor) => visitor.VisitImm14(this);
        public override T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitImm14(this);
        public override T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitImm14(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            ushort immAddr = ImmediateValue.ToUInt16();
            RegisterStorage sfr = PIC18Registers.GetRegister(immAddr);
            if (sfr != RegisterStorage.None)
                writer.Write($"{sfr.Name}");
            else
                writer.Write($"0x{immAddr:X4}");
        }

    }

    /// <summary>
    /// A PIC18 7-bit unsigned offset operand to FSR2. Used by MOVSF, MOVSFL, MOVSS instructions.
    /// </summary>
    public class PIC18FSR2IdxOperand : MachineOperand, IPIC18kOperand
    {
        /// <summary>
        /// Gets the 7-bit unsigned offset constant.
        /// </summary>
        public readonly Constant Offset;

        /// <summary>
        /// Instantiates a 7-bit unsigned offset operand. Used by MOVSF, MOVSFL, MOVSS instructions.
        /// </summary>
        /// <param name="b">The byte value.</param>
        public PIC18FSR2IdxOperand(byte b) : base(PrimitiveType.Byte)
        {
            Offset = Constant.Byte(b);
        }

        public void Accept(IPIC18OperandVisitor visitor) => visitor.VisitFSR2Idx(this);
        public T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitFSR2Idx(this);
        public T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitFSR2Idx(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"[0x{Offset.ToByte():X2}]");
        }

    }

    /// <summary>
    /// A PIC18 Registers Shadowing flag operand. Used by instructions RETFIE, RETURN, CALL.
    /// </summary>
    public class PIC18ShadowOperand : MachineOperand, IPIC18kOperand, IOperandShadow
    {
        /// <summary>
        /// Gets the indication if a Shadow Register(s) are used. If false, no Shadow Register(s) used.
        /// </summary>
        public readonly Constant IsShadow;

        /// <summary>
        /// Instantiates a Registers Shadowing flag operand. Used by instructions RETFIE, RETURN, CALL.
        /// </summary>
        /// <param name="s">An int to process.</param>
        public PIC18ShadowOperand(uint s) : base(PrimitiveType.Bool)
        {
            IsShadow = Constant.Bool(s != 0);
        }

        public void Accept(IPIC18OperandVisitor visitor) => visitor.VisitShadow(this);
        public T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitShadow(this);
        public T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitShadow(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write(IsPresent ? "FAST" : "");
        }

        public bool IsPresent => IsShadow.ToBoolean();

    }

    /// <summary>
    /// The notion of PIC18 program address operand. Must be inherited.
    /// </summary>
    public abstract class PIC18ProgAddrOperand : MachineOperand, IPIC18kOperand
    {

        /// <summary>
        /// Gets the target absolute code byte address. This is a word-aligned address.
        /// </summary>
        public PICProgAddress CodeTarget { get; protected set; }

        public PIC18ProgAddrOperand() : base(PrimitiveType.Ptr32)
        {
        }

        public abstract void Accept(IPIC18OperandVisitor visitor);
        public abstract T Accept<T>(IPIC18OperandVisitor<T> visitor);
        public abstract T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.Write($"{CodeTarget}");
        }

    }

    /// <summary>
    /// A PIC18 8-bit relative code offset operand.
    /// </summary>
    public class PIC18ProgRel8AddrOperand : PIC18ProgAddrOperand
    {
        /// <summary>
        /// Gets the relative offset. This a word offset.
        /// </summary>
        public readonly Constant RelativeWordOffset;

        /// <summary>
        /// Instantiates a 8-bit relative code offset operand.
        /// </summary>
        /// <param name="off">The 8-bit signed offset value. (word offset)</param>
        /// <param name="instrAddr">The instruction address.</param>
        public PIC18ProgRel8AddrOperand(sbyte off, Address instrAddr) 
        {
            RelativeWordOffset = Constant.SByte(off);
            uint absaddr = (uint)((long)instrAddr.ToUInt32() + 2 + (off * 2)) & PICProgAddress.MAXPROGBYTADDR;
            CodeTarget = PICProgAddress.Ptr(absaddr);
        }

        public override void Accept(IPIC18OperandVisitor visitor) => visitor.VisitProgRel8(this);
        public override T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitProgRel8(this);
        public override T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitProgRel8(this, context);

    }

    /// <summary>
    /// A PIC18 11-bit code relative offset operand.
    /// </summary>
    public class PIC18ProgRel11AddrOperand : PIC18ProgAddrOperand
    {
        /// <summary>
        /// Gets the relative offset. This a word offset.
        /// </summary>
        public readonly Constant RelativeWordOffset;

        /// <summary>
        /// Instantiates a 11-bit code relative offset operand.
        /// </summary>
        /// <param name="off">The off.</param>
        /// <param name="instrAddr">The instruction address.</param>
        public PIC18ProgRel11AddrOperand(short off, Address instrAddr) 
        {
            RelativeWordOffset = Constant.Int16(off);
            uint absaddr = (uint)((long)instrAddr.ToUInt32() + 2 + (off * 2)) & PICProgAddress.MAXPROGBYTADDR;
            CodeTarget = PICProgAddress.Ptr(absaddr);
        }

        public override void Accept(IPIC18OperandVisitor visitor) => visitor.VisitProgRel11(this);
        public override T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitProgRel11(this);
        public override T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitProgRel11(this, context);

    }

    /// <summary>
    /// A PIC18 Absolute Code Address operand. Used by GOTO, CALL instructions.
    /// </summary>
    public class PIC18ProgAbsAddrOperand : PIC18ProgAddrOperand
    {
        /// <summary>
        /// Instantiates a Absolute Code Address operand. Used by GOTO, CALL instructions.
        /// </summary>
        /// <param name="absaddr">The program word address.</param>
        public PIC18ProgAbsAddrOperand(uint absaddr)
        {
            CodeTarget = PICProgAddress.Ptr((absaddr << 1) & PICProgAddress.MAXPROGBYTADDR);
        }

        public override void Accept(IPIC18OperandVisitor visitor) => visitor.VisitProgAbs(this);
        public override T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitProgAbs(this);
        public override T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitProgAbs(this, context);

    }

    /// <summary>
    /// A PIC18 FSRn register operand. Used by LFSR, ADDFSR, SUBFSR instructions.
    /// </summary>
    public class PIC18FSRNumOperand : MachineOperand, IPIC18kOperand
    {
        /// <summary>
        /// Gets the FSR register number.
        /// </summary>
        public readonly Constant FSRNum;

        /// <summary>
        /// Instantiates a FSRn register operand. Used by LFSR, ADDFSR, SUBFSR instructions.
        /// </summary>
        /// <param name="fsrnum">The FSR register number [0, 1, 2].</param>
        public PIC18FSRNumOperand(byte fsrnum) : base(PrimitiveType.Byte)
        {
            FSRNum = Constant.Byte(fsrnum);
        }

        public void Accept(IPIC18OperandVisitor visitor) => visitor.VisitFSRNum(this);
        public T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitFSRNum(this);
        public T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitFSRNum(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            byte num = FSRNum.ToByte();
            writer.Write($"FSR{num}");
        }

    }

    /// <summary>
    /// A PIC18 TBLRD/TBLWT Increment Change mode.
    /// </summary>
    public class PIC18TableReadWriteOperand : MachineOperand, IPIC18kOperand
    {
        /// <summary>
        /// Gets the TBL increment mode.
        /// </summary>
        public readonly Constant TBLIncrMode;

        /// <summary>
        /// Instantiates a TBLRD/TBLWT Increment Change mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public PIC18TableReadWriteOperand(uint incrmode) : base(PrimitiveType.Byte)
        {
            TBLIncrMode = Constant.Byte((byte)(incrmode & 3));
        }

        public void Accept(IPIC18OperandVisitor visitor) => visitor.VisitTblRW(this);
        public T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitTblRW(this);
        public T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitTblRW(this, context);

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

    public abstract class PIC18DataAbsAddrOperand : MachineOperand, IPIC18kOperand
    {
        /// <summary>
        /// Gets the 12/14-bit data memory absolute address.
        /// </summary>
        public PICDataAddress DataTarget { get; protected set; }

        public PIC18DataAbsAddrOperand() : base(PrimitiveType.UInt16)
        {
        }

        public abstract void Accept(IPIC18OperandVisitor visitor);
        public abstract T Accept<T>(IPIC18OperandVisitor<T> visitor);
        public abstract T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            RegisterStorage sfr = PIC18Registers.GetRegister(DataTarget);
            if (sfr != RegisterStorage.None)
                writer.Write($"{sfr.Name}");
            else
                writer.Write($"{DataTarget}");
        }

    }

    /// <summary>
    /// A PIC18 12-bit data memory absolute address operand (used by MOVFF, MOVSF instruction)
    /// </summary>
    public class PIC18Data12bitAbsAddrOperand : PIC18DataAbsAddrOperand
    {
        public PIC18Data12bitAbsAddrOperand(ushort w)
        {
            DataTarget = PICDataAddress.Ptr(w & 0xFFFU);
        }

        public override void Accept(IPIC18OperandVisitor visitor) => visitor.VisitDataAbs12(this);
        public override T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitDataAbs12(this);
        public override T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitDataAbs12(this, context);

    }

    /// <summary>
    /// A PIC18 14-bit data memory absolute address operand (used by MOVFFL, MOVSFL instruction)
    /// </summary>
    public class PIC18Data14bitAbsAddrOperand : PIC18DataAbsAddrOperand
    {
        /// <summary>
        /// Instantiates a 14-bit data memory address operand (used by MOVFFL, MOVSFL instruction)
        /// </summary>
        /// <param name="w">An unsigned 16-bit integer to process.</param>
        public PIC18Data14bitAbsAddrOperand(ushort w) 
        {
            DataTarget = PICDataAddress.Ptr(w & 0x3FFFU);
        }

        public override void Accept(IPIC18OperandVisitor visitor) => visitor.VisitDataAbs14(this);
        public override T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitDataAbs14(this);
        public override T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitDataAbs14(this, context);

    }

    /// <summary>
    /// A PIC18 Bank/Access Data Memory Address operand (like "f,a").
    /// </summary>
    public class PIC18BankedAccessOperand : MachineOperand, IPIC18kOperand
    {

        /// <summary>
        /// Gets the 8-bit memory address.
        /// </summary>
        public readonly Constant BankAddr;

        /// <summary>
        /// Gets the indication if RAM location is in Access RAM or specified by BSR register.
        /// </summary>
        public readonly Constant IsAccessRAM;

        /// <summary>
        /// The PIC execution mode.
        /// </summary>
        public readonly PICExecMode ExecMode;

        public bool IsIndexedAddressing
            => (ExecMode == PICExecMode.Extended &&
                IsAccessRAM.ToBoolean() &&
                PIC18MemoryMapper.BelongsToAccessRAMLow(BankAddr.ToByte()));

        public PIC18BankedAccessOperand(PICExecMode mode, byte addr, int a) : base(PrimitiveType.Byte)
        {
            ExecMode = mode;
            BankAddr = Constant.Byte(addr);
            IsAccessRAM = Constant.Bool(a == 0);
        }

        public virtual void Accept(IPIC18OperandVisitor visitor) => visitor.VisitDataBanked(this);
        public virtual T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitDataBanked(this);
        public virtual T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitDataBanked(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            ushort uaddr = BankAddr.ToByte();

            if (IsIndexedAddressing)
            {
                writer.Write($"[0x{uaddr:X2}]");
                return;
            }
            if (!IsAccessRAM.ToBoolean())
            {
                writer.Write($"0x{uaddr:X2}");
                return;
            }
            if (PIC18MemoryMapper.BelongsToAccessRAMHigh(uaddr))
            {
                var aaddr = PIC18MemoryMapper.TranslateAccessAddress(uaddr);
                var sfr = PIC18Registers.GetRegister(aaddr);
                if (sfr != RegisterStorage.None)
                {
                    writer.Write($"{sfr.Name},ACCESS");
                    return;
                }
            }
            writer.Write($"0x{uaddr:X2},ACCESS");

        }

    }

    /// <summary>
    /// A PIC18 data memory bit access with destination operand (like "f,b,a").
    /// Used by Bit-oriented instructions (BCF, BTFSS, ...)
    /// </summary>
    public class PIC18DataBitAccessOperand : PIC18BankedAccessOperand
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
        public PIC18DataBitAccessOperand(PICExecMode mode, byte addr, int a, byte b) : base(mode,addr, a)
        {
            BitNumber = Constant.Byte(b);
        }

        public override void Accept(IPIC18OperandVisitor visitor) => visitor.VisitDataBit(this);
        public override T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitDataBit(this);
        public override T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitDataBit(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            byte uaddr = BankAddr.ToByte();
            byte bitpos = BitNumber.ToByte();

            if (IsIndexedAddressing)
            {
                writer.Write($"[0x{uaddr:X2}],{bitpos}");
                return;
            }
            if (!IsAccessRAM.ToBoolean())
            {
                writer.Write($"0x{uaddr:X2},{bitpos}");
                return;
            }
            if (PIC18MemoryMapper.BelongsToAccessRAMHigh(uaddr))
            {
                var aaddr = PIC18MemoryMapper.TranslateAccessAddress(uaddr);
                var sfr = PIC18Registers.GetRegister(aaddr);
                if (sfr != RegisterStorage.None)
                {
                    var bitname = PIC18Registers.GetBitField(aaddr, bitpos);
                    if (bitname != null)
                    {
                        writer.Write($"{sfr.Name},{bitname.Name},ACCESS");
                        return;
                    }
                    writer.Write($"{sfr.Name},{bitpos},ACCESS");
                    return;
                }
            }
            writer.Write($"0x{uaddr:X2},{bitpos},ACCESS");

        }

    }

    /// <summary>
    /// A PIC18 Bank Data Memory Address with destination operand (like "f,d,a").
    /// </summary>
    public class PIC18DataByteAccessWithDestOperand : PIC18BankedAccessOperand
    {

        /// <summary>
        /// Gets the indication if the Working Register is the destination of the operation.
        /// If false, the File Register designated by the address is the destination.
        /// </summary>
        public readonly Constant WregIsDest;

        public PIC18DataByteAccessWithDestOperand(PICExecMode mode, byte addr, int a, int d) : base(mode, addr, a)
        {
            WregIsDest = Constant.Bool(d == 0);
        }

        public override void Accept(IPIC18OperandVisitor visitor) => visitor.VisitDataByteWDest(this);
        public override T Accept<T>(IPIC18OperandVisitor<T> visitor) => visitor.VisitDataByteWDest(this);
        public override T Accept<T, C>(IPIC18OperandVisitor<T, C> visitor, C context) => visitor.VisitDataByteWDest(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            byte uaddr = BankAddr.ToByte();
            string wdest = (WregIsDest.ToBoolean() ? ",W" : ",F");

            if (IsIndexedAddressing)
            {
                writer.Write($"[0x{uaddr:X2}]{wdest}");
                return;
            }
            if (!IsAccessRAM.ToBoolean())
            {
                writer.Write($"0x{uaddr:X2}{wdest}");
                return;
            }
            if (PIC18MemoryMapper.BelongsToAccessRAMHigh(uaddr))
            {
                var aaddr = PIC18MemoryMapper.TranslateAccessAddress(uaddr);
                var sfr = PIC18Registers.GetRegister(aaddr);
                if (sfr != RegisterStorage.None)
                {
                    writer.Write($"{sfr.Name}{wdest},ACCESS");
                    return;
                }
            }
            writer.Write($"0x{uaddr:X2}{wdest},ACCESS");

        }

    }

}
