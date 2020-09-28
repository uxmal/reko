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

using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Pascal;
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.MacOS.Classic
{
    /// <summary>
    /// Interprets the INLINE bytes in the MPW Pascal file to deduce the 
    /// calling convention used when invoking a MacOS/Toolbox A-line trap.
    /// </summary>
    public class InlineCodeInterpreter
    {
        private IDictionary<string, Constant> constants;
        private ushort[] uOpcodes;
        private List<SerializedRegValue> regValues;
        private Stack<Tuple<int, string>> stackValues;

        public InlineCodeInterpreter(IDictionary<string, Constant> constants)
        {
            this.constants = constants;
        }

        public SerializedService BuildSystemCallFromMachineCode(
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
            int iArg = ssig.Arguments.Length-1;
            int vector = 0;
            int iOpcode;
            this.regValues = new List<SerializedRegValue>();
            this.stackValues = new Stack<Tuple<int, string>>();
            for (iOpcode = 0; iOpcode < uOpcodes.Length; ++iOpcode)
            {
                ushort uInstr = uOpcodes[iOpcode];
                var reg = PopRegister(uInstr);
                if (reg != null)
                {
                    ssig.Arguments[iArg].Kind = new Register_v1(reg.Name);
                    --iArg;
                    continue;
                }
                var regValue = QuickConstant(uInstr);
                if (regValue != null)
                {
                    regValues.Add(regValue);
                    continue;
                }
                regValue = InlineConstant(uInstr, uOpcodes, ref iOpcode);
                if (regValue != null)
                {
                    regValues.Add(regValue);
                    continue;
                }
                var stackValue = PushConstant(uInstr, uOpcodes, ref iOpcode);
                if (stackValue != null)
                {
                    stackValues.Push(stackValue);
                    continue;
                }
                reg = PushRegister(uInstr);
                if (reg != null)
                {
                    var i = regValues.FindIndex(r => r.Register == reg.Name);
                    regValue = regValues[i];
                    regValues.RemoveAt(i);

                    stackValues.Push(Tuple.Create(4, regValue.Value));
                    continue;
                }
                if (IsALineTrap(uInstr))
                {
                    vector = uInstr;
                    ++iOpcode;
                    break;
                }
                throw new NotImplementedException(string.Format("uInstr: {0:X4}", uInstr));
            }
            if (iOpcode < uOpcodes.Length)
            {
                for (; iOpcode < uOpcodes.Length; ++iOpcode)
                {
                    ushort uInstr = uOpcodes[iOpcode];
                    var reg = PostCallRegisterStore(uInstr);
                    if (reg != null)
                    {
                        if (ssig.ReturnValue == null)
                            throw new InvalidOperationException(string.Format(
                                "Service {0} is specified as returning void, but returns value in {1}.", name, reg.Name));
                        ssig.ReturnValue.Kind = new Register_v1 { Name = reg.Name };
                        continue;
                    }
                    throw new NotImplementedException(string.Format("uInstr: {0:X4}", uInstr));
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

        private RegisterStorage PushRegister(ushort uInstr)
        {
            if ((uInstr & 0xFFF8) == 0x2F00)
            {
                return Registers.DataRegister(uInstr & 7);
            }
            return null;
        }

        private SerializedRegValue InlineConstant(ushort uInstr, ushort[] uOpcodes, ref int i)
        {
            RegisterStorage reg;
            switch (uInstr & 0xF1FF)
            {
            case 0x203C:
                var n4 = (uOpcodes[i + 1] << 16 | uOpcodes[i + 2]).ToString("X8");
                i += 2;
                reg = Registers.DataRegister((uInstr >> 9) & 7);
                return new SerializedRegValue(reg.Name, n4);
            case 0x303C:
                var n2 = uOpcodes[i + 1].ToString("X4");
                i += 1;
                reg = Registers.DataRegister((uInstr >> 9) & 7);
                return new SerializedRegValue(reg.Name, n2);
            default:
                return null;
            }
        }

        private Tuple<int, string> PushConstant(ushort uInstr, ushort[] uOpcodes, ref int i)
        {
            switch (uInstr)
            {
            case 0x2F3C:
                i += 2; 
                return Tuple.Create(4, (uOpcodes[i - 1] << 16 | uOpcodes[i]).ToString("X8"));
            case 0x3F3C:
                i += 1;
                return Tuple.Create(2, (uOpcodes[i]).ToString("X4"));
            case 0x4267:
                return Tuple.Create(2, "0000");
            case 0x42A7:
                return Tuple.Create(4, "00000000");
            default:
                return null;
            }
        }

        private RegisterStorage PostCallRegisterStore(ushort uInstr)
        {
            var uMasked = uInstr & 0xFFF8;
            switch (uMasked)
            {
            case 0x1E80:    // move.b (sp),dX
            case 0x2E80:    // move.w (sp),dX
            case 0x3E80:    // move.l (sp),dX
                return Registers.DataRegister(uInstr & 7);
            case 0x2E88:    // movea  (sp),aX
                return Registers.AddressRegister(uInstr & 7);
            default:
                return null;
            }
        }

        private SerializedRegValue QuickConstant(ushort uInstr)
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

        private RegisterStorage PopRegister(ushort uInstr)
        {
            switch (uInstr & 0xF1FF)
            {
            case 0x205F:
                // movea $(sp)+,aX
                return Registers.AddressRegister((uInstr >> 9) & 7);
            case 0x201F:
            case 0x301F:
                // move $(sp)+,dX
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
            Id id = exp as Id;
            if (id != null)
            {
                var n = constants[id.Name];
                return (ushort)n.ToInt64();
            }
            var num = exp as NumericLiteral;
            if (num != null)
            {
                return (ushort)num.Value;
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
