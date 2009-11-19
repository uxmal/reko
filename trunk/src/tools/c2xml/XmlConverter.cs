using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Decompiler.Tools.C2Xml
{
    class XmlConverter
    {
        private TextReader rdr;
        private XmlWriter writer;
        private CToken lastToken;


        public XmlConverter(TextReader rdr, TextWriter writer)
        {
            this.rdr = rdr;
            this.writer = new XmlTextWriter(writer); 
        }

        public void Convert()
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("c2xml");
            int lineNumber = 1;
            for (string line = rdr.ReadLine(); line != null;  line = rdr.ReadLine())
            {
                try
                {
                    if (line.StartsWith("#line") || line.StartsWith("#pragma"))
                        continue;
                    Tokenize(line);
                    ++lineNumber;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error on line {0}. {1}", lineNumber, ex.Message);
                }
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        public void Convertq()
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("c2xml");

            CParser parser = new CParser(GetTokens());
            XmlRenderer renderer = new XmlRenderer(writer);
            foreach (ExternalDeclaration decl in parser.ParseTranslationUnit())
            {
                decl.Accept(renderer);
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();

        }
        private enum State
        {
            Initial, NumberBegin, DecimalNumber, HexNumber, Identifier, String,
            Lt, Gt, Eq, Plus, Minus, Directive, Ampersand, Pipe, Bang,
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


        private void Tokenize(string line)
        {


            int i = 0;
            int iTokStart = 0;
            int ch;
            State st = State.Initial;
            do
            {
                ch = i < line.Length ? line[i++] : -1;
                char c = (char)ch;
                switch (st)
                {
                case State.Initial:
                    if (ch == -1)
                        return;
                    if (Char.IsWhiteSpace(c))
                        continue;

                    if (Char.IsDigit(c))
                    {
                        iTokStart = i-1;
                        st = State.NumberBegin;
                    }
                    else if (Char.IsLetter(c) || ch == '_')
                    {
                        iTokStart = i-1;
                        st = State.Identifier;
                    }
                    else
                        switch (c)
                        {
                        case '"':
                            iTokStart = i;
                            st = State.String;
                            break;
                        case '<':
                            st = State.Lt; break;
                        case '>':
                            st = State.Gt; break;
                        case '=':
                            st = State.Eq; break;
                        case  '+':
                            st = State.Plus; break;
                        case '-':
                            st = State.Minus; break;
                        case '#':
                            st = State.Directive; break;
                        case '&':
                            st = State.Ampersand; break;
                        case '!':
                            st = State.Bang; break;

                        case '{':
                            writer.WriteStartElement("br"); break;
                        case '}':
                            writer.WriteEndElement(); break;
                        case '(':
                            writer.WriteStartElement("p"); break;
                        case ')':
                            writer.WriteEndElement(); break;
                        case ';':
                            writer.WriteStartElement("s"); writer.WriteEndElement(); break;
                        case ',':
                            writer.WriteStartElement("c"); writer.WriteEndElement(); break;
                        case '[':
                            writer.WriteStartElement("sq"); break;
                        case ']':
                            writer.WriteEndElement(); break;
                        case '*':
                            writer.WriteStartElement("st"); writer.WriteEndElement(); break;
                        case '|':
                            writer.WriteStartElement("pi"); writer.WriteEndElement(); break;
                        case ':':
                            writer.WriteStartElement("co"); writer.WriteEndElement(); break;
                        case '?':
                            writer.WriteStartElement("qu"); writer.WriteEndElement(); break;
                        case '.':
                            WriteSingleton("dot"); break;
                        case '^':
                            WriteSingleton("ca"); break;
                        default:
                            throw new NotImplementedException(string.Format("Not handled: '{0}' (U+{1:X4}).", c, ch));
                        }
                    break;
                case State.NumberBegin:
                    if (Char.IsDigit(c))
                    {
                        st = State.DecimalNumber;
                    }
                    else if (c == 'x' || c == 'X')
                    {
                        st = State.HexNumber;
                    }
                    else
                    {
                        Emit("n", line, iTokStart, --i);
                        st = State.Initial;
                    }
                    break;
                case State.DecimalNumber:
                    if (!Char.IsDigit(c))
                    {
                        Emit("n", line, iTokStart, --i);
                        st = State.Initial;
                    }
                    break;
                case State.HexNumber:
                    if ("0123456789ABCDEFabcdef".IndexOf(c) < 0)
                    {
                        Emit("x", line, iTokStart, --i);
                        st = State.Initial;
                    }
                    break;
                case State.Identifier:
                    if (!Char.IsLetterOrDigit(c) && c != '_')
                    {
                        Emit("i", line, iTokStart, --i);
                        st = State.Initial;
                    }
                    break;
                case State.String:
                    if (c == '"')
                    {
                        Emit("str", line, iTokStart, i - 1);
                        st = State.Initial;
                    }
                    break;
                case State.Eq:
                    if (c == '=')
                    {
                        writer.WriteStartElement("eq"); writer.WriteEndElement();
                    }
                    else
                    {
                        --i;
                        writer.WriteStartElement("as"); writer.WriteEndElement();
                    }
                    st = State.Initial;
                    break;
                case State.Lt:
                    if (c == '<')
                    {
                        writer.WriteStartElement("shl"); writer.WriteEndElement();
                    }
                    else if (c == '=')
                    {
                        WriteSingleton("le");
                    }
                    else 
                    {
                        --i;
                        writer.WriteStartElement("lt"); writer.WriteEndElement();
                    }
                    st = State.Initial;
                    break;
                case State.Gt:
                    if (c == '>')
                    {
                        WriteSingleton("shr");
                    }
                    else
                    {
                        --i;
                        WriteSingleton("gt");
                    }
                    st = State.Initial;
                    break;
                case State.Minus:
                    if (c == '-')
                    {
                        WriteSingleton("dec");
                    }
                    else
                    {
                        --i;
                        WriteSingleton("mi");
                    }
                    st = State.Initial;
                    break;
                case State.Plus:
                    if (c == '+')
                    {
                        WriteSingleton("inc");
                    }
                    else
                    {
                        --i;
                        WriteSingleton("pl");
                    }
                    st = State.Initial;
                    break;
                case State.Ampersand:
                    if (c == '&')
                    {
                        WriteSingleton("cand");
                    }
                    else
                    {
                        WriteSingleton("and");
                        --i;
                    }
                    st = State.Initial;
                    break;
                case State.Bang:
                    if (c == '=')
                    {
                        WriteSingleton("en");
                    }
                    else
                    {
                        WriteSingleton("not");
                        --i;
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

        private void Emit(string tag, string line, int iTokStart, int i)
        {
            writer.WriteStartElement(tag);
            writer.WriteString(line.Substring(iTokStart, i - iTokStart));
            writer.WriteEndElement();
        }

        private void WriteSingleton(string tag)
        {
            writer.WriteStartElement(tag);
            writer.WriteEndElement();
        }

    }
}
