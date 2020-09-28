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
    [Guid("22D115E1-E432-4FD1-86D6-F42225063768")]
    [ComVisible(true)]
    [ComImport]
    [NativeInterop]
    public interface INativeTypeFactory
    {
        [PreserveSig] HExpr ArrayOf(HExpr dt, int length);

        [PreserveSig] HExpr PtrTo(HExpr dt, int byte_size);

        [PreserveSig] void BeginStruct(HExpr dt, int byte_size);
        [PreserveSig] void Field(HExpr dt, int offset, [MarshalAs(UnmanagedType.LPStr)]  string name);
        [PreserveSig] HExpr EndStruct();

        [PreserveSig] void BeginFunc(HExpr dt, int byte_size);
        [PreserveSig] void Parameter(HExpr dt, [MarshalAs(UnmanagedType.LPStr)] string name);
        [PreserveSig] HExpr EndFunc();
    }
}
