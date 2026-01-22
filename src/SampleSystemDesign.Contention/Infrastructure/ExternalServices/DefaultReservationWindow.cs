using SampleSystemDesign.Contention.Application.Interfaces;

namespace SampleSystemDesign.Contention.Infrastructure.ExternalServices;

public sealed class DefaultReservationWindow : IReservationWindow
{
    public DefaultReservationWindow(TimeSpan holdDuration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(holdDuration, TimeSpan.Zero);
        HoldDuration = holdDuration;
    }

    public TimeSpan HoldDuration { get; }
}
