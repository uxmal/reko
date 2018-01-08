using Microchip.Crownking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microchip.MemoryMapper
{
    /// <summary>
    /// A factory class which implements the PIC memory mapper based on individual PIC definition.
    /// </summary>
    public sealed class PICMemoryMapper : IPICMemoryMapper,
        IMemProgramRegionVisitor, IMemDataRegionVisitor, IMemDataSymbolVisitor, IMemTraitsSymbolVisitor
    {
        #region Locals

        #region Local classes

        /// <summary>
        /// Memory regions are keyed by domain/subdomain.
        /// </summary>
        internal class MemoryDomainKey
        {

            public MemoryDomain Domain { get; }
            public MemorySubDomain SubDomain { get; }

            public MemoryDomainKey(MemoryDomain dom, MemorySubDomain subdom)
            {
                Domain = dom;
                SubDomain = subdom;
            }

        }

        /// <summary>
        /// Comparer of two memory regions' domain/subdomain.
        /// </summary>
        class MemoryDomainKeyEqualityComparer : IEqualityComparer<MemoryDomainKey>
        {
            bool IEqualityComparer<MemoryDomainKey>.Equals(MemoryDomainKey x, MemoryDomainKey y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null)
                    return false;
                if (x.Domain == y.Domain)
                    return x.SubDomain == y.SubDomain;
                return false;
            }

            int IEqualityComparer<MemoryDomainKey>.GetHashCode(MemoryDomainKey key)
            {
                return ((int)key.Domain << (int)key.SubDomain).GetHashCode(); ;
            }
        }

        /// <summary>
        /// A PIC memory region.
        /// </summary>
        internal class MemoryRegion : IMemoryRegion
        {

            #region Locals

            [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
            private readonly PICMemoryMapper _parent;

            #endregion

            #region IMemoryRegion implementation

            /// <summary>
            /// Gets the name of the memory region.
            /// </summary>
            /// <value>
            /// The ID of the memory region as a string.
            /// </value>
            public string RegionName { get; }

            /// <summary>
            /// Gets the virtual byte addresses range.
            /// </summary>
            /// <value>
            /// A tuple providing the start and end+1 virtual byte addresses of the memory region.
            /// </value>
            public Tuple<int, int> VirtualByteAddress { get; }

            /// <summary>
            /// Gets or sets the physical byte addresses range.
            /// </summary>
            /// <value>
            /// A tuple providing the start and end+1 physical byte addresses of the memory region.
            /// </value>
            public Tuple<int, int> PhysicalByteAddress { get; internal set; }

            /// <summary>
            /// Gets the type of the memory region.
            /// </summary>
            /// <value>
            /// A value from <see cref="MemoryDomain"/> enumeration.
            /// </value>
            public MemoryDomain TypeOfMemory { get; }

            /// <summary>
            /// Gets the subtype of the memory region.
            /// </summary>
            /// <value>
            /// A value from <see cref="MemorySubDomain"/> enumeration.
            /// </value>
            public MemorySubDomain SubtypeOfMemory { get; }

            /// <summary>
            /// Gets the memory region traits.
            /// </summary>
            /// <value>
            /// The characteristics of the memory region.
            /// </value>
            public MemTrait Trait { get; }

            /// <summary>
            /// Gets the memory region total size in bytes.
            /// </summary>
            /// <value>
            /// The size in number of bytes.
            /// </value>
            public int Size { get; }

            /// <summary>
            /// Remaps a virtual byte address to a physical byte address.
            /// </summary>
            /// <param name="iVirtAddr">The virtual memory byte address.</param>
            /// <returns>
            /// The physical byte address as an integer; -1 if no physical byte address found (non-existent
            /// memory).
            /// </returns>
            public int RemapAddress(int iVirtAddr)
            {
                switch (TypeOfMemory)
                {
                    case MemoryDomain.Data:
                        if ((iVirtAddr < 0) || (iVirtAddr > (_parent._remaptable.Count() - 1))) return NOPHYSICAL_MEM;
                        return _parent._remaptable[iVirtAddr];

                    case MemoryDomain.Prog:
                        return iVirtAddr;

                    case MemoryDomain.Absolute:
                        return iVirtAddr;
                }
                throw new NotImplementedException($"Remapping address for '{TypeOfMemory}' domain is not implemented yet.");
            }

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="mapper">The parent PIC memory mapper.</param>
            /// <param name="sRegion">The unique name of the memory region.</param>
            /// <param name="regnAddr">The region's (start,end) addresses.</param>
            /// <param name="memDomain">The memory domain type.</param>
            /// <param name="memSubDomain">The memory subdomain type.</param>
            /// <exception cref="InvalidOperationException">Thrown if the mapper can't provide the region's memory traits.</exception>
            public MemoryRegion(PICMemoryMapper mapper, string sRegion, Tuple<int, int> regnAddr, MemoryDomain memDomain, MemorySubDomain memSubDomain)
            {
                _parent = mapper;
                RegionName = sRegion;
                VirtualByteAddress = regnAddr;
                PhysicalByteAddress = regnAddr;
                Size = (PhysicalByteAddress.Item2 - PhysicalByteAddress.Item1);
                TypeOfMemory = memDomain;
                SubtypeOfMemory = memSubDomain;
                if (SubtypeOfMemory != MemorySubDomain.NNMR)  // Non-Memory-Mapped-Registers have no memory characteristics.
                {
                    MemTrait trait;
                    if (!_parent.maptraits.TryGetValue(new MemoryDomainKey(memDomain, memSubDomain), out trait))
                        throw new InvalidOperationException($"Missing characteristics for [{memDomain}/{memSubDomain}] region '{RegionName}'");
                    Trait = trait;
                }

            }

            #endregion

            #region Methods

            /// <summary>
            /// Checks wether the given memory fragment is contained in this memory region.
            /// </summary>
            /// <param name="iVirtByteAddr">The starting memory byte virtual address of the fragment.</param>
            /// <param name="Len">(Optional) The length in bytes of the fragment.</param>
            /// <returns>
            /// True if the fragment is contained in this memory region, false if not.
            /// </returns>
            public bool Contains(int iVirtByteAddr, int Len = 0) => ((iVirtByteAddr >= VirtualByteAddress.Item1) && ((iVirtByteAddr + Len) < VirtualByteAddress.Item2));

            #endregion

        }

        internal class LinearRegion : ILinearRegion
        {

            #region Locals

            [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
            private readonly PICMemoryMapper _parent;

            #endregion

            #region ILinearRegion implementation

            /// <summary>
            /// Gets the byte size of GPR memory banks.
            /// </summary>
            /// <value>
            /// The size of the memory banks in number of bytes.
            /// </value>
            public int BankSize { get; }

            /// <summary>
            /// Gets the FSR byte address indirect range of the linear data memory region.
            /// </summary>
            /// <value>
            /// A tuple providing the start and end+1 virtual byte addresses of the data memory region.
            /// </value>
            public Tuple<int, int> FSRByteAddress { get; }

            /// <summary>
            /// Gets the block byte range visible thru the linear data region.
            /// </summary>
            /// <value>
            /// The addresses tuple (start, end) representing the GPR block range.
            /// </value>
            public Tuple<int, int> BlockByteRange { get; }

            /// <summary>
            /// Gets the type of the memory region.
            /// </summary>
            public MemoryDomain TypeOfMemory => MemoryDomain.Data;

            /// <summary>
            /// Gets the subtype of the memory region.
            /// </summary>
            public MemorySubDomain SubtypeOfMemory => MemorySubDomain.Linear;

            /// <summary>
            /// Gets the memory characteristics of the linear data memory region.
            /// </summary>
            public MemTrait Trait { get; }

            /// <summary>
            /// Gets the size in bytes of the linear data memory region.
            /// </summary>
            /// <value>
            /// The size in number of bytes.
            /// </value>
            public int Size => FSRByteAddress.Item2 - FSRByteAddress.Item1;

            /// <summary>
            /// Remap a FSR indirect address in the linear data region address to the corresponding GPR
            /// full memory address.
            /// </summary>
            /// <param name="iVirtAddr">The linear data memory byte address.</param>
            /// <returns>
            /// The GPR data memory address or NOPHYSICAL_MEM(-1).
            /// </returns>
            public int RemapAddress(int iVirtAddr)
            {
                if (!Contains(iVirtAddr)) return NOPHYSICAL_MEM;
                var add = RemapFSRIndirect(iVirtAddr);
                return (add.Item1 * BankSize) + add.Item2;
            }

            /// <summary>
            /// Remap a FSR indirect address in linear data region address to the corresponding GPR bank number and
            /// offset.
            /// </summary>
            /// <param name="iFSRVirtAddr">The virtual data memory byte address.</param>
            /// <returns>
            /// A tuple containing the GPR Bank Number and GPR Offset or NOPHYSICAL_MEM(-1, -1) indicator.
            /// </returns>
            public Tuple<int, int> RemapFSRIndirect(int iFSRVirtAddr)
            {
                if (!Contains(iFSRVirtAddr)) return new Tuple<int, int>(NOPHYSICAL_MEM, NOPHYSICAL_MEM);
                int blocksize = (BlockByteRange.Item2 - BlockByteRange.Item1);
                iFSRVirtAddr -= FSRByteAddress.Item1;
                int bankNo = iFSRVirtAddr / blocksize;
                int bankOff = iFSRVirtAddr % blocksize;
                return new Tuple<int,int> (bankNo, bankOff + BlockByteRange.Item1);
            }

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="mapper">The parent PIC memory mapper.</param>
            /// <param name="bankSz">Size of the memory bank in number of bytes.</param>
            /// <param name="regnAddr">The region's (start,end) addresses.</param>
            /// <param name="blockRng">The block memory range addresses.</param>
            /// <exception cref="InvalidOperationException">Thrown if the mapper can't provide the region's memory traits.</exception>
            public LinearRegion(PICMemoryMapper mapper, int bankSz, Tuple<int, int> regnAddr, Tuple<int, int> blockRng)
            {
                _parent = mapper;
                BankSize = bankSz;
                FSRByteAddress = regnAddr;
                BlockByteRange = blockRng;
                MemTrait trait;
                if (!_parent.maptraits.TryGetValue(new MemoryDomainKey(TypeOfMemory, SubtypeOfMemory), out trait))
                    throw new InvalidOperationException($"Missing characteristics for [{TypeOfMemory}/{SubtypeOfMemory}] linear region");
                Trait = trait;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Query if this linear memory region contains the given  memory subrange.
            /// </summary>
            /// <param name="iVirtByteAddr">Zero-based index of the virtual byte address.</param>
            /// <param name="Len">(Optional) The length in bytes of the memory subrange.</param>
            public bool Contains(int iVirtByteAddr, int Len = 0) => ((iVirtByteAddr >= FSRByteAddress.Item1) && ((iVirtByteAddr + Len) < FSRByteAddress.Item2));

            #endregion

        }

        #endregion

        private readonly Dictionary<MemoryDomainKey, MemTrait> maptraits = new Dictionary<MemoryDomainKey, MemTrait>(new MemoryDomainKeyEqualityComparer());
        private List<IMemoryRegion> _progregions;
        private List<IMemoryRegion> _dataregions;
        private IMemoryRegion _emulatorzone;
        private ILinearRegion _linearsector;
        private int[] _remaptable;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isNMMR = false;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _currLoadAddr;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _currRelAddr;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private PICMemoryMapper()
        {
        }

        /// <summary>
        /// Private constructor creating an instance of memory mapper for specified PIC.
        /// </summary>
        /// <param name="thePIC">the PIC descriptor.</param>
        private PICMemoryMapper(PIC thePIC)
        {
            PIC = thePIC;
        }

        /// <summary>
        /// Creates a new PICMemoryMapper.
        /// </summary>
        /// <param name="thePIC">the PIC descriptor.</param>
        /// <returns>
        /// A <see cref="IPICMemoryMapper"/> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="thePIC"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the PIC definition contains an invalid data memory size (less than 12 bytes).</exception>
        public static IPICMemoryMapper Create(PIC thePIC)
        {
            if (thePIC == null) throw new ArgumentNullException(nameof(thePIC));
            var map = new PICMemoryMapper(thePIC)
            {
                _progregions = new List<IMemoryRegion>(),
                _dataregions = new List<IMemoryRegion>()
            };

            int datasize = thePIC.DataSpace?.EndAddr ?? 0;
            if (datasize < 12) throw new ArgumentOutOfRangeException("Too low data memory size. Check PIC definition.");
            map._remaptable = new int[datasize];

            // Build the map for all memory spaces but data-related memory spaces which construction is deferred to when we'll know/use the actual PIC execution mode.
            // 
            map.PIC.ArchDef.MemTraits.Traits.ForEach((e) => { var ee = e as IMemTraitsSymbolAcceptor; if (ee != null) ee.Accept(map); });
            map.PIC.ProgramSpace.Sectors?.ForEach((e) => { var ee = e as IMemProgramRegionAcceptor; if (ee != null) ee.Accept(map); });
            map.ExecMode = PICExecMode.Traditional;
            return map;
        }

        #endregion

        #region IPICMemoryMapper interface

        public PIC PIC { get; }

        public InstructionSetID InstructionSetID { get { return PIC.GetInstructionSetID; } }

        private bool _isPIC18 => (InstructionSetID >= InstructionSetID.PIC18);

        public PICExecMode ExecMode
        {
            get { return _execMode; }
            set
            {
                if (InstructionSetID == InstructionSetID.PIC18)
                    value = PICExecMode.Traditional;
                if (value != _execMode)
                {
                    _execMode = value;
                    _setDataRegions();
                }
            }
        }
        private PICExecMode _execMode = PICExecMode.Traditional;

        public const int NOPHYSICAL_MEM = -1;

        #region Data memory related

        private void _setDataRegions()
        {
            _dataregions.Clear();
            HasSFR = false;
            HasGPR = false;
            HasDPR = false;
            HasNMMR = false;
            HasLinear = false;
            HasEmulatorZone = false;
            for (int i = 0; i < _remaptable.Length; i++)
                _remaptable[i] = NOPHYSICAL_MEM;
            switch (ExecMode)
            {
                case PICExecMode.Traditional:
                    PIC.DataSpace.RegardlessOfMode.Regions?.ForEach((e) => { var ee = e as IMemDataRegionAcceptor; if (ee != null) ee.Accept(this); });
                    PIC.DataSpace.TraditionalModeOnly?.ForEach((e) => { var ee = e as IMemDataRegionAcceptor; if (ee != null) ee.Accept(this); });
                    break;
                case PICExecMode.Extended:
                    if (!PIC.IsExtended) throw new InvalidOperationException("Extended execution mode is not supported by this PIC");
                    PIC.DataSpace.RegardlessOfMode.Regions?.ForEach((e) => { var ee = e as IMemDataRegionAcceptor; if (ee != null) ee.Accept(this); });
                    PIC.DataSpace.ExtendedModeOnly?.ForEach((e) => { var ee = e as IMemDataRegionAcceptor; if (ee != null) ee.Accept(this); });
                    break;
            }
            PIC.IndirectSpace?.ForEach((e) => { var ee = e as IMemDataRegionAcceptor; if (ee != null) ee.Accept(this); });
            PIC.DMASpace?.ForEach((e) => { var ee = e as IMemDataRegionAcceptor; if (ee != null) ee.Accept(this); });
        }

        private List<IMemoryRegion> _getDataRegions()
        {
            if (_dataregions.Count <= 0) _setDataRegions();
            return _dataregions;
        }

        /// <summary>
        /// Gets a data memory region given its name ID.
        /// </summary>
        /// <param name="sregionName">Name ID of the memory region.</param>
        /// <returns>
        /// The data memory region.
        /// </returns>
        public IMemoryRegion GetDataRegion(string sregionName)
            => _getDataRegions().Find((regn) => regn.TypeOfMemory == MemoryDomain.Data && regn.RegionName == sregionName);

        /// <summary>
        /// Gets a data memory region given a memory virtual address.
        /// </summary>
        /// <param name="iVirtAddr">The memory address.</param>
        /// <returns>
        /// The data memory region.
        /// </returns>
        public IMemoryRegion GetDataRegion(int iVirtAddr)
            => _getDataRegions().Find((regn) => regn.Contains(iVirtAddr) && regn.TypeOfMemory == MemoryDomain.Data);

        /// <summary>
        /// Remap a data address.
        /// </summary>
        /// <param name="iVirtAddr">The memory address.</param>
        /// <returns>
        /// The physical address.
        /// </returns>
        public int RemapDataAddr(int iVirtAddr)
            => GetDataRegion(iVirtAddr)?.RemapAddress(iVirtAddr) ?? NOPHYSICAL_MEM;

        /// <summary>
        /// Enumerates the data regions.
        /// </summary>
        /// <value>
        /// The data regions enumeration.
        /// </value>
        public IReadOnlyList<IMemoryRegion> DataRegions => _getDataRegions();

        /// <summary>
        /// Gets the data memory Emulator zone. Valid only if <seealso cref="HasEmulatorZone"/> is true.
        /// </summary>
        /// <value>
        /// The emulator zone/region.
        /// </value>
        public IMemoryRegion EmulatorZone => _emulatorzone;

        /// <summary>
        /// Gets the Linear Data Memory definition. Valid only if <seealso cref="HasLinear"/> is true.
        /// </summary>
        /// <value>
        /// The Linear Data Memory region.
        /// </value>
        public ILinearRegion LinearSector => _linearsector;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more SFR (Special
        /// Functions Register) data sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more SFR data sectors, false if not.
        /// </value>
        public bool HasSFR { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more GPR (General
        /// Purpose Register) data sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more GPR data sectors, false if not.
        /// </value>
        public bool HasGPR { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more DPR (Dual-Port
        /// Register) data sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more DPR data sectors, false if not.
        /// </value>
        public bool HasDPR { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more NMMR (Non-Memory-
        /// Mapped Register) definitions.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more NMMR (Non-Memory-Mapped Register) definitions,
        /// false if not.
        /// </value>
        public bool HasNMMR { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a zone reserved for Emulator.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a zone reserved for Emulator, false if not.
        /// </value>
        public bool HasEmulatorZone { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Linear Access data sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Linear Access data sector, false if not.
        /// </value>
        public bool HasLinear { get; private set; } = false;

        #endregion

        #region Program memory related

        /// <summary>
        /// Gets a program memory region given its name ID.
        /// </summary>
        /// <param name="sregionName">Name ID of the memory region.</param>
        /// <returns>
        /// The program memory region.
        /// </returns>
        public IMemoryRegion GetProgramRegion(string sregionName)
            => _progregions?.Find((r) => r.TypeOfMemory == MemoryDomain.Prog && r.RegionName == sregionName);

        /// <summary>
        /// Gets a program memory region given a memory virtual address.
        /// </summary>
        /// <param name="iVirtAddr">The memory address.</param>
        /// <returns>
        /// The program memory region.
        /// </returns>
        public IMemoryRegion GetProgramRegion(int iVirtAddr)
            => _progregions?.Find((regn) => regn.Contains(iVirtAddr) && regn.TypeOfMemory == MemoryDomain.Prog);

        /// <summary>
        /// Remap a program address.
        /// </summary>
        /// <param name="iVirtAddr">The memory address.</param>
        /// <returns>
        /// The physical address.
        /// </returns>
        public int RemapProgramAddr(int iVirtAddr)
            => GetProgramRegion(iVirtAddr)?.RemapAddress(iVirtAddr) ?? NOPHYSICAL_MEM;

        /// <summary>
        /// Enumerates the program regions.
        /// </summary>
        /// <value>
        /// The program regions enumeration.
        /// </value>
        public IReadOnlyList<IMemoryRegion> ProgramRegions => _progregions;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Debugger program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Debugger program sectors, false if not.
        /// </value>
        public bool HasDebug { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more Code program sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more Code program sectors, false if not.
        /// </value>
        public bool HasCode { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more External Code
        /// program sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more External Code program sectors, false if not.
        /// </value>
        public bool HasExtCode { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Calibration program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Calibration program sector, false if not.
        /// </value>
        public bool HasCalibration { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Configuration Fuses program
        /// sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Configuration Fuses program sector, false if not.
        /// </value>
        public bool HasConfigFuse { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Device ID program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Device ID program sector, false if not.
        /// </value>
        public bool HasDeviceID { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Data EEPROM program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Data EEPROM program sector, false if not.
        /// </value>
        public bool HasEEData { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a User ID program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a User ID program sector, false if not.
        /// </value>
        public bool HasUserID { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Revision ID program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Revision ID program sector, false if not.
        /// </value>
        public bool HasRevisionID { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Device Information Area
        /// program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Device Information Area program sector, false if not.
        /// </value>
        public bool HasDIA { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Device Configuration
        /// Information (DCI) program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Device Configuration Information program sector, false if
        /// not.
        /// </value>
        public bool HasDCI { get; private set; } = false;

        #endregion

        #endregion

        #region Helpers

        private void _resetAddrs(int newAddr)
        {
            _currLoadAddr = newAddr;
            _currRelAddr = 0;
        }

        private void _updateAddr(int incr)
        {
            _currLoadAddr += incr;
            _currRelAddr += incr;
        }

        #endregion

        #region IMemProgramRegionVisitor implementation

        void IMemProgramRegionVisitor.Visit(BACKBUGVectorSector xmlRegion)
        {
            Tuple<int, int> memrng =  _isPIC18 ? new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr) : new Tuple<int, int>(xmlRegion.BeginAddr << 1, xmlRegion.EndAddr << 1);
            _progregions.Add(new MemoryRegion(this, xmlRegion.RegionID, memrng, xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain));
            HasDebug = true;
        }

        void IMemProgramRegionVisitor.Visit(CalDataZone xmlRegion)
        {
            Tuple<int, int> memrng = _isPIC18 ? new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr) : new Tuple<int, int>(xmlRegion.BeginAddr << 1, xmlRegion.EndAddr << 1);
            _progregions.Add(new MemoryRegion(this, xmlRegion.RegionID, memrng, xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain));
            HasCalibration = true;
        }

        void IMemProgramRegionVisitor.Visit(CodeSector xmlRegion)
        {
            Tuple<int, int> memrng = _isPIC18 ? new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr) : new Tuple<int, int>(xmlRegion.BeginAddr << 1, xmlRegion.EndAddr << 1);
            _progregions.Add(new MemoryRegion(this, xmlRegion.RegionID, memrng, xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain));
            HasCode = true;
        }

        void IMemProgramRegionVisitor.Visit(ConfigFuseSector xmlRegion)
        {
            Tuple<int, int> memrng = _isPIC18 ? new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr) : new Tuple<int, int>(xmlRegion.BeginAddr << 1, xmlRegion.EndAddr << 1);
            _progregions.Add(new MemoryRegion(this, xmlRegion.RegionID, memrng, xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain));
            HasConfigFuse = true;
        }

        void IMemProgramRegionVisitor.Visit(DeviceIDSector xmlRegion)
        {
            Tuple<int, int> memrng = _isPIC18 ? new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr) : new Tuple<int, int>(xmlRegion.BeginAddr << 1, xmlRegion.EndAddr << 1);
            _progregions.Add(new MemoryRegion(this, xmlRegion.RegionID, memrng, xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain));
            HasDeviceID = true;
        }

        void IMemProgramRegionVisitor.Visit(EEDataSector xmlRegion)
        {
            Tuple<int, int> memrng = _isPIC18 ? new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr) : new Tuple<int, int>(xmlRegion.BeginAddr << 1, xmlRegion.EndAddr << 1);
            _progregions.Add(new MemoryRegion(this, xmlRegion.RegionID, memrng, xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain));
            HasEEData = true;
        }

        void IMemProgramRegionVisitor.Visit(ExtCodeSector xmlRegion)
        {
            Tuple<int, int> memrng = _isPIC18 ? new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr) : new Tuple<int, int>(xmlRegion.BeginAddr << 1, xmlRegion.EndAddr << 1);
            _progregions.Add(new MemoryRegion(this, xmlRegion.RegionID, memrng, xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain));
            HasExtCode = true;
        }

        void IMemProgramRegionVisitor.Visit(RevisionIDSector xmlRegion)
        {
            Tuple<int, int> memrng = _isPIC18 ? new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr) : new Tuple<int, int>(xmlRegion.BeginAddr << 1, xmlRegion.EndAddr << 1);
            _progregions.Add(new MemoryRegion(this, xmlRegion.RegionID, memrng, xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain));
            HasRevisionID = true;
        }

        void IMemProgramRegionVisitor.Visit(TestZone xmlRegion) { }

        void IMemProgramRegionVisitor.Visit(UserIDSector xmlRegion)
        {
            Tuple<int, int> memrng = _isPIC18 ? new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr) : new Tuple<int, int>(xmlRegion.BeginAddr << 1, xmlRegion.EndAddr << 1);
            _progregions.Add(new MemoryRegion(this, xmlRegion.RegionID, memrng, xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain));
            HasUserID = true;
        }

        void IMemProgramRegionVisitor.Visit(DIASector xmlRegion)
        {
            Tuple<int, int> memrng = _isPIC18 ? new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr) : new Tuple<int, int>(xmlRegion.BeginAddr << 1, xmlRegion.EndAddr << 1);
            _progregions.Add(new MemoryRegion(this, xmlRegion.RegionID, memrng, xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain));
            HasDIA = true;
        }

        void IMemProgramRegionVisitor.Visit(DCISector xmlRegion)
        {
            Tuple<int, int> memrng = _isPIC18 ? new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr) : new Tuple<int, int>(xmlRegion.BeginAddr << 1, xmlRegion.EndAddr << 1);
            _progregions.Add(new MemoryRegion(this, xmlRegion.RegionID, memrng, xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain));
            HasDCI = true;
        }

        #endregion

        #region IMemDataRegionVisitor implementation

        void IMemDataRegionVisitor.Visit(SFRDataSector xmlRegion)
        {
            var regn = new MemoryRegion(this, xmlRegion.RegionID, new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr), xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain);
            _dataregions.Add(regn);
            HasSFR = true;
            _resetAddrs(regn.VirtualByteAddress.Item1);
            _isNMMR = false;
            foreach (var ent in xmlRegion.SFRs)
            {
                var ient = ent as IMemDataSymbolAcceptor;
                if (ient != null)
                    ient.Accept(this);
            }
        }

        void IMemDataRegionVisitor.Visit(GPRDataSector xmlRegion)
        {
            var regn = new MemoryRegion(this, xmlRegion.RegionID, new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr), xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain);
            if (!string.IsNullOrWhiteSpace(xmlRegion.ShadowIDRef))
            {
                var remap = GetDataRegion(xmlRegion.ShadowIDRef);
                if (remap == null) throw new ArgumentOutOfRangeException(nameof(xmlRegion.ShadowIDRef));
                regn.PhysicalByteAddress = remap.VirtualByteAddress;
            }
            _dataregions.Add(regn);
            HasGPR = true;
            for (int i = 0; i < regn.VirtualByteAddress.Item2 - regn.VirtualByteAddress.Item1; i++)
            {
                _remaptable[regn.VirtualByteAddress.Item1 + i] = regn.PhysicalByteAddress.Item1 + i;
            }
        }

        void IMemDataRegionVisitor.Visit(DPRDataSector xmlRegion)
        {
            var regn = new MemoryRegion(this, xmlRegion.RegionID, new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr), xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain);
            if (!string.IsNullOrWhiteSpace(xmlRegion.ShadowIDRef))
            {
                var remap = GetDataRegion(xmlRegion.ShadowIDRef);
                if (remap == null) throw new ArgumentOutOfRangeException(nameof(xmlRegion.ShadowIDRef));
                regn.PhysicalByteAddress = remap.VirtualByteAddress;
            }
            _dataregions.Add(regn);
            HasDPR = true;
            for (int i = 0; i < regn.VirtualByteAddress.Item2 - regn.VirtualByteAddress.Item1; i++)
            {
                _remaptable[regn.VirtualByteAddress.Item1 + i] = regn.PhysicalByteAddress.Item1 + i;
            }
        }

        void IMemDataRegionVisitor.Visit(EmulatorZone xmlRegion)
        {
            _emulatorzone = new MemoryRegion(this, xmlRegion.RegionID, new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr), xmlRegion.MemoryDomain, xmlRegion.MemorySubDomain);
            HasEmulatorZone = true;
        }

        void IMemDataRegionVisitor.Visit(NMMRPlace xmlRegion)
        {
            var regn = new MemoryRegion(this, xmlRegion.RegionID, new Tuple<int, int>(NOPHYSICAL_MEM, NOPHYSICAL_MEM), MemoryDomain.Data, MemorySubDomain.NNMR);
            _dataregions.Add(regn);
            HasNMMR = true;
            _isNMMR = true;
            foreach (var ent in xmlRegion.SFRDefs)
                ent.Accept(this);
            _isNMMR = false;
        }

        void IMemDataRegionVisitor.Visit(LinearDataSector xmlRegion)
        {
            _linearsector = new LinearRegion(this, xmlRegion.BankSize, new Tuple<int, int>(xmlRegion.BeginAddr, xmlRegion.EndAddr), new Tuple<int, int>(xmlRegion.BlockBeginAddr, xmlRegion.BlockEndAddr));
            HasLinear = true;
        }

        #endregion

        #region IMemDataSymbolVisitor implementation

        void IMemDataSymbolVisitor.Visit(DataBitAdjustPoint xmlSymb)
        {
            // Do nothing. We are not interested here in SFR bits definition.
        }

        void IMemDataSymbolVisitor.Visit(DataByteAdjustPoint xmlSymb)
        {
            _updateAddr(xmlSymb.Offset);
        }

        void IMemDataSymbolVisitor.Visit(SFRDef xmlSymb)
        {
            if (_isNMMR) return;
            _remaptable[_currLoadAddr] = _currLoadAddr;
            _updateAddr(xmlSymb.ByteWidth);
        }

        void IMemDataSymbolVisitor.Visit(SFRFieldDef xmlSymb)
        {
            // Do nothing. We are not interested here in SFR bits definition.
        }

        void IMemDataSymbolVisitor.Visit(SFRFieldSemantic xmlSymb)
        {
            // Do nothing. We are not interested here in SFR bits definition.
        }

        void IMemDataSymbolVisitor.Visit(SFRModeList xmlSymb)
        {
            // Do nothing. We are not interested here in SFR bits definition.
        }

        void IMemDataSymbolVisitor.Visit(SFRMode xmlSymb)
        {
            // Do nothing. We are not interested here in SFR bits definition.
        }

        void IMemDataSymbolVisitor.Visit(Mirror xmlSymb)
        {
            var regn = GetDataRegion(xmlSymb.RegionIDRef);
            if (regn != null)
            {
                for (int i = 0; i < xmlSymb.NzSize; i++)
                    _remaptable[_currLoadAddr + i] = regn.PhysicalByteAddress.Item1 + i;
            }
            _updateAddr(xmlSymb.NzSize);
        }

        void IMemDataSymbolVisitor.Visit(JoinedSFRDef xmlSymb)
        {
            xmlSymb.SFRs?.ForEach((e) => e.Accept(this));
        }

        void IMemDataSymbolVisitor.Visit(MuxedSFRDef xmlSymb)
        {
            for (int i = 0; i < ((xmlSymb.NzWidth + 7) >> 3); i++)
            {
                _remaptable[_currLoadAddr + i] = _currLoadAddr + i;
            }
            _updateAddr(xmlSymb.ByteWidth);
        }

        void IMemDataSymbolVisitor.Visit(SelectSFR xmlSymb)
        {
            // Do nothing. Address increment is already handled by parent MuxedSFRDef.
        }

        void IMemDataSymbolVisitor.Visit(DMARegisterMirror xmlSymb)
        {
            // Do nothing for now.
        }

        #endregion

        #region IMemTraitsSymbolVisitor implementation

        void IMemTraitsSymbolVisitor.Visit(CalDataMemTraits mTraits)
            => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Calib), mTraits);

        void IMemTraitsSymbolVisitor.Visit(CodeMemTraits mTraits)
            => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Code), mTraits);

        void IMemTraitsSymbolVisitor.Visit(ConfigFuseMemTraits mTraits)
            => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Config), mTraits);

        void IMemTraitsSymbolVisitor.Visit(ExtCodeMemTraits mTraits)
            => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.ExtCode), mTraits);

        void IMemTraitsSymbolVisitor.Visit(EEDataMemTraits mTraits)
        => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.EEData), mTraits);

        void IMemTraitsSymbolVisitor.Visit(BackgroundDebugMemTraits mTraits)
            => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Debugger), mTraits);

        void IMemTraitsSymbolVisitor.Visit(ConfigWORMMemTraits mTraits)
            => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.Other), mTraits);

        void IMemTraitsSymbolVisitor.Visit(DataMemTraits mTraits)
        {
            maptraits.Add(new MemoryDomainKey(MemoryDomain.Data, MemorySubDomain.DPR), mTraits);
            maptraits.Add(new MemoryDomainKey(MemoryDomain.Data, MemorySubDomain.GPR), mTraits);
            maptraits.Add(new MemoryDomainKey(MemoryDomain.Data, MemorySubDomain.SFR), mTraits);
            maptraits.Add(new MemoryDomainKey(MemoryDomain.Data, MemorySubDomain.Emulator), mTraits);
            maptraits.Add(new MemoryDomainKey(MemoryDomain.Data, MemorySubDomain.Linear), mTraits);
        }

        void IMemTraitsSymbolVisitor.Visit(DeviceIDMemTraits mTraits)
        {
            maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.DeviceID), mTraits);
            maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.RevisionID), mTraits);
        }

        void IMemTraitsSymbolVisitor.Visit(TestMemTraits mTraits) { }

        void IMemTraitsSymbolVisitor.Visit(UserIDMemTraits mTraits)
            => maptraits.Add(new MemoryDomainKey(MemoryDomain.Prog, MemorySubDomain.UserID), mTraits);


        #endregion

    }
}
