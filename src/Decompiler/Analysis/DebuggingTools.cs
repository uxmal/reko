#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Core;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Analysis;

/// <summary>
/// Debugging tools for decompiler development.
/// This class contains methods that are only compiled in DEBUG builds.
/// </summary>
public class DebuggingTools
{
    private readonly Program program;
    private readonly IServiceProvider services;
    private readonly Dictionary<string, int> phaseNumbering;

    /// <summary>
    /// Creates a new instance of <see cref="DebuggingTools"/>.
    /// </summary>
    /// <param name="program"></param>
    /// <param name="services"></param>
    public DebuggingTools(Program program, IServiceProvider services)
    { 
        this.program = program;
        this.services = services;
        this.phaseNumbering = new Dictionary<string, int>();
    }

    /// <summary>
    /// Dumps a procedure to the debug output if it is being "watched",
    /// i.e. is in the <see cref="UserData.DebugTraceProcedures"/>
    /// collection of program being analyzed.
    /// </summary>
    /// <param name="phase">Short identifer of the current analysis phase.</param>
    /// <param name="caption"></param>
    /// <param name="proc"></param>

    [Conditional("DEBUG")]
    public void DumpWatchedProcedure(string phase, string caption, Procedure proc)
    {
        if (program.User.DebugTraceProcedures.Contains(proc.Name) ||
            proc.Name == "usb_device_info" ||
            proc.Name == "fn0002466C" ||
            proc.Name == "PM_CUSOR_DRAW_CreateSurfaceAndImgDecoding" ||
            false)
        {
            Debug.Print("// {0}: {1} ==================", proc.Name, caption);
            //MockGenerator.DumpMethod(proc);
            proc.Dump(true);
            var testSvc = services.GetService<ITestGenerationService>();
            if (testSvc is { })
            {
                if (!this.phaseNumbering.TryGetValue(phase, out int n))
                {
                    n = phaseNumbering.Count + 1;
                    phaseNumbering.Add(phase, n);
                }
                testSvc.ReportProcedure($"analysis_{n:00}_{phase}.txt", $"// {proc.Name} ===========", proc);
            }
        }
    }
}
