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
        IDataTypeVisitor<int>
    {
        private TextReader rdr;
        private XmlWriter writer;
        private Hashtable typedefs;

        public XmlConverter(TextReader rdr, TextWriter writer)
        {
            this.rdr = rdr;
            this.writer = new XmlTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };
            this.typedefs = new Hashtable();
        }

        public void Convert()
        {
            var lexer = new CLexer(rdr);
            var parser = new CParser(lexer);
            var declarations = parser.Parse();
            writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-16\"");
            writer.WriteStartElement("library");
            writer.WriteAttributeString("xmlns", SerializedLibrary.Namespace);
            foreach (var decl in declarations)
            {
                decl.Accept(this);
            }
            writer.WriteEndElement();
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
                    var nt = NamedDataTypeExtractor.GetNameAndType(decl.decl_specs.Skip(1), declarator.Declarator, typedefs);
                    writer.WriteStartElement("typedef");
                    writer.WriteAttributeString("name", nt.Name);
                    nt.DataType.Accept(this);

                    writer.WriteEndElement();
                    typedefs.Add(nt.Name, nt.DataType);
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

        public int VisitArray(ArrayType at)
        {
            throw new NotImplementedException();
        }

        public int VisitEquivalenceClass(EquivalenceClass eq)
        {
            throw new NotImplementedException();
        }

        public int VisitFunctionType(FunctionType ft)
        {
            throw new NotImplementedException();
        }

        public int VisitPrimitive(PrimitiveType pt)
        {
            writer.WriteStartElement("prim");
            writer.WriteAttributeString("domain", pt.Domain.ToString());
            writer.WriteAttributeString("size", pt.Size.ToString());
            writer.WriteEndElement();
            return 0;
        }

        public int VisitMemberPointer(MemberPointer memptr)
        {
            throw new NotImplementedException();
        }

        public int VisitPointer(Pointer ptr)
        {
            throw new NotImplementedException();
        }

        public int VisitStructure(StructureType str)
        {
            throw new NotImplementedException();
        }

        public int VisitTypeVar(TypeVariable tv)
        {
            throw new NotImplementedException();
        }

        public int VisitUnion(UnionType ut)
        {
            throw new NotImplementedException();
        }

        public int VisitUnknownType(UnknownType ut)
        {
            throw new NotImplementedException();
        }
    }
}
