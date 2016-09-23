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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Types;
using Reko.Environments.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.Windows
{
    [TestFixture]
    public class WineSpecFileLoaderTests
    {
        private static string nl = Environment.NewLine;

        private IPlatform platform;
        private WineSpecFileLoader wsfl;

        private void Given_WineSpecLoader_16(string filename, string contents)
        {
            this.platform = new Win16Platform(null, new X86ArchitectureProtected16());
            wsfl = new WineSpecFileLoader(null, filename, Encoding.ASCII.GetBytes(contents));
        }

        private void Given_WineSpecLoader_32(string filename, string contents)
        {
            this.platform = new Win32Platform(null, new X86ArchitectureFlat32());
            wsfl = new WineSpecFileLoader(null, filename, Encoding.ASCII.GetBytes(contents));
        }

        [Test]
        public void Wsfl_Comment()
        {
            Given_WineSpecLoader_16("foo.spec",
                " # comment");
            var lib = wsfl.Load(platform, new TypeLibrary());
            Assert.AreEqual(0, lib.Modules.Count);
        }

        [Test]
        public void Wsfl_Line()
        {
            Given_WineSpecLoader_16("foo.spec",
                " 624 pascal SetFastQueue(long long) SetFastQueue16\n");
            var lib = wsfl.Load(platform, new TypeLibrary());
            var mod = lib.Modules["FOO.DLL"];
            Assert.AreEqual(1, mod.ServicesByVector.Count);
            Assert.AreEqual(0, mod.ServicesByName.Count);
            var svc = mod.ServicesByVector[624];
            Assert.AreEqual("SetFastQueue", svc.Name);
            Assert.AreEqual(
                "void SetFastQueue(Stack word32 dwArg08, Stack word32 dwArg04)" + nl + "// stackDelta: 12; fpuStackDelta: 0; fpuMaxParam: -1" + nl + "",
                svc.Signature.ToString(svc.Name, FunctionType.EmitFlags.AllDetails));
        }

        [Test]
        public void Wsfl_ThreeLines()
        {
            Given_WineSpecLoader_16("foo.spec",
                " 2   pascal -ret16 ExitKernel() ExitKernel16\n" +
                "3    pascal GetVersion() GetVersion16\n" +
                "4   pascal -ret16 LocalInit(word word word) LocalInit16\n");
            var lib = wsfl.Load(platform, new TypeLibrary());
            var mod = lib.Modules["FOO.DLL"];
            Assert.AreEqual(3, mod.ServicesByVector.Count);
            Assert.AreEqual(
                "void ExitKernel()" + nl + "// stackDelta: 4; fpuStackDelta: 0; fpuMaxParam: -1" + nl + "",
                mod.ServicesByVector[2].Signature.ToString("ExitKernel", FunctionType.EmitFlags.AllDetails));
            Assert.AreEqual(
                "void GetVersion()" + nl + "// stackDelta: 4; fpuStackDelta: 0; fpuMaxParam: -1" + nl + "",
                mod.ServicesByVector[3].Signature.ToString("GetVersion", FunctionType.EmitFlags.AllDetails));
            Assert.AreEqual(
                "void LocalInit(Stack word16 wArg08, Stack word16 wArg06, Stack word16 wArg04)" + nl + "// stackDelta: 10; fpuStackDelta: 0; fpuMaxParam: -1" + nl + "",
                mod.ServicesByVector[4].Signature.ToString("LocalInit", FunctionType.EmitFlags.AllDetails));
        }

        [Test]
        public void Wsfl_ParseOptions()
        {
            Given_WineSpecLoader_16("foo.spec",
                " @ stdcall -arch=win32 -norelay SMapLS_IP_EBP_36()" + nl + "");
            var lib = wsfl.Load(platform, new TypeLibrary());
            var mod = lib.Modules["FOO.DLL"];
            Assert.AreEqual(
                "void SMapLS_IP_EBP_36()",
                mod.ServicesByName["SMapLS_IP_EBP_36"].Signature.ToString("SMapLS_IP_EBP_36"));
        }

        [Test]
        public void Wsfl_PointerParameter_32()
        {
            Given_WineSpecLoader_32("foo.spec",
                "115 stdcall WSAStartup(long ptr)\n");
            var lib = wsfl.Load(platform, new TypeLibrary());
            var mod = lib.Modules["FOO.DLL"];
            Assert.AreEqual(
                "void WSAStartup(Stack word32 dwArg04, Stack ptr32 ptrArg08)",
                mod.ServicesByVector[115].Signature.ToString("WSAStartup", FunctionType.EmitFlags.ArgumentKind));
        }

        [Test]
        public void Wsfl_Handle_atsign()
        {
            Given_WineSpecLoader_32("foo.spec",
                "@ stdcall WSCEnableNSProvider(ptr long)\n");
            var lib = wsfl.Load(platform, new TypeLibrary());
            var mod = lib.Modules["FOO.DLL"];
            Assert.AreEqual(
                "void WSCEnableNSProvider(Stack ptr32 ptrArg04, Stack word32 dwArg08)",
                mod.ServicesByName["WSCEnableNSProvider"].Signature.ToString("WSCEnableNSProvider", FunctionType.EmitFlags.ArgumentKind));
        }

        [Test]
        public void Wsfl_cdecl()
        {
            Given_WineSpecLoader_32("msvcrt.spec",
                " @ cdecl fgets(ptr long ptr) MSVCRT_fgets\n");
            var lib = wsfl.Load(platform, new TypeLibrary());
            var mod = lib.Modules["MSVCRT.DLL"];
            Assert.AreEqual(
                "void fgets(Stack ptr32 ptrArg04, Stack word32 dwArg08, Stack ptr32 ptrArg0C)" + nl +
                "// stackDelta: 4; fpuStackDelta: 0; fpuMaxParam: -1" + nl,
                mod.ServicesByName["fgets"].Signature.ToString("fgets", FunctionType.EmitFlags.AllDetails));
        }
    }
}
