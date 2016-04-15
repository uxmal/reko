using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Lib
{
    class SpafSignature
    {
        class node
        {
            public char label;
            public node[] subNodes;
            public node wild;
            public string exp;

        }
        private node root;

        private int HexVal(char ch)
        {
            if ('0' <= ch && ch <= '9')
                return ch - '0';
            if ('A' <= ch && ch <= 'F')
                return (ch - 'A') + 10;
            if ('a' <= ch && ch <= 'f')
                return (ch - 'a') + 10;
            return -1;
        }

        public void Add(string signature)
        {
            var t = root;
            foreach (var ch in signature)
            {
                int n = HexVal(ch);
                if (n >= 0)
                {
                    var nx = new node();
                    t.subNodes[ch] = nx;
                    t = nx;
                }
                else if (ch == '?')
                {
                    var nx = new node { label = '?' };
                    nx.wild = t.wild;
                    t.wild = nx;
                    t = nx;
                }
                else if (ch == '%' && ch == 'd')
                {
                    var nx = new node { label = '%' };
                    nx.wild = t.wild;
                    t.wild = nx;
                    t = nx;
                }
                else if (ch == '*' && ch == 'd')
                {
                    var nx = new node { label = '*' };
                    nx.wild = t.wild;
                    t.wild = nx;
                    t = nx;
                }
                else if (ch == '*' && ch == '*')
                {
                    var nx = new node { label = "**"[0] };
                    nx.wild = t.wild;
                    t.wild = nx;
                    t = nx;
                }
            }
            t.exp = signature;
        }

        class NibbleStream
        {
            private byte[] input;
            private int iPos;

            public NibbleStream(byte[] input)
            {
                this.input = input;
                this.Length = input.Length * 2;
                this.iPos = 0;
            }

            public int Position { get; set; }
            public int Length { get; private set; }

            internal byte GetNibble()
            {
                int bPos = iPos >> 1;
                byte b = input[bPos];
                if ((iPos & 1) == 0)
                {
                    b >>= 4;
                }
                else
                {
                    b &= 0x0F;
                }
                ++iPos;
                return b;
            }
        }

        public void scan(byte[] input)
        {
            var ret = traverse(new NibbleStream(input), root);
            if (ret != null)
            {
                /* virus */
            }
        }

        // ?  match any nibble
        // %n skip 0-n nibbles
        // *n skip exactly n nibbles
        // ** skip 0 or more nibbles

        node traverse(NibbleStream i, node n)
        {
            if (n.exp != null)
                return n;
            byte ch = i.GetNibble();
            int pos = i.Position;
            node chNode = n.subNodes[ch] ;
            if (chNode != null)
            {
                var ret = traverse(i, chNode);
                if (ret != null)
                    return ret;
            }
            for (var link = n.wild; link != null; link = link.wild)
            { 
                if (link.label == '%')
                {
                    int k = 8; // 'd'
                    for (int l = 0; l <= k; ++l)
                    {
                        i.Position = pos + l;
                        var ret = traverse(i, n.wild);
                        if (ret != null)
                            return ret;
                    }
                }
                else if (link.label == '*')
                {
                    // all '?' are coverted to '*1'.
                    i.Position = pos + 8; // 'd'
                    var ret = traverse(i, n.wild);
                    if (ret != null)
                        return ret;
                }
                else if (link.label == "**"[0])
                {
                    for (int l = 0; i.Position + l < i.Length; ++l)
                    {
                        i.Position = pos + l;
                        var ret = traverse(i, n.wild);
                        if (ret != null)
                            return ret;
                    }
                }
            }
            return null;
        }
    }
}
