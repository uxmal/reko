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
    public FlagGroupStorage SCZDOP { get; internal set; } = Registers.SCZDOP;
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