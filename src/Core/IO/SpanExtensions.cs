#region License
/*
 * Copyright (C) 2018-2026 Stefano Moioli <smxdev4@gmail.com>.
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
using System.Runtime.InteropServices;

#pragma warning disable CS1591

namespace Reko.Core.IO
{
    /// <summary>
    /// Extensions to <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
	public static class SpanExtensions
	{
		public unsafe static T Read<T>(this ReadOnlySpan<byte> data, int offset) where T : unmanaged {
			int length = sizeof(T);
			return Cast<T>(data.Slice(offset, length))[0];
		}

		public unsafe static T Read<T>(this Span<byte> data, int offset) where T : unmanaged {
			int length = sizeof(T);
			return Cast<T>(data.Slice(offset, length))[0];
		}

		public unsafe static void Write<T>(this Span<byte> data, int offset, T value) where T : unmanaged {
			int length = sizeof(T);
			Cast<T>(data.Slice(offset, length))[0] = value;
		}

		public unsafe static void CopyTo<TFrom, TTo>(this Span<TFrom> data, Span<TTo> dest, int dstOffset)
			where TFrom : unmanaged
			where TTo : unmanaged
		{
			var srcBytes = MemoryMarshal.Cast<TFrom, byte>(data);
			var dstBytes = MemoryMarshal.Cast<TTo, byte>(dest).Slice(dstOffset);
			srcBytes.CopyTo(dstBytes);
		}

		public unsafe static void CopyTo<TFrom, TTo>(this Memory<TFrom> data, Memory<TTo> dest, int dstOffset)
			where TFrom : unmanaged
			where TTo : unmanaged
		{
			data.Span.CopyTo(dest.Span, dstOffset);
		}

        public unsafe static void WriteBytes(this Span<byte> data, int offset, byte[] bytes)
        {
			var start = data.Slice(offset, bytes.Length);
			var dspan = new Span<byte>(bytes);
			dspan.CopyTo(start);
		}

		public static ReadOnlySpan<T> Cast<T>(this ReadOnlySpan<byte> data) where T : struct {
			return MemoryMarshal.Cast<byte, T>(data);
		}

		public static Span<T> Cast<T>(this Span<byte> data) where T : struct {
			return MemoryMarshal.Cast<byte, T>(data);
		}
	}
}
