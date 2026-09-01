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
 * You should have received a copy of the GNU General Public License,
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;

namespace Reko.Environments.PlayStation.Ps2;

/// <summary>
/// Implements the custom calling convention used by the Sony PlayStation 2 
/// Emotion Engine (MIPS R5900 core) compiler toolchain.
/// </summary>
public class Ps2EmotionEngineCallingConvention : AbstractCallingConvention
{
    private readonly RegisterStorage[] argGprs;
    private readonly RegisterStorage[] returnGprs;

    public Ps2EmotionEngineCallingConvention(IProcessorArchitecture arch)
        : base("ps2ee")
    {
        this.Architecture = arch;

        // PS2 EE uses $a0-$a3, then $t0-$t3 for the first 8 arguments
        this.argGprs = new[]
        {
            arch.GetRegister("r4")!,
            arch.GetRegister("r5")!,
            arch.GetRegister("r6")!,
            arch.GetRegister("r7")!,
            arch.GetRegister("r8")!,
            arch.GetRegister("r9")!,
            arch.GetRegister("r10")!,
            arch.GetRegister("r11")!
        };

        // Return values are passed back in $v0 and $v1
        this.returnGprs = new[]
        {
            arch.GetRegister("r2")!,
            arch.GetRegister("r3")!
        };
    }

    public IProcessorArchitecture Architecture { get; }

    /// <summary>
    /// Allocates storage for parameters based on the PS2 EE convention.
    /// </summary>
    public override void Generate(ICallingConventionBuilder ccr, int retAddressOnStack, DataType? dtRet, DataType? dtThis, List<DataType> dtParams)
    {
        ccr.LowLevelDetails(4, 16);
        int gprIndex = 0;
        foreach (var dtParam in dtParams)
        {
            int sizeInBytes = (int) dtParam.Size;

            if (gprIndex < argGprs.Length)
            {
                // Use parameter register if available ($a0-$a3, then $t0-$t3)
                ccr.RegParam(argGprs[gprIndex++]);
            }
            else
            {
                // Spill over to the stack
                ccr.StackParam(dtParam);
            }
        }
    }

    public override bool IsArgument(Storage stg)
    {
        if (stg is RegisterStorage reg)
        {
            return Array.IndexOf(argGprs, reg) >= 0;
        }
        if (stg is StackStorage stk)
            return stk.StackOffset > 0; //$TODO: check
        return false;
    }

    /// <summary>
    /// Checks if a given register must be preserved by the called function.
    /// </summary>
    public bool IsCalleeSaved(RegisterStorage reg)
    {
        string name = reg.Name.ToLowerInvariant();

        // Callee-saved: $s0-$s7, $gp, $sp, $fp ($s8)
        if (name.StartsWith("s") || name == "gp" || name == "sp" || name == "fp")
        {
            return true;
        }
        return false;
    }

    public override bool IsOutArgument(Storage stg)
    {
        return Array.IndexOf(this.returnGprs, stg) >= 0;
    }
}
