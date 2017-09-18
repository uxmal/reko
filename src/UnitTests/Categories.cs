#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests
{
    public static class Categories
    {
        public const string Regressions = "Regressions";

        // Unit tests in the UnitTests category must complete without 
        // performing IO. This means not using "recordings" saved 
        // on disk or popping up windows during the test.
        public const string UnitTests = "UnitTests";

        // Tests that are slower, because they show UI or hit
        // the file system, are considered integration tests.
        public const string IntegrationTests = "IntegrationTests";

        // The purpose of FailedTests category is to avoid running such unit
        // tests under Travis CI before fixing of Reko so that they could pass.
        public const string FailedTests = "FailedTests";

        // The purpose of UserInterface is to avoid running such unit tests
        // under Travis CI, since they require an X server to pass, and
        // the Travis CI environment doesn't provide one.
        public const string UserInterface = "UserInterface";

        // The Capstone category identifies unit tests that depend on Capstone. 
        // The Reko build provides special mono-compatible linux Capstone 
        // disassembler libraries for ARM, so they can run on Travis.
        public const string Capstone = "Capstone";

        // Tests that are currently failing in the "analysis-development"
        // branch. Before merging back into "master", these must all be 
        // running again.
        public const string AnalysisDevelopment = "AnalysisDevelopment";

        // Tests that are currently failing in the "explicit-x87-stack"
        // branch. Before merging back into "master", these must all be 
        // running again.
        public const string ExplicitX87Stack = "ExplicitX87Stack";

        // Tests that may fail due to work in progress
        public const string WorkInProgress = "WIP";
    }
}
