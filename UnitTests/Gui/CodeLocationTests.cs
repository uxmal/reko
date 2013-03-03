#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using Decompiler.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Decompiler.UnitTests.Gui
{
    [TestFixture]
    public class CodeLocationTests
    {
        MockRepository repository;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
        }

        [Test]
        public void NavigateToAddress()
        {
            var memSvc = repository.DynamicMock<IMemoryViewService>();
            memSvc.Expect(x => x.ShowMemoryAtAddress(
                Arg<Address>.Matches(a => a.Linear == 0x1234)));
            repository.ReplayAll();

            var sc = new ServiceContainer();
            sc.AddService<IMemoryViewService>(memSvc);
            var nav = new AddressNavigator(new Address(0x1234), sc);
            nav.NavigateTo();
            repository.VerifyAll();
        }

        [Test]
        public void NavigateToProcedure()
        {
            var proc = new Procedure("foo", null);

            var codeSvc = repository.DynamicMock<ICodeViewerService>();
            codeSvc.Expect(x => x.DisplayProcedure(
                Arg<Procedure>.Is.Same(proc)));
            repository.ReplayAll();

            var sc = new ServiceContainer();
            sc.AddService<ICodeViewerService>(codeSvc);
            var nav = new ProcedureNavigator(proc, sc);
            nav.NavigateTo();
            repository.VerifyAll();
        }

        [Test]
        public void NavigateToBlock()
        {
            var proc = new Procedure("foo", null);
            var block = new Block(proc, "foo_block");

            var codeSvc = repository.DynamicMock<ICodeViewerService>();
            codeSvc.Expect(x => x.DisplayProcedure(
                Arg<Procedure>.Is.Same(proc)));
            repository.ReplayAll();

            var sc = new ServiceContainer();
            sc.AddService<ICodeViewerService>(codeSvc);
            var nav = new BlockNavigator(block, sc);
            nav.NavigateTo();
            repository.VerifyAll();
        }
    }
}
