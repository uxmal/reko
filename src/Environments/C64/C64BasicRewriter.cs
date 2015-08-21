﻿#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
        private C64Basic arch;
        private Address address;
        private SortedList<ushort, C64BasicInstruction> prog;
        private IRewriterHost host;
        private RtlEmitter emitter;
        private StringType strType;
        private RtlInstructionCluster cluster;
        private byte[] line;
        private int i;

        public C64BasicRewriter(C64Basic arch, Address address, SortedList<ushort, C64BasicInstruction> prog, IRewriterHost host)
        {
            this.arch = arch;
            this.address = address;
            this.prog = prog;
            this.host = host;
            this.strType = StringType.LengthPrefixedStringType(PrimitiveType.Char, PrimitiveType.Byte);
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            int i = prog.IndexOfKey((ushort)address.ToLinear());
            if (i < 0)
                yield break;
            for (; i < prog.Count; ++i)
            {
                var line = prog.Values[i];
                yield return GetRtl(line);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private RtlInstructionCluster GetRtl(C64BasicInstruction line)
        {
            this.cluster = new RtlInstructionCluster(line.Address, 1);
            this.emitter = new RtlEmitter(cluster.Instructions);
            Debug.Print("{0}", line);
            this.line = line.Line; 
            this.i = 0;
            while (this.i < this.line.Length)
            {
                ParseStatement();
            }
            return cluster;
        }

        private void ParseStatement()
        {
            if (!EatSpaces())
                return;
            byte b = line[i++];
            switch (b)
            {
            case (byte)Token.END:
                var ppp = host.EnsurePseudoProcedure("__End", VoidType.Instance, 0);
                ppp.Characteristics = new ProcedureCharacteristics
                {
                    Terminates = true,
                };
                emitter.SideEffect(PseudoProc(ppp, VoidType.Instance));
                i = line.Length;        // We never return from end.
                return;
            case (byte)Token.CLOSE:
                RewriteClose();
                break;
            case (byte)Token.CLR:
                RewriteClr();
                break;
            case (byte)Token.FOR:
                RewriteFor();
                break;
            case (byte)Token.GET:
                RewriteGet();
                break;
            case (byte)Token.GOSUB:
                RewriteGosub();
                break;
            case (byte)Token.GOTO:
                RewriteGoto();
                break;
            case (byte)Token.IF:
                RewriteIf();
                break;
            case (byte)Token.INPUT:
                RewriteInput();
                break;
            case (byte)Token.INPUT_hash:
                RewriteInput_hash();
                break;
            case (byte)Token.NEXT:
                RewriteNext();
                break;
            case (byte)Token.OPEN:
                RewriteOpen();
                break;
            case (byte)Token.POKE:
                RewritePoke();
                break;
            case (byte)Token.PRINT:
                RewritePrint();
                break;
            case (byte)Token.PRINT_hash:
                RewritePrint_hash();
                break;
            case (byte)Token.REM:
                //$TODO: annotation
                i = line.Length;
                return;
            case (byte)Token.RETURN:
                RewriteReturn();
                break;
            case (byte)Token.SYS:
                RewriteSys();
                break;
            case (byte)':':
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
            Identifier id;
            if (!GetIdentifier(out id))
                SyntaxError();
            Expression e = id;
            if (PeekAndDiscard((byte)'('))
            {
                var index = ExpectExpr();
                Expect((byte)')');
                e = emitter.Array(id.DataType, id, index);
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
            id = new Identifier(sb.ToString(), dt, new MemoryStorage());
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
            case (byte)Token.eq: ctor = emitter.Eq; break;
            case (byte)Token.lt: ctor = emitter.Lt; break;
            case (byte)Token.gt: ctor = emitter.Gt; break;
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
                switch (line[i])
                {
                default: return e;
                case (byte)Token.add: ctor = emitter.IAdd; break;
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
                case (byte)Token.mul: ctor = emitter.IMul; break;
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
                return emitter.Neg(e);
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
            switch (line[i++])
            {
            case (byte)Token.CHR_s:
                Expect((byte)'(');
                e = ExpectExpr();
                Expect((byte)')');
                return PseudoProc("__Chr", strType, e);
            case (byte)Token.SPC_lp:
                e = ExpectExpr();
                Expect((byte)')');
                return PseudoProc("__Spc", strType, e);
            case (byte)Token.TAB_lp:
                e = ExpectExpr();
                Expect((byte)')');
                return PseudoProc("__Tab", strType, e);
            case (byte)'"':
                return ParseStringLiteral();
            default:
                --i;
                if (IsDigit(line[i]))
                {
                    int n;
                    if (GetInteger(out n))
                        return Constant.Int16((short)n);
                } 
                else if (IsLetter(line[i]))
                {
                    Identifier id;
                    if (GetIdentifier(out id))
                    {
                        if (PeekAndDiscard((byte)'('))
                        {
                            var index = ExpectExpr();
                            Expect((byte)')');
                            return emitter.Array(id.DataType, id, index);
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

        public Expression PseudoProc(string name, DataType retType, params Expression[] args)
        {
            var ppp = host.EnsurePseudoProcedure(name, retType, args.Length);
            return PseudoProc(ppp, retType, args);
        }

        public Expression PseudoProc(PseudoProcedure ppp, DataType retType, params Expression[] args)
        {
            if (args.Length != ppp.Arity)
                throw new ArgumentOutOfRangeException(
                    string.Format("Pseudoprocedure {0} expected {1} arguments, but was passed {2}.",
                    ppp.Name,
                    ppp.Arity,
                    args.Length));

            return emitter.Fn(new ProcedureConstant(arch.PointerType, ppp), retType, args);
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
            emitter.SideEffect(
                PseudoProc("__Close", VoidType.Instance,
                handle));
        }

        private void RewriteClr()
        {
            emitter.SideEffect(
                PseudoProc("__Clr", VoidType.Instance));
        }

        private void RewriteFor()
        {
            Identifier id;
            if (!GetIdentifier(out id))
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
            emitter.SideEffect(PseudoProc("__For", VoidType.Instance,
                emitter.Out(PrimitiveType.Ptr16, id),
                start,
                end,
                step));
        }

        private void RewriteLet()
        {
            Expression lhs = ExpectLValue();
            Expect((byte)Token.eq);
            Expression rhs = ParseExpr();
            emitter.Assign(lhs, rhs);
        }

        private void RewriteGet()
        {
            Identifier id;
            if (!GetIdentifier(out id))
                SyntaxError();
            emitter.SideEffect(
                PseudoProc("__Get",
                VoidType.Instance,
                emitter.Out(strType, id)));
        }

        private void RewriteGosub()
        {
            int lineNumber = 0;
            if (!EatSpaces() ||
                !GetInteger(out lineNumber))
                SyntaxError();
            emitter.Call(Address.Ptr16((ushort)lineNumber), 2);
        }

        private void RewriteGoto()
        {
            int lineNumber = 0;
            if (!EatSpaces() ||
                !GetInteger(out lineNumber))
                SyntaxError();
            emitter.Goto(Address.Ptr16((ushort)lineNumber));
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
                int lineNumber;
                if (IsDigit(b))
                {
                    if (!GetInteger(out lineNumber))
                        SyntaxError();
                    emitter.Branch(expr, Address.Ptr16((ushort)lineNumber), RtlClass.ConditionalTransfer);
                    return;
                }
                var cl = cluster;
                var em = emitter;
                cluster = new RtlInstructionCluster(cl.Address, 1);
                emitter = new RtlEmitter(cluster.Instructions);
                ParseStatement();
                em.If(expr, cluster.Instructions.Last());
                cluster = cl;
                emitter = em;
                return;
            }
            else if (PeekAndDiscard((byte)Token.GOTO))
            {
                int lineNumber;
                if (!GetInteger(out lineNumber))
                    SyntaxError();
                emitter.Branch(expr, Address.Ptr16((ushort)lineNumber), RtlClass.ConditionalTransfer);
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
                emitter.SideEffect(PseudoProc(fnName, VoidType.Instance, str));
            }
            Expression lValue = ExpectLValue();
            emitter.SideEffect(PseudoProc("__Input", VoidType.Instance,
                emitter.Out(PrimitiveType.Ptr16, lValue)));
        }

        private void RewriteInput_hash()
        {
            var logFileNo = ExpectExpr();
            EatSpaces();
            Expect((byte)',');
            Expression lValue = ExpectLValue();
            emitter.SideEffect(PseudoProc("__InputStm", VoidType.Instance,
                logFileNo,
                emitter.Out(PrimitiveType.Ptr16, lValue)));
            while (EatSpaces() && PeekAndDiscard((byte)','))
            {
                lValue = ExpectLValue();
                emitter.SideEffect(PseudoProc("__InputStm", VoidType.Instance,
                    logFileNo,
                    emitter.Out(PrimitiveType.Ptr16, lValue)));
            }
        }

        private void RewriteNext()
        {
            Identifier id;
            GetIdentifier(out id); // The variable name is redundant.
            emitter.SideEffect(PseudoProc("__Next", VoidType.Instance));
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
            emitter.SideEffect(PseudoProc(
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
            emitter.SideEffect(PseudoProc("__Poke",
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
                emitter.SideEffect(PseudoProc("__PrintEmptyLine", VoidType.Instance));
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
                    emitter.SideEffect(PseudoProc("__PrintTab", VoidType.Instance, expr));
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
                    emitter.SideEffect(PseudoProc(fnName, VoidType.Instance, expr));
                }
                else
                {
                    emitter.SideEffect(PseudoProc(fnName, VoidType.Instance));
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
                emitter.SideEffect(
                    PseudoProc("__PrintStm", VoidType.Instance,
                        Constant.Int32(stm),
                        expr));
            }
        }
        
        private void RewriteReturn()
        {
            emitter.Return(2, 0);
        }

        private void RewriteSys()
        {
            int addr;
            if (!EatSpaces() ||
                !GetInteger(out addr))
                throw new InvalidOperationException("Expected address after SYS.");
            emitter.SideEffect(
                PseudoProc("__Sys", VoidType.Instance,
                    new ProcedureConstant(arch.PointerType, new ExternalProcedure(
                        string.Format("fn{0:X4}", addr),
                        new ProcedureSignature()))));
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
