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
namespace Reko.Libraries.Microchip
{
    using V1;

    #region ArchDef XML element

    /// <summary>
    /// Device ID to revision number.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DEVIDToRev
    {
        public DEVIDToRev() { }

        /// <summary> Gets the silicon revision. </summary>
        [XmlAttribute(AttributeName = "revlist", Form = XmlSchemaForm.None, Namespace = "")]
        public string RevList { get; set; }

        /// <summary> Gets the binary value of the silicon revision. </summary>
        [XmlIgnore]
        public int Value { get; private set; }

        [XmlAttribute(AttributeName = "value", Form = XmlSchemaForm.None, Namespace = "")]
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


        [XmlAttribute(AttributeName = "locsize", Form = XmlSchemaForm.None, Namespace = "")]
        public string _locSizeFormatted { get => $"0x{LocSize:X}"; set => LocSize = value.ToUInt32Ex(); }

        [XmlAttribute(AttributeName = "wordimpl", Form = XmlSchemaForm.None, Namespace = "")]
        public string _wordImplFormatted { get => $"0x{WordImpl:X}"; set => WordImpl = value.ToUInt32Ex(); }

        [XmlAttribute(AttributeName = "wordinit", Form = XmlSchemaForm.None, Namespace = "")]
        public string _wordInitFormatted { get => $"0x{WordInit:X}"; set => WordInit = value.ToUInt32Ex(); }

        [XmlAttribute(AttributeName = "wordsafe", Form = XmlSchemaForm.None, Namespace = "")]
        public string _wordSafeFormatted { get => $"0x{WordSafe:X}"; set => WordSafe = value.ToUInt32Ex(); }

        [XmlAttribute(AttributeName = "wordsize", Form = XmlSchemaForm.None, Namespace = "")]
        public string _wordSizeFormatted { get => $"0x{WordSize:X}"; set => WordSize = value.ToUInt32Ex(); }

    }

    /// <summary>
    /// A default memory trait.
    /// </summary>
    public sealed class DefaultMemTrait : IMemTrait, ITrait
    {

        /// <summary> Gets the default size of the memory word (in bytes). </summary>
        public uint WordSize => 1;

        /// <summary> Gets the default memory location access size (in bytes). </summary>
        public uint LocSize => 1;

        /// <summary> Gets the default memory word implementation (bit mask). </summary>
        public uint WordImpl => 0xFF;

        /// <summary> Gets the default initial (erased) memory word value. </summary>
        public uint WordInit => 0xFF;

        /// <summary> Gets the default memory word 'safe' value. </summary>
        public uint WordSafe => 0x00;

        public PICMemoryDomain Domain => PICMemoryDomain.Unknown;

        public PICMemorySubDomain SubDomain => PICMemorySubDomain.Undef;

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

        [XmlAttribute(AttributeName = "unimplval", Form = XmlSchemaForm.None, Namespace = "")]
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

