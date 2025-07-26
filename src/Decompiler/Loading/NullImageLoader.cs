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
using Reko.Core.Loading;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Loading
{
    /// <summary>
    /// The NullLoader is used when Reko is unable to determine what image
    /// loader to use. It doesn't support disassembly.
    /// </summary>
    public class NullImageLoader : ProgramImageLoader
    {
        private Address baseAddr;
        private readonly byte[] imageBytes;

        /// <summary>
        /// Creates an instance of the <see cref="NullImageLoader"/> class.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> used to access contextual services.
        /// </param>
        /// <param name="imageLocation">Location from which the <paramref name="image"/> was read.</param>
        /// <param name="image">Raw image bytes.</param>
        public NullImageLoader(IServiceProvider services, ImageLocation imageLocation, byte[] image) : base(services, imageLocation, image)
        {
            this.imageBytes = image;
            this.baseAddr = Address.Ptr32(0);
            this.EntryPoints = [];
        }

        /// <summary>
        /// Architecture of the processor for which this image loader is intended.
        /// </summary>
        public IProcessorArchitecture? Architecture { get; set; }

        /// <summary>
        /// Entry points of the program.
        /// </summary>
        public List<ImageSymbol> EntryPoints { get; }

        /// <summary>
        /// <see cref="IPlatform"/> for the loaded image
        /// </summary>
        public IPlatform? Platform { get; set; }

        /// <inheritdoc/>
        public override Address PreferredBaseAddress
        {
            get { return this.baseAddr; }
            set { this.baseAddr = value; }
        }

        /// <inheritdoc/>
        public override Program LoadProgram(Address? addrLoad)
        {
            if (Architecture is null)
                throw new InvalidOperationException("A processor architecture must be specified.");
            return Load(addrLoad, Architecture);
        }

        /// <inheritdoc/>
        public override Program Load(Address? addrLoad, IProcessorArchitecture? arch)
        {
            if (arch is null)
                throw new InvalidOperationException("A processor architecture must be specified.");
            if (addrLoad is null)
                addrLoad = PreferredBaseAddress;
            var platform = Platform ?? new DefaultPlatform(Services, arch);
            return LoadProgram(addrLoad.Value, arch, platform, new());
        }

        /// <inheritdoc/>
        public override Program LoadProgram(
            Address addrLoad,
            IProcessorArchitecture arch,
            IPlatform platform,
            List<UserSegment> userSegments)
        {
            var segmentMap = CreatePlatformSegmentMap(platform, addrLoad, userSegments, imageBytes);
            var program = new Program(
                new ByteProgramMemory(segmentMap),
                arch,
                platform);
            foreach (var ep in this.EntryPoints)
            {
                program.EntryPoints[ep.Address] = ep;
            }
            return program;
        }

        /// <summary>
        /// Creates a <see cref="SegmentMap"/> based on the specified platform,
        /// </summary>
        /// <param name="platform"><see cref="IPlatform"/> </param>
        /// <param name="loadAddr">Base address of the loaded program.</param>
        /// <param name="userSegments">User-supplied segment information.</param>
        /// <param name="rawBytes">The raw bytes of the image.</param>
        /// <returns></returns>
        public SegmentMap CreatePlatformSegmentMap(
            IPlatform platform,
            Address loadAddr,
            List<UserSegment> userSegments,
            byte[] rawBytes)
        {
            var segmentMap = platform.CreateAbsoluteMemoryMap() ?? new SegmentMap(loadAddr);
            var mem = platform.Architecture.CreateCodeMemoryArea(loadAddr, rawBytes);
            if (userSegments.Any())
            {
                foreach (var useg in userSegments)
                {
                    //$TODO: warning?
                    var name = useg.Name ?? useg.Address.GenerateName("seg", "");
                    var seg = new ImageSegment(name, useg.Address, mem, useg.AccessMode);
                    seg.Size = useg.Length;
                    segmentMap.AddSegment(seg);
                }
            }
            else
            {
                segmentMap.AddSegment(mem, "code", AccessMode.ReadWriteExecute);
            }
            return segmentMap;
        }
    }
}
