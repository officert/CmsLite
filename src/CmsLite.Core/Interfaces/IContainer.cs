using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Modules;

namespace CmsLite.Core.Interfaces
{
    public class Container
    {
        private readonly IKernel _kernel;

        public Container()
        {
            _kernel = new StandardKernel();
        }

        public object Get(Type type)
        {
            return _kernel.Get(type);
        }

        public T GetInstance<T>() where T : class
        {
            return _kernel.Get<T>();
        }

        public IEnumerable<T> GetInstances<T>() where T : class, new()
        {
            return _kernel.GetAll<T>();
        }

        public void Load(IEnumerable<INinjectModule> modules)
        {
            _kernel.Load(modules);
        }
    }

    public interface IContainerAdapter
    {
        T GetInstance<T>() where T : class, new();
        IEnumerable<T> GetInstances<T>() where T : class;
    }
}