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

namespace Reko.Assemblers.M68k
{
    public struct Token
    {
        public readonly string Text;
        public readonly TokenType Type;

        public Token(TokenType tokenType)
        {
            this.Type = tokenType;
            this.Text = "";
        }

        public Token(TokenType tokenType, string text)
        {
            this.Type = tokenType;
            this.Text = text;
        }
    }

    public enum TokenType
    {
        EOFile = -1,
        ID = 1,
        WS,
        NL,

        COMMA,
        DOT,
        HASH,
        INTEGER,
        STRING,

        CNOP,
        IDNT,
        OPT,
        PUBLIC,
        SECTION,
        DC,
        EQU,
        REG,

        add,
        addq,
        beq,
        bge,
        bne,
        bra,
        clr,
        cmp,

        jsr,
        lea,
        move,
        movem,
        pea,
        rts,
        subq,
        MINUS,
        LPAREN,
        RPAREN,
        PLUS,
    }
}
