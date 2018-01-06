using Microchip.Crownking;
using System;

namespace Microchip.MemoryMapper
{
    /// <summary>
    /// The <see cref="ILinearRegion"/> interface permits to retrieve information for the PIC linear data memory region.
    /// </summary>
    public interface ILinearRegion
    {
        /// <summary>
        /// Gets the FSR byte address indirect range of the linear data memory region.
        /// </summary>
        /// <value>
        /// A tuple providing the start and end+1 virtual byte addresses of the data memory region.
        /// </value>
        Tuple<int, int> FSRByteAddress { get; }

        /// <summary>
        /// Gets the byte size of GPR memory banks.
        /// </summary>
        /// <value>
        /// The size of the memory banks in number of bytes.
        /// </value>
        int BankSize { get; }

        /// <summary>
        /// Gets the block byte range visible thru the linear data region.
        /// </summary>
        /// <value>
        /// The addresses tuple (start, end) representing the GPR block range.
        /// </value>
        Tuple<int, int> BlockByteRange { get; }

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
        /// Gets the memory characteristics of the linear data memory region.
        /// </summary>
        /// <value>
        /// The characteristics.
        /// </value>
        MemTrait Trait { get; }

        /// <summary>
        /// Gets the size in bytes of the linear data memory region.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        int Size { get; }

        /// <summary>
        /// Remap a FSR indirect address in the linear data region address to the corresponding GPR memory address.
        /// </summary>
        /// <param name="iVirtAddr">The linear data memory byte address.</param>
        /// <returns>
        /// The GPR data memory address or NOPHYSICAL_MEM(-1).
        /// </returns>
        int RemapAddress(int iVirtAddr);

        /// <summary>
        /// Remap a FSR indirect address in linear data region address to the corresponding GPR bank and offset.
        /// </summary>
        /// <param name="iFSRVirtAddr">The virtual data memory byte address.</param>
        /// <returns>
        /// A tuple containing the GPR Bank Number and GPR Offset or NOPHYSICAL_MEM(-1, -1) indicator.
        /// </returns>
        Tuple<int, int> RemapFSRIndirect(int iFSRVirtAddr);

    }

}
