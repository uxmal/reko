using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
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
                switch (this.instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    goto case Mnemonic.Invalid;
                case Mnemonic.Invalid:
                    m.Invalid();
                    instr.InstructionClass = InstrClass.Invalid;
                    break;
                case Mnemonic.di: Rewrite_di(); break;
                case Mnemonic.jp: Rewrite_jp(); break;
                case Mnemonic.ld: Rewrite_ld(); break;
                case Mnemonic.nop: RewriteNop(); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, instr.InstructionClass);
                rtls.Clear();
            }
        }

        private Expression Op(int iop)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterOperand reg:
                return binder.EnsureRegister(reg.Register);
            case AddressOperand addr:
                return addr.Address;
            case StackOperand stack:
                var sp = binder.EnsureRegister(stack.StackRegister);
                return m.AddSubSignedInt(sp, stack.Offset);
            default:
                throw new NotImplementedException($"Unimplemented operand type {op.GetType().Name}.");
            }
        }

        private void RewriteNop()
        {
            m.Nop();
        }

        private void EmitUnitTest()
        {
            var testgenSvc = arch.Services.GetService<ITestGenerationService>();
            testgenSvc?.ReportMissingRewriter("GameboyRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        private void Emit__001()
        {
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.C), 1);
        }

        private void Emit__00C(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.C), e);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 0);
        }

        private void Emit__0HC(Expression e)
        {
            Debug.Assert(e != null);
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
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 1);
        }

        private void Emit__11_()
        {
            m.Assign(binder.EnsureFlagGroup(Registers.N), 1);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 1);
        }

        private void Emit_000C(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.C), e);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), 1);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 1);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 1);
        }

        private void Emit_00HC(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.HC), e);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
        }

        private void Emit_Z_0C(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZC), e);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 0);
        }

        private void Emit_Z000(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), e);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.C), 0);
        }

        private void Emit_Z00C(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZC), e);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 0);
        }

        private void Emit_Z01_(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), e);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 1);
        }

        private void Emit_Z010(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.Z), e);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
            m.Assign(binder.EnsureFlagGroup(Registers.H), 1);
            m.Assign(binder.EnsureFlagGroup(Registers.C), 0);
        }

        private void Emit_Z0H_(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZH), e);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
        }

        private void Emit_Z0HC(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZHC), e);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 0);
        }

        private void Emit_Z1H_(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZH), e);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 1);
        }

        private void Emit_Z1HC(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZHC), e);
            m.Assign(binder.EnsureFlagGroup(Registers.N), 1);
        }

        private void Emit_ZNHC(Expression e)
        {
            Debug.Assert(e != null);
            m.Assign(binder.EnsureFlagGroup(Registers.ZNHC), e);
        }


        private void Emit_____() { }
        private void Rewrite_adc() /* , Reg, d8), */ { EmitUnitTest(); Emit_Z0HC(null!); }
        private void Rewrite_add() /* , HL, BC),        */ { EmitUnitTest(); Emit__0HC(null!); }
        //private void Rewrite_add() /* , Reg, d8),             */ { EmitUnitTest(); Emit_Z0HC(null!); }
        //private void Rewrite_add() /* , Reg, Mem), */ { EmitUnitTest(); Emit_Z0HC(null!); }
        //private void Rewrite_add() /* , Reg, Reg), */ { EmitUnitTest(); Emit_Z0HC(null!); }
        //private void Rewrite_add() /* , SP, s8),            */ { EmitUnitTest(); Emit_00HC(null!); }
