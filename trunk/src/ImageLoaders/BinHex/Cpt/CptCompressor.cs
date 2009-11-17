using System;
using System.IO;
using System.Text;

namespace Decompiler.ImageLoaders.BinHex.Cpt
{

    using OSType = System.Int32;

    public class CptCompressor
    {
        const int HUFF_BE = 0;
        const int HUFF_LE = 1;

        public CptCompressor(byte[] dataFork)
        {
            infp = new MemoryStream(dataFork);
        }

        class HuffNode
        {
            public int flag, @byte;
            public HuffNode one, zero;
        }

        delegate int BitGetter();
        BitGetter get_bit;
        Stream infp;

        int bytesread;

        /* 515 because StuffIt Classic needs more than the needed 511 */
        HuffNode[] nodelist = new HuffNode[515];

        private int nodeptr = 0;

        private int bit;

        void de_huffman(uint obytes)
        {
            while (obytes != 0)
            {
                out_buffer[out_ptr++] = (byte)gethuffbyte(nodelist[0]);
                obytes--;
            }
            return;
        }

        void de_huffman_end(uint term)
        {
            int c;

            while ((c = gethuffbyte(nodelist[0])) != term)
            {
                out_buffer[out_ptr++] = (byte)c;
            }
        }

        void set_huffman(int endian)
        {
            if (endian == HUFF_LE)
            {
                get_bit = getbit_le;
            }
            else if (endian == HUFF_BE)
            {
                get_bit = getbit_be;
            }
        }

        void read_tree()
        {
            nodeptr = 0;  // nodelist;
            bit = 0;		/* put us on a boundary */
            read_sub_tree();
        }

        /* This routine recursively reads the Huffman encoding table and builds
           a decoding tree. */
        private HuffNode read_sub_tree()
        {
            HuffNode np;

            np = nodelist[nodeptr++];
            if (get_bit() == 1)
            {
                np.flag = 1;
                np.@byte = getdecodebyte();
            }
            else
            {
                np.flag = 0;
                np.zero = read_sub_tree();
                np.one = read_sub_tree();
            }
            return np;
        }

        int getbit_be_b;
        const int BYTEMASK = 0xFF;

        /* This routine returns the next bit in the input stream (MSB first) */
        private int getbit_be()
        {
            if (bit == 0)
            {
                getbit_be_b = getb(infp) & BYTEMASK;
                bit = 8;
                bytesread++;
            }
            bit--;
            return (getbit_be_b >> bit) & 1;
        }

        int getbit_le_b;

        /* This routine returns the next bit in the input stream (LSB first) */
        private int getbit_le()
        {
            if (bit == 0)
            {
                getbit_le_b = getb(infp) & BYTEMASK;
                bit = 8;
                bytesread++;
            }
            bit--;
            return (getbit_le_b >> (7 - bit)) & 1;
        }

        void clrhuff()
        {
            bit = 0;
        }

        int gethuffbyte(HuffNode l_nodelist)
        {
            HuffNode np;

            np = l_nodelist;
            while (np.flag == 0)
            {
                np = get_bit() != 0 ? np.one : np.zero;
            }
            return np.@byte;
        }

        int getihuffbyte()
        {
            return gethuffbyte(this.nodelist[0]);
        }

        private int getdecodebyte()
        {
            int i, b;

            b = 0;
            for (i = 8; i > 0; i--)
            {
                b = (b << 1) + get_bit();
            }
            return b;
        }

        const int C_SIGNATURE = 0;
        const int C_VOLUME = 1;
        const int C_XMAGIC = 2;
        const int C_IOFFSET = 4;
        const int CPTHDRSIZE = 8;

        const int C_HDRCRC = 0;
        const int C_ENTRIES = 4;
        const int C_COMMENT = 6;
        const int CPTHDR2SIZE = 7;

        const int CHDRSIZE = (CPTHDRSIZE + CPTHDR2SIZE);

        const int F_FNAME = 0;
        const int F_FOLDER = 32;
        const int F_FOLDERSIZE = 33;
        const int F_VOLUME = 35;
        const int F_FILEPOS = 36;
        const int F_FTYPE = 40;
        const int F_CREATOR = 44;
        const int F_CREATIONDATE = 48;
        const int F_MODDATE = 52;
        const int F_FNDRFLAGS = 56;
        const int F_FILECRC = 58;
        const int F_CPTFLAG = 62;
        const int F_RSRCLENGTH = 64;
        const int F_DATALENGTH = 68;
        const int F_COMPRLENGTH = 72;
        const int F_COMPDLENGTH = 76;
        const int FILEHDRSIZE = 80;


        class CptHdr
        {			/* 8 bytes */
            public byte signature;	/* = 1 -- for verification */
            public byte volume;		/* for multi-file archives */
            public ushort xmagic;		/* verification multi-file consistency*/
            public uint offset;		/* index offset */
            /* The following are really in header2 at offset */
            public uint hdrcrc;		/* header crc */
            public ushort entries;	/* number of index entries */
            public byte commentsize;	/* number of bytes comment that follow*/
        }

        class FileHdr
        {		/* 78 bytes */
            public byte[] fName;	/* a STR32 */
            public byte folder;		/* set to 1 if a folder */
            public ushort foldersize;	/* number of entries in folder */
            public byte volume;		/* for multi-file archives */
            public uint filepos;	/* position of data in file */
            public OSType fType;			/* file type */
            public OSType fCreator;		/* er... */
            public uint creationDate;
            public uint modDate;	/* !restored-compat w/backup prgms */
            public ushort FndrFlags;	/* copy of Finder flags.  For our
						purposes, we can clear:
						busy,onDesk */
            public uint fileCRC;	/* crc on file */
            public ushort cptFlag;	/* cpt flags */
            public uint rsrcLength;	/* decompressed lengths */
            public uint dataLength;
            public uint compRLength;	/* compressed lengths */
            public uint compDLength;
        }


        /* file format is:
            cptArchiveHdr
                file1data
                    file1RsrcFork
                    file1DataFork
                file2data
                    file2RsrcFork
                    file2DataFork
                .
                .
                .
                fileNdata
                    fileNRsrcFork
                    fileNDataFork
            cptIndex
        */



        /* cpt flags */
        const int encryp = 1	/* file is encrypted */;
        const int crsrc = 2	/* resource fork is compressed */;
        const int cdata = 4	/* data fork is compressed */;
        /*      ????	8	/* unknown */

        const int CIRCSIZE = 8192;



        const int ESC1 = 0x81;
        const int ESC2 = 0x82;
        const int NONESEEN = 0;
        const int ESC1SEEN = 1;
        const int ESC2SEEN = 2;

