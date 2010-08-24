using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Tools.C2Xml
{
    public class CToken
    {
        public readonly Token Token;
        public readonly string Value;

        public CToken(Token tok)
        {
            Token = tok;
        }

        public CToken(Token tok, string str)
        {
            Token = tok;
            Value = str;
        }
    }

    public class CLexer : IEnumerable<CToken>
    {
        private TextReader rdr;

        public CLexer(TextReader rdr)
        {
            this.rdr = rdr;
        }

        public IEnumerator<CToken> GetEnumerator()
        {
            return GetTokens();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetTokens();
        }

        private IEnumerator<CToken> GetTokens()
        {
            int ch;
            int chPeek = -1;
            StringBuilder sb = new StringBuilder();

            State st = State.Initial;
            do
            {
                if (chPeek == -1)
                    ch = rdr.Read();
                else
                {
                    ch = chPeek;
                    chPeek = -1;
                }
                char c = (char)ch;
                switch (st)
                {
                case State.Initial:
                    if (ch == -1)
                    {
                        yield break;
                    }
                    if (Char.IsWhiteSpace(c))
                        continue;

                    if (Char.IsDigit(c))
                    {
                        sb = new StringBuilder();
                        sb.Append(c);
                        st = State.NumberBegin;
                    }
                    else if (Char.IsLetter(c) || c == '_')
                    {
                        sb = new StringBuilder();
                        sb.Append(c);
                        st = State.Identifier;
                    }
                    else
                        switch (c)
                        {
                        case '"':
                            sb = new StringBuilder();
                            st = State.String;
                            break;
                        case '<':
                            st = State.Lt; break;
                        case '>':
                            st = State.Gt; break;
                        case '=':
                            st = State.Eq; break;
                        case '+':
                            st = State.Plus; break;
                        case '-':
                            st = State.Minus; break;
                        case '#':
                            st = State.Directive; break;
                        case '&':
                            st = State.Ampersand; break;
                        case '!':
                            st = State.Bang; break;
                        case '|':
                            st = State.Pipe; break;

                        case '{':
                            yield return new CToken(Token.OpenCurly);
                            break;
                        case '}':
                            yield return new CToken(Token.CloseCurly);
                            break;
                        case '(':
                            yield return new CToken(Token.OpenParen);
                            break;
                        case ')':
                            yield return new CToken(Token.CloseParen);
                            break;
                        case ';':
                            yield return new CToken(Token.Semi);
                            break;
                        case ',':
                            yield return new CToken(Token.Comma);
                            break;
                        case '[':
                            yield return new CToken(Token.OpenBracket);
                            break;
                        case ']':
                            yield return new CToken(Token.CloseBracket);
                            break;
                        case '*':
                            yield return new CToken(Token.Asterisk);
                            break;
                        case ':':
                            yield return new CToken(Token.Colon);
                            break;
                        case '?':
                            yield return new CToken(Token.Question);
                            break;
                        case '.':
                            yield return new CToken(Token.Dot);
                            break;
                        case '^':
                            yield return new CToken(Token.Caret);
                            break;
                        default:
                            throw new NotImplementedException(string.Format("Not handled: '{0}' (U+{1:X4}).", c, ch));
                        }
                    break;
                case State.NumberBegin:
                    if (Char.IsDigit(c))
                    {
                        st = State.DecimalNumber;
                        sb.Append(c);
                    }
                    else if (c == 'x' || c == 'X')
                    {
                        st = State.HexNumber;
                        sb = new StringBuilder();
                    }
                    else
                    {
                        yield return new CToken(Token.DecimalNumber, sb.ToString());
                        chPeek = ch;
                        st = State.Initial;
                    }
                    break;
                case State.DecimalNumber:
                    if (!Char.IsDigit(c))
                    {
                        yield return new CToken(Token.DecimalNumber, sb.ToString());
                        chPeek = ch;
                        st = State.Initial;
                    }
                    break;
                case State.HexNumber:
                    if ("0123456789ABCDEFabcdef".IndexOf(c) < 0)
                    {
                        yield return new CToken(Token.HexNumber, sb.ToString());
                        chPeek = ch;
                        st = State.Initial;
                    }
                    break;
                case State.Identifier:
                    if (!Char.IsLetterOrDigit(c) && c != '_')
                    {
                        yield return PossibleKeyword(sb);
                        chPeek = ch;
                        st = State.Initial;
                    }
                    break;
                case State.String:
                    if (c == '"')
                    {
                        yield return new CToken(Token.StringLiteral, sb.ToString());
                        st = State.Initial;
                    }
                    break;
                case State.Eq:
                    if (c == '=')
                    {
                        yield return new CToken(Token.Equals);
                    }
                    else
                    {
                        yield return new CToken(Token.Assign);
                        chPeek = ch;
                    }
                    st = State.Initial;
                    break;
                case State.Lt:
                    if (c == '<')
                    {
                        yield return new CToken(Token.Shl);
                    }
                    else if (c == '=')
                    {
                        yield return new CToken(Token.Le);
                    }
                    else
                    {
                        yield return new CToken(Token.Lt);
                        ch = chPeek;
                    }
                    st = State.Initial;
                    break;
                case State.Gt:
                    if (c == '>')
                    {
                        yield return new CToken(Token.Shr);
                    }
                    else if (c == '=')
                    {
                        yield return new CToken(Token.Ge);
                    }
                    {
                        yield return new CToken(Token.Gt);
                        ch = chPeek;
                    }
                    st = State.Initial;
                    break;
                case State.Minus:
                    if (c == '-')
                    {
                        yield return new CToken(Token.Dec);
                    }
                    else
                    {
                        yield return new CToken(Token.Minus);
                        ch = chPeek;
                    }
                    st = State.Initial;
                    break;
                case State.Plus:
                    if (c == '+')
                    {
                        yield return new CToken(Token.Inc);
                    }
                    else
                    {
                        yield return new CToken(Token.Plus);
                        ch = chPeek;
                    }
                    st = State.Initial;
                    break;
                case State.Ampersand:
                    if (c == '&')
                    {
                        yield return new CToken(Token.Cand);
                    }
                    else
                    {
                        yield return new CToken(Token.And);
                        ch = chPeek;

                    }
                    st = State.Initial;
                    break;
                case State.Pipe:
                    if (c == '|')
                    {
                        yield return new CToken(Token.Cor);
                    }
                    else
                    {
                        yield return new CToken(Token.Or);
                        ch = chPeek;

                    }
                    st = State.Initial;
                    break;

                case State.Bang:
                    if (c == '=')
                    {
                        yield return new CToken(Token.Ne);
                    }
                    else
                    {
                        yield return new CToken(Token.Not);
                        ch = chPeek;
                    }
                    st = State.Initial;
                    break;

                case State.Directive:
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unhandled state {0}", st));
                }
            } while (ch != -1);
        }

        private CToken PossibleKeyword(StringBuilder sb)
        {
            string str = sb.ToString();
            switch (str)
            {
            case "typedef":
                return new CToken(Token.Typedef);
            default:
                return new CToken(Token.Id, str);
            }
        }

        private enum State
        {
            Initial, NumberBegin, DecimalNumber, HexNumber, Identifier, String,
            Lt, Gt, Eq, Plus, Minus, Directive, Ampersand, Pipe, Bang,
        }

    }

    public enum Token
    {
        EOF,
        And,
        Assign,
        Cand,
        Caret,
        Char,
        CloseBracket,
        CloseCurly,
        CloseParen,
        Colon,
        Comma,
        Cor,
        Dec,
        DecimalNumber,
        Dot,
        Equals,
        Ge,
        Gt,
        HexNumber,
        Id,
        Inc,
        Int,
        Le,
        Long,
        Lt,
        Minus,
        Ne,
        Not,
        OpenBracket,
        OpenCurly,
        OpenParen,
        Or,
        Plus,
        Question,
        Semi,
        Shl,
        Short,
        Shr,
        Asterisk,
        Signed,
        Struct,
        StringLiteral,
        Typedef,
        Union,
        Unsigned,
        Void,
    }

}
