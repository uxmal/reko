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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Coff;

/*
 *   Bits for f_flags:
 *
 *	F_RELFLG	relocation info stripped from file
 *	F_EXEC		file is executable  (i.e. no unresolved
 *				externel references)
 *	F_LNNO		line nunbers stripped from file
 *	F_LSYMS		local symbols stripped from file
 *	F_MINMAL	this is a minimal object file (".m") output of fextract
 *	F_UPDATE	this is a fully bound update file, output of ogen
 *	F_SWABD		this file has had its bytes swabbed (in names)
 *	F_AR16WR	this file has the byte ordering of an AR16WR (e.g. 11/70) machine
 *				(it was created there, or was produced by conv)
 *	F_AR32WR	this file has the byte ordering of an AR32WR machine(e.g. vax)
 *	F_AR32W		this file has the byte ordering of an AR32W machine (e.g. 3b,maxi)
 *	F_PATCH		file contains "patch" list in optional header
 *	F_NODF		(minimal file only) no decision functions for
 *				replaced functions

$define F_RELFLG	0000001
$define F_EXEC		0000002
$define F_LNNO		0000004
$define F_LSYMS	0000010
$define F_MINMAL	0000020
$define F_UPDATE	0000040
$define F_SWABD	0000100
$define F_AR16WR	0000200
$define F_AR32WR	0000400
$define F_AR32W	0001000
$define F_PATCH	0002000
$define F_NODF		0002000

     /*
      *   Magic Numbers
      */
public static class MagicNumbers
{
    /* Basic-16 */

    public const ushort B16MAGIC = 0x0142; // 0502;
    public const ushort BTVMAGIC = 0x0143;  // 0503;

    /* x86 */

    public const ushort X86MAGIC = 0x148;   // 0510;
    public const ushort XTVMAGIC = 0x149;   // 0511;

    /* n3b */
    /*
     *   NOTE:   For New 3B, the old values of magic numbers
     *		will be in the optional header in the structure
     *		"aouthdr" (identical to old 3B aouthdr).
     */
    public const ushort N3BMAGIC = 0x168;   // 0550;
    public const ushort NTVMAGIC = 0x169;   // 0551;

    /*  XL  */
    public const ushort XLMAGIC = 0x160;    // 0540;

    /*  MAC-32   3b-5  */

    public const ushort FBOMAGIC = 0x170;   // 0560;
    public const ushort RBOMAGIC = 0x172;   // 0562;
    public const ushort MTVMAGIC = 0x171;   // 0561;


    /* VAX 11/780 and VAX 11/750 */

    /* writeable text segments */
    public const ushort VAXWRMAGIC = 0x178; // 0570;
    /* readonly sharable text segments */
    public const ushort VAXROMAGIC = 0x17D; // 0575;


    /* Motorola 68000 */
    public const ushort MC68MAGIC = 0x150;  // 0520;
    public const ushort MC68TVMAGIC = 0x151;    // 0521;
    public const ushort M68MAGIC = 0x088;   // 0210;
    public const ushort M68TVMAGIC = 0x089; // 0211;


    /* IBM 370 */
    public const ushort U370WRMAGIC = 0x158;    // 0530; /* writeble text segments	*/
    public const ushort U370ROMAGIC = 0x15D;    // 0535; /* readonly sharable text segments	*/

    /* NS32000 */
    public const ushort NS32WRMAGIC = 0x154;    // 0524; /* writeable text segments	*/
    public const ushort NS32ROMAGIC = 0x154;    // 0524; /* readonly shareable text segments */
    public const ushort NS32MDMAGIC = 0x155;    // 0525;	/* module table magic */
}

public class FileHeader
{
    public ushort f_magic;         // magic number
    public ushort f_nscns;         // number of sections
    public uint f_timdat;          // time & date stamp
    public uint f_symptr;          // file pointer to symtab
    public uint f_nsyms;           // number of symtab entries
    public ushort f_opthdr;        // sizeof(optional hdr)
    public ushort f_flags;         // flags
}

