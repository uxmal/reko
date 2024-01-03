#region License
/* 
 * Copyright (C) 2018-2024 Stefano Moioli <smxdev4@gmail.com>.
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.ImageLoaders.Pef
{
    enum PefOpcode : byte {
        Zero = 0,
        BlockCopy = 1,
        RepeatedBlock = 2,
        InterleaveRepeatBlockWithBlockCopy = 3,
        InterleaveRepeatBlockWithZero = 4
    }


    public class PefOpcodeInterpreter
    {
        private int programCounter;

        private Memory<byte> codeStream;
        public Stream Output { get; private set; }

        public PefOpcodeInterpreter(Memory<byte> codeStream, Stream dataOut)
        {
            this.codeStream = codeStream;
            this.Output = dataOut;
        }

        byte Fetch()
        {
            return codeStream.Span[programCounter++];
        }

        uint FetchVarargs()
        {
            uint value = 0;
            bool hasMore;
            do
            {
                byte data = Fetch();
                byte fragment = DecodeArg(data, out hasMore);
                value = (value << 7) | fragment;
            } while (hasMore);

            return value;
        }

        uint ExtractOrFetchCount(byte data)
        {
            var count = ExtractCount(data);
            if(count == 0)
            {
                count = FetchVarargs();
            }
            return count;
        }

        PefOpcode ExtractOp(byte data) => (PefOpcode) ((data & 0xE0) >> 5);
        uint ExtractCount(byte count) => (byte)(count & 0x1F);
        
        byte DecodeArg(byte arg, out bool hasMore)
        {
            hasMore = (arg & 0x80) == 0x80;
            return (byte)(arg & 0x7F);
        }

        private void HandleZeroOp(byte data)
        {
            var count = ExtractOrFetchCount(data);

            // .NET arrays are 0-initialized
            var buffer = new byte[count];
            Output.Write(buffer, 0, buffer.Length);
        }

        private void HandleBlockCopy(byte data)
        {
            var count = ExtractOrFetchCount(data);

            var buffer = new byte[count];
            for(var i=0; i<count; i++)
            {
                buffer[i] = Fetch();
            }
            Output.Write(buffer, 0, buffer.Length);
        }

        private void HandleRepeatedBlock(byte data)
        {
            var blockSize = ExtractOrFetchCount(data);
            var repeatCount = FetchVarargs() + 1;

            var buffer = new byte[blockSize];
            for(var i=0; i<blockSize; i++)
            {
                buffer[i] = Fetch();
            }

            for(var i=0; i<repeatCount; i++)
            {
                Output.Write(buffer, 0, buffer.Length);
            }
        }

        private void InterleavedCommon(byte[] commonData, uint customSize, uint repeatCount)
        {
            var customData = new byte[customSize];

            for (var i = 0; i < repeatCount; i++)
            {
                for (var j = 0; j < customSize; j++)
                {
                    customData[j] = Fetch();
                }

                Output.Write(commonData, 0, commonData.Length);
                Output.Write(customData, 0, customData.Length);
            }

            Output.Write(commonData, 0, commonData.Length);
        }

        private void HandleInterleavedBlockCopy(byte data)
        {
            var commonSize = ExtractOrFetchCount(data);
            var customSize = FetchVarargs();
            var repeatCount = FetchVarargs();

            var commonData = new byte[commonSize];
            for (var i = 0; i < commonSize; i++)
            {
                commonData[i] = Fetch();
            }
            InterleavedCommon(commonData, customSize, repeatCount);
        }

        private void HandleInterleavedZero(byte data)
        {
            var commonSize = ExtractOrFetchCount(data);
            var customSize = FetchVarargs();
            var repeatCount = FetchVarargs();

            // zero buffer
            var commonData = new byte[commonSize];
            InterleavedCommon(commonData, customSize, repeatCount);
        }

        public void RunProgram()
        {
            programCounter = 0;

            while(programCounter < codeStream.Length)
            {
                byte data = Fetch();
                
                var opcode = ExtractOp(data);
                switch (opcode)
                {
                case PefOpcode.Zero:
                    HandleZeroOp(data); break;
                case PefOpcode.BlockCopy:
                    HandleBlockCopy(data); break;
                case PefOpcode.RepeatedBlock:
                    HandleRepeatedBlock(data); break;
                case PefOpcode.InterleaveRepeatBlockWithBlockCopy:
                    HandleInterleavedBlockCopy(data); break;
                case PefOpcode.InterleaveRepeatBlockWithZero:
                    HandleInterleavedZero(data); break;
                default:
                    throw new BadImageFormatException($"Unrecognized PEF Opcode {opcode:X}");
                }
            }
        }
    }
}
