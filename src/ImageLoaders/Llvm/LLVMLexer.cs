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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.LLVM
{
    public class LLVMLexer
    {
        private TextReader rdr;
        private StringBuilder sb;

        public LLVMLexer(TextReader rdr)
        {
            this.rdr = rdr;
            this.LineNumber = 1;
            this.sb = new StringBuilder();
        }

        public int LineNumber { get; private set; }

        private enum State
        {
            Start,
            Comment,
            Reserved,
            String,
            Id,
            c,
            CharArray,
            Zero,
            Decimal,
            HexStart,
            Hex,
            Dot1,
            Dot2,
            i,
            Minus,
            X86Fp_80Start,
            X86Fp_80,
            RealDecimal,
            RealExponentStart,
            RealExponent,
            RealExponentSign,
        }

        public Token GetToken()
        {
            sb.Clear();
            var st = State.Start;
            this.lineStart = LineNumber;
            EatWs();
            for (;;)
            {
                int c = rdr.Peek();
                char ch = (char)c;
                switch (st)
                {
                case State.Start:
                    switch (c)
                    {
                    case -1: return new Token(lineStart, TokenType.EOF);
                    case '"': rdr.Read(); sb = new StringBuilder(); st = State.String; break;
                    case ';': rdr.Read(); sb = new StringBuilder(); st = State.Comment; break;
                    case '@':
                    case '%': rdr.Read(); sb = new StringBuilder(); sb.Append(ch); st = State.Id; break;
                    case '=': rdr.Read(); return Tok(TokenType.EQ);
                    case '(': rdr.Read(); return Tok(TokenType.LPAREN);
                    case '[': rdr.Read(); return Tok(TokenType.LBRACKET);
                    case '{': rdr.Read(); return Tok(TokenType.LBRACE);
                    case ')': rdr.Read(); return Tok(TokenType.RPAREN);
                    case ']': rdr.Read(); return Tok(TokenType.RBRACKET);
                    case '}': rdr.Read(); return Tok(TokenType.RBRACE);
                    case '*': rdr.Read(); return Tok(TokenType.STAR);
                    case ',': rdr.Read(); return Tok(TokenType.COMMA);
                    case '#': rdr.Read(); return Tok(TokenType.HASH);
                    case 'c': rdr.Read(); st = State.c; break;
                    case 'i': rdr.Read(); sb = new StringBuilder(); sb.Append(ch); st = State.i; break;
                    case '0': rdr.Read(); sb = new StringBuilder(); sb.Append(ch); st = State.Zero; break;
                    case '-': rdr.Read(); st = State.Minus; break;
                    case '.': rdr.Read(); st = State.Dot1; break;
                    case '!': rdr.Read(); return Tok(TokenType.BANG);
                    default:
                        if (char.IsLetter(ch) || ch == '_')
                        {
                            rdr.Read();
                            sb = new StringBuilder(); sb.Append(ch); st = State.Reserved; break;
                        }
                        if (char.IsDigit(ch))
                        {
                            rdr.Read();
                            sb = new StringBuilder(); sb.Append(ch); st = State.Decimal; break;
                        }
                        throw new FormatException(
                            string.Format("Unexpected character '{0}' (U+{1:X4}) on line {2}.",
                                ch, c, LineNumber));
                    }
                    break;
                case State.Comment:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.Comment, sb);
                    case '\r':
                    case '\n':
                        EatWs();
                        return Tok(TokenType.Comment, sb);
                    default:
                        rdr.Read();
                        sb.Append(ch);
                        break;
                    }
                    break;
                case State.Reserved:
                    switch (c)
                    {
                    case -1: return ReservedWord(sb);
                    default:
                        if (char.IsLetterOrDigit(ch) || ch == '_')
                        {
                            rdr.Read();
                            sb.Append(ch);
                            break;
                        }
                        else
                        {
                            return ReservedWord(sb);
                        }
                    }
                    break;
                case State.String:
                    switch (c)
                    {
                    case -1: throw new FormatException("Unterminated string.");
                    case '"': rdr.Read(); return Tok(TokenType.String, sb);
                    default: rdr.Read(); sb.Append(ch); break;
                    }
                    break;
                case State.Id:
                    switch (c)
                    {
                    case -1: return Identifier(sb);
                    case '-':
                    case '_':
                    case '.':
                    case '$':
                        rdr.Read(); sb.Append(ch); break;
                    default:
                        if (char.IsLetterOrDigit(ch))
                        {
                            rdr.Read(); sb.Append(ch); break;
                        }
                        return Identifier(sb);
                    }
                    break;
                case State.c:
                    switch (c)
                    {
                    case -1: return ReservedWord("c");
                    case '"': rdr.Read(); sb = new StringBuilder(); st = State.CharArray; break;
                    default:
                        rdr.Read();
                        sb = new StringBuilder();
                        sb.Append('c');
                        sb.Append(ch);
                        st = State.Reserved;
                        break;
                    }
                    break;
                case State.i:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.IntType, sb);
                    default:
                        if (char.IsDigit(ch))
                        {
                            rdr.Read();
                            sb.Append(ch);
                        }
                        else if (char.IsLetter(ch) || ch == '_')
                        {
                            rdr.Read();
                            sb.Append(ch);
                            st = State.Reserved;
                        }
                        else
                        {
                            return Tok(TokenType.IntType, sb);
                        }
                        break;
                    }
                    break;
                case State.CharArray:
                    switch (c)
                    {
                    case -1: throw new FormatException("Unterminated character array.");
                    case '"': rdr.Read(); return Tok(TokenType.CharArray, sb);
                    default: rdr.Read(); sb.Append(ch); break;
                    }
                    break;
                case State.Zero:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.Integer, sb);
                    case 'x':
                    case 'X':
                        rdr.Read(); sb.Clear(); st = State.HexStart; break;
                    case '.':
                        rdr.Read(); sb.Append(ch); st = State.RealDecimal; break;
                    default:
                        if (char.IsDigit(ch))
                        {
                            rdr.Read(); sb.Append(ch); break;
                        }
                        else
                        {
                            return Tok(TokenType.Integer, sb);
                        }
                    }
                    break;
                case State.Decimal:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.Integer, sb);
                    case '.': rdr.Read(); sb.Append(ch); st = State.RealDecimal; break;
                    default:
                        if (char.IsDigit(ch))
                        {
                            rdr.Read(); sb.Append(ch); break;
                        }
                        else
                        {
                            return Tok(TokenType.Integer, sb);
                        }
                    }
                    break;
                case State.HexStart:
                    switch (c)
                    {
                    case -1: Unexpected("0x"); break;
                    case 'K': rdr.Read(); sb = new StringBuilder(); st = State.X86Fp_80Start; break;
                    default:
                        if (IsHexDigit(ch))
                        {
                            rdr.Read(); sb.Append(ch); st = State.Hex; break;
                        }
                        else
                        {
                            return Tok(TokenType.HexInteger, sb);
                        }
                    }
                    break;
                case State.Hex:
                    if (c == -1 || !IsHexDigit(ch))
                    {
                        return Tok(TokenType.HexInteger, sb);
                    }
                    else
                    {
                        rdr.Read(); sb.Append(ch);
                    }
                    break;
                case State.X86Fp_80Start:
                    if (c == -1 || !IsHexDigit(ch))
                    {
                        Unexpected("0xK");
                    }
                    else
                    {
                        rdr.Read(); sb.Append(ch); st = State.X86Fp_80; break;
                    }
                    break;
                case State.X86Fp_80:
                    if (c == -1 || !IsHexDigit(ch))
                    {
                        return Tok(TokenType.X86_fp80_Literal, sb);
                    }
                    else
                    {
                        rdr.Read(); sb.Append(ch);
                    }
                    break;
                case State.RealDecimal:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.DoubleLiteral, sb);
                    case 'e':
                    case 'E': rdr.Read(); sb.Append(ch); st = State.RealExponentStart; break;
                    default:
                        if (char.IsDigit(ch))
                        {
                            rdr.Read(); sb.Append(ch); break;
                        }
                        else
                        {
                            return Tok(TokenType.DoubleLiteral, sb);
                        }
                    }
                    break;
                case State.RealExponentStart:
                    switch (c)
                    {
                    case -1: Unexpected("e"); break;
                    case '+': case '-': rdr.Read(); sb.Append(ch); st = State.RealExponentSign; break;
                    default:
                        if (!char.IsDigit(ch))
                        {
                            Unexpected("e");
                        }
                        else
                        {
                            rdr.Read(); sb.Append(ch); st = State.RealExponent; break;
                        }
                        break;
                    }
                    break;
                case State.RealExponentSign:
                    switch (c)
                    {
                    case -1: Unexpected("e-"); break;
                    default:
                        if (!char.IsDigit(ch))
                        {
                            Unexpected("e-");
                        }
                        else
                        {
                            rdr.Read(); sb.Append(ch); st = State.RealExponent;
                        }
                        break;
                    }
                    break;
                case State.RealExponent:
                    if (c == -1 || !char.IsDigit(ch))
                    {
                        return Tok(TokenType.DoubleLiteral, sb);
                    }
                    else
                    {
                        rdr.Read(); sb.Append(ch);
                    }
                    break;
                case State.Dot1:
                    switch (c)
                    {
                    case -1: return Tok(TokenType.DOT);
                    case '.': rdr.Read(); st = State.Dot2; break;
                    default: return Tok(TokenType.DOT);
                    }
                    break;
                case State.Dot2:
                    switch (c)
                    {
                    case '.': rdr.Read(); return Tok(TokenType.ELLIPSIS);
                    default: Unexpected(".."); break;
                    }
                    break;
                case State.Minus:
                    if (char.IsDigit(ch))
                    {
                        rdr.Read();
                        sb = new StringBuilder();
                        sb.Append('-');
                        sb.Append(ch);
                        st = State.Decimal;
                        break;
                    }
                    Unexpected("-");
                    break;
                }
            }
        }

        private static bool IsHexDigit(char ch)
        {
            return "0123456789abcdefABCDEF".IndexOf(ch) >= 0;
        }

        private void Unexpected(string un)
        {
            throw new FormatException(string.Format("Unexpected string '{0}' on line {1}.", un, LineNumber));
        }

        private Token Identifier(StringBuilder sb)
        {
            if (sb.Length < 2)
            {
                throw new FormatException("Invalid identifier.");
            }
            var type = sb[0] == '@'
                ? TokenType.GlobalId
                : TokenType.LocalId;
            return new Token(this.lineStart, type, sb.ToString(1, sb.Length - 1));
        }

        private Token ReservedWord(string s)
        {
            return new Token(this.lineStart, reservedWords[s]);
        }

        private Token ReservedWord(StringBuilder sb)
        {
            TokenType type;
            if (!reservedWords.TryGetValue(sb.ToString(), out type))
            {
                //throw new FormatException(string.Format("Unknown reserved word '{0}'.", sb.ToString()));
                Debug.Print("Unknown reserved word '{0}'.", sb.ToString());
                return new Token(this.lineStart,TokenType.String, sb.ToString());
            }
            return new Token(this.lineStart, reservedWords[sb.ToString()]);
        }

        private Token Tok(TokenType type, StringBuilder? sb = null)
        {
            return new Token(lineStart, type, 
                sb is not null ? sb.ToString() : null);
        }

        private bool EatWs()
        {
            if (rdr is null)
                return false;
            int ch = rdr.Peek();
            while (ch >= 0 && Char.IsWhiteSpace((char)ch))
            {
                if (ch == '\n')
                    ++LineNumber;
                rdr.Read();
                ch = rdr.Peek();
            }
            return true;
        }

        static Dictionary<string, TokenType> reservedWords = new Dictionary<string, TokenType>
        {
            { "eq", TokenType.eq },
            { "ne", TokenType.ne },
            { "ugt", TokenType.ugt },
            { "uge", TokenType.uge },
            { "ult", TokenType.ult },
            { "ule", TokenType.ule },
            { "sgt", TokenType.sgt },
            { "sge", TokenType.sge },
            { "slt", TokenType.slt },
            { "sle", TokenType.sle },
            { "oeq", TokenType.oeq },
            { "ogt", TokenType.ogt },
            { "oge", TokenType.oge },
            { "olt", TokenType.olt },
            { "ole", TokenType.ole },
            { "one", TokenType.one },
            { "ord", TokenType.ord },
            { "ueq", TokenType.ueq },
            { "une", TokenType.une },
            { "uno", TokenType.uno },

            { "add", TokenType.add },
            { "align", TokenType.align },
            { "alloca", TokenType.alloca },
            { "and", TokenType.and },
            { "ashr", TokenType.ashr },
            { "attributes", TokenType.attributes },
            { "bitcast", TokenType.bitcast },
            { "br", TokenType.br },
            { "call", TokenType.call },
            { "common", TokenType.common },
            { "constant", TokenType.constant },
            { "datalayout", TokenType.datalayout },
            { "declare", TokenType.declare },
            { "define", TokenType.define },
            { "dereferenceable", TokenType.dereferenceable },
            { "double", TokenType.@double },
            { "exact", TokenType.exact },
            { "external", TokenType.external },
            { "extractvalue", TokenType.extractvalue },
            { "false", TokenType.@false },
            { "fadd", TokenType.fadd },
            { "fcmp", TokenType.fcmp },
            { "fdiv", TokenType.fdiv },
            { "fence", TokenType.fence },
            { "fmul", TokenType.fmul },
            { "fpext", TokenType.fpext },
            { "fptosi", TokenType.fptosi },
            { "fptoui", TokenType.fptoui },
            { "fptrunc", TokenType.fptrunc },
            { "fsub", TokenType.fsub },
            { "getelementptr", TokenType.getelementptr },
            { "global", TokenType.global },
            { "i1", TokenType.i1 },
            { "i8", TokenType.i8 },
            { "i16", TokenType.i16 },
            { "i32", TokenType.i32 },
            { "i64", TokenType.i64 },
            { "icmp", TokenType.icmp },
            { "inbounds", TokenType.inbounds },
            { "inrange", TokenType.inrange },
            { "internal", TokenType.@internal },
            { "inttoptr", TokenType.inttoptr },
            { "label", TokenType.label },
            { "load", TokenType.load },
            { "lshr", TokenType.lshr },
            { "mul", TokenType.mul },
            { "noalias", TokenType.noalias },
            { "nocapture", TokenType.nocapture },
            { "noinline", TokenType.noinline },
            { "noreturn", TokenType.noreturn },
            { "nounwind", TokenType.nounwind },
            { "nsw", TokenType.nsw },
            { "null", TokenType.@null },
            { "nuw", TokenType.nuw },
            { "opaque", TokenType.opaque },
            { "or", TokenType.or },
            { "phi", TokenType.phi },
            { "ptrtoint", TokenType.ptrtoint },
            { "private", TokenType.@private },
            { "readonly", TokenType.@readonly },
            { "ret", TokenType.ret },
            { "sdiv", TokenType.sdiv },
            { "select", TokenType.select },
            { "seq_cst", TokenType.seq_cst },
            { "sext", TokenType.sext },
            { "shl", TokenType.shl },
            { "signext", TokenType.signext },
            { "sitofp", TokenType.sitofp },
            { "source_filename", TokenType.source_filename },
            { "srem", TokenType.srem },
            { "store", TokenType.store },
            { "sub", TokenType.sub },
            { "switch", TokenType.@switch },
            { "target", TokenType.target },
            { "to", TokenType.to },
            { "triple", TokenType.triple },
            { "true", TokenType.@true },
            { "trunc", TokenType.trunc },
            { "type", TokenType.type },
            { "udiv", TokenType.udiv },
            { "unnamed_addr", TokenType.unnamed_addr },
            { "unreachable", TokenType.unreachable },
            { "urem", TokenType.urem },
            { "uwtable", TokenType.uwtable },
            { "void", TokenType.@void },
            { "volatile", TokenType.@volatile },
            { "writeonly", TokenType.writeonly },
            { "x", TokenType.x },
            { "x86_fp80", TokenType.x86_fp80 },
            { "xor", TokenType.xor },
            { "zeroext", TokenType.zeroext },
            { "zeroinitializer", TokenType.zeroinitializer },
            { "zext", TokenType.zext }
        };
        private int lineStart;
    }
}
