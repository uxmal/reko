'''
 * Copyright (C) 1999-2021 Pavel Tomin.
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
'''

def addr_from_str(s_addr):
    # $TODO: Make it work with segmented addresses.
    return int(s_addr, 16)

def addr_list_from_str_list(s_addr_list):
    return (addr_from_str(s_addr) for s_addr in s_addr_list)

class RekoDictBase(object):
    def __getitem__(self, key):
        raise KeyError

    def get(self, key, default=None):
        'D.get(k[,d]) -> D[k] if k in D, else d.  d defaults to None.'
        try:
            return self[key]
        except KeyError:
            return default

    def __contains__(self, key):
        try:
            self[key]
        except KeyError:
            return False
        else:
            return True

    def items(self):
        for key in self:
            yield (key, self[key])

class Comments(object):
    def __init__(self, reko):
        self._reko = reko

    def __setitem__(self, addr, comment):
        if not isinstance(comment, str):
            raise TypeError(
                'Unsupported type: {}. Expected type: str'.format(
                    type(comment)))
        self._reko.SetUserComment(addr, comment)

class MemorySlice(object):
    def __init__(self, reko, start_addr, end_addr):
        self._reko = reko
        self._start_addr = start_addr
        self._end_addr = end_addr

    @property
    def byte(self):
        if self._end_addr is None:
            return self._reko.ReadByte(self._start_addr)
        return self._reko.ReadBytes(self._start_addr, self._end_addr)

    @property
    def int16(self):
        if self._end_addr is None:
            return self._reko.ReadInt16(self._start_addr)
        return self._reko.ReadInts16(self._start_addr, self._end_addr)

    @property
    def int32(self):
        if self._end_addr is None:
            return self._reko.ReadInt32(self._start_addr)
        return self._reko.ReadInts32(self._start_addr, self._end_addr)

    @property
    def int64(self):
        if self._end_addr is None:
            return self._reko.ReadInt64(self._start_addr)
        return self._reko.ReadInts64(self._start_addr, self._end_addr)

    @property
    def c_str(self):
        return self._reko.ReadCString(self._start_addr)

class Memory(object):
    def __init__(self, reko):
        self._reko = reko

    def __getitem__(self, key):
        if isinstance(key, slice):
            if key.step is not None:
                raise ValueError('Step is not supported')
            if key.start is None:
                raise ValueError('Start address is required')
            if key.stop is None:
                raise ValueError('End address is required')
            start_addr = key.start
            end_addr = key.stop
        else:
            start_addr = key
            end_addr = None
        return MemorySlice(self._reko, start_addr, end_addr)

class Globals(object):
    def __init__(self, reko):
        self._reko = reko

    def __setitem__(self, addr, decl):
        if isinstance(decl, str):
            self._reko.SetUserGlobal(addr, decl)
            return
        raise TypeError('Unsupported type: {}'.format(type(decl)))

class Procedure(object):
    def __init__(self, reko, addr):
        self._reko = reko
        self._addr = addr

    @property
    def decompile(self):
        return self._reko.GetProcedureDecompileFlag(self._addr)

    @decompile.setter
    def decompile(self, value):
        self._reko.SetUserProcedureDecompileFlag(self._addr, value)

    @property
    def name(self):
        return self._reko.GetProcedureName(self._addr)

class Procedures(RekoDictBase):
    def __init__(self, reko):
        self._reko = reko

    def __getitem__(self, addr):
        if not self._reko.ContainsProcedureAddress(addr):
            raise KeyError(addr)
        return Procedure(self._reko, addr)

    def __setitem__(self, addr, decl):
        if isinstance(decl, str):
            self._reko.SetUserProcedure(addr, decl)
            return
        raise TypeError('Unsupported type: {}'.format(type(decl)))

    def __iter__(self):
        return addr_list_from_str_list(self._reko.GetProcedureAddresses())

class Program(object):
    def __init__(self, reko):
        self._comments = Comments(reko)
        self._memory = Memory(reko)
        self._globals = Globals(reko)
        self._procedures = Procedures(reko)

    @property
    def comments(self):
        return self._comments

    @property
    def memory(self):
        return self._memory

    @property
    def globals(self):
        return self._globals

    @property
    def procedures(self):
        return self._procedures
