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

using Reko.Core;
using Reko.Core.Types;
using Reko.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Reko.UnitTests.Gui
{
    [TestFixture]
    public class ProcedureSearchResultTests
    {
        private MockRepository mr;
        private List<ProcedureSearchHit> procs;
        private ServiceContainer sc;
        private Program program;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            procs = new List<ProcedureSearchHit>();
            program = new Program { Name = "Proggie" };
        }

        [Test]
        public void CreateColumns()
        {
            var psr = new ProcedureSearchResult(mr.Stub<IServiceProvider>(), procs);

            procs.Add(new ProcedureSearchHit(program,  Address.Ptr32(0x00001), new Procedure("foo", new Frame(PrimitiveType.Word32))));
            procs.Add(new ProcedureSearchHit(program, Address.Ptr32(0x00002), new Procedure("bar", new Frame(PrimitiveType.Word32))));

            var view = mr.StrictMock<ISearchResultView>();
            view.Expect(s => view.AddColumn(
                Arg<string>.Is.Equal("Program"),
                Arg<int>.Is.Anything));
            view.Expect(s => view.AddColumn(
                Arg<string>.Is.Equal("Address"),
                Arg<int>.Is.Anything));
            view.Expect(s => view.AddColumn(
                Arg<string>.Is.Equal("Procedure Name"),
                Arg<int>.Is.Anything));

            mr.ReplayAll();

            psr.View = view;
            psr.CreateColumns();

            mr.VerifyAll();
        }

        [Test]
        public void GetItemData()
        {
            ISearchResult psr = new ProcedureSearchResult(mr.Stub<IServiceProvider>(), procs);
            procs.Add(new ProcedureSearchHit(program, Address.Ptr32(0x00001), new Procedure("foo", new Frame(PrimitiveType.Word32))));
            procs.Add(new ProcedureSearchHit(program, Address.Ptr32(0x00002), new Procedure("bar", new Frame(PrimitiveType.Word32))));

            Assert.AreEqual(-1, psr.GetItem(0).ImageIndex);
            string [] str = psr.GetItem(0).Items;
            Assert.AreEqual(3, str.Length);

            Assert.AreEqual("Proggie", str[0]);
            Assert.AreEqual("00000001", str[1]);
            Assert.AreEqual("foo", str[2]);
        }

        private T AddService<T>()
        {
            var svc = mr.StrictMock<T>();
            sc.AddService(typeof(T), svc);
            return svc;
        }

        [Test]
        public void Navigate()
        {
            var mvs = AddService<ILowLevelViewService>();
            var cvs = AddService<ICodeViewerService>();
            var decSvc = AddService<IDecompilerService>();
            var dec = mr.StrictMock<IDecompiler>();

            cvs.Stub(c => c.DisplayProcedure(null, null, true)).IgnoreArguments();
            mvs.Expect(s => s.ShowMemoryAtAddress(
                Arg<Program>.Is.NotNull,
                Arg<Address>.Is.Equal(Address.Ptr32(0x4234))));
            decSvc.Stub(d => d.Decompiler).Return(dec);
            dec.Stub(d => d.Project).Return(new Project { Programs = { new Program() } });
            mr.ReplayAll();

            ISearchResult psr = new ProcedureSearchResult(sc, procs);
            procs.Add(new ProcedureSearchHit(program, Address.Ptr32(0x4234), new Procedure("foo", new Frame(PrimitiveType.Word32))));
            psr.NavigateTo(0);
            mr.VerifyAll();
        }
    }
}
