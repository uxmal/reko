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

namespace Reko.Libraries.Microchip
{
    #region Data Memory Space regions/symbols visitors

    /// <summary>
    /// The <see cref="IMemDataRegionVisitor"/> defines the interface for the implementation of the data memory regions' visitor methods.
    /// </summary>
    /// 
    public interface IMemDataRegionVisitor
    {
        void Visit(SFRDataSector xmlRegion);
        void Visit(GPRDataSector xmlRegion);
        void Visit(DPRDataSector xmlRegion);
        void Visit(NMMRPlace xmlRegion);
        void Visit(LinearDataSector xmlRegion);
    }

    /// <summary>
    /// The <see cref="IMemDataRegionVisitor{T}"/> defines the interface for the implementation of
    /// the data memory regions' visitor functions.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
    public interface IMemDataRegionVisitor<T>
    {
        T Visit(SFRDataSector xmlRegion);
        T Visit(GPRDataSector xmlRegion);
        T Visit(DPRDataSector xmlRegion);
        T Visit(NMMRPlace xmlRegion);
        T Visit(LinearDataSector xmlRegion);
    }

    /// <summary>
    /// The <see cref="IMemDataRegionVisitor{T, C}"/> defines the interface for the implementation of
    /// the data memory regions' visitor functions with a given context.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
    /// <typeparam name="C">Generic type parameter of the function's context parameter.</typeparam>
    public interface IMemDataRegionVisitor<T, C>
    {
        T Visit(SFRDataSector xmlRegion, C context);
        T Visit(GPRDataSector xmlRegion, C context);
        T Visit(DPRDataSector xmlRegion, C context);
        T Visit(NMMRPlace xmlRegion, C context);
        T Visit(LinearDataSector xmlRegion, C context);
    }

    /// <summary>
    /// The <see cref="IMemDataSymbolVisitor"/> defines the interface for the implementation of the data memory symbols' visitor methods.
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
    /// The <see cref="IMemDataSymbolVisitor{T}"/> defines the interface for the implementation of the data memory symbols' visitor functions.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
    /// 
    public interface IMemDataSymbolVisitor<T>
    {
        T Visit(DataBitAdjustPoint xmlSymb);
        T Visit(DataByteAdjustPoint xmlSymb);
        T Visit(SFRDef xmlSymb);
        T Visit(SFRFieldDef xmlSymb);
        T Visit(SFRFieldSemantic xmlSymb);
        T Visit(SFRModeList xmlSymb);
        T Visit(SFRMode xmlSymb);
        T Visit(Mirror xmlSymb);
        T Visit(JoinedSFRDef xmlSymb);
        T Visit(MuxedSFRDef xmlSymb);
        T Visit(SelectSFR xmlSymb);
        T Visit(DMARegisterMirror xmlSymb);
    }

    /// <summary>
    /// The <see cref="IMemDataSymbolVisitor"/> defines the interface for the implementation of the data memory symbols' visitor functions with a given context.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the function result.</typeparam>
    /// <typeparam name="C">Generic type parameter of the function's context parameter.</typeparam>
    /// 
    public interface IMemDataSymbolVisitor<T, C>
    {
        T Visit(DataBitAdjustPoint xmlSymb, C context);
        T Visit(DataByteAdjustPoint xmlSymb, C context);
        T Visit(SFRDef xmlSymb, C context);
        T Visit(SFRFieldDef xmlSymb, C context);
        T Visit(SFRFieldSemantic xmlSymb, C context);
        T Visit(SFRModeList xmlSymb, C context);
        T Visit(SFRMode xmlSymb, C context);
        T Visit(Mirror xmlSymb, C context);
        T Visit(JoinedSFRDef xmlSymb, C context);
        T Visit(MuxedSFRDef xmlSymb, C context);
        T Visit(SelectSFR xmlSymb, C context);
        T Visit(DMARegisterMirror xmlSymb, C context);
    }

