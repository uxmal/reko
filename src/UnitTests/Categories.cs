#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
        /// <summary>
        /// The Regressions category marks unit tests written
        /// in response to regressions in unit tests or failures reported
        /// by users.
        /// </summary>
        public const string Regressions = "Regressions";

        /// <summary>
        /// The UnitTests category is used for unit tests that do not require
        /// any I/O. They should complete in at most 1 ms, which  means not
        /// using "recordings" saved on disk or popping up windows during the test.
        /// </summary>

        public const string UnitTests = "UnitTests";

        /// <summary>
        /// Tests that are slower, because they show UI or hit
        /// the file system, are considered integration tests.
        /// </summary>
        public const string IntegrationTests = "IntegrationTests";

        /// <summary>
        /// The purpose of FailedTests category is to avoid running such unit
        /// tests under Travis CI before fixing of Reko so that they could pass
        /// </summary>
        public const string FailedTests = "FailedTests";

        /// <summary>
        /// The purpose of UserInterface is to avoid running such unit tests
        /// under Travis CI, since they require an X server to pass, and
        /// the Travis CI environment doesn't provide one.
        /// </summary>
        public const string UserInterface = "UserInterface";

        /// <summary>
        /// The Capstone disassembler has bugs. We can't hope to get 
        /// them fixed soon, so we turn off some tests until the fixes are done.
        /// </summary>
        public const string Capstone = "Capstone";
        // Tests that are currently failing in the "analysis-development"
        // branch. Before merging back into "master", these must all be 
        // running again.
        public const string AnalysisDevelopment = "AnalysisDevelopment";

        // Tests that may fail due to work in progress
        public const string WorkInProgress = "WIP";
    }
}
