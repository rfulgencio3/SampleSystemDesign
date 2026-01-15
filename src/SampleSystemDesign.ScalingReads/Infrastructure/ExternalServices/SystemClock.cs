namespace SampleSystemDesign.ScalingReads.Infrastructure.ExternalServices;

using SampleSystemDesign.ScalingReads.Application.Interfaces;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
