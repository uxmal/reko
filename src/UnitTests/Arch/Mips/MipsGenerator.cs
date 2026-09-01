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

namespace Reko.UnitTests.Arch.Mips;

public static class MipsGenerator
{
    /// <summary>
    /// Builds an instruction word out of its fields.
    /// </summary>
    public static uint W(int opcode, int rs = 0, int rt = 0, int rd = 0, int sa = 0, int funct = 0, int rest = 0)
    {
        if (rest != 0)
        {
            // For immediate format instructions
            return (uint) ((opcode << 26) | (rs << 21) | (rt << 16) | (rest & 0xFFFF));
        }
        // R-format: opcode(6) | rs(5) | rt(5) | rd(5) | sa(5) | funct(6)
        return (uint) ((opcode << 26) | (rs << 21) | (rt << 16) | (rd << 11) | (sa << 6) | funct);
    }


    /// <summary>
    /// Builds FPU instruction word out of its fields.
    /// </summary>
    public static uint FpuW(int fmt, int ft = 0, int fs = 0, int fd = 0, int funct = 0, int ccOrRm = 0)
    {
        return (uint) ((0x11 << 26) | (fmt << 21) | (ft << 16) | (fs << 11) | (fd << 6) | funct | (ccOrRm << 8));
    }

    public static uint MmiW(int rs, int rt, int rd, int sa, int funct)
    {
        return W(0x1C, rs: rs, rt: rt, rd: rd, sa: sa, funct: funct);
    }


    // Helper method for COP2 instructions
    public static uint Cop2W(int rs, int rt = 0, int rd = 0, int sa = 0, int funct = 0, int rest = 0)
    {
        const int COP2_OPCODE = 0x12;
        if (rest != 0)
        {
            return (uint) ((COP2_OPCODE << 26) | (rs << 21) | (rt << 16) | (rd << 11) | rest | funct);
        }
        return (uint) ((COP2_OPCODE << 26) | (rs << 21) | (rt << 16) | (rd << 11) | (sa << 6) | funct);
    }
}
