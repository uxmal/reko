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
 * You should have received a copy of the GNU General Public License,
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using NUnit.Framework;
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Environments.PlayStation;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Environments.Ps2
{
    /// <summary>
    /// Tests for the PlayStation 2 environment: EE kernel system services,
    /// syscall recognition and ELF image auto-detection.
    /// </summary>
    [TestFixture]
    public class Ps2PlatformTests
    {
        private readonly MipsLe32Architecture arch = new(
            new ServiceContainer(),
            "mips-le-32",
            new()
            {
                { ProcessorOption.InstructionSet, "ps2ee" },
                { ProcessorOption.Endianness, "le" },
                { ProcessorOption.WordSize, 32 }
            });
        private Ps2Platform platform;

        [SetUp]
        public void Setup()
        {
            this.platform = new Ps2Platform(new ServiceContainer(), arch);
        }

        [Test]
        public void Ps2Svc_ReferThreadStatus()
        {
            var svc = platform.FindService(0x0030, null, null);
            Assert.IsNotNull(svc);
            Assert.AreEqual("ReferThreadStatus", svc!.Name);
            Assert.AreEqual(2, svc.Signature!.Parameters!.Length);
        }

        [Test]
        public void Ps2Svc_WakeupThread_signature()
        {
            var svc = platform.FindService(0x0033, null, null);
            Assert.IsNotNull(svc);
            Assert.AreEqual("WakeupThread", svc!.Name);
            Assert.AreEqual(1, svc.Signature!.Parameters!.Length);
            Assert.AreEqual("a0", svc.Signature.Parameters[0].Name);
        }

        [Test]
        public void Ps2Svc_LoadExecPS2_terminates()
        {
            var svc = platform.FindService(0x0001, null, null);
            Assert.IsNotNull(svc);
            Assert.AreEqual(1, svc.Signature.Parameters.Length);
        }

        [Test]
        public void Ps2Svc_UnknownVector_returns_null()
        {
            Assert.IsNull(platform.FindService(0x1234, null, null));
        }

        ////-----------------------------------------------------------------
        ///// SYSCALL rewriting and recognition.

        private List<RtlInstructionCluster> Rewrite(params uint[] words)
        {
            var mem = new ByteMemoryArea(Address.Ptr32(0x00100000), new byte[256]);
            var writer = arch.CreateImageWriter(mem, mem.BaseAddress);
            uint offset = 0;
            foreach (var w in words)
            {
                writer.WriteUInt32(offset, w);
                offset += 4;
            }
            var frame = arch.CreateFrame();
            var host = new NullRewriterHost();
            var state = arch.CreateProcessorState();
            var clusters = arch.CreateRewriter(
                arch.CreateImageReader(mem, 0),
                state,
                frame,
                host)
                .Take(words.Length)
                .ToList();
            return clusters;
        }

        [Test]
        public void Ps2Svc_ResolvesSyscallFromRtl()
        {
            var clusters = Rewrite(W(0, funct: 0x0C, rest: 0x44 << 6));
            var call = clusters
                .SelectMany(c => c.Instructions)
                .OfType<RtlSideEffect>()
                .First();
            var svc = platform.FindService(call, null, null);
            Assert.IsNotNull(svc);
            Assert.AreEqual("WaitSema", svc!.Name);
            Assert.IsNotNull(svc.Signature);
        }

        [Test]
        public void Ps2Svc_UnrelatedRtlCall_is_not_resolved()
        {
            var m = new RtlEmitter(new List<RtlInstruction>());
            var rtlCall = new RtlCall(m.Word32(0x00100100u), 4, InstrClass.Call);
            Assert.IsNull(platform.FindService(rtlCall, null, null));
        }

        private static uint W(int opc, int rs = 0, int rt = 0, int rd = 0, int sa = 0, int funct = 0, int rest = 0)
        {
            return (uint) ((opc << 26) | (rs << 21) | (rt << 16) | (rd << 11) | (sa << 6) | funct | rest);
        }
    }
}
