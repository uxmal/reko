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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;

namespace Reko.UnitTests.Mocks
{
    /// <summary>
    /// Used for siting components for unit testing. First create component, 
    /// then create a FakeComponentSite and call the AddService method to add 
    /// any services the component might
    /// want to use. Finally set the Site property of the component to this object.
    /// </summary>
    public class FakeComponentSite : ISite, IServiceContainer
    {
        private IServiceContainer sc;

        public FakeComponentSite(IComponent component) : this(component, new ServiceContainer())
        {
            sc = new ServiceContainer();
        }

        public FakeComponentSite(IComponent component, IServiceContainer sc)
        {
            this.sc = sc;
        }

        public FakeComponentSite(IServiceContainer sc)
        {
            this.sc = sc;
        }

        public void AddService(Type svc, object impl)
        {
            sc.AddService(svc, impl);
        }

        public IComponent Component
        {
            get { throw new NotImplementedException(); }
        }

        public IContainer Container
        {
            get { throw new NotImplementedException(); }
        }

        public bool DesignMode
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public object GetService(Type serviceType)
        {
            return sc.GetService(serviceType);
        }


        #region IServiceContainer Members

        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            sc.AddService(serviceType, callback, promote);
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            sc.AddService(serviceType, callback);
        }

        public void AddService(Type serviceType, object serviceInstance, bool promote)
        {
            sc.AddService(serviceType, serviceInstance, promote);
        }

        public void RemoveService(Type serviceType, bool promote)
        {
            sc.RemoveService(serviceType, promote);
        }

        public void RemoveService(Type serviceType)
        {
            sc.RemoveService(serviceType);
        }

        #endregion
    }
}
