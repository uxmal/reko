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
using System.Collections.Generic;

namespace Reko.Arch.X86;

public class RegisterBank
{
    public static RegisterBank IntelInstance = new RegisterBank
    {
     ax = Registers.ax,
     cx = Registers.cx,
     dx = Registers.dx,
     bx = Registers.bx,
     sp = Registers.sp,
     bp = Registers.bp,
     si = Registers.si,
     di = Registers.di,
     ip = Registers.ip,
        
     cs = Registers.cs,
     ds = Registers.ds,
     es = Registers.es,
     ss = Registers.ss,


    Gp16BitRegisters = Registers.Gp16BitRegisters,
    SegmentRegisters =
    [
        Registers.es,
        Registers.cs,
        Registers.ss,
        Registers.ds,
        Registers.fs,
        Registers.gs,
        Registers.ss,
        Registers.ds,
    ],


    SubRegisters = Registers.SubRegisters,
    All = Registers.All,
    /*
    public FlagGroupStorage[] EflagsBits { get; internal set; } = Registers.EflagsBits;
    public RegisterStorage eflags { get; internal set; } = Registers.eflags;
    public IEnumerable<FlagGroupStorage> FpuFlagsBits { get; internal set; } = Registers.FpuFlagsBits;
    public FlagGroupStorage C { get; internal set; } = Registers.C;
    public FlagGroupStorage CZ { get; internal set; } = Registers.CZ;
    public FlagGroupStorage CZP { get; internal set; } = Registers.CZP;
    public FlagGroupStorage SCZ { get; internal set; } = Registers.SCZ;
    public RegisterStorage FPUF { get; internal set; } = Registers.FPUF;
    public FlagGroupStorage S { get; internal set; } = Registers.S;
    public FlagGroupStorage Z { get; internal set; } = Registers.Z;
    public FlagGroupStorage D { get; internal set; } = Registers.D;
    public FlagGroupStorage O { get; internal set; } = Registers.O;
    public FlagGroupStorage P { get; internal set; } = Registers.P;
    public FlagGroupStorage CO { get; internal set; } = Registers.CO;
    public FlagGroupStorage SCZO { get; internal set; } = Registers.SCZO;
    public FlagGroupStorage SCZOP { get; internal set; } = Registers.SCZOP;
    public FlagGroupStorage SZ { get; internal set; } = Registers.SZ;
    public FlagGroupStorage SZO { get; internal set; } = Registers.SZO;
    public FlagGroupStorage SZP { get; internal set; } = Registers.SZP;
    public FlagGroupStorage SO { get; internal set; } = Registers.SO;
    */
        Max = Registers.Max,
        C2 = Registers.C2
    };

    public static RegisterBank V20Instance = new RegisterBank
    {
        ax = V20Registers.aw,
        cx = V20Registers.cw,
        dx = V20Registers.dw,
        bx = V20Registers.bw,
        sp = Registers.sp,
        bp = Registers.bp,
        si = V20Registers.ix,
        di = V20Registers.iy,
        ip = Registers.ip,

        cs = V20Registers.ps,
        ds = V20Registers.ds0,
        es = V20Registers.ds1,
        ss = Registers.ss,


        Gp16BitRegisters = V20Registers.Gp16BitRegisters,
        SegmentRegisters =
        [
            V20Registers.ds1,
            V20Registers.ps,
            Registers.ss,
            V20Registers.ds0,
            Registers.fs,
            Registers.gs,
            Registers.ss,
            V20Registers.ds0,
        ],

        SubRegisters = V20Registers.SubRegisters,
        All = Registers.All,
        EflagsBits = V20Registers.EflagsBits,
        Eflags = V20Registers.psw,
        FpuFlagsBits = Registers.FpuFlagsBits,

        C = V20Registers.C,
        CZ = V20Registers.CZ,
        CZP = V20Registers.CZP,
        SCZ = V20Registers.SCZ,
        FPUF  = Registers.FPUF,
        S = V20Registers.S,
        Z = V20Registers.Z,
        D = V20Registers.D,
        O = V20Registers.O,
        P = V20Registers.P,
        CO =V20Registers.CO,
        SCZO = V20Registers.SCZO,
        SCZOP = V20Registers.SCZOP,
        SZ = V20Registers.SZ,
        SZO = V20Registers.SZO,
        SZP = V20Registers.SZP,
        SO  = V20Registers.SO,
        Max = V20Registers.All.Length,
        C2 = Registers.C2,
    };

