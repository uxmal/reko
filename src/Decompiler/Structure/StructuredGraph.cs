using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using Decompiler.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Decompiler.Structure
{
    public abstract class StructuredGraph
    {
        public static StructuredGraph Seq = new SeqStructType(); // sequential statement (default)
        public static StructuredGraph Loop = new LoopStructType();					// Header of a loop only
        public static StructuredGraph Cond = new CondStructType();					// Header of a conditional only (if-then-else or switch)
        public static StructuredGraph LoopCond = new LoopCondStructType();			    // Header of a loop and a conditional

        [Obsolete]
        public virtual void WriteCode(AbsynCodeGenerator gen, StructureNode node, StructureNode latch, List<StructureNode> followSet, List<StructureNode> gotoSet, List<AbsynStatement> HLLCode) { return; }
        public abstract void GenerateCode(AbsynCodeGenerator gen, StructureNode node, AbsynCodeGeneratorState state);

        public abstract void WriteDetails(StructureNode structureNode, TextWriter writer);
    }

    public class SeqStructType : StructuredGraph
    {
        public override void GenerateCode(AbsynCodeGenerator gen, StructureNode node, AbsynCodeGeneratorState state)
        {
            gen.GenerateSequentialCode(node, state);
        }

        public override void WriteDetails(StructureNode structureNode, TextWriter writer)
        {
        }
    }

    public class LoopStructType : StructuredGraph
    {
        public override void GenerateCode(AbsynCodeGenerator gen, StructureNode node, AbsynCodeGeneratorState state)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void WriteDetails(StructureNode structureNode, TextWriter writer)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class CondStructType : StructuredGraph
    {
        public override void GenerateCode(AbsynCodeGenerator gen, StructureNode node, AbsynCodeGeneratorState state)
        {
            gen.GenerateCondCode(node, state);
        }

        public override void WriteDetails(StructureNode structureNode, TextWriter writer)
        {
            Debug.Assert(structureNode.CondFollow != null);
            writer.WriteLine("    follow: {0}", structureNode.CondFollow.Ident);
        }
    }

    public class LoopCondStructType : StructuredGraph
    {
        public override void GenerateCode(AbsynCodeGenerator gen, StructureNode node, AbsynCodeGeneratorState state)
        {
            gen.GenerateLoopCode(node, state);
        }

        public override void WriteDetails(StructureNode structureNode, TextWriter writer)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}