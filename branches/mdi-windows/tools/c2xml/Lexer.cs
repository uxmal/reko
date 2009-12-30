using System;
using System.Collections.Generic;
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
        Shr,
        Asterisk,
        Struct,
        StringLiteral,
        Typedef,
        Union,
    }

}
