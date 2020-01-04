#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Linq;
using Reko.Assemblers.x86;
using System.ComponentModel.Design;
using System.IO;
using System.Collections.Generic;
using Reko.Core.Services;
using Reko.Core.Lib;

namespace Reko.UnitTests.Scanning
{
	[TestFixture]
	public class Backwalker_x86_Tests
	{
        private ProcedureBuilder m;
        private IProcessorArchitecture arch;
        private ProcessorState state;
        private ExpressionSimplifier expSimp;
        private Identifier SCZO;
        private IBackWalkHost<Block,Instruction> host;

        private class BackwalkerHost : IBackWalkHost<Block,Instruction>
        {
            private IProcessorArchitecture arch;

            #region IBackWalkHost Members

            public BackwalkerHost(IProcessorArchitecture arch)
            {
                this.arch = arch;
            }

            public IProcessorArchitecture Architecture => throw new NotImplementedException();
            public Program Program => throw new NotImplementedException();
            public SegmentMap SegmentMap => throw new NotImplementedException();

            public Tuple<Expression, Expression> AsAssignment(Instruction instr)
            {
                var ass = instr as Assignment;
                if (ass == null)
                    return null;
                return Tuple.Create((Expression)ass.Dst, ass.Src);
            }

            public Expression AsBranch(Instruction instr)
            {
                var bra = instr as Branch;
                if (bra == null)
                    return null;
                return bra.Condition;
            }

            public bool IsFallthrough(Instruction instr, Block block)
            {
                var bra = instr as Branch;
                if (bra == null)
                    return false;
                return bra.Target != block;
            }

            public AddressRange GetSinglePredecessorAddressRange(Address block)
            {
                throw new NotImplementedException();
            }

            public Address GetBlockStartAddress(Address addr)
            {
                throw new NotImplementedException();
            }

            public Block GetSinglePredecessor(Block block)
            {
                return block.Procedure.ControlGraph.Predecessors(block).ToArray()[0];
            }

            public List<Block> GetPredecessors(Block block)
            {
                return block.Procedure.ControlGraph.Predecessors(block).ToList();
            }

            public RegisterStorage GetSubregister(RegisterStorage reg, int off, int width)
            {
                return arch.GetSubregister(reg, off, width);
            }

            public bool IsStackRegister(Storage stg)
            {
                return stg == arch.StackRegister;
            }

            public bool IsValidAddress(Address addr)
            {
                return true;
            }

            public Address MakeAddressFromConstant(Constant c)
            {
                return Address.Ptr32(c.ToUInt32());
            }

            public Address MakeSegmentedAddress(Constant selector, Constant offset)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Instruction> GetBlockInstructions(Block block)
            {
                return block.Statements.Select(s => s.Instruction);
            }

            public int BlockInstructionCount(Block block)
            {
                return block.Statements.Count;
            }
            #endregion
        }

        [SetUp]
        public void Setup()
        {
            arch = new X86ArchitectureFlat32("x86-protected-32");
            m = new ProcedureBuilder();
            var map = new SegmentMap(Address.Ptr32(0x10000000));
            state = arch.CreateProcessorState();
            expSimp = new ExpressionSimplifier(map, state, new FakeDecompilerEventListener());
            SCZO = m.Frame.EnsureFlagGroup(Registers.eflags, (uint)(FlagM.SF | FlagM.CF | FlagM.ZF | FlagM.OF), "SCZO", PrimitiveType.Byte);
            host = new BackwalkerHost(arch);
        }

