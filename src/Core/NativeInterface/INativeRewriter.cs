#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Runtime.InteropServices;

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// Interface implemented by native code lifters.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("12506D0F-1C67-4828-9601-96F8ED4D162D")]
    [ComVisible(true)]
    [ComImport]
    [NativeInterop]
    public interface INativeRewriter
    {
        /// <summary>
        /// Disassembles one instruction and lifts it. 
        /// </summary>
        /// <returns></returns>
        [PreserveSig] int Next();

        /// <summary>
        /// Debugging interface.
        /// </summary>
        /// <returns></returns>
        [PreserveSig] int GetCount();  //$DEBUG: used to track object leaks.
    }

    /// <summary>
    /// Services provided by the lifting managed host to the native lifter.
    /// </summary>

	[ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("06E02449-39EB-4833-9247-D1CA03F3D9B5")]
    [NativeInterop]
    [ComVisible(true)]
    public interface INativeRewriterHost
    {
        /// <summary>
        /// Ensures an identifier for the given register.
        /// </summary>
        /// <param name="regKind">Processor specific register class.</param>
        /// <param name="reg">Processor specific register identifier.</param>
        /// <returns>An <see cref="HExpr"/> wrapping the created identifier.
        /// </returns>
        [PreserveSig] HExpr EnsureRegister(int regKind, int reg);

        /// <summary>
        /// Ensures a identifier for the sequence defined by the <paramref name="regHi"/>
        /// and <paramref name="regLo"/>.
        /// </summary>
        /// <param name="regHi">Most significant part of the sequence of registers.</param>
        /// <param name="regLo">Least significant part of the sequence of registers.</param>
        /// <param name="size">Data type of the sequence.
        /// </param>
        /// <returns>An <see cref="HExpr"/> wrapping the created identifier.
        /// </returns>
        [PreserveSig] HExpr EnsureSequence(int regHi, int regLo, BaseType size);

        /// <summary>
        /// Ensures a identifier for the flag group defined by the parameters.
        /// </summary>
        /// <param name="baseReg">Status register in which the flag group resides.
        /// </param>
        /// <param name="bitmask">Bit mask that defines the flag group.</param>
        /// <param name="name">Name of the flag group.</param>
        /// <returns>An <see cref="HExpr"/> wrapping the created identifier.
        /// </returns>
        [PreserveSig] HExpr EnsureFlagGroup(int baseReg, int bitmask, [MarshalAs(UnmanagedType.LPStr)] string name);

        /// <summary>
        /// Creates a temporary variable of the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>An <see cref="HExpr"/> wrapping the created identifier.
        /// </returns>
        [PreserveSig] HExpr CreateTemporary(BaseType type);

        /// <summary>
        /// Displays the given error message.
        /// </summary>
        /// <param name="uAddress">Address at which the error happened.</param>
        /// <param name="error">Error message.</param>
        [PreserveSig] void Error(ulong uAddress, [MarshalAs(UnmanagedType.LPStr)] string error);

        /// <summary>
        /// Ensures an intrinsic procedure expression.
        /// </summary>
        /// <param name="name">Name of the intrinsic procedure.</param>
        /// <param name="hasSideEffect">True if the intrinsic has a side effect.</param>
        /// <param name="dt">Data type returned from the intrinsic.</param>
        /// <param name="arity">number of parameters to the intrinsic.</param>
        /// <returns>A wrapped <see cref="IntrinsicProcedure"/> to be used in expressions.</returns>
        [PreserveSig] HExpr EnsureIntrinsicProcedure([MarshalAs(UnmanagedType.LPStr)] string name, int hasSideEffect, BaseType dt, int arity);
    }
}
