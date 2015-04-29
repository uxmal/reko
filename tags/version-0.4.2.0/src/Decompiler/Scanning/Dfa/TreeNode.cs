using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Decompiler.Scanning.Dfa
{
    public class TreeNode
    {
        public NodeType Type;
        public int Number;
        public byte Value;
        public BitArray ValueClass;
        public TreeNode Left;
        public TreeNode Right;
        public bool Nullable;
        public bool Starts;
        public HashSet<TreeNode> FirstPos;
        public HashSet<TreeNode> LastPos;
        public HashSet<TreeNode> FollowPos;

        public IEnumerable<byte> GetTransitionCharacters()
        {
            if (Type == NodeType.Char)
            {
                yield return Value;
            }
            else if (Type == NodeType.CharClass)
            {
                int i = 0;
                foreach (bool x in ValueClass)
                {
                    if (x)
                        yield return (byte) i;
                    ++i;
                }
            } 
            else if (Type == NodeType.Any)
            {
                for (int i = 0; i < 0x100; ++i)
                {
                    yield return (byte) i;
                }
            }
        }

        public override string ToString()
        {
            var sw = new StringWriter();
            Write(sw);
            return sw.ToString();
        }

        public void Write(TextWriter writer)
        {
            writer.Write("{0}", Type);
            if (Number != 0)
                writer.Write(" - {0}", Number);
            if (Starts)
                writer.Write(" - Starts");
            writer.WriteLine();
            WriteSet("    First:  ", FirstPos, writer);
            WriteSet("    Last:   ", LastPos, writer);
            WriteSet("    Follow: ", FollowPos, writer);
            switch (Type)
            {
            case NodeType.Char: writer.Write("    {0} (0x{1:X2})", (char) Value, (int) Value); break;
            case NodeType.CharClass: writer.Write("[]"); break;
            case NodeType.Cat: Left.Write(writer); Right.Write(writer); break;
            case NodeType.Plus: Left.Write(writer); break;
            case NodeType.Cut: Left.Write(writer); Right.Write(writer); break;
            }
            writer.WriteLine();
        }

        private static void WriteSet(string caption, HashSet<TreeNode> set, TextWriter sb)
        {
            sb.Write(caption);
            sb.WriteLine(string.Join(",", set.OrderBy(n => n.Number).Select(n => n.Number.ToString())));
        }
    }

    public enum NodeType
    {
        EOS = -1,

        Cut = 1,
        Or = 2,
        Cat = 3,
        Star = 4,
        Plus = 5,

        Char = 6,
        CharClass = 7,
        Any = 8,
        Epsilon = 9,
    }
}