#region License
/*
 * Copyright (C) 2018-2022 Stefano Moioli <smxdev4@gmail.com>.
 * 
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it 
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented;
 *    you must not claim that you wrote the original software.
 *    If you use this software in a product, an acknowledgment
 *    in the product documentation would be appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such,
 *    and must not be misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Core.IO
{
	public class SpanStream : Stream
	{

		delegate T ReaderDelegate<T>() where T : unmanaged;
		delegate void WriterDelegate<T>(T value) where T : unmanaged;

		private ReaderDelegate<ushort> u16Reader;
		private ReaderDelegate<uint> u32Reader;
		private ReaderDelegate<ulong> u64Reader;

		private WriterDelegate<ushort> u16Writer;
		private WriterDelegate<uint> u32Writer;
		private WriterDelegate<ulong> u64Writer;

		private int pos;
		private int marker;

		public override long Position {
			get => pos;
			set => pos = (int)value;
		}

		public void Mark() {
			marker = pos;
		}

		public long Remaining => Length - Position;
		public override long Length => Memory.Length;

		public Memory<byte> Memory { get; private set; }
		public Span<byte> Span => Memory.Span;

		private Endianness endianness = Endianness.LittleEndian;
		public Endianness Endianness {
			get => endianness;
			set {
				endianness = value;
				SetDelegates();
			}
		}

		public override bool CanRead { get; } = true;
		public override bool CanSeek { get; } = true;
		public override bool CanWrite { get; } = true;

		private void SetDelegates() {
			if (
				BitConverter.IsLittleEndian && Endianness == Endianness.LittleEndian ||
				!BitConverter.IsLittleEndian && Endianness == Endianness.BigEndian
			) {
				u16Reader = new ReaderDelegate<ushort>(Read<ushort>);
				u16Writer = new WriterDelegate<ushort>(Write<ushort>);
				u32Reader = new ReaderDelegate<uint>(Read<uint>);
				u32Writer = new WriterDelegate<uint>(Write<uint>);
				u64Reader = new ReaderDelegate<ulong>(Read<ulong>);
				u64Writer = new WriterDelegate<ulong>(Write<ulong>);
			} else {
				u16Reader = new ReaderDelegate<ushort>(ReadUInt16Swapped);
				u16Writer = new WriterDelegate<ushort>(WriteUInt16Swapped);
				u32Reader = new ReaderDelegate<uint>(ReadUInt32Swapped);
				u32Writer = new WriterDelegate<uint>(WriteUInt32Swapped);
				u64Reader = new ReaderDelegate<ulong>(ReadUInt64Swapped);
				u64Writer = new WriterDelegate<ulong>(WriteUInt64Swapped);
			}
		}

		public string ReadString(int length, Encoding? encoding = null) {
			if(encoding == null) {
				encoding = Encoding.ASCII;
			}
			byte[] bytes = ReadBytes(length);
			return encoding.GetString(bytes);
		}
		public byte[] ReadBytes(int count) {
			byte[] ret = Memory.Slice(pos, count).ToArray();
			pos += count;
			return ret;
		}

		private ushort ReadUInt16Swapped() {
			return ByteSwap16(Read<ushort>());
		}

		private void WriteUInt16Swapped(ushort value) {
			Write<ushort>(ByteSwap16(value));
		}

		private void WriteUInt32Swapped(uint value) {
			Write<uint>(ByteSwap32(value));
		}

		private void WriteUInt64Swapped(ulong value) {
			Write<ulong>(ByteSwap64(value));
		}

		private uint ReadUInt32Swapped() {
			return ByteSwap32(Read<uint>());
		}

		private ulong ReadUInt64Swapped() {
			return ByteSwap64(Read<ulong>());
		}

		private static ushort ByteSwap16(ushort num) {
			return (ushort)(
				((num & 0xFF00) >> 8) |
				((num & 0x00FF) << 8)
			);
		}

		private static uint ByteSwap32(uint num) {
			return (
				((num & 0xFF000000) >> 24) |
				((num & 0x00FF0000) >> 8) |
				((num & 0x0000FF00) << 8) |
				((num & 0x000000FF) << 24)
			);
		}

		private static ulong ByteSwap64(ulong num) {
			return (
				((num & 0xFF00000000000000) >> 56) |
				((num & 0x00FF000000000000) >> 40) |
				((num & 0x0000FF0000000000) >> 24) |
				((num & 0x000000FF00000000) >> 8) |
				((num & 0x00000000FF000000) << 8) |
				((num & 0x0000000000FF0000) << 24) |
				((num & 0x000000000000FF00) << 40) |
				((num & 0x00000000000000FF) << 56)
			);
		}


		private int FieldSize(FieldInfo field) {
			if (field.FieldType.IsArray) {
				MarshalAsAttribute attr = (MarshalAsAttribute?)field.GetCustomAttribute(typeof(MarshalAsAttribute), false)!;
				return Marshal.SizeOf(field.FieldType.GetElementType()!) * attr.SizeConst;
			} else {
				if (field.FieldType.IsEnum) {
					return Marshal.SizeOf(Enum.GetUnderlyingType(field.FieldType));
				}
				return Marshal.SizeOf(field.FieldType);
			}
		}

		private void SwapEndian<T>(byte[] data, FieldInfo field) {
			var type = typeof(T);
			int offset = Marshal.OffsetOf(type, field.Name).ToInt32();
			if (field.FieldType.IsArray) {
				MarshalAsAttribute attr = (MarshalAsAttribute?)field.GetCustomAttribute(typeof(MarshalAsAttribute), false)!;
				int subSize = Marshal.SizeOf(field.FieldType.GetElementType()!);
				for (int i = 0; i < attr.SizeConst; i++) {
					Array.Reverse(data, offset + (i * subSize), subSize);
				}
			} else {
				Array.Reverse(data, offset, FieldSize(field));
			}
		}

		private Endianness defaultEndianess = Endianness.LittleEndian;

		/* Adapted from http://stackoverflow.com/a/2624377 */
		private T RespectEndianness<T>(T data) {
			var structEndianness = this.defaultEndianess;
			var type = typeof(T);
			if (type.IsDefined(typeof(EndianAttribute), false)) {
				EndianAttribute attr = (EndianAttribute)type
					.GetCustomAttribute(typeof(EndianAttribute), false)!;
				structEndianness = attr.Endianness;
			}

			var sz = Marshal.SizeOf(data);
			var mem = Marshal.AllocHGlobal(sz);
			try {
				Marshal.StructureToPtr(data, mem, false);
				var bytes = new byte[sz];
				Marshal.Copy(mem, bytes, 0, sz);
				foreach (var field in type.GetFields()) {
					if (field.IsDefined(typeof(EndianAttribute), false)) {
						Endianness fieldEndianess = ((EndianAttribute)field.GetCustomAttributes(typeof(EndianAttribute), false)[0]).Endianness;
						if (
							(fieldEndianess == Endianness.BigEndian && BitConverter.IsLittleEndian) ||
							(fieldEndianess == Endianness.LittleEndian && !BitConverter.IsLittleEndian)
						) {
							SwapEndian<T>(bytes, field);
						}
					} else if (
						(structEndianness == Endianness.BigEndian && BitConverter.IsLittleEndian) ||
						(structEndianness == Endianness.LittleEndian && !BitConverter.IsLittleEndian)
					) {
						SwapEndian<T>(bytes, field);
					}
				}
				Marshal.Copy(bytes, 0, mem, sz);
				data = Marshal.PtrToStructure<T>(mem)!;
			} finally {
				Marshal.FreeHGlobal(mem);
			}

			return data;
		}

		private T AdjustFieldEndianness<T>(T value) where T : unmanaged {
			switch (value) {
				case sbyte num:
					return (T)(object)num;
				case byte num:
					return (T)(object)num;
			}

			if (BitConverter.IsLittleEndian && Endianness == Endianness.LittleEndian)
				return value;

			switch (value) {
				case short num:
					return (T)(object)ByteSwap16((ushort)num);
				case ushort num:
					return (T)(object)ByteSwap16(num);
				case int num:
					return (T)(object)ByteSwap32((uint)num);
				case uint num:
					return (T)(object)ByteSwap32(num);
				case long num:
					return (T)(object)ByteSwap64((ulong)num);
				case ulong num:
					return (T)(object)ByteSwap64(num);
				default:
					throw new NotImplementedException(typeof(T).Name);
			}
		}

		public unsafe T Read<T>() where T : unmanaged {
			var start = Memory.Span.Slice(pos, sizeof(T));
			T ret = MemoryMarshal.Cast<byte, T>(start)[0];
			pos += sizeof(T);
			return ret;
		}

		public override int Read(byte[] buffer, int offset, int count) {
			Memory.Span
				.Slice((int)Position, count)
				.ToArray()
				.CopyTo(buffer, offset);
			pos += count;
			return count;
		}

		public unsafe void Write<T>(T value) where T : unmanaged {
			var start = Memory.Span.Slice(pos, sizeof(T));
			MemoryMarshal.Cast<byte, T>(start)[0] = value;
			pos += sizeof(T);
		}

		public unsafe void WriteAt<T>(long offset, T value) where T : unmanaged {
			Memory.Span.Write<T>((int)offset, value);
		}

		public void WriteMemory<T>(Memory<T> data) where T : unmanaged {
			data.CopyTo(Memory, pos);
			pos += data.Length;
		}

		public void WriteSpan<T>(Span<T> data) where T : unmanaged {
			data.CopyTo(Span, pos);
			pos += data.Length;
		}

		public void WriteString(
			string str,
			bool lengthPrefix = false, int prefixLength = 4,
			bool nullTerminator = false,
			Encoding? encoding = null
		) {
			if(encoding == null) {
				encoding = Encoding.ASCII;
			}

			if (lengthPrefix) {
				int maxLength = 2 << ((8 * prefixLength) - 1);
				if(str.Length > maxLength) {
					throw new ArgumentException("Prefix length is too short");
				}
				switch (prefixLength) {
					case 1: WriteByte((byte)str.Length); break;
					case 2: WriteUInt16((ushort)str.Length); break;
					case 4: WriteUInt32((uint)str.Length); break;
					case 8: WriteUInt64((ulong)str.Length); break;
					default:
						throw new NotSupportedException($"Prefix length {prefixLength} not supported");
				}
			}

			int bufSize = encoding.GetByteCount(str);
			if (nullTerminator) {
				bufSize += encoding.GetByteCount("\0");
			}
			byte[] buf = new byte[bufSize];
			encoding.GetEncoder().GetBytes(str, buf, true);

			WriteBytes(buf);
		}

		public void WriteCString(string str) => WriteString(str,
			lengthPrefix: false,
			nullTerminator: true, encoding: Encoding.ASCII);


		public unsafe T ReadStruct<T>() where T : unmanaged {
			int length = sizeof(T);
			var start = Memory.Span.Slice(pos, length);

			T ret;
			ret = MemoryMarshal.Cast<byte, T>(start)[0];
			ret = RespectEndianness(ret);

			pos += length;
			return ret;
		}

		public SpanStream(SpanStream other) : this() {
			this.Memory = other.Memory.Slice(other.pos);
		}

		public SpanStream(Memory<byte> data, Endianness endianness = Endianness.LittleEndian) : this() {
			this.Memory = data;
			this.Endianness = endianness;
		}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private SpanStream() {
			SetDelegates();
			marker = pos;
		}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int SizeOf() {
			return pos - marker;
		}

		public void Replace(byte[] newData) {
			this.Memory = new Memory<byte>(newData);
		}

		public unsafe T ReadEnum<T>() where T : unmanaged {
			T value = ReadFlagsEnum<T>();

			Type enumType = typeof(T);
			if (!Enum.IsDefined(enumType, value)) {
				throw new InvalidDataException($"Value 0x{value:X} not defined in enum {enumType.FullName}");
			}

			return value;
		}

		public string ReadCString(Encoding encoding) {
			int start = (int)Position;
			while (Span[(int)(Position++)] != 0x00) ;

			// ignore trailing NULL
			byte[] data = Span.Slice(start, (int)(Position - start - 1)).ToArray();
			return encoding.GetString(data);
		}

		public string ReadCString() => ReadCString(Encoding.ASCII);

		public unsafe T ReadFlagsEnum<T>() where T : unmanaged {
			object value;
			switch (sizeof(T)) {
				case 1:
					value = ReadByte();
					break;
				case 2:
					value = ReadUInt16();
					break;
				case 4:
					value = ReadUInt32();
					break;
				case 8:
					value = ReadUInt64();
					break;
				default:
					throw new NotImplementedException();
			}


			return (T)value;
		}
		public int AlignStream(uint alignment) {
			long position = (Position + alignment - 1) & ~(alignment - 1);
			long skipped = position - Position;
			Position = position;
			return (int)skipped;
		}

		public void PerformAt(long offset, Action action) {
			long curPos = Position;
			Position = offset;
			action.Invoke();
			Position = curPos;
		}

		public T PerformAt<T>(long offset, Func<T> action) {
			long curPos = Position;
			Position = offset;
			T result = action.Invoke();
			Position = curPos;
			return result;
		}

		public IEnumerable<T> ReadAll<T>(Func<SpanStream, T> reader) {
			while (Remaining > 0) {
				T item = reader(this);
				yield return item;
			}
		}

		public override long Seek(long offset, SeekOrigin origin) {
			switch (origin) {
				case SeekOrigin.Begin:
					Position = offset;
					break;
				case SeekOrigin.Current:
					Position += offset;
					break;
				case SeekOrigin.End:
					Position = Length - offset;
					break;
			}
			return Position;
		}

		public SpanStream SliceHere() {
			return new SpanStream(this.Memory.Slice(this.pos), this.endianness);
		}

		public SpanStream SliceHere(int length) {
			return new SpanStream(this.Memory.Slice(this.pos, length), this.endianness);
		}

		public virtual byte[] ReadRemaining() {
			return ReadBytes((int)Remaining);
		}

		public byte ReadByte() => Read<byte>();
		public void WriteByte(byte value) => Write(value);

		public void WriteBytes(byte[] data) {
			var start = Memory.Span.Slice(pos, data.Length);
			var dspan = new Span<byte>(data);
			dspan.CopyTo(start);
			pos += data.Length;
		}

		public short ReadInt16() => (short)u16Reader();
		public void WriteInt16(Int16 value) => u16Writer((ushort)value);

		public int ReadInt32() => (int)u32Reader();
		public void WriteInt32(Int32 value) => u32Writer((uint)value);

		public long ReadInt64() => (long)u64Reader();
		public void WriteInt64(Int64 value) => u64Writer((ulong)value);

		//$TODO: support precisions > 32 and 64 bits?
		public float ReadSingle() => u32Reader();
		public void WriteSingle(float value) => u32Writer((uint)value);

		public double ReadDouble() => u64Reader();
		public void WriteDouble(double value) => u64Writer((ulong)value);

		public ushort ReadUInt16() => u16Reader();
		public void WriteUInt16(UInt16 value) => u16Writer(value);

		public uint ReadUInt32() => u32Reader();
		public void WriteUInt32(UInt32 value) => u32Writer(value);

		public ulong ReadUInt64() => u64Reader();
		public void WriteUInt64(UInt64 value) => u64Writer(value);

		public override void Flush() { }

		public override void SetLength(long value) {
			throw new NotSupportedException("SpanStream is not growable, grow the base stream instead");
		}

		public override void Write(byte[] buffer, int offset, int count) {
			Span<byte> source = ((Span<byte>)buffer).Slice(offset, count);
			Span<byte> dest = Memory.Span.Slice((int)Position, count);
			source.CopyTo(dest);
			Position += count;
		}
	}
}
