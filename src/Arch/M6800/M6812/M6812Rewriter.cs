#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.M6800.M6812
{
    public class M6812Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly M6812Architecture arch;
        private readonly EndianImageReader rdr;
        private readonly M6812State state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<M6812Instruction> dasm;
        private M6812Instruction instr;
        private RtlEmitter m;
        private RtlClass rtlc;

        public M6812Rewriter(M6812Architecture arch, EndianImageReader rdr, M6812State state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new M6812Disassembler(rdr).GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                var rtlInstrs = new List<RtlInstruction>();
                this.m = new RtlEmitter(rtlInstrs);
                this.rtlc = RtlClass.Linear;
                switch (instr.Opcode)
                {
                case Opcode.invalid:
                    this.rtlc = RtlClass.Invalid;
                    m.Invalid();
                    break;
                case Opcode.aba: RewriteAba(); break;
                case Opcode.adca: RewriteAdc(Registers.a); break;
                case Opcode.adcb: RewriteAdc(Registers.b); break;
                case Opcode.adda: RewriteAdd(Registers.a); break;
                case Opcode.addb: RewriteAdd(Registers.b); break;
                case Opcode.addd: RewriteAdd(Registers.d); break;
                case Opcode.anda: RewriteAnda(); break;
                case Opcode.andb: RewriteAndb(); break;
                case Opcode.andcc: RewriteAndcc(); break;
                case Opcode.asr: RewriteAsr(); break;
                case Opcode.asra: RewriteAsra(); break;
                case Opcode.asrb: RewriteAsrb(); break;
                case Opcode.bcc: RewriteBcc(); break;
                case Opcode.bclr: RewriteBclr(); break;
                case Opcode.bcs: RewriteBcs(); break;
                case Opcode.beq: RewriteBeq(); break;
                case Opcode.bge: RewriteBge(); break;
                case Opcode.bgnd: RewriteBgnd(); break;
                case Opcode.bgt: RewriteBgt(); break;
                case Opcode.bhi: RewriteBhi(); break;
                case Opcode.bita: RewriteBita(); break;
                case Opcode.bitb: RewriteBitb(); break;
                case Opcode.ble: RewriteBle(); break;
                case Opcode.bls: RewriteBls(); break;
                case Opcode.blt: RewriteBlt(); break;
                case Opcode.bmi: RewriteBmi(); break;
                case Opcode.bne: RewriteBne(); break;
                case Opcode.bpl: RewriteBpl(); break;
                case Opcode.bra: RewriteBra(); break;
                case Opcode.brclr: RewriteBrclr(); break;
                case Opcode.brn: RewriteBrn(); break;
                case Opcode.brset: RewriteBrset(); break;
                case Opcode.bset: RewriteBset(); break;
                case Opcode.bsr: RewriteBsr(); break;
                case Opcode.bvc: RewriteBvc(); break;
                case Opcode.bvs: RewriteBvs(); break;
                case Opcode.call: RewriteCall(); break;
                case Opcode.cba: RewriteCba(); break;
                case Opcode.clr: RewriteClr(); break;
                case Opcode.clra: RewriteClra(); break;
                case Opcode.clrb: RewriteClrb(); break;
                case Opcode.cmpa: RewriteCmpa(); break;
                case Opcode.cmpb: RewriteCmpb(); break;
                case Opcode.com: RewriteCom(); break;
                case Opcode.coma: RewriteComa(); break;
                case Opcode.comb: RewriteComb(); break;
                case Opcode.cpd: RewriteCpd(); break;
                case Opcode.cps: RewriteCps(); break;
                case Opcode.cpx: RewriteCpx(); break;
                case Opcode.cpy: RewriteCpy(); break;
                case Opcode.daa: RewriteDaa(); break;
                case Opcode.dbeq: RewriteDbeq(); break;
                case Opcode.dbne: RewriteDbne(); break;
                case Opcode.dec: RewriteDec(); break;
                case Opcode.deca: RewriteDeca(); break;
                case Opcode.decb: RewriteDecb(); break;
                case Opcode.dex: RewriteDex(); break;
                case Opcode.dey: RewriteDey(); break;
                case Opcode.ediv: RewriteEdiv(); break;
                case Opcode.edivs: RewriteEdivs(); break;
                case Opcode.emacs: RewriteEmacs(); break;
                case Opcode.emaxd: RewriteEmaxd(); break;
                case Opcode.emaxm: RewriteEmaxm(); break;
                case Opcode.emind: RewriteEmind(); break;
                case Opcode.eminm: RewriteEminm(); break;
                case Opcode.emul: RewriteEmul(); break;
                case Opcode.emuls: RewriteEmuls(); break;
                case Opcode.eora: RewriteEora(); break;
                case Opcode.eorb: RewriteEorb(); break;
                case Opcode.etbl: RewriteEtbl(); break;
                case Opcode.fdiv: RewriteFdiv(); break;
                case Opcode.ibeq: RewriteIbeq(); break;
                case Opcode.ibne: RewriteIbne(); break;
                case Opcode.idiv: RewriteIdiv(); break;
                case Opcode.idivs: RewriteIdivs(); break;
                case Opcode.inc: RewriteInc(); break;
                case Opcode.inca: RewriteInca(); break;
                case Opcode.incb: RewriteIncb(); break;
                case Opcode.inx: RewriteInx(); break;
                case Opcode.iny: RewriteIny(); break;
                case Opcode.jmp: RewriteJmp(); break;
                case Opcode.jsr: RewriteJsr(); break;
                case Opcode.lbcc: RewriteLbcc(); break;
                case Opcode.lbcs: RewriteLbcs(); break;
                case Opcode.lbeq: RewriteLbeq(); break;
                case Opcode.lbge: RewriteLbge(); break;
                case Opcode.lbgt: RewriteLbgt(); break;
                case Opcode.lbhi: RewriteLbhi(); break;
                case Opcode.lble: RewriteLble(); break;
                case Opcode.lbls: RewriteLbls(); break;
                case Opcode.lblt: RewriteLblt(); break;
                case Opcode.lbmi: RewriteLbmi(); break;
                case Opcode.lbne: RewriteLbne(); break;
                case Opcode.lbpl: RewriteLbpl(); break;
                case Opcode.lbra: RewriteLbra(); break;
                case Opcode.lbrn: RewriteLbrn(); break;
                case Opcode.lbvc: RewriteLbvc(); break;
                case Opcode.lbvs: RewriteLbvs(); break;
                case Opcode.ldaa: RewriteLdaa(); break;
                case Opcode.ldab: RewriteLdab(); break;
                case Opcode.ldd: RewriteLdd(); break;
                case Opcode.lds: RewriteLds(); break;
                case Opcode.ldx: RewriteLdx(); break;
                case Opcode.ldy: RewriteLdy(); break;
                case Opcode.leas: RewriteLeas(); break;
                case Opcode.leax: RewriteLeax(); break;
                case Opcode.leay: RewriteLeay(); break;
                case Opcode.lsl: RewriteLsl(); break;
                case Opcode.lsla: RewriteLsla(); break;
                case Opcode.lslb: RewriteLslb(); break;
                case Opcode.lsld: RewriteLsld(); break;
                case Opcode.lsr: RewriteLsr(); break;
                case Opcode.lsra: RewriteLsra(); break;
                case Opcode.lsrb: RewriteLsrb(); break;
                case Opcode.lsrd: RewriteLsrd(); break;
                case Opcode.maxa: RewriteMaxa(); break;
                case Opcode.maxm: RewriteMaxm(); break;
                case Opcode.mem: RewriteMem(); break;
                case Opcode.mina: RewriteMina(); break;
                case Opcode.minm: RewriteMinm(); break;
                case Opcode.mov: RewriteMov(); break;
                case Opcode.mul: RewriteMul(); break;
                case Opcode.neg: RewriteNeg(); break;
                case Opcode.nega: RewriteNega(); break;
                case Opcode.negb: RewriteNegb(); break;
                case Opcode.nop: RewriteNop(); break;
                case Opcode.oraa: RewriteOraa(); break;
                case Opcode.orab: RewriteOrab(); break;
                case Opcode.orcc: RewriteOrcc(); break;
                case Opcode.psha: RewritePsha(); break;
                case Opcode.pshb: RewritePshb(); break;
                case Opcode.pshc: RewritePshc(); break;
                case Opcode.pshd: RewritePshd(); break;
                case Opcode.pshx: RewritePshx(); break;
                case Opcode.pshy: RewritePshy(); break;
                case Opcode.pula: RewritePula(); break;
                case Opcode.pulb: RewritePulb(); break;
                case Opcode.pulc: RewritePulc(); break;
                case Opcode.puld: RewritePuld(); break;
                case Opcode.pulx: RewritePulx(); break;
                case Opcode.puly: RewritePuly(); break;
                case Opcode.rev: RewriteRev(); break;
                case Opcode.revw: RewriteRevw(); break;
                case Opcode.rol: RewriteRol(); break;
                case Opcode.rola: RewriteRola(); break;
                case Opcode.rolb: RewriteRolb(); break;
                case Opcode.ror: RewriteRor(); break;
                case Opcode.rora: RewriteRora(); break;
                case Opcode.rorb: RewriteRorb(); break;
                case Opcode.rtc: RewriteRtc(); break;
                case Opcode.rti: RewriteRti(); break;
                case Opcode.rts: RewriteRts(); break;
                case Opcode.sba: RewriteSba(); break;
                case Opcode.sbca: RewriteSbca(); break;
                case Opcode.sbcb: RewriteSbcb(); break;
                case Opcode.sex: RewriteSex(); break;
                case Opcode.staa: RewriteStaa(); break;
                case Opcode.stab: RewriteStab(); break;
                case Opcode.std: RewriteStd(); break;
                case Opcode.stop: RewriteStop(); break;
                case Opcode.sts: RewriteSts(); break;
                case Opcode.stx: RewriteStx(); break;
                case Opcode.sty: RewriteSty(); break;
                case Opcode.suba: RewriteSuba(); break;
                case Opcode.subb: RewriteSubb(); break;
                case Opcode.subd: RewriteSubd(); break;
                case Opcode.swi: RewriteSwi(); break;
                case Opcode.tab: RewriteTab(); break;
                case Opcode.tba: RewriteTba(); break;
                case Opcode.tbeq: RewriteTbeq(); break;
                case Opcode.tbl: RewriteTbl(); break;
                case Opcode.tbne: RewriteTbne(); break;
                case Opcode.tfr: RewriteTfr(); break;
                case Opcode.trap: RewriteTrap(); break;
                case Opcode.tst: RewriteTst(); break;
                case Opcode.tsta: RewriteTsta(); break;
                case Opcode.tstb: RewriteTstb(); break;
                case Opcode.wai: RewriteWai(); break;
                case Opcode.wav: RewriteWav(); break;
                }
                yield return new RtlInstructionCluster(
                    instr.Address,
                    instr.Length,
                    rtlInstrs.ToArray())
                {
                    Class = rtlc
                };
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Expression RewriteOp(MachineOperand op)
        {
            throw new NotImplementedException();
        }

        private void NZCV(Expression e)
        {
            var grf = arch.GetFlagGroup((uint)(FlagM.NF | FlagM.ZF | FlagM.CF | FlagM.VF));
            m.Assign(binder.EnsureFlagGroup(grf), e);
        }

        private void RewriteAba()
        {
            var left = binder.EnsureRegister(Registers.a);
            var right = binder.EnsureRegister(Registers.b);
            m.Assign(left, m.IAdd(left, right));
            NZCV(left);
        }

        private void RewriteAdc(RegisterStorage reg)
        {
            var left = binder.EnsureRegister(reg);
            var right = RewriteOp(instr.Operands[0]);
            var C = binder.EnsureFlagGroup(arch.GetFlagGroup((uint)FlagM.CF));
            m.Assign(left, m.IAdd(m.IAdd(left, right), C));
            NZCV(left);
        }

        private void RewriteAdd(RegisterStorage reg)
        {
            var left = binder.EnsureRegister(reg);
            var right = RewriteOp(instr.Operands[0]);
            m.Assign(left, m.IAdd(left, right));
            NZCV(left);
        }


        private void RewriteWai()
        {
            throw new NotImplementedException();
        }

        private void RewriteTstb()
        {
            throw new NotImplementedException();
        }

        private void RewriteTsta()
        {
            throw new NotImplementedException();
        }

        private void RewriteTst()
        {
            throw new NotImplementedException();
        }

        private void RewriteTrap()
        {
            throw new NotImplementedException();
        }

        private void RewriteTfr()
        {
            throw new NotImplementedException();
        }

        private void RewriteTbne()
        {
            throw new NotImplementedException();
        }

        private void RewriteTbl()
        {
            throw new NotImplementedException();
        }

        private void RewriteTbeq()
        {
            throw new NotImplementedException();
        }

        private void RewriteTba()
        {
            throw new NotImplementedException();
        }

        private void RewriteTab()
        {
            throw new NotImplementedException();
        }

        private void RewriteSwi()
        {
            throw new NotImplementedException();
        }

        private void RewriteSubd()
        {
            throw new NotImplementedException();
        }

        private void RewriteSubb()
        {
            throw new NotImplementedException();
        }

        private void RewriteSuba()
        {
            throw new NotImplementedException();
        }

        private void RewriteSty()
        {
            throw new NotImplementedException();
        }

        private void RewriteStx()
        {
            throw new NotImplementedException();
        }

        private void RewriteSts()
        {
            throw new NotImplementedException();
        }

        private void RewriteStop()
        {
            throw new NotImplementedException();
        }

        private void RewriteStd()
        {
            throw new NotImplementedException();
        }

        private void RewriteStab()
        {
            throw new NotImplementedException();
        }

        private void RewriteStaa()
        {
            throw new NotImplementedException();
        }

        private void RewriteSex()
        {
            throw new NotImplementedException();
        }

        private void RewriteSbcb()
        {
            throw new NotImplementedException();
        }

        private void RewriteSbca()
        {
            throw new NotImplementedException();
        }

        private void RewriteSba()
        {
            throw new NotImplementedException();
        }

        private void RewriteRts()
        {
            throw new NotImplementedException();
        }

        private void RewriteRti()
        {
            throw new NotImplementedException();
        }

        private void RewriteRtc()
        {
            throw new NotImplementedException();
        }

        private void RewriteRorb()
        {
            throw new NotImplementedException();
        }

        private void RewriteRora()
        {
            throw new NotImplementedException();
        }

        private void RewriteRor()
        {
            throw new NotImplementedException();
        }

        private void RewriteRolb()
        {
            throw new NotImplementedException();
        }

        private void RewriteRola()
        {
            throw new NotImplementedException();
        }

        private void RewriteRol()
        {
            throw new NotImplementedException();
        }

        private void RewriteRevw()
        {
            throw new NotImplementedException();
        }

        private void RewriteRev()
        {
            throw new NotImplementedException();
        }

        private void RewritePuly()
        {
            throw new NotImplementedException();
        }

        private void RewritePulx()
        {
            throw new NotImplementedException();
        }

        private void RewritePuld()
        {
            throw new NotImplementedException();
        }

        private void RewritePulc()
        {
            throw new NotImplementedException();
        }

        private void RewritePulb()
        {
            throw new NotImplementedException();
        }

        private void RewritePula()
        {
            throw new NotImplementedException();
        }

        private void RewritePshy()
        {
            throw new NotImplementedException();
        }

        private void RewritePshx()
        {
            throw new NotImplementedException();
        }

        private void RewritePshd()
        {
            throw new NotImplementedException();
        }

        private void RewritePshc()
        {
            throw new NotImplementedException();
        }

        private void RewritePshb()
        {
            throw new NotImplementedException();
        }

        private void RewritePsha()
        {
            throw new NotImplementedException();
        }

        private void RewriteOrcc()
        {
            throw new NotImplementedException();
        }

        private void RewriteOrab()
        {
            throw new NotImplementedException();
        }

        private void RewriteOraa()
        {
            throw new NotImplementedException();
        }

        private void RewriteNop()
        {
            throw new NotImplementedException();
        }

        private void RewriteNegb()
        {
            throw new NotImplementedException();
        }

        private void RewriteNega()
        {
            throw new NotImplementedException();
        }

        private void RewriteNeg()
        {
            throw new NotImplementedException();
        }

        private void RewriteMul()
        {
            throw new NotImplementedException();
        }

        private void RewriteMov()
        {
            throw new NotImplementedException();
        }

        private void RewriteMinm()
        {
            throw new NotImplementedException();
        }

        private void RewriteMina()
        {
            throw new NotImplementedException();
        }

        private void RewriteMem()
        {
            throw new NotImplementedException();
        }

        private void RewriteMaxm()
        {
            throw new NotImplementedException();
        }

        private void RewriteMaxa()
        {
            throw new NotImplementedException();
        }

        private void RewriteLsrd()
        {
            throw new NotImplementedException();
        }

        private void RewriteLsrb()
        {
            throw new NotImplementedException();
        }

        private void RewriteLsra()
        {
            throw new NotImplementedException();
        }

        private void RewriteLsr()
        {
            throw new NotImplementedException();
        }

        private void RewriteLsld()
        {
            throw new NotImplementedException();
        }

        private void RewriteLslb()
        {
            throw new NotImplementedException();
        }

        private void RewriteLsla()
        {
            throw new NotImplementedException();
        }

        private void RewriteLsl()
        {
            throw new NotImplementedException();
        }

        private void RewriteLeay()
        {
            throw new NotImplementedException();
        }

        private void RewriteLeax()
        {
            throw new NotImplementedException();
        }

        private void RewriteLeas()
        {
            throw new NotImplementedException();
        }

        private void RewriteLdy()
        {
            throw new NotImplementedException();
        }

        private void RewriteLdx()
        {
            throw new NotImplementedException();
        }

        private void RewriteLds()
        {
            throw new NotImplementedException();
        }

        private void RewriteLdd()
        {
            throw new NotImplementedException();
        }

        private void RewriteLdab()
        {
            throw new NotImplementedException();
        }

        private void RewriteLdaa()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbvs()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbvc()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbrn()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbra()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbpl()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbne()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbmi()
        {
            throw new NotImplementedException();
        }

        private void RewriteLblt()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbls()
        {
            throw new NotImplementedException();
        }

        private void RewriteLble()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbhi()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbgt()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbge()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbeq()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbcs()
        {
            throw new NotImplementedException();
        }

        private void RewriteLbcc()
        {
            throw new NotImplementedException();
        }

        private void RewriteJsr()
        {
            throw new NotImplementedException();
        }

        private void RewriteJmp()
        {
            throw new NotImplementedException();
        }

        private void RewriteIny()
        {
            throw new NotImplementedException();
        }

        private void RewriteInx()
        {
            throw new NotImplementedException();
        }

        private void RewriteIncb()
        {
            throw new NotImplementedException();
        }

        private void RewriteInca()
        {
            throw new NotImplementedException();
        }

        private void RewriteInc()
        {
            throw new NotImplementedException();
        }

        private void RewriteIdivs()
        {
            throw new NotImplementedException();
        }

        private void RewriteIdiv()
        {
            throw new NotImplementedException();
        }

        private void RewriteIbne()
        {
            throw new NotImplementedException();
        }

        private void RewriteIbeq()
        {
            throw new NotImplementedException();
        }

        private void RewriteFdiv()
        {
            throw new NotImplementedException();
        }

        private void RewriteEtbl()
        {
            throw new NotImplementedException();
        }

        private void RewriteEorb()
        {
            throw new NotImplementedException();
        }

        private void RewriteEora()
        {
            throw new NotImplementedException();
        }

        private void RewriteEmuls()
        {
            throw new NotImplementedException();
        }

        private void RewriteEmul()
        {
            throw new NotImplementedException();
        }

        private void RewriteEminm()
        {
            throw new NotImplementedException();
        }

        private void RewriteEmind()
        {
            throw new NotImplementedException();
        }

        private void RewriteEmaxm()
        {
            throw new NotImplementedException();
        }

        private void RewriteEmaxd()
        {
            throw new NotImplementedException();
        }

        private void RewriteEmacs()
        {
            throw new NotImplementedException();
        }

        private void RewriteEdivs()
        {
            throw new NotImplementedException();
        }

        private void RewriteEdiv()
        {
            throw new NotImplementedException();
        }

        private void RewriteDey()
        {
            throw new NotImplementedException();
        }

        private void RewriteDex()
        {
            throw new NotImplementedException();
        }

        private void RewriteDecb()
        {
            throw new NotImplementedException();
        }

        private void RewriteDeca()
        {
            throw new NotImplementedException();
        }

        private void RewriteDec()
        {
            throw new NotImplementedException();
        }

        private void RewriteDbne()
        {
            throw new NotImplementedException();
        }

        private void RewriteDbeq()
        {
            throw new NotImplementedException();
        }

        private void RewriteDaa()
        {
            throw new NotImplementedException();
        }

        private void RewriteCpy()
        {
            throw new NotImplementedException();
        }

        private void RewriteCpx()
        {
            throw new NotImplementedException();
        }

        private void RewriteCps()
        {
            throw new NotImplementedException();
        }

        private void RewriteCpd()
        {
            throw new NotImplementedException();
        }

        private void RewriteComb()
        {
            throw new NotImplementedException();
        }

        private void RewriteComa()
        {
            throw new NotImplementedException();
        }

        private void RewriteCom()
        {
            throw new NotImplementedException();
        }

        private void RewriteCmpb()
        {
            throw new NotImplementedException();
        }

        private void RewriteCmpa()
        {
            throw new NotImplementedException();
        }

        private void RewriteClrb()
        {
            throw new NotImplementedException();
        }

        private void RewriteClra()
        {
            throw new NotImplementedException();
        }

        private void RewriteClr()
        {
            throw new NotImplementedException();
        }

        private void RewriteCba()
        {
            throw new NotImplementedException();
        }

        private void RewriteCall()
        {
            throw new NotImplementedException();
        }

        private void RewriteBvs()
        {
            throw new NotImplementedException();
        }

        private void RewriteBvc()
        {
            throw new NotImplementedException();
        }

        private void RewriteBsr()
        {
            throw new NotImplementedException();
        }

        private void RewriteBset()
        {
            throw new NotImplementedException();
        }

        private void RewriteBrset()
        {
            throw new NotImplementedException();
        }

        private void RewriteBrn()
        {
            throw new NotImplementedException();
        }

        private void RewriteBrclr()
        {
            throw new NotImplementedException();
        }

        private void RewriteBra()
        {
            throw new NotImplementedException();
        }

        private void RewriteBpl()
        {
            throw new NotImplementedException();
        }

        private void RewriteBne()
        {
            throw new NotImplementedException();
        }

        private void RewriteBmi()
        {
            throw new NotImplementedException();
        }

        private void RewriteBlt()
        {
            throw new NotImplementedException();
        }

        private void RewriteBls()
        {
            throw new NotImplementedException();
        }

        private void RewriteBle()
        {
            throw new NotImplementedException();
        }

        private void RewriteBitb()
        {
            throw new NotImplementedException();
        }

        private void RewriteBita()
        {
            throw new NotImplementedException();
        }

        private void RewriteBhi()
        {
            throw new NotImplementedException();
        }

        private void RewriteBgt()
        {
            throw new NotImplementedException();
        }

        private void RewriteBgnd()
        {
            throw new NotImplementedException();
        }

        private void RewriteBge()
        {
            throw new NotImplementedException();
        }

        private void RewriteBeq()
        {
            throw new NotImplementedException();
        }

        private void RewriteBcs()
        {
            throw new NotImplementedException();
        }

        private void RewriteBclr()
        {
            throw new NotImplementedException();
        }

        private void RewriteBcc()
        {
            throw new NotImplementedException();
        }

        private void RewriteAsrb()
        {
            throw new NotImplementedException();
        }

        private void RewriteAsra()
        {
            throw new NotImplementedException();
        }

        private void RewriteAsr()
        {
            throw new NotImplementedException();
        }

        private void RewriteAndcc()
        {
            throw new NotImplementedException();
        }

        private void RewriteAndb()
        {
            throw new NotImplementedException();
        }

        private void RewriteAnda()
        {
            throw new NotImplementedException();
        }

        private void RewriteWav()
        {
            throw new NotImplementedException();
        }

    }
}