        private byte[] cpt_data;
        private uint cpt_datamax;
        private uint cpt_datasize;
        private byte[] cpt_LZbuff = new byte[CIRCSIZE];
        private uint cpt_LZptr;
        private int cpt_char;
        private uint cpt_crc;
        private uint cpt_inlength;
        private uint cpt_outlength;
        private int cpt_outstat;
        private byte cpt_savechar;
        private uint cpt_newbits;
        private int cpt_bitsavail;
        private int cpt_blocksize;
        /* Lengths is twice the max number of entries, and include slack. */
        const int SLACK = 6;
        HuffNode[] cpt_Hufftree = new HuffNode[512 + SLACK];
        HuffNode[] cpt_LZlength = new HuffNode[128 + SLACK];
        HuffNode[] cpt_LZoffs = new HuffNode[256 + SLACK];


        CrcUpdater updcrc;
        delegate uint CrcUpdater(uint crc, byte[] buf, int offset, int length);

        private uint zip_updcrc(uint crc, byte[] buf, int offset, int length)
        {
            return crc;
        }

        public void cpt()
        {
            CptHdr cpthdr;
            FileHdr filehdr;
            byte[] cptindex;
            int cptindsize;
            int cptptr;
            int i;

            updcrc = zip_updcrc;
            //    crcinit = zip_crcinit;
            //    cpt_crc = INIT_CRC;
            if (readcpthdr(out cpthdr) == 0)
            {
                Console.Error.WriteLine("Can't read archive header");
                exit(1);
            }

            cptindsize = cpthdr.entries * FILEHDRSIZE;
            if (cpthdr.commentsize > cptindsize)
            {
                cptindsize = cpthdr.commentsize;
            }
            cptindex = new byte[cptindsize];
            cptptr = 0; //  cptindex;
            if (infp.Read(cptindex, cptptr, cpthdr.commentsize) != cpthdr.commentsize)
            {
                Console.Error.WriteLine("Can't read comment.");

                exit(1);
            }
            cpt_crc = updcrc(cpt_crc, cptindex, cptptr, cpthdr.commentsize);

            for (i = 0; i < cpthdr.entries; i++)
            {
                cptindex[cptptr] = (byte)infp.ReadByte();
                cpt_crc = updcrc(cpt_crc, cptindex, cptptr, 1);
                if ((cptindex[cptptr] & 0x80) == 0x80)
                {
                    cptindex[cptptr + F_FOLDER] = 1;
                    cptindex[cptptr] &= 0x3f;
                }
                else
                {
                    cptindex[cptptr + F_FOLDER] = 0;
                }
                if (infp.Read(cptindex, cptptr + 1, cptindex[cptptr]) != cptindex[cptptr])
                {
                    Console.Error.WriteLine("Can't read file header #%d", i + 1);

                    exit(1);
                }
                cpt_crc = updcrc(cpt_crc, cptindex, cptptr + 1, cptindex[cptptr]);
                if (cptindex[cptptr + F_FOLDER] != 0)
                {
                    if (infp.Read(cptindex, cptptr + F_FOLDERSIZE, 2) != 2)
                    {
                        Console.Error.WriteLine("Can't read file header #%d", i + 1);

                        exit(1);
                    }
                    cpt_crc = updcrc(cpt_crc, cptindex, cptptr + F_FOLDERSIZE, 2);
                }
                else
                {
                    if (infp.Read(cptindex, cptptr + F_VOLUME, FILEHDRSIZE - F_VOLUME) !=
                    FILEHDRSIZE - F_VOLUME)
                    {
                        Console.Error.WriteLine("Can't read file header #%d", i + 1);

                        exit(1);
                    }
                    cpt_crc = updcrc(cpt_crc, cptindex, cptptr + F_VOLUME,
                            FILEHDRSIZE - F_VOLUME);
                }
                cptptr += FILEHDRSIZE;
            }
            //if(cpt_crc != cpthdr.hdrcrc) {
            //Console.Error.WriteLine("Header CRC mismatch: got 0x%08x, need 0x%08x",
            //    (int)cpthdr.hdrcrc, (int)cpt_crc);

            //exit(1);
            //}

            cptptr = 0; // cptindex;
            for (i = 0; i < cpthdr.entries; i++)
            {
                if (cpt_filehdr(out filehdr, cptindex, cptptr) == -1)
                {
                    Console.Error.WriteLine("Can't read file header #%d", i + 1);

                    exit(1);
                }
                if (filehdr.folder != 0)
                {
                    cpt_folder(text, filehdr, cptindex, cptptr);
                    i += filehdr.foldersize;
                    cptptr += filehdr.foldersize * FILEHDRSIZE;
                }
                else
                {
                    cpt_uncompact(filehdr);
                }
                cptptr += FILEHDRSIZE;
            }
        }

        private int readcpthdr(out CptHdr s)
        {
            s = new CptHdr();
            byte[] temp = new byte[CHDRSIZE];


            if (infp.Read(temp, 0, CPTHDRSIZE) != CPTHDRSIZE)
            {
                return 0;
            }

            if (temp[C_SIGNATURE] != 1)
            {
                Console.Error.WriteLine("Not a Compactor file");
                return 0;
            }

            cpt_datasize = get4(temp, C_IOFFSET);
            s.offset = cpt_datasize;
            cpt_data = new byte[cpt_datasize];
            cpt_datamax = cpt_datasize;

            if (infp.Read(cpt_data, CPTHDRSIZE,
                (int)s.offset - CPTHDRSIZE) != s.offset - CPTHDRSIZE)
            {
                return 0;
            }

            if (infp.Read(temp, CPTHDRSIZE, CPTHDR2SIZE) != CPTHDR2SIZE)
            {
                return 0;
            }

            cpt_crc = updcrc(cpt_crc, temp, CPTHDRSIZE + C_ENTRIES, 3);
            s.hdrcrc = get4(temp, CPTHDRSIZE + C_HDRCRC);
            s.entries = get2(temp, CPTHDRSIZE + C_ENTRIES);
            s.commentsize = temp[CPTHDRSIZE + C_COMMENT];

            return 1;
        }

        uint get4(byte[] buf, int offset)
        {
            uint u =
                ((uint)buf[offset + 0] << 24) |
                ((uint)buf[offset + 1] << 16) |
                ((uint)buf[offset + 2] << 8) |
                ((uint)buf[offset + 3]);
            return u;

        }

        ushort get2(byte[] buf, int offset)
        {
            ushort u0 = (ushort)(buf[offset + 0] << 8);
            ushort u1 = (ushort)(buf[offset + 1]);
            return (ushort)(u0 | u1);
        }

        private byte[] info = new byte[128];

        private string transname(byte[] buf, int offset, int length)
        {
            return Encoding.ASCII.GetString(buf, 0, length);
        }

        string text;

