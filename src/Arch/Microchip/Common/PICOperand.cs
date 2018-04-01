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
using System;
using System.Linq;

namespace Reko.Arch.Microchip.Common
{

    /// <summary>
    /// A PIC immediate operand.
    /// </summary>
    public class PICOperandImmediate : MachineOperand, IOperand
    {

        public PICOperandImmediate(uint immValue, PrimitiveType dataWidth) : base(dataWidth)
        {
            ImmediateValue = Constant.Create(dataWidth, immValue);
        }

        public PICOperandImmediate(int immValue, PrimitiveType dataWidth) : base(dataWidth)
        {
            ImmediateValue = Constant.Create(dataWidth, immValue);
        }

        /// <summary>
        /// The immediate value.
        /// </summary>
        public Constant ImmediateValue { get; }

        public virtual void Accept(IOperandVisitor visitor) => visitor.VisitImmediate(this);
        public virtual T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitImmediate(this);
        public virtual T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitImmediate(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (!ImmediateValue.IsValid)
            {
                writer.WriteString("???");
                return;
            }
            writer.WriteString(FormatValue(ImmediateValue, false, "{0}0x{1}"));
        }

    }

    /// <summary>
    /// A PIC FAST operand.
    /// </summary>
    public class PICOperandFast : MachineOperand, IOperand
    {

        private bool wTab;

        public PICOperandFast(ushort uFast, bool withTab = true) : base(PrimitiveType.Bool)
        {
            IsFast = Constant.Bool(uFast != 0);
            wTab = withTab;
        }

        /// <summary>
        /// The Fast flag.
        /// </summary>
        public Constant IsFast { get; }

        public virtual void Accept(IOperandVisitor visitor) => visitor.VisitFast(this);
        public virtual T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitFast(this);
        public virtual T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitFast(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (IsFast.ToBoolean())
            {
                writer.WriteString(",");
                if (wTab)
                    writer.Tab();
                writer.WriteString("FAST");
            }
        }

    }

    /// <summary>
    /// A PIC register operand.
    /// </summary>
    public class PICOperandRegister : MachineOperand, IOperand
    {

        public PICOperandRegister(PICRegisterStorage reg) : base(reg.DataType)
        {
            Register = reg;
        }

        /// <summary>
        /// The PIC register that this operand is representing.
        /// </summary>
        public PICRegisterStorage Register { get; }

        public virtual void Accept(IOperandVisitor visitor) => visitor.VisitRegister(this);
        public virtual T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitRegister(this);
        public virtual T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitRegister(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString($"{Register.Name}");
        }

    }

    /// <summary>
    /// A PIC program memory full address.
    /// </summary>
    public class PICOperandProgMemoryAddress : MachineOperand, IOperand
    {

        /// <summary>
        /// Instantiates a Absolute Code Address operand. Used by GOTO, CALL instructions.
        /// </summary>
        /// <param name="absAddr">The program word address.</param>
        public PICOperandProgMemoryAddress(uint absAddr) : base(PrimitiveType.Ptr32)
        {
            CodeTarget = PICProgAddress.Ptr((absAddr << 1) & PICProgAddress.MAXPROGBYTADDR);
            RelativeWordOffset = Constant.Int32(0);
        }

        /// <summary>
        /// Instantiates a code relative address operand. Used by BRA, RCALL, etc...
        /// </summary>
        /// <param name="off">The code relative offset.</param>
        /// <param name="instrAddr">The instruction address.</param>
        public PICOperandProgMemoryAddress(short off, Address instrAddr) : base(PrimitiveType.Ptr32)
        {
            CodeTarget = PICProgAddress.Ptr(((uint)((long)instrAddr.ToUInt32() + 2 + (off * 2)) & PICProgAddress.MAXPROGBYTADDR));
            RelativeWordOffset = Constant.Int32(off);
        }

        /// <summary>
        /// Gets the absolute code target byte address. This should be a word-aligned address.
        /// </summary>
        public PICProgAddress CodeTarget { get; }

        /// <summary>
        /// Gets the relative offset. This a word offset.
        /// </summary>
        public Constant RelativeWordOffset { get; }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString($"{CodeTarget}");
        }

