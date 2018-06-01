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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Libraries.Microchip.V1
{

    /// <summary>
    /// This class implements the <see cref="IPICDescriptor"/> interface for <see cref="PIC_v1"/>.
    /// </summary>
    internal sealed class PIC_v1_Interface : IPICDescriptor
    {
        private readonly PIC_v1 pic;
        private Arch_v1_Interface archintf = null;
        private ProgramSpace_v1_Interface progintf = null;
        private DataSpace_v1_Interface dataintf = null;
        private PICRegisters_v1_Interface regintf = null;

        private readonly static Dictionary<string, InstructionSetID> mapInstrID = new Dictionary<string, InstructionSetID>() {
                { "pic16f77", InstructionSetID.PIC16 },
                { "cpu_mid_v10", InstructionSetID.PIC16_ENHANCED },
                { "cpu_p16f1_v1", InstructionSetID.PIC16_FULLFEATURED },
                { "egg", InstructionSetID.PIC18_EXTENDED },
                { "pic18", InstructionSetID.PIC18 },
                { "cpu_pic18f_v6", InstructionSetID.PIC18_ENHANCED }
            };


        public PIC_v1_Interface(PIC_v1 pic)
        {
            this.pic = pic ?? throw new ArgumentNullException(nameof(pic));
        }

        public int Version => pic.Version;

        public string Name
            => pic.Name;

        public string Description
            => pic.Desc;

        public string ArchName
            => pic.Arch;

        public int ProcID
            => pic.ProcID;

        public string InstructionSetFamily
            => pic.InstructionSet.ID;

        public bool HasExtendedMode
            => pic.HasExtendedMode;

        public InstructionSetID GetInstructionSetID
        {
            get
            {
                if (pic.InstructionSet == null)
                    return InstructionSetID.UNDEFINED;
                if (!mapInstrID.TryGetValue(pic.InstructionSet.ID, out InstructionSetID id))
                {
                    id = InstructionSetID.UNDEFINED;
                    if (pic.Arch == "16xxxx")
                        id = InstructionSetID.PIC16;
                    if (pic.Arch == "16Exxx")
                        id = InstructionSetID.PIC16_FULLFEATURED;
                    if (pic.Arch == "18xxxx")
                        id = (HasExtendedMode ? InstructionSetID.PIC18_EXTENDED : InstructionSetID.PIC18);
                }
                return id;
            }
        }

        public PICFamily Family => GetInstructionSetID.GetFamily();

        public IArchDef ArchDefinitions
            => archintf = archintf ?? new Arch_v1_Interface(pic);

        public IProgramSpace ProgramMemorySpace
            => progintf = progintf ?? new ProgramSpace_v1_Interface(pic);

        public IDataSpace DataMemorySpace
            => dataintf = dataintf ?? new DataSpace_v1_Interface(pic);

        public IRegistersDefinitions PICRegisters
            => regintf = regintf ?? new PICRegisters_v1_Interface(pic);

    }

    /// <summary>
    /// This class implements the <see cref="IArchDef"/> interface for <see cref="PIC_v1"/>.
    /// </summary>
    internal sealed class Arch_v1_Interface : IArchDef
    {
        private readonly ArchDef arch;

        public Arch_v1_Interface(PIC_v1 pic)
        {
            arch = pic.ArchDef;
        }

        public string Name => arch.Name;

        public string Description => arch.Description;

        public IEnumerable<ITrait> MemoryTraits
            => arch.MemTraits.Traits.OfType<ITrait>();

        public int BankCount => arch.MemTraits.BankCount;

        public int HWStackDepth => arch.MemTraits.HWStackDepth;

        public uint MagicOffset
        {
            get
            {
                if (!magicOffset.HasValue)
                {
                    magicOffset = arch.MemTraits.Traits.OfType<EEDataMemTraits>().Select(t => t.MagicOffset).FirstOrDefault();
                }
                return magicOffset.Value;
            }
        }
        private uint? magicOffset;
    }

    /// <summary>
    /// This class implements the <see cref="IProgramSpace"/> interface for <see cref="PIC_v1"/>.
    /// </summary>
    internal sealed class ProgramSpace_v1_Interface : IProgramSpace
    {
        private readonly ProgramSpace space;

        public ProgramSpace_v1_Interface(PIC_v1 pic)
        {
            space = pic.ProgramSpace;
        }

        public IEnumerable<IPICMemoryRegion> MemoryRegions
        {
            get
            {
                if (space.CodeSectors != null)
                {
                    foreach (var sect in space.CodeSectors)
                        yield return sect.PICMemoryRegionInterface;
                }
                if (space.FusesSector != null)
                {
                    yield return space.FusesSector.PICMemoryRegionInterface;
                }
                if (space.InfoSectors != null)
                {
                    foreach (var sect in space.InfoSectors)
                        yield return sect.PICMemoryRegionInterface;
                }
            }
        }

        public IEnumerable<IDeviceInfoRegister> DeviceHWInfos
        {
            get
            {
                if (space.InfoSectors != null)
                {
                    foreach (var r in space.InfoSectors.OfType<IDeviceInfoSector>().Select(p => p.Registers).Cast<IDeviceInfoRegister>())
                    {
                        yield return r;
                    }
                }
                yield break;
            }
        }

        public IEnumerable<IDeviceFuse> ConfigurationFuses
        {
            get
            {
                if (space.FusesSector != null)
                {
                    foreach (var r in space.FusesSector.ConfigFuseSector_Interface.Fuses)
                    {
                        yield return r;
                    }
                }
                yield break;
            }
        }

    }

    /// <summary>
    /// This class implements the <see cref="IDataSpace"/> interface for <see cref="PIC_v1"/>.
    /// </summary>
    internal sealed class DataSpace_v1_Interface : IDataSpace
    {
        private readonly DataSpace space;

        public DataSpace_v1_Interface(PIC_v1 pic)
        {
            space = pic.DataSpace;
        }

        public uint DataSpaceSize => space.EndAddr;

        public IEnumerable<IPICMemoryRegion> AllMemoryRegions
        {
            get
            {
                if (space.RegardlessOfMode != null)
                {
                    foreach (var sect in space.RegardlessOfMode.RegistersRegions)
                        yield return sect.PICMemoryRegionInterface;
                }
                if (space.TraditionalModeOnly != null)
                {
                    foreach (var gpr in space.TraditionalModeOnly)
                    {
                        yield return gpr.PICMemoryRegionInterface;
                    }
                }
                if (space.ExtendedModeOnly != null)
                {
                    foreach (var gpr in space.ExtendedModeOnly)
                    {
                        yield return gpr.PICMemoryRegionInterface;
                    }
                }
            }
        }

        public IEnumerable<IPICMirroringRegion> Mirrors
        {
            get
            {
                foreach (var sfrd in space.RegardlessOfMode.RegistersRegions.Where(r => r.MemorySubDomain == PICMemorySubDomain.SFR).Cast<SFRDataSector>())
                {
                    foreach (var mir in sfrd.MirrorSFRs)
                    {
                        yield return mir;
                    }
                }
            }
        }

        public IEnumerable<IPICMemoryRegion> MemoryRegions(PICExecMode mode)
        {
            if (space.RegardlessOfMode != null)
            {
                foreach (var sect in space.RegardlessOfMode.RegistersRegions)
                    yield return sect.PICMemoryRegionInterface;
            }
            switch (mode)
            {
                case PICExecMode.Traditional:
                    if (space.TraditionalModeOnly != null)
                    {
                        foreach (var gpr in space.TraditionalModeOnly)
                            yield return gpr.PICMemoryRegionInterface;
                    }
                    break;
                case PICExecMode.Extended:
                    if (space.ExtendedModeOnly != null)
                    {
                        foreach (var gpr in space.ExtendedModeOnly)
                            yield return gpr.PICMemoryRegionInterface;
                    }
                    break;
            }
        }

    }

    /// <summary>
    /// This class implements the <see cref="IRegistersDefinitions"/> interface for <see cref="PIC_v1"/>.
    /// </summary>
    internal sealed class PICRegisters_v1_Interface : IRegistersDefinitions
    {
        private readonly PIC_v1 pic;

        public PICRegisters_v1_Interface(PIC_v1 pic)
        {
            this.pic = pic;
        }

        public IEnumerable<ISFRRegister> SFRs
        {
            get
            {
                DataSpace space = pic.DataSpace;
                foreach (var reg in space.RegardlessOfMode.RegistersRegions.Where(r => r.MemorySubDomain == PICMemorySubDomain.SFR))
                {
                    if (reg is SFRDataSector sfrd)
                    {
                        foreach (var sfr in sfrd.SFRs)
                            yield return sfr;
                        foreach (var jsfr in sfrd.JoinedSFRs)
                        {
                            byte bitPos = 0;
                            foreach (var sfr in jsfr.SFRs)
                            {
                                sfr.BitPos = bitPos;
                                yield return sfr;
                                bitPos += (byte)(sfr.ByteWidth * 8);
                            }
                        }
                        foreach (var mfsr in sfrd.MuxedSFRs)
                        {
                            yield return mfsr.SelectSFRs.First().SFR;
                        }
                    }
                }
                if (space.RegardlessOfMode.NMMRPlace != null && !IsPIC18)
                {
                    foreach (var sfr in space.RegardlessOfMode.NMMRPlace.SFRDefs)
                        yield return sfr;
                }
            }
        }

        public IEnumerable<IJoinedRegister> JoinedRegisters
        {
            get
            {
                DataSpace space = pic.DataSpace;
                foreach (var reg in space.RegardlessOfMode.RegistersRegions.Where(r => r.MemorySubDomain == PICMemorySubDomain.SFR))
                {
                    if (reg is SFRDataSector sfrd)
                    {
                        foreach (var jsfr in sfrd.JoinedSFRs)
                        {
                            byte jsize = 0;
                            byte bitPos = 0;
                            foreach (var sfr in jsfr.SFRs)
                            {
                                jsize += sfr.BitWidth;
                                sfr.BitPos = bitPos;
                                bitPos += (byte)(sfr.ByteWidth * 8);
                            }
                            jsfr.BitWidth = jsize;
                            yield return jsfr;
                        }
                    }
                }

            }
        }

        private bool IsPIC18 => pic.ArchDef.Name == "18xxxx";
    }

    internal sealed class PICMemoryRegion_v1_Interface : IPICMemoryRegion
    {
        private readonly PICMemoryRegion memReg;

        public PICMemoryRegion_v1_Interface(PICMemoryRegion memRegion)
        {
            memReg = memRegion;
        }

        public string RegionID => memReg.RegionID;

        public bool IsBank => memReg.IsBank;

        public int Bank => memReg.IsBank ? memReg.Bank : -1;

        public string ShadowIDRef => memReg.ShadowIDRef;

        public int ShadowOffset => memReg.ShadowOffset;

        public bool IsSection => memReg.IsSection;

        public string SectionDesc => memReg.IsSection ? memReg.SectionDesc : String.Empty;

        public string SectionName => memReg.IsSection ? memReg.SectionName : String.Empty;

        public uint BeginAddr => memReg.BeginAddr;

        public uint EndAddr => memReg.EndAddr;

        public PICMemoryDomain MemoryDomain => memReg.MemoryDomain;

        public PICMemorySubDomain MemorySubDomain => memReg.MemorySubDomain;
    }

    internal sealed class ConfigFuseSector_v1_Interface : IConfigFuses
    {
        private readonly ConfigFuseSector sector;

        public ConfigFuseSector_v1_Interface(ConfigFuseSector cfgsect)
        {
            sector = cfgsect;
        }

        public IEnumerable<IDeviceFuse> Fuses
        {
            get
            {
                foreach (var f in sector.Defs)
                {
                    switch (f)
                    {
                        case DCRDef dcr:
                            yield return dcr.DeviceFusesConfig_Interface;
                            break;

                        case AdjustPoint adj:
                            break;

                        default:
                            throw new InvalidOperationException($"Invalid configuration register type : {f.GetType()}");
                    }
                }
            }
        }

    }

    internal sealed class DeviceFusesConfig_v1_Interface : IDeviceFuse
    {
        private readonly DCRDef fuse;

        public DeviceFusesConfig_v1_Interface(DCRDef cfgfuse)
        {
            fuse = cfgfuse;
        }

        public int Addr => fuse.Addr;

        public string Name => fuse.CName;

        public string Description => fuse.Desc;

        public int BitWidth => fuse.NzWidth;

        public int ImplMask => fuse.ImplMask;

        public string AccessBits => fuse.Access;

        public int DefaultValue => fuse.DefaultValue;

        public bool IsLangHidden => fuse.IsLangHidden;

        public IEnumerable<IDeviceFusesIllegal> IllegalSettings
        {
            get
            {
                foreach (var ilg in fuse.Illegals)
                    yield return ilg;
            }
        }

        public IEnumerable<IDeviceFusesField> ConfigFields
        {
            get
            {
                foreach (var mode in fuse.DCRModes)
                {
                    int bitpos = 0;
                    foreach (var fld in mode.Fields)
                    {
                        switch (fld)
                        {
                            case AdjustPoint adj:
                                bitpos += adj.Offset;
                                break;

                            case DCRFieldDef fdef:
                                fdef.BitPos = (byte)bitpos;
                                bitpos += fdef.BitWidth;
                                yield return fdef;
                                break;

                            default:
                                throw new InvalidOperationException($"Invalid PIC device configuration field in '{Name}' register: {fld.GetType()}");
                        }
                    }
                }
            }
        }

    }

}
