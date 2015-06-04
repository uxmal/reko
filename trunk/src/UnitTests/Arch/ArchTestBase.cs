#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Rtl;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch
{
    public abstract class ArchTestBase 
    {
        public abstract IProcessorArchitecture Architecture { get; }

        public abstract Address LoadAddress { get; }

        protected virtual IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            yield break;
        }

        private class RewriterHost : IRewriterHost
        {
            public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity)
            {
                return new PseudoProcedure(name, returnType, arity);
            }

            public ExternalProcedure GetImportedProcedure(Address addrThunk, Address addrInstr)
            {
                throw new NotImplementedException();
            }

            public Expression PseudoProcedure(string name, DataType returnType, params Expression[] args)
            {
                throw new NotImplementedException();
            }

            public ExternalProcedure GetInterceptedCall(Address addrImportThunk)
            {
                throw new NotImplementedException();
            }


            public void Error(Address address, string message)
            {
                throw new NotImplementedException();
            }
        }

        protected virtual IRewriterHost CreateRewriterHost()
        {
            return new RewriterHost();
        }

        protected void AssertCode(params string[] expected)
        {
            int i = 0;
            var frame = Architecture.CreateFrame();
            var host = CreateRewriterHost();
            var rewriter = GetInstructionStream(frame, host).GetEnumerator();
            while (i < expected.Length && rewriter.MoveNext())
            {
                Assert.AreEqual(expected[i], string.Format("{0}|{1}", i, rewriter.Current));
                ++i;
                var ee = rewriter.Current.Instructions.GetEnumerator();
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
            if (cBits != Architecture.InstructionBitSize)
                throw new ArgumentException(
                    string.Format("Bit pattern didn't contain exactly {0} binary digits, but {1}.", Architecture.InstructionBitSize, cBits),
                    "bitPattern");
            return instr;
        }
    }
}
