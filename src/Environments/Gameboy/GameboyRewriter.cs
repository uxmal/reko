using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Environments.Gameboy
{
    public class GameboyRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly GameboyArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<GameboyInstruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private GameboyInstruction instr;

        public GameboyRewriter(GameboyArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new GameboyDisassembler(arch, rdr).GetEnumerator();
            this.rtls = new List<RtlInstruction>();
            this.m = new RtlEmitter(rtls);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    m.Invalid();
                    break;
                case Mnemonic.adc: Rewrite_adc(); break;
                case Mnemonic.add: Rewrite_add(); break;
                case Mnemonic.and: Rewrite_and(); break;
                case Mnemonic.bit: Rewrite_bit(); break;
                case Mnemonic.call: Rewrite_call(); break;
                case Mnemonic.ccf: Rewrite_ccf(); break;
                case Mnemonic.cp: Rewrite_cp(); break;
                case Mnemonic.cpl: Rewrite_cpl(); break;
                case Mnemonic.daa: Rewrite_daa(); break;
                case Mnemonic.dec: Rewrite_dec(); break;
                case Mnemonic.di: Rewrite_di(); break;
                case Mnemonic.ei: Rewrite_ei(); break;
                case Mnemonic.halt: Rewrite_halt(); break;
                case Mnemonic.inc: Rewrite_inc(); break;
                case Mnemonic.jp: Rewrite_jp(); break;
                case Mnemonic.jr: Rewrite_jr(); break;
                case Mnemonic.ld: Rewrite_ld(); break;
                case Mnemonic.ldh: Rewrite_ldh(); break;
                case Mnemonic.nop: Rewrite_nop(); break;
                case Mnemonic.or: Rewrite_or(); break;
                case Mnemonic.pop: Rewrite_pop(); break;
                case Mnemonic.push: Rewrite_push(); break;
                case Mnemonic.res: Rewrite_res(); break;
                case Mnemonic.ret: Rewrite_ret(); break;
                case Mnemonic.reti: Rewrite_reti(); break;
                case Mnemonic.rl: Rewrite_rl(); break;
                case Mnemonic.rla: Rewrite_rla(); break;
                case Mnemonic.rlc: Rewrite_rlc(); break;
                case Mnemonic.rlca: Rewrite_rlca(); break;
                case Mnemonic.rr: Rewrite_rr(); break;
                case Mnemonic.rra: Rewrite_rra(); break;
                case Mnemonic.rrc: Rewrite_rrc(); break;
                case Mnemonic.rrca: Rewrite_rrca(); break;
                case Mnemonic.rst: Rewrite_rst(); break;
                case Mnemonic.sbc: Rewrite_sbc(); break;
                case Mnemonic.scf: Rewrite_scf(); break;
                case Mnemonic.set: Rewrite_set(); break;
                case Mnemonic.sla: Rewrite_sla(); break;
                case Mnemonic.sra: Rewrite_sra(); break;
                case Mnemonic.srl: Rewrite_srl(); break;
                case Mnemonic.stop: Rewrite_stop(); break;
                case Mnemonic.sub: Rewrite_sub(); break;
                case Mnemonic.swap: Rewrite_swap(); break;
                case Mnemonic.xor: Rewrite_xor(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, instr.InstructionClass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void EmitUnitTest()
        {
            var instr = dasm.Current;
            arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("GameboyRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private void Emit__001()
        {
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.C), 1);
        }

        private void Emit__00C(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.C), e);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 0);
        }

        private void Emit__0HC(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.HC), e);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
        }

        private void Emit__00_()
        {
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 0);
        }
        private void Emit__01_()
        {
            Clear(Registers.N);
            Set(Registers.H);
        }

        private void Clear(FlagGroupStorage grf)
        {
            m.Assign(binder.EnsureFlagGroup(grf), 0);
        }

        private void Set(FlagGroupStorage grf)
        {
            m.Assign(binder.EnsureFlagGroup(grf), grf.FlagGroupBits);
        }

        private void Emit__11_()
        {
            Set(Registers.N);
            Set(Registers.H);
        }

        private void Emit_000C(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.C), e);
            Clear(Registers.Z);
            Clear(Registers.N);
            Clear(Registers.H);
        }

        private void Emit_00HC(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.HC), e);
            Clear(Registers.Z);
            Clear(Registers.N);
        }

        private void Emit_Z_0C(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZC), e);
            Clear(Registers.H);
        }

        private void Emit_Z000(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), e);
            Clear(Registers.N);
            Clear(Registers.H);
            Clear(Registers.C);
        }

        private void Emit_Z00C(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZC), e);
            Clear(Registers.N);
            Clear(Registers.H);
        }

        private void Emit_Z01_(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), e);
            Clear(Registers.N);
            Set(Registers.H);
        }

        private void Emit_Z010(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), e);
            Clear(Registers.N);
            Set(Registers.H);
            Clear(Registers.C);
        }

        private void Emit_Z0H_(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZH), e);
            Clear(Registers.N);
        }

        private void Emit_Z0HC(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZHC), e);
            Clear(Registers.N);
        }

        private void Emit_Z1H_(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZH), e);
            Set(Registers.N);
        }

        private void Emit_Z1HC(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZHC), e);
            Set(Registers.N);
        }

        private void Emit_ZNHC(Expression e)
        {
            Debug.Assert(e is not null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZNHC), e);
        }

        private Expression Op(int iop, bool highAddress = false)
        {
            switch (instr.Operands[iop])
            {
            case RegisterStorage reg:
                return binder.EnsureRegister(reg);
            case Constant imm:
                return imm;
            case Address addr:
                return addr;
            case MemoryOperand mem:
                Expression ea;
                if (mem.Base is null)
                {
                    var uAddr = mem.Offset;
                    if (highAddress)
                        uAddr |= 0xFF00;
                    ea = Address.Ptr16((ushort) uAddr);
                }
                else
                {
                    ea = binder.EnsureRegister(mem.Base);
                    if (mem.PostDecrement)
                    {
                        var tmp = binder.CreateTemporary(ea.DataType);
                        m.Assign(tmp, ea);
                        m.Assign(ea, m.ISubS(ea, 1));
                        ea = tmp;
                    }
                    if (mem.PostIncrement)
                    {
                        var tmp = binder.CreateTemporary(ea.DataType);
                        m.Assign(tmp, ea);
                        m.Assign(ea, m.IAddS(ea, 1));
                        ea = tmp;
                    }
                }
                return m.Mem(mem.DataType, ea);
            case ConditionOperand<CCode> cond:
                switch (cond.Condition)
                {
                case CCode.C: return m.Test(ConditionCode.ULT, binder.EnsureFlagGroup(Registers.C));
                case CCode.NC: return m.Test(ConditionCode.UGE, binder.EnsureFlagGroup(Registers.C));
                case CCode.Z: return m.Test(ConditionCode.EQ, binder.EnsureFlagGroup(Registers.Z));
                case CCode.NZ: return m.Test(ConditionCode.NE, binder.EnsureFlagGroup(Registers.Z));
                }
                break;
            case StackOperand stack:
                var sp = binder.EnsureRegister(stack.StackRegister);
                return m.IAddS(sp, stack.Offset);
            }
            EmitUnitTest();
            throw new NotImplementedException($"Operand type {instr.Operands[iop].GetType().Name} is not implemented yet.");
        }

        private void AssignDst(Expression dst, Expression src)
        {
            m.Assign(dst, src);
        }

        private void Rewrite_adc()
        {
            var a = Op(0);
            var b = Op(1);
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(a, m.IAddC(a, b, c));
            Emit_Z0HC(m.Cond(a));
        }

        private void Rewrite_add()
        {
            var a = Op(0);
            var b = Op(1);
            if (a is Identifier id && id.Storage == Registers.sp &&
                b is Constant c)
            {
                m.Assign(id, m.AddSubSignedInt(id, c.ToInt16()));
                Emit_00HC(m.Cond(a));
            }
            else if (a.DataType.BitSize == 16)
            {
                m.Assign(a, m.IAdd(a, b));
                Emit__0HC(m.Cond(a));
            }
            else
            {
                m.Assign(a, m.IAdd(a, b));
                Emit_Z0HC(m.Cond(a));
            }
        }

        private void Rewrite_and()
        {
            var a = binder.EnsureRegister(Registers.a);
            var b = Op(0);
            m.Assign(a, m.And(a, b));
            Emit_Z010(m.Cond(a));
        }

        private void Rewrite_bit()
        {
            var bit = Op(0);
            var exp = Op(1);
            var z = binder.EnsureFlagGroup(Registers.Z);
            m.Assign(z, m.Fn(test_bit_intrinsic, exp, bit));
            Emit__01_();
        }

        private void Rewrite_call()
        {
            if (instr.Operands.Length == 2)
            {
                var cond = Op(0).Invert();
                m.Branch(cond, instr.Address + instr.Length);
            }
            var target = (Address)instr.Operands[^1];
            m.Call(target, 2);
        }

        private void Rewrite_ccf()
        {
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(c, m.Not(c));
            Emit__00_();
        }

        private void Rewrite_cp()
        {
            var a = binder.EnsureRegister(Registers.a);
            var b = Op(0);
            var zhc = binder.EnsureFlagGroup(Registers.ZHC);
            m.Assign(zhc, m.Cond(m.ISub(a, b)));
        }

        private void Rewrite_cpl()
        {
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(a, m.Comp(a));
            Emit__11_();
        }

        private void Rewrite_daa()
        {
            var a = binder.EnsureRegister(Registers.a);
            m.Assign(a, m.Fn(decimal_adjust_intrinsic, a));
            Emit_Z_0C(a);
        }

        private void Rewrite_dec()
        {
            var src = Op(0);
            m.Assign(src, m.ISub(src, 1));
            if (src.DataType.BitSize == 8)
            {
                Emit_Z1H_(m.Cond(src));
            }
        }

        private void Rewrite_di()
        {
            m.SideEffect(m.Fn(disable_interrupts_intrinsic));
        }

        private void Rewrite_ei()
        {
            m.SideEffect(m.Fn(enable_interrupts_intrinsic));
        }

        private void Rewrite_halt()
        {
            m.SideEffect(m.Fn(CommonOps.Halt), InstrClass.Terminates);
        }

        private void Rewrite_inc()
        {
            var exp = Op(0);
            m.Assign(exp, m.IAdd(exp, 1));
            if (exp.DataType.BitSize == 8)
            {
                Emit_Z0H_(m.Cond(exp));
            }
        }

        private void Rewrite_jp()
        {
            if (instr.Operands.Length > 1)
            {
                var cond = Op(0);
                var dst = (Address) Op(1);
                m.Branch(cond, dst);
            }
            else
            {
                var addr = Op(0);
                m.Goto(addr);
            }
        }

        private void Rewrite_jr()
        {
            if (instr.Operands.Length == 2)
            {
                var cond = Op(0);
                var target = (Address) Op(1);
                m.Branch(cond, target);
            }
            else
            {
                var target = (Address) Op(0);
                m.Goto(target);
            }
        }

        private void Rewrite_ld()
        {
            var src = Op(1);
            var dst = Op(0);
            m.Assign(dst, src);
        }

        private void Rewrite_ldh()
        {
            var src = Op(1, true);
            var dst = Op(0, true);
            m.Assign(dst, src);
        }

        private void Rewrite_nop()
        {
            m.Nop();
        }

        private void Rewrite_or()
        {
            var a = binder.EnsureRegister(Registers.a);
            var b = Op(0);
            m.Assign(a, m.Or(a, b));
            Emit_Z000(m.Cond(a));
        }

        private void Rewrite_pop()
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var dst = Op(0);
            if (dst is Identifier id)
            {
                m.Assign(id, m.Mem16(sp));
                if (id.Storage == Registers.af)
                {
                    Emit_ZNHC(id);
                }
            }
            else
            {
                var tmp = binder.CreateTemporary(PrimitiveType.Word16);
                m.Assign(tmp, m.Mem16(sp));
                m.Assign(dst, tmp);
            }
            m.Assign(sp, m.IAddS(sp, 2));
        }

        private void Rewrite_push()
        {
            var reg = Op(0);
            var sp = binder.EnsureRegister(Registers.sp);
            m.Assign(sp, m.ISubS(sp, 2));
            m.Assign(m.Mem16(sp), reg);
        }

        private void Rewrite_res()
        {
            var bit = Op(0);
            var exp = Op(1);
            m.Assign(exp, m.Fn(reset_bit_intrinsic, exp, bit));
        }

        private void Rewrite_ret()
        {
            if (instr.Operands.Length == 1)
            {
                var cond = Op(0).Invert();
                m.Branch(cond, instr.Address + instr.Length);
            }
            m.Return(2, 0);
        }

        private void Rewrite_reti()
        {
            Rewrite_ei();
            m.Return(2, 0);
        }

        private void Rewrite_rl()
        {
            var exp = Op(0);
            var cy = binder.EnsureFlagGroup(Registers.C);
            m.Assign(exp, m.Fn(
                CommonOps.RolC.MakeInstance(exp.DataType, PrimitiveType.Byte),
                exp, m.Byte(1), cy));
            Emit_Z00C(m.Cond(exp));
        }

        private void Rewrite_rla()
        {
            var exp = binder.EnsureRegister(Registers.a);
            var cy = binder.EnsureFlagGroup(Registers.C);
            m.Assign(exp, m.Fn(
                CommonOps.RolC.MakeInstance(exp.DataType, PrimitiveType.Byte),
                exp, m.Byte(1), cy));
            Emit_000C(m.Cond(exp));
        }

        private void Rewrite_rlc()
        {
            var exp = Op(0);
            var cy = binder.EnsureFlagGroup(Registers.C);
            m.Assign(exp, m.Fn(
                CommonOps.RolC.MakeInstance(exp.DataType, PrimitiveType.Byte),
                exp, m.Byte(1), cy));
            Emit_Z00C(m.Cond(exp));
        }

        private void Rewrite_rlca()
        {
            var exp = binder.EnsureRegister(Registers.a);
            m.Assign(exp, m.Fn(
                CommonOps.Rol.MakeInstance(exp.DataType, PrimitiveType.Byte),
                exp, m.Byte(1)));
            Emit_000C(m.Cond(exp));
        }

        private void Rewrite_rr()
        {
            var exp = Op(0);
            var cy = binder.EnsureFlagGroup(Registers.C);
            m.Assign(exp, m.Fn(
                CommonOps.RorC.MakeInstance(exp.DataType, PrimitiveType.Byte),
                exp, m.Byte(1), cy));
            Emit_Z00C(m.Cond(exp));
        }

        private void Rewrite_rra()
        {
            var exp = binder.EnsureRegister(Registers.a);
            var cy = binder.EnsureFlagGroup(Registers.C);
            m.Assign(exp, m.Fn(
                CommonOps.RorC.MakeInstance(exp.DataType, PrimitiveType.Byte),
                exp, m.Byte(1), cy));
            Emit_000C(m.Cond(exp));
        }

        private void Rewrite_rrc()
        {
            var exp = Op(0);
            m.Assign(exp, m.Fn(CommonOps.Ror, exp, m.Byte(1)));
            Emit_Z00C(exp);
        }

        private void Rewrite_rrca()
        {
            var exp = binder.EnsureRegister(Registers.a);
            m.Assign(exp, m.Fn(CommonOps.Ror, exp, m.Byte(1)));
            Emit_000C(m.Cond(exp));
        }

        private void Rewrite_rst()
        {
            var uAddr = ((Constant)instr.Operands[0]).ToUInt32();
            m.Call(Address.Ptr16((ushort) uAddr), 2);
        }

        private void Rewrite_sbc()
        {
            var a = Op(0);
            var b = Op(1);
            var c = binder.EnsureFlagGroup(Registers.C);
            m.Assign(a, m.ISubB(a, b, c));
            Emit_Z1HC(m.Cond(a));
        }

        private void Rewrite_scf()
        {
            Emit__001();
        }

        private void Rewrite_set()
        {
            var bit = Op(0);
            var exp = Op(1);
            m.Assign(exp, m.Fn(set_bit_intrinsic, exp, bit));
        }

        private void Rewrite_sla()
        {
            var exp = Op(0);
            m.Assign(exp, m.Shl(exp, m.Byte(1)));
            Emit_Z00C(m.Cond(exp));
        }

        private void Rewrite_sra()
        {
            var exp = Op(0);
            m.Assign(exp, m.Sar(exp, m.Byte(1)));
            Emit_Z000(m.Cond(exp));
        }

        private void Rewrite_srl()
        {
            var exp = Op(0);
            m.Assign(exp, m.Shr(exp, m.Byte(1)));
            Emit_Z00C(m.Cond(exp));
        }

        private void Rewrite_stop()
        {
            m.SideEffect(m.Fn(stop_intrinsic), InstrClass.Terminates);
        }

        private void Rewrite_sub()
        {
            var a = binder.EnsureRegister(Registers.a);
            var b = Op(0);
            m.Assign(a, m.ISub(a, b));
            Emit_Z1HC(m.Cond(a));
        }

        private void Rewrite_swap()
        {
            var exp = Op(0);
            m.Assign(exp, m.Fn(swap_nybbles_intrinsic, exp));
            Emit_Z000(m.Cond(exp));
        }

        private void Rewrite_xor()
        {
            var a = binder.EnsureRegister(Registers.a);
            var b = Op(0);
            AssignDst(a, m.Xor(a, b));
            Emit_Z000(m.Cond(a));
        }

        private readonly static IntrinsicProcedure decimal_adjust_intrinsic = IntrinsicBuilder.Unary("__decimal_adjust", PrimitiveType.Byte);
        private readonly static IntrinsicProcedure disable_interrupts_intrinsic = new IntrinsicBuilder("__disable_interrupts", true)
            .Void();
        private readonly static IntrinsicProcedure enable_interrupts_intrinsic = new IntrinsicBuilder("__enable_interrupts", true)
            .Void();
        private readonly static IntrinsicProcedure reset_bit_intrinsic = new IntrinsicBuilder("__reset_bit", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
        private readonly static IntrinsicProcedure set_bit_intrinsic = new IntrinsicBuilder("__set_bit", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Byte);
        private readonly static IntrinsicProcedure stop_intrinsic = new IntrinsicBuilder("__stop", true, new ProcedureCharacteristics
            {
                Terminates = true
            }).Void();
        private readonly static IntrinsicProcedure swap_nybbles_intrinsic = IntrinsicBuilder.Unary("__swap_nybbles", PrimitiveType.Byte);
        private readonly static IntrinsicProcedure test_bit_intrinsic = new IntrinsicBuilder("__test_bit", false)
            .Param(PrimitiveType.Byte)
            .Param(PrimitiveType.Byte)
            .Returns(PrimitiveType.Bool);
    }
}