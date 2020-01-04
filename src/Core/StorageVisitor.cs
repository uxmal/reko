#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

namespace Reko.Core
{
    /// <summary>
    /// Visitor pattern for storages, parametrized by the returned value of
    /// all the visitor methods.
    /// </summary>
    /// <typeparam name="T">Returned value</typeparam>
	public interface StorageVisitor<T>
	{
		T VisitFlagGroupStorage(FlagGroupStorage grf);
		T VisitFpuStackStorage(FpuStackStorage fpu);
		T VisitMemoryStorage(MemoryStorage global);
		T VisitStackLocalStorage(StackLocalStorage local);
		T VisitOutArgumentStorage(OutArgumentStorage arg);
		T VisitRegisterStorage(RegisterStorage reg);
		T VisitSequenceStorage(SequenceStorage seq);
		T VisitStackArgumentStorage(StackArgumentStorage stack);
		T VisitTemporaryStorage(TemporaryStorage temp);
    }

    public interface StorageVisitor<C, T>
    {
        T VisitFlagGroupStorage(FlagGroupStorage grf, C context);
        T VisitFpuStackStorage(FpuStackStorage fpu, C context);
        T VisitMemoryStorage(MemoryStorage global, C context);
        T VisitStackLocalStorage(StackLocalStorage local, C context);
        T VisitOutArgumentStorage(OutArgumentStorage arg, C context);
        T VisitRegisterStorage(RegisterStorage reg, C context);
        T VisitSequenceStorage(SequenceStorage seq, C context);
        T VisitStackArgumentStorage(StackArgumentStorage stack, C context);
        T VisitTemporaryStorage(TemporaryStorage temp, C context);
    }
}
