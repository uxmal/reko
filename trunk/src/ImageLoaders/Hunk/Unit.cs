using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Hunk
{
    public class Unit
    {
        public int unit_no;
        public uint hunk_begin_offset;
        public List<Segment> segments;
        public Hunk unit;
        public string name;
        public List<Hunk> hunk_infos;
        public Unit index_unit;
    }
}
