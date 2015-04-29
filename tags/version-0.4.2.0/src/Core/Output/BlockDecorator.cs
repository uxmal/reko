using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Output
{
    /// <summary>
    /// This class us used by ProcedureFormatter to render block-specific information
    /// before and after blocks are rendered. These will always be rendered as comments.
    /// </summary>
    public class BlockDecorator
    {
        static string[] empty = new string[0];

        public bool ShowEdges { get; set; }

        public virtual void BeforeBlock(Block block, List<string> lines)
        {
        }

        public virtual void AfterBlock(Block block, List<string> lines)
        {
             WriteBlockGraphEdges(block, lines);
        }

        public virtual void WriteBlockGraphEdges(Block block, List<string> lines)
        {
            if (ShowEdges && block.Succ.Count > 0)
            {
                StringBuilder sb = new StringBuilder("succ: ");
                foreach (var s in block.Succ)
                    sb.AppendFormat(" {0}", s.Name);
                lines.Add(sb.ToString());
            }
        }
    }
}
