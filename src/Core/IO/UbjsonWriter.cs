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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Core.IO
{
    /// <summary>
    /// A class that writes the UBJSON file format.
    /// </summary>
    public class UbjsonWriter
    {
        private readonly Stream stm;

        /// <summary>
        /// Constructs an instance of <see cref="UbjsonWriter"/>.
        /// </summary>
        /// <param name="stm">The stream to write to.</param>
        public UbjsonWriter(Stream stm)
        {
            this.stm = stm;
        }

        /// <summary>
        /// Serializes an object to the UBJSON stream.
        /// </summary>
        /// <param name="o">Object to serialize.</param>
        public void Write(object? o)
        {
            switch (o)
            {
            case null:
                stm.WriteByte((byte)UbjsonMarker.Null);
                return;
            case bool f:
                stm.WriteByte(f ? (byte)UbjsonMarker.True : (byte)UbjsonMarker.False);
                return;
            case int i:
                WriteNumber(i);
                return;
            case long l:
                WriteNumber(l);
                return;
            case ulong ul:
                WriteUnsignedNumber(ul);
                return;
            case string s:
                stm.WriteByte((byte) UbjsonMarker.String);
                WriteString(s);
                return;
            case IDictionary d:
                WriteObject(d);
                return;
            case IEnumerable e:
                var itf = o.GetType().GetInterfaces().FirstOrDefault(x =>
                     x.IsGenericType &&
                     x.GetGenericTypeDefinition() == typeof(ICollection<>));
                if (itf is not null)
                {
                    var elementType = itf.GetGenericArguments()[0];
                    var elementMarker = GetMarker(elementType);
                    if (elementMarker != UbjsonMarker.None)
                    {
                        Action<object, Stream> renderer = GetRenderer(elementType);
                        int c = (int) itf.GetProperty("Count")!.GetValue(o, null)!;
                        WriteArray(e, c, elementMarker, renderer);
                        return;
                    }
                }
                WriteArray(e);
                return;
            default:
                if (o is ValueType)
                    throw new NotSupportedException($"Writing data type {o.GetType().FullName} is not supported.");
                WriteClassAsObject(o);
                return;
            }
        }

        private void WriteString(string s)
        {
            var enc = Encoding.UTF8.GetByteCount(s);
            WriteNumber(enc);
            var tx = new StreamWriter(stm, Encoding.UTF8);
            tx.Write(s);
            tx.Flush();
        }

        private void WriteDictionaryEntry(string key, object? value)
        {
            WriteString(key);
            Write(value);
        }

        private static readonly Dictionary<Type, UbjsonMarker> mpTypeMarker = new Dictionary<Type, UbjsonMarker>
        {
            { typeof(byte), UbjsonMarker.UInt8 },
            { typeof(sbyte), UbjsonMarker.Int8 },
            { typeof(short), UbjsonMarker.Int16 },
            { typeof(int), UbjsonMarker.Int32 },
            { typeof(long), UbjsonMarker.Int64 },
        };

        private UbjsonMarker GetMarker(Type type)
        {
            if (!mpTypeMarker.TryGetValue(type, out UbjsonMarker marker))
                marker = UbjsonMarker.None;
            return marker;
        }

        private static readonly Dictionary<Type, Action<object, Stream>> mpType = new Dictionary<Type, Action<object, Stream>>
        {
            { typeof(byte), WriteByte },
            { typeof(sbyte), WriteSByte },
            { typeof(short), WriteInt16 },
            { typeof(int), WriteInt32 },
            { typeof(long), WriteInt64 },
        };

        private static Action<object,Stream> GetRenderer(Type type)
        {
            //$REVIEW: what if we can't find a renderer?
            if (!mpType.TryGetValue(type, out var action))
                action = null!;
            return action;
        }

        private static void WriteByte(object o, Stream stm)
        {
            stm.WriteByte((byte)o);
        }

        private static void WriteSByte(object o, Stream stm)
        {
            stm.WriteByte((byte)(sbyte)o);
        }

        private static void WriteInt16(object o, Stream stm)
        {
            var n = (short)o;
            stm.WriteByte((byte)(n >> 8));
            stm.WriteByte((byte)n);
        }

        private static void WriteInt32(object o, Stream stm)
        {
            var n = (int)o;
            stm.WriteByte((byte)(n >> 24));
            stm.WriteByte((byte)(n >> 16));
            stm.WriteByte((byte)(n >> 8));
            stm.WriteByte((byte)n);
        }

        private static void WriteInt64(object o, Stream stm)
        {
            var n = (long)o;
            stm.WriteByte((byte)(n >> 56));
            stm.WriteByte((byte)(n >> 48));
            stm.WriteByte((byte)(n >> 40));
            stm.WriteByte((byte)(n >> 32));
            stm.WriteByte((byte)(n >> 24));
            stm.WriteByte((byte)(n >> 16));
            stm.WriteByte((byte)(n >> 8));
            stm.WriteByte((byte)n);
        }

        private void WriteArray(IEnumerable e)
        {
            stm.WriteByte((byte)UbjsonMarker.Array);
            foreach (var item in e)
            {
                Write(item);
            }
            stm.WriteByte((byte)UbjsonMarker.ArrayEnd);
        }

        private void WriteArray(IEnumerable e, int c, UbjsonMarker elementType, Action<object, Stream> writer)
        {
            stm.WriteByte((byte)UbjsonMarker.Array);
            if (elementType != UbjsonMarker.None)
            {
                stm.WriteByte((byte)UbjsonMarker.ElementType);
                stm.WriteByte((byte)elementType);
            }
            stm.WriteByte((byte)UbjsonMarker.ElementCount);
            WriteNumber(c);
            foreach (var item in e)
            {
                writer(item, stm);
            }
        }

        private void WriteObject(IDictionary d)
        {
            stm.WriteByte((byte)UbjsonMarker.Object);
            foreach (DictionaryEntry de in d)
            {
                WriteDictionaryEntry(de.Key.ToString()!, de.Value);
            }
            stm.WriteByte((byte)UbjsonMarker.ObjectEnd);
        }

        private void WriteClassAsObject(object o)
        {
            stm.WriteByte((byte)UbjsonMarker.Object);
            var t = o.GetType();
            foreach (var p in t.GetProperties())
            {
                WriteDictionaryEntry(p.Name, p.GetValue(o, null));
            }
            foreach (var f in t.GetFields())
            {
                WriteDictionaryEntry(f.Name, f.GetValue(o));
            }
            stm.WriteByte((byte)UbjsonMarker.ObjectEnd);
        }

        private void WriteNumber(long num)
        {
            if (num < 256)
            {
                stm.WriteByte((byte)UbjsonMarker.Int8);
                stm.WriteByte((byte)num);
                return;
            }
            if (num < 65536)
            {
                stm.WriteByte((byte)UbjsonMarker.Int16);
                stm.WriteByte((byte)(num >> 8));
                stm.WriteByte((byte)num);
                return;
            }
            if (num < (1L << 32))
            {
                stm.WriteByte((byte)UbjsonMarker.Int32);
                stm.WriteByte((byte)(num >> 24));
                stm.WriteByte((byte)(num >> 16));
                stm.WriteByte((byte)(num >> 8));
                stm.WriteByte((byte)num);
                return;
            }
            stm.WriteByte((byte)UbjsonMarker.Int64);
            stm.WriteByte((byte)(num >> 56));
            stm.WriteByte((byte)(num >> 48));
            stm.WriteByte((byte)(num >> 40));
            stm.WriteByte((byte)(num >> 32));
            stm.WriteByte((byte)(num >> 24));
            stm.WriteByte((byte)(num >> 16));
            stm.WriteByte((byte)(num >> 8));
            stm.WriteByte((byte)num);
        }

        private void WriteUnsignedNumber(ulong num)
        {
            if (num < 128)
            {
                stm.WriteByte((byte)UbjsonMarker.Int8);
                stm.WriteByte((byte)num);
                return;
            }
            if (num < 32768)
            {
                stm.WriteByte((byte)UbjsonMarker.Int16);
                stm.WriteByte((byte)(num >> 8));
                stm.WriteByte((byte)num);
                return;
            }
            if (num < (1L << 31))
            {
                stm.WriteByte((byte)UbjsonMarker.Int32);
                stm.WriteByte((byte)(num >> 24));
                stm.WriteByte((byte)(num >> 16));
                stm.WriteByte((byte)(num >> 8));
                stm.WriteByte((byte)num);
                return;
            }
            stm.WriteByte((byte)UbjsonMarker.Int64);
            stm.WriteByte((byte)(num >> 56));
            stm.WriteByte((byte)(num >> 48));
            stm.WriteByte((byte)(num >> 40));
            stm.WriteByte((byte)(num >> 32));
            stm.WriteByte((byte)(num >> 24));
            stm.WriteByte((byte)(num >> 16));
            stm.WriteByte((byte)(num >> 8));
            stm.WriteByte((byte)num);
        }
    }
}
