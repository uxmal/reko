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
using Reko.Core.Expressions;
using Reko.Core.Pascal;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Environments.MacOS.Classic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Environments.MacOS.Classic
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

            Assert.AreEqual("A01F", svc.SyscallInfo.Vector);
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

            Assert.AreEqual("A485", svc.SyscallInfo.Vector);
            Assert.AreEqual("d0", svc.SyscallInfo.RegisterValues[0].Register);
            Assert.AreEqual(1, Convert.ToInt32(svc.SyscallInfo.RegisterValues[0].Value));
            Assert.AreEqual("fn(void,())", svc.Signature.ToString());
        }

        [Test(Description = "Syscalls that return data in a register push the " +
              "register onto the stack to conform with the weird mac pascal" +
              "calling convention")]
        public void Ici_PostCallStackUpdate()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new Argument_v1[0],
                ReturnValue = new Argument_v1 { Type = PrimitiveType_v1.Int16() }
            };
            var svc = ici.BuildSystemCallFromMachineCode("CountADBs", ssig, Opcodes(0xA077, 0x3E80));

            Assert.AreEqual("A077", svc.SyscallInfo.Vector);
            Assert.AreEqual("d0", ((Register_v1)svc.Signature.ReturnValue.Kind).Name);
        }

        [Test]
        public void Ici_Registers_in_and_out()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new[]
                {
                    new Argument_v1 {
                        Name = "paramBlock",
                        Type = new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = 4 }
                    }
                },
                ReturnValue = new Argument_v1
                {
                    Type = new TypeReference_v1 { TypeName = "OSErr" },
                }
            };
            var svc = ici.BuildSystemCallFromMachineCode(
                "PBOpenDFSync", ssig, Opcodes(0x205F,0x701A,0xA060,0x3E80));
            Assert.AreEqual("A060", svc.SyscallInfo.Vector);
            Assert.AreEqual("d0", svc.SyscallInfo.RegisterValues[0].Register);
            Assert.AreEqual("1a", svc.SyscallInfo.RegisterValues[0].Value);
            Assert.AreEqual("d0", ((Register_v1)svc.Signature.ReturnValue.Kind).Name);
        }

        [Test]
        public void Ici_Return_in_AddressRegister()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new Argument_v1[0],
                ReturnValue = new Argument_v1
                {
                    Type = new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = 4 }
                },
            };
            var svc = ici.BuildSystemCallFromMachineCode(
                "GetZone", ssig, Opcodes(0xA11A, 0x2E88));
            Assert.AreEqual("A11A", svc.SyscallInfo.Vector);
            Assert.AreEqual("a0", ((Register_v1)svc.Signature.ReturnValue.Kind).Name);
        }

        [Test]
        public void Ici_Argument_in_AddressRegister()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new[] {
                    new Argument_v1
                    {
                        Name = "h",
                        Type = new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = 4 }
                    },
                }
            };
            var svc = ici.BuildSystemCallFromMachineCode(
                "HUnlock", ssig, Opcodes(0x205F, 0xA02A));
            Assert.AreEqual("A02A", svc.SyscallInfo.Vector);
            Assert.AreEqual("a0", ((Register_v1)svc.Signature.Arguments[0].Kind).Name);
        }


        [Test]
        public void Ici_Argument_in_DataRegister()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new[] {
                    new Argument_v1
                    {
                        Name = "addr24",
                        Type = new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = 4 }
                    },
                },
                ReturnValue = new Argument_v1
                {
                    Type = new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = 4 }
                }
            };
            var svc = ici.BuildSystemCallFromMachineCode(
                "Translate24To32", ssig, Opcodes(0x201F,0xA091,0x2E80));
            Assert.AreEqual("A091", svc.SyscallInfo.Vector);
            Assert.AreEqual("d0", ((Register_v1)svc.Signature.Arguments[0].Kind).Name);
        }

        [Test]
        public void Ici_Two_Register_Arguments()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new[] {
                    new Argument_v1
                    {
                        Name = "vblBlockPtr",
                        Type = new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = 4 }
                    },
                    new Argument_v1
                    {
                        Name = "theSlot",
                        Type = PrimitiveType_v1.Int16()
                    },
                },
                ReturnValue = new Argument_v1
                {
                    Type = PrimitiveType_v1.Int16()
                }
            };

            var svc = ici.BuildSystemCallFromMachineCode(
                "SlotVInstall", ssig, Opcodes(0x301F, 0x205F, 0xA06F, 0x3E80));
            Assert.AreEqual("A06F", svc.SyscallInfo.Vector);
            Assert.AreEqual("a0", ((Register_v1)svc.Signature.Arguments[0].Kind).Name);
            Assert.AreEqual("d0", ((Register_v1)svc.Signature.Arguments[1].Kind).Name);
            Assert.AreEqual("d0", ((Register_v1)svc.Signature.ReturnValue.Kind).Name);
        }

        [Test]
        public void Ici_Push_Stack_Constant()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new Argument_v1[0]
            };

            var svc = ici.BuildSystemCallFromMachineCode(
                "PRPurge", ssig, Opcodes(0x2F3C,0xA800,0x0000,0xA8FD));
            Assert.AreEqual("A8FD", svc.SyscallInfo.Vector);
            Assert.AreEqual("A8000000", svc.SyscallInfo.StackValues[0].Value);
            Assert.AreEqual("4", svc.SyscallInfo.StackValues[0].Offset);
        }

        [Test]
        public void Ici_Load_Word_Constant_To_Register()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new []
                {
                    new Argument_v1
                    {
                        Name = "typecode",
                        Type = PrimitiveType_v1.Int32(),
                    },
                    new Argument_v1
                    {
                        Name = "dataPtr",
                        Type = new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = 4 },
                    },
                    new Argument_v1
                    {
                        Name = "result",
                        Type = new ReferenceType_v1
                        {
                            Referent = new TypeReference_v1 { TypeName = "AEDesc" }
                        }
                    }
                },
                ReturnValue = new Argument_v1
                {
                    Type = PrimitiveType_v1.Int16()
                }
            };

            var svc = ici.BuildSystemCallFromMachineCode(
                "AECreateDesc", ssig, Opcodes(0x303C, 0x0825, 0xA816));
            Assert.AreEqual("A816", svc.SyscallInfo.Vector);
            Assert.AreEqual("d0", svc.SyscallInfo.RegisterValues[0].Register);
            Assert.AreEqual("0825", svc.SyscallInfo.RegisterValues[0].Value);
        }


        [Test]
        public void Ici_Push_Zero_Constant()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new[]
                {
                    new Argument_v1
                    {
                        Name = "act",
                        Type = PrimitiveType_v1.Bool(),
                    },
                    new Argument_v1
                    {
                        Name = "lHandle",
                        Type = new PrimitiveType_v1 { Domain = Domain.Pointer, ByteSize = 4 },
                    },
                },
            };

            var svc = ici.BuildSystemCallFromMachineCode(
                "LActivate", ssig, Opcodes(0x4267,0xA9E7));
            Assert.AreEqual("A9E7", svc.SyscallInfo.Vector);
            Assert.AreEqual("4", svc.SyscallInfo.StackValues[0].Offset);
            Assert.AreEqual("0000", svc.SyscallInfo.StackValues[0].Value);
        }

        [Test]
        public void Ici_Push_Register_Constant()
        {
            var ici = new InlineCodeInterpreter(constants);
            var ssig = new SerializedSignature
            {
                Arguments = new[]
                {
                    new Argument_v1
                    {
                        Name = "act",
                        Type = new ReferenceType_v1 {
                            Referent = PrimitiveType_v1.Int16(),
                            Size = 4,
                        }
                    },
                },
                ReturnValue = new Argument_v1
                {
                    Type = PrimitiveType_v1.Int16(),
                }
            };

            var svc = ici.BuildSystemCallFromMachineCode(
                "GetFrontProcess", ssig, Opcodes(0x70FF, 0x2F00, 0x3F3C, 0x0039, 0xA88F));
            Assert.AreEqual("A88F", svc.SyscallInfo.Vector);
            Assert.AreEqual("4", svc.SyscallInfo.StackValues[0].Offset);
            Assert.AreEqual("0039", svc.SyscallInfo.StackValues[0].Value);
            Assert.AreEqual("6", svc.SyscallInfo.StackValues[1].Offset);
            Assert.AreEqual("ffffffff", svc.SyscallInfo.StackValues[1].Value);
        }
    }
}
