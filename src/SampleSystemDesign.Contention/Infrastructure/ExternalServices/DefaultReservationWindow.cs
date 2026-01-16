namespace SampleSystemDesign.Contention.Infrastructure.ExternalServices;

using SampleSystemDesign.Contention.Application.Interfaces;

public sealed class DefaultReservationWindow : IReservationWindow
{
    public DefaultReservationWindow(TimeSpan holdDuration)
    {
        if (holdDuration <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(holdDuration));
        HoldDuration = holdDuration;
    }

    public TimeSpan HoldDuration { get; }
}
