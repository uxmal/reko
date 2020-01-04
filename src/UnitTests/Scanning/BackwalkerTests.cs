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

namespace Reko.UnitTests.Scanning
{
    /// <summary>
    /// These unit tests should be architecture independent.
    /// </summary>
    [TestFixture]
    public class BackwalkerTests
    {
        ProcedureBuilder m;
        private IProcessorArchitecture arch;
        private ProcessorState state;
        private ExpressionSimplifier expSimp;
        private IBackWalkHost<Block, Instruction> host;
        private FakeDecompilerEventListener listener;

        private class BackwalkerHost : IBackWalkHost<Block, Instruction>
        {
            private IProcessorArchitecture arch;

            #region IBackWalkHost Members

            public BackwalkerHost(IProcessorArchitecture arch)
            {
                this.arch = arch;
            }

            public IProcessorArchitecture Architecture => arch;

            public Program Program => null;

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
                return block.Pred.ToList();
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
            arch = new FakeArchitecture();
            m = new ProcedureBuilder();
            state = arch.CreateProcessorState();
            listener = new FakeDecompilerEventListener();
            var segmentMap = new SegmentMap(Address.Ptr32(0));
            expSimp = new ExpressionSimplifier(segmentMap, state, listener);
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
            scanner.EnqueueImageSymbol(ImageSymbol.Procedure(program.Architecture, program.ImageMap.BaseAddress), true);
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

        [Test(Description = "Part of regression of issue #121")]
        [Category(Categories.Regressions)]
        [Category(Categories.UnitTests)]
        public void BwReg_00121()
        {
            var d0 = m.Reg32("d0", 0);
            var d3 = m.Reg32("d3", 3);
            var a5 = m.Reg32("a5", 5);
            var v38 = m.Temp(PrimitiveType.Word16, "v38");
            var v39 = m.Temp(PrimitiveType.Byte, "v39");
            var v40 = m.Temp(PrimitiveType.Word16, "v40");
            var VZN = m.Flags("VZN");
            var ZN = m.Flags("ZN");
            var C = m.Flags("C");
            var V = m.Flags("V");
            var CVZN = m.Flags("CVZN");
            var CVZNX = m.Flags("CVZNX");
            m.Assign(d0, d3);
            m.Assign(CVZN, m.Cond(d0));
            m.Assign(v38, m.And(m.Cast(PrimitiveType.Word16, d0), 0xF0));
            m.Assign(d0, m.Dpb(d0, v38, 0));
            m.Assign(ZN, m.Cond(v38));
            m.Assign(C, false);
            m.Assign(V, false);
            m.Assign(v39, m.Shl(m.Cast(PrimitiveType.Byte, d0), 2));
            m.Assign(d0, m.Dpb(d0, v39, 0));
            m.Assign(CVZNX, m.Cond(v39));
            m.Assign(v40, m.ISub(m.Cast(PrimitiveType.Word16, d0), 44));
            m.Assign(CVZN, m.Cond(v40));
            m.BranchIf(m.Test(ConditionCode.GT, VZN), "lDefault");
            m.Assign(a5, m.Mem32(m.IAdd(Address.Ptr32(0x0000C046), d0)));
            var xfer = new RtlCall(a5, 4, InstrClass.Transfer);

            var bw = new Backwalker<Block, Instruction>(host, xfer, expSimp);
            Assert.IsTrue(bw.CanBackwalk());
            Assert.AreEqual("a5", bw.Index.Name);
            bw.BackWalk(m.Block);
            Assert.AreEqual("v40", bw.IndexExpression.ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void BwLoadDirect()
        {
            var r1 = m.Reg32("r1", 1);
            m.Assign(r1, m.Mem32(Constant.Word32(0x00123400)));
            var xfer = new RtlGoto(m.Mem32(m.IAdd(Constant.Word32(0x00113300), m.IMul(r1, 8))), InstrClass.Transfer);

            var bw = new Backwalker<Block, Instruction>(host, xfer, expSimp);
            Assert.IsTrue(bw.CanBackwalk());
            var ops = bw.BackWalk(m.Block);
            Assert.IsNull(ops, "Should have reported missing guard.");
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void BwZeroExtend()
        {
            var rax = m.Reg64("rax", 0);
            var eax = m.Reg32("eax", 0);
            var al = m.Reg8("al", 0);
            var ecx = m.Reg32("ecx", 1);
            var CZ = m.Flags("CZ");

            m.Assign(eax, m.Mem8(rax));
            m.Assign(CZ, m.Cond(m.ISub(al, 0x78)));
            m.BranchIf(m.Test(ConditionCode.UGT, CZ), "ldefault");
            m.Assign(ecx, m.Cast(PrimitiveType.Word32, al));
            var xfer = new RtlGoto(m.Mem32(m.IAdd(Constant.Word32(0x00411F40), m.IMul(ecx, 8))), InstrClass.Transfer);

            var bw = new Backwalker<Block, Instruction>(host, xfer, expSimp);
            Assert.IsTrue(bw.CanBackwalk());
            var ops = bw.BackWalk(m.Block);
            Assert.AreEqual(3, ops.Count);
            Assert.AreEqual("cmp 120", ops[0].ToString());
            Assert.AreEqual("branch UGT", ops[1].ToString());
            Assert.AreEqual("* 8", ops[2].ToString());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void BwInvertedCondition()
        {
            var ebx = m.Reg32("ebx", 3);
            var eax = m.Reg32("eax", 0);

            var CZ = m.Flags("CZ");
            m.Assign(CZ, m.Cond(m.ISub(ebx, 0x30)));
            m.BranchIf(m.Test(ConditionCode.ULE, CZ), "do_switch");
            m.Goto("default_case");

            m.Label("do_switch");
            m.Assign(eax, 0);
            var block = m.CurrentBlock;
            var xfer = new RtlGoto(m.Mem32(m.IAdd(Constant.Word32(0x00123400), m.IMul(ebx, 4))), InstrClass.Transfer);

            m.Label("default_case");
            m.Return();

            var bw = new Backwalker<Block, Instruction>(host, xfer, expSimp);
            Assert.IsTrue(bw.CanBackwalk());
            var ops = bw.BackWalk(block);
            Assert.AreEqual(3, ops.Count);
            Assert.AreEqual("cmp 48", ops[0].ToString());
            Assert.AreEqual("branch UGT", ops[1].ToString());
            Assert.AreEqual("* 4", ops[2].ToString());
        }
    }
}
