#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Hll.Pascal;
using Reko.Core.Serialization;
using System.Linq;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System.Diagnostics;
using TypeSizer = Reko.Core.Hll.C.TypeSizer;
using System.ComponentModel;

namespace Reko.Environments.MacOS.Classic
{
    /// <summary>
    /// Imports type definition in Pascal to Reko's type system.
    /// </summary>
    public class TypeImporter : IPascalSyntaxVisitor<SerializedType>
    {
        private readonly TypeSizer sizer;
        private readonly IDictionary<string, Expression> constants;
        private readonly IDictionary<string, SerializedType> namedTypes;
        private readonly IDictionary<string, int> sizes;
        private readonly ConstantEvaluator ceval;
        private IDictionary<string, PascalType> pascalTypes;
        private readonly TypeLibraryDeserializer tldser;
        private readonly TypeLibrary typelib;

        public TypeImporter(IPlatform platform, TypeLibraryDeserializer tldser, Dictionary<string, Expression> constants, TypeLibrary typelib)
        {
            this.tldser = tldser;
            this.typelib = typelib;
            this.constants = constants;
            this.ceval = new ConstantEvaluator(new Dictionary<string, Exp>(), constants);
            this.namedTypes = new Dictionary<string, SerializedType>(StringComparer.InvariantCultureIgnoreCase);
            this.sizes= new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            this.sizer = new TypeSizer(platform, namedTypes);
            this.pascalTypes = null!;
        }

        public SerializedType VisitArrayType(Core.Hll.Pascal.Array array)
        {
            var dt = array.ElementType.Accept(this);
            foreach (var dim in array.Dimensions)
            {
                if (dim.Low is Id loId && pascalTypes.TryGetValue(loId.Name, out PascalType? dtLo))
                {
                    if (dtLo is Core.Hll.Pascal.EnumType et)
                    {
                        dt = new ArrayType_v1
                        {
                            ElementType = dt,
                            Length = et.Names.Count,
                        };
                        continue;
                    }
                }
                var lo = (Constant) dim.Low.Accept(ceval);
                if (dim.High is not null)
                {
                    // Range a..b size is (b-a)+1
                    var hi = (Constant)dim.High.Accept(ceval);
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

        public void LoadTypes(List<Declaration> declarations)
        {
            // Define all types first.
            this.pascalTypes = new Dictionary<string, PascalType>(StringComparer.InvariantCultureIgnoreCase);
            AddIntrinsicTypes(declarations);
            foreach (var decl in declarations.OfType<TypeDeclaration>())
            {
                pascalTypes[decl.Name] = decl.Type;
            }

            // Convert them to SerializedTypes.
            foreach (var typedef in declarations.OfType<TypeDeclaration>())
            {
                namedTypes[typedef.Name] = typedef.Accept(this);
            }

            // Compute sizes
            foreach (var nt in namedTypes)
            {
                nt.Value.Accept(sizer);
            }

            // Load forward declarations into type library.
            foreach (var nt in namedTypes)
            {
                typelib.Types[nt.Key] = new Core.Types.TypeReference(nt.Key, new UnknownType());
            }

            foreach (var nt in namedTypes)
            {
                typelib.Types[nt.Key] = nt.Value.Accept(this.tldser);
            }
        }

        private void AddIntrinsicTypes(List<Declaration> declarations)
        {
            declarations.AddRange(new[] {
                new TypeDeclaration(
                    "OBJECT",
                    new Primitive(PrimitiveType_v1.Ptr32())),
                new TypeDeclaration(
                    "Comp",
                    new Primitive(PrimitiveType_v1.Ptr32()))
            });
        }

        public SerializedType VisitBinExp(BinExp binExp)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitBooleanLiteral(BooleanLiteral booleanLiteral)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitCallableType(CallableType ct)
        {
            return DoCallableType(ct.Parameters, ct.ReturnType);
        }

        public SerializedType VisitCallableDeclaration(CallableDeclaration cd)
        {
            return DoCallableType(cd.Parameters, cd.ReturnType);
        }

        private SerializedType DoCallableType(List<ParameterDeclaration> pParameters, PascalType? ret)
        {
            var dtRet = ret is not null
                ? ret.Accept(this)
                : new VoidType_v1();

            var parameters = new List<Argument_v1>();
            foreach (var pc in pParameters)
            {
                if (pc.Type is null)
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

        public SerializedType VisitEnumType(Core.Hll.Pascal.EnumType enumType)
        {
            return new SerializedEnumType
            {
                Values = enumType.Names
                    .Select((n, i) => new SerializedEnumValue { Name = n, Value = i })
                    .ToArray()
            };
        }

        public SerializedType VisitFile(Core.Hll.Pascal.File file)
        {
            return new PrimitiveType_v1
            {
                Domain = Domain.Pointer,
                ByteSize = 4,
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

        public SerializedType VisitObject(ObjectType objectType)
        {
            //$TODO: what do we translate Pascal OBJECT to?
            return new StructType_v1
            {

            };
        }

        public SerializedType VisitPointerType(Core.Hll.Pascal.Pointer pointer)
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
            var low = ((Constant)rangeType.Low.Accept(ceval)).ToInt64();
            var hi = ((Constant)rangeType.High.Accept(ceval)).ToInt64();
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
            if (record.Fields is not null)
            {
                fields.AddRange(VisitFields(record.Fields));
            }
            if (record.VariantPart is not null)
            {
                if (record.VariantPart.VariantTag is not null)
                {
                    var sTagType = record.VariantPart.TagType.Accept(this);
                    fields.Add(new StructField_v1
                    {
                        Name = record.VariantPart.VariantTag,
                        Type = sTagType
                    });
                }
                var variants = VisitVariants(record.VariantPart.Variants);
                var union = new UnionType_v1
                {
                    Alternatives = variants.ToArray()
                };
                fields.Add(new StructField_v1
                {
                    Name = "",
                    Type = union,
                });
            }
            return new StructType_v1
            {
                ForceStructure = true,
                Fields = fields.ToArray()
            };
        }

        private IEnumerable<StructField_v1> VisitFields(List<Core.Hll.Pascal.Field> fields)
        {
            var result = new List<StructField_v1>();
            foreach (var recfield in fields)
            {
                var dt = recfield.Type.Accept(this);
                foreach (string fieldName in recfield.Names)
                {
                    var field = new StructField_v1
                    {
                        Name = fieldName,
                        Type = dt
                    };
                    result.Add(field);
                    //$REVIEW: alignment?
                }
            }
            return result;
        }

        private List<UnionAlternative_v1> VisitVariants(List<Variant> variants)
        {
            var alternatives = new List<UnionAlternative_v1>();
            foreach (var variant in variants)
            {
                var fields = VisitFields(variant.Fields);
                foreach (var q in variant.TagValues)
                {
                    var altName = "u" + q;
                    var alt = new UnionAlternative_v1
                    {
                        Name = altName,
                        Type = new StructType_v1
                        {
                             Fields = fields.ToArray()
                        }
                    };
                    alternatives.Add(alt);
                }
            }
            return alternatives;
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

        public SerializedType VisitStringType(Core.Hll.Pascal.StringType strType)
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
            return typedef;
        }

        public SerializedType VisitTypeReference(Core.Hll.Pascal.TypeReference typeref)
        {
            return new TypeReference_v1(typeref.TypeName);
        }

        public SerializedType VisitUnaryExp(UnaryExp unaryExp)
        {
            throw new NotImplementedException();
        }
    }
}
