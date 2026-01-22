using SampleSystemDesign.Contention.Application.Interfaces;

namespace SampleSystemDesign.Contention.Infrastructure.ExternalServices;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
