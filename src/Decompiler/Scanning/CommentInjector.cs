using System;
using System.Collections.Generic;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Lib;

namespace Reko.Scanning
{
    /// <summary>
    /// Injects user-defined comments in code areas as CodeComment instructions.
    /// </summary>
    /// <remarks>
    /// This class is responsible for the flow of comments 
    /// from (Program.User.Annotation) ===> (Procedure). It can only 
    /// happen when scanning. The flow (Procedure) ===> (Program.User.Annotation)
    /// is carried out by the GUI. The command line client has no way of 
    /// adding annotations after loading the Reko project file.
    /// </remarks>
    public class CommentInjector
    {
        private SortedList<ulong, Annotation> annotations;

        public CommentInjector(IEnumerable<Annotation> annotations)
        {
            this.annotations = annotations.ToSortedList(a => a.Address.ToLinear());
        }

        /// <summary>
        /// Creates comment statements in the locations specified by user.
        /// </summary>
        /// <param name="proc"></param>
        public void InjectComments(Procedure proc)
        {
            foreach (var block in proc.ControlGraph.Blocks)
            {
                if (block == proc.EntryBlock || block == proc.ExitBlock)
                    continue;

                Statement stmPrev = null;
                for (int i = 0; i < block.Statements.Count; ++i)
                {
                    var stm = block.Statements[i];
                    if (stmPrev == null || stm.LinearAddress != stmPrev.LinearAddress)
                    {
                        if (this.annotations.TryGetValue(stm.LinearAddress, out var ann))
                        {
                            block.Statements.Insert(i, stm.LinearAddress, new CodeComment(ann.Text));
                            ++i;
                        }
                    }
                    stmPrev = stm;
                }
            }
        }
    }
}