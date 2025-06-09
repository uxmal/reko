/***************************************************************************/
/*                                                                         */
/* Project   :        sim1750 -- Mil-Std-1750 Software Simulator           */
/*                                                                         */
/* Component :       ldm.cs -- ASCII Load Module loading functions          */
/*                                                                         */
/* Copyright :         (C) Daimler-Benz Aerospace AG, 1994-97              */
/*                         (C) 2017 Oliver M. Kellogg                      */
/* Contact   :            okellogg@users.sourceforge.net                   */
/*                                                                         */
/* Disclaimer:                                                             */
/*                                                                         */
/*  This program is free software; you can redistribute it and/or modify   */
/*  it under the terms of the GNU General Public License as published by   */
/*  the Free Software Foundation; either version 2 of the License, or      */
/*  (at your option) any later version.                                    */
/*                                                                         */
/*  This program is distributed in the hope that it will be useful,        */
/*  but WITHOUT ANY WARRANTY; without even the implied warranty of         */
/*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          */
/*  GNU General Public License for more details.                           */
/*                                                                         */
/*  You should have received a copy of the GNU General Public License      */
/*  along with this program; if not, write to the Free Software            */
/*  Foundation, 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.   */
/*                                                                         */
/***************************************************************************/
// Ported to C# by John Källén

