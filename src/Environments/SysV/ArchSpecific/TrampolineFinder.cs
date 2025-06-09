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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class TrampolineFinder
    {
        /// <summary>
        /// Find the destination of a ARM PLT stub.
        /// </summary>
        public static (Expression?, Address) Arm32(IProcessorArchitecture arch, Address addrInstr, List<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            var dst = Arm32_variant1(arch, host, instrs);
            if (dst.Item1 is not null)
                return dst;
            dst = Arm32_variant2(arch, host, instrs);
            return dst;
        }

        public static Expression? Arm32_Old(IProcessorArchitecture arch, Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            var stubInstrs = instrs.Take(4).ToArray();

            var dst = Arm32_variant1_Old(arch, host, stubInstrs);
            if (dst is not null)
                return dst;
            dst = Arm32_variant2_Old(arch, host, stubInstrs);
            if (dst is not null)
                return dst;
            return null;
        }

        /// <summary>
        /// Finds the destination of an ARM PLT stub of the following type:
        ///     ldr rx,[addr1]
        ///     add ry,addr2,rx
        ///     ldr pc,[ry]
        /// </summary>
        /// <param name="arch"></param>
        /// <param name="host"></param>
        /// <param name="stubInstrs"></param>
        /// <returns></returns>
        private static (Expression?, Address) Arm32_variant1(IProcessorArchitecture arch, IRewriterHost host, List<RtlInstructionCluster> stubInstrs)
        {
            if (stubInstrs.Count < 3)
                return (null, default);
            Constant? offset;
            if (stubInstrs[^3].Instructions[0] is RtlAssignment ass &&
                ass.Src is MemoryAccess mem &&
                mem.EffectiveAddress is Address addrOffset &&
                mem.DataType is PrimitiveType dt &&
                dt.BitSize == 32)
            {
                if (!host.TryRead(arch, addrOffset, dt, out offset))
                    return (null, default);
            }
            else return (null, default);

            if (stubInstrs[^2].Instructions[0] is RtlAssignment ass2 &&
                ass2.Src is BinaryExpression bin &&
                bin.Operator is IAddOperator &&
                bin.Left is Address addr &&
                bin.Right == ass.Dst)
            {
                addr = addr + offset.ToInt32();
            }
            else return (null, default);

            if (stubInstrs[^1].Instructions[0] is RtlGoto g &&
                g.Target is MemoryAccess mem2 &&
                mem2.EffectiveAddress == ass2.Dst)
            {
                return (addr, stubInstrs[^3].Address);
            }
            else return (null, default);
        }

        private static Expression? Arm32_variant1_Old(IProcessorArchitecture arch, IRewriterHost host, RtlInstruction[] stubInstrs)
        {
            if (stubInstrs.Length < 3)
                return null;
            Constant? offset;
            if (stubInstrs[0] is RtlAssignment ass &&
                ass.Src is MemoryAccess mem &&
                mem.EffectiveAddress is Address addrOffset &&
                mem.DataType is PrimitiveType dt &&
                dt.BitSize == 32)
            {
                if (!host.TryRead(arch, addrOffset, dt, out offset))
                    return null;
            }
            else return null;

            if (stubInstrs[1] is RtlAssignment ass2 &&
                ass2.Src is BinaryExpression bin &&
                bin.Operator is IAddOperator &&
                bin.Left is Address addr &&
                bin.Right == ass.Dst)
            {
                addr = addr + offset.ToInt32();
            }
            else return null;

            if (stubInstrs[2] is RtlGoto g &&
                g.Target is MemoryAccess mem2 &&
                mem2.EffectiveAddress == ass2.Dst)
            {
                return addr;
            }
            else return null;
        }

        /// <summary>
        /// Finds the destination of an ARM PLT stub of the following type:
        /// add ip,pc,#0
        /// add ip, ip,#&64000
        /// ldr pc,[ip,#&7E0]!
        /// </summary>
        /// <param name="arch"></param>
        /// <param name="host"></param>
        /// <param name="stubInstrs"></param>

        private static (Expression?, Address) Arm32_variant2(IProcessorArchitecture arch, IRewriterHost host, List<RtlInstructionCluster> stubInstrs)
        {
            if (stubInstrs.Count < 4)
                return (null, default);
            Address addr;
            // ip = 0x00010A9C<p32> + 0<32>
            if (stubInstrs[^4].Instructions[0] is RtlAssignment ass &&
                ass.Dst is Identifier dst &&
                ass.Src is BinaryExpression bin &&
                bin.Operator is IAddOperator &&
                bin.Left is Address addrPc &&
                bin.Right is Constant pcOffset)
            {
                addr = addrPc + pcOffset.ToInt32();
            }
            else
                return (null, default);

            // ip = ip + 0x64000<32>
            if (stubInstrs[^3].Instructions[0] is RtlAssignment ass1 &&
                ass1.Dst == dst &&
                ass1.Src is BinaryExpression bin1 &&
                bin1.Operator is IAddOperator &&
                bin1.Left == dst &&
                bin1.Right is Constant offset1)
            {
                addr = addr + offset1.ToInt32();
            }
            else
                return (null, default);

            // ip = ip + 1864<i32>
            if (stubInstrs[^2].Instructions[0] is RtlAssignment ass2 &&
                ass2.Dst == dst &&
                ass2.Src is BinaryExpression bin2 &&
                bin2.Operator is IAddOperator &&
                bin2.Left == dst &&
                bin2.Right is Constant offset2)
            {
                addr = addr + offset2.ToInt32();
            }
            else
                return (null, default);

            // goto Mem0[ip: word32]
            if (stubInstrs[^1].Instructions[0] is RtlGoto g &&
                g.Target is MemoryAccess mem &&
                mem.EffectiveAddress == dst &&
                mem.DataType.BitSize == 32)
            {
                return (addr, stubInstrs[^4].Address);
            }
            return (null, default);
        }

        private static Expression? Arm32_variant2_Old(IProcessorArchitecture arch, IRewriterHost host, RtlInstruction[] stubInstrs)
        {
            if (stubInstrs.Length < 4)
                return null;
            Address addr;
            // ip = 0x00010A9C<p32> + 0<32>
            if (stubInstrs[0] is RtlAssignment ass &&
                ass.Dst is Identifier dst &&
                ass.Src is BinaryExpression bin &&
                bin.Operator is IAddOperator &&
                bin.Left is Address addrPc &&
                bin.Right is Constant pcOffset)
            {
                addr = addrPc + pcOffset.ToInt32();
            }
            else
                return null;

            // ip = ip + 0x64000<32>
            if (stubInstrs[1] is RtlAssignment ass1 &&
                ass1.Dst == dst &&
                ass1.Src is BinaryExpression bin1 &&
                bin1.Operator is IAddOperator &&
                bin1.Left == dst &&
                bin1.Right is Constant offset1)
            {
                addr = addr + offset1.ToInt32();
            }
            else
                return null;

            // ip = ip + 1864<i32>
            if (stubInstrs[2] is RtlAssignment ass2 &&
                ass2.Dst == dst &&
                ass2.Src is BinaryExpression bin2 &&
                bin2.Operator is IAddOperator &&
                bin2.Left == dst &&
                bin2.Right is Constant offset2)
            {
                addr = addr + offset2.ToInt32();
            }
            else
                return null;

            // goto Mem0[ip: word32]
            if (stubInstrs[3] is RtlGoto g &&
                g.Target is MemoryAccess mem &&
                mem.EffectiveAddress == dst &&
                mem.DataType.BitSize == 32)
            {
                return addr;
            }
            return null;
        }

        public static (Expression?, Address) AArch64(IProcessorArchitecture arch, Address addrInstr, List<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            // adrp x16,#&13000
            //ldr x17,[x16]
            //add x16, x16,#0
            //br x17

            if (instrs.Count < 4)
                return (null, default);

            if (instrs[^4].Instructions[0] is RtlAssignment ass &&
                ass.Dst is Identifier idPage &&
                ass.Src is Address addrPage && 
                idPage.Name == "x16")
            {
            }
            else return (null, default);
            Address addr;
            if (instrs[^3].Instructions[0] is RtlAssignment load &&
                load.Dst is Identifier ptrGotSlot &&
                load.Src is MemoryAccess gotslot)
            {
                if (gotslot.EffectiveAddress is BinaryExpression bin &&
                    bin.Operator == Operator.IAdd &&
                    bin.Left == idPage &&
                    bin.Right is Constant offset)
                {
                    addr = addrPage + offset.ToInt32();
                }
                else if (gotslot.EffectiveAddress is Identifier idEa &&
                  idEa == idPage)
                {
                    addr = addrPage;
                }
                else
                    return (null, default);
            }
            else return (null, default);

            if (instrs[^2].Instructions[0] is RtlAssignment addrOf &&
                addrOf.Dst != ptrGotSlot)
            {
            }
            else return (null, default);

            if (instrs[^1].Instructions[0] is RtlGoto g && 
                g.Target == ptrGotSlot)
            {
                return (addr, instrs[^4].Address);
            }
            return (null, default);
        }

        public static Expression? AArch64_Old(IProcessorArchitecture arch, Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            // adrp x16,#&13000
            //ldr x17,[x16]
            //add x16, x16,#0
            //br x17

            var stubInstrs = instrs.Take(4).ToArray();
            if (stubInstrs.Length != 4)
                return null;

            if (stubInstrs[0] is RtlAssignment ass &&
                ass.Dst is Identifier idPage &&
                ass.Src is Address addrPage && 
                idPage.Name == "x16")
            {
            }
            else return null;
            Address addr;
            if (stubInstrs[1] is RtlAssignment load &&
                load.Dst is Identifier ptrGotSlot &&
                load.Src is MemoryAccess gotslot)
            {
                if (gotslot.EffectiveAddress is BinaryExpression bin &&
                    bin.Operator.Type == OperatorType.IAdd &&
                    bin.Left == idPage &&
                    bin.Right is Constant offset)
                {
                    addr = addrPage + offset.ToInt32();
                }
                else if (gotslot.EffectiveAddress is Identifier idEa &&
                  idEa == idPage)
                {
                    addr = addrPage;
                }
                else
                    return null;
            }
            else return null;

            if (stubInstrs[2] is RtlAssignment addrOf &&
                addrOf.Dst != ptrGotSlot)
            {
            }
            else return null;

            if (stubInstrs[3] is RtlGoto g && 
                g.Target == ptrGotSlot)
            {
                return addr;
            }
            return null;
        }


        public static Expression? Mips32(IProcessorArchitecture arch, Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            var stubInstrs = instrs.Take(4).ToArray();

            var dst = Mips32_Variant1(arch, host, stubInstrs);
            if (dst is { })
                return dst;
            return Mips32_Variant2(arch, host, stubInstrs);
        }

        /// <summary>
        /// Find the destination of a MIPS32 plt stub.
        /// </summary>
        /// <code>
        /// lw r25,-7FF0(r28)
        /// addu r15,ra,r0
        /// jalr ra,r25
        /// addiu r24,r0,+00000010
        /// </code>
        public static (Expression?, Address) Mips32(IProcessorArchitecture arch, Address addrInstr, List<RtlInstructionCluster> stubInstrs, IRewriterHost host)
        {
            if (stubInstrs.Count != 4)
                return (null, default);

            if (stubInstrs[^4].Instructions[0] is RtlAssignment load &&
                load.Dst is Identifier idDst &&
                idDst.Name == "r25" &&
                load.Src is MemoryAccess mem &&
                mem.EffectiveAddress is BinaryExpression bin &&
                bin.Left is Identifier gp &&
                gp.Name == "r28")
            {
            }
            else return (null, default);

            if (stubInstrs[^3].Instructions[0] is RtlAssignment copy &&
                copy.Src is Identifier idSrc &&
                idSrc.Name == "ra")
            {
            }
            else return (null, default);

            if (stubInstrs[^2].Instructions[0] is RtlCall call &&
                call.Class.HasFlag(InstrClass.Delay) &&
                call.Target is Identifier idTarget &&
                idTarget == idDst)
            { }
            else return (null, default);

            if (stubInstrs[^1].Instructions[0] is RtlAssignment init &&
                init.Dst is Identifier &&
                init.Src is Constant &&
                host.GlobalRegisterValue is { })
            {
                var sAddrGotSlot = host.GlobalRegisterValue.ToInt32() + ((Constant) bin.Right).ToInt32();
                return (Address.Ptr32((uint)sAddrGotSlot), stubInstrs[^4].Address);
            }
            return (null, default);
        }

        public static Expression? Mips32_Variant1(IProcessorArchitecture arch, IRewriterHost host, RtlInstruction[] instrs)
        {
            var stubInstrs = instrs.Take(4).ToArray();
            if (stubInstrs.Length != 4)
                return null;

            if (stubInstrs[0] is RtlAssignment load &&
                load.Dst is Identifier idDst &&
                idDst.Name == "r25" &&
                load.Src is MemoryAccess mem &&
                mem.EffectiveAddress is BinaryExpression bin &&
                bin.Left is Identifier gp &&
                gp.Name == "r28")
            {
            }
            else return null;

            if (stubInstrs[1] is RtlAssignment copy &&
                copy.Src is Identifier idSrc &&
                idSrc.Name == "ra")
            {
            }
            else return null;

            if (stubInstrs[2] is RtlCall call &&
                call.Class.HasFlag(InstrClass.Delay) &&
                call.Target is Identifier idTarget &&
                idTarget == idDst)
            { }
            else return null;

            if (stubInstrs[3] is RtlAssignment init &&
                init.Dst is Identifier &&
                init.Src is Constant &&
                host.GlobalRegisterValue is { })
            {
                var sAddrGotSlot = host.GlobalRegisterValue.ToInt32() + ((Constant) bin.Right).ToInt32();
                return Address.Ptr32((uint)sAddrGotSlot);
            }
            return null;
        }

        /// <summary>
        /// Find the destination of a MIPS32 plt stub.
        /// </summary>
        /// r15 = 0x00410000
        /// r25 = Mem0[r15 + 0x111C:word32]
        /// r24 = r15 + 0x0000111C
        /// call r25 (retsize: 0;)
        public static Expression? Mips32_Variant2(IProcessorArchitecture arch, IRewriterHost host, RtlInstruction[] stubInstrs)
        {
            if (stubInstrs.Length < 3)
                return null;

            if (stubInstrs[0] is RtlAssignment ass &&
                ass.Dst is Identifier idBase &&
                ass.Src is Constant imm)
            {
            }
            else return null;

            if (stubInstrs[1] is RtlAssignment load &&
                load.Dst is Identifier idDst &&
                idDst.Name == "r25" &&
                load.Src is MemoryAccess mem &&
                mem.EffectiveAddress is BinaryExpression bin &&
                bin.Left == idBase && 
                bin.Right is Constant immOffset)
            {
            }
            else return null;

            if (stubInstrs[2] is RtlGoto jmp &&
                jmp.Class.HasFlag(InstrClass.Delay) &&
                jmp.Target is Identifier idTarget &&
                idTarget == idDst)
            { }
            else return null;

            var sAddrGotSlot = imm.ToInt32() + immOffset.ToInt32();
            return Address.Ptr32((uint) sAddrGotSlot);
        }


        private static bool IsRegister(Expression? e, int regNumber)
        {
            return (e is Identifier id &&
                id.Storage is RegisterStorage reg &&
                reg.Number == regNumber);
        }

        //Mem0[r1 + 24:word64] = r2
        //r12 = r2 + ~0xFFFF
        //r12 = Mem0[r12 + 32440:word64]
        //ctr = r12
        //goto ctr
        private static readonly RtlInstructionMatcher[] ppcStub =
            RtlInstructionMatcher.Build(
                m => m.Assign(
                    m.Mem64(m.IAdd(
                        m.AnyId("base"),
                        m.AnyConst())),
                    m.AnyId("src")),
                m => m.Assign(
                    m.AnyId("dst"),
                    m.IAdd(m.AnyId("src"), m.AnyConst("c"))),
                m => m.Assign(
                    m.AnyId("dst"),
                    m.Mem64(m.IAdd(
                        m.AnyId("base"),
                        m.AnyConst("offset")))),
                m => m.Assign(m.AnyId("dst"), m.AnyId("src")),
                m => m.Goto(m.AnyId("target")));

        /// <summary>
        /// Find the destination of a PowerPC64 PLT stub.
        /// </summary>
        public static (Expression?, Address) PowerPC64(IProcessorArchitecture arch, Address addrInstr, List<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            if (instrs.Count < 5)
                return default;
            return PowerPC64(arch, addrInstr, new[]
            {
                instrs[^5].Instructions[0],
                instrs[^4].Instructions[0],
                instrs[^3].Instructions[0],
                instrs[^2].Instructions[0],
                instrs[^1].Instructions[0],
            }.ToList(), host);

        }

        public static (Expression?, Address) PowerPC64(
            IProcessorArchitecture arch,
            Address addrXferInstr,
            List<RtlInstruction> instrs,
            IRewriterHost host)
        {
            if (instrs.Count < 5)
                return default;
            var m0 = ppcStub[0].Match(instrs[0]);
            var m1 = ppcStub[1].Match(instrs[1]);
            var m2 = ppcStub[2].Match(instrs[2]);
            var m3 = ppcStub[3].Match(instrs[3]);
            var m4 = ppcStub[4].Match(instrs[4]);
            if (!m0.Success || !m1.Success || !m2.Success || !m3.Success || !m4.Success)
                return default;

            if (!IsRegister(m0.CapturedExpression("base"), 1))
                return default;
            if (!IsRegister(m0.CapturedExpression("src"), 2))
                return default;

            if (!IsRegister(m1.CapturedExpression("src"), 2))
                return default;
            var hiword = (Constant?) m1.CapturedExpression("c");
            if (hiword is null)
                return default;

            if (m2.CapturedExpression("base") != m1.CapturedExpression("dst"))
                return default;
            var loword = (Constant?) m2.CapturedExpression("offset");
            if (loword is null)
                return default;

            if (m3.CapturedExpression("src") != m2.CapturedExpression("dst"))
                return default;

            if (m4.CapturedExpression("target") != m3.CapturedExpression("dst"))
                return default;

            if (host.GlobalRegisterValue is null)
                return default;

            var uAddr = host.GlobalRegisterValue.ToUInt64() + hiword.ToUInt64() + loword.ToUInt64();
            var addr = Address.Ptr64(uAddr);

            return (addr, addrXferInstr);
        }

        public static Expression? PowerPC64_Old(IProcessorArchitecture arch, Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            var stubInstrs = instrs.Take(5).ToList();
            var (addr, _) = PowerPC64(arch, addrInstr, stubInstrs, host);
            return addr;
        }


        /// <summary>
        /// Find the destination of a Risc-V PLT stub.
        /// </summary>
        /// <code>
        /// auipc   t3,00000005
        /// ld      t3,t3,+000000F0
        /// jalr    t1,t3,+00000000
        /// </code>
        /// <param name="addrInstr">Address of the beginning of the stub.</param>
        public static (Expression?, Address) RiscV(IProcessorArchitecture arch, Address addrInstr, List<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            if (instrs.Count < 4)
                return (null, default);
            if (instrs[^4].Instructions[0] is RtlAssignment ass &&
                ass.Src is Address addr)
            {

            }
            else return (null, default);

            if (instrs[^3].Instructions[0] is RtlAssignment ld &&
                ld.Src is MemoryAccess mem &&
                mem.EffectiveAddress is BinaryExpression bin &&
                bin.Operator == Operator.IAdd &&
                bin.Left == ass.Dst &&
                bin.Right is Constant offset)
            {
                addr = addr + offset.ToInt64();
            }
            else return (null, default);

            if (instrs[^2].Instructions[0] is RtlAssignment &&
                instrs[^1].Instructions[0] is RtlGoto g &&
                g.Target is Identifier target &&
                target == ld.Dst)
            {
                return (addr, instrs[^4].Address);
            }
            else return (null, default);
        }

        public static Expression? RiscV_Old(IProcessorArchitecture arch, Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            var stubInstrs = instrs.Take(4).ToArray();
            if (stubInstrs.Length != 4)
                return null;
            if (stubInstrs[0] is RtlAssignment ass &&
                ass.Src is Address addr)
            {

            }
            else return null;

            if (stubInstrs[1] is RtlAssignment ld &&
                ld.Src is MemoryAccess mem &&
                mem.EffectiveAddress is BinaryExpression bin &&
                bin.Operator.Type == OperatorType.IAdd &&
                bin.Left == ass.Dst &&
                bin.Right is Constant offset)
            {
                addr = addr + offset.ToInt64();
            }
            else return null;

            if (stubInstrs[2] is RtlAssignment &&
                stubInstrs[3] is RtlGoto g &&
                g.Target is Identifier target &&
                target == ld.Dst)
            {
                return addr;
            }
            else return null;
        }

        public static (Expression?, Address) X86(IProcessorArchitecture arch, Address addrInstr, List<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            if (instrs.Count < 1)
                return (null, default);
            var instr = instrs[^1].Instructions[^1];
            // Match x86 pattern.
            // jmp [destination]
            Address? addrTarget = null;
            if (instr is RtlGoto jump)
            {
                if (jump.Target is ProcedureConstant pc)
                    return (pc, instrs[^1].Address);
                if (jump.Target is not MemoryAccess access)
                    return (null, default);
                if (access.EffectiveAddress is not Address a)
                {
                    if (access.EffectiveAddress is not Constant wAddr)
                    {
                        return (null, default);
                    }
                    addrTarget = arch.MakeAddressFromConstant(wAddr, true);
                }
                else
                {
                    addrTarget = a;
                }
            }
            return (addrTarget, instrs[^1].Address);
        }

        public static Expression? X86_Old(IProcessorArchitecture arch, Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            var instr = instrs.FirstOrDefault();
            if (instr is null)
                return null;
            // Match x86 pattern.
            // jmp [destination]
            Address? addrTarget = null;
            if (instr is RtlGoto jump)
            {
                if (jump.Target is ProcedureConstant pc)
                    return pc;
                if (!(jump.Target is MemoryAccess access))
                    return null;
                if (access.EffectiveAddress is not Address a)
                {
                    if (!(access.EffectiveAddress is Constant wAddr))
                    {
                        return null;
                    }
                    addrTarget = arch.MakeAddressFromConstant(wAddr, true);
                }
                else
                {
                    addrTarget = a;
                }
            }
            return addrTarget;
        }

        public static (Expression?, Address) X86_64(IProcessorArchitecture arch, Address addrInstr, List<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            if (instrs.Count < 1)
                return (null, default);
            var instr = instrs[^1].Instructions[^1];
            // Match x86-64 pattern.
            // jmp [destination]
            Address? addrTarget = null;
            if (instr is RtlGoto jump)
            {
                if (jump.Target is ProcedureConstant pc)
                    return (pc, instrs[^1].Address);
                if (jump.Target is not MemoryAccess access)
                    return (null, default);
                if (access.EffectiveAddress is not Address a)
                {
                    if (access.EffectiveAddress is not Constant wAddr)
                    {
                        return (null, default);
                    }
                    addrTarget = arch.MakeAddressFromConstant(wAddr, true);
                }
                else 
                {
                    addrTarget = a;
                }
            }
            return (addrTarget, instrs[^1].Address);
        }

        public static Expression? X86_64_Old(IProcessorArchitecture arch, Address addrInstr, IEnumerable<RtlInstruction> instrs, IRewriterHost host)
        {
            var instr = instrs.FirstOrDefault();
            if (instr is null)
                return null;
            // Eat leading nops; these are caused by endbr64 instructions.
            if (instr is RtlNop)
            {
                instrs = instrs.Skip(1);
                instr = instrs.FirstOrDefault();
                if (instr is null)
                    return null;
            }
            // Match x86-64 pattern.
            // jmp [destination]
            Address? addrTarget = null;
            if (instr is RtlGoto jump)
            {
                if (jump.Target is ProcedureConstant pc)
                    return pc;
                if (jump.Target is not MemoryAccess access)
                    return null;
                if (access.EffectiveAddress is not Address a)
                {
                    if (access.EffectiveAddress is not Constant wAddr)
                    {
                        return null;
                    }
                    addrTarget = arch.MakeAddressFromConstant(wAddr, true);
                }
                else
                {
                    addrTarget = a;
                }
            }
            return addrTarget;
        }
    }
}