        private void RunFileTestx86_32(string relativePath, string outputFile)
        {
            Program program;
            var sc = new ServiceContainer();
            var fsSvc = new FileSystemServiceImpl();
            var el = new FakeDecompilerEventListener();
            sc.AddService<IFileSystemService>(fsSvc);
            sc.AddService<DecompilerEventListener>(el);
            var arch = new X86ArchitectureFlat32("x86-protected-32");
            var asm = new X86TextAssembler(sc, arch);
            using (var rdr = new StreamReader(FileUnitTester.MapTestPath(relativePath)))
            {
                var platform = new DefaultPlatform(sc, arch);
                asm.Platform = platform;
                program = asm.Assemble(Address.Ptr32(0x10000000), rdr);
            }
            var scanner = new Scanner(program, null, sc);
            scanner.EnqueueImageSymbol(ImageSymbol.Procedure(arch, program.ImageMap.BaseAddress), true);
            scanner.ScanImage();
            using (var fut = new FileUnitTester(outputFile))
            {
                foreach (var proc in program.Procedures.Values)
                {
                    proc.Write(false, fut.TextWriter);
                }
                fut.AssertFilesEqual();
            }
        }

        private void RunTest(IntelArchitecture arch, RtlTransfer rtlTransfer, string outputFile)
        {
            using (var fut = new FileUnitTester(outputFile))
            {
                m.Procedure.Write(false, fut.TextWriter);
                fut.TextWriter.Flush();

                var ibw = new Backwalker<Block,Instruction>(host, rtlTransfer, expSimp);
                var bwoList = ibw.BackWalk(m.CurrentBlock);
                Assert.IsNotNull(bwoList);
                foreach (BackwalkOperation bwo in bwoList)
                {
                    fut.TextWriter.WriteLine(bwo);
                }
                fut.TextWriter.WriteLine("Index register: {0}", ibw.Index);
                fut.AssertFilesEqual();
            }
        }

        [Test]
        public void BwAdd()
        {
            var eax = m.Frame.EnsureRegister(Registers.eax);
            var bw = new Backwalker<Block,Instruction>(
                host,
                new RtlGoto(m.Mem32(m.IAdd(eax, 0x10000)), InstrClass.Transfer),
                expSimp);
            Assert.IsTrue(bw.BackwalkInstruction(m.Assign(eax, m.IAdd(eax, eax))));
            Assert.AreSame(Registers.eax, bw.Index);
            Assert.AreEqual("* 2", bw.Operations[0].ToString());
            Assert.AreEqual(2, bw.Stride);
        }

        [Test]
        public void BwAndMask()
        {
            var eax = m.Frame.EnsureRegister(Registers.eax);
            var bw = new Backwalker<Block,Instruction>(host, new RtlGoto(m.Mem32(m.IAdd(eax, 0x10000)), InstrClass.Transfer), expSimp);
            Assert.IsFalse(bw.BackwalkInstruction(m.Assign(eax, m.And(eax, 0x7))));
            Assert.AreSame(Registers.eax, bw.Index);
            Assert.AreEqual(0x10000ul, bw.VectorAddress.ToLinear());
            Assert.AreEqual("cmp 8", bw.Operations[0].ToString());
        }

        [Test]
        public void BwJmp()
        {
            var eax = m.Frame.EnsureRegister(Registers.eax);
            var SCZO = m.Frame.EnsureFlagGroup(Registers.eflags, (uint)(FlagM.SF|FlagM.ZF|FlagM.CF|FlagM.OF), "SCZO", PrimitiveType.Byte);
            var bw = new Backwalker<Block,Instruction>(host, new RtlGoto(m.Mem32(m.IAdd(eax, 0x1000)), InstrClass.Transfer), expSimp);
            Assert.IsTrue(
                bw.BackwalkInstruction(
                    m.BranchIf(
                    new TestCondition(ConditionCode.UGT, SCZO),
                    "Nizze").Instruction));
            Assert.AreEqual("branch UGT", bw.Operations[0].ToString());
            Assert.AreEqual("SCZO", bw.UsedFlagIdentifier.ToString());
        }

