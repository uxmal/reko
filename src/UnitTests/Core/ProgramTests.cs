#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Types;
using NUnit.Framework;
using Rhino.Mocks;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class ProgramTests
	{
		private Program prog;
        private MockRepository mr;
        private IProcessorArchitecture arch;
        private Address addrBase;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            prog = new Program();
        }

        private void Given_Architecture()
        {
            arch = mr.Stub<IProcessorArchitecture>();
            prog.Architecture = arch;
        }

        private void Given_Image(params byte[] bytes)
        {
            addrBase = Address.Ptr32(0x00010000);
            prog.Image = new LoadedImage(addrBase, bytes);
            arch.Stub(a => a.CreateImageReader(prog.Image, addrBase)).Return(new LeImageReader(prog.Image, 0));
        }

		[Test]
		public void ProgEnsurePseudoProc()
		{
			PseudoProcedure ppp = prog.EnsurePseudoProcedure("foo", VoidType.Instance, 3);
			Assert.IsNotNull(ppp);
			Assert.AreEqual("foo", ppp.Name);
			Assert.AreEqual(1, prog.PseudoProcedures.Count);

            PseudoProcedure ppp2 = prog.EnsurePseudoProcedure("foo", VoidType.Instance, 3);
			Assert.IsNotNull(ppp2);
			Assert.AreSame(ppp, ppp2);
			Assert.AreEqual("foo", ppp.Name);
			Assert.AreEqual(1, prog.PseudoProcedures.Count);
		}

        [Test]
        public void Prog_GetDataSize_of_Integer()
        {
            Given_Architecture();
            Given_Image(0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00, 0x00);
            mr.ReplayAll();

            Assert.AreEqual(4u, prog.GetDataSize(addrBase, PrimitiveType.Int32));
        }


        [Test]
        public void Prog_GetDataSize_of_ZeroTerminatedString()
        {
            Given_Architecture();
            Given_Image(0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00, 0x00);
            mr.ReplayAll();

            var dt = StringType.NullTerminated(PrimitiveType.Char);
            Assert.AreEqual(6u, prog.GetDataSize(addrBase, dt), "5 bytes for 'hello' and 1 for the terminating null'");
        }
	}
}
