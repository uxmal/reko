using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Sparc
{
#if NEVER
    public class Expression 
    {
    }

    public class RtlInstruction
    {
        internal RtlTranslator.Class Class()
        {
 	        throw new NotImplementedException();
        }

        public RtlInstruction[] I_c;

    }

    public class RtlIf : RtlInstruction
    {
        public Expression b;  // branchExpr;
        public Expression nPC; //branchTarget;
        public bool Annul;

    }

    class RtlTranslator
    {
        public enum Class
        {
            NCT,        // non-control-transfer, b = false, a = true
            SU,         // Static unconditional, b = true, a = true
            SD,         // static delayed, b = true, a = false
            SCD,        // static conditional delayed.
        }
        void translate(object pc_s, object pc_t)
        {
            trans:
            // put (pc_s, pc_t) in codemap if not already there

            // get instruction from pc_s
            var I =  GetNextInstr();

            RtlInstruction I_;

            switch (I.Class())
            {
            case Class.NCT:
                emit(I.Rtl);
                goto trans;
            case Class.SU:
                emit(goto I.nPC);
                queueForTranslation(I.nPC);
            case Class.SD:
            case Class.DD:
                // get next instruction.
                I_ = GetNextInstr();
                switch (I_.Class())
                {
                case Class.NCT:
                    emit(I.I_c);
                    emit(I_.I_c);
                    emitGoto(I.nPc);
                    queueForTranslation(I.nPC);
                }
            case Class.SCD:
                I_ = GetNextInstr();
                switch (I_.Class())
                {
                case Class.NCT:
                    var bb = newBlock();
                    emit(I_.I_c);
                    emit("if I.b goto bb");

                    // switch to bb.
                    emit(I_.I_c);
                    emitGoto(I.nPC);
                    goto trans;
                }
            }

        }

        private

private void emit(RtlInstruction[] rtlInstruction)
{
 	throw new NotImplementedException();
} RtlInstruction GetNextInstr()
        {
 	        throw new NotImplementedException();
        //        new RtlIf
        //    {
        //        b = true,       // branch condition
        //        nPC = 0x42,     // next pc if branch
        //        a = false,      // annul

        //        I_c = new int[0] // Rtl to execute.
        //    };
        //}
        }
    }
#endif
}
