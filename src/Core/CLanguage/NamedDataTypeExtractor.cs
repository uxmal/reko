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

using Reko.Core.Types;
using Reko.Core.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Core.CLanguage
{
    /// <summary>
    /// Extracts the name of the declared entity from a C declaration. C is a 
    /// hairy language....
    /// </summary>
    public class NamedDataTypeExtractor :
        DeclaratorVisitor<Func<NamedDataType,NamedDataType>>,
        DeclSpecVisitor<SerializedType>
    {
        private IEnumerable<DeclSpec> specs;
        private SymbolTable symbolTable;
        private SerializedType dt;
        private Domain domain;
        private int byteSize;
        private CTokenType callingConvention;
        private CConstantEvaluator eval;
        private CBasicType basicType;
        private IPlatform platform;

        public NamedDataTypeExtractor(IPlatform platform, IEnumerable<DeclSpec> specs, SymbolTable converter)
        {
            this.platform = platform ?? throw new ArgumentNullException("platform");
            this.specs = specs;
            this.symbolTable = converter;
            this.callingConvention = CTokenType.None;
            this.eval = new CConstantEvaluator(platform, converter.Constants);
            this.basicType = CBasicType.None;
            foreach (var declspec in specs)
            {
                dt = declspec.Accept(this);
            }
        }

        public NamedDataType GetNameAndType(Declarator declarator)
        {
            var nt = new NamedDataType { DataType = dt, Size = byteSize };
            if (declarator != null)
            {
                nt = declarator.Accept(this)(nt);
            }
            return nt;
        }

        public Func<NamedDataType, NamedDataType> VisitId(IdDeclarator id)
        {
            return (nt) => new NamedDataType { Name = id.Name, DataType = nt.DataType, Size = nt.Size};
        }

        public Func<NamedDataType, NamedDataType> VisitArray(ArrayDeclarator array)
        {
            var fn = array.Declarator.Accept(this);
            return (nt) =>
            {
                nt = new NamedDataType
                {
                    Name = nt.Name,
                    DataType = new ArrayType_v1
                    {
                        ElementType = nt.DataType,
                        Length = array.Size != null
                            ? Convert.ToInt32(array.Size.Accept(eval))
                            : 0
                    }
                };
                return fn(nt);
            };
        }

        public Func<NamedDataType, NamedDataType> VisitField(FieldDeclarator field)
        {
            Func<NamedDataType, NamedDataType> fn;
            if (field.Declarator == null)
            {
                fn = (nt) => nt;
            }
            else 
            {
                fn = field.Declarator.Accept(this);
            }
            return fn;
        }

        public Func<NamedDataType,NamedDataType> VisitPointer(PointerDeclarator pointer)
        {
            Func<NamedDataType, NamedDataType> fn;
            if (pointer.Pointee != null)
            {
                fn = pointer.Pointee.Accept(this);
            }
            else
            {
                fn = f => f;
            }
            return (nt) =>
            {
                var size = PointerSize();
                nt.DataType = new PointerType_v1
                {
                    DataType = nt.DataType,
                    PointerSize = size,
                };
                nt.Size = PointerSize();
                return fn(nt);
            };
        }

        public Func<NamedDataType, NamedDataType> VisitReference(ReferenceDeclarator reference)
        {
            Func<NamedDataType, NamedDataType> fn;
            if (reference.Referent!= null)
            {
                fn = reference.Referent.Accept(this);
            }
            else
            {
                fn = f => f;
            }
            return (nt) =>
            {
                var size = PointerSize();
                nt.DataType = new ReferenceType_v1
                {
                    Referent = nt.DataType,
                    Size = size,
                    //$TODO: Qualifier
                };
                nt.Size = PointerSize();
                return fn(nt);
            };
        }

        private int PointerSize()
        {
            if (specs.OfType<TypeQualifier>()
                    .Any(t => t.Qualifier == CTokenType._Near))
                return 2;
            return platform.PointerType.Size;
        }

        public Func<NamedDataType, NamedDataType> VisitFunction(FunctionDeclarator function)
        {
            var fn = function.Declarator.Accept(this);
            return (nt) =>
            {
                var parameters = function.Parameters
                    .Select(p => ConvertParameter(p))
                    .ToArray();

                // Special case for C, where foo(void) means a function with no parameters,
                // not a function with one parameter of type "void".
                if (FirstParameterVoid(parameters))
                    parameters = new Argument_v1[0];

                Argument_v1 ret = null;
                if (nt.DataType != null)
                {
                    ret = new Argument_v1
                    {
                        Type = nt.DataType,
                    };
                }
                nt.DataType = new SerializedSignature
                {
                    Convention = callingConvention != CTokenType.None
                        ? callingConvention.ToString().ToLower()
                        : null,
                    ReturnValue = ret,
                    Arguments = parameters.ToArray(),
                };
                return fn(nt);
            };
        }

        private bool FirstParameterVoid(Argument_v1[] parameters)
        {
            if (parameters == null || parameters.Length != 1)
                return false;
            return parameters[0].Type is VoidType_v1;
        }

        private Argument_v1 ConvertParameter(ParamDecl decl)
        {
            if (decl.IsEllipsis)
            {
                return new Argument_v1
                {
                    Name = "...",
                };
            }
            else
            {
                var ntde = new NamedDataTypeExtractor(platform, decl.DeclSpecs, symbolTable);
                var ntTmp = ntde.GetNameAndType(decl.Declarator);
                var nt = ConvertArrayToPointer(ntTmp);
                var kind = GetArgumentKindFromAttributes("arg", decl.Attributes);
                return new Argument_v1
                {
                    Kind = kind,
                    Name = nt.Name,
                    Type = nt.DataType,
                };
            }
        }

        /// <summary>
        /// Attributes named [[reko::reg(&lt;tokens&gt;)]] expect a single register
        /// in &lt;tokens&gt;. Remaining tokens are ignored
        /// </summary>
        /// <param name="attrs"></param>
        /// <returns></returns>
        public SerializedKind GetArgumentKindFromAttributes(string paramType, List<CAttribute> attrs)
        {
            if (attrs == null)
                return null;
            SerializedKind kind = null;
            foreach (var attr in attrs)
            {
                if (attr.Name.Components == null || attr.Name.Components.Length != 2 ||
                    attr.Name.Components[0] != "reko" || attr.Name.Components[1] != paramType)
                    continue;
                if (attr.Tokens[0].Type == CTokenType.Register &&
                    attr.Tokens[1].Type == CTokenType.Comma)
                {
                    // We have a reko::arg(register, prefix; get the register.
                    if (attr.Tokens.Count < 1 || attr.Tokens[2].Type != CTokenType.StringLiteral)
                        throw new FormatException("[[reko::arg(register,<name>)]] attribute expects a register name.");
                    kind = new Register_v1 { Name = (string)attr.Tokens[2].Value };
                } else if (attr.Tokens[0].Type == CTokenType.Id &&
                           (string)attr.Tokens[0].Value == "fpu")
                {
                    // We have a reko::fpu prefix; mark as FPU
                    kind = new FpuStackVariable_v1();
                }
            }
            return kind;
        }

        /// <summary>
        /// Converts any array parameters to pointer parameters.
        /// </summary>
        /// <remarks>The C language treats an array parameter as a pointer. Thus <code>
        /// int foo(int arr[]);
        /// </code> is equivalent to <code>
        /// int foo(int * ptr);
        /// </code>
        /// </remarks>
        /// <param name="nt"></param>
        /// <returns></returns>
        private NamedDataType ConvertArrayToPointer(NamedDataType nt)
        {
            if (nt.DataType is ArrayType_v1 at)
            {
                return new NamedDataType
                {
                    Name = nt.Name,
                    DataType = new PointerType_v1 { DataType = at.ElementType },
                    Size = platform.PointerType.Size,
                };
            }
            else
            {
                return nt;
            }
        }

        public Func<NamedDataType,NamedDataType> VisitCallConvention(CallConventionDeclarator conv)
        {
            ApplyCallConvention(conv.Convention);
            return (nt) => conv.Declarator.Accept(this)(nt);
        }

        private void ApplyCallConvention(CTokenType convention)
        {
            if (callingConvention != CTokenType.None)
                throw new FormatException(string.Format("Unexpected extra calling convention specifier '{0}'.", callingConvention));
            callingConvention = convention;
        }

        public SerializedType VisitSimpleType(SimpleTypeSpec simpleType)
        {
            switch (simpleType.Type)
            {
            default:
                throw new NotImplementedException(string.Format("{0}", simpleType.Type));
            case CTokenType.Void:
                if (domain != Domain.None)
                    throw new FormatException(string.Format("Can't have 'void' after '{0}'.", domain));
                return new VoidType_v1();
            case CTokenType.__W64:
                return dt;      // Used by Microsoft compilers for 32->64 bit transition, deprecated.
            case CTokenType.Signed:
                if (domain != Domain.None)
                    throw new FormatException(string.Format("Can't have 'signed' after '{0}'.", domain));
                domain = Domain.SignedInt;
                basicType = CBasicType.Int;
                return CreatePrimitive();
            case CTokenType.Unsigned:
                if (domain != Domain.None)
                    throw new FormatException(string.Format("Can't have 'unsigned' after '{0}'.", domain));
                domain = Domain.UnsignedInt;
                basicType = CBasicType.Int;
                return CreatePrimitive();
            case CTokenType.Bool:
            case CTokenType._Bool:
                if (domain != Domain.None)
                    throw new FormatException(string.Format("An '{0}' boolean doesn't make sense.", domain));
                domain = Domain.Boolean;
                basicType = CBasicType.Bool;
                return CreatePrimitive();
            case CTokenType.Char:
                if (domain == Domain.None)
                    domain = Domain.Character;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}.", domain));
                basicType = CBasicType.Char;
                return CreatePrimitive();
            case CTokenType.Wchar_t:
                if (domain == Domain.None)
                    domain = Domain.Character;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                basicType = CBasicType.WChar_t;
                return CreatePrimitive();
            case CTokenType.Short:
                if (domain != Domain.None && domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                basicType = CBasicType.Short;
                return CreatePrimitive();
            case CTokenType.Int:
                if (domain == Domain.None)
                    domain = Domain.SignedInt;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                if (basicType == CBasicType.None)
                    basicType = CBasicType.Int;
                return CreatePrimitive();
            case CTokenType.Long:
                if (basicType == CBasicType.None)
                    basicType = CBasicType.Long;
                else if (basicType == CBasicType.Long)
                    basicType = CBasicType.LongLong;
                return CreatePrimitive();
            case CTokenType.__Int64:
                if (domain == Domain.None)
                    domain = Domain.SignedInt;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                basicType = CBasicType.Int64;
                return CreatePrimitive();
            case CTokenType.Float:
                if (domain != Domain.None)
                    throw new FormatException(string.Format("Unexpected domain {0} before float.", domain));
                domain = Domain.Real;
                basicType = CBasicType.Float;
                return CreatePrimitive();
            case CTokenType.Double:
                if (domain != Domain.None && domain != Domain.SignedInt)  //$REVIEW: short double? long double? long long double?
                    throw new FormatException(string.Format("Unexpected domain {0} before float.", domain));
                domain = Domain.Real;
                if (basicType == CBasicType.None)
                    basicType = CBasicType.Double;
                else if (basicType == CBasicType.Long)
                    basicType = CBasicType.LongDouble;
                return CreatePrimitive();
            }
        }

        private PrimitiveType_v1 CreatePrimitive()
        {
            byteSize = platform.GetByteSizeFromCBasicType(basicType);
            if (domain == Domain.None)
                domain = Domain.SignedInt;
            return new PrimitiveType_v1
            {
                Domain = domain,
                ByteSize = byteSize
            };
        }

        public SerializedType VisitTypedef(TypeDefName typeDefName)
        {
            if (symbolTable.PrimitiveTypes.TryGetValue(typeDefName.Name, out var prim))
            {
                byteSize = prim.ByteSize;
                return prim;
            }
            if (!symbolTable.NamedTypes.TryGetValue(typeDefName.Name, out var type))
            {
                throw new ApplicationException(string.Format(
                        "error: type name {0} not defined.",
                        typeDefName.Name ?? "(null)"));
            }
            byteSize = type.Accept(symbolTable.Sizer);
            return new TypeReference_v1(typeDefName.Name);
        }

        public SerializedType VisitComplexType(ComplexTypeSpec complexType)
        {
            if (complexType.Type == CTokenType.Struct)
            {
                if (complexType.Name == null || symbolTable.StructsSeen.TryGetValue(complexType.Name, out var str))
                {
                    str = new StructType_v1 {
                        Name = complexType.Name ?? string.Format("struct_{0}", symbolTable.StructsSeen.Count)
                    };
                    symbolTable.StructsSeen.Add(str.Name, str);
                }
                else
                {
                    str = new StructType_v1 { Name = complexType.Name };
                }
                if (!complexType.IsForwardDeclaration() && str.Fields == null)
                {
                    str.Fields = ExpandStructFields(complexType.DeclList).ToArray();
                    symbolTable.Sizer.SetSize(str);
                    symbolTable.Types.Add(str);
                    str = new StructType_v1 { Name = str.Name };
                }
                return str;
            }
            else if (complexType.Type == CTokenType.Union)
            {
                if (complexType.Name == null || !symbolTable.UnionsSeen.TryGetValue(complexType.Name, out var un))
                {
                    un = new UnionType_v1 { Name = complexType.Name };
                    if (un.Name != null)
                    {
                        symbolTable.UnionsSeen.Add(un.Name, un);
                    }
                }
                if (!complexType.IsForwardDeclaration() && un.Alternatives == null)
                {
                    un.Alternatives = ExpandUnionFields(complexType.DeclList).ToArray();
                    symbolTable.Sizer.SetSize(un);
                    if (un.Name != null)
                    {
                        symbolTable.Types.Add(un);
                        un = new UnionType_v1 { Name = un.Name };
                    }
                }
                return un;
            }
            else
                throw new NotImplementedException();
        }

        public SerializedType VisitEnum(EnumeratorTypeSpec e)
        {
            if (e.Tag == null || !symbolTable.EnumsSeen.TryGetValue(e.Tag, out var en))
            {
                en = new SerializedEnumType {
                    Name = e.Tag ?? string.Format("enum_{0}", symbolTable.EnumsSeen.Count)
                };
                symbolTable.EnumsSeen.Add(en.Name, en);
                var enumEvaluator = new EnumEvaluator(new CConstantEvaluator(platform, symbolTable.Constants));
                var listMembers = new List<SerializedEnumValue>();
                foreach (var item in e.Enums)
                {
                    var ee = new SerializedEnumValue
                    {
                        Name = item.Name,
                        Value = enumEvaluator.GetValue(item.Value),
                    };
                    symbolTable.Constants.Add(ee.Name, ee.Value);
                    listMembers.Add(ee);
                }
                en.Values = listMembers.ToArray();
                symbolTable.Types.Add(en);
                en = new SerializedEnumType { Name = en.Name };
            }
            else
            {
                en = new SerializedEnumType { Name = e.Tag };
            }
            return en;
        }

        private IEnumerable<StructField_v1> ExpandStructFields(IEnumerable<StructDecl> decls)
        {
            int offset = 0;
            foreach (var decl in decls)
            {
                var ntde = new NamedDataTypeExtractor(platform, decl.SpecQualifierList, symbolTable);
                foreach (var declarator in decl.FieldDeclarators)
                {
                    var nt = ntde.GetNameAndType(declarator);
                    var rawSize = nt.DataType.Accept(symbolTable.Sizer);
                    offset = Align(offset, rawSize, 8);     //$BUG: disregards temp. alignment changes. (__declspec(align))
                    yield return new StructField_v1
                    {
                        Offset = offset,
                        Name = nt.Name,
                        Type = nt.DataType,
                    };
                    offset += rawSize;
                }
            }
        }

        private IEnumerable<UnionAlternative_v1> ExpandUnionFields(IEnumerable<StructDecl> decls)
        {
            foreach (var decl in decls)
            {
                var ndte = new NamedDataTypeExtractor(platform, decl.SpecQualifierList, symbolTable);
                foreach (var declarator in decl.FieldDeclarators)
                {
                    var nt = ndte.GetNameAndType(declarator);
                    yield return new UnionAlternative_v1
                    {
                        Name = nt.Name,
                        Type = nt.DataType
                    };
                }
            }
        }

        private int Align(int offset, int size, int maxAlign)
        {
            size = Math.Min(maxAlign, size);
            if (size == 0)
                size = maxAlign;
            return size * ((offset + (size - 1)) / size);
        }

        public SerializedType VisitStorageClass(StorageClassSpec storageClassSpec)
        {
            switch (storageClassSpec.Type)
            {
            case CTokenType.__Cdecl:
            case CTokenType.__Fastcall:
            case CTokenType.__Stdcall:
            case CTokenType.__Thiscall:
                ApplyCallConvention(storageClassSpec.Type);
                break;
            }
            return dt;       //$TODO make use of CDECL.
        }

        public SerializedType VisitExtendedDeclspec(ExtendedDeclspec declspec)
        {
            return null;
        }

        public SerializedType VisitTypeQualifier(TypeQualifier typeQualifier)
        {
            return dt;      //$TODO: Ignoring 'const' and 'volatile' for now.
        }
    }
}
