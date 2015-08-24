using System;
using Magnum.Reflection;
using MassTransit.Saga;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Util;
using SimpleInjector;

namespace MassTransit.SimpleInjectorIntegration
{
  public class SimpleInjectorSagaFactoryConfigurator
  {
    private readonly SubscriptionBusServiceConfigurator _configurator;
    private readonly Container _container;

    public SimpleInjectorSagaFactoryConfigurator(SubscriptionBusServiceConfigurator configurator, Container container) {
      _configurator = configurator;
      _container = container;
    }

    public void ConfigureSaga(Type messageType) {
      this.FastInvoke(new[] { messageType }, "Configure");
    }

    [UsedImplicitly]
    public void Configure<T>() where T : class, ISaga {
      _configurator.Saga(new SimpleInjectorSagaRepository<T>(_container.GetInstance<ISagaRepository<T>>(), _container));
    }
  }
}