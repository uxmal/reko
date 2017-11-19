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

using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Pascal;
using Reko.Core.Serialization;
using System.Linq;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Diagnostics;

namespace Reko.Environments.MacOS
{
    public class TypeImporter : IPascalSyntaxVisitor<SerializedType>
    {
        private TypeLibraryDeserializer tldser;
        private IDictionary<string, Constant> constants;
        private TypeLibrary typelib;
        private ConstantEvaluator ceval;

        public TypeImporter(TypeLibraryDeserializer tldser, Dictionary<string, Constant> constants, TypeLibrary typelib)
        {
            this.tldser = tldser;
            this.constants = constants;
            this.typelib = typelib;
            this.ceval = new ConstantEvaluator(new Dictionary<string, Exp>(), constants); 
        }

        public SerializedType VisitArrayType(Core.Pascal.Array array)
        {
            var dt = array.ElementType.Accept(this);
            foreach (var dim in array.Dimensions)
            {
                var loId = dim.Low as Id;
                DataType dtLo;
                if (loId != null && typelib.Types.TryGetValue(loId.Name, out dtLo))
                {
                    var et = dtLo as Core.Types.EnumType;
                    if (et != null)
                    {
                        dt = new ArrayType_v1
                        {
                            ElementType = dt,
                            Length = et.Members.Count,
                        };
                        continue;
                    }
                } 
                var lo = dim.Low.Accept(ceval);
                if (dim.High != null)
                {
                    // Range a..b size is (b-a)+1
                    var hi = dim.High.Accept(ceval);
                    dt = new ArrayType_v1
                    {
                        ElementType = dt,
                        Length =
                            (int)(hi.ToInt64() - lo.ToInt64()) + 1
                    };
                }
                else
                {
                    dt = new ArrayType_v1 { ElementType = dt, Length = (int)lo.ToInt64() };
                }
            }
            return dt;
        }

        public SerializedType VisitBinExp(BinExp binExp)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitBooleanLiteral(BooleanLiteral booleanLiteral)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitCallableDeclaration(CallableDeclaration cd)
        {
            var dtRet = cd.ReturnType != null
                ? cd.ReturnType.Accept(this)
                : new VoidType_v1();

            var parameters = new List<Argument_v1>();
            foreach (var pc in cd.Parameters)
            {
                if (pc.Type == null)
                {
                    parameters.Add(new Argument_v1 { Name = "..." });
                }
                else
                {
                    var dt = pc.Type.Accept(this);
                    if (pc.ByReference)
                    {
                        dt = new ReferenceType_v1 { Referent = dt, Size = 4 };
                    }
                    foreach (var n in pc.ParameterNames)
                    {
                        var p = new Argument_v1 { Name = n, Type = dt };
                        parameters.Add(p);
                    }
                }
            }
            return new SerializedSignature
            {
                Arguments = parameters.ToArray(),
                ReturnValue = new Argument_v1 { Type = dtRet }
            };
        }

        public SerializedType VisitConstantDeclaration(ConstantDeclaration cd)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitEnumType(Core.Pascal.EnumType enumType)
        {
            return new SerializedEnumType
            {
                Values = enumType.Names
                    .Select((n, i) => new SerializedEnumValue { Name = n, Value = i })
                    .ToArray()
            };
        }

        public SerializedType VisitIdentifier(Id id)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitInlineMachineCode(InlineMachineCode code)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitNumericLiteral(NumericLiteral number)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitPointerType(Core.Pascal.Pointer pointer)
        {
            var dt = pointer.pointee.Accept(this);
            return new PointerType_v1 { DataType = dt, PointerSize = 4 };
        }

        public SerializedType VisitPrimitiveType(Primitive primitive)
        {
            return primitive.Type;
        }

        public SerializedType VisitRangeType(RangeType rangeType)
        {
            var low = rangeType.Low.Accept(ceval).ToInt64();
            var hi = rangeType.High.Accept(ceval).ToInt64();
            var delta = hi - low + 1;
            if (delta < 0)
                throw new NotImplementedException("Range overflow.");
            if (delta < 256)
            {
                return low < 0
                    ? PrimitiveType_v1.SChar8()
                    : PrimitiveType_v1.UChar8();
            }
            if (delta < 65536)
            {
                return low <= 0
                    ? PrimitiveType_v1.Int16()
                    : PrimitiveType_v1.UInt16();
            }
            if (delta < (1L << 32))
            {
                return low <= 0
                    ? PrimitiveType_v1.Int32()
                    : PrimitiveType_v1.UInt32();
            }
            return PrimitiveType_v1.Int64();
        }

        public SerializedType VisitRealLiteral(RealLiteral realLiteral)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitRecord(Record record)
        {
            var fields = new List<StructField_v1>();
            foreach (var recfield in record.Fields)
            {
                var dt = recfield.Type.Accept(this);
                fields.Add(new StructField_v1 { Name = recfield.Name, Type = dt });
            }
            return new StructType_v1
            {
                ForceStructure = true,
                Fields = fields.ToArray()
            };
        }

        public SerializedType VisitSetType(SetType setType)
        {
            //$TODO: Reko doesn't support bitset semantics natively yet.
            //$For now, return the wordsize of the MacOS.
            return PrimitiveType_v1.Int32();
        }

        public SerializedType VisitStringLiteral(StringLiteral str)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitStringType(Core.Pascal.StringType strType)
        {
            return new StringType_v2
            {
                 CharType = PrimitiveType_v1.Char8(),
                 Termination = StringType_v2.MsbTermination,
            };
        }

        public SerializedType VisitTypeDeclaration(TypeDeclaration td)
        {
            var dt = td.Type.Accept(this);
            var typedef = new SerializedTypedef { Name = td.Name, DataType = dt };
            tldser.LoadType(typedef);
            return typedef;
        }

        public SerializedType VisitTypeReference(Core.Pascal.TypeReference typeref)
        {
            return new TypeReference_v1(typeref.TypeName);
        }

        public SerializedType VisitUnaryExp(UnaryExp unaryExp)
        {
            throw new NotImplementedException();
        }
    }
}