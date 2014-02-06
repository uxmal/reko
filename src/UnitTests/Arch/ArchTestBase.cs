#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch
{
    class ArchTestBase
    {
        private int instructionSize;

        public ArchTestBase(IProcessorArchitecture arch, int instructionSizeInBits)
        {
            this.Architecture = arch;
            this.instructionSize = instructionSizeInBits;
        }

        public IProcessorArchitecture Architecture { get; private set; }

        public uint ParseBitPattern(string bitPattern)
        {
            int cBits = 0;
            uint instr = 0;
            for (int i = 0; i < bitPattern.Length; ++i)
            {
                switch (bitPattern[i])
                {
                case '0':
                case '1':
                    instr = (instr << 1) | (uint) (bitPattern[i] - '0');
                    ++cBits;
                    break;
                }
            }
            if (cBits != instructionSize)
                throw new ArgumentException(
                    string.Format("Bit pattern didn't contain exactly {0} binary digits, but {1}.", instructionSize, cBits),
                    "bitPattern");
            return instr;
        }
    }
}
