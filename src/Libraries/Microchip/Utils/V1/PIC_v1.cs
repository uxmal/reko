#region License
/* 
 * Copyright (c) 2017-2020 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2020 John Källén.
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

// summary:	Implements the Microchip PIC XML definition serialization per Microchip Crownking Database.
// This is version 1.
//
namespace Reko.Libraries.Microchip.V1
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml.Serialization;

    /// <summary>
    /// PIC definition. (Version 1).
    /// To keep independancy between the design of this class and the consumers, all accesses to information contained in any instance
    /// of this class must be done thru the interface <see cref="IPICDescriptor"/> as provided by the property <see cref="PICDescriptorInterface"/>.
    /// </summary>
    /// 
    [Serializable(), XmlType(AnonymousType = true, Namespace = ""), XmlRoot(ElementName = "PIC", Namespace = "", IsNullable = false)]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class PIC_v1
    {
        #region XML Attributes

        /// <summary> Gets the version number of the PIC XML versus interface </summary>
        [XmlAttribute("picversion")]
        public int Version { get; set; } = 1;

        /// <summary> Gets the PIC name from the XML. </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary> Gets the PIC architecture name (16xxxx, 16Exxx, 18xxxx) from the XML. </summary>
        [XmlAttribute("arch")]
        public string Arch { get; set; }

        /// <summary> Gets the PIC description from the XML. </summary>
        [XmlAttribute("desc")]
        public string Desc { get; set; }

        /// <summary> Gets the unique processor identifier from the XML. Used by development tools. </summary>
        [XmlIgnore]
        public int ProcID { get; private set; }
        [XmlAttribute("procid")]
        public string ProcIDFormatted { get => $"0x{ProcID:X}"; set => ProcID = value.ToInt32Ex(); }

        /// <summary> Gets the data sheet identifier of the PIC from the XML. </summary>
        [XmlAttribute("dsid")]
        public string DsID { get; set; }

        [XmlAttribute("dosid")]
        public string DosID { get; set; }

        /// <summary>
        /// Gets the indicator whether this PIC supports the PIC18 extended execution mode.
        /// Overridden by the data space definition containing non-empty extended-mode-only memory space.
        /// </summary>
        [XmlAttribute("isextended", DataType = "boolean")]
        public bool HasExtendedMode { get => DataSpace?.ExtendedModeOnly?.Count > 0; set { } }

        /// <summary> Gets a value indicating whether this PIC supports freezing of peripherals from the XML. </summary>
        [XmlAttribute("hasfreeze", DataType = "boolean")]
        public bool HasFreeze { get; set; }

        /// <summary> Gets a value indicating whether this PIC supports debugging from the XML. </summary>
        [XmlAttribute("isdebuggable", DataType = "boolean")]
        public bool IsDebuggable { get; set; }

        [XmlAttribute("informedby")]
        public string Informedby { get; set; }

        [XmlAttribute("masksetid")]
        public string MaskSetID { get; set; }

        [XmlAttribute("psid")]
        public string PsID { get; set; }

        /// <summary> Gets the name of the PIC, this PIC is the clone of, from the XML. </summary>
        [XmlAttribute("clonedfrom")]
        public string ClonedFrom { get; set; }

        #endregion

        #region XML Elements

        /// <summary> Gets the architecture definition from the XML. </summary>
        [XmlElement("ArchDef", typeof(ArchDef))]
        public ArchDef ArchDef { get; set; }

        /// <summary> Gets the instruction set identifier from the XML. </summary>
        [XmlElement("InstructionSet", typeof(InstructionSet))]
        public InstructionSet InstructionSet { get; set; }

        /// <summary> Gets the list of interruption requests. </summary>
        [XmlArray("InterruptList")]
        [XmlArrayItem("Interrupt")]
        public List<Interrupt> Interrupts { get; set; }

        /// <summary> Gets the program memory space definitions from the XML. </summary>
        [XmlElement("ProgramSpace")]
        public ProgramSpace ProgramSpace { get; set; }

        /// <summary> Gets the data memory space definitions from the XML. </summary>
        [XmlElement("DataSpace")]
        public DataSpace DataSpace { get; set; }

        /// <summary> Gets the DMA memory space definitions from the XML. </summary>
        [XmlArray("DMASpace")]
        [XmlArrayItem("SFRDataSector")]
        public List<SFRDataSector> DMASpace { get; set; }

        /// <summary> Gets the DMA memory space definitions from the XML. </summary>
        [XmlElement("IndirectSpace")]
        public IndirectSpace IndirectSpace { get; set; }

        #endregion

        [XmlIgnore]
        public IPICDescriptor PICDescriptorInterface
            => picdescintf = picdescintf ?? new PIC_v1_Interface(this);

        private IPICDescriptor picdescintf = null;

        private string _debugDisplay => $"PIC (v1) '{Name}' ({Arch}) ";

    }

    #region ArchDef XML definition

    /// <summary> PIC memory architecture definition. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ArchDef
    {

        #region XML Attributes

        /// <summary> Gets the name (16xxxx, 16Exxx, 18xxxx) of the PIC architecture. </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary> Gets the description of the PIC architecture. </summary>
        [XmlAttribute("desc")]
        public string Description { get; set; }

        #endregion

        #region XML Elements

        /// <summary> Gets the memory traits of the PIC. </summary>
        [XmlElement("MemTraits")]
        public MemTraits MemTraits { get; set; }

        #endregion

    }

    /// <summary> The various memory regions' traits. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class MemTraits
    {

        #region XML Attributes

        /// <summary> Gets the depth of the hardware stack. </summary>
        [XmlIgnore]
        public int HWStackDepth { get; private set; }
        [XmlAttribute("hwstackdepth")]
        public string HwStackDepthFormatted { get => $"{HWStackDepth}"; set => HWStackDepth = value.ToInt32Ex(); }

        /// <summary> Gets the number of memory banks. </summary>
        [XmlIgnore]
        public int BankCount { get; private set; }
        [XmlAttribute("bankcount")]
        public string BankCountFormatted { get => $"{BankCount}"; set => BankCount = value.ToInt32Ex(); }

        #endregion

        #region XML Elements

        /// <summary> Gets the list of memory traits of the various memory regions. </summary>
        [XmlElement("CodeMemTraits", Type = typeof(CodeMemTraits))]
        [XmlElement("ExtCodeMemTraits", Type = typeof(ExtCodeMemTraits))]
        [XmlElement("CalDataMemTraits", Type = typeof(CalDataMemTraits))]
        [XmlElement("BackgroundDebugMemTraits", Type = typeof(BackgroundDebugMemTraits))]
        [XmlElement("TestMemTraits", Type = typeof(TestMemTraits))]
        [XmlElement("UserIDMemTraits", Type = typeof(UserIDMemTraits))]
        [XmlElement("ConfigFuseMemTraits", Type = typeof(ConfigFuseMemTraits))]
        [XmlElement("ConfigWORMMemTraits", Type = typeof(ConfigWORMMemTraits))]
        [XmlElement("DeviceIDMemTraits", Type = typeof(DeviceIDMemTraits))]
        [XmlElement("DataMemTraits", Type = typeof(DataMemTraits))]
        [XmlElement("EEDataMemTraits", Type = typeof(EEDataMemTraits))]
        public List<PICMemTrait> Traits { get; set; }

        #endregion

    }


    /// <summary> Memory trait (characteristics) for a given domain/subdomain. Must be inherited. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class PICMemTrait
    {
        public abstract PICMemoryDomain Domain { get; }
        public abstract PICMemorySubDomain SubDomain { get; }

        #region XML Attributes

        /// <summary> Gets the size of the memory word (in bytes). </summary>
        [XmlIgnore]
        public virtual uint WordSize { get; private set; }
        [XmlAttribute("wordsize")]
        public string WordSizeFormatted { get => $"0x{WordSize:X}"; set => WordSize = value.ToUInt32Ex(); }

        /// <summary> Gets the memory location access size (in bytes). </summary>
        [XmlIgnore]
        public virtual uint LocSize { get; private set; }
        [XmlAttribute("locsize")]
        public string LocSizeFormatted { get => $"0x{LocSize:X}"; set => LocSize = value.ToUInt32Ex(); }

        /// <summary> Gets the memory word implementation (bit mask). </summary>
        [XmlIgnore]
        public virtual uint WordImpl { get; private set; }
        [XmlAttribute("wordimpl")]
        public string WordImplFormatted { get => $"0x{WordImpl:X}"; set => WordImpl = value.ToUInt32Ex(); }

        /// <summary> Gets the initial (erased) memory word value. </summary>
        [XmlIgnore]
        public virtual uint WordInit { get; private set; }
        [XmlAttribute("wordinit")]
        public string WordInitFormatted { get => $"0x{WordInit:X}"; set => WordInit = value.ToUInt32Ex(); }

        /// <summary> Gets the memory word 'safe' value. </summary>
        [XmlIgnore]
        public virtual uint WordSafe { get; private set; }
        [XmlAttribute("wordsafe")]
        public string WordSafeFormatted { get => $"0x{WordSafe:X}"; set => WordSafe = value.ToUInt32Ex(); }

        #endregion

        [XmlIgnore]
        public IPICMemTrait V1_Interface
            => picmemtrtintf = picmemtrtintf ?? new PICMemTrait_v1_Interface(this);

        private IPICMemTrait picmemtrtintf = null;

    }

    /// <summary> Code memory traits. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CodeMemTraits : PICMemTrait
    {
        public override PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public override PICMemorySubDomain SubDomain => PICMemorySubDomain.Code;
    }

    /// <summary> External code memory traits. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ExtCodeMemTraits : PICMemTrait
    {
        public override PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public override PICMemorySubDomain SubDomain => PICMemorySubDomain.ExtCode;
    }

    /// <summary> Calibration data memory traits. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CalDataMemTraits : PICMemTrait
    {
        public override PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public override PICMemorySubDomain SubDomain => PICMemorySubDomain.Calib;
    }

    /// <summary> Background debug memory traits. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class BackgroundDebugMemTraits : PICMemTrait
    {
        public override PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public override PICMemorySubDomain SubDomain => PICMemorySubDomain.Debugger;
    }

    /// <summary> Test memory traits. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class TestMemTraits : PICMemTrait
    {
        public override PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public override PICMemorySubDomain SubDomain => PICMemorySubDomain.Test;
    }

    /// <summary> User IDs memory traits. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class UserIDMemTraits : PICMemTrait
    {
        public override PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public override PICMemorySubDomain SubDomain => PICMemorySubDomain.UserID;
    }

    /// <summary> Configuration fuses memory traits. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigFuseMemTraits : PICMemTrait
    {
        public override PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public override PICMemorySubDomain SubDomain => PICMemorySubDomain.DeviceConfig;

        [XmlIgnore]
        public int UnimplVal { get; private set; }
        [XmlAttribute("unimplval")]
        public string UnimplValFormatted { get => $"{UnimplVal}"; set => UnimplVal = value.ToInt32Ex(); }

    }

    /// <summary> Configuration Write-Once-Read-Many memory traits. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigWORMMemTraits : PICMemTrait
    {
        public override PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public override PICMemorySubDomain SubDomain => PICMemorySubDomain.Other;
    }

    /// <summary> Device IDs memory traits. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceIDMemTraits : PICMemTrait
    {
        public override PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public override PICMemorySubDomain SubDomain => PICMemorySubDomain.DeviceID;
    }

    /// <summary> EEPROM data memory traits. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class EEDataMemTraits : PICMemTrait
    {
        public override PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public override PICMemorySubDomain SubDomain => PICMemorySubDomain.EEData;

        [XmlIgnore]
        public override uint LocSize => 1;
        [XmlIgnore]
        public override uint WordSize => 1;
        [XmlIgnore]
        public override uint WordImpl => 0xFF;
        [XmlIgnore]
        public override uint WordInit => 0xFF;
        [XmlIgnore]
        public override uint WordSafe => 0xFF;

        /// <summary>
        /// Gets address magic offset in the binary image for EEPROM content.
        /// </summary>
        [XmlIgnore]
        public uint MagicOffset { get; private set; }
        [XmlAttribute("magicoffset")]
        public string MagicOffsetFormatted { get => $"0x{MagicOffset:X}"; set => MagicOffset = value.ToUInt32Ex(); }

    }

    /// <summary> Data memory traits. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataMemTraits : PICMemTrait
    {
        public override PICMemoryDomain Domain => PICMemoryDomain.Data;
        public override PICMemorySubDomain SubDomain => PICMemorySubDomain.Undef;
    }

    #endregion

    #region InstructionSet XML definition

    /// <summary> The instruction-set family ID as defined by Microchip. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class InstructionSet
    {
        /// <summary> Gets the instruction set ID. </summary>
        [XmlAttribute("instructionsetid")]
        public string ID { get; set; }

    }

    #endregion

    #region InterruptList XML definition

    /// <summary> Interrupt request. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class Interrupt
    {
        /// <summary> Gets the IRQ number. </summary>
        [XmlIgnore]
        public uint IRQ { get; private set; }

        /// <summary> Gets the name of the interrupt request.</summary>
        [XmlAttribute("cname")]
        public string CName { get; set; }

        /// <summary> Gets the description of the interrupt request. </summary>
        [XmlAttribute("desc")]
        public string Desc { get; set; }

        [XmlAttribute("irq")]
        public string IrqFormatted { get => $"{IRQ}"; set => IRQ = value.ToUInt32Ex(); }

        [XmlIgnore]
        public IInterrupt V1_Interface
            => itintf = itintf ?? new Interrupt_v1_Interface(this);

        private IInterrupt itintf;
    }

    #endregion

    #region Commonalities

    /// <summary> Adjust bit/byte address by the specified offset. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class AdjustPoint
    {
        /// <summary> Gets the relative byte offset to add for adjustment. </summary>
        [XmlIgnore]
        public int Offset { get; private set; }
        [XmlAttribute("offset")]
        public string OffsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); }

    }

    /// <summary> A PIC memory region with all possible attributes. Must be inherited. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class PICMemoryRegion : MemoryAddrRange
    {

        #region XML Attributes

        /// <summary> Gets the identifier of the region. </summary>
        [XmlAttribute("regionid")]
        public string RegionID { get; set; }

        [XmlIgnore]
        public virtual bool IsBank { get => false; set { } }

        [XmlIgnore]
        public int Bank { get; set; } = 0;
        [XmlAttribute("bank")]
        public string BankFormatted { get => $"{Bank}"; set => Bank = value.ToInt32Ex(); }
        [XmlIgnore]
        public bool BankFormattedSpecified => IsBank;

        [XmlIgnore]
        public virtual string ShadowIDRef { get => string.Empty; set { } }

        [XmlIgnore]
        public virtual int ShadowOffset { get; set; } = 0;

        [XmlIgnore]
        public virtual bool IsSection { get => false; set { } }

        /// <summary> Gets the textual description of the section. </summary>
        [XmlAttribute("sectiondesc")]
        public virtual string SectionDesc { get; set; } = string.Empty;
        [XmlIgnore]
        public bool SectionDescSpecified => IsSection;

        /// <summary> Gets the name of the section. </summary>
        [XmlAttribute("sectionname")]
        public virtual string SectionName { get; set; } = string.Empty;
        [XmlIgnore]
        public bool SectionNameSpecified => IsSection;

        #endregion

        [XmlIgnore]
        public IPICMemoryRegion V1_Interface
            => picmemregintf = picmemregintf ?? new PICMemoryRegion_v1_Interface(this);

        private IPICMemoryRegion picmemregintf;

    }

    #endregion

    #region ProgramSpace XML definition

    /// <summary> Program memory space. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgramSpace
    {

        #region XML Elements

        /// <summary> Gets the list of program memory sectors. </summary>
        [XmlElement("CodeSector", typeof(CodeSector))]
        [XmlElement("ExtCodeSector", typeof(ExtCodeSector))]
        [XmlElement("CalDataZone", typeof(CalDataZone))]
        [XmlElement("UserIDSector", typeof(UserIDSector))]
        [XmlElement("RevisionIDSector", typeof(RevisionIDSector))]
        [XmlElement("DeviceIDSector", typeof(DeviceIDSector))]
        [XmlElement("BACKBUGVectorSector", typeof(BACKBUGVectorSector))]
        [XmlElement("EEDataSector", typeof(EEDataSector))]
        public List<ProgMemoryRegion> CodeSectors { get; set; }

        [XmlElement("ConfigFuseSector")]
        public ConfigFuseSector FusesSector { get; set; }

        [XmlElement("DIASector", typeof(DIASector))]
        [XmlElement("DCISector", typeof(DCISector))]
        public List<ProgMemoryRegion> InfoSectors { get; set; }

        /// <summary> Gets the list of interrupt vector area. </summary>
        [XmlElement("VectorArea")]
        public List<VectorArea> VectorArea { get; set; }

        #endregion

    }

    /// <summary> A Program memory region. Must be inherited. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class ProgMemoryRegion : PICMemoryRegion
    {
        public override PICMemoryDomain MemoryDomain => PICMemoryDomain.Prog;

        /// <summary> Gets a value indicating whether this memory region is a section. </summary>
        [XmlIgnore]
        public override bool IsSection { get => false; set { } }

    }

    /// <summary> Program Memory region seen as a section. Must be inherited. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class ProgMemorySection : ProgMemoryRegion
    {
        /// <summary> Gets a value indicating whether this memory region is a section. </summary>
        [XmlAttribute(AttributeName = "issection", DataType = "boolean")]
        public override bool IsSection { get; set; }

    }

    /// <summary> Code memory sector. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CodeSector : ProgMemorySection
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Code;
    }

    /// <summary> Calibration data zone memory region. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CalDataZone : ProgMemorySection
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Calib;
    }

    /// <summary> Test zone memory region. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class TestZone : ProgMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Test;
    }

    /// <summary> User IDs memory sector. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class UserIDSector : ProgMemorySection
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.UserID;
    }

    /// <summary> Revision IDs sector. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class RevisionIDSector : ProgMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.RevisionID;

        /// <summary> Gets the device ID to silicon revision relationship. </summary>
        [XmlElement("DEVIDToRev")]
        public List<DEVIDToRev> DEVIDToRev { get; set; }

    }

    /// <summary> Device IDs sector. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceIDSector : ProgMemorySection
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DeviceID;

        /// <summary> Gets the bit mask to isolate device ID. </summary>
        [XmlIgnore]
        public int Mask { get; private set; }
        [XmlAttribute("mask")]
        public string MaskFormatted { get => $"0x{Mask:X}"; set => Mask = value.ToInt32Ex(); }

        /// <summary> Gets the generic value of device ID </summary>
        [XmlIgnore]
        public int Value { get; private set; }
        [XmlAttribute("value")]
        public string ValueFormatted { get => $"0x{Value:X}"; set => Value = value.ToInt32Ex(); }

        /// <summary> Gets the Device IDs to silicon revision level. </summary>
        [XmlElement("DEVIDToRev")]
        public List<DEVIDToRev> DEVIDToRev { get; set; }

    }

    /// <summary> Device ID to revision number. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DEVIDToRev
    {

        /// <summary> Gets the silicon revision name. </summary>
        [XmlAttribute("revlist")]
        public string RevList { get; set; }

        /// <summary> Gets the binary value of the silicon revision. </summary>
        [XmlIgnore]
        public int Value { get; private set; }
        [XmlAttribute("value")]
        public string ValueFormatted { get => $"0x{Value:X}"; set => Value = value.ToInt32Ex(); }

    }

    /// <summary> Configuration Fuses memory sector. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigFuseSector : ProgMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DeviceConfig;

        /// <summary> Gets the list of configuration registers definitions. </summary>
        [XmlElement("DCRDef", typeof(DCRDef))]
        [XmlElement("AdjustPoint", typeof(AdjustPoint))]
        public List<object> Defs { get; set; }

    }

    /// <summary> Device Configuration Register definition. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRDef
    {
        #region XML Attributes

        /// <summary> Gets the memory address of the configuration register. </summary>
        [XmlIgnore]
        public int Addr { get; private set; }
        [XmlAttribute("_addr")]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        /// <summary> Gets the name of the configuration register. </summary>
        [XmlAttribute("cname")]
        public string CName { get; set; }

        /// <summary> Gets the textual description of the configuration register. </summary>
        [XmlAttribute("desc")]
        public string Desc { get; set; }

        /// <summary> Gets the bit width of the register. </summary>
        [XmlIgnore]
        public int NzWidth { get; private set; }
        [XmlAttribute("nzwidth")]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToInt32Ex(); }

        /// <summary> Gets the implemented bit mask. </summary>
        [XmlIgnore]
        public int ImplMask { get; private set; }
        [XmlAttribute("impl")]
        public string ImplMaskFormatted { get => $"0x{ImplMask:X}"; set => ImplMask = value.ToInt32Ex(); }

        /// <summary> Gets the access modes of the register's bits. </summary>
        [XmlAttribute("access")]
        public string Access { get; set; }

        /// <summary> Gets the default value of the register. </summary>
        [XmlIgnore]
        public int DefaultValue { get; private set; }
        [XmlAttribute("default")]
        public string DefaultValueFormatted { get => $"0x{DefaultValue:X}"; set => DefaultValue = value.ToInt32Ex(); }

        /// <summary> Gets the factory default value of the register. </summary>
        [XmlIgnore]
        public int FactoryDefault { get; private set; }
        [XmlAttribute("factorydefault")]
        public string FactoryDefaultFormatted { get => $"0x{FactoryDefault:X}"; set => FactoryDefault = value.ToInt32Ex(); }

        /// <summary> Gets a value indicating whether this register is hidden to language tools. </summary>
        [XmlAttribute("islanghidden", DataType = "boolean")]
        public bool IsLangHidden { get; set; }
        [XmlIgnore]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary> Gets the unimplemented bitmask value. </summary>
        [XmlIgnore]
        public int UnimplVal { get; private set; }
        [XmlAttribute("unimplval")]
        public string UnimplValFormatted { get => $"{UnimplVal}"; set => UnimplVal = value.ToInt32Ex(); }

        [XmlIgnore]
        public int Unused { get; private set; }
        [XmlAttribute("unused")]
        public string UnusedFormatted { get => $"{Unused}"; set => Unused = value.ToInt32Ex(); }

        /// <summary> Gets the bit mask to use in checksum computation. </summary>
        [XmlIgnore]
        public int UseInChecksum { get; private set; }
        [XmlAttribute("useinchecksum")]
        public string UseInChecksumFormatted { get => $"0x{UseInChecksum:X}"; set => UseInChecksum = value.ToInt32Ex(); }

        #endregion

        #region XML Elements

        /// <summary> Gets the list of illegal configuration values (if any). </summary>
        [XmlElement("Illegal")]
        public List<DCRDefIllegal> Illegals { get; set; }

        /// <summary> Gets a list of Device Configuration Register modes. </summary>
        [XmlArray("DCRModeList")]
        [XmlArrayItem("DCRMode", typeof(DCRMode), IsNullable = false)]
        public List<DCRMode> DCRModes { get; set; }

        #endregion

        [XmlIgnore]
        public IDeviceFuse V1_Interface
            => devfuseintf = devfuseintf ?? new DeviceFuse_v1_Interface(this);

        private IDeviceFuse devfuseintf = null;

    }

    /// <summary> Device Configuration Register illegal definition. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRDefIllegal
    {
        /// <summary> Gets the "when" pattern of the illegal condition. </summary>
        [XmlAttribute("when")]
        public string When { get; set; }

        /// <summary> Gets the textual description of the illegal condition. </summary>
        [XmlAttribute("desc")]
        public string Desc { get; set; }

        [XmlIgnore]
        public IDeviceFusesIllegal V1_Interface
            => devfuseillgintf = devfuseillgintf ?? new DeviceFusesIllegal_v1_Interface(this);

        private IDeviceFusesIllegal devfuseillgintf = null;
    }

    /// <summary> Device Configuration Register mode. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRMode
    {
        /// <summary> Gets the identifier of the mode (usually "DS.0"). </summary>
        [XmlAttribute("id")]
        public string ID { get; set; }

        /// <summary> Gets the fields of the configuration register. </summary>
        [XmlElement("DCRFieldDef", typeof(DCRFieldDef))]
        [XmlElement("AdjustPoint", typeof(AdjustPoint))]
        public List<object> Fields { get; set; }

    }

    /// <summary> Device Configuration Register Field definition. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldDef
    {

        #region XML Attributes

        /// <summary> Gets the name of the field. </summary>
        [XmlAttribute("cname")]
        public string CName { get; set; }

        /// <summary> Gets the textual description of the field. </summary>
        [XmlAttribute("desc")]
        public string Desc { get; set; }

        /// <summary> Gets the bit position of the field. </summary>
        [XmlIgnore]
        public byte BitPos { get; private set; }
        [XmlAttribute("bitpos")]
        public string BitPosFormatted { get => $"{BitPos}"; set => BitPos = value.ToByteEx(); }

        /// <summary> Gets the bit width of the field. </summary>
        [XmlIgnore]
        public byte NzWidth { get; private set; }
        [XmlAttribute("nzwidth")]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToByteEx(); }

        /// <summary> Gets the bit mask of the field in the register. </summary>
        [XmlIgnore]
        public int Mask { get; private set; }
        [XmlAttribute("mask")]
        public string MaskFormatted { get => $"0x{Mask:X}"; set => Mask = value.ToInt32Ex(); }

        /// <summary> Gets a value indicating whether this configuration field is hidden. </summary>
        [XmlAttribute("ishidden", DataType = "boolean")]
        public bool IsHidden { get; set; }
        [XmlIgnore]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        /// <summary> Gets a value indicating whether this configuration field is hidden to language tools. </summary>
        [XmlAttribute("islanghidden", DataType = "boolean")]
        public bool IsLangHidden { get; set; }
        [XmlIgnore]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary> Gets a value indicating whether this configuration field is hidden to the MPLAB IDE. </summary>
        [XmlAttribute("isidehidden", DataType = "boolean")]
        public bool IsIDEHidden { get; set; }
        [XmlIgnore]
        public bool IsIDEHiddenSpecified { get => IsIDEHidden; set { } }

        #endregion

        #region XML Elements

        /// <summary> Gets the list of configuration field semantics for various configuration values. </summary>
        [XmlElement("DCRFieldSemantic")]
        public List<DCRFieldSemantic> DCRFieldSemantics { get; set; }

        #endregion

        public IDeviceFusesField V1_Interface
            => devfusesfldintf = devfusesfldintf ?? new DeviceFusesField_v1_Interface(this);

        private IDeviceFusesField devfusesfldintf = null;

    }

    /// <summary> Device Configuration Register field pattern semantic. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldSemantic 
    {

        #region XML Attributes

        /// <summary> Gets the name of the field. </summary>
        [XmlAttribute("cname")]
        public string CName { get; set; }

        /// <summary> Gets the textual description of the field value. </summary>
        [XmlAttribute("desc")]
        public string Descr { get; set; }

        /// <summary> Gets the when condition for the field value. </summary>
        [XmlAttribute("when")]
        public string When { get; set; }

        /// <summary> Gets a value indicating whether this configuration pattern is hidden. </summary>
        [XmlAttribute("ishidden", DataType = "boolean")]
        public bool IsHidden { get; set; }
        [XmlIgnore]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        /// <summary> Gets or sets a value indicating whether this configuration pattern is hidden to language tools. </summary>
        [XmlAttribute("islanghidden", DataType = "boolean")]
        public bool IsLangHidden { get; set; }
        [XmlIgnore]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary>Gets the oscillator mode identifier reference. </summary>
        [XmlIgnore]
        public int OscModeIDRef { get; private set; }
        [XmlAttribute("oscmodeidref")]
        public string OscModeIDRefFormatted { get => $"{OscModeIDRef}"; set => OscModeIDRef = value.ToInt32Ex(); }
        [XmlIgnore]
        public bool OscModeIDRefFormattedSpecified { get => OscModeIDRef != 0; set { } }

        [XmlAttribute("_defeatcoercion")]
        public string DefeatCoercion { get; set; }

        /// <summary> Gets the memory mode identifier reference. </summary>
        [XmlAttribute("memmodeidref")]
        public string MemModeIDRef { get; set; }

        #endregion

        public IDeviceFusesSemantic V1_Interface
            => devfusessemintf = devfusessemintf ?? new DeviceFusesSemantic_v1_Interface(this);

        private IDeviceFusesSemantic devfusessemintf = null;

    }

    /// <summary> Background debugger vector memory sector. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class BACKBUGVectorSector : ProgMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Debugger;
    }

    /// <summary> Data EEPROM memory sector. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class EEDataSector : ProgMemorySection
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.EEData;
    }

    /// <summary> Device Information Area memory region. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DIASector : ProgMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DeviceInfoAry;

        /// <summary> Gets the list of register arrays. </summary>
        [XmlElement("RegisterArray")]
        public List<DIARegisterArray> RegisterArrays { get; set; }

        [XmlIgnore]
        public new IDeviceInfoSector V1_Interface
            => devinfintf = devinfintf ?? new DeviceInfoSector_v1_Interface(this);

        private IDeviceInfoSector devinfintf;

    }

    /// <summary> Device Information Area register array. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DIARegisterArray
    {

        /// <summary> Gets the starting memory address of the DIA registers. </summary>
        [XmlIgnore]
        public int Addr { get; private set; }
        [XmlAttribute("_addr")]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        /// <summary> Gets the name of the DIA registers array. </summary>
        [XmlAttribute("cname")]
        public string Name { get; set; }

        [XmlElement("Register", typeof(DeviceRegister))]
        [XmlElement("AdjustPoint", typeof(AdjustPoint))]
        public List<object> Registers { get; set; }


    }

    /// <summary> Device Configuration Information memory region. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCISector : ProgMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DeviceConfigInfo;

        /// <summary> Gets the list of configuration information registers. </summary>
        [XmlElement("Register")]
        public List<DeviceRegister> DCIRegisters { get; set; }

        [XmlIgnore]
        public new IDeviceInfoSector V1_Interface
            => devinfintf = devinfintf ?? new DeviceInfoSector_v1_Interface(this);

        private IDeviceInfoSector devinfintf;

    }

    /// <summary> Device Information Area (DIA) register. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceRegister
    {

        #region XML Attributes

        /// <summary> Gets the address of the register. </summary>
        [XmlIgnore]
        public int Addr { get; private set; }
        [XmlAttribute("_addr")]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        /// <summary> Gets the name of the register. </summary>
        [XmlAttribute("cname")]
        public string Name { get; set; }

        /// <summary> Gets the bit width of the register. </summary>
        [XmlIgnore]
        public int NzWidth { get; private set; }
        [XmlAttribute("nzwidth")]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToInt32Ex(); }

        #endregion

        [XmlIgnore]
        public IDeviceInfoRegister V1_Interface
            => devinfintf = devinfintf ?? new DeviceInfo_v1_Interface(this);

        private IDeviceInfoRegister devinfintf;
    }

    /// <summary> External Code memory region. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ExtCodeSector : ProgMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.ExtCode;
    }

    /// <summary> Interrupt Vector area. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class VectorArea
    {

        /// <summary> Gets the identifier of the vector area. </summary>
        [XmlAttribute("regionid")]
        public string RegionID { get; set; }

        /// <summary> Gets the bytes size of the area. </summary>
        [XmlIgnore]
        public int Size { get; private set; }
        [XmlAttribute("nzsize")]
        public string NzSizeformatted { get => $"0x{Size:X}"; set => Size = value.ToInt32Ex(); }

    }

    #endregion

    #region DataSpace XML element

    /// <summary> Data memory space. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class DataSpace
    {
        /// <summary> Gets the highest (end) address (+1) of the data memory space. </summary>
        [XmlIgnore] public uint EndAddr { get; private set; }
        [XmlAttribute("endaddr")]
        public string EndAddrFormatted { get => $"0x{EndAddr:X}"; set => EndAddr = value.ToUInt32Ex(); }

        /// <summary> Gets the data memory regions regardless of PIC execution mode. </summary>
        [XmlElement("RegardlessOfMode")]
        public RegardlessOfMode RegardlessOfMode { get; set; }

        /// <summary> Gets the list of GPR data memory regions when PIC is in traditional execution mode. </summary>
        [XmlArray("TraditionalModeOnly")]
        [XmlArrayItem("GPRDataSector", typeof(GPRDataSector), IsNullable = false)]
        public List<GPRDataSector> TraditionalModeOnly { get; set; }

        /// <summary> Gets the list of GPR data memory regions when PIC is in extended execution mode. </summary>
        [XmlArray("ExtendedModeOnly")]
        [XmlArrayItem("GPRDataSector", typeof(GPRDataSector), IsNullable = false)]
        public List<GPRDataSector> ExtendedModeOnly { get; set; }


        private string _debugDisplay
            => $"Data Space [0x0-0x{EndAddr:X}]";

    }

    /// <summary> Data memory regions regardless of PIC execution mode. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class RegardlessOfMode
    {
        /// <summary> Gets the list of data memory regions. </summary>
        [XmlElement("DPRDataSector", typeof(DPRDataSector))]
        [XmlElement("GPRDataSector", typeof(GPRDataSector))]
        [XmlElement("SFRDataSector", typeof(SFRDataSector))]
        public List<DataMemoryBankedRegion> RegistersRegions { get; set; }

        [XmlElement("NMMRPlace")]
        public NMMRPlace NMMRPlace { get; set; }

    }

    /// <summary> Dual Port Registers data memory sector. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class DPRDataSector : DataMemoryBankedRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DPR;

        /// <summary> Gets the list of SFRs as Dual Port registers. </summary>
        [XmlElement("SFRDef", typeof(SFRDef))]
        [XmlElement("AdjustPoint", typeof(AdjustPoint))]
        public List<object> SFRs { get; set; }

        private string _debugDisplay
            => $"DPR sector '{RegionID}' bank{Bank}[{BeginAddr:X3}-{EndAddr:X3}]";

    }

    /// <summary> General Purpose Registers (GPR) data memory region. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class GPRDataSector : DataMemoryBankedRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.GPR;

        private string _debugDisplay
            => $"GPR sector '{RegionID}' bank{Bank}[{BeginAddr:X3}-{EndAddr:X3}]";

    }

    /// <summary> Non-Memory-Mapped-Register (NMMR) definitions. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class NMMRPlace
    {
        public PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.NNMR;

        /// <summary> Gets the identifier of the NMMR region. </summary>
        [XmlAttribute("regionid")]
        public string RegionID { get; set; }

        /// <summary> Gets the list of SFR definitions. </summary>
        [XmlElement("SFRDef")]
        public List<SFRDef> SFRDefs { get; set; }

        private string _debugDisplay
            => $"NMMR '{RegionID}'";

    }

    /// <summary> Special Function Registers data memory region. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRDataSector : DataMemoryBankedRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.SFR;

        #region XML Elements

        /// <summary> Gets the list of SFRs. </summary>
        [XmlElement("SFRDef")]
        public List<SFRDef> SFRs { get; set; }

        /// <summary> Gets the list of joined SFRs. </summary>
        [XmlElement("JoinedSFRDef")]
        public List<JoinedSFRDef> JoinedSFRs { get; set; }

        /// <summary> Gets the list of multiplexed SFRs. </summary>
        [XmlElement("MuxedSFRDef")]
        public List<MuxedSFRDef> MuxedSFRs { get; set; }

        /// <summary> Gets the list of mirrored SFRs. </summary>
        [XmlElement("Mirror")]
        public List<Mirror> MirrorSFRs { get; set; }

        /// <summary> Gets the list of DMA mirrored SFRs. </summary>
        [XmlElement("RegisterMirror")]
        public List<DMARegisterMirror> DMARegisters { get; set; }

        #endregion

        private string _debugDisplay
            => $"SFR sector '{RegionID}' bank{Bank}[{BeginAddr:X3}-{EndAddr:X3}]";

    }

    /// <summary> Special Function Register (SFR) definition. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRDef
    {

        #region XML Attributes

        /// <summary> Gets the data memory address of this SFR. </summary>
        [XmlIgnore]
        public uint Addr { get; private set; }
        [XmlAttribute("_addr")]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); }

        /// <summary> Gets the name of this SFR. </summary>
        [XmlAttribute("cname")]
        public string CName { get; set; }

        /// <summary> Gets the textual description of this SFR. </summary>
        [XmlAttribute("desc")]
        public string Desc { get; set; }

        /// <summary> Gets the bit width of this SFR. </summary>
        [XmlIgnore]
        public byte NzWidth { get; private set; }
        [XmlAttribute("nzwidth")]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToByteEx(); }

        /// <summary> Gets the bit position of this SFR when under a joined SFR. </summary>
        [XmlIgnore]
        public byte BitPos { get; set; } = 0;
        [XmlAttribute("bitpos")]
        public string BitPosFormatted { get => $"{BitPos}"; set => BitPos = value.ToByteEx(); }

        /// <summary> Gets the byte width of this SFR. </summary>
        [XmlIgnore]
        public int ByteWidth => (NzWidth + 7) >> 3;

        /// <summary> Gets the implemented bits mask of this SFR. </summary>
        [XmlIgnore]
        public uint Impl { get; private set; }
        [XmlAttribute("impl")]
        public string ImplFormatted { get => $"0x{Impl:X}"; set => Impl = value.ToUInt32Ex(); }

        /// <summary> Gets the access mode bits descriptor for this SFR. </summary>
        [XmlAttribute("access")]
        public string Access { get; set; }

        /// <summary> Gets the Master Clear (MCLR) bits values (string) of this SFR. </summary>
        [XmlAttribute("mclr")]
        public string MCLR { get; set; }

        /// <summary> Gets the Power-ON Reset bits values (string) of this SFR. </summary>
        [XmlAttribute("por")]
        public string POR { get; set; }

        /// <summary> Gets a value indicating whether this SFR is indirect. </summary>
        [XmlAttribute("isindirect", DataType = "boolean")]
        public bool IsIndirect { get; set; }
        [XmlIgnore]
        public bool IsIndirectSpecified { get => IsIndirect; set { } }

        /// <summary> Gets a value indicating whether this SFR is volatile. </summary>
        [XmlAttribute("isvolatile", DataType = "boolean")]
        public bool IsVolatile { get; set; }
        [XmlIgnore]
        public bool IsVolatileSpecified { get => IsVolatile; set { } }

        /// <summary> Gets a value indicating whether this SFR is hidden. </summary>
        [XmlAttribute("ishidden", DataType = "boolean")]
        public bool IsHidden { get; set; }
        [XmlIgnore]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        /// <summary> Gets a value indicating whether this SFR is hidden to language tools. </summary>
        [XmlAttribute("islanghidden", DataType = "boolean")]
        public bool IsLangHidden { get; set; }
        [XmlIgnore]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary> Gets a value indicating whether this SFR is hidden to MPLAB IDE. </summary>
        [XmlAttribute("isidehidden", DataType = "boolean")]
        public bool IsIDEHidden { get; set; }
        [XmlIgnore]
        public bool IsIDEHiddenSpecified { get => IsIDEHidden; set { } }

        /// <summary> Gets the name of the peripheral this SFR is the base address of. </summary>
        [XmlAttribute("baseofperipheral")]
        public string BaseOfPeripheral { get; set; }

        /// <summary> Gets the Non-Memory-Mapped-Register identifier of the SFR. </summary>
        [XmlAttribute("nmmrid")]
        public string NMMRID { get; set; }
        /// <summary> Gets a value indicating whether this SFR is Non-Memory-Mapped. </summary>
        [XmlIgnore]
        public bool IsNMMR { get => !string.IsNullOrEmpty(NMMRID); set { } }

        #endregion

        #region XML Elements

        /// <summary> Gets the list of modes for this SFR. </summary>
        [XmlArray("SFRModeList")]
        [XmlArrayItem("SFRMode")]
        public List<SFRMode> SFRModes { get; set; }

        #endregion

        [XmlIgnore]
        public ISFRRegister V1_Interface
            => sfrregintf = sfrregintf ?? new SFRRegister_v1_Interface(this);

        private ISFRRegister sfrregintf = null;

        private string _debugDisplay
            => $"SFR '{CName}' @{(IsNMMR ? $"NMMRID({NMMRID})" : $"0x{Addr:X}")}";
    }

    /// <summary> SFR Fields definitions for a given mode. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("SFRMode='{ID}'")]
    public sealed class SFRMode
    {
        /// <summary> Gets the identifier of the mode (usually "DS.0"). </summary>
        [XmlAttribute("id")]
        public string ID { get; set; }

        /// <summary> Gets the Power-ON Reset value of the mode. Not used. </summary>
        [XmlAttribute("por")]
        public string POR { get; set; }

        /// <summary> Gets the list of SFR fields definitions. </summary>
        [XmlElement("SFRFieldDef", typeof(SFRFieldDef))]
        [XmlElement("AdjustPoint", typeof(AdjustPoint))]
        public List<object> Fields { get; set; }

    }

    /// <summary> SFR bits-field definition. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("SFRBitField={Name} @{BitPos}[{BitWidth}]")]
    public sealed class SFRFieldDef
    {

        #region XML Attributes

        /// <summary> Gets the name of this SFR Field. </summary>
        [XmlAttribute("cname")]
        public string CName { get; set; }

        /// <summary> Gets the bit width of this SFR field. </summary>
        [XmlIgnore]
        public byte NzWidth { get; set; }
        [XmlAttribute("nzwidth")]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToByteEx(); }

        /// <summary> Gets the bit position/address (zero-based) of this SFR field. </summary>
        [XmlIgnore]
        public byte BitPos { get; set; }
        [XmlAttribute("bitpos")]
        public string BitPosFormatted { get => $"{BitPos}"; set => BitPos = value.ToByteEx(); }

        /// <summary> Gets the bit mask of this SFR field. </summary>
        [XmlIgnore]
        public int Mask { get; set; }
        [XmlAttribute("mask")]
        public string MaskFormatted { get => $"0x{Mask:X}"; set => Mask = value.ToInt32Ex(); }

        /// <summary> Gets the textual description of this SFR Field. </summary>
        [XmlAttribute("desc")]
        public string Desc { get; set; }

        /// <summary> Gets a value indicating whether this SFR Field is hidden to language tools. </summary>
        [XmlAttribute("islanghidden", DataType = "boolean")]
        public bool IsLangHidden { get; set; }
        [XmlIgnore]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        /// <summary> Gets a value indicating whether this SFR Field is hidden. </summary>
        [XmlAttribute("ishidden", DataType = "boolean")]
        public bool IsHidden { get; set; }
        [XmlIgnore]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        /// <summary> Gets a value indicating whether this SFR Field is hidden to MPLAB IDE. </summary>
        [XmlAttribute("isidehidden", DataType = "boolean")]
        public bool IsIDEHidden { get; set; }
        [XmlIgnore]
        public bool IsIDEHiddenSpecified { get => IsIDEHidden; set { } }

        #endregion

        #region XML elements

        /// <summary> Gets the list of semantics of this SFR field. </summary>
        [XmlElement("SFRFieldSemantic")]
        public List<SFRFieldSemantic> SFRFieldSemantics { get; set; }

        #endregion

        [XmlIgnore]
        public ISFRBitField V1_Interface
            => sfrregintf = sfrregintf ?? new SFRBitField_v1_Interface(this);

        private ISFRBitField sfrregintf = null;

    }

    /// <summary> SFR Field semantic. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRFieldSemantic
    {
        /// <summary> Gets the textual description of the semantic.</summary>
        [XmlAttribute("desc")]
        public string Desc { get; set; }

        /// <summary> Gets the "when" condition of the semantic. </summary>
        [XmlAttribute("when")]
        public string When { get; set; }

        [XmlIgnore]
        public ISFRFieldSemantic V1_Interface
            => sfrregintf = sfrregintf ?? new SFRFieldSemantic_v1_Interface(this);

        private ISFRFieldSemantic sfrregintf = null;

    }

    /// <summary> Mirrored registers area. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class Mirror
    {
        /// <summary> Gets the memory address of the mirror. </summary>
        [XmlIgnore]
        public uint Addr { get; private set; }
        [XmlAttribute("_addr")]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); }

        /// <summary> Gets the size in bytes of the mirrored area. </summary>
        [XmlIgnore]
        public uint NzSize { get; private set; }
        [XmlAttribute("nzsize")]
        public string NzSizeFormatted { get => $"0x{NzSize:X}"; set => NzSize = value.ToUInt32Ex(); }

        /// <summary> Gets the region identifier reference of the mirrored memory region. </summary>
        [XmlAttribute("regionidref")]
        public string RegionIDRef { get; set; }

        [XmlIgnore]
        public IPICMirroringRegion V1_Interface
            => mirintf = mirintf ?? new MirroringRegion_v1_Interface(this);

        private IPICMirroringRegion mirintf;

        private string _debugDisplay
            => $"Mirror '{RegionIDRef}' @0x{Addr:X}[{NzSize}]";
    }

    /// <summary> Joined SFR (e.g. FSR2 register composed of FSR2H:FSR2L registers). </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class JoinedSFRDef
    {

        #region XML Attributes

        /// <summary> Gets the name of the joined SFR. </summary>
        [XmlAttribute("cname")]
        public string CName { get; set; }

        /// <summary> Gets the memory address of the joined SFRs. </summary>
        [XmlIgnore]
        public uint Addr { get; private set; }
        [XmlAttribute("_addr")]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); }

        /// <summary> Gets the bit width of the joined SFR. </summary>
        [XmlIgnore]
        public byte NzWidth { get; set; }
        [XmlAttribute("nzwidth")]
        public string NzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToByteEx(); }

        /// <summary> Gets the textual description of the joined SFR. </summary>
        [XmlAttribute("desc")]
        public string Desc { get; set; }

        #endregion

        #region XML Elements

        /// <summary> Gets the list of adjacent SFRs composing the join. </summary>
        [XmlElement("SFRDef")]
        public List<SFRDef> SFRs { get; set; }

        #endregion

        [XmlIgnore]
        public IJoinedRegister V1_Interface
        => joinsfrintf = joinsfrintf ?? new JoinedSFRRegister_v1_Interface(this);

        private IJoinedRegister joinsfrintf = null;

        private string _debugDisplay
            => $"Joined SFR '{CName}' @0x{Addr:X}[{NzWidth}]";
    }

    /// <summary> Multiplexed SFRs definition. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class MuxedSFRDef
    {
        /// <summary> Gets the memory address of the multiplexed SFRs. </summary>
        [XmlIgnore]
        public int Addr { get; private set; }
        [XmlAttribute("_addr")]
        public string AddrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        /// <summary> Gets the bit width of the multiplex. </summary>
        [XmlIgnore]
        public byte BitWidth { get; private set; }
        [XmlAttribute("nzwidth")]
        public string NzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToByteEx(); }

        /// <summary> Gets the byte width of the multiplex. </summary>
        [XmlIgnore]
        public int ByteWidth => (BitWidth + 7) >> 3;

        /// <summary> Gets the list of selections of SFRs. </summary>
        [XmlElement("SelectSFR")]
        public List<SelectSFR> SelectSFRs { get; set; }

        private string _debugDisplay
            => $"Muxed SFR @0x{Addr:X}[{BitWidth}]";

    }

    /// <summary> Selection of a Muxed SFR. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SelectSFR
    {
        /// <summary> Gets the (optional) "when" condition for selection. </summary>
        [XmlAttribute("when")]
        public string When { get; set; }

        /// <summary> Gets the SFR being selected. </summary>
        [XmlElement("SFRDef")]
        public SFRDef SFR { get; set; }

        private string _debugDisplay
            => $"Select '{SFR.CName}' when '{When}'";

    }

    /// <summary> A data memory banked region. Must be inherited. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class DataMemoryBankedRegion : DataMemoryRegion
    {
        [XmlIgnore]
        public override bool IsBank => true;

        /// <summary> Gets the shadow identifier reference, if any. </summary>
        [XmlAttribute("shadowidref")]
        public override string ShadowIDRef { get; set; }

        /// <summary> Gets the shadow identifier reference, if any. </summary>
        [XmlAttribute("shadowoffset")]
        public string ShadowOffsetFormatted { get => $"0x{ShadowOffset}"; set => ShadowOffset = value.ToInt32Ex(); }

    }

    /// <summary> A Data memory region. Must be inherited. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class DataMemoryRegion : PICMemoryRegion
    {
        public override PICMemoryDomain MemoryDomain => PICMemoryDomain.Data;
    }

    #endregion

    #region DMASpace XML definition

    /// <summary> DMA Register mirror. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DMARegisterMirror
    {
        /// <summary> Gets the name reference. </summary>
        [XmlAttribute("cnameref")]
        public string CNameRef { get; set; }

        /// <summary> Gets the name suffix. </summary>
        [XmlAttribute("cnamesuffix")]
        public string CNameSuffix { get; set; }

    }

    #endregion

    #region IndirectSpace XML definition

    /// <summary>
    /// Indirect space definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class IndirectSpace
    {

        #region XML Elements

        [XmlElement("LinearDataSector", typeof(LinearDataSector), IsNullable = false)]
        public List<LinearDataSector> LinearSector { get; set; }

        #endregion

    }

    /// <summary> Linear data memory region. </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class LinearDataSector : MemoryAddrRange
    {
        public override PICMemoryDomain MemoryDomain => PICMemoryDomain.Data;
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Linear;

        /// <summary> Gets the bytes size of the linear memory bank. </summary>
        [XmlIgnore]
        public int BankSize { get; private set; }
        [XmlAttribute("banksize")]
        public string BankSizeFormatted { get => $"0x{BankSize:X}"; set => BankSize = value.ToInt32Ex(); }

        /// <summary> Gets the beginning address of the linear memory bank. </summary>
        [XmlIgnore]
        public uint BlockBeginAddr { get; private set; }
        [XmlAttribute("blockbeginaddr")]
        public string BlockBeginAddrFormatted { get => $"0x{BlockBeginAddr:X}"; set => BlockBeginAddr = value.ToUInt32Ex(); }

        /// <summary> Gets the ending address (+1) of the linear memory bank. </summary>
        [XmlIgnore]
        public uint BlockEndAddr { get; private set; }
        [XmlAttribute("blockendaddr")]
        public string BlockEndAddrFormatted { get => $"0x{BlockEndAddr:X}"; set => BlockEndAddr = value.ToUInt32Ex(); }

        private string _debugDisplay
            => $"Linear Data sector [0x{BlockBeginAddr:X}-0x{BlockEndAddr:X}]";

    }

    #endregion

}


