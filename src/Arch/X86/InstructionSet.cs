#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.X86
{
    using Decoder = Decoder<X86Disassembler, Mnemonic, X86Instruction>;

    public partial class X86Disassembler
    {
        /// <summary>
        /// This class acts as a factory that builds up the <see cref="Decoder"/>s that
        /// the <see cref="X86Disassembler"/> needs. It is customizable in order to 
        /// accomodate differences between x86 models.
        /// </summary>
        public partial class InstructionSet
        {
            internal static readonly Decoder s_invalid;
            private static readonly Decoder s_nyi;
            private readonly Decoder[] rootDecoders;
            private readonly Decoder[] s_decoders0F;
            private readonly Decoder[] s_decoders0F38;
            private readonly Decoder[] s_decoders0F3A;
            private readonly Decoder[] Grp1;
            private readonly Decoder[] Grp1A;
            private readonly Decoder[] Grp2;
            private readonly Decoder[] Grp3;
            private readonly Decoder[] Grp4;
            private readonly Decoder[] Grp5;
            private readonly Decoder[] Grp6;
            private readonly Decoder[] Grp7;
            private readonly Decoder[] Grp8;
            private readonly Decoder[] Grp9;
            private readonly Decoder[] Grp10;
            private readonly Decoder[] Grp11b;
            private readonly Decoder[] Grp11z;
            private readonly Decoder[] Grp12;
            private readonly Decoder[] Grp13;
            private readonly Decoder[] Grp14;
            private readonly Decoder[] Grp15;
            private readonly Decoder[] Grp16;
            private readonly Decoder[] Grp17;
            private readonly Decoder[] s_fpuDecoders;
            private Func<Mnemonic, InstrClass, Mutator<X86Disassembler>[], Decoder> rexInstr;
            private Func<Mnemonic, InstrClass, Mutator<X86Disassembler>[], Decoder> instr186;
            private Func<Decoder, Decoder> instr286;
            private Func<Decoder, Decoder> instr386;
            private Func<Mnemonic, InstrClass, Mutator<X86Disassembler>[], Decoder> instr486;
            private Func<Decoder> x87instr;
            private Func<Decoder, Decoder, Decoder> amd64instr;

            public static InstructionSet Create(
                bool x86_64,
                bool useRexPrefix,
                Dictionary<string, object> options)
            {
                var isa = new InstructionSet();

                if (x86_64)
                    isa.amd64instr = (x86, x64) => x64;
                else
                    isa.amd64instr = (x86, x64) => x86;

                if (useRexPrefix)
                    isa.rexInstr = isa.MakeRexDecoder;
                else
                    isa.rexInstr = Instr;

                if (options.TryGetValue(ProcessorOption.InstructionSet, out var oIsa))
                {
                    switch (oIsa.ToString())
                    {
                    case "8086":
                        isa.instr186 = MakeInvalid; goto case "80186";
                    case "80186":
                        isa.instr286 = MakeInvalid; goto case "80286";
                    case "80286":
                        isa.instr386 = MakeInvalid; goto case "80386";
                    case "80386":
                        isa.instr486 = MakeInvalid; goto case "80486";
                    case "80486":
                        //$TODO: Pentium support.
                        break;
                    }
                }
                return isa;
            }

#nullable disable
            public InstructionSet()
            {
                this.rootDecoders = new Decoder[256];
                this.s_decoders0F = new Decoder[256];
                this.s_decoders0F38 = new Decoder[256];
                this.s_decoders0F3A = new Decoder[256];
                this.Grp1 = new Decoder[8];
                this.Grp1A = new Decoder[8];
                this.Grp2 = new Decoder[8];
                this.Grp3 = new Decoder[8];
                this.Grp4 = new Decoder[8];
                this.Grp5 = new Decoder[8];
                this.Grp6 = new Decoder[8];
                this.Grp7 = new Decoder[8];
                this.Grp8 = new Decoder[8];
                this.Grp9 = new Decoder[8];
                this.Grp10 = new Decoder[8];
                this.Grp11b = new Decoder[8];
                this.Grp11z = new Decoder[8];
                this.Grp12 = new Decoder[8];
                this.Grp13 = new Decoder[8];
                this.Grp14 = new Decoder[8];
                this.Grp15 = new Decoder[8];
                this.Grp16 = new Decoder[8];
                this.Grp17 = new Decoder[8];
                this.s_fpuDecoders = CreateFpuDecoders();
                x87instr = () => new X87Decoder(s_fpuDecoders);
                instr186 = Instr;
                instr286 = Identity;
                instr386 = Identity;
                instr486 = Instr;
            }
#nullable enable

            private static Decoder Identity(Decoder d) => d;

            private static AddrWidthDecoder AddrWidthDependent(
                Decoder? bit16 = null,
                Decoder? bit32 = null,
                Decoder? bit64 = null)
            {
                return new AddrWidthDecoder(
                    bit16 ?? s_invalid,
                    bit32 ?? s_invalid,
                    bit64 ?? s_invalid);
            }

            public Decoder[] CreateRootDecoders()
            {
                CreateGroupDecoders();
                Create0F38Decoders(s_decoders0F38);
                Create0F3ADecoders(s_decoders0F3A);
                CreateOnebyteDecoders(s_decoders0F);
                CreateTwobyteDecoders(s_decoders0F);

                Debug.Assert(s_fpuDecoders.Length == 8 * 0x48);
                return rootDecoders;
            }

            public static DataWidthDecoder DataWidthDependent(
                Decoder? bit16 = null,
                Decoder? bit32 = null,
                Decoder? bit64 = null)
            {
                return new DataWidthDecoder(
                    bit16 ?? s_invalid,
                    bit32 ?? s_invalid,
                    bit64 ?? s_invalid);
            }

            private static Decoder MakeInvalid(Mnemonic mnemonic, Core.InstrClass iclass, Mutator<X86Disassembler>[] mutators)
            {
                return s_invalid;
            }

            private static Decoder MakeInvalid(Decoder decoder)
            {
                return s_invalid;
            }

            public Decoder Instr186(Mnemonic mnemonic, params Mutator<X86Disassembler>[] mutators)
            {
                return instr186(mnemonic, Core.InstrClass.Linear, mutators);
            }

            public Decoder Instr186(Mnemonic mnemonic, InstrClass iclass, params Mutator<X86Disassembler>[] mutators)
            {
                return instr186(mnemonic, iclass, mutators);
            }

            public Decoder Instr286(Mnemonic mnemonic, params Mutator<X86Disassembler>[] mutators)
            {
                return instr286(Instr(mnemonic, Core.InstrClass.Linear, mutators));
            }

            public Decoder Instr286(Decoder decoder)
            {
                return instr286(decoder);
            }

            public Decoder Instr386(Mnemonic mnemonic, params Mutator<X86Disassembler>[] mutators)
            {
                return instr386(Instr(mnemonic, Core.InstrClass.Linear, mutators));
            }

            public Decoder Instr386(Mnemonic mnemonic, InstrClass iclass, params Mutator<X86Disassembler>[] mutators)
            {
                return instr386(Instr(mnemonic, iclass, mutators));
            }

            public Decoder Instr386(Decoder decoder)
            {
                return instr386(decoder);
            }

            public Decoder Instr486(Mnemonic mnemonic, params Mutator<X86Disassembler>[] mutators)
            {
                return instr486(mnemonic, Core.InstrClass.Linear, mutators);
            }

            public Decoder RexInstr(Mnemonic mnemonic, params Mutator<X86Disassembler>[] mutators)
            {
                return rexInstr(mnemonic, InstrClass.Linear, mutators);
            }

            public Decoder MakeRexDecoder(Mnemonic mnemonic, InstrClass iclass, Mutator<X86Disassembler>[] mutators)
            {
                return new Rex_or_InstructionDecoder(this.rootDecoders);
            }

            public static VexInstructionDecoder VexInstr(Mnemonic vex, params Mutator<X86Disassembler>[] mutators)
            {
                var legDec = s_invalid;
                var vexDec = Instr(vex, mutators);
                return new VexInstructionDecoder(legDec, vexDec);
            }

            public static VexInstructionDecoder VexInstr(Decoder legacy, Decoder vex)
            {
                return new VexInstructionDecoder(legacy, vex);
            }

            public static VexInstructionDecoder VexInstr(Mnemonic legacy, Mnemonic vex, params Mutator<X86Disassembler>[] mutators)
            {
                var legDec = legacy != Mnemonic.illegal
                    ? Instr(legacy, mutators)
                    : s_invalid;
                var vexDec = Instr(vex, mutators);
                return new VexInstructionDecoder(legDec, vexDec);
            }

            /// <summary>
            /// Selects a decoder based on the VEX 'L' (long bit(s))
            /// </summary>
            public static Decoder VexLong(Decoder instrNotLong, Decoder instrLong)
            {
                return new VexLongDecoder(instrNotLong, instrLong, instrLong);
            }

            public static Decoder VexLong(params Decoder [] decoders)
            {
                return new VexLongDecoder(decoders);
            }

            public static EvexInstructionDecoder EvexInstr(Decoder? legacy = null, Decoder? vex = null, Decoder? evex = null)
            {
                Debug.Assert(s_invalid is not null);
                legacy ??= s_invalid;
                vex ??= legacy;
                evex ??= vex;
                return new EvexInstructionDecoder(legacy, vex, evex);
            }

            public Decoder X87Instr()
            {
                return x87instr();
            }

            public Decoder Amd64Instr(Decoder legacy, Decoder amd64)
            {
                return amd64instr(legacy, amd64);
            }

            static InstructionSet()
            {
                s_invalid = Instr(Mnemonic.illegal, InstrClass.Invalid);
                s_nyi = nyi("This could be invalid or it could be not yet implemented");
            }
        }
    }
}
