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

// summary:	Implements the Microchip PIC XML definition (de)serialization per Microchip Crownking Database.
//
namespace Microchip.Crownking
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using Microchip.Utils;

    #region Base common definitions

    public enum MemoryDomain : byte
    {
        Unknown = 0,
        Prog,
        Data,
        Absolute,
        Other
    };

    public enum MemorySubDomain
    {
        /// <summary>Memory region is undefined (transient value)</summary>
        Undef = -1,
        /// <summary>Data region is a Special Function Register</summary>
        SFR,
        /// <summary>Data region is a General Purpose Register</summary>
        GPR,
        /// <summary>Data region is a Dual-Port Register</summary>
        DPR,
        /// <summary>Data region is a Non-Memory-Map Register.</summary>
        NNMR,
        /// <summary>Data region is a zone reserved for Emulator.</summary>
        Emulator,
        /// <summary>Data region is a linear view of data memory.</summary>
        Linear,
        /// <summary>Program region is code</summary>
        Code,
        /// <summary>Program region is external code</summary>
        ExtCode,
        /// <summary>Program region is EEPROM</summary>
        EEData,
        /// <summary>Program region is a Configuration Bits</summary>
        Config,
        /// <summary>Program region is a User IDs words</summary>
        UserID,
        /// <summary>Program region is a Device IDs words</summary>
        DeviceID,
        /// <summary>Program region is a Revision IDs words</summary>
        RevisionID,
        /// <summary>Program region is for Debugger</summary>
        Debugger,
        /// <summary>Program region is a Calibration words</summary>
        Calib,
        /// <summary>Program region is of other type.</summary>
        Other
    };

    /// <summary>
    /// Values that represent PIC Execution modes.
    /// </summary>
    public enum PICExecMode
    {
        Undefined = -1,
        Traditional,
        Extended
    };

    /// <summary>
    /// Values that represent PIC Instruction Set identifiers.
    /// </summary>
    public enum InstructionSetID
    {
        /// <summary> PIC Instruction Set is undefined. </summary>
        UNDEFINED,
        /// <summary> PIC Instruction Set is PIC16 like pic16f77 - basic mid-range. Identified as InstructionSet="pic16f77" in XML definition.</summary>
        PIC16,
        /// <summary> PIC Instruction Set is PIC16 like pic16f1946 - mid-range enhanced 5-bit BSR. Identified as InstructionSet="cpu_mid_v10" in XML definition.</summary>
        PIC16_ENH,
        /// <summary> PIC Instruction Set is PIC16 like pic16f15313 - mid-range enhanced 6-bit BSR. Identified as InstructionSet="cpu_p16f1_v1" in XML definition.</summary>
        PIC16_ENH_V1,
        /// <summary> PIC Instruction Set is traditional PIC18 like pic18f1220 - without any extended mode. Identified as InstructionSet="pic18" in XML definition.</summary>
        PIC18,
        /// <summary> PIC Instruction Set is PIC18 like pic18f1230 - with extended execution mode capabilities. Identified as InstructionSet="egg" in XML definition.</summary>
        PIC18_EXTENDED,
        /// <summary> PIC Instruction Set is PIC18 enhanced like pic18f25k42 - same as PIC18_EXTD + some instructions for bigger RAM size. Identified as InstructionSet="cpu_pic18f_v6" in XML definition.</summary>
        PIC18_ENHANCED,
    }


    /// <summary>
    /// (Serializable)The abstract class <see cref="MemoryAddrRange"/> represents a PIC memory address range (either in data, program or absolute space).
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class MemoryAddrRange :
        IEquatable<MemoryAddrRange>, IEqualityComparer<MemoryAddrRange>,
        IComparer<MemoryAddrRange>, IComparable<MemoryAddrRange>

    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MemoryAddrRange() { }

        #endregion

        #region Properties

        [XmlIgnore]
        public abstract MemoryDomain MemoryDomain { get; }

        [XmlIgnore]
        public abstract MemorySubDomain MemorySubDomain { get; }

        /// <summary>
        /// Used to (de)serialize <see cref="BeginAddr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The begin address content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "beginaddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _beginaddrformatted { get { return $"0x{BeginAddr:X}"; } set { BeginAddr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the begin address of the memory range.
        /// </summary>
        /// <value>
        /// The begin address as an integer.
        /// </value>
        [XmlIgnore]
        public int BeginAddr { get; private set; }

        /// <summary>
        /// Used to (de)serialize <see cref="EndAddr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The end address as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "endaddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _endaddrformatted { get { return $"0x{EndAddr:X}"; } set { EndAddr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the end address of the memory range.
        /// </summary>
        /// <value>
        /// The end address as an integer.
        /// </value>
        [XmlIgnore]
        public int EndAddr { get; private set; }

        #endregion

        #region Implementation of the IEquatable<MemoryAddrRange>, IEqualityComparer<MemoryAddrRange>, IComparer<MemoryAddrRange>, IComparable<MemoryAddrRange> interfaces

        public bool Equals(MemoryAddrRange other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (MemoryDomain != other.MemoryDomain) return false;
            if (BeginAddr != other.BeginAddr) return false;
            return (EndAddr == other.EndAddr);
        }

        public override bool Equals(object obj) => Equals(obj as MemoryAddrRange);

        public override int GetHashCode() => (BeginAddr.GetHashCode() + EndAddr.GetHashCode()) ^ MemoryDomain.GetHashCode();

        public static bool operator ==(MemoryAddrRange reg1, MemoryAddrRange reg2) => reg1.Equals(reg2);

        public static bool operator !=(MemoryAddrRange reg1, MemoryAddrRange reg2) => !reg1.Equals(reg2);

        public int Compare(MemoryAddrRange x, MemoryAddrRange y)
        {
            if (ReferenceEquals(x, y)) return 0;
            return x?.CompareTo(y) ?? -1;
        }

        public bool Equals(MemoryAddrRange x, MemoryAddrRange y)
        {
            if (ReferenceEquals(x, y)) return true;
            return x?.Equals(y) ?? false;
        }

        public int GetHashCode(MemoryAddrRange obj) => obj?.GetHashCode() ?? 0;

        public int CompareTo(MemoryAddrRange other)
        {
            if (other == null) return 1;
            if (ReferenceEquals(this, other)) return 0;
            if (MemoryDomain != other.MemoryDomain) return MemoryDomain.CompareTo(other.MemoryDomain);
            if (BeginAddr == other.BeginAddr) return EndAddr.CompareTo(other.EndAddr);
            return BeginAddr.CompareTo(other.BeginAddr);
        }

        #endregion

    }

    #endregion

    #region ArchDef XML element

    /// <summary>
    /// (Serializable)Device ID to revision number.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DEVIDToRev
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DEVIDToRev()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the silicon revision.
        /// </summary>
        /// <value>
        /// A revision as a string.
        /// </value>
        [XmlAttribute(AttributeName = "revlist", Form = XmlSchemaForm.None, Namespace = "")]
        public string RevList { get; set; }

        /// <summary>
        /// Gets the binary value of the silicon revision.
        /// </summary>
        /// <value>
        /// The value as an integer.
        /// </value>
        [XmlIgnore]
        public int Value { get; private set; }

        #endregion

        #region XML conversion helpers

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "value", Form = XmlSchemaForm.None, Namespace = "")]
        public string _valueformatted { get { return $"0x{Value:X}"; } set { Value = value.ToInt32Ex(); } }

        #endregion

    }

    /// <summary>
    /// (Serializable)Memory trait (characteristics).
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class MemTrait
    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MemTrait() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the size of the memory word.
        /// </summary>
        /// <value>
        /// The size of the memory word in bytes.
        /// </value>
        [XmlIgnore]
        public virtual int WordSize { get; private set; }

        /// <summary>
        /// Gets the memory location access size.
        /// </summary>
        /// <value>
        /// The size of the location in bytes.
        /// </value>
        [XmlIgnore]
        public virtual int LocSize { get; private set; }

        /// <summary>
        /// Gets the memory word implementation (bit mask).
        /// </summary>
        /// <value>
        /// The memory word implementation bitmask.
        /// </value>
        [XmlIgnore]
        public virtual int WordImpl { get; private set; }

        /// <summary>
        /// Gets the initial (erased) memory word value.
        /// </summary>
        /// <value>
        /// The word initialize.
        /// </value>
        [XmlIgnore]
        public virtual int WordInit { get; private set; }

        /// <summary>
        /// Gets the memory word safe value.
        /// </summary>
        /// <value>
        /// The memory word safe value.
        /// </value>
        [XmlIgnore]
        public virtual int WordSafe { get; private set; }

        #endregion

        #region XML conversion helpers

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "locsize", Form = XmlSchemaForm.None, Namespace = "")]
        public string _locsizeformatted { get { return $"0x{LocSize:X}"; } set { LocSize = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "wordimpl", Form = XmlSchemaForm.None, Namespace = "")]
        public string _wordimplformatted { get { return $"0x{WordImpl:X}"; } set { WordImpl = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "wordinit", Form = XmlSchemaForm.None, Namespace = "")]
        public string _wordinitformatted { get { return $"0x{WordInit:X}"; } set { WordInit = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "wordsafe", Form = XmlSchemaForm.None, Namespace = "")]
        public string _wordsafeformatted { get { return $"0x{WordSafe:X}"; } set { WordSafe = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "wordsize", Form = XmlSchemaForm.None, Namespace = "")]
        public string _wordsizeformatted { get { return $"0x{WordSize:X}"; } set { WordSize = value.ToInt32Ex(); } }

        #endregion

    }

    /// <summary>
    /// (Serializable)Code memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CodeMemTraits : MemTrait, IMemTraitsSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CodeMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interface 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        #endregion

    }

    /// <summary>
    /// (Serializable)External code memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ExtCodeMemTraits : MemTrait, IMemTraitsSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExtCodeMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interface 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        #endregion

    }

    /// <summary>
    /// (Serializable)Calibration data memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CalDataMemTraits : MemTrait, IMemTraitsSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CalDataMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interface 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        #endregion

    }

    /// <summary>
    /// (Serializable)Background debug memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class BackgroundDebugMemTraits : MemTrait, IMemTraitsSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BackgroundDebugMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interface 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        #endregion

    }

    /// <summary>
    /// (Serializable)Test memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class TestMemTraits : MemTrait, IMemTraitsSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TestMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interface 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        #endregion

    }

    /// <summary>
    /// (Serializable)User IDs memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class UserIDMemTraits : MemTrait, IMemTraitsSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UserIDMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interface 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        #endregion

    }

    /// <summary>
    /// (Serializable)Configuration fuses memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigFuseMemTraits : MemTrait, IMemTraitsSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigFuseMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interface 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        [XmlIgnore]
        public int UnimplVal { get; private set; }

        #endregion

        #region XML conversion helpers

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "unimplval", Form = XmlSchemaForm.None, Namespace = "")]
        public string _unimplvalformatted { get { return $"{UnimplVal}"; } set { UnimplVal = value.ToInt32Ex(); } }

        #endregion

    }

    /// <summary>
    /// (Serializable)Configuration Write-Once-Read-Many memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigWORMMemTraits : MemTrait, IMemTraitsSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigWORMMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interface 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        #endregion

    }

    /// <summary>
    /// (Serializable)Device IDs memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceIDMemTraits : MemTrait, IMemTraitsSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DeviceIDMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interface 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        #endregion

    }

    /// <summary>
    /// (Serializable)EEPROM data memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class EEDataMemTraits : MemTrait, IMemTraitsSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EEDataMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interface 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        public override int LocSize => 1;
        public override int WordSize => 1;
        public override int WordImpl => 0xFF;
        public override int WordInit => 0xFF;
        public override int WordSafe => 0xFF;

        /// <summary>
        /// Gets address magic offset in the binary image for EEPROM content.
        /// </summary>
        /// <value>
        /// The offset address as an integer.
        /// </value>
        [XmlIgnore]
        public int MagicOffset { get; private set; }

        #endregion

        #region XML conversion helpers

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "magicoffset", Form = XmlSchemaForm.None, Namespace = "")]
        public string _magicoffsetformatted { get { return $"0x{MagicOffset:X}"; } set { MagicOffset = value.ToInt32Ex(); } }

        #endregion

    }

    /// <summary>
    /// (Serializable)Data memory traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataMemTraits : MemTrait, IMemTraitsSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataMemTraits() { }

        #endregion

        #region IMemTraitsSymbolAcceptor interface 

        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate
        /// "Visit()" method for this trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsRegionVisitor"/> visitor to accept.</param>
        public void Accept(IMemTraitsSymbolVisitor v) => v.Visit(this);

        #endregion

    }

    /// <summary>
    /// (Serializable)The various memory regions' traits.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class MemTraits
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MemTraits() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the memory traits of the various memory regions.
        /// </summary>
        /// <value>
        /// The memory traits.
        /// </value>
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

        /// <summary>
        /// Gets the depth of the hardware stack.
        /// </summary>
        /// <value>
        /// The depth of the hardware stack as an integer.
        /// </value>
        [XmlIgnore]
        public int HWStackDepth { get; private set; }

        /// <summary>
        /// Gets the number of memory banks.
        /// </summary>
        /// <value>
        /// The number of banks as an integer.
        /// </value>
        [XmlIgnore]
        public int BankCount { get; private set; }

        #endregion

        #region XML conversion helpers

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "hwstackdepth", Form = XmlSchemaForm.None, Namespace = "")]
        public string _hwstackdepthformatted { get { return $"{HWStackDepth}"; } set { HWStackDepth = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "bankcount", Form = XmlSchemaForm.None, Namespace = "")]
        public string _bankcountformatted { get { return $"{BankCount}"; } set { BankCount = value.ToInt32Ex(); } }

        #endregion

    }

    /// <summary>
    /// (Serializable)PIC memory architecture definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ArchDef
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ArchDef() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the memory traits of the PIC.
        /// </summary>
        /// <value>
        /// The memory traits.
        /// </value>
        [XmlElement(ElementName = "MemTraits", Form = XmlSchemaForm.None, Namespace = "")]
        public MemTraits MemTraits { get; set; }

        /// <summary>
        /// Gets the description of the PIC architecture.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets the name (16xxxx, 16Exxx, 18xxxx) of the PIC architecture.
        /// </summary>
        /// <value>
        /// The name of architecture (16xxxx, 16Exxx, 18xxxx) as a string
        /// </value>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        #endregion

    }

    #endregion

    #region InstructionSet definition

    public sealed class InstructionSet
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InstructionSet() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instruction set ID.
        /// </summary>
        /// <value>
        /// The ID as a string.
        /// </value>
        [XmlAttribute(AttributeName = "instructionsetid", Form = XmlSchemaForm.None, Namespace = "")]
        public string ID { get; set; }

        #endregion

        #region Method


        #endregion

    }

    #endregion

    #region InterruptList XML element

    /// <summary>
    /// (Serializable)Interrupt request.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class Interrupt
    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Interrupt() { }

        #endregion

        #region Properties

        /// <summary>
        /// Used to (de)serialize <see cref="IRQ" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The IRQ content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "irq", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _irqformatted { get { return $"{IRQ}"; } set { IRQ = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the IRQ number.
        /// </summary>
        /// <value>
        /// The IRQ number as an integer.
        /// </value>
        [XmlIgnore]
        public int IRQ { get; private set; }

        /// <summary>
        /// Gets the name of the interrupt request.
        /// </summary>
        /// <value>
        /// The interrupt request name.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the description of the interrupt request.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        #endregion

    }

    #endregion

    #region ProgramSpace XML element

    /// <summary>
    /// A program memory addresses range.
    /// </summary>
    public abstract class ProgMemoryRange : MemoryAddrRange
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgMemoryRange() { }

        #endregion

        public override MemoryDomain MemoryDomain => MemoryDomain.Prog;

    }

    /// <summary>
    /// (Serializable)A Program memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class ProgMemoryRegion : ProgMemoryRange
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgMemoryRegion() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the region.
        /// </summary>
        /// <value>
        /// The identifier of the region as a string.
        /// </value>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Program Memory region seen as a section.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class ProgMemorySection : ProgMemoryRegion
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgMemorySection() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this memory region is a section.
        /// </summary>
        /// <value>
        /// True if this memory region is a section, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "issection", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsSection { get; set; }

        /// <summary>
        /// Gets the section description.
        /// </summary>
        /// <value>
        /// Information describing the section.
        /// </value>
        [XmlAttribute(AttributeName = "sectiondesc", Form = XmlSchemaForm.None, Namespace = "")]
        public string SectionDesc { get; set; }

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        /// <value>
        /// The name of the section as a string.
        /// </value>
        [XmlAttribute(AttributeName = "sectionname", Form = XmlSchemaForm.None, Namespace = "")]
        public string SectionName { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Adjust byte address pointing in program memory spaces.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgByteAdjustPoint : IMemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgByteAdjustPoint() { }

        #endregion

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramSymbolVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Used to (de)serialize <see cref="Offset" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The offset content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _offsetformatted { get { return $"{Offset}"; } set { Offset = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the relative byte offset to add.
        /// </summary>
        /// <value>
        /// The offset as an integer.
        /// </value>
        [XmlIgnore]
        public int Offset { get; private set; }

        #endregion

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            if (Offset < 10) return $"Adjust {Offset} byte(s)";
            return $"Adjust 0x{Offset:X} bytes";
        }

#if FULLPICXML

        /// <summary>
        /// Gets the byte address.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        [XmlIgnore]
        public int Begin { get; private set; }

        [XmlIgnore]
        public int End { get; private set; }

        [XmlAttribute(AttributeName = "_modsrc", Form = XmlSchemaForm.None, Namespace = "")]
        public string ModSrc { get; set; }

        [XmlIgnore]
        public int RefCount { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _addrformatted        { get { return $"0x{Addr:X}"; } set { Addr = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_begin", Form = XmlSchemaForm.None, Namespace = "")]
        public string _beginformatted        { get { return $"0x{Begin:X}"; } set { Begin = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_end", Form = XmlSchemaForm.None, Namespace = "")]
        public string _endformatted        { get { return $"0x{End:X}"; } set { End = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_refcount", Form = XmlSchemaForm.None, Namespace = "")]
        public string _refcountformatted        { get { return $"{RefCount}"; } set { RefCount = value.ToInt32Ex(); ; } }

#endif

    }

    /// <summary>
    /// (Serializable)Adjust bit address pointing in program memory slots.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgBitAdjustPoint : IMemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgBitAdjustPoint() { }

        #endregion

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramSymbolVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Used to (de)serialize <see cref="Offset" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The offset content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _offsetformatted { get { return $"{Offset}"; } set { Offset = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the relative bit offset to add.
        /// </summary>
        /// <value>
        /// The offset as an integer.
        /// </value>
        [XmlIgnore]
        public int Offset { get; private set; }

        #endregion

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            if (Offset < 10) return $"Adjust {Offset} bit(s)";
            return $"Adjust 0x{Offset:X} bit(s)";
        }

#if FULLPICXML

        /// <summary>
        /// Gets the bit address.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        [XmlIgnore]
        public int Begin { get; private set; }

        [XmlIgnore]
        public int End { get; private set; }

        [XmlAttribute(AttributeName = "_modsrc", Form = XmlSchemaForm.None, Namespace = "")]
        public string ModSrc { get; set; }

        [XmlIgnore]
        public int RefCount { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _addrformatted        { get { return $"0x{Addr:X}"; } set { Addr = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_begin", Form = XmlSchemaForm.None, Namespace = "")]
        public string _beginformatted        { get { return $"0x{Begin:X}"; } set { Begin = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_end", Form = XmlSchemaForm.None, Namespace = "")]
        public string _endformatted        { get { return $"0x{End:X}"; } set { End = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_refcount", Form = XmlSchemaForm.None, Namespace = "")]
        public string _refcountformatted        { get { return $"{RefCount}"; } set { RefCount = value.ToInt32Ex(); ; } }

#endif

    }

    /// <summary>
    /// (Serializable)Code memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CodeSector : ProgMemorySection, IMemProgramRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Code;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CodeSector() { }

        #endregion

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramRegionVisitor v) { v.Visit(this); }

        #endregion

    }

    /// <summary>
    /// (Serializable)Calibration data zone memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class CalDataZone : ProgMemorySection, IMemProgramRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Calib;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CalDataZone() { }

        #endregion

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramRegionVisitor v) { v.Visit(this); }

        #endregion

    }

    /// <summary>
    /// (Serializable)Test zone memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class TestZone : ProgMemoryRegion, IMemProgramRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Other;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TestZone() { }

        #endregion

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramRegionVisitor v) { v.Visit(this); }

        #endregion

    }

    /// <summary>
    /// (Serializable)User IDs memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class UserIDSector : ProgMemorySection, IMemProgramRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.UserID;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UserIDSector() { }

        #endregion

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramRegionVisitor v) { v.Visit(this); }

        #endregion

    }

    /// <summary>
    /// (Serializable)Revision IDs sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class RevisionIDSector : ProgMemoryRegion, IMemProgramRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.RevisionID;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RevisionIDSector() { }

        #endregion

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramRegionVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the device ID to silicon revision relationship.
        /// </summary>
        /// <value>
        /// The device IDs to revision levels.
        /// </value>
        [XmlElement(ElementName = "DEVIDToRev", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DEVIDToRev> DEVIDToRev { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Device IDs sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceIDSector : ProgMemorySection, IMemProgramRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.DeviceID;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DeviceIDSector() { }

        #endregion

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramRegionVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Device IDs to silicon revision level.
        /// </summary>
        /// <value>
        /// The Device IDs
        /// </value>
        [XmlElement(ElementName = "DEVIDToRev", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DEVIDToRev> DEVIDToRev { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="Mask" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The mask content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "mask", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _maskformatted { get { return $"0x{Mask:X}"; } set { Mask = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the mask to isolate device ID.
        /// </summary>
        /// <value>
        /// The mask.
        /// </value>
        [XmlIgnore]
        public int Mask { get; private set; }

        /// <summary>
        /// Used to (de)serialize <see cref="Value"/> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The value content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "value", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _valueformatted { get { return $"0x{Value:X}"; } set { Value = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the generic value of device ID
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [XmlIgnore]
        public int Value { get; private set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Device Configuration Register field semantic checksum address range.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldSemanticChecksum : MemoryAddrRange
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Other;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCRFieldSemanticChecksum() { }

        #endregion

        #region Properties

        public override MemoryDomain MemoryDomain => MemoryDomain.Prog;

        /// <summary>
        /// Used to (de)serialize <see cref="ChecksumAlgo" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The checksumalgo content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "checksumalgo", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _checksumalgoformatted { get { return $"0x{ChecksumAlgo:X}"; } set { ChecksumAlgo = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the checksum algorithm code.
        /// </summary>
        /// <value>
        /// The checksum algo. codeas an integer.
        /// </value>
        [XmlIgnore]
        public int ChecksumAlgo { get; private set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Device Configuration Register field pattern semantic.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldSemantic : IMemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCRFieldSemantic() { }

        #endregion

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramSymbolVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the checksum area and algorithm.
        /// </summary>
        /// <value>
        /// The checksum info.
        /// </value>
        [XmlElement(ElementName = "Checksum", Form = XmlSchemaForm.None, Namespace = "")]
        public DCRFieldSemanticChecksum Checksum { get; set; }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>
        /// The name as a string
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the description of the field value.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets the when condition for the field value.
        /// </summary>
        /// <value>
        /// The when expression as a string.
        /// </value>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        /// <summary>
        /// Gets a value indicating whether this configuration pattern is hidden.
        /// </summary>
        /// <value>
        /// True if this configuration is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this configuration pattern is hidden to language tools.
        /// </summary>
        /// <value>
        /// True if this configuration is language hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="OscModeIDRef" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The oscmodeidref content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "oscmodeidref", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _oscmodeidrefformatted { get { return $"{OscModeIDRef}"; } set { OscModeIDRef = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the oscillator mode identifier reference.
        /// </summary>
        /// <value>
        /// The oscillator mode identifier reference as an integer.
        /// </value>
        [XmlIgnore]
        public int OscModeIDRef { get; private set; }

        [XmlAttribute(AttributeName = "_defeatcoercion", Form = XmlSchemaForm.None, Namespace = "")]
        public string DefeatCoercion { get; set; }

        /// <summary>
        /// Gets the memory mode identifier reference.
        /// </summary>
        /// <value>
        /// The memory mode identifier reference as a string.
        /// </value>
        [XmlAttribute(AttributeName = "memmodeidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string MemModeIDRef { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Device Configuration Register Field definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRFieldDef : IMemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCRFieldDef() { }

        #endregion

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramSymbolVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configuration field semantics for various configuration values.
        /// </summary>
        /// <value>
        /// The field semantics.
        /// </value>
        [XmlElement(ElementName = "DCRFieldSemantic", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DCRFieldSemantic> DCRFieldSemantics { get; set; }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>
        /// The field name.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the name of the bitfield.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the description of the field.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="BitAddr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The bitaddr content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "_baddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _bitaddrformatted { get { return $"{BitAddr}"; } set { BitAddr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bit position of the field.
        /// </summary>
        /// <remarks>
        /// This value must be set by caller.
        /// </remarks>
        /// <value>
        /// The lowest bit starting this field.
        /// </value>
        [XmlIgnore]
        public int BitAddr { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The nzwidth content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _nzwidthformatted { get { return $"{NzWidth}"; } set { NzWidth = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bitwidth of the field.
        /// </summary>
        /// <value>
        /// The width in number of bits.
        /// </value>
        [XmlIgnore]
        public int NzWidth { get; private set; }

        /// <summary>
        /// Used to (de)serialize <see cref="Mask" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The mask as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "mask", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _maskformatted { get { return $"0x{Mask:X}"; } set { Mask = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bitmask of the field in the register.
        /// </summary>
        /// <value>
        /// The bitmask as an integer.
        /// </value>
        [XmlIgnore]
        public int Mask { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this configuration field is hidden.
        /// </summary>
        /// <value>
        /// True if this field is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets a value indicating whether this configuration field is hidden to language tools.
        /// </summary>
        /// <value>
        /// True if this field is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        /// <summary>
        /// Gets a value indicating whether this configuration field is hidden to the MPLAB IDE.
        /// </summary>
        /// <value>
        /// True if this field is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isidehidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIDEHidden { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Device Configuration Register mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRMode : IMemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCRMode() { }

        #endregion

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramSymbolVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the fields of the configuration register.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        [XmlElement(ElementName = "DCRFieldDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DCRFieldDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ProgBitAdjustPoint))]
        public List<object> Fields { get; set; }

        /// <summary>
        /// Gets the identifier of the mode (usually "DS.0").
        /// </summary>
        /// <value>
        /// The identifier as a string.
        /// </value>
        [XmlAttribute(AttributeName = "id", Form = XmlSchemaForm.None, Namespace = "")]
        public string ID { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Device Configuration Register illegal definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRDefIllegal : IMemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCRDefIllegal() { }

        #endregion

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramSymbolVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the when pattern of the illegal condition.
        /// </summary>
        /// <value>
        /// The when string.
        /// </value>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        /// <summary>
        /// Gets the description of the illegal condition.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Device Configuration Register definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCRDef : IMemProgramSymbolAcceptor
    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCRDef() { }

        #endregion

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramSymbolVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the illegal configuration values (if any).
        /// </summary>
        /// <value>
        /// The illegal configuration values.
        /// </value>
        [XmlElement(ElementName = "Illegal", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DCRDefIllegal> Illegals { get; set; }

        /// <summary>
        /// Gets a list of Device Configuration Register modes.
        /// </summary>
        /// <value>
        /// A list of modes.
        /// </value>
        [XmlArray(ElementName = "DCRModeList", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem(ElementName = "DCRMode", Type = typeof(DCRMode), Form = XmlSchemaForm.None, IsNullable = false)]
        public List<DCRMode> DCRModes { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The addr content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _addrformatted { get { return $"0x{Addr:X}"; } set { Addr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the memory address of the configuration register
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary>
        /// Gets the name of the register.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the name of the register.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the description of the configuration register.
        /// </summary>
        /// <value>
        /// The description as a string
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The nzwidth content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _nzwidthformatted { get { return $"{NzWidth}"; } set { NzWidth = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bitwidth of the register.
        /// </summary>
        /// <value>
        /// The bitwidth of the register.
        /// </value>
        [XmlIgnore]
        public int NzWidth { get; private set; }

        /// <summary>
        /// Used to (de)serialize <see cref="Impl" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The impl content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "impl", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _implformatted { get { return $"0x{Impl:X}"; } set { Impl = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the implemented bits mask.
        /// </summary>
        /// <value>
        /// The implementation mask as an integer.
        /// </value>
        [XmlIgnore]
        public int Impl { get; private set; }

        /// <summary>
        /// Gets the access of the register's bits.
        /// </summary>
        /// <value>
        /// The access bits as a string.
        /// </value>
        [XmlAttribute(AttributeName = "access", Form = XmlSchemaForm.None, Namespace = "")]
        public string Access { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="Default" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The default content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "default", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _defaultformatted { get { return $"0x{Default:X}"; } set { Default = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the default value of the register.
        /// </summary>
        /// <value>
        /// The default value as an integer.
        /// </value>
        [XmlIgnore]
        public int Default { get; private set; }

        /// <summary>
        /// Used to (de)serialize <see cref="FactoryDefault" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The factorydefault content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "factorydefault", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _factorydefaultformatted { get { return $"0x{FactoryDefault:X}"; } set { FactoryDefault = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the factory default value of the register.
        /// </summary>
        /// <value>
        /// The factory default value as an integer.
        /// </value>
        [XmlIgnore]
        public int FactoryDefault { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this register is hidden to language tools.
        /// </summary>
        /// <value>
        /// True if this register is language hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="UnimplVal" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The unimplval content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "unimplval", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _unimplvalformatted { get { return $"{UnimplVal}"; } set { UnimplVal = value.ToInt32Ex(); } }

        [XmlIgnore]
        public int UnimplVal { get; private set; }

        /// <summary>
        /// Used to (de)serialize <see cref="Unused" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The unused content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "unused", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _unusedformatted { get { return $"{Unused}"; } set { Unused = value.ToInt32Ex(); } }

        [XmlIgnore]
        public int Unused { get; private set; }

        /// <summary>
        /// Used to (de)serialize <see cref="UseInChecksum" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The useinchecksum content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "useinchecksum", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _useinchecksumformatted { get { return $"0x{UseInChecksum:X}"; } set { UseInChecksum = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bitmask to use in checksum computation.
        /// </summary>
        /// <value>
        /// The bitmask as an integer.
        /// </value>
        [XmlIgnore]
        public int UseInChecksum { get; private set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Configuration Fuses memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ConfigFuseSector : ProgMemoryRegion, IMemProgramRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Config;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfigFuseSector() { }

        #endregion

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramRegionVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configuration registers definitions.
        /// </summary>
        /// <value>
        /// The definitions.
        /// </value>
        [XmlElement(ElementName = "DCRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DCRDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ProgByteAdjustPoint))]
        public List<object> Defs { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Background debugger vector memroy sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class BACKBUGVectorSector : ProgMemoryRegion, IMemProgramRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Debugger;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BACKBUGVectorSector() { }

        #endregion

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramRegionVisitor v) { v.Visit(this); }

        #endregion

    }

    /// <summary>
    /// (Serializable)Data EEPROM memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class EEDataSector : ProgMemorySection, IMemProgramRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.EEData;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EEDataSector() { }

        #endregion

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramRegionVisitor v) { v.Visit(this); }

        #endregion

    }

    /// <summary>
    /// (Serializable)Device Information Area (DIA) register.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DeviceRegister : IMemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DeviceRegister() { }

        #endregion

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramSymbolVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Used to (de)serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The addr content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _addrformatted { get { return $"0x{Addr:X}"; } set { Addr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the address of the register.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary>
        /// Gets the name of the register.
        /// </summary>
        /// <value>
        /// The register name.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The nzwidth content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _nzwidthformatted { get { return $"{NzWidth}"; } set { NzWidth = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bitwidth of the register.
        /// </summary>
        /// <value>
        /// The bitwidth as an integer.
        /// </value>
        [XmlIgnore]
        public int NzWidth { get; private set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Device Inormation Area register array.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DIARegisterArray : IMemProgramSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DIARegisterArray() { }

        #endregion

        #region IMemProgramSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramSymbolVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        [XmlElement(ElementName = "Register", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DeviceRegister))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ProgByteAdjustPoint))]
        public List<object> Registers { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The addr content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _addrformatted { get { return $"0x{Addr:X}"; } set { Addr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the starting memory address of the DIA registers.
        /// </summary>
        /// <value>
        /// The memory address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary>
        /// Gets the name of the DIA registers array.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Device Information Area memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DIASector : ProgMemoryRegion, IMemProgramRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Other;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DIASector() { }

        #endregion

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramRegionVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the register arrays.
        /// </summary>
        /// <value>
        /// The register arrays.
        /// </value>
        [XmlElement(ElementName = "RegisterArray", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DIARegisterArray> RegisterArrays { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Device Configuration Information memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DCISector : ProgMemoryRegion, IMemProgramRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Other;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCISector() { }

        #endregion

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramRegionVisitor v) { v.Visit(this); }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configuration information registers.
        /// </summary>
        /// <value>
        /// The registers.
        /// </value>
        [XmlElement(ElementName = "Register", Form = XmlSchemaForm.None, Namespace = "")]
        public List<DeviceRegister> Registers { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)External Code memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ExtCodeSector : ProgMemoryRegion, IMemProgramRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Code;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExtCodeSector() { }

        #endregion

        #region IMemProgramRegionAcceptor implementation

        /// <summary>
        /// The <see cref="Accept"/> method accepts a program memory region visitor and calls the appropriate
        /// "Visit()" method for this program memory region.
        /// </summary>
        /// <param name="v">The program memory region visitor to accept.</param>
        public void Accept(IMemProgramRegionVisitor v) { v.Visit(this); }

        #endregion

    }

    /// <summary>
    /// (Serializable)Interrupt Vector area.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class VectorArea
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public VectorArea() { }

        #endregion

        #region Properties

        /// <summary>
        /// Used to (de)serialize <see cref="NzSize" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The nzsize content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "nzsize", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _nzsizeformatted { get { return $"0x{NzSize:X}"; } set { NzSize = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bytes size of the area.
        /// </summary>
        /// <value>
        /// The size in bytes as an integer.
        /// </value>
        [XmlIgnore]
        public int NzSize { get; private set; }

        /// <summary>
        /// Gets the identifier of the vector area.
        /// </summary>
        /// <value>
        /// The identifier of the region as a string.
        /// </value>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Program memory space.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class ProgramSpace
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProgramSpace() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the program memroy sectors.
        /// </summary>
        /// <value>
        /// The various sectors.
        /// </value>
        [XmlElement(ElementName = "CodeSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(CodeSector))]
        [XmlElement(ElementName = "CalDataZone", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(CalDataZone))]
        [XmlElement(ElementName = "TestZone", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(TestZone))]
        [XmlElement(ElementName = "UserIDSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(UserIDSector))]
        [XmlElement(ElementName = "RevisionIDSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(RevisionIDSector))]
        [XmlElement(ElementName = "DeviceIDSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DeviceIDSector))]
        [XmlElement(ElementName = "ConfigFuseSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ConfigFuseSector))]
        [XmlElement(ElementName = "BACKBUGVectorSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(BACKBUGVectorSector))]
        [XmlElement(ElementName = "EEDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(EEDataSector))]
        [XmlElement(ElementName = "DIASector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DIASector))]
        [XmlElement(ElementName = "DCISector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DCISector))]
        [XmlElement(ElementName = "ExtCodeSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(ExtCodeSector))]
        public List<ProgMemoryRegion> Sectors { get; set; }

        /// <summary>
        /// Gets the interrupt vector area.
        /// </summary>
        /// <value>
        /// The vector area.
        /// </value>
        [XmlElement(ElementName = "VectorArea", Form = XmlSchemaForm.None, Namespace = "")]
        public List<VectorArea> VectorArea { get; set; }

        #endregion

    }

    #endregion

    #region DataSpace XML element

    /// <summary>
    /// A data memory addresses range.
    /// </summary>
    public abstract class DataMemoryRange : MemoryAddrRange
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataMemoryRange() { }

        #endregion

        public override MemoryDomain MemoryDomain => MemoryDomain.Data;

    }

    /// <summary>
    /// (Serializable)A Data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class DataMemoryRegion : DataMemoryRange
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataMemoryRegion() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the region.
        /// </summary>
        /// <value>
        /// The identifier of the region as a string.
        /// </value>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)a memory banked region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public abstract class DataMemoryBankedRegion : DataMemoryRegion
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataMemoryBankedRegion() { }

        #endregion

        #region Properties

        /// <summary>
        /// Used to (de)serialize <see cref="Bank" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The bank number content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "bank", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _bankformatted { get { return $"{Bank}"; } set { Bank = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the memory bank number.
        /// </summary>
        /// <value>
        /// The bank number as an integer.
        /// </value>
        [XmlIgnore]
        public int Bank { get; private set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Adjust byte address pointing in data memory spaces.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataByteAdjustPoint : IMemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataByteAdjustPoint() { }

        #endregion

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Used to (de)serialize <see cref="Offset" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The offset content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _offsetformatted { get { return $"{Offset}"; } set { Offset = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the relative byte offset to add.
        /// </summary>
        /// <value>
        /// The offset as an integer.
        /// </value>
        [XmlIgnore]
        public int Offset { get; private set; }

        #endregion

        public override string ToString()
        {
            if (Offset < 10) return $"Adjust {Offset} byte(s)";
            return $"Adjust 0x{Offset:X} byte(s)";
        }

#if FULLPICXML

        /// <summary>
        /// Gets the byte address.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        [XmlIgnore]
        public int Begin { get; private set; }

        [XmlIgnore]
        public int End { get; private set; }

        [XmlAttribute(AttributeName = "_modsrc", Form = XmlSchemaForm.None, Namespace = "")]
        public string ModSrc { get; set; }

        [XmlIgnore]
        public int RefCount { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _addrformatted        { get { return $"0x{Addr:X}"; } set { Addr = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_begin", Form = XmlSchemaForm.None, Namespace = "")]
        public string _beginformatted        { get { return $"0x{Begin:X}"; } set { Begin = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_end", Form = XmlSchemaForm.None, Namespace = "")]
        public string _endformatted        { get { return $"0x{End:X}"; } set { End = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_refcount", Form = XmlSchemaForm.None, Namespace = "")]
        public string _refcountformatted        { get { return $"{RefCount}"; } set { RefCount = value.ToInt32Ex(); ; } }

#endif

    }

    /// <summary>
    /// (Serializable)Adjust bit address pointing in data memory slots.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataBitAdjustPoint : IMemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataBitAdjustPoint() { }

        #endregion

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Used to (de)serialize <see cref="Offset" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The offset content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "offset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _offsetformatted { get { return $"{Offset}"; } set { Offset = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the relative bit offset to add.
        /// </summary>
        /// <value>
        /// The offset as an integer.
        /// </value>
        [XmlIgnore]
        public int Offset { get; private set; }

        #endregion

        public override string ToString()
        {
            if (Offset < 10) return $"Adjust {Offset} bit(s)";
            return $"Adjust 0x{Offset:X} bit(s)";
        }

#if FULLPICXML

        /// <summary>
        /// Gets the byte address.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        [XmlIgnore]
        public int Begin { get; private set; }

        [XmlIgnore]
        public int End { get; private set; }

        [XmlAttribute(AttributeName = "_modsrc", Form = XmlSchemaForm.None, Namespace = "")]
        public string ModSrc { get; set; }

        [XmlIgnore]
        public int RefCount { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        public string _addrformatted        { get { return $"0x{Addr:X}"; } set { Addr = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_begin", Form = XmlSchemaForm.None, Namespace = "")]
        public string _beginformatted        { get { return $"0x{Begin:X}"; } set { Begin = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_end", Form = XmlSchemaForm.None, Namespace = "")]
        public string _endformatted        { get { return $"0x{End:X}"; } set { End = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_refcount", Form = XmlSchemaForm.None, Namespace = "")]
        public string _refcountformatted        { get { return $"{RefCount}"; } set { RefCount = value.ToInt32Ex(); ; } }

#endif

    }

    /// <summary>
    /// (Serializable)SFR Field semantic.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRFieldSemantic : IMemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SFRFieldSemantic() { }

        #endregion

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the semantic description.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets the condition of the semantic.
        /// </summary>
        /// <value>
        /// The expression associated with the semantic.
        /// </value>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)SFR bits-field definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRFieldDef : IMemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SFRFieldDef() { }

        #endregion

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the various sematics of this SFR field.
        /// </summary>
        /// <value>
        /// The SFR field semantics.
        /// </value>
        [XmlElement(ElementName = "SFRFieldSemantic", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SFRFieldSemantic> SFRFieldSemantics { get; set; }

        /// <summary>
        /// Gets the name of this SFR Field.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the name of this SFR Field.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the description of this SFR Field.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The nzwidth content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _nzwidthformatted { get { return $"{NzWidth}"; } set { NzWidth = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bit width of this SFR field.
        /// </summary>
        /// <value>
        /// The bit width as an integer.
        /// </value>
        [XmlIgnore]
        public int NzWidth { get; private set; }

        /// <summary>
        /// Used to (de)serialize <see cref="Mask" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The mask content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "mask", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _maskformatted { get { return $"0x{Mask:X}"; } set { Mask = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bit mask of this SFR field.
        /// </summary>
        /// <value>
        /// The bit mask as an integer.
        /// </value>
        [XmlIgnore]
        public int Mask { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this SFR Field is hidden to language tools.
        /// </summary>
        /// <value>
        /// True if this field is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SFR Field is hidden.
        /// </summary>
        /// <value>
        /// True if this field is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SFR Field is hidden to MPLAB IDE.
        /// </summary>
        /// <value>
        /// True if this field is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isidehidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIDEHidden { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)SFR Fields definitions for a given mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRMode : IMemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SFRMode() { }

        #endregion

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SFR fields definitions.
        /// </summary>
        /// <value>
        /// The SFR fields or adjustpoints.
        /// </value>
        [XmlElement(ElementName = "SFRFieldDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRFieldDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataBitAdjustPoint))]
        public List<object> Fields { get; set; }

        /// <summary>
        /// Gets the identifier of the mode (usually "DS.0").
        /// </summary>
        /// <value>
        /// The identifier as a string.
        /// </value>
        [XmlAttribute(AttributeName = "id", Form = XmlSchemaForm.None, Namespace = "")]
        public string ID { get; set; }

        /// <summary>
        /// Gets the Power-ON Reset value of the mode. Not used.
        /// </summary>
        [XmlAttribute(AttributeName = "por", Form = XmlSchemaForm.None, Namespace = "")]
        public string POR { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)List of SFR modes.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRModeList : IMemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SFRModeList() { }

        #endregion

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SFR modes.
        /// </summary>
        /// <value>
        /// The modes.
        /// </value>
        [XmlElement(ElementName = "SFRMode", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SFRMode> SFRModes { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Special Function Register (SFR) definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRDef : IMemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SFRDef() { }

        #endregion

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets list of modes for this SFR.
        /// </summary>
        /// <value>
        /// A list of modes.
        /// </value>
        [XmlArray("SFRModeList", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("SFRMode", typeof(SFRMode), Form = XmlSchemaForm.None, Namespace = "", IsNullable = false)]
        public List<SFRMode> SFRModes { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The addr content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _addrformatted { get { return $"0x{Addr:X}"; } set { Addr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the data memory address of this SFR.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary>
        /// Gets the name of this SFR.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the name of this SFR.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the description of this SFR.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The nzwidth content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _nzwidthformatted { get { return $"{NzWidth}"; } set { NzWidth = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bitwidth of this SFR.
        /// </summary>
        /// <value>
        /// The bitwidth as an integer.
        /// </value>
        [XmlIgnore]
        public int NzWidth { get; private set; }

        /// <summary>
        /// Gets the byte width of thi SFR.
        /// </summary>
        /// <value>
        /// The width of the SFR in number of bytes.
        /// </value>
        [XmlIgnore]
        public int ByteWidth => (NzWidth + 7) >> 3;

        /// <summary>
        /// Used to (de)serialize <see cref="Impl" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The impl content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "impl", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _implformatted { get { return $"0x{Impl:X}"; } set { Impl = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the implemented bits mask of this SFR.
        /// </summary>
        /// <value>
        /// The implementation mask as an integer.
        /// </value>
        [XmlIgnore]
        public int Impl { get; private set; }

        /// <summary>
        /// Gets the access mode bits for this SFR.
        /// </summary>
        /// <value>
        /// The access mode bits descriptor as a string.
        /// </value>
        [XmlAttribute(AttributeName = "access", Form = XmlSchemaForm.None, Namespace = "")]
        public string Access { get; set; }

        /// <summary>
        /// Gets the Master Clear (MCLR) bits values of this SFR.
        /// </summary>
        /// <value>
        /// The bits value as a string.
        /// </value>
        [XmlAttribute(AttributeName = "mclr", Form = XmlSchemaForm.None, Namespace = "")]
        public string MCLR { get; set; }

        /// <summary>
        /// Gets the Power-ON Reset bits values of this SFR.
        /// </summary>
        /// <value>
        /// The bits value as a string.
        /// </value>
        [XmlAttribute(AttributeName = "por", Form = XmlSchemaForm.None, Namespace = "")]
        public string POR { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SFR is indirect.
        /// </summary>
        /// <value>
        /// True if this register is indirect, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isindirect", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIndirect { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SFR is volatile.
        /// </summary>
        /// <value>
        /// True if this register is volatile, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isvolatile", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsVolatile { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SFR is hidden.
        /// </summary>
        /// <value>
        /// True if this SFR is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "ishidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SFR is hidden to language tools.
        /// </summary>
        /// <value>
        /// True if this SFR is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "islanghidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsLangHidden { get; set; }

        /// <summary>
        /// Gets a value indicating whether this SFR is hidden to MPLAB IDE.
        /// </summary>
        /// <value>
        /// True if this SFR is hidden, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isidehidden", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsIDEHidden { get; set; }

        /// <summary>
        /// Gets the name of the peripheral this SFR is the base address of.
        /// </summary>
        /// <value>
        /// The name of the peripheral.
        /// </value>
        [XmlAttribute(AttributeName = "baseofperipheral", Form = XmlSchemaForm.None, Namespace = "")]
        public string BaseOfPeripheral { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="NMMRID" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The nmmrid content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "nmmrid", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _nmmridformatted { get { return $"0x{NMMRID:X}"; } set { NMMRID = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the Non-Memory-Mapped-Register identifier of the SFR.
        /// </summary>
        /// <value>
        /// The identifier as an integer.
        /// </value>
        [XmlIgnore]
        public int NMMRID { get; private set; }

        #endregion

#if FULLPICXML

        [XmlAttribute(AttributeName = "_modsrc", Form = XmlSchemaForm.None, Namespace = "")]
        public string ModSrc { get; set; }

        [XmlIgnore]
        public int RefCount { get; private set; }

        [XmlIgnore]
        public int Begin { get; private set; }

        [XmlIgnore]
        public int End { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_refcount", Form = XmlSchemaForm.None, Namespace = "")]
        public string _refcountformatted        { get { return $"{RefCount}"; } set { RefCount = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_begin", Form = XmlSchemaForm.None, Namespace = "")]
        public string _beginformatted        { get { return $"0x{Begin:X}"; } set { Begin = value.ToInt32Ex(); } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [XmlAttribute(AttributeName = "_end", Form = XmlSchemaForm.None, Namespace = "")]
        public string _endformatted        { get { return $"0x{End:X}"; } set { End = value.ToInt32Ex(); } }

#endif

    }

    /// <summary>
    /// (Serializable)Mirrored registers area.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class Mirror : IMemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Mirror() { }

        #endregion

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Used to (de)serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The addr content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _addrformatted { get { return $"0x{Addr:X}"; } set { Addr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the memory address of the mirror
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary>
        /// Used to (de)serialize <see cref="NzSize" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The nzsize content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "nzsize", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _nzsizeformatted { get { return $"0x{NzSize:X}"; } set { NzSize = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the size in bytes of the mirrored area.
        /// </summary>
        /// <value>
        /// The size as an integer.
        /// </value>
        [XmlIgnore]
        public int NzSize { get; private set; }

        /// <summary>
        /// Gets the region identifier reference of the mirrored memory region.
        /// </summary>
        /// <value>
        /// The region identifier reference as a string.
        /// </value>
        [XmlAttribute(AttributeName = "regionidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionIDRef { get; set; }

        #endregion

    }

    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class JoinedSFRDef : IMemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JoinedSFRDef() { }

        #endregion

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the individual SFRs composing the join.
        /// </summary>
        /// <value>
        /// The SFRs.
        /// </value>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "", Order = 1)]
        public List<SFRDef> SFRs { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The addr content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _addrformatted { get { return $"0x{Addr:X}"; } set { Addr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the memory address of the joinded SFR
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary>
        /// Gets the name of the joinded SFR.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cname", Form = XmlSchemaForm.None, Namespace = "")]
        public string CName { get; set; }

        /// <summary>
        /// Gets the description of the joinded SFR.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Gets the name of the joinded SFR.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The nzwidth content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _nzwidthformatted { get { return $"{NzWidth}"; } set { NzWidth = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bit width of the joined SFR.
        /// </summary>
        /// <value>
        /// The bit width as an integer.
        /// </value>
        [XmlIgnore]
        public int NzWidth { get; private set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Selection of a SFR.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SelectSFR : IMemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SelectSFR() { }

        #endregion

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SFR being selected.
        /// </summary>
        /// <value>
        /// The SFR definition.
        /// </value>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        public SFRDef SFR { get; set; }

        /// <summary>
        /// Gets the (optional) when condition.
        /// </summary>
        /// <value>
        /// The condition as a string.
        /// </value>
        [XmlAttribute(AttributeName = "when", Form = XmlSchemaForm.None, Namespace = "")]
        public string When { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Multiplexed SFRs definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class MuxedSFRDef : IMemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MuxedSFRDef() { }

        #endregion

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the selections of SFRs.
        /// </summary>
        /// <value>
        /// The selections.
        /// </value>
        [XmlElement(ElementName = "SelectSFR", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SelectSFR> SelectSFRs { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="Addr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The addr content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "_addr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _addrformatted { get { return $"0x{Addr:X}"; } set { Addr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the memory address of the multiplex.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int Addr { get; private set; }

        /// <summary>
        /// Used to (de)serialize <see cref="NzWidth" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The nzwidth content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "nzwidth", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _nzwidthformatted { get { return $"{NzWidth}"; } set { NzWidth = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bit width of the multiplex.
        /// </summary>
        /// <value>
        /// The bit width as an integer.
        /// </value>
        [XmlIgnore]
        public int NzWidth { get; private set; }

        /// <summary>
        /// Gets the byte width of the multiplex.
        /// </summary>
        /// <value>
        /// The byte width as an integer.
        /// </value>
        [XmlIgnore]
        public int ByteWidth => (NzWidth + 7) >> 3;

        #endregion

    }

    /// <summary>
    /// (Serializable)DMA Register mirror.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DMARegisterMirror : IMemDataSymbolAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DMARegisterMirror() { }

        #endregion

        #region IMemDataSymbolAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and calls the
        /// appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept.</param>
        public void Accept(IMemDataSymbolVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name reference.
        /// </summary>
        /// <value>
        /// The name reference as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cnameref", Form = XmlSchemaForm.None, Namespace = "")]
        public string CNameRef { get; set; }

        /// <summary>
        /// Gets the name suffix.
        /// </summary>
        /// <value>
        /// The name suffix as a string.
        /// </value>
        [XmlAttribute(AttributeName = "cnamesuffix", Form = XmlSchemaForm.None, Namespace = "")]
        public string CNameSuffix { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Special Function Registers data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class SFRDataSector : DataMemoryBankedRegion, IMemDataRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.SFR;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SFRDataSector() { }

        #endregion

        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the various SFRs (simple, joined, muxed, mirrored, DMA, adjusted) defined in this memory region.
        /// </summary>
        /// <value>
        /// The SFRs, Muxed, Joined, ... definitions
        /// </value>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataByteAdjustPoint))]
        [XmlElement(ElementName = "JoinedSFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(JoinedSFRDef))]
        [XmlElement(ElementName = "MuxedSFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(MuxedSFRDef))]
        [XmlElement(ElementName = "Mirror", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(Mirror))]
        [XmlElement(ElementName = "RegisterMirror", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DMARegisterMirror))]
        public List<object> SFRs { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)General Purpose Registers (GPR) data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class GPRDataSector : DataMemoryBankedRegion, IMemDataRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.GPR;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GPRDataSector() { }

        #endregion

        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the shadow identifier reference, if any
        /// </summary>
        /// <value>
        /// The shadow identifier reference as a string.
        /// </value>
        [XmlAttribute(AttributeName = "shadowidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string ShadowIDRef { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="ShadowOffset" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The shadowoffset content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "shadowoffset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _shadowoffsetformatted { get { return $"0x{ShadowOffset:X}"; } set { ShadowOffset = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the shadow memory address offset.
        /// </summary>
        /// <value>
        /// The shadow offset as an integer.
        /// </value>
        [XmlIgnore]
        public int ShadowOffset { get; private set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Dual Port Registers data memory sector.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DPRDataSector : DataMemoryBankedRegion, IMemDataRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.DPR;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DPRDataSector() { }

        #endregion

        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SFR as Dual Port registers.
        /// </summary>
        /// <value>
        /// The SFRs.
        /// </value>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRDef))]
        [XmlElement(ElementName = "AdjustPoint", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DataByteAdjustPoint))]
        public List<object> SFRs { get; set; }

        /// <summary>
        /// Gets the shadowed memroy region identifier reference.
        /// </summary>
        /// <value>
        /// The shadow identifier reference as a string.
        /// </value>
        [XmlAttribute(AttributeName = "shadowidref", Form = XmlSchemaForm.None, Namespace = "")]
        public string ShadowIDRef { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="ShadowOffset" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The shadowoffset content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "shadowoffset", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _shadowoffsetformatted { get { return $"0x{ShadowOffset:X}"; } set { ShadowOffset = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the shadow memory offset.
        /// </summary>
        /// <value>
        /// The shadow offset as an integer.
        /// </value>
        [XmlIgnore]
        public int ShadowOffset { get; private set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Emulator memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class EmulatorZone : DataMemoryRegion, IMemDataRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Emulator;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EmulatorZone() { }

        #endregion

        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        #endregion

    }

    /// <summary>
    /// (Serializable)Non-Memory-Mapped-Register (NMMR) definitions.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class NMMRPlace : IMemDataRegionAcceptor
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NMMRPlace() { }

        #endregion

        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SFR definitions.
        /// </summary>
        /// <value>
        /// The SFR definitions.
        /// </value>
        [XmlElement(ElementName = "SFRDef", Form = XmlSchemaForm.None, Namespace = "")]
        public List<SFRDef> SFRDefs { get; set; }

        /// <summary>
        /// Gets the identifier of the NMMR region.
        /// </summary>
        /// <value>
        /// The identifier as a string.
        /// </value>
        [XmlAttribute(AttributeName = "regionid", Form = XmlSchemaForm.None, Namespace = "")]
        public string RegionID { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Linear data memory region.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class LinearDataSector : DataMemoryRange, IMemDataRegionAcceptor
    {
        public override MemorySubDomain MemorySubDomain => MemorySubDomain.Linear;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LinearDataSector() { }

        #endregion

        #region IMemDataRegionAcceptor interface

        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        public void Accept(IMemDataRegionVisitor v) => v.Visit(this);

        #endregion

        #region Properties

        /// <summary>
        /// Used to (de)serialize <see cref="BankSize" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The banksize content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "banksize", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _banksizeformatted { get { return $"0x{BankSize:X}"; } set { BankSize = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the bytes size of the linear memory bank.
        /// </summary>
        /// <value>
        /// The size in bytes as an integer.
        /// </value>
        [XmlIgnore]
        public int BankSize { get; private set; }

        /// <summary>
        /// Used to (de)serialize <see cref="BlockBeginAddr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The blockbeginaddr content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "blockbeginaddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _blockbeginaddrformatted { get { return $"0x{BlockBeginAddr:X}"; } set { BlockBeginAddr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the beginning address of the linear memory bank.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int BlockBeginAddr { get; private set; }

        /// <summary>
        /// Used to (de)serialize <see cref="BlockEndAddr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The blockendaddr content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "blockendaddr", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _blockendaddrformatted { get { return $"0x{BlockEndAddr:X}"; } set { BlockEndAddr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the ending address of the linear memory bank.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int BlockEndAddr { get; private set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Data memory regions regardless of PIC execution mode.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class RegardlessOfMode
    {

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RegardlessOfMode() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the data memory regions.
        /// </summary>
        /// <value>
        /// The memory regions.
        /// </value>
        [XmlElement(ElementName = "SFRDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(SFRDataSector))]
        [XmlElement(ElementName = "DPRDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(DPRDataSector))]
        [XmlElement(ElementName = "GPRDataSector", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(GPRDataSector))]
        [XmlElement(ElementName = "EmulatorZone", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(EmulatorZone))]
        [XmlElement(ElementName = "NMMRPlace", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(NMMRPlace))]
        public List<object> Regions { get; set; }

        #endregion

    }

    /// <summary>
    /// (Serializable)Data memory space.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class DataSpace
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataSpace() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the data memory regions regardless of PIC execution mode.
        /// </summary>
        /// <value>
        /// The memory regions.
        /// </value>
        [XmlElement(ElementName = "RegardlessOfMode", Form = XmlSchemaForm.None, Namespace = "")]
        public RegardlessOfMode RegardlessOfMode { get; set; }

        /// <summary>
        /// Gets the data memory regions when PIC is in traditional execution mode.
        /// </summary>
        /// <value>
        /// The memory regions (GPRs).
        /// </value>
        [XmlArray("TraditionalModeOnly", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("GPRDataSector", typeof(GPRDataSector), IsNullable = false, Namespace = "")]
        public List<GPRDataSector> TraditionalModeOnly { get; set; }

        /// <summary>
        /// Gets the data memory regions when PIC is in extended execution mode.
        /// </summary>
        /// <value>
        /// The memory regions (GPRs).
        /// </value>
        [XmlArray("ExtendedModeOnly", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("GPRDataSector", typeof(GPRDataSector), IsNullable = false)]
        public List<GPRDataSector> ExtendedModeOnly { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="EndAddr" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The endaddr content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "endaddr", Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _endaddrformatted { get { return $"0x{EndAddr:X}"; } set { EndAddr = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the highest (end) address of the data memory space.
        /// </summary>
        /// <value>
        /// The address as an integer.
        /// </value>
        [XmlIgnore]
        public int EndAddr { get; private set; }

        #endregion

    }

    #endregion

    #region PIC XML definition

    /// <summary>
    /// (Serializable)PIC definition.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public sealed class PIC
    {
        #region Locals

        // Maps the 'instructionsetid' to internal code.
        private static Dictionary<string, InstructionSetID> _mapInstrID = new Dictionary<string, InstructionSetID>() {
                { "pic16f77", InstructionSetID.PIC16 },
                { "cpu_mid_v10", InstructionSetID.PIC16_ENH },
                { "cpu_p16f1_v1", InstructionSetID.PIC16_ENH_V1 },
                { "egg", InstructionSetID.PIC18_EXTENDED },
                { "pic18", InstructionSetID.PIC18 },
                { "cpu_pic18f_v6", InstructionSetID.PIC18_ENHANCED }
            };

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PIC() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the architecture definition.
        /// </summary>
        /// <value>
        /// The architecture definition.
        /// </value>
        [XmlElement(ElementName = "ArchDef", Form = XmlSchemaForm.None, Namespace = "")]
        public ArchDef ArchDef { get; set; }

        /// <summary>
        /// Gets the instruction set identifier.
        /// </summary>
        /// <value>
        /// The instruction set definition.
        /// </value>
        [XmlElement(ElementName = "InstructionSet", Form = XmlSchemaForm.None, Namespace = "")]
        public InstructionSet InstructionSet { get; set; }

        /// <summary>
        /// Gets a list of interrupts (IRQ).
        /// </summary>
        /// <value>
        /// A list of interrupts.
        /// </value>
        [XmlArray("InterruptList", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("Interrupt", typeof(Interrupt), Form = XmlSchemaForm.None, IsNullable = false, Namespace = "")]
        public List<Interrupt> Interrupts { get; set; }

        /// <summary>
        /// Gets the program memory space.
        /// </summary>
        /// <value>
        /// The program memory space.
        /// </value>
        [XmlElement(ElementName = "ProgramSpace", Form = XmlSchemaForm.None, Namespace = "")]
        public ProgramSpace ProgramSpace { get; set; }

        /// <summary>
        /// Gets the data memory space.
        /// </summary>
        /// <value>
        /// The data memory space.
        /// </value>
        [XmlElement(ElementName = "DataSpace", Form = XmlSchemaForm.None, Namespace = "")]
        public DataSpace DataSpace { get; set; }

        /// <summary>
        /// Gets the SFRs in the DMA space.
        /// </summary>
        /// <value>
        /// The SFRs in DMA space.
        /// </value>
        [XmlArray("DMASpace", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("SFRDataSector", typeof(SFRDataSector), IsNullable = false, Namespace = "")]
        public List<SFRDataSector> DMASpace { get; set; }

        /// <summary>
        /// Gets the linear data memory region in the indirect space.
        /// </summary>
        /// <value>
        /// The linear data memory regions.
        /// </value>
        [XmlArray("IndirectSpace", Form = XmlSchemaForm.None, Namespace = "")]
        [XmlArrayItem("LinearDataSector", typeof(LinearDataSector), Form = XmlSchemaForm.None, IsNullable = false, Namespace = "")]
        public List<LinearDataSector> IndirectSpace { get; set; }

        /// <summary>
        /// Gets the PIC name.
        /// </summary>
        /// <value>
        /// The name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the PIC architecture name (16xxxx, 16Exxx, 18xxxx)
        /// </summary>
        /// <value>
        /// The architecture as a string.
        /// </value>
        [XmlAttribute(AttributeName = "arch", Form = XmlSchemaForm.None, Namespace = "")]
        public string Arch { get; set; }

        /// <summary>
        /// Gets the PIC description.
        /// </summary>
        /// <value>
        /// The description as a string.
        /// </value>
        [XmlAttribute(AttributeName = "desc", Form = XmlSchemaForm.None, Namespace = "")]
        public string Desc { get; set; }

        /// <summary>
        /// Used to (de)serialize <see cref="ProcID" /> property from/to hexadecimal string.
        /// </summary>
        /// <value>
        /// The procid content as an hexa string.
        /// </value>
        [XmlAttribute(AttributeName = "procid", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string _procidformatted { get { return $"0x{ProcID:X}"; } set { ProcID = value.ToInt32Ex(); } }

        /// <summary>
        /// Gets the unique processor identifier.
        /// </summary>
        /// <value>
        /// The identifier as an integer.
        /// </value>
        [XmlIgnore]
        public int ProcID { get; private set; }

        /// <summary>
        /// Gets the datasheet identifier of the PIC.
        /// </summary>
        /// <value>
        /// The identifier as a string.
        /// </value>
        [XmlAttribute(AttributeName = "dsid", Form = XmlSchemaForm.None, Namespace = "")]
        public string DsID { get; set; }

        [XmlAttribute(AttributeName = "dosid", Form = XmlSchemaForm.None, Namespace = "")]
        public string DosID { get; set; }

        /// <summary>
        /// Gets the indicator whether this PIC supports the PIC18 extended execution mode.
        /// Overriden by the dataspace definition containing - or not - extended-mode-only memory space.
        /// </summary>
        /// <value>
        /// True if this PIC supports extended execution mode, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isextended", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsExtended { get { return DataSpace?.ExtendedModeOnly?.Count > 0; } set { } }

        /// <summary>
        /// Gets a value indicating whether this PIC supports freezing of peripherals.
        /// </summary>
        /// <value>
        /// True if this PIC has freeze capabilities, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "hasfreeze", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool HasFreeze { get; set; }

        /// <summary>
        /// Gets a value indicating whether this PIC is debuggable.
        /// </summary>
        /// <value>
        /// True if this PIC is debuggable, false if not.
        /// </value>
        [XmlAttribute(AttributeName = "isdebuggable", DataType = "boolean", Form = XmlSchemaForm.None, Namespace = "")]
        public bool IsDebuggable { get; set; }

        [XmlAttribute(AttributeName = "informedby", Form = XmlSchemaForm.None, Namespace = "")]
        public string Informedby { get; set; }

        [XmlAttribute(AttributeName = "masksetid", Form = XmlSchemaForm.None, Namespace = "")]
        public string MaskSetID { get; set; }

        [XmlAttribute(AttributeName = "psid", Form = XmlSchemaForm.None, Namespace = "")]
        public string PsID { get; set; }

        /// <summary>
        /// Gets the name of the PIC this PIC is cloned from.
        /// </summary>
        /// <value>
        /// The cloned PIC name as a string.
        /// </value>
        [XmlAttribute(AttributeName = "clonedfrom", Form = XmlSchemaForm.None, Namespace = "")]
        public string ClonedFrom { get; set; }

        /// <summary>
        /// Gets the instruction set identifier of this PIC as a value from the <see cref="InstructionSetID"/> enumeration.
        /// </summary>
        /// <value>
        /// The instruction set as a value from <see cref="InstructionSetID"/> enumeration.
        /// </value>
        [XmlIgnore]
        public InstructionSetID GetInstructionSetID
        {
            get
            {
                if (InstructionSet == null)
                    return InstructionSetID.UNDEFINED;
                InstructionSetID id;
                if (!_mapInstrID.TryGetValue(InstructionSet.ID, out id))
                {
                    id = InstructionSetID.UNDEFINED;
                    if (Arch == "16xxxx") id = InstructionSetID.PIC16;
                    if (Arch == "16Exxx") id = InstructionSetID.PIC16_ENH_V1;
                    if (Arch == "18xxxx") id = (IsExtended ? InstructionSetID.PIC18_EXTENDED : InstructionSetID.PIC18);
                }
                return id;
            }
        }

        #endregion

    }

    #endregion

    #region PICCrownking Extensions

    public static partial class PICCrownkingExtensions
    {
        /// <summary>
        /// A PICCrownking extension method that gets a PIC descriptor.
        /// </summary>
        /// <param name="db">The PIC database to act on.</param>
        /// <param name="sPICName">Name of the PIC.</param>
        /// <returns>
        /// The PIC descriptor.
        /// </returns>
        public static PIC GetPIC(this PICCrownking db, string sPICName)
            => db.GetPICAsXML(sPICName)?.ToObject<PIC>();

    }

    #endregion

}


