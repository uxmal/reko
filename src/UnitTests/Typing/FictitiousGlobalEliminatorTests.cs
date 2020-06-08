#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Typing;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Typing
{
    public class FictitiousGlobalEliminatorTests
    {
        private ProgramBuilder pb;

        [SetUp]
        public void Setup()
        {
            this.pb = new ProgramBuilder();
            pb.BuildProgram();
        }

        private Expression GlobalAccess(
            ProcedureBuilder m, DataType dt, int offset, string name = null)
        {
            var field = new StructureField(offset, dt, name);
            return m.Field(dt, m.Deref(pb.Program.Globals), field);
        }

        private Procedure Given_Procedure(string name, Action<ProcedureBuilder> builder)
        {
            return pb.Add(name, builder);
        }

        private void When_RunFictitiousGlobalEliminator()
        {
            var program = pb.Program;
            var proc = program.Procedures.Values.First();
            var tge = new FictitiousGlobalEliminator(program);
            tge.Transform(proc);
        }

        private void AssertProcedureCode(string expected)
        {
            var proc = this.pb.Program.Procedures.Values.First();
            var writer = new StringWriter();
            proc.WriteBody(false, writer);
            var actual = writer.ToString();
            if (actual != expected)
            {
                Console.WriteLine(actual);
            }
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Fge_Global()
        {
            Given_Procedure("proc", m =>
            {
                m.Store(
                    GlobalAccess(m, PrimitiveType.Int32, 0x1234),
                    m.Word32(0));
            });

            When_RunFictitiousGlobalEliminator();

            var expected =
            #region Expected
@"proc_entry:
l1:
	g_dw1234 = 0<32>
proc_exit:
";
            #endregion
            AssertProcedureCode(expected);
        }

        [Test]
        public void Fge_UserGlobal()
        {
            Given_Procedure("proc", m =>
            {
                m.Store(
                    GlobalAccess(m, PrimitiveType.Int32, 0x1234, "usr"),
                    m.Word32(0));
            });

            When_RunFictitiousGlobalEliminator();

            var expected =
            #region Expected
@"proc_entry:
l1:
	usr = 0<32>
proc_exit:
";
            #endregion
            AssertProcedureCode(expected);
        }

        [Test]
        public void Fge_NestedFieldAccess()
        {
            Given_Procedure("proc", m =>
            {
                m.Store(
                    m.Field(
                        PrimitiveType.Int32,
                        GlobalAccess(m, new UnknownType(16), 0x1234),
                        new StructureField(0x4, PrimitiveType.Int32)),
                    m.Word32(0));
            });

            When_RunFictitiousGlobalEliminator();

            var expected =
            #region Expected
@"proc_entry:
l1:
	g_t1234.dw0004 = 0<32>
proc_exit:
";
            #endregion
            AssertProcedureCode(expected);
        }
    }
}
