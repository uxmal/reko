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

using Reko.Core;
using Reko.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.ImageLoaders.BinHex.Cpt
{
    public class CptCompressor
    {
        private byte[] cpt_data;
        private int cpt_char;       // index into cpt_data.
        private byte[] out_buffer;
        private int out_ptr;
        private byte[] cpt_LZbuff = new byte[CIRCSIZE];
        private uint cpt_LZptr;


        private uint cpt_inlength;
        private uint cpt_outlength;
        private int cpt_outstat;
        private byte cpt_savechar;
        private uint cpt_newbits;
        private int cpt_bitsavail;
        private int cpt_blocksize;

        // Lengths are twice the max number of entries, and include slack. 
        const int SLACK = 6;
        HuffNode[] cpt_Hufftree = new HuffNode[512 + SLACK];
        HuffNode[] cpt_LZlength = new HuffNode[128 + SLACK];
        HuffNode[] cpt_LZoffs = new HuffNode[256 + SLACK];





        const int BYTEMASK = 0xFF;
        const int CIRCSIZE = 8192;
        const int ESC1 = 0x81;
        const int ESC2 = 0x82;
        const int NONESEEN = 0;
        const int ESC1SEEN = 1;
        const int ESC2SEEN = 2;


        public CptCompressor(byte[] compressedData)
        {
            this.cpt_data = compressedData;
        }

        public byte[] Uncompact(
            uint offsetCompressed,
            uint lengthCompressed,
            uint lengthUncompressed,
            ushort type)
        {
            cpt_char = (int)offsetCompressed;
            byte[] uncompressed = CreateOutputBuffer(lengthUncompressed);
            cpt_wrfile(
                lengthCompressed,
                lengthUncompressed,
                type);
            return uncompressed;
        }


        class HuffNode
        {
            public int flag, Byte;
            public HuffNode one, zero;
        }

        Func<int> get_bit;
        HuffNode[] nodelist = new HuffNode[515]; // 515 because StuffIt Classic needs more than the needed 511


        int gethuffbyte(HuffNode l_nodelist)
        {
            HuffNode np;

            np = l_nodelist;
            while (np.flag == 0)
            {
                np = get_bit() != 0 ? np.one : np.zero;
            }
            return np.Byte;
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



        private void cpt_wrfile(uint ibytes, uint obytes, ushort compressionType)
        {
            if (ibytes == 0)
            {
                return;
            }
            cpt_outstat = NONESEEN;
            cpt_inlength = ibytes;
            cpt_outlength = obytes;
            cpt_LZptr = 0;
            cpt_blocksize = 0x1FFFF0;
            if (compressionType == 0)
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

        //	Run length encoding	
        private void cpt_rle()
        {
            while (cpt_inlength-- > 0)
            {
                cpt_outch(cpt_data[cpt_char++]);
            }
        }

        //	Run length encoding plus LZ compression plus Huffman encoding
        private void cpt_rle_lzh()
        {
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
                int block_count = 0;
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

        // Based on unimplod from unzip; differences are noted below.
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
            sf_entry[] tree_entry = new sf_entry[256 + SLACK]; // maximal number of elements 
            int tree_entries;
            int tree_MaxLength; // finishes local declaration of tree 
            int len;  // declarations from ReadLengths 
            int j;
            int codelen, lvlstart, next, parents;
            int[] tree_count = new int[32];

            // next paraphrased from ReadLengths with adaption for Compactor. 
            int treeBytes = cpt_data[cpt_char++];
            if (size < treeBytes * 2)   // too many entries, something is wrong! 
                throw new ApplicationException(string.Format("Bytes is: {0}, expected: {1}.", treeBytes, size / 2));
            
            tree_MaxLength = 0;
            tree_entries = 0;
            int i = 0;
            while (treeBytes-- > 0)
            { // adaption for Compactor 
                len = cpt_data[cpt_char] >> 4;
                if (len != 0)
                { // only if length unequal zero 
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
                { // only if length unequal zero 
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

            // Compactor allows unused trailing codes in its Huffman tree! 
            j = 0;
            for (i = 0; i <= tree_MaxLength; i++)
            {
                j = (j << 1) + tree_count[i];
            }
            j = (1 << tree_MaxLength) - j;
            // Insert the unused entries for sorting purposes. 
            for (i = 0; i < j; i++)
            {
                tree_entry[tree_entries].Value = size;
                tree_entry[tree_entries++].BitLength = (uint)tree_MaxLength;
            }

            // adaption from SortLengths 
            SortEntries(tree_entry, tree_entries);

            // Adapted from GenerateTrees 
            i = tree_entries - 1;
            // starting at the upper end (and reversing loop) because of Compactor 
            lvlstart = next = size * 2 + SLACK - 1;
            // slight adaption because of different node format used 
            for (codelen = tree_MaxLength; codelen >= 1; --codelen)
            {
                while ((i >= 0) && (tree_entry[i].BitLength == codelen))
                {
                    Hufftree[next] = new HuffNode();
                    Hufftree[next].Byte = tree_entry[i].Value;
                    Hufftree[next].flag = 1;
                    next--;
                    i--;
                }
                parents = next;
                if (codelen > 1)
                {
                    // reversed loop 
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

        private void SortEntries(sf_entry[] tree_entry, int entries)
        {
            for (int i = 0; ++i < entries; )
            {
                sf_entry tmp = tree_entry[i];
                uint a;
                uint b = (uint)tmp.BitLength;
                int j = i;
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
            --cpt_bitsavail;
            if (cpt_bitsavail < 16)
            {
                cpt_newbits |= ((uint)cpt_data[cpt_char++] << 8);
                cpt_newbits |= cpt_data[cpt_char++];
                cpt_bitsavail += 16;
            }
            cpt_newbits <<= 1;
            return b;
        }

        private byte[] CreateOutputBuffer(uint size)
        {
            uint s = (((size + 127) >> 7) << 7);
            out_buffer = new byte[s];
            out_ptr = 0;
            return out_buffer;
        }
    }
}
