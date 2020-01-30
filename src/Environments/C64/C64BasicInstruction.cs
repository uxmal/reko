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

using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Environments.C64
{
    /// <summary>
    /// That's right, your eyes are not fooling you. This class models
    /// C64 basic instructions as MachineInstructions.
    /// </summary>
    public class C64BasicInstruction : MachineInstruction
    {
        public ushort NextAddress;
        public byte[] Line;

        internal const int TokenMin = 0x80;
        internal const int TokenMax = 0xCC;

        public C64BasicInstruction()
        {
            this.InstructionClass = InstrClass.Linear;
        }

        public override int MnemonicAsInteger
        {
            get { throw new NotImplementedException(); }
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteFormat("{0} ", Address.ToLinear());
            bool inString = false;
            for (int i = 0; i < Line.Length; ++i)
            {
                int b = Line[i];
                if (inString)
                {
                    writer.WriteString(Encoding.UTF8.GetString(Line, i, 1));
                    inString = (b != 0x22);
                }
                else
                {
                    if (TokenMin <= b && b < TokenMax)
                    {
                        writer.WriteString(TokenStrs[b - TokenMin]);
                    }
                    else
                    {
                        writer.WriteString(Encoding.UTF8.GetString(Line, i, 1));
                    }
                    inString = (b == 0x22);
                }
            }
        }

        public void Write(TextWriter writer)
        {
            writer.Write("{0} ", Address.ToLinear());
            bool inString = false;
            for (int i = 0; i < Line.Length; ++i)
            {
                int b = Line[i];
                if (inString)
                {
                    writer.Write(Encoding.UTF8.GetString(Line, i, 1));
                    inString = (b != 0x22);
                }
                else
                {
                    if (TokenMin <= b && b < TokenMax)
                    {
                        writer.Write(TokenStrs[b - TokenMin]);
                    }
                    else
                    {
                        writer.Write(Encoding.UTF8.GetString(Line, i, 1));
                    }
                    inString = (b == 0x22);
                }
            }
            writer.WriteLine();
        }

        public static string TokenToString(byte b)
        {
            if (TokenMin <= b && b < TokenMax)
            {
                return TokenStrs[b - TokenMin];
            }
            else
            {
                return new string((char)b, 1);
            }
        }

        internal static readonly string[] TokenStrs = new string[]
        {
            "END",
            "FOR",
            "NEXT",
            "DATA",
            "INPUT#",
            "INPUT",
            "DIM",
            "READ",
            "LET",
            "GOTO",
            "RUN",
            "IF",
            "RESTORE",
            "GOSUB",
            "RETURN",
            "REM",
            "STOP",
            "ON",
            "WAIT",

            "LOAD",
            "SAVE",
            "VERIFY",
            "DEF",
            "POKE",
            "PRINT#",
            "PRINT",
            "CONT",
            "LIST",
            "CLR",
            "CMD",
            "SYS",
            "OPEN",
            "CLOSE",
            "GET",
            "NEW",
            "TAB(",
            "TO",
            "FN",

            "SPC(",
            "THEN",
            "NOT",
            "STEP",
            "+",
            "-",
            "*",
            "/",
            "^",
            "AND",
            "OR",
            ">",
            "=",
            "<",
            "SGN",
            "INT",
            "ABS",
            "USR",
            "FRE",

            "POS",
            "SQR",
            "RND",
            "LOG",
            "EXP",
            "COS",
            "SIN",
            "TAN",
            "ATN",
            "PEEK",
            "LEN",
            "STR$",
            "VAL",
            "ASC",
            "CHR$",
            "LEFT$",
            "RIGHT$",
            "MID$",
            "GO",
        };
    }

    public enum Token
    {
        QUOTE = (byte)'"',
        COLON = (byte)':',
        END = 0x80,
        FOR = 0x81,
        NEXT = 0x82,
        DATA = 0x83,
        INPUT_hash = 0x84,
        INPUT = 0x85,
        DIM = 0x86,
        READ = 0x87,
        LET = 0x88,
        GOTO = 0x89,
        RUN = 0x8A,
        IF = 0x8B,
        RESTORE = 0x8C,
        GOSUB = 0x8D,
        RETURN = 0x8E,
        REM = 0x8F,
        STOP = 0x90,
        ON = 0x91,
        WAIT = 0x92,

        LOAD = 0x93,
        SAVE = 0x94,
        VERIFY = 0x95,
        DEF = 0x96,
        POKE = 0x97,
        PRINT_hash = 0x98,
        PRINT = 0x99,
        CONT = 0x9A,
        LIST = 0x9B,
        CLR = 0x9C,
        CMD = 0x9D,
        SYS = 0x9E,
        OPEN = 0x9F,
        CLOSE = 0xA0,
        GET = 0xA1,
        NEW = 0xA2,
        TAB_lp = 0xA3,
        TO = 0xA4,
        FN = 0xA5,

        SPC_lp = 0xA6,
        THEN = 0xA7,
        NOT = 0xA8,
        STEP = 0xA9,
        add = 0xAA,
        sub = 0xAB,
        mul = 0xAC,
        div = 0xAD,
        pow = 0xAE,
        AND = 0xAF,
        OR = 0xB0,
        gt = 0xB1,
        eq = 0xB2,
        lt = 0xB3,
        SGN = 0xB4,
        INT = 0xB5,
        ABS = 0xB6,
        USR = 0xB7,
        FRE = 0xB8,

        POS = 0xB9,
        SQR = 0xBA,
        RND = 0xBB,
        LOG = 0xBC,
        EXP = 0xBD,
        COS = 0xBE,
        SIN = 0xBF,
        TAN = 0xC0,
        ATN = 0xC1,
        PEEK = 0xC2,
        LEN = 0xC3,
        STR_s = 0xC4,
        VAL = 0xC5,
        ASC = 0xC6,
        CHR_s = 0xC7,
        LEFT_s = 0xC8,
        RIGHT_s = 0xC9,
        MID_s = 0xCA,
        GO = 0xCB,
    }
}