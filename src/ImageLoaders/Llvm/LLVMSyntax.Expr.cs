using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.LLVM
{
    public abstract class Binary : Value
    {
    }

    public abstract class BitwiseBinary : Value
    {

    }

    public class GetElementPtrExpr : Value
    {
        public bool Inbounds;
        public LLVMType BaseType;
        public LLVMType PointerType;
        public Value Pointer;
        public List<Tuple<LLVMType, Value>> Indices;

        public override void Write(Formatter w)
        {
            w.WriteKeyword("getelementptr");
            if (Inbounds)
            {
                w.Write(' ');
                w.WriteKeyword("inbounds");
            }
            w.Write(" (");
            BaseType.Write(w);
            w.Write(", ");
            PointerType.Write(w);
            w.Write(' ');
            Pointer.Write(w);
            foreach (var index in Indices)
            {
                w.Write(", ");
                index.Item1.Write(w);
                w.Write(' ');
                index.Item2.Write(w);
            }
            w.Write(")");
        }
    }
}
