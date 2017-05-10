#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

namespace Reko.ImageLoaders.LLVM
{
    public class Token
    {
        public Token(int line, TokenType type)
        {
            this.LineNumber = line;
            this.Type = type;
        }

        public Token(int line, TokenType type, string value)
        {
            this.LineNumber = line;
            this.Type = type;
            this.Value = value;
        }

        public int LineNumber { get; private set; }
        public TokenType Type { get; private set; }
        public string  Value { get; private set; }
    }

    public enum TokenType
    {
        EOF,
        CharArray,
        Comment,
        GlobalId,
        LocalId,
        HexInteger,
        Integer,
        IntType,
        String,

        BANG,
        COMMA,
        DOT,
        ELLIPSIS,
        EQ,
        HASH,
        LPAREN,
        LBRACKET,
        LBRACE,
        RPAREN,
        RBRACE,
        RBRACKET,
        STAR,

        eq,
        ne,
        ugt,
        uge,
        ult,
        ule,
        sgt,
        sge,
        slt,
        sle,

        add,
        align,
        alloca,
        and,
        attributes,
        bitcast,
        br,
        call,
        constant,
        datalayout,
        declare,
        define,
        dereferenceable,
        external,
        extractvalue,
        @false,
        getelementptr,
        global,
        i1,
        i8,
        i16,
        i32,
        i64,
        icmp,
        inbounds,
        inrange,
        inttoptr,
        label,
        load,
        @null,
        @private,
        nocapture,
        noinline,
        nounwind,
        nsw,
        nuw,
        phi,
        ret,
        source_filename,
        store,
        sub,
        @switch,
        target,
        to,
        triple,
        type,
        unnamed_addr,
        uwtable,
        @void,
        @volatile,
        x,

    }
}
