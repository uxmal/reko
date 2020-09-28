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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Types;
using Reko.Gui;
using Reko.Gui.Design;
using Reko.UnitTests.Mocks;
using System;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Gui.Design
{
    [TestFixture]
    public class ProcedureDesignerTests
    {
        private ServiceContainer services;
        private Program program;

        [SetUp]
        public void Setup()
        {
            this.services = new ServiceContainer();
            this.program = new Program
            {
                Architecture = new FakeArchitecture()
            };
        }

        private void Given_Service<T>(T svc)
        {
            services.AddService<T>(svc);
        }

        [Test]
        public void ProcDesigner_DefaultAction_ShowProcedure()
        {
            var proc = new Procedure(program.Architecture, "foo", Address.Ptr32(0x001100000),  new Frame(PrimitiveType.Ptr32));
            var des = new ProcedureDesigner(program, proc, null, proc.EntryAddress, false);
            des.Services = services;
            var codeSvc = new Mock<ICodeViewerService>();
            Given_Service<ICodeViewerService>(codeSvc.Object);
            codeSvc.Setup(c => c.DisplayProcedure(program, proc, true)).Verifiable();

            des.DoDefaultAction();

            codeSvc.VerifyAll();
        }
    }
}
