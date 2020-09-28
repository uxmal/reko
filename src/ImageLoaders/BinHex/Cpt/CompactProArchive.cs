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

using Reko.Core.Archives;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.BinHex.Cpt
{
    public class CompactProArchive : Archive
    {
        private byte[] cpt_data;
        private uint cpt_datamax;
        private uint cpt_datasize;

        const int BYTEMASK = 0xFF;

        CrcUpdater updcrc;
        delegate uint CrcUpdater(uint crc, byte[] buf, int offset, int length);
        uint cpt_crc;

        private uint zip_updcrc(uint crc, byte[] buf, int offset, int length)
        {
            return crc;
        }

        public List<ArchiveDirectoryEntry> Load(Stream infp)
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
            if (readcpthdr(infp, out cpthdr) == 0)
            {
                throw new ApplicationException("Can't read archive header..");
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
                throw new ApplicationException("Can't read comment..");
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
                    throw new ApplicationException(string.Format("Can't read file header #{0}.", i + 1));
                cpt_crc = updcrc(cpt_crc, cptindex, cptptr + 1, cptindex[cptptr]);
                if (cptindex[cptptr + F_FOLDER] != 0)
                {
                    if (infp.Read(cptindex, cptptr + F_FOLDERSIZE, 2) != 2)
                        throw new ApplicationException(string.Format("Can't read file header #{0}.", i + 1));
                    cpt_crc = updcrc(cpt_crc, cptindex, cptptr + F_FOLDERSIZE, 2);
                }
                else
                {
                    if (infp.Read(cptindex, cptptr + F_VOLUME, FILEHDRSIZE - F_VOLUME) != FILEHDRSIZE - F_VOLUME)
                        throw new ApplicationException(string.Format("Can't read file header #{0}.", i + 1));
                    cpt_crc = updcrc(cpt_crc, cptindex, cptptr + F_VOLUME,
                            FILEHDRSIZE - F_VOLUME);
                }
                cptptr += FILEHDRSIZE;
            }
            //if(cpt_crc != cpthdr.hdrcrc) {
            //Console.Error.WriteLine("Header CRC mismatch: got 0x{0:X8}, need 0x{1:X8}",
            //    (int)cpthdr.hdrcrc, (int)cpt_crc);

            //exit(1);
            //}

            List<ArchiveDirectoryEntry> entries = new List<ArchiveDirectoryEntry>();
            cptptr = 0; // cptindex;
            for (i = 0; i < cpthdr.entries; i++)
            {
                if (cpt_filehdr(out filehdr, cptindex, cptptr) == -1)
                {
                    throw new ApplicationException(string.Format("Can't read file header #{0}", i + 1));
                }
                if (filehdr.folder != 0)
                {
                    entries.Add(cpt_folder(text, filehdr, cptindex, cptptr));
                    i += filehdr.foldersize;
                    cptptr += filehdr.foldersize * FILEHDRSIZE;

                }
                else
                {
                    if ((filehdr.cptFlag & 1) == 1)
                    {
                        Console.Error.WriteLine("\tFile is password protected, skipping file");
                    }
                    else
                    {
                        entries.Add(new MacFileEntry(this, filehdr));
                    }
                }
                cptptr += FILEHDRSIZE;
            }
            return entries;
        }

        private int readcpthdr(Stream infp, out CptHdr s)
        {
            s = new CptHdr();
            byte[] temp = new byte[CHDRSIZE];

            if (infp.Read(temp, 0, CPTHDRSIZE) != CPTHDRSIZE)
            {
                return 0;
            }

            if (temp[C_SIGNATURE] != 1)
            {
                Console.Error.WriteLine("Not a Compactor file.");
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
            return Encoding.ASCII.GetString(buf, offset, length);
        }

        string text;

        private int cpt_filehdr(out FileHdr f, byte[] hdr, int hdrOff)
        {
            f = new FileHdr();

            info = new byte[INFOBYTES];

            int n = hdr[hdrOff + F_FNAME] & BYTEMASK;
            if (n > F_NAMELEN)
            {
                n = F_NAMELEN;
            }
            info[I_NAMEOFF] = (byte)n;
            copy(info, I_NAMEOFF + 1, hdr, hdrOff + F_FNAME + 1, n);
            text = transname(hdr, hdrOff + F_FNAME + 1, n);

            f.folder = hdr[hdrOff + F_FOLDER];
            f.fName = text;
            if (f.folder != 0)
            {
                f.foldersize = get2(hdr, hdrOff + F_FOLDERSIZE);
            }
            else
            {
                f.fName = text;
                f.cptFlag = get2(hdr, hdrOff + F_CPTFLAG);
                f.rsrcLength = get4(hdr, hdrOff + F_RSRCLENGTH);
                f.dataLength = get4(hdr, hdrOff + F_DATALENGTH);
                f.compRLength = get4(hdr, hdrOff + F_COMPRLENGTH);
                f.compDLength = get4(hdr, hdrOff + F_COMPDLENGTH);
                f.fType = transname(hdr, hdrOff + F_FTYPE, 4);
                f.fCreator = transname(hdr, hdrOff + F_CREATOR, 4);
                f.fileCRC = get4(hdr, hdrOff + F_FILECRC);
                f.FndrFlags = get2(hdr, hdrOff + F_FNDRFLAGS);
                f.filepos = get4(hdr, hdrOff + F_FILEPOS);
                f.volume = hdr[hdrOff + F_VOLUME];
            }

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
            return 1;
        }

        private void copy(byte[] dst, int dstOff, byte[] src, int srcOff, int cb)
        {
            Array.Copy(src, srcOff, dst, dstOff, cb);
        }

        private MacFolderEntry cpt_folder(string name, FileHdr fileh, byte[] cptindex, int cptptr)
        {
            int nfiles;
            FileHdr filehdr;

            List<ArchiveDirectoryEntry> entries = new List<ArchiveDirectoryEntry>();

            cptptr += FILEHDRSIZE;
            nfiles = fileh.foldersize;
            for (int i = 0; i < nfiles; i++, cptptr += FILEHDRSIZE)
            {
                if (cpt_filehdr(out filehdr, cptindex, cptptr) == -1)
                {
                    throw new ApplicationException(string.Format("Can't read file header #{0}", i + 1));
                }
                if (filehdr.folder != 0)
                {
                    entries.Add(cpt_folder(text, filehdr, cptindex, cptptr));
                    i += filehdr.foldersize;
                    cptptr += filehdr.foldersize * FILEHDRSIZE;
                }
                else
                {
                    if ((filehdr.cptFlag & 1) == 1)
                    {
                        Console.Error.WriteLine("File is password protected, skipping file");
                    }
                    else
                    {
                        entries.Add(new MacFileEntry(this, filehdr));
                    }
                }
            }
            return new MacFolderEntry(name, entries);
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

        public class CptHdr
        {			// 8 bytes
            public byte signature;	    // = 1 -- for verification 
            public byte volume;		    // for multi-file archives 
            public ushort xmagic;		// verification multi-file consistency
            public uint offset;		    // index offset 
            // The following are really in header2 at offset 
            public uint hdrcrc;		    // header crc 
            public ushort entries;	    // number of index entries
            public byte commentsize;	// number of bytes comment that follow
        }



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

        public class FileHdr // 78 bytes
        {
            public string fName;	/* a STR32 */
            public byte folder;		/* set to 1 if a folder */
            public ushort foldersize;	/* number of entries in folder */
            public byte volume;		/* for multi-file archives */
            public uint filepos;	/* position of data in file */
            public string fType;			/* file type */
            public string fCreator;		/* er... */
            public uint creationDate;
            public uint modDate;	/* !restored-compat w/backup prgms */
            public ushort FndrFlags;	/* copy of Finder flags.  For our
						purposes, we can clear:
						busy,onDesk */
            public uint fileCRC;	// crc on file 
            public ushort cptFlag;	// cpt flags 
            public uint rsrcLength;	// decompressed lengths
            public uint dataLength;
            public uint compRLength;	// compressed lengths 
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


        public class MacFileEntry : ArchivedFolder
        {
            private CompactProArchive archive;
            private FileHdr hdr;
            private ForkFolder forks;

            public MacFileEntry(CompactProArchive archive, FileHdr hdr)
            {
                this.archive = archive;
                this.hdr = hdr;
                this.forks = new ForkFolder(archive, hdr);
            }

            public string Name { get { return hdr.fName; } set { hdr.fName = value; } }

            public ICollection<ArchiveDirectoryEntry> Items { get { return forks; } }

            private class ForkEntry : ArchivedFile
            {
                private CompactProArchive archive;
                private uint offsetCompressed;
                private uint lengthCompressed;
                private uint lengthUncompressed;
                private ushort type;
                private string name;

                public ForkEntry(
                    CompactProArchive archive,
                    string name,
                    uint offsetCompressed,
                    uint lengthCompressed,
                    uint lengthUncompressed,
                    ushort type)
                {
                    this.archive = archive;
                    this.name = name;
                    this.offsetCompressed = offsetCompressed;
                    this.lengthCompressed = lengthCompressed;
                    this.lengthUncompressed = lengthUncompressed;
                    this.type = type;
                }

                public string Name { get { return name; } }

                public uint Size { get { return lengthUncompressed; } }

                public byte[] GetBytes()
                {
                    return archive.CreateUncompactor().Uncompact(offsetCompressed, lengthCompressed, lengthUncompressed, type);
                }
            }

            public class ForkFolder : ICollection<ArchiveDirectoryEntry>
            {
                private ForkEntry dataFork;
                private ForkEntry rsrcFork;

                public ForkFolder(CompactProArchive archive, FileHdr hdr)
                {
                    rsrcFork = new ForkEntry(archive, "rsrc", hdr.filepos, hdr.compRLength, hdr.rsrcLength, ((ushort) (hdr.cptFlag & 2)));
                    dataFork = new ForkEntry(archive, "data", hdr.filepos + hdr.compRLength, hdr.compDLength, hdr.dataLength, ((ushort)(hdr.cptFlag & 4)));
                }

                public IEnumerator<ArchiveDirectoryEntry> GetEnumerator()
                {
                    yield return rsrcFork;
                    yield return dataFork;
                }

                public int Count
                {
                    get { return 2; }
                }

                public ArchiveDirectoryEntry this[string entryName]
                {
                    get
                    {
                        if (entryName == "data")
                            return dataFork;
                        else if (entryName == "rsrc")
                            return rsrcFork;
                        else
                            return null;
                    }
                }

                #region IEnumerable Members

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion

                #region ICollection<ArchiveDirectoryEntry> Members

                void ICollection<ArchiveDirectoryEntry>.Add(ArchiveDirectoryEntry item)
                {
                    throw new NotImplementedException();
                }

                void ICollection<ArchiveDirectoryEntry>.Clear()
                {
                    throw new NotImplementedException();
                }

                bool ICollection<ArchiveDirectoryEntry>.Contains(ArchiveDirectoryEntry item)
                {
                    throw new NotImplementedException();
                }

                void ICollection<ArchiveDirectoryEntry>.CopyTo(ArchiveDirectoryEntry[] array, int arrayIndex)
                {
                    throw new NotImplementedException();
                }

                bool ICollection<ArchiveDirectoryEntry>.IsReadOnly
                {
                    get { return true; }
                }

                bool ICollection<ArchiveDirectoryEntry>.Remove(ArchiveDirectoryEntry item)
                {
                    throw new NotImplementedException();
                }

                #endregion
            }
        }

        public class MacFolderEntry : ArchivedFolder
        {
            private string name;
            private ICollection<ArchiveDirectoryEntry> items;

            public MacFolderEntry(string name, ICollection<ArchiveDirectoryEntry> items)
            {
                this.name = name;
                this.items = items;
            }

            public ICollection<ArchiveDirectoryEntry> Items
            {
                get { return items; }
            }

            public string Name
            {
                get { return name; }
            }

        }


        public CptCompressor CreateUncompactor()
        {
            return new CptCompressor(cpt_data);
        }
    }
}
