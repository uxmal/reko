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
using System.Collections.Generic;
using System.Linq;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Serialization;
using Reko.Core.Types;

namespace Reko.ImageLoaders.MzExe.CodeView
{
    public class TypeBuilder
    {
        private static readonly Dictionary<int, SerializedType> reservedTypes;
        private static readonly Register_v1 regAl = new Register_v1("al");
        private static readonly Register_v1 regAx = new Register_v1("ax");
        private static readonly Register_v1 regDx = new Register_v1("dx");
        private static readonly SerializedSequence regDxAx = new SerializedSequence
        {
            Registers = new[] { regDx, regAx }
        };

        private readonly IProcessorArchitecture arch;
        private readonly Dictionary<int, TypeDefinition> dictionary;
        private readonly Dictionary<int, SerializedType> dataTypesByTypeIndex;
        
        public TypeBuilder(IProcessorArchitecture arch, Dictionary<int, TypeDefinition> dictionary)
        {
            this.arch = arch;
            this.dictionary = dictionary;
            this.dataTypesByTypeIndex = new Dictionary<int, SerializedType>();
            if (dictionary is not null)
            {
                BuildTypes();
            }
        }

        public static List<ImageSymbol> Build(
            IProcessorArchitecture arch,
            Dictionary<int, TypeDefinition> dictionary,
            List<CodeViewLoader.PublicSymbol> list)
        {
            var builder = new TypeBuilder(arch, dictionary);
            return list.Select(builder.BuildSymbol).ToList();
        }

        /// <summary>
        /// Build Reko types from CodeView types.
        /// </summary>
        /// <remarks>
        /// We make two passes through the CodeView types. First we locate 
        /// and build (empty) <see cref="StructureType"/>s from all
        /// CodeView structures. Then 
        /// </remarks>
        public ImageSymbol BuildSymbol(CodeViewLoader.PublicSymbol cvSymbol)
        {
            var imgSym = ImageSymbol.Location(arch, cvSymbol.addr);
            imgSym.Name = cvSymbol.name;
            if (!this.dataTypesByTypeIndex.TryGetValue(cvSymbol.typeidx, out var fnType))
                return imgSym;
            if (fnType is SerializedSignature ssig)
            {
                imgSym = ImageSymbol.Procedure(arch, cvSymbol.addr, cvSymbol.name, signature:ssig);
            }
            return imgSym;
        }

        private void BuildTypes()
        {
            // First build placeholders.
            foreach (var de in this.dictionary)
            {
                switch (de.Value.Leaves![0])
                {
                case Array a:
                    dataTypesByTypeIndex.Add(de.Key,
                        new ArrayType_v1());
                    break;
                case Pointer pt:
                    dataTypesByTypeIndex.Add(de.Key,
                        pt.Model == LeafType.FAR
                            ? new PointerType_v1 { PointerSize = 4 }
                            : (SerializedType) new MemberPointer_v1 { Size = 2 });
                    break;
                case Procedure p:
                    dataTypesByTypeIndex.Add(de.Key,
                        new SerializedSignature
                        {
                             Convention = CallingConvention(p.CallingConvention),
                             Arguments = new Argument_v1[p.ParameterCount]
                        });
                    break;
                case Structure str:
                    dataTypesByTypeIndex.Add(de.Key,
                        new StructType_v1 { Name = str.Name });
                    break;
                case LeafType lt:
                    switch (lt)
                    {
                    default:
                        throw new NotImplementedException($"Unimplemented leaf type {lt}.");
                    case LeafType.Nil:
                        continue;
                    }
                }
            }

            // Now resolve all the types.
            foreach (var de in this.dictionary)
            {
                switch (de.Value.Leaves![0])
                {
                case Array a:
                    var arr = (ArrayType_v1) dataTypesByTypeIndex[de.Key];
                    var elemType = TranslateType(a.ElementType);
                    arr.ElementType = elemType;
                    break;
                case Pointer pt:
                    if (dataTypesByTypeIndex[de.Key] is PointerType_v1 ptr)
                    {
                        ptr.DataType = TranslateType(pt.TypeIndex);
                    }
                    else if (dataTypesByTypeIndex[de.Key] is MemberPointer_v1 mptr)
                    {
                        mptr.MemberType = TranslateType(pt.TypeIndex);
                    }
                    break;
                case Procedure p:
                    var sig = (SerializedSignature) dataTypesByTypeIndex[de.Key];
                    BuildSignature(p, sig);
                    break;
                case LeafType lt:
                    switch (lt)
                    {
                    default:
                        throw new NotImplementedException($"Unimplemented leaf type {lt}.");
                    case LeafType.Nil:
                        continue;
                    }
                case Structure st:
                    var str = (StructType_v1) dataTypesByTypeIndex[de.Key];
                    BuildStructure(st, str);
                    break;
                }
            }
        }

