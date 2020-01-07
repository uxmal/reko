//!/usr/bin/env python3
//
// Symbolik: parser for Microsoft CodeView debug symbols in MZ EXEs
// Phil Pemberton, 2019
//
// Works with CodeView version NB00, which is produced by
//   Microsoft LINK version 3.x, 4.x, 5.0x (up to 5.03), pre-September 1989
//
// Similar formats include -
//   NB01 - MS LINK v5.05 (BASIC PDS 7.0)
//   NB02 - MS LINK v5.10 (Microsoft C 6.0)
//
// References -
//   Microsoft C 6.0 Developer's Toolkit Reference Manual, chapter 3
//     "Extended .EXE Format for Debug Information"
//
// 

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.MzExe.CodeView
{
    public class CodeViewLoader
    {
        private readonly IProcessorArchitecture arch;
        private readonly byte[] rawImage;
        private readonly Address addrLoad;
        private List<object> subsections;

        public CodeViewLoader(IProcessorArchitecture arch, byte[] rawImage, Address addrLoad)
        {
            this.arch = arch;
            this.rawImage = rawImage;
            this.addrLoad = addrLoad;
        }

        // ----------------------------------------
        // Subsection type
        // ----------------------------------------
        public enum SST
        {
            SST_MODULES = 0x101,
            SST_PUBLICS = 0x102,
            SST_TYPE = 0x103,
            SST_SYMBOLS = 0x104,
            SST_SRCLINES = 0x105,
            SST_LIBRARIES = 0x106,
            SST_COMPACTED = 0x108,
            SST_SRCLNSEG = 0x109,
        }

        // ----------------------------------------
        // Subsection -- base class for all Codeview subsections
        // ----------------------------------------
        public class Subsection
        {
            public byte[] data;
            public object module;
            public SST sst;

            public Subsection(SST sst, object module, byte[] data)
            {
                this.sst = sst;
                this.module = module;
                this.data = data;
            }

            public override string ToString()
            {
                return String.Format("<Subsection sst={0}(0x{1:X}), module={2}, len(data)={3}>", this.sst, (int) this.sst, this.module, this.data.Length);
            }
        }

        // ----------------------------------------
        // sstModules -- module definition
        // ----------------------------------------
        public class sstModules
            : Subsection
        {

            private ushort csBase;
            private ushort csOfs;
            private ushort csLen;
            private ushort ovl;
            private ushort libIndx;
            private byte nsegs;
            public string @string;

            public sstModules(SST sst, object module, byte[] data)
                : base(sst, module, data)
            {
                var _tup_1 = new LeImageReader(data);
                // @struct.unpack_from("<HHHHHBBB", data);
                this.csBase = _tup_1.ReadLeUInt16();
                this.csOfs = _tup_1.ReadLeUInt16();
                this.csLen = _tup_1.ReadLeUInt16();
                this.ovl = _tup_1.ReadLeUInt16();
                this.libIndx = _tup_1.ReadLeUInt16();
                this.nsegs = _tup_1.ReadByte();
                _tup_1.ReadByte();
                var strlen = _tup_1.ReadByte();
                this.@string = Encoding.ASCII.GetString(data, data.Length - strlen, strlen);
            }

            public virtual object @__repr__()
            {
                return String.Format("<sstModules cs(base=0x%X, ofs=0x%X, len=%d), ovl=%d, libIndx=%X, nsegs=%d, str=\'%s\'>", this.csBase, this.csOfs, this.csLen, this.ovl, this.libIndx, this.nsegs, this.@string);
            }
        }

        public class PublicSymbol
        {

            public string name;
            public ushort offset;
            public ushort segment;
            public uint typeidx;

            public PublicSymbol(ushort offset, ushort segment, uint typeidx, string name)
            {
                this.offset = offset;
                this.segment = segment;
                this.typeidx = typeidx;
                this.name = name;
            }

            public override string ToString()
            {
                return String.Format("<PublicSymbol \'{0}\', seg 0x{1:X4} ofs 0x{2:X4} type {3}>", this.name, this.segment, this.offset, this.typeidx);
            }
        }

        // ----------------------------------------
        // sstPublics
        // ----------------------------------------
        public class sstPublics
            : Subsection
        {

            public List<PublicSymbol> symbols;

            public sstPublics(SST sst, object module, byte[] data)
                : base(sst, module, data)
            {
                //using (var fo = open("pub", "wb")) {
                //    fo.write(data);
                //}
                var syms = new List<PublicSymbol>();

                var rdr = new LeImageReader(data);
                while (rdr.IsValid)
                {
                    //var _tup_1 = @struct.unpack_from("<HHHB", data, dofs);
                    var ofs = rdr.ReadLeUInt16();
                    var seg = rdr.ReadLeUInt16();
                    var typeidx = rdr.ReadLeUInt16();
                    var namelen = rdr.ReadByte();
                    var name = Encoding.ASCII.GetString(data, (int) rdr.Offset, namelen);
                    syms.Add(new PublicSymbol(ofs, seg, typeidx, name));
                }
                this.symbols = syms;
            }

            public override string ToString()
            {
                //return '<sstPublics len=%d nSymbols=%d>' % (len(self.data), len(self.symbols))
                return String.Format("<sstPublics {0}>", string.Join(" ", this.symbols));
            }
        }

        /// <summary>
        /// Find Codeview pointer in the last 256 bytes of the EXE
        /// </summary>
        /// <param name="rawImage">The raw image.</param>
        /// <returns>
        /// A tuple of (signature, start of CodeView data/dlfaBase, start of subsection directory)
        /// </returns>
        public static (string, int, int) FindCodeView(byte[] rawImage)
        {
            // get filesize -- seek to EOF then 
            var fp = new LeImageReader(rawImage);
            var szfile = rawImage.Length;
            for (var ofs = 8; ofs <= 256 && ofs < szfile; ++ofs)
            {
                fp.Offset = ofs;
                var sig = Encoding.ASCII.GetString(fp.ReadBytes(4));
                long dlfaBase = fp.ReadLeUInt32();
                if (sig.StartsWith("NB0"))
                {
                    // calculate start of debug data
                    dlfaBase = fp.Offset - dlfaBase;
                    // bounds check
                    if (dlfaBase < 0 || dlfaBase > szfile)
                    {
                        continue;
                    }
                    // try to read the debug data and check the signature
                    fp.Offset = dlfaBase;
                    var nsig = Encoding.ASCII.GetString(fp.ReadBytes(4));
                    var lfoSubsecDir = fp.ReadLeUInt32();
                    if (nsig.StartsWith("NB0"))
                    {
                        return (sig, (int)dlfaBase, (int)(lfoSubsecDir + dlfaBase));
                    }
                }
            }
            // found nothing
            return (null, 0, 0);
        }

        public Dictionary<Address, ImageSymbol> LoadSymbols()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read the CodeView subsection directory
        /// </summary>
        public static List<object> ReadSubsectionDirectory(LeImageReader fp, int dlfaBase)
        {
            // get number of subsections
            var cdnt = fp.ReadLeUInt16();
            var subsecs = new List<object>();
            foreach (var i in Enumerable.Range(0, cdnt))
            {
                // read subsection headers
                //var _tup_2 = @struct.unpack("<HHLH", fp.read(10));
                var usst = fp.ReadLeUInt16();
                var module = fp.ReadLeUInt16();
                var lfoStart = fp.ReadLeInt32();
                var cb = fp.ReadLeUInt16();
                // read subsection data
                //var pos = /*fp.tell();*/
                var rdr = new LeImageReader(fp.Bytes, (uint)(lfoStart + dlfaBase));
                var data = fp.ReadBytes(cb);
                //fp.seek(pos, 0);
                var sst = (SST) usst;
                // mash the subsection into one
                var FACTORY = new Dictionary<SST, Func<SST, ushort, byte[], Subsection>>
                {
                    { SST.SST_MODULES, (s,m,d) => new sstModules(s,m,d)},
                    { SST.SST_PUBLICS, (s,m,d) => new sstPublics(s,m,d)}
                };
                if (FACTORY.ContainsKey(sst))
                {
                    subsecs.Add(FACTORY[sst](sst, module, data));
                }
                else
                {
                    subsecs.Add(new Subsection(sst, module, data));
                }
            }
            return subsecs;
        }

        public bool LoadCodeViewInfo()
        {
            // read CodeView header
            var (ver, dlfaBase, subsecBase) = FindCodeView(rawImage);
            if (ver is null)
                return false;

            // print CodeView header
            Console.WriteLine("CodeView version '{0}', with dlfaBase=0x{1:X8} and subsecBase=0x{2:X8}", ver, dlfaBase, subsecBase);

            // read the subsection directory
            this.subsections = ReadSubsectionDirectory(new LeImageReader(this.rawImage, (uint) subsecBase), dlfaBase);
            return true;
        }
    }
}