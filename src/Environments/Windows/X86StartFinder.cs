#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Serialization;

namespace Reko.Environments.Windows
{
    class X86StartFinder
    {
        private const byte WILD = 0xF4;

        private readonly Address addrStart;
        private readonly Program program;

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

        private static readonly byte[] msvcDebugCrt =
        {
            0x8B, 0x95, 0x70, 0xFF, 0xFF, 0xFF, // mov edx,[ebp+FFFFFF70]
            0x52,                               // push edx
            0x8B, 0x45, 0x8C,                   // mov eax,[ebp - 74]
            0x50,                               // push eax
            0x6A, 0x00,                         // push 00
            0x6A, 0x00,                         // push 00
            0xFF, 0x15, WILD, WILD, WILD, WILD, // call dword ptr[GetModuleHandle]
            0x50,                               // push eax
            0xE8, WILD, WILD, WILD, WILD,       // call fn00458D40
            0x89, 0x85, 0x7C, 0xFF, 0xFF, 0xFF, // mov [ebp + FFFFFF7C],eax
        };

        private static readonly SerializedSignature mainSignature = new SerializedSignature
        {
            Convention = "cdecl",
            ReturnValue = new Argument_v1
            {
                Type = PrimitiveType_v1.Int32(),
            },
            Arguments = new[]
            {
                Arg("argc", PrimitiveType_v1.Int32()),
                Arg("argv", new PointerType_v1 {
                    PointerSize = 4,
                    DataType = new PointerType_v1 {
                        PointerSize = 4,
                        DataType = PrimitiveType_v1.Char8(),
                    }
                })
            }
        };

        private static readonly SerializedSignature winmainSignature = new SerializedSignature
        {
            Convention = "stdapi",
            Arguments = new Argument_v1[]
            {
                Arg("hInstance",     "HINSTANCE"),
                Arg("hPrevInstance", "HINSTANCE"),
                Arg("lpCmdLine",     "LPSTR"),
                Arg("nCmdShow",      "INT"),
            },
            ReturnValue = Arg(null, "INT")
        };

        private static Argument_v1 Arg(string? name, string typename)
        {
            return new Argument_v1
            {
                Name = name,
                Type = new TypeReference_v1 { TypeName = typename }
            };
        }

        private static Argument_v1 Arg(string name, SerializedType sType)
        {
            return new Argument_v1
            {
                Name = name,
                Type = sType,
            };
        }

        public ImageSymbol? FindMainProcedure()
        {
            const uint MaxDistanceFromEntry = 0x400;

            uint idx;

            // Start at program entry point
            if (!program.SegmentMap.TryFindSegment(this.addrStart, out ImageSegment? seg))
                return null;
            var offsetStart = this.addrStart - seg.MemoryArea.BaseAddress;
            var offsetMax = Math.Min(seg.MemoryArea.Length, offsetStart + MaxDistanceFromEntry);
            var rdr = program.Architecture.CreateImageReader(
                seg.MemoryArea,
                offsetStart,
                offsetMax);
            var dasm = program.Architecture.CreateDisassembler(rdr);
            var p = dasm.GetEnumerator();
            if (!p.MoveNext())
                return null;

            var instr = p.Current;
            var op0 = instr.Operands.Length > 0 ? instr.Operands[0] : null;
            if (instr.InstructionClass == InstrClass.Transfer)
            {
                if (op0 is Address addrOp0 && addrOp0 > instr.Address)
                {
                    // Forward jump (appears in Borland binaries)
                    p.Dispose();
                    // Search for this pattern.
                    var bmem = (ByteMemoryArea) seg.MemoryArea;
                    if (!LocatePattern(
                        bmem.Bytes,
                        (uint) (addrOp0 - seg.MemoryArea.BaseAddress),
                        (uint) (offsetMax),
                        borlandPattern,
                        out idx))
                        return null;
                    var iMainInfo = idx + 0x0E;
                    var addrMainInfo = Address.Ptr32(bmem.ReadLeUInt32(iMainInfo));
                    if (!program.SegmentMap.TryFindSegment(addrMainInfo, out ImageSegment? segMainInfo))
                        return null;
                    var addrMain = Address.Ptr32(
                        ((ByteMemoryArea)segMainInfo.MemoryArea).ReadLeUInt32(addrMainInfo + 0x18));
                    if (program.SegmentMap.IsExecutableAddress(addrMain))
                    {
                        return ImageSymbol.Procedure(program.Architecture, addrMain, "main", signature: mainSignature);
                    }
                }
            }
            var bmem2 = (ByteMemoryArea) seg.MemoryArea;
            if (LocatePattern(
                   bmem2.Bytes,
                   (uint)(addrStart - seg.MemoryArea.BaseAddress),
                   (uint)(offsetMax),
                   msvcDebugCrt,
                   out idx))
            {
                idx += 0x17;        // skip to call <offset>
                seg.MemoryArea.TryReadLeInt32(idx, out int offset);
                var addrMain = seg.MemoryArea.BaseAddress + idx + 5 + offset;
                if (program.SegmentMap.IsExecutableAddress(addrMain))
                {
                    return ImageSymbol.Procedure(program.Architecture, addrMain, "WinMain", signature: winmainSignature);
                }
            }
            return null;
        }

        /* 
         * Search the source array between limits iMin and iMax for the 
         * pattern (length iPatLen). The pattern can contain wild bytes; if 
         * you really want to match for the pattern that is used up by the
         * WILD uint8_t, tough - it will match with everything else as well.
         */
 //$TODO move this to memoryarea
        public static bool LocatePattern(
            byte[] source, uint iMin, uint iMax,
            byte[] pattern,
            out uint index)
        {
            uint i, j;
            uint pSrc;                             /* Pointer to start of considered source */
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
                        // A definite mismatch
                        break;                      // Break to outer loop 
                    }
                    pSrc++;
                }
                if (j >= pattern.Length)
                {
                    /* Pattern has been found */
                    index = i;                      /* Pass start of pattern */
                    return true;                    /* Indicate success */
                }
                /* Else just try next value of i */
            }
            /* Pattern was not found */
            index = ~0u;                            /* Invalidate index */
            return false;                           /* Indicate failure */
        }
    }
}
 