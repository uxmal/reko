using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Hunk
{
    public class HunkFile
    {
        public FileType type;
        public List<Hunk> hunks;
        public HeaderHunk header;
        public List<List<Hunk>> segments;
        public Hunk overlay;
        public List<HeaderHunk> overlay_headers;
        public List<List<List<Hunk>>> overlay_segments;
        public List<Lib> libs;
        public List<Unit> units;

        public HunkFile()
        {
            this.type = FileType.TYPE_UNKNOWN;
            this.hunks = new List<Hunk>();
            this.header = null;
            this.segments = new List<List<Hunk>>();
            this.overlay = null;
            this.overlay_headers = null;
            this.overlay_segments = null;
            this.libs = null;
            this.units = null;

        }
    }

}
