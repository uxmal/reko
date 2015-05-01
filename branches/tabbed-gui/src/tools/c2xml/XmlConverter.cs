#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
        private List<SerializedProcedureBase_v1> procs;

        public XmlConverter(TextReader rdr, XmlWriter writer)
        {
            this.rdr = rdr;
            this.writer = writer;
            this.parserState = new ParserState();
            this.Types = new List<SerializedType>();
            this.procs = new List<SerializedProcedureBase_v1>();
            this.Sizer = new TypeSizer(this);
            this.NamedTypes = new Dictionary<string, SerializedType>
            {
                { "size_t", new PrimitiveType_v1 { Domain=Domain.UnsignedInt, ByteSize=4 } },    //$BUGBUG: arch-dependent!
                { "va_list", new PrimitiveType_v1 { Domain=Domain.Pointer, ByteSize=4 } }, //$BUGBUG: arch-dependent!
            };
            StructsSeen = new Dictionary<string, SerializedStructType>();
            UnionsSeen = new Dictionary<string, UnionType_v1>();
            EnumsSeen = new Dictionary<string, SerializedEnumType>();
            Constants = new Dictionary<string, int>();
        }

        public List<SerializedType> Types { get; private set; }
        public Dictionary<string, SerializedStructType> StructsSeen { get; private set; }
        public Dictionary<string, UnionType_v1> UnionsSeen { get; private set; }
        public Dictionary<string, SerializedEnumType> EnumsSeen { get; private set; }
        public Dictionary<string, int> Constants { get; private set; }
        public Dictionary<string, SerializedType> NamedTypes { get; private set; }
        public TypeSizer Sizer { get; private set; }

        public void Convert()
        {
            var lexer = new CLexer(rdr);
            var parser = new CParser(parserState, lexer);
            var declarations = parser.Parse();
            foreach (var decl in declarations)
            {
                decl.Accept(this);
            }

            var lib = new SerializedLibrary
            {
                Types = Types.ToArray(),
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

         
            var ntde = new NamedDataTypeExtractor(declspecs, this);
            foreach (var declarator in decl.init_declarator_list)
            {
                var nt = ntde.GetNameAndType(declarator.Declarator);
                var serType = nt.DataType;

                var sSig = nt.DataType as SerializedSignature;
                if (sSig != null)
                {
                    procs.Add(new Procedure_v1
                    {
                        Name = nt.Name,
                        Signature = sSig,
                    });
                }
                if (isTypedef)
                {
                    //$REVIEW: should make sure that if the typedef already exists, 
                    // then the types match but a real compiler would have validated that.
                    parserState.Typedefs.Add(nt.Name);
                    var typedef = new SerializedTypedef
                    {
                        Name = nt.Name,
                        DataType = serType
                    };
                    Types.Add(typedef);
                    //$REVIEW: do we really need to check for consistence?
                    NamedTypes[typedef.Name] = serType;
                }
            }
            return 0;
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
            return new PrimitiveType_v1
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
