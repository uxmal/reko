#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Services;
using System;

namespace Reko.ImageLoaders.MzExe.Pe
{
    public class x86_64Relocator : Relocator
    {
        private DecompilerEventListener dcSvc;

        public x86_64Relocator(IServiceProvider services, Program program) : base(program)
        {
            this.dcSvc = services.RequireService<DecompilerEventListener>();
        }

        private const ushort RelocationAbsolute = 0;
        private const ushort RelocationHigh = 1;
        private const ushort RelocationLow = 2;
        private const ushort RelocationHighLow = 3;


        public override void ApplyRelocation(Address baseOfImage, uint page, EndianImageReader rdr, RelocationDictionary relocations)
        {
            ushort fixup = rdr.ReadLeUInt16();
            Address offset = baseOfImage + page + (fixup & 0x0FFFu);
            var arch = program.Architecture;
            var imgR = program.CreateImageReader(arch, offset);
            var imgW = program.CreateImageWriter(arch, offset);
            switch (fixup >> 12)
            {
            case RelocationAbsolute:
                // Used for padding to 4-byte boundary, ignore.
                break;
            case RelocationHighLow:
                {
                    uint n = (uint)(imgR.ReadUInt32() + (baseOfImage - program.ImageMap.BaseAddress));
                    imgW.WriteUInt32(n);
                    relocations.AddPointerReference(offset.ToLinear(), n);
                    break;
                }
            case 0xA:
                break;
            default:
                dcSvc.Warn(
                    dcSvc.CreateAddressNavigator(program, offset),
                    string.Format(
                        "Unsupported i386 PE fixup type: {0:X}",
                        fixup >> 12));
                break;
            }
        }
    }
}