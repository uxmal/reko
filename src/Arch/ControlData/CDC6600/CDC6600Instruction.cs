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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;

namespace Reko.Arch.ControlData.CDC6600;

public class CDC6600Instruction : MachineInstruction
{
    public Mnemonic Mnemonic { get; set; }
    public override int MnemonicAsInteger => (int) Mnemonic;

    public override string MnemonicAsString => Mnemonic.ToString();

    public bool FuseMnemonic { get; set; }

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        var info = infos[Mnemonic];
        var sMnemonic = info.Mnemonic;
        int iop;
        if (sMnemonic is not null)
        {
            sMnemonic += Operands[0];
            iop = 1;
        }
        else
        {
            sMnemonic = this.Mnemonic.ToString();
            iop = 0;
        }
        renderer.WriteMnemonic(sMnemonic);

        if (iop >= Operands.Length)
            return;
        renderer.WriteChar(' ');
        if (info.Prefix is not null)
        {
            renderer.WriteString(info.Prefix);
        }
        RenderOperand(Operands[iop++], renderer, options);
        if (iop >= Operands.Length)
            return;
        renderer.WriteString(info.Infix ?? " ");
        RenderOperand(Operands[iop++], renderer, options);
        if (iop >= Operands.Length)
            return;
        renderer.WriteChar(' ');
        RenderOperand(Operands[iop++], renderer, options);
    }

    private int RenderMnemonic(MachineInstructionRenderer renderer)
    {
        int cSkip = 0;
        string sMnemonic = this.Mnemonic.ToString();
        if (FuseMnemonic)
        {
            sMnemonic += Operands[0].ToString();
            cSkip = 1;
        }
        renderer.WriteMnemonic(sMnemonic);
        return cSkip;
    }

    protected override void RenderOperand(MachineOperand operand, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        if (operand is Constant c)
        {
            renderer.WriteString(Convert.ToString(c.ToInt64(), 8));
            return;
        }
        base.RenderOperand(operand, renderer, options);
    }

    private static readonly Dictionary<Mnemonic, Info> infos = new ()
    {
        { Mnemonic.Invalid, new Info(null, null, null) },
        { Mnemonic.DF, new Info(null, null, null) },
        { Mnemonic.EQ, new Info(null, null, null) },
        { Mnemonic.GE, new Info(null, null, null) },
        { Mnemonic.ID, new Info(null, null, null) },
        { Mnemonic.IR, new Info(null, null, null) },
        { Mnemonic.JP, new Info(null, null, " + ") },
        { Mnemonic.NE, new Info(null, null, null) },
        { Mnemonic.NZ, new Info(null, null, null) },
        { Mnemonic.NG, new Info(null, null, null) },
        { Mnemonic.NO, new Info(null, null, null) },
        { Mnemonic.OR, new Info(null, null, null) },
        { Mnemonic.PL, new Info(null, null, null) },
        { Mnemonic.PS, new Info(null, null, null) },
        { Mnemonic.REC, new Info(null, null, " + ") },
        { Mnemonic.RJ, new Info(null, null, null) },
        { Mnemonic.WEC, new Info(null, null, " + ") },
        { Mnemonic.ZR, new Info(null, null, null) },
        { Mnemonic.and, new Info("B", null, " * ") },
        { Mnemonic.andn, new Info("B", "-", " * ") },
        { Mnemonic.asr, new Info("A", null, null) },
        { Mnemonic.asrn, new Info("A", null, null) },
        { Mnemonic.dadd, new Info("D", null, " + ") },
        { Mnemonic.dsub, new Info("D", null, " - ") },
        { Mnemonic.dmul, new Info("D", null, " * ") },
        { Mnemonic.fadd, new Info("F", null, " + ") },
        { Mnemonic.fadd_round, new Info("R", null, " + ") },
        { Mnemonic.fdiv, new Info("F", null, " / ") },
        { Mnemonic.fdiv_round, new Info("R", null, " / ") },
        { Mnemonic.fmul, new Info("F", null, " * ") },
        { Mnemonic.fmul_round, new Info("R", null, " * ") },
        { Mnemonic.fsub, new Info("F", null, " - ") },
        { Mnemonic.fsub_round, new Info("R", null, " - ") },
        { Mnemonic.iadd, new Info("I", null, " + ") },
        { Mnemonic.isub, new Info("I", null, " - ") },
        { Mnemonic.lsl, new Info("L", null, null) },
        { Mnemonic.lsln, new Info("L", null, null) },
        { Mnemonic.mask, new Info("M", null, null) },
        { Mnemonic.mov, new Info("B", null, null) },
        { Mnemonic.neg, new Info("B", "-", null) },
        { Mnemonic.normalize, new Info("N", null, null) },
        { Mnemonic.or, new Info("B", null, " + ") },
        { Mnemonic.orn, new Info("B", "-", " + ") },
        { Mnemonic.pack, new Info("P", null, null) },
        { Mnemonic.popcnt, new Info("C", null, null) },
        { Mnemonic.round_normalize, new Info("Z", null, null) },
        { Mnemonic.sadd, new Info("S", null, " + ") },
        { Mnemonic.ssub, new Info("S", null, " - ") },
        { Mnemonic.unpack, new Info("U", null, null) },
        { Mnemonic.xor, new Info("B", null, " - ") },
        { Mnemonic.xorn, new Info("B", "-", " - ") },
    };

    private class Info
    {
        public Info(string? mnemonic, string? prefix, string? infix)
        {
            this.Mnemonic = mnemonic;
            this.Prefix = prefix;
            this.Infix = infix;
        }

        public string? Mnemonic { get; }
        public string? Prefix { get; }
        public string? Infix { get; }
    }
}
