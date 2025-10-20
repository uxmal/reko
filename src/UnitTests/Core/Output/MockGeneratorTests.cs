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

using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Core.Output;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;
using System.Diagnostics;
using Reko.Core;

namespace Reko.UnitTests.Core.Output
{
    [TestFixture]
    public class MockGeneratorTests
    {
        private StringWriter sb;
        private string nl = Environment.NewLine;

        [SetUp]
        public void Setup()
        {
            sb = new StringWriter();
        }

        private void CompileClassTest(Action<ProcedureBuilder> buildMock)
        {
            ProcedureBuilder m = new ProcedureBuilder();
            buildMock(m);
            MockGenerator g = new MockGenerator(sb, "");
            g.WriteClass(m.Procedure);
        }

        private void CompileMethodTest(Action<ProcedureBuilder> buildMock)
        {
            ProcedureBuilder m = new ProcedureBuilder();
            buildMock(m);
            MockGenerator g = new MockGenerator(sb, "m.");
            g.WriteMethod(m.Procedure);
        }

        private void VerifyTest(string sExp)
        {
            string s = sb.ToString();
            try
            {
                Assert.AreEqual(sExp, s);
            }
            catch
            {
                Debug.Print("{0}", s);
                throw;
            }
        }


        [Test]
        public void Mg_EmptyFunction()
        {
            CompileClassTest(m =>
            {
                m.Return();
            });
            string sExp =
                "public class ProcedureBuilder : ProcedureMock" + nl + 
                "{" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl + 
                "    Return();" + nl + 
                "}" + nl + 
                "" + nl;
            VerifyTest(sExp);
        }

