#region License
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Rtl;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Environments.C64
{
    public class C64BasicRewriter : IEnumerable<RtlInstructionCluster> 
    {
        private C64Basic arch;
        private Address address;
        private SortedList<ushort, C64BasicInstruction> prog;
        private IRewriterHost host;
        private RtlEmitter emitter;
        private StringType strType;
        private RtlInstructionCluster cluster;

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
            int i = prog.IndexOfKey((ushort)address.Linear);
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
            int i = 0;
            while (i < line.Line.Length)
            {
                ParseStatement(line.Line, ref i);
            }
            return cluster;
        }

        private void ParseStatement(byte[] line, ref int i)
        {
            if (!EatSpaces(line, ref i))
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
                RewriteClose(line, ref i);
                break;
            case (byte)Token.CLR:
                RewriteClr();
                break;
            case (byte)Token.FOR:
                RewriteFor(line, ref i);
                break;
            case (byte)Token.GET:
                RewriteGet(line, ref i);
                break;
            case (byte)Token.GOSUB:
                RewriteGosub(line, ref i);
                break;
            case (byte)Token.GOTO:
                RewriteGoto(line, ref i);
                break;
            case (byte)Token.IF:
                RewriteIf(line, ref i);
                break;
            case (byte)Token.INPUT:
                RewriteInput(line, ref i);
                break;
            case (byte)Token.INPUT_hash:
                RewriteInput_hash(line, ref i);
                break;
            case (byte)Token.NEXT:
                RewriteNext(line, ref i);
                break;
            case (byte)Token.OPEN:
                RewriteOpen(line, ref i);
                break;
            case (byte)Token.POKE:
                RewritePoke(line, ref i);
                break;
            case (byte)Token.PRINT:
                RewritePrint(line, ref i);
                break;
            case (byte)Token.PRINT_hash:
                RewritePrint_hash(line, ref i);
                break;
            case (byte)Token.REM:
                //$TODO: annotation
                i = line.Length;
                return;
            case (byte)Token.RETURN:
                RewriteReturn(line, ref i);
                break;
            case (byte)Token.SYS:
                RewriteSys(line, ref i);
                break;
            case (byte)':':
                // Statement separator.
                break;
            default:
                if (0x41 <= b && b <= 0x5A)
                {
                    --i;
                    RewriteLet(line, ref i);
                    break;
                }
                throw new NotImplementedException(string.Format(
                    "Unimplemented BASIC token {0:X2} [{1}].",
                    (int)line[i - 1],
                    C64BasicInstruction.TokenToString(b)));
            }
        }

        private void Expect(byte tok, byte[] line, ref int i)
        {
            if (!EatSpaces(line, ref i) ||
                line[i] != tok)
                throw new InvalidOperationException(string.Format("Expected token {0}.", (Token)tok));
            ++i;
        }

        private Expression ExpectExpr(byte[] line, ref int i)
        {
            var expr = ParseExpr(line, ref i);
            if (expr == null)
                SyntaxError();
            return expr;
        }

        private Expression ExpectLValue(byte[]line, ref int i)
        {
            Identifier id;
            if (!GetIdentifier(line, ref i, out id))
                SyntaxError();
            Expression e = id;
            if (PeekAndDiscard((byte)'(', line, ref i))
            {
                var index = ExpectExpr(line, ref i);
                Expect((byte)')', line, ref i);
                e = emitter.Array(id.DataType, id, index);
            }
            return e;
        }

        private bool GetIdentifier(byte[] line, ref int i, out Identifier id)
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
            id = new Identifier(sb.ToString(), -1, dt, new MemoryStorage());
            return true;
        }

        private Expression ParseExpr(byte[] line, ref int i)
        {
            var lhs = ParseOrExp(line, ref i);
            if (!EatSpaces(line , ref i))
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
            var rhs = ParseOrExp(line, ref i);
            if (rhs == null)
                SyntaxError();
            return ctor(lhs, rhs);
        }

        private Expression ParseOrExp(byte[] line, ref int i)
        {
            var e = ParseTerm(line, ref i);
            if (e == null)
                return null;
            while (EatSpaces(line, ref i))
            {
                Func<Expression, Expression, Expression> ctor;
                switch (line[i])
                {
                default: return e;
                case (byte)Token.add: ctor = emitter.IAdd; break;
                }
                ++i;
                var e2 = ParseTerm(line, ref i);
                e = ctor(e, e2);
            }
            return e;
        }

        private Expression ParseTerm(byte[] line, ref int i)
        {
            var e = ParseFactor(line, ref i);
            if (e == null)
                return null;
            while (EatSpaces(line, ref i))
            {
                Func<Expression, Expression, Expression> ctor;
                switch (line[i])
                {
                default: return e;
                case (byte)Token.mul: ctor = emitter.IMul; break;
                }
                ++i;
                var e2 = ParseFactor(line, ref i);
                e = ctor(e, e2);
            }
            return e;
        }

        private Expression ParseFactor(byte[] line, ref int i)
        {
            if (PeekAndDiscard((byte)Token.sub, line, ref i))
            {
                var e = ParseFactor(line, ref i);
                if (e == null)
                    return null;
                return emitter.Neg(e);
            }
            return ParseAtom(line, ref i);
        }

        private Expression ParseAtom(byte[] line, ref int i)
        {
            if (!EatSpaces(line, ref i))
                return null;
            if (i >= line.Length)
                return null;
            Expression e;
            switch (line[i++])
            {
            case (byte)Token.CHR_s:
                Expect((byte)'(', line, ref i);
                e = ExpectExpr(line, ref i);
                Expect((byte)')', line, ref i);
                return PseudoProc("__Chr", strType, e);
            case (byte)Token.SPC_lp:
                e = ExpectExpr(line, ref i);
                Expect((byte)')', line, ref i);
                return PseudoProc("__Spc", strType, e);
            case (byte)Token.TAB_lp:
                e = ExpectExpr(line, ref i);
                Expect((byte)')', line, ref i);
                return PseudoProc("__Tab", strType, e);
            case (byte)'"':
                return ParseStringLiteral(line, ref i);
            default:
                --i;
                if (IsDigit(line[i]))
                {
                    int n;
                    if (GetInteger(line, ref i, out n))
                        return Constant.Int16((short)n);
                } 
                else if (IsLetter(line[i]))
                {
                    Identifier id;
                    if (GetIdentifier(line, ref i, out id))
                    {
                        if (PeekAndDiscard((byte)'(', line, ref i))
                        {
                            var index = ExpectExpr(line, ref i);
                            Expect((byte)')', line, ref i);
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

        private Expression ParseStringLiteral(byte[] line, ref int i)
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

        private bool PeekAndDiscard(byte b, byte[] line, ref int i)
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

        private void RewriteClose(byte[] line, ref int i)
        {
            var handle = ExpectExpr(line, ref i);
            if (PeekAndDiscard((byte)',', line, ref i))
            {
                ExpectExpr(line, ref i); //$TODO == what is this for?
                if (PeekAndDiscard((byte)',', line, ref i))
                {
                    ExpectExpr(line, ref i); //$TODO == what is this for?
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

        private void RewriteFor(byte[] line, ref int i)
        {
            Identifier id;
            if (!GetIdentifier(line, ref i, out id))
                SyntaxError();
            Expect((byte)Token.eq, line, ref i);
            var start = ExpectExpr(line, ref i);
            Expect((byte)Token.TO, line, ref i);
            var end = ExpectExpr(line, ref i);
            Expression step;
            if (PeekAndDiscard((byte)Token.STEP, line, ref i))
            {
                step = ExpectExpr(line, ref i);
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

        private void RewriteLet(byte[] line, ref int i)
        {
            Expression lhs = ExpectLValue(line, ref i);
            Expect((byte)Token.eq, line, ref i);
            Expression rhs = ParseExpr(line, ref i);
            emitter.Assign(lhs, rhs);
        }

        private void RewriteGet(byte[] line, ref int i)
        {
            Identifier id;
            if (!GetIdentifier(line, ref i, out id))
                SyntaxError();
            emitter.SideEffect(
                PseudoProc("__Get",
                VoidType.Instance,
                emitter.Out(strType, id)));
        }

        private void RewriteGosub(byte[] line, ref int i)
        {
            int lineNumber = 0;
            if (!EatSpaces(line, ref i) ||
                !GetInteger(line, ref i, out lineNumber))
                SyntaxError();
            emitter.Call(new Address((uint)lineNumber), 2);
        }

        private void RewriteGoto(byte[] line, ref int i)
        {
            int lineNumber = 0;
            if (!EatSpaces(line, ref i) ||
                !GetInteger(line, ref i, out lineNumber))
                SyntaxError();
            emitter.Goto(new Address((uint)lineNumber));
        }

        private void RewriteIf(byte[] line, ref int i)
        {
            var expr = ParseExpr(line, ref i);
            if (expr == null)
                SyntaxError();
            if (PeekAndDiscard((byte)Token.THEN, line, ref i))
            {
                if (!EatSpaces(line, ref i))
                    SyntaxError();
                byte b = line[i];
                int lineNumber;
                if (IsDigit(b))
                {
                    if (!GetInteger(line, ref i, out lineNumber))
                        SyntaxError();
                    emitter.Branch(expr, new Address((uint)lineNumber), RtlClass.ConditionalTransfer);
                    return;
                }
                var cl = cluster;
                var em = emitter;
                cluster = new RtlInstructionCluster(cl.Address, 1);
                emitter = new RtlEmitter(cluster.Instructions);
                ParseStatement(line, ref i);
                em.If(expr, cluster.Instructions.Last());
                cluster = cl;
                emitter = em;
                return;
            }
            else if (PeekAndDiscard((byte)Token.GOTO, line, ref i))
            {
                int lineNumber;
                if (!GetInteger(line, ref i, out lineNumber))
                    SyntaxError();
                emitter.Branch(expr, new Address((uint)lineNumber), RtlClass.ConditionalTransfer);
                return;
            }
            throw new NotImplementedException();
        }

        private void SyntaxError()
        {
            throw new InvalidOperationException("?SN Error");
        }

        private void RewriteInput(byte[] line, ref int i)
        {
            if (!EatSpaces(line, ref i))
                SyntaxError();
            if (PeekAndDiscard((byte)'"', line, ref i))
            {
                var str = ParseStringLiteral(line, ref i);
                if (str == null)
                    SyntaxError();
                var fnName = "__PrintLine";
                if (PeekAndDiscard((byte)';', line, ref i))
                    fnName = "__Print";
                emitter.SideEffect(PseudoProc(fnName, VoidType.Instance, str));
            }
            Expression lValue = ExpectLValue(line, ref i);
            emitter.SideEffect(PseudoProc("__Input", VoidType.Instance,
                emitter.Out(PrimitiveType.Ptr16, lValue)));
        }

        private void RewriteInput_hash(byte[] line, ref int i)
        {
            var logFileNo = ExpectExpr(line, ref i);
            EatSpaces(line, ref i);
            Expect((byte)',', line, ref i);
            Expression lValue = ExpectLValue(line, ref i);
            emitter.SideEffect(PseudoProc("__InputStm", VoidType.Instance,
                logFileNo,
                emitter.Out(PrimitiveType.Ptr16, lValue)));
            while (EatSpaces(line, ref i) && PeekAndDiscard((byte)',', line, ref i))
            {
                lValue = ExpectLValue(line, ref i);
                emitter.SideEffect(PseudoProc("__InputStm", VoidType.Instance,
                    logFileNo,
                    emitter.Out(PrimitiveType.Ptr16, lValue)));
            }
        }

        private void RewriteNext(byte[] line, ref int i)
        {
            Identifier id;
            GetIdentifier(line, ref i, out id); // The variable name is redundant.
            emitter.SideEffect(PseudoProc("__Next", VoidType.Instance));
        }

        private void RewriteOpen(byte[] line, ref int i)
        {
            var logicalFileNo = ParseExpr(line, ref i);
            if (logicalFileNo == null)
                SyntaxError();
            Expression deviceNo = Constant.Int16(-1);
            Expression secondaryNo = Constant.Int16(-1);
            Expression fileName = new StringConstant(strType, "");
            if (EatSpaces(line, ref i) && PeekAndDiscard((byte)',', line, ref i))
            {
                if (PeekAndDiscard((byte)'"', line, ref i))
                {
                    fileName = ParseStringLiteral(line, ref i);
                }
                else
                {
                    deviceNo = ParseExpr(line, ref i);
                    if (deviceNo == null)
                        SyntaxError();
                    if (EatSpaces(line, ref i) && PeekAndDiscard((byte)',', line, ref i))
                    {
                        if (PeekAndDiscard((byte)'"', line, ref i))
                        {
                            fileName = ParseStringLiteral(line, ref i);
                        }
                        else
                        {
                            secondaryNo = ParseExpr(line, ref i);
                            if (secondaryNo == null)
                                SyntaxError();
                            if (EatSpaces(line, ref i) && PeekAndDiscard((byte)',', line, ref i))
                            {
                                Expect((byte)'"', line, ref i);
                                fileName = ParseStringLiteral(line, ref i);
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
            
        private void RewritePoke(byte[] line, ref int i)
        {
            var addr = ParseExpr(line, ref i);
            if (!EatSpaces(line, ref i))
                throw new InvalidOperationException("?SN Error");
            Expect((byte)',', line, ref i);
            var val = ParseExpr(line, ref i);
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

        private void RewritePrint(byte[] line, ref int i)
        {
            string fnName;
            if (!EatSpaces(line, ref i) ||
                line[i] == ':')
            {
                emitter.SideEffect(PseudoProc("__PrintEmptyLine", VoidType.Instance));
                return;
            }
            do
            {
                Expression expr;
                if (PeekAndDiscard((byte)Token.TAB_lp, line, ref i))
                {
                    expr = ParseExpr(line, ref i);
                    if (expr == null)
                        SyntaxError();
                    Expect((byte)')', line, ref i);
                    emitter.SideEffect(PseudoProc("__PrintTab", VoidType.Instance, expr));
                    PeekAndDiscard((byte)';', line, ref i);
                    continue;
                }
                fnName = "__PrintLine";
                expr = ParseExpr(line, ref i);
                if (EatSpaces(line, ref i) &&
                    PeekAndDiscard((byte)';', line, ref i))
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
            } while (EatSpaces(line, ref i) && line[i] != ':');
            
        }

        private void RewritePrint_hash(byte[] line, ref int i)
        {
            int stm = 0;
            if (!EatSpaces(line, ref i) || !GetInteger(line, ref i, out stm))
                SyntaxError();
            while (PeekAndDiscard((byte)',', line, ref i) ||
                   PeekAndDiscard((byte)';', line, ref i))
            {
                var expr = ParseExpr(line, ref i);
                if (expr == null)
                    break;
                emitter.SideEffect(
                    PseudoProc("__PrintStm", VoidType.Instance,
                        Constant.Int32(stm),
                        expr));
            }
        }
        
        private void RewriteReturn(byte[] line, ref int i)
        {
            emitter.Return(2, 0);
        }

        private void RewriteSys(byte[] line, ref int i)
        {
            int addr;
            if (!EatSpaces(line, ref i) ||
                !GetInteger(line, ref i, out addr))
                throw new InvalidOperationException("Expected address after SYS.");
            emitter.SideEffect(
                PseudoProc("__Sys", VoidType.Instance,
                    new ProcedureConstant(arch.PointerType, new ExternalProcedure(
                        string.Format("fn{0:X4}", addr),
                        new ProcedureSignature()))));
        }

        private bool GetInteger(byte[] line, ref int i, out int number)
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

        private bool EatSpaces(byte[] line, ref int i)
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
