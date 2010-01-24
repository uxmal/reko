/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Gui
{
    [TestFixture]
    public class ProcedureSearchResultTests
    {
        private MockRepository repository;
        private SortedList<Address, Procedure> procs;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            procs = new SortedList<Address, Procedure>();
        }

        [Test]
        public void CreateColumns()
        {

            var psr = new ProcedureSearchResult(repository.Stub<IServiceProvider>(), procs);

            procs.Add(new Address(0x00001), new Procedure("foo", new Frame(PrimitiveType.Word32)));
            procs.Add(new Address(0x00002), new Procedure("bar", new Frame(PrimitiveType.Word32)));

            var view = repository.StrictMock<ISearchResultView>();
            view.Expect(s => view.AddColumn(
                Arg<string>.Is.Equal("Address"),
                Arg<int>.Is.Anything));
            view.Expect(s => view.AddColumn(
                Arg<string>.Is.Equal("Procedure Name"),
                Arg<int>.Is.Anything));

            repository.ReplayAll();

            psr.CreateColumns(view);

            repository.VerifyAll();
        }

        [Test]
        public void GetItemData()
        {
            ISearchResult psr = new ProcedureSearchResult(repository.Stub<IServiceProvider>(), procs);
            procs.Add(new Address(0x00001), new Procedure("foo", new Frame(PrimitiveType.Word32)));
            procs.Add(new Address(0x00002), new Procedure("bar", new Frame(PrimitiveType.Word32)));

            Assert.AreEqual(-1, psr.GetItemImageIndex(0));
            string [] str = psr.GetItemStrings(0);
            Assert.AreEqual(2, str.Length);
            Assert.AreEqual("00000001", str[0]);
            Assert.AreEqual("foo", str[1]);

        }

        [Test]
        public void Navigate()
        {
            var sp = repository.StrictMock<IServiceProvider>();
            var mvs = repository.StrictMock<IMemoryViewService>();
            
            sp.Expect(s => s.GetService(
                Arg<Type>.Is.Same(typeof (IMemoryViewService)))).Return(mvs);
            mvs.Expect(s => s.ShowMemoryAtAddress(
                Arg<Address>.Is.Equal(new Address(0x4234))));
            repository.ReplayAll();

            ISearchResult psr = new ProcedureSearchResult(sp, procs);
            procs.Add(new Address(0x4234), new Procedure("foo", new Frame(PrimitiveType.Word32)));
            psr.NavigateTo(0);
            repository.VerifyAll();

        }
    }
}
