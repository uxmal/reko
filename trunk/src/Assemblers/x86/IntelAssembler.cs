#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Environments.Msdos;
using Decompiler.Core;
using Decompiler.Core.Assemblers;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Assemblers.x86
{
    /// <summary>
    /// A crude MASM-style assembler for x86 opcodes.
    /// </summary>
    public class IntelAssembler
    {
        private IntelArchitecture arch;
        private Platform platform;
        private Address addrBase;
        private ModRmBuilder modRm;
        private PrimitiveType defaultWordSize;
        private IntelEmitter emitter;
        private SymbolTable symtab;
        private List<EntryPoint> entryPoints;
        private SortedDictionary<string, SignatureLibrary> importLibraries;
        private Dictionary<uint, PseudoProcedure> importThunks;

        public IntelAssembler(IntelArchitecture arch, Address addrBase, IntelEmitter emitter, List<EntryPoint> entryPoints)
        {
            this.arch = new IntelArchitecture(ProcessorMode.Real);
            this.platform = new MsdosPlatform(arch);
            this.addrBase = addrBase;
            this.emitter = emitter;
            this.entryPoints = entryPoints;
            this.defaultWordSize = arch.WordWidth;
            modRm = new ModRmBuilder(defaultWordSize, emitter);
            symtab = new SymbolTable();
            importLibraries = new SortedDictionary<string, SignatureLibrary>();
            importThunks = new Dictionary<uint, PseudoProcedure>();

            SetDefaultWordWidth(defaultWordSize);
        }

        public IntelArchitecture Architecture
        {
            get { return arch; }
        }

        public Platform Platform
        {
            get { return platform; }
        }

        public Dictionary<uint, PseudoProcedure> ImportThunks
        {
            get { return importThunks; }
        }

        public ProgramImage GetImage()
        {
            return new ProgramImage(addrBase, emitter.Bytes);
        }

        public void Mov(ParsedOperand op, int constant)
        {
            ProcessMov(
                op,
                Imm(op.Operand.Width, constant));
        }


        public void Mov(ParsedOperand dst, ParsedOperand src)
        {
            ProcessMov(dst, src);
        }

        public void Sahf()
        {
            emitter.EmitByte(0x9E);
        }

        public void Shld(ParsedOperand op1, ParsedOperand op2, ParsedOperand op3)
        {
            ProcessDoubleShift(0, op1, op2, op3);
        }

        public void Shrd(ParsedOperand op1, ParsedOperand op2, byte count)
        {
            ProcessDoubleShift(4, op1, op2, new ParsedOperand(ImmediateOperand.Byte(count)));
        }

        public void Sub(ParsedOperand op, int constant)
        {
            Sub(op, Imm(op.Operand.Width, constant));
        }

        public void Sub(ParsedOperand minuend, ParsedOperand subtrahend)
        {
            ProcessBinop(0x05, minuend, subtrahend);
        }


        public void Test(ParsedOperand op1, ParsedOperand op2)
        {
            ProcessTest(new ParsedOperand[] { op1, op2 });
        }
        public ParsedOperand BytePtr(int offset)
        {
            return new ParsedOperand(
                new MemoryOperand(PrimitiveType.Byte, new Constant(emitter.AddressWidth, offset)));
        }

        internal PrimitiveType EnsureValidOperandSize(ParsedOperand op)
        {
            PrimitiveType w = op.Operand.Width;
            if (w == null)
                Error("Width of the operand is unknown");
            return w;
        }



        internal void ProcessComm(string sym)
        {
            DefineSymbol(sym);
            emitter.EmitLeUint32(0);
        }

        internal void ProcessFpuCommon(int opcodeFreg, int opcodeMem, int fpuOperation, bool isPop, bool fixedOrder, ParsedOperand[] ops)
        {
            if (ops.Length == 0)
            {
                ops = new ParsedOperand[] { new ParsedOperand(new FpuOperand(0)), new ParsedOperand(new FpuOperand(1)) };
            }
            else if (ops.Length == 1 && ops[0].Operand is FpuOperand)
            {
                ops = new ParsedOperand[] { new ParsedOperand(new FpuOperand(0)), ops[0] };
            }

            FpuOperand fop1 = ops[0].Operand as FpuOperand;
            FpuOperand fop2 = ops.Length > 1 ? ops[1].Operand as FpuOperand : null;
            MemoryOperand mop = ops[0].Operand as MemoryOperand;
            if (mop == null && ops.Length > 1)
                mop = ops[1].Operand as MemoryOperand;
            if (mop != null)
            {
                emitter.EmitOpcode(opcodeMem | (mop.Width == PrimitiveType.Word64 ? 4 : 0), null);
                EmitModRM(fpuOperation, mop, null);
                return;
            }
            if (isPop)
            {
                if (fop1 == null)
                    Error("First operand must be of type ST(n)");
                emitter.EmitOpcode(opcodeFreg, null);
                if (fixedOrder)
                    fpuOperation ^= 1;
                if (fop1.StNumber == 0)
                    EmitModRM(fpuOperation, new ParsedOperand(fop2, null));
                else
                    EmitModRM(fpuOperation, new ParsedOperand(fop1, null));
                return;
            }
            if (fop1 != null)
            {
                fop2 = (FpuOperand) ops[1].Operand;
                if (fop1.StNumber != 0)
                {
                    if (fop2.StNumber != 0)
                        Error("at least one of the floating point stack arguments must be ST(0)");
                    if (fixedOrder)
                        fpuOperation ^= 1;
                    fop2 = fop1;
                }
                emitter.EmitOpcode(opcodeFreg, null);
                EmitModRM(fpuOperation, new ParsedOperand(fop2, null));
                return;
            }
            throw new NotImplementedException("NYI");
        }

        internal void ProcessFst(bool pop, ParsedOperand operand)
        {
            FpuOperand fop = operand.Operand as FpuOperand;
            MemoryOperand mop = operand.Operand as MemoryOperand;
            int regBits = pop ? 3 : 2;
            if (mop != null)
            {
                switch (mop.Width.Size)
                {
                case 4: emitter.EmitOpcode(0xD9, null); break;
                case 8: emitter.EmitOpcode(0xDD, null); break;
                default: Error("Unexpected operator width"); break;
                }
                EmitModRM(regBits, operand);
            }
            else if (fop != null)
            {
                emitter.EmitOpcode(0xDD, null);
                EmitModRM(regBits, new ParsedOperand(fop, null));
            }
            else
                Error("Unexpected operator type");
        }


        public void ProcessImul(params ParsedOperand[] ops)
        {
            PrimitiveType dataWidth;
            if (ops.Length == 1)
            {
                dataWidth = EnsureValidOperandSize(ops[0]);
                emitter.EmitOpcode(0xF6 | IsWordWidth(ops[0].Operand), dataWidth);
                EmitModRM(0x05, ops[0]);
            }
            else
            {
                dataWidth = EnsureValidOperandSizes(ops, 2);
                RegisterOperand regOp = ops[0].Operand as RegisterOperand;
                if (regOp == null)
                    throw new ApplicationException("First operand must be a register");
                if (IsWordWidth(regOp) == 0)
                    throw new ApplicationException("Destination register must be word-width");

                if (ops.Length == 2)
                {
                    emitter.EmitOpcode(0x0F, dataWidth);
                    emitter.EmitByte(0xAF);
                    EmitModRM(RegisterEncoding(regOp.Register), ops[1]);
                }
                else
                {
                    ImmediateOperand op3 = ops[2].Operand as ImmediateOperand;
                    if (op3 == null)
                        throw new ApplicationException("Third operand must be an immediate value");
                    if (IsSignedByte(op3.Value.ToInt32()))
                    {
                        emitter.EmitOpcode(0x6B, dataWidth);
                        EmitModRM(RegisterEncoding(regOp.Register), ops[1]);
                        emitter.EmitByte(op3.Value.ToInt32());
                    }
                    else
                    {
                        emitter.EmitOpcode(0x69, dataWidth);
                        EmitModRM(RegisterEncoding(regOp.Register), ops[1]);
                        emitter.EmitLeImmediate(op3.Value, dataWidth);
                    }
                }
            }
        }

        internal void ProcessIncDec(bool fDec, ParsedOperand op)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(op);
            RegisterOperand regOp = op.Operand as RegisterOperand;
            if (regOp != null)
            {
                if (IsWordWidth(dataWidth) != 0)
                {
                    emitter.EmitOpcode((fDec ? 0x48 : 0x40) | RegisterEncoding(regOp.Register), dataWidth);
                }
                else
                {
                    emitter.EmitOpcode(0xFE | IsWordWidth(dataWidth), dataWidth);
                    EmitModRM(fDec ? 1 : 0, op);
                }
                return;
            }

            MemoryOperand memOp = op.Operand as MemoryOperand;
            if (memOp != null)
            {
                emitter.EmitOpcode(0xFE | IsWordWidth(dataWidth), dataWidth);
                EmitModRM(fDec ? 1 : 0, op);
            }
            else
            {
                throw new ApplicationException("constant operator illegal");
            }
        }

        public void ProcessInOut(bool fOut, params ParsedOperand[] ops)
        {
            ParsedOperand opPort;
            ParsedOperand opData;

            if (fOut)
            {
                opPort = ops[0];
                opData = ops[1];
            }
            else
            {
                opData = ops[0];
                opPort = ops[1];
            }

            RegisterOperand regOpData = opData.Operand as RegisterOperand;
            if (regOpData == null || IsAccumulator(regOpData.Register) == 0)
                throw new ApplicationException("invalid register for in or out instruction");

            int opcode = IsWordWidth(regOpData) | (fOut ? 0xE6 : 0xE4);

            RegisterOperand regOpPort = opPort.Operand as RegisterOperand;
            if (regOpPort != null)
            {
                if (regOpPort.Register == Registers.dx || regOpPort.Register == Registers.edx)
                {
                    emitter.EmitOpcode(8 | opcode, regOpPort.Width);
                }
                else
                    throw new ApplicationException("port must be specified with 'immediate', dx, or edx register");
                return;
            }

            ImmediateOperand immOp = opPort.Operand as ImmediateOperand;
            if (immOp != null)
            {
                if (immOp.Value.ToUInt32() > 0xFF)
                {
                    throw new ApplicationException("port number must be between 0 and 255");
                }
                else
                {
                    emitter.EmitOpcode(opcode, regOpPort.Width);
                    emitter.EmitByte(immOp.Value.ToInt32());
                }
            }
            else
            {
                throw new ApplicationException("port must be specified with 'immediate', dx, or edx register");
            }
        }

        internal void ProcessInt(ParsedOperand vector)
        {
            ImmediateOperand op = (ImmediateOperand) vector.Operand;
            emitter.EmitOpcode(0xCD, null);
            emitter.EmitByte(op.Value.ToInt32());
        }


        internal void ProcessLongBranch(int cc, string destination)
        {
            emitter.EmitOpcode(0x0F, null);
            emitter.EmitByte(0x80 | cc);
            EmitRelativeTarget(destination, emitter.SegmentAddressWidth);
        }

        internal void ProcessLoop(int opcode, string destination)
        {
            emitter.EmitOpcode(0xE0 | opcode, null);
            emitter.EmitByte(-(emitter.Length + 1));
            ReferToSymbol(symtab.CreateSymbol(destination),
                emitter.Length - 1, PrimitiveType.Byte);
        }

        public void ProcessLxs(int prefix, int b, params ParsedOperand[] ops)
        {
            RegisterOperand opDst = (RegisterOperand) ops[0].Operand;

            if (prefix > 0)
            {
                emitter.EmitOpcode(prefix, opDst.Width);
                emitter.EmitByte(b);
            }
            else
                emitter.EmitOpcode(b, emitter.SegmentDataWidth);
            EmitModRM(RegisterEncoding(opDst.Register), ops[1]);
        }


        internal void ProcessMov(params ParsedOperand[] ops)
        {
            PrimitiveType dataWidth = EnsureValidOperandSizes(ops, 2);

            RegisterOperand regOpSrc = ops[1].Operand as RegisterOperand;
            RegisterOperand regOpDst = ops[0].Operand as RegisterOperand;
            if (regOpDst != null)	//$BUG: what about segment registers?
            {
                byte reg = RegisterEncoding(regOpDst.Register);
                if (regOpDst.Register is SegmentRegister)
                {
                    if (regOpSrc != null)
                    {
                        if (regOpSrc.Register is SegmentRegister)
                            Error("Cannot assign between two segment registers");
                        if (ops[1].Operand.Width != PrimitiveType.Word16)
                            Error(string.Format("Values assigned to/from segment registers must be 16 bits wide"));
                        emitter.EmitOpcode(0x8E, PrimitiveType.Word16);
                        EmitModRM(reg, ops[1]);
                        return;
                    }
                    MemoryOperand mopSrc = ops[1].Operand as MemoryOperand;
                    if (mopSrc != null)
                    {
                        emitter.EmitOpcode(0x8E, PrimitiveType.Word16);
                        EmitModRM(reg, mopSrc, ops[1].Symbol);
                        return;
                    }
                }

                if (regOpSrc != null && regOpSrc.Register is SegmentRegister)
                {
                    if (regOpDst.Register is SegmentRegister)
                        Error("Cannot assign between two segment registers");
                    if (ops[0].Operand.Width != PrimitiveType.Word16)
                        Error(string.Format("Values assigned to/from segment registers must be 16 bits wide"));
                    emitter.EmitOpcode(0x8C, PrimitiveType.Word16);
                    EmitModRM(RegisterEncoding(regOpSrc.Register), ops[0]);
                    return;
                }

                int isWord = IsWordWidth(regOpDst);
                if (regOpSrc != null)
                {
                    if (regOpSrc.Width != regOpDst.Width)
                        this.Error(string.Format("size mismatch between {0} and {1}", regOpSrc.Register, regOpDst.Register));
                    emitter.EmitOpcode(0x8A | (isWord & 1), dataWidth);
                    modRm.EmitModRM(reg, regOpSrc);
                    return;
                }

                MemoryOperand memOpSrc = ops[1].Operand as MemoryOperand;
                if (memOpSrc != null)
                {
                    emitter.EmitOpcode(0x8A | (isWord & 1), dataWidth);
                    EmitModRM(reg, memOpSrc, ops[1].Symbol);
                    return;
                }

                ImmediateOperand immOpSrc = ops[1].Operand as ImmediateOperand;
                if (immOpSrc != null)
                {
                    emitter.EmitOpcode(0xB0 | (isWord << 3) | reg, dataWidth);
                    if (isWord != 0)
                        emitter.EmitLe(dataWidth, immOpSrc.Value.ToInt32());
                    else
                        emitter.EmitByte(immOpSrc.Value.ToInt32());

                    if (ops[1].Symbol != null && isWord != 0)
                    {
                        ReferToSymbol(ops[1].Symbol, emitter.Length - (int) immOpSrc.Width.Size, immOpSrc.Width);
                    }
                    return;
                }
                throw new ApplicationException("unexpected");
            }

            MemoryOperand memOpDst = (MemoryOperand) ops[0].Operand;
            regOpSrc = ops[1].Operand as RegisterOperand;
            if (regOpSrc != null)
            {
                if (regOpSrc.Register is SegmentRegister)
                {
                    emitter.EmitOpcode(0x8C, PrimitiveType.Word16);
                }
                else
                {
                    emitter.EmitOpcode(0x88 | IsWordWidth(ops[1].Operand), dataWidth);
                }
                EmitModRM(RegisterEncoding(regOpSrc.Register), memOpDst, ops[0].Symbol);
            }
            else
            {
                ImmediateOperand immOpSrc = (ImmediateOperand) ops[1].Operand;
                int isWord = (dataWidth != PrimitiveType.Byte) ? 1 : 0;
                emitter.EmitOpcode(0xC6 | IsWordWidth(dataWidth), dataWidth);
                EmitModRM(0, memOpDst, ops[0].Symbol);
                emitter.EmitLeImmediate(immOpSrc.Value, dataWidth);
            }
        }
        internal void ProcessMovx(int opcode, ParsedOperand[] ops)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(ops[1]);
            RegisterOperand regDst = ops[0].Operand as RegisterOperand;
            if (regDst == null)
                Error("First operand must be a register");
            emitter.EmitOpcode(0x0F, regDst.Width);
            emitter.EmitByte(opcode | IsWordWidth(dataWidth));
            EmitModRM(RegisterEncoding(regDst.Register), ops[1]);
        }

        internal void ProcessMul(ParsedOperand op)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(op);
            // Single operand doesn't accept immediate values.
            if (op.Operand is ImmediateOperand)
                Error("Immediate operand not allowed for single-argument multiplication");

            emitter.EmitOpcode(0xF6 | IsWordWidth(dataWidth), dataWidth);
            EmitModRM(4, op);
        }



        internal void ProcessPushPop(bool fPop, ParsedOperand op)
        {
            int imm;
            ImmediateOperand immOp = op.Operand as ImmediateOperand;
            if (immOp != null)
            {
                if (fPop)
                    throw new ApplicationException("Can't pop an immediate value");
                imm = immOp.Value.ToInt32();
                if (IsSignedByte(imm))
                {
                    emitter.EmitOpcode(0x6A, PrimitiveType.Byte);
                    emitter.EmitByte(imm);
                }
                else
                {
                    emitter.EmitOpcode(0x68, emitter.SegmentDataWidth);
                    emitter.EmitLe(emitter.SegmentDataWidth, imm);
                }
                return;
            }

            PrimitiveType dataWidth = EnsureValidOperandSize(op);
            RegisterOperand regOp = op.Operand as RegisterOperand;
            if (regOp != null)
            {
                IntelRegister rrr = (IntelRegister) regOp.Register;
                if (rrr.IsBaseRegister)
                {
                    emitter.EmitOpcode(0x50 | (fPop ? 8 : 0) | RegisterEncoding(regOp.Register), dataWidth);
                }
                else
                {
                    int mask = (fPop ? 1 : 0);
                    if (regOp.Register == Registers.es) emitter.EmitByte(0x06 | mask);
                    else
                        if (regOp.Register == Registers.cs) emitter.EmitByte(0x0E | mask);
                        else
                            if (regOp.Register == Registers.ss) emitter.EmitByte(0x16 | mask);
                            else
                                if (regOp.Register == Registers.ds) emitter.EmitByte(0x1E | mask);
                                else
                                    if (regOp.Register == Registers.fs) { emitter.EmitByte(0x0F); emitter.EmitByte(0xA0 | mask); }
                                    else
                                        if (regOp.Register == Registers.gs) { emitter.EmitByte(0x0F); emitter.EmitByte(0xA8 | mask); }
                }
                return;
            }

            emitter.EmitOpcode(fPop ? 0x8F : 0xFF, dataWidth);
            EmitModRM(fPop ? 0 : 6, op);
        }

        internal void ProcessSetCc(byte bits, ParsedOperand op)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(op);
            if (dataWidth != PrimitiveType.Byte)
                Error("Instruction takes only a byte operand");
            emitter.EmitOpcode(0x0F, dataWidth);
            emitter.EmitByte(0x90 | (bits & 0xF));
            EmitModRM(0, op);
        }


        internal void ProcessShiftRotation(byte bits, ParsedOperand dst, ParsedOperand count)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(dst);

            ImmediateOperand immOp = count.Operand as ImmediateOperand;
            if (immOp != null)
            {
                int imm = immOp.Value.ToInt32();
                if (imm == 1)
                {
                    emitter.EmitOpcode(0xD0 | IsWordWidth(dataWidth), dataWidth);
                    EmitModRM(bits, dst);
                }
                else
                {
                    emitter.EmitOpcode(0xC0 | IsWordWidth(dataWidth), dataWidth);
                    EmitModRM(bits, dst, (byte) immOp.Value.ToInt32());
                }
                return;
            }

            RegisterOperand regOp = count.Operand as RegisterOperand;
            if (regOp != null && regOp.Register == Registers.cl)
            {
                emitter.EmitOpcode(0xD2 | IsWordWidth(dataWidth), dataWidth);
                EmitModRM(bits, dst);
                return;
            }

            throw new ApplicationException("Shift/rotate instructions must be followed by a constant or CL");
        }

        internal void ProcessShortBranch(int cc, string destination)
        {
            emitter.EmitOpcode(0x70 | cc, null);
            EmitRelativeTarget(destination, PrimitiveType.Byte);
        }

        internal void ProcessStringInstruction(byte opcode, PrimitiveType width)
        {
            emitter.EmitOpcode(opcode | IsWordWidth(width), width);
        }

        internal void ProcessTest(params ParsedOperand[] ops)
        {
            PrimitiveType dataWidth = EnsureValidOperandSizes(ops, 2);

            RegisterOperand regOpSrc;

            byte isWord = (byte) ((dataWidth != PrimitiveType.Byte) ? 0xFF : 0);

            RegisterOperand regOpDst = ops[0].Operand as RegisterOperand;
            if (regOpDst != null)	//$BUG: what about segment registers?
            {
                byte reg = RegisterEncoding(regOpDst.Register);
                regOpSrc = ops[1].Operand as RegisterOperand;
                if (regOpSrc != null)
                {
                    if (regOpSrc.Width != regOpDst.Width)
                        Error("Operand size mismatch");
                    emitter.EmitOpcode(0x84 | (isWord & 1), dataWidth);
                    modRm.EmitModRM(reg, regOpSrc);
                    return;
                }

                MemoryOperand memOpSrc = ops[1].Operand as MemoryOperand;
                if (memOpSrc != null)
                {
                    emitter.EmitOpcode(0x84 | (isWord & 1), dataWidth);
                    EmitModRM(reg, ops[1]);
                    return;
                }

                ImmediateOperand immOpSrc = ops[1].Operand as ImmediateOperand;
                if (immOpSrc != null)
                {
                    emitter.EmitOpcode(0xF6 | (isWord & 1), dataWidth);
                    EmitModRM(0, ops[0]);
                    if (isWord != 0)
                        emitter.EmitLe(dataWidth, immOpSrc.Value.ToInt32());
                    else
                        emitter.EmitByte(immOpSrc.Value.ToInt32());

                    if (ops[1].Symbol != null && isWord != 0)
                    {
                        Debug.Assert(immOpSrc.Value.ToUInt32() == 0);
                        ReferToSymbol(ops[1].Symbol, emitter.Length - 2, PrimitiveType.Word16);
                    }
                    return;
                }
                throw new ApplicationException("unexpected");
            }

            ImmediateOperand immOp = (ImmediateOperand) ops[1].Operand;
            emitter.EmitOpcode(0xF6 | (isWord & 1), dataWidth);
            EmitModRM(0, ops[0]);
            if (isWord != 0)
                emitter.EmitLeImmediate(immOp.Value, dataWidth);
            else
                emitter.EmitByte(immOp.Value.ToInt32());

            if (ops[1].Symbol != null && isWord != 0)
            {
                ReferToSymbol(ops[1].Symbol, emitter.Length - 2, PrimitiveType.Word16);
            }
        }

        internal void ProcessUnary(int operation, ParsedOperand op)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(op);
            emitter.EmitOpcode(0xF6 | IsWordWidth(dataWidth), dataWidth);
            EmitModRM(operation, op);
        }

        private void DefineSymbol(string pstr)
        {
            symtab.DefineSymbol(pstr, emitter.Position).Resolve(emitter);
        }


        internal void EmitModRM(int reg, ParsedOperand op)
        {
            RegisterOperand regOp = op.Operand as RegisterOperand;
            if (regOp != null)
            {
                modRm.EmitModRM(reg, regOp);
                return;
            }
            FpuOperand fpuOp = op.Operand as FpuOperand;
            if (fpuOp != null)
            {
                modRm.EmitModRM(reg, fpuOp);
            }
            else
            {
                EmitModRM(reg, (MemoryOperand) op.Operand, op.Symbol);
            }
        }

        internal void EmitModRM(int reg, ParsedOperand op, byte b)
        {
            RegisterOperand regOp = op.Operand as RegisterOperand;
            if (regOp != null)
            {
                modRm.EmitModRM(reg, regOp);
                emitter.EmitByte(b);
            }
            else
            {
                EmitModRM(reg, (MemoryOperand) op.Operand, b, op.Symbol);
            }
        }


        internal void EmitModRM(int reg, MemoryOperand memOp, Symbol sym)
        {
            Constant offset = modRm.EmitModRMPrefix(reg, memOp);
            int offsetPosition = emitter.Position;
            EmitOffset(offset);
            if (sym != null)
                ReferToSymbol(sym, offsetPosition, offset.DataType);
        }

        internal void EmitModRM(int reg, MemoryOperand memOp, byte b, Symbol sym)
        {
            Constant offset = modRm.EmitModRMPrefix(reg, memOp);
            emitter.EmitByte(b);
            int offsetPosition = emitter.Position;
            EmitOffset(offset);
            if (sym != null)
                ReferToSymbol(sym, emitter.Position, offset.DataType);
        }

        private void EmitOffset(Constant v)
        {
            if (v == null)
                return;
            emitter.EmitLe((PrimitiveType) v.DataType, v.ToInt32());
        }

        private void EmitRelativeTarget(string target, PrimitiveType offsetSize)
        {
            int offBytes = (int) offsetSize.Size;
            switch (offBytes)
            {
            case 1: emitter.EmitByte(-(emitter.Length + 1)); break;
            case 2: emitter.EmitLeUint16(-(emitter.Length + 2)); break;
            case 4: emitter.EmitLeUint32((uint)-(emitter.Length + 4)); break;
            }
            symtab.CreateSymbol(target).ReferTo(emitter.Length - offBytes, offsetSize, emitter);
        }

        private PrimitiveType EnsureValidOperandSizes(ParsedOperand[] ops, int count)
        {
            if (count == 0)
                return null;
            PrimitiveType w = ops[0].Operand.Width;
            if (count == 1 && ops[0].Operand.Width == null)
                Error("Width of the first operand is unknown");
            if (count == 2)
            {
                if (w == null)
                {
                    w = ops[1].Operand.Width;
                    if (w == null)
                        Error("Width of the first operand is unknown");
                    else
                        ops[0].Operand.Width = w;
                }
                else
                {
                    if (ops[1].Operand.Width == null)
                        ops[1].Operand.Width = w;
                    else if (ops[0].Operand.Width != ops[0].Operand.Width)
                        Error("Operand widths don't match");
                }
            }
            return w;
        }

        private int IsAccumulator(MachineRegister reg)
        {
            return (reg == Registers.eax || reg == Registers.ax || reg == Registers.al) ? 1 : 0;
        }

        private bool IsSignedByte(int n)
        {
            return -128 <= n && n < 128;
        }


        private int IsWordWidth(MachineOperand op)
        {
            return IsWordWidth(op.Width);
        }

        internal void SetDefaultWordWidth(PrimitiveType width)
        {
            emitter.SegmentDataWidth = width;
            emitter.SegmentAddressWidth = width;
            modRm = new ModRmBuilder(width, emitter);
            modRm.Error += modRm_Error;
        }



        private int IsWordWidth(PrimitiveType width)
        {
            if (width == null)
                Error("Operand width is undefined");
            if (width.Size == 1)
                return 0;
            return 1;
        }



        private bool Error(string pstr)
        {
            throw new ApplicationException(pstr);
        }


        private void ReferToSymbol(Symbol psym, int off, DataType width)
        {
            if (psym.fResolved)
            {
                emitter.Patch(off, psym.offset, width);
            }
            else
            {
                // Add forward references to the backpatch list.
                psym.AddForwardReference(off, width);
            }
        }

        public static byte RegisterEncoding(byte b)
        {
            return registerEncodings[b];
        }

        public static byte RegisterEncoding(MachineRegister reg)
        {
            return registerEncodings[reg.Number];
        }

        public static Constant IntegralConstant(int i, PrimitiveType width)
        {
            if (-0x80 <= i && i < 0x80)
                width = PrimitiveType.SByte;
            else if (width == null)
            {
                if (-0x8000 <= i && i < 0x8000)
                    width = PrimitiveType.Word16;
                else
                    width = PrimitiveType.Word32;
            }
            return new Constant(width, i);
        }

        public static Constant IntegralConstant(int i)
        {
            PrimitiveType width;
            if (-0x80 <= i && i < 0x80)
                width = PrimitiveType.SByte;
            else if (-0x8000 <= i && i < 0x8000)
                width = PrimitiveType.Word16;
            else
                width = PrimitiveType.Word32;
            return new Constant(width, i);
        }


        private void modRm_Error(object sender, ErrorEventArgs args)
        {
            Error(args.Message);
        }

        private static readonly byte[] registerEncodings = 
		{
			0x00, // eax
			0x01, // ecx
			0x02, // edx
			0x03, // ebx
			0x04, // esp
			0x05, // ebp
			0x06, // esi
			0x07, // edi

			0x00, // ax
			0x01, // cx
			0x02, // dx
			0x03, // bx
			0x04, // sp
			0x05, // bp
			0x06, // si
			0x07, // di

			0x00, // al
			0x01, // cl
			0x02, // dl
			0x03, // bl
			0x04, // ah
			0x05, // ch
			0x06, // dh
			0x07, // bh

			0x00, // es
			0x01, // cs
			0x02, // ss
			0x03, // ds
			0x04, // fs
			0x05, // gs
		};



        internal void i386()
        {
            arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
            SetDefaultWordWidth(PrimitiveType.Word32);
        }

        public void i86()
        {
            arch = new IntelArchitecture(ProcessorMode.Real);
            platform = new MsdosPlatform(arch);
            SetDefaultWordWidth(PrimitiveType.Word16);
        }

        public void Call(string destination)
        {
            ProcessCallJmp(0xE8, destination);
        }


        public void Call(ParsedOperand parsedOperand)
        {
            ProcessCallJmp(0x02, parsedOperand);
        }





        internal void ProcessFild(ParsedOperand op)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(op);
            int opCode;
            int reg;
            switch (dataWidth.Size)
            {
            case 2: opCode = 0xDF; reg = 0x00; break;
            case 4: opCode = 0xDD; reg = 0x00; break;
            case 8: opCode = 0xDF; reg = 0x05; break;
            default: Error(string.Format("Instruction doesn't support {0}-byte operands", dataWidth.Size)); return;
            }
            emitter.EmitOpcode(opCode, dataWidth);
            EmitModRM(reg, op);
        }

        internal void Fistp(ParsedOperand src)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(src);
            int opCode;
            int reg;
            switch (dataWidth.Size)
            {
            case 2: opCode = 0xDF; reg = 0x03; break;
            case 4: opCode = 0xDB; reg = 0x03; break;
            case 8: opCode = 0xDF; reg = 0x07; break;
            default: Error(string.Format("Instruction doesn't support {0}-byte operands", dataWidth.Size)); return;
            }
            emitter.EmitOpcode(opCode, null);
            EmitModRM(reg, src);
        }

        internal void Fld1()
        {
            emitter.EmitByte(0xD9);
            emitter.EmitByte(0xE8);
        }

        internal void Fldz()
        {
            emitter.EmitOpcode(0xD9, null);
            emitter.EmitByte(0xEE);
        }

        public void Fstsw(ParsedOperand dst)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(dst);
            RegisterOperand regOp = dst.Operand as RegisterOperand;
            if (regOp != null)
            {
                if (regOp.Register != Registers.ax)
                    Error("Register operand must be AX");
                emitter.EmitOpcode(0xDF, dataWidth);
                emitter.EmitByte(0xE0);
            }
            MemoryOperand mop = dst.Operand as MemoryOperand;
            if (mop != null)
            {
                if (dataWidth != PrimitiveType.Word16)
                    Error("Destination must be two bytes");
                emitter.EmitOpcode(0xDD, dataWidth);
                EmitModRM(0x07, dst);
            }
        }

        public void Ret()
        {
            emitter.EmitOpcode(0xC3, null);
        }

        public void Ret(int n)
        {
            emitter.EmitOpcode(0xC2, null);
            emitter.EmitLeUint16(n);
        }

        public void Proc(string procName)
        {
            DefineSymbol(procName);
            if (entryPoints != null && entryPoints.Count == 0)
                entryPoints.Add(new EntryPoint(addrBase + emitter.Position, new IntelState()));
        }

        public void Push(ParsedOperand op)
        {
            ProcessPushPop(false, op);
        }

        public void Adc(ParsedOperand op, int constant)
        {
            Adc(op, new ParsedOperand(new ImmediateOperand(IntelAssembler.IntegralConstant(constant))));
        }

        public void Adc(ParsedOperand op1, ParsedOperand op2)
        {
            ProcessBinop(0x02, op1, op2);
        }

        public void Add(IntelRegister reg, int constant)
        {
            ProcessBinop(
                0x00,
                new ParsedOperand(new RegisterOperand(reg)),
                new ParsedOperand(new ImmediateOperand(IntelAssembler.IntegralConstant(constant))));
        }

        public void Add(ParsedOperand op, int constant)
        {
            ProcessBinop(
                0x00,
                op,
                new ParsedOperand(new ImmediateOperand(IntelAssembler.IntegralConstant(constant))));
        }


        public void And(ParsedOperand dst, ParsedOperand src)
        {
            ProcessBinop(0x04, dst, src);
        }

        public ParsedOperand Imm( uint constant)
        {
            return Imm((int)constant);
        }

        public ParsedOperand Imm(PrimitiveType width, int constant)
        {
            return new ParsedOperand(new ImmediateOperand(IntelAssembler.IntegralConstant(constant, width)));
        }

        public ParsedOperand Imm(int constant)
        {
            return new ParsedOperand(new ImmediateOperand(IntelAssembler.IntegralConstant(constant)));
        }

        public ParsedOperand WordPtr(int directOffset)
        {
            return new ParsedOperand(new MemoryOperand(
                PrimitiveType.Word16,
                IntegralConstant(directOffset, emitter.AddressWidth)));
        }

        public ParsedOperand WordPtr(ParsedOperand reg, int offset)
        {
            return new ParsedOperand(new MemoryOperand(
                PrimitiveType.Word16,
                ((RegisterOperand) reg.Operand).Register,
                IntegralConstant(offset, emitter.AddressWidth)));
        }

        public void Lea(ParsedOperand dst, ParsedOperand addr)
        {
            RegisterOperand ropLhs = (RegisterOperand) dst.Operand;
            emitter.EmitOpcode(0x8D, ropLhs.Width);
            EmitModRM(RegisterEncoding(ropLhs.Register), addr);
        }

        public void ProcessCwd(PrimitiveType width)
        {
            emitter.EmitOpcode(0x99, width);
        }

        public void Add(ParsedOperand dst, ParsedOperand src)
        {
            ProcessBinop(0, dst, src);
        }

        public void Bswap(ParsedOperand op)
        {
            var reg = ((RegisterOperand)op.Operand).Register;
            emitter.EmitOpcode(0x0F, op.Operand.Width);
            emitter.EmitByte(0xC8 | RegisterEncoding(reg));
        }

        public void Dec(ParsedOperand op)
        {
            ProcessIncDec(true, op);
        }

        public void Import(string s, string fnName, string dllName)
        {
            DefineSymbol(s);
            SignatureLibrary lib;
            if (!importLibraries.TryGetValue(dllName, out lib))
            {
                lib = new SignatureLibrary(arch);
                lib.Load(Path.ChangeExtension(dllName, ".xml"));
                importLibraries[dllName] = lib;
            }
            AddImport(fnName, lib.Lookup(fnName), PrimitiveType.Word32);
        }

        public void Imul(ParsedOperand dx)
        {
            ProcessImul(dx);
        }

        public void Int(int serviceVector)
        {
            ProcessInt(Const(serviceVector));

        }

        public void In(ParsedOperand dst, ParsedOperand port)
        {
            ProcessInOut(false, dst, port);
        }

        public void Inc(ParsedOperand op)
        {
            ProcessIncDec(false, op);
        }

        public void Ja(string destination)
        {
            ProcessShortBranch(0x07, destination);
        }

        public void Jcxz(string destination)
        {
            emitter.EmitOpcode(0xE3, null);
            EmitRelativeTarget(destination, PrimitiveType.Byte);
        }

        public void Jnz(string destination)
        {
            ProcessShortBranch(0x05, destination);
        }

        public void Jz(string destination)
        {
            ProcessShortBranch(0x04, destination);
        }

        public void Jmp(string destination)
        {
            ProcessCallJmp(0xE9, destination);
        }

        public void Jmp(ParsedOperand parsedOperand)
        {
            ProcessCallJmp(0x04, parsedOperand);
        }

        public void JmpF(Address address)
        {
            emitter.EmitByte(0xEA);
            emitter.EmitLeUint16((ushort)address.Offset);
            emitter.EmitLeUint16(address.Selector);
        }

        public IntelAssembler Label(string label)
        {
            DefineSymbol(label);
            return this;
        }

        public void Endp(string p)
        {
        }

        public void Enter(int cbStack, int nLevel)
        {
            emitter.EmitOpcode(0xC8, null);
            emitter.EmitLeUint16(cbStack);
            emitter.EmitByte(nLevel);
        }

        public void Equ(string s, int value)
        {
            symtab.Equates[s] = value;
        }

        internal void Extern(string externSymbol, PrimitiveType size)
        {
            DefineSymbol(externSymbol);
            AddImport(externSymbol, null, size);
        }

        public void Neg(ParsedOperand op)
        {
            ProcessUnary(0x03, op);
        }

        public void Not(ParsedOperand op)
        {
            ProcessUnary(0x02, op);
        }

        public void Or(ParsedOperand opDst, ParsedOperand opSrc)
        {
            ProcessBinop(0x01, opDst, opSrc);
        }

        public void Out(ParsedOperand op1, ParsedOperand op2)
        {
            ProcessInOut(true, op1, op2);
        }

        public void Pop(ParsedOperand op)
        {
            ProcessPushPop(true, op);
        }

        internal void ProcessBinop(int binop, params ParsedOperand[] ops)
        {
            PrimitiveType dataWidth = EnsureValidOperandSizes(ops, 2);
            ImmediateOperand immOp = ops[1].Operand as ImmediateOperand;
            if (immOp != null)
            {
                int imm = immOp.Value.ToInt32();
                RegisterOperand regOpDst = ops[0].Operand as RegisterOperand;
                if (regOpDst != null && IsAccumulator(regOpDst.Register) != 0)
                {
                    emitter.EmitOpcode((binop << 3) | 0x04 | IsWordWidth(ops[0].Operand), dataWidth);
                    emitter.EmitLeImmediate(immOp.Value, dataWidth);
                    return;
                }

                switch (dataWidth.Size)
                {
                default:
                    Error("Must specify operand width");
                    return;
                case 1:
                    emitter.EmitOpcode(0x80, dataWidth);
                    EmitModRM(binop, ops[0]);
                    emitter.EmitByte(imm);
                    break;
                case 2:
                case 4:
                    if (IsSignedByte(imm))
                    {
                        emitter.EmitOpcode(0x83, dataWidth);
                        EmitModRM(binop, ops[0]);
                        emitter.EmitByte(imm);
                    }
                    else
                    {
                        emitter.EmitOpcode(0x81, dataWidth);
                        EmitModRM(binop, ops[0]);
                        emitter.EmitLeImmediate(immOp.Value, dataWidth);
                    }
                    break;
                }
                return;
            }

            MemoryOperand memOpDst = ops[0].Operand as MemoryOperand;
            if (memOpDst != null)
            {
                RegisterOperand regOpSrc = (RegisterOperand) ops[1].Operand;
                emitter.EmitOpcode((binop << 3) | 0x00 | IsWordWidth(ops[1].Operand), dataWidth);
                EmitModRM(RegisterEncoding(regOpSrc.Register), ops[0]);
            }
            else
            {
                RegisterOperand regOpDst = ops[0].Operand as RegisterOperand;
                emitter.EmitOpcode((binop << 3) | 0x02 | IsWordWidth(regOpDst), dataWidth);
                EmitModRM(RegisterEncoding(regOpDst.Register), ops[1]);
            }
        }

        internal void ProcessBitOp(ParsedOperand[] ops)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(ops[0]);
            ImmediateOperand imm2 = ops[1].Operand as ImmediateOperand;
            if (imm2 != null)
            {
                emitter.EmitOpcode(0x0F, dataWidth);
                emitter.EmitByte(0xBA);
                EmitModRM(0x04, ops[0]);
                emitter.EmitByte(imm2.Value.ToInt32());
            }
            else
            {
                emitter.EmitOpcode(0x0F, dataWidth);
                emitter.EmitByte(0xA3);
                EmitModRM(RegisterEncoding(((RegisterOperand) ops[1].Operand).Register), ops[0]);
            }
        }

        internal void ProcessBitScan(byte opCode, ParsedOperand dst, ParsedOperand src)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(src);
            RegisterOperand regDst = dst.Operand as RegisterOperand;
            if (regDst == null)
                Error("First operand of bit scan instruction must be a register");
            emitter.EmitOpcode(0x0F, dataWidth);
            emitter.EmitByte(opCode);
            EmitModRM(RegisterEncoding(regDst.Register), src);
        }


        internal void ProcessCallJmp(int direct, string destination)
        {
            emitter.EmitOpcode(direct, null);
            EmitRelativeTarget(destination, emitter.SegmentAddressWidth);
        }

        internal void ProcessCallJmp(int indirect, ParsedOperand op)
        {
            emitter.EmitOpcode(0xFF, emitter.SegmentDataWidth);
            EmitModRM(indirect, op);
        }

        internal void ProcessDiv(int operation, ParsedOperand op)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(op);
            emitter.EmitOpcode(0xF6 | IsWordWidth(dataWidth), dataWidth);
            EmitModRM(operation, op);
        }

        internal void ProcessDoubleShift(byte bits, ParsedOperand op0, ParsedOperand op1, ParsedOperand count)
        {
            PrimitiveType dataWidth = EnsureValidOperandSize(op0);

            RegisterOperand regSrc = op1.Operand as RegisterOperand;
            if (regSrc == null)
                Error("Second operand of SHLD/SHRD must be a register");

            ImmediateOperand immShift = count.Operand as ImmediateOperand;
            RegisterOperand regShift = count.Operand as RegisterOperand;
            if (regShift != null && regShift.Register == Registers.cl)
            {
                bits |= 0x01;
            }
            else if (immShift == null)
            {
                Error("SHLD/SHRD instruction must be followed by a constant or CL");
            }

            emitter.EmitOpcode(0x0F, dataWidth);
            emitter.EmitByte(0xA4 | bits);
            EmitModRM(RegisterEncoding(regSrc.Register), op0);
            if (immShift != null)
                emitter.EmitByte((byte) immShift.Value.ToUInt32());
        }

        public void AddImport(string fnName, ProcedureSignature sig, PrimitiveType size)
        {
            uint u = (uint) (addrBase.Linear + emitter.Position);
            ImportThunks.Add(u, new PseudoProcedure(fnName, sig));
            emitter.EmitLe(size, 0);
        }

        internal OperandParser CreateOperandParser(Lexer lexer)
        {
            return new OperandParser(lexer, symtab, addrBase, emitter.SegmentDataWidth, emitter.SegmentAddressWidth);
        }

        internal void Db(int b)
        {
            emitter.EmitByte(b);
        }

        public void Db(params int [] bytes)
        {
            for (int i = 0; i < bytes.Length; ++i)
            {
                emitter.EmitByte(bytes[i]);
            }
        }

        internal void Dstring(string str)
        {
            emitter.EmitString(str);
        }

        public void Dw(string symbolText)
        {
            DefineWord(PrimitiveType.Word16, symbolText);
        }

        public void Dd(params string[] symbols)
        {
            foreach (string sym in symbols)
            {
                DefineWord(PrimitiveType.Word32, sym);
            }
        }

        public void Dd(params int[] values)
        {
            foreach (int n in values)
            {
                DefineWord(PrimitiveType.Word32, n);
            }
        }

        public void Dw(int w)
        {
            emitter.EmitByte(w & 0xFF);
            emitter.EmitByte(w >> 8);
        }


        internal void DefineWord(PrimitiveType width, string symbolText)
        {
            Symbol sym = symtab.CreateSymbol(symbolText);
            emitter.EmitLe(width, (int)addrBase.Offset);
            ReferToSymbol(sym, emitter.Length - (int) width.Size, emitter.SegmentAddressWidth);
        }

        internal void DefineWord(PrimitiveType width, int value)
        {
            emitter.EmitLe(width, value);
        }

        internal void ReportUnresolvedSymbols()
        {
            Symbol[] s = symtab.GetUndefinedSymbols();
            if (s.Length > 0)
            {
                StringWriter writer = new StringWriter();
                writer.WriteLine("The following symbols were undefined:");
                for (int i = 0; i < s.Length; ++i)
                {
                    writer.WriteLine("  {0}", s[i].ToString());
                }
                throw new ApplicationException(writer.ToString());
            }
        }

        internal void Leave()
        {
            emitter.EmitOpcode(0xC9, null);
        }

        public void Lodsw()
        {
            ProcessStringInstruction(0xAC, PrimitiveType.Word16);
        }

        public void Loop(string target)
        {
            ProcessLoop(2, target);
        }

        public void Loope(string target)
        {
            ProcessLoop(1, target);
        }

        public void Loopne(string target)
        {
            ProcessLoop(0, target);
        }

        public void Rep()
        {
            emitter.EmitByte(0xF3);
        }

        public void Stc()
        {
            emitter.EmitOpcode(0xF9, null);
        }


        public void Clc()
        {
            emitter.EmitOpcode(0xF8, null);
        }

        public void Cmc()
        {
            emitter.EmitOpcode(0xF5, null);
        }

        public void Cmp(ParsedOperand src, int dst)
        {
            ProcessBinop(0x7, src, Imm(dst));
        }

        public void Fcompp()
        {
            emitter.EmitByte(0xDE);
            emitter.EmitByte(0xD9);
        }


        internal void DbDup(int by, int count)
        {
            emitter.EmitBytes((byte) by, count);
        }

        internal void Xchg(ParsedOperand[] ops)
        {
            PrimitiveType dataWidth = EnsureValidOperandSizes(ops, 2);
            RegisterOperand regOp = ops[0].Operand as RegisterOperand;
            ParsedOperand otherOp = ops[1];
            if (regOp == null)
            {
                regOp = ops[1].Operand as RegisterOperand;
                if (regOp == null)
                {
                    Error("One operand must be a register.");
                }
                otherOp = ops[0];
            }
            emitter.EmitOpcode(0x86 | IsWordWidth(regOp), dataWidth);
            EmitModRM(RegisterEncoding(regOp.Register), otherOp);
        }

        public void Xor(ParsedOperand dst, ParsedOperand src)
        {
            ProcessBinop(0x06, dst, src);
        }

        public ParsedOperand ax
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.ax)); }
        }

        public ParsedOperand bx
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.bx)); }
        }

        public ParsedOperand cx
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.cx)); }
        }

        public ParsedOperand dx
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.dx)); }
        }

        public ParsedOperand si
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.si)); }
        }

        public ParsedOperand di
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.di)); }
        }


        public ParsedOperand al
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.al)); }
        }

        public ParsedOperand cl
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.cl)); }
        }

        public ParsedOperand dl
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.dl)); }
        }

        public ParsedOperand bl
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.bl)); }
        }

        public ParsedOperand ah
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.ah)); }
        }


        public ParsedOperand bh
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.bh)); }
        }


        public ParsedOperand eax
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.eax)); }
        }

        public ParsedOperand ebx
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.ebx)); }
        }

        public ParsedOperand ecx
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.ecx)); }
        }

        public ParsedOperand edx
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.edx)); }
        }

        public ParsedOperand ebp
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.ebp)); }
        }

        public ParsedOperand esp
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.esp)); }
        }

        public ParsedOperand esi
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.esi)); }
        }

        public ParsedOperand edi
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.edi)); }
        }

        public ParsedOperand cs
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.cs)); }
        }

        public ParsedOperand ds
        {
            get { return new ParsedOperand(new RegisterOperand(Registers.ds)); }
        }

        public ParsedOperand Const(int n)
        {
            return new ParsedOperand(new ImmediateOperand(IntegralConstant(n, this.defaultWordSize)));
        }

        public ParsedOperand MemW(SegmentRegister seg, MachineRegister @base, int offset)
        {
            var mem = new MemoryOperand(PrimitiveType.Word16);
            mem.Base = @base;
            mem.Offset = IntegralConstant(offset);
            mem.SegOverride = seg;
            return new ParsedOperand(mem);
        }


        public ParsedOperand MemDw(MachineRegister @base, int scale, string offset)
        {
            return Mem(PrimitiveType.Word32, null, @base, scale, offset);
        }

        public ParsedOperand MemDw(string offset)
        {
            return Mem(PrimitiveType.Word32, null, null, 1, offset);
        }

        public ParsedOperand MemDw(MachineRegister @base, string offset)
        {
            return Mem(PrimitiveType.Word32, null, @base, 1, offset);
        }

        public ParsedOperand MemW(MachineRegister @base, string offset)
        {
            return Mem(PrimitiveType.Word16, null, @base,1, offset);
        }

        public ParsedOperand MemB(MachineRegister @base, string offset)
        {
            return Mem(PrimitiveType.Byte, null, @base,1, offset);
        }

        public ParsedOperand MemW(SegmentRegister seg, MachineRegister @base, string offset)
        {
            return Mem(PrimitiveType.Word16, seg, @base, 1, offset);
        }

        public ParsedOperand MemDw(MachineRegister @base, int offset)
        {
            return Mem(PrimitiveType.Word32, @base, offset);
        }

        public ParsedOperand MemW(MachineRegister @base, int offset)
        {
            return Mem(PrimitiveType.Word16, @base, offset);
        }

        public ParsedOperand MemB(MachineRegister @base, int offset)
        {
            return Mem(PrimitiveType.Byte, @base, offset);
        }

        private ParsedOperand Mem(PrimitiveType width, SegmentRegister seg, MachineRegister @base, int scale, string offset)
        {
            int val;
            MemoryOperand mem;
            Symbol sym;
            if (symtab.Equates.TryGetValue(offset, out val))
            {
                mem = new MemoryOperand(width, @base, IntegralConstant(val, @base.DataType));
                sym = null;
            }
            else
            {
                sym = symtab.CreateSymbol(offset);
                val = (int)this.addrBase.Offset;
                Constant off = new Constant(@base == null
                    ? seg != null ? PrimitiveType.Word16 : PrimitiveType.Word32
                    : @base.DataType,
                    val);
                mem = new MemoryOperand(width, @base ?? MachineRegister.None, off);
            }
            if (seg != null)
            {
                mem.SegOverride = seg;
                emitter.SegmentOverride = seg;
            }
            mem.Scale = (byte)scale;
            if (scale > 1)
            {
                mem.Index = mem.Base;
                mem.Base = MachineRegister.None;
            }
            return new ParsedOperand(mem, sym);
        }

        private ParsedOperand Mem(PrimitiveType width, MachineRegister @base, int offset)
        {
            return new ParsedOperand(
                new MemoryOperand(width, @base, IntegralConstant(offset)));
        }
    }
}

