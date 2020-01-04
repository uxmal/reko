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

using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.PaRisc
{
    public class ConditionOperand : MachineOperand
    {
        public readonly ConditionType Type;
        public readonly string Display;

        public ConditionOperand(ConditionType ct, string display) : base(PrimitiveType.Byte)
        {
            this.Type = ct;
            this.Display = display;
        }

        public override void Write(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            writer.WriteString(Display);
        }

        public static readonly ConditionOperand Never = new ConditionOperand(ConditionType.Never, "");
        public static readonly ConditionOperand Eq = new ConditionOperand(ConditionType.Eq, "=");
        public static readonly ConditionOperand Lt = new ConditionOperand(ConditionType.Lt, "<");
        public static readonly ConditionOperand Le = new ConditionOperand(ConditionType.Le, "<=");

        public static readonly ConditionOperand Nuv = new ConditionOperand(ConditionType.Nuv, "nuv");
        public static readonly ConditionOperand Znv = new ConditionOperand(ConditionType.Znv, "znv");
        public static readonly ConditionOperand Sv = new ConditionOperand(ConditionType.Sv, "sv");
        public static readonly ConditionOperand Odd = new ConditionOperand(ConditionType.Odd, "od");

        public static readonly ConditionOperand Tr = new ConditionOperand(ConditionType.Tr, "tr");
        public static readonly ConditionOperand Ne = new ConditionOperand(ConditionType.Ne, "<>");
        public static readonly ConditionOperand Ge = new ConditionOperand(ConditionType.Ge, ">=");
        public static readonly ConditionOperand Gt = new ConditionOperand(ConditionType.Gt, ">");

        public static readonly ConditionOperand Uv = new ConditionOperand(ConditionType.Uv, "uv");
        public static readonly ConditionOperand Vnz = new ConditionOperand(ConditionType.Vnz, "vnz");
        public static readonly ConditionOperand Nsv = new ConditionOperand(ConditionType.Nsv, "nsv");
        public static readonly ConditionOperand Even = new ConditionOperand(ConditionType.Even, "ev");

        public static readonly ConditionOperand Ult = new ConditionOperand(ConditionType.Ult, "<<");
        public static readonly ConditionOperand Ule = new ConditionOperand(ConditionType.Ule, "<<=");
        public static readonly ConditionOperand Uge = new ConditionOperand(ConditionType.Uge, ">>=");
        public static readonly ConditionOperand Ugt = new ConditionOperand(ConditionType.Ugt, ">>");

        public static readonly ConditionOperand Never64 = new ConditionOperand(ConditionType.Never, "");
        public static readonly ConditionOperand Eq64 = new ConditionOperand(ConditionType.Eq64, "*=");
        public static readonly ConditionOperand Lt64 = new ConditionOperand(ConditionType.Lt64, "*<");
        public static readonly ConditionOperand Le64 = new ConditionOperand(ConditionType.Le64, "*<=");

        public static readonly ConditionOperand Nuv64 = new ConditionOperand(ConditionType.Nuv64, "*nuv");
        public static readonly ConditionOperand Znv64 = new ConditionOperand(ConditionType.Znv64, "*znv");
        public static readonly ConditionOperand Sv64 = new ConditionOperand(ConditionType.Sv64, "*sv");
        public static readonly ConditionOperand Odd64 = new ConditionOperand(ConditionType.Odd64, "*od");

        public static readonly ConditionOperand Tr64 = new ConditionOperand(ConditionType.Tr, "*tr");
        public static readonly ConditionOperand Ne64 = new ConditionOperand(ConditionType.Ne64, "*<>");
        public static readonly ConditionOperand Ge64 = new ConditionOperand(ConditionType.Ge64, "*>=");
        public static readonly ConditionOperand Gt64 = new ConditionOperand(ConditionType.Gt64, "*>");

        public static readonly ConditionOperand Uv64 = new ConditionOperand(ConditionType.Uv64, "*uv");
        public static readonly ConditionOperand Vnz64 = new ConditionOperand(ConditionType.Vnz64, "*vnz");
        public static readonly ConditionOperand Nsv64 = new ConditionOperand(ConditionType.Nsv64, "*nsv");
        public static readonly ConditionOperand Even64 = new ConditionOperand(ConditionType.Even64, "*ev");

        public static readonly ConditionOperand Ult64 = new ConditionOperand(ConditionType.Ult64, "*<<");
        public static readonly ConditionOperand Ule64 = new ConditionOperand(ConditionType.Ule64, "*<<=");
        public static readonly ConditionOperand Uge64 = new ConditionOperand(ConditionType.Uge64, "*>>=");
        public static readonly ConditionOperand Ugt64 = new ConditionOperand(ConditionType.Ugt64, "*>>");

    }
}
