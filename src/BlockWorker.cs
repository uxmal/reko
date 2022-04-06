using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Evaluation;
using System.Collections.Generic;

namespace Reko.ScannerV2
{
    public class BlockWorker : AbstractBlockWorker
    {
        private readonly ProcessorState state;
        private readonly ExpressionSimplifier eval;

        public BlockWorker(
            RecursiveScanner scanner,
            ProcedureWorker worker,
            Address address,
            IEnumerator<RtlInstructionCluster> trace,
            ProcessorState state)
            : base(scanner, worker, address, trace, state)
        {
            this.state = state;
            this.eval = scanner.CreateEvaluator(state);
        }

        protected override void EmulateState(RtlAssignment ass)
        {
            try
            {
                var value = GetValue(ass.Src);
                switch (ass.Dst)
                {
                case Identifier id:
                    state.SetValue(id, value);
                    return;
                case SegmentedAccess smem:
                    state.SetValueEa(smem.BasePointer, GetValue(smem.EffectiveAddress), value);
                    return;
                case MemoryAccess mem:
                    state.SetValueEa(GetValue(mem.EffectiveAddress), value);
                    return;
                }
            } catch
            {
                // Drop all on the floor.
            }
        }

        private Expression GetValue(Expression e)
        {
            var (value, _) = e.Accept(eval);
            return value;
        }
    }
}
