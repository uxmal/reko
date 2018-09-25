#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.zSeries
{
    using Mutator = Func<uint, zSeriesDisassembler, bool>;

    public class zSeriesDisassembler : DisassemblerBase<zSeriesInstruction>
    {
        private readonly static Decoder[] decoders;

        private readonly zSeriesArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly State state;
        private Address addr;

        public zSeriesDisassembler(zSeriesArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = new State();
        }

        public override zSeriesInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            if (!rdr.TryReadBeUInt16(out var opcode))
                return null;
            var instr = decoders[opcode >> 8].Decode(opcode, this);
            state.Reset();
            instr.Address = addr;
            instr.Length = (int)(rdr.Address - addr);
            return instr;
        }


        private zSeriesInstruction Invalid()
        {
            return new zSeriesInstruction
            {
                Opcode = Opcode.invalid,
                Ops = new MachineOperand[0]
            };
        }

        private static HashSet<uint> seen = new HashSet<uint>();

        private void Nyi(uint uInstr, string message)
        {
#if DEBUG
            if (true && !seen.Contains(uInstr))
            {
                seen.Add(uInstr);

                var rdr2 = rdr.Clone();
                int len = (int)(rdr.Address - this.addr);
                rdr2.Offset -= len;
                var bytes = rdr2.ReadBytes(len);
                var hexBytes = string.Join("", bytes
                        .Select(b => b.ToString("X2")));
                Console.WriteLine($"// A zSeries decoder for the instruction {uInstr:X8} ({message}) has not been implemented yet.");
                Console.WriteLine("[Test]");
                Console.WriteLine($"public void zSysDasm_{uInstr:X8}()");
                Console.WriteLine("{");
                Console.WriteLine("    AssertCode(\"@@@\", \"{0}\");", hexBytes);
                Console.WriteLine("}");
                Console.WriteLine();
            }
#endif
        }

        private class State
        {
            public List<MachineOperand> ops = new List<MachineOperand>();
            public Opcode opcode;

            public void Reset()
            {
                this.opcode = Opcode.invalid;
                this.ops.Clear();
            }

            public zSeriesInstruction MakeInstruction()
            {
                var instr = new zSeriesInstruction
                {
                    Opcode = this.opcode,
                    Ops = this.ops.ToArray(),
                };
                return instr;
            }
        }

        #region Mutators

        public static bool RR(uint uInstr, zSeriesDisassembler dasm)
        {
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr >> 4) & 0xF]));
            dasm.state.ops.Add(new RegisterOperand(Registers.GpRegisters[(uInstr) & 0xF]));
            return true;
        }

        #endregion

        public abstract class Decoder
        {
            public abstract zSeriesInstruction Decode(uint uInstr, zSeriesDisassembler dasm);
        }

        public class InstrDecoder : Decoder
        {
            private Opcode opcode;
            private Mutator[] mutators;

            public InstrDecoder(Opcode opcode, params Mutator[] mutators)
            {
                this.opcode = opcode;
                this.mutators = mutators;
            }

            public override zSeriesInstruction Decode(uint uInstr, zSeriesDisassembler dasm)
            {
                dasm.state.opcode = opcode;
                foreach (var m in mutators)
                {
                    if (!m(uInstr, dasm))
                        return dasm.Invalid();
                }
                return dasm.state.MakeInstruction();
            }
        }

        public class NyiDecoder : Decoder
        {
            private string msg;

            public NyiDecoder(string msg = "")
            {
                this.msg = msg;
            }

            public override zSeriesInstruction Decode(uint uInstr, zSeriesDisassembler dasm)
            {
                dasm.Nyi(uInstr, msg);
                return dasm.Invalid();
            }
        }

        public static InstrDecoder Instr(Opcode opcode, params Mutator[] mutators)
        {
            return new InstrDecoder(opcode, mutators);
        }

        public static NyiDecoder Nyi(string msg)
        {
            return new NyiDecoder(msg);
        }

        static zSeriesDisassembler()
        {
            decoders = new Decoder[256]
            {
                // 00
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 10
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Instr(Opcode.nr, RR),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Instr(Opcode.ar, RR),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 20
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 30
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 40
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 50
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 60
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 70
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 80
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // 90
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // A0
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // B0
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // C0
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // D0
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // E0
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                // F0
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),

                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
                Nyi("*"),
            };
        }
    }
}