        private int cpt_filehdr(out FileHdr f, byte[] hdr, int hdrOff)
        {
            f = new FileHdr();

            int i;
            int n;
            string ftype, fauth;

            info = new byte[INFOBYTES];

            n = hdr[hdrOff + F_FNAME] & BYTEMASK;
            if (n > F_NAMELEN)
            {
                n = F_NAMELEN;
            }
            info[I_NAMEOFF] = (byte)n;
            copy(info, I_NAMEOFF + 1, hdr, hdrOff + F_FNAME + 1, n);
            text = transname(hdr, hdrOff + F_FNAME + 1, n);

            f.folder = hdr[hdrOff + F_FOLDER];
            if (f.folder != 0)
            {
                f.foldersize = get2(hdr, hdrOff + F_FOLDERSIZE);
            }
            else
            {
                f.cptFlag = get2(hdr, hdrOff + F_CPTFLAG);
                f.rsrcLength = get4(hdr, hdrOff + F_RSRCLENGTH);
                f.dataLength = get4(hdr, hdrOff + F_DATALENGTH);
                f.compRLength = get4(hdr, hdrOff + F_COMPRLENGTH);
                f.compDLength = get4(hdr, hdrOff + F_COMPDLENGTH);
                f.fileCRC = get4(hdr, hdrOff + F_FILECRC);
                f.FndrFlags = get2(hdr, hdrOff + F_FNDRFLAGS);
                f.filepos = get4(hdr, hdrOff + F_FILEPOS);
                f.volume = hdr[hdrOff + F_VOLUME];
            }

            write_it = true;
            if (list)
            {
                //        do_indent(indent);
                if (f.folder != 0)
                {
                    Console.Error.Write("folder=\"%s\"", text);
                }
                else
                {
                    ftype = transname(hdr, hdrOff + F_FTYPE, 4);
                    fauth = transname(hdr, hdrOff + F_CREATOR, 4);
                    Console.Error.Write(
                        "name=\"%s\", type=%4.4s, author=%4.4s, data=%ld, rsrc=%ld",
                        text, ftype, fauth,
                        (long)f.dataLength, (long)f.rsrcLength);
                }
                if (info_only)
                {
                    write_it = false;
                }
                if (query)
                {
                    //            write_it = do_query();
                }
                else
                {
                    Console.Error.WriteLine();
                }
            }


            if (write_it)
            {
                define_name(text);

                if (f.folder == 0)
                {
                    copy(info, I_TYPEOFF, hdr, hdrOff + F_FTYPE, 4);
                    copy(info, I_AUTHOFF, hdr, hdrOff + F_CREATOR, 4);
                    copy(info, I_FLAGOFF, hdr, hdrOff + F_FNDRFLAGS, 2);
                    copy(info, I_DLENOFF, hdr, hdrOff + F_DATALENGTH, 4);
                    copy(info, I_RLENOFF, hdr, hdrOff + F_RSRCLENGTH, 4);
                    copy(info, I_CTIMOFF, hdr, hdrOff + F_CREATIONDATE, 4);
                    copy(info, I_MTIMOFF, hdr, hdrOff + F_MODDATE, 4);
                }
            }
            return 1;
        }

        private void copy(byte[] dst, int dstOff, byte[] src, int srcOff, int cb)
        {
            Array.Copy(src, srcOff, dst, dstOff, cb);
        }

        private void cpt_folder(string name, FileHdr fileh, byte[] cptindex, int cptptr)
        {
            int i, nfiles;
            char[] loc_name = new char[64];
            FileHdr filehdr;

            for (i = 0; i < 64; i++)
            {
                loc_name[i] = name[i];
            }
            if (write_it || info_only)
            {
                cptptr += FILEHDRSIZE;
                nfiles = fileh.foldersize;
                if (write_it)
                {
                    //	    do_mkdir(text, info);
                }
                indent++;
                for (i = 0; i < nfiles; i++)
                {
                    if (cpt_filehdr(out filehdr, cptindex, cptptr) == -1)
                    {
                        Console.Error.WriteLine("Can't read file header #%d", i + 1);

                        exit(1);
                    }
                    if (filehdr.folder != 0)
                    {
                        cpt_folder(text, filehdr, cptindex, cptptr);
                        i += filehdr.foldersize;
                        cptptr += filehdr.foldersize * FILEHDRSIZE;
                    }
                    else
                    {
                        cpt_uncompact(filehdr);
                    }
                    cptptr += FILEHDRSIZE;
                }
                if (write_it)
                {
                    //	    enddir();
                }
                indent--;
                if (list)
                {
                    //	    do_indent(indent);
                    Console.Error.WriteLine("leaving folder \"%s\"", loc_name);
                }
            }
        }

        bool write_it;
        bool list;
        bool verbose;
        bool info_only;
        bool query;
        int indent;

        uint INIT_CRC = 0;

        private void cpt_uncompact(FileHdr filehdr)
        {
            if ((filehdr.cptFlag & 1) == 1)
            {
                Console.Error.WriteLine("\tFile is password protected, skipping file");

                return;
            }
            if (write_it)
            {
                start_info(info, filehdr.rsrcLength, filehdr.dataLength);
                cpt_crc = INIT_CRC;
                cpt_char = (int)filehdr.filepos; // + cpt_data
            }
            if (verbose)
            {
                Console.Error.Write("\tRsrc: ");
                if (filehdr.compRLength == 0)
                {
                    Console.Error.Write("empty");
                }
                else if ((filehdr.cptFlag & 2) == 2)
                {
                    Console.Error.Write("RLE/LZH compressed (%4.1f%%)",
                        100.0 * filehdr.compRLength / filehdr.rsrcLength);
                }
                else
                {
                    Console.Error.Write("RLE compressed (%4.1f%%)",
                        100.0 * filehdr.compRLength / filehdr.rsrcLength);
                }
            }
            if (write_it)
            {
                start_rsrc();
                cpt_wrfile(
                    filehdr.compRLength,
                    filehdr.rsrcLength,
                    (ushort)(filehdr.cptFlag & 2u));
                cpt_char = (int)(filehdr.filepos + filehdr.compRLength);
            }
            if (verbose)
            {
                Console.Error.Write(", Data: ");
                if (filehdr.compDLength == 0)
                {
                    Console.Error.Write("empty");
                }
                else if ((filehdr.cptFlag & 4) == 4)
                {
                    Console.Error.Write("RLE/LZH compressed (%4.1f%%)",
                        100.0 * filehdr.compDLength / filehdr.dataLength);
                }
                else
                {
                    Console.Error.Write("RLE compressed (%4.1f%%)",
                        100.0 * filehdr.compDLength / filehdr.dataLength);
                }
            }
            if (write_it)
            {
                start_data();
                cpt_wrfile(filehdr.compDLength, filehdr.dataLength,
                       (ushort)(filehdr.cptFlag & 4));
                //if (filehdr.fileCRC != cpt_crc)
                //{
                //    Console.Error.WriteLine(
                //    "CRC error on file: need 0x%08lx, got 0x%08lx",
                //    (long)filehdr.fileCRC, (long)cpt_crc);

                //    exit(1);
                //}
                end_file();
            }
            if (verbose)
            {
                Console.Error.WriteLine(".");
            }
        }

