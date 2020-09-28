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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.Types;
using DataType = Reko.Core.Types.DataType;
using Identifier = Reko.Core.Expressions.Identifier;
using Reko.Core;

namespace Reko.ImageLoaders.LLVM
{
    /// <summary>
    /// Translatew LLVM data types to Reko data types.
    /// </summary>
    public class TypeTranslator : LLVMTypeVisitor<DataType>
    {
        private readonly int ptrBitSize;

        public TypeTranslator(int pointerBitSize)
        {
            this.ptrBitSize = pointerBitSize;
        }

        public DataType VisitArray(LLVMArrayType a)
        {
            var elemType = a.ElementType.Accept(this);
            return new ArrayType(elemType, a.Length);
        }

        public DataType VisitBaseType(LLVMBaseType b)
        {
            switch (b.Domain)
            {
            case Domain.Integral:
                if (b.BitSize == 1)
                    return PrimitiveType.Bool;
                else 
                    return PrimitiveType.CreateWord(b.BitSize);
            case Domain.Real:
                return PrimitiveType.Create(Core.Types.Domain.Real, b.BitSize);
            case Domain.Void:
                return VoidType.Instance;
            }
            throw new NotImplementedException(string.Format("{0}", b));
        }

        public DataType VisitFunction(LLVMFunctionType fn)
        {
            return TranslateFn(fn.ReturnType, fn.Parameters, new StorageBinder());
        }

        private DataType TranslateFn(LLVMType retType, List<LLVMParameter> parameters, IStorageBinder binder)
        {
            var rt = retType.Accept(this);
            var sigRet = binder.CreateTemporary("", rt);
            var sigParameters = new List<Identifier>();
            foreach (var param in parameters)
            {
                if (param.name == "...")
                {
                    var dt = new UnknownType();
                    var id = binder.CreateTemporary("...", dt);
                    sigParameters.Add(id);
                }
                else
                {
                    var pt = param.Type.Accept(this);
                    var id = binder.CreateTemporary(pt);
                    sigParameters.Add(id);
                }
            }
            return new FunctionType(sigRet, sigParameters.ToArray());
        }

        public DataType VisitPointer(LLVMPointer p)
        {
            var pointee = p.Pointee.Accept(this);
            return new Pointer(pointee, ptrBitSize);
        }

        public DataType VisitStructure(StructureType s)
        {
            var str = new Reko.Core.Types.StructureType(null, 0, true);
            int offset = 0;
            foreach (var field in s.Fields)
            {
                var ft = field.Accept(this);
                str.Fields.Add(offset, ft);
                offset += ft.Size;
            }
            return str;
        }

        public DataType VisitTypeReference(TypeReference typeref)
        {
            var tv = new TypeVariable(typeref.TypeName.Name, 0);
            return tv;
        }
    }
}
