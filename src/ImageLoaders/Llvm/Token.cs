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
        DoubleLiteral,
        GlobalId,
        LocalId,
        HexInteger,
        Integer,
        IntType,
        String,
        X86_fp80_Literal,

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
        oeq,
        ogt,
        oge,
        olt,
        ole,
        one,
        ord,
        ueq,
        une,
        uno,

        add,
        align,
        alloca,
        and,
        ashr,
        attributes,
        bitcast,
        br,
        call,
        constant,
        common,
        datalayout,
        declare,
        define,
        dereferenceable,
        @double,
        exact,
        external,
        extractvalue,
        @false,
        fadd,
        fcmp,
        fdiv,
        fence,
        fmul,
        fsub,
        fpext,
        fptosi,
        fptoui,
        fptrunc,
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
        @internal,
        inttoptr,
        label,
        load,
        lshr,
        mul,
        @null,
        @private,
        noalias,
        nocapture,
        noinline,
        noreturn,
        nounwind,
        nsw,
        nuw,
        opaque,
        or,
        phi,
        ptrtoint,
        @readonly,
        ret,
        sdiv,
        select,
        seq_cst,
        sext,
        shl,
        sitofp,
        signext,
        source_filename,
        srem,
        store,
        sub,
        @switch,
        target,
        to,
        triple,
        @true,
        trunc,
        type,
        udiv,
        unnamed_addr,
        unreachable,
        urem,
        uwtable,
        @void,
        @volatile,
        writeonly,
        x,
        x86_fp80,
        xor,
        zeroext,
        zeroinitializer,
        zext,
    }
}
