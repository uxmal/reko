#region License
/* 
 * Copyright (C) 2017-2022 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Libraries.Microchip;
using NUnit.Framework;
using Reko.Arch.MicrochipPIC.Common;
using Reko.Core;
using Reko.Core.Rtl;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Design;
using Reko.Core.Memory;

namespace Reko.UnitTests.Arch.Microchip.Common
{
    public class PICRewriterTestsBase : RewriterTestBase
    {
        protected IPICProcessorModel picModel;
        protected PICArchitecture arch;
        protected Address baseAddr = PICProgAddress.Ptr(0x200);
        protected ByteMemoryArea bmem;

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => baseAddr;

        protected void Given_UInt16s(uint[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[]
            {
                (byte) w,
                (byte) (w >> 8),
            }).ToArray();
            this.bmem = new ByteMemoryArea(LoadAddress, bytes);
        }

        protected string _fmtBinary(uint[] words)
        {
            string sPIC = $"{arch.PICDescriptor.PICName}/{PICMemoryDescriptor.ExecMode}";
            if (words.Length >= 1)
            {
                sPIC += "[" + string.Join("-", words.Select(w => w.ToString("X4"))) + "] ";
            }
            return $"{sPIC}";
        }

        protected void SetPICModel(string picName, PICExecMode mode = PICExecMode.Traditional)
        {
            arch = new PICArchitecture(new ServiceContainer(), "pic", new Dictionary<string, object>()) { Options = new PICArchitectureOptions(picName, mode) };
            picModel = arch.ProcessorModel;
            arch.CreatePICProcessorModel();
            PICMemoryDescriptor.ExecMode = mode;
        }

        protected void AssertCode(string mesg, params string[] expected)
        {
            int i = 0;
            var frame = Architecture.CreateFrame();
            var host = CreateRewriterHost();
            var rewriter = GetRtlStream(bmem, frame, host).GetEnumerator();
            while (i < expected.Length && rewriter.MoveNext())
            {
                Assert.AreEqual(expected[i], $"{i}|{RtlInstruction.FormatClass(rewriter.Current.Class)}|{rewriter.Current}", mesg);
                ++i;
                var ee = rewriter.Current.Instructions.OfType<RtlInstruction>().GetEnumerator();
                while (i < expected.Length && ee.MoveNext())
                {
                    Assert.AreEqual(expected[i], $"{i}|{RtlInstruction.FormatClass(ee.Current.Class)}|{ee.Current}", mesg);
                    ++i;
                }
            }
            Assert.AreEqual(expected.Length, i, mesg + " Expected " + expected.Length + " instructions.");
            Assert.IsFalse(rewriter.MoveNext(), mesg + " More instructions were emitted than were expected. ");
        }

        protected uint[] Words(params uint[] words)
        {
            if (words.Length > 0)
                return words.ToArray();
            return null;
        }

        public void ExecTest(uint[] words, params string[] expected)
        {
            Given_UInt16s(words);
            AssertCode(_fmtBinary(words), expected);
        }
    }
}
