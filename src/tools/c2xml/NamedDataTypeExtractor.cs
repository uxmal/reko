#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Types;
using Decompiler.Core.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Tools.C2Xml
{
    public class NamedDataTypeExtractor : 
        DeclaratorVisitor<NamedDataType>,
        DeclSpecVisitor<SerializedType>
    {
        private IEnumerable<DeclSpec> specs;
        private XmlConverter converter;
        private SerializedType dt;
        private Domain domain;
        private int byteSize;
        private CTokenType callingConvention;
        private CConstantEvaluator eval;

        public NamedDataTypeExtractor(IEnumerable<DeclSpec> specs, XmlConverter converter)
        {
            this.specs = specs;
            this.converter = converter;
            this.callingConvention = CTokenType.None;
            this.eval = new CConstantEvaluator(converter.Constants);
            foreach (var declspec in specs)
            {
                dt = declspec.Accept(this);
            }
        }

        public NamedDataType GetNameAndType(Declarator declarator)
        {
            if (declarator != null)
            {
                return declarator.Accept(this);
            }
            else
            {
                return new NamedDataType { DataType = dt };
            }
        }

        public NamedDataType VisitId(IdDeclarator id)
        {
            return new NamedDataType { Name = id.Name, DataType = dt, Size = byteSize };
        }

        public NamedDataType VisitArray(ArrayDeclarator array)
        {
            var nt = array.Declarator.Accept(this);
            return new NamedDataType
            {
                Name = nt.Name,
                DataType = new SerializedArrayType
                {
                    ElementType = nt.DataType,
                    Length = array.Size != null
                        ? Convert.ToInt32(array.Size.Accept(eval))
                        : 0
                }
            };
        }

        public NamedDataType VisitField(FieldDeclarator field)
        {
            if (field.Declarator == null)
            {
                return new NamedDataType
                {
                    DataType = dt,
                    Name = null
                };
            }
            else
            {
                return field.Declarator.Accept(this);
            }
        }

        public NamedDataType VisitPointer(PointerDeclarator pointer)
        {
            NamedDataType nt;
            if (pointer.Pointee != null)
            {
                nt = pointer.Pointee.Accept(this);
            }
            else 
            {
                nt = new NamedDataType { DataType = dt };
            }
            nt.DataType = new SerializedPointerType
            {
                DataType = nt.DataType,
                //$BUG: architecture-specific type to go here.
            };
            nt.Size = 4;            //$BUG: this is also architecture-specific (2 for PDP-11 for instance)
            return nt;
        }

        public NamedDataType VisitFunction(FunctionDeclarator function)
        {
            var nt = function.Declarator.Accept(this);
            var parameters =
                function.Parameters.Select(p => ConvertParameter(p));
            SerializedArgument ret = null;
            if (nt.DataType != null)
            {
                ret = new SerializedArgument
                {
                    Kind = new SerializedRegister { Name = "eax" },       //$REVIEW platform-specific.
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
            return nt;
        }

        private SerializedArgument ConvertParameter(ParamDecl decl)
        {
            if (decl.IsEllipsis)
            {
                return new SerializedArgument
                {
                    Kind = new SerializedStackVariable { },
                    Name = "...",
                };
            }
            else
            {
                var ntde = new NamedDataTypeExtractor(decl.DeclSpecs, converter);
                var nt = ntde.GetNameAndType(decl.Declarator);
                return new SerializedArgument
                {
                    Kind = new SerializedStackVariable { ByteSize = ToStackSize(nt.Size), },
                    Name = nt.Name,
                    Type = nt.DataType
                };
            }
        }

        private int ToStackSize(int p)
        {
            const int align = 4;
            //$REVIEW: depends on type and call convention + alignment
            return ((p + 1) / align) * align;
        }
        
        public NamedDataType VisitCallConvention(CallConventionDeclarator conv)
        {
            return conv.Declarator.Accept(this);
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
                domain = Domain.Void;
                byteSize = 0;
                return CreatePrimitive();
            case CTokenType.__W64:
                return dt;      // Used by Microsoft compilers for 32->64 bit transition, deprecated.
            case CTokenType.Signed:
                if (domain != Domain.None)
                    throw new FormatException(string.Format("Can't have 'signed' after '{0}'.", domain));
                domain = Domain.SignedInt;
                byteSize = 4;                   // 'unsigned' == 'unsigned int'
                //$TODO: bitsize is platform-dependent. For instance, a 'long' is 32-bits on Windows x86-64 but 64-bits on 64-bit Unix
                return CreatePrimitive();
            case CTokenType.Unsigned:
                if (domain != Domain.None)
                    throw new FormatException(string.Format("Can't have 'unsigned' after '{0}'.", domain));
                domain = Domain.UnsignedInt;
                byteSize = 4;                   // 'unsigned' == 'unsigned int'
                //$TODO: bitsize is platform-dependent. For instance, a 'long' is 32-bits on Windows x86-64 but 64-bits on 64-bit Unix
                return CreatePrimitive();
            case CTokenType.Char:
                if (domain == Domain.None)
                    domain = Domain.Character;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                byteSize = 1;
                return CreatePrimitive();
            case CTokenType.Wchar_t:
                if (domain == Domain.None)
                    domain = Domain.Character;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                byteSize = 2;       //$TODO: this is different on Unix platforms.
                return CreatePrimitive();
            case CTokenType.Short:
                if (domain == Domain.None)
                    domain = Domain.SignedInt;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                byteSize = 2;
                return CreatePrimitive();
            case CTokenType.Int:
                if (domain == Domain.None)
                    domain = Domain.SignedInt;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                byteSize = 4;
                return CreatePrimitive();
            //$TODO: bitsize is platform-dependent. For instance, an 'int' is 32-bits on Windows x86-64 but 16-bits on MS-DOS
            case CTokenType.Long:
                if (domain == Domain.None)
                    domain = Domain.SignedInt;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                byteSize = 4;
                return CreatePrimitive();
            //$TODO: bitsize is platform-dependent. For instance, a 'long' is 32-bits on Windows x86-64 but 64-bits on 64-bit Unix
            case CTokenType.__Int64:
                if (domain == Domain.None)
                    domain = Domain.SignedInt;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                byteSize = 8;
                return CreatePrimitive();
            case CTokenType.Float:
                if (domain != Domain.None)
                    throw new FormatException(string.Format("Unexpected domain {0} before float.", domain));
                domain = Domain.Real;
                byteSize = 4;
                return CreatePrimitive();
            case CTokenType.Double:
                if (domain != Domain.None)  //$REVIEW: short double? long double? long long double?
                    throw new FormatException(string.Format("Unexpected domain {0} before float.", domain));
                domain = Domain.Real;
                byteSize = 8;       //$REVIEW: arch-specific.
                return CreatePrimitive();
            }
        }

        private SerializedPrimitiveType CreatePrimitive()
        {
            return new SerializedPrimitiveType
            {
                Domain = domain,
                ByteSize = byteSize
            };
        }

        public SerializedType VisitTypedef(TypeDefName typeDefName)
        {
            byteSize = converter.NamedTypes[typeDefName.Name].Accept(converter.Sizer);
            return new SerializedTypeReference(typeDefName.Name);
        }

        public SerializedType VisitComplexType(ComplexTypeSpec complexType)
        {
            if (complexType.Type == CTokenType.Struct)
            {
                SerializedStructType str;
                if (complexType.Name == null || converter.StructsSeen.TryGetValue(complexType.Name, out str))
                {
                    str = new SerializedStructType {
                        Name = complexType.Name != null
                            ? complexType.Name
                            : string.Format("struct_{0}", converter.StructsSeen.Count)
                    };
                    converter.StructsSeen.Add(str.Name, str);
                }
                else
                {
                    str = new SerializedStructType { Name = complexType.Name };
                }
                if (!complexType.IsForwardDeclaration() && str.Fields == null)
                {
                    str.Fields = ExpandStructFields(complexType.DeclList).ToArray();
                    converter.Sizer.SetSize(str);
                    converter.Types.Add(str);
                    str = new SerializedStructType { Name = str.Name };
                }
                return str;
            }
            else if (complexType.Type == CTokenType.Union)
            {
                SerializedUnionType un;
                if (complexType.Name == null || !converter.UnionsSeen.TryGetValue(complexType.Name, out un))
                {
                    un = new SerializedUnionType { Name = complexType.Name };
                    if (un.Name != null)
                    {
                        converter.UnionsSeen.Add(un.Name, un);
                    }
                }
                if (!complexType.IsForwardDeclaration() && un.Alternatives == null)
                {
                    un.Alternatives = ExpandUnionFields(complexType.DeclList).ToArray();
                    converter.Sizer.SetSize(un);
                    if (un.Name != null)
                    {
                        converter.Types.Add(un);
                        un = new SerializedUnionType { Name = un.Name };
                    }
                }
                return un;
            }
            else
                throw new NotImplementedException();
        }

        public SerializedType VisitEnum(EnumeratorTypeSpec e)
        {
            SerializedEnumType en;
            if (e.Tag == null || !converter.EnumsSeen.TryGetValue(e.Tag, out en))
            {
                en = new SerializedEnumType {
                    Name = e.Tag != null 
                        ? e.Tag 
                        : string.Format("enum_{0}", converter.EnumsSeen.Count)
                };
                converter.EnumsSeen.Add(en.Name, en);
                var enumEvaluator = new EnumEvaluator(new CConstantEvaluator(converter.Constants));
                var listMembers = new List<SerializedEnumValue>();
                foreach (var item in e.Enums)
                {
                    var ee = new SerializedEnumValue
                    {
                        Name = item.Name,
                        Value = enumEvaluator.GetValue(item.Value),
                    };
                    converter.Constants.Add(ee.Name, ee.Value);
                    listMembers.Add(ee);
                }
                en.Values = listMembers.ToArray();
                converter.Types.Add(en);
                en = new SerializedEnumType { Name = en.Name };
            }
            else
            {
                en = new SerializedEnumType { Name = e.Tag };
            }
            return en;
        }

        private IEnumerable<SerializedStructField> ExpandStructFields(IEnumerable<StructDecl> decls)
        {
            int offset = 0;
            foreach (var decl in decls)
            {
                var ntde = new NamedDataTypeExtractor(decl.SpecQualifierList, converter);
                foreach (var declarator in decl.FieldDeclarators)
                {
                    var nt = ntde.GetNameAndType(declarator);
                    var rawSize = nt.DataType.Accept(converter.Sizer);
                    offset = Align(offset, rawSize, 8);     //$BUG: disregards temp. alignment changes. (__declspec(align))
                    yield return new SerializedStructField
                    {
                        Offset = offset,
                        Name = nt.Name,
                        Type = nt.DataType,
                    };
                    offset += rawSize;
                }
            }
        }

        private IEnumerable<SerializedUnionAlternative> ExpandUnionFields(IEnumerable<StructDecl> decls)
        {
            foreach (var decl in decls)
            {
                var ndte = new NamedDataTypeExtractor(decl.SpecQualifierList, converter);
                foreach (var declarator in decl.FieldDeclarators)
                {
                    var nt = ndte.GetNameAndType(declarator);
                    yield return new SerializedUnionAlternative
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
                if (callingConvention != CTokenType.None)
                    throw new FormatException(string.Format("Unexpected extra calling convetion specifier '{0}'.", callingConvention));
                callingConvention = storageClassSpec.Type;
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
