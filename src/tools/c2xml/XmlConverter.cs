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
        CSyntaxVisitor<int>,
        IDataTypeVisitor<SerializedType>
    {
        private TextReader rdr;
        private XmlWriter writer;
        private Hashtable mpNameType;
        private List<SerializedType> types;
        private List<SerializedProcedureBase> procs;

        public XmlConverter(TextReader rdr, XmlWriter writer)
        {
            this.rdr = rdr;
            this.writer = writer;
            this.mpNameType = new Hashtable();
            this.types = new List<SerializedType>();
            this.procs = new List<SerializedProcedureBase>();
        }

        public void Convert()
        {
            var lexer = new CLexer(rdr);
            var parser = new CParser(lexer);
            var declarations = parser.Parse();
            foreach (var decl in declarations)
            {
                decl.Accept(this);
            }

            var lib = new SerializedLibrary
            {
                Types = types.ToArray(),
                Procedures = procs,
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
            var scspec = decl.decl_specs[0] as StorageClassSpec;
            if (scspec != null && scspec.Type == CTokenType.Typedef)
            {
                foreach (var declarator in decl.init_declarator_list)
                {
                    var nt = NamedDataTypeExtractor.GetNameAndType(decl.decl_specs.Skip(1), declarator.Declarator, mpNameType);
                    var serType = nt.DataType.Accept(this);
                    mpNameType.Add(nt, serType);
                    types.Add(new SerializedTypedef
                    {
                        Name = nt.Name,
                        DataType = serType
                    });
                }
                return 0;
            }
            throw new NotImplementedException(decl.ToString());
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
