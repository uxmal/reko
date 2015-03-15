using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.ImageLoaders.Elf
{
    public class Elf32_Dyn
    {
        public int d_tag; /* how to interpret value */
        private int val;
        public int d_val { get { return val; } set { val = value; } }
        public uint d_ptr { get { return (uint) val; } set { val = (int)value; } }
        public int d_off { get { return val; } set { val = value; } }
    }
}
