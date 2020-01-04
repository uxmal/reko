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
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using Reko.Core.Types;
using System.IO;

namespace Reko.Core
{
	public class StructureMemberAttribute : Attribute
    {
		/// <summary>
		/// </summary>
		/// <param name="memberType">Type of a class to invoke to process the member</param>
		public StructureMemberAttribute(Type memberType) {
			this.MemberType = memberType;
		}

        public Type MemberType { get; private set; }

    }

    public static class BinaryReaderStructureReader
    {
        public static T ReadStruct<T>(this BinaryReader rdr) where T : struct
        {
            return new StructureReader<T>(rdr).Read();
        }
    }

    /// <summary>
    /// Reads in a structure field by field from an image reader.
    /// </summary>
    public class StructureReader<T> where T : struct
    {
		private Func<int, byte[]> readBytes;
		private Endianness defaultEndianess = Endianness.LittleEndian;

        public StructureReader(ImageReader reader) : this(reader.ReadBytes)
        {
        }

        public StructureReader(BinaryReader reader) : this(reader.ReadBytes)
        {
        }

        public StructureReader(Func<int, byte[]> readBytes)
        {
            this.readBytes = readBytes;
            if (typeof(T).IsDefined(typeof(EndianAttribute), false))
            {
                EndianAttribute attr = (EndianAttribute)(typeof(T).GetCustomAttribute(typeof(EndianAttribute), false));
                this.defaultEndianess = attr.Endianness;
            }
        }

        public T Read()
        {
			return this.BytesToStruct(this.readBytes);
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

		private int FieldSize(FieldInfo field) {
			if (field.FieldType.IsArray) {
				MarshalAsAttribute attr = (MarshalAsAttribute)field.GetCustomAttribute(typeof(MarshalAsAttribute), false);
				return Marshal.SizeOf(field.FieldType.GetElementType()) * attr.SizeConst;
			} else {
                if (field.FieldType.IsEnum) {
                    return Marshal.SizeOf( Enum.GetUnderlyingType(field.FieldType) );
                }
				return Marshal.SizeOf(field.FieldType);
			}
		}

		private void SwapEndian(byte[] data, Type type, FieldInfo field) {
			int offset = Marshal.OffsetOf(type, field.Name).ToInt32();
			if (field.FieldType.IsArray) {
				MarshalAsAttribute attr = (MarshalAsAttribute)field.GetCustomAttribute(typeof(MarshalAsAttribute), false);
				int subSize = Marshal.SizeOf(field.FieldType.GetElementType());
				for(int i=0; i<attr.SizeConst; i++) {
					Array.Reverse(data, offset + (i * subSize), subSize);
				}
			} else {
				Array.Reverse(data, offset, FieldSize(field));
			}
		}

		/* Adapted from http://stackoverflow.com/a/2624377 */
		private void RespectEndianness(Type type, byte[] data) {
			foreach (var field in type.GetFields()) {
				if (field.IsDefined(typeof(EndianAttribute), false)) {
					Endianness fieldEndianess = ((EndianAttribute)field.GetCustomAttributes(typeof(EndianAttribute), false)[0]).Endianness;
					if (
						(fieldEndianess == Endianness.BigEndian && BitConverter.IsLittleEndian) ||
						(fieldEndianess == Endianness.LittleEndian && !BitConverter.IsLittleEndian)
					) {
						SwapEndian(data, type, field);
					}
				} else if (
					(this.defaultEndianess == Endianness.BigEndian && BitConverter.IsLittleEndian) ||
					(this.defaultEndianess == Endianness.LittleEndian && !BitConverter.IsLittleEndian)
				) {
					SwapEndian(data, type, field);
				}
			}
		}

		private byte[] StructToBytes(T data) {
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

		private T BytesToStruct(Func<int, byte[]> readBytes) {
			byte[] bytes = readBytes(Marshal.SizeOf(typeof(T)));
			return this.BytesToStruct(bytes);
		}

		private T BytesToStruct(byte[] rawData) {
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
