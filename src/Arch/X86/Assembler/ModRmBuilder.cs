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

using Reko.Core;
using Reko.Core.Assemblers;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Diagnostics;

namespace Reko.Arch.X86.Assembler
{
	/// <summary>
	/// Accumulates information to generate a ModRm byte(and possibly SIB byte) 
	/// </summary>
	public class ModRmBuilder
	{
		public event ErrorEventHandler? Error;

		private readonly IEmitter emitter;
		private readonly PrimitiveType defaultWordSize;
		private Constant? offset;

		public ModRmBuilder(PrimitiveType defaultWordSize, IEmitter emitter)
		{
			this.emitter = emitter;
			this.defaultWordSize = defaultWordSize;
		}

		private Constant EmitDirectAddress(int reg, MemoryOperand memOp)
		{
			Debug.Assert(memOp.Offset is not null && memOp.Offset.IsValid);
			if (defaultWordSize == PrimitiveType.Word16)
			{
				reg |= 0x6;
				emitter.EmitByte(reg);
				return Constant.Create(PrimitiveType.Word16, memOp.Offset!.ToUInt32());
			}
			else
			{
				reg |= 0x5;
				emitter.EmitByte(reg);
				return Constant.Word32(memOp.Offset!.ToUInt32());
			}
		}


		public void EmitModRM(int reg, FpuOperand op)
		{
			reg <<= 3;
			emitter.EmitByte(0xC0 | reg | op.StNumber);
		}


		public void EmitModRM(int reg, RegisterStorage op)
		{
			reg <<= 3;
			emitter.EmitByte(0xC0 | reg | X86Assembler.RegisterEncoding(op));
		}

		/// <summary>
		/// Emits the ModRM byte (and SIB byte if applicable)
		/// </summary>
		/// <param name="reg"></param>
		/// <param name="memOp"></param>
		/// <returns>The offset value to be emitted as the last piece of the instruction</returns>
		public Constant? EmitModRMPrefix(int reg, MemoryOperand memOp)
		{
			offset = null;
			reg <<= 3;
			if (memOp.Base != RegisterStorage.None || memOp.Index != RegisterStorage.None)
			{
				var baseWidth = memOp.Base.DataType;
				var indexWidth = memOp.Index.DataType;
				if (memOp.Base != RegisterStorage.None && memOp.Index != RegisterStorage.None)
				{
					if (baseWidth != indexWidth)
					{
						OnError("mismatched base and index registers");
						return null;
					}
				}

				// Add the 'mod' bits

				if (memOp.Offset is not null)
				{
					Debug.Assert(memOp.Offset.IsValid);
					if (memOp.Offset.DataType == PrimitiveType.SByte)
					{
						reg |= 0x40;
						offset = memOp.Offset;
					}
					else
					{
						reg |= 0x80;
						offset = Constant.Create(defaultWordSize, memOp.Offset.ToInt32());
					}
				}

				bool fNeedsSib = false;
				int sib = 0;
				if (baseWidth == PrimitiveType.Word16)
				{
					reg |= Get16AddressingModeMask(memOp);
				}
				else if (baseWidth == PrimitiveType.Word32 || indexWidth == PrimitiveType.Word32)
				{
					if (memOp.Index == RegisterStorage.None)
					{
						if (memOp.Base != Registers.esp)
						{
							reg |= X86Assembler.RegisterEncoding(memOp.Base);
							if (memOp.Offset is null && memOp.Base == Registers.ebp)
							{
								reg |= 0x40;
								offset = Constant.Byte(0);
							}
						}
						else
						{
							reg |= 0x04;
							fNeedsSib = true;
							sib = 0x24;
						}
					}
					else
					{
						reg |= 0x04;
						fNeedsSib = true;
						switch (memOp.Scale)
						{
						case 1: sib = 0; break;
						case 2: sib = 0x40; break;
						case 4: sib = 0x80; break;
						case 8: sib = 0xC0; break;
						default: OnError("Scale factor must be 1, 2, 4, or 8"); return InvalidConstant.Create(this.defaultWordSize);
						}

						if (memOp.Base == RegisterStorage.None)
						{
							sib |= 0x05;
							reg &= ~0xC0;			// clear mod part of modRM.

							if (memOp.Offset is null)
							{
								offset = Constant.Word32(0);
							}
						}
						else
						{
							sib |= X86Assembler.RegisterEncoding(memOp.Base);
						}

						if (memOp.Index != Registers.esp)
						{
							sib |= X86Assembler.RegisterEncoding(memOp.Index) << 3;
						}
						else
							throw new ApplicationException("ESP register can't be used as an index register");
					}
				}
				else
				{
					throw new ApplicationException("unexpected address width");
				}

				emitter.EmitByte(reg);
				if (fNeedsSib)
					emitter.EmitByte(sib);
				return offset;

			}
			else
				return EmitDirectAddress(reg, memOp);
		}

		public int Get16AddressingModeMask(MemoryOperand memOp)
		{
			int mask = (1 << memOp.Base.Number);
			if (memOp.Index != RegisterStorage.None)
			{
				mask |= (1 << memOp.Index.Number);
			}
			if (mask == ((1<<Registers.bx.Number)|(1<<Registers.si.Number)))
				return 0;
			if (mask == ((1<<Registers.bx.Number)|(1<<Registers.di.Number)))
				return 1;
			if (mask == ((1<<Registers.bp.Number)|(1<<Registers.si.Number)))
				return 2;
			if (mask == ((1<<Registers.bp.Number)|(1<<Registers.di.Number)))
				return 3;
			if (mask == 1<<Registers.si.Number)
				return 4;
			if (mask == 1<<Registers.di.Number)
				return 5;
			if (mask == 1<<Registers.bp.Number)
			{
				mask = 6;
				if (memOp.Offset is null || !memOp.Offset.IsValid)
				{
					mask |= 0x40;
					offset = Constant.Byte(0);
				}
				return mask;
			}
			if (mask == 1<<(int) Registers.bx.Number)
				return 7;
			OnError(string.Format("Illegal 16-bit addressing mode: {0} ", memOp.ToString()));
			return 0;
		}

		private void OnError(string message)
		{
            Error?.Invoke(this, new ErrorEventArgs(message));
        }
	}
}
