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
        public Token(TokenType type)
        {
            this.Type = type;
        }


        public Token(TokenType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public TokenType Type { get; set; }
        public string  Value { get; set; }
    }

    public enum TokenType
    {
        EOF,
        COMMA,
        Comment,
        GlobalId,
        LocalId,
        HexInteger,
        Integer,
        String,

        BANG,
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
        ident,
        inbounds,
        inttoptr,
        label,
        load,
        llvm,
        ne,
        @null,
        @private,
        noinline,
        nounwind,
        nsw,
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
        x,
    }
}
