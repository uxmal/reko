using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Core.CLanguage
{
    /// <summary>
    /// Lexer phase that performs concatenation of C string literals.
    /// </summary>
    /// <remarks>
    /// This class performs the processing of phases 6 of described here:
    /// https://en.cppreference.com/w/c/language/translation_phases
    /// </remarks>
    public class StringConcatenator
    {
        private readonly CDirectiveLexer lexer;
        private readonly StringBuilder sb;
        private CToken tokenPrev;
        private CToken tokenString; // Only valid in firstString state.

        public int LineNumber => lexer.LineNumber;

        public StringConcatenator(CDirectiveLexer lexer)
        {
            this.lexer = lexer;
            this.sb = new StringBuilder();
            this.tokenPrev = new CToken(CTokenType.None);
        }

        private enum State
        {
            Start,
            FirstString,
            NextString,
        }

        public CToken Read()
        {
            var state = State.Start;
            Debug.Assert(sb.Length == 0);
            for (; ;)
            {
                var token = ReadToken();
                switch (state)
                {
                case State.Start:
                    if (token.Type != CTokenType.StringLiteral)
                        return token;
                    this.tokenString = token;
                    state = State.FirstString;
                    break;
                case State.FirstString:
                    if (token.Type != CTokenType.StringLiteral)
                    {
                        this.tokenPrev = token;
                        return tokenString;
                    }
                    sb.Append((string) tokenString.Value!);
                    sb.Append((string) token.Value!);
                    state = State.NextString;
                    break;
                case State.NextString:
                    if (token.Type != CTokenType.StringLiteral)
                    {
                        this.tokenPrev = token;
                        var concToken = new CToken(this.tokenString.Type, sb.ToString());
                        sb.Clear();
                        return concToken;
                    }
                    sb.Append((string) token.Value!);
                    break;
                }
            }
        }

        private CToken ReadToken()
        {
            if (this.tokenPrev.Type == CTokenType.None)
                return lexer.Read();
            var token = this.tokenPrev;
            this.tokenPrev = new CToken();
            return token;
        }
    }
}