using Reko.Arch.MilStd1750;
using Reko.Core;
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Ldm
{
    using static Reko.Core.BytePattern;

    public class LdmLoader : ProgramImageLoader
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(LdmLoader), "Trace LDM loader");

        private const int TLDADDR = 2;
        private const int XTCADDR = 3;
        private const int COUNT = 7;
        private const int CHECKSUM = 8;

        private const int TLD_LDM = 0;
        private const int XTC_LDM = 1;

        // Banks
        private const int CODE = 0;
        private const int DATA = 1;

        private const int DATASTART = 12;

        private readonly Action<string>[] load_ldmline;
        private int linecnt;
        private ProcessorState? simreg;
        private mem_t [] mem;


        public LdmLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw) 
            : base(services, imageLocation, imgRaw)
        {
            this.load_ldmline = new Action<string>[] { load_tldline, load_xtcline };
            mem = new mem_t[128];
        }

        public override Address PreferredBaseAddress
        {
            get { return Address.Ptr16(0); }
            set { throw new NotSupportedException(); }
        }

        private static int get_word(string str, int offset) => get_nibbles(str, offset, 4);

        // Rotate-left a 16 bit word
        private static int rotl16(int num, int n_shifts)   
        {
            uint buf;

            if (n_shifts < 0 || n_shifts > 15)
                throw new ArgumentOutOfRangeException($"rotl16 called with illegal n_shifts {n_shifts}.");
            buf = ((uint) num & 0xFFFF) << n_shifts;
            buf = (buf & 0xFFFF) | (buf >> 16);
            return (int) buf;
        }

        /* Compute the checksum for a line from a TLD Load Module file */

        private int check_tldline(string line)
        {
            int code; 
            bool address_field_used = true, cmd_m_c_t = false;

            if (line[0] != '/')
                throw new ArgumentOutOfRangeException($"Illegal start character for LDM line");

            switch (line[1])
            {
            case 'M':
                cmd_m_c_t = true;       // slides through to following case
                goto case 'I';
            case 'I':
            case 'O':
                code = 0x9;
                break;
            case 'N':
                code = 0xA;
                break;
            case 'L':
                code = 0xA + (line[5] - '0');  /* Instruction => 0xA, Oprnd => 0xB */
                break;
            case 'Q':
                code = 0xB;
                break;
            case 'A':
                address_field_used = false;
                code = 0x6;
                break;
            case 'T':
                code = 0x6;
                cmd_m_c_t = true;
                break;
            case 'Z':
                address_field_used = false; /* slides through to following case */
                goto case 'G';
            case 'G':
            case 'H':
                code = 0x8;
                break;
            case 'C':
                cmd_m_c_t = true;       /* slides through to following case */
                goto case 'B';
            case 'B':
            case 'P':
                code = 0xE;
                break;
            default:
                throw new InvalidOperationException("Illegal Command type in TLD LDM line.");
            }
            code <<= 1;
            if (address_field_used)
            {
                if (!TryParseHexDigit(line[TLDADDR], out byte addr_hinibble))
                    throw new BadImageFormatException("ldm: illegal char in MS-nibble of address.");
                int addr16 = get_word(line, TLDADDR + 1);
                if (addr16 == -1)
                    throw new BadImageFormatException("ldm: illegal char in address field.");
                code ^= (int) addr16;
                if (cmd_m_c_t)
                    code = rotl16(code, 1) ^ addr_hinibble;
            }
            if (!TryParseHexDigit(line[COUNT], out byte datacnt))
                throw new BadImageFormatException("ldm: illegal character in data count");
            for (int i = 0; i < datacnt; i++)
            {
                int dataword = get_word(line, DATASTART + (4 * i));
                if (dataword == -1)
                    throw new BadImageFormatException("ldm: illegal char in data word.");
                code = rotl16(code, 1) ^ (int) dataword;
            }

            return code;
        }

        /* Analyze and load a line from a TLD Load Module file */

        private void load_tldline(string line)
        {
            int cmd, actual_chksum;
            int claimed_chksum;
                uint address;

            if (line[0] != '/')
                throw new ArgumentException($"Illegal format at line {linecnt}.");
            cmd = line[1];
            if (cmd == ';' || cmd == 'E' || cmd == 'H' || cmd == 'Z')
                return;
            if ((claimed_chksum = get_word(line, CHECKSUM)) == -1)
                throw new BadImageFormatException($"numeric syntax error in checksum at TLD LDM line {linecnt}.");
            actual_chksum = check_tldline(line);
            if ((int) claimed_chksum != actual_chksum)
                trace.Warn($"Incorrect checksum in TLD LDM line {linecnt} (computed: {actual_chksum:X4}h).");
            switch (cmd)
            {
            case 'A':
                UpdateReg(Registers.sw, u => (ushort) ((u & 0xFFF0) | ((ushort) get_word(line, DATASTART) & 0x000F)));
                break;
            case 'I':
            case 'O':
                {
                    int i, bank = (cmd == 'I') ? CODE : DATA;
                    if (!TryParseHexDigit(line[TLDADDR], out byte @as))
                        throw new BadImageFormatException($"line {linecnt}: /{cmd} error in load address state.");
                    if (!TryParseHexDigit(line[TLDADDR + 1], out byte logaddr_hinibble))
                        throw new BadImageFormatException($"line {linecnt}: /{cmd} error in logical address MS-nibble.");
                    if ((address = (uint)get_nibbles(line, TLDADDR + 2, 3)) == ~0u)
                        throw new BadImageFormatException($"line {linecnt}: /{cmd} error in logical address.");
                    address |= (uint) pagereg[bank,@as,logaddr_hinibble].ppa << 12;
                    if (!TryParseHexDigit(line[COUNT], out byte datacnt))
                        throw new BadImageFormatException($"line {linecnt}: /{cmd} error in data count.");
                    for (i = 0; i < datacnt; i++)
                    {
                        int word = get_word(line, DATASTART + (4 * i));
                        if (word == -1)
                            throw new BadImageFormatException($"line {linecnt}: /{cmd} data error.");
                        poke(address++, (ushort) word);
                    }
                }
                break;
            case 'L':
                {
                    if (!TryParseHexDigit(line[TLDADDR], out byte @as))
                        throw new BadImageFormatException($"line {linecnt}: /L error in load address state.");
                    if (!TryParseHexDigit(line[TLDADDR + 3], out byte bank))
                        throw new BadImageFormatException($"line {linecnt}: /L error in load address bank.");
                    if (!TryParseHexDigit(line[TLDADDR + 4], out byte pagereg_number))
                        throw new BadImageFormatException($"line {linecnt}: /L error in pagereg number.");
                    if (!TryParseHexDigit(line[COUNT], out byte datacnt))
                        throw new BadImageFormatException($"line {linecnt}: /L error in data count.");
                    for (int i = 0; i < datacnt; i++)
                    {
                        if (!TryParseHexDigit(line[DATASTART + (4 * i)], out byte allocation_type))
                            throw new BadImageFormatException($"line {linecnt}: /L error in allocation type.");
                        if (allocation_type != 2)
                            throw new BadImageFormatException($"line {linecnt}: /L allocation type {allocation_type} unimplemented.");
                        int pagereg_contents = get_nibbles(line, DATASTART + (4 * i) + 1, 3);
                        if (pagereg_contents == -1 || pagereg_contents > 0xFF)
                            throw new BadImageFormatException($"line {linecnt}: /L pagereg contents error.");
                        pagereg[bank,@as,pagereg_number].ppa = (ushort) pagereg_contents;
                        if (++pagereg_number > 0xF)
                            break;
                    }
                }
                break;
            case 'M':
                {
                    if (!TryParseHexDigit(line[TLDADDR], out byte physaddr_hinibble))
                        throw new BadImageFormatException($"line {linecnt}: /M error in load address high nibble.");
                    if ((address = (uint) get_word(line, TLDADDR + 1)) == ~0u)
                        throw new BadImageFormatException($"line {linecnt}: /M error in load address.");
                    address |= (uint)physaddr_hinibble << 16;
                    if (!TryParseHexDigit(line[COUNT], out byte datacnt))
                        throw new BadImageFormatException($"line {linecnt}: /M error in data count.");
                    for (int i = 0; i < datacnt; i++)
                    {
                        int word = get_word(line, DATASTART + (4 * i));
                        if (word == -1)
                            throw new BadImageFormatException($"line {linecnt}: /M data error");
                        poke(address++, (ushort) word);
                    }
                }
                break;
            case 'N':
            case 'Q':
                {
                    int i, bank = (cmd == 'N') ? CODE : DATA;
                    if ((address = (uint) get_nibbles(line, TLDADDR, 5)) == ~0u)
                        throw new BadImageFormatException($"line {linecnt}: /{cmd} data error in address field.");
                    if (address > 0xFF)
                        throw new BadImageFormatException($"line {linecnt}: /{cmd} starting pagereg number too large.");
                    if (!TryParseHexDigit(line[COUNT], out byte datacnt))
                        throw new BadImageFormatException($"line {linecnt}: /{cmd} error in data count.");
                    for (i = 0; i < datacnt; i++)
                    {
                        int @as = (int) address >> 4;
                        int pagenum = (int) address & 0xF;
                        int word = get_word(line, DATASTART + (4 * i));
                        if (word == -1)
                            throw new BadImageFormatException($"line {linecnt}: /{cmd} data error.");
                        pagereg[bank,@as,pagenum].Value = (ushort) word;
                        address++;
                    }
                }
                break;
            case 'T':
                if ((address = (uint)get_nibbles(line, TLDADDR, 5)) == ~0u)
                    throw new BadImageFormatException("TLD LDM: numeric syntax error in transfer address.");
                UpdateReg(Registers.ic, u => (ushort) address);
                if (address > 0xFFFF)
                    throw new BadImageFormatException("TLD LDM: cannot handle transfer address (too high)");
                break;
            default:
                trace.Warn("TLD LDM: unimplemented command type '{0}' (ignored)", cmd);
                break;
            }
        }


        /* Compute the checksum for a line from an XTC Load Module file */

        private int check_xtcline(string line)
        {
            int code;
            bool address_field_used = true;

            if (line[0] != '/')
                throw new BadImageFormatException("illegal start character for XTC LDM line");

            switch (line[1])
            {
            case 'I':
                code = 0;
                break;
            case 'O':
                code = 1;
                break;
            case 'E':
                code = 2;
                address_field_used = false;
                break;
            case 'W':
                code = 3;
                address_field_used = false;
                break;
            case 'D':
                code = 4;
                address_field_used = false;
                break;
            case 'P':
                code = 5;
                address_field_used = false;
                break;
            case 'T':
                code = 6;
                break;
            case 'C':
                code = 7;
                break;
            case 'Z':
                code = 8;
                address_field_used = false;
                break;
            default:
                trace.Warn($"Illegal record type '{line[1]}' in XTC LDM line.");
                return 0;
            }
            if (address_field_used)
            {
                int addr16 = get_word(line, XTCADDR);
                if (addr16 == -1)
                    throw new BadImageFormatException("xtcldm: illegal char in address field.");
                code = (code << 1) ^ (int) addr16;
            }
            if (!TryParseHexDigit(line[COUNT], out byte datacnt))
                throw new BadImageFormatException("xtcldm: illegal character in data count.");
            for (int i = 0; i < datacnt; i++)
            {
                int dataword = get_word(line, DATASTART + (4 * i));
                if (dataword == -1)
                    throw new BadImageFormatException("xtcldm: illegal char in data word");
                code = rotl16(code, 1) ^ (int) dataword;
            }
            return code;
        }

        /* Analyze and load a line from an XTC Load Module file */

        private void load_xtcline(string line)
        {
            int cmd, actual_chksum;
            int claimed_chksum;
            uint address;

            if (line[0] != '/')
                throw new BadImageFormatException($"XTC LDM: illegal format at line {linecnt}.");
            cmd = line[1];
            if (cmd == 'C' || cmd == 'Z')       /* Comment or end-of-file */
                return;
            if ((claimed_chksum = get_word(line, CHECKSUM)) == -1)
                throw new BadImageFormatException($"Numeric syntax error in checksum at XTC LDM line {linecnt}.");
            actual_chksum = check_xtcline(line);
            if ((int) claimed_chksum != actual_chksum)
                trace.Warn("incorrect checksum in XTC LDM line {0} (computed: {1:X4}h)", linecnt, actual_chksum);
            switch (cmd)
            {
            case 'D':       /* DMA/Processor write protect (TBD) */
            case 'P':
                break;
            case 'E':       /* Execution/operand-Write protect (TBD) */
            case 'W':
                break;
            case 'I':       /* Instruction/Operand memory */
            case 'O':
                {
                    int bank = (cmd == 'I') ? CODE : DATA;
                    int @as = simreg!.GetRegister(Registers.sw).ToInt32() & 0xF;
                    if (!TryParseHexDigit(line[XTCADDR], out byte logaddr_hinibble))
                        throw new BadImageFormatException($"line {linecnt}: /{cmd} error in logical address MS-nibble.");
                    if ((address = (uint) get_nibbles(line, XTCADDR + 1, 3)) == ~0u)
                        throw new BadImageFormatException($"line {linecnt}: /{cmd} error in logical address.");
                    address |= (uint) pagereg[bank,@as,logaddr_hinibble].ppa << 12;
                    if (!TryParseHexDigit(line[COUNT], out byte datacnt))
                        throw new BadImageFormatException($"line {linecnt}: /{cmd} error in data count.");
                    for (int i = 0; i < datacnt; i++)
                    {
                        int word = get_word(line, DATASTART + (4 * i));
                        if (word == -1)
                            throw new BadImageFormatException($"line {linecnt}: /{cmd} data error.");
                        poke(address++, (ushort) word);
                    }
                }
                break;
            case 'T':       /* Transfer address */
                if ((address = (uint)get_nibbles(line, XTCADDR, 4)) == ~0u)
                    throw new BadImageFormatException("XTC LDM: numeric syntax error in transfer address");
                UpdateReg(Registers.ic, u => (ushort) address);
                break;
            default:
                trace.Warn("XTC LDM: unimplemented command type '{0}' (ignored)", cmd);
                break;
            }
        }


        public override Program LoadProgram(Address? addrLoad) => throw new NotSupportedException();

        /* load file in TLD or XTC Load Module format */
        public override Program LoadProgram(
            Address addrLoad,
            IProcessorArchitecture arch, 
            IPlatform platform,
            List<UserSegment> userSegments)
        {
            linecnt = 0;
            var loadfile_type = TLD_LDM;
            this.simreg = arch.CreateProcessorState();
            using var mem = new MemoryStream(RawImage);
            using var loadfile = new StreamReader(mem, Encoding.ASCII);
            for (; ; )
            {
                string? lline = loadfile.ReadLine();
                if (lline is null)
                    break;
                ++linecnt;
                if (lline.Length < 2)
                    continue;
                this.load_ldmline[loadfile_type](lline);
            }
            DumpMap();
            return new Program(new ByteProgramMemory(MakeSegmentMap()), arch, platform);
        }

        private SegmentMap MakeSegmentMap()
        {
            var mems = new List<mem_t>();
            var segs = new List<ImageSegment>();
            uint? uAddr = null;
            for (int i = 0; i < this.mem.Length; ++i)
            {
                var mem = this.mem[i];
                if (mem is null)
                {
                    if (mems.Count > 0)
                    {
                        var seg = MakeSegment(uAddr!.Value, mems);
                        segs.Add(seg);
                        mems.Clear();
                        uAddr = null;
                    }
                }
                else
                {
                    if (!uAddr.HasValue)
                        uAddr = (uint) i * 0x1000u;
                    mems.Add(mem);
                }
            }
            if (mems.Count > 0)
            {
                var seg = MakeSegment(uAddr!.Value, mems);
                segs.Add(seg);
            }
            if (segs.Count == 0)
            {
                trace.Warn("No memory segments found.");
                return new SegmentMap(Address.Ptr16(0));
            }
            else
            {
                var addrBase = segs.Min(s => s.Address)!;
                return new SegmentMap(addrBase, segs.ToArray());
            }
        }

        private ImageSegment MakeSegment(uint uAddr, List<mem_t> mems)
        {
            var totalLength = mems.Sum(m => m.word.Length);
            var words = new ushort[totalLength];
            int offset = 0;
            foreach (var mem in mems)
            {
                Array.Copy(mem.word, 0, words, offset, mem.word.Length);
                offset += mem.word.Length;
            }
            var wmem = new Word16MemoryArea(Address.Ptr32(uAddr), words);
            var seg = new ImageSegment($"seg{uAddr:X6}", wmem, AccessMode.ReadWriteExecute);
            return seg;
        }

        private void DumpMap()
        {
            for (int i = 0; i < this.mem.Length; ++i)
            {
                Debug.Print("{0:X4}: mem: {0}", i, mem[i] is not null);
            }
        }


        /*
    int si_tld(string argv[])
    {
        string filename = argv[1];

        if (argv.Length <= 1)
            return error("filename missing");
        loadfile_type = TLD_LDM;
        if (*filename == '"')
        {
            *filename++ = '\0';
            *(filename + strlen(filename) - 1) = '\0';
        }

        return load_ldm(filename);
    }


    int si_xtc(string[] argv)
    {
        string filename = argv[1];

        if (argc <= 1)
            return error("filename missing");
        loadfile_type = XTC_LDM;
        if (*filename == '"')
        {
            *filename++ = '\0';
            *(filename + strlen(filename) - 1) = '\0';
        }

        return load_ldm(filename);
    }
        */

        /* Convert n-digit (1 <= n <= 8) hex string to number. */
        /* Returns -1 on error, else a number in the range 0 .. 0x7FFFFFFF. */
        /* Of course, this means that for (n = 8), the most significant bit
           (sign bit) can not be used, so the maximum readable bitwidth is 31. */
        private static int get_nibbles(string src, int pos, int n_nibbles)
        {
            int outnum = 0;

            while (n_nibbles-- > 0)
            {
                if (!TryParseHexDigit(src[pos++], out byte i))
                    return -1;
                outnum = (outnum << 4) | i;
            }
            return outnum;
        }

        private class mem_t
        {
            public readonly ushort[] word = new ushort[4096];
            public readonly uint[] was_written = new uint[128];
        }

        private void poke(uint phys_address, ushort value)
        {
            uint page = (uint) (phys_address >> 12);
            uint log_addr = (uint) (phys_address & 0x0FFF);
            mem_t memptr;

            if (page > 0xFF)
                throw new InvalidOperationException($"poke: absolute memory address {phys_address:X6} too large.");
            if ((memptr = mem[page]) is null)
            {
                trace.Verbose("poke: dynamically allocating page {0:X2}", page);
                memptr = mem[page] = new mem_t();
            }
            memptr.word[log_addr] = value;
            memptr.was_written[log_addr / 32] |= 1u << ((int)log_addr % 32);
        }

        private void UpdateReg(RegisterStorage reg, Func<ushort, ushort> fn)
        {
            var v = (Constant) simreg!.GetValue(reg);
            var vNew = fn(v.ToUInt16());
            simreg.SetRegister(reg, Constant.UInt16(vNew));
        }

        /* Each MMU page points to 4k words of memory.
           Indexing of a page is done by: pagereg[D/I][AS][logaddr_hinibble].ppa
           where D/I is the processsor Data/Instruction signal, AS is the
           Address State (least significant nibble of the Status Word), and
           logaddr_hinibble is the most significant nibble of the 16 bit logical
           address. The pagereg array, thus indexed, yields an 8-bit number, which
           becomes the most significant byte in the 20-bit physical address.
           The remaining bits of the physical address, i.e. the lower 12 bits,
           are a copy of the least significant 12 bits of the logical address.

           Default (startup) setting of pagereg[][][].ppa is done in
           init_cpu(). It can be changed in any desired way.
           By default, the MMU behaves just as though it were not there at all.
         */

        public struct Mmureg
        {
            public ushort ppa     ;/* : 8;*/
            public ushort reserved;/* : 3;*/
            public ushort e_w     ;/* : 1;*/
            public ushort al;      /* : 4;*/

            public ushort Value { get; set; }
        }
        readonly Mmureg [,,] pagereg = new Mmureg[2,16,16];

    }
}