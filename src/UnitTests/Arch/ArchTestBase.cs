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
using Reko.Core.Memory;

namespace Reko.UnitTests.Arch
{
    public abstract class ArchTestBase 
    {
        public abstract IProcessorArchitecture Architecture { get; }

        public abstract Address LoadAddress { get; }

        /// <summary>
        /// Generates a stream of <see cref="RtlInstructionCluster"/>s. 
        /// </summary>
        /// <remarks>
        /// The default implementation should work for most architectures, but you have the option
        /// of overriding for specialized testing.
        /// </remarks>
        protected virtual IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var state = Architecture.CreateProcessorState();
            var rdr = Architecture.Endianness.CreateImageReader(mem, 0);
            return Architecture.CreateRewriter(rdr, state, binder, host);
        }

        public class RewriterHost : IRewriterHost
        {
            private readonly IProcessorArchitecture arch;
            private readonly Dictionary<Address, ImportReference> importThunks;
            private readonly Dictionary<string, IntrinsicProcedure> intrinsics;

            public RewriterHost(IProcessorArchitecture arch) : this(arch, new Dictionary<Address, ImportReference>())
            {
            }

            public RewriterHost(IProcessorArchitecture arch, Dictionary<Address, ImportReference> imports)
            {
                this.arch = arch;
                this.importThunks = imports;
                this.intrinsics = new Dictionary<string, IntrinsicProcedure>();
            }

            public IntrinsicProcedure EnsureIntrinsic(string name, bool isIdempotent, DataType returnType, int arity)
            {
                if (intrinsics.TryGetValue(name, out var p))
                    return p;
                p = new IntrinsicProcedure(name, isIdempotent, returnType, arity);
                intrinsics.Add(name, p);
                return p;
            }

            public Expression CallIntrinsic(string name, bool isIdempotent, FunctionType fnType, params Expression[] args)
            {
                if (!intrinsics.TryGetValue(name, out var intrinsic))
                {
                    intrinsic = new IntrinsicProcedure(name, isIdempotent, fnType);
                    intrinsics.Add(name, intrinsic);
                }
                return new Application(
                    new ProcedureConstant(PrimitiveType.Ptr32, intrinsic),
                    intrinsic.ReturnType,
                    args);
            }

            public Expression Intrinsic(string name, bool isIdempotent, DataType returnType, params Expression[] args)
            {
                var intrinsic = EnsureIntrinsic(name, isIdempotent, returnType, args.Length);
                return new Application(
                    new ProcedureConstant(PrimitiveType.Ptr32, intrinsic),
                    returnType,
                    args);
            }

            public Expression Intrinsic(string name, bool isIdempotent, ProcedureCharacteristics c, DataType returnType, params Expression[] args)
            {
                var intrinsic = EnsureIntrinsic(name, isIdempotent, returnType, args.Length);
                intrinsic.Characteristics = c;
                return new Application(
                    new ProcedureConstant(PrimitiveType.Ptr32, intrinsic),
                    returnType,
                    args);
            }
            public Expression GetImport(Address addrThunk, Address addrInstr)
            {
                return null;
            }

            public ExternalProcedure GetImportedProcedure(IProcessorArchitecture arch, Address addrThunk, Address addrInstruction)
            {
                if (importThunks.TryGetValue(addrThunk, out var p))
                    throw new NotImplementedException();
                else
                    return null;
            }

            public virtual IProcessorArchitecture GetArchitecture(string archLabel)
            {
                throw new NotImplementedException();
            }

            public ExternalProcedure GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk)
            {
                throw new NotImplementedException();
            }

            public bool TryRead(IProcessorArchitecture arch, Address addr, PrimitiveType dt, out Constant value)
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

        public uint BitStringToUInt32(string bitPattern)
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

        public static byte[] OctalStringToBytes(string octalBytes)
        {
            var w = new BeImageWriter();
            int h = 0;
            int nDigits = 0;
            for (int i = 0; i < octalBytes.Length; ++i)
            {
                var digit = octalBytes[i] - '0';
                if (0 <= digit && digit <= 9)
                {
                    ++nDigits;
                    h = h * 8 + digit;
                    if (nDigits == 6)
                    {
                        w.WriteBeUInt16((ushort) h);
                        h = 0;
                        nDigits = 0;
                    }
                }
            }
            var aOut = new byte[w.Position];
            Array.Copy(w.Bytes, aOut, aOut.Length);
            return aOut;
        }

    }
}
