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
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Environments.SysV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Environments.SysV
{
    [TestFixture]
    public class MipsProcedureSerializerTests
    {
        private Argument_v1 d1 = new Argument_v1 { Name = "d1", Type = PrimitiveType_v1.Real64() };
        private Argument_v1 d2 = new Argument_v1 { Name = "d2", Type = PrimitiveType_v1.Real64() };
        private Argument_v1 s1 = new Argument_v1 { Name = "s1", Type = PrimitiveType_v1.Real32() };
        private Argument_v1 s2 = new Argument_v1 { Name = "s2", Type = PrimitiveType_v1.Real32() };
        private Argument_v1 s3 = new Argument_v1 { Name = "s3", Type = PrimitiveType_v1.Real32() };
        private Argument_v1 s4 = new Argument_v1 { Name = "s4", Type = PrimitiveType_v1.Real32() };
        private Argument_v1 n1 = new Argument_v1 { Name = "n1", Type = PrimitiveType_v1.Int32() };
        private Argument_v1 n2 = new Argument_v1 { Name = "n2", Type = PrimitiveType_v1.Int32() };
        private Argument_v1 n3 = new Argument_v1 { Name = "n3", Type = PrimitiveType_v1.Int32() };
        private Argument_v1 n4 = new Argument_v1 { Name = "n4", Type = PrimitiveType_v1.Int32() };

        private void AssertSignature(string sExp, params Argument_v1[] args)
        {
            var arch = new MipsBe32Architecture();
            var tser = new TypeDeserializer();
            var mipsps = new MipsProcedureSerializer(arch, tser, null);
            var sig = mipsps.Deserialize(new SerializedSignature
            {
                Arguments = args
            },
            arch.CreateFrame());
            var sArgs = string.Join(", ", sig.Parameters.Select(p => RenderArg(p.Storage)));
            Assert.AreEqual(sExp.Trim(), sArgs);
        }

        private string RenderArg(Storage arg)
        {
            var reg = arg as RegisterStorage;
            if (reg != null)
            {
                return RenderReg(reg);
            }
            var seq = arg as SequenceStorage;
            if (seq != null)
            {
                var head = RenderArg(seq.Head);
                var tail = RenderArg(seq.Tail);
                return string.Format("({0}, {1})", head, tail);
            }
            return "stack";
        }

        private static string RenderReg(RegisterStorage reg)
        {
            return reg.Name;
        }

        private class TypeDeserializer : ISerializedTypeVisitor<DataType>
        {
            public DataType VisitArray(ArrayType_v1 array)
            {
                throw new NotImplementedException();
            }

            public DataType VisitCode(CodeType_v1 code)
            {
                throw new NotImplementedException();
            }

            public DataType VisitEnum(SerializedEnumType serializedEnumType)
            {
                throw new NotImplementedException();
            }

            public DataType VisitMemberPointer(MemberPointer_v1 memptr)
            {
                throw new NotImplementedException();
            }

            public DataType VisitPointer(PointerType_v1 pointer)
            {
                throw new NotImplementedException();
            }

            public DataType VisitReference(ReferenceType_v1 reference)
            {
                throw new NotImplementedException();
            }

            public DataType VisitPrimitive(PrimitiveType_v1 primitive)
            {
                return PrimitiveType.Create(primitive.Domain, primitive.ByteSize);
            }

            public DataType VisitSignature(SerializedSignature signature)
            {
                throw new NotImplementedException();
            }

            public DataType VisitString(StringType_v2 str)
            {
                throw new NotImplementedException();
            }

            public DataType VisitStructure(StructType_v1 structure)
            {
                throw new NotImplementedException();
            }

            public DataType VisitTemplate(SerializedTemplate serializedTemplate)
            {
                throw new NotImplementedException();
            }

            public DataType VisitTypedef(SerializedTypedef typedef)
            {
                throw new NotImplementedException();
            }

            public DataType VisitTypeReference(TypeReference_v1 typeReference)
            {
                throw new NotImplementedException();
            }

            public DataType VisitUnion(UnionType_v1 union)
            {
                throw new NotImplementedException();
            }

            public DataType VisitVoidType(VoidType_v1 serializedVoidType)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void MipsPS_Regular()
        {
            //$TODO: there doesn't seem to be a consistent
            // piece of documentation for MIPS ABI....
            AssertSignature("(f12, f13), (f14, f15)", d1, d2);
            AssertSignature("f12, f14              ", s1, s2);
            AssertSignature("f12, (f14, f15)       ", s1, d1);
            AssertSignature("(f12, f13), f14       ", d1, s1);
            AssertSignature("r4, r5, r6, r7        ", n1, n2, n3, n4);
            AssertSignature("(f12, f13), r6, stack ", d1, n1, d2);
            AssertSignature("(f12, f13), r6, r7    ", d1, n1, n2);
            AssertSignature("f12, r5, r6           ", s1, n1, n2);
            AssertSignature("r4, r5, r6, stack     ", n1, n2, n3, d1);
            AssertSignature("r4, r5, r6, r7        ", n1, n2, n3, s1);
            AssertSignature("r4, r5, (r6, r7)      ", n1, n2, d1);
            AssertSignature("r4, (r6, r7)          ", n1, d1);
            //AssertSignature("f12, f14, r6, r7      ", s1, s2, s3, s4);
            //AssertSignature("f12, r5, r6, r7    ", s1, n1, s2, n2);
            //AssertSignature("f12, f14, r6      ", d1, s1, s2);
            //AssertSignature("f12, f14, (r6, r7)", s1, s2, d1);
            AssertSignature("r4, r5, r6, r7      ", n1, s1, n2, s2);
            AssertSignature("r4, r5, r6, r7      ", n1, s1, n2, n3);
            AssertSignature("r4, r5, r6, r7      ", n1, n2, s1, n3);
        }

        [Test]
        [Ignore("Wait until printf encountered")]
        public void MipsPS_VarArgs()
        { 
            // In the following examples, an ellipsis appears in the second argument
            // slot.
            AssertSignature("r4, (r6, r7), stack ", n1, d1, d2);
            AssertSignature("f12, r5            ", s1, n1);
            AssertSignature("f12, r5, (r6, r7)  ", s1, n1, d1);
            AssertSignature("f12, f6            ", d1, n1);
            AssertSignature("f12,r6, stack      ", d1, n1, d2);
        }
    }
}
