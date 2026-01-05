#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core.Loading;
using System.IO;

namespace Reko.ImageLoaders.MzExe.Pe;

internal class PeBinaryDumper : IBinaryDumper
{
    private PeBinaryImage peBinaryImage;

    public PeBinaryDumper(PeBinaryImage peBinaryImage)
    {
        this.peBinaryImage = peBinaryImage;
    }

    public void DisassembleContents(TextWriter writer, bool allSections)
    {
        throw new System.NotImplementedException();
    }

    public void DumpAllHeaders(TextWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public void DumpDynamicRelocations(TextWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public void DumpDynamicSymbols(TextWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public void DumpFileHeader(TextWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public void DumpImageFormat(TextWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public void DumpPrivateHeaders(TextWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public void DumpRelocations(TextWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public void DumpSectionContents(TextWriter writer, bool allSections)
    {
        throw new System.NotImplementedException();
    }

    public void DumpSectionHeaders(TextWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public void DumpSymbols(IBinaryImage image, TextWriter writer)
    {
        throw new System.NotImplementedException();
    }

    /*
      /ALL
      /ARCHIVEMEMBERS
      /CLRHEADER
      /DEPENDENTS
      /DIRECTIVES
      /DISASM[:{BYTES|NOBYTES|NOWIDE|WIDE}]
      /ERRORREPORT:{NONE|PROMPT|QUEUE|SEND}
      /EXPORTS
      /FPO
      /HEADERS
      /IMPORTS[:filename]
      /LINENUMBERS
      /LINKERMEMBER[:{1|2|4|8|16|32}]
      /LOADCONFIG
      /NOLOGO
      /NOPDB
      /NOSECTION:name
      /OUT:filename
      /OTHERARCHEXPORTS (requires /EXPORTS)
      /OTHERARCHMETADATA:{REDIRECTION|THUNKMAPPINGS|ALL} (requires /LOADCONFIG)
      /OTHERLOADCONFIG (requires /LOADCONFIG)
      /PDATA
      /PDBPATH[:VERBOSE]
      /RANGE:vaMin[,vaMax]
      /RAWDATA[:{NONE|1|2|4|8}[,#]]
    */
}