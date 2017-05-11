using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;
using DataType = Reko.Core.Types.DataType;
using Identifier = Reko.Core.Expressions.Identifier;

namespace Reko.ImageLoaders.LLVM
{
    public class TypeTranslator : LLVMTypeVisitor<DataType>
    {
        public DataType VisitArray(LLVMArrayType a)
        {
            throw new NotImplementedException();
        }

        public DataType VisitBaseType(LLVMBaseType b)
        {
            switch (b.Domain)
            {
            case Domain.Integral:
                return PrimitiveType.CreateWord(b.BitSize / 8);
            case Domain.Real:
                return PrimitiveType.Create(Core.Types.Domain.Real, b.BitSize / 8);
            case Domain.Void:
                return VoidType.Instance;
            }
            throw new NotImplementedException(string.Format("{0}", b));
        }

        public DataType VisitFunction(LLVMFunctionType fn)
        {
            return TranslateFn(fn.ReturnType, fn.Arguments);
        }

        private DataType TranslateFn(LLVMType retType, List<LLVMArgument> arguments, IStorageBinder)
        {
            var rt = retType.Accept(this);
            var sigRet = new Identifier("", rt, gen
            var sigParameters = new List<Identifier>();
            foreach (var param in parameters)
            {
                if (param.name == "...")
                {
                    throw new NotImplementedException("Varargs");
                }
                else
                {
                    var pt = param.Type.Accept();
                    var name = state.NextTemp();
                    var id = state.Procedure.Frame.CreateTemporary(name, pt);
                    sigParameters.Add(id);
                }
            }
            return new FunctionType(rt, sigParameters.ToArray());
        }

        public DataType VisitPointer(LLVMPointer p)
        {
            throw new NotImplementedException();
        }

        public DataType VisitStructure(StructureType s)
        {
            throw new NotImplementedException();
        }

        public DataType VisitTypeReference(TypeReference typeref)
        {
            throw new NotImplementedException();
        }
    }
}
