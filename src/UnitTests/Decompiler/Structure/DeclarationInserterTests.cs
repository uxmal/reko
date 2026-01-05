#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.UnitTests.Decompiler.Structure;

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Absyn;
using Reko.Core.Expressions;
using Reko.Core.Output;
using Reko.Core.Types;
using Reko.Structure;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;

public class DeclarationInserterTests
{
    private List<Identifier> parameters = default;

    [SetUp]
    public void Setup()
    {
        this.parameters = default;
    }

    private void RunTest(string sExp, Action<AbsynCodeEmitter> gen)
    {
        var arch = new FakeArchitecture();
        var proc = new Procedure(arch, "test", Address.Ptr32(0x00123400), arch.CreateFrame());
        proc.Body = new List<AbsynStatement>();
        parameters = new List<Identifier>();
        var m = new AbsynCodeEmitter(proc.Body);
        gen(m);
        proc.Signature = FunctionType.Action(parameters.ToArray());

        var decli = new DeclarationInserter(proc);
        decli.Transform();

        var sw = new StringWriter();
        GenCode(proc, sw);
        var sActual = sw.ToString();
        if (sExp != sActual)
        {
            Console.WriteLine(sActual);
            Assert.AreEqual(sExp, sActual);
        }
    }

    private void AddParameter(Identifier id)
    {
        parameters.Add(id);
    }

    private void GenCode(Procedure proc, StringWriter sw)
    {
        sw.WriteLine("");
        sw.WriteLine("{0}()", proc.Name);
        sw.WriteLine("{");

        var cf = new AbsynCodeFormatter(new TextFormatter(sw) { UseTabs = false });
        cf.WriteStatementList(proc.Body);

        sw.WriteLine("}");
    }

    [Test]
    public void Deci_IfElseDeclaration()
    {
        var sExp =
@"
test()
{
    int32 y;
    if (g_var != 0)
        y = 3;
    else
        y = 9;
    g_y = y * 2;
}
";
        RunTest(sExp, m => {

            var g_var = Identifier.Create(
                new GlobalStorage("g_var", PrimitiveType.Int32));
            var g_y = Identifier.Create(
                new GlobalStorage("g_y", PrimitiveType.Int32));
            var y = Identifier.CreateTemporary("y", PrimitiveType.Int32);

            m.If(m.Ne0(g_var),
                mm => mm.Assign(y, m.Int32(3)),
                mm => mm.Assign(y, m.Int32(9)));
            m.Assign(g_y, m.IMul(y, 2));
        });
    }

    [Test]
    public void Deci_LocalInBranch()
    {
        var sExp =
@"
test()
{
    if (*arg >= 0)
    {
        int32 x = *arg;
        x = x + 1;
        *arg = x;
    }
}
";
        RunTest(sExp, m => {
            var pint = new Pointer(PrimitiveType.Int32, 32);
            var arg = Identifier.Create(RegisterStorage.Reg32("arg", 0));
            arg.DataType = pint;
            AddParameter(arg);

            var x = Identifier.CreateTemporary("x", PrimitiveType.Int32);

            m.If(m.Ge0(new Dereference(PrimitiveType.Int32, arg)),
                mm =>
                {
                    mm.Assign(x, mm.Deref(arg));
                    mm.Assign(x, mm.IAdd(x, m.Int32(1)));
                    mm.Assign(mm.Deref(arg), x);
                });
        });
    }

    [Test]
    public void Deci_ModifyParameter()
    {

        var sExp =
@"
test()
{
    if (arg < 0)
        arg = 0;
    return arg;
}
";
        RunTest(sExp, m => {
            
            var arg = Identifier.Create(RegisterStorage.Reg32("arg", 0));
            arg.DataType = PrimitiveType.Int32;
            AddParameter(arg);

            m.If(m.Lt(arg, 0),
                mm => mm.Assign(arg, m.Int32(0)));
            m.Return(arg);
        });
    }

    [Test]
    public void Deci_ChainedIfElses()
    {
        var sExp =
@"
test()
{
    char * result;
    if (arg == 0x01)
        result = ""one"";
    else if (arg == 0x02)
        result = ""two"";
    else if (arg == 0x03)
        result = ""three"";
    else
        result = ""many"";
    return result;
}
";
        RunTest(sExp, m =>
        {
            var pch = new Pointer(PrimitiveType.Char, 32);
            var arg = Identifier.Create(RegisterStorage.Reg32("arg", 0));
            AddParameter(arg);

            var result = Identifier.Create(RegisterStorage.Reg32("result", 0));
            result.DataType = pch;

            m.If(m.Eq(arg, 1),
                mm => mm.Assign(result, new StringConstant(pch, "one")),
                mm => mm.If(mm.Eq(arg, 2),
                    nn => nn.Assign(result, new StringConstant(pch, "two")),
                    nn => nn.If(nn.Eq(arg, 3),
                        oo => oo.Assign(result, new StringConstant(pch, "three")),
                        oo => oo.Assign(result, new StringConstant(pch, "many")))));
            m.Return(result);
        });
    }

    [Test]
    public void Deci_OutArgument()
    {
        var sExp =
@"
test()
{
    int32 chunk;
    while (GetChunk(out chunk))
        process(chunk);
}
";
        RunTest(sExp, m => {
            var chunk = Identifier.CreateTemporary("chunk", PrimitiveType.Int32);
            m.While(m.Fn("GetChunk", m.Out(PrimitiveType.Int32, chunk)),
                mm => mm.SideEffect(mm.Fn("process", chunk)));
        });
    }

    [Test]
    public void Deci_DoWhile()
    {
        var sExp =
@"
test()
{
    barify();
    bool dirty;
    do
        dirty = frobulate();
    while (dirty);
}
";
        RunTest(sExp, m => {
            var dirty = Identifier.CreateTemporary("dirty", PrimitiveType.Bool);
            m.SideEffect(m.Fn("barify"));
            m.DoWhile(
                mm => mm.Assign(dirty, m.Fn("frobulate")),
                dirty);
        });
    }

    [Test]
    public void Deci_SideEffect()
    {
        var sExp =
@"
test()
{
    int32 chunk;
    process(out chunk);
}
";
        RunTest(sExp, m => {
            var chunk = Identifier.CreateTemporary("chunk", PrimitiveType.Int32);
            m.SideEffect(m.Fn("process", m.Out(chunk.DataType, chunk)));
        });
    }

    [Test]
    public void Deci_Struct_Field_Assigment()
    {
        var sExp =
@"
test()
{
    struct struct_t id;
    id.foo = 3;
}
";
        RunTest(sExp, m =>
        {
            var foo = new StructureField(0, PrimitiveType.Int32, "foo");
            var id = Identifier.CreateTemporary("id", new StructureType
            {
                Name = "struct_t",
                Fields =
                {
                    foo
                }
            });
            m.Assign(m.Field(foo.DataType, id, foo), Constant.Int32(3));
        });
    }

    [Test]
    public void Deci_Return()
    {
        var sExp =
@"
test()
{
    word32 mystery;
    return foo(mystery);
}
";

        RunTest(sExp, m =>
        {
            var mystery = Identifier.CreateTemporary("mystery", PrimitiveType.Word32);
            m.Return(m.Fn("foo", mystery));
        });
    }
}