        [Test]
        public void BwComparison()
        {
            var eax = m.Frame.EnsureRegister(Registers.eax);
            var SCZO = m.Frame.EnsureFlagGroup(Registers.eflags, (uint)(FlagM.SF | FlagM.ZF | FlagM.CF | FlagM.OF), "SCZO", PrimitiveType.Byte);
            var bw = new Backwalker<Block,Instruction>(host, new RtlGoto(m.Mem32(m.IAdd(eax, 0x1000)), InstrClass.Transfer), expSimp);
            bw.UsedFlagIdentifier = m.Frame.EnsureFlagGroup(Registers.eflags,(uint)FlagM.CF, "C", PrimitiveType.Byte);
            Assert.IsFalse(
                bw.BackwalkInstruction(
                    m.Assign(SCZO, new ConditionOf(m.ISub(eax, 3)))),
                "Encountering this comparison should terminate the backwalk");
            Assert.AreSame(Registers.eax, bw.Index);
            Assert.AreEqual("cmp 3", bw.Operations[0].ToString());
        }

        [Test]
        public void BwMaskWithHoles()
        {
            var eax = m.Frame.EnsureRegister(Registers.eax);
            var bw = new Backwalker<Block,Instruction>(host, new RtlGoto(m.Mem32(m.IAdd(eax, 0x10000)), InstrClass.Transfer), expSimp);
            Assert.IsFalse(bw.BackwalkInstruction(m.Assign(eax, m.And(eax, 0x0A))));
            Assert.IsNull(bw.Index);
            Assert.AreEqual(0, bw.Operations.Count);
        }

        [Test]
        public void BwLoadIndexed()
        {
            var eax = m.Frame.EnsureRegister(Registers.eax);
            var edx = m.Frame.EnsureRegister(Registers.edx);
            var al = m.Frame.EnsureRegister(Registers.al);
            m.Frame.EnsureFlagGroup(Registers.eflags, (uint)(FlagM.SF | FlagM.ZF | FlagM.CF | FlagM.OF), "SCZO", PrimitiveType.Byte);
            var bw = new Backwalker<Block,Instruction>(host, new RtlGoto(m.Mem32(m.IAdd(eax, 0x1000)), InstrClass.Transfer), expSimp);
            Assert.IsTrue(bw.BackwalkInstruction(
                m.Assign(al, m.Mem8(m.IAdd(edx, 0x1004)))));
            Assert.AreSame(Registers.edx, bw.Index);
        }

        [Test]
        public void BwXorHiwordOfIndex()
        {
            var bx = m.Frame.EnsureRegister(Registers.bx);
            var bl = m.Frame.EnsureRegister(Registers.bl);
            var bh = m.Frame.EnsureRegister(Registers.bh);
            var bw = new Backwalker<Block,Instruction>(host, new RtlGoto(m.Mem32(m.IAdd(bx, 0x1000)), InstrClass.Transfer), expSimp);

            Assert.IsTrue(bw.BackwalkInstruction(
                m.Assign(bh, m.Xor(bh, bh))));
            Assert.AreSame(Registers.bl, bw.Index);
            Assert.AreEqual("& 255", bw.Operations[0].ToString());
        }

