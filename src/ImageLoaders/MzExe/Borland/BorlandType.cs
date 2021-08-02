using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe.Borland
{
    public class BorlandType
    {
        public string? name;
    }

    public class SimpleType : BorlandType
    {
        public SerializedType? DataType;
    }

    public class ComplexType : BorlandType
    {
        public Func<SerializedType, SerializedType>? ConstructType;

        public ushort SubType { get; internal set; }
    }

    public class Callable : BorlandType
    {
        public bool IsNested { get; internal set; }
        public bool IsVararg { get; internal set; }
        public int Type { get; internal set; }
        public int ReturnType { get; internal set; }
    }

    public class StructUnionType : BorlandType
    {
        public int iMembers;
    }
}
