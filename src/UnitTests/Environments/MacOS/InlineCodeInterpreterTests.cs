#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Pascal;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Environments.MacOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Environments.MacOS
{
    [TestFixture]
    public class InlineCodeInterpreterTests
    {
        private Dictionary<string, Constant> constants;

        [SetUp]
        public void Setup()
        {
            this.constants = new Dictionary<string, Constant>();
        }

        private Exp Exp(int n)
        {
            return new NumericLiteral(n);
        }

        private List<Exp> Opcodes(params int [] uInstrs)
        {
            return uInstrs.Select(n => (Exp)new NumericLiteral(n)).ToList();
        }

        [Test]
        public void Ici_Evaluate_Symbolic()
        {
            constants.Add("foo", Constant.Int64(0xAAAA));
            var ici = new InlineCodeInterpreter(constants);
            var uShort = ici.EvaluateOpcode(new Id("foo"));
            Assert.AreEqual((ushort)0xAAAA, uShort);
        }

        [Test]
        public void Ici_PascalService()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new[]
                {
                    new Argument_v1 { Name = "arg1", Type=  PrimitiveType_v1.Int32() },
                    new Argument_v1 { Name = "arg2", Type=  PrimitiveType_v1.Int16() },
                }
            };
            var svc = ici.BuildSystemCallFromMachineCode("fooSvc", ssig, new List<Exp> { Exp(0xAAAA) });
            Assert.AreEqual((ushort)0xAAAA, svc.Ordinal);
            Assert.AreEqual("fooSvc", svc.Name);
            Assert.AreEqual("fn(pascal,void,(arg(arg1,prim(SignedInt,4)),arg(arg2,prim(SignedInt,2))))", svc.Signature.ToString());
        }

        [Test]
        public void Ici_RegisterBasedService()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new[]
                {
                    new Argument_v1 { Name = "p", Type =  new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = 4 } }
                }
            };
            var svc = ici.BuildSystemCallFromMachineCode("DisposePtr", ssig, new List<Exp> { Exp(0x205F), Exp(0xA01F) });

            Assert.AreEqual((ushort)0xA01F, svc.Ordinal);
            Assert.AreEqual("DisposePtr", svc.Name);
            Assert.AreEqual("fn(void,(arg(p,prim(Pointer,4))))", svc.Signature.ToString());
            Assert.AreEqual("a0", ((Register_v1)svc.Signature.Arguments[0].Kind).Name);
        }

        [Test]
        public void Ici_RegisterDispatchService()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new Argument_v1[0],
            };
            var svc = ici.BuildSystemCallFromMachineCode("DisableIdle", ssig, Opcodes(0x7001, 0xA485));

            Assert.AreEqual((ushort)0xA485, svc.Ordinal);
            Assert.AreEqual("d0", svc.SyscallInfo.RegisterValues[0].Register);
            Assert.AreEqual(1, Convert.ToInt32(svc.SyscallInfo.RegisterValues[0].Value));
            Assert.AreEqual("fn(void,())", svc.Signature.ToString());
        }
    }
}
