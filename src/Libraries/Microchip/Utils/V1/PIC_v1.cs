#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
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
using System.Diagnostics;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

// summary:	Implements the Microchip PIC XML definition serialization per Microchip Crownking Database.
// This is version 1.
//
namespace Reko.Libraries.Microchip.V1
{
    /// <summary>
    /// PIC definition. (Version 1)
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = ""), XmlRoot(ElementName = "PIC", Namespace = "", IsNullable = false)]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class PIC_v1
    {
        /// <summary> Gets the version number of the interface </summary>
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

        /// <summary> Gets the architecture definition from the XML. </summary>
        [XmlElement("ArchDef", typeof(ArchDef))]
        public ArchDef ArchDef { get; set; }

        /// <summary> Gets the instruction set identifier from the XML. </summary>
        [XmlElement("InstructionSet", typeof(InstructionSet))]
        public InstructionSet InstructionSet { get; set; }

        /// <summary> Gets the unique processor identifier from the XML. Used by development tools. </summary>
        [XmlIgnore]
        public int ProcID { get; private set; }

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

        /// <summary> Gets a list of interrupts (IRQ) from the XML. </summary>
        [XmlArray("InterruptList")]
        [XmlArrayItem("Interrupt", typeof(Interrupt), IsNullable = false)]
        public List<Interrupt> Interrupts { get; set; }

        /// <summary> Gets the program memory space definitions from the XML. </summary>
        [XmlElement("ProgramSpace")]
        public ProgramSpace ProgramSpace { get; set; }

        /// <summary> Gets the data memory space definitions from the XML. </summary>
        [XmlElement("DataSpace")]
        public DataSpace DataSpace { get; set; }

        /// <summary> Gets the list of SFRs in the DMA space from the XML. </summary>
        [XmlArray("DMASpace")]
        [XmlArrayItem("SFRDataSector", typeof(SFRDataSector), IsNullable = false)]
        public List<SFRDataSector> DMASpace { get; set; }

        /// <summary> Gets the list of linear data memory regions in the indirect space from the XML. </summary>
        [XmlArray("IndirectSpace")]
        [XmlArrayItem("LinearDataSector", typeof(LinearDataSector), IsNullable = false)]
        public List<LinearDataSector> IndirectSpace { get; set; }

        [XmlAttribute("procid")]
        public string _procIDFormatted { get => $"0x{ProcID:X}"; set => ProcID = value.ToInt32Ex(); }

        [XmlIgnore]
        public IPICDescriptor PICDescriptorInterface
            => picdescintf = picdescintf ?? new PIC_v1_Interface(this);

        private IPICDescriptor picdescintf = null;

        private string _debugDisplay => $"PIC (v1) '{Name}' ({Arch}) ";

    }


    #region ArchDef XML element

    /// <summary>
    /// Device ID to revision number.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DEVIDToRev
    {
        public DEVIDToRev() { }

        /// <summary> Gets the silicon revision. </summary>
        [XmlAttribute("revlist")]
        public string RevList { get; set; }

        /// <summary> Gets the binary value of the silicon revision. </summary>
        [XmlIgnore]
        public int Value { get; private set; }

        [XmlAttribute("value")]
        public string _valueFormatted { get => $"0x{Value:X}"; set => Value = value.ToInt32Ex(); }

    }

    /// <summary>
    /// Memory trait (characteristics). Must be inherited.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class MemTrait : IMemTrait
    {

        /// <summary> Gets the size of the memory word (in bytes). </summary>
        [XmlIgnore]
        public virtual uint WordSize { get; private set; }

        /// <summary> Gets the memory location access size (in bytes). </summary>
        [XmlIgnore]
        public virtual uint LocSize { get; private set; }

        /// <summary> Gets the memory word implementation (bit mask). </summary>
        [XmlIgnore]
        public virtual uint WordImpl { get; private set; }

        /// <summary> Gets the initial (erased) memory word value. </summary>
        [XmlIgnore]
        public virtual uint WordInit { get; private set; }

        /// <summary> Gets the memory word 'safe' value. </summary>
        [XmlIgnore]
        public virtual uint WordSafe { get; private set; }


        [XmlAttribute("locsize")]
        public string _locSizeFormatted { get => $"0x{LocSize:X}"; set => LocSize = value.ToUInt32Ex(); }

        [XmlAttribute("wordimpl")]
        public string _wordImplFormatted { get => $"0x{WordImpl:X}"; set => WordImpl = value.ToUInt32Ex(); }

        [XmlAttribute("wordinit")]
        public string _wordInitFormatted { get => $"0x{WordInit:X}"; set => WordInit = value.ToUInt32Ex(); }

        [XmlAttribute("wordsafe")]
        public string _wordSafeFormatted { get => $"0x{WordSafe:X}"; set => WordSafe = value.ToUInt32Ex(); }

        [XmlAttribute("wordsize")]
        public string _wordSizeFormatted { get => $"0x{WordSize:X}"; set => WordSize = value.ToUInt32Ex(); }

    }

    /// <summary>
    /// Code memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CodeMemTraits : MemTrait, ITrait
    {
        public PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public PICMemorySubDomain SubDomain => PICMemorySubDomain.Code;
    }

    /// <summary>
    /// External code memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ExtCodeMemTraits : MemTrait, ITrait
    {
        public PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public PICMemorySubDomain SubDomain => PICMemorySubDomain.ExtCode;
    }

    /// <summary>
    /// Calibration data memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CalDataMemTraits : MemTrait, ITrait
    {
        public PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public PICMemorySubDomain SubDomain => PICMemorySubDomain.Calib;
    }

    /// <summary>
    /// Background debug memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class BackgroundDebugMemTraits : MemTrait
    {
        public PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public PICMemorySubDomain SubDomain => PICMemorySubDomain.Debugger;
    }

    /// <summary>
    /// Test memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class TestMemTraits : MemTrait
    {
        public PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public PICMemorySubDomain SubDomain => PICMemorySubDomain.Test;
    }

    /// <summary>
    /// User IDs memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class UserIDMemTraits : MemTrait, ITrait
    {
        public PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public PICMemorySubDomain SubDomain => PICMemorySubDomain.UserID;
    }

    /// <summary>
    /// Configuration fuses memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigFuseMemTraits : MemTrait, ITrait
    {
        public PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public PICMemorySubDomain SubDomain => PICMemorySubDomain.DeviceConfig;

        [XmlIgnore]
        public int UnimplVal { get; private set; }

        [XmlAttribute("unimplval")]
        public string _unimplValFormatted { get => $"{UnimplVal}"; set => UnimplVal = value.ToInt32Ex(); }

    }

    /// <summary>
    /// Configuration Write-Once-Read-Many memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigWORMMemTraits : MemTrait, ITrait
    {
        public PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public PICMemorySubDomain SubDomain => PICMemorySubDomain.Other;
    }

    /// <summary>
    /// Device IDs memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceIDMemTraits : MemTrait, ITrait
    {
        public PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public PICMemorySubDomain SubDomain => PICMemorySubDomain.DeviceID;
    }

    /// <summary>
    /// EEPROM data memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class EEDataMemTraits : MemTrait, ITrait
    {
        public PICMemoryDomain Domain => PICMemoryDomain.Prog;
        public PICMemorySubDomain SubDomain => PICMemorySubDomain.EEData;

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
        public string _magicOffsetFormatted { get => $"0x{MagicOffset:X}"; set => MagicOffset = value.ToUInt32Ex(); }

    }

    /// <summary>
    /// Data memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataMemTraits : MemTrait, ITrait
    {
        public PICMemoryDomain Domain => PICMemoryDomain.Data;
        public PICMemorySubDomain SubDomain => PICMemorySubDomain.Undef;
    }

    /// <summary>
    /// The various memory regions' traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class MemTraits
    {
        /// <summary> Gets the depth of the hardware stack. </summary>
        [XmlIgnore]
        public int HWStackDepth { get; private set; }

        /// <summary> Gets the number of memory banks. </summary>
        [XmlIgnore]
        public int BankCount { get; private set; }

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
        public List<MemTrait> Traits { get; set; }

        [XmlAttribute("hwstackdepth")]
        public string _hwStackDepthFormatted { get => $"{HWStackDepth}"; set => HWStackDepth = value.ToInt32Ex(); }

        [XmlAttribute("bankcount")]
        public string _bankCountFormatted { get => $"{BankCount}"; set => BankCount = value.ToInt32Ex(); }

    }

    /// <summary>
    /// PIC memory architecture definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ArchDef
    {

        /// <summary> Gets the name (16xxxx, 16Exxx, 18xxxx) of the PIC architecture. </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary> Gets the memory traits of the PIC. </summary>
        [XmlElement("MemTraits")]
        public MemTraits MemTraits { get; set; }

        /// <summary> Gets the description of the PIC architecture. </summary>
        [XmlAttribute("desc")]
        public string Description { get; set; }

    }

    #endregion

    #region InstructionSet definition

    /// <summary>
    /// This class defines the instruction-set family ID as define dby Microchip.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class InstructionSet
    {
        /// <summary> Gets the instruction set ID. </summary>
        [XmlAttribute("instructionsetid")]
        public string ID { get; set; }

    }

    #endregion

    #region InterruptList XML element

    /// <summary>
    /// Interrupt request.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class Interrupt : IInterrupt
    {
        /// <summary> Gets the IRQ number. </summary>
        [XmlIgnore]
        public uint IRQ { get; private set; }

        /// <summary> Gets the name of the interrupt request.</summary>
        [XmlAttribute("cname")]
        public string Name { get; set; }

        /// <summary> Gets the description of the interrupt request. </summary>
        [XmlAttribute("desc")]
        public string Description { get; set; }

        [XmlAttribute("irq")]
        public string _irqFormatted { get => $"{IRQ}"; set => IRQ = value.ToUInt32Ex(); }

    }

    #endregion

    #region Commonalities

    /// <summary>
    /// Adjust bit/byte address by the specified offset.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class AdjustPoint
    {
        /// <summary> Gets the relative byte offset to add for adjustment. </summary>
        [XmlIgnore]
        public int Offset { get; private set; }

        [XmlAttribute("offset")]
        public string _offsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); }

    }

    /// <summary>
    /// A PIC memory region with all possible attributes. Must be inherited.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class PICMemoryRegion : MemoryAddrRange
    {
        /// <summary> Gets the identifier of the region. </summary>
        [XmlAttribute("regionid")]
        public string RegionID { get; set; }

        [XmlIgnore]
        public virtual bool IsBank { get => false; set { } }

        [XmlIgnore]
        public int Bank { get; set; } = 0;

        [XmlIgnore]
        public virtual string ShadowIDRef { get => string.Empty; set { } }

        [XmlIgnore]
        public virtual int ShadowOffset { get; set; } = 0;

        [XmlIgnore]
        public virtual bool IsSection { get => false; set { } }

        /// <summary> Gets the textual description of the section. </summary>
        [XmlAttribute("sectiondesc")]
        public virtual string SectionDesc { get; set; } = string.Empty;

        /// <summary> Gets the name of the section. </summary>
        [XmlAttribute("sectionname")]
        public virtual string SectionName { get; set; } = string.Empty;

        [XmlAttribute("bank")]
        public string _bankFormatted { get => $"{Bank}"; set => Bank = value.ToInt32Ex(); }

        [XmlIgnore]
        public bool _bankFormattedSpecified => IsBank;

        [XmlIgnore]
        public bool SectionDescSpecified => IsSection;

        [XmlIgnore]
        public bool SectionNameSpecified => IsSection;

        [XmlIgnore]
        public IPICMemoryRegion PICMemoryRegionInterface
            => picmemregintf = picmemregintf ?? new PICMemoryRegion_v1_Interface(this);

        private IPICMemoryRegion picmemregintf;

    }

    #endregion

    #region ProgramSpace XML element

    /// <summary>
    /// A Program memory region. Must be inherited.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class ProgMemoryRegion : PICMemoryRegion
    {
        public override PICMemoryDomain MemoryDomain => PICMemoryDomain.Prog;

        /// <summary> Gets a value indicating whether this memory region is a section. </summary>
        [XmlIgnore]
        public override bool IsSection { get => false; set { } }

    }

    /// <summary>
    /// Program Memory region seen as a section. Must be inherited.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class ProgMemorySection : ProgMemoryRegion
    {
        /// <summary> Gets a value indicating whether this memory region is a section. </summary>
        [XmlAttribute(AttributeName = "issection", DataType = "boolean")]
        public override bool IsSection { get; set; }

    }

    /// <summary>
    /// Code memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CodeSector : ProgMemorySection
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Code;
    }

    /// <summary>
    /// Calibration data zone memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CalDataZone : ProgMemorySection
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Calib;
    }

    /// <summary>
    /// Test zone memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class TestZone : ProgMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Test;
    }

    /// <summary>
    /// User IDs memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class UserIDSector : ProgMemorySection
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.UserID;
    }

    /// <summary>
    /// Revision IDs sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class RevisionIDSector : ProgMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.RevisionID;

        /// <summary> Gets the device ID to silicon revision relationship. </summary>
        [XmlElement("DEVIDToRev")]
        public List<DEVIDToRev> DEVIDToRev { get; set; }

    }

    /// <summary>
    /// Device IDs sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceIDSector : ProgMemorySection
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DeviceID;

        /// <summary> Gets the bit mask to isolate device ID. </summary>
        [XmlIgnore]
        public int Mask { get; private set; }

        /// <summary> Gets the generic value of device ID </summary>
        [XmlIgnore]
        public int Value { get; private set; }

        /// <summary> Gets the Device IDs to silicon revision level. </summary>
        [XmlElement("DEVIDToRev")]
        public List<DEVIDToRev> DEVIDToRev { get; set; }

        [XmlAttribute("mask")]
        public string _maskFormatted { get => $"0x{Mask:X}"; set => Mask = value.ToInt32Ex(); }

        [XmlAttribute("value")]
        public string _valueFormatted { get => $"0x{Value:X}"; set => Value = value.ToInt32Ex(); }

    }

    /// <summary>
    /// Device Configuration Register field pattern semantic.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldSemantic : IDeviceFusesSemantic
    {
        /// <summary> Gets the name of the field. </summary>
        [XmlAttribute("cname")]
        public string Name { get; set; }

        /// <summary> Gets the textual description of the field value. </summary>
        [XmlAttribute("desc")]
        public string Description { get; set; }

        /// <summary> Gets the when condition for the field value. </summary>
        [XmlAttribute("when")]
        public string When { get; set; }

        /// <summary> Gets a value indicating whether this configuration pattern is hidden. </summary>
        [XmlAttribute("ishidden", DataType = "boolean")]
        public bool IsHidden { get; set; }

        /// <summary> Gets or sets a value indicating whether this configuration pattern is hidden to language tools. </summary>
        [XmlAttribute("islanghidden", DataType = "boolean")]
        public bool IsLangHidden { get; set; }

        /// <summary>Gets the oscillator mode identifier reference. </summary>
        [XmlIgnore]
        public int OscModeIDRef { get; private set; }

        [XmlAttribute("_defeatcoercion")]
        public string DefeatCoercion { get; set; }

        /// <summary> Gets the memory mode identifier reference. </summary>
        [XmlAttribute("memmodeidref")]
        public string MemModeIDRef { get; set; }

        [XmlIgnore]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        [XmlIgnore]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        [XmlAttribute("oscmodeidref")]
        public string _oscModeIDRefFormatted { get => $"{OscModeIDRef}"; set => OscModeIDRef = value.ToInt32Ex(); }

        [XmlIgnore]
        public bool _oscModeIDRefFormattedSpecified { get => OscModeIDRef != 0; set { } }

    }

    /// <summary>
    /// Device Configuration Register Field definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldDef : IDeviceFusesField
    {

        /// <summary> Gets the name of the field. </summary>
        [XmlAttribute("cname")]
        public string Name { get; set; }

        /// <summary> Gets the textual description of the field. </summary>
        [XmlAttribute("desc")]
        public string Description { get; set; }

        /// <summary> Gets the bit position of the field. (Not currently populated by Microchip). </summary>
        [XmlIgnore]
        public byte BitPos { get; set; }

        /// <summary> Gets the bit width of the field. </summary>
        [XmlIgnore]
        public byte BitWidth { get; private set; }

        /// <summary> Gets the bit mask of the field in the register. </summary>
        [XmlIgnore]
        public int BitMask { get; private set; }

        /// <summary> Gets a value indicating whether this configuration field is hidden. </summary>
        [XmlAttribute("ishidden", DataType = "boolean")]
        public bool IsHidden { get; set; }

        /// <summary> Gets a value indicating whether this configuration field is hidden to language tools. </summary>
        [XmlAttribute("islanghidden", DataType = "boolean")]
        public bool IsLangHidden { get; set; }

        /// <summary> Gets a value indicating whether this configuration field is hidden to the MPLAB IDE. </summary>
        [XmlAttribute("isidehidden", DataType = "boolean")]
        public bool IsIDEHidden { get; set; }

        /// <summary> Gets the list of configuration field semantics for various configuration values. </summary>
        [XmlElement("DCRFieldSemantic")]
        public List<DCRFieldSemantic> DCRFieldSemantics { get; set; }

        [XmlAttribute("nzwidth")]
        public string _nzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToByteEx(); }

        [XmlAttribute("mask")]
        public string _maskFormatted { get => $"0x{BitMask:X}"; set => BitMask = value.ToInt32Ex(); }

        [XmlIgnore]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        [XmlIgnore]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        [XmlIgnore]
        public bool IsIDEHiddenSpecified { get => IsIDEHidden; set { } }

        [XmlIgnore]
        public IEnumerable<IDeviceFusesSemantic> Semantics
        {
            get
            {
                foreach (var fs in DCRFieldSemantics)
                {
                    yield return fs;

                }
            }
        }

    }

    /// <summary>
    /// Device Configuration Register mode.
    /// </summary>
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

    /// <summary>
    /// Device Configuration Register illegal definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRDefIllegal : IDeviceFusesIllegal
    {
        /// <summary> Gets the "when" pattern of the illegal condition. </summary>
        [XmlAttribute("when")]
        public string When { get; set; }

        /// <summary> Gets the textual description of the illegal condition. </summary>
        [XmlAttribute("desc")]
        public string Description { get; set; }

    }

    /// <summary>
    /// Device Configuration Register definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRDef
    {
        /// <summary> Gets the memory address of the configuration register. </summary>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary> Gets the name of the configuration register. </summary>
        [XmlAttribute("cname")]
        public string CName { get; set; }

        /// <summary> Gets the textual description of the configuration register. </summary>
        [XmlAttribute("desc")]
        public string Desc { get; set; }

        /// <summary> Gets the bit width of the register. </summary>
        [XmlIgnore]
        public int NzWidth { get; private set; }

        /// <summary> Gets the implemented bit mask. </summary>
        [XmlIgnore]
        public int ImplMask { get; private set; }

        /// <summary> Gets the access modes of the register's bits. </summary>
        [XmlAttribute("access")]
        public string Access { get; set; }

        /// <summary> Gets the default value of the register. </summary>
        [XmlIgnore]
        public int DefaultValue { get; private set; }

        /// <summary> Gets the factory default value of the register. </summary>
        [XmlIgnore]
        public int FactoryDefault { get; private set; }

        /// <summary> Gets a value indicating whether this register is hidden to language tools. </summary>
        [XmlAttribute("islanghidden", DataType = "boolean")]
        public bool IsLangHidden { get; set; }

        /// <summary> Gets the unimplemented bitmask value. </summary>
        [XmlIgnore]
        public int UnimplVal { get; private set; }

        [XmlIgnore]
        public int Unused { get; private set; }

        /// <summary> Gets the bit mask to use in checksum computation. </summary>
        [XmlIgnore]
        public int UseInChecksum { get; private set; }

        /// <summary> Gets the list of illegal configuration values (if any). </summary>
        [XmlElement("Illegal")]
        public List<DCRDefIllegal> Illegals { get; set; }

        /// <summary> Gets a list of Device Configuration Register modes. </summary>
        [XmlArray("DCRModeList")]
        [XmlArrayItem("DCRMode", typeof(DCRMode), IsNullable = false)]
        public List<DCRMode> DCRModes { get; set; }

        [XmlIgnore]
        public IDeviceFuse DeviceFusesConfig_Interface
            => devcfgfuseintf = devcfgfuseintf ?? new DeviceFusesConfig_v1_Interface(this);

        private IDeviceFuse devcfgfuseintf = null;

        [XmlAttribute("_addr")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        [XmlAttribute("nzwidth")]
        public string _nzWidthFormatted { get => $"{NzWidth}"; set => NzWidth = value.ToInt32Ex(); }

        [XmlAttribute("impl")]
        public string _implFormatted { get => $"0x{ImplMask:X}"; set => ImplMask = value.ToInt32Ex(); }

        [XmlAttribute("default")]
        public string _defaultFormatted { get => $"0x{DefaultValue:X}"; set => DefaultValue = value.ToInt32Ex(); }

        [XmlAttribute("factorydefault")]
        public string _factoryDefaultFormatted { get => $"0x{FactoryDefault:X}"; set => FactoryDefault = value.ToInt32Ex(); }

        [XmlAttribute("unimplval")]
        public string _unimplValFormatted { get => $"{UnimplVal}"; set => UnimplVal = value.ToInt32Ex(); }

        [XmlAttribute("unused")]
        public string _unusedFormatted { get => $"{Unused}"; set => Unused = value.ToInt32Ex(); }

        [XmlAttribute("useinchecksum")]
        public string _useInChecksumFormatted { get => $"0x{UseInChecksum:X}"; set => UseInChecksum = value.ToInt32Ex(); }

        [XmlIgnore]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

    }

    /// <summary>
    /// Configuration Fuses memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigFuseSector : ProgMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DeviceConfig;

        /// <summary> Gets the list of configuration registers definitions. </summary>
        [XmlElement("DCRDef", typeof(DCRDef))]
        [XmlElement("AdjustPoint", typeof(AdjustPoint))]
        public List<object> Defs { get; set; }

        [XmlIgnore]
        public IConfigFuses ConfigFuseSector_Interface
            => cfgfusesecitf = cfgfusesecitf ?? new ConfigFuseSector_v1_Interface(this);

        private IConfigFuses cfgfusesecitf = null;


    }

    /// <summary>
    /// Background debugger vector memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class BACKBUGVectorSector : ProgMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Debugger;
    }

    /// <summary>
    /// Data EEPROM memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class EEDataSector : ProgMemorySection
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.EEData;
    }

    /// <summary>
    /// Device Information Area (DIA) register.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceRegister : IDeviceInfoRegister
    {
        /// <summary> Gets the address of the register. </summary>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary> Gets the name of the register. </summary>
        [XmlAttribute("cname")]
        public string Name { get; set; }

        /// <summary> Gets the bit width of the register. </summary>
        [XmlIgnore]
        public int BitWidth { get; private set; }

        [XmlAttribute("_addr")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        [XmlAttribute("nzwidth")]
        public string _nzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToInt32Ex(); }

    }

    /// <summary>
    /// Device Information Area register array.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DIARegisterArray
    {

        /// <summary> Gets the starting memory address of the DIA registers. </summary>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary> Gets the name of the DIA registers array. </summary>
        [XmlAttribute("cname")]
        public string Name { get; set; }

        [XmlElement("Register", typeof(DeviceRegister))]
        [XmlElement("AdjustPoint", typeof(AdjustPoint))]
        public List<object> Registers { get; set; }

        [XmlAttribute("_addr")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

    }

    /// <summary>
    /// Device Information Area memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DIASector : ProgMemoryRegion, IDeviceInfoSector
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DeviceInfoAry;

        /// <summary> Gets the list of register arrays. </summary>
        [XmlElement("RegisterArray")]
        public List<DIARegisterArray> RegisterArrays { get; set; }

        [XmlIgnore]
        public IEnumerable<IDeviceInfoRegister> Registers
        {
            get
            {
                foreach (var ra in RegisterArrays)
                {
                    foreach (var r in ra.Registers)
                    {
                        switch (r)
                        {
                            case DeviceRegister dreg:
                                yield return dreg;
                                break;

                            case AdjustPoint adj:
                                break;

                            default:
                                throw new InvalidOperationException($"Invalid PIC device info register in '{ra.Name}' : {r.GetType()}");
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// Device Configuration Information memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCISector : ProgMemoryRegion, IDeviceInfoSector
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DeviceConfigInfo;

        /// <summary> Gets the list of configuration information registers. </summary>
        [XmlElement("Register")]
        public List<DeviceRegister> DCIRegisters { get; set; }

        [XmlIgnore]
        public IEnumerable<IDeviceInfoRegister> Registers
        {
            get
            {
                foreach (var r in DCIRegisters)
                    yield return r;
            }
        }

    }

    /// <summary>
    /// External Code memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ExtCodeSector : ProgMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.ExtCode;
    }

    /// <summary>
    /// Interrupt Vector area.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class VectorArea
    {
        /// <summary> Gets the bytes size of the area. </summary>
        [XmlIgnore]
        public int Size { get; private set; }

        /// <summary> Gets the identifier of the vector area. </summary>
        [XmlAttribute("regionid")]
        public string RegionID { get; set; }

        [XmlAttribute("nzsize")]
        public string _nzsizeformatted { get => $"0x{Size:X}"; set => Size = value.ToInt32Ex(); }

    }

    /// <summary>
    /// Program memory space.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgramSpace
    {
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

    }

    #endregion

    #region DataSpace XML element

    /// <summary>
    /// A Data memory region. Must be inherited.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class DataMemoryRegion : PICMemoryRegion
    {
        public override PICMemoryDomain MemoryDomain => PICMemoryDomain.Data;
    }

    /// <summary>
    /// A memory banked region. Must be inherited.
    /// </summary>
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
        public string _shadowOffsetFormatted { get => $"0x{ShadowOffset}"; set => ShadowOffset = value.ToInt32Ex(); }

    }

    /// <summary>
    /// SFR Field semantic.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRFieldSemantic : ISFRFieldSemantic
    {
        /// <summary> Gets the textual description of the semantic.</summary>
        [XmlAttribute("desc")]
        public string Description { get; set; }

        /// <summary> Gets the "when" condition of the semantic. </summary>
        [XmlAttribute("when")]
        public string When { get; set; }

    }

    /// <summary>
    /// SFR bits-field definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRFieldDef : ISFRBitField
    {
        /// <summary> Gets the bit width of this SFR field. </summary>
        [XmlIgnore]
        public byte BitWidth { get; set; }

        /// <summary> Gets the bit position/address (zero-based) of this SFR field. </summary>
        [XmlIgnore]
        public byte BitPos { get; set; }

        /// <summary> Gets the bit mask of this SFR field. </summary>
        [XmlIgnore]
        public int BitMask { get; set; }

        /// <summary> Gets the name of this SFR Field. </summary>
        [XmlAttribute("cname")]
        public string Name { get; set; }

        /// <summary> Gets the textual description of this SFR Field. </summary>
        [XmlAttribute("desc")]
        public string Description { get; set; }

        /// <summary> Gets a value indicating whether this SFR Field is hidden to language tools. </summary>
        [XmlAttribute("islanghidden", DataType = "boolean")]
        public bool IsLangHidden { get; set; }

        /// <summary> Gets a value indicating whether this SFR Field is hidden. </summary>
        [XmlAttribute("ishidden", DataType = "boolean")]
        public bool IsHidden { get; set; }

        /// <summary> Gets a value indicating whether this SFR Field is hidden to MPLAB IDE. </summary>
        [XmlAttribute("isidehidden", DataType = "boolean")]
        public bool IsIDEHidden { get; set; }

        /// <summary> Gets the list of semantics of this SFR field. </summary>
        [XmlElement("SFRFieldSemantic")]
        public List<SFRFieldSemantic> SFRFieldSemantics { get; set; }

        [XmlAttribute("nzwidth")]
        public string _nzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToByteEx(); }

        [XmlAttribute("bitpos")]
        public string _bitPosFormatted { get => $"{BitPos}"; set => BitPos = value.ToByteEx(); }

        [XmlAttribute("mask")]
        public string _maskFormatted { get => $"0x{BitMask:X}"; set => BitMask = value.ToInt32Ex(); }

        [XmlIgnore]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        [XmlIgnore]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        [XmlIgnore]
        public bool IsIDEHiddenSpecified { get => IsIDEHidden; set { } }

        [XmlIgnore]
        public IEnumerable<ISFRFieldSemantic> FieldSemantics
            => SFRFieldSemantics.Select(p => p).Cast<ISFRFieldSemantic>();

        private string _debugDisplay
            => $"SFRBitField={Name} @{BitPos}[{BitWidth}]";
    }

    /// <summary>
    /// SFR Fields definitions for a given mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
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

        private string _debugDisplay
            => $"SFRMode={ID}";
    }

    /// <summary>
    /// List of SFR modes.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRModeList
    {
        /// <summary> Gets the list of SFR modes. </summary>
        [XmlElement("SFRMode")]
        public List<SFRMode> SFRModes { get; set; }

    }

    /// <summary>
    /// Special Function Register (SFR) definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRDef : ISFRRegister
    {
        /// <summary> Gets the data memory address of this SFR. </summary>
        [XmlIgnore]
        public uint Addr { get; private set; }

        /// <summary> Gets the name of this SFR. </summary>
        [XmlAttribute("cname")]
        public string Name { get; set; }

        /// <summary> Gets the textual description of this SFR. </summary>
        [XmlAttribute("desc")]
        public string Description { get; set; }

        /// <summary> Gets the bit width of this SFR. </summary>
        [XmlIgnore]
        public byte BitWidth { get; private set; }

        /// <summary> Gets the bit position of this SFR when under a joined SFR. </summary>
        [XmlIgnore]
        public byte BitPos { get; set; } = 0;

        /// <summary> Gets the byte width of this SFR. </summary>
        [XmlIgnore]
        public int ByteWidth => (BitWidth + 7) >> 3;

        /// <summary> Gets the implemented bits mask of this SFR. </summary>
        [XmlIgnore]
        public uint ImplMask { get; private set; }

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

        /// <summary> Gets a value indicating whether this SFR is volatile. </summary>
        [XmlAttribute("isvolatile", DataType = "boolean")]
        public bool IsVolatile { get; set; }

        /// <summary> Gets a value indicating whether this SFR is hidden. </summary>
        [XmlAttribute("ishidden", DataType = "boolean")]
        public bool IsHidden { get; set; }

        /// <summary> Gets a value indicating whether this SFR is hidden to language tools. </summary>
        [XmlAttribute("islanghidden", DataType = "boolean")]
        public bool IsLangHidden { get; set; }

        /// <summary> Gets a value indicating whether this SFR is hidden to MPLAB IDE. </summary>
        [XmlAttribute("isidehidden", DataType = "boolean")]
        public bool IsIDEHidden { get; set; }

        /// <summary> Gets the name of the peripheral this SFR is the base address of. </summary>
        [XmlAttribute("baseofperipheral")]
        public string BaseOfPeripheral { get; set; }

        /// <summary> Gets the Non-Memory-Mapped-Register identifier of the SFR. </summary>
        [XmlAttribute("nmmrid")]
        public string NMMRID { get; set; }

        /// <summary> Gets a value indicating whether this SFR is Non-Memory-Mapped. </summary>
        [XmlIgnore]
        public bool IsNMMR { get => !String.IsNullOrEmpty(NMMRID); set { } }

        [XmlIgnore]
        public IEnumerable<ISFRBitField> BitFields
        {
            get
            {
                foreach (var smod in SFRModes)
                {
                    int bitPos = 0;
                    foreach (var bf in smod.Fields)
                    {
                        switch (bf)
                        {
                            case SFRFieldDef sfd:
                                sfd.BitPos = (byte)bitPos;
                                bitPos += sfd.BitWidth;
                                yield return sfd;
                                break;

                            case AdjustPoint badj:
                                bitPos += badj.Offset;
                                break;

                            default:
                                throw new InvalidOperationException($"Invalid SFR field type in '{Name}': {bf.GetType()}");
                        }
                    }
                }
            }
        }

        /// <summary> Gets the list of modes for this SFR. </summary>
        [XmlArray("SFRModeList")]
        [XmlArrayItem("SFRMode", typeof(SFRMode), IsNullable = false)]
        public List<SFRMode> SFRModes { get; set; }

        [XmlAttribute("_addr")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); }

        [XmlAttribute("nzwidth")]
        public string _nzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToByteEx(); }

        [XmlAttribute("bitpos")]
        public string _bitPosFormatted { get => $"{BitPos}"; set => BitPos = value.ToByteEx(); }

        [XmlAttribute("impl")]
        public string _implFormatted { get => $"0x{ImplMask:X}"; set => ImplMask = value.ToUInt32Ex(); }

        [XmlIgnore]
        public bool IsIndirectSpecified { get => IsIndirect; set { } }

        [XmlIgnore]
        public bool IsVolatileSpecified { get => IsVolatile; set { } }

        [XmlIgnore]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        [XmlIgnore]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        [XmlIgnore]
        public bool IsIDEHiddenSpecified { get => IsIDEHidden; set { } }

        private string _debugDisplay
            => $"SFR '{Name}' @{(IsNMMR ? $"NMMRID({NMMRID})" : $"0x{Addr:X}")}";
    }

    /// <summary>
    /// Mirrored registers area.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class Mirror : IPICMirroringRegion
    {
        /// <summary> Gets the memory address of the mirror. </summary>
        [XmlIgnore]
        public uint Addr { get; private set; }

        /// <summary> Gets the size in bytes of the mirrored area. </summary>
        [XmlIgnore]
        public uint Size { get; private set; }

        /// <summary> Gets the region identifier reference of the mirrored memory region. </summary>
        [XmlAttribute("regionidref")]
        public string TargetRegionID { get; set; }

        [XmlAttribute("_addr")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); }

        [XmlAttribute("nzsize")]
        public string _nzSizeFormatted { get => $"0x{Size:X}"; set => Size = value.ToUInt32Ex(); }

        private string _debugDisplay
            => $"Mirror '{TargetRegionID}' @0x{Addr:X}[{Size}]";
    }

    /// <summary>
    /// Joined SFR (e.g. FSR2 register composed of FSR2H:FSR2L registers).
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class JoinedSFRDef : IJoinedRegister
    {
        /// <summary> Gets the memory address of the joined SFRs. </summary>
        [XmlIgnore]
        public uint Addr { get; private set; }

        /// <summary> Gets the bit width of the joined SFR. </summary>
        [XmlIgnore]
        public byte BitWidth { get; set; }

        /// <summary> Gets the name of the joined SFR. </summary>
        [XmlAttribute("cname")]
        public string Name { get; set; }

        /// <summary> Gets the textual description of the joined SFR. </summary>
        [XmlAttribute("desc")]
        public string Description { get; set; }

        /// <summary> Gets the list of adjacent SFRs composing the join. </summary>
        [XmlElement("SFRDef")]
        public List<SFRDef> SFRs { get; set; }

        [XmlAttribute("_addr")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); }

        [XmlAttribute("nzwidth")]
        public string _nzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToByteEx(); }

        [XmlIgnore]
        public IEnumerable<ISFRRegister> ChildSFRs
        {
            get
            {
                foreach (var sfr in SFRs)
                    yield return sfr;
            }
        }

        private string _debugDisplay
            => $"Joined SFR '{Name}' @0x{Addr:X}[{BitWidth}]";
    }

    /// <summary>
    /// Selection of a SFR.
    /// </summary>
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
            => $"Select '{SFR.Name}' when '{When}'";

    }

    /// <summary>
    /// Multiplexed SFRs definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class MuxedSFRDef
    {
        /// <summary> Gets the memory address of the multiplexed SFRs. </summary>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary> Gets the bit width of the multiplex. </summary>
        [XmlIgnore]
        public byte BitWidth { get; private set; }

        /// <summary> Gets the byte width of the multiplex. </summary>
        [XmlIgnore]
        public int ByteWidth => (BitWidth + 7) >> 3;

        /// <summary> Gets the list of selections of SFRs. </summary>
        [XmlElement("SelectSFR")]
        public List<SelectSFR> SelectSFRs { get; set; }

        [XmlAttribute("_addr")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        [XmlAttribute("nzwidth")]
        public string _nzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToByteEx(); }

        private string _debugDisplay
            => $"Muxed SFR @0x{Addr:X}[{BitWidth}]";

    }

    /// <summary>
    /// DMA Register mirror.
    /// </summary>
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

    /// <summary>
    /// Special Function Registers data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRDataSector : DataMemoryBankedRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.SFR;

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

        private string _debugDisplay
            => $"SFR sector '{RegionID}' bank{Bank}[{BeginAddr:X3}-{EndAddr:X3}]";

    }

    /// <summary>
    /// General Purpose Registers (GPR) data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class GPRDataSector : DataMemoryBankedRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.GPR;

        private string _debugDisplay
            => $"GPR sector '{RegionID}' bank{Bank}[{BeginAddr:X3}-{EndAddr:X3}]";

    }

    /// <summary>
    /// Dual Port Registers data memory sector.
    /// </summary>
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

    /// <summary>
    /// Non-Memory-Mapped-Register (NMMR) definitions.
    /// </summary>
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

    /// <summary>
    /// Linear data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class LinearDataSector : MemoryAddrRange, IPICMemoryAddrRange
    {
        public override PICMemoryDomain MemoryDomain => PICMemoryDomain.Data;
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Linear;

        /// <summary> Gets the bytes size of the linear memory bank. </summary>
        [XmlIgnore]
        public int BankSize { get; private set; }

        /// <summary> Gets the beginning address of the linear memory bank. </summary>
        [XmlIgnore]
        public uint BlockBeginAddr { get; private set; }

        /// <summary> Gets the ending address (+1) of the linear memory bank. </summary>
        [XmlIgnore]
        public uint BlockEndAddr { get; private set; }

        [XmlAttribute("banksize")]
        public string _bankSizeFormatted { get => $"0x{BankSize:X}"; set => BankSize = value.ToInt32Ex(); }

        [XmlAttribute("blockbeginaddr")]
        public string _blockBeginAddrFormatted { get => $"0x{BlockBeginAddr:X}"; set => BlockBeginAddr = value.ToUInt32Ex(); }

        [XmlAttribute("blockendaddr")]
        public string _blockEndAddrFormatted { get => $"0x{BlockEndAddr:X}"; set => BlockEndAddr = value.ToUInt32Ex(); }

        private string _debugDisplay
            => $"Linear Data sector [0x{BlockBeginAddr:X}-0x{BlockEndAddr:X}]";

    }

    /// <summary>
    /// Data memory regions regardless of PIC execution mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class RegardlessOfMode
    {
        /// <summary> Gets the list of data memory regions. </summary>
        [XmlElement("SFRDataSector", typeof(SFRDataSector))]
        [XmlElement("DPRDataSector", typeof(DPRDataSector))]
        [XmlElement("GPRDataSector", typeof(GPRDataSector))]
        public List<DataMemoryBankedRegion> RegistersRegions { get; set; }

        [XmlElement("NMMRPlace")]
        public NMMRPlace NMMRPlace { get; set; }

    }

    /// <summary>
    /// Data memory space.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class DataSpace
    {
        /// <summary> Gets the highest (end) address (+1) of the data memory space. </summary>
        [XmlIgnore] public uint EndAddr { get; private set; }

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

        [XmlAttribute("endaddr")]
        public string _endAddrFormatted { get => $"0x{EndAddr:X}"; set => EndAddr = value.ToUInt32Ex(); }

        private string _debugDisplay
            => $"Data Space [0x0-0x{EndAddr:X}]";

    }

    #endregion

}


