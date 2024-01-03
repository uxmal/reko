# Copyright (C) 1999-2024 Pavel Tomin.
#
# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation; either version 2, or (at your option)
# any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; see the file COPYING.  If not, write to
# the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.

'''This module contains classes which provides decompiling program
functionality.
WARNING: This is a part of internal API. Do not import it directly to your
scripts.
'''

def reko_address(reko, addr):
    if isinstance(addr, tuple):
        addr = ":".join(map('0x{:X}'.format, addr))
    return reko.Address(addr)

def addr_from_str(s_addr):
    pos = s_addr.find(":")
    if pos >= 0:
        return (int(s_addr[:pos], 16), int(s_addr[pos + 1:], 16))
    return int(s_addr, 16)

def addr_list_from_str_list(s_addr_list):
    return (addr_from_str(s_addr) for s_addr in s_addr_list)

def get_memory_length(start_addr, end_addr):
    if isinstance(start_addr, tuple) and isinstance(end_addr, tuple):
        start_seg, start_offset = start_addr
        end_seg, end_offset = end_addr
        if start_seg != end_seg:
            raise Exception("Can't work with different memory segments.")
        return end_offset - start_offset
    return end_addr - start_addr

class RekoDictBase:
    __slots__ = []

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

class Comments:
    __slots__ = '_reko'

    def __init__(self, reko):
        self._reko = reko

    def __setitem__(self, addr, comment):
        addr = reko_address(self._reko, addr)
        if not isinstance(comment, str):
            raise TypeError(
                'Unsupported type: {}. Expected type: str'.format(
                    type(comment)))
        self._reko.SetUserComment(addr, comment)

class MemorySlice:
    __slots__ = '_reko', '_start_addr', '_length'

    def __init__(self, reko, start_addr, length):
        self._reko = reko
        self._start_addr = start_addr
        self._length = length

    @property
    def byte(self):
        if self._length is None:
            return self._reko.ReadByte(self._start_addr)
        return self._reko.ReadBytes(self._start_addr, self._length)

    @property
    def int16(self):
        if self._length is None:
            return self._reko.ReadInt16(self._start_addr)
        return self._reko.ReadInts16(self._start_addr, self._length)

    @property
    def int32(self):
        if self._length is None:
            return self._reko.ReadInt32(self._start_addr)
        return self._reko.ReadInts32(self._start_addr, self._length)

    @property
    def int64(self):
        if self._length is None:
            return self._reko.ReadInt64(self._start_addr)
        return self._reko.ReadInts64(self._start_addr, self._length)

    @property
    def c_str(self):
        return self._reko.ReadCString(self._start_addr)

class Memory:
    __slots__ = '_reko'

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
            length = get_memory_length(key.start, key.stop)
        else:
            start_addr = key
            length = None
        start_addr = reko_address(self._reko, start_addr)
        return MemorySlice(self._reko, start_addr, length)

class Globals:
    __slots__ = '_reko'

    def __init__(self, reko):
        self._reko = reko

    def __setitem__(self, addr, decl):
        addr = reko_address(self._reko, addr)
        if isinstance(decl, str):
            self._reko.SetUserGlobal(addr, decl)
            return
        raise TypeError('Unsupported type: {}'.format(type(decl)))

class Procedure:
    __slots__ = '_reko', '_addr'

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

    @property
    def file(self):
        return self._reko.GetProcedureOutputFile(self._addr)

    @file.setter
    def file(self, value):
        self._reko.SetUserProcedureOutputFile(self._addr, value)

class Procedures(RekoDictBase):
    __slots__ = '_reko'

    def __init__(self, reko):
        self._reko = reko

    def __getitem__(self, addr):
        try:
            addr = reko_address(self._reko, addr)
        except Exception:
            raise KeyError(addr)
        if not self._reko.ContainsProcedureAddress(addr):
            raise KeyError(addr)
        return Procedure(self._reko, addr)

    def __setitem__(self, addr, decl):
        addr = reko_address(self._reko, addr)
        if isinstance(decl, str):
            self._reko.SetUserProcedure(addr, decl)
            return
        raise TypeError('Unsupported type: {}'.format(type(decl)))

    def __iter__(self):
        return addr_list_from_str_list(self._reko.GetProcedureAddresses())

class Program:
    __slots__ = '_comments', '_memory', '_globals', '_procedures'

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