#if NYI
        private void Rewrite_and() /* , d8),                */ { EmitUnitTest(); Emit_Z010(null!); }
        private void Rewrite_bit() /* , Num, Mem), */ { EmitUnitTest(); Emit_Z01_(e); }
        private void Rewrite_call() /* , a16),              */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_ccf() /* ),                */ { EmitUnitTest(); Emit__00C(null!); }
        private void Rewrite_cp() /* , d8),                 */ { EmitUnitTest(); Emit_Z1HC(null!); }
        private void Rewrite_cp() /* , Mem),               */ { EmitUnitTest(); Emit_Z1HC(null!); }
        private void Rewrite_cp() /* , Reg),                  */ { EmitUnitTest(); Emit_Z1HC(null!); }
        private void Rewrite_cp() /* , Reg), */ { EmitUnitTest(); Emit_Z1HC(null!); }
        private void Rewrite_cpl() /* ),                */ { EmitUnitTest(); Emit__11_(null!); }
        private void Rewrite_daa() /* ),                */ { EmitUnitTest(); Emit_Z_0C(null!); }
        private void Rewrite_dec() /* , BC),            */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_dec() /* , DE),            */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_dec() /* , HL),            */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_dec() /* , Mem),          */ { EmitUnitTest(); Emit_Z1H_(null!); }
        private void Rewrite_dec() /* , Reg),             */ { EmitUnitTest(); Emit_Z1H_(null!); }
        private void Rewrite_dec() /* , SP),            */ { EmitUnitTest(); Emit_____(null!); }
#endif
        private void Rewrite_di() {
            m.SideEffect(host.Intrinsic("__disable_interrupts", false, VoidType.Instance));
        }
#if NYI
        private void Rewrite_ei() /* ),                     */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_halt() /* ),        */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_inc() /* , BC),            */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_inc() /* , DE),            */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_inc() /* , HL),            */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_inc() /* , Mem),          */ { EmitUnitTest(); Emit_Z0H_(null!); }
        private void Rewrite_inc() /* , Reg),             */ { EmitUnitTest(); Emit_Z0H_(null!); }
        private void Rewrite_inc() /* , SP),            */ { EmitUnitTest(); Emit_____(null!); }
#endif
        private void Rewrite_jp() /* , a16),                */ {
            if (instr.Operands.Length == 1)
            {
                m.Goto(Op(0));
                return;
            }
            EmitUnitTest(); 
        }
        //private void Rewrite_jp() /* , Cy, a16),            */ { EmitUnitTest(); }
        //private void Rewrite_jp() /* , M_hl_w),             */ { EmitUnitTest(); }
        //private void Rewrite_jp() /* , NC, a16),            */ { EmitUnitTest(); }
        //private void Rewrite_jp() /* , NZ, a16),            */ { EmitUnitTest(); }
        //private void Rewrite_jp() /* , Z, a16),             */ { EmitUnitTest(); }
#if NYI
        private void Rewrite_jr() /* , Cy, r8),          */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_jr() /* , NC, r8),         */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_jr() /* , NZ, r8),         */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_jr() /* , r8),             */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_jr() /* , Z, r8),          */ { EmitUnitTest(); Emit_____(null!); }
#endif
        private void Rewrite_ld() {
            var src = Op(1);
            var dst = Op(0);
            m.Assign(dst, src);
            if (instr.Operands[1] is StackOperand)
            {
                Emit_00HC(m.Cond(dst));
            }
        }