    public RegisterStorage ax { get; internal set; } = Registers.ax;
    public RegisterStorage cx { get; internal set; } = Registers.cx;
    public RegisterStorage dx { get; internal set; } = Registers.dx;
    public RegisterStorage bx { get; internal set; } = Registers.bx;
    public RegisterStorage sp { get; internal set; } = Registers.sp;
    public RegisterStorage bp { get; internal set; } = Registers.bp;
    public RegisterStorage si { get; internal set; } = Registers.si;
    public RegisterStorage di { get; internal set; } = Registers.di;
    public RegisterStorage ip { get; internal set; } = Registers.ip;

    public RegisterStorage cs { get; internal set; } = Registers.cs;
    public RegisterStorage ds { get; internal set; } = Registers.ds;
    public RegisterStorage es { get; internal set; } = Registers.es;
    public RegisterStorage ss { get; internal set; } = Registers.ss;


    public RegisterStorage[] Gp16BitRegisters { get; internal set; } = Registers.Gp16BitRegisters;
    public RegisterStorage[] SegmentRegisters { get; internal set; } =
    [
        Registers.es,
        Registers.cs,
        Registers.ss,
        Registers.ds,
        Registers.fs,
        Registers.gs,
        Registers.ss,
        Registers.ds,
    ];


    public Dictionary<StorageDomain, RegisterStorage[]> SubRegisters { get; internal set; } = Registers.SubRegisters;
    public IEnumerable<RegisterStorage> All { get; internal set; } = Registers.All;
    public FlagGroupStorage[] EflagsBits { get; internal set; } = Registers.EflagsBits;
    public RegisterStorage Eflags { get; internal set; } = Registers.eflags;
    public IEnumerable<FlagGroupStorage> FpuFlagsBits { get; internal set; } = Registers.FpuFlagsBits;
    public FlagGroupStorage C { get; internal set; } = Registers.C;
    public FlagGroupStorage CZ { get; internal set; } = Registers.CZ;
    public FlagGroupStorage CZP { get; internal set; } = Registers.CZP;
    public FlagGroupStorage SCZ { get; internal set; } = Registers.SCZ;
    public RegisterStorage FPUF { get; internal set; } = Registers.FPUF;
    public FlagGroupStorage S { get; internal set; } = Registers.S;
    public FlagGroupStorage Z { get; internal set; } = Registers.Z;
    public FlagGroupStorage D { get; internal set; } = Registers.D;
    public FlagGroupStorage O { get; internal set; } = Registers.O;
    public FlagGroupStorage P { get; internal set; } = Registers.P;
    public FlagGroupStorage CO { get; internal set; } = Registers.CO;
    public FlagGroupStorage SCZO { get; internal set; } = Registers.SCZO;
    public FlagGroupStorage SCZOP { get; internal set; } = Registers.SCZOP;
    public FlagGroupStorage SZ { get; internal set; } = Registers.SZ;
    public FlagGroupStorage SZO { get; internal set; } = Registers.SZO;
    public FlagGroupStorage SZP { get; internal set; } = Registers.SZP;
    public FlagGroupStorage SO { get; internal set; } = Registers.SO;
    public int Max { get; internal set; } = Registers.Max;
    public FlagGroupStorage C2 { get; internal set; } = Registers.C2;

    public RegisterStorage GetRegister(string s)
    {
        return Registers.GetRegister(s);
    }

}