namespace SampleSystemDesign.Contention.Application.Interfaces;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
