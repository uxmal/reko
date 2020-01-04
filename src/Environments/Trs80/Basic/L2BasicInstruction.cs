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
using System.Text;
using System.IO;

namespace Reko.Environments.Trs80.Basic
{
    public class L2BasicInstruction : MachineInstruction
    {
        public ushort NextAddress;
        public byte[] Line;

        internal const int TokenMin = 0x80;
        internal const int TokenMax = 0xCC;

        public L2BasicInstruction()
        {
            this.InstructionClass = InstrClass.Linear;
        }

        public override int OpcodeAsInteger
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
            "RESET",
            "SET",
            "CLS",
            "CMD",
            "RANDOM",
            "NEXT",
            "DATA",
            "INPUT",
            "DIM",
            "READ",
            "LET",
            "GOTO",
            "RUN",
            "IF",
            "RESTO",
            "GOSUB",
            "RETURN",
            "REM",
            "STOP",
            "ELSE",
            "TRON",
            "TROFF",
            "DEFSTR",
            "DEFINT",
            "DEFSNG",
            "DEFDBL",
            "LINE",
            "EDIT",
            "ERROR",
            "RESUME",
            "OUT",
            "ON",
            "OPEN",
            "FIELD",
            "GET",
            "PUT",
            "CLOSE",
            "LOAD",
            "MERGE",
            "NAME",
            "KILL",
            "LSET",
            "RSET",
            "SAVE",
            "SYSTEM",
            "LPRINT",
            "DEF",
            "POKE",
            "PRINT",
            "CONT",
            "LIST",
            "LLIST",
            "DELETE",
            "AUTO",
            "CLEAR",
            "CLOAD",
            "CSAVE",
            "NEW",
            "TAB",
            "TO",
            "FN",
            "USING",
            "VARPTR",
            "USR",
            "ERL",
            "ERR",
            "STRING$",
            "INSTR",
            "POINT",
            "TIME$",
            "MEM",
            "INKEY$",
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
            "FRE",
            "INP",
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
            "CVI",
            "CVS",
            "CVD",
            "EOF",
            "LOC",
            "LOF",
            "MKI$",
            "MKS$",
            "MKD$",
            "CINT",
            "CSNG",
            "CDBL",
            "FIX",
            "LEN",
            "STR$",
            "VAL",
            "ASC",
            "CHR$",
            "LEFT$",
            "RIGHT$",
            "MID$",
            "(REM QUOTE)        };",
        };

        public enum Token
        {
            QUOTE = (byte)'"',
            COLON = (byte)':',
            END = 0x80,
            FOR = 0x81,
            RESET = 0x82,
            SET = 0x83,
            CLS = 0x84,
            CMD = 0x85,
            RANDOM = 0x86,
            NEXT = 0x87,
            DATA = 0x88,
            INPUT = 0x89,
            DIM = 0x8A,
            READ = 0x8B,
            LET = 0x8C,
            GOTO = 0x8D,
            RUN = 0x8E,
            IF = 0x8F,
            RESTO = 0x90,
            GOSUB = 0x91,
            RETURN = 0x92,
            REM = 0x93,
            STOP = 0x94,
            ELSE = 0x95,
            TRON = 0x96,
            TROFF = 0x97,
            DEFSTR = 0x98,
            DEFINT = 0x99,
            DEFSNG = 0x9A,
            DEFDBL = 0x9B,
            LINE = 0x9C,
            EDIT = 0x9D,
            ERROR = 0x9E,
            RESUME = 0x9F,
            OUT = 0xA0,
            ON = 0xA1,
            OPEN = 0xA2,
            FIELD = 0xA3,
            GET = 0xA4,
            PUT = 0xA5,
            CLOSE = 0xA6,
            LOAD = 0xA7,
            MERGE = 0xA8,
            NAME = 0xA9,
            KILL = 0xAA,
            LSET = 0xAB,
            RSET = 0xAC,
            SAVE = 0xAD,
            SYSTEM = 0xAE,
            LPRINT = 0xAF,
            DEF = 0xB0,
            POKE = 0xB1,
            PRINT = 0xB2,
            CONT = 0xB3,
            LIST = 0xB4,
            LLIST = 0xB5,
            DELETE = 0xB6,
            AUTO = 0xB7,
            CLEAR = 0xB8,
            CLOAD = 0xB9,
            CSAVE = 0xBA,
            NEW = 0xBB,
            TAB = 0xBC,
            TO = 0xBD,
            FN = 0xBE,
            USING = 0xBF,
            VARPTR = 0xC0,
            USR = 0xC1,
            ERL = 0xC2,
            ERR = 0xC3,
            STRING_s = 0xC4,
            INSTR = 0xC5,
            POINT = 0xC6,
            TIME_s = 0xC7,
            MEM = 0xC8,
            INKEY_s = 0xC9,
            THEN = 0xCA,
            NOT = 0xCB,
            STEP = 0xCC,
            add = 0xCD,
            sub = 0xCE,
            mul = 0xCF,
            div = 0xD0,
            exp = 0xD1,
            AND = 0xD2,
            OR = 0xD3,
            gt = 0xD4,
            eq = 0xD5,
            lt = 0xD6,
            SGN = 0xD7,
            INT = 0xD8,
            ABS = 0xD9,
            FRE = 0xDA,
            INP = 0xDB,
            POS = 0xDC,
            SQR = 0xDD,
            RND = 0xDE,
            LOG = 0xDF,
            EXP = 0xE0,
            COS = 0xE1,
            SIN = 0xE2,
            TAN = 0xE3,
            ATN = 0xE4,
            PEEK = 0xE5,
            CVI = 0xE6,
            CVS = 0xE7,
            CVD = 0xE8,
            EOF = 0xE9,
            LOC = 0xEA,
            LOF = 0xEB,
            MKI_s = 0xEC,
            MKS_s = 0xED,
            MKD_s = 0xEE,
            CINT = 0xEF,
            CSNG = 0xE0,
            CDBL = 0xF1,
            FIX = 0xF2,
            LEN = 0xF3,
            STR_s = 0xF4,
            VAL = 0xF5,
            ASC = 0xF6,
            CHR_s = 0xF7,
            LEFT_s = 0xF8,
            RIGHT_s = 0xF9,
            MID_s = 0xFA,
            Rem_QUOTE = 0xFB,
        }
    }
}