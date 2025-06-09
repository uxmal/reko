#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.MicrochipPIC.Common
{
    /// <summary>
    /// A PIC program loaded image postprocessor. Allows validation of loaded image and customization per the PIC program memory space definition.
    /// </summary>
    public class PICProgramPostprocessor
    {
        private readonly Program program;
        private readonly PICArchitecture architecture;
        private readonly SegmentMap newMap;

        private Dictionary<string, int> renamingCounter;

        private PICProgramPostprocessor(Program prog, PICArchitecture arch)
        {
            program = prog;
            architecture = arch;
            newMap = new SegmentMap(Address.Ptr32(0));
            renamingCounter = null!;
        }

        public static Program Validate(Program prog, PICArchitecture arch)
            => new PICProgramPostprocessor(prog ?? throw new ArgumentNullException(nameof(prog)),
                                         arch ?? throw new ArgumentNullException(nameof(arch)))
                    .PerformValidation();

        private Program PerformValidation()
        {
            var user = program.User;
            user.TextEncoding = Encoding.ASCII;
            bool renameSection = architecture.Options.LoaderType == "raw";

            // Re-assign memory segments according to PIC program memory space definition. Rename segments, as-needed (raw file, segment larger than PIC memory region).
            
            foreach (var segt in program.SegmentMap.Segments.Values)
            {
                var curAddr = segt.Address;
                var curSize = segt.ContentSize;
                do
                {
                    var regn = PICMemoryDescriptor.GetProgramRegion(curAddr);
                    if (regn is null)
                        throw new InvalidOperationException("Attempt to load a binary image which is not compatible with the selected PIC's program memory space.");
                    var fitSize = Math.Min(regn.PhysicalByteAddrRange.End - curAddr, curSize);
                    if (fitSize <= 0)
                        throw new InvalidOperationException("Attempt to load a binary image which is not compatible with the selected PIC's program memory space.");
                    var rd = segt.MemoryArea.CreateLeReader(curAddr);
                    var b = rd.ReadBytes((int)fitSize);
                    var splitMem = new ByteMemoryArea(curAddr, b);
                    string segmentName = (renameSection ? GetRegionSequentialName(regn) : GetSegmentSequentialName(segt));
                    var newsegt = new ImageSegment(segmentName, splitMem, GetAccessMode(regn));
                    newMap.AddSegment(newsegt);
                    curSize -= (uint)fitSize;
                    curAddr += fitSize;
                } while (curSize > 0);

            }
            program.SegmentMap = newMap;
            SetPICExecMode();
            SetRegistersAtPOR();

            return program;
        }

        /// <summary>
        /// Gets a unique region's name.
        /// </summary>
        /// <param name="regn">The PIC program memory region descriptor.</param>
        private string GetRegionSequentialName(IMemoryRegion regn)
        {
            if (renamingCounter is null)
            {
                renamingCounter = new Dictionary<string, int>();
            }
            if (renamingCounter.TryGetValue(regn.RegionName, out var counter))
            {
                renamingCounter[regn.RegionName] = counter + 1;
                return $"{regn.RegionName}_{counter}";
            }
            renamingCounter[regn.RegionName] = 2;
            return $"{regn.RegionName}_1";
        }

        /// <summary>
        /// Gets a unique segment's name.
        /// </summary>
        /// <param name="regn">The binary file image segment.</param>
        private string GetSegmentSequentialName(ImageSegment segt)
        {
            if (renamingCounter is null)
            {
                renamingCounter = new Dictionary<string, int>();
            }
            if (renamingCounter.TryGetValue(segt.Name, out var counter))
            {
                renamingCounter[segt.Name] = counter + 1;
                return $"{segt.Name}_{counter}";
            }
            renamingCounter[segt.Name] = 1;
            return $"{segt.Name}";
        }

        /// <summary>
        /// Gets the memory segment access mode based on PIC memory region's attributes
        /// </summary>
        /// <param name="regn">The PIC program memory region descriptor.</param>
        /// <returns>
        /// The Reko memory segment access mode.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        private AccessMode GetAccessMode(IMemoryRegion regn)
        {
            if (regn.TypeOfMemory != PICMemoryDomain.Prog && regn.TypeOfMemory != PICMemoryDomain.Other)
                throw new InvalidOperationException($"{nameof(GetAccessMode)}({regn.TypeOfMemory})");

            switch (regn.SubtypeOfMemory)
            {
                case PICMemorySubDomain.Code:
                case PICMemorySubDomain.ExtCode:
                case PICMemorySubDomain.Debugger:
                case PICMemorySubDomain.Test:
                    return AccessMode.ReadExecute;
            }
            return AccessMode.Read;
        }

        /// <summary>
        /// Sets the PIC execution mode per the XINST device configuration bit value (if present in the memory image).
        /// If no XINST bit can be found in the image or for this processor, the PIC execution mode is left to the user's choice.
        /// </summary>
        private void SetPICExecMode()
        {
            PICExecMode pexec = architecture.Options.PICExecutionMode;

            var dcf = PICMemoryDescriptor.GetDCRField("XINST");
            if (dcf is not null)
            {
                var dcr = PICMemoryDescriptor.GetDCR(dcf.RegAddress);
                if (dcr is not null && program.SegmentMap.TryFindSegment(dcr.Address, out ImageSegment? xinstsegt))
                {
                    var mem = (ByteMemoryArea) xinstsegt.MemoryArea;
                    uint xinstval;
                    if (dcr.BitWidth <= 8)
                        xinstval = mem.ReadByte(dcf.RegAddress);
                    else if (dcr.BitWidth <= 16)
                        xinstval = mem.ReadLeUInt16(dcf.RegAddress);
                    else
                        xinstval = mem.ReadLeUInt32(dcf.RegAddress);
                    pexec = (dcr.CheckIf("XINST", "ON", xinstval) ? PICExecMode.Extended : PICExecMode.Traditional);
                }
            }
            else
            {
                pexec = PICExecMode.Traditional;
            }
            PICMemoryDescriptor.ExecMode = pexec;
            architecture.Options.PICExecutionMode = pexec;
            architecture.Description = architecture.Options.ProcessorModel.PICName + $" ({pexec})";
        }

        /// <summary>
        /// Sets the PIC registers at Power-On-Reset.
        /// </summary>
        private void SetRegistersAtPOR()
        {
            program.User.RegisterValues[PICProgAddress.Ptr(0)] = PICRegisters.GetPORRegistersList();
        }

    }

}
