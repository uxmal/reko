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
using System.IO;

namespace Reko.Scripts.Python
{
    /// <summary>
    /// Class for execution of Python modules. Use IronPython
    /// (https://ironpython.net) engine.
    /// </summary>
    public class PythonModule : ScriptFile
    {
        private readonly ScriptEngine engine;
        private readonly TextWriter outputWriter;
        private readonly PythonAPI pythonAPI;
        private readonly DecompilerEventListener eventListener;
        private readonly RekoEventsAPI eventsAPI;

        public PythonModule(
            IServiceProvider services, string filename, byte[] bytes)
        : base(services, filename, bytes)
        {
            this.eventListener = services.RequireService<DecompilerEventListener>();
            this.engine = IronPython.Hosting.Python.CreateEngine();
            var outputService = services.RequireService<IOutputService>();
            this.outputWriter = outputService.RegisterOutputSource("Scripting");
            RedirectConsoleOutput(outputWriter, engine);
            this.pythonAPI = new PythonAPI(services, engine);
            var stream = new MemoryStream(bytes);
            using var rdr = new StreamReader(stream);
            this.eventsAPI = Evaluate(rdr.ReadToEnd(), filename);
        }

        public override void FireEvent(ScriptEvent @event, Program program)
        {
            try
            {
                var programAPI = new RekoProgramAPI(program);
                var programWrapper = pythonAPI.CreateProgramWrapper(
                    programAPI);
                eventsAPI.FireEvent(@event, programWrapper);
            }
            catch (Exception ex)
            {
                eventListener.Error(
                    new NullCodeLocation(Filename),
                    ex,
                    "An error occurred while running the Python script.");
                DumpPythonStack(outputWriter, ex, engine);
            }
        }

        private RekoEventsAPI Evaluate(string script, string filename)
        {
            outputWriter.WriteLine($"Evaluating {filename}");
            var eventsAPI = new RekoEventsAPI();
            var scope = CreateRekoVariable(engine, pythonAPI, eventsAPI);
            var src = engine.CreateScriptSourceFromString(script, filename);
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
                DumpPythonStack(outputWriter, ex, engine);
                return new RekoEventsAPI();
            }
            return eventsAPI;
        }

        private static ScriptScope CreateRekoVariable(
            ScriptEngine engine,
            PythonAPI pythonAPI,
            RekoEventsAPI eventsAPI)
        {
            var scope = engine.CreateScope();
            var rekoWrapper = pythonAPI.CreateRekoWrapper(eventsAPI);
            scope.SetVariable("reko", rekoWrapper);
            return scope;
        }

        private static void DumpPythonStack(
            TextWriter writer, Exception ex, ScriptEngine engine)
        {
            var exceptionOperations = engine.GetService<ExceptionOperations>();
            // $TODO: Allow user to go to failed line. It can be obtained by
            // GetStackFrames method of ExceptionOperations class */
            var msg = exceptionOperations.FormatException(ex);
            writer.WriteLine(msg);
        }

        private static void RedirectConsoleOutput(
            TextWriter writer, ScriptEngine engine)
        {
            var stream = new MemoryStream();
            engine.Runtime.IO.SetOutput(stream, writer);
            engine.Runtime.IO.SetErrorOutput(stream, writer);
            writer.WriteLine(engine.Setup.DisplayName);
        }
    }
}
