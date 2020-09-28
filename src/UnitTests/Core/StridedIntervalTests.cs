#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
 .
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
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core
{
    [TestFixture]
    public class StridedIntervalTests
    {
        [Test]
        public void SI_Empty()
        {
            Assert.AreEqual("\x27D8", StridedInterval.Empty.ToString());
        }

        [Test]
        public void SI_Constant()
        {
            Assert.AreEqual("0[9,9]", StridedInterval.Constant(Constant.Int32(9)).ToString());
        }

        [Test]
        public void SI_Range()
        {
            Assert.AreEqual("10[-28,28]", StridedInterval.Create(16, -40, 40).ToString());
        }
    }
}
