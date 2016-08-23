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
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Environments.Windows;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.Windows
{
    [TestFixture]
    public class X86_64ProcedureSerializerTests
    {
        private X86ArchitectureFlat64 arch;
        private Frame frame;

        [SetUp]
        public void Setup()
        {
            this.arch = new X86ArchitectureFlat64();
            this.frame = arch.CreateFrame();
        }
        
        private X86_64ProcedureSerializer Given_ProcedureSerializer()
        {
            return new X86_64ProcedureSerializer(arch, new FakeTypeDeserializer(arch.PointerType.Size), "");
        }

        private void ExpectArgs(FunctionType sig, params string[] args)
        {
            int c = Math.Min(sig.Parameters.Length, args.Length);
            for (int i = 0; i < c; ++i)
            {
                Assert.AreEqual(args[i], sig.Parameters[i].Storage.ToString(), string.Format("Argument {0} ain't right", i));
            }
            Assert.AreEqual(args.Length, c, "Fewer args than expected");
        }

        [Test]
        public void X86_64Psig_Deserialize()
        {
            var pser = Given_ProcedureSerializer();
            var ssig = new SerializedSignature
            {
            };

            var sig = pser.Deserialize(ssig, frame);

            Assert.AreEqual("void test()", sig.ToString("test"));
        }

        [Test]
        public void X86_64Psig_AllInts()
        {
            var pser = Given_ProcedureSerializer();
            var ssig = new SerializedSignature
            {
                ReturnValue = new Argument_v1 { Type = PrimitiveType_v1.Int32() },
                Arguments = new Argument_v1[]
                {
                    new Argument_v1 { Type=PrimitiveType_v1.Int32(), Name = "a" },
                    new Argument_v1 { Type=PrimitiveType_v1.Int32(), Name = "b" },
                    new Argument_v1 { Type=PrimitiveType_v1.Int32(), Name = "c" },
                    new Argument_v1 { Type=PrimitiveType_v1.Int32(), Name = "d" },
                    new Argument_v1 { Type=PrimitiveType_v1.Int32(), Name = "e" },
                    new Argument_v1 { Type= PointerType_v1.Create(PrimitiveType_v1.Int32(), 8), Name = "e" },
                }
            };

            var sig = pser.Deserialize(ssig, frame);

            Assert.AreEqual("rax", sig.ReturnValue.Storage.ToString());
            ExpectArgs(sig, "rcx", "rdx", "r8", "r9", "Stack +0008");
        }

        [Test]
        public void X86_64Psig_AllFloats()
        {
            var pser = Given_ProcedureSerializer();
            var ssig = new SerializedSignature
            {
                ReturnValue = new Argument_v1 { Type = PrimitiveType_v1.Real32() },
                Arguments = new Argument_v1[]
                {
                    new Argument_v1 { Type=PrimitiveType_v1.Real32(), Name = "a" },
                    new Argument_v1 { Type=PrimitiveType_v1.Real64(), Name = "b" },
                    new Argument_v1 { Type=PrimitiveType_v1.Real32(), Name = "c" },
                    new Argument_v1 { Type=PrimitiveType_v1.Real64(), Name = "d" },
                    new Argument_v1 { Type=PrimitiveType_v1.Real32(), Name = "e" },
                }
            };

            var sig = pser.Deserialize(ssig, frame);

            Assert.AreEqual("xmm0", sig.ReturnValue.Storage.ToString());
            ExpectArgs(sig, "xmm0", "xmm1", "xmm2", "xmm3", "Stack +0008");
        }

        [Test]
        public void X86_64Psig_MixedIntsFloats()
        {
            var pser = Given_ProcedureSerializer();
            var ssig = new SerializedSignature
            {
                ReturnValue = new Argument_v1 { Type = PrimitiveType_v1.Int32() },
                Arguments = new Argument_v1[]
                {
                    new Argument_v1 { Type=PrimitiveType_v1.Int32(), Name = "a" },
                    new Argument_v1 { Type=PrimitiveType_v1.Real64(), Name = "b" },
                    new Argument_v1 { Type=PointerType_v1.Create(PrimitiveType_v1.Char8(), 8), Name = "c" },
                    new Argument_v1 { Type=PrimitiveType_v1.Real64(), Name = "d" },
                    new Argument_v1 { Type=PrimitiveType_v1.Real32(), Name = "e" },
                }
            };

            var sig = pser.Deserialize(ssig, frame);

            Assert.AreEqual("rax", sig.ReturnValue.Storage.ToString());
            ExpectArgs(sig, "rcx", "xmm1", "r8", "xmm3", "Stack +0008");
        }


        [Test(Description = "Verifies that small stack arguments are properly aligned on stack")]
        public void X86_64Psig_SmallStackArguments()
        {
            var pser = Given_ProcedureSerializer();
            var ssig = new SerializedSignature
            {
                ReturnValue = new Argument_v1 { Type = PrimitiveType_v1.Int32() },
                Arguments = new Argument_v1[]
                {
                    new Argument_v1 { Type=PrimitiveType_v1.Int32(), Name = "a" },
                    new Argument_v1 { Type=PrimitiveType_v1.Real64(), Name = "b" },
                    new Argument_v1 { Type=PointerType_v1.Create(PrimitiveType_v1.Char8(), 8), Name = "c" },
                    new Argument_v1 { Type=PrimitiveType_v1.Real64(), Name = "d" },
                    new Argument_v1 { Type=PrimitiveType_v1.Char8(), Name = "e" },
                    new Argument_v1 { Type=PrimitiveType_v1.Char8(), Name = "f" },
                    new Argument_v1 { Type=PrimitiveType_v1.Char8(), Name = "g" },
                }
            };

            var sig = pser.Deserialize(ssig, frame);

            Assert.AreEqual("rax", sig.ReturnValue.Storage.ToString());
            ExpectArgs(sig, "rcx", "xmm1", "r8", "xmm3", "Stack +0008", "Stack +0010", "Stack +0018");
        }
    }
}
