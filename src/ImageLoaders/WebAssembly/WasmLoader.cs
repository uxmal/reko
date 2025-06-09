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

using Reko.Core;
using Reko.Core.Assemblers;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Types;
using Reko.ImageLoaders.WebAssembly.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.WebAssembly
{
    /// <summary>
    /// This class loads WebAssembly wasm files into a <see cref="Program"/>.
    /// </summary>
    public class WasmLoader : ProgramImageLoader
    {
        public WasmLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw)
            : base(services, imageLocation, imgRaw)
        {
            PreferredBaseAddress = Address.Ptr32(0);
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program LoadProgram(Address? addrLoad)
        {
            var rdr = LoadHeader();
            var sections = LoadSections(rdr);
            var wasmFile = new WasmFile(sections);
            var arch = new WasmArchitecture(Services, "wasm", new());
            var platform = new WasmPlatform(Services, arch, wasmFile);
            var preprocessor = new WasmPreprocessor(arch, platform, wasmFile);
            return preprocessor.Preprocess();
        }

        private List<Section> LoadSections(WasmImageReader rdr)
        {
            var sections = new List<Section>();
            for (;;)
            {
                var s = LoadSection(rdr);
                if (s is null)
                    break;
                sections.Add(s);
            }
            return sections;
        }

        public WasmImageReader LoadHeader()
        {
            var rdr = new WasmImageReader(RawImage);
            if (!rdr.TryReadLeUInt32(out uint magic))
                throw new BadImageFormatException();
            if (!rdr.TryReadLeUInt32(out uint version))
                throw new BadImageFormatException();
            return rdr;
        }

        public Section? LoadSection(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarUInt7(out byte bType))
                return null;            // no more data, return.
            var type = (WasmSection)bType;
            if (!rdr.TryReadVarUInt32(out uint payload_len))
                throw new BadImageFormatException();
            string name;
            if (type == WasmSection.Custom)
            {
                var offset = rdr.Offset;
                // Custom sections' names are part of the payload.
                if (!rdr.TryReadVarUInt32(out uint name_len) || name_len == 0)
                    throw new NotImplementedException();
                name = Encoding.UTF8.GetString(rdr.ReadBytes(name_len));
                payload_len -= (uint)(rdr.Offset - offset);
            }
            else
            {
                name = type.ToString();
            }

            byte[] bytes;
            if (payload_len > 0)
            {
                bytes = rdr.ReadBytes(payload_len);
            }
            else
            {
                bytes = Array.Empty<byte>();
            }
            var rdr2 = new WasmImageReader(bytes);
            switch (type)
            {
            case WasmSection.Custom: return LoadCustomSection(name, bytes); // custom section

            case WasmSection.Type: return LoadTypeSection(rdr2);            // Function signature declarations
            case WasmSection.Import: return LoadImportSection(rdr2);        // Import declarations
            case WasmSection.Function: return LoadFunctionSection(rdr2);    // Function declarations
            case WasmSection.Table: return LoadTableSection(rdr2);          // Indirect function table and other tables
            case WasmSection.Memory: return LoadMemorySection(rdr2);        // Memory attributes
            case WasmSection.Global: return LoadGlobalSection(rdr2);        // Global declarations
            case WasmSection.Export: return LoadExportSection(rdr2);        // Exports
            case WasmSection.Start: return LoadStartSection(rdr2);          // Start function declaration
            case WasmSection.Element: return LoadElementSection(rdr2);      // Elements section
            case WasmSection.Code: return LoadCodeSection(rdr2, bytes);     // Function bodies (code)
            case WasmSection.Data: return LoadDataSection(rdr2);            // Data segments
            default: throw new NotSupportedException();
            }
        }

        private Section LoadCustomSection(string name, byte[] bytes)
        {
            return new CustomSection(name, bytes);
        }

        /// <summary>
        /// The type section declares all function signatures that will be used in the module.
        /// </summary>
        private Section? LoadTypeSection(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarUInt32(out uint count))
                return null;

            var types = new List<FunctionType>();
            for (int i = 0; i < count; ++i)
            {
                var ft = LoadFuncType(rdr);
                if (ft is null)
                    return null;
                types.Add(ft);
            }
            return new TypeSection(".types",  rdr.Bytes, types);
        }

        private Section? LoadImportSection(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarUInt32(out uint count))
                return null;
            var imps = new List<Import>();
            for (uint i = 0; i < count; ++i)
            {
                if (!rdr.TryReadVarUInt32(out uint len))
                    return null;
                string module = Encoding.UTF8.GetString(rdr.ReadBytes(len));
                if (!rdr.TryReadVarUInt32(out len))
                    return null;
                string field = Encoding.UTF8.GetString(rdr.ReadBytes(len));
                if (!rdr.TryReadByte(out byte external_kind))
                    return null;
                switch (external_kind)
                {
                case 0:
                    uint type_index;
                    if (!rdr.TryReadVarUInt32(out type_index))
                        return null;
                    imps.Add(new Import
                    {
                        Type = SymbolType.ExternalProcedure,
                        Module = module,
                        Name = field,
                        Index = type_index,
                    });
                    break;
                case 1:
                    var table = this.ReadTableType(rdr);
                    if (table is null)
                        return null;
                    imps.Add(new Import
                    {
                        Type = SymbolType.Table,
                        Module = module,
                        Name = field,
                        TableType = table,
                    });
                    break;
                case 2:
                    var memory_type = ReadResizableLimits(rdr);
                    if (memory_type is null)
                        return null;
                    imps.Add(new Import
                    {
                        Type = SymbolType.AddressSpace,
                        Module = module,
                        Name = field,
                        MemoryType = memory_type.Value,
                    });
                    break;
                case 3:
                    var global_type = ReadGlobalType(rdr);
                    if (global_type is null)
                        return null;
                    imps.Add(new Import
                    {
                        Type = SymbolType.Data,
                        Module = module,
                        Name = field,
                        GlobalType = global_type.Value,
                    });
                    break;
                default:
                    throw new NotImplementedException();
                }
            }
            return new ImportSection(".imports", rdr.Bytes, imps);
        }

        private Section? LoadFunctionSection(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarUInt32(out uint count))
                return null;
            var decls = new List<uint>();
            for (int i = 0; i < count; ++i)
            {
                if (!rdr.TryReadVarUInt32(out uint decl))
                    return null;
                decls.Add(decl);
            }
            return new FunctionSection(".functions", rdr.Bytes, decls);
        }

        private Section? LoadTableSection(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarUInt32(out uint count))
                return null;
            var tables = new List<TableType>();
            for (int i = 0; i < count; ++i)
            {
                TableType? tt = ReadTableType(rdr);
                if (tt is null)
                    return null;
                tables.Add(tt);
            }
            return new TableSection(".tables", rdr.Bytes, tables);
        }

        private TableType? ReadTableType(WasmImageReader rdr)
        {
            var dt = ReadValueType(rdr);
            if (dt is null)
                return null;

            var tpl = ReadResizableLimits(rdr);
            if (tpl is null)
                return null;

            return new TableType
            {
                EntryType = dt,
                Initial = tpl.Value.Item1,
                Maximum = tpl.Value.Item2,
            };
        }

        private (uint,uint)? ReadResizableLimits(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarUInt32(out uint flags))
                return null;
            if (!rdr.TryReadVarUInt32(out uint init))
                return null;
            uint max = 0;
            if ((flags & 1) != 0)
            {
                if (!rdr.TryReadVarUInt32(out max))
                    return null;
            }
            return (init, max);
        }

        private Section? LoadMemorySection(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarUInt32(out uint count))
                return null;
            var mems = new List<Memory>();
            for (int i = 0; i < count; ++i)
            {
                if (!rdr.TryReadVarUInt32(out uint flags))
                    return null;
                if (!rdr.TryReadVarUInt32(out uint init))
                    return null;
                uint max = 0;
                if ((flags & 1) != 0)
                {
                    if (!rdr.TryReadVarUInt32(out max))
                        return null;
                }
                var mem = new Memory
                {
                    Flags = flags,
                    Initial = init,
                    Maximum = max,
                };
                mems.Add(mem);
            }
            return new MemorySection(".memory", rdr.Bytes, mems);
        }

        private Section? LoadGlobalSection(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarUInt32(out uint count))
                return null;
            var globals = new List<GlobalEntry>();
            for (int i = 0; i < count; ++i)
            {
                var global_type = ReadGlobalType(rdr);
                if (global_type is null)
                    return null;
                var expr = LoadInitExpr(rdr);
                globals.Add(new GlobalEntry
                {
                    Type = global_type.Value,
                    InitExpr = expr,
                });
            }
            return new GlobalSection(".globals", rdr.Bytes, globals);
        }

        private Section? LoadExportSection(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarUInt32(out uint count))
                return null;
            var exports = new List<ExportEntry>();
            for (int i = 0; i < count; ++i)
            {
                if (!rdr.TryReadVarUInt32(out uint len))
                    return null;
                var name = Encoding.UTF8.GetString(rdr.ReadBytes(len));
                if (!rdr.TryReadByte(out byte kind))
                    return null;
                if (!rdr.TryReadVarUInt32(out uint index))
                    return null;
                exports.Add(new ExportEntry
                {
                    Name = name,
                    Type = kind switch
                    {
                        0 => SymbolType.Procedure,
                        1 => SymbolType.Table,
                        2 => SymbolType.AddressSpace,
                        3 => SymbolType.Data,
                        _ => SymbolType.Unknown,
                    },
                    Index = index,
                });
            }
            return new ExportSection(".exports", rdr.Bytes, exports);
        }

        private Section LoadStartSection(WasmImageReader rdr)
        {
            throw new NotImplementedException();
        }

        private Section? LoadElementSection(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarUInt32(out uint count))
                return null;
            var elementSegs = new List<ElementSegment>();
            for (int i = 0; i < count; ++i)
            {
                if (!rdr.TryReadVarUInt32(out uint table_index))
                    return null;
                var offset = LoadInitExpr(rdr);
                if (!rdr.TryReadVarUInt32(out uint cElems))
                    return null;
                var elements = new List<uint>();
                for (int j = 0; j < cElems; ++j)
                {
                    if (!rdr.TryReadVarUInt32(out uint elem))
                        return null;
                    elements.Add(elem);
                }
                elementSegs.Add(new ElementSegment
                {
                    TableIndex = table_index,
                    Offset = offset,
                    Elements = elements,
                });
            }
            return new ElementSection(".elements", rdr.Bytes, elementSegs);
        }

        private Section? LoadCodeSection(WasmImageReader rdr, byte[] bytes)
        {
            if (!rdr.TryReadVarUInt32(out uint count))
                return null;
            var funcBodies = new List<FunctionDefinition>();
            for (int i = 0; i < count; ++i)
            {
                var fd = LoadFunctionDefinition(rdr);
                if (fd is null)
                    return null;
                funcBodies.Add(fd);
            }
            return new CodeSection(".text", bytes, funcBodies);
        }

        private FunctionDefinition? LoadFunctionDefinition(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarUInt32(out uint len))
                return null;
            var end = (int)rdr.Offset + (int)len;
            if (!rdr.TryReadVarUInt32(out uint cEntries))
                return null;
            var locals = new List<LocalVariable>();
            for (int i = 0; i < cEntries; ++i)
            {
                if (!rdr.TryReadVarUInt32(out uint n))
                    return null;
                var dt = ReadValueType(rdr);
                if (dt is null)
                    return null;
                locals.AddRange(Enumerable.Range(0, (int)n).Select(nn => new LocalVariable(dt)));
            }
            var start = (int) rdr.Offset;
            rdr.Offset = end;
            return new FunctionDefinition(
                start,
                end,
                locals.ToArray(),
                rdr.Bytes);
        }

        private Section? LoadDataSection(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarUInt32(out uint count))
                return null;
            var segments = new List<DataSegment>();
            for (int i = 0; i < count; ++i)
            {
                /*
                index 	varuint32 	the linear memory index (0 in the MVP)
                offset 	init_expr 	an i32 initializer expression that computes the offset at which to place the data
                size 	varuint32 	size of data (in bytes)
                data 	bytes 	sequence of size bytes
                 */
                if (!rdr.TryReadVarUInt32(out uint index))
                    return null;
                var offset = LoadInitExpr(rdr);
                if (!rdr.TryReadVarUInt32(out uint size))
                    return null;
                var bytes = rdr.ReadBytes(size);
                if (bytes is null)
                    return null;
                segments.Add(new DataSegment(index, offset, bytes));
            }
            return new DataSection(".data", rdr.Bytes, segments);
        }

        private FunctionType? LoadFuncType(WasmImageReader rdr)
        {
            byte form;              // varint7     the value for the func type constructor as defined above
            uint param_count;       // varuint32   the number of parameters to the function
            byte return_count;      // varuint1    the number of results from the function
            Identifier? ret = null;  // value_type ? the result type of the function(if return_count is 1)

            if (!rdr.TryReadVarUInt7(out form))
                return null;
            if (!rdr.TryReadVarUInt32(out param_count))
                return null;
            var args = new List<Identifier>();
            int cbOffset = 0;
            for (int i = 0; i < param_count; ++i)
            {
                var dt = ReadValueType(rdr);
                if (dt is null)
                    return null;
                args.Add(new Identifier(
                    "arg" + i,
                    dt,
                    new StackStorage(cbOffset, dt)));
                cbOffset += dt.Size;
            }
            if (!rdr.TryReadVarUInt7(out return_count))
                return null;
            if (return_count == 1)
            {
                var dt = ReadValueType(rdr);
                if (dt is null)
                    return null;
                ret = new Identifier(
                    "",
                    dt,
                    new StackStorage(0, dt));
            }
            return new FunctionType(
                args.ToArray(),
                ret is not null ? [ret] : []);
        }

        private DataType? ReadValueType(WasmImageReader rdr)
        {
            if (!rdr.TryReadVarInt7(out sbyte ty))
                return null;
            switch (ty)
            {
            case -0x01: return PrimitiveType.Word32; // i32
            case -0x02: return PrimitiveType.Word64; // i64
            case -0x03: return PrimitiveType.Real32; // f32
            case -0x04: return PrimitiveType.Real64; // f64
            case -0x10: return new TypeReference("anyfunc", new StructureType("anyfunc", 4)); // anyfunc
            case -0x20: throw new NotImplementedException(); // func
            case -0x40: throw new NotImplementedException(); // pseudo type for representing an empty block_type
            default: throw new NotImplementedException($"Unknown value type: {ty:X}.");
            }
        }

        private uint LoadInitExpr(WasmImageReader rdr)
        {
            var eval = new WasmEvaluator(rdr);
            return Convert.ToUInt32(eval.Run());
        }

        private (DataType, bool)? ReadGlobalType(WasmImageReader rdr)
        {
            var dt = this.ReadValueType(rdr);
            if (!rdr.TryReadByte(out byte b))
                return null;
            return (dt!, b != 0);
        }

    }

    public enum WasmSection
    {
        Custom = 0,     // custom section

        Type = 1,       // Function signature declarations
        Import = 2,     // Import declarations
        Function = 3,   // Function declarations
        Table = 4,      // Indirect function table and other tables
        Memory = 5,     // Memory attributes
        Global = 6,     // Global declarations
        Export = 7,     // Exports
        Start = 8,      // Start function declaration
        Element = 9,    // Elements section
        Code = 10,      // Function bodies (code)
        Data = 11,      // Data segments
    }

    public abstract class Section
    {
        public Section(string name, byte[] bytes)
        {
            this.Name = name;
            this.Bytes = bytes;
        }

        /// <summary>
        /// The preferred name of this section.
        /// </summary>
        public string Name { get;  }
        public byte[] Bytes { get; }

        /// <summary>
        /// Optionally creates an <see cref="ImageSegmentRenderer"/>
        /// </summary>
        /// <returns></returns>
        public virtual ImageSegmentRenderer? CreateDesigner(WasmArchitecture wasm, WasmFile wasmFile) => null;

        public override string ToString() => Name;
    }

    public class CustomSection : Section
    {
        public CustomSection(string name, byte[] bytes) 
            : base( name, bytes)
        {
        }
    }

    public class TypeSection : Section
    {

        public TypeSection(string name, byte[] bytes, List<FunctionType> types)
            : base(name, bytes)
        {
            this.Types = types;
        }

        public List<FunctionType> Types { get; }

        public override ImageSegmentRenderer? CreateDesigner(WasmArchitecture wasm, WasmFile wasmFile)
            => new TypeSectionRenderer(this);
        
        public override string ToString()
        {
            return $"Types: {Types.Count}";
        }
    }

    public class ImportSection : Section
    {
        public List<Import> Imports;

        public ImportSection (string name, byte[] bytes, List<Import> imports)
            : base(name, bytes)
        {
            this.Imports = imports;
        }

    }

    public class Import
    {
        public SymbolType Type;
        public string? Module;
        public string? Name;
        public uint Index = ~0u;
        public (DataType, bool) GlobalType;
        public (uint, uint) MemoryType;
        public TableType? TableType;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("(import \"{0}\", \"{1}\" (", Module, Name);
            switch (Type)
            {
            case SymbolType.Data:
                sb.AppendFormat("global {0} {1}", GlobalType.Item2, GlobalType.Item1);
                break;
            case SymbolType.ExternalProcedure:
                sb.AppendFormat("func {0}", Index);
                break;
            case SymbolType.AddressSpace:
                sb.AppendFormat("memory {0} {1}", MemoryType.Item1, MemoryType.Item2);
                break;
            case SymbolType.Table:
                sb.AppendFormat("table {0} {1} {2}", TableType?.Initial ?? 0, TableType?.Maximum ?? 0, TableType?.EntryType ?? new UnknownType());
                break;
            default:
                throw new NotImplementedException();
            }
            sb.Append("))");
            return sb.ToString();
        }
    }

    public class FunctionSection : Section
    {
        public FunctionSection(string name, byte[] bytes, List<uint> declarations)
            :base(name, bytes)
        {
            this.Declarations = declarations;
        }

        public override ImageSegmentRenderer? CreateDesigner(WasmArchitecture wasm, WasmFile wasmFile)
            => new FunctionSectionRenderer(wasm, this, wasmFile);
        
        public List<uint> Declarations { get; }
    }

    public class TableSection : Section
    {
        public TableSection(string name, byte[] bytes, List<TableType> tables)
            : base(name, bytes)
        {
            this.Tables = tables;
        }

        public List<TableType> Tables { get; }
    }

    public class TableType
    {
        public DataType? EntryType;
        public uint Flags;
        public uint Initial;
        public uint Maximum;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("(table {0}", Initial);
            sb.AppendFormat(" {0}", Maximum);
            sb.AppendFormat(" {0})", EntryType);
            return sb.ToString();
        }
    }

    public class MemorySection : Section
    {
        public MemorySection(string name, byte[] bytes, List<Memory> memories)
            : base(name, bytes)
        {
            this.Memories = memories;
        }

        public List<Memory> Memories { get; }
    }

    public class Memory
    {
        public uint Flags;
        public uint Initial;
        public uint Maximum;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("(memory {0} {1})", Initial, Maximum);
            return sb.ToString();
        }
    }

    public class ExportSection : Section
    {
        public ExportSection(string name, byte[] bytes, List<ExportEntry> exports)
            : base(name, bytes)
        {
            this.ExportEntries = exports;
        }
        public List<ExportEntry> ExportEntries { get; }

        public override ImageSegmentRenderer? CreateDesigner(WasmArchitecture arch, WasmFile wasmFile)
        {
            return new ExportSectionRenderer(arch, this, wasmFile);
        }
    }

    public enum ExportEntryKind
    {
        Function = 0,
        Table = 1,
        Memory = 2,
        Global = 3,
    }

    public class ExportEntry
    {
        public const byte Func = 0;

        public string? Name;
        public SymbolType Type;
        public uint Index;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("(export \"{0}\" (", Name);
            sb.AppendFormat("{0} {1}))",
                Type switch
                {
                    SymbolType.Procedure => "func",
                    SymbolType.Table => "table",
                    SymbolType.Data => "global",
                    SymbolType.AddressSpace => "memory",
                    _ => "???"
                },
                Index);
            return sb.ToString();
        }
    }

    public class CodeSection : Section
    {
        public CodeSection(string name, byte[] bytes, List<FunctionDefinition> functions)
            : base(name, bytes)
        {
            this.Functions = functions;
        }

        public List<FunctionDefinition> Functions { get; }

        public override ImageSegmentRenderer? CreateDesigner(WasmArchitecture arch, WasmFile wasmFile)
        {
            return new CodeSectionRenderer(arch, this, wasmFile);
        }
    }

    public class FunctionDefinition
    {
        public FunctionDefinition(int iStart, int iEnd, LocalVariable[] locals, byte[] bytes)
        {
            this.Start = iStart;
            this.End = iEnd;
            this.Locals = locals;
            this.ByteCode = bytes;
        }

        public string? Name;
        public int FunctionIndex;
        public uint TypeIndex;
        public int Start;
        public int End;
        public LocalVariable[] Locals;
        public byte[] ByteCode;
    }

    public class DataSection : Section
    {
        public DataSection(string name, byte[] bytes, List<DataSegment> segments, List<int>? offsets = null)
            : base(name, bytes)
        {
            this.Segments = segments;
            this.FunctionBodyOffsets = offsets ?? new();
        }
        public List<DataSegment> Segments;
        public List<int> FunctionBodyOffsets;
    }

    public class DataSegment
    {
        public DataSegment(uint memoryIndex, uint offset, byte[] bytes)
        {
            this.MemoryIndex = memoryIndex;
            this.Offset = offset;
            this.Bytes = bytes;
        }

        public uint MemoryIndex;
        public uint Offset;
        public byte[] Bytes;
    }

    public class GlobalSection : Section
    {
        public GlobalSection(string name, byte[] bytes, List<GlobalEntry> globals)
            : base(name, bytes)
        {
            this.Globals = globals;
        }

        public List<GlobalEntry> Globals { get; }
    }

    public class GlobalEntry
    {
        public object? InitExpr { get; set; }
        public (DataType, bool) Type { get; set; }

        public int GlobalIndex { get; set; }
        public string? Name { get; set; }
    }

    public class ElementSegment
    {
        public List<uint>? Elements { get; set; }
        public object? Offset { get; set; }
        public uint TableIndex { get; set; }
    }

    public class ElementSection : Section
    {
        public ElementSection(string name, byte[] bytes, List<ElementSegment> segments)
            : base(name, bytes)
        {
            this.Segments = segments;
        }
        public List<ElementSegment> Segments { get; }
    }

    public class LocalVariable
    {
        public LocalVariable(DataType dt) { this.DataType = dt; }
        internal DataType DataType;
    }
#nullable enable
}
