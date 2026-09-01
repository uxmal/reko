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

using Reko.Core.Machine;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Arch.Mips.Machine;

public class MipsInstruction : MachineInstruction
{
    public Mnemonic Mnemonic;

    public override int MnemonicAsInteger => (int)Mnemonic;

    public override string MnemonicAsString => Mnemonic.ToString();

    public int LaneMask { get; set; }

    protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
    {
        WriteMnemonic(renderer);
        RenderOperands(renderer, options);
    }

    private void WriteMnemonic(MachineInstructionRenderer renderer)
    {
        var name = this.Mnemonic.ToString().Replace('_', '.');
        // Some instructions encode a lane mask or subtype selector in
        // their last operand; we render it as a suffix of the mnemonic
        // instead, which matches the usual PS2 assembly syntax.
        if (this.TryGetSuffix(out string? suffix) && !string.IsNullOrEmpty(suffix))
        {
            name += $".{suffix}";
        }
        renderer.WriteMnemonic(name);
    }


    private bool TryGetSuffix([MaybeNullWhen(false)] out string? suffix)
    {
        suffix = null;
        if (this.LaneMask == 0)
            return false;
        int i = this.LaneMask;
        switch (this.Mnemonic)
        {
        case Mnemonic.vmtir:
        case Mnemonic.vsqrt:
            suffix = i switch
            {
                0 => "x",
                1 => "y",
                2 => "z",
                3 => "w",
                _ => null,
            };
            break;
        default:
            if (CarriesDestMask(this.Mnemonic))
            {
                var sb = new System.Text.StringBuilder();
                if ((i & 8) != 0) sb.Append('x');
                if ((i & 4) != 0) sb.Append('y');
                if ((i & 2) != 0) sb.Append('z');
                if ((i & 1) != 0) sb.Append('w');
                suffix = sb.ToString();
                return true;    // Empty masks are legal; no dot then.
            }
            return false;
        }
        return suffix is not null;
    }

    /// <summary>
    /// True of the VU0 macro mode instructions whose last operand holds
    /// the x/y/z/w destination lane mask.
    /// </summary>
    private static bool CarriesDestMask(Mnemonic mnemonic)
    {
        switch (mnemonic)
        {
        case Mnemonic.vadd:
        case Mnemonic.vaddi:
        case Mnemonic.vaddx:
        case Mnemonic.vaddy:
        case Mnemonic.vaddz:
        case Mnemonic.vaddw:
        case Mnemonic.vdiv:
        case Mnemonic.vmul:
        case Mnemonic.vsub:
        case Mnemonic.vmfir:
        case Mnemonic.vlqi:
        case Mnemonic.vsqi:
        case Mnemonic.vlqd:
        case Mnemonic.vsqd:
        case Mnemonic.vrnext:
        case Mnemonic.vrget:
        case Mnemonic.vilwr:
        case Mnemonic.viswr:
        case Mnemonic.vabs:
        case Mnemonic.vmini:
        case Mnemonic.vmove:
        case Mnemonic.vmr32:
        case Mnemonic.vadda:
        case Mnemonic.vmadda:
        case Mnemonic.vmula:
        case Mnemonic.vmulq:
        case Mnemonic.vsuba:
        case Mnemonic.vmsuba:
            case Mnemonic.vopmula:
            return true;
        default:
            return mnemonic is (>= Mnemonic.vaddx and <= Mnemonic.vmulw)
                or (>= Mnemonic.vmul_q and <= Mnemonic.vmini)
                or (>= Mnemonic.vaddax and <= Mnemonic.vsubaw)
                or (>= Mnemonic.vmaddax and <= Mnemonic.vmsubaw)
                or (>= Mnemonic.vitof0 and <= Mnemonic.vftoi15)
                or (>= Mnemonic.vmulax and <= Mnemonic.vmsubai);
        }
    }
}
