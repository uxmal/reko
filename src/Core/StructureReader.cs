#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using Reko.Core.Types;

namespace Reko.Core
{
	public class StructureMemberAttribute : Attribute {
		public Type memberType { get; private set; }

		/// <summary>
		/// </summary>
		/// <param name="memberType">Type of a class to invoke to process the member</param>
		public StructureMemberAttribute(Type memberType) {
			this.memberType = memberType;
		}
	}

	/// <summary>
	/// Reads in a structure field by field from an image reader.
	/// </summary>
	public class StructureReader<T> where T : struct
    {
        private T structure;
		private ImageReader reader;
		private Endianness defaultEndianess = Endianness.LittleEndian;

        public StructureReader(ImageReader reader)
        {
			this.reader = reader;
			if (typeof(T).IsDefined(typeof(EndianAttribute), false)) {
				EndianAttribute attr = (EndianAttribute)(typeof(T).GetCustomAttribute(typeof(EndianAttribute), false));
				this.defaultEndianess = attr.Endianness;
			}
        }

        public T Read()
        {
			return this.BytesToStruct<T>(this.reader);
        }

        private int GetAlignment(FieldInfo f)
        {
            var attrs = f.GetCustomAttributes(typeof(FieldAttribute), true);
            if (attrs == null || attrs.Length == 0)
            {
				return 1;
            }
            return ((FieldAttribute)attrs[0]).Align; 
        }

		/* Adapted from http://stackoverflow.com/a/2624377 */
		private void RespectEndianness(Type type, byte[] data) {
			var fields = type.GetFields().Where(f => f.IsDefined(typeof(EndianAttribute), false))
				.Select(f => new {
					Field = f,
					Attribute = (EndianAttribute)f.GetCustomAttributes(typeof(EndianAttribute), false)[0],
					Offset = Marshal.OffsetOf(type, f.Name).ToInt32()
				}).ToList();

			foreach (var field in fields) {
				if (
					((field.Attribute.Endianness == Endianness.BigEndian || this.defaultEndianess == Endianness.BigEndian)	&& BitConverter.IsLittleEndian) ||
					((field.Attribute.Endianness == Endianness.LittleEndian || this.defaultEndianess == Endianness.LittleEndian) && !BitConverter.IsLittleEndian)) {
					Array.Reverse(data, field.Offset, Marshal.SizeOf(field.Field.FieldType));
				}
			}
		}

		private byte[] StructToBytes<T>(T data) {
			byte[] rawData = new byte[Marshal.SizeOf(data)];
			GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
			try {
				IntPtr rawDataPtr = handle.AddrOfPinnedObject();
				Marshal.StructureToPtr(data, rawDataPtr, false);
			} finally {
				handle.Free();
			}

			RespectEndianness(typeof(T), rawData);

			return rawData;
		}

		private T BytesToStruct<T>(ImageReader reader) {
			byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
			return this.BytesToStruct<T>(bytes);
		}

		private T BytesToStruct<T>(byte[] rawData) {
			T result = default(T);

			RespectEndianness(typeof(T), rawData);

			GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);

			try {
				IntPtr rawDataPtr = handle.AddrOfPinnedObject();
				result = (T)Marshal.PtrToStructure(rawDataPtr, typeof(T));
			} finally {
				handle.Free();
			}

			return result;
		}
	}
}
