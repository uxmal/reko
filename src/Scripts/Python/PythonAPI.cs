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
using Reko.Core.Configuration;
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
        public PythonAPI(IConfigurationService cfgSvc, ScriptEngine engine)
        {
            this.CreateProgramWrapper = EvaluatePythonDefinitions(
                cfgSvc, engine, "_program.py", "Program");
            this.CreateRekoWrapper = EvaluatePythonDefinitions(
                cfgSvc, engine, "_reko.py", "Reko");
        }

        public readonly Func<object, object> CreateProgramWrapper;
        public readonly Func<object, object> CreateRekoWrapper;

        private Func<object, object> EvaluatePythonDefinitions(
            IConfigurationService cfgSvc,
            ScriptEngine engine,
            string fileName,
            string className)
        {
            var scope = engine.CreateScope();
            var absolutePath = cfgSvc.GetInstallationRelativePath(fileName);
            engine.ExecuteFile(absolutePath, scope);
            return scope.GetVariable<Func<object, object>>(className);
        }
    }
}
