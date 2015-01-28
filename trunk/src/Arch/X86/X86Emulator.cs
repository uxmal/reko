#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.X86
{
    using Decompiler.Core.Machine;
    using System.Diagnostics;
    using TWord = System.UInt32;

    /// <summary>
    /// Simple emulator of X86 instructions. No attempt is made to be high-performance
    /// as long as correctness is maintained.
    /// </summary>
    public class X86Emulator
    {
        public event EventHandler BeforeStart;
        public event EventHandler ExceptionRaised;

        public const uint Cmask = 1u << 0;
        public const uint Zmask = 1u << 6;
        public const uint Omask = 1u << 11;

        private IntelArchitecture arch;
        private LoadedImage img;
        private Dictionary<Address, ImportReference> importReferences;
        private Win32Emulator envEmulator;
        IEnumerator<IntelInstruction> dasm;
        private bool running;
        private Dictionary<uint, Action> bpExecute = new Dictionary<uint, Action>();

        public readonly ulong[] Registers;
        public readonly bool[] Valid;
        public TWord Flags;
        private Address ip;
        private Dictionary<TWord, ImportReference> intereptedCalls;
        private TWord uPseudoFn;
        private Action stepAction;
        private bool stepInto;
        private TWord stepOverAddress;

        public X86Emulator(IntelArchitecture arch, LoadedImage loadedImage, Dictionary<Address, ImportReference> importReferences)
        {
            this.arch = arch;
            this.img = loadedImage;
            this.importReferences = importReferences;
            this.Registers = new ulong[40];
            this.Valid = new bool[40];
            this.envEmulator = new Win32Emulator();
            this.uPseudoFn = 0xDEAD0000u;
            this.intereptedCalls = new Dictionary<uint, ImportReference>();
            InterceptCallsToImports();
        }

        public Core.Address InstructionPointer
        {
            get { return ip; }
            set
            {
                ip = value;
                var rdr = arch.CreateImageReader(img, value);
                dasm = arch.CreateDisassembler(rdr).GetEnumerator();
            }
        }

        private void InterceptCallsToImports()
        {
            foreach (var imp in importReferences)
            {
                uint pseudoPfn = AddInterceptedCall(imp.Value);
                img.WriteLeUInt32(imp.Key, pseudoPfn);
            }
        }

        private TWord AddInterceptedCall(ImportReference importReference)
        {
            intereptedCalls[++this.uPseudoFn] = importReference;
            return uPseudoFn;
        }

        public void StepOver(Action callback)
        {
            stepOverAddress = (TWord)(dasm.Current.Address.Linear + dasm.Current.Length);
            stepAction = callback;
            Run();
        }

        public void StepInto(Action callback)
        {
            stepInto = true;
            stepAction = callback;
            Run();
        }

        public void Start()
        {
            running = true;
            CreateStack();
            BeforeStart.Fire(this);
            Run();
        }

        private StringBuilder DumpRegs()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 8; ++i)
            {
                sb.AppendFormat(" {0} {1:X8}", arch.GetRegister(i).Name, Registers[i]);
            }
            return sb;
        }

        private void Run()
        {
            int counter = 0;
            try
            {
                while (running && dasm.MoveNext())
                {
                    //Debug.Print("emu: {0} {1,-15} {2}", dasm.Current.Address, dasm.Current, DumpRegs());
                    Action bpAction;
                    TWord eip = dasm.Current.Address.Linear;
                    if (bpExecute.TryGetValue(eip, out bpAction))
                    {
                        ++counter;
                        stepOverAddress = 0;
                        stepInto = false;
                        bpAction();
                    }
                    else if (stepInto)
                    {
                        stepInto = false;
                        var s = stepAction;
                        stepAction = null;
                        s();
                    }
                    else if (stepOverAddress == eip)
                    {
                        stepOverAddress = 0;
                        var s = stepAction;
                        stepAction = null;
                        s();
                    }
                    Execute(dasm.Current);
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Emulator exception when executing {0}. {1}\r\n{2}", dasm.Current, ex.Message, ex.StackTrace);
                ExceptionRaised.Fire(this);
            }
        }

        public void Stop()
        {
            running = false;
        }

        public void CreateStack()
        {

        }

        public void Execute(IntelInstruction instr)
        {
            switch (instr.code)
            {
            default:
                throw new NotImplementedException(string.Format("Instruction emulation for {0} not implemented yet.", instr));
            case Opcode.adc: Adc(instr.op1, instr.op2); return;
            case Opcode.add: Add(instr.op1, instr.op2); return;
            case Opcode.and: And(instr.op1, instr.op2); return;
            case Opcode.call: Call(instr.op1); return;
            case Opcode.cmp: Cmp(instr.op1, instr.op2); return;
            case Opcode.dec: Dec(instr.op1); return;
            case Opcode.hlt: running = false; return;
            case Opcode.inc: Inc(instr.op1); return;
            case Opcode.ja: if ((Flags & (Cmask | Zmask)) == 0) InstructionPointer = ((AddressOperand)instr.op1).Address; return;
            case Opcode.jbe: if ((Flags & (Cmask | Zmask)) != 0) InstructionPointer = ((AddressOperand)instr.op1).Address; return;
            case Opcode.jc: if ((Flags & Cmask) != 0) InstructionPointer = ((AddressOperand)instr.op1).Address; return;
            case Opcode.jmp: Jump(instr.op1); return;
            case Opcode.jnc: if ((Flags & Cmask) == 0) InstructionPointer = ((AddressOperand)instr.op1).Address; return;
            case Opcode.jnz: if ((Flags & Zmask) == 0) InstructionPointer = ((AddressOperand)instr.op1).Address; return;
            case Opcode.jz: if ((Flags & Zmask) != 0) InstructionPointer = ((AddressOperand)instr.op1).Address; return;
            case Opcode.lea: Write(instr.op1, GetEffectiveAddress((MemoryOperand)instr.op2)); break;
            case Opcode.loop: Loop(instr.op1); break;
            case Opcode.mov: Write(instr.op1, Read(instr.op2)); break;
            case Opcode.or: Or(instr.op1, instr.op2); return;
            case Opcode.pop: Write(instr.op1, Pop()); return;
            case Opcode.popa: Popa(); return;
            case Opcode.push: Push(Read(instr.op1)); return;
            case Opcode.pusha: Pusha(); return;
            case Opcode.repne: Repne(); return;
            case Opcode.rol: Rol(instr.op1, instr.op2); return;
            case Opcode.scasb: Scasb(); return;
            case Opcode.shl: Shl(instr.op1, instr.op2); return;
            case Opcode.shr: Shr(instr.op1, instr.op2); return;
            case Opcode.sub: Sub(instr.op1, instr.op2); return;
            case Opcode.xor: Xor(instr.op1, instr.op2); return;
            case Opcode.xchg: Xchg(instr.op1, instr.op2); return;
            }
        }


        private void Adc(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            TWord sum = l + r + (Flags & 1);
            Write(dst, sum);
            uint ov = ((~(l ^ r) & (l ^ sum)) & 0x80000000u) >> 20;
            Flags =
                (r > sum ? 1u : 0u) |       // Carry
                (sum == 0 ? 1u << 6 : 0u) | // Zero
                (ov)                        // Overflow
                ;
        }

        private void Add(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            TWord sum = l + r;
            Write(dst, sum);
            uint ov = ((~(l ^ r) & (l ^ sum)) & 0x80000000u) >> 20;
            Flags =
                (r > sum ? 1u : 0u) |     // Carry
                (sum == 0 ? 1u << 6: 0u)  | // Zero
                (ov)                        // Overflow
                ;
        }

        private void Repne()
        {
            dasm.MoveNext();
            var strInstr = dasm.Current;
            uint ecx = ReadRegister(X86.Registers.ecx);
            if  (ecx != 0)
            {
                for (; ; )
                {
                    Execute(strInstr);
                    --ecx;
                    if (ecx == 0)
                        break;
                    if ((Flags & Zmask) != 0)
                        break;
                }
            }
        }

        private void Rol(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            byte sh = (byte)Read(src);
            TWord r = (l << sh) | (l >> (32 - sh));
            Write(dst, r);
            Flags =
                (r == 0 ? Zmask : 0u);      // Zero
        }

        private void Scasb()
        {
            byte al = (byte) ReadRegister(X86.Registers.al);
            TWord edi = ReadRegister(X86.Registers.edi);
            byte mem = (byte)(al - img.Bytes[edi - img.BaseAddress.Linear]);
            WriteRegister(X86.Registers.edi, edi + 1);      //$BUG: Direction flag not respected
            Flags =
                (mem == 0 ? Zmask : 0u);
        }

        private void Shl(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            byte sh = (byte)Read(src);
            TWord r = l << sh;
            Write(dst, r);
            Flags =
                (r == 0 ? Zmask : 0u);      // Zero
        }

        private void Shr(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            byte sh = (byte)Read(src);
            TWord r = l >> sh;
            Write(dst, r);
            Flags =
                (r == 0 ? Zmask : 0u);      // Zero
        }

        private void Call(MachineOperand op)
        {
            Push(InstructionPointer.Linear + (uint)dasm.Current.Length);   // Push return value on stack
      
            TWord l = Read(op);
            ImportReference impProc;
            if (this.intereptedCalls.TryGetValue(l, out impProc))
            {
                // Called an imported procedure. //$REVIEW: this should go into an "EnvironmentEmulator" 
                // and a Win32Emulator would take of understanding what "loadLibraryA" does, for instance.
                envEmulator.CallImportedProcedure(this, impProc);
                return;
            }

            InstructionPointer = new Address(l);
        }

        private void Jump(MachineOperand op)
        {
            TWord l = Read(op);
            ImportReference impProc;
            if (this.intereptedCalls.TryGetValue(l, out impProc))
            {
                throw new NotImplementedException();
            }
            InstructionPointer = new Address(l);
        }

        private void Cmp(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            r = ~r + 1u;
            TWord diff = l + r;
            uint ov = ((~(l ^ r) & (l ^ diff)) & 0x80000000u) >> 20;
            Flags =
                (l < diff ? 1u : 0u) |     // Carry
                (diff == 0 ? Zmask : 0u) | // Zero
                (ov)                        // Overflow
                ;
        }

        private void Sub(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            r = ~r + 1u;        // Two's complement subtraction.
            TWord diff = l + r;
            Write(dst, diff);
            uint ov = ((~(l ^ r) & (l ^ diff)) & 0x80000000u) >> 20;
            Flags =
                (l < diff ? 1u : 0u) |     // Carry
                (diff == 0 ? Zmask : 0u) | // Zero
                (ov)                        // Overflow
                ;
        }


        private void And(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            var and = l & r;
            Write(dst, and);
            Flags =
                0 |                         // Clear Carry
                (and == 0 ? Zmask : 0u) |    // Zero
                0;                          // Clear Overflow
        }

        private void Or(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            var or = l | r;
            Write(dst, or);
            Flags =
                0 |                         // Clear Carry
                (or == 0 ? Zmask : 0u) |    // Zero
                0;                          // Clear Overflow
        }


        private void Dec(MachineOperand op)
        {
            TWord old = Read(op);
            TWord gnu = old - 1;
            Write(op, gnu);
            uint ov = ((old ^ gnu) & ~gnu & 0x80000000u) >> 20;
            Flags =
                Flags & Cmask |             // Carry preserved
                (gnu == 0 ? Zmask : 0u) |   // Zero
                ov;                          //$BUG:
        }
        private void Inc(MachineOperand op)
        {
            TWord old = Read(op);
            TWord gnu = old + 1;
            Write(op, gnu);
            uint ov = ((old ^ gnu) & gnu & 0x80000000u) >> 20;
            Flags =
                Flags & Cmask |             // Carry preserved
                (gnu == 0 ? Zmask : 0u) |   // Zero
                ov;                          //$BUG:
        }

        private void Xor(MachineOperand dst, MachineOperand src)
        {
            TWord l = Read(dst);
            TWord r = Read(src);
            if (src.Width.Size < dst.Width.Size)
                r = (TWord)(sbyte)r;
            var xor = l ^ r;
            Write(dst, xor);
            Flags =
                0 |                         // Carry
                (xor == 0 ? Zmask : 0u) |   // Zero
                0;                          // Overflow
        }

        private TWord Read(MachineOperand op)
        {
            var r = op as RegisterOperand;
            if (r != null)
            {
                return ReadRegister(r.Register);
            }
            var i = op as ImmediateOperand;
            if (i != null)
                return i.Value.ToUInt32();
            var a = op as AddressOperand;
            if (a != null)
                return a.Address.Linear;
            var m = op as MemoryOperand;
            if (m != null)
            {
                TWord ea = GetEffectiveAddress(m);
                switch (op.Width.Size)
                {
                case 1: return img.ReadByte(new Address(ea));
                case 4: return img.ReadLeUInt32(new Address(ea));
                }
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        private TWord GetEffectiveAddress(MemoryOperand m)
        {
            TWord ea = 0;
            if (m.Offset.IsValid)
                ea += m.Offset.ToUInt32();
            if (m.Index != RegisterStorage.None)
                ea += ReadRegister(m.Index) * m.Scale;
            if (m.Base != null && m.Base != RegisterStorage.None)
            {
                ea += ReadRegister(m.Base);
            }
            return ea;
        }

        private TWord ReadRegister(RegisterStorage r)
        {
            return (TWord) Registers[r.Number];
        }

        private void Write(MachineOperand op, TWord w)
        {
            var r = op as RegisterOperand;
            if (r != null)
            {
                WriteRegister(r.Register, w);
                return;
            }
            var m = op as MemoryOperand;
            if (m != null)
            {
                var ea = GetEffectiveAddress(m);
                switch (op.Width.Size)
                {
                case 1: img.WriteByte(new Address(ea), (byte)w); return;
                case 4: img.WriteLeUInt32(new Address(ea), (UInt32)w); return;
                }
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        public void WriteRegister(RegisterStorage r, TWord value)
        {
            ((IntelRegister)r).SetRegisterFileValues(Registers, value, Valid);
        }

        public void Loop(MachineOperand op)
        {
            var c = ReadRegister(X86.Registers.ecx)  -1u;
            WriteRegister(X86.Registers.ecx, c);
            if (c != 0)
                InstructionPointer = ((AddressOperand)op).Address;
        }

        public void Popa()
        {
            Registers[X86.Registers.edi.Number] = Pop();
            Registers[X86.Registers.esi.Number] = Pop();
            Registers[X86.Registers.ebp.Number] = Pop();
            Pop();
            Registers[X86.Registers.ebx.Number] = Pop();
            Registers[X86.Registers.edx.Number] = Pop();
            Registers[X86.Registers.ecx.Number] = Pop();
            Registers[X86.Registers.eax.Number] = Pop();
        }

        public void Pusha()
        {
            var temp = Registers[X86.Registers.esp.Number];
            Push(Registers[X86.Registers.eax.Number]);
            Push(Registers[X86.Registers.ecx.Number]);
            Push(Registers[X86.Registers.edx.Number]);
            Push(Registers[X86.Registers.ebx.Number]);
            Push(temp);
            Push(Registers[X86.Registers.ebp.Number]);
            Push(Registers[X86.Registers.esi.Number]);
            Push(Registers[X86.Registers.edi.Number]);
        }

        public TWord Pop()
        {
            var esp = Registers[X86.Registers.esp.Number];
            var u = (uint)esp - img.BaseAddress.Linear;
            var word = img.ReadLeUInt32(u);
            esp += 4;
            WriteRegister(X86.Registers.esp, (uint)esp);
            return word;
        }

        public void Push(ulong word)
        {
            var esp = Registers[X86.Registers.esp.Number] - 4;
            var u = (uint)esp - img.BaseAddress.Linear;
            img.WriteLeUInt32(u, (uint) word);
            WriteRegister(X86.Registers.esp, (uint) esp);
        }

        private void Xchg(MachineOperand op1, MachineOperand op2)
        {
            var tmp = Read(op1);
            Write(op1, Read(op2));
            Write(op2, tmp);
        }

        public void SetBreakpoint(uint address, Action callback)
        {
            bpExecute.Add(address, callback);
        }

        public void DeleteBreakpoint(uint address)
        {
            bpExecute.Remove(address);
        }

        public class SimulatedProc : ExternalProcedure
        {
            public SimulatedProc(string name, Action<X86Emulator> emulator) : base(name, null) { Emulator = emulator; }

            public Action<X86Emulator> Emulator;
        }

        public class Win32Emulator : IImportResolver
        {
            private List<string> modules; 

            Dictionary<string, Dictionary<string, Action<X86Emulator>>> wellKnownFunctions;
            private ExternalProcedure epDummy;

            public Win32Emulator()
            {
                modules = new List<string>
                {
                    "kernel32.dll",
                    "user32.dll"
                };
          
                wellKnownFunctions = new Dictionary<string, Dictionary<string, Action<X86Emulator>>>(StringComparer.InvariantCultureIgnoreCase)
                {
                    { "kernel32.dll", new Dictionary<string, Action<X86Emulator>>
                        {
                            { "LoadLibraryA", LoadLibraryA },
                            { "GetProcAddress", GetProcAddress },
                            { "ExitProcess", ExitProcess },
                            { "VirtualProtect", VirtualProtect }
                        }
                     },
                     { "user32.dll", new Dictionary<string, Action<X86Emulator>>
                     {
                         { "MessageBoxA", MessageBoxA }
                     }
                     }
                };
                epDummy = new ExternalProcedure(">Dummy<", null);
            }


            void LoadLibraryA(X86Emulator emulator)
            {
                // M[Esp] is return address.
                // M[Esp + 4] is pointer to DLL name.
                uint esp = (uint)emulator.Registers[X86.Registers.esp.Number];
                uint pstrLibName = emulator.img.ReadLeUInt32(esp + 4u - emulator.img.BaseAddress.Linear);
                string szLibName = ReadMbString(emulator.img, pstrLibName);
                uint hModule = (uint) modules.IndexOf(szLibName.ToLower());
                if ((int)hModule < 0)
                    throw new NotImplementedException(string.Format("Unknown library {0}.", szLibName));
                hModule += 10;
                emulator.WriteRegister(X86.Registers.eax, hModule);

                // Clean up the stack.
                emulator.WriteRegister(X86.Registers.esp, esp + 8);
            }

            void GetProcAddress(X86Emulator emulator)
            {
                // M[esp] is return address
                // M[esp + 4] is hmodule
                // M[esp + 4] is pointer to function name
                uint esp = (uint)emulator.Registers[X86.Registers.esp.Number];
                uint hmodule = emulator.img.ReadLeUInt32(esp + 4u - emulator.img.BaseAddress.Linear);
                uint pstrFnName = emulator.img.ReadLeUInt32(esp + 8u - emulator.img.BaseAddress.Linear);
                if ((pstrFnName & 0xFFFF0000) != 0)
                {
                    string importName = ReadMbString(emulator.img, pstrFnName);
                    hmodule -= 10;
                    var moduleName = modules[(int)hmodule];
                    Dictionary<string, Action<X86Emulator>> module;
                    wellKnownFunctions.TryGetValue(moduleName, out module);
                    Action<X86Emulator> fn;
                    if (!module.TryGetValue(importName, out fn))
                        throw new NotImplementedException();
                    uint uIntercept = emulator.AddInterceptedCall(new NamedImportReference(null, moduleName, importName));
                    emulator.WriteRegister(X86.Registers.eax, uIntercept);
                    emulator.WriteRegister(X86.Registers.esp, esp + 12);

                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            void ExitProcess(X86Emulator emulator)
            {
                emulator.Stop();
            }

            void VirtualProtect(X86Emulator emulator)
            {
                uint esp = (uint)emulator.Registers[X86.Registers.esp.Number];
                emulator.WriteRegister(X86.Registers.eax, 1u);
                emulator.WriteRegister(X86.Registers.esp, esp + 20);
            }

            void MessageBoxA(X86Emulator emulator)
            {
                throw new NotImplementedException();
            }

            private string ReadMbString(LoadedImage img, TWord pstrLibName)
            {
                int iStart  = (int)(pstrLibName - img.BaseAddress.Linear);
                int i = iStart;
                for (i = iStart; img.Bytes[i] != 0; ++i)
                {
                }
                return Encoding.ASCII.GetString(img.Bytes, iStart, i - iStart);
            }

            internal void CallImportedProcedure(X86Emulator emu, ImportReference impProc)
            {
                var proc = impProc.ResolveImportedProcedure(this, null, null);
                var simProc = proc as SimulatedProc;
                if (simProc == null)
                    throw new NotImplementedException();
                simProc.Emulator(emu);
            }

            ExternalProcedure IImportResolver.ResolveProcedure(string moduleName, string importName, Platform platform)
            {
                Dictionary<string, Action<X86Emulator>> module;
                if (!this.wellKnownFunctions.TryGetValue(moduleName, out module))
                    return epDummy;
                Action<X86Emulator> fn;
                if (!module.TryGetValue(importName, out fn))
                    return epDummy;
                return new SimulatedProc(importName, fn);
            }

            ExternalProcedure IImportResolver.ResolveProcedure(string moduleName, int ordinal, Platform platform)
            {
                throw new NotImplementedException();
            }
        }
    }
}