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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Reko.UnitTests.Analysis
{
    /// <summary>
    /// These are SSA integration tests: they hit the filesystem a lot. 
    /// Therefore they aren't strictly speaking "unit" tests.
    /// </summary>
	[TestFixture]
    [Category(Categories.IntegrationTests)]
	public class SsaTests : AnalysisTestBase
	{
		private SsaState ssa;
        private Identifier r1;
        private Identifier r2;

        [SetUp]
        public void Setup()
        {
            this.r1 = new Identifier("r1", PrimitiveType.Word32, new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));
            this.r2 = new Identifier("r2", PrimitiveType.Word32, new RegisterStorage("r2", 2, 0, PrimitiveType.Word32));
        }

        private Identifier EnsureRegister16(ProcedureBuilder m, string name)
        {
            return m.Frame.EnsureRegister(new RegisterStorage(name, m.Frame.Identifiers.Count, 0, PrimitiveType.Word16));
        }

        private Identifier EnsureRegister32(ProcedureBuilder m, string name)
        {
            return m.Frame.EnsureRegister(new RegisterStorage(name, m.Frame.Identifiers.Count, 0, PrimitiveType.Word32));
        }

        protected override void RunTest(Program program, TextWriter writer)
		{
            var flow = new ProgramDataFlow();
            var dynamicLinker = new Mock<IDynamicLinker>();
            foreach (Procedure proc in program.Procedures.Values)
            {
                var sst = new SsaTransform(
                    program,
                    proc, 
                    new HashSet<Procedure>(),
                    dynamicLinker.Object,
                    flow);
                sst.Transform();
                sst.AddUsesToExitBlock();
                sst.RemoveDeadSsaIdentifiers();
                Debug.Print("SsaTest: {0}", new StackFrame(3).GetMethod().Name);
                ssa = sst.SsaState;
                ssa.Write(writer);
                proc.Write(false, true, writer);
                writer.WriteLine();
                ssa.Validate(s => Assert.Fail(s));
            }
		}

        private void RunUnitTest(ProcedureBuilder m, string outfile)
        {
            var flow = new ProgramDataFlow();
            var dynamicLinker = new Mock<IDynamicLinker>();

            var proc = m.Procedure;
            var platform = new FakePlatform(null, m.Architecture)
            {
                Test_CreateTrashedRegisters = () =>
                    new HashSet<RegisterStorage>()
                {
                    (RegisterStorage)r1.Storage,
                    (RegisterStorage)r2.Storage,
                }
            };
            var program = new Program()
            {
                Architecture = m.Architecture,
                Platform = platform,
            };
            var sst = new SsaTransform(
                program,
                proc, 
                new HashSet<Procedure>(),
                dynamicLinker.Object,
                flow);
            sst.Transform();
            ssa = sst.SsaState;
            using (var fut = new FileUnitTester(outfile))
            {
                ssa.Write(fut.TextWriter);
                proc.Write(false, fut.TextWriter);
                fut.AssertFilesEqual();
                ssa.Validate(s => Assert.Fail(s));
            }
        }

        private void Dump(CallGraph cg)
        {
            var sw = new StringWriter();
            cg.Write(sw);
            Debug.Print("{0}", sw.ToString());
        }

        [Test]
		public void SsaSimple()
		{
			RunFileTest_x86_real("Fragments/ssasimple.asm", "Analysis/SsaSimple.txt");
		}

		[Test]
		public void SsaConverge()
		{
			RunFileTest_x86_real("Fragments/3converge.asm", "Analysis/SsaConverge.txt");
		}

		[Test]
		public void SsaMemoryTest()
		{
			RunFileTest_x86_real("Fragments/memory_simple.asm", "Analysis/SsaMemoryTest.txt");			
		}

		[Test]
		public void SsaAddSubCarries()
		{
			RunFileTest_x86_real("Fragments/addsubcarries.asm", "Analysis/SsaAddSubCarries.txt");
		}

		[Test]
		public void SsaSwitch()
		{
			RunFileTest_x86_real("Fragments/switch.asm", "Analysis/SsaSwitch.txt");
		}
		
		[Test]
		public void SsaFactorial()
		{
			RunFileTest_x86_real("Fragments/factorial.asm", "Analysis/SsaFactorial.txt");
		}

		[Test]
		public void SsaFactorialReg()
		{
			RunFileTest_x86_real("Fragments/factorial_reg.asm", "Analysis/SsaFactorialReg.txt");
		}

		[Test]
		public void SsaForkedLoop()
		{
			RunFileTest_x86_real("Fragments/forkedloop.asm", "Analysis/SsaForkedLoop.txt");
		}

		[Test]
		public void SsaNestedRepeats()
		{
			RunFileTest_x86_real("Fragments/nested_repeats.asm", "Analysis/SsaNestedRepeats.txt");
		}

        [Test]
        public void SsaNegsNots()
        {
            RunFileTest_x86_real("Fragments/negsnots.asm", "Analysis/SsaNegsNots.txt");
        }

        [Test]
        public void SsaOutParamters()
        {
            var m = new ProcedureBuilder("foo");
            Identifier r4 = m.Register("r4");
            m.MStore(m.Word32(0x400), m.Fn("foo", m.Out(PrimitiveType.Ptr32, r4)));
            m.Return();

            RunFileTest(m, "Analysis/SsaOutParameters.txt");
        }

        [Test]
        public void SsaPushAndPop()
        {
            // Mirrors the pattern of stack accesses used by x86 compilers.
            var m = new ProcedureBuilder("SsaPushAndPop");
            var esp = EnsureRegister32(m, "esp");
            var ebp = EnsureRegister32(m, "ebp");
            var eax = EnsureRegister32(m, "eax");
            m.Assign(esp, m.ISub(esp, 4));
            m.MStore(esp, ebp);
            m.Assign(ebp, esp);
            m.Assign(eax, m.Mem32(m.IAdd(ebp, 8)));  // dwArg04
            m.Assign(ebp, m.Mem32(esp));
            m.Assign(esp, m.IAdd(esp,4));
            m.Return();

            RunUnitTest(m, "Analysis/SsaPushAndPop.txt");
        }

        [Test]
        public void SsaStackReference_Load()
        {
            var m = new ProcedureBuilder("SsaStackReference");
            var wRef = m.Frame.EnsureStackArgument(4, PrimitiveType.Word16);
            var ax = EnsureRegister16(m, "ax");
            m.Assign(ax, wRef);
            m.Return();

            RunUnitTest(m, "Analysis/SsaStackReference.txt");
        }

        [Test]
        public void SsaCallIndirect()
        {
            var m = new ProcedureBuilder("SsaCallIndirect");
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);
            m.Assign(r1, m.Mem32(r2));
            m.Call(r1, 4);
            m.Return();

            RunUnitTest(m, "Analysis/SsaCallIndirect.txt");
        }

        [Test]
        public void SsaSwitchWithSharedBranches()
        {
            var m = new ProcedureBuilder("SsaSwitchWithSharedBranches");

            var sp = m.Frame.EnsureRegister(m.Architecture.StackRegister);
            var r1 = m.Reg32("r1", 1);
            var r2 = m.Reg32("r2", 2);
            var foo = new ExternalProcedure("foo", new FunctionType(
                new Identifier("", VoidType.Instance, null),
                new Identifier("arg1", PrimitiveType.Int32, new StackArgumentStorage(4, PrimitiveType.Int32))));
            m.Assign(sp, m.Frame.FramePointer);
            m.Assign(r1, m.Mem32(m.IAdd(sp, 4)));
            m.BranchIf(m.Ugt(r1, m.Word32(0x5)), "m4_default");
            m.Label("m1");
            m.Switch(r1,
                "m2", "m2", "m3", "m3", "m2", "m3");
            m.Label("m2");
            m.Assign(sp, m.ISub(sp, 4));
            m.MStore(sp, m.Word32(0x42));
            m.Call(foo, 4);
            m.Assign(sp, m.IAdd(sp, 4));
            // fall through
            m.Label("m3");
            m.Assign(sp, m.ISub(sp, 4));
            m.MStore(sp, m.Word32(42));
            m.Call(foo, 4);
            m.Assign(sp, m.IAdd(sp, 4));
            // fall through
            m.Label("m4_default");
            m.Assign(sp, m.ISub(sp, 4));
            m.MStore(sp, m.Word32(0));
            m.Call(foo, 4);
            m.Assign(sp, m.IAdd(sp, 4));

            m.Return();

            RunUnitTest(m, "Analysis/SsaSwitchWithSharedBranches.txt");
        }

        [Test]
        public void SsaAliasFlags()
        {
            var sExp =
@"esi:esi
    def:  def esi
    uses: SZ_2 = cond(esi & esi)
          SZ_2 = cond(esi & esi)
SZ_2: orig: SZ
    def:  SZ_2 = cond(esi & esi)
    uses: al_4 = Test(ULE,SZ_2)
          S_5 = SLICE(SZ_2, bool, 0) (alias)
          Z_6 = SLICE(SZ_2, bool, 1) (alias)
C_3: orig: C
    def:  C_3 = false
    uses: use C_3
al_4: orig: al
    def:  al_4 = Test(ULE,SZ_2)
    uses: use al_4
S_5: orig: S
    def:  S_5 = SLICE(SZ_2, bool, 0) (alias)
    uses: use S_5
Z_6: orig: Z
    def:  Z_6 = SLICE(SZ_2, bool, 1) (alias)
    uses: use Z_6
// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	def esi
	// succ:  l1
l1:
	SZ_2 = cond(esi & esi)
	S_5 = SLICE(SZ_2, bool, 0) (alias)
	Z_6 = SLICE(SZ_2, bool, 1) (alias)
	C_3 = false
	al_4 = Test(ULE,SZ_2)
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
	use al_4
	use C_3
	use S_5
	use Z_6

";
            RunStringTest(sExp, m =>
            {
                var eflags = new RegisterStorage("eflags", 9, 0, PrimitiveType.Word32);
                var sz = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("SZ"));
                var cz = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("SZ"));
                var c = m.Frame.EnsureFlagGroup(m.Architecture.GetFlagGroup("C"));
                var al = m.Reg8("al", 0);
                var esi = m.Reg32("esi", 6);
                m.Assign(sz, m.Cond(m.And(esi, esi)));
                m.Assign(c, Constant.False());
                m.Assign(al, m.Test(ConditionCode.ULE, cz));
                m.Return();
            });
        }

        [Test]
        [Category(Categories.IntegrationTests)]
        public void SsaPreservedAlias()
        {
            RunFileTest_x86_real("Fragments/multiple/preserved_alias.asm", "Analysis/SsaPreservedAlias.txt");
        }
    }
}
