namespace MassTransit.Fakes
{
  public class FakeMessageHandler : Consumes<FakeMessage>.All
  {
    public void Consume(FakeMessage message) {
      ++FakeMessage.MessageConsumedCount;
    }
  }
}