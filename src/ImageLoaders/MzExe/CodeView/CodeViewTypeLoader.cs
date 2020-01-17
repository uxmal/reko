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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.MzExe.CodeView
{
    public class CodeViewTypeLoader
    {
        private byte[] typeSection;
        private LeImageReader rdr;

        public CodeViewTypeLoader(byte[] typeSection)
        {
            this.typeSection = typeSection;
            this.rdr = new LeImageReader(typeSection);
        }

        public Dictionary<int, TypeDefinition> Load()
        {
            var dict = new Dictionary<int, TypeDefinition>();
            int index = 0x200;  // Type indices start at 0x200 according to Microsoft docs
            while (rdr.IsValid)
            {
                var def = ReadTypeDefinition();
                dict.Add(index, def);
                ++index;
            }
            return dict;
        }

        public static object ReadNumericLeaf(LeImageReader rdr)
        {
            var b = rdr.PeekByte(0);
            if (0 <= b && b <= 0x7F)
                return rdr.ReadByte();
            else
                return ReadLeaf(rdr);
        }

        public static object ReadLeaf(LeImageReader rdr)
        {
            var b = rdr.ReadByte();
            var lt = (LeafType) b;
            switch (lt)
            {
            default:
                throw new NotImplementedException($"CodeView leaf type {lt} {(int)lt:X2} not implemented yet.");
            case LeafType.Int8:
                return rdr.ReadSByte();
            case LeafType.UInt16:
                return rdr.ReadLeUInt16();
            case LeafType.ARRAY:
                return ReadArray(rdr);
            case LeafType.INDEX:    // Type index
                return rdr.ReadLeUInt16();
            case LeafType.LABEL:
                return ReadLabel(rdr);
            case LeafType.LIST:
                return ReadList(rdr);
            case LeafType.POINTER:
                return ReadPointer(rdr);
            case LeafType.PROCEDURE:
                return ReadProcedure(rdr);
            case LeafType.STRING:
                return ReadString(rdr);
            case LeafType.STRUCTURE:
                return ReadStructure(rdr);
            case LeafType.Nil:
            case LeafType.C_FAR:
            case LeafType.C_NEAR:
            case LeafType.FAR:
            case LeafType.NEAR:
            case LeafType.UNPACKED:
            case (LeafType) 0x81:   //$REVIEW: void?
                return lt;
            }
        }

        private static Array ReadArray(LeImageReader rdr)
        {
            var length = Convert.ToInt32(ReadNumericLeaf(rdr));
            var elemType = (ushort) ReadNumericLeaf(rdr);
            var idxType = rdr.IsValid
                ? (ushort) ReadNumericLeaf(rdr)
                : ~0;        //$TODO: should be 'integer'
            var name = rdr.IsValid
                ? (string) ReadLeaf(rdr)
                : null;
            return new Array
            {
                BitSize = length,
                ElementType = elemType,
                IndexType = idxType,
                Name = name,
            };
        }

        private static Label ReadLabel(LeImageReader rdr)
        {
            var nil = (LeafType)ReadLeaf(rdr);
            if (nil != LeafType.Nil)
                throw new FormatException();
            var labelType = (LeafType)ReadLeaf(rdr);
            return new Label
            {
                Type = labelType
            };
        }

        private static object[] ReadList(LeImageReader rdr)
        {
            var list = new List<object>();
            while (rdr.IsValid)
            {
                list.Add(ReadLeaf(rdr));
            }
            return list.ToArray();
        }

        private static Pointer ReadPointer(LeImageReader rdr)
        {
            var model = (LeafType)ReadLeaf(rdr);
            var type = Convert.ToInt32(ReadNumericLeaf(rdr));
            var name = rdr.IsValid
                ? (string) ReadLeaf(rdr)
                : null;
            return new Pointer
            {
                Model = model,
                TypeIndex = type,
                Name = name,
            };
        }

        private static Procedure ReadProcedure(LeImageReader rdr)
        {
            var nil = (LeafType)ReadLeaf(rdr);
            if (nil != LeafType.Nil)
                throw new FormatException();
            var rvType = Convert.ToInt32(ReadNumericLeaf(rdr));
            var cc = (LeafType) ReadLeaf(rdr);
            var nParams = Convert.ToInt32(ReadNumericLeaf(rdr));
            var paramTypeList = Convert.ToInt32(ReadNumericLeaf(rdr));
            return new Procedure
            {
                CallingConvention = cc,
                ReturnType = rvType,
                ParameterCount = nParams,
                ParameterTypeList = paramTypeList,
            };
        }

        private static string ReadString(LeImageReader rdr)
        {
            var len = rdr.ReadByte();
            return Encoding.ASCII.GetString(rdr.ReadBytes(len));
        }

        private static Structure ReadStructure(LeImageReader rdr)
        {
            int cbits = Convert.ToInt32(ReadNumericLeaf(rdr));
            int fields = Convert.ToInt32(ReadNumericLeaf(rdr));
            var typeList = (ushort) ReadNumericLeaf(rdr);
            var nameOffsetList = (ushort) ReadNumericLeaf(rdr);
            var name = rdr.IsValid
                    ? (string)ReadLeaf(rdr)
                    : null;

            var packed = rdr.IsValid
                    ? (LeafType) ReadLeaf(rdr)
                    : LeafType.Nil;
            return new Structure
            {
                BitSize = cbits,
                FieldCount = fields,
                TypeList = typeList,
                NameOffsetList = nameOffsetList,
                Name = name
            };
        }

        public TypeDefinition ReadTypeDefinition()
        {
            var linkage = rdr.ReadByte();
            var length = rdr.ReadLeUInt16();
            var rdrLeaves = new LeImageReader(rdr.ReadBytes(length));
            var leaves = new List<object>();
            while (rdrLeaves.IsValid)
            {
                leaves.Add(ReadLeaf(rdrLeaves));
            }
            return new TypeDefinition
            {
                linkage = linkage,
                Leaves = leaves.ToArray()
            };
        }
    }

    public class TypeDefinition
    {
        public int linkage;
        public object[] Leaves;
    }

    public class Array
    {
        public int BitSize;
        public ushort ElementType;
        public int IndexType;
        public string Name;

        public override string ToString()
        {
            return $"Array {Name} BitSize: {BitSize} Elem:{ElementType} Idx:{IndexType}";
        }
    }

    public class Label
    {
        public LeafType Type;

        public override string ToString()
        {
            return $"Label {Type}";
        }
    }

    public class Pointer
    {
        public LeafType Model;
        public int TypeIndex;
        public string Name;

        public override string ToString()
        {
            return $"Pointer {Name} {Model} type: {TypeIndex:X4}";
        }
    }

    public class Procedure
    {
        public LeafType CallingConvention;
        public int ReturnType;
        public int ParameterCount;
        public int ParameterTypeList;

        public override string ToString()
        {
            return $"Procedure {CallingConvention} return:{ReturnType} ({ParameterCount}: #{ParameterTypeList:X4})";
        }
    }

    public class Structure
    {
        public int BitSize;
        public int FieldCount;
        public int TypeList;
        public int NameOffsetList;
        public string Name;

        public override string ToString()
        {
            return $"Structure {Name}: bitsize {BitSize} fields: {FieldCount} types: {TypeList:X4} names: {NameOffsetList:X4}";
        }
    }

}
