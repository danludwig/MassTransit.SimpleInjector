using System;
using MassTransit.Fakes;
using SimpleInjector;
using Xunit;
using System.Threading;

namespace MassTransit
{
  public class WhenIntegratingMassTransitWithSimpleInjector
  {
    [Fact]
    public void ItShouldGetConsumersFromTheContainer() {
      using (var container = new Container()) {

        var consumerAssemblies = new[] { GetType().Assembly };
        container.Register(typeof(Consumes<>.All), consumerAssemblies);
        var endpointUri = new Uri("loopback://localhost/queue");
        container.RegisterSingleton(() =>
          ServiceBusFactory.New(sbc => {
            sbc.ReceiveFrom(endpointUri);
            sbc.Subscribe(subs => {
              // ReSharper disable AccessToDisposedClosure
              subs.LoadFrom(container);
              // ReSharper restore AccessToDisposedClosure
            });
          }));
        container.Verify();

        var bus = container.GetInstance<IServiceBus>();
        var message = new FakeMessage { Moment = DateTime.Now };
        bus.GetEndpoint(endpointUri).Send(message);

        Thread.Sleep(100);
        Assert.Equal(1, FakeMessage.MessageConsumedCount);
      }
    }
  }
}
