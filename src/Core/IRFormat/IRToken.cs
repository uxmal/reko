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

namespace Reko.Core.IRFormat
{
    public enum IRTokenType
    {
        None,

        EOF,
        ERROR,
        Define,
        ID,
        CONST,
        SMUL,
        MUL,
        UMUL,
        FMUL,
        PLUS,
        MINUS,
        UDIV,
        SDIV,
        LogicalNot,
        LBRACKET,
        RBRACKET,
        LPAREN,
        RPAREN,
        COLON,
        LE,
        LT,
        GT,
        GE,
        ULE,
        ULT,
        UGT,
        UGE,
        SHL,
        SAR,
        Goto,
        If,
        XOR,
        LogicalAnd,
        BinaryAnd,
        LogicalOr,
        BinaryOr,
        NE,
        EQ,
    }

    public struct Token
    {
        public IRTokenType Type { get; }
        public object? Value { get; }

        public Token(IRTokenType type, object? value = null)
        {
            this.Type = type;
            this.Value = value;
        }

        public static readonly Token None = new Token(IRTokenType.None);

        public static Token Error(string errorMessage)
        {
            return new Token(IRTokenType.ERROR, errorMessage);
        }

        public override string ToString()
        {
            if (Value is null)
                return Type.ToString();
            return $"{Type}: {Value}";
        }

        public T? ValueAs<T>() where T : class
        {
            return this.Value as T;
        }
    }
}