#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
/*
* Copyright (C) 2000, The University of Queensland
* Copyright (C) 2001, Sun Microsystems, Inc
* Copyright (C) 2002, Trent Waddington
*
* See the file "LICENSE.TERMS" for information on usage and
* redistribution of this file, and for a DISCLAIMER OF ALL
* WARRANTIES.
*
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Serialization;

namespace Reko.Environments.Windows
{
    class X86StartFinder
    {
        private const byte WILD = 0xF4;

        private Address addrStart;
        private Program program;

        public X86StartFinder(Program program, Address addrStart)
        {
            this.program = program;
            this.addrStart = addrStart;
        }


        private static readonly byte[] borlandPattern =
        {
            0xE8, WILD, WILD, WILD, WILD,   // call <something>
            0x6A, 0x00,                     // push 0
            0xE8, WILD, WILD, WILD, WILD,   // call <__ExceptInit>
            0x59,                           // pop ecx
            0x68, WILD, WILD, WILD, WILD,   // push offset mainInfo
            0x6A, 0x00,                     // push 0
            0xE8,                           // call
        };

        private static readonly SerializedSignature mainSignature = new SerializedSignature
        {
            Convention = "cdecl",
            ReturnValue = new Argument_v1
            {
                Kind = new Register_v1 { Name = "eax" },
                Type = PrimitiveType_v1.Int32(),
            },
            Arguments = new[]
            {
                new Argument_v1
                {
                    Name = "argc",
                    Kind = new StackVariable_v1(),
                    Type = PrimitiveType_v1.Int32(),
                },
                new Argument_v1
                {
                    Name = "argv",
                    Kind = new StackVariable_v1(),
                    Type = new PointerType_v1 {
                        PointerSize = 4,
                        DataType = new PointerType_v1 {
                            PointerSize = 4,
                            DataType = PrimitiveType_v1.Char8(),
                        }
                    }
                },
            }
        };


        public ImageSymbol FindMainProcedure()
        {
            const uint MaxDistanceFromEntry = 0x200;

            // Start at program entry point
            ImageSegment seg;
            if (!program.SegmentMap.TryFindSegment(this.addrStart, out seg))
                return null;
            var addrMax = Address.Min(seg.MemoryArea.EndAddress, addrStart + MaxDistanceFromEntry);
            var rdr = program.Architecture.CreateImageReader(
                seg.MemoryArea,
                addrStart,
                addrMax);
            var dasm = program.Architecture.CreateDisassembler(rdr);
            var p = dasm.GetEnumerator();
            while (p.MoveNext())
            {
                var instr = p.Current;
                var op0 = instr.GetOperand(0);
                var addrOp0 = op0 as AddressOperand;
                switch (instr.InstructionClass)
                {
                case InstructionClass.Transfer:
                    if (addrOp0 != null && addrOp0.Address > instr.Address)
                    {
                        p.Dispose();
                        // Forward jump (appears in Borland binaries)
                        dasm = program.CreateDisassembler(addrOp0.Address);

                        // Search for this pattern.
                        uint idx;
                        if (!locatePattern(
                            seg.MemoryArea.Bytes,
                            (uint)(addrOp0.Address - seg.MemoryArea.BaseAddress),
                            (uint)(addrMax - seg.MemoryArea.BaseAddress),
                            borlandPattern,
                            out idx))
                            return null;
                        var iMainInfo = idx + 0x0E;
                        var addrMainInfo = Address.Ptr32(
                            seg.MemoryArea.ReadLeUInt32(iMainInfo));
                        ImageSegment segMainInfo;
                        if (!program.SegmentMap.TryFindSegment(addrMainInfo, out segMainInfo))
                            return null;
                        var addrMain = Address.Ptr32(
                            segMainInfo.MemoryArea.ReadLeUInt32(addrMainInfo + 0x18));
                        if (program.SegmentMap.IsExecutableAddress(addrMain))
                        {
                            return new ImageSymbol(addrMain)
                            {
                                Type = SymbolType.Procedure,
                                Name = "main",
                                Signature = mainSignature,
                            };
                        }

                    }
                    break;
                }
            }
            return null;
        }

        /* Search the source array between limits iMin and iMax for the pattern (length
 iPatLen). The pattern can contain wild bytes; if you really want to match
 for the pattern that is used up by the WILD uint8_t, tough - it will match with
 everything else as well. */
 //$TODO moe this to memoryarea
        static bool locatePattern(
            byte[] source, uint iMin, uint iMax,
            byte[] pattern,
            out uint index)
        {
            index = 0;
            uint i, j;
            uint pSrc = 0;                             /* Pointer to start of considered source */
            uint iLast;

            iLast = iMax - (uint)pattern.Length;                 /* Last source uint8_t to consider */

            for (i = iMin; i <= iLast; i++)
            {
                pSrc = i;                  /* Start of current part of source */
                                           /* i is the index of the start of the moving pattern */
                for (j = 0; j < pattern.Length; j++)
                {
                    /* j is the index of the uint8_t being considered in the pattern. */
                    if ((source[pSrc] != pattern[j]) && (pattern[j] != WILD))
                    {
                        /* A definite mismatch */
                        break;                      /* Break to outer loop */
                    }
                    pSrc++;
                }
                if (j >= pattern.Length)
                {
                    /* Pattern has been found */
                    index = i;                     /* Pass start of pattern */
                    return true;                       /* Indicate success */
                }
                /* Else just try next value of i */
            }
            /* Pattern was not found */
            index = ~0u;                            /* Invalidate index */
            return false;                               /* Indicate failure */
        }



    }

}
 