        [XmlAttribute(AttributeName = "magicoffset", Form = XmlSchemaForm.None, Namespace = "")]
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
        [XmlElement(ElementName = "CodeMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(CodeMemTraits))]
        [XmlElement(ElementName = "ExtCodeMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ExtCodeMemTraits))]
        [XmlElement(ElementName = "CalDataMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(CalDataMemTraits))]
        [XmlElement(ElementName = "BackgroundDebugMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(BackgroundDebugMemTraits))]
        [XmlElement(ElementName = "TestMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(TestMemTraits))]
        [XmlElement(ElementName = "UserIDMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(UserIDMemTraits))]
        [XmlElement(ElementName = "ConfigFuseMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ConfigFuseMemTraits))]
        [XmlElement(ElementName = "ConfigWORMMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ConfigWORMMemTraits))]
        [XmlElement(ElementName = "DeviceIDMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DeviceIDMemTraits))]
        [XmlElement(ElementName = "DataMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataMemTraits))]
        [XmlElement(ElementName = "EEDataMemTraits", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(EEDataMemTraits))]
        public List<MemTrait> Traits { get; set; }

        [XmlAttribute(AttributeName = "hwstackdepth", Form = XmlSchemaForm.None, Namespace = "")]
        public string _hwStackDepthFormatted { get => $"{HWStackDepth}"; set => HWStackDepth = value.ToInt32Ex(); }

        [XmlAttribute(AttributeName = "bankcount", Form = XmlSchemaForm.None, Namespace = "")]
        public string _bankCountFormatted { get => $"{BankCount}"; set => BankCount = value.ToInt32Ex(); }

    }

    /// <summary>
    /// PIC memory architecture definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ArchDef
    {

        /// <summary> Gets the name (16xxxx, 16Exxx, 18xxxx) of the PIC architecture. </summary>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary> Gets the memory traits of the PIC. </summary>
        [XmlElement(ElementName = "MemTraits", Form = XmlSchemaForm.None, Namespace = "")]
        public MemTraits MemTraits { get; set; }

        /// <summary> Gets the description of the PIC architecture. </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Description { get; set; }

    }

    #endregion

    #region InstructionSet definition

    /// <summary>
    /// This class defines the instruction-set family ID as define dby Microchip.
    /// </summary>
    public sealed class InstructionSet
    {
        /// <summary> Gets the instruction set ID. </summary>
        [XmlAttribute(AttributeName = "instructionsetid", Form = XmlSchemaForm.None, Namespace = "")]
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
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary> Gets the description of the interrupt request. </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Description { get; set; }

        [XmlAttribute(AttributeName = "irq", Form = XmlSchemaForm.None, Namespace = "")]
        public string _irqFormatted { get => $"{IRQ}"; set => IRQ = value.ToUInt32Ex(); }

    }

    #endregion

    #region ProgramSpace XML element

    public abstract class MemProgramSymbolAcceptorBase : IMemProgramSymbolAcceptor
    {

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this program memory symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public abstract void Accept(IMemProgramSymbolVisitor v);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this program memory symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IMemProgramSymbolVisitor<T> v);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this program memory symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context);

        #endregion

    }

    /// <summary>
    /// A program memory addresses [range). Must be inherited.
    /// </summary>
    public abstract class ProgMemoryRange : MemoryAddrRange, IPICMemoryAddrRange
    {
        public override PICMemoryDomain MemoryDomain => PICMemoryDomain.Prog;
    }

    /// <summary>
    /// A Program memory region. Must be inherited.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class ProgMemoryRegion : ProgMemoryRange, IPICMemoryRegion, IMemProgramRegionAcceptor
    {

        /// <summary> Gets the identifier of the region. </summary>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

        /// <summary> Gets a value indicating whether this memory region is a section. </summary>
        [XmlIgnore]
        public virtual bool IsSection { get => false; set { } }

        /// <summary> Gets the textual description of the section. </summary>
        [XmlIgnore]
        public virtual string SectionDesc { get => string.Empty; set { } }

        /// <summary> Gets the name of the section. </summary>
        [XmlIgnore]
        public virtual string SectionName { get => string.Empty; set { } }

        /// <summary> Gets the shadow identifier reference, if any. </summary>
        [XmlIgnore]
        public string ShadowIDRef { get => string.Empty; set { } }

        /// <summary> Gets the shadow memory address offset, if any. </summary>
        [XmlIgnore]
        public int ShadowOffset { get => 0; set { } }

        [XmlIgnore]
        public int Bank { get => -1; set { } }

        #region IMemProgramRegionAcceptor interfaces

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public abstract void Accept(IMemProgramRegionVisitor v);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IMemProgramRegionVisitor<T> v);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context);

        #endregion

    }

    /// <summary>
    /// Program Memory region seen as a section. Must be inherited.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class ProgMemorySection : ProgMemoryRegion, IPICMemoryRegion
    {
        /// <summary> Gets a value indicating whether this memory region is a section. </summary>
        [XmlAttribute(AttributeName = "issection", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public override bool IsSection { get; set; }

        /// <summary> Gets the textual description of the section. </summary>
        [XmlAttribute(AttributeName = "sectiondesc", Form = XmlSchemaForm.None, Namespace = "")]
        public override string SectionDesc { get; set; }

        /// <summary> Gets the name of the section. </summary>
        [XmlAttribute(AttributeName = "sectionname", Form = XmlSchemaForm.None, Namespace = "")]
        public override string SectionName { get; set; }

    }

    /// <summary>
    /// Adjust byte address pointing in program memory spaces.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgByteAdjustPoint : MemProgramSymbolAcceptorBase, IAdjustPoint
    {
        /// <summary> Gets the relative byte offset to add for adjustment. </summary>
        [XmlIgnore]
        public int Offset { get; private set; }

        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        public string _offsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); }

        public override string ToString()
            => (Offset < 10 ? $"Adjust {Offset} byte(s)" : $"Adjust 0x{Offset:X} bytes");

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this program memory byte address adjustment symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this program memory byte address adjustment symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this program memory byte address adjustment symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Adjust bit address pointing in program memory slots.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgBitAdjustPoint : MemProgramSymbolAcceptorBase, IAdjustPoint
    {
        /// <summary> Gets the relative bit offset to add for adjustment. </summary>
        [XmlIgnore]
        public int Offset { get; private set; }

        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        public string _offsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); }

        public override string ToString()
            => (Offset < 10 ? $"Adjust {Offset} bit(s)" : $"Adjust 0x{Offset:X} bits");

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this program memory bit address adjustment symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this program memory bit address adjustment symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this program memory bit address adjustment symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Code memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CodeSector : ProgMemorySection, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Code;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this code program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this code program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this code program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Calibration data zone memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CalDataZone : ProgMemorySection, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Calib;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this calibration data program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this calibration data program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this calibration data program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Test zone memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class TestZone : ProgMemoryRegion, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Test;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this test zone program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this test zone program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this test zone program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// User IDs memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class UserIDSector : ProgMemorySection, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.UserID;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this User ID program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this User ID program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this User ID program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Revision IDs sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class RevisionIDSector : ProgMemoryRegion, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.RevisionID;

        /// <summary> Gets the device ID to silicon revision relationship. </summary>
        [XmlElement(ElementName = "DEVIDToRev", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DEVIDToRev> DEVIDToRev { get; set; }


        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this Revision ID program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this Revision ID program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this Revision ID program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device IDs sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceIDSector : ProgMemorySection, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DeviceID;

        /// <summary> Gets the bit mask to isolate device ID. </summary>
        [XmlIgnore]
        public int Mask { get; private set; }

        /// <summary> Gets the generic value of device ID </summary>
        [XmlIgnore]
        public int Value { get; private set; }

        /// <summary> Gets the Device IDs to silicon revision level. </summary>
        [XmlElement(ElementName = "DEVIDToRev", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DEVIDToRev> DEVIDToRev { get; set; }

        [XmlAttribute(AttributeName = "mask", Form = XmlSchemaForm.None, Namespace = "")]
        public string _maskFormatted { get => $"0x{Mask:X}"; set => Mask = value.ToInt32Ex(); }

        [XmlAttribute(AttributeName = "value", Form = XmlSchemaForm.None, Namespace = "")]
        public string _valueFormatted { get => $"0x{Value:X}"; set => Value = value.ToInt32Ex(); }


        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this Device ID program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this Device ID program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this Device ID program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Configuration Register field pattern semantic.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldSemantic : MemProgramSymbolAcceptorBase, IDeviceFusesSemantic
    {
        /// <summary> Gets the name of the field. </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary> Gets the textual description of the field value. </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Description { get; set; }

        /// <summary> Gets the when condition for the field value. </summary>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        /// <summary> Gets a value indicating whether this configuration pattern is hidden. </summary>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }

        /// <summary> Gets or sets a value indicating whether this configuration pattern is hidden to language tools. </summary>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        /// <summary>Gets the oscillator mode identifier reference. </summary>
        [XmlIgnore]
        public int OscModeIDRef { get; private set; }

        [XmlAttribute(AttributeName = "_defeatcoercion", Form = XmlSchemaForm.None, Namespace = "")]
        public string DefeatCoercion { get; set; }

        /// <summary> Gets the memory mode identifier reference. </summary>
        [XmlAttribute(AttributeName = "memmodeidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string MemModeIDRef { get; set; }

        [XmlIgnore]
        public bool IsHiddenSpecified { get => IsHidden; set { } }

        [XmlIgnore]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }

        [XmlAttribute(AttributeName = "oscmodeidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string _oscModeIDRefFormatted { get => $"{OscModeIDRef}"; set => OscModeIDRef = value.ToInt32Ex(); }

        [XmlIgnore]
        public bool _oscModeIDRefFormattedSpecified { get => OscModeIDRef != 0; set { } }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this device configuration register field semantic symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register field semantic symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register field symbol semantic with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Configuration Register Field definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldDef : MemProgramSymbolAcceptorBase, IDeviceFusesField
    {

        /// <summary> Gets the name of the field. </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary> Gets the textual description of the field. </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
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
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }

        /// <summary> Gets a value indicating whether this configuration field is hidden to language tools. </summary>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        /// <summary> Gets a value indicating whether this configuration field is hidden to the MPLAB IDE. </summary>
        [XmlAttribute(AttributeName = "isidehidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIDEHidden { get; set; }

        /// <summary> Gets the list of configuration field semantics for various configuration values. </summary>
        [XmlElement(ElementName = "DCRFieldSemantic", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DCRFieldSemantic> DCRFieldSemantics { get; set; }

        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        public string _nzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToByteEx(); }

        [XmlAttribute(AttributeName = "mask", Form = XmlSchemaForm.None, Namespace = "")]
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

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this device configuration register field symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register field symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register field symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Configuration Register mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRMode : MemProgramSymbolAcceptorBase
    {
        /// <summary> Gets the identifier of the mode (usually "DS.0"). </summary>
        [XmlAttribute(AttributeName = "id", Form = XmlSchemaForm.None, Namespace = "")]
        public string ID { get; set; }

        /// <summary> Gets the fields of the configuration register. </summary>
        [XmlElement(ElementName = "DCRFieldDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DCRFieldDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ProgBitAdjustPoint))]
        public List<object> Fields { get; set; }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this device configuration register mode symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register mode symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register mode symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Configuration Register illegal definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRDefIllegal : MemProgramSymbolAcceptorBase, IDeviceFusesIllegal
    {
        /// <summary> Gets the "when" pattern of the illegal condition. </summary>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        /// <summary> Gets the textual description of the illegal condition. </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Description { get; set; }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this invalid device configuration register symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this invalid device configuration register symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this invalid device configuration register symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Configuration Register definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRDef : MemProgramSymbolAcceptorBase, IDeviceFusesConfig
    {
        /// <summary> Gets the memory address of the configuration register. </summary>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary> Gets the name of the configuration register. </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary> Gets the textual description of the configuration register. </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Description { get; set; }

        /// <summary> Gets the bit width of the register. </summary>
        [XmlIgnore]
        public int BitWidth { get; private set; }

        /// <summary> Gets the implemented bit mask. </summary>
        [XmlIgnore]
        public int ImplMask { get; private set; }

        /// <summary> Gets the access modes of the register's bits. </summary>
        [XmlAttribute(AttributeName = "access", Form = XmlSchemaForm.None, Namespace = "")]
        public string AccessBits { get; set; }

        /// <summary> Gets the default value of the register. </summary>
        [XmlIgnore]
        public int DefaultValue { get; private set; }

        /// <summary> Gets the factory default value of the register. </summary>
        [XmlIgnore]
        public int FactoryDefault { get; private set; }

        /// <summary> Gets a value indicating whether this register is hidden to language tools. </summary>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
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
        [XmlElement(ElementName = "Illegal", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DCRDefIllegal> Illegals { get; set; }

        [XmlIgnore]
        public IEnumerable<IDeviceFusesIllegal> IllegalSettings
        {
            get
            {
                foreach (var ilg in Illegals)
                    yield return ilg;
            }
        }

        /// <summary> Gets a list of Device Configuration Register modes. </summary>
        [XmlArray(ElementName = "DCRModeList", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem(ElementName = "DCRMode", Type = typeof(DCRMode), Form = XmlSchemaForm.None, IsNullable = false)]
        public List<DCRMode> DCRModes { get; set; }

        [XmlIgnore]
        public IEnumerable<IDeviceFusesField> ConfigFields
        {
            get
            {
                foreach (var mode in DCRModes)
                {
                    int bitpos = 0;
                    foreach (var fld in mode.Fields)
                    {
                        switch (fld)
                        {
                            case ProgBitAdjustPoint adj:
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

        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        public string _nzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToInt32Ex(); }

        [XmlAttribute(AttributeName = "impl", Form = XmlSchemaForm.None, Namespace = "")]
        public string _implFormatted { get => $"0x{ImplMask:X}"; set => ImplMask = value.ToInt32Ex(); }

        [XmlAttribute(AttributeName = "default", Form = XmlSchemaForm.None, Namespace = "")]
        public string _defaultFormatted { get => $"0x{DefaultValue:X}"; set => DefaultValue = value.ToInt32Ex(); }

        [XmlAttribute(AttributeName = "factorydefault", Form = XmlSchemaForm.None, Namespace = "")]
        public string _factoryDefaultFormatted { get => $"0x{FactoryDefault:X}"; set => FactoryDefault = value.ToInt32Ex(); }

        [XmlAttribute(AttributeName = "unimplval", Form = XmlSchemaForm.None, Namespace = "")]
        public string _unimplValFormatted { get => $"{UnimplVal}"; set => UnimplVal = value.ToInt32Ex(); }

        [XmlAttribute(AttributeName = "unused", Form = XmlSchemaForm.None, Namespace = "")]
        public string _unusedFormatted { get => $"{Unused}"; set => Unused = value.ToInt32Ex(); }

        [XmlAttribute(AttributeName = "useinchecksum", Form = XmlSchemaForm.None, Namespace = "")]
        public string _useInChecksumFormatted { get => $"0x{UseInChecksum:X}"; set => UseInChecksum = value.ToInt32Ex(); }

        [XmlIgnore]
        public bool IsLangHiddenSpecified { get => IsLangHidden; set { } }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this device configuration register symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device configuration register symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Configuration Fuses memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigFuseSector : ProgMemoryRegion, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DeviceConfig;

        /// <summary> Gets the list of configuration registers definitions. </summary>
        [XmlElement(ElementName = "DCRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DCRDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ProgByteAdjustPoint))]
        public List<object> Defs { get; set; }

        [XmlIgnore]
        public IEnumerable<IDeviceFusesConfig> Fuses
        {
            get
            {
                foreach (var f in Defs.OfType<IDeviceFusesConfig>())
                {
                    yield return f;
                }
            }

        }
        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this configuration fuses program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this configuration fuses program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this configuration fuses program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Background debugger vector memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class BACKBUGVectorSector : ProgMemoryRegion, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.Debugger;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this debugger program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this debugger program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this debugger program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Data EEPROM memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class EEDataSector : ProgMemorySection, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.EEData;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this Data EEPROM program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this Data EEPROM program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this Data EEPROM program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Information Area (DIA) register.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceRegister : MemProgramSymbolAcceptorBase, IDeviceInfoRegister
    {
        /// <summary> Gets the address of the register. </summary>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary> Gets the name of the register. </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary> Gets the bit width of the register. </summary>
        [XmlIgnore]
        public int BitWidth { get; private set; }

        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        public string _nzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToInt32Ex(); }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this device information area program memory symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device information area program memory symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device information area program memory symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Information Area register array.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DIARegisterArray : MemProgramSymbolAcceptorBase
    {

        /// <summary> Gets the starting memory address of the DIA registers. </summary>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary> Gets the name of the DIA registers array. </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Register", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DeviceRegister))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ProgByteAdjustPoint))]
        public List<object> Registers { get; set; }

        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }


        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory symbol visitor and calls the appropriate
        /// "Visit()" method for this device information areas program memory symbol.
        /// </summary>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        public override void Accept(IMemProgramSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device information areas program memory symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a program memory symbol visitor and calls the
        /// appropriate "Visit()" function for this device information areas program memory symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Information Area memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DIASector : ProgMemoryRegion, IPICMemoryRegion, IDeviceInfoSector
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DeviceInfoAry;

        /// <summary> Gets the list of register arrays. </summary>
        [XmlElement(ElementName = "RegisterArray", Form = XmlSchemaForm.None, Namespace = "")]
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

                            case ProgByteAdjustPoint adj:
                                break;

                            default:
                                throw new InvalidOperationException($"Invalid PIC device info register in '{ra.Name}' : {r.GetType()}");
                        }
                    }
                }
            }
        }


        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this device information area program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this device information area program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this device information area program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Device Configuration Information memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCISector : ProgMemoryRegion, IPICMemoryRegion, IDeviceInfoSector
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DeviceConfigInfo;

        /// <summary> Gets the list of configuration information registers. </summary>
        [XmlElement(ElementName = "Register", Form = XmlSchemaForm.None, Namespace = "")]
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

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this device configuration information program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this device configuration information program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this device configuration information program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// External Code memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ExtCodeSector : ProgMemoryRegion, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.ExtCode;

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this external code program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public override void Accept(IMemProgramRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this external code program memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemProgramRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a program memory region visitor and calls the
        /// appropriate "Visit()" function for this external code program memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The program memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemProgramRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

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
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

        [XmlAttribute(AttributeName = "nzsize", Form = XmlSchemaForm.None, Namespace = "")]
        public string _nzsizeformatted { get => $"0x{Size:X}"; set => Size = value.ToInt32Ex(); }

    }

    /// <summary>
    /// Program memory space.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgramSpace
    {
        /// <summary> Gets the list of program memory sectors. </summary>
        [XmlElement(ElementName = "CodeSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(CodeSector))]
        [XmlElement(ElementName = "ExtCodeSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ExtCodeSector))]
        [XmlElement(ElementName = "CalDataZone", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(CalDataZone))]
        [XmlElement(ElementName = "UserIDSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(UserIDSector))]
        [XmlElement(ElementName = "RevisionIDSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(RevisionIDSector))]
        [XmlElement(ElementName = "DeviceIDSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DeviceIDSector))]
        [XmlElement(ElementName = "BACKBUGVectorSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(BACKBUGVectorSector))]
        [XmlElement(ElementName = "EEDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(EEDataSector))]
        public List<ProgMemoryRegion> CodeSectors { get; set; }

        [XmlElement(ElementName = "ConfigFuseSector", Form = XmlSchemaForm.None, Namespace = "")]
        public ConfigFuseSector FusesSector { get; set; }

        [XmlElement(ElementName = "DIASector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DIASector))]
        [XmlElement(ElementName = "DCISector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DCISector))]
        public List<ProgMemoryRegion> InfoSectors { get; set; }

        /// <summary> Gets the list of interrupt vector area. </summary>
        [XmlElement(ElementName = "VectorArea", Form = XmlSchemaForm.None, Namespace = "")]
        public List<VectorArea> VectorArea { get; set; }

    }

    #endregion

    #region DataSpace XML element

    public abstract class MemDataSymbolAcceptorBase : IMemDataSymbolAcceptor
    {

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory symbol visitor and calls the appropriate
        /// "Visit()" method for this data memory symbol.
        /// </summary>
        /// <param name="v">The data memory symbol visitor to accept.</param>
        public abstract void Accept(IMemDataSymbolVisitor v);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory symbol visitor and calls the
        /// appropriate "Visit()" function for this data memory symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The data memory symbol visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IMemDataSymbolVisitor<T> v);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a data memory symbol visitor and calls the
        /// appropriate "Visit()" function for this data memory symbol with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context);

        #endregion

    }

    public abstract class MemDataRegionAcceptorBase : IMemDataRegionAcceptor
    {

        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the appropriate
        /// "Visit()" method for this data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public abstract void Accept(IMemDataRegionVisitor v);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this data memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IMemDataRegionVisitor<T> v);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this data memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context);

        #endregion

    }

    /// <summary>
    /// A data memory addresses range. Must be inherited.
    /// </summary>
    public abstract class DataMemoryRange : MemoryAddrRange, IPICMemoryAddrRange
    {
        public override PICMemoryDomain MemoryDomain => PICMemoryDomain.Data;
    }

    /// <summary>
    /// A Data memory region. Must be inherited.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class DataMemoryRegion : DataMemoryRange, IPICMemoryRegion, IMemDataRegionAcceptor
    {
        /// <summary> Gets the identifier of the region. </summary>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

        /// <summary> Gets a value indicating whether this memory region is a section. </summary>
        [XmlIgnore]
        public virtual bool IsSection { get => false; set { } }

        /// <summary> Gets the textual description of the section. </summary>
        [XmlIgnore]
        public virtual string SectionDesc { get => string.Empty; set { } }

        /// <summary> Gets the name of the section. </summary>
        [XmlIgnore]
        public virtual string SectionName { get => string.Empty; set { } }

        /// <summary> Gets the shadow identifier reference, if any. </summary>
        [XmlIgnore]
        public virtual string ShadowIDRef { get => string.Empty; set { } }

        /// <summary> Gets the shadow memory address offset, if any. </summary>
        [XmlIgnore]
        public virtual int ShadowOffset { get => 0; set { } }

        [XmlIgnore]
        public virtual int Bank { get => -1; set { } }


        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the appropriate
        /// "Visit()" method for this data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public abstract void Accept(IMemDataRegionVisitor v);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this data memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IMemDataRegionVisitor<T> v);

        /// <summary>
        /// The <see cref="Accept{T, C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this data memory region with the specified context.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context);

        #endregion

    }

    /// <summary>
    /// A memory banked region. Must be inherited.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class DataMemoryBankedRegion : DataMemoryRegion, IPICMemoryRegion
    {
        /// <summary> Gets the memory bank number. </summary>
        [XmlIgnore]
        public override int Bank { get; set; }

        [XmlAttribute(AttributeName = "bank", Form = XmlSchemaForm.None, Namespace = "")]
        public string _bankFormatted { get => $"{Bank}"; set => Bank = value.ToInt32Ex(); }

    }

    /// <summary>
    /// Adjust byte address pointing in data memory spaces.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataByteAdjustPoint : MemDataSymbolAcceptorBase, IAdjustPoint
    {
        /// <summary> Gets the relative byte offset to add for adjustment. </summary>
        [XmlIgnore]
        public int Offset { get; private set; }

        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        public string _offsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); }


        public override string ToString()
            => (Offset < 10 ? $"Adjust {Offset} byte(s)" : $"Adjust 0x{Offset:X} byte(s)");

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data memory byte address adjustment symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this data memory byte address adjustment symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this data memory byte address adjustment symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Adjust bit address pointing in data memory slots.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataBitAdjustPoint : MemDataSymbolAcceptorBase, IAdjustPoint
    {

        /// <summary> Gets the relative bit offset to add for adjustment. </summary>
        [XmlIgnore]
        public int Offset { get; private set; }

        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        public string _offsetFormatted { get => $"{Offset}"; set => Offset = value.ToInt32Ex(); }

        public override string ToString()
            => (Offset < 10 ? $"Adjust {Offset} bit(s)" : $"Adjust 0x{Offset:X} bit(s)");

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data memory bit address adjustment symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this data memory bit address adjustment symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this data memory bit address adjustment symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// SFR Field semantic.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRFieldSemantic : MemDataSymbolAcceptorBase, ISFRFieldSemantic
    {
        /// <summary> Gets the textual description of the semantic.</summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Description { get; set; }

        /// <summary> Gets the "when" condition of the semantic. </summary>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this special function register field semantic symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register field semantic symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register field semantic symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// SFR bits-field definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRFieldDef : MemDataSymbolAcceptorBase, ISFRBitField
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
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary> Gets the textual description of this SFR Field. </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Description { get; set; }

        /// <summary> Gets a value indicating whether this SFR Field is hidden to language tools. </summary>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        /// <summary> Gets a value indicating whether this SFR Field is hidden. </summary>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }

        /// <summary> Gets a value indicating whether this SFR Field is hidden to MPLAB IDE. </summary>
        [XmlAttribute(AttributeName = "isidehidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIDEHidden { get; set; }

        /// <summary> Gets the list of semantics of this SFR field. </summary>
        [XmlElement(ElementName = "SFRFieldSemantic", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SFRFieldSemantic> SFRFieldSemantics { get; set; }

        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        public string _nzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToByteEx(); }

        [XmlAttribute(AttributeName = "bitpos", Form = XmlSchemaForm.None, Namespace = "")]
        public string _bitPosFormatted { get => $"{BitPos}"; set => BitPos = value.ToByteEx(); }

        [XmlAttribute(AttributeName = "mask", Form = XmlSchemaForm.None, Namespace = "")]
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

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this special function register field symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register field symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register field symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"SFRBitField={Name} @{BitPos}[{BitWidth}]";
    }

    /// <summary>
    /// SFR Fields definitions for a given mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRMode : MemDataSymbolAcceptorBase
    {
        /// <summary> Gets the identifier of the mode (usually "DS.0"). </summary>
        [XmlAttribute(AttributeName = "id", Form = XmlSchemaForm.None, Namespace = "")]
        public string ID { get; set; }

        /// <summary> Gets the Power-ON Reset value of the mode. Not used. </summary>
        [XmlAttribute(AttributeName = "por", Form = XmlSchemaForm.None, Namespace = "")]
        public string POR { get; set; }

        /// <summary> Gets the list of SFR fields definitions. </summary>
        [XmlElement(ElementName = "SFRFieldDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRFieldDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataBitAdjustPoint))]
        public List<object> Fields { get; set; }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this special function register mode symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register mode symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register mode symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"SFRMode={ID}";
    }

    /// <summary>
    /// List of SFR modes.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRModeList : MemDataSymbolAcceptorBase
    {
        /// <summary> Gets the list of SFR modes. </summary>
        [XmlElement(ElementName = "SFRMode", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<SFRMode> SFRModes { get; set; }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this special function register modes list symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register modes list symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register modes list symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Special Function Register (SFR) definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRDef : MemDataSymbolAcceptorBase, ISFRRegister
    {
        /// <summary> Gets the data memory address of this SFR. </summary>
        [XmlIgnore]
        public uint Addr { get; private set; }

        /// <summary> Gets the name of this SFR. </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary> Gets the textual description of this SFR. </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
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
        [XmlAttribute(AttributeName = "access", Form = XmlSchemaForm.None, Namespace = "")]
        public string Access { get; set; }

        /// <summary> Gets the Master Clear (MCLR) bits values (string) of this SFR. </summary>
        [XmlAttribute(AttributeName = "mclr", Form = XmlSchemaForm.None, Namespace = "")]
        public string MCLR { get; set; }

        /// <summary> Gets the Power-ON Reset bits values (string) of this SFR. </summary>
        [XmlAttribute(AttributeName = "por", Form = XmlSchemaForm.None, Namespace = "")]
        public string POR { get; set; }

        /// <summary> Gets a value indicating whether this SFR is indirect. </summary>
        [XmlAttribute(AttributeName = "isindirect", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIndirect { get; set; }

        /// <summary> Gets a value indicating whether this SFR is volatile. </summary>
        [XmlAttribute(AttributeName = "isvolatile", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsVolatile { get; set; }

        /// <summary> Gets a value indicating whether this SFR is hidden. </summary>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }

        /// <summary> Gets a value indicating whether this SFR is hidden to language tools. </summary>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        /// <summary> Gets a value indicating whether this SFR is hidden to MPLAB IDE. </summary>
        [XmlAttribute(AttributeName = "isidehidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIDEHidden { get; set; }

        /// <summary> Gets the name of the peripheral this SFR is the base address of. </summary>
        [XmlAttribute(AttributeName = "baseofperipheral", Form = XmlSchemaForm.None, Namespace = "")]
        public string BaseOfPeripheral { get; set; }

        /// <summary> Gets the Non-Memory-Mapped-Register identifier of the SFR. </summary>
        [XmlAttribute(AttributeName = "nmmrid", Form = XmlSchemaForm.None, Namespace = "")]
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

                            case DataBitAdjustPoint badj:
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
        [XmlArray("SFRModeList", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("SFRMode", typeof(SFRMode), Form = XmlSchemaForm.None, Namespace = "", IsNullable = false)]
        public List<SFRMode> SFRModes { get; set; }

        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); }

        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        public string _nzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToByteEx(); }

        [XmlAttribute(AttributeName = "bitpos", Form = XmlSchemaForm.None, Namespace = "")]
        public string _bitPosFormatted { get => $"{BitPos}"; set => BitPos = value.ToByteEx(); }

        [XmlAttribute(AttributeName = "impl", Form = XmlSchemaForm.None, Namespace = "")]
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


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this special function register symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"SFR '{Name}' @{(IsNMMR ? $"NMMRID({NMMRID})" : $"0x{Addr:X}")}";
    }

    /// <summary>
    /// Mirrored registers area.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class Mirror : MemDataSymbolAcceptorBase, IPICMirroringRegion
    {
        /// <summary> Gets the memory address of the mirror. </summary>
        [XmlIgnore]
        public uint Addr { get; private set; }

        /// <summary> Gets the size in bytes of the mirrored area. </summary>
        [XmlIgnore]
        public uint Size { get; private set; }

        /// <summary> Gets the region identifier reference of the mirrored memory region. </summary>
        [XmlAttribute(AttributeName = "regionidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string TargetRegionID { get; set; }

        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); }

        [XmlAttribute(AttributeName = "nzsize", Form = XmlSchemaForm.None, Namespace = "")]
        public string _nzSizeFormatted { get => $"0x{Size:X}"; set => Size = value.ToUInt32Ex(); }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this mirrored data memory symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this mirrored data memory symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this mirrored data memory symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"Mirror '{TargetRegionID}' @0x{Addr:X}[{Size}]";
    }

    /// <summary>
    /// Joined SFR (e.g. FSR2 register composed of FSR2H:FSR2L registers).
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class JoinedSFRDef : MemDataSymbolAcceptorBase, IJoinedRegister
    {
        /// <summary> Gets the memory address of the joined SFRs. </summary>
        [XmlIgnore]
        public uint Addr { get; private set; }

        /// <summary> Gets the bit width of the joined SFR. </summary>
        [XmlIgnore]
        public byte BitWidth { get; set; }

        /// <summary> Gets the name of the joined SFR. </summary>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary> Gets the textual description of the joined SFR. </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Description { get; set; }

        /// <summary> Gets the list of adjacent SFRs composing the join. </summary>
        [XmlElement("SFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<SFRDef> SFRs { get; set; }

        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToUInt32Ex(); }

        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
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

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this joined special function registers symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this joined special function registers symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this joined special function registers symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"Joined SFR '{Name}' @0x{Addr:X}[{BitWidth}]";
    }

    /// <summary>
    /// Selection of a SFR.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SelectSFR : MemDataSymbolAcceptorBase
    {
        /// <summary> Gets the (optional) "when" condition for selection. </summary>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        /// <summary> Gets the SFR being selected. </summary>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        public SFRDef SFR { get; set; }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this special function register selection symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register selection symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this special function register selection symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"Select '{SFR.Name}' when '{When}'";

    }

    /// <summary>
    /// Multiplexed SFRs definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class MuxedSFRDef : MemDataSymbolAcceptorBase
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
        [XmlElement(ElementName = "SelectSFR", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SelectSFR> SelectSFRs { get; set; }

        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _addrFormatted { get => $"0x{Addr:X}"; set => Addr = value.ToInt32Ex(); }

        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        public string _nzWidthFormatted { get => $"{BitWidth}"; set => BitWidth = value.ToByteEx(); }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this multiplexed special function registers symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this multiplexed special function registers symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this multiplexed special function registers symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"Muxed SFR @0x{Addr:X}[{BitWidth}]";

    }

    /// <summary>
    /// DMA Register mirror.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DMARegisterMirror : MemDataSymbolAcceptorBase
    {
        /// <summary> Gets the name reference. </summary>
        [XmlAttribute(AttributeName = "cnameref", Form = XmlSchemaForm.None, Namespace = "")]
        public string CNameRef { get; set; }

        /// <summary> Gets the name suffix. </summary>
        [XmlAttribute(AttributeName = "cnamesuffix", Form = XmlSchemaForm.None, Namespace = "")]
        public string CNameSuffix { get; set; }


        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this Direct Memory Access register symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public override void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this Direct Memory Access register symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataSymbolVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" function for this Direct Memory Access register symbol.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataSymbolVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

    }

    /// <summary>
    /// Special Function Registers data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class SFRDataSector : DataMemoryBankedRegion, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.SFR;

        /// <summary> Gets the list of SFRs. </summary>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SFRDef> SFRs { get; set; }

        /// <summary> Gets the list of joined SFRs. </summary>
        [XmlElement(ElementName = "JoinedSFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        public List<JoinedSFRDef> JoinedSFRs { get; set; }

        /// <summary> Gets the list of multiplexed SFRs. </summary>
        [XmlElement(ElementName = "MuxedSFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        public List<MuxedSFRDef> MuxedSFRs { get; set; }

        /// <summary> Gets the list of mirrored SFRs. </summary>
        [XmlElement(ElementName = "Mirror", Form = XmlSchemaForm.None, Namespace = "")]
        public List<Mirror> MirrorSFRs { get; set; }

        /// <summary> Gets the list of DMA mirrored SFRs. </summary>
        [XmlElement(ElementName = "RegisterMirror", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DMARegisterMirror> DMARegisters { get; set; }


        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this special function registers memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public override void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this special function registers memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this special function registers memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"SFR sector '{RegionID}' bank{Bank}[{BeginAddr:X3}-{EndAddr:X3}]";

    }

    /// <summary>
    /// General Purpose Registers (GPR) data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class GPRDataSector : DataMemoryBankedRegion, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.GPR;

        /// <summary> Gets the shadow identifier reference, if any. </summary>
        [XmlAttribute(AttributeName = "shadowidref", Form = XmlSchemaForm.None, Namespace = "")]
        public override string ShadowIDRef { get; set; }

        /// <summary> Gets the shadow memory address offset. </summary>
        [XmlIgnore]
        public override int ShadowOffset { get; set; }

        [XmlAttribute(AttributeName = "shadowoffset", Form = XmlSchemaForm.None, Namespace = "")]
        public string _shadowOffsetFormatted { get => $"0x{ShadowOffset:X}"; set => ShadowOffset = value.ToInt32Ex(); }


        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this general purpose registers memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public override void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this general purpose registers memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this general purpose registers memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"GPR sector '{RegionID}' bank{Bank}[{BeginAddr:X3}-{EndAddr:X3}]";

    }

    /// <summary>
    /// Dual Port Registers data memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class DPRDataSector : DataMemoryBankedRegion, IPICMemoryRegion
    {
        public override PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.DPR;

        /// <summary> Gets the shadowed memory region identifier reference. </summary>
        [XmlAttribute(AttributeName = "shadowidref", Form = XmlSchemaForm.None, Namespace = "")]
        public override string ShadowIDRef { get; set; }

        /// <summary> Gets the shadow memory offset. </summary>
        [XmlIgnore] public override int ShadowOffset { get; set; }

        /// <summary> Gets the list of SFRs as Dual Port registers. </summary>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataByteAdjustPoint))]
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public List<object> SFRs { get; set; }

        [XmlAttribute(AttributeName = "shadowoffset", Form = XmlSchemaForm.None, Namespace = "")]
        public string _shadowOffsetFormatted { get => $"0x{ShadowOffset:X}"; set => ShadowOffset = value.ToInt32Ex(); }


        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this dual port registers memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public override void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this dual port registers memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this dual port registers memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"DPR sector '{RegionID}' bank{Bank}[{BeginAddr:X3}-{EndAddr:X3}]";

    }

    /// <summary>
    /// Non-Memory-Mapped-Register (NMMR) definitions.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class NMMRPlace : MemDataRegionAcceptorBase
    {
        public PICMemorySubDomain MemorySubDomain => PICMemorySubDomain.NNMR;

        /// <summary> Gets the identifier of the NMMR region. </summary>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

        /// <summary> Gets the list of SFR definitions. </summary>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SFRDef> SFRDefs { get; set; }


        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this Non-Memory-Mapped register definition.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public override void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this Non-Memory-Mapped register definition.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T>(IMemDataRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this Non-Memory-Mapped register definition.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public override T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

        private string _debugDisplay
            => $"NMMR '{RegionID}'";

    }

    /// <summary>
    /// Linear data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class LinearDataSector : DataMemoryRange, IMemDataRegionAcceptor
    {
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

        [XmlAttribute(AttributeName = "banksize", Form = XmlSchemaForm.None, Namespace = "")]
        public string _bankSizeFormatted { get => $"0x{BankSize:X}"; set => BankSize = value.ToInt32Ex(); }

        [XmlAttribute(AttributeName = "blockbeginaddr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _blockBeginAddrFormatted { get => $"0x{BlockBeginAddr:X}"; set => BlockBeginAddr = value.ToUInt32Ex(); }

        [XmlAttribute(AttributeName = "blockendaddr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _blockEndAddrFormatted { get => $"0x{BlockEndAddr:X}"; set => BlockEndAddr = value.ToUInt32Ex(); }


        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this linear access data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this linear access data memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public T Accept<T>(IMemDataRegionVisitor<T> v) => v.Visit(this);

        /// <summary>
        /// The <see cref="Accept{T,C}"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this linear access data memory region.
        /// </summary>
        /// <typeparam name="T">Generic type parameter of function result.</typeparam>
        /// <typeparam name="C">Generic type parameter of the context.</typeparam>
        /// <param name="v">The data memory region visitor to accept.</param>
        /// <param name="context">The context of generic type <typeparamref name="C"/>.</param>
        /// <returns>
        /// A result of generic type <typeparamref name="T"/>.
        /// </returns>
        public T Accept<T, C>(IMemDataRegionVisitor<T, C> v, C context) => v.Visit(this, context);

        #endregion

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
        [XmlElement(ElementName = "SFRDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRDataSector))]
        [XmlElement(ElementName = "DPRDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DPRDataSector))]
        [XmlElement(ElementName = "GPRDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(GPRDataSector))]
        public List<DataMemoryBankedRegion> RegistersRegions { get; set; }

        [XmlElement(ElementName = "NMMRPlace", Form = XmlSchemaForm.None, Namespace = "")]
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
        [XmlElement(ElementName = "RegardlessOfMode", Form = XmlSchemaForm.None, Namespace = "")]
        public RegardlessOfMode RegardlessOfMode { get; set; }

        /// <summary> Gets the list of GPR data memory regions when PIC is in traditional execution mode. </summary>
        [XmlArray("TraditionalModeOnly", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("GPRDataSector", typeof(GPRDataSector), IsNullable = false, Namespace = "")]
        public List<GPRDataSector> TraditionalModeOnly { get; set; }

        /// <summary> Gets the list of GPR data memory regions when PIC is in extended execution mode. </summary>
        [XmlArray("ExtendedModeOnly", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("GPRDataSector", typeof(GPRDataSector), IsNullable = false, Namespace = "")]
        public List<GPRDataSector> ExtendedModeOnly { get; set; }

        [XmlAttribute(AttributeName = "endaddr", Namespace = "")]
        public string _endAddrFormatted { get => $"0x{EndAddr:X}"; set => EndAddr = value.ToUInt32Ex(); }

        private string _debugDisplay
            => $"Data Space [0x0-0x{EndAddr:X}]";

    }

    #endregion


    /// <summary>
    /// PIC definition. (Version 1)
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [XmlRoot(ElementName = "PIC", Namespace = "", IsNullable = false)]
    [DebuggerDisplay("{_debugDisplay,nq}")]
    public sealed class PIC_v1
    {
        /// <summary>
        /// Gets the version number of the interface
        /// </summary>
        public int Version { get; set; } = 1;

        // Maps the 'InstructionsetID' identifiers to internal code.
        /// <summary> Gets the PIC name from the XML. </summary>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary> Gets the PIC architecture name (16xxxx, 16Exxx, 18xxxx) from the XML. </summary>
        [XmlAttribute(AttributeName = "arch", Form = XmlSchemaForm.None, Namespace = "")]
        public string Arch { get; set; }

        /// <summary> Gets the PIC description from the XML. </summary>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary> Gets the architecture definition from the XML. </summary>
        [XmlElement(ElementName = "ArchDef", Form = XmlSchemaForm.None, Namespace = "")]
        public ArchDef ArchDef { get; set; }

        /// <summary> Gets the instruction set identifier from the XML. </summary>
        [XmlElement(ElementName = "InstructionSet", Form = XmlSchemaForm.None, Namespace = "")]
        public InstructionSet InstructionSet { get; set; }

        /// <summary> Gets the unique processor identifier from the XML. Used by development tools. </summary>
        [XmlIgnore]
        public int ProcID { get; private set; }

        /// <summary> Gets the data sheet identifier of the PIC from the XML. </summary>
        [XmlAttribute(AttributeName = "dsid", Form = XmlSchemaForm.None, Namespace = "")]
        public string DsID { get; set; }

        [XmlAttribute(AttributeName = "dosid", Form = XmlSchemaForm.None, Namespace = "")]
        public string DosID { get; set; }

        /// <summary>
        /// Gets the indicator whether this PIC supports the PIC18 extended execution mode.
        /// Overridden by the data space definition containing non-empty extended-mode-only memory space.
        /// </summary>
        [XmlAttribute(AttributeName = "isextended", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool HasExtendedMode { get => DataSpace?.ExtendedModeOnly?.Count > 0; set { } }

        /// <summary> Gets a value indicating whether this PIC supports freezing of peripherals from the XML. </summary>
        [XmlAttribute(AttributeName = "hasfreeze", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool HasFreeze { get; set; }

        /// <summary> Gets a value indicating whether this PIC supports debugging from the XML. </summary>
        [XmlAttribute(AttributeName = "isdebuggable", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsDebuggable { get; set; }

        [XmlAttribute(AttributeName = "informedby", Form = XmlSchemaForm.None, Namespace = "")]
        public string Informedby { get; set; }

        [XmlAttribute(AttributeName = "masksetid", Form = XmlSchemaForm.None, Namespace = "")]
        public string MaskSetID { get; set; }

        [XmlAttribute(AttributeName = "psid", Form = XmlSchemaForm.None, Namespace = "")]
        public string PsID { get; set; }

        /// <summary> Gets the name of the PIC, this PIC is the clone of, from the XML. </summary>
        [XmlAttribute(AttributeName = "clonedfrom", Form = XmlSchemaForm.None, Namespace = "")]
        public string ClonedFrom { get; set; }

        /// <summary> Gets a list of interrupts (IRQ) from the XML. </summary>
        [XmlArray("InterruptList", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("Interrupt", typeof(Interrupt), Form = XmlSchemaForm.None, IsNullable = false, Namespace = "")]
        public List<Interrupt> Interrupts { get; set; }

        /// <summary> Gets the program memory space definitions from the XML. </summary>
        [XmlElement(ElementName = "ProgramSpace", Form = XmlSchemaForm.None, Namespace = "")]
        public ProgramSpace ProgramSpace { get; set; }

        /// <summary> Gets the data memory space definitions from the XML. </summary>
        [XmlElement(ElementName = "DataSpace", Form = XmlSchemaForm.None, Namespace = "")]
        public DataSpace DataSpace { get; set; }

        /// <summary> Gets the list of SFRs in the DMA space from the XML. </summary>
        [XmlArray("DMASpace", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("SFRDataSector", typeof(SFRDataSector), IsNullable = false, Namespace = "")]
        public List<SFRDataSector> DMASpace { get; set; }

        /// <summary> Gets the list of linear data memory regions in the indirect space from the XML. </summary>
        [XmlArray("IndirectSpace", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("LinearDataSector", typeof(LinearDataSector), Form = XmlSchemaForm.None, IsNullable = false, Namespace = "")]
        public List<LinearDataSector> IndirectSpace { get; set; }

        [XmlAttribute(AttributeName = "procid", Form = XmlSchemaForm.None, Namespace = "")]
        public string _procIDFormatted { get => $"0x{ProcID:X}"; set => ProcID = value.ToInt32Ex(); }

        [XmlIgnore]
        public IPICDescriptor PICDescriptorInterface
            => picintf = picintf ?? new PIC_v1_Interface(this);

        private PIC_v1_Interface picintf = null;

        private string _debugDisplay => $"PIC (v1) '{Name}' ({Arch}) ";

    }

}


