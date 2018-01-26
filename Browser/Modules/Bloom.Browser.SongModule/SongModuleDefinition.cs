﻿using Bloom.Browser.SongModule.Services;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace Bloom.Browser.SongModule
{
    /// <summary>
    /// Browser song module.
    /// </summary>
    [Module(ModuleName = "SongModule")]
    public class SongModuleDefinition : IModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SongModule"/> class.
        /// </summary>
        /// <param name="container">The DI container.</param>
        public SongModuleDefinition(IUnityContainer container)
        {
            _container = container;
        }
        private readonly IUnityContainer _container;

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            // Register services this module provides
            _container.RegisterType<ISongService, SongService>(new ContainerControlledLifetimeManager());
            _container.Resolve(typeof(ISongService));
        }
    }
}