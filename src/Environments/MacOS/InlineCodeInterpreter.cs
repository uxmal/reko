#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

namespace Reko.Environments.MacOS
{
    public class InlineCodeInterpreter
    {
        private IDictionary<string, Constant> constants;

        public InlineCodeInterpreter(IDictionary<string, Constant> constants)
        {
            this.constants = constants;
        }

        public SerializedService BuildSystemCallFromMachineCode(
            string name,
            SerializedSignature ssig,
            List<Exp> opcodes)
        {
            var uOpcodes = opcodes.Select(EvaluateOpcode).ToArray();

            if (uOpcodes.Length == 1 && IsALineTrap(uOpcodes[0]))
            {
                return PascalSystemCall(name, ssig, uOpcodes[0]);
            }
            int iArg = 0;
            int vector = 0;
            int i;
            var regValues = new List<SerializedRegValue>();
            for (i = 0; i < uOpcodes.Length; ++i)
            {
                ushort uInstr = uOpcodes[i];
                var reg = PopRegister(uInstr);
                if (reg != null)
                {
                    ssig.Arguments[iArg].Kind = new Register_v1(reg.Name);
                    ++iArg;
                }
                if (IsALineTrap(uInstr))
                {
                    vector = uInstr;
                }
                var regValue = RegisterConstant(uInstr);
                if (regValue != null)
                {
                    regValues.Add(regValue);
                }
            }
            return new SerializedService
            {
                Ordinal = vector,
                Name = name,
                Signature = ssig,
            };
        }

        private SerializedRegValue RegisterConstant(ushort uInstr)
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
            if ((uInstr & 0xF1FF) == 0x205F)
            {
                // movea $(sp)+,aX
                return Registers.AddressRegister((uInstr >> 9) & 7);
            }
            return null;
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
