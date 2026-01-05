#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.ComponentModel.Design;
using System.Text;

namespace Reko.Core.Services
{
    /// <summary>
    /// Utility class for eliminating redundant mentions of service types when getting and adding services with
    /// the IServiceProvider and IServiceContainer interfaces.
    /// </summary>
    /// <remarks>Instead of:
    /// <code>
    ///     ISampleService foo = (ISampleService) sp.GetService(typeof(ISampleService));
    ///     serviceContainer.AddService(typeof(IOtherService), otherservice);
    /// </code>
    /// write
    /// <code>
    ///     var foo = sp.GetService&lt;ISampleService&gt;();
    ///     serviceContainer.AddService&lt;IOtherService&gt;(otherservice);
    /// </code>
    /// </remarks>
    public static class TypedServiceProvider
    {
        /// <summary>
        /// Gets a reference to the requested service, or null if it couldn't be found.
        /// </summary>
        /// <typeparam name="T">Type of the requested service.
        /// </typeparam>
        /// <param name="sp"><see cref="IServiceProvider"/> from which to request the service.
        /// </param>
        /// <returns>If the service exists, an instance of <typeparamref name="T"/>; otherwise null.
        /// </returns>
        public static T? GetService<T>(this IServiceProvider sp)
        {
            return (T?)sp.GetService(typeof(T));
        }

        /// <summary>
        /// Requests a reference to the requested service. If the service is not available
        /// throws an exception.
        /// </summary>
        /// <typeparam name="T">Type of the requested service.
        /// </typeparam>
        /// <param name="sp"><see cref="IServiceProvider"/> from which to request the service.
        /// </param>
        /// <returns>An instance of <typeparamref name="T"/>.
        /// </returns>
        public static T RequireService<T>(this IServiceProvider sp) where T : class
        {
            var result = sp.GetService<T>();
            if (result == default(T))
                throw new InvalidOperationException($"Service {typeof(T).Name} is required.");
            return result;
        }

        /// <summary>
        /// Adds a service to a <see cref="IServiceContainer"/>.
        /// </summary>
        /// <typeparam name="T">Type of the added service.</typeparam>
        /// <param name="sc">Service container to add the service to.</param>
        /// <param name="service">Service to add.</param>
        public static void AddService<T>(this IServiceContainer sc, T service)
            where T : notnull
        {
            sc.AddService(typeof(T), service);
        }
    }
}
