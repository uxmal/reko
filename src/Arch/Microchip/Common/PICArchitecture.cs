#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Libraries.Microchip;
using Reko.Core.Machine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.Common
{
    public abstract class PICArchitecture : ProcessorArchitecture
    {
        protected List<FlagGroupStorage> flagGroups;


        /// <summary>
        /// Instantiates a new PIC architecture for the specified PIC generic family.
        /// </summary>
        /// <param name="archID">Identifier for the architecture. Can't be interpreted as the name of the PIC.</param>
        protected PICArchitecture(string archID, IPICProcessorMode mode) : base(archID)
        {
            ProcessorMode = mode ?? throw new ArgumentNullException(nameof(mode));
            PICDescriptor = mode.PICDescriptor;
            flagGroups = new List<FlagGroupStorage>();
            FramePointerType = PrimitiveType.Offset16;
            InstructionBitSize = 8;
            PointerType = PrimitiveType.Ptr32;
            WordWidth = PrimitiveType.Byte;
            LoadConfiguration();
        }


        /// <summary>
        /// Gets PIC descriptor as retrieved from the Microchip Crownking database.
        /// </summary>
        public PIC PICDescriptor { get; }

        protected IPICProcessorMode ProcessorMode { get; }

        public PICOptions Options { get; set; }

        /// <summary>
        /// Loads the PIC configuration. Creates memory mapper and registers for the targetted PIC.
        /// </summary>
        protected abstract void LoadConfiguration();

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
            => new PICInstructionComparer(norm);

        public override SortedList<string, int> GetOpcodeNames()
        {
            return Enum.GetValues(typeof(Opcode))
                .Cast<Opcode>()
                .ToSortedList(
                    v => v.ToString().ToUpper(),
                    v => (int)v);
        }

        public override int? GetOpcodeNumber(string name)
        {
            if (!Enum.TryParse(name, true, out Opcode result))
                return null;
            return (int)result;
        }

        /// <summary>
        /// Gets a register given its index number.
        /// </summary>
        /// <param name="i">Zero-based index of the register.</param>
        /// <returns>
        /// The register instance or null.
        /// </returns>
        public override RegisterStorage GetRegister(int i)
            => PICRegisters.PeekRegisterByIdx(i);

        /// <summary>
        /// Gets a register given its name.
        /// </summary>
        /// <param name="regName">Name of the register.</param>
        /// <returns>
        /// The register.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        ///                                     illegal values.</exception>
        public override RegisterStorage GetRegister(string regName)
            => PICRegisters.GetRegister(regName);

        /// <summary>
        /// Get the proper sub-register of <paramref name="reg" /> that starts at offset
        /// <paramref name="offset" /> and is of size <paramref name="width"/>.
        /// </summary>
        /// <param name="reg">The parent register.</param>
        /// <param name="offset">The bit offset of the sub-register.</param>
        /// <param name="width">The bit width of the sub-register.</param>
        /// <returns>
        /// The sub-register.
        /// </returns>
        /// <remarks>
        /// Most architectures not have sub-registers, and will use this default implementation. This
        /// method is overridden for architectures like x86 and Z80, where sub-registers <code>(ah, al,
        /// etc)</code>
        /// do exist.
        /// </remarks>
        public override RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
            => PICRegisters.GetSubregister(reg, offset, width);

        /// <summary>
        /// Find the widest sub-register that covers the register <paramref name="reg"/>.
        /// </summary>
        /// <param name="reg">The target register.</param>
        /// <param name="regs">.</param>
        /// <returns>
        /// The widest subregister(s).
        /// </returns>
        public override RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> regs)
            => PICRegisters.GetWidestSubregister(reg, regs);

        /// <summary>
        /// Find the parent (joined) register of the register <paramref name="reg"/>.
        /// </summary>
        /// <param name="reg">The child (joinee) register.</param>
        /// <returns>
        /// The parent register or null.
        /// </returns>
        public RegisterStorage GetParentRegister(RegisterStorage reg)
            => PICRegisters.GetParentRegister(reg);

        /// <summary>
        /// Gets the registers.
        /// </summary>
        /// <returns>
        /// An array of register storage.
        /// </returns>
        public override RegisterStorage[] GetRegisters()
            => PICRegisters.GetRegisters;

        /// <summary>
        /// Attempts to get a <see cref="RegisterStorage"/> from its name.
        /// </summary>
        /// <param name="regName">Name of the register.</param>
        /// <param name="reg">[out] The register.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public override bool TryGetRegister(string regName, out RegisterStorage reg)
        {
            var res = PICRegisters.TryGetRegister(regName, out var preg);
            reg = preg;
            return res;
        }

        public override void LoadUserOptions(Dictionary<string, object> options)
        {
            if (options != null)
            {
                Options = new PICOptions
                {
                    ExecMode = (options.ContainsKey("ExtendedMode") && (string)options["ExtendedMode"] == "true" ?
                                PICExecMode.Extended : PICExecMode.Traditional)
                };
                if (options.TryGetValue("Model", out var smodel))
                    Options.Mode = PICProcessorMode.GetMode(smodel as string);
                else
                    throw new InvalidOperationException("Missing PIC model in options.");
            }
        }

        public override Dictionary<string, object> SaveUserOptions()
        {
            if (Options == null)
                return null;
            var dict = new Dictionary<string, object>();
            if (Options.ExecMode == PICExecMode.Extended)
            {
                dict["ExtendedMode"] = "true";
            }
            dict["Model"] = Options.Mode.PICName;
            return dict;
        }

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addr)
            => new LeImageReader(image, addr);

        public override EndianImageReader CreateImageReader(MemoryArea image, Address addrBegin, Address addrEnd)
            => new LeImageReader(image, addrBegin, addrEnd);

        public override EndianImageReader CreateImageReader(MemoryArea image, ulong offset)
            => new LeImageReader(image, offset);

        public override ImageWriter CreateImageWriter()
            => new LeImageWriter();

        public override ImageWriter CreateImageWriter(MemoryArea mem, Address addr)
            => new LeImageWriter(mem, addr);

        public override Address MakeAddressFromConstant(Constant c)
            => Address.Ptr32(c.ToUInt32());

        public override Address MakeSegmentedAddress(Constant seg, Constant offset)
            => ProcessorMode.CreateBankedAddress(seg.ToByte(), offset.ToByte());

        public override Address ReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState state)
            => PICProgAddress.Ptr(rdr.ReadLeUInt32());

        public override bool TryParseAddress(string txtAddress, out Address addr)
            => Address.TryParse32(txtAddress, out addr);

        public override bool TryRead(MemoryArea mem, Address addr, PrimitiveType dt, out Constant value)
            => mem.TryReadLe(addr, dt, out value);

    }

}
