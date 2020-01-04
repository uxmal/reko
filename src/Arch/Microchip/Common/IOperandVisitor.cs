#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2020 John Källén.
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

namespace Reko.Arch.MicrochipPIC.Common
{

    /// <summary>
    /// Interface for PIC Operands' visitors.
    /// </summary>
    public interface IOperand
    {
        void Accept(IOperandVisitor visitor);
        T Accept<T>(IOperandVisitor<T> visitor);
        T Accept<T, C>(IOperandVisitor<T, C> visitor, C context);

    }

    /// <summary>
    /// Interface defining the permitted visitor methods on PIC Operands.
    /// </summary>
    public interface IOperandVisitor
    {
        void VisitImmediate(PICOperandImmediate imm);
        void VisitFast(PICOperandFast fast);
        void VisitProgMemory(PICOperandProgMemoryAddress prgAddr);
        void VisitDataMemory(PICOperandDataMemoryAddress dataAddr);
        void VisitBankedMemory(PICOperandBankedMemory mem);
        void VisitMemBitNo(PICOperandMemBitNo memBitNo);
        void VisitMemWRegDest(PICOperandMemWRegDest memWRegDest);
        void VisitRegister(PICOperandRegister reg);
        void VisitFSRNumber(PICOperandFSRNum idx);
        void VisitFSRIndexation(PICOperandFSRIndexation idx);
        void VisitTblRW(PICOperandTBLRW reg);
        void VisitTris(PICOperandTris trisnum);
        void VisitEEPROM(PICOperandDEEPROM eeprom);
        void VisitASCII(PICOperandDASCII ascii);
        void VisitDB(PICOperandDByte databyte);
        void VisitDW(PICOperandDWord dataword);
        void VisitIDLocs(PICOperandIDLocs idloc);
        void VisitConfig(PICOperandConfigBits configbits);
    }

    /// <summary>
    /// Interface defining the permitted visitor functions on PIC Operands.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the functions result.</typeparam>
    public interface IOperandVisitor<T>
    {
        T VisitImmediate(PICOperandImmediate imm);
        T VisitFast(PICOperandFast fast);
        T VisitProgMemory(PICOperandProgMemoryAddress prgAddr);
        T VisitDataMemory(PICOperandDataMemoryAddress dataAddr);
        T VisitBankedMemory(PICOperandBankedMemory mem);
        T VisitMemBitNo(PICOperandMemBitNo memBitNo);
        T VisitMemWRegDest(PICOperandMemWRegDest memWRegDest);
        T VisitRegister(PICOperandRegister reg);
        T VisitFSRNumber(PICOperandFSRNum idx);
        T VisitFSRIndexation(PICOperandFSRIndexation idx);
        T VisitTblRW(PICOperandTBLRW reg);
        T VisitTris(PICOperandTris trisnum);
        T VisitEEPROM(PICOperandDEEPROM eeprom);
        T VisitASCII(PICOperandDASCII ascii);
        T VisitDB(PICOperandDByte databyte);
        T VisitDW(PICOperandDWord dataword);
        T VisitIDLocs(PICOperandIDLocs idloc);
        T VisitConfig(PICOperandConfigBits configbits);
    }

    /// <summary>
    /// Interface defining the permitted visitor functions with context on PIC Operands.
    /// </summary>
    /// <typeparam name="T">Generic type parameter of the functions result.</typeparam>
    /// <typeparam name="C">Generic type parameter of the context.</typeparam>
    public interface IOperandVisitor<T, C>
    {
        T VisitImmediate(PICOperandImmediate imm, C ctx);
        T VisitFast(PICOperandFast fast, C ctx);
        T VisitProgMemory(PICOperandProgMemoryAddress prgAddr, C ctx);
        T VisitDataMemory(PICOperandDataMemoryAddress dataAddr, C ctx);
        T VisitBankedMemory(PICOperandBankedMemory mem, C ctx);
        T VisitMemBitNo(PICOperandMemBitNo memBitNo, C ctx);
        T VisitMemWRegDest(PICOperandMemWRegDest memWRegDest, C ctx);
        T VisitRegister(PICOperandRegister reg, C ctx);
        T VisitFSRNumber(PICOperandFSRNum idx, C ctx);
        T VisitFSRIndexation(PICOperandFSRIndexation idx, C ctx);
        T VisitTblRW(PICOperandTBLRW reg, C ctx);
        T VisitTris(PICOperandTris trisnum, C ctx);
        T VisitEEPROM(PICOperandDEEPROM eeprom, C ctx);
        T VisitASCII(PICOperandDASCII ascii, C ctx);
        T VisitDB(PICOperandDByte databyte, C ctx);
        T VisitDW(PICOperandDWord dataword, C ctx);
        T VisitIDLocs(PICOperandIDLocs idloc, C ctx);
        T VisitConfig(PICOperandConfigBits configbits, C ctx);
    }

}
