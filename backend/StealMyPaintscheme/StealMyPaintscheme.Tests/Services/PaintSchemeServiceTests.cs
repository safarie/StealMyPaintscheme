using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;
using StealMyPaintscheme.Api.Services;
using Xunit;

namespace StealMyPaintscheme.Tests.Services;

public class PaintSchemeServiceTests
{
    [Fact]
    public async Task CreatePaintSchemeAsync_SeparatesUserGlobalAndInvalidPaintsWithoutChangingStepOrder()
    {
        var schemeRepository = new Mock<IPaintSchemeRepository>();
        var paintRepository = new Mock<IPaintRepository>();
        paintRepository.Setup(repo => repo.GetValidPaintIdsAsync(7, It.IsAny<List<int>>()))
            .ReturnsAsync([11]);
        var globalPaintRepository = new Mock<IGlobalPaintRepository>();
        globalPaintRepository.Setup(repo => repo.GetAllIdsAsync()).ReturnsAsync([22]);
        var service = CreateService(schemeRepository, paintRepository, globalPaintRepository);
        var scheme = new PaintScheme
        {
            Name = "Scheme",
            Steps =
            [
                new Step
                {
                    PaintId = 11,
                    Paint = new Paint { Id = 11, UserId = 7, Name = "User paint", Type = "Base", Maker = "Maker" }
                },
                new Step
                {
                    PaintId = 22,
                    Paint = new Paint { Id = 22, UserId = 0, Name = "Global paint", Type = "Base", Maker = "Maker" }
                },
                new Step
                {
                    PaintId = 33,
                    Paint = new Paint { Id = 33, UserId = 9, Name = "Other paint", Type = "Base", Maker = "Maker" }
                }
            ]
        };

        var result = await service.CreatePaintSchemeAsync(scheme, 7);

        Assert.Same(scheme, result.Scheme);
        Assert.Null(result.Error);
        Assert.Equal(7, scheme.UserId);
        Assert.Equal(3, scheme.Steps.Count);
        Assert.Equal(11, scheme.Steps[0].PaintId);
        Assert.Null(scheme.Steps[0].GlobalPaintId);
        Assert.Null(scheme.Steps[1].PaintId);
        Assert.Equal(22, scheme.Steps[1].GlobalPaintId);
        Assert.Null(scheme.Steps[1].Paint);
        Assert.Null(scheme.Steps[2].PaintId);
        Assert.Null(scheme.Steps[2].GlobalPaintId);
        Assert.Null(scheme.Steps[2].Paint);
        schemeRepository.Verify(repo => repo.AddAsync(scheme), Times.Once);
        schemeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPaintSchemesAsync_SortsCurrentUsersFirstThenNewestFirst()
    {
        var currentUserOlder = new PaintScheme { Id = 1, UserId = 7, CreatedAt = new DateTime(2026, 1, 1) };
        var currentUserNewer = new PaintScheme { Id = 2, UserId = 7, CreatedAt = new DateTime(2026, 2, 1) };
        var otherUserNewer = new PaintScheme { Id = 3, UserId = 8, CreatedAt = new DateTime(2026, 3, 1) };
        var schemeRepository = new Mock<IPaintSchemeRepository>();
        schemeRepository.Setup(repo => repo.GetAllWithStepsAsync())
            .ReturnsAsync([currentUserOlder, otherUserNewer, currentUserNewer]);
        var service = CreateService(schemeRepository, new Mock<IPaintRepository>(), new Mock<IGlobalPaintRepository>());

        var schemes = await service.GetPaintSchemesAsync(7);

        Assert.Collection(schemes,
            scheme => Assert.Same(currentUserNewer, scheme),
            scheme => Assert.Same(currentUserOlder, scheme),
            scheme => Assert.Same(otherUserNewer, scheme));
    }

    [Fact]
    public async Task DeletePaintSchemeAsync_AllowsOwnerAndUsesOwnerScopedLookup()
    {
        var scheme = new PaintScheme { Id = 3, UserId = 7 };
        var schemeRepository = new Mock<IPaintSchemeRepository>();
        schemeRepository.Setup(repo => repo.GetByIdForUserAsync(3, 7)).ReturnsAsync(scheme);
        var service = CreateService(schemeRepository, new Mock<IPaintRepository>(), new Mock<IGlobalPaintRepository>());

        var deleted = await service.DeletePaintSchemeAsync(3, 7, false);

        Assert.True(deleted);
        schemeRepository.Verify(repo => repo.GetByIdForUserAsync(3, 7), Times.Once);
        schemeRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
        schemeRepository.Verify(repo => repo.DeleteAsync(scheme), Times.Once);
        schemeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletePaintSchemeAsync_DeniesNonOwnerWhenSchemeIsNotInUserScope()
    {
        var schemeRepository = new Mock<IPaintSchemeRepository>();
        schemeRepository.Setup(repo => repo.GetByIdForUserAsync(3, 8)).ReturnsAsync((PaintScheme?)null);
        var service = CreateService(schemeRepository, new Mock<IPaintRepository>(), new Mock<IGlobalPaintRepository>());

        var deleted = await service.DeletePaintSchemeAsync(3, 8, false);

        Assert.False(deleted);
        schemeRepository.Verify(repo => repo.DeleteAsync(It.IsAny<PaintScheme>()), Times.Never);
        schemeRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeletePaintSchemeAsync_AllowsAdminToDeleteAnyScheme()
    {
        var scheme = new PaintScheme { Id = 3, UserId = 7 };
        var schemeRepository = new Mock<IPaintSchemeRepository>();
        schemeRepository.Setup(repo => repo.GetByIdAsync(3)).ReturnsAsync(scheme);
        var service = CreateService(schemeRepository, new Mock<IPaintRepository>(), new Mock<IGlobalPaintRepository>());

        var deleted = await service.DeletePaintSchemeAsync(3, 8, true);

        Assert.True(deleted);
        schemeRepository.Verify(repo => repo.GetByIdAsync(3), Times.Once);
        schemeRepository.Verify(repo => repo.GetByIdForUserAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        schemeRepository.Verify(repo => repo.DeleteAsync(scheme), Times.Once);
    }

    private static PaintSchemeService CreateService(
        Mock<IPaintSchemeRepository> schemeRepository,
        Mock<IPaintRepository> paintRepository,
        Mock<IGlobalPaintRepository> globalPaintRepository)
    {
        return new PaintSchemeService(
            schemeRepository.Object,
            paintRepository.Object,
            globalPaintRepository.Object,
            NullLogger<PaintSchemeService>.Instance);
    }
}