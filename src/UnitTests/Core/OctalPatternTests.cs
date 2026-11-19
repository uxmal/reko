#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core;

[TestFixture]
public class OctalPatternTests
{
    [Test]
    public void OctalPattern_ToUInt64s()
    {
        ulong[] words = OctalPattern.ToUInt64s("12345670123456701234", 20);
        Assert.AreEqual(1, words.Length);
        Assert.AreEqual("12345670123456701234", Convert.ToString((long) words[0], 8).PadLeft(20, '0'));
    }

    [Test]
    public void OctalPattern_ToUInt64s_Short()
    {
        ulong[] words = OctalPattern.ToUInt64s("234", 3);
        Assert.AreEqual(1, words.Length);
        Assert.AreEqual("00000000000000000234", Convert.ToString((long) words[0], 8).PadLeft(20, '0'));
    }
}
