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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Symbol table for the C parser.
    /// </summary>
    public class SymbolTable 
    {
        private readonly IPlatform platform;
        private readonly int pointerSize;

        /// <summary>
        /// Creates a symbol table.
        /// </summary>
        /// <param name="platform"><see cref="IPlatform"/> instance to use.
        /// </param>
        public SymbolTable(IPlatform platform) : this(
            platform,
            new Dictionary<string, PrimitiveType_v1>(),
            new Dictionary<string, SerializedType>())
        {
        }

        /// <summary>
        /// Creates a symbol table.
        /// </summary>
        /// <param name="platform"><see cref="IPlatform"/> instance to use.
        /// </param>
        /// <param name="pointerSize">The size of a pointer measured in storage units.
        /// </param>
        public SymbolTable(IPlatform platform, int pointerSize) : this(
            platform,
            new Dictionary<string, PrimitiveType_v1>(),
            new Dictionary<string, SerializedType>())
        {
            this.pointerSize = pointerSize;
        }

        /// <summary>
        /// Creates a symbol, filling it with previously known types.
        /// </summary>
        /// <param name="platform"><see cref="IPlatform"/> instance to use.</param>
        /// <param name="primitiveTypes">C primitive types.</param>
        /// <param name="namedTypes">Known type names.</param>
        public SymbolTable(
            IPlatform platform,
            Dictionary<string, PrimitiveType_v1> primitiveTypes,
            Dictionary<string, SerializedType> namedTypes)
        {
            this.platform = platform;
            this.pointerSize = platform.PointerType.Size;

            this.Types = new List<SerializedType>();
            this.StructsSeen = new Dictionary<string, StructType_v1>();
            this.UnionsSeen = new Dictionary<string, UnionType_v1>();
            this.EnumsSeen = new Dictionary<string, SerializedEnumType>();
            this.Constants = new Dictionary<string, int>();
            this.Procedures = new List<ProcedureBase_v1>();
            this.Variables = new List<GlobalDataItem_v2>();
            this.Annotations = new List<Annotation_v3>();
            this.Segments = new List<MemorySegment_v1>();
            this.PrimitiveTypes = primitiveTypes;
            this.NamedTypes = namedTypes;
            this.Sizer = new TypeSizer(platform, this.NamedTypes);
        }

        /// <summary>
        /// The list of types in the symbol table.
        /// </summary>
        public List<SerializedType> Types { get; }

        /// <summary>
        /// Tagged structs seen so far.
        /// </summary>
        public Dictionary<string, StructType_v1> StructsSeen { get; }

        /// <summary>
        /// Tagged unions seen so far.
        /// </summary>
        public Dictionary<string, UnionType_v1> UnionsSeen { get; }

        /// <summary>
        /// Enums seen so far.
        /// </summary>
        public Dictionary<string, SerializedEnumType> EnumsSeen { get; }

        /// <summary>
        /// Constants.
        /// </summary>
        public Dictionary<string, int> Constants { get; }

        /// <summary>
        /// Named types seen.
        /// </summary>
        public Dictionary<string, SerializedType> NamedTypes { get; }

        /// <summary>
        /// C primitive types.
        /// </summary>
        public Dictionary<string, PrimitiveType_v1> PrimitiveTypes { get; }

        /// <summary>
        /// The procedures.
        /// </summary>
        public List<ProcedureBase_v1> Procedures { get; }

        /// <summary>
        /// The global variables from [[reko::global]] attributes.
        /// </summary>
        public List<GlobalDataItem_v2> Variables { get; }

        /// <summary>
        /// Annotations (from [[reko::annotation]] attributes).
        /// </summary>
        public List<Annotation_v3> Annotations { get; }

        /// <summary>
        /// Segments (from [[reko::segment]] attributes).
        /// </summary>
        public List<MemorySegment_v1> Segments { get; }

        /// <summary>
        /// Auxiliary class that computes the size of C data types.
        /// </summary>
        public TypeSizer Sizer { get; }

        /// <summary>
        /// Given a C declaration, adds it to the symbol table 
        /// as a function or a type declaration.
        /// </summary>
        /// <param name="decl"></param>
        public List<SerializedType> AddDeclaration(Decl decl)
        {
            ProcessDeclarationAttributes(decl.attribute_list);

            var types = new List<SerializedType>();
            if (decl is FunctionDecl)
            {
                return types;
            }
            IEnumerable<DeclSpec> declspecs = decl.decl_specs;
            var isTypedef = false;
            if (decl.decl_specs[0] is StorageClassSpec scspec &&
                scspec.Type == CTokenType.Typedef)
            {
                declspecs = decl.decl_specs.Skip(1);
                isTypedef = true;
            }

            var ntde = new NamedDataTypeExtractor(platform, declspecs, this, pointerSize);
            foreach (var declarator in decl.init_declarator_list)
            {
                var nt = ntde.GetNameAndType(declarator.Declarator);
                var serType = nt.DataType!;

                if (nt.DataType is SerializedSignature sSig)
                {
                    sSig.Convention ??= GetCallingConventionFromAttributes(decl.attribute_list);
                    if (sSig.ReturnValue is not null)
                    {
                        var (kind, _) = ntde.GetArgumentKindFromAttributes(
                            "returns", decl.attribute_list);
                        sSig.ReturnValue.Kind = kind;
                    }
                    var sProc = MakeProcedure(nt.Name!, sSig, decl.attribute_list);
                    Procedures.Add(sProc);
                    types.Add(sSig);
                }
                else if (!isTypedef)
                {
                    GlobalDataItem_v2 variable = MakeGlobalVariable(nt, serType, decl.attribute_list);
                    Variables.Add(variable);
                    types.Add(serType);
                }
                if (isTypedef)
                {
                    //$REVIEW: should make sure that if the typedef already exists, 
                    // then the types match but a real compiler would have validated that.
                    var typedef = new SerializedTypedef
                    {
                        Name = nt.Name,
                        DataType = serType
                    };
                    Types.Add(typedef);
                    //$REVIEW: do we really need to check for consistence?
                    NamedTypes[typedef.Name!] = serType;
                    types.Add(serType);
                }
            }
            return types;
        }

        /// <summary>
        /// Process the any reko-specific attributes before a declaration.
        /// </summary>
        /// <param name="attributes">The attributes to process.</param>
        /// <exception cref="FormatException"></exception>
        public void ProcessDeclarationAttributes(List<CAttribute>? attributes)
        {
            if (attributes is null)
                return;
            foreach (var a in attributes)
            {
                if (a.Name.Components.Length != 2 ||
                    a.Name.Components[0] != "reko")
                    continue;

                var tokenStream = new TokenStream(a.Tokens);
                switch (a.Name.Components[1])
                {
                case "annotation":
                    var ann = ProcessAnnotationAttribute(tokenStream);
                    if (ann is null)
                        throw new FormatException(
                            "[[reko::annotation]] attribute is malformed. " +
                            "Expected an address and an annotation text separated by commas.");
                    this.Annotations.Add(ann);
                    break;
                case "segment":
                    var seg = ProcessSegmentAttribute(tokenStream);
                    if (seg is null)
                        throw new FormatException(
                            "[[reko::segment]] attribute is malformed. " +
                            "Expected an address, a segment name, a segment size, and access mode.");
                    this.Segments.Add(seg);
                    break;
                }
            }
        }

        private MemorySegment_v1? ProcessSegmentAttribute(TokenStream tokenStream)
        {
            if (!tokenStream.ExpectStringLiteral(out var sAddress))
                return null;
            if (!tokenStream.Expect(CTokenType.Comma))
                return null;
            if (!tokenStream.ExpectStringLiteral(out var name))
                return null;
            if (!tokenStream.Expect(CTokenType.Comma))
                return null;
            if (!tokenStream.ExpectNumericLiteral(out var size))
                return null;
            if (!tokenStream.Expect(CTokenType.Comma))
                return null;
            if (!tokenStream.ExpectStringLiteral(out var attrs))
                return null;
            string? description = null;
            if (tokenStream.PeekAndDiscard(CTokenType.Comma))
            {
                if (!tokenStream.ExpectStringLiteral(out description))
                    return null;
            }
            return new MemorySegment_v1
            {
                Address = sAddress,
                Name = name,
                Size = $"0x{size:X}",
                Attributes = attrs,
                Description = description
            };
        }

        private Annotation_v3? ProcessAnnotationAttribute(TokenStream tokenStream)
        {
            if (!tokenStream.ExpectStringLiteral(out var sAddress))
                return null;
            if (!tokenStream.Expect(CTokenType.Comma))
                return null;
            if (!tokenStream.ExpectStringLiteral(out var text))
                return null;
            var annotation = new Annotation_v3
            {
                Address = sAddress,
                Text = text,
            };
            return annotation;
        }

        private string? GetCallingConventionFromAttributes(List<CAttribute>? attributes)
        {
            var attrConvention = attributes?.Find(a =>
                a.Name.Components.Length == 2 &&
                a.Name.Components[0] == "reko" &&
                a.Name.Components[1] == "convention");
            if (attrConvention?.Tokens is null)
                return null;
            if (attrConvention.Tokens.Count == 1 && 
                attrConvention.Tokens[0].Type == CTokenType.Id)
            {
                return (string?) attrConvention.Tokens[0].Value;
            }
            throw new CParserException("Incorrect syntax for [[reko::convention]].");
        }

        private ProcedureBase_v1 MakeProcedure(string name, SerializedSignature sSig, List<CAttribute>? attributes)
        {
            var attrChar = attributes?.Find(a =>
                a.Name.Components.Length == 2 &&
                a.Name.Components[0] == "reko" &&
                a.Name.Components[1] == "characteristics");
            var c = MakeCharacteristics(attrChar);
            var attrService = attributes?.Find(a =>
                a.Name.Components.Length == 2 && 
                a.Name.Components[0] == "reko" &&
                a.Name.Components[1] == "service");
            var noreturn = attributes?.Find(a =>
                a.Name.Components.Length == 1 &&
                a.Name.Components[0] == "noreturn")
                is not null;
            if (noreturn)
            {
                if (c is null)
                    c = new ProcedureCharacteristics();
                c.Terminates = true;
            }
            if (attrService is not null)
            {
                var sService = MakeService(name, sSig, attrService);
                sService.Characteristics = c;
                return sService;
            }
            else
            {
                var addr = FindRekoAddressAttribute(attributes);
                return new Procedure_v1
                {
                    Name = name,
                    Signature = sSig,
                    Address = addr,
                    Characteristics = c,
                };
            }
        }

        private static GlobalDataItem_v2 MakeGlobalVariable(NamedDataType nt, SerializedType serType, List<CAttribute>? attributes)
        {
            var sAddr = FindRekoAddressAttribute(attributes);
            return new GlobalDataItem_v2
            {
                Name = nt.Name,
                DataType = serType,
                Address = sAddr
            };
        }

        private static string? FindRekoAddressAttribute(List<CAttribute>? attributes)
        {
            string? addr = null;
            var attrAddress = attributes?.Find(a =>
                a.Name.Components.Length == 2 &&
                a.Name.Components[0] == "reko" &&
                a.Name.Components[1] == "address");
            if (attrAddress is not null && attrAddress.Tokens is { })
            {
                if (attrAddress.Tokens.Count != 1 ||
                    attrAddress.Tokens[0].Type != CTokenType.StringLiteral)
                    throw new FormatException("[[reko::address]] attribute is malformed. Expected a string constant.");
                addr = (string?) attrAddress.Tokens[0].Value;
            }
            return addr;
        }

        private SerializedService MakeService(string name, SerializedSignature sSig, CAttribute attrService)
        {
            var sap = new ServiceAttributeParser(attrService);
            var syscall = sap.Parse();
            return new SerializedService
            {
                Signature = sSig,
                Name = name,
                SyscallInfo = syscall,
            };
        }

        private ProcedureCharacteristics? MakeCharacteristics(CAttribute? attrCharacteristics)
        {
            if (attrCharacteristics is null)
                return null;
            var cp = new CharacteristicsParser(attrCharacteristics);
            return cp.Parse();
        }

        private class TokenStream
        {
            private readonly CToken[] tokens;
            private int iNext;

            public TokenStream(List<CToken>? tokens)
            {
                this.tokens = tokens?.ToArray() ?? Array.Empty<CToken>();
                this.iNext = 0;
            }

            public TokenStream(CToken[]? tokens)
            {
                this.tokens = tokens ?? Array.Empty<CToken>();
                this.iNext = 0;
            }

            internal bool ExpectStringLiteral([MaybeNullWhen(false)] out string sValue)
            {
                if (iNext < tokens.Length && tokens[iNext].Type == CTokenType.StringLiteral)
                {
                    sValue = (string) tokens[iNext].Value!;
                    ++iNext;
                    return true;
                }
                sValue = null;
                return false;
            }

            public bool Expect(CTokenType type)
            {
                if (iNext < tokens.Length && tokens[iNext].Type == type)
                {
                    ++iNext;
                    return true;
                }
                return false;
            }

            public bool ExpectNumericLiteral(out long size)
            {
                if (iNext < tokens.Length && tokens[iNext].Type == CTokenType.NumericLiteral)
                {
                    size = Convert.ToInt64(tokens[iNext].Value);
                    ++iNext;
                    return true;
                }
                size = 0;
                return false;
            }

            public bool PeekAndDiscard(CTokenType type)
            {
                if (iNext < tokens.Length && tokens[iNext].Type == type)
                {
                    ++iNext;
                    return true;
                }
                return false;
            }
        }
    }
}
