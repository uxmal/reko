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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Output;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class CommentInjectorTests
    {
        private Program program;
        private ProcedureBuilder m;
        private CommentInjector cinj;

        [SetUp]
        public void Setup()
        {
            this.program = new Program
            {
                User = new UserData
                {
                }
            };
            this.m = new ProcedureBuilder();
        }

        private void Given_Comment(uint linAddress, string text)
        {
            program.User.Annotations[Address.Ptr32(linAddress)] = text;
        }

        private void When_CreateInjector()
        {
            this.cinj = new CommentInjector(program.User.Annotations);
        }


        private void AssertProcedure(string sExp)
        {
            var sw = new StringWriter();
            m.Procedure.WriteBody(false, sw);
            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Debug.Print(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        [Test]
        public void Cinj_OneComment()
        {
            Given_Comment(0, "This is a comment");

            var a = m.Reg32("a", 1);
            m.MStore(m.Word32(0x00123400), a);

            When_CreateInjector();
            cinj.InjectComments(m.Procedure);

            var sExp =
            #region Expected
@"ProcedureBuilder_entry:
l1:
	// This is a comment
	Mem0[0x00123400:word32] = a
ProcedureBuilder_exit:
";
            #endregion
            AssertProcedure(sExp);
        }

        [Test]
        public void Cinj_CommentSecondItem()
        {
            // This is misaligned, we expect it to be injected at address 4
            Given_Comment(1, "This is a comment");

            var a = m.Reg32("a", 1);
            m.MStore(m.Word32(0x00123400), a);  // addr 0
            m.MStore(m.Word32(0x00123404), a);  // addr 1
            m.MStore(m.Word32(0x00123408), a);  // addr 2

            When_CreateInjector();
            cinj.InjectComments(m.Procedure);

            var sExp =
            #region Expected
@"ProcedureBuilder_entry:
l1:
	Mem0[0x00123400:word32] = a
	// This is a comment
	Mem0[0x00123404:word32] = a
	Mem0[0x00123408:word32] = a
ProcedureBuilder_exit:
";
            #endregion
            AssertProcedure(sExp);
        }
    }
}
