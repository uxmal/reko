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
        MockRepository mr;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
        }

        [Test]
        public void NavigateToAddress()
        {
            var program = new Program();
            var project = new Project { Programs = { program } };
            var memSvc = mr.DynamicMock<ILowLevelViewService>();
            var decSvc = mr.DynamicMock<IDecompilerService>();
            var dec = mr.DynamicMock<IDecompiler>();
            memSvc.Expect(x => x.ShowMemoryAtAddress(
                Arg<Program>.Is.NotNull,
                Arg<Address>.Matches(a => a.ToLinear() == 0x1234)));
            decSvc.Stub(d => d.Decompiler).Return(dec);
            dec.Stub(d => d.Project).Return(project);
            mr.ReplayAll();

            var sc = new ServiceContainer();
            sc.AddService<ILowLevelViewService>(memSvc);
            sc.AddService<IDecompilerService>(decSvc);
            var nav = new AddressNavigator(program, Address.Ptr32(0x1234), sc);
            nav.NavigateTo();

            mr.VerifyAll();
        }

        [Test]
        public void NavigateToProcedure()
        {
            var proc = new Procedure("foo", null);

            var codeSvc = mr.DynamicMock<ICodeViewerService>();
            codeSvc.Expect(x => x.DisplayProcedure(
                Arg<Procedure>.Is.Same(proc)));
            mr.ReplayAll();

            var sc = new ServiceContainer();
            sc.AddService<ICodeViewerService>(codeSvc);
            var nav = new ProcedureNavigator(proc, sc);
            nav.NavigateTo();
            mr.VerifyAll();
        }

        [Test]
        public void NavigateToBlock()
        {
            var proc = new Procedure("foo", null);
            var block = new Block(proc, "foo_block");

            var codeSvc = mr.DynamicMock<ICodeViewerService>();
            codeSvc.Expect(x => x.DisplayProcedure(
                Arg<Procedure>.Is.Same(proc)));
            mr.ReplayAll();

            var sc = new ServiceContainer();
            sc.AddService<ICodeViewerService>(codeSvc);
            var nav = new BlockNavigator(block, sc);
            nav.NavigateTo();
            mr.VerifyAll();
        }
    }
}
