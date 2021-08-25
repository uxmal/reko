using Reko.Arch.Pdp10.Disassembler;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp10.Rewriter
{
    public partial class Pdp10Rewriter
    {
        private void RewriteAddiSubi(Func<Expression, Expression, Expression> fn)
        {
            var src = RewriteEa(1);
            var dst = Ac();
            m.Assign(dst, fn(dst, src));
            m.Assign(binder.EnsureFlagGroup(C0C1VT), m.Cond(dst));
        }

        private Expression Andca(Expression ac, Expression e) => m.And(m.Comp(ac), e);
        private Expression Andcb(Expression ac, Expression e) => m.And(m.Comp(ac), m.Comp(e));
        private Expression Andcm(Expression ac, Expression e) => m.And(ac, m.Comp(e));

        private void RewriteAndcb()
        {
            var src = AccessEa(1);
            var dst = Ac();
            m.Assign(dst, m.And(m.Comp(dst), m.Comp(src)));
        }

        private void RewriteAndcmm()
        {
            //$REVIEW is this correct?
            var src = Ac();
            var dst = AccessEa(1);
            m.Assign(dst, m.And(m.Comp(dst), src));
        }


        private void RewriteAobjn()
        {
            var left = binder.CreateTemporary(word18);
            var right = binder.CreateTemporary(word18);
            var ac = Ac();
            m.Assign(left, m.IAdd(m.Slice(ac, left.DataType, 18), 1));
            m.Assign(right, m.IAdd(m.Slice(ac, right.DataType, 0), 1));
            m.Assign(ac, m.Seq(left, right));
            Branch(m.Lt0(left), RewriteEa(1)); 
        }

        private void RewriteAoj(Func<Expression,Expression> fn)
        {
            var dst = Ac();
            m.Assign(dst, m.IAdd(dst, 1));
            m.Assign(binder.EnsureFlagGroup(C0C1VT), m.Cond(dst));
            Branch(fn(dst), RewriteEa(1));
        }

        private void RewriteAoj()
        {
            var dst = Ac();
            m.Assign(dst, m.IAdd(dst, 1));
            m.Assign(binder.EnsureFlagGroup(C0C1VT), m.Cond(dst));
        }

        private void RewriteAoja()
        {
            var dst = Ac();
            m.Assign(dst, m.IAdd(dst, 1));
            m.Assign(binder.EnsureFlagGroup(C0C1VT), m.Cond(dst));
            m.Goto(RewriteEa(1));
        }

        private void RewriteAos()
        {
            var tmp = binder.CreateTemporary(word36);
            var cop = instr.Operands.Length;
            m.Assign(tmp, m.IAdd(AccessEa(cop - 1), 1));
            m.Assign(binder.EnsureFlagGroup(C0C1VT), m.Cond(tmp));
            if (cop == 2)
            {
                m.Assign(Ac(), tmp);
            }
            m.Assign(AccessEa(cop - 1), tmp);
        }

        private void RewriteAos(Func<Expression, Expression> fn)
        {
            var tmp = binder.CreateTemporary(word36);
            var cop = instr.Operands.Length;
            m.Assign(tmp, m.IAdd(AccessEa(cop - 1), 1));
            m.Assign(AccessEa(cop - 1), tmp);
            if (cop == 2)
            {
                m.Assign(Ac(), tmp);
            }
            m.Assign(binder.EnsureFlagGroup(C0C1VT), m.Cond(tmp));
            SkipIf(fn(tmp));
        }

        private void RewriteAsh()
        {
            var sh = RewriteEa(1);
            var dst = Ac();
            Expression src;
            if (sh is Constant c)
            {
                // Force to signed
                var lSh = c.ToUInt32();
                if (lSh < 36)
                {
                    src = m.Shl(dst, (int) lSh);
                }
                else
                {
                    var rSh = (0 - lSh) & ((1u << 18) - 1u);
                    src = m.Sar(dst, (int) rSh);
                }
            }
            else
            {
                src = m.Fn(lshIntrinsic, dst, sh);
            }
            m.Assign(dst, src);
        }

        private void RewriteAshc()
        {
            var sh = RewriteEa(1);
            var dst = AcPair();
            Expression src;
            if (sh is Constant c)
            {
                // Force to signed
                var lSh = c.ToUInt32();
                if (lSh < 36)
                {
                    src = m.Shl(dst, (int) lSh);
                }
                else
                {
                    var rSh = (0 - lSh) & ((1u << 18) - 1u);
                    src = m.Sar(dst, (int) rSh);
                }
            }
            else
            {
                src = m.Fn(lshIntrinsic, dst, sh);
            }
            m.Assign(dst, src);
        }

        private void RewriteCai(Func<Expression, Expression, Expression> fn)
        {
            var src = RewriteEa(1);
            var dst = Ac();
            SkipIf(fn(dst, src));
        }
        
        private void RewriteCam(Func<Expression, Expression, Expression> fn)
        {
            var src = RewriteEa(1);
            var dst = Ac();
            SkipIf(fn(dst, src));
        }

        private void RewriteDadd()
        {
            var src = AccessEa(1);
            src.DataType = word72;
            var reg_hi = (RegisterStorage) instr.Operands[0];
            var reg_lo = Registers.Accumulators[(reg_hi.Number + 1) & 0xF];
            var dst = binder.EnsureSequence(src.DataType, reg_hi, reg_lo);
            m.Assign(dst, m.IAdd(dst, src));
        }

        private Expression Eqv(Expression ac, Expression ea) => m.Comp(m.Xor(ac, ea));

        private void RewriteHll()
        {
            var src = AccessEa(1);
            var dst = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, word18, 18));
            m.Assign(dst, m.Dpb(dst, tmp, 18));
        }

        private void RewriteHllm()
        {
            var src = Ac();
            var tmp = binder.CreateTemporary(word18);
            var tmp2 = binder.CreateTemporary(word36);
            m.Assign(tmp, m.Slice(src, tmp.DataType, 0));
            m.Assign(tmp2, AccessEa(1));
            m.Assign(AccessEa(1), m.Dpb(tmp2, tmp, 0));
        }

        private void RewriteHllo()
        {
            var src = AccessEa(1);
            var dst = Ac();
            m.Assign(dst, m.Seq(m.Slice(src, word18, 0), Word18OfOnes()));
        }

        private void RewriteHllom()
        {
            var dst = AccessEa(1);
            var src = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(dst, word18, 0));
            Assign(AccessEa(1), m.Seq(tmp, Word18OfOnes()));
        }


        private void RewriteHllz()
        {
            var src = AccessEa(1);
            var dst = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, tmp.DataType, 18));
            m.Assign(dst, m.Shl(m.Convert(tmp, tmp.DataType, word36), 18));
        }

        private void RewriteHllzm()
        {
            var src = Ac();
            var dst = AccessEa(1);
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, tmp.DataType, 18));
            Assign(AccessEa(1), m.Shl(m.Convert(tmp, tmp.DataType, word36), 18));
        }

        private void RewriteHllzs()
        {
            var src = Ac();
            var dst = AccessEa(1);
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, tmp.DataType, 18));
            Assign(AccessEa(1), m.Shl(m.Convert(tmp, tmp.DataType, word36), 18));
        }


        private void RewriteHlr()
        {
            var src = AccessEa(1);
            var dst = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, word18, 18));
            m.Assign(dst, m.Dpb(dst, tmp, 18));
        }

        private void RewriteHlre()
        {
            var src = AccessEa(1);
            var dst = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, tmp.DataType, 18));
            m.Assign(dst, m.Convert(tmp, word18, int36));
        }

        private void RewriteHlrem()
        {
            var src = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, tmp.DataType, 18));
            m.Assign(AccessEa(1), m.Convert(tmp, word18, int36));
        }

        private void RewriteHlrm()
        {
            var src = Ac();
            var tmp = binder.CreateTemporary(word18);
            var tmp2 = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, tmp.DataType, 18));
            m.Assign(tmp2, m.Slice(AccessEa(1), tmp.DataType, 18));
            m.Assign(AccessEa(1), m.Seq(tmp2, tmp));
        }

        private void RewriteHlrz()
        {
            var src = AccessEa(1);
            var dst = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, tmp.DataType, 18));
            m.Assign(dst, m.Convert(tmp, word18, word36));
        }

        private void RewriteHrl()
        {
            var src = AccessEa(1);
            var dst = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, word18, 0));
            m.Assign(dst, m.Dpb(dst, tmp, 18));
        }

        private void RewriteHrli()
        {
            var src = RewriteEa(1);
            var dst = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, word18, 0));
            m.Assign(AccessEa(1), m.Dpb(dst, tmp, 18));
        }

        private void RewriteHrlm()
        {
            var src = Ac();
            var dst = RewriteEa(1);
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(dst, word18, 0));
            m.Assign(src, m.Dpb(src, tmp, 18));
        }

        private void RewriteHrls()
        {
            var src = AccessEa(1);
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, word18, 0));
            m.Assign(AccessEa(1), m.Seq(tmp, tmp));
        }


        private void RewriteHrlzm()
        {
            var src= Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, word18, 0));
            m.Assign(AccessEa(1), m.Convert(tmp, tmp.DataType, word36));
        }

        private void RewriteHrri()
        {
            var src = RewriteEa(1);
            var dst = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, word18, 0));
            m.Assign(dst, m.Dpb(dst, tmp, 18));
        }

        private void RewriteHrr()
        {
            var src = AccessEa(1);
            var dst = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, tmp.DataType, 0));
            m.Assign(dst, m.Dpb(dst, tmp, 0));
        }

        private void RewriteHrrm()
        {
            var src = Ac();
            var tmp = binder.CreateTemporary(word18);
            var tmp2 = binder.CreateTemporary(word36);
            m.Assign(tmp, m.Slice(src, tmp.DataType, 0));
            m.Assign(tmp2, AccessEa(1));
            m.Assign(AccessEa(1), m.Dpb(tmp2, tmp, 0));
        }

        private void RewriteHrrz()
        {
            var src = AccessEa(1);
            var dst = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, tmp.DataType, 0));
            m.Assign(dst, m.Convert(tmp, tmp.DataType, word36));
        }

        private void RewriteHrrzm()
        {
            var src = Ac();
            var tmp = binder.CreateTemporary(word18);
            m.Assign(tmp, m.Slice(src, tmp.DataType, 0));
            m.Assign(AccessEa(1), m.Convert(tmp, tmp.DataType, word36));
        }

        private void RewriteIdiv()
        {
            var src = AccessEa(1);
            var dst = Ac();
            var acp1 = Ac_plus_1();
            m.Assign(dst, m.SDiv(dst, src));
            m.Assign(acp1, m.Mod(dst, src));
            m.Assign(binder.EnsureFlagGroup(VTND), m.Cond(dst));
        }

        private void RewriteIdivb()
        {
            var src = RewriteEa(1);
            var dst = Ac();
            var acp1 = Ac_plus_1();
            var tmp = binder.CreateTemporary(dst.DataType);
            m.Assign(tmp, m.SDiv(dst, src));
            m.Assign(dst, tmp);
            Assign(RewriteEa(1), tmp);
            m.Assign(acp1, m.Mod(dst, src));
            m.Assign(binder.EnsureFlagGroup(VTND), m.Cond(dst));
        }

        private void RewriteIdivi()
        {
            var src = RewriteEa(1);
            var dst = Ac();
            var acp1 = Ac_plus_1();
            m.Assign(dst, m.SDiv(dst, src));
            m.Assign(acp1, m.Mod(dst, src));
            m.Assign(binder.EnsureFlagGroup(VTND), m.Cond(dst));
        }

        private void RewriteImul()
        {
            var src = AccessEa(1);
            var dst = Ac();
            m.Assign(dst, m.SMul(dst, src));
            m.Assign(binder.EnsureFlagGroup(VT), m.Cond(dst));
        }

        private void RewriteImuli()
        {
            var src = RewriteEa(1);
            var dst = Ac();
            m.Assign(dst, m.SMul(dst, src));
            m.Assign(binder.EnsureFlagGroup(VT), m.Cond(dst));
        }

        private void RewriteLsh()
        {
            var sh = RewriteEa(1);
            var dst = Ac();
            Expression src;
            if (sh is Constant c)
            {
                // Force to signed
                var lSh = c.ToUInt32();
                if (lSh < 36)
                {
                    src = m.Shl(dst, (int)lSh);
                }
                else
                {
                    var rSh = (0 - lSh) & ((1u << 18) - 1u);
                    src = m.Shr(dst, (int)rSh);
                }
            }
            else
            {
                src = m.Fn(lshIntrinsic, dst, sh);
            }
            m.Assign(dst, src);
        }

        private void RewriteMove()
        {
            var src = AccessEa(1);
            var dst = Ac();
            m.Assign(dst, src);
        }

        private void RewriteMovei()
        {
            var src = RewriteEa(1);
            var dst = Ac();
            m.Assign(dst, src);
        }

        private void RewriteMovem()
        {
            var src = Ac();
            var dst = AccessEa(1);
            m.Assign(dst, src);
        }

        private Expression Movm(Expression e) => m.Fn(absIntrinsic, e);
        private Expression Movn(Expression e) => m.Neg(e);

        private void RewriteMovms()
        {
            var i = instr.Operands.Length - 1;
            var dst = AccessEa(i);
            var tmp = binder.CreateTemporary(dst.DataType);
            m.Assign(tmp, m.Fn(absIntrinsic, dst));
            m.Assign(AccessEa(i), tmp);
            if (i == 1)
            {
                m.Assign(Ac(), tmp);
            }
        }

        private void RewriteMovns()
        {
            var src = AccessEa(1);
            var tmp = binder.CreateTemporary(src.DataType);
            m.Assign(tmp, m.Neg(src));
            Assign(AccessEa(1), tmp);
        }

        private void RewriteMovs()
        {
            var src = AccessEa(1);
            var dst = Ac();
            SwapWordHalves(dst, src);
        }

        private void RewriteMovsi()
        {
            var src = RewriteEa(1);
            var dst = Ac();
            if (src is Constant c)
            {
                // special case for movsi N,imm
                src = Constant.Create(word36, c.ToUInt64() << 18);
                m.Assign(dst, src);
            }
            else
            {
                SwapWordHalves(dst, src);
            }
        }

        private Expression Orca(Expression ac, Expression ea) => m.Or(m.Comp(ac), ea);
        private Expression Orcb(Expression ac, Expression ea) => m.Or(m.Comp(ac), m.Comp(ea));
        private Expression Orcm(Expression ac, Expression ea) => m.Or(ac, m.Comp(ea));


        private void RewritePop()
        {
            var reg = Ac();
            m.Assign(AccessEa(1), m.Mem(word36, reg));
            m.Assign(reg, m.ISubS(reg, 1));
        }

        private void RewritePush()
        {
            var reg = Ac();
            m.Assign(reg, m.IAddS(reg, 1));
            m.Assign(m.Mem(word36, reg), AccessEa(1));
        }

        private Expression Seta(Expression ac, Expression ea) => ac;
        private Expression Setcm(Expression ac, Expression ea) => m.Comp(ea);
        private Expression Setm(Expression ac, Expression ea) => ea;
        private Expression Seto(Expression ac, Expression ea) => Word36OfOnes();
        private Expression Setz(Expression ac, Expression ea) => Constant.Zero(word36);

        private void RewriteSetz()
        {
            m.Assign(Ac(), 0);
        }

        private void RewriteSkip(Func<Expression, Expression> fn)
        {
            if (instr.Operands.Length == 1)
            {
                var src = AccessEa(0);
                SkipIf(fn(src));
            }
            else
            {
                var src = AccessEa(1);
                var tmp = binder.CreateTemporary(src.DataType);
                m.Assign(tmp, src);
                m.Assign(Ac(), tmp);
                SkipIf(fn(src));
            }
        }

        private void RewriteSkipa()
        {
            if (instr.Operands.Length == 1)
            {
                Skip();
            }
            else
            {
                var src = AccessEa(1);
                var dst = Ac();
                m.Assign(dst, src);
                Skip();
            }
        }

        private void RewriteSoj(Func<Expression,Expression> fn)
        {
            var ac = Ac();
            m.Assign(ac, m.ISub(ac, 1));
            var cc = binder.EnsureFlagGroup(C0C1VT);
            m.Assign(cc, m.Cond(ac));
            Branch(fn(ac), RewriteEa(1));
        }

        private void RewriteSoja()
        {
            var ac = Ac();
            m.Assign(ac, m.ISub(ac, 1));
            var cc = binder.EnsureFlagGroup(C0C1VT);
            m.Assign(cc, ac);
            m.Goto(RewriteEa(1));
        }

        private void RewriteSos()
        {
            var tmp = binder.CreateTemporary(word36);
            var cop = instr.Operands.Length;
            m.Assign(tmp, m.ISub(AccessEa(cop - 1), 1));
            m.Assign(AccessEa(cop - 1), tmp);
            if (cop == 2)
            {
                m.Assign(Ac(), tmp);
            }
            m.Assign(binder.EnsureFlagGroup(C0C1VT), m.Cond(tmp));
        }

        private void RewriteSos(Func<Expression,Expression> fn)
        {
            var tmp = binder.CreateTemporary(word36);
            var cop = instr.Operands.Length;
            m.Assign(tmp, m.ISub(AccessEa(cop - 1), 1));
            m.Assign(AccessEa(cop - 1), tmp);
            if (cop == 2)
            {
                m.Assign(Ac(), tmp);
            }
            m.Assign(binder.EnsureFlagGroup(C0C1VT), m.Cond(tmp));
            SkipIf(fn(tmp));
        }

        private void RewriteTlc()
        {
            var mask = RewriteEa(1, 18);
            var ac = Ac();
            m.Assign(ac, m.Xor(ac, mask));
        }

        private void RewriteTlo()
        {
            var mask = RewriteEa(1, 18);
            var ac = Ac();
            m.Assign(ac, m.Or(ac, mask));
        }

        private void RewriteTloa()
        {
            var mask = RewriteEa(1, 18);
            var ac = Ac();
            m.Assign(ac, m.Or(ac, mask));
            Skip();
        }


        private void RewriteTln(Func<Expression, Expression> fn)
        {
            var src = RewriteEa(1);
            var dst = Ac();
            SkipIf(fn(m.And(m.Shr(dst, 18), src)));
        }

        private void RewriteTlz()
        {
            var mask = RewriteEa(1, 18);
            var ac = Ac();
            m.Assign(ac, m.And(ac, m.Comp(mask)));
        }

        private void RewriteTlza()
        {
            var mask = RewriteEa(1, 18);
            var ac = Ac();
            m.Assign(ac, m.And(ac, m.Comp(mask)));
            Skip();
        }

        private void RewriteTrn(Func<Expression, Expression> fn)
        {
            var src = RewriteEa(1);
            var dst = Ac();
            SkipIf(fn(m.And(m.And(dst, Word18OfOnes()), src)));
        }

        private void RewriteTro()
        {
            var mask = RewriteEa(1);
            var dst = Ac();
            m.Assign(dst, m.Or(dst, mask));
        }

        private void RewriteTroa()
        {
            var mask = RewriteEa(1);
            var dst = Ac();
            m.Assign(dst, m.Or(dst, mask));
            Skip();
        }

        private void RewriteTrz()
        {
            var mask = RewriteEa(1);
            var ac = Ac();
            m.Assign(ac, m.And(ac, m.Comp(mask)));
        }

        private void RewriteTrz(Func<Expression,Expression> fn)
        {
            var mask = RewriteEa(1);
            var ac = Ac();
            var tmp = binder.CreateTemporary(ac.DataType);
            m.Assign(tmp, ac);
            m.Assign(ac, m.And(tmp, m.Comp(mask)));
            SkipIf(fn(m.And(tmp, mask)));
        }

        private void RewriteXor()
        {
            var src = AccessEa(1);
            var dst = Ac();
            m.Assign(dst, m.Xor(dst, src));
        }

        private void RewriteXori()
        {
            var src = RewriteEa(1);
            var dst = Ac();
            m.Assign(dst, m.Xor(dst, src));
        }
    }
}