    /// <summary>
    /// The <see cref="IMemDataRegionAcceptor"/> defines the interface for data memory region accepting a visitor method.
    /// </summary>
    public interface IMemDataRegionAcceptor
    {
        /// <summary>
        /// The <see cref="Accept"/> method accepts a data memory region visitor and calls the
        /// appropriate "Visit()" method for this data memory region.
        /// </summary>
        /// <param name="v">An <see cref="IMemDataRegionVisitor"/> visitor to accept.</param>
        void Accept(IMemDataRegionVisitor v);
    }

    /// <summary>
    /// The <see cref="IMemDataRegionAcceptor{T}"/> defines the interface for data memory region
    /// accepting a visitor function.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the visitor function's return value.</typeparam>
    public interface IMemDataRegionAcceptor<T>
    {
        /// <summary>
        /// The <see cref="Accept"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this data memory region.
        /// </summary>
        /// <param name="v">An <see cref="IMemDataRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A returned value of generic type T.
        /// </returns>
        T Accept(IMemDataRegionVisitor<T> v);
    }

    /// <summary>
    /// The <see cref="IMemDataRegionAcceptor{T, C}"/> defines the interface for data memory region
    /// accepting a visitor function with a given context.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the visitor function's return value.</typeparam>
    /// <typeparam name="C">Generic type parameter of the function's context parameter.</typeparam>
    public interface IMemDataRegionAcceptor<T, C>
    {
        /// <summary>
        /// The <see cref="Accept"/> function accepts a data memory region visitor and calls the
        /// appropriate "Visit()" function for this data memory region with the given context.
        /// </summary>
        /// <param name="v">An <see cref="IMemDataRegionVisitor{T, C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type C.</param>
        /// <returns>
        /// A returned value of generic type T.
        /// </returns>
        T Accept(IMemDataRegionVisitor<T, C> v, C context);
    }

    /// <summary>
    /// The <see cref="IMemDataSymbolAcceptor"/> defines the interface for data memory symbol accepting a visitor method.
    /// </summary>
    public interface IMemDataSymbolAcceptor
    {
        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition data symbol visitor and
        /// calls the appropriate "Visit()" method for this data symbol.
        /// </summary>
        /// <param name="v">An <see cref="IMemDataSymbolVisitor"/> visitor to accept.</param>
        ///
        void Accept(IMemDataSymbolVisitor v);
    }

    /// <summary>
    /// The <see cref="IMemDataSymbolAcceptor{T}"/> defines the interface for data memory symbol accepting a visitor function.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the visitor function's return value.</typeparam>
    public interface IMemDataSymbolAcceptor<T>
    {
        /// <summary>
        /// The <see cref="Accept"/> function accepts a definition data symbol visitor and
        /// calls the appropriate "Visit()" function for this data symbol.
        /// </summary>
        /// <param name="v">An <see cref="IMemDataSymbolVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A returned value of generic type T.
        /// </returns>
        T Accept(IMemDataSymbolVisitor<T> v);
    }

    /// <summary>
    /// The <see cref="IMemDataSymbolAcceptor{T,C}"/> defines the interface for data memory symbol accepting a visitor function with a given context.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the visitor function's return value.</typeparam>
    /// <typeparam name="C">Generic type parameter of the function's context parameter.</typeparam>
    public interface IMemDataSymbolAcceptor<T, C>
    {
        /// <summary>
        /// The <see cref="Accept"/> function accepts a definition data symbol visitor and
        /// calls the appropriate "Visit()" function for this data symbol with the given context.
        /// </summary>
        /// <param name="v">An <see cref="IMemDataSymbolVisitor{T,C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type C.</param>
        /// <returns>
        /// A returned value of generic type T.
        /// </returns>
        T Accept(IMemDataSymbolVisitor<T, C> v, C context);
    }

    #endregion

    #region Program Memory Space regions/symbols visitors

