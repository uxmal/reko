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
// ----------------------------------------
// Subsection type
// ----------------------------------------
// ----------------------------------------
// Subsection -- base class for all Codeview subsections
// ----------------------------------------
// ----------------------------------------
// sstModules -- module definition
// ----------------------------------------
// ----------------------------------------
// sstPublics
// ----------------------------------------
// 

using @struct;

using IntEnum = @enum.IntEnum;

using pprint = pprint.pprint;

using pformat = pprint.pformat;

using System;

using System.Collections.Generic;

using System.Linq;

public static class CodeViewLoader {
    
    public class SST
        : IntEnum {
        
        public int SST_COMPACTED;
        
        public int SST_LIBRARIES;
        
        public int SST_MODULES;
        
        public int SST_PUBLICS;
        
        public int SST_SRCLINES;
        
        public int SST_SRCLNSEG;
        
        public int SST_SYMBOLS;
        
        public int SST_TYPE;
        
        public int SST_MODULES = 0x101;
        
        public int SST_PUBLICS = 0x102;
        
        public int SST_TYPE = 0x103;
        
        public int SST_SYMBOLS = 0x104;
        
        public int SST_SRCLINES = 0x105;
        
        public int SST_LIBRARIES = 0x106;
        
        public int SST_COMPACTED = 0x108;
        
        public int SST_SRCLNSEG = 0x109;
    }
    
    public class Subsection
        : object {
        
        public object data;
        
        public object module;
        
        public object sst;
        
        public Subsection(SST sst, object module, object data) {
            this.sst = sst;
            this.module = module;
            this.data = data;
        }
        
        public virtual object @__repr__() {
            return String.Format("<Subsection sst=%s(0x%X), module=%d, len(data)=%s>", this.sst, this.sst, this.module, this.data.Count);
        }
    }
    
    public class sstModules
        : Subsection {
        
        public object @string;
        
        public sstModules(SST sst, object module, object data)
            : base(module, data) {
            var _tup_1 = @struct.unpack_from("<HHHHHBBB", data);
            this.csBase = _tup_1.Item1;
            this.csOfs = _tup_1.Item2;
            this.csLen = _tup_1.Item3;
            this.ovl = _tup_1.Item4;
            this.libIndx = _tup_1.Item5;
            this.nsegs = _tup_1.Item6;
            var strlen = _tup_1.Item8;
            this.@string = data[-strlen].decode("ascii");
        }
        
        public virtual object @__repr__() {
            return String.Format("<sstModules cs(base=0x%X, ofs=0x%X, len=%d), ovl=%d, libIndx=%X, nsegs=%d, str=\'%s\'>", this.csBase, this.csOfs, this.csLen, this.ovl, this.libIndx, this.nsegs, this.@string);
        }
    }
    
    public class PublicSymbol {
        
        public object name;
        
        public object offset;
        
        public object segment;
        
        public object typeidx;
        
        public PublicSymbol(object offset, object segment, object typeidx, object name) {
            this.offset = offset;
            this.segment = segment;
            this.typeidx = typeidx;
            this.name = name;
        }
        
        public virtual object @__repr__() {
            return String.Format("<PublicSymbol \'%s\', seg 0x%X ofs 0x%X type %d>", this.name, this.segment, this.offset, this.typeidx);
        }
    }
    
    public class sstPublics
        : Subsection {
        
        public List<object> symbols;
        
        public sstPublics(object sst, object module, object data)
            : base(module, data) {
            using (var fo = open("pub", "wb")) {
                fo.write(data);
            }
            var syms = new List<object>();
            var dofs = 0;
            while (dofs < data.Count) {
                var _tup_1 = @struct.unpack_from("<HHHB", data, dofs);
                var ofs = _tup_1.Item1;
                var seg = _tup_1.Item2;
                var typeidx = _tup_1.Item3;
                var namelen = _tup_1.Item4;
                dofs += 7;
                var name = data[dofs::(dofs  +  namelen)].decode("ascii");
                dofs += namelen;
                syms.append(new PublicSymbol(ofs, seg, typeidx, name));
            }
            this.symbols = syms;
        }
        
        public virtual string @__repr__() {
            //return '<sstPublics len=%d nSymbols=%d>' % (len(self.data), len(self.symbols))
            return String.Format("<sstPublics %s>", pformat(this.symbols, indent: 2));
        }
    }

    //     Find Codeview pointer in the last 256 bytes of the EXE
    // 
    //     fp: file object open in binary mode
    // 
    //     returns -- (signature, start of CodeView data/dlfaBase, start of subsection directory)
    //     

    public static Tuple<object, int, int> findCodeview(file fp) {
        // get filesize -- seek to EOF then 
        fp.seek(0, 2);
        var szfile = fp.tell();
        foreach (var ofs in Enumerable.Range(0, Convert.ToInt32(Math.Ceiling(Convert.ToDouble(-256 - -8) / -1))).Select(_x_1 => -8 + _x_1 * -1)) {
            fp.seek(ofs, 2);
            var _tup_1 = @struct.unpack("<4sL", fp.read(8));
            var sig = _tup_1.Item1;
            var dlfaBase = _tup_1.Item2;
            if (sig.startswith(new byte[] { (byte)'N', (byte)'B', (byte)'0' })) {
                // calculate start of debug data
                dlfaBase = fp.tell() - dlfaBase;
                // bounds check
                if (dlfaBase < 0 || dlfaBase > szfile) {
                    continue;
                }
                // try to read the debug data and check the signature
                fp.seek(dlfaBase);
                var _tup_2 = @struct.unpack("<4sL", fp.read(8));
                var nsig = _tup_2.Item1;
                var lfoSubsecDir = _tup_2.Item2;
                if (nsig.startswith(new byte[] { (byte)'N', (byte)'B', (byte)'0' })) {
                    return (sig.decode("ascii"), dlfaBase, lfoSubsecDir + dlfaBase);
                }
            }
        }
        // found nothing
        return null;
    }
    
    // 
    //     Read the CodeView subsection directory
    //     
    public static List<object> readSubsectionDirectory(file fp, object dlfaBase) {
        // get number of subsections
        var _tup_1 = @struct.unpack("<H", fp.read(2));
        var cdnt = _tup_1.Item1;
        var subsecs = new List<object>();
        Console.WriteLine(cdnt);
        foreach (var i in Enumerable.Range(0, cdnt)) {
            // read subsection headers
            var _tup_2 = @struct.unpack("<HHLH", fp.read(10));
            var sst = _tup_2.Item1;
            var module = _tup_2.Item2;
            var lfoStart = _tup_2.Item3;
            var cb = _tup_2.Item4;
            // read subsection data
            var pos = fp.tell();
            fp.seek(lfoStart + dlfaBase, 0);
            var data = fp.read(cb);
            fp.seek(pos, 0);
            sst = new SST(sst);
            // mash the subsection into one
            var FACTORY = new Dictionary<object, object> {
                {
                    SST.SST_MODULES,
                    sstModules},
                {
                    SST.SST_PUBLICS,
                    sstPublics}};
            if (FACTORY.Contains(sst)) {
                subsecs.append(FACTORY[sst](sst, module, data));
            } else {
                subsecs.append(new Subsection(sst, module, data));
            }
        }
        return subsecs;
    }
    
    public static Tuple<object, int, int> x = findCodeview(fp);


    static CodeViewLoader() {
        fp.seek(subsecBase);
        pprint(readSubsectionDirectory(fp, dlfaBase));
    }
}
