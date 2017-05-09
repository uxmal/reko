using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Output;

namespace Reko.ImageLoaders.LLVM
{
    public class LLVMCall : Instruction
    {
        public string Result;
        public List<Tuple<LLVMType,Value>> Arguments;
        public LLVMType FnType;
        public Value FnPtr;

        public override void Write(Formatter w)
        {
            if (Result != null)
            {
                w.Write("%{0}", Result);
                w.Write(" = ");
            }
            w.WriteKeyword("call");
            w.Write(" ");
            FnType.Write(w);
            w.Write(" ");
            FnPtr.Write(w);
            w.Write("(");
            var sep = "";
            foreach (var arg in Arguments)
            {
                w.Write(sep);
                sep = ", ";
                arg.Item1.Write(w);
                w.Write(" ");
                arg.Item2.Write(w);
            }
            w.Write(")");
        }
    }

    public class GetElementPtr : MemoryInstruction
    {
        public string Result;
        public List<Tuple<LLVMType, Value>> Indices;

        public LLVMType BaseType { get; internal set; }
        public LLVMType PtrType { get; internal set; }
        public Value PtrValue { get; internal set; }

        public override void Write(Formatter w)
        {
            w.Write("%{0}", Result);
            w.Write(" = ");
            w.WriteKeyword("getelementptr");
            w.Write(" ");
            BaseType.Write(w);
            w.Write(", ");
            PtrType.Write(w);
            w.Write(" ");
            PtrValue.Write(w);
            foreach (var index in Indices)
            {
                w.Write(", ");
                index.Item1.Write(w);
                w.Write(" ");
                index.Item2.Write(w);
            }
        }
    }
}
