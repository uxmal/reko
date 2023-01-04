#region License
/* 
 * Copyright (C) 1999-2023 Pavel Tomin.
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
using Reko.Core.Configuration;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.Scripts.Python
{
    /// <summary>
    /// Сlass for evaluation of Python API definitions which can be used in
    /// user-defined scripts.
    /// </summary>
    public class PythonAPI
    {
        public PythonAPI(
            IConfigurationService cfgSvc,
            IFileSystemService fsSvc,
            ScriptEngine engine)
        {
            this.CreateProgramWrapper = EvaluatePythonDefinitions(
                cfgSvc, fsSvc, engine, "_program.py", "Program");
            this.CreateRekoWrapper = EvaluatePythonDefinitions(
                cfgSvc, fsSvc, engine, "_reko.py", "Reko");
        }

        public readonly Func<object, object> CreateProgramWrapper;
        public readonly Func<object, object> CreateRekoWrapper;

        private static Func<object, object> EvaluatePythonDefinitions(
            IConfigurationService cfgSvc,
            IFileSystemService fsSvc,
            ScriptEngine engine,
            string fileName,
            string className)
        {
            var scope = engine.CreateScope();
            var fileDirectory = "Python";
            var absolutePath = cfgSvc.GetInstallationRelativePath(
                fileDirectory, fileName);
            var src = CreateScriptSource(fsSvc, engine, absolutePath);
            src.Execute(scope);
            return scope.GetVariable<Func<object, object>>(className);
        }

        private static ScriptSource CreateScriptSource(
            IFileSystemService fsSvc,
            ScriptEngine engine,
            string path)
        {
            var bytes = fsSvc.ReadAllBytes(path);
            var stream = new MemoryStream(bytes);
            using var rdr = new StreamReader(stream);
            var script = rdr.ReadToEnd();
            return engine.CreateScriptSourceFromString(script, path);
        }
    }
}
