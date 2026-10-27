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

using Reko.Arch.Dec.Memory;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;

namespace Reko.Arch.Dec.ImageLoaders;

public class LincIndexEntry : ArchivedFile
{
    private readonly LincTape tape;

    public LincIndexEntry(LincTape tape, string name, ushort startBlock, ushort cBlocks, ushort execAddress, ushort typeId)
    {
        this.tape = tape;
        Name = name;
        StartBlock = startBlock;
        Length = cBlocks;
        ExecAddress = execAddress;
        TypeId = typeId;
    }

    public long Length { get; }

    public string Name { get; }

    public ArchiveDirectoryEntry? Parent => null;

    public ushort StartBlock { get; }
    public ushort ExecAddress { get; }
    public ushort TypeId { get; }

    public byte[] GetBytes()
    {
        throw new NotImplementedException();
    }

    public ILoadedImage LoadImage(IServiceProvider services, Address? addrPreferred)
    {
        if (ExecAddress == LincTape.EmptyMarker || this.TypeId == LincTape.EmptyMarker)
        {
            return new Blob(tape.Location.AppendFragment(Name), []);
        }
        var words = new ushort[Length * LincTape.WordsPerBlock];
        if (!tape.TryReadBlocks(words, ExecAddress, TypeId))
        {
            return new Blob(tape.Location.AppendFragment(Name), []);
        }
        var mem = new Word12MemoryArea(LincArchitecture.Ptr12(0), words);
        var segmap = new SegmentMap(
            new ImageSegment("code", mem, AccessMode.ReadWriteExecute));
        var arch = new LincArchitecture(services, "linc", []);
        var platform = new DefaultPlatform(services, arch);
        var program = new Program(
            new ProgramMemory(segmap),
            arch,
            platform);
        program.Location = tape.Location.AppendFragment(Name);
        program.Name = Name;
        return program;
    }

    private static string oct(char prefix, uint value)
    {
        if (value == LincTape.EmptyMarker)
            return "     ";
        return $"{prefix}:{Convert.ToString(value, 8).PadLeft(3, '0')}";
    }

    public override string ToString()
    {
        return $"{Name,-8} {oct('s',StartBlock)} {oct('l',(uint)Length)} {oct('x', ExecAddress)} {oct('t', TypeId)}";
    }
}
