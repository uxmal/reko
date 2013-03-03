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

       #region CSyntaxVisitor<CSyntax> Members

       public CSyntax VisitTypedef(Typedef typedef)
       {
           throw new NotImplementedException();
       }

       public CSyntax VisitTypeSpecifier(TypeSpecifier typeSpecifier)
       {
           throw new NotImplementedException();
       }

       #endregion
    }
}
