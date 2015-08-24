using System;

namespace MassTransit.Fakes
{
  public class FakeMessage
  {
    public static int MessageConsumedCount = 0;

    public DateTime? Moment { get; set; }
  }
}
