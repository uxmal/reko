#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Environments.MacOS
{
// http://developer.apple.com/legacy/mac/library/documentation/mac/MoreToolbox/MoreToolbox-99.html

    public class ResourceFork
    {
        private byte[]  image;
        private IProcessorArchitecture arch;
        private ResourceTypeCollection rsrcTypes;

        uint rsrcDataOff;
        uint rsrcMapOff;
        uint dataSize;
        uint mapSize;


        public ResourceFork(byte[] bytes, IProcessorArchitecture arch)
        {
            this.image = bytes;
            this.arch = arch;

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
            private string name;
            private ReferenceCollection references;

            public ResourceType(byte[] bytes, string name, uint offset, uint nameListOffset, int crsrc)
            {
                references = new ReferenceCollection(bytes, offset, nameListOffset, crsrc);
                this.name = name;
            }

            public string Name
            {
                get { return name; }
            }

            public ReferenceCollection References
            {
                get { return references; }
            }
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

        public void AddResourcesToImageMap(Address addrLoad, MemoryArea mem, SegmentMap segmentMap, List<ImageSymbol> entryPoints)
        {
            foreach (ResourceType type in ResourceTypes)
            {
                foreach (ResourceReference rsrc in type.References)
                {
                    Address addrSegment = addrLoad + rsrc.DataOffset + rsrcDataOff;
                    var segment = segmentMap.AddSegment(new ImageSegment(
                        ResourceDescriptiveName(type, rsrc),
                        addrSegment,
                        mem,
                        AccessMode.Read));
                    if (type.Name == "CODE")
                    {
                        if (rsrc.ResourceID == 0)
                        {
                            ProcessJumpTable(rsrcDataOff + rsrc.DataOffset + 4);
                        }
                        else
                        {
                            entryPoints.Add(new ImageSymbol(addrSegment + 4));
                        }
                    }
                }
            }
        }

        public class JumpTable
        {
            public uint AboveA5Size;
            public uint BelowA5Size;
            public uint JumpTableSize;
            public uint JumpTableOffset;
        }

        public class JumpTableEntry
        {
            public ushort RoutineOffsetFromSegmentStart;
            public ulong Instruction;
            public ushort LoadSegTrapNumber;
        }

        private void ProcessJumpTable(uint jtOffset)
        {
            var j = new JumpTable();
            EndianImageReader ir = new BeImageReader(image, jtOffset);
            j.AboveA5Size = ir.ReadBeUInt32();
            j.BelowA5Size = ir.ReadBeUInt32();
            j.JumpTableSize = ir.ReadBeUInt32();
            j.JumpTableOffset = ir.ReadBeUInt32();
            uint size = j.JumpTableSize;
            while (size > 0)
            {
                JumpTableEntry jte = new JumpTableEntry();
                jte.RoutineOffsetFromSegmentStart = ir.ReadBeUInt16();
                jte.Instruction = ir.ReadBeUInt32();
                jte.LoadSegTrapNumber = ir.ReadBeUInt16();
                Debug.WriteLine(string.Format("Jump table entry: {0:x2} {1:X4} {2:X2}", jte.RoutineOffsetFromSegmentStart, jte.Instruction, jte.LoadSegTrapNumber));
                size -= 8;
            }
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
