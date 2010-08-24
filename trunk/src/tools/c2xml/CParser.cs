using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Tools.C2Xml
{
    class CParser
    {
        private CToken lastToken;
        IEnumerator<CToken> rdr;
        public CParser(IEnumerator<CToken> rdr)
        {
            this.rdr = rdr;
        }

        public IEnumerable<ExternalDeclaration> ParseTranslationUnit()
        {
            ExternalDeclaration ext;
            while ((ext = ParseExternalDeclaration()) != null)
            {
                yield return ext;
            }
        }

        private ExternalDeclaration ParseExternalDeclaration()
        {
            if (PeekAndEat(Token.Typedef))
            {
                return new Typedef(ParseDeclaration());
            }
            Unexpected(lastToken.Token);
            return null;
        }

        private Declaration ParseDeclaration()
        {
            List<TypeSpecifier> tspecifiers = new List<TypeSpecifier>();
            foreach (TypeSpecifier spec in ParseTypeSpecifiers())
            {
                tspecifiers.Add(spec);
            }
            var declarators= ParseDeclaratorList();
            Expect(Token.Semi);
            return new Declaration();
        }

        private IEnumerable<TypeSpecifier> ParseTypeSpecifiers()
        {
            yield return ParseTypeSpecifier();
            while (PeekAndEat(Token.Comma))
            {
                yield return ParseTypeSpecifier();
            }
        }

        private List<Initializer> ParseDeclaratorList()
        {
            var declarators = new List<Initializer>();
            declarators.Add(ParseInitDeclarator());
            while (PeekAndEat(Token.Comma))
            {
                declarators.Add(ParseInitDeclarator());
            }
            return declarators;
        }


        private Initializer ParseInitDeclarator()
        {
            var decl = ParseDeclarator();
            if (PeekAndEat(Token.Assign))
            {
                return new Initializer { Declarator = decl, Init = ParseInitializer() };
            }
            return new Initializer { Declarator = decl, Init = null };
        }

        private Initializer ParseInitializer()
        {
            throw new NotImplementedException();
        }

private Declarator ParseDeclarator()
{
    if (PeekAndEat(Token.Asterisk))
    {
    }
    throw new NotImplementedException();
}

        private void Expect(Token token)
        {
            if (token != Get().Token)
                Unexpected(token);
        }


        private void Unexpected(Token token)
        {
            
            throw new NotSupportedException(string.Format("Didn't expect {0}", token));
        }

        private CToken Get()
        {
            CToken tok = Peek();
            lastToken = null;
            return tok;
        }

        private bool PeekAndEat(Token tokExpected)
        {
            if (Peek().Token == tokExpected)
            {
                Expect(tokExpected);
                return true;
            }
            return false;
        }

        private CToken Peek()
        {
            if (lastToken == null)
            {
                if (rdr.MoveNext())
                {
                    lastToken = rdr.Current;
                }
                else
                {
                    lastToken = new CToken(Token.EOF);
                }
            }
            return lastToken;
        }

        public Typedef ParseTypedef()
        {
            TypeSpecifier ts = ParseTypeSpecifier();
            List<Initializer> decls = ParseDeclaratorList();
            return new Typedef(ts, decls);

        }

        private TypeSpecifier ParseTypeSpecifier()
        {
            switch (Peek().Token)
            {
            case Token.Void:
            case Token.Char:
            case Token.Short:
            case Token.Int:
            case Token.Long:
            case Token.Float:
            case Token.Double:
            case Token.Signed:
            case Token.Unsigned:
                return PrimitiveTypeSpecifier(Get());
        type_specifier
            :
            VOID
                | CHAR
                | SHORT
                | INT
                | LONG
                | FLOAT
                | DOUBLE
                | SIGNED
                | UNSIGNED
                | struct_or_union_specifier
                | enum_specifier
                | TYPE_NAME
                ;
        }
    }

}
