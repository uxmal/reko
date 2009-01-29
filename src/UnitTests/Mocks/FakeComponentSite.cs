using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    /// <summary>
    /// Used for siting components for unit testing. First create component, 
    /// then create a FakeComponentSite and call the AddService method to add 
    /// any services the component might
    /// want to use. Finally set the Site property of the component to this object.
    /// </summary>
    public class FakeComponentSite : ISite
    {
        private ServiceContainer sc;

        public FakeComponentSite(IComponent component)
        {
            sc = new ServiceContainer();
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

    }
}
