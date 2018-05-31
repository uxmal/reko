#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

using System.Collections.Generic;

namespace Reko.Libraries.Microchip
{

    public interface IRegisterBasicInfo
    {
        /// <summary> Gets the data memory address of this register. </summary>
        uint Addr { get; }

        /// <summary> Gets the name of this register. </summary>
        string Name { get; }

        /// <summary> Gets the textual description of this register. </summary>
        string Description { get; }

        /// <summary> Gets the bit width of this register. </summary>
        byte BitWidth { get; }

    }

    /// <summary>
    /// This interface provides access to the definition of a Special Function Register (SFR).
    /// </summary>
    public interface ISFRRegister : IRegisterBasicInfo
    {

        /// <summary> Gets the byte width of this SFR. </summary>
        int ByteWidth { get; }

        /// <summary> Gets the implemented bits mask of this SFR. </summary>
        uint ImplMask { get; }

        /// <summary> Gets the access mode bits descriptor for this SFR. </summary>
        string Access { get; }

        /// <summary> Gets the Master Clear (MCLR) bits values (string) of this SFR. </summary>
        string MCLR { get; }

        /// <summary> Gets the Power-ON Reset bits values (string) of this SFR. </summary>
        string POR { get; }

        /// <summary> Gets a value indicating whether this SFR is indirect. </summary>
        bool IsIndirect { get; }

        /// <summary> Gets a value indicating whether this SFR is volatile. </summary>
        bool IsVolatile { get; }

        /// <summary> Gets a value indicating whether this SFR is hidden. </summary>
        bool IsHidden { get; }

        /// <summary> Gets a value indicating whether this SFR is hidden to language tools. </summary>
        bool IsLangHidden { get; }

        /// <summary> Gets a value indicating whether this SFR is hidden to MPLAB IDE. </summary>
        bool IsIDEHidden { get; }

        /// <summary> Gets the Non-Memory-Mapped-Register identifier of the SFR. </summary>
        string NMMRID { get; }

        /// <summary> Gets a value indicating whether this SFR is Non-Memory-Mapped. </summary>
        bool IsNMMR { get; }

        /// <summary> Enumerates the definition of the bit fields contained in this SFR. </summary>
        IEnumerable<ISFRBitField> BitFields { get; }

    }

    /// <summary>
    /// This interface provides access to a Special Function Register (SFR) bit field.
    /// </summary>
    public interface ISFRBitField : IRegisterBitField
    {
        /// <summary> Enumerates the list of semantics of this SFR field. </summary>
        IEnumerable<ISFRFieldSemantic> FieldSemantics { get; }
    }

    /// <summary>
    /// This interface provides access to a SFR field semantic (condition of activation in the PIC).
    /// </summary>
    public interface ISFRFieldSemantic
    {
        /// <summary> Gets the textual description of the semantic.</summary>
        string Description { get; }

        /// <summary> Gets the "when" condition of the semantic. </summary>
        string When { get; }
    }

    public interface IJoinedRegister : IRegisterBasicInfo
    {

        /// <summary>
        /// Enumerates the child SFRs of this joined register.
        /// </summary>
        IEnumerable<ISFRRegister> ChildSFRs { get; }

    }

}