        private void cpt_wrfile(uint ibytes, uint obytes, ushort type)
        {
            if (ibytes == 0)
            {
                return;
            }
            cpt_outstat = NONESEEN;
            cpt_inlength = ibytes;
            cpt_outlength = obytes;
            cpt_LZptr = 0;
            cpt_blocksize = 0x1fff0;
            if (type == 0)
            {
                cpt_rle();
            }
            else
            {
                cpt_rle_lzh();
            }
            cpt_crc = updcrc(cpt_crc, out_buffer, 0, (int)obytes);
        }

        void cpt_wrfile1(byte[] in_char, uint ibytes, uint obytes, uint type, int blocksize)
        {
            // cpt_char = in_char;
            cpt_data = in_char;
            cpt_char = 0;

            if (ibytes == 0)
            {
                return;
            }
            cpt_outstat = NONESEEN;
            cpt_inlength = ibytes;
            cpt_outlength = obytes;
            cpt_LZptr = 0;
            cpt_blocksize = blocksize;
            if (type == 0)
            {
                cpt_rle();
            }
            else
            {
                cpt_rle_lzh();
            }
        }

        private void cpt_outch(byte ch)
        {
            cpt_LZbuff[cpt_LZptr++ & (CIRCSIZE - 1)] = ch;
            switch (cpt_outstat)
            {
                case NONESEEN:
                    if (ch == ESC1 && cpt_outlength != 1)
                    {
                        cpt_outstat = ESC1SEEN;
                    }
                    else
                    {
                        cpt_savechar = ch;
                        out_buffer[out_ptr++] = ch;
                        cpt_outlength--;
                    }
                    break;
                case ESC1SEEN:
                    if (ch == ESC2)
                    {
                        cpt_outstat = ESC2SEEN;
                    }
                    else
                    {
                        cpt_savechar = ESC1;
                        out_buffer[out_ptr++] = ESC1;
                        cpt_outlength--;
                        if (cpt_outlength == 0)
                        {
                            return;
                        }
                        if (ch == ESC1 && cpt_outlength != 1)
                        {
                            return;
                        }
                        cpt_outstat = NONESEEN;
                        cpt_savechar = ch;
                        out_buffer[out_ptr++] = ch;
                        cpt_outlength--;
                    }
                    break;
                case ESC2SEEN:
                    cpt_outstat = NONESEEN;
                    if (ch != 0)
                    {
                        while (--ch != 0)
                        {
                            out_buffer[out_ptr++] = cpt_savechar;
                            cpt_outlength--;
                            if (cpt_outlength == 0)
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        out_buffer[out_ptr++] = ESC1;
                        cpt_outlength--;
                        if (cpt_outlength == 0)
                        {
                            return;
                        }
                        cpt_savechar = ESC2;
                        out_buffer[out_ptr++] = cpt_savechar;
                        cpt_outlength--;
                    }
                    break;
            }
        }

        /*---------------------------------------------------------------------------*/
        /*	Run length encoding						     */
        /*---------------------------------------------------------------------------*/
        private void cpt_rle()
        {
            while (cpt_inlength-- > 0)
            {
                cpt_outch(cpt_data[cpt_char++]);
            }
        }

        /*---------------------------------------------------------------------------*/
        /*	Run length encoding plus LZ compression plus Huffman encoding	     */
        /*---------------------------------------------------------------------------*/
        private void cpt_rle_lzh()
        {
            int block_count;
            uint bptr;
            int Huffchar, LZlength, LZoffs;

            get_bit = cpt_getbit;
            cpt_LZbuff[CIRCSIZE - 3] = 0;
            cpt_LZbuff[CIRCSIZE - 2] = 0;
            cpt_LZbuff[CIRCSIZE - 1] = 0;
            cpt_LZptr = 0;
            while (cpt_outlength != 0)
            {
                cpt_readHuff(256, cpt_Hufftree);
                cpt_readHuff(64, cpt_LZlength);
                cpt_readHuff(128, cpt_LZoffs);
                block_count = 0;
                cpt_newbits = (uint)(cpt_data[cpt_char++] << 8);
                cpt_newbits = cpt_newbits | cpt_data[cpt_char++];
                cpt_newbits = cpt_newbits << 16;
                cpt_bitsavail = 16;
                while (block_count < cpt_blocksize && cpt_outlength != 0)
                {
                    if (cpt_getbit() != 0)
                    {
                        Huffchar = gethuffbyte(cpt_Hufftree[0]);
                        cpt_outch((byte)Huffchar);
                        block_count += 2;
                    }
                    else
                    {
                        LZlength = gethuffbyte(cpt_LZlength[0]);
                        LZoffs = gethuffbyte(cpt_LZoffs[0]);
                        LZoffs = (LZoffs << 6) | cpt_get6bits();
                        bptr = (uint)(cpt_LZptr - LZoffs);
                        while (LZlength-- > 0)
                        {
                            cpt_outch(cpt_LZbuff[bptr++ & (CIRCSIZE - 1)]);
                        }
                        block_count += 3;
                    }
                }
            }
        }

        /* Based on unimplod from unzip; difference are noted below. */
        struct sf_entry
        {
            public int Value;
            public uint BitLength;
        }

        /* See routine LoadTree.  The parameter tree (actually an array and
           two integers) are only used locally in this version and hence locally
           declared.  The parameter nodes has been renamed Hufftree.... */
        private void cpt_readHuff(int size, HuffNode[] Hufftree)
        {
            sf_entry[] tree_entry = new sf_entry[256 + SLACK]; /* maximal number of elements */
            int tree_entries;
            int tree_MaxLength; /* finishes local declaration of tree */

            int treeBytes, i, len;  /* declarations from ReadLengths */

            /* declarations from SortLengths */
            int j;
            /*  int i already above */
            sf_entry tmp;
            int entries;
            uint a, b;

            /* declarations from GenerateTrees */
            int codelen, lvlstart, next, parents;
            /*  int i, j already above */

            /* for Compactor */
            int[] tree_count = new int[32];
            /* end declarations */

            /* next paraphrased from ReadLengths with adaption for Compactor. */
            treeBytes = cpt_data[cpt_char++];
            if (size < treeBytes * 2)
            { /* too many entries, something is wrong! */
                Console.Error.WriteLine("Bytes is: %d, expected: %d", treeBytes,
                    size / 2);

                exit(1);
            }
            for (i = 0; i < 32; i++)
            {
                tree_count[i] = 0;
            }
            i = 0;
            tree_MaxLength = 0;
            tree_entries = 0;
            while (treeBytes-- > 0)
            { /* adaption for Compactor */
                len = cpt_data[cpt_char] >> 4;
                if (len != 0)
                { /* only if length unequal zero */
                    if (len > tree_MaxLength)
                    {
                        tree_MaxLength = len;
                    }
                    tree_count[len]++;
                    tree_entry[tree_entries].Value = i;
                    tree_entry[tree_entries++].BitLength = (uint)len;
                }
                i++;
                const int NIBBLEMASK = 0x0F;
                len = cpt_data[cpt_char++] & NIBBLEMASK;
                if (len != 0)
                { /* only if length unequal zero */
                    if (len > tree_MaxLength)
                    {
                        tree_MaxLength = len;
                    }
                    tree_count[len]++;
                    tree_entry[tree_entries].Value = i;
                    tree_entry[tree_entries++].BitLength = (uint)len;
                }
                i++;
            }

            /* Compactor allows unused trailing codes in its Huffman tree! */
            j = 0;
            for (i = 0; i <= tree_MaxLength; i++)
            {
                j = (j << 1) + tree_count[i];
            }
            j = (1 << tree_MaxLength) - j;
            /* Insert the unused entries for sorting purposes. */
            for (i = 0; i < j; i++)
            {
                tree_entry[tree_entries].Value = size;
                tree_entry[tree_entries++].BitLength = (uint)tree_MaxLength;
            }

            /* adaption from SortLengths */
            entries = tree_entries;
            for (i = 0; ++i < entries; )
            {
                tmp = tree_entry[i];
                b = (uint)tmp.BitLength;
                j = i;
                while ((j > 0) && ((a = tree_entry[j - 1].BitLength) >= b))
                {
                    if ((a == b) && (tree_entry[j - 1].Value <= tmp.Value))
                    {
                        break;
                    }
                    tree_entry[j] = tree_entry[j - 1];
                    --j;
                }
                tree_entry[j] = tmp;
            }

            /* Adapted from GenerateTrees */
            i = tree_entries - 1;
            /* starting at the upper end (and reversing loop) because of Compactor */
            lvlstart = next = size * 2 + SLACK - 1;
            /* slight adaption because of different node format used */
            for (codelen = tree_MaxLength; codelen >= 1; --codelen)
            {
                while ((i >= 0) && (tree_entry[i].BitLength == codelen))
                {
                    Hufftree[next] = new HuffNode();
                    Hufftree[next].@byte = tree_entry[i].Value;
                    Hufftree[next].flag = 1;
                    next--;
                    i--;
                }
                parents = next;
                if (codelen > 1)
                {
                    /* reversed loop */
                    for (j = lvlstart; j > parents + 1; j -= 2)
                    {
                        Hufftree[next] = new HuffNode();
                        Hufftree[next].one = Hufftree[j];
                        Hufftree[next].zero = Hufftree[j - 1];
                        Hufftree[next].flag = 0;
                        next--;
                    }
                }
                lvlstart = parents;
            }
            Hufftree[0] = new HuffNode();
            Hufftree[0].one = Hufftree[next + 2];
            Hufftree[0].zero = Hufftree[next + 1];
            Hufftree[0].flag = 0;
        }

        private int cpt_get6bits()
        {
            int b = 0, cn;

            b = (int)(cpt_newbits >> 26) & 0x3F;
            cpt_bitsavail -= 6;
            cpt_newbits <<= 6;
            if (cpt_bitsavail < 16)
            {
                cn = cpt_data[cpt_char++] << 8;
                cn |= cpt_data[cpt_char++];
                cpt_newbits |= (uint)(cn << (16 - cpt_bitsavail));
                cpt_bitsavail += 16;
            }
            return b;
        }

        private int cpt_getbit()
        {
            int b;

            b = (int)(cpt_newbits >> 31) & 1;
            cpt_bitsavail--;
            if (cpt_bitsavail < 16)
            {
                cpt_newbits |= ((uint)cpt_data[cpt_char++] << 8);
                cpt_newbits |= cpt_data[cpt_char++];
                cpt_bitsavail += 16;
            }
            cpt_newbits <<= 1;
            return b;
        }

        const int TEXT = 0;
        const int DATA = 1;
        const int RSRC = 2;
        const int FULL = 3;
        const int MACB = 4;
        const int FORK = 5;
        const int APSH = 6;
        const int MACS = 7;
        const int UNIX = 8;


        void exit(int exitCode)
        {
            System.Environment.Exit(exitCode);
        }

        //#if AUFS
        //private void check_aufs();
        //private void aufs_namings();
        //private void wr_aufs_info();
        //#endif
        //#if APPLEDOUBLE
        //private void check_appledouble();
        //private void appledouble_namings();
        //private void wr_appledouble_info();
        //#endif
        //#if APPLESHARE
        //private void mk_share_name();
        //#endif

        //#ifndef BSD
        ///* all those stupid differences! */
        //#define bcopy(src,dest,length)	memcpy((dest),(src),(length))
        //#define bzero(block,length)	memset((block),0,(length))
        //#endif

        const int INFO_FORK = 1;
        const int RSRC_FORK = 2;
        const int DATA_FORK = 3;

        private string f_info;
        private string f_data;
        private string f_rsrc;
        private string f_text;
        private string f_unix;
        private string f_bin;
        private string f_folder = ".foldername";
        private string share_name;
        private string hex = "0123456789abcdef";
        private string infodir = ".finderinfo";
        private string rsrcdir = ".resource";
        // #define INFOSZ	sizeof(infodir)
        // #define RSRCSZ	sizeof(rsrcdir)
        //private char[] f_info_aufs = new char[I_NAMELEN * 3 + infodir.Length];
        //private char [] f_rsrc_aufs = new char[I_NAMELEN*3+rsrcdir.Length];
        // private char [] infodir = ".AppleDouble";
        // #define INFOSZ	sizeof(infodir)
        //private char [] f_info_appledouble = new char[I_NAMELEN*3+INFOSZ];

        private int mode = MACB;
        private bool mode_restricted = false;
        private bool mode_s_restricted = false;
        byte[] out_buffer;
        int out_ptr;

        private byte[] init_buffer = new byte[128];
        private byte[] buffer;
        private byte[] rbuffer = null;
        private byte[] dbuffer = null;
        private int ptr;
        private uint rsz, dsz, totsize, maxsize;

        void define_name(string text)
        {
            f_info = text + ".info";
            f_rsrc = text + ".rsrc";
            f_data = text + ".data";
            f_text = text + ".text";
            f_bin = text + ".bin";
            f_unix = text;
        }

        void start_info(byte[] info, uint rsize, uint dsize)
        {
            uint rs, ds;

            rsz = rsize;
            dsz = dsize;
            rs = (((rsz + 127) >> 7) << 7);
            ds = (((dsz + 127) >> 7) << 7);
            totsize = rs + ds + 128;
            buffer = new byte[totsize];
            maxsize = totsize;
            dbuffer = new byte[ds];
            rbuffer = new byte[rs];
            ptr = 0;
            System.Array.Copy(info, buffer, 128);
        }

        void start_rsrc()
        {
            out_buffer = rbuffer;
            out_ptr = 0;
        }

        void start_data()
        {
            out_buffer = dbuffer;
            out_ptr = 0;
        }

        void end_file()
        {
            Stream fp;
            int i, c;

            buffer[I_FLAGOFF] &= unchecked((byte)(~INITED_MASK));
            switch (mode)
            {
                case FULL:
                case FORK:
                    fp = new FileStream(f_info, FileMode.Create, FileAccess.Write);
                    fp.Write(buffer, 0, 128);
                    fp.Close();
                    if (rsz != 0 || mode == FULL)
                    {
                        fp = new FileStream(f_rsrc, FileMode.Create, FileAccess.Write);
                        fp.Write(rbuffer, 0, (int)rsz);
                        fp.Close();
                    }
                    if (dsz != 0 || mode == FULL)
                    {
                        fp = new FileStream(f_data, FileMode.Create, FileAccess.Write);
                        fp.Write(dbuffer, 0, (int)dsz);
                        fp.Close();
                    }
                    break;
                case RSRC:
                    fp = new FileStream(f_rsrc, FileMode.Create, FileAccess.Write);
                    fp.Write(rbuffer, 0, (int)rsz);
                    fp.Close();
                    break;
                case DATA:
                    fp = new FileStream(f_data, FileMode.Create, FileAccess.Write);
                    fp.Write(dbuffer, 0, (int)dsz);
                    fp.Close();
                    break;
                case TEXT:
                    fp = new FileStream(f_text, FileMode.Create, FileAccess.Write);
                    for (i = 0; i < dsz; i++)
                    {
                        c = dbuffer[i];
                        if (c == '\x0A' || c == '\x0D')
                        {
                            dbuffer[i] = (byte)('\x1B' - c);
                        }
                    }
                    fp.Write(dbuffer, 0, (int)dsz);
                    fp.Close();
                    break;
                case UNIX:
                    fp = new FileStream(f_unix, FileMode.Create, FileAccess.Write);
                    for (i = 0; i < dsz; i++)
                    {
                        c = dbuffer[i];
                        if (c == '\x0A' || c == '\x0D')
                        {
                            dbuffer[i] = (byte)('\x1B' - c);
                        }
                    }
                    fp.Write(dbuffer, 0, (int)dsz);
                    fp.Close();
                    break;
                /*
        case MACB:
        fp = fopen(f_bin, "w");
        if(buffer[I_FLAGOFF + 1] & PROTCT_MASK) {
            buffer[I_LOCKOFF] = 1;
        }
        buffer[I_FLAGOFF + 1] = 0;
        buffer[I_LOCKOFF + 1] = 0;
        fwrite(buffer, 1, (int)totsize, fp);
        fclose(fp);
        break;
        case MACS:

        if(buffer[I_FLAGOFF + 1] & PROTCT_MASK) {
            buffer[I_LOCKOFF] = 1;
        }
        buffer[I_FLAGOFF + 1] = 0;
        buffer[I_LOCKOFF + 1] = 0;
        fwrite(buffer, 1, (int)totsize, stdout);
        break;
                 */
            }
        }

#if NYI
void do_mkdir(name, header)
char *name, *header;
{
struct stat sbuf;
FILE *fp;
#if NOMKDIR
char command[21]; /* Systems without mkdir system call but more than 14
		     char file names?  Ridiculous! */
int sysreturn;
#endif
#if APPLESHARE
char dirinfo[I_NAMELEN*3+INFOSZ+10];
#endif

#if !SCAN
    if(mode == MACS) {
#else
    if(mode == MACS || mode == MACI) {
#endif
        header[I_NAMEOFF] |= 0x80;
	fwrite(header, 1, INFOBYTES, stdout);
	header[I_NAMEOFF] &= 0x7f;
	return;
    }
#if APPLESHARE
    if(mode == APSH) {
	bcopy(header, buffer, INFOBYTES);
	mk_share_name();
    } else {
	strcpy(share_name, name);
    }
#else 
    strcpy(share_name, name);
#endif
    if(stat(share_name, &sbuf) == -1) {  /* directory doesn't exist */
#if !NOMKDIR
	if(mkdir(share_name, 0777) == -1) {
	    Console.Error.WriteLine("Can't create subdirectory %s", share_name);
	    exit(1);
	}
#else 
	sprintf(command, "mkdir %s", share_name);
	if((sysreturn = system(command)) != 0) {
	    Console.Error.WriteLine("Can't create subdirectory %s", share_name);
	    exit(sysreturn);
	}
#endif 
    } else {		/* something exists with this name */
	if((sbuf.st_mode & S_IFMT) != S_IFDIR) {
	    Console.Error.WriteLine("Directory name %s already in use",
		share_name);
	    exit(1);
	}
    }
    chdir(share_name);
#if APPLESHARE
#if AUFS
    if(mode == APSH) {
	if(stat(rsrcdir, &sbuf) == -1) {  /* directory doesn't exist */
	    if(mkdir(rsrcdir, 0777) == -1) {
	 	Console.Error.WriteLine("Can't create subdirectory %s",
			rsrcdir);
	 	exit(1);
	    }
	} else {
	    if((sbuf.st_mode & S_IFMT) != S_IFDIR) {
		Console.Error.WriteLine("Directory name %s already in use",
			rsrcdir);
		exit(1);
	    }
	}
	if(stat(infodir, &sbuf) == -1) {  /* directory doesn't exist */
	    if(mkdir(infodir, 0777) == -1) {
	 	Console.Error.WriteLine("Can't create subdirectory %s",
			infodir);
	 	exit(1);
	    }
	} else {
	    if((sbuf.st_mode & S_IFMT) != S_IFDIR) {
		Console.Error.WriteLine("Directory name %s already in use",
			infodir);
		exit(1);
	    }
	}
	dirinfo[0] = 0;
	strcat(dirinfo, "../");
	strcat(dirinfo, infodir);
	strcat(dirinfo, "/");
	strcat(dirinfo, share_name);
	fp = fopen(dirinfo, "w");
	if(fp == NULL) {
	    perror(dirinfo);
	    exit(1);
	}
	wr_aufs_info(fp);
	fclose(fp);
    } else {
	fp = fopen(f_folder, "w");
	if(fp == NULL) {
	    perror(f_folder);
	    exit(1);
	}
        header[I_NAMEOFF] |= 0x80;
	fwrite(header, 1, INFOBYTES, fp);
	header[I_NAMEOFF] &= 0x7f;
	fclose(fp);
    }
#endif
#if APPLEDOUBLE
    if(mode == APSH) {
	if(stat(infodir, &sbuf) == -1) {  /* directory doesn't exist */
	    if(mkdir(infodir, 0777) == -1) {
	 	Console.Error.WriteLine("Can't create subdirectory %s",
			infodir);
	 	exit(1);
	    }
	} else {
	    if((sbuf.st_mode & S_IFMT) != S_IFDIR) {
		Console.Error.WriteLine("Directory name %s already in use",
			infodir);
		exit(1);
	    }
	}
	dirinfo[0] = 0;
	strcat(dirinfo, infodir);
	strcat(dirinfo, "/.Parent");
	fp = fopen(dirinfo, "w");
	if(fp == NULL) {
	    perror(dirinfo);
	    exit(1);
	}
	rsz = 0;
	wr_appledouble_info(fp);
	fclose(fp);
    } else {
	fp = fopen(f_folder, "w");
	if(fp == NULL) {
	    perror(f_folder);
	    exit(1);
	}
	header[I_NAMEOFF] |= 0x80;
	fwrite(header, 1, INFOBYTES, fp);
	header[I_NAMEOFF] &= 0x7f;
	fclose(fp);
    }
#endif
#else
    fp = fopen(f_folder, "w");
    if(fp == NULL) {
	perror(f_folder);
	exit(1);
    }
    header[I_NAMEOFF] |= 0x80;
    fwrite(header, 1, INFOBYTES, fp);
    header[I_NAMEOFF] &= 0x7f;
    fclose(fp);
#endif
}

void enddir()
{
char header[INFOBYTES];
int i;

#if !SCAN
    if(mode == MACS) {
#else
    if(mode == MACS || mode == MACI) {
#endif
	for(i = 0; i < INFOBYTES; i++) {
	    header[i] = 0;
	}
	header[I_NAMEOFF] = 0x80;
	fwrite(header, 1, INFOBYTES, stdout);
    } else {
	chdir("..");
    }
}

#if APPLESHARE
#if AUFS
private void check_aufs()
{
    /* check for .resource/ and .finderinfo/ */
    struct stat stbuf;
    int error = 0;

    if(stat(rsrcdir,&stbuf) < 0) {
	error ++;
    } else {
	if((stbuf.st_mode & S_IFMT) != S_IFDIR) {
		  error ++;
	}
    }
    if(stat(infodir,&stbuf) < 0) {
	error ++;
    } else {
	if((stbuf.st_mode & S_IFMT) != S_IFDIR) {
		  error++;
	}
    }
    if(error) {
	Console.Error.WriteLine("Not in an Aufs folder.");
	exit(1);
    }
}

private void aufs_namings()
{
    mk_share_name();
    sprintf(f_info_aufs, "%s/%s", infodir, share_name);
    sprintf(f_rsrc_aufs, "%s/%s", rsrcdir, share_name);
    sprintf(f_data, "%s", share_name);
}

private void wr_aufs_info(fp)
FILE *fp;
{
    FileInfo theinfo;
    int n;

    bzero((char *) &theinfo, sizeof theinfo);
    theinfo.fi_magic1 = FI_MAGIC1;
    theinfo.fi_version = FI_VERSION;
    theinfo.fi_magic = FI_MAGIC;
    theinfo.fi_bitmap = FI_BM_MACINTOSHFILENAME;

    /* AUFS stores Unix times. */
#if AUFSPLUS
    theinfo.fi_datemagic = FI_MAGIC;
    theinfo.fi_datevalid = FI_CDATE | FI_MDATE;
    put4(theinfo.fi_ctime, get4(buffer + I_CTIMOFF) - TIMEDIFF);
    put4(theinfo.fi_mtime, get4(buffer + I_MTIMOFF) - TIMEDIFF);
    put4(theinfo.fi_utime, (uint)time((time_t *)0));
#endif
    bcopy(buffer + I_TYPEOFF, theinfo.fi_fndr, 4);
    bcopy(buffer + I_AUTHOFF, theinfo.fi_fndr + 4, 4);
    bcopy(buffer + I_FLAGOFF, theinfo.fi_fndr + 8, 2);
    if((n = buffer[I_NAMEOFF] & 0xff) > F_NAMELEN) {
	n = F_NAMELEN;
    }
    strncpy((char *)theinfo.fi_macfilename, buffer + I_NAMEOFF + 1,n);
    /* theinfo.fi_macfilename[n] = '\0'; */
    strcpy((char *)theinfo.fi_comnt,
	"Converted by Unix utility to Aufs format");
    theinfo.fi_comln = strlen((char *)theinfo.fi_comnt);
    fwrite((char *) &theinfo, 1, sizeof theinfo, fp);
}
#endif

#if APPLEDOUBLE
private void check_appledouble()
{
    /* check for .AppleDouble/ */
    struct stat stbuf;
    int error = 0;

    if(stat(infodir,&stbuf) < 0) {
	error ++;
    } else {
	if((stbuf.st_mode & S_IFMT) != S_IFDIR) {
		  error++;
	}
    }
    if(error) {
	Console.Error.WriteLine("Not in an AppleDouble folder.");
	exit(1);
    }
}

private void appledouble_namings()
{
    mk_share_name();
    sprintf(f_info_appledouble, "%s/%s", infodir, share_name);
    sprintf(f_data, "%s", share_name);
}

private void wr_appledouble_info(fp)
FILE *fp;
{
    FileInfo theinfo;
    int n;

    bzero((char *) &theinfo, sizeof theinfo);
    put4(theinfo.fi_magic, (uint)FI_MAGIC);
    put2(theinfo.fi_version, (uint)FI_VERSION);
    put4(theinfo.fi_fill5, (uint)FI_FILL5);
    put4(theinfo.fi_fill6, (uint)FI_FILL6);
    put4(theinfo.fi_hlen, (uint)FI_HLEN);
    put4(theinfo.fi_fill7, (uint)FI_FILL7);
    put4(theinfo.fi_namptr, (uint)FI_NAMPTR);
    put4(theinfo.fi_fill9, (uint)FI_FILL9);
    put4(theinfo.fi_commptr, (uint)FI_COMMPTR);
    put4(theinfo.fi_fill12, (uint)FI_FILL12);
    put4(theinfo.fi_timeptr, (uint)FI_TIMEPTR);
    put4(theinfo.fi_timesize, (uint)FI_TIMESIZE);
    put4(theinfo.fi_fill15, (uint)FI_FILL15);
    put4(theinfo.fi_infoptr, (uint)FI_INFOPTR);
    put4(theinfo.fi_infosize, (uint)FI_INFOSIZE);

    bcopy(buffer + I_TYPEOFF, theinfo.fi_type, 4);
    bcopy(buffer + I_AUTHOFF, theinfo.fi_auth, 4);
    bcopy(buffer + I_FLAGOFF, theinfo.fi_finfo, 2);
    /* AppleDouble stores Unix times. */
    put4(theinfo.fi_ctime, get4(buffer + I_CTIMOFF) - TIMEDIFF);
    put4(theinfo.fi_mtime, get4(buffer + I_MTIMOFF) - TIMEDIFF);
    if((n = buffer[I_NAMEOFF] & 0xff) > F_NAMELEN) {
	n = F_NAMELEN;
    }
    put4(theinfo.fi_namlen, (uint)n);
    strncpy((char *)theinfo.fi_name, buffer + I_NAMEOFF + 1,n);
    /* theinfo.fi_macfilename[n] = '\0'; */
    strcpy((char *)theinfo.fi_comment,
	"Converted by Unix utility to AppleDouble format");
    put4(theinfo.fi_commsize, (uint)strlen(theinfo.fi_comment));
    put4(theinfo.fi_rsrc, (uint)rsz);
    /*  Still TODO */
    /*  char	fi_ctime[4];	/* Creation time (Unix time) */
    /*  char	fi_mtime[4];	/* Modification time (Unix time) */
    fwrite((char *) &theinfo, 1, sizeof theinfo, fp);
}
#endif

private void mk_share_name()
{
    int ch;
    char *mp, *up;

    mp = buffer + 2;
    up = &(share_name[0]);
    while(ch = *mp++) {
	if(isascii(ch) && ! iscntrl(ch) && isprint(ch) && ch != '/') {
	    *up++ = ch;
	} else {
	    *up++ = ':';
	    *up++ = hex[(ch >> 4) & 0xf];
	    *up++ = hex[ch & 0xf];
	}
    }
    *up = 0;
}
#endif

#endif

        int wrfileopt(char c)
        {
            switch (c)
            {
                case 'b':
                    mode = MACB;
                    break;
                case 'r':
                    if (mode_restricted)
                    {
                        return 0;
                    }
                    mode = RSRC;
                    break;
                case 'd':
                    if (mode_restricted)
                    {
                        return 0;
                    }
                    mode = DATA;
                    break;
                case 'u':
                    if (mode_restricted)
                    {
                        return 0;
                    }
                    mode = TEXT;
                    break;
                case 'U':
                    if (mode_restricted)
                    {
                        return 0;
                    }
                    mode = UNIX;
                    break;
                case 'f':
                    mode = FORK;
                    break;
                case '3':
                    mode = FULL;
                    break;
                case 's':
                    if (mode_s_restricted)
                    {
                        return 0;
                    }
                    mode = MACS;
                    break;
#if SCAN
    case 'S':
	if(mode_s_restricted) {
	    return 0;
	}
	mode = MACI;
	break;
#endif
                case 'a':
#if APPLESHARE
#if AUFS
	check_aufs();
	mode = APSH;
	break;
#endif
#if APPLEDOUBLE
	check_appledouble();
	mode = APSH;
	break;
#endif
#else
                    Console.Error.WriteLine("Sorry, Apple-Unix sharing is not supported.");
                    Console.Error.WriteLine("Recompile or omit -a option.");
                    exit(1);
                    break;
#endif
                default:
                    return 0;
            }
            return 1;
        }

        void give_wrfileopt()
        {
            Console.Error.WriteLine("File output options:");
            Console.Error.WriteLine("-b:\tMacBinary (default)");
            if (!mode_s_restricted)
            {
                Console.Error.WriteLine("-s:\tMacBinary stream to standard output");
#if SCAN
	fprintf(stderr,
	    "-S:\tas -s but with indication of orignal Unix filename\n");
#endif
            }
            Console.Error.WriteLine("-f:\tthree fork mode, skipping empty forks");
            Console.Error.WriteLine("-3:\tthe same, writing also empty forks");
            if (!mode_restricted)
            {
                Console.Error.WriteLine("-r:\tresource forks only");
                Console.Error.WriteLine("-d:\tdata forks only");
                Console.Error.WriteLine("-u:\tdata forks only with Mac -> Unix text file translation");
                Console.Error.WriteLine("-U:\tas -u, but filename will not have an extension");
            }
#if APPLESHARE
#if AUFS
    Console.Error.WriteLine("-a:\tAUFS format");
#endif
#if APPLEDOUBLE
    Console.Error.WriteLine("-a:\tAppleDouble format");
#endif
#else
            Console.Error.WriteLine("-a:\tnot supported, needs recompilation");
#endif
        }

        void set_wrfileopt(bool restricted)
        {
            mode_restricted = restricted;
        }

        void set_s_wrfileopt(bool restricted)
        {
            mode_s_restricted = restricted;
        }

        private string options;

        string get_wrfileopt()
        {

            options = "b";
            if (!mode_s_restricted)
            {
                options += "s";

            }
            options += "f3";
            if (!mode_restricted)
            {
                options += "rduU";
            }
            options += "a";
            return options;
        }

        string get_mina()
        {
#if APPLESHARE
#if AUFS
    return ", AUFS supported";
#endif
#if APPLEDOUBLE
    return ", AppleDouble supported";
#endif
#else
            return ", no Apple-Unix sharing supported";
#endif
        }

        const int INFOBYTES = 128;

        /* The following are copied out of macput.c/macget.c */
        const int I_NAMEOFF = 1;
        /* 65 <-> 80 is the FInfo structure */
        const int I_TYPEOFF = 65;
        const int I_AUTHOFF = 69;
        const int I_FLAGOFF = 73;
        const int I_LOCKOFF = 81;
        const int I_DLENOFF = 83;
        const int I_RLENOFF = 87;
        const int I_CTIMOFF = 91;
        const int I_MTIMOFF = 95;

        const int F_NAMELEN = 63;
        const int I_NAMELEN = 69    /* 63 + strlen(".info") + 1 */;

        const byte INITED_MASK = 1;
        const int PROTCT_MASK = 0x40;


        byte getb(Stream infp)
        {
            int by = infp.ReadByte();
            if (by < 0)
                throw new InvalidOperationException("Unexpected end of file.");
            return (byte)by;
        }


        public byte[] DataFork
        {
            get { return dbuffer; }
        }


        public byte[] ResourceFork
        {
            get { return rbuffer; }
        }
    }
}
