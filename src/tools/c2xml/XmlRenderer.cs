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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Decompiler.Tools.C2Xml
{
   public class XmlRenderer : CSyntaxVisitor<CSyntax>
   {
       XmlWriter writer;

       public XmlRenderer(XmlWriter writer)
       {
           this.writer = writer;
       }

       public CSyntax VisitType(CType cType)
       {
           throw new NotImplementedException();
       }

       public CSyntax VisitDeclaration(Decl decl)
       {
           throw new NotImplementedException();
       }

       public CSyntax VisitEnumerator(Enumerator enumerator)
       {
           throw new NotImplementedException();
       }

       public CSyntax VisitStatement(Stat stm)
       {
           throw new NotImplementedException();
       }

       public CSyntax VisitExpression(CExpression stm)
       {
           throw new NotImplementedException();
       }

       public CSyntax VisitInitDeclarator(InitDeclarator initDeclarator)
       {
           throw new NotImplementedException();
       }

       public object VisitParamDeclaration(ParamDecl paramDecl)
       {
           throw new NotImplementedException();
       }


       public CSyntax VisitDeclSpec(DeclSpec declSpec)
       {
           throw new NotImplementedException();
       }

       CSyntax CSyntaxVisitor<CSyntax>.VisitParamDeclaration(ParamDecl paramDecl)
       {
           throw new NotImplementedException();
       }
   }
}
