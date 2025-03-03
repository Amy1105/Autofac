﻿// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using Autofac.Features.Decorators;
using Autofac.Test.Scenarios.RegistrationSources;
using Autofac.Test.Util;

namespace Autofac.Test.Core.Lifetime;

public class LifetimeScopeTests
{
    [Fact]
    public void AdaptersInNestedScopeOverrideAdaptersInParent()
    {
        const string parentInstance = "p";
        const string childInstance = "c";

        var builder = new ContainerBuilder();
        // _dynamicRegistrationSources    外部注册来源list中，添加一个
        builder.ComponentRegistryBuilder.AddRegistrationSource(new ObjectRegistrationSource(parentInstance));

        var parent = builder.Build();
        var child = parent.BeginLifetimeScope(lifetimeScopeBuilder =>
                lifetimeScopeBuilder.RegisterSource(new ObjectRegistrationSource(childInstance)));
        var fromChild = child.Resolve<object>();
        Assert.Same(childInstance, fromChild);
    }

    [Fact]
    public void NestedComponentRegistryIsProperlyDisposed()
    {
        var builder = new ContainerBuilder();
        var container = builder.Build();
        using var nestedRegistration = Mocks.GetComponentRegistration();
        var child = container.BeginLifetimeScope(b => b.RegisterComponent(nestedRegistration));
        child.Dispose();
        Assert.True(nestedRegistration.IsDisposed);
    }

    [Fact]
    public void NestedComponentRegistryIsProperlyDisposedEvenWhenRegistryUpdatedLater()
    {
        var builder = new ContainerBuilder();
        var container = builder.Build();
        using var nestedRegistration = Mocks.GetComponentRegistration();
        var child = container.BeginLifetimeScope(x => x.ComponentRegistryBuilder.Register(nestedRegistration));
        child.Dispose();
        Assert.True(nestedRegistration.IsDisposed);
    }

    [Fact]
    public void NestedLifetimeScopesMaintainServiceLimitTypes()
    {
        // Issue #365
        var cb = new ContainerBuilder();
        cb.RegisterType<Person>();
        var container = cb.Build();
        var service = new TypedService(typeof(Person));
        using (var unconfigured = container.BeginLifetimeScope())
        {
            Assert.True(unconfigured.ComponentRegistry.TryGetRegistration(service, out IComponentRegistration reg), "The registration should have been found in the unconfigured scope.");
            Assert.Equal(typeof(Person), reg.Activator.LimitType);
        }

        using (var configured = container.BeginLifetimeScope(b => { }))
        {
            Assert.True(configured.ComponentRegistry.TryGetRegistration(service, out IComponentRegistration reg), "The registration should have been found in the configured scope.");
            Assert.Equal(typeof(Person), reg.Activator.LimitType);
        }
    }

    [Fact]
    public async Task AsyncDisposeLifetimeScopeDisposesRegistrationsAsync()
    {
        var cb = new ContainerBuilder();

        cb.RegisterType<DisposeTracker>().InstancePerLifetimeScope().AsSelf();
        cb.RegisterType<AsyncDisposeTracker>().InstancePerLifetimeScope().AsSelf();
        cb.RegisterType<AsyncOnlyDisposeTracker>().InstancePerLifetimeScope().AsSelf();

        var container = cb.Build();

        DisposeTracker tracker;
        AsyncDisposeTracker asyncTracker;
        AsyncOnlyDisposeTracker asyncOnlyTracker;

        await using (var scope = container.BeginLifetimeScope())
        {
            tracker = scope.Resolve<DisposeTracker>();
            asyncTracker = scope.Resolve<AsyncDisposeTracker>();
            asyncOnlyTracker = scope.Resolve<AsyncOnlyDisposeTracker>();

            Assert.False(tracker.IsDisposed);
            Assert.False(asyncTracker.IsSyncDisposed);
            Assert.False(asyncTracker.IsAsyncDisposed);
            Assert.False(asyncOnlyTracker.IsAsyncDisposed);
        }

        Assert.True(tracker.IsDisposed);
        Assert.True(asyncTracker.IsAsyncDisposed);
        Assert.True(asyncOnlyTracker.IsAsyncDisposed);
        Assert.False(asyncTracker.IsSyncDisposed);
    }

