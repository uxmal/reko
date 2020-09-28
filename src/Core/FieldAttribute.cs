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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Reko.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldAttribute : Attribute
    {
        public int Align = 1;

        private static Dictionary<Type, Func<EndianImageReader, object>> readers = new Dictionary<Type,Func<EndianImageReader, object>>
        {
            { typeof(ushort), r => r.ReadUInt16() },
            { typeof(uint), r => r.ReadUInt32() },
            { typeof(short), r => r.ReadInt16() },
            { typeof(int), r => r.ReadInt32() },
        };
                
        public virtual object ReadValue(System.Reflection.FieldInfo f, EndianImageReader rdr, ReaderContext ctx)
        {
            Func<EndianImageReader, object> fn;
            if (readers.TryGetValue(f.FieldType, out fn))
            {
                return fn(rdr);
            }
            throw new NotSupportedException(string.Format("Field type {0} not supported.", f.FieldType.FullName));
        }
    }

    public class ReaderContext
    {
        private object obj;
        private FieldInfo[] fields;
        public ReaderContext(object obj, FieldInfo[] fields)
        {
            this.obj = obj;
            this.fields = fields;
        }

        public object GetValue(string valueName)
        {
            return fields.Where(f => f.Name == valueName).Single().GetValue(obj);
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class StringFieldAttribute : FieldAttribute
    {
        public bool NullTerminated;

        public override object ReadValue(FieldInfo f, EndianImageReader rdr, ReaderContext ctx)
        {
            int i = 0;
            for (; rdr.PeekByte(i) != 0; ++i)
            {
            }
            var s = Encoding.UTF8.GetString(rdr.ReadBytes(i));
            rdr.Offset++;
            return s;
        }
    }

#if false
	[AttributeUsage(AttributeTargets.Field)]
    public class PointerFieldAttribute : FieldAttribute
    {
        public int Size;

        public override object ReadValue(FieldInfo f, EndianImageReader rdr, ReaderContext ctx)
        {
            return ReadPointer(f.FieldType, Size, rdr, ctx);
        }

        public static object ReadPointer(Type pointerType, int size, EndianImageReader rdr, ReaderContext ctx)
        {
            Debug.Print("Reading pointer at offset {0}, size {1}", rdr.Offset, size);
            uint newOffset;
            switch (size)
            {
            default:
                throw new InvalidOperationException("Field size must be > 0.");
            case 1: newOffset = rdr.ReadByte(); break;
            case 2: newOffset = rdr.ReadUInt16(); break;
            case 4: newOffset = rdr.ReadUInt32(); break;
            }
            Debug.Print("Structure of type {0} must start at offset {1:X}", pointerType.Name, newOffset);
            rdr = rdr.Clone();
            rdr.Offset = newOffset;

            var dst = Activator.CreateInstance(pointerType);
            var sr = new StructureReader(dst);
            sr.Read(rdr);
            return dst;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ArrayFieldAttribute : FieldAttribute
    {
        public int Length;
        public int PointerElementSize;

        public override object ReadValue(FieldInfo f, EndianImageReader rdr, ReaderContext ctx)
        {
            var elemType = f.FieldType.GetElementType(); 
            if (Length > 0)
            {
                Debug.Print("Array: at offset {0}, reading {1} entries", rdr.Offset, Length);
                var a = (Array) Activator.CreateInstance(f.FieldType, Length);
                if (PointerElementSize > 0)
                {
                    for (int i = 0; i < Length; ++i)
                    {
                        var value= PointerFieldAttribute.ReadPointer(elemType, PointerElementSize, rdr, ctx);
                        a.SetValue(value, i);
                    }
                    return a;
                }
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }
    }
#endif
}
