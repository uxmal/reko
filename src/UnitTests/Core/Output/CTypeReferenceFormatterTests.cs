#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Output;
using Reko.Core.Types;
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
        private Mock<IPlatform> platform;
        private Mock<Formatter> formatter;
        private CTypeReferenceFormatter ctrf;

        [SetUp]
        public void Setup()
        {
            this.formatter = new Mock<Formatter>();
        }

        private void Given_Msdos_ish_platform()
        {
            this.platform = new Mock<IPlatform>();
            platform.Setup(p => p.GetPrimitiveTypeName(PrimitiveType.Int16, "C")).Returns("int");
            platform.Setup(p => p.GetPrimitiveTypeName(PrimitiveType.Int32, "C")).Returns("long");
        }

        private void Given_CTypeReferenceFormatter()
        {
            Debug.Assert(platform is not null);
            Debug.Assert(formatter is not null);
            this.ctrf = new CTypeReferenceFormatter(platform.Object, formatter.Object);
        }

        [Test]
        public void Ctrf_Render_MSDOS_C_Code_int16()
        {
            Given_Msdos_ish_platform();
            Given_CTypeReferenceFormatter();
            formatter.Setup(f => f.WriteKeyword("int")).Verifiable();
            formatter.Setup(f => f.Write("")).Verifiable();

            ctrf.WriteTypeReference(PrimitiveType.Int16);

            formatter.Verify();
        }

        [Test]
        public void Ctrf_Render_MSDOS_C_Code_int32()
        {
            Given_Msdos_ish_platform();
            Given_CTypeReferenceFormatter();
            formatter.Setup(f => f.WriteKeyword("long")).Verifiable();
            formatter.Setup(f => f.Write("")).Verifiable();

            ctrf.WriteTypeReference(PrimitiveType.Int32);

            formatter.Verify();
        }
    }
}