#if Nyi
        private void Rewrite_ldh() /* , M_a8, Reg),           */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_ldh() /* , Reg, M_a8),           */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_nop() /* ),                */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_or() /* , d8),                 */ { EmitUnitTest(); Emit_Z000(null!); }
        private void Rewrite_or() /* , Mem), */ { EmitUnitTest(); Emit_Z000(null!); }
        private void Rewrite_or() /* , Reg), */ { EmitUnitTest(); Emit_Z000(null!); }
        private void Rewrite_pop() /* , AF),                */ { EmitUnitTest(); Emit_ZNHC(null!); }
        private void Rewrite_pop() /* , BC),                */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_pop() /* , DE),                */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_pop() /* , HL),                */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_push() /* , AF),               */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_push() /* , BC),               */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_push() /* , DE),               */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_push() /* , HL),               */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_res() /* , Num, Mem), */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_res() /* , Num, Reg),    */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_res() /* , Num, Reg), */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_ret() /* ),                    */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_ret() /* , Cy),                */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_ret() /* , NC),                */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_ret() /* , NZ),                */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_ret() /* , Z),                 */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_reti() /* ),                   */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_rl() /* , Mem), */ { EmitUnitTest(); Emit_Z00C(null!); }
        private void Rewrite_rl() /* , Reg), */ { EmitUnitTest(); Emit_Z00C(null!); }
        private void Rewrite_rla() /* ),                */ { EmitUnitTest(); Emit_000C(null!); }
        private void Rewrite_rlc() /* , Mem), */ { EmitUnitTest(); Emit_Z00C(null!); }
        private void Rewrite_rlc() /* , Reg), */ { EmitUnitTest(); Emit_Z00C(null!); }
        private void Rewrite_rlca() /* ),               */ { EmitUnitTest(); Emit_000C(null!); }
        private void Rewrite_rr() /* , Mem), */ { EmitUnitTest(); Emit_Z00C(null!); }
        private void Rewrite_rr() /* , Reg), */ { EmitUnitTest(); Emit_Z00C(null!); }
        private void Rewrite_rra() /* ),                */ { EmitUnitTest(); Emit_000C(null!); }
        private void Rewrite_rrc() /* , Mem), */ { EmitUnitTest(); Emit_Z00C(null!); }
        private void Rewrite_rrc() /* , Reg), */ { EmitUnitTest(); Emit_Z00C(null!); }
        private void Rewrite_rrca() /* ),               */ { EmitUnitTest(); Emit_000C(null!); }
        private void Rewrite_rst() /* , Implicit(00)),      */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_rst() /* , Implicit(08)),      */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_rst() /* , Implicit(0x10)),    */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_rst() /* , Implicit(0x18)),    */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_rst() /* , Implicit(0x20)),    */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_rst() /* , Implicit(0x28)),    */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_rst() /* , Implicit(0x30)),    */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_rst() /* , Implicit(0x38)));   */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_sbc() /* , Reg, d8),             */ { EmitUnitTest(); Emit_Z1HC(null!); }
        private void Rewrite_sbc() /* , Reg, Mem), */ { EmitUnitTest(); Emit_Z1HC(null!); }
        private void Rewrite_sbc() /* , Reg, Reg), */ { EmitUnitTest(); Emit_Z1HC(null!); }
        private void Rewrite_scf() /* ),                */ { EmitUnitTest(); Emit__001(null!); }
        private void Rewrite_set() /* , Num, Mem), */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_set() /* , Num, Reg));   */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_set() /* , Num, Reg),    */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_set() /* , Num, Reg), */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_sla() /* , Mem), */ { EmitUnitTest(); Emit_Z00C(null!); }
        private void Rewrite_sla() /* , Reg), */ { EmitUnitTest(); Emit_Z00C(null!); }
        private void Rewrite_sra() /* , Mem), */ { EmitUnitTest(); Emit_Z000(null!); }
        private void Rewrite_sra() /* , Reg), */ { EmitUnitTest(); Emit_Z000(null!); }
        private void Rewrite_srl() /* , Mem), */ { EmitUnitTest(); Emit_Z00C(null!); }
        private void Rewrite_srl() /* , Reg), */ { EmitUnitTest(); Emit_Z00C(null!); }
        private void Rewrite_stop() /* , 0),            */ { EmitUnitTest(); Emit_____(null!); }
        private void Rewrite_sub() /* , d8),                */ { EmitUnitTest(); Emit_Z1HC(null!); }
        private void Rewrite_sub() /* , Mem), */ { EmitUnitTest(); Emit_Z1HC(null!); }
        private void Rewrite_sub() /* , Reg), */ { EmitUnitTest(); Emit_Z1HC(null!); }
        private void Rewrite_swap() /* , Mem), */ { EmitUnitTest(); Emit_Z000(null!); }
        private void Rewrite_swap() /* , Reg), */ { EmitUnitTest(); Emit_Z000(null!); }
        private void Rewrite_xor() /* , d8),                */ { EmitUnitTest(); Emit_Z000(null!); }
        private void Rewrite_xor() /* , Mem), */ { EmitUnitTest(); Emit_Z000(null!); }
        private void Rewrite_xor() /* , Reg), */ { EmitUnitTest(); Emit_Z000(null!); }
#endif
    }
}