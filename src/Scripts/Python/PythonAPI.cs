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
        private readonly ScriptScope scope;

        public PythonAPI(IServiceProvider services, ScriptEngine engine)
        {
            scope = EvaluatePythonDefinitions(services, engine);
        }

        public object CreateProgramWrapper(RekoProgramAPI rekoAPI)
        {
            var programCreator = scope.GetVariable<Func<RekoProgramAPI, object>>(
                "Program");
            return programCreator(rekoAPI);
        }

        private ScriptScope EvaluatePythonDefinitions(
            IServiceProvider services, ScriptEngine engine)
        {
            var scope = engine.CreateScope();
            var cfgSvc = services.RequireService<IConfigurationService>();
            var programAPIFile = cfgSvc.GetInstallationRelativePath(
                "_program.py");
            engine.ExecuteFile(programAPIFile, scope);
            return scope;
        }
    }
}
