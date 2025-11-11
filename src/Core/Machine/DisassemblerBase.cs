#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Core.Machine
{
    /// <summary>
    /// A disassembler can be considered an enumerator of disassembled
    /// instructions. This convenience class lets implementors focus on the
    /// important method, <see cref="DisassembleInstruction" />.
    /// </summary>
    /// <remarks>
    /// All methods and properties that are specific to the instruction type 
    /// <typeparamref name="TInstr"/> should go here.
    /// </remarks>
    public abstract class DisassemblerBase<TInstr, TMnemonic> : DisassemblerBase, IEnumerable<TInstr>
        where TInstr : MachineInstruction
        where TMnemonic : struct
    {
        /// <inheritdoc/>
        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<TInstr> IEnumerable<TInstr>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Enumerator class for iterating through instructions.
        /// </summary>
        public struct Enumerator : IEnumerator<TInstr>
        {
            private readonly DisassemblerBase<TInstr, TMnemonic> dasm;
            private TInstr current;

            /// <summary>
            /// Constructs an instance of <see cref="Enumerator"/>.
            /// </summary>
            /// <param name="dasm">Disassembler to use when decoding an
            /// instruction.
            /// </param>
            public Enumerator(DisassemblerBase<TInstr, TMnemonic> dasm)
            {
                this.dasm = dasm;
                this.current = default!;
            }

            /// <inheritdoc/>
            public readonly TInstr Current => this.current;

            readonly object IEnumerator.Current => this.current;

            /// <inheritdoc/>
            public readonly void Dispose()
            {
            }

            /// <inheritdoc/>
            public bool MoveNext()
            {
                var instr = dasm.DisassembleInstruction();
                if (instr is null || instr.Length < 0)
                    return false;
                Debug.Assert(instr.Length > 0);
                this.current = instr;
                return true;
            }


            /// <inheritdoc/>
            /// <exception cref="NotSupportedException"></exception>
            public readonly void Reset()
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Disassemble a single instruction. 
        /// </summary>
        /// <remarks>
        /// Implementors should make an effort to make sure this method doesn't
        /// throw an exception. Instead, they should return an invalid instruction
        /// and possibly spew a diagnostic message.
        /// </remarks>
        /// <returns>Returns a disassembled machine instruction, or null
        /// if the end of the reader has been reached</returns>
        public abstract TInstr? DisassembleInstruction();

        /// <summary>
        /// This method is called by the <see cref="NyiDecoder{TDasm, TMnemonic, TInstr}"/> and
        /// is used in partially completed disassembler implementations. Subclasses will typically
        /// request the <see cref="Reko.Core.Services.ITestGenerationService"/> in order to generate
        /// the skeleton of a unit test for ease of implementation.
        /// </summary>
        /// <param name="message">A message describing why the instruction is not yet implemented.</param>
        /// <returns>
        /// Implementations should return an instruction whose 
        /// <see cref="MachineInstruction.InstructionClass"/> property 
        /// has the value <see cref="InstrClass.Invalid"/>.
        /// </returns>
        public abstract TInstr NotYetImplemented(string message);

        /// <summary>
        /// After the instruction has been fully decoded, this method is called to construct 
        /// an instance of a class derived from <see cref="MachineInstruction"/>.
        /// </summary>
        /// <param name="iclass">The <see cref="InstrClass"/> of the instruction.</param>
        /// <param name="mnemonic">The mnemonic of this instruction.</param>
        /// <returns></returns>
        public abstract TInstr MakeInstruction(InstrClass iclass, TMnemonic mnemonic);

        /// <summary>
        /// This method creates an invalid instruction. The instruction must 
        /// have its <see cref="MachineInstruction.InstructionClass" /> property set
        /// to <see cref="InstrClass.Invalid" />
        /// </summary>
        /// <remarks>
        /// The disassembler should generate invalid instructions when the 
        /// opcode it is decoding doesn't correspond to a valid instruction.
        /// Processors with variable-length instructions should return invalid
        /// instructions when the available machine code is shorter than the
        /// expected length of the instruction.
        /// </remarks>
        public abstract TInstr CreateInvalidInstruction();

        // Utility functions 

        /// <summary>
        /// Creates a <see cref="Decoder"/> which in turn generates a <see cref="MachineInstruction"/>
        /// with the given <paramref name="mnemonic"/> and with operands generated
        /// by the <paramref name="mutators"/>.
        /// </summary>
        /// <param name="mnemonic"></param>
        /// <param name="mutators"></param>
        /// <returns>An instance of <see cref="InstrDecoder{TDasm, TMnemonic, TInstr}"/>.</returns>
        public static Decoder<TDasm, TMnemonic, TInstr> Instr<TDasm>(TMnemonic mnemonic, params Mutator<TDasm>[] mutators)
            where TDasm : DisassemblerBase<TInstr, TMnemonic>
        {
            return new InstrDecoder<TDasm, TMnemonic, TInstr>(InstrClass.Linear, mnemonic, mutators);
        }

        /// <summary>
        /// Creates a <see cref="Decoder"/> which in turn generates a <see cref="MachineInstruction"/>
        /// with the given <paramref name="mnemonic"/>, with the given <see cref="InstrClass"/>
        /// <paramref name="iclass"/>, and with operands generatedby the <paramref name="mutators"/>.
        /// </summary>
        /// <param name="mnemonic">Mnemonic to give the generated <see cref="MachineInstruction"/>.</param>
        /// <param name="iclass">Instruction class to give the generated <see cref="MachineInstruction"/>.</param>
        /// <param name="mutators">Operand decoders that will fill in the instruction.</param>
        /// <returns>An instance of <see cref="InstrDecoder{TDasm, TMnemonic, TInstr}"/>.</returns>
        public static Decoder<TDasm, TMnemonic, TInstr> Instr<TDasm>(TMnemonic mnemonic, InstrClass iclass, params Mutator<TDasm>[] mutators)
            where TDasm : DisassemblerBase<TInstr, TMnemonic>
        {
            return new InstrDecoder<TDasm, TMnemonic, TInstr>(iclass, mnemonic, mutators);
        }

        /// <summary>
        /// Creates a <see cref="Decoder"/> which, when executed, extracts the
        /// bitfield defined by <paramref name="bitPos"/> and <paramref name="bitLength"/>
        /// and uses the value of the bitfield to select from one of the given
        /// <paramref name="decoders"/>.
        /// </summary>
        /// <param name="bitPos">Starting position of the bitfield, counted in 
        /// little-endian bit order.</param>
        /// <param name="bitLength">The size of the bitfield in bits.</param>
        /// <param name="decoders">The sub-decoders to which to dispatch.</param>
        /// <returns>An instance of <see cref="MaskDecoder{TDasm, TMnemonic, TInstr}"/>.</returns>
        public static MaskDecoder<TDasm, TMnemonic, TInstr> Mask<TDasm>(int bitPos, int bitLength, params Decoder<TDasm, TMnemonic, TInstr>[] decoders)
            where TDasm : DisassemblerBase<TInstr, TMnemonic>
        {
            return new MaskDecoder<TDasm, TMnemonic, TInstr>(new Bitfield(bitPos, bitLength), "", decoders);
        }

        /// <summary>
        /// Creates a <see cref="Decoder"/> which, when executed, extracts the
        /// bitfield defined by <paramref name="bitPos"/> and <paramref name="bitLength"/>
        /// and uses the value of the bitfield to select from one of the given
        /// <paramref name="decoders"/>.
        /// </summary>
        /// <param name="bitPos">Starting position of the bitfield, counted in 
        /// little-endian bit order.</param>
        /// <param name="bitLength">The size of the bitfield in bits.</param>
        /// <param name="tag">A tag for use when debugging. See
        /// <see cref="Decoder.DumpMaskedInstruction(int, ulong, ulong, string)"/>.
        /// </param>
        /// <param name="decoders">The sub-decoders to which to dispatch.</param>
        /// <returns>An instance of <see cref="MaskDecoder{TDasm, TMnemonic, TInstr}"/>.</returns>
        public static MaskDecoder<TDasm, TMnemonic, TInstr> Mask<TDasm>(int bitPos, int bitLength, string tag, params Decoder<TDasm, TMnemonic, TInstr>[] decoders)
            where TDasm : DisassemblerBase<TInstr, TMnemonic>
        {
            return new MaskDecoder<TDasm, TMnemonic, TInstr>(new Bitfield(bitPos, bitLength), tag, decoders);
        }

        /// <summary>
        /// Creates a wide <see cref="Decoder"/> which, when executed, extracts the
        /// bitfield defined by <paramref name="bitPos"/> and <paramref name="bitLength"/>
        /// and uses the value of the bitfield to select from one of the given
        /// <paramref name="decoders"/>.
        /// </summary>
        /// <param name="bitPos">Starting position of the bitfield, counted in 
        /// little-endian bit order.</param>
        /// <param name="bitLength">The size of the bitfield in bits.</param>
        /// <param name="tag">A tag for use when debugging. See
        /// <see cref="Decoder.DumpMaskedInstruction(int, ulong, ulong, string)"/>.
        /// </param>
        /// <param name="decoders">The sub-decoders to which to dispatch.</param>
        /// <returns>An instance of <see cref="MaskDecoder{TDasm, TMnemonic, TInstr}"/>.</returns>
        public static WideMaskDecoder<TDasm, TMnemonic, TInstr> WideMask<TDasm>(int bitPos, int bitLength, string tag, params WideDecoder<TDasm, TMnemonic, TInstr>[] decoders)
            where TDasm : DisassemblerBase<TInstr, TMnemonic>
        {
            return new WideMaskDecoder<TDasm, TMnemonic, TInstr>(new Bitfield(bitPos, bitLength), tag, decoders);
        }

        /// <summary>
        /// Creates a <see cref="Decoder"/> which, when executed, extracts two bitfields
        /// which when combined yields the index of a sub-decoder to dispatch to.
        /// </summary>
        /// <param name="p1">Bit position of first field.</param>
        /// <param name="l1">Length of first field.</param>
        /// <param name="p2">Bit position of second field.</param>
        /// <param name="l2">Length of second field.</param>
        /// <param name="tag">A tag for use when debugging.</param>
        /// <param name="decoders">The sub-decoders to which to dispatch.</param>
        /// <returns>An instance of <see cref="BitfieldDecoder{TDasm, TMnemonic, TInstr}"/>.
        /// </returns>
        public static BitfieldDecoder<TDasm, TMnemonic, TInstr> Mask<TDasm>(
            int p1, int l1, int p2, int l2,
            string tag,
            params Decoder<TDasm, TMnemonic, TInstr>[] decoders)
        {
            return new BitfieldDecoder<TDasm, TMnemonic, TInstr>(Bf((p1, l1), (p2, l2)), tag, decoders);
        }

        /// <summary>
        /// Creates a <see cref="Decoder"/> which, when executed, extracts two bitfields
        /// which when combined yields the index of a sub-decoder to dispatch to.
        /// </summary>
        /// <param name="p1">Bit position of first field.</param>
        /// <param name="l1">Length of first field.</param>
        /// <param name="p2">Bit position of second field.</param>
        /// <param name="l2">Length of second field.</param>
        /// <param name="tag">A tag for use when debugging.</param>
        /// <param name="defaultDecoder">Default decoder to use.</param>
        /// <param name="sparseDecoders">Sparse set of sub-decoders.</param>
        /// <returns>An instance of <see cref="BitfieldDecoder{TDasm, TMnemonic, TInstr}"/>.
        /// </returns>
        public static BitfieldDecoder<TDasm, TMnemonic, TInstr> Mask<TDasm>(
            int p1, int l1, int p2, int l2, 
            string tag,
            Decoder<TDasm, TMnemonic, TInstr> defaultDecoder,
            params (uint, Decoder<TDasm, TMnemonic, TInstr>)[] sparseDecoders)
        {
            int totalBits = l1 + l2;
            var fields = Bf((p1, l1), (p2, l2));
            var decoders = Decoder<TDasm, TMnemonic, TInstr>.BuildSparseDecoderArray(
                totalBits,
                defaultDecoder,
                sparseDecoders);
            return new BitfieldDecoder<TDasm, TMnemonic, TInstr>(fields, tag, decoders);
        }

        /// <summary>
        /// Creates a <see cref="Decoder"/> which, when executed, extracts values from
        /// several bitfields, which when combined yields the index of a sub-decoder
        /// to dispatch to.
        /// </summary>
        /// <param name="bitfields">Bit fields used to extract values.</param>
        /// <param name="tag">A tag for use when debugging.</param>
        /// <param name="decoders">Sub-decoders.</param>
        /// <returns>An instance of <see cref="BitfieldDecoder{TDasm, TMnemonic, TInstr}"/>.
        /// </returns>
        public static BitfieldDecoder<TDasm, TMnemonic, TInstr> Mask<TDasm>(
            Bitfield[] bitfields,
            string tag,
            params Decoder<TDasm, TMnemonic, TInstr>[] decoders)
        {
            return new BitfieldDecoder<TDasm, TMnemonic, TInstr>(bitfields, tag, decoders);
        }

        /// <summary>
        /// Creates a <see cref="Decoder"/> which, when executed, extracts values from
        /// several bitfields, which when combined yields the index of a sub-decoder
        /// to dispatch to.
        /// </summary>
        /// <param name="bitfields">Bit fields used to extract values.</param>
        /// <param name="decoders">Sub-decoders.</param>
        /// <returns>An instance of <see cref="BitfieldDecoder{TDasm, TMnemonic, TInstr}"/>.
        /// </returns>
        public static BitfieldDecoder<TDasm, TMnemonic, TInstr> Mask<TDasm>(
            Bitfield[] bitfields,
            params Decoder<TDasm, TMnemonic, TInstr>[] decoders)
        {
            return new BitfieldDecoder<TDasm, TMnemonic, TInstr>(bitfields, "", decoders);
        }

        /// <summary>
        /// Creates a conditional decoder which selects a sub-decoder depending on a predicate.
        /// </summary>
        /// <typeparam name="TDasm"></typeparam>
        /// <param name="predicate">Predicate accepting the instruction opcode.</param>
        /// <param name="decoderTrue">Sub-decoder to invoke when the predicate evaluates to true.</param>
        /// <param name="decoderFalse">Sub-decoder to invoke when the predicate evaluates to false.</param>
        /// <returns>A conditional decoder.</returns>
        public static ConditionalDecoder<TDasm, TMnemonic, TInstr> Select<TDasm>(
            Predicate<uint> predicate,
            Decoder<TDasm, TMnemonic, TInstr> decoderTrue,
            Decoder<TDasm, TMnemonic, TInstr> decoderFalse)
        {
            var fields = new[]
            {
                new Bitfield(0, 32, 0xFFFFFFFF)
            };
            return new ConditionalDecoder<TDasm, TMnemonic, TInstr>(fields, predicate, "", decoderTrue, decoderFalse);
        }

        /// <summary>
        /// Creates a <see cref="IfDecoder{TDasm, TMnemonic, TInstr}"/> which invokes a sub-decoder if the predicate is true.
        /// </summary>
        /// <typeparam name="TDasm"></typeparam>
        /// <param name="predicate">Predicate accepting the instruction opcode.</param>
        /// <param name="decoderTrue">Sub-decoder to invoke when the predicate evaluates to true.</param>
        /// <returns>An <see cref="IfDecoder{TDasm, TMnemonic, TInstr}"/>.</returns>
        public static IfDecoder<TDasm, TMnemonic, TInstr> If<TDasm>(
            Predicate<uint> predicate,
            Decoder<TDasm, TMnemonic, TInstr> decoderTrue)
            where TDasm : DisassemblerBase<TInstr, TMnemonic>
        {
            var bf = new Bitfield(0, 32, 0xFFFFFFFF);
            return new IfDecoder<TDasm, TMnemonic, TInstr>(bf, predicate, decoderTrue);
        }

        /// <summary>
        /// Creates a if decoder which invokes a sub-decoder if the predicate is true.
        /// </summary>
        /// <typeparam name="TDasm"></typeparam>
        /// <param name="bitpos">Bit position of the bitfield to extract.</param>
        /// <param name="bitlen">Bit size of the bitfield to extract.</param>
        /// <param name="predicate">Predicate accepting the extracted value.</param>
        /// <param name="decoderTrue">Sub-decoder to invoke when the predicate evaluates to true.</param>
        /// <returns>A if decoder.</returns>
        public static IfDecoder<TDasm, TMnemonic, TInstr> If<TDasm>(
            int bitpos, int bitlen,
            Predicate<uint> predicate,
            Decoder<TDasm, TMnemonic, TInstr> decoderTrue)
            where TDasm : DisassemblerBase<TInstr, TMnemonic>
        {
            var bf = new Bitfield(bitpos, bitlen);
            return new IfDecoder<TDasm, TMnemonic, TInstr>(bf, predicate, decoderTrue);
        }

        /// <summary>
        /// Creates a conditional decoder which selects a sub-decoder depending on a predicate.
        /// </summary>
        /// <typeparam name="TDasm"></typeparam>
        /// <param name="fieldSpecifier">Specifies the bit position and size.</param>
        /// <param name="predicate">Predicate accepting the instruction opcode.</param>
        /// <param name="tag">Tag used for debugging.</param>
        /// <param name="decoderTrue">Sub-decoder to invoke when the predicate evaluates to true.</param>
        /// <param name="decoderFalse">Sub-decoder to invoke when the predicate evaluates to false.</param>
        /// <returns>A conditional decoder.</returns>
        public static ConditionalDecoder<TDasm, TMnemonic, TInstr> Select<TDasm>(
            (int, int) fieldSpecifier,
            Predicate<uint> predicate,
            string tag,
            Decoder<TDasm, TMnemonic, TInstr> decoderTrue,
            Decoder<TDasm, TMnemonic, TInstr> decoderFalse)
        {
            var fields = new[]
            {
                new Bitfield(fieldSpecifier.Item1, fieldSpecifier.Item2)
            };
            return new ConditionalDecoder<TDasm, TMnemonic, TInstr>(fields, predicate, tag, decoderTrue, decoderFalse);
        }

        /// <summary>
        /// Creates a conditional decoder which selects a sub-decoder depending on a predicate.
        /// </summary>
        /// <typeparam name="TDasm"></typeparam>
        /// <param name="fieldSpecifier">Specifies the bit position and size.</param>
        /// <param name="predicate">Predicate accepting the instruction opcode.</param>
        /// <param name="tag">Tag used for debugging.</param>
        /// <param name="decoderTrue">Sub-decoder to invoke when the predicate evaluates to true.</param>
        /// <param name="decoderFalse">Sub-decoder to invoke when the predicate evaluates to false.</param>
        /// <returns>A conditional decoder.</returns>
        public static WideConditionalDecoder<TDasm, TMnemonic, TInstr> WideSelect<TDasm>(
            (int, int) fieldSpecifier,
            Predicate<ulong> predicate,
            string tag,
            WideDecoder<TDasm, TMnemonic, TInstr> decoderTrue,
            WideDecoder<TDasm, TMnemonic, TInstr> decoderFalse)
        {
            var fields = new[]
            {
                new Bitfield(fieldSpecifier.Item1, fieldSpecifier.Item2)
            };
            return new WideConditionalDecoder<TDasm, TMnemonic, TInstr>(fields, predicate, tag, decoderTrue, decoderFalse);
        }

        /// <summary>
        /// Creates a conditional decoder which selects a sub-decoder depending on a predicate.
        /// </summary>
        /// <typeparam name="TDasm"></typeparam>
        /// <param name="fieldSpecifier">Specifies the bit position and size.</param>
        /// <param name="predicate">Predicate accepting the instruction opcode.</param>
        /// <param name="decoderTrue">Sub-decoder to invoke when the predicate evaluates to true.</param>
        /// <param name="decoderFalse">Sub-decoder to invoke when the predicate evaluates to false.</param>
        /// <returns>A conditional decoder.</returns>
        public static ConditionalDecoder<TDasm, TMnemonic, TInstr> Select<TDasm>(
            (int, int) fieldSpecifier,
            Predicate<uint> predicate,
            Decoder<TDasm, TMnemonic, TInstr> decoderTrue,
            Decoder<TDasm, TMnemonic, TInstr> decoderFalse)
        {
            var fields = new[]
            {
                new Bitfield(fieldSpecifier.Item1, fieldSpecifier.Item2)
            };
            return new ConditionalDecoder<TDasm, TMnemonic, TInstr>(fields, predicate, "", decoderTrue, decoderFalse);
        }

        /// <summary>
        /// Creates a conditional decoder which selects a sub-decoder depending on a predicate.
        /// </summary>
        /// <param name="fields">Specifies fields to read a value to pass to the predicate.</param>
        /// <param name="predicate">Predicate accepting the instruction opcode.</param>
        /// <param name="decoderTrue">Sub-decoder to invoke when the predicate evaluates to true.</param>
        /// <param name="decoderFalse">Sub-decoder to invoke when the predicate evaluates to false.</param>
        /// <returns>A conditional decoder.</returns>
        public static ConditionalDecoder<TDasm, TMnemonic, TInstr> Select<TDasm>(
            Bitfield[] fields,
            Predicate<uint> predicate,
            Decoder<TDasm, TMnemonic, TInstr> decoderTrue,
            Decoder<TDasm, TMnemonic, TInstr> decoderFalse)
        {
            return new ConditionalDecoder<TDasm, TMnemonic, TInstr>(fields, predicate, "", decoderTrue, decoderFalse);
        }

        /// <summary>
        /// Creates a conditional decoder which selects a sub-decoder depending on a predicate.
        /// </summary>
        /// <param name="field">Specifies a field to read a value to pass to the predicate.</param>
        /// <param name="predicate">Predicate accepting the instruction opcode.</param>
        /// <param name="decoderTrue">Sub-decoder to invoke when the predicate evaluates to true.</param>
        /// <param name="decoderFalse">Sub-decoder to invoke when the predicate evaluates to false.</param>
        /// <returns>A conditional decoder.</returns>
        public static ConditionalDecoder<TDasm, TMnemonic, TInstr> Select<TDasm>(
             Bitfield field,
             Predicate<uint> predicate,
             Decoder<TDasm, TMnemonic, TInstr> decoderTrue,
             Decoder<TDasm, TMnemonic, TInstr> decoderFalse)
        {
            return new ConditionalDecoder<TDasm, TMnemonic, TInstr>([field], predicate, "", decoderTrue, decoderFalse);
        }

        /// <summary>
        /// Creates a conditional decoder which selects a sub-decoder depending on a predicate.
        /// </summary>
        /// <param name="field">Specifies a field to read a value to pass to the predicate.</param>
        /// <param name="predicate">Predicate accepting the instruction opcode.</param>
        /// <param name="tag">Tag used for debugging.</param>
        /// <param name="decoderTrue">Sub-decoder to invoke when the predicate evaluates to true.</param>
        /// <param name="decoderFalse">Sub-decoder to invoke when the predicate evaluates to false.</param>
        /// <returns>A conditional decoder.</returns>
        public static ConditionalDecoder<TDasm, TMnemonic, TInstr> Select<TDasm>(
             Bitfield field,
             Predicate<uint> predicate,
             string tag,
             Decoder<TDasm, TMnemonic, TInstr> decoderTrue,
             Decoder<TDasm, TMnemonic, TInstr> decoderFalse)
        {
            return new ConditionalDecoder<TDasm, TMnemonic, TInstr>([field], predicate, tag, decoderTrue, decoderFalse);
        }

        /// <summary>
        /// Creates a conditional decoder which selects a sub-decoder depending on a predicate.
        /// </summary>
        /// <param name="fields">Specifies fields to read a value to pass to the predicate.</param>
        /// <param name="predicate">Predicate accepting the instruction opcode.</param>
        /// <param name="tag">Tag used for debugging.</param>
        /// <param name="decoderTrue">Sub-decoder to invoke when the predicate evaluates to true.</param>
        /// <param name="decoderFalse">Sub-decoder to invoke when the predicate evaluates to false.</param>
        /// <returns>A conditional decoder.</returns>
        public static ConditionalDecoder<TDasm, TMnemonic, TInstr> Select<TDasm>(
             Bitfield[] fields,
             Predicate<uint> predicate,
             string tag,
             Decoder<TDasm, TMnemonic, TInstr> decoderTrue,
             Decoder<TDasm, TMnemonic, TInstr> decoderFalse)
        {
            return new ConditionalDecoder<TDasm, TMnemonic, TInstr>(fields, predicate, tag, decoderTrue, decoderFalse);
        }

        /// <summary>
        /// Creates a sparsely populated <see cref="MaskDecoder{TDasm, TMnemonic, TInstr}"/> where 
        /// most of the decoders default to <paramref name="defaultDecoder"/>.
        /// </summary>
        public static MaskDecoder<TDasm, TMnemonic, TInstr> Sparse<TDasm>(
            int bitPosition, int bits, string tag,
            Decoder<TDasm, TMnemonic, TInstr> defaultDecoder,
            params (uint, Decoder<TDasm, TMnemonic, TInstr>)[] sparseDecoders)
            where TDasm : DisassemblerBase<TInstr, TMnemonic>
        {
            var decoders = Decoder<TDasm, TMnemonic, TInstr>.BuildSparseDecoderArray(bits, defaultDecoder, sparseDecoders);
            return new MaskDecoder<TDasm, TMnemonic, TInstr>(new Bitfield(bitPosition, bits), tag, decoders);
        }

        /// <summary>
        /// Creates a sparsely populated <see cref="WideMaskDecoder{TDasm, TMnemonic, TInstr}"/> where 
        /// most of the wide decoders default to <paramref name="defaultDecoder"/>.
        /// </summary>
        protected static WideMaskDecoder<TDasm, TMnemonic, TInstr> WideSparse<TDasm>(
            int bitPosition, int bits, string tag,
            WideDecoder<TDasm, TMnemonic, TInstr> defaultDecoder,
            params (uint, WideDecoder<TDasm, TMnemonic, TInstr>)[] sparseDecoders)
            where TDasm : DisassemblerBase<TInstr, TMnemonic>
        {
            var decoders = new WideDecoder<TDasm, TMnemonic, TInstr>[1 << bits];
            foreach (var (code, decoder) in sparseDecoders)
            {
                Debug.Assert(0 <= code && code < decoders.Length);
                Debug.Assert(decoders[code] is null, $"Decoder {code:X} has already a value!");
                decoders[code] = decoder;
            }
            for (int i = 0; i < decoders.Length; ++i)
            {
                if (decoders[i] is null)
                    decoders[i] = defaultDecoder;
            }
            return new WideMaskDecoder<TDasm, TMnemonic, TInstr>(new Bitfield(bitPosition, bits), tag, decoders);
        }

        /// <summary>
        /// Creates a sparsely populated <see cref="MaskDecoder{TDasm, TMnemonic, TInstr}"/> where 
        /// most of the decoders default to <paramref name="defaultDecoder"/>.
        /// </summary>
        public static MaskDecoder<TDasm, TMnemonic, TInstr> Sparse<TDasm>(
            int bitPosition, int bits,
            Decoder<TDasm, TMnemonic, TInstr> defaultDecoder,
            params (uint, Decoder<TDasm, TMnemonic, TInstr>)[] sparseDecoders)
            where TDasm : DisassemblerBase<TInstr, TMnemonic>
        {
            return Sparse(bitPosition, bits, "", defaultDecoder, sparseDecoders);
        }
    }

    /// <summary>
    /// Base class that implements <see cref="IDisposable"/> interface.
    /// </summary>
    public class DisassemblerBase : IDisposable
    {
        /// <summary>
        /// Disposes the disassembler.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer for the disassembler.
        /// </summary>
        ~DisassemblerBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Creates a monolithic <see cref="Bitfield"/>.
        /// </summary>
        /// <param name="pos">
        /// A bit positions in little endian order, i.e. the least 
        /// significant bit is numbered with 0.</param>
        /// <param name="len">
        /// The width of the bitfield.
        /// </param>
        /// <returns>
        /// A <see cref="Bitfield">bitfield</see>.
        /// </returns>
        public static Bitfield Bf(int pos, int len)
        {
            return new Bitfield(pos, len);
        }

        /// <summary>
        /// Creates an array of <see cref="Bitfield"/>s.
        /// </summary>
        /// <param name="fields">
        /// An array of (position, length) tuples, indicating the bitfields.
        /// The bit positions are in little endian order, i.e. the least 
        /// significant bits are numbered with 0.</param>
        /// <returns>
        /// An array of <see cref="Bitfield">bitfields</see>.
        /// </returns>
        public static Bitfield[] Bf(params (int pos, int len)[] fields)
        {
            return fields.Select(f => new Bitfield(f.pos, f.len)).ToArray();
        }

        /// <summary>
        /// Reads the fields from the instruction and returns the result.
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="instr"></param>
        /// <returns></returns>
        protected static uint ReadFields(Bitfield[] fields, uint instr)
        {
            Decoder.DumpMaskedInstruction(32, instr, fields, "");
            Bitfield.ReadFields(fields, instr);
            uint result = 0;
            foreach (var field in fields)
            {
                result <<= field.Length;
                result |= field.Read(instr);
            }
            return result;
        }

        /// <summary>
        /// Handles the disposal of the disassembler.
        /// </summary>
        /// <param name="disposing">True if being disposed; false 
        /// if being finalized.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}