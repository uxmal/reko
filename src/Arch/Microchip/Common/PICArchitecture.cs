#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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
        protected PICArchitecture(string archID, PICProcessorMode mode) : base(archID)
        {
            if (mode is null)
                throw new ArgumentNullException(nameof(mode));
            ProcessorMode = mode;
            PICDescriptor = mode.PICDescriptor;
            flagGroups = new List<FlagGroupStorage>();
            FramePointerType = PrimitiveType.Offset16;
            InstructionBitSize = 8;
            PointerType = PrimitiveType.Ptr32;
            WordWidth = PrimitiveType.Byte;
            LoadConfiguration();
        }

        /// <summary>
        /// Loads the PIC configuration. Creates memory mapper and registers for the targetted PIC.
        /// </summary>
        protected abstract void LoadConfiguration();


        #region Public Methods/Properties

        /// <summary>
        /// Gets PIC descriptor as retrieved from the Microchip Crownking database.
        /// </summary>
        public PIC PICDescriptor { get; }

        /// <summary>
        /// Gets or sets the PIC execution mode.
        /// </summary>
        /// <value>
        /// The PIC execution mode.
        /// </value>
        public virtual PICExecMode ExecMode
        {
            get => MemoryDescriptor.ExecMode;
            set => MemoryDescriptor.ExecMode = value;
        }

        /// <summary>
        /// Gets the PIC memory mapper.
        /// </summary>
        public abstract IPICMemoryDescriptor MemoryDescriptor { get; }

        /// <summary>
        /// Gets the device configuration definitions.
        /// </summary>
        public IPICDeviceConfigDefs DeviceConfigDefinitions
        {
            get
            {
                if (deviceConfigDefinitions is null)
                {
                    deviceConfigDefinitions = PICDeviceConfigDefs.Create(PICDescriptor);
                }
                return deviceConfigDefinitions;
            }
        }
        private IPICDeviceConfigDefs deviceConfigDefinitions;

        protected PICProcessorMode ProcessorMode { get; }

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
        {
            var r = PICRegisters.GetRegister(regName);
            if (r == RegisterStorage.None)
                throw new ArgumentException($"'{regName}' is not a known register name.");
            return r;
        }

        public override RegisterStorage[] GetRegisters()
            => PICRegisters.GetRegisters;

        public override bool TryGetRegister(string regName, out RegisterStorage reg)
        {
            reg = default(RegisterStorage);
            var res = PICRegisters.TryGetRegister(regName, out var preg);
            if (res)
            {
                reg = preg;
            }
            return res;
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

        public override Address ReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState state)
            => PICProgAddress.Ptr(rdr.ReadLeUInt32());

        public override bool TryParseAddress(string txtAddress, out Address addr)
            => Address.TryParse32(txtAddress, out addr);

        #endregion

    }

}
