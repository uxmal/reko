using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.WindowsItp.Decoders
{
    public class Disassembler : IEnumerable<TestInstruction>
    {
        private static readonly RegisterStorage[] Registers;
        private static readonly RegisterOperand[] RegOps;

        private readonly EndianImageReader rdr;
        private readonly Decoder root;
        private readonly List<MachineOperand> operands;
        private Opcode opcode;

        public Disassembler(EndianImageReader rdr, Decoder root)
        {
            this.rdr = rdr;
            this.root = root;
            this.opcode = Opcode.Invalid;
            this.operands = new List<MachineOperand>();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<TestInstruction> IEnumerable<TestInstruction>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TestInstruction Disassemble()
        {
            if (!rdr.TryReadUInt32(out var uInstr))
                return null;
            operands.Clear();
            return root.Decode(uInstr, this);
        }

        public TestInstruction Invalid()
        {
            return new TestInstruction
            {
                Opcode = Opcode.Invalid,
            };
        }

        public TestInstruction MakeInstruction()
        {
#if !ARRAY_OPERANDS
            var aOps = new MachineOperand[operands.Count];
            var c = aOps.Length;
            for (int i =0; i < aOps.Length; ++i)
            {
                aOps[i] = operands[i];
            }
#endif
            return new TestInstruction
            {
                Opcode = opcode,
#if !ARRAY_OPERANDS
                Operands = aOps,
#else 
                Op1 = operands.Count > 0 ? operands[0] : null,
                Op2 = operands.Count > 1 ? operands[1] : null,
                Op3 = operands.Count > 2 ? operands[2] : null,
#endif
            };
        }

        public TestInstruction Decode(uint wInstr, Opcode opcode, string format)
        {
            this.opcode = opcode;
            for (int i = 0; i < format.Length; ++i)
            {
                switch (format[i])
                {
                case ',':
                    continue;
                case 'r':
                    {
                        ++i;
                        int n = ReadDecimal(format, ref i);
                        var iReg = Bits.ZeroExtend(wInstr >> n, 4);
                        operands.Add(new RegisterOperand(Registers[iReg]));
                    }
                    break;
                default:
                    throw new NotImplementedException($"{format[i]}");
                }
            }
            return this.MakeInstruction();
        }

        class State
        {
            public Opcode opcode;
            public List<MachineOperand> operands = new List<MachineOperand>();

            public void Reset()
            {
                this.opcode = Opcode.Invalid;
                this.operands.Clear();
            }

            public TestInstruction MakeInstruction()
            {
                return new TestInstruction
                {
                    Opcode = opcode,
                    //Op1 = operands.Count > 0 ? operands[0] : null,
                    //Op2 = operands.Count > 1 ? operands[1] : null,
                    //Op3 = operands.Count > 2 ? operands[2] : null,
                };
            }
        }

        public static int ReadDecimal(string format, ref int i)
        {
            int n = 0;
            while (i < format.Length && char.IsDigit(format[i]))
            {
                n = n * 10 + (format[i] - '0');
                ++i;
            }
            return n;
        }

        public static Mutator<Disassembler> Reg(int n)
        {
            var regField = new Bitfield(n, 4);
            return (u, d) =>
            {
                var nReg = regField.Read(u);
                d.operands.Add(RegOps[nReg]);
                return true;
            };
        }

        static Disassembler()
        {
            Registers = Enumerable.Range(0, 16)
                .Select(n => new RegisterStorage($"r{n}", n, 0, PrimitiveType.Word32))
                .ToArray();
            RegOps = Registers.Select(r => new RegisterOperand(r)).ToArray();
        }

        public struct Enumerator : IEnumerator<TestInstruction>
        {
            private Disassembler dasm;
            private TestInstruction instr;

            public Enumerator(Disassembler dasm)
            {
                this.dasm = dasm;
                this.instr = null;
            }

            public TestInstruction Current => instr ?? throw new InvalidOperationException();

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                var instr = dasm.Disassemble();
                if (instr == null)
                    return false;
                this.instr = instr;
                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }
    }
}
