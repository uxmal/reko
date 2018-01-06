using Microchip.Crownking;
using System;

namespace Microchip.MemoryMapper
{

    /// <summary>
    /// The <see cref="IMemoryRegion"/> interface permits to retrieve information for a given PIC memory region (data or program).
    /// </summary>
    public interface IMemoryRegion
    {
        /// <summary>
        /// Gets the name of the memory region.
        /// </summary>
        /// <value>
        /// The ID of the memory region as a string.
        /// </value>
        string RegionName { get; }

        /// <summary>
        /// Gets the virtual byte addresses range.
        /// </summary>
        /// <value>
        /// A tuple providing the start and end+1 virtual byte addresses of the memory region.
        /// </value>
        Tuple<int, int> VirtualByteAddress { get; }

        /// <summary>
        /// Gets the physical byte addresses range.
        /// </summary>
        /// <value>
        /// A tuple providing the start and end+1 physical byte addresses of the memory region.
        /// </value>
        Tuple<int, int> PhysicalByteAddress { get; }

        /// <summary>
        /// Gets the memory region total size in bytes.
        /// </summary>
        /// <value>
        /// The size in number of bytes.
        /// </value>
        int Size { get; }

        /// <summary>
        /// Gets the type of the memory region.
        /// </summary>
        /// <value>
        /// A value from <see cref="MemoryDomain"/> enumeration.
        /// </value>
        MemoryDomain TypeOfMemory { get; }

        /// <summary>
        /// Gets the subtype of the memory region.
        /// </summary>
        /// <value>
        /// A value from <see cref="MemorySubDomain"/> enumeration.
        /// </value>
        MemorySubDomain SubtypeOfMemory { get; }

        /// <summary>
        /// Gets the memory region traits.
        /// </summary>
        /// <value>
        /// The characteristics of the memory region.
        /// </value>
        MemTrait Trait { get; }

        /// <summary>
        /// Remaps a virtual byte address to a physical byte address.
        /// </summary>
        /// <param name="iVirtByteAddr">The virtual memory byte address.</param>
        /// <returns>
        /// The physical byte address as an integer; -1 if no physical byte address found (non-existent memory).
        /// </returns>
        int RemapAddress(int iVirtByteAddr);

        /// <summary>
        /// Checks wether the given memory fragment is contained in this memory region.
        /// </summary>
        /// <param name="iVirtByteAddr">The starting memory byte virtual address of the fragment.</param>
        /// <param name="Len">The length in bytes of the fragment.</param>
        /// <returns>
        /// True if the fragment is contained in this memory region, false if not.
        /// </returns>
        bool Contains(int iVirtByteAddr, int Len = 0);

    }
}
