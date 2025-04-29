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

namespace Reko.Core.Hll.Pascal
{

    /// <summary>
    /// A Pascal token.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Constructs a token of the specified type.
        /// </summary>
        /// <param name="type">Token type.</param>
        public Token(TokenType type)
        {
            Type = type;
        }

        /// <summary>
        /// Constructs a token of the specified type and value.
        /// </summary>
        /// <param name="type">Token type.</param>
        /// <param name="value">Token value.</param>
        public Token(TokenType type, object? value = null)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// The <see cref="TokenType" /> of this token.
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// The value of this token.
        /// </summary>
        public object? Value;

        /// <summary>
        /// Returns a string representation of this token.
        /// </summary>
        public override string ToString()
        {
            if (Value is null)
                return string.Format("{0}", Type);
            else
                return string.Format("{0}:{1}", Type, Value);
        }
    }

    /// <summary>
    /// An enumeration of the various token types in the Pascal language.
    /// </summary>
    public enum TokenType
    {
#pragma warning disable CS1591
        EOF,

        Colon,
        Comma,
        Dot,
        DotDot,
        DotDotDot,
        Eq,
        Id,
        LBracket,
        LParen,
        Plus,
        Minus,
        Star,
        Slash,
        Number,
        Ptr,
        RBracket,
        RParen,
        RealLiteral,
        Semi,
        StringLiteral,

        Array,
        Boolean,
        Case,
        Char,
        Const,
        End,
        Extended,
        False,
        File,
        Function,
        Interface,
        Inline,
        Integer,
        Longint,
        Of,
        Packed,
        Procedure,
        Record,
        Set,
        String,
        True,
        Type,
        Unit,
        Univ,
        Var,
    }
}
