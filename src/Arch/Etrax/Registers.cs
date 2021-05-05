#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Types;
using System.Linq;

namespace Reko.Arch.Etrax
{
    public static class Registers
    {
        public static RegisterStorage sp { get; }
        public static RegisterStorage pc { get; }
        public static RegisterStorage [] GpRegisters { get; }
        public static RegisterStorage [] SystemRegisters { get; }

        public static RegisterStorage p0   { get; }
        public static RegisterStorage vr   { get; }
        public static RegisterStorage p2   { get; }
        public static RegisterStorage p3   { get; }
        public static RegisterStorage p4   { get; }
        public static RegisterStorage ccr  { get; }
        public static RegisterStorage p6   { get; }
        public static RegisterStorage mof  { get; }
        public static RegisterStorage p8   { get; }
        public static RegisterStorage ibr  { get; }
        public static RegisterStorage irp  { get; }
        public static RegisterStorage srp  { get; }
        public static RegisterStorage bar  { get; }
        public static RegisterStorage dccr  { get; }
        public static RegisterStorage brp  { get; }
        public static RegisterStorage usp { get; }

        public static FlagGroupStorage F { get; }
        public static FlagGroupStorage P { get; }
        public static FlagGroupStorage U { get; }
        public static FlagGroupStorage M { get; }
        public static FlagGroupStorage B { get; }
        public static FlagGroupStorage I { get; }
        public static FlagGroupStorage X { get; }
        public static FlagGroupStorage N { get; }
        public static FlagGroupStorage Z { get; }
        public static FlagGroupStorage V { get; }
        public static FlagGroupStorage C { get; }

        public static FlagGroupStorage NZ { get; }
        public static FlagGroupStorage NZVC { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            var gpregs = factory.RangeOfReg32(14, "r{0}");
            sp = factory.Reg32("sp");
            pc = factory.Reg32("pc");

            GpRegisters = gpregs.Concat(new[] { sp, pc }).ToArray();

            var spfactory = new StorageFactory(StorageDomain.SystemRegister);
            p0 = spfactory.Reg("p0", PrimitiveType.Byte);
            vr = spfactory.Reg("vr", PrimitiveType.Byte);
            p2 = spfactory.Reg("p2", PrimitiveType.Byte);
            p3 = spfactory.Reg("p3", PrimitiveType.Byte);
            
            p4 = spfactory.Reg("p4", PrimitiveType.Word16);
            ccr = spfactory.Reg("ccr", PrimitiveType.Word16);
            p6 = spfactory.Reg("p6", PrimitiveType.Word16);
            mof = spfactory.Reg("mof", PrimitiveType.Word32);

            p8 = spfactory.Reg("p8", PrimitiveType.Word32);
            ibr = spfactory.Reg("ibr", PrimitiveType.Word32);
            irp = spfactory.Reg("irp", PrimitiveType.Word32);
            srp = spfactory.Reg("srp", PrimitiveType.Word32);

            bar = spfactory.Reg("bar", PrimitiveType.Word32);
            dccr = spfactory.Reg("dccr", PrimitiveType.Word32);
            brp = spfactory.Reg("brp", PrimitiveType.Word32);
            usp = spfactory.Reg("usp", PrimitiveType.Word32);

            SystemRegisters = new RegisterStorage[16]
            {
                p0 ,
                vr ,
                p2 ,
                p3 ,

                p4 ,
                ccr,
                p6 ,
                mof,

                p8 ,
                ibr,
                irp,
                srp,

                bar,
                dccr,
                brp,
                usp,
            };

            F =  new FlagGroupStorage(dccr, (uint)FlagM.FF, "F", PrimitiveType.Bool);
            P =  new FlagGroupStorage(dccr, (uint)FlagM.PF, "P", PrimitiveType.Bool);
            U =  new FlagGroupStorage(dccr, (uint)FlagM.UF, "U", PrimitiveType.Bool);
            M =  new FlagGroupStorage(dccr, (uint)FlagM.MF, "M", PrimitiveType.Bool);
            B =  new FlagGroupStorage(dccr, (uint)FlagM.BF, "B", PrimitiveType.Bool);
            I =  new FlagGroupStorage(dccr, (uint)FlagM.IF, "I", PrimitiveType.Bool);
            X =  new FlagGroupStorage(dccr, (uint)FlagM.XF, "X", PrimitiveType.Bool);
            N =  new FlagGroupStorage(dccr, (uint)FlagM.NF, "N", PrimitiveType.Bool);
            Z =  new FlagGroupStorage(dccr, (uint)FlagM.ZF, "Z", PrimitiveType.Bool);
            V =  new FlagGroupStorage(dccr, (uint)FlagM.VF, "V", PrimitiveType.Bool);
            C =  new FlagGroupStorage(dccr, (uint)FlagM.CF, "C", PrimitiveType.Bool);

            NZ = new FlagGroupStorage(dccr, (uint) (FlagM.NF|FlagM.ZF), "NZ", dccr.DataType);
            NZVC = new FlagGroupStorage(dccr, (uint) (FlagM.NF|FlagM.ZF|FlagM.VF|FlagM.CF), "NZVC", dccr.DataType);
        }
    }

    public enum FlagM : uint
    {
        FF = 1 << 10,
        PF = 1 << 9,
        UF = 1 << 8,
        MF = 1 << 7,
        BF = 1 << 6,
        IF = 1 << 5,
        XF = 1 << 4,
        NF = 1 << 3,
        ZF = 1 << 2,
        VF = 1 << 1,
        CF = 1 << 0,
    }
}