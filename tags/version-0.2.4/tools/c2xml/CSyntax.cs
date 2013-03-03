using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Tools.C2Xml
{
    public abstract class CSyntax
    {
        public abstract T Accept<T>(CSyntaxVisitor<T> visitor);
    }

    public class Declaration : CSyntax
    {
        public override T Accept<T>(CSyntaxVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
    public class Declarator : CSyntax
    {
        public override T Accept<T>(CSyntaxVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }
    }

    public class Initializer : CSyntax
    {
        public Declarator Declarator;
        public Initializer Init { get; set; }

        public override T Accept<T>(CSyntaxVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }
    }

    public class TypeSpecifier : CSyntax
    {
        public override T Accept<T>(CSyntaxVisitor<T> visitor)
        {
            return visitor.VisitTypeSpecifier(this);
        }
    }

    public class ExternalDeclaration : CSyntax
    {
        public override T Accept<T>(CSyntaxVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }
    }


    public class Typedef : ExternalDeclaration
    {
        public Declaration decl;

        public Typedef(Declaration decl)
        {
            this.decl = decl;
        }

        public Typedef(TypeSpecifier typeSpec, List<Initializer> decls)
        {
            TypeSpecifier = typeSpec;
            Declarators = decls;
        }

        public override T Accept<T>(CSyntaxVisitor<T> visitor)
        {
            return visitor.VisitTypedef(this);
        }

        public TypeSpecifier TypeSpecifier { get; private set;}
        public List<Initializer> Declarators { get; private set; }
    }
}