    /// <summary>
    /// The <see cref="IMemProgramRegionVisitor"/> defines the interface for the implementation of the program memory regions' visitor methods.
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
    /// The <see cref="IMemProgramRegionVisitor{T}"/> defines the interface for the implementation of the program memory regions' visitor functions.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the visitor function's return value.</typeparam>
    public interface IMemProgramRegionVisitor<T>
    {
        T Visit(BACKBUGVectorSector xmlRegion);
        T Visit(CalDataZone xmlRegion);
        T Visit(CodeSector xmlRegion);
        T Visit(ConfigFuseSector xmlRegion);
        T Visit(DeviceIDSector xmlRegion);
        T Visit(EEDataSector xmlRegion);
        T Visit(ExtCodeSector xmlRegion);
        T Visit(RevisionIDSector xmlRegion);
        T Visit(TestZone xmlRegion);
        T Visit(UserIDSector xmlRegion);
        T Visit(DIASector xmlRegion);
        T Visit(DCISector xmlRegion);
    }

    /// <summary>
    /// The <see cref="IMemProgramRegionVisitor{T,C}"/> defines the interface for the implementation of the program memory regions' visitor functions with a given context.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the visitor function's return value.</typeparam>
    /// <typeparam name="C">Generic type parameter of the function's context parameter.</typeparam>
    public interface IMemProgramRegionVisitor<T, C>
    {
        T Visit(BACKBUGVectorSector xmlRegion, C context);
        T Visit(CalDataZone xmlRegion, C context);
        T Visit(CodeSector xmlRegion, C context);
        T Visit(ConfigFuseSector xmlRegion, C context);
        T Visit(DeviceIDSector xmlRegion, C context);
        T Visit(EEDataSector xmlRegion, C context);
        T Visit(ExtCodeSector xmlRegion, C context);
        T Visit(RevisionIDSector xmlRegion, C context);
        T Visit(TestZone xmlRegion, C context);
        T Visit(UserIDSector xmlRegion, C context);
        T Visit(DIASector xmlRegion, C context);
        T Visit(DCISector xmlRegion, C context);
    }

    /// <summary>
    /// The <see cref="IMemProgramSymbolVisitor"/> defines the interface for the implementation of the program memory symbols' visitor methods.
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
    /// The <see cref="IMemProgramSymbolVisitor{T}"/> defines the interface for the implementation of the program memory symbols' visitor functions.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the visitor function's return value.</typeparam>
    public interface IMemProgramSymbolVisitor<T>
    {
        T Visit(ProgBitAdjustPoint xmlSymb);
        T Visit(ProgByteAdjustPoint xmlSymb);
        T Visit(DCRDef xmlSymb);
        T Visit(DCRMode xmlSymb);
        T Visit(DCRFieldDef xmlSymb);
        T Visit(DCRFieldSemantic xmlSymb);
        T Visit(DEVIDToRev xmlSymb);
        T Visit(DCRDefIllegal xmlSymb);
        T Visit(DeviceRegister xmlSymb);
        T Visit(DIARegisterArray xmlSymb);
    }

    /// <summary>
    /// The <see cref="IMemProgramSymbolVisitor{T}"/> defines the interface for the implementation of the program memory symbols' visitor functions with a given context.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the visitor function's return value.</typeparam>
    /// <typeparam name="C">Generic type parameter of the function's context parameter.</typeparam>
    public interface IMemProgramSymbolVisitor<T, C>
    {
        T Visit(ProgBitAdjustPoint xmlSymb, C context);
        T Visit(ProgByteAdjustPoint xmlSymb, C context);
        T Visit(DCRDef xmlSymb, C context);
        T Visit(DCRMode xmlSymb, C context);
        T Visit(DCRFieldDef xmlSymb, C context);
        T Visit(DCRFieldSemantic xmlSymb, C context);
        T Visit(DEVIDToRev xmlSymb, C context);
        T Visit(DCRDefIllegal xmlSymb, C context);
        T Visit(DeviceRegister xmlSymb, C context);
        T Visit(DIARegisterArray xmlSymb, C context);
    }

