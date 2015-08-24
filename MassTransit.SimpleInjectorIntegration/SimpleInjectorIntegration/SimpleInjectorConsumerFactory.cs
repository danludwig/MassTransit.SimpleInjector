using MassTransit.Exceptions;
using MassTransit.Pipeline;
using SimpleInjector;
using System;
using System.Collections.Generic;

namespace MassTransit.SimpleInjectorIntegration
{
  public class SimpleInjectorConsumerFactory<T> : IConsumerFactory<T> where T : class
  {
    private readonly Container _container;

    public SimpleInjectorConsumerFactory(Container container) {
      _container = container;
    }

    public IEnumerable<Action<IConsumeContext<TMessage>>> GetConsumer<TMessage>
      (IConsumeContext<TMessage> context, InstanceHandlerSelector<T, TMessage> selector)
      where TMessage : class {

      using (_container.BeginLifetimeScope()) {
        T consumer = _container.GetInstance<T>();
        if (consumer == null)
          throw new ConfigurationException(string.Format("Unable to resolve type '{0}' from container: ", typeof(T)));
        foreach (Action<IConsumeContext<TMessage>> action in selector(consumer, context))
          yield return action;
      }
    }
  }
}