        [Test]
        public void BwSwitch32()
        {
            // samples of switch statement emitted
            // by the Microsoft VC compiler

            var esp = m.Frame.EnsureRegister(Registers.esp);
            var eax = m.Frame.EnsureRegister(Registers.eax);
            var edx = m.Frame.EnsureRegister(Registers.edx);
            var dl = m.Frame.EnsureRegister(Registers.dl);
            var SCZO = m.Frame.EnsureFlagGroup(Registers.eflags, (uint)(FlagM.SF | FlagM.CF | FlagM.ZF | FlagM.OF), "SCZO", PrimitiveType.Byte);

            //m.Proc("foo");
            //m.Mov(m.eax, m.MemDw(Registers.esp, 4));
            //m.Cmp(m.eax, 3);
            //m.Ja("default");

            var block0 = m.CurrentBlock;
            m.Assign(eax, m.Mem32(m.IAdd(esp, 4)));
            m.Assign(SCZO, new ConditionOf(m.ISub(eax, 3)));
            m.BranchIf(new TestCondition(ConditionCode.UGT, SCZO), "default");


            //m.Xor(m.edx, m.edx);
            //m.Mov(m.dl, m.MemB(Registers.eax, "bytes"));
            //m.Jmp(m.MemDw(Registers.edx, 4, "jumps"));

            var block1 = m.CurrentBlock;

            m.Assign(edx, m.Xor(edx, edx));
            m.Assign(SCZO, new ConditionOf(edx));
            m.Assign(dl, m.Mem8(m.IAdd(eax, 0x10000)));


            //m.Label("bytes");
            //m.Db(01, 0, 01, 02);

            //m.Label("jumps");
            //m.Dd("jump0", "jump1", "jump2");

            //m.Label("jump0");
            //m.Assign(eax, 0);
            //m.Jump("done");

            //m.Label("jump1");
            //m.Assign(eax, 1);
            //m.Jump("done");

            //m.Label("jump2");
            //m.Assign(eax, 2);
            //m.Jump("done");

            //m.Label("default");
            //m.Assign(eax, 01);

            //m.Label("done");
            //m.Store(new Constant(0x012310), eax);
            //m.Return();

            //m.Label("dummy");
            //m.Dd(0);

            RunTest(new X86ArchitectureFlat32("x86-protected-32"),
                new RtlGoto(m.Mem32(m.IAdd(m.IMul(edx, 4), 0x10010)), InstrClass.Transfer),
                "Scanning/BwSwitch32.txt");
        }

        [Test]
        public void BwSwitch16()
        {
            var sp = m.Frame.EnsureRegister(Registers.sp);
            var cs = m.Frame.EnsureRegister(Registers.cs);
            var ds = m.Frame.EnsureRegister(Registers.ds);
            var bl = m.Frame.EnsureRegister(Registers.bl);
            var bh = m.Frame.EnsureRegister(Registers.bh);
            var bx = m.Frame.EnsureRegister(Registers.bx);
            var si = m.Frame.EnsureRegister(Registers.si);

            m.Assign(sp, m.ISub(sp, 2));
            m.MStore(sp, cs);
            m.Assign(ds, m.Mem16(sp));
            m.Assign(sp, m.IAdd(sp, 2));
            m.Assign(bl, m.Mem8(si));
            m.Assign(SCZO, new ConditionOf(m.ISub(bl, 2)));
            m.BranchIf(new TestCondition(ConditionCode.UGT, SCZO), "grox");

            m.Assign(bh, m.Xor(bh, bh));
            m.Assign(SCZO, new ConditionOf(bh));
            m.Assign(bx, m.IAdd(bx, bx));
            m.Assign(SCZO, new ConditionOf(bx));

            RunTest(new X86ArchitectureReal("x86-real-16"),
                new RtlGoto(m.Mem16(m.IAdd(bx, 0x1234)), InstrClass.Transfer),
                "Scanning/BwSwitch16.txt");
        }

        [Test]
        public void BwInc()
        {
            var map = new SegmentMap(Address.Ptr32(0x10000000));
            var state = arch.CreateProcessorState();
            var di = new Identifier("di", Registers.di.DataType, Registers.di);
            var bw = new Backwalker<Block, Instruction>(host, new RtlGoto(new MemoryAccess(di, di.DataType), InstrClass.Transfer),
                new ExpressionSimplifier(map, state, new FakeDecompilerEventListener()));
            var instrs = new StatementList(new Block(null, "foo"));
            bw.BackwalkInstructions(Registers.di, new Instruction[] {
                new Assignment(di, new BinaryExpression(Operator.IAdd, di.DataType, di, Constant.Word16(1)))
                });
			Assert.AreSame(Registers.di, bw.Index);
			Assert.AreEqual("+ 1", bw.Operations[0].ToString());
		}

