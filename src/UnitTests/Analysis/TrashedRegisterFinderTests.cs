#region License
/* 
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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Analysis;
using Reko.Evaluation;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    public class TrashedRegisterFinderTests : AnalysisTestBase
    {
        private ProcedureBuilder m;
        private Program program;
        private TrashedRegisterFinder trf;
        private Procedure exit;
        private ProgramDataFlow flow;
        private IntelArchitecture arch;
        private ProgramBuilder p;
        private Frame frame;
        private SymbolicEvaluationContext ctx;
        private BlockFlow blockflow;

        [SetUp]
        public void Setup()
        {
            arch = new X86ArchitectureFlat32("x86-protected-32");
            m = new ProcedureBuilder(arch);
            program = new Program();
            program.Architecture = arch;
            program.SegmentMap = new SegmentMap(Address.Ptr32(0));
            exit = new Procedure(arch, "exit", new Frame(PrimitiveType.Word32));
            flow = new ProgramDataFlow();
            p = new ProgramBuilder();
            p.Program.Architecture = this.arch;
        }

        private BlockFlow CreateBlockFlow(Block block, Frame frame)
        {
            var bflow = new BlockFlow(
                block,
                new HashSet<RegisterStorage>(),
                new SymbolicEvaluationContext(
                    program.Architecture,
                    frame));
            flow[block] = bflow;
            return bflow;
        }

        private TrashedRegisterFinder CreateTrashedRegisterFinder()
        {
            return new TrashedRegisterFinder(program, program.Procedures.Values, this.flow, new FakeDecompilerEventListener());
        }

        private TrashedRegisterFinder CreateTrashedRegisterFinder(Program prog)
        {
            return new TrashedRegisterFinder(prog, prog.Procedures.Values, this.flow, new FakeDecompilerEventListener());
        }

        private string DumpProcedureSummaries()
        {
            var sw = new StringWriter();
            return DumpProcedureSummaries(sw).ToString();
        }

        private TextWriter DumpProcedureSummaries(TextWriter writer)
        {
            foreach (Procedure proc in program.Procedures.Values)
            {
                DataFlow.EmitRegisters(program.Architecture, proc.Name, flow[proc].grfTrashed, flow[proc].TrashedRegisters, writer);
                writer.WriteLine();
                if (flow[proc].ConstantRegisters.Count > 0)
                {
                    DataFlow.EmitRegisterValues("const", flow[proc].ConstantRegisters, writer);
                    writer.WriteLine();
                }
                foreach (var block in proc.ControlGraph.Blocks.OrderBy(b => b, new Procedure.BlockComparer()))
                {
                    DataFlow.EmitRegisterValues("    " + block.Name, flow[block].SymbolicIn.RegisterState, writer);
                    writer.WriteLine();
                }
                writer.WriteLine();
            }
            return writer;
        }

        private void RunTest(ProgramBuilder p, string sExp)
        {
            program = p.BuildProgram(arch);

            flow = new ProgramDataFlow(program);
            trf = CreateTrashedRegisterFinder();
            trf.Compute();

            var summary = DumpProcedureSummaries().Trim();
            if (sExp == summary)
                return;
            try
            {
                Assert.AreEqual(sExp, summary);
            }
            catch
            {
                Console.WriteLine(summary);
                throw;
            }
        }

        protected override void RunTest(Program prog, TextWriter writer)
        {
            this.program = prog;
            flow = new ProgramDataFlow(prog);
            trf = CreateTrashedRegisterFinder();
            trf.Compute();
            DumpProcedureSummaries(writer);
        }

        private string DumpValues()
        {
            var l = new SortedList<string, string>();
            foreach (var item in trf.RegisterSymbolicValues)
            {
                l.Add(item.Key.ToString(), item.Value.ToString());
            }
            foreach (var item in trf.StackSymbolicValues)
            {
                l.Add(
                    string.Format("Stack {0}{1:X}", item.Key >= 0 ? "+" : "-", Math.Abs(item.Key)),
                    item.Value.ToString());
            }
            var sb = new StringBuilder();
            string sep = "";
            foreach (KeyValuePair<string, string> de in l)
            {
                sb.Append(sep);
                sb.AppendFormat("({0}:{1})", de.Key, de.Value);
                sep = ", ";
            }
            return sb.ToString();
        }

        private string TrashToString(Storage o)
        {
            RegisterStorage rst = o as RegisterStorage;
            if (rst != null)
                return rst.Name;
            return o.ToString();
        }

        [Test]
        public void TrashRegister()
        {
            var r1 = m.Register(1);
            var stm = m.Assign(r1, m.Int32(0));

            trf = CreateTrashedRegisterFinder();
            CreateBlockFlow(m.Block, m.Frame);
            trf.StartProcessingBlock(m.Block);

            stm.Accept(trf);
            Debug.WriteLine(trf.RegisterSymbolicValues[(RegisterStorage) r1.Storage].ToString());
            Assert.IsTrue(trf.IsTrashed(r1.Storage), "r1 should have been marked as trashed.");
        }

        [Test]
        public void TrashFlag()
        {
            var flags = arch.GetFlagGroup(0x7).FlagRegister;
            var scz = m.Frame.EnsureFlagGroup(flags, 0x7, arch.GrfToString(0x7), PrimitiveType.Byte);
            var stm = m.Assign(scz, m.Int32(3));

            trf = CreateTrashedRegisterFinder();
            CreateBlockFlow(m.Block, m.Frame);
            trf.StartProcessingBlock(m.Block);
            stm.Accept(trf);
            Assert.AreEqual(0x7, trf.TrashedFlags);
        }

        [Test]
        public void TrashCompoundRegister()
        {
            Identifier ax = m.Frame.EnsureRegister(Registers.ax);
            var stm = m.Assign(ax, 1);

            trf = CreateTrashedRegisterFinder();
            CreateBlockFlow(m.Block, m.Frame);
            trf.StartProcessingBlock(m.Block);

            stm.Accept(trf);
            Assert.AreEqual("(ax:0x0001)", DumpValues());
        }

        [Test]
        public void TrfCopy()
        {
            Identifier r1 = m.Register(1);
            Identifier r2 = m.Register(2);
            var ass = m.Assign(r2, r1);

            trf = CreateTrashedRegisterFinder();
            CreateBlockFlow(m.Block, m.Frame);
            trf.StartProcessingBlock(m.Block);

            ass.Accept(trf);
            Assert.AreEqual(r1, trf.RegisterSymbolicValues[(RegisterStorage) r2.Storage], "r2 should now be equal to r1");
        }

        [Test]
        public void TrfCopyBack()
        {
            var esp = m.Frame.EnsureRegister(Registers.esp);
            var r2 = m.Register(2);
            var stm1 = m.MStore(m.ISub(esp, 0x10), r2);
            var stm2 = m.Assign(r2, m.Int32(0));
            var stm3 = m.Assign(r2, m.Mem32(m.ISub(esp, 0x10)));

            trf = CreateTrashedRegisterFinder();
            var flow = CreateBlockFlow(m.Block, m.Frame);
            flow.SymbolicIn.SetValue((Identifier)esp, (Expression)this.m.Frame.FramePointer);
            trf.StartProcessingBlock(m.Block);

            stm1.Instruction.Accept(trf);
            stm2.Accept(trf);
            stm3.Accept(trf);

            Assert.AreEqual(r2, trf.RegisterSymbolicValues[(RegisterStorage) r2.Storage]);
        }

        [Test]
        public void TrfOutParameters()
        {
            var r2 = m.Register(2);
            var stm = m.SideEffect(m.Fn("Hello", m.AddrOf(r2)));

            trf = CreateTrashedRegisterFinder();
            trf.EnsureEvaluationContext(CreateBlockFlow(m.Block, m.Frame));

            stm.Instruction.Accept(trf);
            Assert.AreEqual("<invalid>", trf.RegisterSymbolicValues[(RegisterStorage) r2.Storage].ToString());
        }

        [Test]
        public void TrfCallInstruction()
        {
            var callee = new Procedure(arch, "Callee", program.Architecture.CreateFrame());
            var stm = m.Call(callee, 4);
            var pf = new ProcedureFlow(callee, program.Architecture);
            pf.TrashedRegisters.Add(Registers.ebx);
            flow[callee] = pf;

            trf = CreateTrashedRegisterFinder();
            CreateBlockFlow(m.Block, m.Frame);
            trf.StartProcessingBlock(m.Block);

            stm.Instruction.Accept(trf);
            Assert.AreEqual("(ebx:<invalid>)", DumpValues());
        }

        [Test]
        public void TrfPropagateToSuccessorBlocks()
        {
            Procedure proc = new Procedure(program.Architecture, "test", program.Architecture.CreateFrame());
            var frame = proc.Frame;
            Identifier ecx = m.Register(1);
            Identifier edx = m.Register(2);
            Identifier ebx = m.Register(3);
            Block b = proc.AddBlock("b");
            Block t = proc.AddBlock("t");
            Block e = proc.AddBlock("e");
            proc.ControlGraph.AddEdge(b, e);
            proc.ControlGraph.AddEdge(b, t);
            flow[t] = new BlockFlow(t, null, new SymbolicEvaluationContext(program.Architecture, frame));
            flow[e] = new BlockFlow(e, null, new SymbolicEvaluationContext(program.Architecture, frame));

            trf = CreateTrashedRegisterFinder(program);
            CreateBlockFlow(b, frame);
            trf.StartProcessingBlock(b);
            trf.RegisterSymbolicValues[(RegisterStorage) ecx.Storage] = Constant.Invalid;
            trf.RegisterSymbolicValues[(RegisterStorage) edx.Storage] = ebx;

            flow[e].SymbolicIn.RegisterState[(RegisterStorage) ecx.Storage] = edx;
            flow[e].SymbolicIn.RegisterState[(RegisterStorage) edx.Storage] = ebx;

            flow[t].SymbolicIn.RegisterState[(RegisterStorage) ecx.Storage] = Constant.Invalid;
            flow[t].SymbolicIn.RegisterState[(RegisterStorage) edx.Storage] = edx;

            trf.PropagateToSuccessorBlock(e);
            trf.PropagateToSuccessorBlock(t);
            Assert.AreEqual(2, proc.ControlGraph.Successors(b).Count);
            Assert.AreEqual("<invalid>", flow[e].SymbolicIn.RegisterState[(RegisterStorage) ecx.Storage].ToString(), "trash & r2 => trash");
            Assert.AreEqual("ebx", flow[e].SymbolicIn.RegisterState[(RegisterStorage) edx.Storage].ToString(), "ebx & ebx => ebx");
            Assert.AreEqual("<invalid>", flow[e].SymbolicIn.RegisterState[(RegisterStorage) ecx.Storage].ToString(), "trash & r2 => trash");
            Assert.AreEqual("ebx", flow[e].SymbolicIn.RegisterState[(RegisterStorage) edx.Storage].ToString(), "ebx & ebx => ebx");

            Assert.AreEqual("<invalid>", flow[t].SymbolicIn.RegisterState[(RegisterStorage) ecx.Storage].ToString(), "trash & trash => trash");
            Assert.AreEqual("<invalid>", flow[t].SymbolicIn.RegisterState[(RegisterStorage) edx.Storage].ToString(), "r3 & r2 => trash");
        }

        [Test]
        public void TrfPropagateStackValuesToSuccessor()
        {
            m.Label("Start");
            Identifier ecx = m.Register(1);
            trf = CreateTrashedRegisterFinder(program);
            CreateBlockFlow(m.Block, m.Frame);
            trf.StartProcessingBlock(m.Block);

            trf.StackSymbolicValues[-4] = ecx;
            trf.StackSymbolicValues[-8] = ecx;
            trf.StackSymbolicValues[-12] = ecx;
            trf.StackSymbolicValues[-16] = m.Word32(0x1234);
            trf.StackSymbolicValues[-20] = m.Word32(0x5678);
            trf.StackSymbolicValues[-24] = m.Word32(0x9ABC);

            var succ = new Block(m.Procedure, "succ");
            var sf = CreateBlockFlow(succ, m.Frame);
            flow[succ] = sf;
            sf.SymbolicIn.StackState[-8] = ecx;
            sf.SymbolicIn.StackState[-12] = Constant.Word32(1231);
            sf.SymbolicIn.StackState[-20] = Constant.Word32(0x5678);
            sf.SymbolicIn.StackState[-24] = Constant.Word32(0xCCCC);

            trf.PropagateToSuccessorBlock(succ);

            Assert.AreEqual("ecx", sf.SymbolicIn.StackState[-4].ToString(), "Didn't have a value before");
            Assert.AreEqual("ecx", sf.SymbolicIn.StackState[-8].ToString(), "Same value as before");
            Assert.AreEqual("<invalid>", sf.SymbolicIn.StackState[-12].ToString());
            Assert.AreEqual("0x00001234", sf.SymbolicIn.StackState[-16].ToString());
            Assert.AreEqual("0x00005678", sf.SymbolicIn.StackState[-20].ToString());
            Assert.AreEqual("<invalid>", sf.SymbolicIn.StackState[-24].ToString());
        }

        [Test]
        public void TrfPropagateToProcedureSummary()
        {
            Procedure proc = new Procedure(program.Architecture, "proc", program.Architecture.CreateFrame());
            program.CallGraph.AddProcedure(proc);
            Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
            Identifier ebx = proc.Frame.EnsureRegister(Registers.ebx);
            Identifier ecx = proc.Frame.EnsureRegister(Registers.ecx);
            Identifier esi = proc.Frame.EnsureRegister(Registers.esi);
            flow[proc] = new ProcedureFlow(proc, program.Architecture);

            trf = CreateTrashedRegisterFinder();
            CreateBlockFlow(proc.ExitBlock, proc.Frame);
            trf.StartProcessingBlock(proc.ExitBlock);

            trf.RegisterSymbolicValues[(RegisterStorage) eax.Storage] = eax;			// preserved
            trf.RegisterSymbolicValues[(RegisterStorage) ebx.Storage] = ecx;			// trashed
            trf.RegisterSymbolicValues[(RegisterStorage) esi.Storage] = Constant.Invalid;				// trashed
            trf.PropagateToProcedureSummary(proc);
            ProcedureFlow pf = flow[proc];
            Assert.AreEqual(" ebx esi", pf.EmitRegisters(program.Architecture, "", pf.TrashedRegisters));
            Assert.AreEqual(" eax", pf.EmitRegisters(program.Architecture, "", pf.PreservedRegisters));
        }

        [Test]
        public void TrfPropagateFlagsToProcedureSummary()
        {
            var proc = new Procedure(program.Architecture, "proc", program.Architecture.CreateFrame());
            program.CallGraph.AddProcedure(proc);
            var flags = program.Architecture.GetFlagGroup("SZ");
            var sz = m.Frame.EnsureFlagGroup(flags.FlagRegister, flags.FlagGroupBits, flags.Name, flags.DataType);
            var stm = m.Assign(sz, m.Int32(3));
            flow[proc] = new ProcedureFlow(proc, program.Architecture);
            trf = CreateTrashedRegisterFinder(program);
            CreateBlockFlow(m.Block, m.Frame);
            trf.StartProcessingBlock(m.Block);
            stm.Accept(trf);
            trf.PropagateToProcedureSummary(proc);
            Assert.AreEqual(" SZ", flow[proc].EmitFlagGroup(program.Architecture, "", flow[proc].grfTrashed));
        }

        [Test]
        public void TrfPreserveEbp()
        {
            Identifier esp = m.Frame.EnsureRegister(Registers.esp);
            Identifier ebp = m.Frame.EnsureRegister(Registers.ebp);
            m.MStore(esp, ebp);
            m.Assign(ebp, m.Mem32(m.Int32(0x12345678)));
            m.Assign(ebp, m.Mem32(esp));
            m.Return();

            Procedure proc = m.Procedure;
            program.Procedures.Add(Address.Ptr32(0x10000), proc);
            program.CallGraph.AddProcedure(proc);
            flow = new ProgramDataFlow(program);

            trf = CreateTrashedRegisterFinder(program);
            trf.Compute();
            ProcedureFlow pf = flow[proc];
            Assert.AreEqual(" ebp esp", pf.EmitRegisters(program.Architecture, "", pf.PreservedRegisters), "ebp should have been preserved");
        }

        [Test]
        public void TrfProcessBlock()
        {
            Identifier eax = m.Procedure.Frame.EnsureRegister(Registers.eax);
            Identifier esp = m.Procedure.Frame.EnsureRegister(Registers.esp);
            m.MStore(m.ISub(esp, 4), eax);
            m.Assign(eax, m.Int32(3));
            m.Assign(eax, m.Mem32(m.ISub(esp, 4)));

            flow[m.Block] = CreateBlockFlow(m.Block, m.Frame);
            this.flow[this.m.Block].SymbolicIn.SetValue(esp, (Expression)this.m.Frame.FramePointer);
            trf = CreateTrashedRegisterFinder(program);
            trf.ProcessBlock(m.Block);
            Assert.AreEqual("(eax:eax), (esp:fp), (Stack -4:eax)", DumpValues());
        }

        [Test]
        public void TrfTerminatingProcedure()
        {
            var eax = m.Procedure.Frame.EnsureRegister(Registers.eax);
            m.Assign(eax, m.Word32(0x40));
            m.Call(exit, 4);

            flow[m.Block] = CreateBlockFlow(m.Block, m.Frame);
            flow[exit] = new ProcedureFlow(exit, program.Architecture);
            flow[exit].TerminatesProcess = true;
            trf = CreateTrashedRegisterFinder(program);
            trf.ProcessBlock(m.Block);
            Assert.AreEqual("", DumpValues());
        }

        [Test]
        public void TrfTwoProcedures()
        {
            p.Add("main", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                var tmp = m.Local32("tmp");
                m.Assign(tmp, eax);
                m.Call("TrashEaxEbx", 4);
                m.Assign(eax, tmp);     // eax is preserved!
                m.Return();
            });

            p.Add("TrashEaxEbx", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                var ebx = m.Frame.EnsureRegister(Registers.ebx);
                m.Assign(ebx, m.Word32(0x1231313));
                m.Assign(eax, m.Mem32(ebx));
                m.Return();
            });

            RunTest(p,
@"main bh bl bx ebx rbx
const ebx:0x01231313
    main_entry esp:fp
    l1 esp:fp
    main_exit eax:eax ebx:0x01231313 esp:fp

TrashEaxEbx ah al ax bh bl bx eax ebx rax rbx
const eax:<invalid> ebx:0x01231313
    TrashEaxEbx_entry esp:fp
    l1 esp:fp
    TrashEaxEbx_exit eax:<invalid> ebx:0x01231313 esp:fp");
        }


        [Test]
        public void TrfPreservedValues()
        {
            arch = new X86ArchitectureReal("x86-real-16");
            p.Program.Architecture = arch;
            p.Add("main", m =>
            {
                var sp = m.Frame.EnsureRegister(Registers.sp);
                var ss = m.Frame.EnsureRegister(Registers.ss);
                var ax = m.Frame.EnsureRegister(Registers.ax);
                m.Assign(sp, m.ISub(sp, 2));
                m.SStore(ss, sp, ax);
                m.Assign(ax, 1);
                m.Assign(ax, m.SegMem16(ss, sp));
                m.Assign(sp, m.IAdd(sp, 2));
                m.Return();
            });

            RunTest(p,
@"main
    main_entry sp:fp
    l1 sp:fp
    main_exit ax:ax sp:fp");
        }

        [Test]
        public void TrashFlagProcedure()
        {
            p.Add("main", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                m.Assign(eax, m.IAdd(eax, 4));
                m.Assign(m.Flags("SZCO"), m.Cond(eax));
                m.Return();
            });

            var sExp =
@"main SCZO ah al ax eax rax
const eax:<invalid>
    main_entry esp:fp
    l1 esp:fp
    main_exit eax:eax + 0x00000004 esp:fp";
            RunTest(p, sExp);
        }

        [Test]
        public void RegistersPreservedOnStack()
        {
            p.Add("main", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                var esp = m.Frame.EnsureRegister(Registers.esp);
                m.Assign(esp, m.ISub(esp, 4));
                m.MStore(esp, eax);
                m.Assign(eax, 1);
                m.Assign(m.Flags("SCZO"), m.Cond(eax));
                m.MStore(m.Word32(0x12340000), eax);
                m.Assign(eax, m.Mem32(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Return();
            });

            var sExp =
@"main SCZO
    main_entry esp:fp
    l1 esp:fp
    main_exit eax:eax esp:fp";
            RunTest(p, sExp);
        }

        [Test]
        public void TrashInLoop()
        {
            p.Add("main", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                var ebx = m.Frame.EnsureRegister(Registers.ebx);
                m.Assign(eax, 1);
                m.Label("Lupe");
                m.MStore(m.IAdd(ebx, eax), m.Word16(0));
                m.Assign(eax, m.IAdd(eax, 2));
                m.BranchIf(m.Le(eax, 10), "Lupe");
                m.Return();
            });

            var sExp =
@"main ah al ax eax rax
const eax:<invalid>
    main_entry esp:fp
    l1 esp:fp
    l2 eax:<invalid> esp:fp
    Lupe eax:<invalid> esp:fp
    main_exit eax:<invalid> esp:fp";
            RunTest(p, sExp);
        }

        [Test]
        [Ignore("scanning-development")]
        public void TrfConstNonConst()
        {
            // Constant in one branch, not constant in other.
            p.Add("main", m =>
            {
                var ax = m.Frame.EnsureRegister(Registers.ax);
                var cl = m.Frame.EnsureRegister(Registers.cl);
                var cx = m.Frame.EnsureRegister(Registers.cx);
                m.BranchIf(m.Eq0(ax), "zero");
                m.Assign(cl, 0);
                m.Return();

                m.Label("zero");
                m.Assign(cx, m.Mem16(ax));
                m.Return();
            });
            var sExp =
@"main ax cx al cl ah ch
const ax:0x0000 cx:<invalid>
    main_entry:esp:fp
    
";
            RunTest(p, sExp);
        }

        [Test]
        public void TrfFactorial()
        {
            RunFileTest("Fragments/factorial.asm", "Analysis/TrfFactorial.txt");
        }

        [Test]
        public void TrfReg00005()
        {
            RunFileTest("Fragments/regressions/r00005.asm", "Analysis/TrfReg00005.txt");
        }

        [Test]
        public void TrfReg00007()
        {
            RunFileTest("Fragments/regressions/r00007.asm", "Analysis/TrfReg00007.txt");
        }

        [Test]
        public void TrfProcIsolation()
        {
            RunFileTest("Fragments/multiple/procisolation.asm", "Analysis/TrfProcIsolation.txt");
        }

        [Test]
        [Ignore("scanning-development")]
        public void TrfMergeSubregisterRegister()
        {
            Given_Contexts();
            var cl = frame.EnsureRegister(Registers.cl);
            var cx = frame.EnsureRegister(Registers.cx);
            trf.RegisterSymbolicValues[cl.Storage] = Constant.Zero(cl.DataType);
            blockflow.SymbolicIn.SetValue(cx, Constant.Invalid);
            trf.MergeDataFlow(blockflow);

            var sw = new StringWriter();
            DataFlow.EmitRegisterValues("", blockflow.SymbolicIn.RegisterState, sw);
            Assert.AreEqual("cx:<invalid>", sw.ToString());
        }

        private void Given_Contexts()
        {
            frame = new Frame(PrimitiveType.Ptr32);
            ctx = new SymbolicEvaluationContext(arch, frame);
            blockflow = new BlockFlow(null, new HashSet<RegisterStorage>(), ctx);
            trf.EnsureEvaluationContext(blockflow);
        }

        /// <summary>
        /// This crazy code is generated when Reko starts disassembling large 
        /// areas of data as M68k code. At least we won't run out of memory
        /// trying to evaluate the data.
        /// </summary>
        [Test]
        public void TrashRepeatedDpbs()
        {
            p.Add("main", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                var v1 = m.Frame.CreateTemporary("v1", PrimitiveType.Byte);
                var v2 = m.Frame.CreateTemporary("v2", PrimitiveType.Byte);
                var v3 = m.Frame.CreateTemporary("v3", PrimitiveType.Byte);
                m.Assign(v1, m.Or(m.Cast(v1.DataType, eax), 0));
                m.Assign(eax, m.Dpb(eax, v1, 0));
                m.Assign(v2, m.Or(m.Cast(v1.DataType, eax), 0));
                m.Assign(eax, m.Dpb(eax, v2, 0));
                m.Assign(v3, m.Or(m.Cast(v1.DataType, eax), 0));
                m.Assign(eax, m.Dpb(eax, v3, 0));
                m.Return();
            });

            var sExp =
@"main
    main_entry esp:fp
    l1 esp:fp
    main_exit eax:eax esp:fp";
            RunTest(p, sExp);
        }
    }
}
