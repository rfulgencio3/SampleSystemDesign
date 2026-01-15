namespace SampleSystemDesign.ScalingReads.Application.Interfaces;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
