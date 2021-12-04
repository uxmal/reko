#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Types;
using Reko.Environments.AmigaOS;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.AmigaOS
{
    [TestFixture]
    public class AmigaHeaderLoaderTests
    {
        [Test]
        public void Ahl_ParseSimpleDef()
        {
            var sc = new ServiceContainer();
            var arch = new M68kArchitecture(sc, "m68k", new Dictionary<string, object>());
            var platform = new AmigaOSPlatform(sc, arch);
            var ahl = new AmigaHeaderLoader(sc, ImageLocation.FromUri("file:x"), Encoding.UTF8.GetBytes(
                "[[reko::amiga_function_vector(ExecLibrary, -432)]] [[reko::returns(register,\"A0\")]] " +
                "void * FlobDevice([[reko::arg(register, \"A1\")]] struct Device * device);"));
            ahl.Load(platform, new TypeLibrary());
            var svc = ahl.SystemServices[-432];
            Assert.AreEqual("a0", svc.Signature.ReturnValue.Storage.ToString());
            Assert.AreEqual("a1", svc.Signature.Parameters[0].Storage.ToString());
        }

        [Test]
        public void Ahl_ParseSimpleDef_voidfn()
        {
            var sc = new ServiceContainer();
            var arch = new M68kArchitecture(sc, "m68k", new Dictionary<string, object>());
            var platform = new AmigaOSPlatform(sc, arch);
            var ahl = new AmigaHeaderLoader(sc, ImageLocation.FromUri("file:test"), Encoding.UTF8.GetBytes(
                "[[reko::amiga_function_vector(ExecLibrary, -432)]]  " +
                "void  FrabDevice([[reko::arg(register, \"A1\")]] struct Device * device);"));
            var Q = ahl.Load(platform, new TypeLibrary());
            var svc = ahl.SystemServices[-432];
            Assert.IsInstanceOf<VoidType>(svc.Signature.ReturnValue.DataType);
        }
    }
}
