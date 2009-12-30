/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Serialization;
using Decompiler.Arch.Intel;
using Decompiler.Loading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class FakeLoader : LoaderBase
    {
        internal byte[] ImageBytes;
        private IProcessorArchitecture arch;
        private ProgramImage image;
        private Exception onLoadImageException;
        private int exceptionDelay;

        public IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Program Load(byte[] imageFile, Address addrLoad)
        {
            Program prog = new Program();
            if (arch == null)
            {
                arch = new IntelArchitecture(ProcessorMode.Real);
            }
            prog.Architecture = Architecture;

            if (addrLoad == null)
            {
                addrLoad = new Address(0xC00, 0);
            }
            if (image == null)
            {
                image = new ProgramImage(addrLoad, imageFile);
            }
            prog.Image = image;
            return prog;
        }


        public override byte[] LoadImageBytes(string fileName, int offset)
        {
            if (onLoadImageException != null)
            {
                if (--exceptionDelay < 0)
                    throw onLoadImageException;
            }
            if (ImageBytes != null)
                return ImageBytes;
            else
                return new byte[300];
        }

        internal void ThrowOnLoadImage(Exception ex, int exceptionDelay)
        {
            this.exceptionDelay = exceptionDelay;
            onLoadImageException = ex;
        }
    }
}
