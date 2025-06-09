#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Output;
using System.Collections.Generic;

namespace Reko.ImageLoaders.LLVM
{
    public abstract class LLVMType : LLVMSyntax
    {
        public abstract T Accept<T>(LLVMTypeVisitor<T> visitor);

        private static Dictionary<int, LLVMBaseType> intTypes = new Dictionary<int, LLVMBaseType>
        {
        };

        public static LLVMType Void { get; internal set; }
        public static LLVMType Double { get; internal set; }
        public static LLVMType X86_fp80 { get; internal set; }

        public static LLVMType GetBaseType(string sType)
        {
            if (sType[0] == 'i')
            {
                int bitsize = int.Parse(sType.Substring(1));
                if (!intTypes.TryGetValue(bitsize, out LLVMBaseType? i))
                {
                    i = new LLVMBaseType(sType, Domain.Integral, bitsize);
                    intTypes.Add(bitsize, i);
                }
                return i;
            }
            throw new NotImplementedException();
        }
       
        static LLVMType()
        {
            Void = new LLVMBaseType("void", Domain.Void, 0);
            Double = new LLVMBaseType("double", Domain.Real, 64);
            X86_fp80 = new LLVMBaseType("x86_fp80", Domain.Real, 80);
        }
    }

    public enum Domain
    {
        Void,
        Integral,
        Real,
    }

    public class LLVMBaseType : LLVMType
    {
        public readonly string Name;
        public readonly Domain Domain;
        public readonly int BitSize;

        public LLVMBaseType(string name, Domain dom, int bitSize)
        {
            this.Name = name;
            this.Domain = dom;
            this.BitSize = bitSize;
        }

        public override T Accept<T>(LLVMTypeVisitor<T> visitor)
        {
            return visitor.VisitBaseType(this);
        }

        public override void Write(Formatter w)
        {
            w.Write(Name);
        }
    }

    public class TypeReference : LLVMType
    {
        public LocalId TypeName;

        public TypeReference(LocalId name)
        {
            this.TypeName = name;
        }

        public override T Accept<T>(LLVMTypeVisitor<T> visitor)
        {
            return visitor.VisitTypeReference(this);
        }

        public override void Write(Formatter w)
        {
            TypeName.Write(w);
        }
    }

    public class LLVMPointer : LLVMType
    {
        public LLVMType Pointee;

        public LLVMPointer(LLVMType pointee)
        {
            this.Pointee = pointee;
        }

        public override T Accept<T>(LLVMTypeVisitor<T> visitor)
        {
            return visitor.VisitPointer(this);
        }

        public override void Write(Formatter w)
        {
            Pointee.Write(w);
            w.Write("*");
        }
    }

    public class LLVMArrayType : LLVMType
    {
        public readonly int Length;
        public readonly LLVMType ElementType;

        public LLVMArrayType(int length, LLVMType elType)
        {
            this.Length = length;
            this.ElementType = elType;
        }

        public override T Accept<T>(LLVMTypeVisitor<T> visitor)
        {
            return visitor.VisitArray(this);
        }
    
        public override void Write(Formatter w)
        {
            w.Write("[{0} x ", Length);
            ElementType.Write(w);
            w.Write("]");
        }

    }

    public class StructureType : LLVMType
    {
        public List<LLVMType>? Fields;

        public override T Accept<T>(LLVMTypeVisitor<T> visitor)
        {
            return visitor.VisitStructure(this);
        }

        public override void Write(Formatter w)
        {
            w.Write("{");
            if (Fields!.Count > 0)
            {
                var sep = " ";
                foreach (var field in Fields)
                {
                    w.Write(sep);
                    sep = ", ";
                    field.Write(w);
                }
                w.Write(" ");
            }
            w.Write("}");
        }
    }

    public class LLVMFunctionType : LLVMType
    {
        public string? Convention;
        public LLVMType? ReturnType;
        public ParameterAttributes? ret_attrs;
        public List<LLVMParameter>? Parameters;

        public override T Accept<T>(LLVMTypeVisitor<T> visitor)
        {
            return visitor.VisitFunction(this);
        }

        public override void Write(Formatter w)
        {
            ReturnType!.Write(w);
            w.Write(" (");
            var sep = "";
            foreach (var arg in Parameters!)
            {
                w.Write(sep);
                sep = ", ";
                if (arg.Type is not null)
                {
                    arg.Type.Write(w);
                    if (arg.name is not null)
                    {
                        w.Write(" ");
                        w.Write(arg.name);
                    }
                }
                else if (arg.name is not null)
                {
                    w.Write(arg.name);
                }
            }
            w.Write(")");
        }
    }

    public class LLVMParameter : LLVMSyntax
    {
        public LLVMType? Type; // [parameter Attrs]
        public string? name;
        public ParameterAttributes? attrs;

        public override void Write(Formatter w)
        {
            Type!.Write(w);
            if (string.IsNullOrEmpty(name))
                return;
            w.Write(' ');
            w.Write(name);
        }
    }

    public class ParameterAttributes
    {
        public bool signext;
        public bool zeroext;
        public bool noalias;

        public void Write(Formatter w)
        {
            var sep = "";
            if (signext)
            {
                w.WriteKeyword("signext");
                sep = " ";
            }
            else if (zeroext)
            {
                w.WriteKeyword("zeroext");
                sep = " ";
            }
            if (noalias)
            {
                w.Write(sep);
                w.WriteKeyword("noalias");
                sep = " ";
            }
        }
    }

    public interface LLVMTypeVisitor<T>
    {
        T VisitArray(LLVMArrayType a);
        T VisitBaseType(LLVMBaseType b);
        T VisitFunction(LLVMFunctionType fn);
        T VisitPointer(LLVMPointer p);
        T VisitStructure(StructureType s);
        T VisitTypeReference(TypeReference typeref);
    }
}