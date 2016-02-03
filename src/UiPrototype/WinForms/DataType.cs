using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UiPrototype.WinForms
{
    public abstract class DataType
    {
        public abstract int GetSize();
    }

    public class SimpleType : DataType
    {
        public SimpleType(Domain dom, int byteSize) { this.Domain = dom; this.Size = byteSize; }
        public Domain Domain { get; private set; }
        public int Size { get; private set; }
        public override int GetSize() { return Size; }
    }

    [Flags]
    public enum Domain
    {
        Raw =        0x0000,
        Boolean =    0x0001,
        Character =  0x0002,
        Integral =   0x0004,
        Real =       0x0008,
        Pointer =    0x0010,
        Offset =     0x0020,
        Segment =    0x0040,     // x86-style

        Function =   0x0080,
        Executable = 0x0100,
    }

    public class VoidType : DataType
    {
        public override int GetSize() { return 0; }
    }

    public class PointerType : DataType
    {
        public PointerType(DataType dt, int ptrSize)
        {
            this.Pointee = dt;
            this.Size = ptrSize;
        }

        public DataType Pointee { get; private set; }
        public int Size { get; private set; }
        public override int GetSize() { return Size; }
    }

    public class MemberPointerType : DataType
    {
        public MemberPointerType(DataType dtSeg, DataType dtPointee, int offsetSize)
        {
            this.SegmentType = dtSeg;
            this.Pointee = dtPointee;
            this.Size = offsetSize;
        }

        public DataType SegmentType { get; private set; }
        public DataType Pointee { get; private set; }
        public int Size { get; private set; }
        public override int GetSize() { return Size; }
    }

    public class SegmentedPointerType : DataType
    {
        public SegmentedPointerType(DataType dtSeg, DataType dtPointee, int offsetSize)
        {
            this.SegmentType = dtSeg;
            this.Pointee = dtPointee;
            this.Size = offsetSize;
        }

        public DataType SegmentType { get; private set; }
        public DataType Pointee { get; private set; }
        public int Size { get; private set; }
        public override int GetSize() { return Size; }
    }

    /// <summary>
    /// A compound type consists of subcomponents.
    /// </summary>
    public abstract class CompoundType : DataType
    {
    }

    public class ArrayType : CompoundType
    {
        public ArrayType(DataType dtElem, int length)
        {
            this.ElementType = dtElem;
            this.Length = length;
        }

        public override int GetSize()
        {
            return ElementType.GetSize() * Length;
        }

        public DataType ElementType { get; set; }
        public int Length { get; set; }
    }

    public class Field
    {
        public string Name;
        public int Offset;
        public DataType DataType;
    }

    public class StructureType : CompoundType
    {
        int? fixedSize;

        public StructureType()
        {
            Fields = new SortedList<int, Field>();
        }

        public StructureType(int size)
        {
            this.fixedSize = size;
            Fields = new SortedList<int, Field>();
        }

        public SortedList<int, Field> Fields { get; private set; }

        public override int GetSize()
        {
            if (fixedSize.HasValue)
                return fixedSize.Value;
            if (Fields.Count == 0)
                return 0;
            Field firstField = Fields.Values[0];
            int startOffset = firstField.Offset;
            if (startOffset > 0)
                startOffset = 0;
            Field lastField = Fields.Values[Fields.Count - 1];
            int rawSize = lastField.Offset + lastField.DataType.GetSize();
            return rawSize - startOffset;
        }
    }

    public class Alternative
    {
        public string Name;
        public DataType DataType;
    }

    public class UnionType : CompoundType
    {
        public UnionType()
        {
            Alternatives = new List<Alternative>();
        }

        public List<Alternative> Alternatives { get; private set; }

        public override int GetSize()
        {
            int maxSize = 0;
            int c = Alternatives.Count;
            for (int i = 0; i < c; ++i)
            {
                maxSize = Math.Max(
                    maxSize,
                    Alternatives[i].DataType.GetSize());
            }
            return maxSize;
        }
    }

    public class FuncType : DataType
    {
        public string Convention;
        public Parameter[] Paramters;
        public Parameter ReturnValue;
        public Dictionary<string, int> PostCallAdjustments;
        public override int GetSize()
        {
            return 0; throw new NotImplementedException();
        }
    }

    public class Parameter
    {
        public string Name;
        public Storage Storage;
        public DataType DataType;
    }

    public abstract class Storage
    {
        public abstract T Accept<T>(IStorageVisitor<T> visitor);
    }

    public class RegisterStorage : Storage
    {
        public override T Accept<T>(IStorageVisitor<T> visitor)
        {
            return visitor.VisitRegister(this);
        }
    }

    public class MemoryStorage : Storage
    {
        public int Offset; 

        public override T Accept<T>(IStorageVisitor<T> visitor)
        {
            return visitor.VisitMemory(this);
        }
    }

    public class FlagStorage: Storage
    {
        public override T Accept<T>(IStorageVisitor<T> visitor)
        {
            return visitor.VisitFlags(this);
        }
    }

    public class SequenceStorage : Storage
    {
        public override T Accept<T>(IStorageVisitor<T> visitor)
        {
            return visitor.VisitSequence(this);
        }
    }

    public interface IStorageVisitor<T>
    {
        T VisitRegister(RegisterStorage reg);
        T VisitMemory(MemoryStorage stack);
        T VisitFlags(FlagStorage flags);
        T VisitSequence(SequenceStorage flags);
    }
}
