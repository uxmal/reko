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
using Reko.Core.Services;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Loading
{
    [TestFixture]
    public class DccSignatureLoaderTests
    {
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            this.sc = new ServiceContainer();
            var diagSvc = new Mock<IDiagnosticsService>();
            sc.AddService<IDiagnosticsService>(diagSvc.Object);
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
