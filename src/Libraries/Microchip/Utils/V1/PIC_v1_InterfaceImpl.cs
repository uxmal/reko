#region License
/* 
 * Copyright (c) 2017-2025 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2025 John Källén.
 *
 * The contents of this file are subject to the terms of the Common Development
 * and Distribution License (the License), or the GPL v2, or (at your option)
 * any later version. 
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the License at http://www.gnu.org/licenses/gpl-2.0.html.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *
 * If applicable, add the following below the header, with the fields
 * enclosed by brackets [] replaced by your own identifying information:
 * "Portions Copyrighted (c) [year] [name of copyright owner]"
 *
 */

#endregion

namespace Reko.Libraries.Microchip.V1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// This class implements the <see cref="IPICDescriptor"/> interface for <see cref="PIC_v1"/> class instances.
    /// </summary>
    internal sealed class PIC_v1_Interface : IPICDescriptor
    {
        private readonly PIC_v1 pic;
        private readonly ArchDef arch;
        private readonly ProgramSpace progspace;
        private readonly DataSpace dataspace;
        private uint? magicOffset;

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
            arch = pic.ArchDef ?? throw new ArgumentNullException(nameof(pic.ArchDef));
            progspace = pic.ProgramSpace ?? throw new ArgumentNullException(nameof(pic.ProgramSpace));
            dataspace = pic.DataSpace ?? throw new ArgumentNullException(nameof(pic.DataSpace));
        }

        int IPICDescriptor.Version => pic.Version;
        string IPICDescriptor.PICName => pic.Name;
        string IPICDescriptor.Description => pic.Desc;
        string IPICDescriptor.ArchName => pic.Arch;
        int IPICDescriptor.ProcID => pic.ProcID;
        string IPICDescriptor.InstructionSetFamily => pic.InstructionSet.ID;
        bool IPICDescriptor.IsPIC18 => IsPIC18;
        bool IPICDescriptor.HasExtendedMode => pic.HasExtendedMode;
        InstructionSetID IPICDescriptor.GetInstructionSetID => GetInstructionSetID;
        PICFamily IPICDescriptor.Family => GetInstructionSetID.GetFamily();
        int IPICDescriptor.BankCount => arch.MemTraits.BankCount;
        int IPICDescriptor.HWStackDepth => arch.MemTraits.HWStackDepth;
        IEnumerable<IPICMemTrait> IPICDescriptor.PICMemoryTraits
        {
            get
            {
                foreach (var t in arch.MemTraits.Traits)
                    yield return t.V1_Interface;
            }
        }
        uint IPICDescriptor.DataSpaceSize => dataspace.EndAddr;

        uint IPICDescriptor.MagicOffset
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

        IEnumerable<IPICMemoryRegion> IPICDescriptor.ProgMemoryRegions
        {
            get
            {
                if (progspace.CodeSectors is not null)
                {
                    foreach (var sect in progspace.CodeSectors)
                        yield return sect.V1_Interface;
                }
                if (progspace.FusesSector is not null)
                {
                    yield return progspace.FusesSector.V1_Interface;
                }
                if (progspace.InfoSectors is not null)
                {
                    foreach (var sect in progspace.InfoSectors)
                        yield return sect.V1_Interface;
                }
            }
        }

        IEnumerable<IDeviceInfoRegister> IPICDescriptor.DeviceHWInfos
        {
            get
            {
                if (progspace.InfoSectors is not null)
                {
                    foreach (var r in progspace.InfoSectors.OfType<IDeviceInfoSector>().Select(p => p.Registers).Cast<IDeviceInfoRegister>())
                    {
                        yield return r;
                    }
                }
                yield break;
            }
        }

        IEnumerable<IDeviceFuse> IPICDescriptor.ConfigurationFuses
        {
            get
            {
                if (progspace.FusesSector is not null)
                {
                    foreach (var d in progspace.FusesSector.Defs)
                    {
                        switch (d)
                        {
                        case DCRDef dcr:
                            yield return dcr.V1_Interface;
                            break;

                        case AdjustPoint adj:
                        default:
                            throw new InvalidOperationException($"Invalid configuration fuse type: {d.GetType()}");
                        }
                    }
                }
                yield break;
            }
        }

        IEnumerable<IPICMemoryRegion> IPICDescriptor.AllDataMemoryRegions
        {
            get
            {
                if (dataspace.RegardlessOfMode is not null)
                {
                    foreach (var sect in dataspace.RegardlessOfMode.RegistersRegions)
                        yield return sect.V1_Interface;
                }
                if (dataspace.TraditionalModeOnly is not null)
                {
                    foreach (var gpr in dataspace.TraditionalModeOnly)
                    {
                        yield return gpr.V1_Interface;
                    }
                }
                if (dataspace.ExtendedModeOnly is not null)
                {
                    foreach (var gpr in dataspace.ExtendedModeOnly)
                    {
                        yield return gpr.V1_Interface;
                    }
                }
            }
        }

        IEnumerable<IPICMirroringRegion> IPICDescriptor.MirroringRegions
        {
            get
            {
                foreach (var sfrd in dataspace.RegardlessOfMode.RegistersRegions.Where(r => r.MemorySubDomain == PICMemorySubDomain.SFR).Cast<SFRDataSector>())
                {
                    foreach (var mir in sfrd.MirrorSFRs)
                    {
                        yield return mir.V1_Interface;
                    }
                }
            }
        }

        IEnumerable<IPICMemoryRegion> IPICDescriptor.DataMemoryRegions(PICExecMode mode)
        {
            if (dataspace.RegardlessOfMode is not null)
            {
                foreach (var sect in dataspace.RegardlessOfMode.RegistersRegions)
                    yield return sect.V1_Interface;
            }
            switch (mode)
            {
            case PICExecMode.Traditional:
                if (dataspace.TraditionalModeOnly is not null)
                {
                    foreach (var gpr in dataspace.TraditionalModeOnly)
                        yield return gpr.V1_Interface;
                }
                break;
            case PICExecMode.Extended:
                if (dataspace.ExtendedModeOnly is not null)
                {
                    foreach (var gpr in dataspace.ExtendedModeOnly)
                        yield return gpr.V1_Interface;
                }
                break;
            }
        }

        IEnumerable<ISFRRegister> IPICDescriptor.SFRs
        {
            get
            {
                foreach (var reg in dataspace.RegardlessOfMode.RegistersRegions.Where(r => r.MemorySubDomain == PICMemorySubDomain.SFR))
                {
                    if (reg is SFRDataSector sfrd)
                    {
                        foreach (var sfr in sfrd.SFRs)
                            yield return sfr.V1_Interface;
                        foreach (var jsfr in sfrd.JoinedSFRs)
                        {
                            byte bitPos = 0;
                            foreach (var sfr in jsfr.SFRs)
                            {
                                sfr.BitPos = bitPos;
                                yield return sfr.V1_Interface;
                                bitPos += (byte) (sfr.ByteWidth * 8);
                            }
                        }
                        foreach (var mfsr in sfrd.MuxedSFRs)
                        {
                            yield return mfsr.SelectSFRs.First().SFR.V1_Interface;
                        }
                    }
                }
                if (dataspace.RegardlessOfMode.NMMRPlace is not null && !IsPIC18)
                {
                    foreach (var sfr in dataspace.RegardlessOfMode.NMMRPlace.SFRDefs)
                        yield return sfr.V1_Interface;
                }
            }
        }

        IEnumerable<IJoinedRegister> IPICDescriptor.JoinedRegisters
        {
            get
            {
                foreach (var reg in dataspace.RegardlessOfMode.RegistersRegions.Where(r => r.MemorySubDomain == PICMemorySubDomain.SFR))
                {
                    if (reg is SFRDataSector sfrd)
                    {
                        foreach (var jsfr in sfrd.JoinedSFRs)
                        {
                            byte jsize = 0;
                            byte bitPos = 0;
                            foreach (var sfr in jsfr.SFRs)
                            {
                                jsize += sfr.NzWidth;
                                sfr.BitPos = bitPos;
                                bitPos += (byte) (sfr.ByteWidth * 8);
                            }
                            jsfr.NzWidth = jsize;
                            yield return jsfr.V1_Interface;
                        }
                    }
                }

            }
        }

        IEnumerable<IInterrupt> IPICDescriptor.Interrupts
        {
            get
            {
                if (pic.Interrupts is not null)
                {
                    foreach (var it in pic.Interrupts)
                        yield return it.V1_Interface;
                }
                yield break;
            }
        }

        private bool IsPIC18 => pic.Arch == "18xxxx";

        private InstructionSetID GetInstructionSetID
        {
            get
            {
                if (pic.InstructionSet is null)
                    return InstructionSetID.UNDEFINED;
                if (!mapInstrID.TryGetValue(pic.InstructionSet.ID, out InstructionSetID id))
                {
                    id = InstructionSetID.UNDEFINED;
                    if (pic.Arch == "16xxxx")
                        id = InstructionSetID.PIC16;
                    if (pic.Arch == "16Exxx")
                        id = InstructionSetID.PIC16_FULLFEATURED;
                    if (pic.Arch == "18xxxx")
                        id = (pic.HasExtendedMode ? InstructionSetID.PIC18_EXTENDED : InstructionSetID.PIC18);
                }
                return id;
            }
        }

    }

    internal sealed class PICMemTrait_v1_Interface : IPICMemTrait
    {
        private readonly PICMemTrait picmemtrt;

        public PICMemTrait_v1_Interface(PICMemTrait picMemTrait) 
            => picmemtrt = picMemTrait ?? throw new ArgumentNullException(nameof(picMemTrait));

        PICMemoryDomain IPICMemTrait.Domain => picmemtrt.Domain;
        PICMemorySubDomain IPICMemTrait.SubDomain => picmemtrt.SubDomain;
        uint IPICMemTrait.WordSize => picmemtrt.WordSize;
        uint IPICMemTrait.LocSize => picmemtrt.LocSize;
        uint IPICMemTrait.WordImpl => picmemtrt.WordImpl;
        uint IPICMemTrait.WordInit => picmemtrt.WordInit;
        uint IPICMemTrait.WordSafe => picmemtrt.WordSafe;
    }

    internal sealed class DeviceFuse_v1_Interface : IDeviceFuse
    {
        private readonly DCRDef fuse;

        public DeviceFuse_v1_Interface(DCRDef cfgfuse) 
            => fuse = cfgfuse;

        int IDeviceFuse.Addr => fuse.Addr;
        string IDeviceFuse.Name => fuse.CName;
        string IDeviceFuse.Description => fuse.Desc;
        int IDeviceFuse.BitWidth => fuse.NzWidth;
        int IDeviceFuse.ImplMask => fuse.ImplMask;
        string IDeviceFuse.AccessBits => fuse.Access;
        int IDeviceFuse.DefaultValue => fuse.DefaultValue;
        bool IDeviceFuse.IsLangHidden => fuse.IsLangHidden;

        IEnumerable<IDeviceFusesIllegal> IDeviceFuse.IllegalSettings
        {
            get
            {
                foreach (var ilg in fuse.Illegals)
                    yield return ilg.V1_Interface;
            }
        }

        IEnumerable<IDeviceFusesField> IDeviceFuse.ConfigFields
        {
            get
            {
                foreach (var mode in fuse.DCRModes)
                {
                    foreach (var fld in mode.Fields)
                    {
                        switch (fld)
                        {
                        case DCRFieldDef fdef:
                            yield return fdef.V1_Interface;
                            break;

                        case AdjustPoint adj:
                        default:
                            throw new InvalidOperationException($"Invalid PIC device configuration field in '{fuse.CName}' register: {fld.GetType()}");
                        }
                    }
                }
            }
        }

    }

    internal sealed class DeviceFusesIllegal_v1_Interface : IDeviceFusesIllegal
    {
        private readonly DCRDefIllegal illg;

        public DeviceFusesIllegal_v1_Interface(DCRDefIllegal dcrIllg) 
            => illg = dcrIllg ?? throw new ArgumentNullException(nameof(dcrIllg));

        string IDeviceFusesIllegal.When => illg.When;
        string IDeviceFusesIllegal.Description => illg.Desc;

    }

    internal sealed class DeviceFusesField_v1_Interface : IDeviceFusesField
    {
        private readonly DCRFieldDef dcrflddef;

        public DeviceFusesField_v1_Interface(DCRFieldDef dcrFieldDef) 
            => dcrflddef = dcrFieldDef ?? throw new ArgumentNullException(nameof(dcrFieldDef));

        string IRegisterBitField.Name => dcrflddef.CName;
        string IRegisterBitField.Description => dcrflddef.Desc;
        byte IRegisterBitField.BitPos => dcrflddef.BitPos;
        byte IRegisterBitField.BitWidth => dcrflddef.NzWidth;
        int IRegisterBitField.BitMask => dcrflddef.Mask;
        bool IRegisterBitField.IsHidden => dcrflddef.IsHidden;
        bool IRegisterBitField.IsLangHidden => dcrflddef.IsLangHidden;
        bool IRegisterBitField.IsIDEHidden => dcrflddef.IsIDEHidden;

        IEnumerable<IDeviceFusesSemantic> IDeviceFusesField.Semantics
        {
            get
            {
                if (dcrflddef.DCRFieldSemantics is not null)
                {
                    foreach (var fsem in dcrflddef.DCRFieldSemantics)
                        yield return fsem.V1_Interface;
                }
                yield break;
            }
        }

    }

    internal sealed class DeviceFusesSemantic_v1_Interface : IDeviceFusesSemantic
    {
        private readonly DCRFieldSemantic dcrfldsem;

        public DeviceFusesSemantic_v1_Interface(DCRFieldSemantic dcrFieldSem) 
            => dcrfldsem = dcrFieldSem ?? throw new ArgumentNullException(nameof(dcrFieldSem));

        string IDeviceFusesSemantic.Name => dcrfldsem.CName;
        string IDeviceFusesSemantic.Description => dcrfldsem.Descr;
        string IDeviceFusesSemantic.When => dcrfldsem.When;
        bool IDeviceFusesSemantic.IsHidden => dcrfldsem.IsHidden;
        bool IDeviceFusesSemantic.IsLangHidden => dcrfldsem.IsLangHidden;
    }

    internal sealed class SFRRegister_v1_Interface : ISFRRegister
    {
        private readonly SFRDef sfrreg;

        public SFRRegister_v1_Interface(SFRDef sfrDef)
            => sfrreg = sfrDef ?? throw new ArgumentNullException(nameof(sfrDef));

        uint IRegisterBasicInfo.Addr => sfrreg.Addr;
        string IRegisterBasicInfo.Name => sfrreg.CName;
        string IRegisterBasicInfo.Description => sfrreg.Desc;
        byte IRegisterBasicInfo.BitWidth => sfrreg.NzWidth;
        int ISFRRegister.ByteWidth => sfrreg.ByteWidth;
        uint ISFRRegister.ImplMask => sfrreg.Impl;
        string ISFRRegister.AccessBits => sfrreg.Access;
        string ISFRRegister.MCLR => sfrreg.MCLR;
        string ISFRRegister.POR => sfrreg.POR;
        bool ISFRRegister.IsIndirect => sfrreg.IsIndirect;
        bool ISFRRegister.IsVolatile => sfrreg.IsVolatile;
        bool ISFRRegister.IsHidden => sfrreg.IsHidden;
        bool ISFRRegister.IsLangHidden => sfrreg.IsLangHidden;
        bool ISFRRegister.IsIDEHidden => sfrreg.IsIDEHidden;
        string ISFRRegister.NMMRID => sfrreg.NMMRID;
        bool ISFRRegister.IsNMMR => sfrreg.IsNMMR;

        IEnumerable<ISFRBitField> ISFRRegister.BitFields
        {
            get
            {
                foreach (var smod in sfrreg.SFRModes)
                {
                    foreach (var bf in smod.Fields)
                    {
                        switch (bf)
                        {
                        case SFRFieldDef sfd:
                            yield return sfd.V1_Interface;
                            break;

                        case AdjustPoint adj:
                        default:
                            throw new InvalidOperationException($"Invalid SFR field type in '{sfrreg.CName}': {bf.GetType()}");
                        }
                    }
                }
            }
        }

    }

    internal sealed class JoinedSFRRegister_v1_Interface : IJoinedRegister
    {
        private readonly JoinedSFRDef jsfrreg;

        public JoinedSFRRegister_v1_Interface(JoinedSFRDef joinedSFRDef)
            => jsfrreg = joinedSFRDef;

        uint IRegisterBasicInfo.Addr => jsfrreg.Addr;
        string IRegisterBasicInfo.Name => jsfrreg.CName;
        string IRegisterBasicInfo.Description => jsfrreg.Desc;
        byte IRegisterBasicInfo.BitWidth => jsfrreg.NzWidth;

        IEnumerable<ISFRRegister> IJoinedRegister.ChildSFRs
        {
            get
            {
                foreach (var sfr in jsfrreg.SFRs)
                    yield return sfr.V1_Interface;
            }
        }

    }

    internal sealed class SFRBitField_v1_Interface : ISFRBitField
    {
        private readonly SFRFieldDef sfbitdef;

        public SFRBitField_v1_Interface(SFRFieldDef sfrFieldDef) 
            => sfbitdef = sfrFieldDef ?? throw new ArgumentNullException(nameof(sfrFieldDef));

        string IRegisterBitField.Name => sfbitdef.CName;
        string IRegisterBitField.Description => sfbitdef.Desc;
        byte IRegisterBitField.BitPos => sfbitdef.BitPos;
        byte IRegisterBitField.BitWidth => sfbitdef.NzWidth;
        int IRegisterBitField.BitMask => sfbitdef.Mask;
        bool IRegisterBitField.IsHidden => sfbitdef.IsHidden;
        bool IRegisterBitField.IsLangHidden => sfbitdef.IsLangHidden;
        bool IRegisterBitField.IsIDEHidden => sfbitdef.IsIDEHidden;

        IEnumerable<ISFRFieldSemantic> ISFRBitField.FieldSemantics
        {
            get
            {
                if (sfbitdef.SFRFieldSemantics is not null)
                {
                    foreach (var fsem in sfbitdef.SFRFieldSemantics)
                        yield return fsem.V1_Interface;
                }
            }
        }

    }

    internal sealed class SFRFieldSemantic_v1_Interface : ISFRFieldSemantic
    {
        private readonly SFRFieldSemantic fsem;

        public SFRFieldSemantic_v1_Interface(SFRFieldSemantic sfrFieldSemantic)
            => fsem = sfrFieldSemantic ?? throw new ArgumentNullException(nameof(sfrFieldSemantic));

        string ISFRFieldSemantic.Description => fsem.Desc;
        string ISFRFieldSemantic.When => fsem.When;

    }

    internal sealed class PICMemoryRegion_v1_Interface : IPICMemoryRegion
    {
        private readonly PICMemoryRegion memReg;

        public PICMemoryRegion_v1_Interface(PICMemoryRegion memRegion)
            => memReg = memRegion;

        string IPICMemoryRegion.RegionID => memReg.RegionID;
        bool IPICMemoryRegion.IsBank => memReg.IsBank;
        int IPICMemoryRegion.Bank => memReg.IsBank ? memReg.Bank : -1;
        string IPICMemoryRegion.ShadowIDRef => memReg.ShadowIDRef;
        int IPICMemoryRegion.ShadowOffset => memReg.ShadowOffset;
        bool IPICMemoryRegion.IsSection => memReg.IsSection;
        string IPICMemoryRegion.SectionDesc => memReg.IsSection ? memReg.SectionDesc : string.Empty;
        string IPICMemoryRegion.SectionName => memReg.IsSection ? memReg.SectionName : string.Empty;
        uint IPICMemoryAddrRange.BeginAddr => memReg.BeginAddr;
        uint IPICMemoryAddrRange.EndAddr => memReg.EndAddr;
        PICMemoryDomain IPICMemoryAddrRange.MemoryDomain => memReg.MemoryDomain;
        PICMemorySubDomain IPICMemoryAddrRange.MemorySubDomain => memReg.MemorySubDomain;
    }

    internal sealed class MirroringRegion_v1_Interface : IPICMirroringRegion
    {
        private readonly Mirror mir;

        public MirroringRegion_v1_Interface(Mirror mirror)
            => mir = mirror ?? throw new ArgumentNullException(nameof(mirror));

        uint IPICMirroringRegion.Addr
            => mir.Addr;
        uint IPICMirroringRegion.ByteSize
            => mir.NzSize;
        string IPICMirroringRegion.TargetRegionID 
            => mir.RegionIDRef;
    }

    internal sealed class DeviceInfoSector_v1_Interface : IDeviceInfoSector
    {
        private readonly DIASector diasect;
        private readonly DCISector dcisect;
        private readonly ProgMemoryRegion memreg;

        public DeviceInfoSector_v1_Interface(DIASector diaSector)
        {
            diasect = diaSector ?? throw new ArgumentNullException(nameof(diaSector));
            memreg = diasect;
            dcisect = null;
        }

        public DeviceInfoSector_v1_Interface(DCISector dciSector)
        {
            dcisect = dciSector ?? throw new ArgumentNullException(nameof(dciSector));
            memreg = dcisect;
            diasect = null;
        }

        string IPICMemoryRegion.RegionID => memreg.RegionID;
        bool IPICMemoryRegion.IsBank => memreg.IsBank;
        int IPICMemoryRegion.Bank => memreg.Bank;
        string IPICMemoryRegion.ShadowIDRef => memreg.ShadowIDRef;
        int IPICMemoryRegion.ShadowOffset => memreg.ShadowOffset;
        bool IPICMemoryRegion.IsSection => memreg.IsSection;
        string IPICMemoryRegion.SectionDesc => memreg.SectionDesc;
        string IPICMemoryRegion.SectionName => memreg.SectionName;
        uint IPICMemoryAddrRange.BeginAddr => memreg.BeginAddr;
        uint IPICMemoryAddrRange.EndAddr => memreg.EndAddr;
        PICMemoryDomain IPICMemoryAddrRange.MemoryDomain => memreg.MemoryDomain;
        PICMemorySubDomain IPICMemoryAddrRange.MemorySubDomain => memreg.MemorySubDomain;

        IEnumerable<IDeviceInfoRegister> IDeviceInfoSector.Registers
        {
            get
            {
                if (diasect is not null)
                {
                    foreach (var ra in diasect.RegisterArrays)
                    {
                        foreach (var r in ra.Registers)
                        {
                            switch (r)
                            {
                            case DeviceRegister dreg:
                                yield return dreg.V1_Interface;
                                break;

                            case AdjustPoint adj:
                            default:
                                throw new InvalidOperationException($"Invalid PIC device info register in '{ra.Name}' : {r.GetType()}");
                            }
                        }
                    }
                }
                else if (dcisect is not null)
                {
                    foreach (var r in dcisect.DCIRegisters)
                        yield return r.V1_Interface;
                }
                else
                    yield break;
            }
        }

    }

    internal sealed class DeviceInfo_v1_Interface : IDeviceInfoRegister
    {
        private readonly DeviceRegister dreg;

        public DeviceInfo_v1_Interface(DeviceRegister deviceRegister)
            => dreg = deviceRegister ?? throw new ArgumentNullException(nameof(deviceRegister));

        int IDeviceInfoRegister.Addr => dreg.Addr;
        string IDeviceInfoRegister.Name => dreg.Name;
        int IDeviceInfoRegister.BitWidth => dreg.NzWidth;

    }

    internal sealed class Interrupt_v1_Interface : IInterrupt
    {
        private readonly Interrupt inter;

        public Interrupt_v1_Interface(Interrupt interrupt) 
            => inter = interrupt ?? throw new ArgumentNullException(nameof(interrupt));

        uint IInterrupt.IRQ => inter.IRQ;
        string IInterrupt.Name => inter.CName;
        string IInterrupt.Description => inter.Desc;

    }

}
