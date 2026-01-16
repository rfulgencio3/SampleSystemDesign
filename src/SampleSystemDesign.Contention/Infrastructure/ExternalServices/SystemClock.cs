namespace SampleSystemDesign.Contention.Infrastructure.ExternalServices;

using SampleSystemDesign.Contention.Application.Interfaces;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
