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
using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Services;
using Reko.Environments.MacOS.Classic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.MacOS.Classic
{
    [TestFixture]
    public class MpwPascalInterfaceLoaderTests
    {
        private MacOSClassic platform;
        private TypeLibrary tlib;
        private Mock<IDiagnosticsService> diagSvc;
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            this.platform = new MacOSClassic(null, new M68kArchitecture("m68k"));
            this.tlib = new TypeLibrary();
            this.diagSvc = new Moq.Mock<IDiagnosticsService>();
            this.sc = new ServiceContainer();
            this.sc.AddService(diagSvc.Object);
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
            var mpwl = new MpwPascalInterfaceLoader(sc, "foo.pas", bytes);
            var tlibNew = mpwl.Load(platform, tlib);

            var svc = tlibNew.Modules.Values.First().ServicesByVector[0xA970].First(s => s.SyscallInfo.Matches(0xA970, null));
            Assert.AreEqual("Stack bool GetNextEvent(Stack int16 eventMask, Stack (ref EventRecord) theEvent)", svc.Signature.ToString(svc.Name));
        }

        [Test]
        public void Mpwl_GetResource()
        {
            var image = Encoding.ASCII.GetBytes(
@"UNIT test; INTERFACE
TYPE ResType = PACKED ARRAY[1..4] OF CHAR;
FUNCTION GetResource(theType: ResType;theID: INTEGER): Handle;
    INLINE $A9A0;
END.");
            var mpwl = new MpwPascalInterfaceLoader(sc, "foo.pas", image);
            var tlibNew = mpwl.Load(platform, tlib);

            var svc = tlibNew.Modules.Values.First().ServicesByVector[0xA9A0].First(s => s.SyscallInfo.Matches(0xA9A0, null));
            Assert.AreEqual("Stack Handle GetResource(Stack ResType theType, Stack int16 theID)", svc.Signature.ToString(svc.Name));
            Assert.AreEqual(4, svc.Signature.Parameters[0].DataType.Size);
        }
    }
}
