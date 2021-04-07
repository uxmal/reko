#region License
/* 
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
 */
#endregion

using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Scripts.Python
{
    /// <summary>
    /// Сlass for evaluation of Python API definitions which can be used in
    /// user-defined scripts.
    /// </summary>
    public class PythonAPI
    {
        private readonly ScriptScope scope;

        public PythonAPI(ScriptEngine engine)
        {
            scope = EvaluatePythonDefinitions(engine);
        }

        public object CreateProgramWrapper(RekoAPI rekoAPI)
        {
            var programCreator = scope.GetVariable<Func<RekoAPI, object>>(
                "Program");
            return programCreator(rekoAPI);
        }

        private ScriptScope EvaluatePythonDefinitions(ScriptEngine engine)
        {
            var pythonAPIDefinitions = @"
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
        self._reko = reko

    @property
    def comments(self):
        return Comments(self._reko)

    @property
    def memory(self):
        return Memory(self._reko)

    @property
    def globals(self):
        return Globals(self._reko)

    @property
    def procedures(self):
        return Procedures(self._reko)
";
            var scope = engine.CreateScope();
            engine.Execute(pythonAPIDefinitions, scope);
            return scope;
        }
    }
}
