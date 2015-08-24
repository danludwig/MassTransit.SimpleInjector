using Magnum.Extensions;
using MassTransit.Saga;
using MassTransit.Saga.SubscriptionConfigurators;
using MassTransit.SimpleInjectorIntegration;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Util;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MassTransit
{
  public static class SimpleInjectorExtensions
  {
    public static void LoadFrom([NotNull] this SubscriptionBusServiceConfigurator configurator, [NotNull] Container container) {
      if (configurator == null) throw new ArgumentNullException("configurator");
      if (container == null) throw new ArgumentNullException("container");

      IList<Type> consumerTypes = FindTypes<IConsumer>(container, x => !x.Implements<ISaga>());
      if (consumerTypes.Count > 0) {
        var consumerFactoryConfigurator = new SimpleInjectorConsumerFactoryConfigurator(configurator, container);
        foreach (Type consumerType in consumerTypes)
          consumerFactoryConfigurator.ConfigureConsumer(consumerType);
      }

      IList<Type> sagaTypes = FindTypes<ISaga>(container, x => true);
      if (sagaTypes.Count <= 0) return;
      var sagaFactoryConfigurator = new SimpleInjectorSagaFactoryConfigurator(configurator, container);
      foreach (Type sagaType in sagaTypes)
        sagaFactoryConfigurator.ConfigureSaga(sagaType);
    }

    public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>
      ([NotNull] this SubscriptionBusServiceConfigurator configurator, [NotNull] Container container)
      where TConsumer : class, IConsumer {

      if (configurator == null) throw new ArgumentNullException("configurator");
      if (container == null) throw new ArgumentNullException("container");

      var consumerFactory = new SimpleInjectorConsumerFactory<TConsumer>(container);
      return configurator.Consumer(consumerFactory);
    }

    public static SagaSubscriptionConfigurator<TSaga> Saga<TSaga>
      ([NotNull] this SubscriptionBusServiceConfigurator configurator, [NotNull] Container container)
      where TSaga : class, ISaga {

      if (configurator == null) throw new ArgumentNullException("configurator");
      if (container == null) throw new ArgumentNullException("container");

      var sagaRepository = new SimpleInjectorSagaRepository<TSaga>(container.GetInstance<ISagaRepository<TSaga>>(), container);
      return configurator.Saga(sagaRepository);
    }

    private static IList<Type> FindTypes<T>(Container container, Func<Type, bool> filter) {
      // BUG?? unless we call GetRelationships on the collection registration, the
      // types won't show up in GetCurrentRegistrations
      var collectionRegistration = container.GetRegistration(typeof(IEnumerable<T>));
      if (collectionRegistration != null)
        collectionRegistration.GetRelationships();

      return container.GetCurrentRegistrations()
        .Where(x => x.ServiceType.Implements<T>())
        .Select(x => x.Registration.ImplementationType)
        .Where(filter)
        .ToList();
    }
  }
}
