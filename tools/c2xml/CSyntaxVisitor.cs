using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Tools.C2Xml
{
    public interface CSyntaxVisitor<T>
    {
        T VisitTypedef(Typedef typedef);


        T VisitTypeSpecifier(TypeSpecifier typeSpecifier);
    }
}
