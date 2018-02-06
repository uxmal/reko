#region License
/* 
 * Copyright (C) 1999-2018 Pavel Tomin.
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

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System.Linq;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    class IndirectCallRewriterTests : AnalysisTestBase
    {
        private string CSignature;
        private IDictionary<string, DataType> types;
        SsaProcedureBuilder m;

        [SetUp]
        public void Setup()
        {
            m = new SsaProcedureBuilder();
        }

        private void Given_CSignature(string CSignature)
        {
            this.CSignature = CSignature;
        }

        private void Given_Typedef(string name, DataType dt)
        {
            if (types == null)
                types = new Dictionary<string, DataType>();
            types[name] = dt;
        }

        private DataType Ptr32(DataType pointee)
        {
            return new Pointer(pointee, 4);
        }

        private DataType Int32()
        {
            return PrimitiveType.Int32;
        }

        private DataType FnPtr32(
            Identifier returnValue,
            params Identifier[] parameters)
        {
            var storages = parameters.Select(p => p.Storage as StackStorage)
                .Where(stg => stg != null);
            var stackDelta = storages.Count() == 0 ? 4 :
                storages.Max(stg => stg.StackOffset + stg.DataType.Size);
            var ft = new FunctionType(returnValue, parameters)
            {
                StackDelta = stackDelta,
            };
            return Ptr32(ft);
        }

        private DataType RefFnPtr32(
            string name,
            Identifier returnValue,
            params Identifier[] parameters)
        {
            return new TypeReference(name, FnPtr32(returnValue, parameters));
        }

        private Identifier VoidId()
        {
            return null;
        }

        private Identifier StackId(int offset, DataType dt)
        {
            return new Identifier(
                "",
                dt,
                new StackArgumentStorage(offset, dt));
        }

        private Identifier EaxId()
        {
            return new Identifier(
                "",
                PrimitiveType.Word32,
                RegisterStorage.Reg32("eax", 0));
        }

        private Identifier EcxId()
        {
            return new Identifier(
                "",
                PrimitiveType.Word32,
                RegisterStorage.Reg32("ecx", 1));
        }

        private DataType VtblStr(params DataType[] methods)
        {
            var vtbl_str = new StructureType(null, 0, true);
            int offset = 0;
            foreach (var method in methods)
            {
                vtbl_str.Fields.Add(offset, method);
                offset += 4;
            }
            return new StructureType(null, 0, true)
            {
                Fields =
                    {
                        { 0, Ptr32(vtbl_str), "vtbl" },
                    }
            };
        }

        private DataType TestStr()
        {
            var fn0000 = FnPtr32(
                VoidId());
            var fn0004 = FnPtr32(
                VoidId(),
                StackId(4, Int32()));
            var fn0008 = FnPtr32(
                VoidId(),
                StackId(4, Int32()),
                StackId(8, Int32()));
            return VtblStr(fn0000, fn0004, fn0008);
        }

        private DataType TestStrEcx()
        {
            var fn0000 = FnPtr32(
                EaxId(),
                EcxId(),
                StackId(4, Int32()));
            var fn0004 = FnPtr32(
                EaxId(),
                EcxId(),
                StackId(4, Int32()));
            var fn0008 = FnPtr32(
                EaxId(),
                EcxId(),
                StackId(4, Int32()),
                StackId(8, Int32()));
            return VtblStr(fn0000, fn0004, fn0008);
        }

        private DataType TestStrNoFuncs()
        {
            return VtblStr(Ptr32(Int32()), Ptr32(Int32()), Ptr32(Int32()));
        }

        private void SetCSignatures(Program program)
        {
            foreach (var addr in program.Procedures.Keys)
            {
                program.User.Procedures.Add(
                    addr,
                    new Procedure_v1
                    {
                        CSignature = this.CSignature
                    });
            }
        }

        private void SetTypedefs(Program program)
        {
            foreach (var de in types)
            {
                program.EnvironmentMetadata.Types.Add(de);
            }
        }

        private void InitProgram(Program program)
        {
            SetCSignatures(program);
            SetTypedefs(program);
        }

        protected override void RunTest(Program program, TextWriter fut)
        {
            InitProgram(program);
            IImportResolver importResolver = null;
            var eventListener = new FakeDecompilerEventListener();
            DataFlowAnalysis dfa = new DataFlowAnalysis(
                program,
                importResolver,
                eventListener);
            dfa.UntangleProcedures();

            foreach (Procedure proc in program.Procedures.Values)
            {
                SsaTransform sst = new SsaTransform(
                    dfa.ProgramDataFlow,
                    proc,
                    importResolver,
                    proc.CreateBlockDominatorGraph(),
                    new HashSet<RegisterStorage>());
                SsaState ssa = sst.SsaState;

                var icrw = new IndirectCallRewriter(
                    program,
                    ssa,
                    eventListener);
                icrw.Rewrite();

                ssa.Write(fut);
                proc.Write(false, fut);
                fut.WriteLine();
            }
        }

        public void RunIndirectCallRewriter()
        {
            var eventListener = new FakeDecompilerEventListener();
            var program = new Program
            {
                Architecture = m.Architecture,
            };
            var icrw = new IndirectCallRewriter(
                program,
                m.Ssa,
                eventListener);
            icrw.Rewrite();
            m.Ssa.CheckUses(s => Assert.Fail(s));
        }

        [Test]
        public void Icrw_NoArguments()
        {
            Given_Typedef("str", TestStr());
            Given_CSignature("void test(str *a)");
            RunFileTest(
                "Fragments/icrw/indirect_call_no_arguments.asm",
                "Analysis/IcrwNoArguments.txt");
        }

        [Test]
        public void Icrw_InvalidArguments()
        {
            Given_Typedef("str", TestStrEcx());
            Given_CSignature("void test(str *a)");
            RunFileTest(
                "Fragments/icrw/indirect_call_no_arguments.asm",
                "Analysis/IcrwInvalidArguments.txt");
        }

        [Test]
        public void Icrw_OneArgument()
        {
            Given_Typedef("str", TestStr());
            Given_CSignature("void test(str *a)");
            RunFileTest(
                "Fragments/icrw/indirect_call_one_argument.asm",
                "Analysis/IcrwOneArgument.txt");
        }

        [Test]
        public void Icrw_OneArgumentNoFuncs()
        {
            Given_Typedef("str", TestStrNoFuncs());
            Given_CSignature("void test(str *a)");
            RunFileTest(
                "Fragments/icrw/indirect_call_one_argument.asm",
                "Analysis/IcrwOneArgumentNoFuncs.txt");
        }

        [Test]
        public void Icrw_OneArgumentPassEcx()
        {
            Given_Typedef("str", TestStrEcx());
            Given_CSignature("void test(str *a)");
            RunFileTest(
                "Fragments/icrw/indirect_call_one_argument.asm",
                "Analysis/IcrwOneArgumentPassEcx.txt");
        }

        [Test]
        public void Icrw_TwoArguments()
        {
            Given_Typedef("str", TestStr());
            Given_CSignature("void test(str *a)");
            RunFileTest(
                "Fragments/icrw/indirect_call_two_arguments.asm",
                "Analysis/IcrwTwoArguments.txt");
        }

        [Test]
        public void Icrw_TwoArgumentsNoFuncs()
        {
            Given_Typedef("str", TestStrNoFuncs());
            Given_CSignature("void test(str *a)");
            RunFileTest(
                "Fragments/icrw/indirect_call_two_arguments.asm",
                "Analysis/IcrwTwoArgumentsNoFuncs.txt");
        }

        [Test]
        public void Icrw_TypeReferenceToFunc()
        {
            var a = m.Reg32("a");
            var b = m.Reg32("b");
            var fn = m.Reg32("fn");
            fn.DataType = RefFnPtr32("FnPtr", a, b);
            var uses = new Identifier[] { b };
            var defines = new Identifier[] { a };
            var callStm = m.Call(fn, 4, uses, defines);

            RunIndirectCallRewriter();

            Assert.AreEqual("a = fn(b)", callStm.Instruction.ToString());
        }
    }
}
