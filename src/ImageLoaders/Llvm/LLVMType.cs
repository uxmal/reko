#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
        private static Dictionary<int, LLVMBaseType> intTypes = new Dictionary<int, LLVMBaseType>
        {
        };

        public static LLVMType Void { get; internal set; }

        public static LLVMType GetBaseType(string sType)
        {
            if (sType[0] == 'i')
            {
                int bitsize = int.Parse(sType.Substring(1));
                LLVMBaseType i;
                if (!intTypes.TryGetValue(bitsize, out i))
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

        public override void Write(Formatter w)
        {
            w.Write("[{0} x ", Length);
            ElementType.Write(w);
            w.Write("]");
        }
    }

    public class StructureType : LLVMType
    {
        public List<LLVMType> Fields;

        public override void Write(Formatter w)
        {
            w.Write("{ ");
            var sep = "";
            foreach (var field in Fields)
            {
                w.Write(sep);
                sep = ", ";
                field.Write(w);
            }
            w.Write(" }");
        }
    }

    public class LLVMFunctionType : LLVMType
    {
        public string Convention;
        public LLVMType ReturnType;
        public List<LLVMArgument> Arguments;

        public override void Write(Formatter w)
        {
            ReturnType.Write(w);
            w.Write(" (");
            var sep = "";
            foreach (var arg in Arguments)
            {
                w.Write(sep);
                sep = ", ";
                if (arg.Type != null)
                {
                    arg.Type.Write(w);
                    if (arg.name != null)
                    {
                        w.Write(" ");
                        w.Write(arg.name);
                    }
                }
                else if (arg.name != null)
                {
                    w.Write(arg.name);
                }
            }
            w.Write(")");
        }
    }

    public class LLVMArgument : LLVMSyntax
    {
        public LLVMType Type; // [parameter Attrs]
        public string name;

        public override void Write(Formatter w)
        {
            Type.Write(w);
            if (string.IsNullOrEmpty(name))
                return;
            w.Write(' ');
            w.Write(name);
        }
    }
}