#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Pascal
{
    public class Token
    {
        public TokenType Type;
        public object Value;

        public override string ToString()
        {
            if (Value == null)
                return string.Format("{0}", Type);
            else
                return string.Format("{0}:{1}", Type, Value);
        }
    }

    public enum TokenType
    {
        EOF,

        Colon,
        Comma,
        Dot,
        DotDot,
        Eq,
        Id,
        LBracket,
        LParen,
        Minus,
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
