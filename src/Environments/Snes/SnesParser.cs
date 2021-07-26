using Reko.Arch.Mos6502;
using Reko.Core;
using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.Snes
{
    //     Parse a SNES image. Valid extensions are smc, swc, fig.
    //     SNES header references and related source code:
    //     * http://romhack.wikia.com/wiki/SNES_header
    //     * http://softpixel.com/~cwright/sianse/docs/Snesrom.txt
    //     * initc.c of the ZSNES project:
    //     * http://zsnes.cvs.sourceforge.net/viewvc/zsnes/zsnes/src/initc.c?view=markup
    //     * memmap.cpp of the Snes9x project:
    //     * https://github.com/snes9xgit/snes9x/blob/master/memmap.cpp
    //     * sns_slot.c of the MAME project:
    //     * http://git.redump.net/mame/tree/src/mess/machine/sns_slot.c
    //       https://github.com/gocha/ida-snes-ldr
    // Ported from Python to C# using pytocs:
    // https://github.com/uxmal/pytocs
    public class SnesParser : ImageLoader
    {

        public const int FORMAT_LoROM = 0;
        public const int FORMAT_HiROM = 1;
        public const int FORMAT_SMALLFIRST = 0;
        public const int FORMAT_BIGFIRST = 1;

        public SnesParser(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
        {

        }

        public override Address PreferredBaseAddress
        {
            get
            {
                return Address.Ptr32(0x8000);
            }
            set
            {
                throw new NotImplementedException();
            }
        }




        public virtual List<string> getValidExtensions()
        {
            return new List<string> {
                    "smc",
                    "swc",
                    "fig"
                };
        }

        public virtual bool isValidData(byte[] data)
        {
            if (data.Length > 0)
            {
                if (this.hasSMCHeader(data))
                {
                    return true;
                }
                //$TODO: Need more conclusive tests
                return false;
            }
            return false;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            throw new NotImplementedException();
        }

        public override Program Load(Address? addrLoad)
        {
            var romdata = RawImage;
            bool bsHeader;
            bool bs;
            var props = new Dictionary<object, object> { };
            var forceInterleavedOff = false;
            while (true)
            {
                // Check for a header (512 bytes), and skip it if found
                var data = this.hasSMCHeader(romdata) ? romdata.Skip(512).ToArray() : romdata;
                var (hiScore, loScore, extendedFormat, headerOffsetRef) = this.findHiLoMode(data, forceInterleavedOff);
                // These two games fail to be detected (Source: Snes9x)
                int mapType;
                bool interleaved, tales;
                if (Encoding.ASCII.GetString(data, 32704, 22) == "YUYU NO QUIZ DE GO!GO!" ||
                    Encoding.ASCII.GetString(data, 65472, 21) == "BATMAN--REVENGE JOKER")
                {
                    (mapType, interleaved, tales) = (FORMAT_LoROM, false, false);
                }
                else
                {
                    (mapType, interleaved, tales) = this.findMemoryModel(data.Skip(headerOffsetRef).ToArray(), hiScore, loScore);
                }
                if (!forceInterleavedOff && interleaved)
                {
                    mapType = this.convertInterleaved(data, extendedFormat, mapType, tales);
                    // Modifying ROM, so we need to re-score
                    hiScore = this.scoreHiRom(data);
                    loScore = this.scoreLoRom(data);
                    if (mapType == FORMAT_HiROM && (loScore >= hiScore || hiScore < 0) || mapType == FORMAT_LoROM && (hiScore > loScore || loScore < 0))
                    {
                        // Game image lied about its type! Trying again...
                        forceInterleavedOff = true;
                        continue;
                    }
                }
                if (tales || extendedFormat == FORMAT_SMALLFIRST)
                {
                    // Fix swapped ExHiROM
                    var tmp = new byte[data.Length - 0x400000];
                    var tmp2 = new byte[0x400000];
                    Array.Copy(data, 0, tmp, 0, tmp.Length);
                    Array.Copy(data, tmp.Length, tmp2, 0, tmp2.Length);
                    Array.Copy(tmp2, 0, data, 0, tmp2.Length);
                    Array.Copy(tmp, 0, data, tmp2.Length, tmp.Length);
                }
                if (Encoding.ASCII.GetString(data, 32704, 21) == "Satellaview BS-X     ")
                {
                    bs = true;
                    bsHeader = false;
                    mapType = FORMAT_LoROM;
                }
                else
                {
                    var bLo = this.isBSX(data, 32704, 27) == 1;
                    var bHi = this.isBSX(data, 65472, 27) == 1;
                    bs = bLo || bHi;
                    bsHeader = bs;
                    if (bs)
                    {
                        mapType = bLo ? FORMAT_LoROM : FORMAT_HiROM;
                    }
                }
                // Re-calculate the header offset (include extended header, 0x10
                // bytes before the actual 64-byte SNES header starts)
                // See http://romhack.wikia.com/wiki/SNES_header#Extended_header_.28bytes_.24ffb2...24ffb5.29
                var headerOffset = 32688;
                if (extendedFormat == FORMAT_BIGFIRST)
                {
                    headerOffset += 4194304;
                }
                if (mapType == FORMAT_HiROM)
                {
                    headerOffset += 0x8000;
                }
                var header = headerOffset;
                // Instead of branching on bsHeader, simply apply the different
                // values to the ROM data and use the same code below to set props
                if (bsHeader)
                {
                    // The BS game's SRAM was not found
                    // Only use the first 16 of 21 title characters
                    //$TODO data[header+16 + 16, 16 + 21] = "     ";
                    // Rom speed flag uses 0x28 (RAM size?) instead of 0x25
                    data[header + 37] = data[header + 40];
                    // Cartridge type is specific to Satellaview BS-X
                    data[header + 38] = 229;
                    // Rom size (Is this correct?)
                    var p = 0;
                    while (1 << p < data.Length)
                    {
                        p += 1;
                    }
                    data[header + 39] = (byte) Math.Max(p - 10, 8);
                    // Ram size is 256 Kbit (standard for most copiers according to Snesrom.txt)
                    data[header + 40] = 5;
                    // Region is hard-coded to Japan apparently
                    data[header + 41] = 0;
                }
                // Remember, addresses start at 000, but extended headers are padded by 0x10 bytes
                // See http://romhack.wikia.com/wiki/SNES_header
                // 000-014 - Title, UPPER CASE ASCII
                props["title"] = this._sanitize(data, header + 16, 21);
                // Game code - part of the extended header, not always present
                props["code"] = this._sanitize(data, header + 2, 4);
                // 015 - ROM layout and ROM speed, a bitwise-or of these flags:
                //       0x20 is always set
                //       0x10 is set when using FastROM
                //       0x01 is set for HiROM or cleared for LoROM
                var HiROM = extendedFormat != 0 ? "ExHiROM" : "HiROM";
                props["memory_layout"] = mapType == FORMAT_HiROM ? HiROM : "LoROM";
                props["rom_speed"] = (data[header + 37] & 16) != 0 ? "FastROM" : "SlowROM";
                // 016 - Cartridge type, values greater than 0x02 indicate special add-on hardware in the cartridge
                props["cartridge_type"] = this.getCartridgeType(data, header, bs);
                // 017 - ROM size: 1 << (ROM_SIZE - 7) Mbits, range is 8..12 (256KB..4MB, 2Mb..32Mb)
                var b = data[header + 39];
                props["rom_size"] = 8 <= b && b <= 12 ? String.Format("%d Mbit", 1 << b - 7) : "";
                // 018 - RAM size: 1 << (3 + SRAM_BYTE) Kbits, range is 0..5 (0..32 kilobytes, 0..256 kbit)
                props["ram_size"] = data[header + 40] <= 5 ? String.Format("%d Kbit", 1 << 3 + data[header + 40]) : "";
                // 019 - Country code, video region
                props["region"] = snes_regions.TryGetValue(data[header + 41], out var region) ? region : "";
                props["video_output"] = new List<int> {
                        0,
                        1,
                        13
                    }.Contains(data[header + 41]) ? "NTSC" : data[header + 41] < 13 ? "PAL" : "";
                // 01A - Licensee code 0x33 implies an extended header at bytes ffb0..ffbf
                var company = this.getCompanyCode(data, header);
                props["publisher"] = snes_publishers.TryGetValue(company, out var pub) ? pub : "";
                props["publisher_code"] = company != -1 ? String.Format("%04X", company) : "";
                // 01B - Version, typically contains 0x00. Most ROM hackers never touch this
                //       byte, so multiple versions of a ROM hack may share the same value
                props["version"] = String.Format("%02X", data[header + 43]);
                // 01C-01F - Checksum complement and checksum, respectively. The checksum is
                //           the unsigned little-endian 16-bit sum of the values of the bytes
                //           in the ROM. If the size of the ROM is not a power of 2, then some
                //           bytes may enter the sum multiple times through mirroring. The
                //           checksum complement is the bitwise-xor of the checksum with 0xFFFF.
                props["checksum"] = String.Format("{0:X4}", data[header + 46] + (data[header + 47] << 8));
                props["checksum_complement"] = String.Format("{0:X4}", data[header + 44] + (data[header + 45] << 8));

                var arch = new Mos65816Architecture(Services, "m65816", new Dictionary<string, object>());
                return new Program
                {
                    Platform = new SnesPlatform(Services, arch, "snes"),
                    Architecture =  arch,
                    SegmentMap = new SegmentMap(Address.Ptr32(0x8000))
                };
            }
        }

        // 
        //         Check for a 512-byte SMC, SWC or FIG header prepended to the beginning
        //         of the file.
        //         
        public virtual bool hasSMCHeader(byte[] data)
        {
            if (512 <= data.Length)
            {
                if (data[8] == 170 && data[9] == 187 && data[10] == 4)
                {
                    // Found an SMC/SWC identifier (Source: MAME and ZSNES)
                    return true;
                }
                if (new List<(int, int)> {
                        (0, 128),
                        (17, 2),
                        (71, 131),
                        (119, 131),
                        (221, 2),
                        (221, 130),
                        (247, 131),
                        (253, 130)
                    }.Contains((data[4], data[5])))
                {
                    // Found a FIG header (Source: ZSNES)
                    return true;
                }
                if ((data[1] << 8 | data[0]) == data.Length - 512 >> 13)
                {
                    // Some headers have the rom size at the start, if this matches with
                    // the actual rom size, we probably have a header (Source: MAME)
                    return true;
                }
                if (data.Length % 0x8000 == 512)
                {
                    // As a last check we'll see if there's exactly 512 bytes extra
                    // to this image. MAME takes len modulus 0x8000 (32kb), Snes9x
                    // uses len / 0x2000 (8kb) * 0x2000.
                    return true;
                }
            }
            return false;
        }

        // 
        //         Swap blocks in a range of ROM memory.
        //         
        public virtual void deinterleaveType1(byte[] data, int size)
        {
            var nbanks = size >> 15;
            var nblocks = nbanks >> 2;
            var blocks = (from i in Enumerable.Range(0, nblocks * 2)
                          select i % 2 == 0 ? (i >> 1) + nblocks : i >> 1).ToList();
            foreach (var i in Enumerable.Range(0, nblocks * 2))
            {
                foreach (var j in Enumerable.Range(0, nblocks * 2))
                {
                    if (blocks[j] == i)
                    {
                        var tmp = new byte[0x8000];
                        Array.Copy(data, blocks[j] * 0x8000, tmp, 0, tmp.Length);
                        Array.Copy(data, blocks[i] * 0x8000, data, blocks[j] * 0x8000, tmp.Length);
                        Array.Copy(tmp, 0, data, blocks[i] * 0x8000, tmp.Length);
                        var b = blocks[j];
                        blocks[j] = blocks[i];
                        blocks[i] = b;
                        break;
                    }
                }
            }
        }

        public virtual (int, int, int, int) findHiLoMode(byte[] data, bool forceInterleavedOff)
        {
            var hiScore = this.scoreHiRom(data);
            var loScore = this.scoreLoRom(data);
            var extendedFormat = 0;
            var headerOffsetRef = 0;
            if (data.Length > 4194304 && !new List<int> {
                    13347,
                    13603,
                    17202,
                    17714
                }.Contains(data[32725] + (data[32726] << 8)) && !new List<int> {
                    63802,
                    62778
                }.Contains(data[65493] + (data[65494] << 8)))
            {
                var swappedHiRom = this.scoreHiRom(data, 4194304);
                var swappedLoRom = this.scoreLoRom(data, 4194304);
                if (Math.Max(swappedLoRom, swappedHiRom) >= Math.Max(loScore, hiScore))
                {
                    extendedFormat = FORMAT_BIGFIRST;
                    hiScore = swappedHiRom;
                    loScore = swappedLoRom;
                    headerOffsetRef = 4194304;
                }
                else
                {
                    extendedFormat = FORMAT_SMALLFIRST;
                }
            }
            else if (data[32764] + (data[32765] << 8) < 0x8000 && data[65532] + (data[65533] << 8) < 0x8000 && !forceInterleavedOff)
            {
                // If both vectors are invalid, it's type 1 interleaved LoROM
                this.deinterleaveType1(data, data.Length);
                // Modifying ROM, so we need to re-score
                hiScore = this.scoreHiRom(data);
                loScore = this.scoreLoRom(data);
            }
            return (hiScore, loScore, extendedFormat, headerOffsetRef);
        }

        // 
        //         Determine if the ROM is a LoROM Memory Model (32k Banks) or HiROM
        //         Memory Model (64k Banks).
        //         
        public virtual (int, bool, bool) findMemoryModel(byte[] data, int hiScore, int loScore)
        {
            int mapType = -1;
            var interleaved = false;
            var tales = false;
            if (loScore >= hiScore)
            {
                mapType = FORMAT_LoROM;
                // Ignore map type byte if not 0x2x or 0x3x
                if (new List<int> {
                        32,
                        48
                    }.Contains(data[32725] & 240))
                {
                    if ((data[32725] & 15) == 1)
                    {
                        interleaved = true;
                    }
                    else if ((data[32725] & 15) == 5)
                    {
                        interleaved = true;
                        tales = true;
                    }
                }
            }
            else
            {
                mapType = FORMAT_HiROM;
                if (new List<int> {
                        32,
                        48
                    }.Contains(data[65493] & 240))
                {
                    if (new List<int> {
                            0,
                            3
                        }.Contains(data[65493] & 15))
                    {
                        interleaved = true;
                    }
                }
            }
            return (mapType, interleaved, tales);
        }

        public virtual int convertInterleaved(byte[] data, int extendedFormat, int oldMapType, bool tales)
        {
            byte[] tmpdata;
            // ROM image is in interleaved format, converting...
            if (tales)
            {
                if (extendedFormat == FORMAT_BIGFIRST)
                {
                    this.deinterleaveType1(data, 0x400000);
                    tmpdata = new byte[data.Length - 0x400000];
                    Array.Copy(data, 0x400000, tmpdata, 0, tmpdata.Length);
                    this.deinterleaveType1(tmpdata, data.Length);
                    Array.Copy(tmpdata, 0, data, 0x400000, tmpdata.Length);
                }
                else
                {
                    this.deinterleaveType1(data, data.Length - 0x400000);
                    tmpdata = new byte[0x400000];
                    Array.Copy(data, data.Length - 0x400000, tmpdata, 0, tmpdata.Length);
                    this.deinterleaveType1(tmpdata, 0x400000);
                    Array.Copy(tmpdata, 0, data, data.Length - 0x400000, tmpdata.Length);
                }
                return FORMAT_HiROM;
            }
            else
            {
                // Swap memory models
                this.deinterleaveType1(data, data.Length);
                return oldMapType == FORMAT_HiROM ? FORMAT_LoROM : FORMAT_HiROM;
            }
        }

        public virtual int scoreHiRom(byte[] data, int offset = 0)
        {
            var size = data.Length;
            offset = 65280 + offset;
            var score = 0;
            if (data[offset + 212] == 32)
            {
                score += 2;
            }
            if ((data[offset + 213] & 1) != 0)
            {
                score += 2;
            }
            // Mode23 is SA-1
            if (data[offset+213] == 35)
            {
                score -= 2;
            }
            if ((data[offset+213] & 15) < 4)
            {
                score += 2;
            }
            if (1 << Math.Max(data[offset+215] - 7, 0) > 48)
            {
                score -= 1;
            }
            if (data[offset+218] == 51)
            {
                score += 2;
            }
            if (data[offset+220] + (data[offset+221] << 8) + data[offset+222] + (data[offset+223] << 8) == 65535)
            {
                score += 2;
                if (data[offset+222] + (data[offset+223] << 8) != 0)
                {
                    score += 1;
                }
            }
            if (data[offset+252] + (data[offset+253] << 8) > 65456)
            {
                score -= 2;
            }
            if ((data[offset+253] & 128) == 0)
            {
                score -= 6;
            }
            if (!this._allASCII(data, offset +176, 6))
            {
                score -= 1;
            }
            if (!this._allASCII(data, offset+192, 22))
            {
                score -= 1;
            }
            if (size > 1024 * 1024 * 3)
            {
                score += 4;
            }
            return score;
        }

        public virtual int scoreLoRom(byte[] data, int offset = 0)
        {
            var size = data.Length;
            offset = 32512 + offset;
            var score = 0;
            if ((data[offset + 213] & 1) == 0)
            {
                score += 3;
            }
            // Mode23 is SA-1
            if (data[offset + 213] == 35)
            {
                score += 2;
            }
            if ((data[offset + 213] & 15) < 4)
            {
                score += 2;
            }
            if (1 << Math.Max(data[offset + 215] - 7, 0) > 48)
            {
                score -= 1;
            }
            if (data[offset + 218] == 51)
            {
                score += 2;
            }
            if (data[offset + 220] + (data[offset + 221] << 8) + data[offset + 222] + (data[offset + 223] << 8) == 65535)
            {
                score += 2;
                if (data[offset + 222] + (data[offset + 223] << 8) != 0)
                {
                    score += 1;
                }
            }
            if (data[offset + 252] + (data[offset + 253] << 8) > 65456)
            {
                score -= 2;
            }
            if ((data[offset + 253] & 128) == 0)
            {
                score -= 6;
            }
            if (!this._allASCII(data, 176, 6))
            {
                score -= 1;
            }
            if (!this._allASCII(data, 192, 22))
            {
                score -= 1;
            }
            if (size <= 1024 * 1024 * 16)
            {
                score += 2;
            }
            return score;
        }

        //
        //         Only need the first 0x1B (27) bytes of data to test for BS-X BIOSes.
        //         
        public virtual int isBSX(byte[] data, int off, int len)
        {
            if ((data[off + 21] == 0 || (data[off + 21] & 131) == 80) && new List<int> {
                    32,
                    33,
                    48,
                    49
                }.Contains(data[off + 24]) && new List<int> {
                    51,
                    255
                }.Contains(data[off + 26]))
            {
                if (data[off + 22] == 0 && data[off + 23] == 0)
                {
                    return 2;
                }
                if (data[off + 22] == 255 && data[off + 23] == 255 || (data[off + 22] & 15) == 0 && data[off + 22] >> 4 < 13)
                {
                    return 1;
                }
            }
            return 0;
        }

        public virtual string getCartridgeType(byte[] data, int header, bool bs)
        {
            var romSpeed = data[header + 37];
            var romType = data[header + 38];
            string kart;
            if (romType == 0 && !bs)
            {
                kart = "ROM";
            }
            else
            {
                var identifier = ((romType & 255) << 8) + (romSpeed & 255);
                var contents = new List<string> {
                        "ROM",
                        "ROM+RAM",
                        "ROM+RAM+BATT"
                    };
                var chip = "";
                if (bs)
                {
                    chip = "BS";
                }
                else if (new List<int> {
                        4896,
                        5152,
                        5408,
                        6688
                    }.Contains(identifier))
                {
                    chip = "SuperFX";
                    // Set the SRAM size
                    if (data[header + 42] == 51)
                    {
                        data[header + 40] = data[header + 43];
                    }
                    else
                    {
                        data[header + 40] = 5;
                    }
                    data[header + 40] = 2;
                }
                else if (new List<int> {
                        17202,
                        17714
                    }.Contains(identifier))
                {
                    chip = "SDD1";
                }
                else if (identifier == 9520)
                {
                    chip = "OBC1";
                }
                else if (new List<int> {
                        13347,
                        13603
                    }.Contains(identifier))
                {
                    chip = "SA1";
                }
                else if (identifier == 63802)
                {
                    chip = "SPC7110+RTC";
                }
                else if (identifier == 62778)
                {
                    chip = "SPC7110";
                }
                else if (identifier == 21813)
                {
                    chip = "SRTC";
                }
                else if (identifier == 62240)
                {
                    chip = "C4";
                }
                else if (identifier == 63024)
                {
                    // data[header+0x27] == 0x09 is ROM Size of 4 Mbit
                    chip = data[header + 39] == 9 ? "ST-011" : "ST-010";
                }
                else if (identifier == 62768)
                {
                    chip = "ST-018";
                    // Set the SRAM  size to 32 Kbit
                    data[header + 40] = 2;
                }
                else if (romType == 3)
                {
                    if (romSpeed == 48)
                    {
                        chip = "DSP-4";
                    }
                    else
                    {
                        chip = "DSP-1";
                    }
                }
                else if (romType == 5)
                {
                    if (romSpeed == 32)
                    {
                        chip = "DSP-2";
                    }
                    else if (romSpeed == 48 && data[header + 42] == 178)
                    {
                        chip = "DSP-3";
                    }
                    else
                    {
                        chip = "DSP-1";
                    }
                }
                if (!string.IsNullOrEmpty(chip))
                {
                    kart = String.Format("{0}+{1}", contents[(romType & 15) % 3], chip);
                }
                else
                {
                    kart = contents[(romType & 15) % 3];
                }
            }
            return kart;
        }

        public virtual int getCompanyCode(byte[] data, int header)
        {
            var companyCode = -1;
            if (data[header + 42] != 51)
            {
                companyCode = (data[header + 42] >> 4 & 15) * 36 + (data[header + 42] & 15);
            }
            else
            {
                var l = Char.ToUpper((char) data[header + 0]);
                var r = Char.ToUpper((char) data[header + 1]);
                var l2 = l > '9' ? l - '7' : l - '0';
                var r2 = r > '9' ? r - '7' : r - '0';
                companyCode = l2 >= 0 && r2 >= 0 ? l2 * 36 + r2 : -1;
            }
            return companyCode;
        }

        // Turn all non-ASCII characters into spaces(tab, CR and LF line breaks
        // are OK to preserve formatting), and then return a stripped string.
        private string _sanitize(byte[] title, int off, int len)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < len; ++i)
            {
                var c = title[i + off];
                sb.Append(
                    ((char) c == '\t' ||
                     (char) c == '\n' ||
                     (char) c == '\r' ||
                             (0x20 <= (int) c && (int) c <= 0x7E))
                        ? (char) c
                        : ' ');
            }
            return sb.ToString();
        }


        private bool _allASCII(byte[] data, int off, int len)
        {
            for (int i = 0; i < len; ++i)
            {
                var b = data[off + i];
                if (!(0x20 <= b && b <= 0x7E))
                    return false;
            }
            return true;
        }

        public static Dictionary<int, string> snes_regions = new Dictionary<int, string> {
            { 0, "Japan"},
            { 1, "USA/Canada"},
            { 2, "Europe/Asia/Oceania"},
            { 3, "Sweden"},
            { 4, "Finland"},
            { 5, "Denmark"},
            { 6, "France"},
            { 7, "Holland"},
            { 8, "Spain"},
            { 9, "Germany/Austria/Switzerland"},
            { 10, "Italy"},
            { 11, "Hong Kong/China"},
            { 12, "Indonesia"},
            { 13, "South Korea"}};

        public static Dictionary<int, string> snes_publishers = new Dictionary<int, string> {
            { 1, "Nintendo"},
            { 2, "Rocket Games/Ajinomoto"},
            { 3, "Imagineer-Zoom"},
            { 4, "Gray Matter"},
            { 5, "Zamuse"},
            { 6, "Falcom"},
            { 8, "Capcom"},
            { 9, "Hot B Co."},
            { 10, "Jaleco"},
            { 11, "Coconuts Japan"},
            { 12, "Coconuts Japan/G.X.Media"},
            { 13, "Micronet"},
            { 14, "Technos"},
            { 15, "Mebio Software"},
            { 16, "Shouei System"},
            { 17, "Starfish"},
            { 19, "Mitsui Fudosan/Dentsu"},
            { 21, "Warashi Inc."},
            { 23, "Nowpro"},
            { 25, "Game Village"},
            { 26, "IE Institute"},
            { 36, "Banarex"},
            { 37, "Starfish"},
            { 38, "Infocom"},
            { 39, "Electronic Arts Japan"},
            { 41, "Cobra Team"},
            { 42, "Human/Field"},
            { 43, "KOEI"},
            { 44, "Hudson Soft"},
            { 45, "S.C.P./Game Village"},
            { 46, "Yanoman"},
            { 48, "Tecmo Products"},
            { 49, "Japan Glary Business"},
            { 50, "Forum/OpenSystem"},
            { 51, "Virgin Games (Japan)"},
            { 52, "SMDE"},
            { 53, "Yojigen"},
            { 55, "Daikokudenki"},
            { 61, "Creatures Inc."},
            { 62, "TDK Deep Impresion"},
            { 72, "Destination Software/KSS"},
            { 73, "Sunsoft/Tokai Engineering"},
            { 74, "POW (Planning Office Wada)/VR 1 Japan"},
            { 75, "Micro World"},
            { 77, "San-X"},
            { 78, "Enix"},
            { 79, "Loriciel/Electro Brain"},
            { 80, "Kemco Japan"},
            { 81, "Seta Co.,Ltd."},
            { 82, "Culture Brain"},
            { 83, "Irem Corp."},
            { 84, "Palsoft"},
            { 85, "Visit Co., Ltd."},
            { 86, "Intec"},
            { 87, "System Sacom"},
            { 88, "Poppo"},
            { 89, "Ubisoft Japan"},
            { 91, "Media Works"},
            { 92, "NEC InterChannel"},
            { 93, "Tam"},
            { 94, "Gajin/Jordan"},
            { 95, "Smilesoft"},
            { 98, "Mediakite"},
            { 108, "Viacom"},
            { 109, "Carrozzeria"},
            { 110, "Dynamic"},
            { 112, "Magifact"},
            { 113, "Hect"},
            { 114, "Codemasters"},
            { 115, "Taito/GAGA Communications"},
            { 116, "Laguna"},
            { 117, "Telstar Fun & Games/Event/Taito"},
            { 119, "Arcade Zone Ltd."},
            { 120, "Entertainment International/Empire Software"},
            { 121, "Loriciel"},
            { 122, "Gremlin Graphics"},
            { 144, "Seika Corp."},
            { 145, "UBI SOFT Entertainment Software"},
            { 146, "Sunsoft US"},
            { 148, "Life Fitness"},
            { 150, "System 3"},
            { 151, "Spectrum Holobyte"},
            { 153, "Irem"},
            { 155, "Raya Systems"},
            { 156, "Renovation Products"},
            { 157, "Malibu Games"},
            { 159, "Eidos/U.S. Gold"},
            { 160, "Playmates Interactive"},
            { 163, "Fox Interactive"},
            { 164, "Time Warner Interactive"},
            { 170, "Disney Interactive"},
            { 172, "Black Pearl"},
            { 174, "Advanced Productions"},
            { 177, "GT Interactive"},
            { 178, "RARE"},
            { 179, "Crave Entertainment"},
            { 180, "Absolute Entertainment"},
            { 181, "Acclaim"},
            { 182, "Activision"},
            { 183, "American Sammy"},
            { 184, "Take 2/GameTek"},
            { 185, "Hi Tech"},
            { 186, "LJN Ltd."},
            { 188, "Mattel"},
            { 190, "Mindscape/Red Orb Entertainment"},
            { 191, "Romstar"},
            { 192, "Taxan"},
            { 193, "Midway/Tradewest"},
            { 195, "American Softworks Corp."},
            { 196, "Majesco Sales Inc."},
            { 197, "3DO"},
            { 200, "Hasbro"},
            { 201, "NewKidCo"},
            { 202, "Telegames"},
            { 203, "Metro3D"},
            { 205, "Vatical Entertainment"},
            { 206, "LEGO Media"},
            { 208, "Xicat Interactive"},
            { 209, "Cryo Interactive"},
            { 212, "Red Storm Entertainment"},
            { 213, "Microids"},
            { 215, "Conspiracy/Swing"},
            { 216, "Titus"},
            { 217, "Virgin Interactive"},
            { 218, "Maxis"},
            { 220, "LucasArts Entertainment"},
            { 223, "Ocean"},
            { 225, "Electronic Arts"},
            { 227, "Laser Beam"},
            { 230, "Elite Systems"},
            { 231, "Electro Brain"},
            { 232, "The Learning Company"},
            { 233, "BBC"},
            { 235, "Software 2000"},
            { 237, "BAM! Entertainment"},
            { 238, "Studio 3"},
            { 242, "Classified Games"},
            { 244, "TDK Mediactive"},
            { 246, "DreamCatcher"},
            { 247, "JoWood Produtions"},
            { 248, "SEGA"},
            { 249, "Wannado Edition"},
            { 250, "LSP (Light & Shadow Prod.)"},
            { 251, "ITE Media"},
            { 252, "Infogrames"},
            { 253, "Interplay"},
            { 254, "JVC (US)"},
            { 255, "Parker Brothers"},
            { 257, "SCI (Sales Curve Interactive)/Storm"},
            { 260, "THQ Software"},
            { 261, "Accolade Inc."},
            { 262, "Triffix Entertainment"},
            { 264, "Microprose Software"},
            { 265, "Universal Interactive/Sierra/Simon & Schuster"},
            { 267, "Kemco"},
            { 268, "Rage Software"},
            { 269, "Encore"},
            { 271, "Zoo"},
            { 272, "Kiddinx"},
            { 273, "Simon & Schuster Interactive"},
            { 274, "Asmik Ace Entertainment Inc./AIA"},
            { 275, "Empire Interactive"},
            { 278, "Jester Interactive"},
            { 280, "Rockstar Games"},
            { 281, "Scholastic"},
            { 282, "Ignition Entertainment"},
            { 283, "Summitsoft"},
            { 284, "Stadlbauer"},
            { 288, "Misawa"},
            { 289, "Teichiku"},
            { 290, "Namco Ltd."},
            { 291, "LOZC"},
            { 292, "KOEI"},
            { 294, "Tokuma Shoten Intermedia"},
            { 295, "Tsukuda Original"},
            { 296, "DATAM-Polystar"},
            { 299, "Bullet-Proof Software"},
            { 300, "Vic Tokai Inc."},
            { 302, "Character Soft"},
            { 303, "I'Max"},
            { 304, "Saurus"},
            { 307, "General Entertainment"},
            { 310, "I'Max"},
            { 311, "Success"},
            { 313, "SEGA Japan"},
            { 324, "Takara"},
            { 325, "Chun Soft"},
            { 326, "Video System Co., Ltd./McO'River"},
            { 327, "BEC"},
            { 329, "Varie"},
            { 330, "Yonezawa/S'pal"},
            { 331, "Kaneko"},
            { 333, "Victor Interactive Software/Pack-in-Video"},
            { 334, "Nichibutsu/Nihon Bussan"},
            { 335, "Tecmo"},
            { 336, "Imagineer"},
            { 339, "Nova"},
            { 340, "Den'Z"},
            { 341, "Bottom Up"},
            { 343, "TGL (Technical Group Laboratory)"},
            { 345, "Hasbro Japan"},
            { 347, "Marvelous Entertainment"},
            { 349, "Keynet Inc."},
            { 350, "Hands-On Entertainment"},
            { 360, "Telenet"},
            { 361, "Hori"},
            { 364, "Konami"},
            { 365, "K.Amusement Leasing Co."},
            { 366, "Kawada"},
            { 367, "Takara"},
            { 369, "Technos Japan Corp."},
            { 370, "JVC (Europe/Japan)/Victor Musical Industries"},
            { 372, "Toei Animation"},
            { 373, "Toho"},
            { 375, "Namco"},
            { 376, "Media Rings Corp."},
            { 377, "J-Wing"},
            { 379, "Pioneer LDC"},
            { 380, "KID"},
            { 381, "Mediafactory"},
            { 385, "Infogrames Hudson"},
            { 396, "Acclaim Japan"},
            { 397, "ASCII Co./Nexoft"},
            { 398, "Bandai"},
            { 400, "Enix"},
            { 402, "HAL Laboratory/Halken"},
            { 403, "SNK"},
            { 405, "Pony Canyon Hanbai"},
            { 406, "Culture Brain"},
            { 407, "Sunsoft"},
            { 408, "Toshiba EMI"},
            { 409, "Sony Imagesoft"},
            { 411, "Sammy"},
            { 412, "Magical"},
            { 413, "Visco"},
            { 415, "Compile"},
            { 417, "MTO Inc."},
            { 419, "Sunrise Interactive"},
            { 421, "Global A Entertainment"},
            { 422, "Fuuki"},
            { 432, "Taito"},
            { 434, "Kemco"},
            { 435, "Square"},
            { 436, "Tokuma Shoten"},
            { 437, "Data East"},
            { 438, "Tonkin House"},
            { 440, "KOEI"},
            { 442, "Konami/Ultra/Palcom"},
            { 443, "NTVIC/VAP"},
            { 444, "Use Co., Ltd."},
            { 445, "Meldac"},
            { 446, "Pony Canyon (Japan)/FCI (US)"},
            { 447, "Angel/Sotsu Agency/Sunrise"},
            { 448, "Yumedia/Aroma Co., Ltd."},
            { 451, "Boss"},
            { 452, "Axela/Crea-Tech"},
            { 453, "Sekaibunka-Sha/Sumire kobo/Marigul Management Inc."},
            { 454, "Konami Computer Entertainment Osaka"},
            { 457, "Enterbrain"},
            { 468, "Taito/Disco"},
            { 469, "Sofel"},
            { 470, "Quest Corp."},
            { 471, "Sigma"},
            { 472, "Ask Kodansha"},
            { 474, "Naxat"},
            { 475, "Copya System"},
            { 476, "Capcom Co., Ltd."},
            { 477, "Banpresto"},
            { 478, "TOMY"},
            { 479, "Acclaim/LJN Japan"},
            { 481, "NCS"},
            { 482, "Human Entertainment"},
            { 483, "Altron"},
            { 484, "Jaleco"},
            { 485, "Gaps Inc."},
            { 491, "Elf"},
            { 504, "Jaleco"},
            { 506, "Yutaka"},
            { 507, "Varie"},
            { 508, "T&ESoft"},
            { 509, "Epoch Co., Ltd."},
            { 511, "Athena"},
            { 512, "Asmik"},
            { 513, "Natsume"},
            { 514, "King Records"},
            { 515, "Atlus"},
            { 516, "Epic/Sony Records (Japan)"},
            { 518, "IGS (Information Global Service)"},
            { 520, "Chatnoir"},
            { 521, "Right Stuff"},
            { 523, "NTT COMWARE"},
            { 525, "Spike"},
            { 526, "Konami Computer Entertainment Tokyo"},
            { 527, "Alphadream Corp."},
            { 529, "Sting"},
            { 540, "A Wave"},
            { 541, "Motown Software"},
            { 542, "Left Field Entertainment"},
            { 543, "Extreme Entertainment Group"},
            { 544, "TecMagik"},
            { 549, "Cybersoft"},
            { 551, "Psygnosis"},
            { 554, "Davidson/Western Tech."},
            { 555, "Unlicensed"},
            { 560, "The Game Factory Europe"},
            { 561, "Hip Games"},
            { 562, "Aspyr"},
            { 565, "Mastiff"},
            { 566, "iQue"},
            { 567, "Digital Tainment Pool"},
            { 568, "XS Games"},
            { 569, "Daiwon"},
            { 577, "PCCW Japan"},
            { 580, "KiKi Co. Ltd."},
            { 581, "Open Sesame Inc."},
            { 582, "Sims"},
            { 583, "Broccoli"},
            { 584, "Avex"},
            { 585, "D3 Publisher"},
            { 587, "Konami Computer Entertainment Japan"},
            { 589, "Square-Enix"},
            { 590, "KSG"},
            { 591, "Micott & Basara Inc."},
            { 593, "Orbital Media"},
            { 610, "The Game Factory USA"},
            { 613, "Treasure"},
            { 614, "Aruze"},
            { 615, "Ertain"},
            { 616, "SNK Playmore"},
            { 665, "Yojigen"}};
    }
}