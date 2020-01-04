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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Configuration
{
    [TestFixture]
    public class OperatingEnvironmentElementTests
    {
        [Test]
        public void Oee_LoadPrologPattern_WithMask()
        {
            var sPattern = new BytePattern_v1
            {
                Bytes = "55 32 12",
                Mask = "FF C0 0F",
            };
            var element = new PlatformDefinition();
            var pattern = element.LoadMaskedPattern(sPattern);
            Assert.AreEqual(new byte[] { 0x55, 0x32, 0x12 }, pattern.Bytes);
            Assert.AreEqual(new byte[] { 0xFF, 0xC0, 0x0F }, pattern.Mask);
        }

        [Test]
        public void Oee_LoadPrologPattern_WithoutMask()
        {
            var sPattern = new BytePattern_v1
            {
                Bytes = "55 3? ?2",
            };
            var element = new PlatformDefinition();
            var pattern = element.LoadMaskedPattern(sPattern);
            Assert.AreEqual(new byte[] { 0x55, 0x30, 0x02 }, pattern.Bytes);
            Assert.AreEqual(new byte[] { 0xFF, 0xF0, 0x0F }, pattern.Mask);
        }

        [Test]
        public void Oee_LoadPlatform_NoHeuristics()
        {
            var element = new PlatformDefinition
            {
            };
            var platform = new DefaultPlatform(null, null);
            element.LoadSettingsFromConfiguration(null, platform);
            Assert.IsNotNull(platform.Heuristics);
            Assert.IsNotNull(platform.Heuristics.ProcedurePrologs);
            Assert.AreEqual(0, platform.Heuristics.ProcedurePrologs.Length);
        }
    }
}
