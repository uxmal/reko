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

namespace Reko.Arch.Cray
{
    public enum Mnemonic
    {
        Invalid,
        err,
        _and,
        _fmul,
        _mov,
        j,
        _iadd,
        _isub,
        _neg,
        r,
        jts,
        _imul,
        _movlo,
        _movhi,
        _lmask,
        _rmask,
        _andnot,
        _xor,
        _eqv,
        _and_or,
        _or,
        _lsl,
        _lsr,
        _store,
        _load,
        _movst,
        pass,
        jaz,
        jan,
        jap,
        jam,
        jsz,
        jsn,
        jsp,
        jsm,
        _dlsl,
        _dlsr,
        _movz,
        _movs,
    }
}