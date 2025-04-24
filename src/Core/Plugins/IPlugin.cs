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

using Reko.Core.Configuration;
using System.Collections.Generic;

namespace Reko.Core.Plugins
{
    /// <summary>
    /// A class implementing this interface can be loaded into Reko using its 
    /// plugin extension mechanism.
    /// </summary>
    /// <remarks>
    /// Reko will locate extension plugins by loading any assemblies it can find
    /// in the "plugins" subdirectory of the directory where the Reko executable
    /// is located.
    /// </remarks>
    public interface IPlugin
    {
        /// <summary>
        /// The collection of <see cref="IProcessorArchitecture"/>s supported
        /// by this plugin.
        /// </summary>
        /// <remarks>
        /// The <see cref="ArchitectureDefinition.Name"/> and <see cref="ArchitectureDefinition.Type"/>
        /// properties are essential to the proper operation of the plugin. Neglecting to specify
        /// these properties will cause the architecture to fail to load. The <see cref="ArchitectureDefinition.Description"/>
        /// field helps users identify the architecture in the Reko user interface.
        /// </remarks>
        IReadOnlyCollection<ArchitectureDefinition> Architectures { get; }

        /// <summary>
        /// The collection of <see cref="Reko.Core.Loading.ImageLoader"/>s supported
        /// by this plugin.
        /// </summary>
        /// <remarks>
        /// The <see cref="LoaderDefinition.TypeName"/> and <see cref="LoaderDefinition.Type"/>
        /// properties are essential to the proper operation of the plugin. Neglecting to specify
        /// these properties will cause the architecture to fail to load.
        /// </remarks>
        IReadOnlyCollection<LoaderDefinition> Loaders { get; }
    }
}