        [Test]
        public void Mg_Assign()
        {
            CompileClassTest(m =>
            {
                var id = m.Local(PrimitiveType.Word32, "id");
                m.Assign(id, 42);
                m.Return();
            });
            string sExp =
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier id = m.Frame.EnsureStackVariable(-4, PrimitiveType.Word32, \"id\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(id, Word32(0x2A));" + nl +
                "    Return();" + nl +
                "}" + nl +
                "" + nl;
            VerifyTest(sExp);

        }

        [Test]
        public void Mg_AddSubMul()
        {
            CompileClassTest(m =>
            {
                Identifier a = m.Local(PrimitiveType.Word32, "a");
                Identifier b = m.Local(PrimitiveType.Word32, "b");
                m.Assign(a, m.SMul(m.UMul(a, b), m.IAdd(a, m.ISub(b, m.IMul(a, b)))));
                m.Return();
            });
            string sExp =
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier a = m.Frame.EnsureStackVariable(-4, PrimitiveType.Word32, \"a\");" + nl +
                "    Identifier b = m.Frame.EnsureStackVariable(-8, PrimitiveType.Word32, \"b\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(a, SMul(UMul(a, b), IAdd(a, ISub(b, IMul(a, b)))));" + nl +
                "    Return();" + nl +
                "}" + nl +
                "" + nl;
            VerifyTest(sExp);
        }

        [Test]
        public void Mg_LoadStore()
        {
            CompileClassTest(m =>
            {
                m.MStore(m.Word32(0x123456), m.Mem(PrimitiveType.Byte, m.Word32(0x12348)));
                m.Return();
            });
            string sExp =
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    MStore(Word32(0x123456), Mem(PrimitiveType.Byte, Word32(0x12348)));" + nl +
                "    Return();" + nl +
                "}" + nl +
                "" + nl;
            VerifyTest(sExp); 
        }

        [Test]
        public void Mg_Declaration()
        {
            CompileClassTest(m =>
            {
                Identifier id = m.Local16("id");
                m.Assign(id, m.Word16(0x1234));
                m.Return();
            });
            string sExp = 
"public class ProcedureBuilder : ProcedureMock" + nl + 
"{" + nl +
"    Identifier id = m.Frame.EnsureStackVariable(-4, PrimitiveType.Word16, \"id\");" + nl + 
"    " + nl + 
"    Label(\"l1\");" + nl + 
"    Assign(id, Word16(0x1234));" + nl + 
"    Return();" + nl + 
"}" + nl + 
"" + nl;
            VerifyTest(sExp);
        }

        [Test]
        public void Mg_Application()
        {
            CompileClassTest(m =>
            {
                Identifier inp = m.Local32("inp");
                Identifier outp = m.Local32("outp");
                m.Assign(inp, 0);
                m.Assign(inp, m.Fn("foo", inp, m.AddrOf(PrimitiveType.Ptr32, outp)));
                m.Return();
            });
            string sExp =
"public class ProcedureBuilder : ProcedureMock" + nl + 
"{" + nl + 
"    Identifier inp = m.Frame.EnsureStackVariable(-4, PrimitiveType.Word32, \"inp\");" + nl +
"    Identifier outp = m.Frame.EnsureStackVariable(-8, PrimitiveType.Word32, \"outp\");" + nl +
"    " + nl + 
"    Label(\"l1\");" + nl + 
"    Assign(inp, Word32(0x0));" + nl + 
"    Assign(inp, Fn(\"foo\", inp, AddrOf(outp)));" + nl + 
"    Return();" + nl + 
"}" + nl + 
"" + nl;
            VerifyTest(sExp);
        }

        [Test]
        public void Mg_SelectorSideEffect()
        {
            CompileClassTest(m =>
            {
                Identifier es = m.Local(PrimitiveType.SegmentSelector, "es");
                m.SideEffect(m.Fn("foo", es));
            });
            string sExp = 
"public class ProcedureBuilder : ProcedureMock" + nl + 
"{" + nl + 
"    Identifier es = m.Frame.EnsureStackVariable(-2, PrimitiveType.SegmentSelector, \"es\");" + nl + 
"    " + nl + 
"    Label(\"l1\");" + nl + 
"    SideEffect(Fn(\"foo\", es));" + nl + 
"}" + nl + 
"" + nl;
            VerifyTest(sExp);

        }

        [Test]
        public void Mg_Branch()
        {
            CompileClassTest(m =>
            {
                Identifier i = m.Local(PrimitiveType.Int32, "i");

                m.BranchIf(m.Eq(i, 0), "skip");
                m.Label("fade");
                m.MStore(m.Word32(0x123456), i);
                m.Label("skip");
                m.Return(i);
            });
            string sExp = 
"public class ProcedureBuilder : ProcedureMock" + nl + 
"{" + nl + 
"    Identifier i = m.Frame.EnsureStackVariable(-4, PrimitiveType.Int32, \"i\");" + nl + 
"    " + nl + 
"    Label(\"l1\");" + nl + 
"    BranchIf(Eq(i, Int32(0)), \"skip\");" + nl + 
"    " + nl + 
"    Label(\"fade\");" + nl + 
"    MStore(Word32(0x123456), i);" + nl + 
"    " + nl + 
"    Label(\"skip\");" + nl + 
"    Return(i);" + nl + 
"}" + nl + 
"" + nl;

            VerifyTest(sExp);
        }

        [Test]
        public void Mg_ComparisonOperators()
        {
            CompileClassTest(m =>
            {
                Identifier f = m.Local(PrimitiveType.Bool, "f");
                Identifier a = m.Local(PrimitiveType.Word32, "a");
                Identifier b = m.Local(PrimitiveType.Word32, "b");

                m.Assign(f, m.Le(a, b)); 
                m.Assign(f, m.Ge(a, b)); 
                m.Assign(f, m.Uge(a, b)); 
                m.Assign(f, m.Ule(a, b)); 
                m.Assign(f, m.Lt(a, b)); 
                m.Assign(f, m.Gt(a, b)); 
                m.Assign(f, m.Ugt(a, b)); 
                m.Assign(f, m.Ult(a, b)); 
            });
            VerifyTest(
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier f = m.Frame.EnsureStackVariable(-1, PrimitiveType.Bool, \"f\");" + nl +
                "    Identifier a = m.Frame.EnsureStackVariable(-5, PrimitiveType.Word32, \"a\");" + nl +
                "    Identifier b = m.Frame.EnsureStackVariable(-9, PrimitiveType.Word32, \"b\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(f, Le(a, b));" + nl +
                "    Assign(f, Ge(a, b));" + nl +
                "    Assign(f, Uge(a, b));" + nl +
                "    Assign(f, Ule(a, b));" + nl +
                "    Assign(f, Lt(a, b));" + nl +
                "    Assign(f, Gt(a, b));" + nl +
                "    Assign(f, Ugt(a, b));" + nl +
                "    Assign(f, Ult(a, b));" + nl +
                "}" + nl +
                "" + nl);
        }

        [Test]
        public void Mg_LogicalOperators()
        {
            CompileClassTest(m =>
            {
                Identifier f = m.Local(PrimitiveType.Bool, "f");
                Identifier a = m.Local(PrimitiveType.Word32, "a");
                Identifier b = m.Local(PrimitiveType.Word32, "b");

                m.Assign(f, m.Cand(a, m.Cor(a, b)));
            });
            VerifyTest(
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier f = m.Frame.EnsureStackVariable(-1, PrimitiveType.Bool, \"f\");" + nl +
                "    Identifier a = m.Frame.EnsureStackVariable(-5, PrimitiveType.Word32, \"a\");" + nl +
                "    Identifier b = m.Frame.EnsureStackVariable(-9, PrimitiveType.Word32, \"b\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(f, Cand(a, Cor(a, b)));" + nl +
                "}" + nl +
                "" + nl);

        }

        [Test]
        public void Mg_BitwiseOperators()
        {
            CompileClassTest(m =>
            {
                Identifier a = m.Local(PrimitiveType.Word32, "a");
                Identifier b = m.Local(PrimitiveType.Word32, "b");
                Identifier c = m.Local(PrimitiveType.Word32, "c");

                m.Assign(c, m.Xor(a, m.Or(a, m.And(a, b))));
            });
            VerifyTest(
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier a = m.Frame.EnsureStackVariable(-4, PrimitiveType.Word32, \"a\");" + nl +
                "    Identifier b = m.Frame.EnsureStackVariable(-8, PrimitiveType.Word32, \"b\");" + nl +
                "    Identifier c = m.Frame.EnsureStackVariable(-12, PrimitiveType.Word32, \"c\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(c, Xor(a, Or(a, And(a, b))));" + nl +
                "}" + nl +
                "" + nl);

        }

        [Test]
        public void Mg_SliceDpb()
        {
            CompileClassTest(m =>
            {
                Identifier a = m.Local(PrimitiveType.Word32, "a");
                Identifier b = m.Local(PrimitiveType.Word32, "b");

                m.Assign(a, m.Dpb(a, m.Slice(b, PrimitiveType.Word16, 16), 0));
            });
            VerifyTest(
                "public class ProcedureBuilder : ProcedureMock" + nl +
                "{" + nl +
                "    Identifier a = m.Frame.EnsureStackVariable(-4, PrimitiveType.Word32, \"a\");" + nl +
                "    Identifier b = m.Frame.EnsureStackVariable(-8, PrimitiveType.Word32, \"b\");" + nl +
                "    " + nl +
                "    Label(\"l1\");" + nl +
                "    Assign(a, Seq(Slice(PrimitiveType.Word16, a, 16), Slice(PrimitiveType.Word16, b, 16)));" + nl +
                "}" + nl +
                "" + nl);
        }

        [Test]
        public void Mg_SliceSeq()
        {
            CompileMethodTest(m =>
            {
                m.Store(m.Mem32(m.Word32(0x00123400)), m.Seq(m.Byte(1), m.Byte(2), m.Word16(0x0304)));
            });
            VerifyTest(
@"RunTest(m =>
{
    
    m.Label(""l1"");
    m.MStore(m.Word32(0x123400), m.Seq(m.Byte(0x1), m.Byte(0x2), m.Word16(0x304)));
});

");
        }

        [Test]
        public void Mg_NegativeInt32()
        {
            CompileMethodTest(m =>
            {
                var fp = m.Frame.FramePointer;
                m.MStore(m.IAdd(fp, 32), Constant.Int32(-12));
            });
            VerifyTest(@"RunTest(m =>
{
    Identifier fp = m.Frame.FramePointer;
    
    m.Label(""l1"");
    m.MStore(m.IAdd(fp, m.Word32(0x20)), m.Int32(-12));
});

");
        }

        [Test]
        public void Mg_RegisterVar()
        {
            CompileMethodTest(m =>
            {
                var eax = m.Register(RegisterStorage.Reg32("eax", 0));
                var ebx = m.Register(RegisterStorage.Reg32("ebx", 3));
                m.Assign(eax, m.IAdd(ebx, 1));
            });
            VerifyTest(@"RunTest(m =>
{
    RegisterStorage reg_eax = new RegisterStorage(""eax"", 0, 0, PrimitiveType.Word32);
    RegisterStorage reg_ebx = new RegisterStorage(""ebx"", 3, 0, PrimitiveType.Word32);
    Identifier eax = m.Frame.EnsureRegister(reg_eax);
    Identifier ebx = m.Frame.EnsureRegister(reg_ebx);
    
    m.Label(""l1"");
    m.Assign(eax, m.IAdd(ebx, m.Word32(0x1)));
});

");
        }

        [Test]
        public void Mg_SequenceVariable()
        {
            CompileMethodTest(m =>
            {
                var eax = m.Register(RegisterStorage.Reg32("eax", 0));
                var ebx = m.Register(RegisterStorage.Reg32("ebx", 3));
                var ebx_eax = m.Frame.EnsureSequence(PrimitiveType.Word64, ebx.Storage, eax.Storage);
                m.Assign(ebx_eax, 1);
            });
            VerifyTest(@"RunTest(m =>
{
    RegisterStorage reg_ebx = new RegisterStorage(""ebx"", 3, 0, PrimitiveType.Word32);
    RegisterStorage reg_eax = new RegisterStorage(""eax"", 0, 0, PrimitiveType.Word32);
    SequenceStorage seq_ebx_eax = new SequenceStorage(PrimitiveType.Word64, reg_ebx, reg_eax);
    Identifier ebx_eax = m.Frame.EnsureSequence(PrimitiveType.Word64, seq_ebx_eax);
    
    m.Label(""l1"");
    m.Assign(ebx_eax, m.Word64(0x1));
});

");
        }

        [Test]
        public void Mg_TempIdentifiers()
        {
            CompileMethodTest(m =>
            {
                var tmp = m.Frame.CreateTemporary(PrimitiveType.Word32);
                var eax = m.Register(RegisterStorage.Reg32("eax", 0));
                var ebx = m.Register(RegisterStorage.Reg32("ebx", 3));
                m.Assign(tmp, eax);
                m.Assign(ebx, tmp);
            });
            VerifyTest(@"RunTest(m =>
{
    RegisterStorage reg_eax = new RegisterStorage(""eax"", 0, 0, PrimitiveType.Word32);
    RegisterStorage reg_ebx = new RegisterStorage(""ebx"", 3, 0, PrimitiveType.Word32);
    Identifier eax = m.Frame.EnsureRegister(reg_eax);
    Identifier ebx = m.Frame.EnsureRegister(reg_ebx);
    Identifier v3 = m.Frame.CreateTemporary(""v3"", 3, PrimitiveType.Word32);
    
    m.Label(""l1"");
    m.Assign(v3, eax);
    m.Assign(ebx, v3);
});

");
        }

        [Test]
        public void Mg_FlagGroups()
        {
            CompileMethodTest(m =>
            {
                var status = new RegisterStorage("Status", 42, 0, PrimitiveType.Word32);
                var grf = m.Frame.EnsureFlagGroup(new FlagGroupStorage(status, 0x3, "CZ"));
                m.Assign(grf, m.Cond(grf.DataType, m.Mem32(Address.Ptr32(0x123400))));
            });
            VerifyTest(@"RunTest(m =>
{
    RegisterStorage reg_Status = new RegisterStorage(""Status"", 42, 0, PrimitiveType.Word32);
    FlagGroupStorage grf_CZ = new FlagGroupStorage(reg_Status, 0x3, ""CZ"");
    Identifier CZ = m.Frame.EnsureFlagGroup(grf_CZ);
    
    m.Label(""l1"");
    m.Assign(CZ, m.Cond(m.Mem(PrimitiveType.Word32, Address.Ptr32(0x123400))));
});

");
        }

    }
}
