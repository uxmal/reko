using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Structure.Relooping
{
    using BlockSet = HashSet<Block>;
    using BlockBlockSetMap = Dictionary<Block, HashSet<Block>>;
    using BlockBranchMap = Dictionary<Block, Branch>;
    using BlockShapeMap = Dictionary<Block, Shape>;
    using BlockBlockMap = Dictionary<Block, Block>;
    using BlockList = List<Block>;

    /*
    This is a port of the optimized C++ implemention of the Relooper algorithm originally
    developed as part of Emscripten. This implementation includes optimizations
    added since the original academic paper [1] was published about it, and is
    written in an LLVM-friendly way with the goal of inclusion in upstream
    LLVM.

    [1] Alon Zakai. 2011. Emscripten: an LLVM-to-JavaScript compiler. In Proceedings of the ACM international conference companion on Object oriented programming systems languages and applications companion (SPLASH '11). ACM, New York, NY, USA, 301-312. DOI=10.1145/2048147.2048224 http://doi.acm.org/10.1145/2048147.2048224
    */

    // Info about a branching from one block to another
    public partial class Branch
    {
        public enum FlowType
        {
            Direct = 0, // We will directly reach the right location through other means, no need for continue or break
            Break = 1,
            Continue = 2
        };
        public Shape Ancestor; // If not null, this shape is the relevant one for purposes of getting to the target block. We break or continue on it
        public Branch.FlowType Type; // If Ancestor is not null, this says whether to break or continue
        public bool Labeled; // If a break or continue, whether we need to use a label
        public string Condition; // The condition for which we branch. For example, "my_var == 1". Conditions are checked one by one. One of the conditions should have null as the condition, in which case it is the default
        public string Code; // If provided, code that is run right before the branch is taken. This is useful for phis

    }


    // Represents a basic block of code - some instructions that end with a
    // control flow modifier (a branch, return or throw).
    public partial class Block
    {
        // Branches become processed after we finish the shape relevant to them. For example,
        // when we recreate a loop, branches to the loop start become continues and are now
        // processed. When we calculate what shape to generate from a set of blocks, we ignore
        // processed branches.
        // Blocks own the Branch objects they use, and destroy them when done.
        public BlockBranchMap BranchesOut;
        public BlockBranchMap BranchesIn; // TODO: make this just a list of Incoming, without branch info - should be just on BranchesOut
        public BlockBranchMap ProcessedBranchesOut;
        public BlockBranchMap ProcessedBranchesIn;
        public Shape Parent; // The shape we are directly inside
        public int Id; // A unique identifier
        public string Code; // The string representation of the code in this block. Owning pointer (we copy the input)
        public Block DefaultTarget; // The block we branch to without checking the condition, if none of the other conditions held.
        // Since each block *must* branch somewhere, this must be set
        public bool IsCheckedMultipleEntry; // If true, we are a multiple entry, so reaching us requires setting the label variable
    }

    // Represents a structured control flow shape, one of
    //
    //  Simple: No control flow at all, just instructions. If several
    //          blocks, then 
    //
    //  Multiple: A shape with more than one entry. If the next block to
    //            be entered is among them, we run it and continue to
    //            the next shape, otherwise we continue immediately to the
    //            next shape.
    //
    //  Loop: An infinite loop.
    //
    //  Emulated: Control flow is managed by a switch in a loop. This
    //            is necessary in some cases, for example when control
    //            flow is not known until runtime (indirect branches,
    //            setjmp returns, etc.)
    //

    public abstract partial class Shape
    {
        public int Id; // A unique identifier. Used to identify loops, labels are Lx where x is the Id.
        public Shape Next; // The shape that will appear in the code right after this one

        public enum ShapeType
        {
            Simple,
            Multiple,
            Loop
        }
        public ShapeType Type;

        public Shape(ShapeType TypeInit)
        {
            Id = (Shape.IdCounter++);
            Next = (null);
            Type = (TypeInit);
        }

        public abstract void Render(bool inLoop);

        public static SimpleShape IsSimple(Shape It) { return It != null && It.Type == ShapeType.Simple ? (SimpleShape)It : null; }
        public static MultipleShape IsMultiple(Shape It) { return It != null && It.Type == ShapeType.Multiple ? (MultipleShape)It : null; }
        public static LoopShape IsLoop(Shape It) { return It != null && It.Type == ShapeType.Loop ? (LoopShape)It : null; }
        public static LabeledShape IsLabeled(Shape It) { return IsMultiple(It) != null || IsLoop(It) != null ? (LabeledShape)It : null; }

        // INTERNAL
        private static int IdCounter;
    };

    public partial class SimpleShape : Shape
    {
        public Block Inner;

        public SimpleShape()
            : base(ShapeType.Simple)
        {
            Inner = (null);
        }
        public override void Render(bool InLoop)
        {
            Inner.Render(InLoop);
            if (Next != null) Next.Render(InLoop);
        }
    }


    // A shape that may be implemented with a labeled loop.
    public abstract partial class LabeledShape : Shape
    {
        public bool Labeled; // If we have a loop, whether it needs to be labeled

        public LabeledShape(ShapeType TypeInit)
            : base(TypeInit)
        {
            Labeled = (false);
        }
    }

    public partial class MultipleShape : LabeledShape
    {
        public BlockShapeMap InnerMap; // entry block . shape
        public int NeedLoop; // If we have branches, we need a loop. This is a counter of loop requirements,
        // if we optimize it to 0, the loop is unneeded

        public MultipleShape() :
            base(ShapeType.Multiple)
        {
            NeedLoop = (0);
        }
    }

    public partial class LoopShape : LabeledShape
    {
        public Shape Inner;

        public LoopShape()
            : base(ShapeType.Loop)
        {
            Inner = (null);
        }
    };

    /*
    public partial class  EmulatedShape : public Shape {
      std.deque<Block*> Blocks;
      void Render(bool InLoop);
    };
    */

    

#if DEBUG
    public partial class Debugging
    {
        static void Dump(BlockSet Blocks, string prefix = null) { }
    };
#endif





    //#if DEBUG
    //#define DebugDump(x, ...) Debugging.Dump(x, __VA_ARGS__)
    //#else
    //#define DebugDump(x, ...)
    //#endif

    public partial class Indenter
    {

        public static void Indent() { CurrIndent++; }
        public static void Unindent() { CurrIndent--; }

        static StringBuilder OutputBuffer = new StringBuilder();

        public static void PrintIndented(string Format, params object[] Args)
        {
            OutputBuffer.Append(' ', Indenter.CurrIndent * 2);
            OutputBuffer.AppendFormat(Format, Args);
        }
        public static void PutIndented(string String)
        {
            OutputBuffer.Append(' ', Indenter.CurrIndent * 2);
            OutputBuffer.Append(String);
        }

        public static int CurrIndent = 0;

        internal static void Reset()
        {
            OutputBuffer = new StringBuilder();

        }
    }
    // Branch
    public partial class Branch
    {
        public Branch(string ConditionInit, string CodeInit= null)
        {
            Ancestor = (null);
            Labeled = (false);
            Condition = ConditionInit != null ? (string)ConditionInit.Clone() : null;
            Code = CodeInit != null ? (string)CodeInit.Clone() : null;
        }

        // Prints out the branch
        public void Render(Block Target, bool SetLabel)
        {
            if (Code != null) Indenter.PrintIndented("%s\n", Code);
            if (SetLabel) Indenter.PrintIndented("label = %d;\n", Target.Id);
            if (Ancestor != null)
            {
                if (Type != FlowType.Direct)
                {
                    if (Labeled)
                    {
                        Indenter.PrintIndented("%s L%d;\n", Type == FlowType.Break ? "break" : "continue", Ancestor.Id);
                    }
                    else
                    {
                        Indenter.PrintIndented("%s;\n", Type == FlowType.Break ? "break" : "continue");
                    }
                }
            }
        }
    }


    public partial class Block
    {
        private static int IdCounter = 1; // 0 is reserved for clearings

        public Block(string CodeInit)
        {
            Parent = (null);
            Id = Block.IdCounter++;
            DefaultTarget = (null);
            IsCheckedMultipleEntry = (false);
            Code = (string)CodeInit.Clone();
        }

        public void AddBranchTo(Block Target, string Condition, string Code= null)
        {
            Debug.Assert(!BranchesOut.ContainsKey(Target)); // cannot add more than one branch to the same target
            BranchesOut[Target] = new Branch(Condition, Code);
        }

        // Prints out the instructions code and branchings
        public void Render(bool InLoop)
        {
            if (IsCheckedMultipleEntry && InLoop)
            {
                Indenter.PrintIndented("label = 0;\n");
            }

            if (Code != null)
            {
                // Print code in an indented manner, even over multiple lines
                //char *Start = const_cast<char*>(Code);
                //while (*Start) {
                //  char *End = strchr(Start, '\n');
                //  if (End) *End = 0;
                //  PutIndented(Start);
                //  if (End) *End = '\n'; else break;
                //  Start = End+1;
                //}
            }

            if (ProcessedBranchesOut.Count == 0) return;

            bool SetLabel = true; // in some cases it is clear we can avoid setting label, see later

            if (ProcessedBranchesOut.Count == 1 && ProcessedBranchesOut.First().Value.Type == Branch.FlowType.Direct)
            {
                SetLabel = false;
            }

            // A setting of the label variable (label = x) is necessary if it can
            // cause an impact. The main case is where we set label to x, then elsewhere
            // we check if label is equal to that value, i.e., that label is an entry
            // in a multiple block. We also need to reset the label when we enter
            // that block, so that each setting is a one-time action: consider
            //
            //    while (1) {
            //      if (check) label = 1;
            //      if (label == 1) { label = 0 }
            //    }
            //
            // (Note that this case is impossible due to fusing, but that is not
            // material here.) So setting to 0 is important just to clear the 1 for
            // future iterations.
            // TODO: When inside a loop, if necessary clear the label variable
            //       once on the top, and never do settings that are in effect clears

            // Fusing: If the next is a Multiple, we can fuse it with this block. Note
            // that we must be the Inner of a Simple, so fusing means joining a Simple
            // to a Multiple. What happens there is that all options in the Multiple
            // *must* appear in the Simple (the Simple is the only one reaching the
            // Multiple), so we can remove the Multiple and add its independent groups
            // into the Simple's branches.
            MultipleShape Fused = Shape.IsMultiple(Parent.Next);
            if (Fused != null)
            {
                Debug.Print("Fusing Multiple to Simple\n");
                Parent.Next = Parent.Next.Next;
                Fused.RenderLoopPrefix();

                // When the Multiple has the same number of groups as we have branches,
                // they will all be fused, so it is safe to not set the label at all
                if (SetLabel && Fused.InnerMap.Count == ProcessedBranchesOut.Count)
                {
                    SetLabel = false;
                }
            }

            // We must do this here, because blocks can be split and even comparing their Ids is not enough. We must check the conditions.
            foreach (var it in ProcessedBranchesOut)
            {
                if (it.Value.Condition == null)
                {
                    Debug.Assert(DefaultTarget == null); // Must be exactly one default
                    DefaultTarget = it.Key;
                }
            }
            Debug.Assert(DefaultTarget != null); // Must be a default

            var RemainingConditions = new StringBuilder();
            bool First = true;
            for (var iter = ProcessedBranchesOut.GetEnumerator(); ; )
            {
                Block Target;
                Branch Details;
                bool isAtEnd = iter.MoveNext();
                if (!isAtEnd)
                {
                    Target = iter.Current.Key;
                    if (Target == DefaultTarget) continue; // done at the end
                    Details = iter.Current.Value;
                    Debug.Assert(Details.Condition != null); // must have a condition if this is not the default target
                }
                else
                {
                    Target = DefaultTarget;
                    Details = ProcessedBranchesOut[DefaultTarget];
                }
                bool SetCurrLabel = SetLabel && Target.IsCheckedMultipleEntry;
                bool HasFusedContent = Fused != null && Fused.InnerMap.ContainsKey(Target);
                bool HasContent = SetCurrLabel || Details.Type != Branch.FlowType.Direct || HasFusedContent || Details.Code != null;
                if (isAtEnd)
                {
                    // If there is nothing to show in this branch, omit the condition
                    if (HasContent)
                    {
                        Indenter.PrintIndented("{0}if ({1}) {\n", First ? "" : "} else ", Details.Condition);
                        First = false;
                    }
                    else
                    {
                        if (RemainingConditions.Length > 0)
                            RemainingConditions.Append(" && ");
                        RemainingConditions.AppendFormat("!({0})", Details.Condition);
                    }
                }
                else
                {
                    if (HasContent)
                    {
                        if (RemainingConditions.Length > 0)
                        {
                            if (First)
                            {
                                Indenter.PrintIndented("if (%s) {\n", RemainingConditions);
                                First = false;
                            }
                            else
                            {
                                Indenter.PrintIndented("} else if (%s) {\n", RemainingConditions);
                            }
                        }
                        else if (!First)
                        {
                            Indenter.PrintIndented("} else {\n");
                        }
                    }
                }
                if (!First) Indenter.Indent();
                Details.Render(Target, SetCurrLabel);
                if (HasFusedContent)
                {
                    Fused.InnerMap[Target].Render(InLoop);
                }
                if (!First) Indenter.Unindent();
                if (isAtEnd) break;
            }
            if (!First) Indenter.PrintIndented("}\n");

            if (Fused != null)
            {
                Fused.RenderLoopPostfix();
            }
        }

    }

    public partial class MultipleShape
    {
        public void RenderLoopPrefix()
        {
            if (NeedLoop != 0)
            {
                if (Labeled)
                {
                    Indenter.PrintIndented("L%d: do {\n", Id);
                }
                else
                {
                    Indenter.PrintIndented("do {\n");
                }
                Indenter.Indent();
            }
        }

        public void RenderLoopPostfix()
        {
            if (NeedLoop != 0)
            {
                Indenter.Unindent();
                Indenter.PrintIndented("} while(0);\n");
            }
        }

        public override void Render(bool InLoop)
        {
            RenderLoopPrefix();
            bool First = true;
            foreach (var iter in InnerMap)
            {
                Indenter.PrintIndented("%sif (label == %d) {\n", First ? "" : "else ", iter.Key.Id);
                First = false;
                Indenter.Indent();
                iter.Value.Render(InLoop);
                Indenter.Unindent();
                Indenter.PrintIndented("}\n");
            }
            RenderLoopPostfix();
            if (Next != null) Next.Render(InLoop);
        }
    }

    public partial class LoopShape
    {
        public override void Render(bool InLoop)
        {
            if (Labeled)
            {
                Indenter.PrintIndented("L%d: while(1) {\n", Id);
            }
            else
            {
                Indenter.PrintIndented("while(1) {\n");
            }
            Indenter.Indent();
            Inner.Render(true);
            Indenter.Unindent();
            Indenter.PrintIndented("}\n");
            if (Next != null) Next.Render(InLoop);
        }
    }

    /*
    // EmulatedShape

    void EmulatedShape.Render(bool InLoop) {
      Indenter.PrintIndented("while(1) {\n");
      Indenter.Indent();
      Indenter.PrintIndented("switch(label) {\n");
      Indenter.Indent();
      for (int i = 0; i < Blocks.Count; i++) {
        Block *Curr = Blocks[i];
        Indenter.PrintIndented("case %d: {\n", Curr.Id);
        Indenter.Indent();
        Curr.Render(InLoop);
        Indenter.PrintIndented("break;\n");
        Indenter.Unindent();
        Indenter.PrintIndented("}\n");
      }
      Indenter.Unindent();
      Indenter.PrintIndented("}\n");
      Indenter.Unindent();
      Indenter.PrintIndented("}\n");
      if (Next) Next.Render(InLoop);
    };
    */

    // Relooper

    // Implements the relooper algorithm for a function's blocks.
    //
    // Usage:
    //  1. Instantiate this struct.
    //  2. Call AddBlock with the blocks you have. Each should already
    //     have its branchings in specified (the branchings out will
    //     be calculated by the relooper).
    //  3. Call Render().
    //
    // Implementation details: The Relooper instance has
    // ownership of the blocks and shapes, and frees them when done.
    public partial class Relooper
    {
        Dequeue<Block> Blocks;
        Dequeue<Shape> Shapes;
        Shape Root;

        public Relooper()
        {
            Root = null;
            Blocks = new Dequeue<Block>();
            Shapes = new Dequeue<Shape>();
        }

        public void AddBlock(Block New)
        {
            Blocks.PushBack(New);
        }
    }

    public partial class RelooperRecursor
    {
        public Relooper Parent;
        public RelooperRecursor(Relooper ParentInit) { Parent = (ParentInit); }
    }

    public partial class PreOptimizer : RelooperRecursor
    {
        public BlockSet Live;

        public PreOptimizer(Relooper Parent) : base(Parent) { }

        public void FindLive(Block Curr)
        {
            if (Live.Contains(Curr)) return;
            Live.Add(Curr);
            foreach (var iter in Curr.BranchesOut)
            {
                FindLive(iter.Key);
            }
        }

        // If a block has multiple entries but no exits, and it is small enough, it is useful to split it.
        // A common example is a C++ function where everything ends up at a final exit block and does some
        // RAII cleanup. Without splitting, we will be forced to introduce labelled loops to allow
        // reaching the final block
        public void SplitDeadEnds()
        {
            int TotalCodeSize = Live.Sum(c => c.Code.Length);

            foreach (var Original in Live)
            {
                if (Original.BranchesIn.Count <= 1 || Original.BranchesOut.Count > 0) continue;
                if (Original.Code.Length * (Original.BranchesIn.Count - 1) > TotalCodeSize / 5) continue; // if splitting increases raw code size by a significant amount, abort
                // Split the node (for simplicity, we replace all the blocks, even though we could have reused the original)
                foreach (var iter in Original.BranchesIn)
                {
                    Block Prior = iter.Key;
                    Block Split = new Block(Original.Code);
                    Split.BranchesIn[Prior] = new Branch(null);
                    Prior.BranchesOut[Split] = new Branch(Prior.BranchesOut[Original].Condition, Prior.BranchesOut[Original].Code);
                    Prior.BranchesOut.Remove(Original);
                    Parent.AddBlock(Split);
                    Live.Add(Split);
                }
            }
        }
    }

    public partial class Relooper
    {
        public void Calculate(Block Entry)
        {
            // Scan and optimize the input
            PreOptimizer Pre = new PreOptimizer(this);
            Pre.FindLive(Entry);

            // Add incoming branches from live blocks, ignoring dead code
            foreach (var Curr in Blocks)
            {
                if (!Pre.Live.Contains(Curr)) continue;
                foreach (var iter in Curr.BranchesOut)
                {
                    iter.Key.BranchesIn[Curr] = new Branch(null);
                }
            }

            Pre.SplitDeadEnds();

            // Main

            var AllBlocks = new BlockSet();
            foreach (var i in Blocks)
            {
                AllBlocks.Add(i);
#if DEBUG
                Debug.Print("Adding block %d (%s)\n", i.Id, i.Code);
                foreach (var iter in i.BranchesOut)
                {
                    Debug.Print("  with branch out to %d\n", iter.Key.Id);
                }
#endif
            }

            BlockSet Entries = new BlockSet();
            Entries.Add(Entry);
            Root = new Analyzer(this).Process(AllBlocks, Entries, null);

            Debug.Print("=== Optimizing shapes ===\n");

            new PostOptimizer(this).Process(Root);
        }

        public partial class Analyzer : RelooperRecursor
        {
            public Analyzer(Relooper Parent) : base(Parent) { }

            // Add a shape to the list of shapes in this Relooper calculation
            void Notice(Shape New)
            {
                Parent.Shapes.PushBack(New);
            }

            // Create a list of entries from a block. If LimitTo is provided, only results in that set
            // will appear
            void GetBlocksOut(Block Source, BlockSet Entries, BlockSet LimitTo = null)
            {
                foreach (var iter in Source.BranchesOut)
                {
                    if (LimitTo == null || LimitTo.Contains(iter.Key))
                    {
                        Entries.Add(iter.Key);
                    }
                }
            }

            // Converts/processes all branchings to a specific target
            void Solipsize(Block Target, Branch.FlowType Type, Shape Ancestor, BlockSet From)
            {
                Debug.Print("Solipsizing branches into %d\n", Target.Id);
                DebugDump(From, "  relevant to solipsize: ");
                foreach (var iter in Target.BranchesIn)
                {
                    Block Prior = iter.Key;
                    if (!From.Contains(Prior))
                    {
                        //iter++;
                        continue;
                    }
                    Branch TargetIn = iter.Value;
                    Branch PriorOut = Prior.BranchesOut[Target];
                    PriorOut.Ancestor = Ancestor; // Do we need this info
                    PriorOut.Type = Type;         // on TargetIn too?
                    MultipleShape Multiple = Shape.IsMultiple(Ancestor);
                    if (Multiple != null)
                    {
                        Multiple.NeedLoop++; // We are breaking out of this Multiple, so need a loop
                    }
                    //iter++; // carefully increment iter before erasing
                    Target.BranchesIn.Remove(Prior);
                    Target.ProcessedBranchesIn[Prior] = TargetIn;
                    Prior.BranchesOut.Remove(Target);
                    Prior.ProcessedBranchesOut[Target] = PriorOut;
                    Debug.Print("  eliminated branch from %d\n", Prior.Id);
                }
            }

            Shape MakeSimple(BlockSet Blocks, Block Inner, BlockSet NextEntries)
            {
                Debug.Print("creating simple block with block #%d\n", Inner.Id);
                SimpleShape Simple = new SimpleShape();
                Notice(Simple);
                Simple.Inner = Inner;
                Inner.Parent = Simple;
                if (Blocks.Count > 1)
                {
                    Blocks.Remove(Inner);
                    GetBlocksOut(Inner, NextEntries, Blocks);
                    BlockSet JustInner = new BlockSet();
                    JustInner.Add(Inner);
                    foreach (var iter in NextEntries)
                    {
                        Solipsize(iter, Branch.FlowType.Direct, Simple, JustInner);
                    }
                }
                return Simple;
            }

            Shape MakeLoop(BlockSet Blocks, BlockSet Entries, BlockSet NextEntries)
            {
                // Find the inner blocks in this loop. Proceed backwards from the entries until
                // you reach a seen block, collecting as you go.
                BlockSet InnerBlocks = new BlockSet();
                Queue<Block> Queue = new Queue<Block>(Entries);
                while (Queue.Count > 0)
                {
                    Block Curr = Queue.Dequeue();
                    if (!InnerBlocks.Contains(Curr))
                    {
                        // This element is new, mark it as inner and remove from outer
                        InnerBlocks.Add(Curr);
                        Blocks.Remove(Curr);
                        // Add the elements prior to it
                        foreach (var iter in Curr.BranchesIn)
                            Queue.Enqueue(iter.Key);
                    }
                }

                Debug.Assert(InnerBlocks.Count > 0);

                foreach (var Curr in InnerBlocks)
                {
                    foreach (var iter in Curr.BranchesOut)
                    {
                        Block Possible = iter.Key;
                        if (!InnerBlocks.Contains(Possible) &&
                            NextEntries.Contains(Possible) == NextEntries.Contains(Possible)) //WTF?
                        {
                            NextEntries.Add(Possible);
                        }
                    }
                }

                Debug.Print("creating loop block:\n");
                DebugDump(InnerBlocks, "  inner blocks:");
                DebugDump(Entries, "  inner entries:");
                DebugDump(Blocks, "  outer blocks:");
                DebugDump(NextEntries, "  outer entries:");

                // TODO: Optionally hoist additional blocks into the loop

                LoopShape Loop = new LoopShape();
                Notice(Loop);

                // Solipsize the loop, replacing with break/continue and marking branches as Processed (will not affect later calculations)
                // A. Branches to the loop entries become a continue to this shape
                foreach (var iter in Entries)
                {
                    Solipsize(iter, Branch.FlowType.Continue, Loop, InnerBlocks);
                }
                // B. Branches to outside the loop (a next entry) become breaks on this shape
                foreach (var iter in NextEntries)
                {
                    Solipsize(iter, Branch.FlowType.Break, Loop, InnerBlocks);
                }
                // Finish up
                Shape Inner = Process(InnerBlocks, Entries, null);
                Loop.Inner = Inner;
                return Loop;
            }

            public partial class HelperClass
            {
                BlockBlockSetMap IndependentGroups;
                public BlockBlockMap Ownership = new BlockBlockMap(); // For each block, which entry it belongs to. We have reached it from there.

                public HelperClass(BlockBlockSetMap IndependentGroupsInit)
                {
                    IndependentGroups = new BlockBlockSetMap(IndependentGroupsInit);
                }
                public void InvalidateWithChildren(Block New)
                { // TODO: rename New
                    var ToInvalidate = new LinkedList<Block>(); // Being in the list means you need to be invalidated
                    ToInvalidate.AddLast(New);
                    while (ToInvalidate.Count > 0)
                    {
                        Block Invalidatee = ToInvalidate.First.Value;
                        ToInvalidate.RemoveFirst();
                        Block Owner = Ownership[Invalidatee];
                        if (IndependentGroups.ContainsKey(Owner))
                        { // Owner may have been invalidated, do not add to IndependentGroups!
                            IndependentGroups[Owner].Remove(Invalidatee);
                        }
                        if (Ownership[Invalidatee] != null)
                        { // may have been seen before and invalidated already
                            Ownership[Invalidatee] = null;
                            foreach (var iter in Invalidatee.BranchesOut)
                            {
                                Block Target = iter.Key;
                                Block TargetOwner;
                                if (Ownership.TryGetValue(Target, out TargetOwner))
                                {
                                    if (TargetOwner != null)
                                    {
                                        ToInvalidate.AddLast(Target);
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // For each entry, find the independent group reachable by it. The independent group is
            // the entry itself, plus all the blocks it can reach that cannot be directly reached by another entry. Note that we
            // ignore directly reaching the entry itself by another entry.
            void FindIndependentGroups(HashSet<Block> Blocks, HashSet<Block> Entries, Dictionary<Block,BlockSet> IndependentGroups)
            {
                var Helper = new HelperClass(IndependentGroups);

                // We flow out from each of the entries, simultaneously.
                // When we reach a new block, we add it as belonging to the one we got to it from.
                // If we reach a new block that is already marked as belonging to someone, it is reachable by
                // two entries and is not valid for any of them. Remove it and all it can reach that have been
                // visited.

                var Queue = new LinkedList<Block>(); // Being in the queue means we just added this item, and we need to add its children
                foreach (var Entry in Entries)
                {
                    Helper.Ownership[Entry] = Entry;
                    IndependentGroups[Entry].Add(Entry);
                    Queue.AddLast(Entry);
                }
                while (Queue.Count > 0)
                {
                    Block Curr = Queue.First();
                    Queue.RemoveFirst();
                    Block Owner = Helper.Ownership[Curr]; // Curr must be in the ownership map if we are in the queue
                    if (Owner == null) continue; // we have been invalidated meanwhile after being reached from two entries
                    // Add all children
                    foreach (var iter in Curr.BranchesOut)
                    {
                        Block New = iter.Key;
                        Block NewOwner;
                        if (!Helper.Ownership.TryGetValue(New, out NewOwner))
                        {
                            // New node. Add it, and put it in the queue
                            Helper.Ownership[New] = Owner;
                            IndependentGroups[Owner].Add(New);
                            Queue.AddLast(New);
                            continue;
                        }
                        if (NewOwner == null) continue; // We reached an invalidated node
                        if (NewOwner != Owner)
                        {
                            // Invalidate this and all reachable that we have seen - we reached this from two locations
                            Helper.InvalidateWithChildren(New);
                        }
                        // otherwise, we have the same owner, so do nothing
                    }
                }

                // Having processed all the interesting blocks, we remain with just one potential issue:
                // If a->b, and a was invalidated, but then b was later reached by someone else, we must
                // invalidate b. To check for this, we go over all elements in the independent groups,
                // if an element has a parent which does *not* have the same owner, we must remove it
                // and all its children.

                foreach (var it in Entries)
                {
                    var CurrGroup = IndependentGroups[it];
                    LinkedList<Block> ToInvalidate = new LinkedList<Block>();
                    foreach (var Child in CurrGroup)
                    {
                        foreach (var iter in Child.BranchesIn)
                        {
                            Block Parent = iter.Key;
                            if (Helper.Ownership[Parent] != Helper.Ownership[Child])
                            {
                                ToInvalidate.AddLast(Child);
                            }
                        }
                    }
                    while (ToInvalidate.Count > 0)
                    {
                        Block Invalidatee = ToInvalidate.First.Value;
                        ToInvalidate.RemoveFirst();
                        Helper.InvalidateWithChildren(Invalidatee);
                    }
                }

                // Remove empty groups
                foreach (var iter in Entries)
                {
                    if (IndependentGroups[iter].Count == 0)
                    {
                        IndependentGroups.Remove(iter);
                    }
                }

#if DEBUG
                Debug.Print("Investigated independent groups:\n");
                foreach (var iter in IndependentGroups)
                {
                    DebugDump(iter.Value, " group: ");
                }
#endif
            }

            Shape MakeMultiple(BlockSet Blocks, BlockSet Entries, BlockBlockSetMap IndependentGroups, Shape Prev, BlockSet NextEntries)
            {
                Debug.Print("creating multiple block with %d inner groups\n", IndependentGroups.Count);
                bool Fused = (Shape.IsSimple(Prev)) != null;
                MultipleShape Multiple = new MultipleShape();
                Notice(Multiple);
                var CurrEntries = new BlockSet();
                foreach (var iter in IndependentGroups)
                {
                    Block CurrEntry = iter.Key;
                    var CurrBlocks = iter.Value;
                    Debug.Print("  multiple group with entry %d:\n", CurrEntry.Id);
                    DebugDump(CurrBlocks, "    ");
                    // Create inner block
                    CurrEntries.Clear();
                    CurrEntries.Add(CurrEntry);
                    foreach (var CurrInner in CurrBlocks)
                    {
                        // Remove the block from the remaining blocks
                        Blocks.Remove(CurrInner);
                        // Find new next entries and fix branches to them
                        foreach (var iter2 in CurrInner.BranchesOut)
                        {
                            Block CurrTarget = iter.Key;
                            //BlockBranchMap.iterator Next = iter;
                            //Next++;
                            if (!CurrBlocks.Contains(CurrTarget))
                            {
                                NextEntries.Add(CurrTarget);
                                Solipsize(CurrTarget, Branch.FlowType.Break, Multiple, CurrBlocks);
                            }
                            // iter = Next; // increment carefully because Solipsize can remove us
                        }
                    }
                    Multiple.InnerMap[CurrEntry] = Process(CurrBlocks, CurrEntries, null);
                    // If we are not fused, then our entries will actually be checked
                    if (!Fused)
                    {
                        CurrEntry.IsCheckedMultipleEntry = true;
                    }
                }
                DebugDump(Blocks, "  remaining blocks after multiple:");
                // Add entries not handled as next entries, they are deferred
                foreach (var Entry in Entries)
                {
                    if (IndependentGroups.ContainsKey(Entry))
                    {
                        NextEntries.Add(Entry);
                    }
                }
                return Multiple;
            }

            public class MakeState
            {
                Shape Prev;
                public Shape Ret = null;
                public BlockSet NextEntries;
                public BlockSet Entries;

                public MakeState(Shape Prev, BlockSet InitialEntries)
                {
                    this.Prev = Prev;
                    NextEntries = new BlockSet();
                    Entries = new BlockSet(InitialEntries);
                }
                public bool Make(Shape call)
                {
                    Shape Temp = call;
                    if (Prev != null) Prev.Next = Temp;
                    if (Ret == null) Ret = Temp;
                    if (NextEntries.Count == 0) { Debug.Print("Process() returning\n"); return true; }
                    Prev = Temp;
                    Entries = NextEntries;
                    return false;
                }
            }

            // Main function.
            // Process a set of blocks with specified entries, returns a shape
            // The Make* functions receive a NextEntries. If they fill it with data, those are the entries for the
            //   .Next block on them, and the blocks are what remains in Blocks (which Make* modify). In this way
            //   we avoid recursing on Next (imagine a long chain of Simples, if we recursed we could blow the stack).
            public Shape Process(BlockSet Blocks, BlockSet InitialEntries, Shape Prev)
            {
                Debug.Print("Process() called\n");
                var ms = new MakeState(Prev, InitialEntries);
                BlockSet[] TempEntries = new BlockSet[] { new BlockSet(), new BlockSet() };
                int CurrTempIndex = 0;
                while (true)
                {
                    Debug.Print("Process() running\n");
                    DebugDump(Blocks, "  blocks : ");
                    DebugDump(ms.Entries, "  entries: ");

                    CurrTempIndex = 1 - CurrTempIndex;
                    ms.NextEntries = TempEntries[CurrTempIndex];
                    ms.NextEntries.Clear();

                    if (ms.Entries.Count == 0) return ms.Ret;
                    if (ms.Entries.Count == 1)
                    {
                        Block Curr = ms.Entries.First();
                        if (Curr.BranchesIn.Count == 0)
                        {
                            // One entry, no looping ==> Simple
                            if (ms.Make(MakeSimple(Blocks, Curr, ms.NextEntries)))
                                return ms.Ret;
                            continue;
                        }
                        // One entry, looping ==> Loop
                        if (ms.Make(MakeLoop(Blocks, ms.Entries, ms.NextEntries)))
                            return ms.Ret;
                        continue;
                    }
                    // More than one entry, try to eliminate through a Multiple groups of
                    // independent blocks from an entry/ies. It is important to remove through
                    // multiples as opposed to looping since the former is more performant.
                    var IndependentGroups = new BlockBlockSetMap();
                    FindIndependentGroups(Blocks, ms.Entries, IndependentGroups);

                    Debug.Print("Independent groups: {0}\n", IndependentGroups.Count);

                    if (IndependentGroups.Count > 0)
                    {
                        var ig = new BlockBlockSetMap();
                        // We can handle a group in a multiple if its entry cannot be reached by another group.
                        // Note that it might be reachable by itself - a loop. But that is fine, we will create
                        // a loop inside the multiple block (which is the performant order to do it).
                        foreach (var iter in IndependentGroups)
                        {
                            Block Entry = iter.Key;
                            var Group = iter.Value;
                            bool survives = true;
                            //BlockBlockSetMap.iterator curr = iter++; // iterate carefully, we may delete
                            foreach (var iterBranch in Entry.BranchesIn)
                            {
                                Block Origin = iterBranch.Key;
                                if (!Group.Contains(Origin))
                                {
                                    // Reached from outside the group, so we cannot handle this
                                    Debug.Print("Cannot handle group with entry {0} because of incoming branch from {1}", Entry.Id, Origin.Id);
                                    survives = false;
                                    break;
                                }
                            }
                            if (survives)
                                ig.Add(iter.Key, iter.Value);
                        }
                        IndependentGroups = ig;

                        // As an optimization, if we have 2 independent groups, and one is a small dead end, we can handle only that dead end.
                        // The other then becomes a Next - without nesting in the code and recursion in the analysis.
                        // TODO: if the larger is the only dead end, handle that too
                        // TODO: handle >2 groups
                        // TODO: handle not just dead ends, but also that do not branch to the NextEntries. However, must be careful
                        //       there since we create a Next, and that Next can prevent eliminating a break (since we no longer
                        //       naturally reach the same place), which may necessitate a one-time loop, which makes the unnesting
                        //       pointless.
                        if (IndependentGroups.Count == 2)
                        {
                            // Find the smaller one
                            var iter = IndependentGroups.GetEnumerator();
                            iter.MoveNext();
                            Block SmallEntry = iter.Current.Key;
                            int SmallSize = iter.Current.Value.Count;
                            iter.MoveNext();
                            Block LargeEntry = iter.Current.Key;
                            int LargeSize = iter.Current.Value.Count;
                            if (SmallSize != LargeSize)
                            { // ignore the case where they are identical - keep things symmetrical there
                                if (SmallSize > LargeSize)
                                {
                                    Block Temp = SmallEntry;
                                    SmallEntry = LargeEntry;
                                    LargeEntry = Temp; // Note: we did not flip the Sizes too, they are now invalid. TODO: use the smaller size as a limit?
                                }
                                // Check if dead end
                                bool DeadEnd = true;
                                var SmallGroup = IndependentGroups[SmallEntry];
                                foreach (var Curr in SmallGroup)
                                {
                                    foreach (var iter2 in Curr.BranchesOut)
                                    {
                                        Block Target = iter2.Key;
                                        if (!SmallGroup.Contains(Target))
                                        {
                                            DeadEnd = false;
                                            break;
                                        }
                                    }
                                    if (!DeadEnd) break;
                                }
                                if (DeadEnd)
                                {
                                    Debug.Print("Removing nesting by not handling large group because small group is dead end\n");
                                    IndependentGroups.Remove(LargeEntry);
                                }
                            }
                        }

                        Debug.Print("Handleable independent groups: {0}", IndependentGroups.Count);

                        if (IndependentGroups.Count > 0)
                        {
                            // Some groups removable ==> Multiple
                            if (ms.Make(MakeMultiple(Blocks, ms.Entries, IndependentGroups, Prev, ms.NextEntries)))
                                return ms.Ret;
                            continue;
                        }
                    }
                    // No independent groups, must be loopable ==> Loop
                    if (ms.Make(MakeLoop(Blocks, ms.Entries, ms.NextEntries)))
                        return ms.Ret;
                    continue;
                }
            } 
        }

        // Post optimizations

        public partial class PostOptimizer
        {
            Relooper Parent;
            object Closure;

            public PostOptimizer(Relooper ParentInit)
            {
                Parent = (ParentInit);
                Closure = (null);
            }

            private void RECURSE_MULTIPLE_MANUAL(Action<Shape> func, MultipleShape manual)
            {
                foreach (var iter in manual.InnerMap)
                {
                    func(iter.Value);
                }
            }

            private void RECURSE_LOOP(Action<Shape> func, LoopShape Loop)
            {
                func(Loop.Inner);
            }

            private void SHAPE_SWITCH(Shape var, Action<SimpleShape> simple, Action<MultipleShape> multiple, Action<LoopShape> loop)
            {
                SimpleShape Simple = Shape.IsSimple(var);
                if (Simple != null)
                {
                    simple(Simple);
                    return;
                }
                MultipleShape Multiple = Shape.IsMultiple(var);
                if (Multiple != null)
                {
                    multiple(Multiple);
                    return;
                }
                LoopShape Loop = Shape.IsLoop(var);
                if (Loop != null)
                {
                    loop(Loop);
                }
            }

            // Remove unneeded breaks and continues.
            // A flow operation is trivially unneeded if the shape we naturally get to by normal code
            // execution is the same as the flow forces us to.
            void RemoveUnneededFlows(Shape Root, Shape Natural = null)
            {
                SHAPE_SWITCH(Root, (Simple) =>
                {
                    // If there is a next block, we already know at Simple creation time to make direct branches,
                    // and we can do nothing more. If there is no next however, then Natural is where we will
                    // go to by doing nothing, so we can potentially optimize some branches to direct.
                    if (Simple.Next != null)
                    {
                        RemoveUnneededFlows(Simple.Next, Natural);
                    }
                    else
                    {
                        foreach (var iter in Simple.Inner.ProcessedBranchesOut)
                        {
                            Block Target = iter.Key;
                            Branch Details = iter.Value;
                            if (Details.Type != Branch.FlowType.Direct && Target.Parent == Natural)
                            {
                                Details.Type = Branch.FlowType.Direct;
                                MultipleShape Multiple = Shape.IsMultiple(Details.Ancestor);
                                if (Multiple != null)
                                {
                                    Multiple.NeedLoop--;
                                }
                            }
                        }
                    }
                }, (Multiple) =>
                {
                    foreach (var iter in Multiple.InnerMap)
                    {
                        RemoveUnneededFlows(iter.Value, Multiple.Next);
                    }
                    RemoveUnneededFlows(Multiple.Next, Natural);
                }, (Loop) =>
                {
                    RemoveUnneededFlows(Loop.Inner, Loop.Inner);
                    RemoveUnneededFlows(Loop.Next, Natural);
                });
            }

            // After we know which loops exist, we can calculate which need to be labeled
            void FindLabeledLoops(Shape Root)
            {
                bool First = Closure == null;
                if (First)
                {
                    Closure = new Stack<Shape>();
                }
                Stack<Shape> LoopStack = (Stack<Shape>)Closure;

                SHAPE_SWITCH(Root, (Simple) =>
                {
                    MultipleShape Fused = Shape.IsMultiple(Root.Next);
                    // If we are fusing a Multiple with a loop into this Simple, then visit it now
                    if (Fused != null && Fused.NeedLoop != 0)
                    {
                        LoopStack.Push(Fused);
                        RECURSE_MULTIPLE_MANUAL(FindLabeledLoops, Fused);
                    }
                    foreach (var iter in Simple.Inner.ProcessedBranchesOut)
                    {
                        Block Target = iter.Key;
                        Branch Details = iter.Value;
                        if (Details.Type != Branch.FlowType.Direct)
                        {
                            Debug.Assert(LoopStack.Count > 0);
                            if (Details.Ancestor != LoopStack.Peek())
                            {
                                LabeledShape Labeled = Shape.IsLabeled(Details.Ancestor);
                                Labeled.Labeled = true;
                                Details.Labeled = true;
                            }
                            else
                            {
                                Details.Labeled = false;
                            }
                        }
                    }
                    if (Fused != null && Fused.NeedLoop != 0)
                    {
                        LoopStack.Pop();
                        if (Fused.Next != null) FindLabeledLoops(Fused.Next);
                    }
                    else
                    {
                        if (Root.Next != null) FindLabeledLoops(Root.Next);
                    }
                }, (Multiple) =>
                {
                    if (Multiple.NeedLoop != 0)
                    {
                        LoopStack.Push(Multiple);
                    }
                    RECURSE_MULTIPLE_MANUAL(FindLabeledLoops, Multiple);
                    if (Multiple.NeedLoop != 0)
                    {
                        LoopStack.Pop();
                    }
                    if (Root.Next != null) FindLabeledLoops(Root.Next);
                }, (Loop) =>
                {
                    LoopStack.Push(Loop);
                    RECURSE_LOOP(FindLabeledLoops, Loop);
                    LoopStack.Pop();
                    if (Root.Next != null) FindLabeledLoops(Root.Next);
                });

                if (First)
                {
                    Closure = null;
                }
            }

            public void Process(Shape Root)
            {
                RemoveUnneededFlows(Root);
                FindLabeledLoops(Root);
            }
        };


        public void Render()
        {
            Indenter.Reset();
            Root.Render(false);
        }
    }

    // Debugging

    public partial class Relooper
    {
        [Conditional("DEBUG")]
        public static void DebugDump(BlockSet Blocks, string prefix)
        {
            if (prefix != null) Debug.Write(prefix);
            foreach (var iter in Blocks)
            {
                Debug.Write(string.Format("{0} ", iter.Id));
            }
            Debug.WriteLine("");
        }


        // C API - useful for binding to other languages

        //typedef std.map<void*, int> VoidIntMap;
        //VoidIntMap __blockDebugMap__; // maps block pointers in currently running code to block ids, for generated debug output

        //extern "C" {

        //void rl_set_output_buffer(char *buffer, int size) {
        //#if DEBUG
        //  printf("#include \"Relooper.h\"\n");
        //  printf("int main() {\n");
        //  printf("  char buffer[100000];\n");
        //  printf("  rl_set_output_buffer(buffer);\n");
        //#endif
        //  Relooper.SetOutputBuffer(buffer, size);
        //}

        //void rl_make_output_buffer(int size) {
        //  Relooper.SetOutputBuffer((char*)malloc(size), size);
        //}

        //void *rl_new_block(const char *text) {
        //  Block *ret = new Block(text);
        //#if DEBUG
        //  printf("  void *b%d = rl_new_block(\"// code %d\");\n", ret.Id, ret.Id);
        //  __blockDebugMap__[ret] = ret.Id;
        //  printf("  block_map[%d] = b%d;\n", ret.Id, ret.Id);
        //#endif
        //  return ret;
        //}

        //void rl_delete_block(void *block) {
        //#if DEBUG
        //  printf("  rl_delete_block(block_map[%d]);\n", ((Block*)block).Id);
        //#endif
        //  delete (Block*)block;
        //}

        //void rl_block_add_branch_to(void *from, void *to, const char *condition, const char *code) {
        //#if DEBUG
        //  printf("  rl_block_add_branch_to(block_map[%d], block_map[%d], %s%s%s, %s%s%s);\n", ((Block*)from).Id, ((Block*)to).Id, condition ? "\"" : "", condition ? condition : "null", condition ? "\"" : "", code ? "\"" : "", code ? code : "null", code ? "\"" : "");
        //#endif
        //  ((Block*)from).AddBranchTo((Block*)to, condition, code);
        //}

        //void *rl_new_relooper() {
        //#if DEBUG
        //  printf("  void *block_map[10000];\n");
        //  printf("  void *rl = rl_new_relooper();\n");
        //#endif
        //  return new Relooper;
        //}

        //void rl_delete_relooper(void *relooper) {
        //  delete (Relooper*)relooper;
        //}

        //void rl_relooper_add_block(void *relooper, void *block) {
        //#if DEBUG
        //  printf("  rl_relooper_add_block(rl, block_map[%d]);\n", ((Block*)block).Id);
        //#endif
        //  ((Relooper*)relooper).AddBlock((Block*)block);
        //}

        //void rl_relooper_calculate(void *relooper, void *entry) {
        //#if DEBUG
        //  printf("  rl_relooper_calculate(rl, block_map[%d]);\n", ((Block*)entry).Id);
        //  printf("  rl_relooper_render(rl);\n");
        //  printf("  rl_delete_relooper(rl);\n");
        //  printf("  puts(buffer);\n");
        //  printf("  return 0;\n");
        //  printf("}\n");
        //#endif
        //  ((Relooper*)relooper).Calculate((Block*)entry);
        //}

        //void rl_relooper_render(void *relooper) {
        //  ((Relooper*)relooper).Render();
        //}

        //}

    }
}