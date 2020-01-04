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
using Reko.Core.CLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.CLanguage
{
    [TestFixture]
    public class EnumEvaluatorTests
    {
        private EnumEvaluator enev;
        private Dictionary<string, int> constants;

        [SetUp]
        public void Setup()
        {
            var platform = new Mock<IPlatform>();
            platform.Setup(p => p.GetByteSizeFromCBasicType(CBasicType.Int)).Returns(4);
            constants = new Dictionary<string, int>();
            enev = new EnumEvaluator(new CConstantEvaluator(platform.Object, constants));
        }

        [Test]
        public void ENEV_DefaultValue()
        {
            var value = enev.GetValue(null);
            Assert.AreEqual(0, value);
        }

        [Test]
        public void ENEV_Next()
        {
            var value = enev.GetValue(null);
            Assert.AreEqual(0, value);
            value = enev.GetValue(null);
            Assert.AreEqual(1, value);
        }

        [Test]
        public void ENEV_SpecifiedValue()
        {
            var value = enev.GetValue(null);
            Assert.AreEqual(0, value);
            value = enev.GetValue(new ConstExp { Const = 42 });
            Assert.AreEqual(42, value);
        }

        [Test]
        public void ENEV_IncAfterSpecifiedValue()
        {
            var value = enev.GetValue(new ConstExp { Const = 42 });
            Assert.AreEqual(42, value);
            value = enev.GetValue(null);
            Assert.AreEqual(43, value);
        }
    }
}
