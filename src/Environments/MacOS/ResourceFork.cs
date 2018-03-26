#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Arch.M68k;
using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Environments.MacOS
{
// http://developer.apple.com/legacy/mac/library/documentation/mac/MoreToolbox/MoreToolbox-99.html

    /// <summary>
    /// This class knows how to unpack a resource fork into its constituent resources.
    /// </summary>
    public class ResourceFork
    {
        private byte[]  image;
        private MacOSClassic platform;
        private IProcessorArchitecture arch;
        private ResourceTypeCollection rsrcTypes;

        uint rsrcDataOff;
        uint rsrcMapOff;
        uint dataSize;
        uint mapSize;
        private object a5dr;

        public ResourceFork(MacOSClassic platform, byte[] bytes)
        {
            this.image = bytes;
            this.platform = platform;
            this.arch = platform.Architecture;

            rsrcDataOff = MemoryArea.ReadBeUInt32(bytes, 0);
            rsrcMapOff = MemoryArea.ReadBeUInt32(bytes, 4);
            dataSize = MemoryArea.ReadBeUInt32(bytes, 8);
            mapSize = MemoryArea.ReadBeUInt32(bytes, 0x0C);

            rsrcTypes = new ResourceTypeCollection(image, rsrcMapOff, mapSize);
        }

        public ResourceTypeCollection ResourceTypes
        {
            get { return rsrcTypes; }
        }

        public class ResourceType
        {
            public ResourceType(byte[] bytes, string name, uint offset, uint nameListOffset, int crsrc)
            {
                this.References = new ReferenceCollection(bytes, offset, nameListOffset, crsrc);
                this.Name = name;
            }

            public string Name { get; }

            public ReferenceCollection References { get; }
        }

        public class ResourceReference
        {
            private ushort rsrcID; 
            private string name; 
            private uint offset;
            
            public ResourceReference(ushort rsrcID, string name, uint offset)
            {
                this.rsrcID = rsrcID;
                this.name = name;
                this.offset = offset;
            }

            public ushort ResourceID { get { return rsrcID; } }
            public string Name { get { return name; } }
            public uint DataOffset { get { return offset; } }
        }

        public class ResourceTypeCollection : ICollection<ResourceType>
        {
            private byte[] bytes;
                uint rsrcTypeListOff ;
                uint rsrcNameListOff;
                int crsrcTypes;

            public ResourceTypeCollection(byte [] bytes, uint offset, uint size)
            {
                this.bytes = bytes;
                rsrcTypeListOff = offset + MemoryArea.ReadBeUInt16(bytes, offset + 0x18) + 2u;
                rsrcNameListOff = offset + MemoryArea.ReadBeUInt16(bytes, offset + 0x1A);
                crsrcTypes = MemoryArea.ReadBeUInt16(bytes, offset + 0x1C) + 1;
            }

            #region ICollection<ResourceType> Members

            void ICollection<ResourceType>.Add(ResourceType item)
            {
                throw new NotSupportedException();
            }

            void ICollection<ResourceType>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<ResourceType>.Contains(ResourceType item)
            {
                throw new NotSupportedException();
            }

            public void CopyTo(ResourceType[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return crsrcTypes; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            bool ICollection<ResourceType>.Remove(ResourceType item)
            {
                throw new NotSupportedException();
            }

            #endregion

            #region IEnumerable<ResourceType> Members

            public IEnumerator<ResourceType> GetEnumerator()
            {
                var offset = rsrcTypeListOff;
                for (int i = 0; i < crsrcTypes; ++i)
                {
                    string rsrcTypeName = Encoding.ASCII.GetString(bytes, (int)offset, 4);
                    int crsrc = MemoryArea.ReadBeUInt16(bytes, offset + 4) + 1;
                    uint rsrcReferenceListOffset = rsrcTypeListOff + MemoryArea.ReadBeUInt16(bytes, offset + 6) - 2;
                    yield return new ResourceType(bytes, rsrcTypeName, rsrcReferenceListOffset, rsrcNameListOff, crsrc);
                    offset += 8;
                }
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        public class ReferenceCollection : ICollection<ResourceReference>
        {
            private byte [] bytes;
            private uint offset;
            private uint nameListOffset;
            private int count;

            public ReferenceCollection(byte [] bytes, uint offset, uint nameListOffset, int count)
            {
                this.bytes = bytes;
                this.offset  = offset;
                this.nameListOffset = nameListOffset;
                this.count = count;
            }

            #region ICollection<ResourceReference> Members

            void ICollection<ResourceReference>.Add(ResourceReference item)
            {
                throw new NotSupportedException();
            }

            void ICollection<ResourceReference>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<ResourceReference>.Contains(ResourceReference item)
            {
                throw new NotImplementedException();
            }

            void ICollection<ResourceReference>.CopyTo(ResourceReference[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            bool ICollection<ResourceReference>.Remove(ResourceReference item)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable<ResourceReference> Members

            public IEnumerator<ResourceReference> GetEnumerator()
            {
                var offset = this.offset;
                for (int i = 0; i < count; ++i)
                {
                    ushort rsrcID = MemoryArea.ReadBeUInt16(bytes, offset);
                    string name = ReadName(MemoryArea.ReadBeUInt16(bytes, offset + 2));
                    uint dataOff = MemoryArea.ReadBeUInt32(bytes, offset + 4) & 0x00FFFFFFU;
                    yield return new ResourceReference(rsrcID, name, dataOff);

                    offset += 0x0C;
                }
            }

            private string ReadName(ushort rsrcInstanceNameOffset)
            {
                if (rsrcInstanceNameOffset == 0xFFFF)
                    return null;
                uint nameOff = this.nameListOffset + rsrcInstanceNameOffset;
                byte length = bytes[nameOff];
                return Encoding.ASCII.GetString(bytes, (int) nameOff + 1, length);
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public void Dump()
        {
            foreach (ResourceType type in ResourceTypes)
            {
                Debug.WriteLine(string.Format("Resource type {0}", type.Name));
                foreach (ResourceReference rsrc in type.References)
                {
                    Debug.WriteLine(string.Format("   Resource ID: {0:X4}", rsrc.ResourceID));
                    Debug.WriteLine(string.Format("   Name: {0}", rsrc.Name));
                    Debug.WriteLine(string.Format("   Offset: {0:X8}", rsrc.DataOffset));
                    Debug.WriteLine("    ====");
                }
            }
        }

        public void AddResourcesToImageMap(
            Address addrLoad, 
            MemoryArea mem,
            SegmentMap segmentMap, 
            List<ImageSymbol> entryPoints,
            SortedList<Address, ImageSymbol> symbols)
        {
            JumpTable jt = null;
            var codeSegs = new Dictionary<int, ImageSegment>();
            foreach (ResourceType type in ResourceTypes)
            {
                foreach (ResourceReference rsrc in type.References)
                {
                    uint uSegOffset = rsrc.DataOffset + rsrcDataOff;
                    int segmentSize = mem.ReadBeInt32(uSegOffset);

                    // Skip the size longword at the beginning of the segment, and an extra 4 
                    // if it is a code segement other than CODE#0.
                    int bytesToSkip = 4;
                    if (type.Name == "CODE" && rsrc.ResourceID != 0)
                        bytesToSkip += 4;
                    var abSegment = new byte[segmentSize];
                    Array.Copy(mem.Bytes, uSegOffset + bytesToSkip, abSegment, 0, segmentSize);

                    Address addrSegment = addrLoad + uSegOffset + bytesToSkip;    //$TODO pad to 16-byte boundary?
                    var memSeg = new MemoryArea(addrSegment, abSegment);
                    var segment = segmentMap.AddSegment(new ImageSegment(
                        ResourceDescriptiveName(type, rsrc),
                        memSeg,
                        AccessMode.Read));
                    if (type.Name == "CODE")
                    {
                        if (rsrc.ResourceID == 0)
                        {
                            jt = ProcessJumpTable(memSeg, symbols);
                        }
                        else
                        {
                            codeSegs.Add(rsrc.ResourceID, segment);
                            var macsBug = new MacsBugSymbolScanner(segment.MemoryArea);
                            var mbSymbols = macsBug.ScanForSymbols();
                            foreach (var symbol in mbSymbols)
                            {
                                symbols[symbol.Address] = symbol;
                            }
                        }
                    }
                }
            }

            // We have found a jump table, so we allocate an A5World.
            if (jt != null)
            {
                // Find an address beyond all known segments.
                var addr = segmentMap.Segments.Values.Max(s => s.Address + s.Size).Align(0x10);
                var a5world = LoadA5World(jt, addr, codeSegs, symbols);
                platform.A5World = a5world;
                platform.A5Offset = jt.BelowA5Size;
                segmentMap.AddSegment(a5world);

                // Check for %A5Init CODE Segment and unpack global data to A5World
                foreach ( var a5dataseg  in segmentMap.Segments.Values)
                {
                    if (a5dataseg.Name.Length >= 12 )
                    {
                        if (a5dataseg.Name.Substring(5,7) == "%A5Init")
                        {
                            var a5data = a5dataseg.MemoryArea;
                            var a5dr = a5data.CreateBeReader(0);
                            var a5d0 = a5dr.ReadBeUInt32();
                            var a5d1 = a5dr.ReadBeUInt16();
                            var a5dhdroffset = a5dr.ReadBeUInt16();
                            if (a5d0 == 0x48e77ff8 && a5d1 == 0x49fa && a5dhdroffset < a5dataseg.MemoryArea.Length)
                            {
                                a5dr.Seek(a5dhdroffset - 2);
                                var a5dbelow = a5dr.ReadBeUInt32();
                                var a5dbanksize = a5dr.ReadBeUInt32();
                                var a5doffset = a5dr.ReadBeUInt32();
                                var a5dreloc = a5dr.ReadBeUInt32();
                                var a5dhdrend = a5dr.ReadBeUInt32();
                                if (a5dbanksize == 0x00010000)
                                {
                                    var a5worldbaseoffset = a5world.MemoryArea.BaseAddress + jt.BelowA5Size;
                                    var a5beloww = new BeImageWriter(a5world.MemoryArea, jt.BelowA5Size - a5dbelow);
                                    int a5rl1 = 0;
                                    int a5rl2 = 0;
                                    int a5rpt = 0;
                                    var a5copylength = 0;
                                    bool a5dend = false;

                                    do
                                    {
                                        a5rpt = 1;
                                        a5rl1 = a5dr.ReadByte();
                                        a5rl2 = a5rl1;
                                        a5rl1 = a5rl1 & 0x0F;
                                        if (a5rl1 == 0)
                                        {
                                            a5rl1 = GetRL(a5dr, ref a5rpt);
                                            if (a5rl1 == 0)
                                            {
                                                a5dend = true;
                                            }

                                        }
                                        else
                                        {
                                            a5rl1 = a5rl1 * 2;
                                        }
                                        if (!a5dend)
                                        {
                                            a5rl2 = a5rl2 & 0xF0;
                                            if (a5rl2 == 0)
                                            {
                                                a5rl2 = GetRL(a5dr, ref a5rpt);
                                            }
                                            else
                                            {
                                                a5rl2 = a5rl2 >> 3;
                                            }
                                            do
                                            {
                                                a5beloww.Position = a5beloww.Position + a5rl2;
                                                a5copylength = a5rl1;
                                                do
                                                {
                                                    a5beloww.WriteByte(a5dr.ReadByte());
                                                    a5copylength -= 1;
                                                } while (a5copylength > 0);
                                                a5rpt -= 1;
                                            } while (a5rpt > 0);
                                        }
                                    } while (!a5dend);
                                }
                            }
                        }
                    }
                }
                    
            }
        }

        private int GetRL(BeImageReader a5dr, ref int a5rpt)
        {
            int RL = a5dr.ReadByte();
            if ((RL & 0x80) == 0)
            {
                return RL;
            }
            if ((RL & 0x40) == 0)
            {
                RL = RL & 0x3F;
                RL = (RL << 8) + a5dr.ReadByte();
                return RL;
            }
            if ((RL & 0x20) == 0)
            {
                RL = RL & 0x1F;
                RL = (RL << 16) + a5dr.ReadBeInt16();
                return RL;
            }
            if ((RL & 0x10) == 0)
            {
                return a5dr.ReadBeInt32();
            }
            RL = GetRL(a5dr, ref a5rpt);
            a5rpt = GetRL(a5dr, ref a5rpt);
            return RL;
        }

        /// <summary>
        /// Builds the MacOS A5World, based on the contents of the jump table in CODE#0.
        /// </summary>
        /// <param name="jt"></param>
        /// <param name="addr"></param>
        /// <param name="codeSegs"></param>
        /// <returns></returns>
        public ImageSegment LoadA5World(
            JumpTable jt, 
            Address addr, 
            Dictionary<int, ImageSegment> codeSegs,
            SortedList<Address, ImageSymbol> symbols)
        {
            var size = jt.BelowA5Size + jt.AboveA5Size;
            var mem = new MemoryArea(addr, new byte[size]);
            var w = new BeImageWriter(mem, jt.BelowA5Size + jt.JumpTableOffset);
            int i = 0;
            foreach (var entry in jt.Entries)
            {
                w.WriteBeUInt16(entry.RoutineOffsetFromSegmentStart);
                int iSeg = (ushort)entry.Instruction;
                var targetSeg = codeSegs[iSeg];
                var addrDst = targetSeg.Address + entry.RoutineOffsetFromSegmentStart;
                if (!symbols.ContainsKey(addrDst))
                {
                    symbols.Add(addrDst, new ImageSymbol(addrDst) { Type = SymbolType.Procedure });
                }
                w.WriteBeUInt16(0x4EF9);            // jmp (xxxxxxxxx).L
                w.WriteBeUInt32(addrDst.ToUInt32());
                ++i;
            }
            return new ImageSegment("A5World", mem, AccessMode.ReadWriteExecute);
        }

        public class JumpTable
        {
            public uint AboveA5Size;
            public uint BelowA5Size;
            public uint JumpTableSize;
            public uint JumpTableOffset;

            public List<JumpTableEntry> Entries = new List<JumpTableEntry>();
        }

        public class JumpTableEntry
        {
            public ushort RoutineOffsetFromSegmentStart;
            public ulong Instruction;
            public ushort LoadSegTrapNumber;

            public override string ToString()
            {
                return string.Format("Jump table entry: {0:X4} {1:X4} {2:X2}", RoutineOffsetFromSegmentStart, Instruction, LoadSegTrapNumber);
            }
        }

        private JumpTable ProcessJumpTable(MemoryArea memJt, IDictionary<Address, ImageSymbol> symbols)
        {
            var j = new JumpTable();
            var ir = memJt.CreateBeReader(0);
            j.AboveA5Size = ir.ReadBeUInt32();
            j.BelowA5Size = ir.ReadBeUInt32();
            j.JumpTableSize = ir.ReadBeUInt32();
            j.JumpTableOffset = ir.ReadBeUInt32();
            uint size = j.JumpTableSize;
            while (size > 0)
            {
                JumpTableEntry jte = new JumpTableEntry();
                var addr = ir.Address;
                jte.RoutineOffsetFromSegmentStart = ir.ReadBeUInt16();
                jte.Instruction = ir.ReadBeUInt32();
                jte.LoadSegTrapNumber = ir.ReadBeUInt16();
                j.Entries.Add(jte);
                Debug.Print("{0}", jte);
                size -= 8;
            }
            return j;
        }

        private string ResourceDescriptiveName(ResourceType type, ResourceReference rsrc)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(type.Name);
            sb.Append(':');
            if (rsrc.Name != null)
            {
                sb.Append(rsrc.Name);
            }
            sb.Append('#');
            sb.Append(rsrc.ResourceID);
            return sb.ToString();
        }
    }
}
