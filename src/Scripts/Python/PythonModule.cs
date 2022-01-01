#region License
/* 
 * Copyright (C) 1999-2022 Pavel Tomin.
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
using Reko.Core.Configuration;
using Reko.Core.Lib;
using Reko.Core.Scripts;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Scripts.Python
{
    /// <summary>
    /// Class for execution of Python modules. Use IronPython
    /// (https://ironpython.net) engine.
    /// </summary>
    public class PythonModule : ScriptFile
    {
        private readonly TextWriter outputWriter;
        private readonly DecompilerEventListener eventListener;
        private readonly IConfigurationService cfgSvc;
        private readonly IFileSystemService fsSvc;
        private RekoEventsAPI eventsAPI;

        public PythonModule(IServiceProvider services, ImageLocation scriptLocation, byte[] bytes)
            : base(services, scriptLocation, bytes)
        {
            if (scriptLocation.HasFragments)
                throw new NotSupportedException("Loading scripts inside archives is not supported yet.");
            var filename = scriptLocation.FilesystemPath;
            this.eventListener = services.RequireService<DecompilerEventListener>();
            this.cfgSvc = services.RequireService<IConfigurationService>();
            this.fsSvc = services.RequireService<IFileSystemService>();
            var outputService = services.RequireService<IOutputService>();
            this.outputWriter = outputService.EnsureOutputSource("Scripting");
            var stream = new MemoryStream(bytes);
            using var rdr = new StreamReader(stream);
            this.eventsAPI = Evaluate(
                outputWriter, eventListener, cfgSvc, fsSvc, rdr.ReadToEnd(),
                filename);
        }

        public override void Evaluate(string script)
        {
            var filename = this.Location.FilesystemPath;
            this.eventsAPI = Evaluate(
                outputWriter, eventListener, cfgSvc, fsSvc, script, filename);
        }

        public override void FireEvent(ScriptEvent @event, Program program)
        {
            var eventsAPI = this.eventsAPI;
            var engine = eventsAPI.Engine;
            try
            {
                var pythonAPI = new PythonAPI(cfgSvc, fsSvc, engine);
                var programAPI = new RekoProgramAPI(program);
                var programWrapper = pythonAPI.CreateProgramWrapper(
                    programAPI);
                eventsAPI.FireEvent(@event, programWrapper);
            }
            catch (Exception ex)
            {
                var scriptError = CreateError(
                    Location.FilesystemPath,
                    ex,
                    "An error occurred while running the Python script.",
                    engine);
                eventListener.Error(scriptError);
                DumpPythonStack(outputWriter, ex, engine);
            }
        }

        private static RekoEventsAPI Evaluate(
            TextWriter outputWriter,
            DecompilerEventListener eventListener,
            IConfigurationService cfgSvc,
            IFileSystemService fsSvc,
            string script,
            string filename)
        {
            var engine = CreateEngine(outputWriter);
            outputWriter.WriteLine($"Evaluating {filename}");
            var pythonAPI = new PythonAPI(cfgSvc, fsSvc, engine);
            var eventsAPI = new RekoEventsAPI(engine);
            var scope = CreateRekoVariable(engine, pythonAPI, eventsAPI);
            var src = engine.CreateScriptSourceFromString(script, filename);
            try
            {
                src.Execute(scope);
            }
            catch (Exception ex)
            {
                var scriptError = CreateError(
                    filename,
                    ex,
                    "An error occurred while evaluating the Python script.",
                    engine);
                eventListener.Error(scriptError);
                DumpPythonStack(outputWriter, ex, engine);
                return new RekoEventsAPI(engine);
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
            var msg = exceptionOperations.FormatException(ex);
            writer.WriteLine(msg);
        }

        private static ScriptError CreateError(
            string fileName, Exception ex, string message, ScriptEngine engine)
        {
            var stackFrames = GetStackFrames(ex, engine);
            return new ScriptError(fileName, ex, message, stackFrames);
        }

        private static IEnumerable<ScriptStackFrame> GetStackFrames(
            Exception ex, ScriptEngine engine)
        {
            var exceptionOperations = engine.GetService<ExceptionOperations>();
            foreach (var stackFrame in exceptionOperations.GetStackFrames(ex))
            {
                yield return new ScriptStackFrame(
                    stackFrame.GetFileName(),
                    stackFrame.GetFileLineNumber(),
                    stackFrame.GetMethodName()
                );
            }
        }

        private static ScriptEngine CreateEngine(
            TextWriter outputWriter,
            int recursionLimit = 100)
        {
            // Set recursion limit to avoid application crash if there is
            // infinite recursion
            var options = new Dictionary<string, object>()
            {
                { "RecursionLimit", recursionLimit },
            };
            // Redirect console output before engine was created
            // See https://github.com/IronLanguages/ironpython3/issues/961
            // $TODO: remove this workaround when new IronPython will be
            // released
            var runtime = IronPython.Hosting.Python.CreateRuntime(options);
            RedirectConsoleOutput(outputWriter, runtime);
            var engine = IronPython.Hosting.Python.GetEngine(runtime);
            outputWriter.WriteLine(engine.Setup.DisplayName);
            return engine;
        }

        private static void RedirectConsoleOutput(
            TextWriter writer, ScriptRuntime runtime)
        {
            var stream = new TextWriterStream(writer);
            runtime.IO.SetOutput(stream, writer.Encoding);
            runtime.IO.SetErrorOutput(stream, writer.Encoding);
        }
    }
}
