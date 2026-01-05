#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.IO;

namespace Reko.Core.Serialization.Json
{
    /// <summary>
    /// Writes JSON data to a text writer.
    /// </summary>
    public class JsonWriter
    {
        private TextWriter w;
        private Stack<bool> needsSeparator;

        /// <summary>
        /// Creates an instance of <see cref="JsonWriter"/> with the specified text writer.
        /// </summary>
        /// <param name="w">Output sink.</param>
        public JsonWriter(TextWriter w)
        {
            this.w = w;
            this.needsSeparator = new Stack<bool>();
        }

        /// <summary>
        /// Begins writing a JSON object.
        /// </summary>
        public void BeginObject()
        {
            needsSeparator.Push(false);
            w.Write('{');
        }


        /// <summary>
        /// Terminates a JSON object.
        /// </summary>
        public void EndObject()
        {
            needsSeparator.Pop();
            w.Write('}');
        }

        /// <summary>
        /// Writes a JSON array.
        /// </summary>
        /// <typeparam name="T">Array item type.</typeparam>
        /// <param name="items">The items to render.</param>
        /// <param name="itemWriter">Delegate used to write individual items.</param>
        public void WriteList<T>(IEnumerable<T> items, Action<T> itemWriter)
        {
            w.Write('[');
            WriteListContents(items, itemWriter);
            w.Write(']');
        }

        /// <summary>
        /// Writes the contents of a JSON array.
        /// </summary>
        /// <param name="items">The items to render.</param>
        /// <param name="itemWriter">Delegate used to write individual items.</param>
        public void WriteListContents<T>(IEnumerable<T> items, Action<T> itemWriter)
        {
            bool sep = false;
            foreach (var item in items)
            {
                if (sep)
                    w.Write(',');
                sep = true;
                itemWriter(item);
            }
        }

        private static Dictionary<Type, Action<JsonWriter, object>> atomWriters =
            new Dictionary<Type, Action<JsonWriter, object>>
            {
                { typeof(bool), (js, o) => js.Write((bool)o) },
                { typeof(int), (js, o) => js.Write((int)o) },
                { typeof(uint), (js, o) => js.Write((uint)o) },
                { typeof(long), (js, o) => js.Write((long)o) },
                { typeof(ulong), (js, o) => js.Write((ulong)o) },
                { typeof(string), (js, o) => js.Write((string)o) },
            };

        /// <summary>
        /// Writes a JSON value.
        /// </summary>
        /// <param name="o">Value to write.</param>
        public void Write(object o)
        {
            if (o is null)
            {
                w.Write("null");
                return;
            }
            Action<JsonWriter, object>? atomWriter;
            if (atomWriters.TryGetValue(o.GetType(), out atomWriter))
            {
                atomWriter(this, o);
                return;
            }
            throw new NotImplementedException(string.Format("Don't know how to write {0}.", o.GetType()));
        }

        /// <summary>
        /// Writes a JSON boolean value.
        /// </summary>
        /// <param name="value">Value to write.</param>
        public void Write(bool value)
        {
            w.Write(value ? "true" : "false");
        }

        /// <summary>
        /// Writes a JSON integer value.
        /// </summary>
        /// <param name="u64">Value to write.</param>
        public void Write(ulong u64)
        {
            // Max integer representable with a IEEE 64-bit double
            const ulong MaxExactUInt = (1ul << 53) - 1ul;
            if (u64 > MaxExactUInt)
                Write(u64.ToString());
            else
                w.Write(u64);
        }

        /// <summary>
        /// Writes a JSON integer value.
        /// </summary>
        /// <param name="i64">Value to write.</param>
        public void Write(long i64)
        {
            // Max integer representable with a IEEE 64-bit double
            const long MaxExactInt = (1L << 53) - 1L;
            if (Math.Abs(i64) > MaxExactInt)
                Write(i64.ToString());
            else
                w.Write(i64);
        }

        /// <summary>
        /// Writes a JSON string value.
        /// </summary>
        /// <param name="s">String to write.</param>
        public void Write(string s)
        {
            w.Write('"');
            foreach (char c in s)
            {
                switch (c)
                {
                case '\b': w.Write(@"\"); break;
                case '\t': w.Write(@"\"); break;
                case '\n': w.Write(@"\"); break;
                case '\f': w.Write(@"\"); break;
                case '\r': w.Write(@"\"); break;
                case '\"': w.Write(@"\"""); break;
                case '\\': w.Write(@"\\"); break;
                default:
                    if (0 <= c && c < ' ' || 0x7F <= c)
                    {
                        w.Write(@"\u{0:X4}", (int)c);
                    }
                    else
                    {
                        w.Write(c);
                    }
                    break;
                }
            }
            w.Write('"');
        }

        /// <summary>
        /// Writes a key-value pair to the JSON object.
        /// </summary>
        /// <param name="key">Key to write.</param>
        /// <param name="value">Value to write.</param>
        public void WriteKeyValue(string key, object value)
        {
            WriteSeparator();
            Write(key);
            w.Write(':');
            Write(value);
        }

        /// <summary>
        /// Writes a key-value pair to the JSON object.
        /// </summary>
        /// <param name="key">Key to write.</param>
        /// <param name="valueWriter">Delegate that writes the value.</param>
        public void WriteKeyValue(string key, Action valueWriter)
        {
            WriteSeparator();
            Write(key);
            w.Write(':');
            valueWriter();
        }

        private void WriteSeparator()
        {
            if (needsSeparator.Peek())
            {
                w.Write(',');
            }
            else
            {
                needsSeparator.Pop();
                needsSeparator.Push(true);
            }
        }
    }
}
