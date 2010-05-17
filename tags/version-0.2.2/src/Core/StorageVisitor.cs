/* 
 * Copyright (C) 1999-2010 John Källén.
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

using System;

namespace Decompiler.Core
{
	public interface StorageVisitor
	{
		void VisitFlagGroupStorage(FlagGroupStorage grf);
		void VisitFpuStackStorage(FpuStackStorage fpu);
		void VisitMemoryStorage(MemoryStorage global);
		void VisitStackLocalStorage(StackLocalStorage local);
		void VisitOutArgumentStorage(OutArgumentStorage arg);
		void VisitRegisterStorage(RegisterStorage reg);
		void VisitSequenceStorage(SequenceStorage seq);
		void VisitStackArgumentStorage(StackArgumentStorage stack);
		void VisitTemporaryStorage(TemporaryStorage temp);
	}
}
