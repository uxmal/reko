#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Assemblers;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Arch.X86.Assembler
{
    /// <summary>
    /// A crude MASM-style assembler for x86 instructions.
    /// </summary>
    public class X86Assembler
    {
        enum StringInstructionBaseOps : byte
        {
            Move = 0xA4,
            Compare = 0xA6,
            Store = 0xAA,
            Load = 0xAC,
            Scan = 0xAE,
        }
        private readonly Address addrBase;
        private readonly PrimitiveType defaultWordSize;
        private readonly AssembledSegment unknownSegment;
        private readonly SymbolTable symtab;
        private readonly List<ImageSymbol> entryPoints;
        private readonly Dictionary<Address, ImportReference> importReferences;
        private readonly List<AssembledSegment> segments;
        private readonly Dictionary<string, AssembledSegment> mpNameToSegment;
        private readonly Dictionary<Symbol, AssembledSegment> symbolSegments;        // The segment to which a symbol belongs.
        private IProcessorArchitecture arch;
        private ModRmBuilder modRm;
        private IEmitter emitter;
        private AssembledSegment currentSegment;

#nullable disable
        public X86Assembler(IntelArchitecture arch, Address addrBase, List<ImageSymbol> entryPoints)
        {
            this.arch = arch;
            this.addrBase = addrBase;
            this.entryPoints = entryPoints;
            this.defaultWordSize = arch.WordWidth;
            this.textEncoding = Encoding.GetEncoding("ISO_8859-1");
            symtab = new SymbolTable();
            importReferences = new Dictionary<Address, ImportReference>();
            segments = new List<AssembledSegment>();
            mpNameToSegment = new Dictionary<string, AssembledSegment>();
            symbolSegments = new Dictionary<Symbol, AssembledSegment>();
            this.SegmentOverride = RegisterStorage.None;

            unknownSegment = new AssembledSegment(new Emitter(), symtab.DefineSymbol("", 0));
            segments.Add(unknownSegment);

            SwitchSegment(unknownSegment);

            SetDefaultWordWidth(defaultWordSize);
        }

        /// <summary>
        /// This constructor makes an X86Assembler that modifies an existing chunk of memory.
        /// It is intended for small mutations of code (like patching NOPs over unwanted code).
        /// It should not be used for generation of large amounts of code.
        /// </summary>
        /// <param name="program">Program to mutate.</param>
        /// <param name="addrStart">The address at which to start mutating.</param>
        public X86Assembler(Program program, Address addrStart)
        {
            this.arch = (IntelArchitecture) program.Architecture;
            this.addrBase = program.SegmentMap.BaseAddress;
            this.entryPoints = program.EntryPoints.Values.ToList();
            this.defaultWordSize = arch.WordWidth;
            this.textEncoding = Encoding.GetEncoding("ISO_8859-1");
            symtab = new SymbolTable();
            importReferences = program.ImportReferences;
            mpNameToSegment = new Dictionary<string, AssembledSegment>();
            symbolSegments = new Dictionary<Symbol, AssembledSegment>();
            this.SegmentOverride = RegisterStorage.None;
            segments = program.SegmentMap.Segments.Values
                .Select(seg => new AssembledSegment(new Emitter(seg.MemoryArea), new Symbol(seg.Name)))
                .ToList();
            if (!program.SegmentMap.TryFindSegment(addrStart, out var segmentToMutate))
                throw new InvalidOperationException($"Address {addrStart} is not a valid location in the program.");
            var offset = addrStart - segmentToMutate.MemoryArea.BaseAddress;
            var asmSeg = this.segments.Single(seg => seg.Symbol!.Name == segmentToMutate.Name);
            asmSeg.Emitter.Position = (int)offset;
            SwitchSegment(asmSeg);
            SetDefaultWordWidth(defaultWordSize);
        }
#nullable enable

        public int CurrentPosition => this.emitter.Position;

        public Dictionary<Address, ImportReference> ImportReferences
        {
            get { return importReferences; }
        }

        /// <summary>
        /// Extracts the assembled machine code into a <see cref="Program"/> instance.
        /// </summary>
        /// <remarks>
        /// Callers must provide their own <see cref="IPlatform"/> instance.
        /// </remarks>
        public Program GetImage()
        {
            var stm = new MemoryStream();
            LoadSegments(stm);
            var mem = new ByteMemoryArea(addrBase, stm.ToArray());
            RelocateSegmentReferences(mem);
            var segmentMap = new SegmentMap(
                mem.BaseAddress,
                new ImageSegment("code", mem, AccessMode.ReadWriteExecute));
            return new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                null);
        }

        private void LoadSegments(MemoryStream stm)
        {
            foreach (var seg in segments)
            {
                while ((stm.Position & 0xF) != 0)
                {
                    stm.WriteByte(0x90);        // 1-byte NOP instruction.
                }
                
                ushort sel;
                if (addrBase.Selector.HasValue)
                    sel = (ushort)(this.addrBase.Selector.Value + (stm.Position >> 4));
                else
                    sel = 0;
                seg.Selector = sel;
                var ab = seg.Emitter.GetBytes();
                stm.Write(ab, 0, ab.Length);
            }
        }

        private void RelocateSegmentReferences(ByteMemoryArea bmem)
        {
            foreach (var seg in segments)
            {
                foreach (var reloc in seg.Relocations)
                {
                    bmem.WriteLeUInt16((uint)((reloc.Segment.Selector - addrBase.Selector!.Value) * 16u + reloc.Offset), seg.Selector);
                }
            }
        }

        public void Align(int alignment)
        {
            emitter.Align(0, alignment);
        }

        public void Mov(ParsedOperand op, int constant)
        {
            ProcessMov(
                op,
                Imm(op.Operand.DataType, constant));
        }


        public void Mov(ParsedOperand dst, ParsedOperand src)
        {
            ProcessMov(dst, src);
        }

        public void Rcl(ParsedOperand dst, byte c)
        {
            ProcessShiftRotation(0x02, dst, new ParsedOperand(Constant.Byte(c)));
        }

        public void Rcr(ParsedOperand dst, byte c)
        {
            ProcessShiftRotation(0x03, dst, new ParsedOperand(Constant.Byte(c)));
        }

        public void Rol(ParsedOperand dst, byte c)
        {
            ProcessShiftRotation(0x00, dst, new ParsedOperand(Constant.Byte(c)));
        }

        public void Ror(ParsedOperand dst, byte c)
        {
            ProcessShiftRotation(0x01, dst, new ParsedOperand(Constant.Byte(c)));
        }

        public void Sahf()
        {
            emitter.EmitByte(0x9E);
        }

        public void Lahf()
        {
            emitter.EmitByte(0x9F);
        }

        public void Sar(ParsedOperand dst, byte c)
        {
            ProcessShiftRotation(0x07, dst, new ParsedOperand(Constant.Byte(c)));
        }

        public void Sar(ParsedOperand op1, ParsedOperand op2)
        {
            ProcessShiftRotation(0x7, op1, op2);
        }

        public void Shl(ParsedOperand dst, byte c)
        {
            ProcessShiftRotation(0x04, dst, new ParsedOperand(Constant.Byte(c)));
        }

        public void Shr(ParsedOperand dst, byte c)
        {
            ProcessShiftRotation(0x05, dst, new ParsedOperand(Constant.Byte(c)));
        }

        public void Shr(ParsedOperand dst, ParsedOperand sh)
        {
            ProcessShiftRotation(0x05, dst, sh);
        }

        public void Shld(ParsedOperand op1, ParsedOperand op2, ParsedOperand op3)
        {
            ProcessDoubleShift(0, op1, op2, op3);
        }

        public void Shrd(ParsedOperand op1, ParsedOperand op2, byte count)
        {
            ProcessDoubleShift(8, op1, op2, new ParsedOperand(Constant.Byte(count)));
        }

        public void Sub(ParsedOperand op, int constant)
        {
            Sub(op, Imm(op.Operand.DataType, constant));
        }

        public void Sub(ParsedOperand minuend, ParsedOperand subtrahend)
        {
            ProcessBinop(0x05, minuend, subtrahend);
        }

        public void Test(ParsedOperand op1, ParsedOperand op2)
        {
            ProcessTest( op1, op2 );
        }

        public void Test(ParsedOperand op1, int imm)
        {
            ProcessTest(op1, Imm(imm));
        }

        public ParsedOperand BytePtr(int offset)
        {
            AddressWidth = SegmentAddressWidth;
            return new ParsedOperand(
                new MemoryOperand(PrimitiveType.Byte, Constant.Create(AddressWidth, offset)));
        }

        internal DataType? EnsureValidOperandSize(ParsedOperand op)
        {
            DataType w = op.Operand.DataType;
            if (w is null)
                Error("DataType of the operand is unknown");
            return w;
        }

        internal void ProcessComm(string sym)
        {
            DefineSymbol(sym);
            emitter.EmitLeUInt32(0);
        }

        internal void ProcessFpuCommon(int opcodeFreg, int opcodeMem, int fpuOperation, bool isPop, bool fixedOrder, params ParsedOperand[] ops)
        {
            if (ops.Length == 0)
            {
                ops = new ParsedOperand[] { new ParsedOperand(new FpuOperand(0)), new ParsedOperand(new FpuOperand(1)) };
            }
            else if (ops.Length == 1 && ops[0].Operand is FpuOperand)
            {
                ops = new ParsedOperand[] { new ParsedOperand(new FpuOperand(0)), ops[0] };
            }

            FpuOperand? fop1 = ops[0].Operand as FpuOperand;
            FpuOperand? fop2 = ops.Length > 1 ? ops[1].Operand as FpuOperand : null;
            MemoryOperand? mop = ops[0].Operand as MemoryOperand;
            if (mop is null && ops.Length > 1)
                mop = ops[1].Operand as MemoryOperand;
            if (mop is not null)
            {
                EmitOpcode(opcodeMem | (mop.DataType == PrimitiveType.Word64 ? 4 : 0), null);
                EmitModRM(fpuOperation, mop, null);
                return;
            }
            if (isPop)
            {
                if (fop1 is null)
                {
                    Error("First operand must be of type ST(n)");
                    return;
                }
                EmitOpcode(opcodeFreg, null);
                if (fixedOrder)
                    fpuOperation ^= 1;
                if (fop1.StNumber == 0)
                    EmitModRM(fpuOperation, new ParsedOperand(fop2!, null));
                else
                    EmitModRM(fpuOperation, new ParsedOperand(fop1, null));
                return;
            }
            if (fop1 is not null)
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
                EmitOpcode(opcodeFreg, null);
                EmitModRM(fpuOperation, new ParsedOperand(fop2, null));
                return;
            }
            throw new NotImplementedException("NYI");
        }

        internal void ProcessFst(bool pop, ParsedOperand operand)
        {
            int regBits = pop ? 3 : 2;
            if (operand.Operand is MemoryOperand mop)
            {
                switch (mop.DataType.Size)
                {
                case 4: EmitOpcode(0xD9, null); break;
                case 8: EmitOpcode(0xDD, null); break;
                default: Error("Unexpected operator width"); break;
                }
                EmitModRM(regBits, operand);
            }
            else if (operand.Operand is FpuOperand fop)
            {
                EmitOpcode(0xDD, null);
                EmitModRM(regBits, new ParsedOperand(fop, null));
            }
            else
                Error("Unexpected operator type");
        }


        public void ProcessImul(params ParsedOperand[] ops)
        {
            DataType? dataWidth;
            if (ops.Length == 1)
            {
                dataWidth = EnsureValidOperandSize(ops[0]);
                EmitOpcode(0xF6 | IsWordWidth(ops[0].Operand), dataWidth);
                EmitModRM(0x05, ops[0]);
            }
            else
            {
                dataWidth = EnsureValidOperandSizes(ops, 2);
                if (dataWidth is null)
                    return;
                if (!(ops[0].Operand is RegisterStorage regOp))
                    throw new ApplicationException("First operand must be a register");
                if (IsWordWidth(regOp) == 0)
                    throw new ApplicationException("Destination register must be word-width");

                if (ops.Length == 2)
                {
                    EmitOpcode(0x0F, dataWidth);
                    emitter.EmitByte(0xAF);
                    EmitModRM(RegisterEncoding(regOp), ops[1]);
                }
                else
                {
                    if (ops[2].Operand is not Constant op3)
                        throw new ApplicationException("Third operand must be an immediate value");
                    if (IsSignedByte(op3.ToInt32()))
                    {
                        EmitOpcode(0x6B, dataWidth);
                        EmitModRM(RegisterEncoding(regOp), ops[1]);
                        emitter.EmitByte(op3.ToInt32());
                    }
                    else
                    {
                        EmitOpcode(0x69, dataWidth);
                        EmitModRM(RegisterEncoding(regOp), ops[1]);
                        emitter.EmitLeImmediate(op3, dataWidth);
                    }
                }
            }
        }

        internal void ProcessIncDec(bool fDec, ParsedOperand op)
        {
            DataType? dataWidth = EnsureValidOperandSize(op);
            if (dataWidth is null)
                return;
            if (op.Operand is RegisterStorage regOp)
            {
                if (IsWordWidth(dataWidth) != 0)
                {
                    EmitOpcode((fDec ? 0x48 : 0x40) | RegisterEncoding(regOp), dataWidth);
                }
                else
                {
                    EmitOpcode(0xFE | IsWordWidth(dataWidth), dataWidth);
                    EmitModRM(fDec ? 1 : 0, op);
                }
                return;
            }

            if (op.Operand is MemoryOperand memOp)
            {
                EmitOpcode(0xFE | IsWordWidth(dataWidth), dataWidth);
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

            if (!(opData.Operand is RegisterStorage regOpData) || IsAccumulator(regOpData) == 0)
                throw new ApplicationException("Invalid register for in or out instruction.");

            int opcode = IsWordWidth(regOpData) | (fOut ? 0xE6 : 0xE4);

            if (opPort.Operand is RegisterStorage regOpPort)
            {
                if (regOpPort == Registers.dx || regOpPort == Registers.edx)
                {
                    EmitOpcode(8 | opcode, regOpPort.DataType);
                }
                else
                    throw new ApplicationException("port must be specified with 'immediate', dx, or edx register");
                return;
            }

            if (opPort.Operand is Constant immOp)
            {
                if (immOp.ToUInt32() > 0xFF)
                {
                    throw new ApplicationException("port number must be between 0 and 255");
                }
                else
                {
                    emitter.EmitByte(opcode);
                    emitter.EmitByte(immOp.ToInt32());
                }
            }
            else
            {
                throw new ApplicationException("port must be specified with 'immediate', dx, or edx register");
            }
        }

        internal void ProcessInt(ParsedOperand vector)
        {
            Constant op = (Constant) vector.Operand;
            EmitOpcode(0xCD, null);
            emitter.EmitByte(op.ToInt32());
        }


        internal void ProcessLongBranch(int cc, string destination)
        {
            EmitOpcode(0x0F, null);
            emitter.EmitByte(0x80 | cc);
            EmitRelativeTarget(destination, SegmentAddressWidth);
        }

        internal void ProcessLoop(int opcode, string destination)
        {
            EmitOpcode(0xE0 | opcode, null);
            emitter.EmitByte(-(emitter.Position + 1));
            ReferToSymbol(symtab.CreateSymbol(destination),
                emitter.Position - 1, PrimitiveType.Byte);
        }

        public void ProcessLxs(int prefix, int b, params ParsedOperand[] ops)
        {
            RegisterStorage opDst = (RegisterStorage) ops[0].Operand;

            if (prefix > 0)
            {
                EmitOpcode(prefix, opDst.DataType);
                emitter.EmitByte(b);
            }
            else
                EmitOpcode(b, SegmentDataWidth);
            EmitModRM(RegisterEncoding(opDst), ops[1]);
        }

        internal void ProcessMov(params ParsedOperand[] ops)
        {
            DataType? dataWidth = EnsureValidOperandSizes(ops, 2);
            if (dataWidth is null)
                return;

            RegisterStorage? regOpSrc = ops[1].Operand as RegisterStorage;
            RegisterStorage? regOpDst = ops[0].Operand as RegisterStorage;
            if (regOpDst is not null)	//$BUG: what about segment registers?
            {
                byte reg = RegisterEncoding(regOpDst);
                if (IsSegmentRegister(regOpDst))
                {
                    if (regOpSrc is not null)
                    {
                        if (IsSegmentRegister(regOpSrc))
                            Error("Cannot assign between two segment registers");
                        if (ops[1].Operand.DataType != PrimitiveType.Word16)
                            Error(string.Format("Values assigned to/from segment registers must be 16 bits wide"));
                        EmitOpcode(0x8E, PrimitiveType.Word16);
                        EmitModRM(reg, ops[1]);
                        return;
                    }
                    if (ops[1].Operand is MemoryOperand mopSrc)
                    {
                        EmitOpcode(0x8E, PrimitiveType.Word16);
                        EmitModRM(reg, mopSrc, ops[1].Symbol);
                        return;
                    }
                }

                if (regOpSrc is not null && IsSegmentRegister(regOpSrc))
                {
                    if (IsSegmentRegister(regOpDst))
                        Error("Cannot assign between two segment registers");
                    if (ops[0].Operand.DataType.BitSize != 16)
                        Error(string.Format("Values assigned to/from segment registers must be 16 bits wide"));
                    EmitOpcode(0x8C, PrimitiveType.Word16);
                    EmitModRM(RegisterEncoding(regOpSrc), ops[0]);
                    return;
                }

                int isWord = IsWordWidth(regOpDst);
                if (regOpSrc is not null)
                {
                    if (regOpSrc.DataType.BitSize > regOpDst.DataType.BitSize)
                        this.Error(string.Format("size mismatch between {0} and {1}", regOpSrc, regOpDst));
                    EmitOpcode(0x8A | (isWord & 1), dataWidth);
                    modRm.EmitModRM(reg, regOpSrc);
                    return;
                }

                if (ops[1].Operand is MemoryOperand memOpSrc)
                {
                    EmitOpcode(0x8A | (isWord & 1), dataWidth);
                    EmitModRM(reg, memOpSrc, ops[1].Symbol);
                    return;
                }

                if (ops[1].Operand is Constant immOpSrc)
                {
                    EmitOpcode(0xB0 | (isWord << 3) | reg, dataWidth);
                    if (isWord != 0)
                        emitter.EmitLe(dataWidth, immOpSrc.ToInt32());
                    else
                        emitter.EmitByte(immOpSrc.ToInt32());

                    if (ops[1].Symbol is not null && isWord != 0)
                    {
                        ReferToSymbol(ops[1].Symbol!, emitter.Position - (int) immOpSrc.DataType.Size, immOpSrc.DataType);
                    }
                    return;
                }
                throw new ApplicationException("unexpected");
            }

            MemoryOperand memOpDst = (MemoryOperand) ops[0].Operand;
            regOpSrc = ops[1].Operand as RegisterStorage;
            if (regOpSrc is not null)
            {
                if (IsSegmentRegister(regOpSrc))
                {
                    EmitOpcode(0x8C, PrimitiveType.Word16);
                }
                else
                {
                    EmitOpcode(0x88 | IsWordWidth(ops[1].Operand), dataWidth);
                }
                EmitModRM(RegisterEncoding(regOpSrc), memOpDst, ops[0].Symbol);
            }
            else
            {
                var immOpSrc = (Constant) ops[1].Operand;
                int isWord = (dataWidth != PrimitiveType.Byte) ? 1 : 0;
                EmitOpcode(0xC6 | IsWordWidth(dataWidth), dataWidth);
                EmitModRM(0, memOpDst, ops[0].Symbol);
                emitter.EmitLeImmediate(immOpSrc, dataWidth);
            }
        }

        public static bool IsSegmentRegister(RegisterStorage seg)
        {
            return seg.Domain == Registers.cs.Domain ||
                   seg.Domain == Registers.ds.Domain ||
                   seg.Domain == Registers.es.Domain ||
                   seg.Domain == Registers.fs.Domain ||
                   seg.Domain == Registers.gs.Domain ||
                   seg.Domain == Registers.ss.Domain;
        }

        internal void ProcessMovx(int opcode, ParsedOperand[] ops)
        {
            DataType? dataWidth = EnsureValidOperandSize(ops[1]);
            if (dataWidth is null)
                return;

            if (!(ops[0].Operand is RegisterStorage regDst))
            {
                Error("First operand must be a register");
                return;
            }
            EmitOpcode(0x0F, regDst.DataType);
            emitter.EmitByte(opcode | IsWordWidth(dataWidth));
            EmitModRM(RegisterEncoding(regDst), ops[1]);
        }

        public void Mul(ParsedOperand op)
        {
            DataType? dataWidth = EnsureValidOperandSize(op);
            if (dataWidth is null)
                return;

            // Single operand doesn't accept immediate values.
            if (op.Operand is Constant)
                Error("Immediate operand not allowed for single-argument multiplication");

            EmitOpcode(0xF6 | IsWordWidth(dataWidth), dataWidth);
            EmitModRM(4, op);
        }

        internal void ProcessPushPop(bool fPop, ParsedOperand op)
        {
            int imm;
            if (op.Operand is Constant immOp)
            {
                if (fPop)
                    throw new ApplicationException("Can't pop an immediate value");
                imm = immOp.ToInt32();
                if (IsSignedByte(imm))
                {
                    EmitOpcode(0x6A, PrimitiveType.Byte);
                    emitter.EmitByte(imm);
                }
                else
                {
                    EmitOpcode(0x68, SegmentDataWidth);
                    emitter.EmitLe(SegmentDataWidth, imm);
                }
                return;
            }

            DataType? dataWidth = EnsureValidOperandSize(op);
            if (dataWidth is null)
                return;

            if (op.Operand is RegisterStorage regOp)
            {
                var rrr = regOp;
                if (IsBaseRegister(rrr))
                {
                    EmitOpcode(0x50 | (fPop ? 8 : 0) | RegisterEncoding(regOp), dataWidth);
                }
                else
                {
                    int mask = (fPop ? 1 : 0);
                    if (regOp == Registers.es) emitter.EmitByte(0x06 | mask);
                    else if (regOp == Registers.cs) emitter.EmitByte(0x0E | mask);
                    else if (regOp == Registers.ss) emitter.EmitByte(0x16 | mask);
                    else if (regOp == Registers.ds) emitter.EmitByte(0x1E | mask);
                    else if (regOp == Registers.fs) { emitter.EmitByte(0x0F); emitter.EmitByte(0xA0 | mask); }
                    else if (regOp == Registers.gs) { emitter.EmitByte(0x0F); emitter.EmitByte(0xA8 | mask); }
                }
                return;
            }

            EmitOpcode(fPop ? 0x8F : 0xFF, dataWidth);
            EmitModRM(fPop ? 0 : 6, op);
        }

        private bool IsBaseRegister(RegisterStorage reg)
        {
            var r = (int)reg.Domain;
            return (int)Registers.eax.Domain <= r && r <= (int)Registers.edi.Domain;
        }

        internal void ProcessSetCc(byte bits, ParsedOperand op)
        {
            DataType? dataWidth = EnsureValidOperandSize(op);
            if (dataWidth is null)
                return;
            if (dataWidth != PrimitiveType.Byte)
                Error("Instruction takes only a byte operand");
            EmitOpcode(0x0F, dataWidth);
            emitter.EmitByte(0x90 | (bits & 0xF));
            EmitModRM(0, op);
        }


        internal void ProcessShiftRotation(byte subOpcode, ParsedOperand dst, ParsedOperand count)
        {
            DataType? dataWidth = EnsureValidOperandSize(dst);
            if (dataWidth is null)
                return;
            if (count.Operand is Constant immOp)
            {
                int imm = immOp.ToInt32();
                if (imm == 1)
                {
                    EmitOpcode(0xD0 | IsWordWidth(dataWidth), dataWidth);
                    EmitModRM(subOpcode, dst);
                }
                else
                {
                    EmitOpcode(0xC0 | IsWordWidth(dataWidth), dataWidth);
                    EmitModRM(subOpcode, dst, (byte) immOp.ToInt32());
                }
                return;
            }

            if (count.Operand is RegisterStorage regOp &&
                regOp == Registers.cl)
            {
                EmitOpcode(0xD2 | IsWordWidth(dataWidth), dataWidth);
                EmitModRM(subOpcode, dst);
                return;
            }

            throw new ApplicationException("Shift/rotate instructions must be followed by a constant or CL");
        }

        internal void ProcessShortBranch(int cc, string destination)
        {
            EmitOpcode(0x70 | cc, null);
            EmitRelativeTarget(destination, PrimitiveType.Byte);
        }

        internal void ProcessStringInstruction(byte opcode, PrimitiveType width)
        {
            EmitOpcode(opcode | IsWordWidth(width), width);
        }

        internal void ProcessTest(params ParsedOperand[] ops)
        {
            var dataWidth = EnsureValidOperandSizes(ops, 2);
            if (dataWidth is null)
                return;

            byte isWord = (byte) ((dataWidth != PrimitiveType.Byte) ? 0xFF : 0);

            if (ops[0].Operand is RegisterStorage regOpDst)	//$BUG: what about segment registers?
            {
                byte reg = RegisterEncoding(regOpDst);
                if (ops[1].Operand is RegisterStorage regOpSrc)
                {
                    if (regOpSrc.DataType != regOpDst.DataType)
                        Error("Operand size mismatch");
                    EmitOpcode(0x84 | (isWord & 1), dataWidth);
                    modRm.EmitModRM(reg, regOpSrc);
                    return;
                }

                if (ops[1].Operand is MemoryOperand)
                {
                    EmitOpcode(0x84 | (isWord & 1), dataWidth);
                    EmitModRM(reg, ops[1]);
                    return;
                }
                else if (ops[1].Operand is Constant immOpSrc)
                {
                    EmitOpcode(0xF6 | (isWord & 1), dataWidth);
                    EmitModRM(0, ops[0]);
                    if (isWord != 0)
                        emitter.EmitLe(dataWidth, immOpSrc.ToInt32());
                    else
                        emitter.EmitByte(immOpSrc.ToInt32());

                    var sym = ops[1].Symbol;
                    if (sym is not null && isWord != 0)
                    {
                        Debug.Assert(immOpSrc.ToUInt32() == 0);
                        ReferToSymbol(sym, emitter.Position - 2, PrimitiveType.Word16);
                    }
                    return;
                }
                throw new ApplicationException("unexpected");
            }

            var immOp = (Constant) ops[1].Operand;
            EmitOpcode(0xF6 | (isWord & 1), dataWidth);
            EmitModRM(0, ops[0]);
            if (isWord != 0)
                emitter.EmitLeImmediate(immOp, dataWidth);
            else
                emitter.EmitByte(immOp.ToInt32());

            if (ops[1].Symbol is not null && isWord != 0)
            {
                ReferToSymbol(ops[1].Symbol!, emitter.Position - 2, PrimitiveType.Word16);
            }
        }

        internal void ProcessUnary(int operation, ParsedOperand op)
        {
            DataType? dataWidth = EnsureValidOperandSize(op);
            if (dataWidth is null)
                return;
            EmitOpcode(0xF6 | IsWordWidth(dataWidth), dataWidth);
            EmitModRM(operation, op);
        }

        private void DefineSymbol(string pstr)
        {
            var sym = symtab.DefineSymbol(pstr, emitter.Position);
            sym.ResolveLe(emitter);
            ResolveSegmentForwardReferences(sym);
            symbolSegments[sym] = currentSegment;
        }

        private void ResolveSegmentForwardReferences(Symbol sym)
        {
            if (symbolSegments.TryGetValue(sym, out AssembledSegment? asmSeg))
            {
                currentSegment.Relocations.AddRange(asmSeg.Relocations);
            }
        }

        internal void EmitModRM(int reg, ParsedOperand op)
        {
            switch (op.Operand)
            {
            case RegisterStorage regOp:
                modRm.EmitModRM(reg, regOp);
                return;
            case FpuOperand fpuOp:
                modRm.EmitModRM(reg, fpuOp);
                return;
            case MemoryOperand mem:
                EmitModRM(reg, mem, op.Symbol);
                break;
            default:
                throw new NotImplementedException();
            }
        }

        internal void EmitModRM(int reg, ParsedOperand op, byte b)
        {
            if (op.Operand is RegisterStorage regOp)
            {
                modRm.EmitModRM(reg, regOp);
                emitter.EmitByte(b);
            }
            else
            {
                EmitModRM(reg, (MemoryOperand) op.Operand, b, op.Symbol);
            }
        }

        internal void EmitModRM(int reg, MemoryOperand memOp, Symbol? sym)
        {
            Constant? offset = modRm.EmitModRMPrefix(reg, memOp);
            if (offset is null)
                return;
            int offsetPosition = emitter.Position;
            EmitOffset(offset);
            if (sym is not null)
                ReferToSymbol(sym, offsetPosition, offset.DataType);
        }

        internal void EmitModRM(int reg, MemoryOperand memOp, byte b, Symbol? sym)
        {
            Constant? offset = modRm.EmitModRMPrefix(reg, memOp);
            if (offset is null)
                return;
            emitter.EmitByte(b);
            int offsetPosition = emitter.Position;
            EmitOffset(offset);
            if (sym is not null)
                ReferToSymbol(sym, emitter.Position, offset.DataType);
        }

        // width of the address of this opcode.
        public PrimitiveType? AddressWidth { get; set; }

        // Default address width for this segment.
        public PrimitiveType SegmentAddressWidth { get; set; }

        // Default data width for this segment.
        public PrimitiveType SegmentDataWidth { get; set; }

        public RegisterStorage SegmentOverride { get; set; }

        private bool IsDataWidthOverridden(DataType? dataWidth)
        {
            return dataWidth is not null &&
                !dataWidth.IsReal &&
                dataWidth.Size != 1 &&
                dataWidth != SegmentDataWidth;
        }

		public void EmitOpcode(int b, DataType? dataWidth)
		{
			if (SegmentOverride != RegisterStorage.None)
			{
				byte bOv;
				if (SegmentOverride == Registers.es) bOv = 0x26; else
				if (SegmentOverride == Registers.cs) bOv = 0x2E; else
				if (SegmentOverride == Registers.ss) bOv = 0x36; else
				if (SegmentOverride == Registers.ds) bOv = 0x3E; else
				if (SegmentOverride == Registers.fs) bOv = 0x64; else
				if (SegmentOverride == Registers.gs) bOv = 0x65; else
				throw new ArgumentOutOfRangeException(string.Format("Invalid segment register {0}." , SegmentOverride));
				emitter.EmitByte(bOv);
				SegmentOverride = RegisterStorage.None;
			}
			if (IsDataWidthOverridden(dataWidth))
			{
				emitter.EmitByte(0x66);
			}
			if (AddressWidth is not null && AddressWidth != SegmentAddressWidth)
			{
				emitter.EmitByte(0x67);
			}
			emitter.EmitByte(b);
		}

        private void EmitOffset(Constant v)
        {
            if (v is null)
                return;
            emitter.EmitLe((PrimitiveType) v.DataType, v.ToInt32());
        }

        private Symbol EmitRelativeTarget(string target, PrimitiveType offsetSize)
        {
            int offBytes = (int) offsetSize.Size;
            switch (offBytes)
            {
            case 1: emitter.EmitByte(-(emitter.Position + 1)); break;
            case 2: emitter.EmitLeUInt16(-(emitter.Position + 2)); break;
            case 4: emitter.EmitLeUInt32((uint)-(emitter.Position + 4)); break;
            }
            var sym = symtab.CreateSymbol(target);
            sym.ReferToLe(emitter.Position - offBytes, offsetSize, emitter);
            return sym;
        }

        private void EmitReferenceToSymbolSegment(Symbol sym)
        {
            var seg = GetSymbolSegmentReference(sym);
            seg.Relocations.Add(new AssembledSegment.Relocation(currentSegment, (uint) emitter.Position));
            emitter.EmitLeUInt16(0);            // make space for the segment selector, will be overwritten at relocation time.
        }

        private AssembledSegment GetSymbolSegmentReference(Symbol sym)
        {
            if (symbolSegments.TryGetValue(sym, out AssembledSegment? seg))
                return seg;
            seg = new AssembledSegment(emitter, null);
            symbolSegments.Add(sym, seg);
            return seg;
        }

        private DataType? EnsureValidOperandSizes(ParsedOperand[] ops, int count)
        {
            DataType? w = ops[0].Operand.DataType;
            if (count == 1 && ops[0].Operand.DataType is null)
            {
                Error("The width of the first operand is unknown.");
                return null;
            }
            if (count == 2)
            {
                if (w is null)
                {
                    w = ops[1].Operand.DataType;
                    if (w is null)
                        Error("The width of the first operand is unknown");
                    else
                        ops[0].Operand.DataType = w;
                }
                else
                {
                    if (ops[1].Operand.DataType is null)
                        ops[1].Operand.DataType = w;
                    else if (ops[0].Operand.DataType.BitSize < ops[1].Operand.DataType.BitSize)
                        Error("Operand widths don't match");
                }
            }
            return w;
        }

        private int IsAccumulator(RegisterStorage reg)
        {
            return (reg == Registers.eax || reg == Registers.ax || reg == Registers.al) ? 1 : 0;
        }

        private bool IsSignedByte(int n)
        {
            return -128 <= n && n < 128;
        }


        private int IsWordWidth(MachineOperand op)
        {
            return IsWordWidth(op.DataType);
        }

        internal void SetDefaultWordWidth(PrimitiveType width)
        {
            SegmentDataWidth = width;
            SegmentAddressWidth = width;
            modRm = new ModRmBuilder(width, emitter);
            modRm.Error += modRm_Error;
        }

        private void SwitchSegment(AssembledSegment newSegment)
        {
            currentSegment = newSegment;
            emitter = newSegment.Emitter;
            modRm = new ModRmBuilder(defaultWordSize, emitter);
        }


        private int IsWordWidth(DataType? width)
        {
            if (width is null)
                Error("Operand width is undefined");
            else if (width.Size == 1)
                return 0;
            return 1;
        }

        private bool Error(string pstr)
        {
            throw new ApplicationException(pstr);
        }

        private void ReferToSymbol(Symbol psym, int off, DataType width)
        {
            if (psym.IsResolved)
            {
                emitter.PatchLe(off, psym.Offset, width);
            }
            else
            {
                psym.AddForwardReference(off, width, 1);
            }
        }

        public static byte RegisterEncoding(RegisterStorage reg)
        {
            return registerEncodings[reg];
        }

        public static Constant IntegralConstant(int i, DataType? width)
        {
            if (-0x80 <= i && i < 0x80)
                width = PrimitiveType.SByte;
            else if (width is null)
            {
                if (-0x8000 <= i && i < 0x8000)
                    width = PrimitiveType.Word16;
                else
                    width = PrimitiveType.Word32;
            }
            return Constant.Create(width, i);
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
            return Constant.Create(width, i);
        }


        private void modRm_Error(object sender, ErrorEventArgs args)
        {
            Error(args.Message);
        }

        private static readonly Dictionary<RegisterStorage, byte> registerEncodings = 
            new Dictionary<RegisterStorage, byte>
		{
			{ Registers.eax, 0x00 },
			{ Registers.ecx, 0x01 },
			{ Registers.edx, 0x02 },
			{ Registers.ebx, 0x03 },
			{ Registers.esp, 0x04 },
			{ Registers.ebp, 0x05 },
			{ Registers.esi, 0x06 },
			{ Registers.edi, 0x07 },

			{ Registers.ax, 0x00 },
			{ Registers.cx, 0x01 },
			{ Registers.dx, 0x02 },
			{ Registers.bx, 0x03 },
			{ Registers.sp, 0x04 },
			{ Registers.bp, 0x05 },
			{ Registers.si, 0x06 },
			{ Registers.di, 0x07 },

			{ Registers.al, 0x00 },
			{ Registers.cl, 0x01 },
			{ Registers.dl, 0x02 },
			{ Registers.bl, 0x03 },
			{ Registers.ah, 0x04 },
			{ Registers.ch, 0x05 },
			{ Registers.dh, 0x06 },
			{ Registers.bh, 0x07 },

			{ Registers.es, 0x00 },
			{ Registers.cs, 0x01 },
			{ Registers.ss, 0x02 },
			{ Registers.ds, 0x03 },
			{ Registers.fs, 0x04 },
			{ Registers.gs, 0x05 },
		};
        private Encoding textEncoding;

        public void i386()
        {
            arch = new X86ArchitectureFlat32(arch.Services, "x86-protected-32", new Dictionary<string, object>());
            SetDefaultWordWidth(PrimitiveType.Word32);
        }

        public void i86()
        {
            arch = new X86ArchitectureReal(arch.Services, "x86-real-16", new Dictionary<string, object>());
            SetDefaultWordWidth(PrimitiveType.Word16);
        }

        public void Aaa()
        {
            emitter.EmitByte(0x37);
        }

        public void Aam()
        {
            emitter.EmitByte(0xD4);
            emitter.EmitByte(0x0A);
        }

        public void Call(string destination)
        {
            ProcessCallJmp(false, 0xE8, destination);
        }


        public void Call(ParsedOperand parsedOperand)
        {
            var far = (parsedOperand.Operand is MemoryOperand && parsedOperand.Operand.DataType.BitSize == 32 && SegmentDataWidth.BitSize == 16);
            ProcessCallJmp(far, 0x02, parsedOperand);
        }


        public void CallF(string destination)
        {
            ProcessCallJmp(true, 0x9A, destination);
        }


        public void CallF(ParsedOperand parsedOperand)
        {
            ProcessCallJmp(true, 0x03, parsedOperand);
        }

        public void Fadd(ParsedOperand op)
        {
            ProcessFpuCommon(0xD8, 0xD8, 0, false, false, op);
        }

        public void Fcomi(ParsedOperand op1, ParsedOperand op2)
        {
            if (op1.Operand is not FpuOperand st0 || st0.StNumber != 0)
                throw new NotSupportedException("Operand must be st.");
            if (op2.Operand is not FpuOperand st)
                throw new NotSupportedException("Operand must be st(x).");
            EmitOpcode(0xDB, null);
            emitter.EmitByte(0xF0 | st.StNumber);
        }

        public void Fcomp(ParsedOperand op)
        {
            if (op.Operand is MemoryOperand mem)
            {
                EmitOpcode(0xD8, mem.DataType);
                EmitModRM(3, op);
            }
            else
                throw new NotImplementedException($"fcomp {op}");
        }

        public void Fcompp()
        {
            emitter.EmitByte(0xDE);
            emitter.EmitByte(0xD9);
        }

        public void Fiadd(ParsedOperand operand)
        {
            var dataWidth = EnsureValidOperandSize(operand);
            if (dataWidth is null)
                return;
            int opcode;
            switch (dataWidth.Size)
            {
            case 2: opcode = 0xDE; break;
            case 4: opcode = 0xDA; break;
            default: Error(string.Format("Instruction doesn't support {0}-byte operands.", dataWidth.Size)); return;
            }
            EmitOpcode(opcode, dataWidth);
            EmitModRM(0, operand);
        }

        public void Fild(ParsedOperand op)
        {
            var dataWidth = EnsureValidOperandSize(op);
            if (dataWidth is null)
                return;
            int opCode;
            int reg;
            switch (dataWidth.Size)
            {
            case 2: opCode = 0xDF; reg = 0x00; break;
            case 4: opCode = 0xDB; reg = 0x00; break;
            case 8: opCode = 0xDF; reg = 0x05; break;
            default: Error(string.Format("Instruction doesn't support {0}-byte operands", dataWidth.Size)); return;
            }
            EmitOpcode(opCode, dataWidth);
            EmitModRM(reg, op);
        }

        public void Fistp(ParsedOperand src)
        {
            var dataWidth = EnsureValidOperandSize(src);
            if (dataWidth is null)
                return;
            int opCode;
            int reg;
            switch (dataWidth.Size)
            {
            case 2: opCode = 0xDF; reg = 0x03; break;
            case 4: opCode = 0xDB; reg = 0x03; break;
            case 8: opCode = 0xDF; reg = 0x07; break;
            default: Error(string.Format("Instruction doesn't support {0}-byte operands", dataWidth.Size)); return;
            }
            EmitOpcode(opCode, null);
            EmitModRM(reg, src);
        }

        public void Fld1()
        {
            emitter.EmitByte(0xD9);
            emitter.EmitByte(0xE8);
        }

        public void Fldz()
        {
            EmitOpcode(0xD9, null);
            emitter.EmitByte(0xEE);
        }

        public void Fmul(ParsedOperand op)
        {
            ProcessFpuCommon(0xD8, 0xD8, 1, false, false, op);
        }

        public void Fstsw(ParsedOperand dst)
        {
            DataType? dataWidth = EnsureValidOperandSize(dst);
            if (dataWidth is null)
                return;
            if (dst.Operand is RegisterStorage regOp)
            {
                if (regOp != Registers.ax)
                    Error("Register operand must be AX");
                EmitOpcode(0xDF, dataWidth);
                emitter.EmitByte(0xE0);
            }
            if (dst.Operand is MemoryOperand)
            {
                if (dataWidth != PrimitiveType.Word16)
                    Error("Destination must be two bytes");
                EmitOpcode(0xDD, dataWidth);
                EmitModRM(0x07, dst);
            }
        }

        public void Fst(ParsedOperand parsedOperand)
        {
            ProcessFst(false, parsedOperand);
        }

        public void Fstp(ParsedOperand parsedOperand)
        {
            ProcessFst(true, parsedOperand);
        }

        public void Ret()
        {
            EmitOpcode(0xC3, null);
        }

        public void Ret(int n)
        {
            if (n == 0)
            {
                Ret();
            }
            else
            {
                EmitOpcode(0xC2, null);
                emitter.EmitLeUInt16(n);
            }
        }

        public void Retf()
        {
            EmitOpcode(0xCB, null);
        }

        public void Retf(int n)
        {
            if (n == 0)
                Retf();
            else
            {
                EmitOpcode(0xCA, null);
                emitter.EmitLeUInt16(n);
            }
        }

        public void Proc(string procName)
        {
            DefineSymbol(procName);
            //$BUG: should be symbols. the ORG directive specifies the start symbol.
            if (entryPoints is not null && entryPoints.Count == 0)
            {
                entryPoints.Add(
                    ImageSymbol.Procedure(arch, addrBase + emitter.Position));
            }
        }

        public void Push(ParsedOperand op)
        {
            ProcessPushPop(false, op);
        }

        public void Pusha()
        {
            emitter.EmitByte(0x60);
        }

        public void Pushf()
        {
            emitter.EmitByte(0x9C);
        }

        public void Popf()
        {
            emitter.EmitByte(0x9D);
        }

        public void Adc(ParsedOperand op, int constant)
        {
            Adc(op, new ParsedOperand(X86Assembler.IntegralConstant(constant)));
        }

        public void Adc(ParsedOperand op1, ParsedOperand op2)
        {
            ProcessBinop(0x02, op1, op2);
        }

        public void Add(RegisterStorage reg, int constant)
        {
            ProcessBinop(
                0x00,
                new ParsedOperand(reg),
                new ParsedOperand(X86Assembler.IntegralConstant(constant)));
        }

        public void Add(ParsedOperand op, int constant)
        {
            ProcessBinop(
                0x00,
                op,
                new ParsedOperand(X86Assembler.IntegralConstant(constant)));
        }


        public void And(ParsedOperand dst, ParsedOperand src)
        {
            ProcessBinop(0x04, dst, src);
        }

        public void And(ParsedOperand op1, int imm)
        {
            And(op1, Imm(imm));
        }

        public ParsedOperand Imm( uint constant)
        {
            return Imm((int)constant);
        }

        public ParsedOperand Imm(DataType width, int constant)
        {
            return new ParsedOperand(X86Assembler.IntegralConstant(constant, width));
        }

        public ParsedOperand Imm(int constant)
        {
            return new ParsedOperand(X86Assembler.IntegralConstant(constant));
        }

        public ParsedOperand WordPtr(int directOffset)
        {
            return new ParsedOperand(new MemoryOperand(
                PrimitiveType.Word16,
                IntegralConstant(directOffset, AddressWidth)));
        }

        public ParsedOperand WordPtr(ParsedOperand reg, int offset)
        {
            return new ParsedOperand(new MemoryOperand(
                PrimitiveType.Word16,
                (RegisterStorage) reg.Operand,
                IntegralConstant(offset, AddressWidth)));
        }

        public ParsedOperand DwordPtr(ParsedOperand reg, int offset)
        {
            return new ParsedOperand(new MemoryOperand(
                PrimitiveType.Word32,
                (RegisterStorage)reg.Operand,
                IntegralConstant(offset, AddressWidth)));
        }

        public void Lea(ParsedOperand dst, ParsedOperand addr)
        {
            RegisterStorage ropLhs = (RegisterStorage) dst.Operand;
            EmitOpcode(0x8D, ropLhs.DataType);
            EmitModRM(RegisterEncoding(ropLhs), addr);
        }

        public void ProcessCwd(PrimitiveType width)
        {
            EmitOpcode(0x99, width);
        }

        public void Add(ParsedOperand dst, ParsedOperand src)
        {
            ProcessBinop(0, dst, src);
        }

        public void Bswap(ParsedOperand op)
        {
            var reg = (RegisterStorage)op.Operand;
            EmitOpcode(0x0F, op.Operand.DataType);
            emitter.EmitByte(0xC8 | RegisterEncoding(reg));
        }

        public void Dec(ParsedOperand op)
        {
            ProcessIncDec(true, op);
        }

        public void Div(ParsedOperand op)
        {
            ProcessDiv(0x06, op);
        }

        public void Idiv(ParsedOperand op)
        {
            ProcessDiv(0x07, op);
        }

        public void Import(string symbolName, string fnName, string dllName)
        {
            DefineSymbol(symbolName);
            AddImport(dllName, fnName, PrimitiveType.Word32);
        }

        public void Imul(ParsedOperand op)
        {
            ProcessImul(op);
        }

        public void Hlt()
        {
            emitter.EmitByte(0xF4);
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

        public void Jc(string destination)
        {
            ProcessShortBranch(0x02, destination);
        }

        public void Jcxz(string destination)
        {
            EmitOpcode(0xE3, null);
            EmitRelativeTarget(destination, PrimitiveType.Byte);
        }

        public void Jns(string destination)
        {
            ProcessShortBranch(0x09, destination);
        }

        public void Jnz(string destination)
        {
            ProcessShortBranch(0x05, destination);
        }

        public void Jpe(string destination)
        {
            ProcessShortBranch(0x0A, destination);
        }

        public void Jpo(string destination)
        {
            ProcessShortBranch(0x0B, destination);
        }

        public void Jz(string destination)
        {
            ProcessShortBranch(0x04, destination);
        }

        public void Jmp(string destination)
        {
            ProcessCallJmp(false, 0xE9, destination);
        }

        public void Jmp(ParsedOperand parsedOperand)
        {
            ProcessCallJmp(false, 0x04, parsedOperand);
        }

        public void JmpF(Address address)
        {
            emitter.EmitByte(0xEA);
            emitter.EmitLeUInt16((ushort)address.Offset);
            emitter.EmitLeUInt16(address.Selector!.Value);
        }

        public X86Assembler Label(string label)
        {
            DefineSymbol(label);
            return this;
        }

        public void Endp(string p)
        {
        }

        public void Ends()
        {
            SwitchSegment(unknownSegment);
        }


        public void Enter(int cbStack, int nLevel)
        {
            EmitOpcode(0xC8, null);
            emitter.EmitLeUInt16(cbStack);
            emitter.EmitByte(nLevel);
        }

        public void Equ(string s, int value)
        {
            symtab.Equates[s] = value;
        }

        internal void Extern(string externSymbol, PrimitiveType size)
        {
            DefineSymbol(externSymbol);
            AddImport(null, externSymbol, size);
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
            DataType? dataWidth = EnsureValidOperandSizes(ops, 2);
            if (dataWidth is null)
                return;
            if (ops[1].Operand is Constant immOp)
            {
                int imm = immOp.ToInt32();
                if (ops[0].Operand is RegisterStorage regOpDst && IsAccumulator(regOpDst) != 0)
                {
                    EmitOpcode((binop << 3) | 0x04 | IsWordWidth(ops[0].Operand), dataWidth);
                    emitter.EmitLeImmediate(immOp, dataWidth);
                    return;
                }

                switch (dataWidth.Size)
                {
                default:
                    Error("Must specify operand width");
                    return;
                case 1:
                    EmitOpcode(0x80, dataWidth);
                    EmitModRM(binop, ops[0]);
                    emitter.EmitByte(imm);
                    break;
                case 2:
                case 4:
                    if (IsSignedByte(imm))
                    {
                        EmitOpcode(0x83, dataWidth);
                        EmitModRM(binop, ops[0]);
                        emitter.EmitByte(imm);
                    }
                    else
                    {
                        EmitOpcode(0x81, dataWidth);
                        EmitModRM(binop, ops[0]);
                        emitter.EmitLeImmediate(immOp, dataWidth);
                    }
                    break;
                }
                return;
            }

            if (ops[0].Operand is MemoryOperand)
            {
                RegisterStorage regOpSrc = (RegisterStorage) ops[1].Operand;
                EmitOpcode((binop << 3) | 0x00 | IsWordWidth(ops[1].Operand), dataWidth);
                EmitModRM(RegisterEncoding(regOpSrc), ops[0]);
            }
            else
            {
                RegisterStorage regOpDst = (RegisterStorage) ops[0].Operand;
                EmitOpcode((binop << 3) | 0x02 | IsWordWidth(regOpDst), dataWidth);
                EmitModRM(RegisterEncoding(regOpDst), ops[1]);
            }
        }

        internal void ProcessBitOp(ParsedOperand[] ops)
        {
            DataType? dataWidth = EnsureValidOperandSize(ops[0]);
            if (dataWidth is null)
                return;

            if (ops[1].Operand is Constant imm2)
            {
                EmitOpcode(0x0F, dataWidth);
                emitter.EmitByte(0xBA);
                EmitModRM(0x04, ops[0]);
                emitter.EmitByte(imm2.ToInt32());
            }
            else
            {
                EmitOpcode(0x0F, dataWidth);
                emitter.EmitByte(0xA3);
                EmitModRM(RegisterEncoding((RegisterStorage) ops[1].Operand), ops[0]);
            }
        }

        internal void ProcessBitScan(byte opCode, ParsedOperand dst, ParsedOperand src)
        {
            DataType? dataWidth = EnsureValidOperandSize(src);
            if (dataWidth is null)
                return;

            if (!(dst.Operand is RegisterStorage regDst))
            {
                Error("First operand of bit scan instruction must be a register");
                return;
            }
            EmitOpcode(0x0F, dataWidth);
            emitter.EmitByte(opCode);
            EmitModRM(RegisterEncoding(regDst), src);
        }

        internal void ProcessCallJmp(bool far, int direct, string destination)
        {
            EmitOpcode(direct, null);
            if (far)
            {
                var sym = symtab.CreateSymbol(destination);
                sym.ReferToLe(emitter.Position, SegmentAddressWidth, emitter);

                emitter.EmitLe(SegmentAddressWidth, 0);

                EmitReferenceToSymbolSegment(sym);
            }
            else
            {
                EmitRelativeTarget(destination, SegmentAddressWidth);
            }
        }

        internal void ProcessCallJmp(bool far, int indirect, ParsedOperand op)
        {
            if (far) indirect |= 1;
            EmitOpcode(0xFF, SegmentDataWidth);
            EmitModRM(indirect, op);
        }

        internal void ProcessDiv(int operation, ParsedOperand op)
        {
            DataType? dataWidth = EnsureValidOperandSize(op);
            if (dataWidth is null)
                return;

            EmitOpcode(0xF6 | IsWordWidth(dataWidth), dataWidth);
            EmitModRM(operation, op);
        }

        internal void ProcessDoubleShift(byte bits, ParsedOperand op0, ParsedOperand op1, ParsedOperand count)
        {
            DataType? dataWidth = EnsureValidOperandSize(op0);
            if (dataWidth is null)
                return;

            if (!(op1.Operand is RegisterStorage regSrc))
            {
                Error("Second operand of SHLD/SHRD must be a register");
                return;
            }

            Constant? immShift = count.Operand as Constant;
            if (count.Operand is RegisterStorage regShift && regShift == Registers.cl)
            {
                bits |= 0x01;
            }
            else if (immShift is null)
            {
                Error("SHLD/SHRD instruction must be followed by a constant or CL");
            }

            EmitOpcode(0x0F, dataWidth);
            emitter.EmitByte(0xA4 | bits);
            EmitModRM(RegisterEncoding(regSrc), op0);
            if (immShift is not null)
                emitter.EmitByte((byte) immShift.ToUInt32());
        }

        public void AddImport(string? moduleName, string fnName,  PrimitiveType size)
        {
            Address u =  (addrBase + emitter.Position);
            ImportReferences.Add(u, new NamedImportReference(u, Path.GetFileNameWithoutExtension(moduleName), fnName, SymbolType.ExternalProcedure));
            emitter.EmitLe(size, 0);
        }

        internal OperandParser CreateOperandParser(Lexer lexer)
        {
            return new OperandParser(lexer, symtab, addrBase, SegmentDataWidth, SegmentAddressWidth);
        }


        internal void DefineWord(PrimitiveType width, string symbolText)
        {
            Symbol sym = symtab.CreateSymbol(symbolText);
            emitter.EmitLe(width, (int)addrBase.Offset);
            ReferToSymbol(sym, emitter.Position - (int) width.Size, SegmentAddressWidth);
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

        public void Bsr(ParsedOperand dst, ParsedOperand src)
        {
            ProcessBitScan(0xBD, dst, src);
        }

        public void Clc()
        {
            EmitOpcode(0xF8, null);
        }

        public void Cld()
        {
            EmitOpcode(0xFC, null);
        }

        public void Cmc()
        {
            EmitOpcode(0xF5, null);
        }

        public void Cmp(ParsedOperand dst, int src)
        {
            ProcessBinop(0x7, dst, Imm(src));
        }

        public void Cmp(ParsedOperand dst, ParsedOperand src)
        {
            ProcessBinop(0x7, dst, src);
        }

        public void Db(params byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; ++i)
            {
                emitter.EmitByte(bytes[i]);
            }
        }

        public void Db(int b)
        {
            emitter.EmitByte(b);
        }

        internal void Dstring(string str)
        {
            emitter.EmitString(str, textEncoding);
        }

        public void Dw(int w)
        {
            emitter.EmitByte(w & 0xFF);
            emitter.EmitByte(w >> 8);
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

        public void Repeat(int count, Action<X86Assembler> action)
        {
            for (int i = 0; i < count; ++i)
                action(this);
        }

        internal void Leave()
        {
            EmitOpcode(0xC9, null);
        }

        public void Les(ParsedOperand dst, ParsedOperand src)
        {
            ProcessLxs(-1, 0xC4, dst, src);
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

        public void Repne()
        {
            emitter.EmitByte(0xF2);
        }

        public void Lodsb()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Load, PrimitiveType.Byte);
        }
        public void Lodsw()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Load, PrimitiveType.Word16);
        }
        public void Lodsd()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Load, PrimitiveType.Word32);
        }

        public void Stosb()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Store, PrimitiveType.Byte);
        }
        public void Stosw()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Store, PrimitiveType.Word16);
        }
        public void Stosd()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Store, PrimitiveType.Word32);
        }

        public void Movsb()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Move, PrimitiveType.Byte);
        }
        public void Movsw()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Move, PrimitiveType.Word16);
        }
        public void Movsd()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Move, PrimitiveType.Word32);
        }

        public void Movzx(ParsedOperand dst, ParsedOperand src)
        {
            if (dst.Operand is not RegisterStorage regOpDst)
            {
                Error("Destination must be a regsiter");
                return;
            }
            EmitOpcode(0x0F, null);
            emitter.EmitByte(0xB6 | IsWordWidth(src.Operand.DataType));
            EmitModRM(RegisterEncoding(regOpDst), src);
        }

        public void Scasb()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Scan, PrimitiveType.Byte);
        }
        public void Scasw()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Scan, PrimitiveType.Word16);
        }
        public void Scasd()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Scan, PrimitiveType.Word32);
        }

        public void Cmpsb()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Compare, PrimitiveType.Byte);
        }
        public void Cmpsw()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Compare, PrimitiveType.Word16);
        }
        public void Cmpsd()
        {
            ProcessStringInstruction((byte)StringInstructionBaseOps.Compare, PrimitiveType.Word32);
        }

        public ParsedOperand St(int n)
        {
            return new ParsedOperand(new FpuOperand(n));
        }

        public void Stc()
        {
            EmitOpcode(0xF9, null);
        }

        public void Std()
        {
            EmitOpcode(0xFD, null);
        }

        internal void DbDup(int by, int count)
        {
            emitter.EmitBytes((byte) by, count);
        }

        internal void Xchg(ParsedOperand[] ops)
        {
            DataType? dataWidth = EnsureValidOperandSizes(ops, 2);
            if (dataWidth is null)
                return;
            ParsedOperand otherOp = ops[1];
            if (!(ops[0].Operand is RegisterStorage regOp))
            {
                if (!(ops[1].Operand is RegisterStorage regOp2))
                {
                    Error("One operand must be a register.");
                    return;
                }
                regOp = regOp2;
                otherOp = ops[0];
            }
            EmitOpcode(0x86 | IsWordWidth(regOp), dataWidth);
            EmitModRM(RegisterEncoding(regOp), otherOp);
        }

        public void Xor(ParsedOperand dst, ParsedOperand src)
        {
            ProcessBinop(0x06, dst, src);
        }

        public void Xor(ParsedOperand op1, int imm)
        {
            Xor(op1, Imm(imm));
        }


        public ParsedOperand ax
        {
            get { return new ParsedOperand(Registers.ax); }
        }

        public ParsedOperand bx
        {
            get { return new ParsedOperand(Registers.bx); }
        }

        public ParsedOperand cx
        {
            get { return new ParsedOperand(Registers.cx); }
        }

        public ParsedOperand dx
        {
            get { return new ParsedOperand(Registers.dx); }
        }

        public ParsedOperand si
        {
            get { return new ParsedOperand(Registers.si); }
        }

        public ParsedOperand di
        {
            get { return new ParsedOperand(Registers.di); }
        }

        public ParsedOperand sp
        {
            get { return new ParsedOperand(Registers.sp); }
        }

        public ParsedOperand bp
        {
            get { return new ParsedOperand(Registers.bp); }
        }

        public ParsedOperand al
        {
            get { return new ParsedOperand(Registers.al); }
        }

        public ParsedOperand cl
        {
            get { return new ParsedOperand(Registers.cl); }
        }

        public ParsedOperand dl
        {
            get { return new ParsedOperand(Registers.dl); }
        }

        public ParsedOperand bl
        {
            get { return new ParsedOperand(Registers.bl); }
        }

        public ParsedOperand ah
        {
            get { return new ParsedOperand(Registers.ah); }
        }

        public ParsedOperand bh
        {
            get { return new ParsedOperand(Registers.bh); }
        }

        public ParsedOperand eax
        {
            get { return new ParsedOperand(Registers.eax); }
        }

        public ParsedOperand ebx
        {
            get { return new ParsedOperand(Registers.ebx); }
        }

        public ParsedOperand ecx
        {
            get { return new ParsedOperand(Registers.ecx); }
        }

        public ParsedOperand edx
        {
            get { return new ParsedOperand(Registers.edx); }
        }

        public ParsedOperand ebp
        {
            get { return new ParsedOperand(Registers.ebp); }
        }

        public ParsedOperand esp
        {
            get { return new ParsedOperand(Registers.esp); }
        }

        public ParsedOperand esi
        {
            get { return new ParsedOperand(Registers.esi); }
        }

        public ParsedOperand edi
        {
            get { return new ParsedOperand(Registers.edi); }
        }

        public ParsedOperand cs
        {
            get { return new ParsedOperand(Registers.cs); }
        }

        public ParsedOperand ds
        {
            get { return new ParsedOperand(Registers.ds); }
        }

        public ParsedOperand es
        {
            get { return new ParsedOperand(Registers.es); }
        }

        public ParsedOperand st => new ParsedOperand(new FpuOperand(0));

        public ParsedOperand Const(int n)
        {
            return new ParsedOperand(IntegralConstant(n, this.defaultWordSize));
        }

        public ParsedOperand MemW(RegisterStorage seg, RegisterStorage @base, int offset)
        {
            var mem = new MemoryOperand(PrimitiveType.Word16);
            mem.Base = @base;
            mem.Offset = IntegralConstant(offset);
            mem.SegOverride = seg;
            return new ParsedOperand(mem);
        }


        public ParsedOperand MemDw(RegisterStorage @base, int scale, string offset)
        {
            return Mem(PrimitiveType.Word32, null, @base, null, scale, offset);
        }

        public ParsedOperand MemDw(RegisterStorage @base, RegisterStorage index, int scale, string offset)
        {
            return Mem(PrimitiveType.Word32, null, @base, index, scale, offset);
        }

        public ParsedOperand MemDw(string offset)
        {
            return Mem(PrimitiveType.Word32, null, null, null, 1, offset);
        }

        public ParsedOperand MemDw(object @base, string offset)
        {
            RegisterStorage reg = ExpectRegister(@base);
            return Mem(PrimitiveType.Word32, null, reg, null, 1, offset);
        }

        private static RegisterStorage ExpectRegister(object @base)
        {
            if (!(@base is RegisterStorage reg))
            {
                var op = (ParsedOperand) @base;
                reg = (RegisterStorage) op.Operand;
            }
            return reg;
        }

        public ParsedOperand MemW(RegisterStorage @base, string offset)
        {
            return Mem(PrimitiveType.Word16, null, @base, null, 1, offset);
        }

        public ParsedOperand MemB(RegisterStorage @base, string offset)
        {
            return Mem(PrimitiveType.Byte, null, @base, null, 1, offset);
        }

        public ParsedOperand MemW(RegisterStorage seg, RegisterStorage @base, string offset)
        {
            return Mem(PrimitiveType.Word16, seg, @base, null, 1, offset);
        }

        public ParsedOperand MemDw(object @base, int offset)
        {
            return Mem(PrimitiveType.Word32, ExpectRegister(@base), offset);
        }

        public ParsedOperand MemW(RegisterStorage @base, int offset)
        {
            return Mem(PrimitiveType.Word16, @base, offset);
        }

        public ParsedOperand MemB(RegisterStorage @base, int offset)
        {
            return Mem(PrimitiveType.Byte, @base, offset);
        }

        public ParsedOperand MemB(int offset)
        {
            return Mem(PrimitiveType.Byte, RegisterStorage.None, offset);
        }

        private ParsedOperand Mem(
            PrimitiveType width, 
            RegisterStorage? seg, 
            RegisterStorage? @base,  
            RegisterStorage? index, 
            int scale, 
            string offset)
        {
            MemoryOperand mem;
            Symbol? sym = null;
            if (offset is not null)
            {
                if (symtab.Equates.TryGetValue(offset, out int val))
                {
                    mem = new MemoryOperand(width, @base ?? RegisterStorage.None, IntegralConstant(val, @base!.DataType));
                    sym = null;
                }
                else
                {
                    sym = symtab.CreateSymbol(offset);
                    val = (int)this.addrBase.Offset;
                    Constant off = Constant.Create(@base is null
                        ? seg is not null ? PrimitiveType.Word16 : PrimitiveType.Word32
                        : @base.DataType,
                        val);
                    mem = new MemoryOperand(width, @base ?? RegisterStorage.None, off);
                }
            }
            else
            {
                mem = new MemoryOperand(width)
                {
                };
            }
            if (seg is not null)
            {
                mem.SegOverride = seg;
                this.SegmentOverride = seg;
            }
            mem.Scale = (byte)scale;
            if (scale > 1)
            {
                if (index is null)
                {
                    mem.Index = mem.Base;
                    mem.Base = RegisterStorage.None;
                }
                else
                {
                    mem.Index = index;
                    mem.Base = @base!;
                }
            }
            return new ParsedOperand(mem, sym);
        }

        private ParsedOperand Mem(PrimitiveType width, RegisterStorage @base, int offset)
        {
            return new ParsedOperand(
                new MemoryOperand(width, @base, IntegralConstant(offset)));
        }

        public void Segment(string segmentName)
        {
            if (!mpNameToSegment.TryGetValue(segmentName, out AssembledSegment? seg))
            {
                var sym = symtab.DefineSymbol(segmentName, 0);
                seg = new AssembledSegment(new Emitter(), sym);
                segments.Add(seg);
            }
            SwitchSegment(seg);
        }

        public void Xlat()
        {
            emitter.EmitByte(0xD7);
        }
    }
}

