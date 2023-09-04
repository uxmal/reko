#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Lexer that deals with #pragma, #line and other preprocessor directives that can happen at any time.
    /// Each directive occupies one logical line, terminated by a newline.
    /// </summary>
    /// <remarks>
    /// This class performs the processing of phases 4 and 5 of described here:
    /// https://en.cppreference.com/w/c/language/translation_phases
    /// </remarks>
    public class CDirectiveLexer
    {
        private readonly Dictionary<string, Directive> directives = new()
        {
            { "define", Directive.Define },
            { "ifdef", Directive.Ifdef },
            { "ifndef", Directive.Ifndef },
            { "else", Directive.Else },
            { "endif", Directive.Endif },
            { "line", Directive.Line },
            { "pragma", Directive.Pragma },
            { "__pragma", Directive.Pragma },
            { "undef", Directive.Undef },
        };

        private readonly ParserState parserState;
        private readonly Dictionary<string, List<CToken>> macros;
        private readonly CLexer lexer;
        private readonly Stack<(bool prev, bool total)> ifdefs;
        private IEnumerator<CToken>? expandedTokens;
        private State state;
        private bool ignoreTokens;

        public CDirectiveLexer(ParserState state, CLexer lexer)
        {
            this.parserState = state;
            this.lexer = lexer;
            this.macros = new();
            this.state = State.StartLine;
            this.ifdefs = new Stack<(bool, bool)>();
            this.ignoreTokens = false;
        }

        public int LineNumber => lexer.LineNumber;

        private enum State
        {
            StartLine,
            InsideLine,
            Hash,
            IgnoreToEndOfLine,
        }

        private enum Directive
        {
            Define,
            Else,
            Endif,
            Ifdef,
            Ifndef,
            Line,
            Pragma,
            Undef,
        }

        public CToken Read()
        {
            var token = ReadToken();
            bool startIgnoring;
            for (; ; )
            {
                switch (state)
                {
                case State.StartLine:
                    switch (token.Type)
                    {
                    case CTokenType.NewLine:
                        token = ReadToken();
                        break;
                    case CTokenType.Hash:
                        state = State.Hash;
                        token = ReadToken();
                        break;
                    case CTokenType.Id:
                    case CTokenType.__Pragma:
                        state = State.InsideLine;
                        break;
                    default:
                        state = State.InsideLine;
                        if (ignoreTokens)
                        {
                            token = ReadToken();
                            break;
                        }
                        return token;
                    }
                    break;
                case State.InsideLine:
                    switch (token.Type)
                    {
                    case CTokenType.NewLine:
                        token = ReadToken();
                        state = State.StartLine;
                        break;
                    case CTokenType.Hash:
                        throw new FormatException("Preprocessor directive not allowed here.");
                    case CTokenType.Id:
                        var id = (string) token.Value!;
                        if (macros.TryGetValue(id, out var macro))
                        {
                            this.expandedTokens = macro.GetEnumerator();
                            token = ReadToken();
                            break;
                        }
                        if (ignoreTokens)
                        {
                            token = ReadToken();
                            break;
                        }
                        return token;
                    case CTokenType.__Pragma:
                        Expect(CTokenType.LParen);
                        ReadPragma((string) ReadToken().Value!, ignoreTokens);
                        token = ReadToken();
                        state = State.InsideLine;
                        break;
                    default:
                        if (ignoreTokens)
                        {
                            token = ReadToken();
                            break;
                        }
                        return token;
                    }
                    break;
                case State.Hash:
                    switch (token.Type)
                    {
                    case CTokenType.NewLine:
                        // Null directive is acceptable, and ignored.
                        token = ReadToken();
                        state = State.StartLine;
                        break;
                    case CTokenType.Id:
                        if (!directives.TryGetValue((string) token.Value!, out var directive))
                            throw new FormatException($"Unknown preprocessor directive '{token.Value!}'.");
                        switch (directive)
                        {
                        case Directive.Line:
                            Expect(CTokenType.NumericLiteral);
                            Expect(CTokenType.StringLiteral);
                            token = ReadToken();
                            state = State.StartLine;
                            break;
                        case Directive.Pragma:
                            token = ReadPragma((string) ReadToken().Value!, ignoreTokens);
                            state = State.StartLine;
                            break;
                        case Directive.Define:
                            token = ReadDefine(ignoreTokens);
                            state = State.StartLine;
                            break;
                        case Directive.Undef:
                            token = ReadUndef(ignoreTokens);
                            state = State.IgnoreToEndOfLine;
                            break;
                        case Directive.Ifdef:
                            var ifdefVar = (string) Expect(CTokenType.Id)!;
                            startIgnoring = !IsDefined(ifdefVar);
                            ifdefs.Push((startIgnoring, ignoreTokens));
                            ignoreTokens |= startIgnoring;
                            token = ReadToken();
                            state = State.IgnoreToEndOfLine;
                            break;
                        case Directive.Ifndef:
                            ifdefVar = (string) Expect(CTokenType.Id)!;
                            startIgnoring = IsDefined(ifdefVar);
                            ifdefs.Push((startIgnoring, ignoreTokens));
                            ignoreTokens |= startIgnoring;
                            token = ReadToken();
                            state = State.IgnoreToEndOfLine;
                            break;
                        case Directive.Endif:
                            if (ifdefs.Count == 0)
                                throw new FormatException($"Unbalanced #if/#endif");
                            (_, ignoreTokens) = ifdefs.Pop();
                            token = ReadToken();
                            state = State.IgnoreToEndOfLine;
                            break;
                        }
                        break;
                    case CTokenType.Else:
                        if (ifdefs.Count == 0)
                            throw new FormatException($"Unbalanced #if/#else");
                        state = State.StartLine;
                        (startIgnoring, ignoreTokens) = ifdefs.Peek();
                        ignoreTokens |= !startIgnoring;
                        token = ReadToken();
                        state = State.IgnoreToEndOfLine;
                        break;
                    default:
                        throw new FormatException($"Unexpected token {token.Type} on line {lexer.LineNumber}.");
                    }
                    break;
                case State.IgnoreToEndOfLine:
                    if (token.Type == CTokenType.EOF)
                        return token;
                    if (token.Type == CTokenType.NewLine)
                        state = State.StartLine;
                    token = ReadToken();
                    break;
                }
            }
        }

        private CToken ReadToken()
        {
            if (this.expandedTokens != null)
            {
                if (this.expandedTokens.MoveNext())
                    return this.expandedTokens.Current;
                this.expandedTokens = null;
            }
            return lexer.Read();
        }

        public CToken ReadDefine(bool ignoreTokens)
        {
            var macroName = (string) Expect(CTokenType.Id)!;
            //$TODO: arguments #define(A,B) A##B
            var tokens = new List<CToken>();
            for (; ; )
            {
                var token = ReadToken();
                if (token.Type == CTokenType.EOF || token.Type == CTokenType.NewLine)
                {
                    if (!ignoreTokens)
                    {
                        if (!this.macros.TryAdd(macroName, tokens))
                        {
                            //$TODO: emit redefinition warning.
                            this.macros[macroName] = tokens;
                        }
                    }
                    return token;
                }
                tokens.Add(token);
            }
        }

        public CToken ReadUndef(bool ignoreUndef)
        {
            var macroName = (string) Expect(CTokenType.Id)!;
            if (!ignoreUndef)
                this.macros.Remove(macroName);
            return ReadToken();
        }

        public virtual CToken ReadPragma(string pragma, bool ignorePragma)
        {
            if (pragma == "once")
            {
                return ReadToken();
            }
            else if (pragma == "warning")
            {
                Expect(CTokenType.LParen);
                var value = (string) Expect(CTokenType.Id)!;
                if (value == "disable" || value == "default")
                {
                    Expect(CTokenType.Colon);
                    Expect(CTokenType.NumericLiteral);
                    var token = ReadToken();
                    while (token.Type == CTokenType.NumericLiteral)
                    {
                        token = ReadToken();
                    }
                    Expect(CTokenType.RParen, token);
                    return ReadToken();
                }
                else if (value == "push" || value == "pop")
                {
                }
                Expect(CTokenType.RParen);
                return ReadToken();
            }
            else if (pragma == "prefast")
            {
                Expect(CTokenType.LParen);
                for (;;)
                {
                    var token = ReadToken();
                    if (token.Type == CTokenType.RParen)
                        break;
                }
                return ReadToken();
            }
            else if (pragma == "intrinsic" || pragma == "function")
            {
                Expect(CTokenType.LParen);
                Expect(CTokenType.Id);
                for (; ;)
                {
                    var tok = ReadToken();
                    if (tok.Type != CTokenType.Comma)
                    {
                        Expect(CTokenType.RParen, tok);
                        break;
                    }
                    Expect(CTokenType.Id);
                }
                return ReadToken();
            }
            else if (pragma == "pack")
            {
                Expect(CTokenType.LParen);
                var token = ReadToken();
                switch (token.Type)
                {
                case CTokenType.NumericLiteral:
                    if (!ignorePragma)
                        this.parserState.PushAlignment((int) token.Value!);
                    Expect(CTokenType.RParen);
                    Expect(CTokenType.NewLine);
                    break;
                case CTokenType.RParen:
                    if (!ignorePragma)
                        this.parserState.PopAlignment();
                    Expect(CTokenType.NewLine);
                    break;
                case CTokenType.Id:
                    var verb = (string) token.Value!;
                    if (verb == "push")
                    {
                        token = ReadToken();
                        if (token.Type == CTokenType.Comma)
                        {
                            var al = (int) Expect(CTokenType.NumericLiteral)!;
                            if (!ignorePragma)
                                this.parserState.PushAlignment(al);
                            token = ReadToken();
                        }
                        Expect(CTokenType.RParen, token);
                    }
                    else if (verb == "pop")
                    {
                        if (!ignorePragma)
                            this.parserState.PopAlignment();
                        Expect(CTokenType.RParen);
                    }
                    else
                        throw new FormatException(string.Format("Unknown verb {0}.", verb));
                    break;
                default:
                    throw new FormatException(string.Format("Unexpected token {0}.", token.Type));
                }
                return ReadToken();
            }
            else if (pragma == "deprecated")
            {
                Expect(CTokenType.LParen);
                Expect(CTokenType.Id);
                Expect(CTokenType.RParen);
                return ReadToken();
            }
            else if (pragma == "comment")
            {
                Expect(CTokenType.LParen);
                Expect(CTokenType.Id);
                Expect(CTokenType.Comma);
                Expect(CTokenType.StringLiteral);
                Expect(CTokenType.RParen);
                return ReadToken();
            }
            else if (pragma == "region" || pragma == "endregion")
            {
                lexer.SkipToNextLine();
                return ReadToken();
            }
            else
            {
                // Ignore unknown #pragmas
                lexer.SkipToNextLine();
                return ReadToken();
            }
        }


        private object? Expect(CTokenType expected)
        {
            var token = ReadToken();
            return Expect(expected, token);
        }

        private object? Expect(CTokenType expected, CToken actualToken)
        {
            if (actualToken.Type != expected)
                throw new FormatException(string.Format("Expected '{0}' but got '{1}' on line {2}.", expected, actualToken.Type, lexer.LineNumber));
            return actualToken.Value;
        }

        private bool IsDefined(string preprocessorVar)
        {
            return macros.ContainsKey(preprocessorVar);
        }
    }
}