    /// <summary>
    /// The <see cref="IMemProgramRegionAcceptor"/> defines the interface for program memory region accepting a visitor method.
    /// </summary>
    public interface IMemProgramRegionAcceptor
    {
        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition program region visitor and
        /// calls the appropriate "Visit()" method for this program region.
        /// </summary>
        /// <param name="v">An <see cref="IMemProgramRegionVisitor"/> visitor to accept.</param>
        void Accept(IMemProgramRegionVisitor v);
    }

    /// <summary>
    /// The <see cref="IMemProgramRegionAcceptor{T}"/> defines the interface for program memory region accepting a visitor function.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the visitor function's return value.</typeparam>
    public interface IMemProgramRegionAcceptor<T>
    {
        /// <summary>
        /// The <see cref="Accept"/> function accepts a definition program region visitor and
        /// calls the appropriate "Visit()" function for this program region.
        /// </summary>
        /// <param name="v">An <see cref="IMemProgramRegionVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A returned value of generic type T.
        /// </returns>
        T Accept(IMemProgramRegionVisitor<T> v);
    }

    /// <summary>
    /// The <see cref="IMemProgramRegionAcceptor{T,C}"/> defines the interface for program memory region accepting a visitor function with a given context.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the visitor function's return value.</typeparam>
    /// <typeparam name="C">Generic type parameter of the function's context parameter.</typeparam>
    public interface IMemProgramRegionAcceptor<T, C>
    {
        /// <summary>
        /// The <see cref="Accept"/> function accepts a definition program region visitor and
        /// calls the appropriate "Visit()" function for this program region with the given context.
        /// </summary>
        /// <param name="v">An <see cref="IMemProgramRegionVisitor{T,C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type C.</param>
        /// <returns>
        /// A returned value of generic type T.
        /// </returns>
        T Accept(IMemProgramRegionVisitor<T, C> v, C context);
    }

    /// <summary>
    /// The <see cref="IMemProgramSymbolAcceptor"/> defines the interface for program memory symbol accepting a visitor method.
    /// </summary>
    public interface IMemProgramSymbolAcceptor
    {
        /// <summary>
        /// The <see cref="Accept"/> method accepts a definition program symbol visitor and
        /// calls the appropriate "Visit()" method for this program symbol.
        /// </summary>
        /// <param name="v">An <see cref="IMemProgramSymbolVisitor"/> visitor to accept.</param>
        void Accept(IMemProgramSymbolVisitor v);
    }

    /// <summary>
    /// The <see cref="IMemProgramSymbolAcceptor{T}"/> defines the interface for program memory symbol accepting a visitor function.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the visitor function's return value.</typeparam>
    public interface IMemProgramSymbolAcceptor<T>
    {
        /// <summary>
        /// The <see cref="Accept"/> function accepts a definition program symbol visitor and
        /// calls the appropriate "Visit()" function for this program symbol.
        /// </summary>
        /// <param name="v">An <see cref="IMemProgramSymbolVisitor{T}"/> visitor to accept.</param>
        /// <returns>
        /// A returned value of generic type T.
        /// </returns>
        T Accept(IMemProgramSymbolVisitor<T> v);
    }

    /// <summary>
    /// The <see cref="IMemProgramSymbolAcceptor{T,C}"/> defines the interface for program memory symbol accepting a visitor function with a given context.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the visitor function's return value.</typeparam>
    /// <typeparam name="C">Generic type parameter of the function's context parameter.</typeparam>
    public interface IMemProgramSymbolAcceptor<T, C>
    {
        /// <summary>
        /// The <see cref="Accept"/> function accepts a definition program symbol visitor and
        /// calls the appropriate "Visit()" function for this program symbol with the given context.
        /// </summary>
        /// <param name="v">An <see cref="IMemProgramSymbolVisitor{T,C}"/> visitor to accept.</param>
        /// <param name="context">The context of generic type C.</param>
        /// <returns>
        /// A returned value of generic type T.
        /// </returns>
        T Accept(IMemProgramSymbolVisitor<T, C> v, C context);
    }

    #endregion

}