        [Test]
        public void BwDetectIndexRegister()
        {
            var edx = m.Frame.EnsureRegister(Registers.edx);
            var xfer = new RtlGoto(m.Mem32(m.IAdd(m.Word32(0x10001234), m.IMul(edx, 4))), InstrClass.Transfer);
            var bw = new Backwalker<Block,Instruction>(host, xfer, expSimp);
            Assert.AreSame(Registers.edx, bw.Index);
        }

        [Test]
        public void BwIndexInMemoryAddress()
        {
            // samples of switch statement emitted
            // by the Microsoft VC compiler

            var ebp = m.Frame.EnsureRegister(Registers.ebp);
            var eax = m.Frame.EnsureRegister(Registers.eax);
            var edx = m.Frame.EnsureRegister(Registers.edx);
            
            // cmp [ebp-66],1D

            m.Assign(SCZO, m.Cond(m.ISub(m.Mem32(m.ISub(ebp, 0xC4)), 0x1D)));
            var block0 = m.CurrentBlock;
            m.BranchIf(new TestCondition(ConditionCode.UGT, SCZO), "default");

            // mov edx,[ebp-66]
            // movzx eax,byte ptr [edx + 0x10000]
            // jmp [eax + 0x12000]

            m.Assign(edx, m.Mem32(m.ISub(ebp, 0xC4)));
            m.Assign(eax, m.Cast(PrimitiveType.Word32, m.Mem8(m.IAdd(edx, 0x10000))));
            var xfer = new RtlGoto(m.Mem32(m.IAdd(eax, 0x12000)), InstrClass.Transfer);

            var block1 = m.CurrentBlock;
            var bw = new Backwalker<Block,Instruction>(host, xfer, expSimp);
            var ret = bw.BackwalkInstructions(Registers.eax, block1);
            Assert.AreEqual("None", bw.Index.ToString());
            Assert.AreEqual("Mem0[ebp - 0x000000C4:word32]", bw.IndexExpression.ToString());
            Assert.AreEqual(4, bw.JumpSize);
            Assert.IsTrue(ret);

            ret = bw.BackwalkInstructions(null, block0);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void BwTempRegister()
        {
            var v1 = m.Frame.CreateTemporary(PrimitiveType.Word32);
            var edi = m.Frame.CreateTemporary(PrimitiveType.Word32);
            var esi = m.Frame.EnsureRegister(Registers.esi);
            var xfer = new RtlCall(m.Mem32(m.IAdd(esi, 40)), 4, InstrClass.Transfer);
            m.Assign(v1, m.Mem32(edi));
            m.Assign(esi, v1);
            var bw = new Backwalker<Block,Instruction>(host, xfer, expSimp);
            bool result;
            result = bw.BackwalkInstruction(m.Block.Statements[1].Instruction);
            result = bw.BackwalkInstruction(m.Block.Statements[0].Instruction);

            Assert.IsFalse(result);
            Assert.AreEqual("None", bw.Index.ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void BwUnresolveableIndirect()
        {
            var eax = m.Reg32("eax", 0);
            var esi = m.Reg32("esi", 6);
            var Z = m.Frame.EnsureFlagGroup(Registers.eflags, 1, "Z", PrimitiveType.Bool);

            var xfer = new RtlCall(eax, 4, InstrClass.Transfer);
            m.Assign(eax, m.Mem32(esi));
            m.Assign(Z, m.Cond(m.And(eax, eax)));
            m.BranchIf(m.Test(ConditionCode.EQ, Z), "null_ptr");
            m.Label("do_call");

            var bw = new Backwalker<Block,Instruction>(host, xfer, expSimp);
            Assert.IsTrue(bw.CanBackwalk());
            Assert.AreEqual("eax", bw.Index.Name);
            bw.BackWalk(m.Block);
            Assert.AreEqual("None", bw.Index.ToString());
        }

        [Test]
        [Category(Categories.Regressions)]
        public void BwReg00018()
        {
            RunFileTestx86_32("Fragments/regressions/r00018.asm", "Scanning/BwReg00018.txt");
        }
	}
}
