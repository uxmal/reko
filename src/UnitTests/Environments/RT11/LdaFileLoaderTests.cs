#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Environments.RT11;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.RT11
{
    [TestFixture]
    public class LdaFileLoaderTests
    {
        private LdaFileLoader ldaLdr;

        private void Given_LdaFile(params byte [] bytes)
        {
            var services = MockRepository.GenerateStrictMock<IServiceProvider>();
            this.ldaLdr = new LdaFileLoader(services, "foo.lda", bytes);
        }

        [Test]
        public void LdaLdr_LoadEmpty()
        {
            Given_LdaFile(0x00, 0x00);

            var program = ldaLdr.Load(Address.Ptr16(0));
            Assert.AreEqual(0, program.SegmentMap.Segments.Count);
        }

    }
}
