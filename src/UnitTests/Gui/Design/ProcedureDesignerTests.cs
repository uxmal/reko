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

using Reko.Gui;
using Reko.Gui.Controls;
using Reko.Gui.Design;
using Reko.Core;
using Reko.Core.Types;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Gui.Design
{
    [TestFixture]
    public class ProcedureDesignerTests
    {
        private MockRepository mr;
        private IServiceProvider services;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.services = mr.StrictMock<IServiceProvider>();
            this.program = new Program();
        }

        private void Given_Service<T>(T svc)
        {
            services.Stub(s => s.GetService(typeof(T))).Return(svc);
        }

        [Test]
        public void ProcDesigner_DefaultAction_ShowProcedure()
        {
            var proc = new Procedure("foo", new Frame(PrimitiveType.Ptr32));
            var des = new ProcedureDesigner(program, proc, null, Address.Ptr32(0x001100000), false);
            des.Services = services;
            var codeSvc = mr.StrictMock<ICodeViewerService>();
            Given_Service<ICodeViewerService>(codeSvc);
            codeSvc.Expect(c => c.DisplayProcedure(program, proc, true));
            mr.ReplayAll();

            des.DoDefaultAction();

            mr.VerifyAll();
        }
    }
}
