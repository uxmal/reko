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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Core.Lib
{
    public class UbjsonReader
    {
        private Stream stm;

        /// <summary>
        /// Convenience constructor. Wraps the byte array into a stream
        /// </summary>
        /// <param name="bytes"></param>
        public UbjsonReader(byte [] bytes) : this(new MemoryStream(bytes))
        {
        }

        public UbjsonReader(Stream stm)
        {
            this.stm = stm;
        }

        public object Read()
        {
            int n = stm.ReadByte();
            if (n < 0)
                throw new FormatException();

            return Read((UbjsonMarker)n);
        }

        private object Read(UbjsonMarker m)
        {
            for (;;)
            {
                switch (m)
                {
                case UbjsonMarker.Null: return null;
                case UbjsonMarker.Noop: m = (UbjsonMarker)stm.ReadByte(); break;
                case UbjsonMarker.False: return false; 
                case UbjsonMarker.True: return true;
                case UbjsonMarker.Int8: return (sbyte)ReadInteger(stm, 1);
                case UbjsonMarker.UInt8: return (byte)ReadInteger(stm, 1);
                case UbjsonMarker.Int16: return (short)ReadInteger(stm, 2);
                case UbjsonMarker.Int32: return (int) ReadInteger(stm, 4);
                case UbjsonMarker.Int64: return (long) ReadInteger(stm, 8);
                case UbjsonMarker.Float32: return BitConverter.ToSingle(ReadFloatBits(4), 0);
                case UbjsonMarker.Float64: return BitConverter.ToDouble(ReadFloatBits(8), 0);
                case UbjsonMarker.String: return ReadString();
                case UbjsonMarker.Array: return ReadArray();
                case UbjsonMarker.Object: return ReadObject();
                default:
                    throw new NotSupportedException(string.Format("Unknown marker {0}.", (char)m));
                }
            }
        }

        private static long ReadInteger(Stream stm, int bytes)
        {
            long num = 0;
            while (bytes > 0)
            {
                int n = stm.ReadByte();
                if (n < 0)
                    throw new FormatException();
                num = (num << 8) | (byte)n;
                --bytes;
            }
            return num;
        } 

        private byte[] ReadFloatBits(int size)
        {
            var buf = new byte[size];
            int n = stm.Read(buf, 0, buf.Length);
            if (n < size)
                throw new FormatException();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buf);
            return buf;
        }

        private uint ReadLength()
        {
            int n = stm.ReadByte();
            if (n < 0)
                throw new FormatException();
            return ReadLength((UbjsonMarker)n);
        }

        private uint ReadLength(UbjsonMarker m)
        { 
            switch(m)
            {
            case UbjsonMarker.Int8: return (byte)ReadInteger(stm, 1);
            case UbjsonMarker.UInt8: return (byte)ReadInteger(stm, 1);
            case UbjsonMarker.Int16: return (ushort)ReadInteger(stm, 2);
            case UbjsonMarker.Int32: return (uint)ReadInteger(stm, 4);
            }
            throw new NotSupportedException();
        }

        private string ReadString()
        {
            uint len = ReadLength();
            return ReadString(len);
        }

        private string ReadString(uint len)
        {
            var buf = new byte[len];
            stm.Read(buf, 0, (int)len);
            return Encoding.UTF8.GetString(buf);
        }

        private object ReadArray()
        {
            var b = stm.ReadByte();
            if (b < 0)
                throw new FormatException();
            var m = (UbjsonMarker)b;
            if (m == UbjsonMarker.ElementType)
            {
                int n = stm.ReadByte();
                if (n < 0)
                    throw new FormatException();
                var rdr = mpTypeArrayReader[(UbjsonMarker)n];
                n = stm.ReadByte();
                if ((UbjsonMarker)n != UbjsonMarker.ElementCount)
                    throw new FormatException();
                uint c = ReadLength();
                return rdr(stm, (int)c);
            }
            else if (m == UbjsonMarker.ElementType)
            {
                throw new NotImplementedException();
            }
            else
            {
                // Unoptimized array == ArrayList.
                var array = new List<object>();
                while (m != UbjsonMarker.ArrayEnd)
                {
                    array.Add(Read(m));
                    b = stm.ReadByte();
                    if (b < 0)
                        throw new FormatException();
                    m = (UbjsonMarker)b;
                }
                return array;
            }
        }

        private static Dictionary<UbjsonMarker, Func<Stream, int, object>> mpTypeArrayReader =
            new Dictionary<UbjsonMarker, Func<Stream, int, object>>
        {
            { UbjsonMarker.UInt8, ReadByteArray },
            { UbjsonMarker.Int32, ReadInt32Array }
        };

        private static object ReadByteArray(Stream stm, int count)
        {
            var arr = new byte[count];
            stm.Read(arr, 0, arr.Length);
            return arr;
        }

        private static object ReadInt32Array(Stream stm, int count)
        {
            var arr = new int[count];
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = (int) ReadInteger(stm, 4);
            }
            return arr;
        }

        private object ReadObject()
        {
            // Unoptimized object
            var dict = new Dictionary<string, object>();
            for (;;)
            {
                var n = stm.ReadByte();
                if (n < 0)
                    throw new FormatException();
                var m = (UbjsonMarker)n;
                if (m == UbjsonMarker.ObjectEnd)
                    break;
                var len = ReadLength(m);
                var key = ReadString(len);
                var obj = Read();
                dict[key] = obj;
            }
            return dict;
        }
    }
}
