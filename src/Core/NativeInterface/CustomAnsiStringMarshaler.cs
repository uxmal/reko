#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
    public class CustomAnsiStringMarshaler : ICustomMarshaler
	{
		public void CleanUpManagedData(object ManagedObj) {
			// Intentionally left blank, there's nothing to do here
		}

		public void CleanUpNativeData(IntPtr pNativeData) {
			// Intentionally left blank, we don't want to free the original string
		}

		public int GetNativeDataSize()
		{
			// This means the object we're marshaling is not a value of predefined size
			return -1;
		}

		public IntPtr MarshalManagedToNative(object ManagedObj)
		{
			return Marshal.StringToHGlobalAnsi(ManagedObj as string);
		}

		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			return Marshal.PtrToStringAnsi(pNativeData)!;
		}
	}
}
