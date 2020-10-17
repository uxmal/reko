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
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Reko.Core;
using Reko.Core.Services;

namespace Reko.ImageLoaders.TekHex
{
    /// <summary>
    /// Loads (extended) Tektronix HEX files.
    /// </summary>
    public class TekHexLoader : ImageLoader
    {
        private readonly List<Section> sections = new List<Section>();
        private readonly List<Symbol> symbols = new List<Symbol>();
        private readonly List<(Address, List<byte>)> mems = new List<(Address, List<byte>)>();
        private readonly DecompilerEventListener eventListener;
        private int nLines;
        private uint? uAddrLast;

        public TekHexLoader(IServiceProvider services, string filename, byte[] rawImage)
            : base(services, filename, rawImage)
        {
            this.eventListener = services.RequireService<DecompilerEventListener>();
        }

        public override Address PreferredBaseAddress 
        {
            get { return Address.Ptr16(0); }
            set { throw new NotImplementedException(); }
        }

        int ComputeChecksum(string line)
        {
            int i, tekval, len = line.Length, chksum = 0;
            int p = 1;

            for (i = 1; i < len; i++, p++)
            {
                if (i != 4 && i != 5)   // skip checksum bytes
                {
                    char ch = line[p];
                    if (Char.IsDigit(ch))
                        tekval = ch - '0';
                    else if (Char.IsUpper(ch))
                        tekval = ch - 'A' + 10;
                    else if (Char.IsLower(ch))
                        tekval = ch - 'a' + 10;
                    else
                    {
                        switch (ch)
                        {
                        case '$': tekval = 36; break;
                        case '%': tekval = 37; break;
                        case '.': tekval = 38; break;
                        case '_': tekval = 39; break;
                        default:
                            throw new BadImageFormatException($"Illegal character '{ch}' (U+{(int)ch:X4}) in tekhex line.");
                        }
                    }
                    chksum += tekval;
                }
            }
            return (chksum & 0xff);
        }

        private void ParseLine(string line, IProcessorArchitecture arch)
        {
            if (line[0] != '%')
                throw new BadImageFormatException("illegal format");
            int line_len = line.Length - 1; // Skip the '%'
            int linep = 1;
            var blk_len = ReadHexNumber(line, ref linep, 2);
            var lineType = ReadHexNumber(line, ref linep, 1);
            var checksum = ReadHexNumber(line, ref linep, 2);
            var addr_len = ReadHexNumber(line, ref linep, 1);
            if (addr_len == 0)
                addr_len = 16;

            if (line_len != blk_len)
                throw new BadImageFormatException($"Line length error in line {nLines}.");
            if (checksum != (ComputeChecksum(line) & 0xff))
                eventListener.Warn($"Checksum error in line {nLines}.");
            switch (lineType)
            {
            case 3:     // Symbol
                var sectname = line.Substring(linep, addr_len);
                linep += addr_len;
                while (linep < line_len)
                {
                    var type = ReadHexNumber(line, ref linep, 1); // symbol type
                    if (type == 0)  // section definition
                    {
                        int baseaddr_len = (int) ReadHexNumber(line, ref linep, 1);
                        uint baseaddr = (uint) ReadHexNumber(line, ref linep, baseaddr_len);
                        int sectlen_len = (int) ReadHexNumber(line, ref linep, 1);
                        uint sectlen = (uint) ReadHexNumber(line, ref linep, sectlen_len);

                        sections.Add(new Section
                        {
                            name = sectname,
                            uAddrBase = baseaddr,
                            length = sectlen,
                        });
                    }
                    else            // symbol definition
                    {
                        var symbolLength = ReadHexNumber(line, ref linep, 1); // symbol name length
                        if (symbolLength == 0)
                            symbolLength = 16;
                        var sym = line.Substring(linep, symbolLength);   // symbol name
                        linep += symbolLength;
                        var valLength = (int) ReadHexNumber(line, ref linep, 1); // value length
                        uint val = (uint) ReadHexNumber(line, ref linep, valLength);       // value
                        // lprintf ("ELM %s %s %s\n", sym, sectname, linep);
                        AddSymbol(sym, type, val, sections.Last());
                    }
                }
                break;

            case 6:     // Data
                var uAddr = (uint) ReadHexNumber(line, ref linep, addr_len);
                Debug.Print("  Data: {0:X6}h {1} {2}", uAddr, line_len, line);
                if (!uAddrLast.HasValue || uAddrLast.Value != uAddr)
                {
                    // For word oriented archs, we have to scale the addresses.
                    var addrTarget = Address.Create(arch.PointerType, uAddr * 8u / (uint) arch.MemoryGranularity);
                    var ab = new List<byte>();
                    this.mems.Add((addrTarget, ab));
                }
                var (_, mem) = mems.Last();
                while (linep < line_len)
                {
                    mem.Add((byte) ReadHexNumber(line, ref linep, 2));
                    ++uAddr;
                }
                uAddrLast = uAddr;
                break;

            case 8:     // Terminator
                uAddr = (uint) ReadHexNumber(line, ref linep, addr_len);
                //simreg.ic = (ushort) ((address >> 1) & 0xffff);
                break;

            default:
                throw new BadImageFormatException($"Illegal type in line {nLines}.");
            }
        }

