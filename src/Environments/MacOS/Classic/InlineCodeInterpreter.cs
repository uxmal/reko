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

using Reko.Arch.M68k.Machine;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Hll.Pascal;
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Environments.MacOS.Classic
{
    /// <summary>
    /// Interprets the INLINE bytes in the MPW Pascal file to deduce the 
    /// calling convention used when invoking a MacOS/Toolbox A-line trap.
    /// </summary>
    public class InlineCodeInterpreter
    {
        private IDictionary<string, Expression> constants;
        private ushort[] uOpcodes;
        private List<SerializedRegValue> regValues;
        private Stack<(int, string?)> stackValues;

        public InlineCodeInterpreter(IDictionary<string, Expression> constants)
        {
            this.constants = constants;
            this.uOpcodes = null!;
            this.regValues = null!;
            this.stackValues = null!;
        }

        public SerializedService? BuildSystemCallFromMachineCode(
            string name,
            SerializedSignature ssig,
            List<Exp> opcodes)
        {
            this.uOpcodes = opcodes.Select(EvaluateOpcode).ToArray();
            if (!uOpcodes.Any(IsALineTrap))
            {
                return null;
            }
            if (uOpcodes.Length == 1 && IsALineTrap(uOpcodes[0]))
            {
                return PascalSystemCall(name, ssig, uOpcodes[0]);
            }
            int iArg = ssig.Arguments!.Length-1;
            int vector = 0;
            int iOpcode;
            this.regValues = new List<SerializedRegValue>();
            this.stackValues = new Stack<(int, string?)>();
            for (iOpcode = 0; iOpcode < uOpcodes.Length; ++iOpcode)
            {
                ushort uInstr = uOpcodes[iOpcode];
                var reg = PopRegister(uInstr);
                if (reg is not null)
                {
                    ssig.Arguments[iArg].Kind = new Register_v1(reg.Name);
                    --iArg;
                    continue;
                }
                var regValue = QuickConstant(uInstr);
                if (regValue is not null)
                {
                    regValues.Add(regValue);
                    continue;
                }
                regValue = InlineConstant(uInstr, uOpcodes, ref iOpcode);
                if (regValue is not null)
                {
                    regValues.Add(regValue);
                    continue;
                }
                var stackValue = PushConstant(uInstr, uOpcodes, ref iOpcode);
                if (stackValue.sValue is not null)
                {
                    stackValues.Push(stackValue);
                    continue;
                }
                reg = PushRegister(uInstr);
                if (reg is not null)
                {
                    var i = regValues.FindIndex(r => r.Register == reg.Name);
                    regValue = regValues[i];
                    regValues.RemoveAt(i);

                    stackValues.Push((4, regValue.Value));
                    continue;
                }
                if (IsIgnorable(uInstr, ref iOpcode))
                {
                    continue;
                }

                if (IsALineTrap(uInstr))
                {
                    vector = uInstr;
                    ++iOpcode;
                    break;
                }
                Debug.Print("****** pre: uInstr: {0:X4}", uInstr);
                return null;
                //throw new NotImplementedException(string.Format("uInstr: {0:X4}", uInstr));
            }
            if (iOpcode < uOpcodes.Length)
            {
                for (; iOpcode < uOpcodes.Length; ++iOpcode)
                {
                    ushort uInstr = uOpcodes[iOpcode];
                    var reg = PostCallRegisterStore(uInstr);
                    if (reg is not null)
                    {
                        if (ssig.ReturnValue is null)
                            throw new InvalidOperationException(
                                $"Service {name} is specified as returning void, but returns value in {reg.Name}.");
                        ssig.ReturnValue.Kind = new Register_v1 { Name = reg.Name };
                        continue;
                    }
                    Debug.Print("****** post: uInstr: {0:X4}", uInstr);
                    return null;
                    //throw new NotImplementedException(string.Format("uInstr: {0:X4}", uInstr));
                }
            }
            if (vector == 0)
                return null;

            int offset = 4;
            var sStackValues = new List<StackValue_v1>();
            while (stackValues.Count > 0)
            {
                var t = stackValues.Pop();
                sStackValues.Add(new StackValue_v1
                {
                    Offset = offset.ToString("X"),
                    Value = t.Item2
                });
                offset += t.Item1;
            }
            ssig.StackDelta = offset + 4;   // 4 = return address size.
            var syscallinfo = new SyscallInfo_v1
            {
                Vector = vector.ToString("X4"),
                RegisterValues = regValues.Count > 0 ? regValues.ToArray() : null,
                StackValues = sStackValues.Count > 0 ? sStackValues.ToArray() : null,
            };
            return new SerializedService
            {
                Name = name,
                Signature = ssig,
                SyscallInfo = syscallinfo
            };
        }

        private RegisterStorage? PushRegister(ushort uInstr)
        {
            var opcode = (uInstr & 0xFFF8);
            switch (opcode)
            {
            case 0x2F00:    // move.l dN,-(a7)
            case 0x3F00:    // move.w dN,-(a7)
                return Registers.DataRegister(uInstr & 7);
            case 0x2F08:    // move.l aN,-(a7)
                return Registers.AddressRegister(uInstr & 7);
            }
            return null;
        }

        private SerializedRegValue? InlineConstant(ushort uInstr, ushort[] uOpcodes, ref int i)
        {
            RegisterStorage reg;
            switch (uInstr & 0xF1FF)
            {
            case 0x203C:    // move.l   <w32>,dN
                var n4 = (uOpcodes[i + 1] << 16 | uOpcodes[i + 2]).ToString("X8");
                i += 2;
                reg = Registers.DataRegister((uInstr >> 9) & 7);
                return new SerializedRegValue(reg.Name, n4);
            case 0x303C:    // move.w   <w16>,dN
                var n2 = uOpcodes[i + 1].ToString("X4");
                i += 1;
                reg = Registers.DataRegister((uInstr >> 9) & 7);
                return new SerializedRegValue(reg.Name, n2);
            default:
                return null;
            }
        }

        bool IsIgnorable(ushort uInstr, ref int i)
        {
            switch (uInstr)
            {
            case 0x1010:    // move.l (a0),d0
                return true;
            case 0x2038:    // move.l   <w16>,dN
                i += 1;     // Skip short address
                return true;
            case 0x4840:    // swap.l d0
                return true;
            }
            return false;
        }

        private (int cBytes, string? sValue) PushConstant(ushort uInstr, ushort[] uOpcodes, ref int i)
        {
            switch (uInstr)
            {
            case 0x2F3C:        // move.l <w32>,-(a7)
                i += 2; 
                return (4, (uOpcodes[i - 1] << 16 | uOpcodes[i]).ToString("X8"));
            case 0x3F3C:        // move.w <w16>,-(a7)
                i += 1;
                return (2, (uOpcodes[i]).ToString("X4"));
            case 0x4267:        // clr.w -(a7)
                return (2, "0000");
            case 0x42A7:        // clr.l -(a7)
                return (4, "00000000");
            default:
                return (0, null);
            }
        }

        private RegisterStorage? PostCallRegisterStore(ushort uInstr)
        {
            var uMasked = uInstr & 0xFFF8;
            switch (uMasked)
            {
            case 0x1E80:    // move.b (sp),dX
            case 0x2E80:    // move.w (sp),dX
            case 0x2080:    // move.l dX,(a0)
            case 0x2280:    // move.w d0,(a1)
            case 0x3E80:    // move.l (sp),dX
            case 0x5240:    // addq.w #1,dX
            case 0x1080:    // move.b dX,(a0)
                return Registers.DataRegister(uInstr & 7);
            case 0x2E88:    // movea  (sp),aX
            case 0x2288:    // move.l aX,aY
                return Registers.AddressRegister(uInstr & 7);
            }
            //case 0x2257:    // move.w (sp+),aX
            //if (uInstr == )
            return null;
        }

        private SerializedRegValue? QuickConstant(ushort uInstr)
        {
            if ((uInstr & 0xF100) == 0x7000)
            {
                // moveq $xx,dx
                var reg = Registers.DataRegister((uInstr >> 9) & 7);
                var value = Convert.ToString((int)(sbyte)uInstr, 16);
                return new SerializedRegValue(reg.Name, value);
            }
            return null;
        }

        private RegisterStorage? PopRegister(ushort uInstr)
        {
            switch (uInstr & 0xF1FF)
            {
            case 0x205F:    // movea $(sp)+,aX
            case 0x305F:    // movea.w $(sp)+,aX
                return Registers.AddressRegister((uInstr >> 9) & 7);
            case 0x101F:    // move.b $(sp)+,dX
            case 0x201F:    // move.l $(sp)+,dX
            case 0x301F:    // move.w $(sp)+,dX
                return Registers.DataRegister((uInstr >> 9) & 7);
            default:
                return null;
            }
        }

        private SerializedService PascalSystemCall(
            string name, 
            SerializedSignature ssig, 
            ushort vector)
        {
            ssig.Convention = "pascal";
            return new SerializedService
            {
                Ordinal = vector,
                Name = name,
                Signature = ssig,
            };
        }

        public ushort EvaluateOpcode(Exp exp)
        {
            if (exp is Id id)
            {
                var n = constants[id.Name];
                return (ushort) ((Constant)n).ToInt64();
            }
            if (exp is NumericLiteral num)
            {
                return (ushort) num.Value;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the instruction is of the form Axxx which is the 
        /// form used to trigger traps on MacOS.
        /// </summary>
        /// <param name="uInstr"></param>
        /// <returns></returns>
        private bool IsALineTrap(ushort uInstr)
        {
            return ((uInstr & 0xF000) == 0xA000);
        }
    }
}