    [Fact]
    public void DisposeLifetimeScopeDisposesRegistrationsThatAreAsyncAndSyncDispose()
    {
        var cb = new ContainerBuilder();

        cb.RegisterType<DisposeTracker>().InstancePerLifetimeScope().AsSelf();
        cb.RegisterType<AsyncDisposeTracker>().InstancePerLifetimeScope().AsSelf();

        var container = cb.Build();

        DisposeTracker tracker;
        AsyncDisposeTracker asyncTracker;

        using (var scope = container.BeginLifetimeScope())
        {
            tracker = scope.Resolve<DisposeTracker>();
            asyncTracker = scope.Resolve<AsyncDisposeTracker>();

            Assert.False(tracker.IsDisposed);
            Assert.False(asyncTracker.IsSyncDisposed);
            Assert.False(asyncTracker.IsAsyncDisposed);
        }

        Assert.True(tracker.IsDisposed);
        Assert.False(asyncTracker.IsAsyncDisposed);
        Assert.True(asyncTracker.IsSyncDisposed);
    }

    [Fact]
    public void RegistrationSourcesAreIsolatedToLifetimeScopes()
    {
        // Issue #1071 - Custom registration source implementations need to account for DecoratorService.
        var container = new ContainerBuilder().Build();

        var scope1RegisteredInstance = new Test();
        var scope2RegisteredInstance = new Test();

        using var scope1 = container.BeginLifetimeScope(builder => builder.RegisterSource(new SimplifiedRegistrationSource(scope1RegisteredInstance)));
        var scope1ResolvedInstance = scope1.Resolve<ITest>();
        Assert.Same(scope1RegisteredInstance, scope1ResolvedInstance);

        using var scope2 = scope1.BeginLifetimeScope(builder => builder.RegisterSource(new SimplifiedRegistrationSource(scope2RegisteredInstance)));
        var scope2ResolvedInstance = scope2.Resolve<ITest>();
        Assert.Same(scope2RegisteredInstance, scope2ResolvedInstance);
    }

    private interface ITest
    {
    }

    private class Test : ITest
    {
    }

    private class SimplifiedRegistrationSource : IRegistrationSource
    {
        private readonly ITest _instance;

        public bool IsAdapterForIndividualComponents { get; }

        public SimplifiedRegistrationSource(ITest instance) => _instance = instance;

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            // Important that DecoratorService is not included here.
            if (service is not IServiceWithType serviceWithType || service is DecoratorService)
            {
                yield break;
            }

            if (IsTestType(serviceWithType.ServiceType))
            {
                yield return CreateRegistration(service, serviceWithType.ServiceType, (c, p) => _instance);
            }
        }

        private static bool IsTestType(Type serviceType) => typeof(ITest).IsAssignableFrom(serviceType);

        [SuppressMessage("CA2000", "CA2000", Justification = "The registration disposes of the activator automatically.")]
        private static ComponentRegistration CreateRegistration(Service service, Type serviceType, Func<IComponentContext, IEnumerable<Parameter>, object> factory) =>
            new(
                Guid.NewGuid(),
                new DelegateActivator(serviceType, factory),
                CurrentScopeLifetime.Instance,
                InstanceSharing.None,
                InstanceOwnership.OwnedByLifetimeScope,
                new[] { service },
                new Dictionary<string, object>());
    }

    internal class DependsOnRegisteredInstance
    {
        public DependsOnRegisteredInstance(object instance)
        {
            Instance = instance;
        }

        internal object Instance { get; set; }
    }

    private class Person
    {
    }
}
