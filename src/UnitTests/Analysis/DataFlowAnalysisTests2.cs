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

using NUnit.Framework;
using Reko.Analysis;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using Reko.UnitTests.TestCode;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class DataFlowAnalysisTests2
    {
        private ProgramBuilder pb;

        private void GivenProgram(ProgramBuilder pb)
        {
        }

        private void AssertProgram(string sExp, Program program)
        {
            var sw = new StringWriter();
            program.Procedures.Values.First().Write(false, sw);
            var sActual = sw.ToString();
            if (sExp != sActual) 
                Debug.WriteLine(sActual);
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        [Ignore("")]
        public void Dfa2_Simple()
        {
            var pb = new ProgramBuilder(new FakeArchitecture());
            pb.Add("test", m=>
                {
                    var r1 = m.Reg32("r1", 1);
                    var r2 = m.Reg32("r2", 2);
                    m.Assign(r1, m.LoadDw(m.Word32(0x010000)));
                    m.Assign(r2, m.LoadDw(m.Word32(0x010004)));
                    m.Store(m.Word32(0x010008), m.IAdd(r1, r2));
                    m.Return();
                });
            var dfa = new DataFlowAnalysis(pb.BuildProgram(), null, new FakeDecompilerEventListener());
            dfa.AnalyzeProgram2();
            var sExp = @"// test
// Return size: 0
void test()
test_entry:
	// succ:  l1
l1:
	Mem3[0x00010008:word32] = Mem0[0x00010000:word32] + Mem0[0x00010004:word32]
	return
	// succ:  test_exit
test_exit:
";
            AssertProgram(sExp, pb.Program);
        }

        [Test]
        [Ignore("")]
        public void Dfa2_StackArgs()
        {
            var pb = new ProgramBuilder(new FakeArchitecture());
            pb.Add("test", m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Assign(sp, m.Frame.FramePointer);
                m.Assign(r1, m.LoadDw(m.IAdd(sp, 4)));
                m.Assign(r2, m.LoadDw(m.IAdd(sp, 8)));
                m.Assign(r1, m.IAdd(r1, r2));
                m.Store(m.Word32(0x010008), r1);
                m.Return();
            });
            var dfa = new DataFlowAnalysis(pb.BuildProgram(), null, new FakeDecompilerEventListener());
            dfa.AnalyzeProgram2();
            var sExp = @"// test
// Return size: 0
void test()
test_entry:
	// succ:  l1
l1:
	Mem6[0x00010008:word32] = dwArg04 + dwArg08
	word32 r63_1 = fp
	word32 r2_4 = dwArg08
	return
	// succ:  test_exit
test_exit:
";
            AssertProgram(sExp, pb.Program);
        }

        private ProcedureBase GivenFunction(string name, RegisterStorage ret, params object [] args)
        {
            var frame = pb.Program.Architecture.CreateFrame();
            Identifier idRet = null;
            if (ret != null)
            {
                idRet = frame.EnsureRegister(ret);
            }
            List<Identifier> parameters = new List<Identifier>();
            foreach (int offset in args)
            {
                parameters.Add(frame.EnsureStackArgument(offset, pb.Program.Architecture.WordWidth));
            }
            var proc = new ExternalProcedure(name, new FunctionType(
                idRet, 
                parameters.ToArray()));
            return proc;
        }

        [Test]
        [Ignore("scanning-development")]
        public void Dfa2_CallProc()
        {
            pb = new ProgramBuilder();
            pb.Add("test", m =>
            {
                var sp = m.Register(m.Architecture.StackRegister);

                var fooProc = GivenFunction("foo", m.Architecture.GetRegister(1), 4, 8);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, m.Word32(2));
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, m.Word32(1));
                m.Call(fooProc, 4);
                m.Assign(sp, m.IAdd(sp, 8));
                m.Return();
            });

            var dfa = new DataFlowAnalysis(pb.BuildProgram(), null, new FakeDecompilerEventListener());
            dfa.AnalyzeProgram2();
            var sExp = @"// test
// Return size: 0
void test()
test_entry:
	// succ:  l1
l1:
	Mem9[0x00010008:word32] = Mem0[fp + 0x00000004:word32] + Mem0[fp + 0x00000008:word32]
	return
	// succ:  test_exit
test_exit:
";
            AssertProgram(sExp, pb.Program);
        }

        [Test]
        [Ignore("")]
        public void Dfa2_FactorialReg()
        {
            var program = Factorial.BuildSample();
            var dfa = new DataFlowAnalysis(program, null, new FakeDecompilerEventListener());
            dfa.AnalyzeProgram2();
            var sExp =
            @"@@@";
            AssertProgram(sExp, program);
        }

        [Test]
        [Category(Categories.FailedTests)]
        [Ignore(Categories.FailedTests)]
        public void Dfa2_UserDefinedStackArgs()
        {
            var arch = new X86ArchitectureFlat32();
            var pb = new ProgramBuilder(arch);
            var test = pb.Add(
                new Procedure_v1
                {
                    CSignature = "void test(int a, int b)"
                },
                m => {
                    var r1 = m.Reg32("r1", 1);
                    var r2 = m.Reg32("r2", 2);
                    var fp = m.Frame.FramePointer;
                    m.Assign(r1, m.LoadDw(m.IAdd(fp, 4)));
                    m.Assign(r2, m.LoadDw(m.IAdd(fp, 8)));
                    m.Assign(r1, m.IAdd(r1, r2));
                    m.Store(m.Word32(0x010008), r1);
                    m.Return();
                });
            var program = pb.BuildProgram();
            var platform = new FakePlatform(null, arch)
            {
                Test_DefaultCallingConvention = "__cdecl",
            };
            platform.Test_CreateImplicitArgumentRegisters = () =>
                new HashSet<RegisterStorage>();
            //platform.Test_CreateProcedureSerializer = (t, d) =>
            //{
            //    var typeLoader = new TypeLibraryDeserializer(platform, false, new TypeLibrary());
            //    return new ProcedureSerializer(program.Platform, typeLoader, "__cdecl");
            //};

            var importResolver = MockRepository.GenerateStub<IImportResolver>();
            importResolver.Replay();
            program.Platform = platform;
            var dfa = new DataFlowAnalysis(program, importResolver, new FakeDecompilerEventListener());
            dfa.AnalyzeProgram2();
            var sExp = @"// test
// Return size: 4
void test(int32 a, int32 b)
test_entry:
	// succ:  l1
l1:
	word32 r1_7 = a + b
	Mem8[0x00010008:word32] = r1_7
	word32 r2_6 = b
	return
	// succ:  test_exit
test_exit:
";
            AssertProgram(sExp, pb.Program);
        }
    }
}
