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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmLoader : ImageLoader
    {
        internal static string[] ExportKinds =
        {
            "func",
            "table",
            "memory",
            "global",
        };

        public WasmLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
        {
        }

        public override Address PreferredBaseAddress { get; set; }

        public override Program Load(Address addrLoad)
        {
            var rdr = LoadHeader();
            LoadSections(rdr);
            throw new NotImplementedException();
        }

        private void LoadSections(LeImageReader rdr)
        {
            Console.WriteLine("Wasm");
            for (;;)
            {
                var s = LoadSection(rdr);
                Console.WriteLine("{0,-20}: {1,-20} {2}", s.Name, s.Type, s.Bytes.Length);
            }
            throw new NotImplementedException();
        }

        public LeImageReader LoadHeader()
        {
            var rdr = new LeImageReader(RawImage);
            uint magic;
            if (!rdr.TryReadLeUInt32(out magic))
                throw new BadImageFormatException();
            uint version;
            if (!rdr.TryReadLeUInt32(out version))
                throw new BadImageFormatException();
            return rdr;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }

        public Section LoadSection(LeImageReader rdr)
        {
            byte bType;
            uint payload_len;
            uint name_len;
            if (!TryReadVarUInt7(rdr, out bType))
                return null;            // no more data, return.
            var type = (WasmSection)bType;
            if (!TryReadVarUInt32(rdr, out payload_len))
                throw new BadImageFormatException();
            string name;
            if (type == WasmSection.Custom)
            {
                if (!TryReadVarUInt32(rdr, out name_len) || name_len == 0)
                    throw new NotImplementedException();
                name = Encoding.UTF8.GetString(rdr.ReadBytes(name_len));
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
                bytes = new byte[0];
            }
            var rdr2 = new LeImageReader(bytes);
            switch (type)
            {
            case WasmSection.Custom: return LoadCustomSection(name, rdr2);     // custom section

            case WasmSection.Type: return LoadTypeSection(rdr2);       // Function signature declarations
            case WasmSection.Import: return LoadImportSection(rdr2);     // Import declarations
            case WasmSection.Function: return LoadFunctionSection(rdr2);   // Function declarations
            case WasmSection.Table: return LoadTableSection(rdr2);      // Indirect function table and other tables
            case WasmSection.Memory: return LoadMemorySection(rdr2);     // Memory attributes
            case WasmSection.Global: return LoadGlobalSection(rdr2);     // Global declarations
            case WasmSection.Export: return LoadExportSection(rdr2);     // Exports
            case WasmSection.Start: return LoadStartSection(rdr2);      // Start function declaration
            case WasmSection.Element: return LoadElementSection(rdr2);    // Elements section
            case WasmSection.Code: return LoadCodeSection(rdr2);      // Function bodies (code)
            case WasmSection.Data: return LoadDataSection(rdr2);      // Data segments
            }

            return new Section
            {
                Type = type,
                Name = name,
                Bytes = bytes,
            };
        }

        private Section LoadCustomSection(string name, LeImageReader rdr)
        {
            throw new NotImplementedException();
        }

        // The type section declares all function signatures that will be used in the module.
        private Section LoadTypeSection(LeImageReader rdr)
        {
            uint count;
            if (!this.TryReadVarUInt32(rdr, out count))
                return null;

            var types = new List<FunctionType>();
            for (int i = 0; i < count; ++i)
            {
                var ft = LoadFuncType(rdr);
                types.Add(ft);
            }
            return new TypeSection
            {
                Types = types,
            };
        }

        private Section LoadImportSection(LeImageReader rdr)
        {
            uint count;
            if (!this.TryReadVarUInt32(rdr, out count))
                return null;
            var imps = new List<Import>();
            for (uint i = 0; i < count; ++i)
            {
                uint len;
                if (!this.TryReadVarUInt32(rdr, out len))
                    return null;
                string module = Encoding.UTF8.GetString(rdr.ReadBytes(len));
                if (!this.TryReadVarUInt32(rdr, out len))
                    return null;
                string field = Encoding.UTF8.GetString(rdr.ReadBytes(len));
                byte external_kind;
                if (!rdr.TryReadByte(out external_kind))
                    return null;
                switch (external_kind)
                {
                case 0:
                    uint function_index;
                    if (!TryReadVarUInt32(rdr, out function_index))
                        break;
                    imps.Add(new Import
                    {
                        Module = module,
                        Field = field,
                        FunctionIndex = function_index,
                    });
                    break;
                default:
                    throw new NotImplementedException();
                }

                /*

    0 indicating a Function import or definition
    1 indicating a Table import or definition
    2 indicating a Memory import or definition
    3 indicating a Global import or definition
                 * 
                 */

            }
            return new ImportSection
            {
                Imports = imps,
            };
        }

        private Section LoadFunctionSection(LeImageReader rdr)
        {
            uint count;
            if (!this.TryReadVarUInt32(rdr, out count))
                return null;
            var decls = new List<uint>();
            for (int i = 0; i < count; ++i)
            {
                uint decl;
                if (!this.TryReadVarUInt32(rdr, out decl))
                    return null;
                decls.Add(decl);
            }
            return new FunctionSection
            {
                Declarations = decls
            };
        }

        private Section LoadTableSection(LeImageReader rdr)
        {
            uint count;
            if (!this.TryReadVarUInt32(rdr, out count))
                return null;
            var tables = new List<TableType>();
            for (int i = 0; i < count; ++i)
            {
                var dt = ReadValueType(rdr);
                if (dt == null)
                    return null;
                uint flags;
                if (!TryReadVarUInt32(rdr, out flags))
                    return null;
                uint init;
                if (!TryReadVarUInt32(rdr, out init))
                    return null;
                    uint max = 0;
                if ((flags & 1) != 0)
                {
                    if (!TryReadVarUInt32(rdr, out max))
                        return null;
                }
                var tt = new TableType
                {
                    EntryType = dt,
                    Initial = init,
                    Maximum = max,
                };
                tables.Add(tt);
            }
            return new TableSection
            {
                Tables = tables
            };
        }

        private Section LoadMemorySection(LeImageReader rdr)
        {
            uint count;
            if (!this.TryReadVarUInt32(rdr, out count))
                return null;
            var mems = new List<Memory>();
            for (int i = 0; i < count; ++i)
            {
                uint flags;
                if (!TryReadVarUInt32(rdr, out flags))
                    return null;
                uint init;
                if (!TryReadVarUInt32(rdr, out init))
                    return null;
                uint max = 0;
                if ((flags & 1) != 0)
                {
                    if (!TryReadVarUInt32(rdr, out max))
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
            return new MemorySection
            {
                Memories = mems,
            };
        }

        private Section LoadGlobalSection(LeImageReader rdr)
        {
            throw new NotImplementedException();
        }

        private Section LoadExportSection(LeImageReader rdr)
        {
            uint count;
            if (!this.TryReadVarUInt32(rdr, out count))
                return null;
            var exports = new List<ExportEntry>();
            for (int i = 0; i < count; ++i)
            {
                uint len;
                if (!TryReadVarUInt32(rdr, out len))
                    return null;
                var field = Encoding.UTF8.GetString(rdr.ReadBytes(len));
                byte kind;
                if (!rdr.TryReadByte(out kind))
                    return null;
                uint index;
                if (!TryReadVarUInt32(rdr, out index))
                    return null;
                exports.Add(new ExportEntry
                {
                    Field = field,
                    Kind = kind,
                    Index = index,
                });
            }
            return new ExportSection
            {
                ExportEntries = exports
            };
        }

        private Section LoadStartSection(LeImageReader rdr)
        {
            throw new NotImplementedException();
        }

        private Section LoadElementSection(LeImageReader rdr)
        {
            throw new NotImplementedException();
        }

        private Section LoadCodeSection(LeImageReader rdr)
        {
            uint count;
            if (!this.TryReadVarUInt32(rdr, out count))
                return null;
            var funcBodies = new List<byte[]>();
            for (int i = 0; i < count; ++i)
            {
                uint len;
                if (!TryReadVarUInt32(rdr, out len))
                    return null;
                var codeBytes = rdr.ReadBytes(len);
                funcBodies.Add(codeBytes);
            }
            return new CodeSection
            {
                 FunctionBodies = funcBodies,
            };
        }

        private Section LoadDataSection(LeImageReader rdr)
        {
            throw new NotImplementedException();
        }

        private FunctionType LoadFuncType(LeImageReader rdr)
        {
            byte form;          // varint7     the value for the func type constructor as defined above
            uint param_count;   //  varuint32   the number of parameters to the function
            byte return_count;    // varuint1    the number of results from the function
            Identifier ret = null;   //      value_type ? the result type of the function(if return_count is 1)

            if (!TryReadVarUInt7(rdr, out form))
                return null;
            if (!TryReadVarUInt32(rdr, out param_count))
                return null;
            var args = new List<Identifier>();
            int cbOffset = 0;
            for (int i = 0; i < param_count; ++i)
            {
                var dt = ReadValueType(rdr);
                if (dt == null)
                    return null;
                args.Add(new Identifier(
                    "arg" + i,
                    dt,
                    new StackArgumentStorage(cbOffset, dt)));
                cbOffset += dt.Size;
            }
            if (!TryReadVarUInt7(rdr, out return_count))
                return null;
            if (return_count == 1)
            {
                var dt = ReadValueType(rdr);
                ret = new Identifier(
                    "",
                    dt,
                    new StackArgumentStorage(0, dt));
            }
            return new FunctionType(
                ret,
                args.ToArray());
        }

        private DataType ReadValueType(LeImageReader rdr)
        {
            sbyte ty;
            if (!TryReadVarInt7(rdr, out ty))
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
            default: throw new NotImplementedException();
            }
        }

        private bool TryReadVarUInt32(LeImageReader rdr, out uint u)
        {
            u = 0;
            int sh = 0;
            byte b;
            do
            {
                if (!rdr.TryReadByte(out b))
                    return false;
                u = ((b & 0x7Fu) << sh) | u;
                sh += 7;
                //$TODO: overflow.
            } while ((b & 0x80) != 0);
            return true;
        }

        private bool TryReadVarUInt7(ImageReader rdr, out byte b)
        {
            if (!rdr.TryReadByte(out b))
                return false;
            if ((b & 0x80) != 0)
                return false;
            return true;
        }

        private bool TryReadVarInt7(ImageReader rdr, out sbyte sb)
        {
            byte b;
            sb = 0;
            if (!rdr.TryReadByte(out b))
                return false;
            if ((b & 0x80) != 0)
                return false;
            if ((b & 40) != 0)
            {
                sb = (sbyte)(0xC0 | b);
            }
            else
            {
                sb = (sbyte)b;
            }
            return true;
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

    public class Section
    {
        public byte[] Bytes { get; internal set; }
        public string Name { get; internal set; }
        public WasmSection Type { get; internal set; }
    }

    public class TypeSection :Section
    {
        public List<FunctionType> Types;
    }

    public class ImportSection : Section
    {
        public List<Import> Imports;
    }

    public class Import
    {
        public string Module;
        public string Field;
        public uint FunctionIndex;
    }

    public class FunctionSection : Section
    {
        public List<uint> Declarations;
    }

    public class TableSection : Section
    {
        public List<TableType> Tables;
    }

    public class TableType
    {
        public DataType EntryType;
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
        public List<Memory> Memories;
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
        public List<ExportEntry> ExportEntries;
    }

    public class ExportEntry
    {
        public string Field;
        public byte Kind;
        public uint Index;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("(export \"{0}\" (", Field);
            sb.AppendFormat("{0} {1}))",
                WasmLoader.ExportKinds[Kind],
                Index);
            return sb.ToString();
        }
    }

    public class CodeSection : Section
    {
        public List<byte[]> FunctionBodies;
    }
}
