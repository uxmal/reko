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

using Reko.Core;
using Reko.Core.Types;
using Reko.Gui;
using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Reko.UnitTests.Mocks;

namespace Reko.UnitTests.Gui
{
    [TestFixture]
    public class ProcedureSearchResultTests
    {
        private List<ProcedureSearchHit> procs;
        private ServiceContainer sc;
        private Program program;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            procs = new List<ProcedureSearchHit>();
            program = new Program { Name = "Proggie", Architecture = new FakeArchitecture() };
        }

        [Test]
        public void CreateColumns()
        {
            var psr = new ProcedureSearchResult(new Mock<IServiceProvider>().Object, procs);

            var addr1 = Address.Ptr32(0x00001);
            var addr2 = Address.Ptr32(0x00002);

            procs.Add(new ProcedureSearchHit(program, addr1, new Procedure(program.Architecture, "foo", addr1, new Frame(PrimitiveType.Word32))));
            procs.Add(new ProcedureSearchHit(program, addr2, new Procedure(program.Architecture, "bar", addr2, new Frame(PrimitiveType.Word32))));

            var view = new Mock<ISearchResultView>();
            view.Setup(s => s.AddColumn(
                "Program",
                It.IsAny<int>())).Verifiable();
            view.Setup(s => s.AddColumn(
                "Address",
                It.IsAny<int>())).Verifiable();
            view.Setup(s => s.AddColumn(
                "Procedure Name",
                It.IsAny<int>())).Verifiable();

            psr.View = view.Object;
            psr.CreateColumns();

            view.VerifyAll();
        }

        [Test]
        public void GetItemData()
        {
            ISearchResult psr = new ProcedureSearchResult(new Mock<IServiceProvider>().Object, procs);

            var addr1 = Address.Ptr32(0x00001);
            var addr2 = Address.Ptr32(0x00002);
            procs.Add(new ProcedureSearchHit(program, addr1, new Procedure(program.Architecture, "foo", addr1, new Frame(PrimitiveType.Word32))));
            procs.Add(new ProcedureSearchHit(program, addr2, new Procedure(program.Architecture, "bar", addr2, new Frame(PrimitiveType.Word32))));

            Assert.AreEqual(-1, psr.GetItem(0).ImageIndex);
            string [] str = psr.GetItem(0).Items;
            Assert.AreEqual(3, str.Length);

            Assert.AreEqual("Proggie", str[0]);
            Assert.AreEqual("00000001", str[1]);
            Assert.AreEqual("foo", str[2]);
        }

        private Mock<T> AddService<T>() where T : class
        {
            var svc = new Mock<T>();
            sc.AddService(typeof(T), svc.Object);
            return svc;
        }

        [Test]
        public void Navigate()
        {
            var mvs = AddService<ILowLevelViewService>();
            var cvs = AddService<ICodeViewerService>();
            var decSvc = AddService<IDecompilerService>();
            var dec = new Mock<IDecompiler>();

            cvs.Setup(c => c.DisplayProcedure(
                It.IsAny<Program>(),
                It.IsAny<Procedure>(),
                true));
            mvs.Setup(s => s.ShowMemoryAtAddress(
                It.IsNotNull<Program>(),
                It.Is<Address>(a => a.ToLinear() == 0x4234))).Verifiable();
            decSvc.Setup(d => d.Decompiler).Returns(dec.Object);
            dec.Setup(d => d.Project).Returns(new Project { Programs = { new Program() } });

            ISearchResult psr = new ProcedureSearchResult(sc, procs);
            var addr = Address.Ptr32(0x4234);
            procs.Add(new ProcedureSearchHit(
                program, addr,
                new Procedure(program.Architecture, "foo", addr, new Frame(PrimitiveType.Word32))));
            psr.NavigateTo(0);

            mvs.VerifyAll();
        }
    }
}
