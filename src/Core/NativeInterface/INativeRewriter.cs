#region License
/* 
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("12506D0F-1C67-4828-9601-96F8ED4D162D")]
    [ComVisible(true)]
    [ComImport]
    [NativeInterop]
    public interface INativeRewriter
    {
        [PreserveSig] int Next();
        [PreserveSig] int GetCount();  //$DEBUG: used to track object leaks.
    }

	[ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("56E6600F-619E-441F-A2C3-A37F07BA0DA0")]
    [NativeInterop]
    [ComVisible(true)]
    public interface INativeRewriterHost
    {
        [PreserveSig] HExpr EnsureRegister(int regKind, int reg);
        [PreserveSig] HExpr EnsureSequence(int regHi, int regLo, BaseType size);
        [PreserveSig] HExpr EnsureFlagGroup(int baseReg, int bitmask, [MarshalAs(UnmanagedType.LPStr)] string name, BaseType size);
        [PreserveSig] HExpr CreateTemporary(BaseType type);
        [PreserveSig] void Error(ulong uAddress, [MarshalAs(UnmanagedType.LPStr)] string error);
        [PreserveSig] HExpr EnsurePseudoProcedure([MarshalAs(UnmanagedType.LPStr)] string name, BaseType dt, int arity);
    }
}