        public override Program Load(Address? addrLoad)
            => throw new NotSupportedException();

        public override Program Load(Address addrLoad, IProcessorArchitecture arch, IPlatform platform)
        { 
            nLines = 0;
            this.uAddrLast = null;
            mems.Clear();
            sections.Clear();
            symbols.Clear();
            using var lines = new StreamReader(new MemoryStream(RawImage));
            for (; ; )
            {
                var lline = lines.ReadLine();
                if (lline is null)
                    break;
                ++nLines;
                if (lline.Length < 2)
                    continue;
                ParseLine(lline, arch);
            }
            var segmentMap = MakeSegmentMap(arch);
            return new Program(
                segmentMap,
                arch,
                platform);
        }

        private SegmentMap MakeSegmentMap(IProcessorArchitecture arch)
        {
            var segs = new List<ImageSegment>();
            foreach (var section in sections)
            {
                var addrBase = Address.Create(arch.PointerType, section.uAddrBase);
                var bytes = mems.Find(m => m.Item1 == addrBase);
                var mem = arch.CreateMemoryArea(addrBase, bytes.Item2.ToArray());
                var seg = new ImageSegment(section.name!, mem, AccessMode.ReadWriteExecute);
                segs.Add(seg);
            }
            var baseAddr = segs.Min(s => s.Address);

            return new SegmentMap(baseAddr, segs.ToArray());
        }

        private SortedList<Address, ImageSymbol> MakeSymbols(IProcessorArchitecture arch)
        {
            return this.symbols.Select(s =>
            {
                var addr = Address.Create(arch.PointerType, s.value);
                return s.type switch
                {
                    SymbolType.GLOBAL_CODE_ADDRESS =>
                        ImageSymbol.Procedure(arch, addr, s.name),
                    _ => throw new NotSupportedException($"Unsupported symbol type '{s.type}.")
                };
            }).ToSortedList(s => s.Address!);
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(
                new List<ImageSymbol>(),
                MakeSymbols(program.Architecture));
        }

        public void Dump()
        {
            var sw = new StringWriter();
            Write(sw);
            Debug.WriteLine(sw);
        }

        private void Write(TextWriter w)
        {
            int i;

            for (i = 0; i < sections.Count; i++)
            {
                if (i == 0)
                    w.WriteLine("Section             BaseAddr Length");
                w.WriteLine("{0,-20}  {1:X4}h     {2}", sections[i].name,
                  sections[i].uAddrBase, sections[i].length);
            }
            for (i = 0; i < symbols.Count; i++)
            {
                if (i == 0)
                {
                    w.WriteLine();
                    w.WriteLine("Symbolname           Value Type                 Section");
                }
                w.WriteLine("{0,-20} {1:X5} {2,-20} {3}", symbols[i].name,
                  symbols[i].value, symbols[i].type,
                  symbols[i].section!.name);
            }
        }

        private static int ReadHexNumber(string line, ref int pos, int digits)
        {
            uint retval = 0;
            int p = pos;

            while (digits-- > 0)
            {
                char chNibble = line[p++];
                if (!Reko.Core.BytePattern.TryParseHexDigit(chNibble, out byte nibble))
                    return -1;
                retval = (retval << 4) | (uint) (nibble & 0xF);
            }
            pos = p;
            return (int) retval;
        }

        private void AddSymbol(string name, int type, uint value, Section section)
        {
            symbols.Add(new Symbol
            {
                name = name,
                type = (SymbolType) type,
                value = value,
                section = section
            });
        }

        public class Section
        {
            public string? name;
            public uint uAddrBase;
            public uint length;
        }

        public class Symbol
        {
            public string? name;
            public SymbolType type;
            public uint value;
            public Section? section;
        }


        // TekHex symbol type codes
        public enum SymbolType
        {
            GLOBAL_ADDRESS = 1,
            GLOBAL_SCALAR = 2,
            GLOBAL_CODE_ADDRESS = 3,
            GLOBAL_DATA_ADDRESS = 4,
            LOCAL_ADDRESS = 5,
            LOCAL_SCALAR = 6,
            LOCAL_CODE_ADDRESS = 7,
            LOCAL_DATA_ADDRESS = 8,
        }
    }
}