/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler;
using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Loading;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class FakeDecompiler : IDecompiler
    {
        public bool LoadProgram_Called;
        public bool ScanProgram_Called;

        private DecompilerProject project;
        private Program prog;
        private LoaderBase loader;

        public FakeDecompiler(LoaderBase loader)
        {
            this.loader = loader;
        }

        #region IDecompiler Members

        public Program Program
        {
            get { return prog; }
        }

        public DecompilerProject Project
        {
            get { return project; }
        }

        public void LoadProgram(string fileName)
        {
            LoadProgram_Called = true;
            byte [] image = loader.LoadImageBytes(fileName, 0);

            this.prog = loader.Load(image, null);
            this.project = new DecompilerProject();
            this.project.Input.Filename = fileName;
        }

        public void ScanProgram()
        {
            ScanProgram_Called = true;
        }

        public Procedure ScanProcedure(Address procAddress)
        {
            throw new NotImplementedException();
        }

        public void RewriteMachineCode()
        {
            throw new NotImplementedException();
        }

        public DataFlowAnalysis AnalyzeDataFlow()
        {
            throw new NotImplementedException();
        }

        public void ReconstructTypes()
        {
            throw new NotImplementedException();
        }

        public void StructureProgram()
        {
            throw new NotImplementedException();
        }

        public void WriteDecompilerProducts()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
