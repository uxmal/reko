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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Microchip.Common
{
    using Common;

    public abstract class PICArchitecture : ProcessorArchitecture
    {
        protected List<FlagGroupStorage> flagGroups;

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="archID">Identifier for the architecture. Can't be interpreted as the name of the PIC.</param>
        public PICArchitecture(string archID) : base(archID)
        {
            flagGroups = new List<FlagGroupStorage>();
            FramePointerType = PrimitiveType.Offset16;
            InstructionBitSize = 8;
            PointerType = PrimitiveType.Ptr32;
            WordWidth = PrimitiveType.Byte;
        }

        /// <summary>
        /// Constructor. Used for tests purpose.
        /// </summary>
        /// <param name="picDescr">PIC descriptor.</param>
        public PICArchitecture(string picFamily, PIC picDescr) : this(picFamily)
        {
            picDescriptor = picDescr ?? throw new ArgumentNullException(nameof(picDescr));
            CPUModel = picDescriptor.Name;
        }

        #endregion

        #region Helpers

        protected abstract void LoadConfiguration();

        #endregion

        #region Public Methods/Properties

        /// <summary>
        /// Gets PIC descriptor as retrieved from the Microchip Crownking database.
        /// </summary>
        public PIC PICDescriptor
        {
            get
            {
                if (picDescriptor is null)
                {
                    try
                    {
                        PICCrownking db = PICCrownking.GetDB() ??
                            throw new InvalidOperationException($"Cannot get access to PIC database. (DBError={PICCrownking.LastErrMsg}).");
                        PIC pic = db.GetPIC(CPUModel) ?? throw new InvalidOperationException($"No such PIC: '{CPUModel}'");
                        PICDescriptor = pic;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Unable to retrieve PIC definition for PIC name '{CPUModel}'", ex);
                    }
                }
                return picDescriptor;
            }
            private set
            {
                if (picDescriptor != value)
                {
                    picDescriptor = value;
                    if (!(picDescriptor is null))
                        LoadConfiguration();
                }
            }
        }
        protected PIC picDescriptor;

        /// <summary>
        /// Gets or sets the CPU model.
        /// </summary>
        public override string CPUModel
        {
            get => cpuModel;
            set
            {
                if (cpuModel != value)
                {
                    cpuModel = value;
                    picDescriptor = null;
                }
            }
        }
        private string cpuModel = String.Empty;

        /// <summary>
        /// Gets or sets the PIC execution mode.
        /// </summary>
        /// <value>
        /// The PIC execution mode.
        /// </value>
        public PICExecMode ExecMode
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
        public IDeviceConfigDefs DeviceConfigDefinitions
        {
            get
            {
                if (deviceConfigDefinitions is null)
                {
                    deviceConfigDefinitions = DeviceConfigDefs.Create(PICDescriptor);
                }
                return deviceConfigDefinitions;
            }
        }
        private IDeviceConfigDefs deviceConfigDefinitions;

        public override RegisterStorage GetRegister(int i)
            => PICRegisters.GetCoreRegisterByIdx(i);

        public override RegisterStorage GetRegister(string regName)
        {
            var r = PICRegisters.GetRegisterByName(regName);
            if (r == RegisterStorage.None)
                throw new ArgumentException($"'{regName}' is not a register name.");
            return r;
        }

        public override RegisterStorage[] GetRegisters()
            => PICRegisters.GetRegisters;

        public override bool TryGetRegister(string regName, out RegisterStorage reg)
        {
            reg = PICRegisters.GetRegisterByName(regName);
            return (reg != RegisterStorage.None);
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