        private void BuildSignature(Procedure p, SerializedSignature sig)
        {
            var retType = TranslateType(p.ReturnType);
            var byteSize = ByteSize(retType);
            var stg = ReturnTypeStorage(byteSize);
            sig.ReturnValue = new Argument_v1(
                "",
                retType,
                stg,
                false);
            sig.ReturnAddressOnStack = ReturnAddressSize(p.CallingConvention);
            sig.Arguments = TranslateArgs(p.ParameterCount, p.ParameterTypeList);
        }

        private SerializedStorage ReturnTypeStorage(int byteSize)
        {
            switch (byteSize)
            {
            case 0:
            case 1:
                return regAl;
            case 2:
                return regAx;
            case 3:
            case 4:
                return regDxAx;
            }
            throw new NotImplementedException();
        }

        private int ReturnAddressSize(LeafType callingConvention)
        {
            switch (callingConvention)
            {
            case LeafType.C_NEAR: return 2;
            case LeafType.C_FAR: return 4;
            }
            throw new NotImplementedException();
        }

        private void BuildStructure(Structure st, StructType_v1 str)
        {
            var fieldTypes =
                (object[]) dictionary[st.TypeList].Leaves![0];
            var fieldNamesOffsets = 
                (object[]) dictionary[st.NameOffsetList].Leaves![0];
            if (2 * fieldTypes.Length != fieldNamesOffsets.Length ||
                fieldTypes.Length != st.FieldCount)
                throw new FormatException();
            var fields = new StructField_v1[st.FieldCount];
            for (int i = 0; i < st.FieldCount; ++i)
            {
                var type = TranslateType(Convert.ToInt32(fieldTypes[i]));
                var name = (string) fieldNamesOffsets[i * 2];
                var offset = Convert.ToInt32(fieldNamesOffsets[i * 2 + 1]);
                fields[i] = new StructField_v1(offset, name, type);
            }
            str.Fields = fields;
            str.ByteSize = (st.BitSize + 7) / 8;
        }

        private int ByteSize(SerializedType type)
        {
            switch (type)
            {
            case PrimitiveType_v1 pt:
                return pt.ByteSize;
            case PointerType_v1 ptr:
                return ptr.PointerSize;
            case MemberPointer_v1 mptr:
                return mptr.Size;
            case ArrayType_v1 arr:
                return arr.Length * ByteSize(arr.ElementType!);
            }
            return 0;
        }

        private Argument_v1[] TranslateArgs(int parameterCount, int parameterTypeList)
        {
            if (parameterCount == 0)
                return System.Array.Empty<Argument_v1>();
            var parameterList = (object[]) dictionary[parameterTypeList].Leaves![0];
            if (parameterCount != parameterList.Length)
                throw new FormatException();
            var args = new Argument_v1[parameterCount];
            for (int i = 0; i < parameterCount; ++i)
            {
                args[i] = new Argument_v1(
                    null!,  // a name will be generated by signature deserialization.
                    TranslateType(Convert.ToInt32(parameterList[i])),
                    new StackVariable_v1(),
                    false);
            }
            return args;
        }

