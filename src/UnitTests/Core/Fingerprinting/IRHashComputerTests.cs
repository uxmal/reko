#region License
/* 
 * Copyright (C) 1999-2026 John K�ll�n.
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
using Reko.Core.FingerPrinting;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Fingerprinting;

[TestFixture]
public class IRHashComputerTests
{
    [Test]
    public void Phc_EmptyProcedure()
    {
        var program = new Program();
        var m = new ProcedureBuilder();

        var phc = new IRHashComputer();
        var fp = phc.ComputeFeature(m.Procedure, program);

        Assert.AreEqual(0, fp.Value);
    }

    [Test]
    public void Phc_ReturnNothing()
    {
        var program = new Program();
        var m = new ProcedureBuilder();
        m.Return();

        var phc = new IRHashComputer();
        var fp = phc.ComputeFeature(m.Procedure, program);

        Assert.AreEqual(114, fp.Value);
    }
}
