using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Hunk
{
    public class Unit
    {
        public int unit_no;
        public string? name;
        public short hunk_begin_offset;
        public List<IHunk>? hunk_infos;
        public List<List<Hunk>>? segments;
        public Hunk? unit;
    }
}
