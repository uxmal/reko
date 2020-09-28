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

using Reko.Arch.Mos6502;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Environments.C64
{
    /// <summary>
    /// Rewrites C64 Basic tokens into intermediate code.
    /// </summary>
    public class C64BasicRewriter : IEnumerable<RtlInstructionCluster> 
    {
        private readonly C64Basic arch;
        private readonly Address address;
        private readonly SortedList<ushort, C64BasicInstruction> program;
        private readonly IRewriterHost host;
        private readonly StringType strType;
        private RtlEmitter m;
        private InstrClass rtlc;
        private List<RtlInstruction> rtlInstructions;
        private byte[] line;
        private int i;

        public C64BasicRewriter(C64Basic arch, Address address, SortedList<ushort, C64BasicInstruction> program, IRewriterHost host)
        {
            this.arch = arch;
            this.address = address;
            this.program = program;
            this.host = host;
            this.strType = StringType.LengthPrefixedStringType(PrimitiveType.Char, PrimitiveType.Byte);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            int i = program.IndexOfKey((ushort)address.ToLinear());
            if (i < 0)
                yield break;
            for (; i < program.Count; ++i)
            {
                var line = program.Values[i];
                yield return GetRtl(line);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private RtlInstructionCluster GetRtl(C64BasicInstruction line)
        {
            this.rtlInstructions = new List<RtlInstruction>();
            this.rtlc = InstrClass.Linear;
            this.m = new RtlEmitter(rtlInstructions);
            Debug.Print("{0}", line);
            this.line = line.Line;
            this.i = 0;
            while (this.i < this.line.Length)
            {
                ParseStatement();
            }
            return new RtlInstructionCluster(line.Address, 1, rtlInstructions.ToArray())
            {
                Class = rtlc
            };
        }

        private void ParseStatement()
        {
            if (!EatSpaces())
                return;
            byte b = line[i++];
            switch ((Token)b)
            {
            case Token.END:
                var c = new ProcedureCharacteristics { Terminates = true };
                var ppp = host.PseudoProcedure("__End", c, VoidType.Instance);
                m.SideEffect(ppp);
                i = line.Length;        // We never return from end.
                return;
            case Token.CLOSE:
                RewriteClose();
                break;
            case Token.CLR:
                RewriteClr();
                break;
            case Token.FOR:
                RewriteFor();
                break;
            case Token.GET:
                RewriteGet();
                break;
            case Token.GOSUB:
                RewriteGosub();
                break;
            case Token.GOTO:
                RewriteGoto();
                break;
            case Token.IF:
                RewriteIf();
                break;
            case Token.INPUT:
                RewriteInput();
                break;
            case Token.INPUT_hash:
                RewriteInput_hash();
                break;
            case Token.NEXT:
                RewriteNext();
                break;
            case Token.OPEN:
                RewriteOpen();
                break;
            case Token.POKE:
                RewritePoke();
                break;
            case Token.PRINT:
                RewritePrint();
                break;
            case Token.PRINT_hash:
                RewritePrint_hash();
                break;
            case Token.REM:
                //$TODO: annotation
                i = line.Length;
                return;
            case Token.RETURN:
                RewriteReturn();
                break;
            case Token.SYS:
                RewriteSys();
                break;
            case Token.COLON:
                // Statement separator.
                break;
            default:
                if (0x41 <= b && b <= 0x5A)
                {
                    --i;
                    RewriteLet();
                    break;
                }
                throw new NotImplementedException(string.Format(
                    "Unimplemented BASIC token {0:X2} [{1}].",
                    (int)line[i - 1],
                    C64BasicInstruction.TokenToString(b)));
            }
        }

        private void Expect(byte tok)
        {
            if (!EatSpaces() ||
                line[i] != tok)
                throw new InvalidOperationException(string.Format("Expected token {0}.", (Token)tok));
            ++i;
        }

        private Expression ExpectExpr()
        {
            var expr = ParseExpr();
            if (expr == null)
                SyntaxError();
            return expr;
        }

        private Expression ExpectLValue()
        {
            if (!GetIdentifier(out Identifier id))
                SyntaxError();
            Expression e = id;
            if (PeekAndDiscard((byte)'('))
            {
                var index = ExpectExpr();
                Expect((byte)')');
                e = m.Array(id.DataType, id, index);
            }
            return e;
        }

        private bool GetIdentifier(out Identifier id)
        {
            var sb = new StringBuilder();
            id = null;
            if (i >= line.Length)
                return false;
            char ch = (char)line[i];
            if ('A' > ch || ch > 'Z')
                return false;
            sb.Append(ch);
            ++i;
            while (i < line.Length)
            {
                ch = (char)line[i];
                if (('A' > ch || ch > 'Z') &&
                    ('0' > ch || ch > '9'))
                {
                    break;
                }
                sb.Append(ch);
                ++i;
            }

            // Get the sigil if any.
            DataType dt = PrimitiveType.Real32;
            string suffix = "r";
            if (i < line.Length)
            {
                if (ch == '$')
                {
                    ++i;
                    suffix = "s";
                    dt = StringType.LengthPrefixedStringType(PrimitiveType.Char, PrimitiveType.Byte);
                }
                else if (ch == '%')
                {
                    ++i;
                    suffix = "i";
                    dt = PrimitiveType.Int16;
                }
            }
            sb.AppendFormat("_{0}", suffix);
            id = Identifier.Global(sb.ToString(), dt);
            return true;
        }

        private Expression ParseExpr()
        {
            var lhs = ParseOrExp();
            if (!EatSpaces())
                return lhs;
            Func<Expression,Expression,Expression> ctor;
            switch (line[i])
            {
            default: return lhs;
            case (byte)Token.eq: ctor = m.Eq; break;
            case (byte)Token.lt: ctor = m.Lt; break;
            case (byte)Token.gt: ctor = m.Gt; break;
            }
            ++i;
            var rhs = ParseOrExp();
            if (rhs == null)
                SyntaxError();
            return ctor(lhs, rhs);
        }

        private Expression ParseOrExp()
        {
            var e = ParseTerm();
            if (e == null)
                return null;
            while (EatSpaces())
            {
                Func<Expression, Expression, Expression> ctor;
                switch ((Token)line[i])
                {
                default: return e;
                case Token.add: ctor = m.IAdd; break;
                }
                ++i;
                var e2 = ParseTerm();
                e = ctor(e, e2);
            }
            return e;
        }

        private Expression ParseTerm()
        {
            var e = ParseFactor();
            if (e == null)
                return null;
            while (EatSpaces())
            {
                Func<Expression, Expression, Expression> ctor;
                switch (line[i])
                {
                default: return e;
                case (byte)Token.mul: ctor = m.IMul; break;
                }
                ++i;
                var e2 = ParseFactor();
                e = ctor(e, e2);
            }
            return e;
        }

        private Expression ParseFactor()
        {
            if (PeekAndDiscard((byte)Token.sub))
            {
                var e = ParseFactor();
                if (e == null)
                    return null;
                return m.Neg(e);
            }
            return ParseAtom();
        }

        private Expression ParseAtom()
        {
            if (!EatSpaces())
                return null;
            if (i >= line.Length)
                return null;
            Expression e;
            switch ((Token)line[i++])
            {
            case Token.CHR_s:
                Expect((byte)'(');
                e = ExpectExpr();
                Expect((byte)')');
                return host.PseudoProcedure("__Chr", strType, e);
            case Token.SPC_lp:
                e = ExpectExpr();
                Expect((byte)')');
                return host.PseudoProcedure("__Spc", strType, e);
            case Token.TAB_lp:
                e = ExpectExpr();
                Expect((byte)')');
                return host.PseudoProcedure("__Tab", strType, e);
            case Token.QUOTE:
                return ParseStringLiteral();
            default:
                --i;
                if (IsDigit(line[i]))
                {
                    if (GetInteger(out int n))
                        return Constant.Int16((short)n);
                } 
                else if (IsLetter(line[i]))
                {
                    if (GetIdentifier(out Identifier id))
                    {
                        if (PeekAndDiscard((byte)'('))
                        {
                            var index = ExpectExpr();
                            Expect((byte)')');
                            return m.Array(id.DataType, id, index);
                        }
                        else
                        {
                            return id;
                        }
                    }

                }
                return null;
            }
        }

        private Expression ParseStringLiteral()
        {
            var sb = new StringBuilder();
            while (i < line.Length)
            {
                if (line[i] == (byte)'"')
                {
                    ++i;
                    return Constant.String(sb.ToString(), strType);
                }
                sb.Append((char)line[i]);
                ++i;
            }
            throw new InvalidOperationException("?SN Error");
        }

        private bool PeekAndDiscard(byte b)
        {
            if (i < line.Length && b == line[i])
            {
                ++i;
                return true;
            }
            else
                return false;
        }

        private bool IsDigit(byte b)
        {
            return ((byte)'0' <= b && b <= (byte)'9');
        }

        private bool IsLetter(byte b)
        {
            return ((byte)'A' <= b && b <= (byte)'Z');
        }

        private void RewriteClose()
        {
            var handle = ExpectExpr();
            if (PeekAndDiscard((byte)','))
            {
                ExpectExpr(); //$TODO == what is this for?
                if (PeekAndDiscard((byte)','))
                {
                    ExpectExpr(); //$TODO == what is this for?
                }
            }
            rtlc = InstrClass.Linear;
            m.SideEffect(
                host.PseudoProcedure("__Close", VoidType.Instance,
                handle));
        }

        private void RewriteClr()
        {
            m.SideEffect(
                host.PseudoProcedure("__Clr", VoidType.Instance));
        }

        private void RewriteFor()
        {
            if (!GetIdentifier(out Identifier id))
                SyntaxError();
            Expect((byte)Token.eq);
            var start = ExpectExpr();
            Expect((byte)Token.TO);
            var end = ExpectExpr();
            Expression step;
            if (PeekAndDiscard((byte)Token.STEP))
            {
                step = ExpectExpr();
            }
            else
            {
                step = Constant.Int32(1);
            }
            rtlc = InstrClass.Linear;
            m.SideEffect(host.PseudoProcedure("__For", VoidType.Instance,
                m.Out(PrimitiveType.Ptr16, id),
                start,
                end,
                step));
        }

        private void RewriteLet()
        {
            Expression lhs = ExpectLValue();
            Expect((byte)Token.eq);
            Expression rhs = ParseExpr();
            m.Assign(lhs, rhs);
        }

        private void RewriteGet()
        {
            if (!GetIdentifier(out Identifier id))
                SyntaxError();
            rtlc = InstrClass.Linear;
            m.SideEffect(
                host.PseudoProcedure("__Get",
                VoidType.Instance,
                m.Out(strType, id)));
        }

        private void RewriteGosub()
        {
            int lineNumber = 0;
            if (!EatSpaces() ||
                !GetInteger(out lineNumber))
                SyntaxError();
            rtlc = InstrClass.Transfer | InstrClass.Call;
            m.Call(Address.Ptr16((ushort)lineNumber), 2);
        }

        private void RewriteGoto()
        {
            int lineNumber = 0;
            if (!EatSpaces() ||
                !GetInteger(out lineNumber))
                SyntaxError();
            rtlc = InstrClass.Transfer;
            m.Goto(Address.Ptr16((ushort)lineNumber));
        }

        private void RewriteIf()
        {
            var expr = ParseExpr();
            if (expr == null)
                SyntaxError();
            if (PeekAndDiscard((byte)Token.THEN))
            {
                if (!EatSpaces())
                    SyntaxError();
                byte b = line[i];
                if (IsDigit(b))
                {
                    if (!GetInteger(out int lineNumber))
                        SyntaxError();
                    rtlc = InstrClass.ConditionalTransfer;
                    m.Branch(expr, Address.Ptr16((ushort)lineNumber), InstrClass.ConditionalTransfer);
                    return;
                }
                var cl = rtlInstructions;
                var em = m;
                rtlInstructions = new List<RtlInstruction>();
                m = new RtlEmitter(rtlInstructions);
                ParseStatement();
                em.If(expr, rtlInstructions.Last());
                rtlInstructions = cl;
                m = em;
                return;
            }
            else if (PeekAndDiscard((byte)Token.GOTO))
            {
                if (!GetInteger(out int lineNumber))
                    SyntaxError();
                rtlc = InstrClass.ConditionalTransfer;
                m.Branch(expr, Address.Ptr16((ushort)lineNumber), InstrClass.ConditionalTransfer);
                return;
            }
            throw new NotImplementedException();
        }

        private void SyntaxError()
        {
            throw new InvalidOperationException("?SN Error");
        }

        private void RewriteInput()
        {
            if (!EatSpaces())
                SyntaxError();
            if (PeekAndDiscard((byte)'"'))
            {
                var str = ParseStringLiteral();
                if (str == null)
                    SyntaxError();
                var fnName = "__PrintLine";
                if (PeekAndDiscard((byte)';'))
                    fnName = "__Print";
                m.SideEffect(host.PseudoProcedure(fnName, VoidType.Instance, str));
            }
            Expression lValue = ExpectLValue();
            rtlc = InstrClass.Linear;
            m.SideEffect(host.PseudoProcedure("__Input", VoidType.Instance,
                m.Out(PrimitiveType.Ptr16, lValue)));
        }

        private void RewriteInput_hash()
        {
            var logFileNo = ExpectExpr();
            EatSpaces();
            Expect((byte)',');
            Expression lValue = ExpectLValue();
            rtlc = InstrClass.Linear;
            m.SideEffect(host.PseudoProcedure("__InputStm", VoidType.Instance,
                logFileNo,
                m.Out(PrimitiveType.Ptr16, lValue)));
            while (EatSpaces() && PeekAndDiscard((byte)','))
            {
                lValue = ExpectLValue();
                m.SideEffect(host.PseudoProcedure("__InputStm", VoidType.Instance,
                    logFileNo,
                    m.Out(PrimitiveType.Ptr16, lValue)));
            }
        }

        private void RewriteNext()
        {
            GetIdentifier(out Identifier id); // The variable name is redundant.
            m.SideEffect(host.PseudoProcedure("__Next", VoidType.Instance));
        }

        private void RewriteOpen()
        {
            var logicalFileNo = ParseExpr();
            if (logicalFileNo == null)
                SyntaxError();
            Expression deviceNo = Constant.Int16(-1);
            Expression secondaryNo = Constant.Int16(-1);
            Expression fileName = new StringConstant(strType, "");
            if (EatSpaces() && PeekAndDiscard((byte)','))
            {
                if (PeekAndDiscard((byte)'"'))
                {
                    fileName = ParseStringLiteral();
                }
                else
                {
                    deviceNo = ParseExpr();
                    if (deviceNo == null)
                        SyntaxError();
                    if (EatSpaces() && PeekAndDiscard((byte)','))
                    {
                        if (PeekAndDiscard((byte)'"'))
                        {
                            fileName = ParseStringLiteral();
                        }
                        else
                        {
                            secondaryNo = ParseExpr();
                            if (secondaryNo == null)
                                SyntaxError();
                            if (EatSpaces() && PeekAndDiscard((byte)','))
                            {
                                Expect((byte)'"');
                                fileName = ParseStringLiteral();
                            }
                        }
                    }
                }
            }
            m.SideEffect(host.PseudoProcedure(
                "__Open",
                VoidType.Instance,
                logicalFileNo,
                deviceNo,
                secondaryNo,
                fileName));
        }
            
        private void RewritePoke()
        {
            var addr = ParseExpr();
            if (!EatSpaces())
                throw new InvalidOperationException("?SN Error");
            Expect((byte)',');
            var val = ParseExpr();
            m.SideEffect(host.PseudoProcedure("__Poke",
                VoidType.Instance,
                addr,
                val));
        }

        // Print
        // Print A
        // Print A;
        // Print A;B
        // Print A;B;

        private void RewritePrint()
        {
            string fnName;
            if (!EatSpaces() ||
                line[i] == ':')
            {
                m.SideEffect(host.PseudoProcedure("__PrintEmptyLine", VoidType.Instance));
                return;
            }
            do
            {
                Expression expr;
                if (PeekAndDiscard((byte)Token.TAB_lp))
                {
                    expr = ParseExpr();
                    if (expr == null)
                        SyntaxError();
                    Expect((byte)')');
                    m.SideEffect(host.PseudoProcedure("__PrintTab", VoidType.Instance, expr));
                    PeekAndDiscard((byte)';');
                    continue;
                }
                fnName = "__PrintLine";
                expr = ParseExpr();
                if (EatSpaces() &&
                    PeekAndDiscard((byte)';'))
                {
                    fnName = "__Print";
                }
                else if (expr == null)
                {
                    fnName = "__PrintEmptyLine";
                }
                if (expr != null)
                {
                    m.SideEffect(host.PseudoProcedure(fnName, VoidType.Instance, expr));
                }
                else
                {
                    m.SideEffect(host.PseudoProcedure(fnName, VoidType.Instance));
                }
            } while (EatSpaces() && line[i] != ':');
            
        }

        private void RewritePrint_hash()
        {
            int stm = 0;
            if (!EatSpaces() || !GetInteger(out stm))
                SyntaxError();
            while (PeekAndDiscard((byte)',') ||
                   PeekAndDiscard((byte)';'))
            {
                var expr = ParseExpr();
                if (expr == null)
                    break;
                m.SideEffect(
                    host.PseudoProcedure("__PrintStm", VoidType.Instance,
                        Constant.Int32(stm),
                        expr));
            }
        }
        
        private void RewriteReturn()
        {
            rtlc = InstrClass.Transfer;
            m.Return(2, 0);
        }

        private void RewriteSys()
        {
            if (!EatSpaces() ||
                !GetInteger(out int addr))
                throw new InvalidOperationException("Expected address after SYS.");
            var addrMachineCode = Address.Ptr16((ushort) addr);
            IProcessorArchitecture arch6502 = host.GetArchitecture("m6502");
            rtlc = InstrClass.Transfer | InstrClass.Call;
            m.CallX(addrMachineCode, 2, arch6502);
        }

        private bool GetInteger(out int number)
        {
            int n = 0;
            if (i >= line.Length || !Char.IsDigit((char)line[i]))
            {
                number = 0;
                return false;
            }
            while (i < line.Length && Char.IsDigit((char)line[i]))
            {
                n = n * 10 + ((char)line[i] - '0');
                ++i;
            }
            number = n;
            return true;
        }

        private bool EatSpaces()
        {
            while (i < line.Length)
            {
                if (line[i] != ' ')
                    return true;
                ++i;
            }
            return false;
        }
    }
}
