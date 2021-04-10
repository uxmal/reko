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
    def __init__(self, reko, start_addr):
        self._reko = reko
        self._start_addr = start_addr

    @property
    def int32(self):
        return self._reko.ReadInt32(self._start_addr)

    @property
    def c_str(self):
        return self._reko.ReadCString(self._start_addr)

class Memory(object):
    def __init__(self, reko):
        self._reko = reko

    def __getitem__(self, addr):
        return MemorySlice(self._reko, addr)

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
        raise AttributeError('Can not read decompile attribute')

    @decompile.setter
    def decompile(self, value):
        self._reko.SetUserProcedureDecompileFlag(self._addr, value)

class Procedures(object):
    def __init__(self, reko):
        self._reko = reko

    def __getitem__(self, addr):
        return Procedure(self._reko, addr)

    def __setitem__(self, addr, proc):
        if isinstance(proc, str):
            self._reko.SetUserProcedure(addr, proc)
            return
        raise TypeError('Unsupported type: {}'.format(type(proc)))

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
