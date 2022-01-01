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

namespace Reko.Arch.Cray
{
    public enum Mnemonic
    {
        Invalid,

        _and,
        _and_or,
        _andnot,
        _clz,
        _dlsl,
        _dlsr,
        _eqv,
        _fmul,
        _iadd,
        _imul,
        _isub,
        _lmask,
        _load,
        _load_inc,
        _lsl,
        _lsr,
        _mov,
        _movhi,
        _movlo,
        _movs,
        _movst,
        _movz,
        _neg,
        _popcnt,
        _rmask,
        _store,
        _store_inc,
        _viadd,
        _vmov,
        _vor,
        _xor,
        err,
        ex,
        j,
        jam,
        jan,
        jap,
        jaz,
        jsm,
        jsn,
        jsp,
        jsz,
        jts,
        pass,
        r,
    }
}