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
        DeclSpecVisitor<DataType>
    {
        private IEnumerable<DeclSpec> specs;
        private DataType dt;
        private Domain domain;
        private int byteSize;
        private Hashtable typedefs;

        public NamedDataTypeExtractor(IEnumerable<DeclSpec> specs, Hashtable typedefs)
        {
            this.specs = specs;
            this.typedefs = typedefs;
            foreach (var declspec in specs)
            {
                dt = declspec.Accept(this);
            }
        }

        public static NamedDataType GetNameAndType(IEnumerable<DeclSpec> declspecs, Declarator declarator, Hashtable typedefs)
        {
            try
            {
                return declarator.Accept(new NamedDataTypeExtractor(declspecs, typedefs));
            }
            catch
            {
                Debug.Print("Horfed while processing {0}", declarator);
                throw;
            }
        }

        public NamedDataType VisitId(IdDeclarator id)
        {
            return new NamedDataType { Name = id.Name, DataType = dt };
        }


        public NamedDataType VisitArray(ArrayDeclarator array)
        {
            throw new NotImplementedException();
        }

        public NamedDataType VisitField(FieldDeclarator field)
        {
            throw new NotImplementedException();
        }

        public NamedDataType VisitPointer(PointerDeclarator pointer)
        {
            var nt = pointer.Pointee.Accept(this);
            nt.DataType = new Pointer(nt.DataType, 4);      //$TODO: architecture-specific pointer size.
            return nt;
        }

        public NamedDataType VisitFunction(FunctionDeclarator function)
        {
            throw new NotImplementedException();
        }

        public DataType VisitSimpleType(SimpleTypeSpec simpleType)
        {
            switch (simpleType.Type)
            {
            case CTokenType.Void:
                if (domain != Domain.None)
                    throw new FormatException(string.Format("Can't have 'void' after '{0}'.", domain));
                domain = Domain.Void;
                return PrimitiveType.Void;
            case CTokenType.__W64:
                return dt;      // Used by Microsoft compilers for 32->64 bit transition, deprecated.
            case CTokenType.Signed:
                if (domain != Domain.None)
                    throw new FormatException(string.Format("Can't have 'signed' after '{0}'.", domain));
                domain = Domain.SignedInt;
                byteSize = 4;                   // 'unsigned' == 'unsigned int'
                //$TODO: bitsize is platform-dependent. For instance, a 'long' is 32-bits on Windows x86-64 but 64-bits on 64-bit Unix
                return PrimitiveType.Create(domain, byteSize);
            case CTokenType.Unsigned:
                if (domain != Domain.None)
                    throw new FormatException(string.Format("Can't have 'unsigned' after '{0}'.", domain));
                domain = Domain.UnsignedInt;
                byteSize = 4;                   // 'unsigned' == 'unsigned int'
                //$TODO: bitsize is platform-dependent. For instance, a 'long' is 32-bits on Windows x86-64 but 64-bits on 64-bit Unix
                return PrimitiveType.Create(domain, byteSize);
            case CTokenType.Char:
                if (domain == Domain.None)
                    domain = Domain.Character;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                byteSize = 1;
                return PrimitiveType.Create(domain, byteSize);
            case CTokenType.Wchar_t:
                if (domain == Domain.None)
                    domain = Domain.Character;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                byteSize = 2;       //$TODO: this is different on Unix platforms.
                return PrimitiveType.Create(domain, byteSize);
            case CTokenType.Short:
                if (domain == Domain.None)
                    domain = Domain.SignedInt;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                byteSize = 2;
                return PrimitiveType.Create(domain, byteSize);
            case CTokenType.Int:
                if (domain == Domain.None)
                    domain = Domain.SignedInt;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                byteSize = 4;
                return PrimitiveType.Create(domain, byteSize);
            //$TODO: bitsize is platform-dependent. For instance, an 'int' is 32-bits on Windows x86-64 but 16-bits on MS-DOS
            case CTokenType.Long:
                if (domain == Domain.None)
                    domain = Domain.SignedInt;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                byteSize = 4;
                return PrimitiveType.Create(domain, byteSize);
            //$TODO: bitsize is platform-dependent. For instance, a 'long' is 32-bits on Windows x86-64 but 64-bits on 64-bit Unix
            case CTokenType.__Int64:
                if (domain == Domain.None)
                    domain = Domain.SignedInt;
                else if (domain != Domain.SignedInt && domain != Domain.UnsignedInt)
                    throw new FormatException(string.Format("Unexpected domain {0}", domain));
                byteSize = 8;
                return PrimitiveType.Create(domain, byteSize);
            case CTokenType.Float:
                if (domain != Domain.None)
                    throw new FormatException(string.Format("Unexpected domain {0} before float.", domain));
                domain = Domain.Real;
                byteSize = 8;
                return PrimitiveType.Create(domain, byteSize);
            }
            throw new NotImplementedException(string.Format("{0}", simpleType.Type));
        }

        public DataType VisitTypedef(TypeDefName typeDefName)
        {
            return (DataType) typedefs[typeDefName.Name];
        }

        public DataType VisitComplexType(ComplexTypeSpec complexTypeSpec)
        {
            if (complexTypeSpec.Type == CTokenType.Struct)
            {
                var str = new StructureType
                {
                    Name = complexTypeSpec.Name,
                };
                str.Fields.AddRange(ExpandStructFields(complexTypeSpec.DeclList));
                return str;
            }
            else if (complexTypeSpec.Type == CTokenType.Union)
            {
                var un = new UnionType
                {
                    Name = complexTypeSpec.Name,
                };
                un.Alternatives.AddRange(ExpandUnionFields(complexTypeSpec.DeclList));
                return un;
            }
            else
                throw new NotImplementedException();
        }

        private IEnumerable<StructureField> ExpandStructFields(IEnumerable<StructDecl> decls)
        {
            int offset = 0;
            foreach (var decl in decls)
            {
                foreach (var declarator in decl.FieldDeclarators)
                {
                    var nt = GetNameAndType(decl.SpecQualifierList, declarator, typedefs);
                    offset = Align(offset, nt.DataType, 8);
                    yield return new StructureField(offset, nt.DataType, nt.Name);
                }
            }
        }

        private IEnumerable<UnionAlternative> ExpandUnionFields(IEnumerable<StructDecl> decls)
        {
            foreach (var decl in decls)
            {
                foreach (var declarator in decl.FieldDeclarators)
                {
                    var nt = GetNameAndType(decl.SpecQualifierList, declarator, typedefs);
                    yield return new UnionAlternative(nt.Name, nt.DataType);
                }
            }
        }

        private int Align(int offset, DataType dt, int maxAlign)
        {
            var size = Math.Min(maxAlign, dt.Size);
            return size * ((offset + (size - 1)) / size);
        }

        public DataType VisitStorageClass(StorageClassSpec storageClassSpec)
        {
            throw new NotImplementedException();
        }


        public DataType VisitTypeQualifier(TypeQualifier typeQualifier)
        {
            return dt;      //$TODO: Ignoring 'const' and 'volatile' for now.
        }

        public DataType VisitEnum(EnumeratorTypeSpec enumeratorTypeSpec)
        {
            throw new NotImplementedException();
        }
    }
}
