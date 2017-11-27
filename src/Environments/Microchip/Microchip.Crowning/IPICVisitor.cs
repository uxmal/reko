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

namespace Microchip.Crownking
{
    /// <summary>
    /// The <see cref="IMemTraitsSymbolVisitor"/> defines the interface for the implementation of the memory traits visitors.
    /// </summary>
    /// 
    public interface IMemTraitsSymbolVisitor
    {
        void Visit(CalDataMemTraits mTraits);
        void Visit(CodeMemTraits mTraits);
        void Visit(ConfigFuseMemTraits mTraits);
        void Visit(ExtCodeMemTraits mTraits);
        void Visit(EEDataMemTraits mTraits);
        void Visit(BackgroundDebugMemTraits mTraits);
        void Visit(ConfigWORMMemTraits mTraits);
        void Visit(DataMemTraits mTraits);
        void Visit(DeviceIDMemTraits mTraits);
        void Visit(TestMemTraits mTraits);
        void Visit(UserIDMemTraits mTraits);
    }

    /// <summary>
    /// The <see cref="IMemTraitsSymbolAcceptor"/> defines the interface for memory trait accepting a visitor.
    /// </summary>
    public interface IMemTraitsSymbolAcceptor
    {
        /// <summary>
        /// The <see cref="Accept"/> method accepts a memory trait visitor and calls the appropriate "Visit()" method for this trait.
        /// </summary>
        /// <param name="v">An <see cref="IMemTraitsSymbolVisitor"/> visitor to accept.</param>
        void Accept(IMemTraitsSymbolVisitor v);

    }

    /// <summary>
    /// The <see cref="IMemDataRegionVisitor"/> defines the interface for the implementation of the data memory regions' visitors.
    /// </summary>
    /// 
    public interface IMemDataRegionVisitor
    {
        void Visit(SFRDataSector xmlRegion);
        void Visit(GPRDataSector xmlRegion);
        void Visit(DPRDataSector xmlRegion);
        void Visit(EmulatorZone xmlRegion);
        void Visit(NMMRPlace xmlRegion);
        void Visit(LinearDataSector xmlRegion);
    }

    /// <summary>
    /// The <see cref="IMemDataRegionAcceptor"/> defines the interface for data memory region accepting a visitor.
    /// </summary>
    public interface IMemDataRegionAcceptor
    {
        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this data memory region.
        /// </summary>
        /// <param name="v">The data memory region visitor to accept.</param>
        void Accept(IMemDataRegionVisitor v);
    }

    /// <summary>
    /// The <see cref="IMemDataSymbolVisitor"/> defines the interface for the implementation of the data memory symbols' visitors.
    /// </summary>
    /// 
    public interface IMemDataSymbolVisitor
    {
        void Visit(DataBitAdjustPoint xmlSymb);
        void Visit(DataByteAdjustPoint xmlSymb);
        void Visit(SFRDef xmlSymb);
        void Visit(SFRFieldDef xmlSymb);
        void Visit(SFRFieldSemantic xmlSymb);
        void Visit(SFRModeList xmlSymb);
        void Visit(SFRMode xmlSymb);
        void Visit(Mirror xmlSymb);
        void Visit(JoinedSFRDef xmlSymb);
        void Visit(MuxedSFRDef xmlSymb);
        void Visit(SelectSFR xmlSymb);
        void Visit(DMARegisterMirror xmlSymb);
    }

    /// <summary>
    /// The <see cref="IMemDataSymbolAcceptor"/> defines the interface for data memory symbol accepting a visitor.
    /// </summary>
    public interface IMemDataSymbolAcceptor
    {
        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and
        /// calls the appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">The definition data symbol visitor to accept</param>
        ///
        void Accept(IMemDataSymbolVisitor v);
    }

    /// <summary>
    /// The <see cref="IMemProgramRegionVisitor"/> defines the interface for the implementation of the program memory regions' visitors.
    /// </summary>
    /// 
    public interface IMemProgramRegionVisitor
    {
        void Visit(BACKBUGVectorSector xmlRegion);
        void Visit(CalDataZone xmlRegion);
        void Visit(CodeSector xmlRegion);
        void Visit(ConfigFuseSector xmlRegion);
        void Visit(DeviceIDSector xmlRegion);
        void Visit(EEDataSector xmlRegion);
        void Visit(ExtCodeSector xmlRegion);
        void Visit(RevisionIDSector xmlRegion);
        void Visit(TestZone xmlRegion);
        void Visit(UserIDSector xmlRegion);
        void Visit(DIASector xmlRegion);
        void Visit(DCISector xmlRegion);
    }

    /// <summary>
    /// The <see cref="IMemProgramRegionAcceptor"/> defines the interface for program memory region accepting a visitor.
    /// </summary>
    public interface IMemProgramRegionAcceptor
    {
        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition program region visitor and
        /// calls the appropriate "Visit()" method for this program region.
        /// </summary>
        /// <param name="v">The definition program region visitor to accept</param>
        ///
        void Accept(IMemProgramRegionVisitor v);
    }

    /// <summary>
    /// The <see cref="IMemProgramSymbolVisitor"/> defines the interface for the implementation of the program memory symbols' visitors.
    /// </summary>
    /// 
    public interface IMemProgramSymbolVisitor
    {
        void Visit(ProgBitAdjustPoint xmlSymb);
        void Visit(ProgByteAdjustPoint xmlSymb);
        void Visit(DCRDef xmlSymb);
        void Visit(DCRMode xmlSymb);
        void Visit(DCRFieldDef xmlSymb);
        void Visit(DCRFieldSemantic xmlSymb);
        void Visit(DEVIDToRev xmlSymb);
        void Visit(DCRDefIllegal xmlSymb);
        void Visit(DeviceRegister xmlSymb);
        void Visit(DIARegisterArray xmlSymb);
    }

    /// <summary>
    /// The <see cref="IMemProgramSymbolAcceptor"/> defines the interface for program memory symbol accepting a visitor.
    /// </summary>
    public interface IMemProgramSymbolAcceptor
    {
        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition program symbol visitor and
        /// calls the appropriate "Visit()" method for this program region.
        /// </summary>
        /// <param name="v">The definition program symbol visitor to accept</param>
        ///
        void Accept(IMemProgramSymbolVisitor v);
    }

}
