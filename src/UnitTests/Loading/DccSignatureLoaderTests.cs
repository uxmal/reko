#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Services;
using Reko.Loading;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Loading
{
    [TestFixture]
    public class DccSignatureLoaderTests
    {
        private MockRepository mr;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.sc = new ServiceContainer();
            var diagSvc = mr.Stub<IDiagnosticsService>();
            sc.AddService<IDiagnosticsService>(diagSvc);
        }

        /*
         * Use this scaffolding  during development testing only, 
         * it will be converted to a proper unit test once all is 
         * working.
         *
        [Test]
        public void Dccs_Load()
        {
            mr.ReplayAll();

            var dccsl = new DccSignatureLoader();
            var file = @"d:\dev\uxmal\reko\master\external\dccb3c.sig";
            var bytes = File.ReadAllBytes(file);
            dccsl.SetupLibCheck(sc, file, bytes);
        }
        */
    }
}
