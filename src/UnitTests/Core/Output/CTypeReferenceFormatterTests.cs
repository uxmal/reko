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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Output;
using Reko.Core.Types;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Output
{
    [TestFixture]
    public class CTypeReferenceFormatterTests
    {
        private MockRepository mr;
        private IPlatform platform;
        private Formatter formatter;
        private CTypeReferenceFormatter ctrf;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.formatter = mr.StrictMock<Formatter>();
        }

        private void Given_Msdos_ish_platform()
        {
            this.platform = mr.Stub<IPlatform>();
            platform.Stub(p => p.GetPrimitiveTypeName(PrimitiveType.Int16, "C")).Return("int");
            platform.Stub(p => p.GetPrimitiveTypeName(PrimitiveType.Int32, "C")).Return("long");
        }

        private void Given_CTypeReferenceFormatter()
        {
            Debug.Assert(platform != null);
            Debug.Assert(formatter != null);
            this.ctrf = new CTypeReferenceFormatter(platform, formatter);
        }

        [Test]
        public void Ctrf_Render_MSDOS_C_Code_int16()
        {
            Given_Msdos_ish_platform();
            Given_CTypeReferenceFormatter();
            formatter.Expect(f => f.WriteKeyword("int"));
            mr.ReplayAll();

            ctrf.WriteTypeReference(PrimitiveType.Int16);
            mr.VerifyAll();
        }

        [Test]
        public void Ctrf_Render_MSDOS_C_Code_int32()
        {
            Given_Msdos_ish_platform();
            Given_CTypeReferenceFormatter();
            formatter.Expect(f => f.WriteKeyword("long"));
            mr.ReplayAll();

            ctrf.WriteTypeReference(PrimitiveType.Int32);
            mr.VerifyAll();
        }
    }
}
