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

        public IEnumerator<ExternalDeclaration> ParseTranslationUnit()
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
        }

        private Declaration ParseDeclaration()
        {
            List<TypeSpecifier> tspecifiers = new List<TypeSpecifier>();
            foreach (TypeSpecifier spec in ParseTypeSpecifiers())
            {
                tspecifiers.Add(spec);
            }
            List<Declarator> ParseDeclaratorList();
            Expect(Token.Semi);
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
                return new Initializer { Declarator = decl, Initializer = ParseInitializer() };
            }
            return new Initializer { Declarator = decl; init = null };
        }

private Declarator ParseDeclarator()
{
    if (PeekAndEat(Token.Star))
    {
    }
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
    }

}