        private SerializedType TranslateType(int typeIndex)
        {
            if (dataTypesByTypeIndex.TryGetValue(typeIndex, out var type))
            {
                if (type is StructType_v1 str)
                    return new StructType_v1 { Name = str.Name };
                else
                    return type;
            }

            if (reservedTypes.TryGetValue(typeIndex, out type))
                return type;
            throw new NotImplementedException($"Unknown type index #{typeIndex:X}.");
        }

        private static string CallingConvention(LeafType lt)
        {
            switch (lt)
            {
            default: throw new NotImplementedException($"Unknown calling convention {lt}.");
            case LeafType.C_NEAR:
                return "__cdecl";
            }
        }

        static TypeBuilder()
        {
            reservedTypes = new Dictionary<int, SerializedType>
            {
                // Type indices 0-511 are reserved.Types 0-255 (high byte = 0) have meaning according
                // to the decoding of the following bits:
                //
                // xxxx xxxx x xx xxx xx
                // xxxx xxxx i md typ sz
                //
                // The format of Type Index(and Reserved Types) is illustrated in the next
                // four tables.
                //
                // Table 1.2, Format of i
                // i    Action
                // -----------
                // 0    Special type, not interpreted as follows
                //      (see "Special Types" below)
                // 1    Low 7 bits are interpreted as follows:

                // Table 1.3, Format of md

                // md Mode
                // -------
                // 00   Direct
                // 01   Near Pointer
                // 10   Far pointer
                // 11   Huge pointer

                // Table 1.4, Format of typ

                // typ  Basic type
                // ---------------
                // 000  Signed
                // 001  Unsigned
                // 010  Real
                // 011  Complex
                // 100  Boolean
                // 101  ASCII
                // 110  Currency
                // 111  Reserved

                // Table   1.5 Format of sz

                // sz   Size      (Real)   (Complex)  (Currency)
                // ---------------------------------------------
                // 00   8-bit     4-byte   8-byte Reserved
                // 01   16-bit    8-byte   16-byte    8-byte
                // 10   32-bit    10-byte  20-byte Reserved
                // 11   Reserved

                // Tables 1.6 and 1.7 list the predefined primitive types of the symbolic
                // debugging OMF.

                // Table   1.6 Special Types (8th bit = 0)

                // Name      Value  Description
                // ----------------------------
                // T_NOTYPE  0      Uncharacterized type(no type)
                // T_ABS     1      Absolute symbol

                // Table   1.7 Primitive Type Listing(8th bit = 1)

                // Name       Value     Description
                // --------------------------------
                // T_CHAR     80H       8-bit signed
                // T_SHORT    8lH       16-bit signed
                // T_LONG     82H       32-bit signed
                // T_UCHAR    84H       8-bit unsigned
                // T_USHORT   85H       16-bit unsigned
                // T_ULONG    86H       32-bit unsigned
                // T_REAL2    88H       32-bit real
                // T_REAL64   89H       64-bit real
                // T_REAL80   8AH(10)  80-bit real
                // T_CPLX64   8CH(12)  64-bit complex
                // T_CPLX128  8DH(13)  128-bit complex
                // T_CPLX160  8EH(14)  160-bit complex
                { 0x80, PrimitiveType_v1.Char8() },
                { 0x81, PrimitiveType_v1.Int16() },
                { 0x82, PrimitiveType_v1.Int32() },
                { 0x84, new PrimitiveType_v1(Domain.UnsignedInt, 1) },
                { 0x85, PrimitiveType_v1.UInt16() },
                { 0x86, PrimitiveType_v1.UInt32() },
                { 0x88, PrimitiveType_v1.Real32() },
                { 0x89, PrimitiveType_v1.Real64() },
                { 0x8A, PrimitiveType_v1.Real80() },

                // T_BOOL08     90H(16)   8-bit Boolean
                // T_BOOL16     91H(17)   16-bit Boolean
                // T_BOOL32     9H(18)    32-bit Boolean
                // T_ ASCII     94H(20)   8-bit character
                // T_ASCII16    95H(21)   16-bit characters
                // T_ASCII32    96H(22)   32-bit characters
                // T_BSTRING    97H(23)   Basic string type
                { 0x90, PrimitiveType_v1.Bool() },
                { 0x91, PrimitiveType_v1.Bool(2) },
                { 0x92, PrimitiveType_v1.Bool(4) },

                // T_PCHAR      A0H(32)   Near pointer to 8-bit signed
                // T_PSHORT     A1H(33)   Near pointer to 16-bit signed
                // T_PLONG      A2H(34)   Near pointer to 32-bit signed
                // T_PUCHAR     A4H(36)   Near pointer to 8-bit unsigned
                // T_PUSHORT    A5H(37)   Near pointer to 16-bit unsigned
                // T_PULONG     A6H(38)   Near pointer to 32-bit unsigned
                // T_PREAL32    A8H(40)   Near pointer to 32-bit real
                // T_PREAL64    A9H(41)   Near pointer to 64-bit real
                // T_PREAL80    AAH(42)   Near pointer to 80-bit real
                // T_PCPLX64    ACH(44)   Near pointer to 64-bit complex
                // T_PCPLX128   ADH(45)   Near pointer to 128-bit complex
                // T_ PCPLX160  AEH(46)   Near pointer to 160-bit complex
                { 0xA0, NearPtr(PrimitiveType_v1.Char8()) },
                { 0xA1, NearPtr(PrimitiveType_v1.Int16()) },
                { 0xA2, NearPtr(PrimitiveType_v1.Int32()) },
                { 0xA4, NearPtr(new PrimitiveType_v1 { Domain=Domain.UnsignedInt, ByteSize=1 }) },
                { 0xA5, NearPtr(PrimitiveType_v1.UInt16()) },
                { 0xA6, NearPtr(PrimitiveType_v1.UInt32()) },
                { 0xA8, NearPtr(PrimitiveType_v1.Real32()) },
                { 0xA9, NearPtr(PrimitiveType_v1.Real64()) },
                { 0xAA, NearPtr(PrimitiveType_v1.Real80()) },

                // T_PBOOL08    B0H(48)   Near pointer to 8-bit Boolean
                // T_PBOOL16    B1H(49)   Near pointer to 16-bit Boolean
                // T_PBOOL32    B2H(50)   Near pointer to 32-bit Boolean
                // T_PASCII     B4H(52)   Near pointer to 8-bit character
                // T_PASCII16   B5H(53)   Near pointer to 16-bit character
                // T_PASC1132   B6H(54)   Near pointer to 32-bit character
                // T_PBSTRING   B7H(55)   Near pointer to Basic string
                { 0xB0, NearPtr(PrimitiveType_v1.Bool()) },
                { 0xB1, NearPtr(PrimitiveType_v1.Bool(2)) },
                { 0xB2, NearPtr(PrimitiveType_v1.Bool(4)) },

                // T_PFCHAR     C0H(64)   Far pointer to 8-bit signed
                // T_PFSHORT    C1H(65)   Far pointer to 16-bit signed
                // T_PFLONG     C2H(66)   Far pointer to 32-bit signed
                // T_PFUCHAR    C4H(68)   Far pointer to 8-bit unsigned
                // T_PFUSHORT   C5H(69)   Far pointer to 16-bit unsigned
                // T_PFULONG    C6H(70)   Far pointer to 32-bit unsigned
                // T_PFREAL32   C8H(72)   Far pointer to 32-bit real
                // T_PFREAL64   C9H(73)    Far pointer to 64-bit real
                // T_PFREAL80   CAH(74)    Far pointer to 80-bit real
                // T_PFCPLX64   CCH(76)    Far pointer to 64-bit complex
                // T_PFCPLX128  CDH(77)    Far pointer to 128-bit complex
                // T_PFCPLX160  CEH(78)    Far pointer to 160-bit complex
                { 0xC0, FarPtr(PrimitiveType_v1.Char8()) },
                { 0xC1, FarPtr(PrimitiveType_v1.Int16()) },
                { 0xC2, FarPtr(PrimitiveType_v1.Int32()) },
                { 0xC4, FarPtr(new PrimitiveType_v1 { Domain=Domain.UnsignedInt, ByteSize=1 }) },
                { 0xC5, FarPtr(PrimitiveType_v1.UInt16()) },
                { 0xC6, FarPtr(PrimitiveType_v1.UInt32()) },
                { 0xC8, FarPtr(PrimitiveType_v1.Real32()) },
                { 0xC9, FarPtr(PrimitiveType_v1.Real64()) },
                { 0xCA, FarPtr(PrimitiveType_v1.Real80()) },

                // T_PFBOOL08   D0H(80)    Far pointer to 8-bit Boolean
                // T_PFBOOL16   D1H(81)    Far pointer to 16-bit Boolean
                // T_PFBOO132   D2H(82)    Far pointer to 32-bit Boolean
                // T_PFASCII    D4H(84)    Far pointer to 8-bit character
                // T_PFASCII16  D5H(85)    Far pointer to 16-bit character
                // T_PFASCII32  D6H(86)    Far pointer to 32-bit character
                // T_PFBSTRING  D7H(87)    Far pointer to Basic string
                { 0xD0, NearPtr(PrimitiveType_v1.Bool()) },
                { 0xD1, NearPtr(PrimitiveType_v1.Bool(2)) },
                { 0xD2, NearPtr(PrimitiveType_v1.Bool(4)) },

                // T_PHCHAR     E0H(96)    Huge pointer to 8-bit signed
                // T_PHSHORT    E1H(97)    Huge pointer to 16-bit signed
                // T_PHLONG     E2H(98)    Huge pointer to 32-bit signed
                // T_PHUCHAR    E4H(100)   Huge pointer to 8-bit unsigned
                // T_PHUSHORT   E5H(101)   Huge pointer to 16-bit unsigned
                // T_PHULONG    E6H(102)   Huge pointer to 32-bit unsigned
                // T_PHREAL32   E8H(104)   Huge pointer to 32-bit real
                // T_PHREAL64   E9H(105)   Huge pointer to 64-bit real
                // T_PHREAL80   EAH(106)   Huge pointer to 80-bit real
                // T_PHCPLX64   ECH(108)   Huge pointer to 64-bit complex
                // T_PHCPLX128  EDH(109)   Huge pointer to 128-bit complex
                // T_PHCPLX160  EEH(110)   Huge pointer to 160-bit complex

                // T_PHBOOL08   F0H(112)   Huge pointer to 8-bit Boolean
                // T_PHBOOL16   F1H(113)   Huge pointer to 16-bit Boolean
                // T_PHBOOL32   F2H(114)   Huge pointer to 32-bit Boolean
                // T_PHASCII    F4H(116)   Huge pointer to 8-bit character
                // T_PHASC1116  F5H(117)   Huge pointer to 16-bit character
                // T_PHASC1132  F6H(118)   Huge pointer to 32-bit character
                // T_PHBSTRING  F7H(119)   Huge pointer to Basic string
            };
        }

        private static MemberPointer_v1 NearPtr(SerializedType type)
        {
            return new MemberPointer_v1
            {
                 Size = 2,
                 MemberType = type,
            };
        }

        private static SerializedType FarPtr(SerializedType type)
        {
            return PointerType_v1.Create(type, 4);
        }
    }
}
