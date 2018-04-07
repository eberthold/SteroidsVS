﻿using System;
using Steroids.CodeQuality.Services;
using Steroids.Contracts;
using Steroids.Contracts.Core;
using SteroidsVS.Contracts;
using Unity;
using Unity.Lifetime;

namespace SteroidsVS
{
    public class Bootstrapper : IServiceProvider, IBootstrapper
    {
        protected static IUnityContainer RootContainer { get; private set; }

        protected virtual IUnityContainer Container { get; set; }

        public void Initialize(IVsPackageServices services)
        {
            RootContainer = new UnityContainer();
            Container = RootContainer;

            Container.RegisterInstance(services.Workspace);
            Container.RegisterInstance(services.ErrorList);
            Container.RegisterInstance(services.OutliningManagerService);

            Container.RegisterType<IWorkspaceManager, WorkspaceManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDiagnosticProvider, ErrorListDiagnosticProvider>(new ContainerControlledLifetimeManager());
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }
    }
}
