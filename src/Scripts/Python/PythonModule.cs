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
using Reko.Core;
using Reko.Core.Scripts;
using Reko.Core.Services;
using System;
using System.Diagnostics;
using System.IO;

namespace Reko.Scripts.Python
{
    /// <summary>
    /// Class for execution of Python modules. Use IronPython
    /// (https://ironpython.net) engine.
    /// </summary>
    public class PythonModule : ScriptFile
    {
        private readonly ScriptScope scope;
        private readonly PythonAPI pythonAPI;
        private readonly DecompilerEventListener eventListener;

        public PythonModule(
            IServiceProvider services, string filename, byte[] bytes)
        : base(services, filename, bytes)
        {
            this.eventListener = services.RequireService<DecompilerEventListener>();
            var stream = new MemoryStream(bytes);
            var engine = IronPython.Hosting.Python.CreateEngine();
            // $TODO: Redirect script output to Reko Console tab
            engine.Runtime.IO.RedirectToConsole();
            using var rdr = new StreamReader(stream);
            this.scope = Evaluate(rdr.ReadToEnd(), filename, engine);
            this.pythonAPI = new PythonAPI(services, engine);
        }

        public override void CallFunction(string funcName, Program program)
        {
            try
            {
                var rekoAPI = new RekoProgramAPI(program);
                var programWrapper = pythonAPI.CreateProgramWrapper(rekoAPI);
                var func = GetFunction(funcName);
                func(programWrapper);
            }
            catch (Exception ex)
            {
                eventListener.Error(
                    new NullCodeLocation(Filename),
                    ex,
                    "An error occurred while running the Python script.");
                DumpPythonStack(ex, scope.Engine);
            }
        }

        private Action<object> GetFunction(string funcName)
        {
            return scope.GetVariable<Action<object>>(funcName);
        }

        private ScriptScope Evaluate(
            string script,
            string filename,
            ScriptEngine engine)
        {
            var src = engine.CreateScriptSourceFromString(script, filename);
            var scope = engine.CreateScope();
            try
            {
                src.Execute(scope);
            }
            catch (Exception ex)
            {
                eventListener.Error(
                    new NullCodeLocation(filename),
                    ex,
                    "An error occurred while evaluating the Python script.");
                DumpPythonStack(ex, engine);
                return engine.CreateScope();
            }
            return scope;
        }

        private void DumpPythonStack(Exception ex, ScriptEngine engine)
        {
            var exceptionOperations = engine.GetService<ExceptionOperations>();
            // $TODO: Allow user to go to failed line. It can be obtained by
            // GetStackFrames method of ExceptionOperations class */
            var msg = exceptionOperations.FormatException(ex);
            Debug.Print(msg);
        }
    }
}
