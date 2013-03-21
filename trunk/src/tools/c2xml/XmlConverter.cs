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

using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;  
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.Tools.C2Xml
{
    public class XmlConverter : 
        CSyntaxVisitor<int>
    {
        private TextReader rdr;
        private XmlWriter writer;
        private ParserState parserState;
        private List<SerializedType> types;
        private List<SerializedProcedureBase> procs;

        public XmlConverter(TextReader rdr, XmlWriter writer)
        {
            this.rdr = rdr;
            this.writer = writer;
            this.parserState = new ParserState();
            this.types = new List<SerializedType>();
            this.procs = new List<SerializedProcedureBase>();
        }

        public void Convert()
        {
            var parserState = new ParserState();
            var lexer = new CLexer(rdr);
            var parser = new CParser(parserState, lexer);
            var declarations = parser.Parse();
            foreach (var decl in declarations)
            {
                decl.Accept(this);
            }

            var lib = new SerializedLibrary
            {
                Types = types.ToArray(),
                Procedures = procs.ToList(),
            };
            var ser = SerializedLibrary.CreateSerializer();
            ser.Serialize(writer, lib);
        }

        public int VisitType(CType cType)
        {
            throw new NotImplementedException();
        }

        public int VisitDeclaration(Decl decl)
        {
            var fndec = decl as FunctionDecl;
            if (fndec != null)
            {
                return 0;
            }

            IEnumerable<DeclSpec> declspecs = decl.decl_specs;
            var isTypedef = false;
            var scspec = decl.decl_specs[0] as StorageClassSpec;
            if (scspec != null && scspec.Type == CTokenType.Typedef)
            {
                declspecs = decl.decl_specs.Skip(1);
                isTypedef = true;
            }

            var t = declspecs.First() as ComplexTypeSpec;
            if (t != null)
            {
                if (t.Type == CTokenType.Struct)
                {
                    types.Add(ConvertStructure(t));
                }
                else if (t.Type == CTokenType.Union)
                {
                    types.Add(ConvertUnion(t));
                }
            }
            foreach (var declarator in decl.init_declarator_list)
            {
                var nt = NamedDataTypeExtractor.GetNameAndType(
                    declspecs,
                    declarator.Declarator,
                    parserState);
                var serType = nt.DataType;

                var sSig = nt.DataType as SerializedSignature;
                if (sSig != null)
                {
                    procs.Add(new SerializedProcedure
                    {
                        Name = nt.Name,
                        Signature = sSig,
                    });
                }
                if (isTypedef)
                {
                    //$REVIEW: make sure that if it already exists, types match
                    // but a real compiler would have validated that.
                    parserState.Typedefs[nt.Name] = serType;
                    types.Add(new SerializedTypedef
                    {
                        Name = nt.Name,
                        DataType = serType
                    });
                }
            }
            return 0;
        }

        private SerializedStructType ConvertStructure(ComplexTypeSpec cpxSpec)
        {
            int offset = 0;
            var str = new SerializedStructType
            {
                Name = cpxSpec.Name
            };
            if (cpxSpec.DeclList == null)
                return str;
            var fields = str.Fields;
            foreach (var strspec in cpxSpec.DeclList)
            {
                foreach (var declarator in strspec.FieldDeclarators)
                {
                    var nt = NamedDataTypeExtractor.GetNameAndType(
                        strspec.SpecQualifierList,
                        declarator.Declarator,
                        parserState);
                    fields.Add(new SerializedStructField(
                        offset,
                        nt.Name,
                        nt.DataType));
                    offset += nt.DataType.GetSize();
                }
            }
            return str;
        }

        private SerializedType ConvertUnion(ComplexTypeSpec cpxSpec)
        {
            var u = new SerializedUnionType
            {
                Name = cpxSpec.Name
            };
            if (cpxSpec.DeclList == null)
                return u;
            var alts = u.Alternatives;
            foreach (var uspec in cpxSpec.DeclList)
            {
                foreach (var declarator in uspec.FieldDeclarators)
                {
                    var nt = NamedDataTypeExtractor.GetNameAndType(
                        uspec.SpecQualifierList,
                        declarator.Declarator,
                        parserState);
                    alts.Add(new SerializedUnionAlternative
                    {
                        Name = nt.Name,
                        Type = nt.DataType,
                    });
                }
            }
            return u;
        }

        public int VisitDeclSpec(DeclSpec declSpec)
        {
            throw new NotImplementedException();
        }

        public int VisitInitDeclarator(InitDeclarator initDeclarator)
        {
            throw new NotImplementedException();
        }

        public int VisitEnumerator(Enumerator enumerator)
        {
            throw new NotImplementedException();
        }

        public int VisitStatement(Stat stm)
        {
            throw new NotImplementedException();
        }

        public int VisitExpression(CExpression stm)
        {
            throw new NotImplementedException();
        }

        public int VisitParamDeclaration(ParamDecl paramDecl)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitArray(ArrayType at)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitEquivalenceClass(EquivalenceClass eq)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitFunctionType(FunctionType ft)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitPrimitive(PrimitiveType pt)
        {
            return new SerializedPrimitiveType
            {
                Domain = pt.Domain,
                ByteSize = pt.Size,
            };
        }

        public SerializedType VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitPointer(Pointer ptr)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitStructure(StructureType str)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitTypeVar(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitUnion(UnionType ut)
        {
            throw new NotImplementedException();
        }

        public SerializedType VisitUnknownType(UnknownType ut)
        {
            throw new NotImplementedException();
        }
    }
}
