using System;
using Magnum.Reflection;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Util;
using SimpleInjector;

namespace MassTransit.SimpleInjectorIntegration
{
  public class SimpleInjectorConsumerFactoryConfigurator
  {
    private readonly SubscriptionBusServiceConfigurator _configurator;
    private readonly Container _container;

    public SimpleInjectorConsumerFactoryConfigurator(SubscriptionBusServiceConfigurator configurator, Container container) {
      _configurator = configurator;
      _container = container;
    }

    public void ConfigureConsumer(Type messageType) {
      this.FastInvoke(new[] { messageType }, "Configure");
    }

    [UsedImplicitly]
    public void Configure<T>() where T : class, IConsumer {
      _configurator.Consumer(new SimpleInjectorConsumerFactory<T>(_container));
    }
  }
}
