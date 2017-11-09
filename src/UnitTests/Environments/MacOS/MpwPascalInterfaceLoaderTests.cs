#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Arch.M68k;
using Reko.Core;
using Reko.Environments.MacOS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Environments.MacOS
{
    [TestFixture]
    public class MpwPascalInterfaceLoaderTests
    {
        [Test]
        public void Mpwl_Regress()
        {
            var filename = @"C:\dev\uxmal\reko\master\src\Environments\MacOS\Mac MPW Interfaces 1991 PASCAL.pas";
            var bytes = File.ReadAllBytes(filename);
            var mpwl = new MpwPascalInterfaceLoader(null, filename, bytes);
            var platform = new MacOSClassic(null, new M68kArchitecture());
            var tlib = new TypeLibrary();
            var tlibNew =  mpwl.Load(platform, tlib);
        }

        [Test]
        public void Mpwl_PascalService()
        {
            var bytes = Encoding.ASCII.GetBytes(
@"UNIT test; INTERFACE 
TYPE
Point = RECORD
    x: INTEGER;
    y: INTEGER;
END;
EventRecord = RECORD
    what: INTEGER;
    message: LONGINT;
    when: LONGINT;
    where: Point;
    modifiers: INTEGER;
END; 
FUNCTION GetNextEvent(eventMask: INTEGER;VAR theEvent: EventRecord): BOOLEAN; INLINE $A970;
END.");
            var mpwl = new MpwPascalInterfaceLoader(null, "foo.pas", bytes);
            var platform = new MacOSClassic(null, new M68kArchitecture());
            var tlib = new TypeLibrary();
            var tlibNew = mpwl.Load(platform, tlib);

            var svc = tlibNew.Modules.Values.First().ServicesByVector[0xA970].First(s => s.SyscallInfo.Matches(0xA970, null));
            Assert.AreEqual("Register BOOLEAN GetNextEvent(Stack int16 eventMask, Stack (ref EventRecord) theEvent)", svc.Signature.ToString(svc.Name));
        }
    }
}
