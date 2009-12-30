using Decompiler.Arch.Intel;
using Decompiler.Assemblers.x86;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Arch.Intel
{
    public abstract class AssemblerFragment
    {
        protected ParsedOperand eax = new ParsedOperand(new RegisterOperand(Registers.eax));
        protected ParsedOperand ecx = new ParsedOperand(new RegisterOperand(Registers.ecx));
        protected ParsedOperand edx = new ParsedOperand(new RegisterOperand(Registers.edx));
        protected ParsedOperand ebx = new ParsedOperand(new RegisterOperand(Registers.ebx));
        protected ParsedOperand ebp = new ParsedOperand(new RegisterOperand(Registers.ebp));
        protected ParsedOperand esp = new ParsedOperand(new RegisterOperand(Registers.esp));
        protected ParsedOperand esi = new ParsedOperand(new RegisterOperand(Registers.esi));
        protected ParsedOperand edi = new ParsedOperand(new RegisterOperand(Registers.edi));
        protected ParsedOperand ax = new ParsedOperand(new RegisterOperand(Registers.ax));
        protected ParsedOperand cx = new ParsedOperand(new RegisterOperand(Registers.cx));
        protected ParsedOperand dx = new ParsedOperand(new RegisterOperand(Registers.dx));
        protected ParsedOperand bx = new ParsedOperand(new RegisterOperand(Registers.bx));
        protected ParsedOperand bp = new ParsedOperand(new RegisterOperand(Registers.bp));
        protected ParsedOperand sp = new ParsedOperand(new RegisterOperand(Registers.sp));
        protected ParsedOperand si = new ParsedOperand(new RegisterOperand(Registers.si));
        protected ParsedOperand di = new ParsedOperand(new RegisterOperand(Registers.di));

        public abstract void Build(IntelAssembler m);
    }
}
