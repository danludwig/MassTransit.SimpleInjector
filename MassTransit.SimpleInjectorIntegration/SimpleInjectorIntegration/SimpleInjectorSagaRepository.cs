using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit.Pipeline;
using MassTransit.Saga;
using SimpleInjector;

namespace MassTransit.SimpleInjectorIntegration
{
  public class SimpleInjectorSagaRepository<T> : ISagaRepository<T> where T : class, ISaga
  {
    private readonly Container _container;
    private readonly ISagaRepository<T> _repository;

    public SimpleInjectorSagaRepository(ISagaRepository<T> repository, Container container) {
      _repository = repository;
      _container = container;
    }

    public IEnumerable<Action<IConsumeContext<TMessage>>> GetSaga<TMessage>
      (IConsumeContext<TMessage> context, Guid sagaId, InstanceHandlerSelector<T, TMessage> selector, ISagaPolicy<T, TMessage> policy)
      where TMessage : class {

      return _repository.GetSaga(context, sagaId, selector, policy)
        .Select(consumer => (Action<IConsumeContext<TMessage>>)(x => {
          using (_container.BeginLifetimeScope())
            consumer(x);
        }));
    }

    public IEnumerable<Guid> Find(ISagaFilter<T> filter) {
      return _repository.Find(filter);
    }

    public IEnumerable<T> Where(ISagaFilter<T> filter) {
      return _repository.Where(filter);
    }

    public IEnumerable<TResult> Where<TResult>(ISagaFilter<T> filter, Func<T, TResult> transformer) {
      return _repository.Where(filter, transformer);
    }

    public IEnumerable<TResult> Select<TResult>(Func<T, TResult> transformer) {
      return _repository.Select(transformer);
    }
  }
}
