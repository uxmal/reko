#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Reko.Core.Serialization;
using Reko.Core.Configuration;

namespace Reko.UnitTests.Arch
{
    public abstract class ArchTestBase 
    {
        public abstract IProcessorArchitecture Architecture { get; }

        public abstract Address LoadAddress { get; }

        protected virtual IEnumerable<RtlInstructionCluster>GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            yield break;
        }

        public class RewriterHost : IRewriterHost
        {
            private IProcessorArchitecture arch;

            public RewriterHost(IProcessorArchitecture arch)
            {
                this.arch = arch;
            }

            public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity)
            {
                return new PseudoProcedure(name, returnType, arity);
            }

            public Expression GetImport(Address addrThunk, Address addrInstr)
            {
                return null;
            }

            public ExternalProcedure GetImportedProcedure(IProcessorArchitecture arch, Address addrThunk, Address addrInstr)
            {
                throw new NotImplementedException();
            }

            public Expression PseudoProcedure(string name, DataType returnType, params Expression[] args)
            {
                return PseudoProcedure(name, new ProcedureCharacteristics(), returnType, args);
            }

            public Expression PseudoProcedure(string name, ProcedureCharacteristics c, DataType returnType, params Expression[] args)
            {
                var ppp = new PseudoProcedure(name, returnType, args.Length);
                return new Application(
                    new ProcedureConstant(arch.PointerType, ppp),
                    returnType,
                    args);
            }

            public virtual IProcessorArchitecture GetArchitecture(string archLabel)
            {
                throw new NotImplementedException();
            }

            public ExternalProcedure GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk)
            {
                throw new NotImplementedException();
            }

            public void Error(Address address, string message, params object[] args)
            {
                throw new Exception(string.Format("{0}: {1}", address, 
                    string.Format(message, args)));
            }

            public void Warn(Address address, string format, params object[] args)
            {
                throw new Exception(string.Format("{0}: {1}", address,
                    string.Format(format, args)));
            }
        }

        protected virtual IRewriterHost CreateRewriterHost()
        {
            return new RewriterHost(Architecture);
        }

        protected void AssertCode(params string[] expected)
        {
            int i = 0;
            var frame = Architecture.CreateFrame();
            var host = CreateRewriterHost();
            var rewriter = GetInstructionStream(frame, host).GetEnumerator();
            while (i < expected.Length && rewriter.MoveNext())
            {
                Assert.AreEqual(expected[i], string.Format("{0}|{1}|{2}", i, RtlInstruction.FormatClass(rewriter.Current.Class), rewriter.Current));
                ++i;
                var ee = rewriter.Current.Instructions.OfType<RtlInstruction>().GetEnumerator();
                while (i < expected.Length && ee.MoveNext())
                {
                    Assert.AreEqual(expected[i], string.Format("{0}|{1}|{2}", i, RtlInstruction.FormatClass(ee.Current.Class), ee.Current));
                    ++i;
                }
            }
            Assert.AreEqual(expected.Length, i, "Expected " + expected.Length + " instructions.");
            Assert.IsFalse(rewriter.MoveNext(), "More instructions were emitted than were expected.");
        }

        public uint ParseBitPattern(string bitPattern)
        {
            int cBits = 0;
            uint instr = 0;
            for (int i = 0; i < bitPattern.Length; ++i)
            {
                switch (bitPattern[i])
                {
                case '0':
                case '1':
                    instr = (instr << 1) | (uint) (bitPattern[i] - '0');
                    ++cBits;
                    break;
                }
            }
            if (cBits == 0 || cBits % Architecture.InstructionBitSize != 0)
                throw new ArgumentException(
                    string.Format("Bit pattern didn't contain exactly a multiple of {0} binary digits, but {1}.", Architecture.InstructionBitSize, cBits),
                    "bitPattern");
            return instr;
        }

        public byte[] ParseHexPattern(string hexPattern)
        {
            return PlatformDefinition.LoadHexBytes(hexPattern)
                .ToArray();
        }

    }
}
