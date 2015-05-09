using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Decompiler.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldAttribute : Attribute
    {
        public int Align = 1;

        private static Dictionary<Type, Func<ImageReader, object>> readers = new Dictionary<Type,Func<ImageReader,object>>
        {
            { typeof(ushort), r => r.ReadUInt16() },
            { typeof(uint), r => r.ReadUInt32() },
            { typeof(short), r => r.ReadInt16() },
            { typeof(int), r => r.ReadInt32() },
        };
                
        public virtual object ReadValue(System.Reflection.FieldInfo f, ImageReader rdr, ReaderContext ctx)
        {
            Func<ImageReader, object> fn;
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

        public override object ReadValue(FieldInfo f, ImageReader rdr, ReaderContext ctx)
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

    [AttributeUsage(AttributeTargets.Field)]
    public class PointerFieldAttribute : FieldAttribute
    {
        public int Size;

        public override object ReadValue(FieldInfo f, ImageReader rdr, ReaderContext ctx)
        {
            return ReadPointer(f.FieldType, Size, rdr, ctx);
        }

        public static object ReadPointer(Type pointerType, int size, ImageReader rdr, ReaderContext ctx)
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

        public override object ReadValue(FieldInfo f, ImageReader rdr, ReaderContext ctx)
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
}
