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
using Reko.Core.NativeInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DEBUG || TRAVIS_RELEASE

namespace Reko.Tools.HdrGen
{
    [TestFixture]
    public class HeaderGeneratorTests
    {
        private StringWriter sw;

        [SetUp]
        public void Setup()
        {
            this.sw = new StringWriter();
        }

        [Test]
        public void Hdrgen_Enum()
        {
            var hdrgen = new HeaderGenerator(typeof(Address).Assembly, sw);
            hdrgen.WriteEnumDefinition(typeof(Reko.Core.AccessMode));
            var sExp =
            #region Expected
@"enum class AccessMode
{
    Execute = 1,
    Write = 2,
    Read = 4,
    ReadExecute = 5,
    ReadWrite = 6,
    ReadWriteExecute = 7,
};
";
            #endregion
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void Hdrgen_Guid()
        {
            var sExp =
            #region Expected
@"// {12506D0F-1C67-4828-9601-96F8ED4D162D}
DEFINE_GUID(IID_INativeRewriter, 0x12506D0F,0x1C67,0x4828,0x96,0x01,0x96,0xF8,0xED,0x4D,0x16,0x2D);
";
            #endregion
            var hdrgen = new HeaderGenerator(typeof(Address).Assembly, sw);
            hdrgen.WriteGuidDefinition("INativeRewriter", "12506D0F-1C67-4828-9601-96F8ED4D162D");
            Debug.Print(sw.ToString());
            Assert.AreEqual(sExp, sw.ToString());
        }

        [Test]
        public void Hdrgen_Interface()
        {
            var hdrgen = new HeaderGenerator(typeof(Address).Assembly, sw);
            hdrgen.WriteInterfaceDefinition(typeof(INativeRewriter));
            var sExp =
            #region Expected
@"// {12506D0F-1C67-4828-9601-96F8ED4D162D}
DEFINE_GUID(IID_INativeRewriter, 0x12506D0F,0x1C67,0x4828,0x96,0x01,0x96,0xF8,0xED,0x4D,0x16,0x2D);
class INativeRewriter : public IUnknown
{
public:
    virtual int32_t STDAPICALLTYPE Next() = 0;
    virtual int32_t STDAPICALLTYPE GetCount() = 0;
};
";
            #endregion
            Debug.Print(sw.ToString());
            Assert.AreEqual(sExp, sw.ToString());
        }

        public void Hdrgen_rekodoth()
        {
            var hdrgen = new HeaderGenerator(typeof(Address).Assembly, sw);
            hdrgen.Execute();
            Debug.WriteLine(sw);
        }

        [Test]
        public void Hdrgen_OutParameter()
        {
            var hdrgen = new HeaderGenerator(typeof(Address).Assembly, sw);
            var method = typeof(INativeArchitecture).GetMethod("GetAllRegisters");
            hdrgen.WriteInterfaceMethod(method);
            var sExp =
            #region Expected
@"    virtual HRESULT STDAPICALLTYPE GetAllRegisters(int32_t registerType, int32_t* n, void ** aregs) = 0;
";
            #endregion
            Assert.AreEqual(sExp, sw.ToString());
        }
    }
}

#endif