        public void Accept(IOperandVisitor visitor) => visitor.VisitProgMemory(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitProgMemory(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitProgMemory(this, context);

    }

    /// <summary>
    /// A PIC data memory full address.
    /// </summary>
    public class PICOperandDataMemoryAddress : MachineOperand, IOperand
    {

        /// <summary>
        /// Instantiates a Absolute Data Address operand. Used by MOVFF instructions.
        /// </summary>
        /// <param name="absAddr">The data byte address.</param>
        public PICOperandDataMemoryAddress(uint absAddr) : base(PrimitiveType.Ptr32)
        {
            DataTarget = PICDataAddress.Ptr(absAddr);
        }

        /// <summary>
        /// Gets the absolute data target byte address.
        /// </summary>
        public PICDataAddress DataTarget { get; }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString($"{DataTarget}");
        }

        public void Accept(IOperandVisitor visitor) => visitor.VisitDataMemory(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitDataMemory(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitDataMemory(this, context);

    }

    /// <summary>
    /// A PIC data bank-relative memory operand.
    /// </summary>
    public class PICOperandOffsetBankedMemory : MachineOperand, IOperand
    {

        /// <summary>
        /// Instantiates a banked data memory address operand.
        /// </summary>
        /// <param name="bsr">The BSR register or none for Access-RAM.</param>
        /// <param name="off">The offset in the memory bank.</param>
        /// <param name="accFlag">True to specify the Access-RAM bank.</param>
        public PICOperandOffsetBankedMemory(Constant off) : base(PrimitiveType.UInt16)
        {
            Offset = off;
        }

        /// <summary>
        /// Gets the bank offset or absolute data address.
        /// </summary>
        public Constant Offset { get; }

        public virtual void Accept(IOperandVisitor visitor) => visitor.VisitOffsetBankedMem(this);
        public virtual T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitOffsetBankedMem(this);
        public virtual T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitOffsetBankedMem(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString($"{Offset}");
        }

    }

    /// <summary>
    /// A PIC FSR indexation mode operand.
    /// </summary>
    public class PICOperandFSRIndexation : MachineOperand, IOperand
    {
        /// <summary>
        /// Instantiates a FSR indexation mode operand.
        /// </summary>
        /// <param name="off">The offset relative to the FSR register content.</param>
        /// <param name="mode">The indexation mode with the FSR.</param>
        public PICOperandFSRIndexation(Constant off, FSRIndexedMode mode) : base((PrimitiveType)off.DataType)
        {
            Offset = off;
            Mode = mode;
        }

        /// <summary>
        /// Gets the offset to the index register.
        /// </summary>
        public Constant Offset { get; }

        /// <summary>
        /// Gets the indexation mode.
        /// </summary>
        public FSRIndexedMode Mode { get; }

        public virtual void Accept(IOperandVisitor visitor) => visitor.VisitFSRIndexation(this);
        public virtual T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitFSRIndexation(this);
        public virtual T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitFSRIndexation(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// A PIC register with destination flag (like "f,d").
    /// Used by Byte-oriented instructions (ADDWF, XORWF, ...)
    /// </summary>
    public class PICOperandWregDest : MachineOperand, IOperand
    {

        /// <summary>
        /// Gets the indication if the Working Register is the destination of the operation.
        /// If false, the Register is the destination.
        /// </summary>
        public readonly Constant WregIsDest;

        public PICOperandWregDest(ushort dest) : base(PrimitiveType.Bool)
        {
            WregIsDest = Constant.Bool(dest == 0);
        }

        public void Accept(IOperandVisitor visitor) => visitor.VisitWregDest(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitWregDest(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitWregDest(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString((WregIsDest.ToBoolean() ? ",W" : ",F"));
        }

    }

    /// <summary>
    /// A PIC memory address (like "f,d,a").
    /// Used by Byte-oriented instructions (ADDWF, XORWF, ...)
    /// </summary>
    public class PICOperandIsAccessBank : MachineOperand, IOperand
    {

        public PICOperandIsAccessBank(ushort acc) : base(PrimitiveType.Bool)
        {
            IsAccessBank = Constant.Bool(acc == 0);
        }

        /// <summary>
        /// Gets the indication if the bank offset referred to the Access-RAM. Applicable to PIC18 only.
        /// </summary>
        public Constant IsAccessBank { get; }

        public void Accept(IOperandVisitor visitor) => visitor.VisitIsAccessBank(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitIsAccessBank(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitIsAccessBank(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString((IsAccessBank.ToBoolean() ? ",ACCESS" : ""));
        }

    }

    /// <summary>
    /// A TBLRD/TBLWT Increment Change mode operand. Applicable to PIC18 only.
    /// </summary>
    public class PICOperandTBLRW : MachineOperand, IOperand
    {
        /// <summary>
        /// Gets the TBL increment mode.
        /// </summary>
        public readonly Constant TBLIncrMode;

        private static string[] ops = new string[4] { "*", "*+", "*-", "+*" };

        /// <summary>
        /// Instantiates a TBLRD/TBLWT Increment Change mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public PICOperandTBLRW(ushort incrmode) : base(PrimitiveType.Byte)
        {
            TBLIncrMode = Constant.Byte((byte)(incrmode & 3));
        }

        public void Accept(IOperandVisitor visitor) => visitor.VisitTblRW(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitTblRW(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitTblRW(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            byte mode = TBLIncrMode.ToByte();
            writer.WriteString(ops[mode & 3]);
        }

    }

    /// <summary>
    /// A TRIS register operand. Applicable to PIC16 only.
    /// </summary>
    public class PICOperandTris : MachineOperand, IOperand
    {
        /// <summary>
        /// Instantiates a TRIS instruction operand.
        /// </summary>
        /// <param name="mode">The TRIS register number [5, 6, 7].</param>
        public PICOperandTris(byte trisnum) : base(PrimitiveType.Byte)
        {
            TrisNum = Constant.Byte((byte)(trisnum & 7));
        }

        /// <summary>
        /// Gets the TRIS register number.
        /// </summary>
        public Constant TrisNum { get; }

        public void Accept(IOperandVisitor visitor) => visitor.VisitTris(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitTris(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitTris(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            switch (TrisNum.ToByte())
            {
                case 5:
                    writer.WriteString("A");
                    return;
                case 6:
                    writer.WriteString("B");
                    return;
                case 7:
                    writer.WriteString("C");
                    return;
            }
            throw new InvalidOperationException($"Invalid TRIS operand: {TrisNum}");
        }

    }

    public abstract class PICOperandPseudo : MachineOperand
    {

        public PICOperandPseudo(byte config) : base(PrimitiveType.Byte)
        {
            Values = new ushort[1] { config };
        }

        public PICOperandPseudo(ushort idlocs) : base(PrimitiveType.UInt16)
        {
            Values = new ushort[1] { idlocs };
        }

        public PICOperandPseudo(params byte[] db) : base(PrimitiveType.Byte)
        {
            if (db is null)
                throw new ArgumentNullException(nameof(db));
            Values = new ushort[db.Length];
            Array.Copy(db, Values, Values.Length);
        }

        public PICOperandPseudo(params ushort[] dw) : base(PrimitiveType.UInt16)
        {
            if (dw is null)
                throw new ArgumentNullException(nameof(dw));
            Values = new ushort[dw.Length];
            Array.Copy(dw, Values, Values.Length);
        }

        public ushort[] Values { get; }


    }

    /// <summary>
    /// A PIC data EEPROM series of bytes.
    /// </summary>
    public class PICOperandDEEPROM : PICOperandPseudo, IOperand
    {

        public PICOperandDEEPROM(params byte[] eedata) : base(eedata)
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
    /// A PIC declare ASCII bytes operand.
    /// </summary>
    public class PICOperandDASCII : PICOperandPseudo, IOperand
    {

        public PICOperandDASCII(params byte[] bytes) : base(bytes)
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
    /// A PIC declare data bytes operand.
    /// </summary>
    public class PICOperandDByte : PICOperandPseudo, IOperand
    {

        public PICOperandDByte(params byte[] bytes) : base(bytes)
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
    /// A PIC declare data words operand.
    /// </summary>
    public class PICOperandDWord : PICOperandPseudo, IOperand
    {

        public PICOperandDWord(params ushort[] words) : base(words)
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
    /// A PIC set processor ID locations operand (used for __IDLOCS).
    /// </summary>
    public class PICOperandIDLocs : PICOperandPseudo, IOperand
    {

        private Address addr;

        public PICOperandIDLocs(Address addr, ushort idlocs) : base(idlocs)
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
    /// A PIC processor configuration bits operand (used for CONFIG or __CONFIG).
    /// </summary>
    public class PICOperandConfigBits : PICOperandPseudo, IOperand
    {

        private PICArchitecture arch;
        private Address addr;

        public PICOperandConfigBits(PICArchitecture arch, Address addr, byte config) : base(config)
        {
            this.arch = arch;
            this.addr = addr;
        }

        public PICOperandConfigBits(PICArchitecture arch, Address addr, ushort config) : base(config)
        {
            this.arch = arch;
            this.addr = addr;
        }

        public void Accept(IOperandVisitor visitor) => visitor.VisitConfig(this);
        public T Accept<T>(IOperandVisitor<T> visitor) => visitor.VisitConfig(this);
        public T Accept<T, C>(IOperandVisitor<T, C> visitor, C context) => visitor.VisitConfig(this, context);

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            var s = PICMemoryDescriptor.RenderDeviceConfigRegister(addr, Values[0]);
            writer.WriteString(s);
        }

    }


}
