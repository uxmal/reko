#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Hll.Pascal;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using static System.Formats.Asn1.AsnWriter;

namespace Reko.Arch.X86
{
    /// <summary>
    /// NEC V20/V30 register aliases.
    /// </summary>
    public static class V20Registers
    {
        // General-purpose register aliases (V20 names)
        public static readonly RegisterStorage aw;
        public static readonly RegisterStorage cw;
        public static readonly RegisterStorage dw;
        public static readonly RegisterStorage bw;
        public static readonly RegisterStorage ix;
        public static readonly RegisterStorage iy;

        // Segment register aliases (V20 names)
        public static readonly RegisterStorage ds1;   // ES → DS1
        public static readonly RegisterStorage ps;    // CS → PS
        public static readonly RegisterStorage ds0;   // DS → DS0

        // Flags register alias (V20 name)
        public static readonly RegisterStorage psw;   // EFLAGS → PSW

        internal static readonly RegisterStorage[] Gp64BitRegisters;
        internal static readonly RegisterStorage[] Gp32BitRegisters;
        internal static readonly RegisterStorage[] Gp16BitRegisters;
        internal static readonly RegisterStorage[] Gp8BitRegisters;
        internal static readonly RegisterStorage[] Gp8BitRegisters_Rex;

        public static FlagGroupStorage C { get; }
        public static FlagGroupStorage CZ { get; }
        public static FlagGroupStorage CZP { get; }
        public static FlagGroupStorage SCZ { get; }
        public static FlagGroupStorage S { get; }
        public static FlagGroupStorage Z { get; }
        public static FlagGroupStorage D { get; }
        public static FlagGroupStorage O { get; }
        public static FlagGroupStorage P { get; }
        public static FlagGroupStorage CO { get; }
        public static FlagGroupStorage SCZO { get; }
        public static FlagGroupStorage SCZOP { get; }
        public static FlagGroupStorage SZ { get; }
        public static FlagGroupStorage SZO { get; }
        public static FlagGroupStorage SZP { get; }
        public static FlagGroupStorage SO { get; }
        public static FlagGroupStorage[] EflagsBits { get; }


        public static readonly RegisterStorage[] All;
        public static readonly Dictionary<StorageDomain, RegisterStorage[]> DomainToRegister;
        public static readonly Dictionary<string, RegisterStorage> NameToRegister;

        public static Dictionary<StorageDomain, RegisterStorage[]> SubRegisters { get; }

        static V20Registers()
        {
            aw = Alias("aw", Registers.ax);
            cw = Alias("cw", Registers.cx);
            dw = Alias("dw", Registers.dx);
            bw = Alias("bw", Registers.bx);
            ix = Alias("ix", Registers.si);
            iy = Alias("iy", Registers.di);

            // Segment register aliases
            ds1 = Alias("ds1", Registers.es);
            ps = Alias("ps", Registers.cs);
            ds0 = Alias("ds0", Registers.ds);

            // Flags alias
            psw = Alias("psw", Registers.eflags);

            Gp64BitRegisters = Registers.Gp64BitRegisters;
            Gp32BitRegisters = Registers.Gp32BitRegisters;
            Gp16BitRegisters =
            [
                aw, cw, dw, bw,
                Registers.sp, Registers.bp, ix, iy,
                Registers.r8w, Registers.r9w, Registers.r10w, Registers.r11w,
                Registers.r12w, Registers.r13w, Registers.r14w, Registers.r15w,
                Registers.r16w, Registers.r17w, Registers.r18w, Registers.r19w,
                Registers.r20w, Registers.r21w, Registers.r22w, Registers.r23w,
                Registers.r24w, Registers.r25w, Registers.r26w, Registers.r27w,
                Registers.r28w, Registers.r29w, Registers.r30w, Registers.r31w,
            ];
            Gp8BitRegisters = Registers.Gp8BitRegisters;
            Gp8BitRegisters_Rex = Registers.Gp8BitRegisters_Rex;

            All =
            [
                .. Registers.All,
                aw,
                cw,
                dw,
                bw,
                ix,
                iy,
                ds1,
                ps,
                ds0,
                psw,
            ];

            C = FlagAlias(Registers.C);
            CZ = FlagAlias(Registers.CZ);
            CZP = FlagAlias(Registers.CZP);
            SCZ = FlagAlias(Registers.SCZ);
            S = FlagAlias(Registers.S);
            Z = FlagAlias(Registers.Z);
            D = FlagAlias(Registers.D);
            O = FlagAlias(Registers.O);
            P = FlagAlias(Registers.P);
            CO = FlagAlias(Registers.CO);
            SCZO = FlagAlias(Registers.SCZO);
            SCZOP = FlagAlias(Registers.SCZOP);
            SZ = FlagAlias(Registers.SZ);
            SZO = FlagAlias(Registers.SZO);
            SZP = FlagAlias(Registers.SZP);
            SO = FlagAlias(Registers.SO);
            EflagsBits = [ S, C, Z, D, O, P ];

            DomainToRegister = new Dictionary<StorageDomain, RegisterStorage[]>
            {
                { Registers.rax.Domain, [ Registers.rax, Registers.eax, aw, Registers.al, Registers.ah ] },
                { Registers.rcx.Domain, [ Registers.rcx, Registers.ecx, cw, Registers.cl, Registers.ch ] },
                { Registers.rdx.Domain, [ Registers.rdx, Registers.edx, dw, Registers.dl, Registers.dh ] },
                { Registers.rbx.Domain, [ Registers.rbx, Registers.ebx, bw, Registers.bl, Registers.bh ] },
                { Registers.rsp.Domain, [ Registers.rsp, Registers.esp, Registers.sp, Registers.spl ] },
                { Registers.rbp.Domain, [ Registers.rbp, Registers.ebp, Registers.bp, Registers.bpl ] },
                { Registers.rsi.Domain, [ Registers.rsi, Registers.esi, ix, Registers.sil ] },
                { Registers.rdi.Domain, [ Registers.rdi, Registers.edi, iy, Registers.dil ] },
            };



            NameToRegister = new Dictionary<string, RegisterStorage>(StringComparer.OrdinalIgnoreCase);
            AddRegisters(NameToRegister, Registers.All);
            AddRegisters(NameToRegister, new[] { aw, cw, dw, bw, ix, iy, ds1, ps, ds0, psw });

            SubRegisters = new Dictionary<StorageDomain, RegisterStorage[]>
            {
                { aw.Domain, new [] { aw, Registers.al, Registers.ah, } },
                { cw.Domain, new [] { cw, Registers.cl, Registers.ch, } },
                { dw.Domain, new [] { dw, Registers.dl, Registers.dh, } },
                { bw.Domain, new [] { bw, Registers.bl, Registers.bh, } },
                { Registers.sp.Domain, new [] { Registers.sp, } },
                { Registers.bp.Domain, new [] { Registers.bp, } },
                { ix.Domain, new [] { ix, } },
                { iy.Domain, new [] { iy, } },
            };
        }

        private static void AddRegisters(Dictionary<string, RegisterStorage> map, IEnumerable<RegisterStorage> regs)
        {
            foreach (var reg in regs)
            {
                if (reg is not null)
                {
                    map[reg.Name] = reg;
                }
            }
        }

        private static RegisterStorage Alias(string name, RegisterStorage reg)
        {
            return new RegisterStorage(name, reg.Number, (uint) reg.BitAddress, (PrimitiveType) reg.DataType);
        }

        public static FlagGroupStorage FlagAlias(FlagGroupStorage grf)
        {
            return new FlagGroupStorage(psw, grf.FlagGroupBits, grf.Name);
        }
    }
}
