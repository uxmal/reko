#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Configuration
{
    /// <summary>
    /// Provides access to information stored in the app.config file.
    /// </summary>
    public interface IDecompilerConfigurationService
    {
         ICollection GetImageLoaders();
         ICollection GetArchitectures();
         ICollection GetEnvironments();

         OperatingEnvironment GetEnvironment(string envName);
    }

    public class DecompilerConfiguration : IDecompilerConfigurationService
    {
        public virtual ICollection GetImageLoaders()
        {
            var handler = (LoaderSectionHandler) ConfigurationManager.GetSection("Decompiler/Loaders");
            return handler.ImageLoaders;
        }

        public virtual ICollection GetArchitectures()
        {
            var handler = (ArchitectureSectionHandler) ConfigurationManager.GetSection("Decompiler/Architectures");
            return handler.Architectures;
        }

        public virtual ICollection GetEnvironments()
        {
            var handler = (OperatingEnvironmentSectionHandler) ConfigurationManager.GetSection("Decompiler/Environments");
            return handler.Environments;
        }

        public OperatingEnvironment GetEnvironment(string envName)
        {
            return GetEnvironments().OfType<OperatingEnvironment>().Where(e => e.Name == envName).Single();
        }
    }
}
