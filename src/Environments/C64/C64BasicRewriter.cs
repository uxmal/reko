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

using Reko.Arch.Mos6502;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
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
        private static readonly StringType strType =
            StringType.LengthPrefixedStringType(PrimitiveType.Char, PrimitiveType.Byte);

        private readonly IDictionary<Address, C64BasicInstruction> instrs;
        private readonly Address address;
        private readonly IRewriterHost host;
        private RtlEmitter m;
        private InstrClass iclass;
        private List<RtlInstruction> rtlInstructions;
        private byte[] line;
        private int i;

        public C64BasicRewriter(
            C64Basic arch, 
            Address address, 
            SortedList<ushort, C64BasicInstruction> lines, 
            IDictionary<Address, C64BasicInstruction> instrs,
            IRewriterHost host)
        {
            this.address = address;
            this.Lines = lines;
            this.instrs = instrs; 
            this.host = host;
            this.line = null!;
            this.m = null!;
            this.rtlInstructions = null!;
        }

        public SortedList<ushort, C64BasicInstruction> Lines { get; }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            if (!instrs.TryGetValue(address, out var instr))
                yield break;
            int i = Lines.IndexOfKey(instr.LineNumber);
            if (i < 0)
                yield break;
            for (; i < Lines.Count; ++i)
            {
                var line = Lines.Values[i];
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
            this.iclass = InstrClass.Linear;
            this.m = new RtlEmitter(rtlInstructions);
            Debug.Print("{0}", line);
            this.line = line.Line;
            this.i = 0;
            while (this.i < this.line.Length)
            {
                ParseStatement();
            }
            return m.MakeCluster(line.Address, 1, iclass);
        }

        private void ParseStatement()
        {
            if (!EatSpaces())
                return;
            byte b = line[i++];
            switch ((Token)b)
            {
            case Token.END:
                var intrinsic = m.Fn(end_intrinsic);
                m.SideEffect(intrinsic, InstrClass.Terminates);
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

        private Expression? ExpectExpr()
        {
            var expr = ParseExpr();
            if (expr is null)
                SyntaxError();
            return expr;
        }

        private Expression? ExpectLValue()
        {
            if (!GetIdentifier(out Identifier id))
                SyntaxError();
            Expression e = id;
            if (PeekAndDiscard((byte)'('))
            {
                var index = ExpectExpr();
                if (index is null)
                {
                    SyntaxError();
                    return null;
                }
                Expect((byte)')');
                e = m.Array(id.DataType, id, index);
            }
            return e;
        }

        private bool GetIdentifier(out Identifier id)
        {
            var sb = new StringBuilder();
            id = null!;
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

        private Expression? ParseExpr()
        {
            var lhs = ParseOrExp();
            if (lhs is null || !EatSpaces())
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
            if (rhs is null)
            {
                SyntaxError();
                return null;
            }
            return ctor(lhs, rhs);
        }

        private Expression? ParseOrExp()
        {
            var e = ParseTerm();
            if (e is null)
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
                if (e2 is null)
                {
                    SyntaxError();
                    return null;
                }
                e = ctor(e, e2);
            }
            return e;
        }

        private Expression? ParseTerm()
        {
            var e = ParseFactor();
            if (e is null)
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
                e = ctor(e, e2!);
            }
            return e;
        }

        private Expression? ParseFactor()
        {
            if (PeekAndDiscard((byte)Token.sub))
            {
                var e = ParseFactor();
                if (e is null)
                    return null;
                return m.Neg(e);
            }
            return ParseAtom();
        }

        private Expression? ParseAtom()
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
                e = ExpectExpr()!;
                Expect((byte)')');
                return m.Fn(chr_intrinsic, e);
            case Token.SPC_lp:
                e = ExpectExpr()!;
                Expect((byte)')');
                return m.Fn(spc_intrinsic, e);
            case Token.TAB_lp:
                e = ExpectExpr()!;
                Expect((byte)')');
                return m.Fn(tab_intrinsic, e);
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
                            var index = ExpectExpr()!;
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
            iclass = InstrClass.Linear;
            m.SideEffect(m.Fn(close_intrinsic, handle!));
        }

        private void RewriteClr()
        {
            m.SideEffect(m.Fn(clr_intrinsic));
        }

        private void RewriteFor()
        {
            if (!GetIdentifier(out Identifier id))
                SyntaxError();
            Expect((byte)Token.eq);
            var start = ExpectExpr();
            Expect((byte)Token.TO);
            var end = ExpectExpr();
            Expression? step;
            if (PeekAndDiscard((byte)Token.STEP))
            {
                step = ExpectExpr();
                if (step is null)
                {
                    SyntaxError();
                    return;
                }
            }
            else
            {
                step = Constant.Int32(1);
            }
            iclass = InstrClass.Linear;
            m.SideEffect(m.Fn(for_intrinsic, 
                m.Out(PrimitiveType.Ptr16, id),
                start!,
                end!,
                step!));
        }

        private void RewriteLet()
        {
            Expression lhs = ExpectLValue()!;
            Expect((byte)Token.eq);
            Expression rhs = ParseExpr()!;
            m.Assign(lhs, rhs);
        }

        private void RewriteGet()
        {
            if (!GetIdentifier(out Identifier id))
                SyntaxError();
            iclass = InstrClass.Linear;
            m.SideEffect(m.Fn(get_intrinsic, m.Out(strType, id)));
        }

        private void RewriteGosub()
        {
            int lineNumber = 0;
            if (!EatSpaces() ||
                !GetInteger(out lineNumber))
                SyntaxError();
            iclass = InstrClass.Transfer | InstrClass.Call;
            var addr = Lines[(ushort) lineNumber].Address;
            m.Call(addr, 2);
        }

        private void RewriteGoto()
        {
            int lineNumber = 0;
            if (!EatSpaces() ||
                !GetInteger(out lineNumber))
                SyntaxError();
            iclass = InstrClass.Transfer;
            var addr = Lines[(ushort) lineNumber].Address;
            m.Goto(addr);
        }

        private void RewriteIf()
        {
            var expr = ParseExpr();
            if (expr is null)
            {
                SyntaxError();
                return;
            }
            if (PeekAndDiscard((byte)Token.THEN))
            {
                if (!EatSpaces())
                    SyntaxError();
                byte b = line[i];
                if (IsDigit(b))
                {
                    if (!GetInteger(out int lineNumber))
                        SyntaxError();
                    iclass = InstrClass.ConditionalTransfer;
                    m.Branch(expr, Lines[(ushort)lineNumber].Address, InstrClass.ConditionalTransfer);
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
                iclass = InstrClass.ConditionalTransfer;
                m.Branch(expr, Lines[(ushort) lineNumber].Address, InstrClass.ConditionalTransfer);
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
                if (str is null)
                {
                    SyntaxError();
                    return;
                }
                var intrinsic = printLine_intrinsic;
                if (PeekAndDiscard((byte)';'))
                    intrinsic = print_intrinsic;
                m.SideEffect(m.Fn(intrinsic, str));
            }
            Expression lValue = ExpectLValue()!;
            iclass = InstrClass.Linear;
            m.SideEffect(m.Fn(input_intrinsic, m.Out(PrimitiveType.Ptr16, lValue)));
        }

        private void RewriteInput_hash()
        {
            var logFileNo = ExpectExpr()!;
            EatSpaces();
            Expect((byte)',');
            Expression? lValue = ExpectLValue();
            if (lValue is null)
                return;
            iclass = InstrClass.Linear;
            m.SideEffect(m.Fn(inputStm_intrinsic, logFileNo!, m.Out(PrimitiveType.Ptr16, lValue)));
            while (EatSpaces() && PeekAndDiscard((byte)','))
            {
                lValue = ExpectLValue()!;
                m.SideEffect(m.Fn(inputStm_intrinsic,
                    logFileNo,
                    m.Out(PrimitiveType.Ptr16, lValue)));
            }
        }

        private void RewriteNext()
        {
            GetIdentifier(out Identifier id); // The variable name is redundant.
            m.SideEffect(m.Fn(next_intrinsic));
        }

        private void RewriteOpen()
        {
            var logicalFileNo = ParseExpr();
            if (logicalFileNo is null)
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
                    deviceNo = ParseExpr()!;
                    if (deviceNo is null)
                        SyntaxError();
                    if (EatSpaces() && PeekAndDiscard((byte)','))
                    {
                        if (PeekAndDiscard((byte)'"'))
                        {
                            fileName = ParseStringLiteral();
                        }
                        else
                        {
                            secondaryNo = ParseExpr()!;
                            if (secondaryNo is null)
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
            m.SideEffect(m.Fn(open_intrinsic, 
                logicalFileNo!,
                deviceNo!,
                secondaryNo!,
                fileName));
        }
            
        private void RewritePoke()
        {
            var addr = ParseExpr();
            if (addr is null || !EatSpaces())
            {
                SyntaxError();
                return;
            }
            Expect((byte)',');
            var val = ParseExpr();
            if (val is null)
            {
                SyntaxError();
                return;
            }
            m.SideEffect(m.Fn(poke_intrinsic, addr, val));
        }

        // Print
        // Print A
        // Print A;
        // Print A;B
        // Print A;B;

        private void RewritePrint()
        {
            IntrinsicProcedure intrinsic;
            if (!EatSpaces() ||
                line[i] == ':')
            {
                m.SideEffect(m.FnVariadic(printLine_intrinsic));
                return;
            }
            do
            {
                Expression? expr;
                if (PeekAndDiscard((byte)Token.TAB_lp))
                {
                    expr = ParseExpr();
                    if (expr is null)
                    {
                        SyntaxError();
                        return;
                    }
                    Expect((byte)')');
                    m.SideEffect(m.FnVariadic(printTab_intrinsic, expr));
                    PeekAndDiscard((byte)';');
                    continue;
                }
                intrinsic = printLine_intrinsic;
                expr = ParseExpr();
                if (EatSpaces() &&
                    PeekAndDiscard((byte)';'))
                {
                    intrinsic = print_intrinsic;
                }
                else if (expr is null)
                {
                    intrinsic = printLine_intrinsic;
                }
                if (expr is not null)
                {
                    m.SideEffect(m.Fn(intrinsic, expr));
                }
                else
                {
                    m.SideEffect(m.Fn(intrinsic));
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
                if (expr is null)
                    break;
                m.SideEffect(
                    m.Fn(printStm_intrinsic,
                        Constant.Int32(stm),
                        expr));
            }
        }
        
        private void RewriteReturn()
        {
            iclass = InstrClass.Transfer|InstrClass.Return;
            m.Return(2, 0);
        }

        private void RewriteSys()
        {
            if (!EatSpaces() ||
                !GetInteger(out int addr))
                throw new InvalidOperationException("Expected address after SYS.");
            var addrMachineCode = Address.Ptr16((ushort) addr);
            IProcessorArchitecture arch6502 = host.GetArchitecture("m6502");
            iclass = InstrClass.Transfer | InstrClass.Call;
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

        private static readonly IntrinsicProcedure chr_intrinsic = new IntrinsicBuilder("__Chr", false)
            .Param(PrimitiveType.Int16)
            .Returns(strType);
        private static readonly IntrinsicProcedure close_intrinsic = new IntrinsicBuilder("__Close", true)
            .Param(PrimitiveType.Int16)
            .Void();
        private static readonly IntrinsicProcedure clr_intrinsic = new IntrinsicBuilder("__Clr", true)
            .Void();
        private static readonly IntrinsicProcedure end_intrinsic = new IntrinsicBuilder(
            "__End", true, new ProcedureCharacteristics { Terminates = true })
            .Void();
        private static readonly IntrinsicProcedure for_intrinsic = new IntrinsicBuilder("__For", true)
            .OutParam(new UnknownType())
            .Param(new UnknownType())
            .Param(new UnknownType())
            .Param(new UnknownType())
            .Void();
        private static readonly IntrinsicProcedure input_intrinsic = new IntrinsicBuilder("__Input", true)
            .OutParam(PrimitiveType.Ptr16)
            .Void();
        private static readonly IntrinsicProcedure inputStm_intrinsic = new IntrinsicBuilder("__InputStm", true)
            .Param(PrimitiveType.Int16)
            .OutParam(PrimitiveType.Ptr16)
            .Void();
        private static readonly IntrinsicProcedure get_intrinsic = new IntrinsicBuilder("__Get", true)
            .OutParam(strType)
            .Void();
        private static readonly IntrinsicProcedure open_intrinsic = new IntrinsicBuilder("__Open", true)
            .Param(PrimitiveType.Int16)
            .Param(PrimitiveType.Int16)
            .Param(PrimitiveType.Int16)
            .Param(strType)
            .Void();
        private static readonly IntrinsicProcedure next_intrinsic = new IntrinsicBuilder("__Next", true)
            .Void();
        private static readonly IntrinsicProcedure peek_intrinsic = new IntrinsicBuilder("__Peek", true)
            .Param(PrimitiveType.Int16)
            .Returns(PrimitiveType.Int16);
        private static readonly IntrinsicProcedure poke_intrinsic = new IntrinsicBuilder("__Poke", true)
            .Param(PrimitiveType.Int16)
            .Param(PrimitiveType.Int16)
            .Void();
        private static readonly IntrinsicProcedure print_intrinsic = new IntrinsicBuilder("__Print", true)
            .Param(new UnknownType())
            .Void();
        private static readonly IntrinsicProcedure printLine_intrinsic = IntrinsicBuilder.SideEffect("__PrintLine")
            .Param(new UnknownType())
            .Void();
        private static readonly IntrinsicProcedure printStm_intrinsic = new IntrinsicBuilder("__PrintStm", true)
            .Param(PrimitiveType.Int16)
            .Param(new UnknownType())
            .Void();
        private static readonly IntrinsicProcedure printTab_intrinsic = new IntrinsicBuilder("__PrintTab", true)
            .Param(PrimitiveType.Int16)
            .Param(new UnknownType())
            .Void();
        private static readonly IntrinsicProcedure spc_intrinsic = new IntrinsicBuilder("__Spc", false)
            .Param(PrimitiveType.Int16)
            .Returns(strType);
        private static readonly IntrinsicProcedure tab_intrinsic = new IntrinsicBuilder("__Tab", false)
            .Param(PrimitiveType.Int16)
            .Returns(strType);
    }
}
