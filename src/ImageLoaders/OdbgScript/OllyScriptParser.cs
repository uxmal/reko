#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Reko.ImageLoaders.OdbgScript
{
    /// <summary>
    /// This class parses an extended version of OllyScript 
    /// (https://github.com/epsylon3/odbgscript/blob/master/doc/ODbgScript.txt). In addition
    /// to the language described there, it supports reading sequences: 
    ///  expr ':' expr ... ':' expr
    /// In particular, a sequence seg ':' offset is used to model segmented pointers 
    /// on x86 real mode.
    /// </summary>
    public class OllyScriptParser : IDisposable
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(OllyScriptParser), "OLLYDBG parser")
        {
            Level = TraceLevel.Warning
        };
        private static readonly UnknownType unk = new UnknownType();
        private static readonly ProcedureConstant interpolate = new ProcedureConstant(unk, new IntrinsicProcedure("Interpolate", false, unk, 1));
        private static readonly ProcedureConstant hexString = new ProcedureConstant(unk, new IntrinsicProcedure("HexString", false, unk, 1));

        private readonly string currentDir;
        private readonly IOdbgScriptHost host;
        private readonly IFileSystemService fsSvc;
        private Lexer lexer;
        private Stack<Lexer> lexerStack;
        private Token tok;
        private OllyScript script;

        public OllyScriptParser(TextReader rdr, string currentDir, IOdbgScriptHost host, IFileSystemService fsSvc)
        {
            this.host = host;
            this.fsSvc = fsSvc;
            this.script = new OllyScript();
            this.currentDir = currentDir;
            this.lexerStack = new Stack<Lexer>();
            this.lexerStack.Push(new Lexer(rdr));
            this.lexer = default!;
        }

        public void Dispose()
        {
            if (lexerStack is null)
                throw new ObjectDisposedException(nameof(OllyScriptParser));
            foreach (var lexer in lexerStack)
            {
                lexer.Dispose();
            }
            lexerStack = null!;
        }

        public OllyScript ParseScript()
        {
            int lastLineNumber = 0;

            while (lexerStack.Count > 0)
            {
                this.lexer = lexerStack.Pop();
                var line = Line();
                while (line is not null)
                {
                    while (lastLineNumber < line.LineNumber)
                    {
                        this.script.Lines.Add(new OllyScript.Line { LineNumber = lastLineNumber });
                        ++lastLineNumber;
                    }
                    this.script.Lines.Add(line);
                    ++lastLineNumber;
                    line = Line();
                }
                this.lexer.Dispose();
                this.lexer = null!;
            }

            var script = this.script;
            this.script = new OllyScript();
            return script;
        }

        private OllyScript.Line? Line()
        {
            var tok = GetToken();
            if (tok.Type == TokenType.EOF)
            {
                return null;
            }
            if (tok.Type == TokenType.Directive)
            {
                Directive(tok);
            }
            // Eat the label.
            if (tok.Type == TokenType.Id)
            {
                if (PeekAndDiscard(TokenType.Colon))
                {
                    script.Labels[(string) tok.Value!] = tok.LineNumber;
                    tok = GetToken();
                }
            }

            var line = new OllyScript.Line { LineNumber = tok.LineNumber };
            if (tok.Type == TokenType.Id)
            {
                line.Command = (string) tok.Value!;
                line.IsCommand = true;
                line.Args = ExpressionList();
                tok = GetToken();
            }
            if (tok.Type != TokenType.EOF && tok.Type != TokenType.Newline)
            {
                host.MsgError($"Unexpected token on line {tok.LineNumber + 1}.");
                do
                {
                    tok = GetToken();
                } while (tok.Type != TokenType.Newline && tok.Type != TokenType.EOF);
            }
            return line;
        }

        private void Directive(Token tok)
        {
            var directive = (string) tok.Value!;
            if (directive == "inc")
            {
                var tokPath = GetToken();
                if (tokPath.Type != TokenType.String)
                {
                    host.MsgError("Expected a path after #inc directive.");
                    return;
                }
                tok = GetToken();
                while (tok.Type != TokenType.EOF && tok.Type != TokenType.Newline)
                {
                    // Discard trailing garbage.
                    tok = GetToken();
                }
                this.lexerStack!.Push(lexer); 
                var path = (string) tokPath.Value!;
                if (!Path.IsPathRooted(path))
                {
                    path = Path.Combine(this.currentDir, path);
                }
                var rdr = fsSvc.CreateStreamReader(path, Encoding.UTF8);
                this.lexer = new Lexer(rdr);
            }
        }

        private Expression[] ExpressionList()
        {
            var exps = new List<Expression>();
            var exp = Expression();
            if (exp is null)
                return Array.Empty<Expression>();
            exps.Add(exp);
            while (PeekAndDiscard(TokenType.Comma))
            {
                exp = Expression();
                if (exp is null)
                    break;
                exps.Add(exp);
            }
            return exps.ToArray();
        }

        //exp ::=  sum
        //        | exp ':' sum

        private Expression? Expression()
        {
            var sums = new List<Expression>();
            var sum = Sum();
            if (sum is null)
                return null;
            sums.Add(sum);
            while (PeekAndDiscard(TokenType.Colon))
            {
                sum = Sum();
                if (sum is null)
                    return null;
                sums.Add(sum);
            }
            if (sums.Count == 1)
                return sums[0];
            else
                return new MkSequence(unk, sums.ToArray());
        }

        private Expression? OrExpr()
        {
            var left = XorExpr();
            if (left is null)
                return null;
            while (PeekAndDiscard(TokenType.Or))
            {
                var right = XorExpr();
                if (right is null)
                    return null;
                left = new BinaryExpression(Operator.Or, unk, left, right);
            }
            return left;
        }

        private Expression? XorExpr()
        {
            var left = AndExpr();
            if (left is null)
                return null;
            while (PeekAndDiscard(TokenType.Xor))
            {
                var right = AndExpr();
                if (right is null)
                    return null;
                left = new BinaryExpression(Operator.Xor, unk, left, right);
            }
            return left;
        }

        private Expression? AndExpr()
        {
            var left = ShiftExpr();
            if (left is null)
                return null;
            while (PeekAndDiscard(TokenType.And))
            {
                var right = ShiftExpr();
                if (right is null)
                    return null;
                left = new BinaryExpression(Operator.And, unk, left, right);
            }
            return left;
        }

        private Expression? ShiftExpr()
        {
            var left = Sum();
            if (left is null)
                return null;
            for (; ; )
            {
                var tok = PeekToken();
                BinaryOperator op;
                switch (tok.Type)
                {
                case TokenType.Shl:
                    op = Operator.Shl;
                    break;
                case TokenType.Shr:
                    op = Operator.Shr;
                    break;
                default:
                    return left;
                }
                GetToken();
                var right = Sum();
                if (right is null)
                    return null;
                left = new BinaryExpression(op, unk, left, right);
            }
        }

        //sum ::= term
        //        | sum '+'/'-' term

        private Expression? Sum()
        {
            var left = Term();
            if (left is null)
                return null;
            for (; ; )
            {
                var tok = PeekToken();
                BinaryOperator op;
                switch (tok.Type)
                {
                case TokenType.Plus:
                    op = Operator.IAdd;
                    break;
                case TokenType.Minus:
                    op = Operator.ISub;
                    break;
                default:
                    return left;
                }
                GetToken();
                var right = Term();
                if (right is null)
                    return null;
                left = new BinaryExpression(op, unk, left, right);
            }
        }

        private Expression? Term()
        {
            var left = Atom();
            if (left is null)
                return null;
            for (; ; )
            {
                var tok = PeekToken();
                BinaryOperator op;
                switch (tok.Type)
                {
                case TokenType.Times:
                    op = Operator.IMul;
                    break;
                case TokenType.Slash:
                    op = Operator.UDiv;
                    break;
                default:
                    return left;
                }
                GetToken();
                var right = Atom();
                if (right is null)
                    return null;
                left = new BinaryExpression(op, unk, left, right);
            }
        }

        //atom ::=     number
        //      |      string
        //      |     interpolated_string
        //      |     id
        //      |     exp : exp
        //      |     [ exp ]
        //      |     ( exp )
    
        private Expression? Atom()
        {
            var token = PeekToken();
            switch (token.Type)
            {
            case TokenType.EOF:
            case TokenType.Newline:
                return null;
            case TokenType.String:
                GetToken();
                return MkString(token);
            case TokenType.InterpolatedString:
                GetToken();
                return new Application( interpolate, unk, MkString(token));
            case TokenType.HexString:
                GetToken();
                return new Application( hexString, unk,  MkString(token));
            case TokenType.Id:
                GetToken();
                return new Identifier((string) token.Value!, new UnknownType(), MemoryStorage.Instance);
            case TokenType.Integer:
                GetToken();
                return Constant.UInt64(Convert.ToUInt64(token.Value));
            case TokenType.LBracket:
                GetToken();
                var ea = Expression();
                if (ea is null)
                    return null;    //$TODO: SyncTo(Comma, NewLine)
                if (!PeekAndDiscard(TokenType.RBracket))
                    return null;
                return new MemoryAccess(ea, new UnknownType());
            case TokenType.LParen:
                GetToken();
                var exp = Expression();
                if (exp is null)
                    return null;    //$TODO: sync.
                if (!PeekAndDiscard(TokenType.RParen))
                    return null;
                return exp;
            }
            throw new NotImplementedException($"Token {tok.Type} on line {tok.LineNumber} not implemented.");
        }

        private Token GetToken()
        {
            if (this.tok.Type == TokenType.EOF)
            {
                return lexer.Get();
            }
            var tmp = this.tok;
            this.tok = new Token(TokenType.EOF, 0, null);
            return tmp;
        }

        private Token PeekToken()
        {
            if (this.tok.Type == TokenType.EOF)
            {
                tok = lexer.Get();
            }
            return tok;
        }

        private bool PeekAndDiscard(TokenType type)
        {
            if (PeekToken().Type == type)
            {
                GetToken();
                return true;
            }
            return false;
        }

        private static StringConstant MkString(Token token)
        {
            return Constant.String((string) token.Value!, StringType.NullTerminated(PrimitiveType.Char));
        }

        public static OllyScriptParser FromFile(IOdbgScriptHost host, IFileSystemService fsSvc, string file, string? dir = null)
        {
            string cdir = Environment.CurrentDirectory;
            string curdir = Helper.pathfixup(cdir, true);
            string sdir;

            var path = Helper.pathfixup(file, false);
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(cdir, path);
            }
            if (string.IsNullOrEmpty(dir))
                sdir = Path.GetDirectoryName(path)!;
            else
                sdir = dir;
            return new OllyScriptParser(fsSvc.CreateStreamReader(path, Encoding.UTF8), sdir, host, fsSvc);
        }

        public static OllyScriptParser FromString(IOdbgScriptHost host, IFileSystemService fsSvc, string buff, string dir)
        {
            string curdir = Helper.pathfixup(Environment.CurrentDirectory, true);
            var sdir = dir ?? curdir;
            return new OllyScriptParser(new StringReader(buff), sdir, host, fsSvc);
        }

        public class Lexer : IDisposable
        {
            private readonly StringBuilder sb;
            private TextReader? rdr;
            private int lineNumber;

            public Lexer(TextReader rdr)
            {
                this.rdr = rdr;
                this.lineNumber = 0;
                this.sb = new StringBuilder();
            }

            public void Dispose()
            {
                if (rdr is not null)
                {
                    rdr.Dispose();
                    rdr = null;
                }
            }

            private enum State
            {
                Start,
                String,
                Cr,
                LineComment,
                Identifier,
                Number,
                HexNumber,
                Slash,
                BlockComment,
                BlockComment_Star,
                HexString,
                HexString_odd,
                Dollar,
                Directive,
                Hash
            }

            public Token Get()
            {
                var state = State.Start;
                this.sb.Clear();
                var lineStart = this.lineNumber;
                TokenType stringType = TokenType.EOF;
                for (; ; )
                {
                    int c = rdr!.Peek();
                    char ch = (char) c;
                    switch (state)
                    {
                    case State.Start:
                        switch (c)
                        {
                        case -1: return MakeToken(TokenType.EOF);
                        case ' ': rdr.Read(); break;
                        case '\t': rdr.Read(); break;
                        case '\r': rdr.Read(); state = State.Cr; break;
                        case '\n': rdr.Read(); return MakeNlToken();
                        case '"': rdr.Read(); stringType = TokenType.String; state = State.String; break;
                        case '#': rdr.Read(); state = State.Hash; break;
                        case ';': rdr.Read(); state = State.LineComment; break;
                        case '/': rdr.Read(); state = State.Slash; break;
                        case '$': rdr.Read(); state = State.Dollar; break;
                        case ':': rdr.Read(); return MakeToken(TokenType.Colon);
                        case ',': rdr.Read(); return MakeToken(TokenType.Comma);
                        case '[': rdr.Read(); return MakeToken(TokenType.LBracket);
                        case ']': rdr.Read(); return MakeToken(TokenType.RBracket);
                        case '(': rdr.Read(); return MakeToken(TokenType.LParen);
                        case '|': rdr.Read(); return MakeToken(TokenType.Or);
                        case '^': rdr.Read(); return MakeToken(TokenType.Xor);
                        case '&': rdr.Read(); return MakeToken(TokenType.And);
                        case '>': rdr.Read(); return MakeToken(TokenType.Shr);
                        case '<': rdr.Read(); return MakeToken(TokenType.Shl);
                        case '+': rdr.Read(); return MakeToken(TokenType.Plus);
                        case '-': rdr.Read(); return MakeToken(TokenType.Minus);
                        case '*': rdr.Read(); return MakeToken(TokenType.Times);
                        default:
                            if (Char.IsLetter(ch) || ch == '!')
                            {
                                rdr.Read();
                                sb.Append(ch);
                                state = State.Identifier;
                                break;
                            }
                            else if (char.IsDigit(ch))
                            {
                                rdr.Read();
                                sb.Append(ch);
                                state = State.Number;
                                break;
                            }
                            throw new NotImplementedException();
                        }
                        break;
                    case State.Dollar:
                        switch (c)
                        {
                        case -1: return MakeToken(TokenType.Dollar);
                        case '"': rdr.Read(); stringType = TokenType.InterpolatedString; state = State.String; break;
                        default:
                            if (Char.IsLetter(ch))
                            {
                                rdr.Read();
                                sb.Append('$');
                                sb.Append(ch);
                                state = State.Identifier;
                                break;
                            }
                            return MakeToken(TokenType.Dollar);
                        }
                        break;
                    case State.String:
                        switch (c)
                        {
                        case -1: return MakeToken(TokenType.Error, "String constant not terminated.");
                        case '"': rdr.Read(); return MakeToken(stringType, sb.ToString());
                        default: rdr.Read(); sb.Append(ch); break;
                        }
                        break;
                    case State.Cr:
                        switch (c)
                        {
                        case -1: return MakeNlToken();
                        case '\n': rdr.Read(); return MakeNlToken();
                        default: return MakeNlToken();
                        }
                    case State.LineComment:
                        switch (c)
                        {
                        case -1: return MakeToken(TokenType.EOF);
                        case '\r':
                            rdr.Read(); state = State.Cr; break;
                        case '\n':
                            rdr.Read(); return MakeNlToken();
                        default:
                            rdr.Read(); break;
                        }
                        break;
                    case State.Identifier:
                        if (c == -1 || !(char.IsLetterOrDigit(ch) || ch == '_' || ch == '.'))
                            return MakeToken(TokenType.Id, sb.ToString());
                        rdr.Read();
                        sb.Append(ch);
                        break;
                    case State.Number:
                        switch (c)
                        {
                        case -1: return MakeToken(TokenType.Integer, Convert.ToUInt64(sb.ToString(), 16));
                        case '.': rdr.Read(); return MakeToken(TokenType.Integer, Convert.ToUInt64(sb.ToString(), 10));
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            rdr.Read(); sb.Append(ch); break;
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                            rdr.Read(); sb.Append(ch); state = State.HexNumber; break;
                        default:
                            return MakeToken(TokenType.Integer, Convert.ToUInt64(sb.ToString(), 16));
                        }
                        break;
                    case State.HexNumber:
                        switch (c)
                        {
                        case -1: return MakeToken(TokenType.Integer, Convert.ToUInt64(sb.ToString(), 16));
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                            rdr.Read(); sb.Append(ch); break;
                        default:
                            return MakeToken(TokenType.Integer, Convert.ToUInt64(sb.ToString(), 16));
                        }
                        break;
                    case State.Slash:
                        switch (c)
                        {
                        case -1: return MakeToken(TokenType.Slash);
                        case '*': rdr.Read(); state = State.BlockComment; break;
                        case '/': rdr.Read(); state = State.LineComment; break;
                        default: rdr.Read(); return MakeToken(TokenType.Slash);
                        }
                        break;
                    case State.BlockComment:
                        switch (c)
                        {
                        case -1: return MakeToken(TokenType.Error, lineStart, "Unterminated comment.");
                        case '*': rdr.Read(); state = State.BlockComment_Star; break;
                        case '\r': rdr.Read(); if (rdr.Peek() == '\n') rdr.Read(); ++lineNumber; break;
                        case '\n': rdr.Read(); ++lineNumber; break;
                        default: rdr.Read(); break;
                        }
                        break;
                    case State.BlockComment_Star:
                        switch (c)
                        {
                        case -1: return MakeToken(TokenType.Error, lineStart, "Unterminated comment.");
                        case '/':
                            rdr.Read();
                            if (lineStart != lineNumber)
                                return new Token(TokenType.Newline, lineStart, null);
                            state = State.Start;
                            break;
                        default: rdr.Read(); break;
                        }
                        break;
                    case State.Hash:
                        switch (c)
                        {
                        case -1: return Error(lineStart, "Unterminated hex string.");
                        case 'i': rdr.Read(); sb.Append(ch); state = State.Directive; break;
                        default: state = State.HexString; break;
                        }
                        break;
                    case State.HexString:
                        switch (c)
                        {
                        case -1: return Error(lineStart, "Unterminated hex string.");
                        case '#': rdr.Read(); return MakeToken(TokenType.HexString, sb.ToString());
                        case '\r':
                            rdr.Read();
                            if (rdr.Peek() == '\n') rdr.Read();
                            ++lineNumber;
                            return Error(lineStart, "Unterminated hex string.");
                        case '\n':
                            rdr.Read(); ++lineNumber;
                            return Error(lineStart, "Unterminated hex string.");
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                        case '?':
                            rdr.Read(); sb.Append(ch); state = State.HexString_odd; break;
                        default:
                            rdr.Read(); return Error(lineStart, $"Unexpected character '{ch}' (U+{c:X4}) in hex string.");
                        }
                        break;
                    case State.HexString_odd:
                        switch (c)
                        {
                        case -1: return Error(lineStart, "Unterminated hex string.");
                        case '#': rdr.Read(); return Error(lineStart, "Hex strings must have an even number of characters.");
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                        case '?':
                            rdr.Read(); sb.Append(ch); state = State.HexString; break;
                        default:
                            rdr.Read(); return Error(lineStart, $"Unexpected character '{ch}' (U+{c:X4}) in hex string.");
                        }
                        break;
                    case State.Directive:
                        if (c == -1 || !char.IsLetter(ch))
                        {
                            return MakeToken(TokenType.Directive, sb.ToString());
                        }
                        rdr.Read();
                        sb.Append(ch);
                        break;
                    }
                }
            }

            // For tokens that may span multiple lines.
            private Token MakeToken(TokenType type, int lineNumber, object? value = null)
            {
                var tok = new Token(type, lineNumber, value);
                return tok;
            }

            private Token MakeNlToken()
            {
                var tok = new Token(TokenType.Newline, this.lineNumber, null);
                ++lineNumber;
                return tok;
            }

            private Token MakeToken(TokenType type, object? value = null)
            {
                var tok = new Token(type, this.lineNumber, value);
                Debug.Assert(type != TokenType.Newline);
                trace.Verbose("Olly: {0} ({1})", type, value!);
                return tok;
            }

            private Token Error(int lineNumber, string message)
            {
                var tok = new Token(TokenType.Error, lineNumber, message);
                return tok;
            }
        }

        public struct Token
        {
            public readonly TokenType Type;
            public readonly int LineNumber;
            public readonly object? Value;

            public Token(TokenType type, int lineNumber, object? value)
            {
                this.Type = type;
                this.LineNumber = lineNumber;
                this.Value = value;
            }

            public override string ToString()
            {
                return $"({Type}: {Value})";
            }
        }

        public enum TokenType
        {
            EOF,
            Id,
            String,
            InterpolatedString,
            HexString,
            Integer,
            LBracket,
            RBracket,
            Comma,
            Colon,
            Newline,
            Error,
            Slash,
            Dollar,
            LParen,
            RParen,
            Plus,
            Minus,
            Directive,
            Xor,
            Or,
            And,
            Shr,
            Shl,
            Times,
        }

    }
}
