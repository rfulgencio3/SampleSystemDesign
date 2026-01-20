using SampleSystemDesign.LongRunning.Application.UseCases;
using SampleSystemDesign.LongRunning.Infrastructure.Persistence;

namespace SampleSystemDesign.LongRunning.Tests.Application.UseCases;

public class GetImageJobStatusQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsNotFoundWhenMissing()
    {
        var repository = new InMemoryImageJobRepository();
        var handler = new GetImageJobStatusQueryHandler(repository);

        var result = await handler.HandleAsync(new GetImageJobStatusQuery(Guid.NewGuid()));

        Assert.False(result.Found);
        Assert.Null(result.Status);
    }
}
