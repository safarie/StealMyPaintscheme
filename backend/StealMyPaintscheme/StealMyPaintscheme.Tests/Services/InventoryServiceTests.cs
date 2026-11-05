using Moq;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;
using StealMyPaintscheme.Api.Services;
using Xunit;

namespace StealMyPaintscheme.Tests.Services;

public class InventoryServiceTests
{
    [Fact]
    public async Task AddOrUpdateInventoryItemAsync_ForNewItemAssignsOwnerAndSaves()
    {
        var repository = new Mock<IInventoryRepository>();
        repository.Setup(repo => repo.GetByUserAndPaintAsync(7, 12)).ReturnsAsync((InventoryItem?)null);
        var service = new InventoryService(repository.Object);
        var item = new InventoryItem { PaintId = 12, Quantity = 3 };

        var result = await service.AddOrUpdateInventoryItemAsync(item, 7);

        Assert.Same(item, result);
        Assert.Equal(7, item.UserId);
        Assert.Equal(12, item.PaintId);
        Assert.Equal(3, item.Quantity);
        repository.Verify(repo => repo.AddAsync(item), Times.Once);
        repository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateInventoryItemAsync_ForExistingItemAddsQuantity()
    {
        var existingItem = new InventoryItem { Id = 2, UserId = 7, PaintId = 12, Quantity = 4 };
        var repository = new Mock<IInventoryRepository>();
        repository.Setup(repo => repo.GetByUserAndPaintAsync(7, 12)).ReturnsAsync(existingItem);
        var service = new InventoryService(repository.Object);

        var result = await service.AddOrUpdateInventoryItemAsync(new InventoryItem { PaintId = 12, Quantity = 3 }, 7);

        Assert.Same(existingItem, result);
        Assert.Equal(7, existingItem.UserId);
        Assert.Equal(12, existingItem.PaintId);
        Assert.Equal(7, existingItem.Quantity);
        repository.Verify(repo => repo.AddAsync(It.IsAny<InventoryItem>()), Times.Never);
        repository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteInventoryItemAsync_AllowsOwnerToDelete()
    {
        var item = new InventoryItem { Id = 2, UserId = 7 };
        var repository = new Mock<IInventoryRepository>();
        repository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(item);
        var service = new InventoryService(repository.Object);

        var deleted = await service.DeleteInventoryItemAsync(2, 7);

        Assert.True(deleted);
        repository.Verify(repo => repo.DeleteAsync(item), Times.Once);
        repository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteInventoryItemAsync_DeniesOtherUser()
    {
        var item = new InventoryItem { Id = 2, UserId = 7 };
        var repository = new Mock<IInventoryRepository>();
        repository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(item);
        var service = new InventoryService(repository.Object);

        var deleted = await service.DeleteInventoryItemAsync(2, 8);

        Assert.False(deleted);
        repository.Verify(repo => repo.DeleteAsync(It.IsAny<InventoryItem>()), Times.Never);
        repository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateInventoryItemQuantityAsync_AllowsOwnerToUpdateQuantity()
    {
        var item = new InventoryItem { Id = 2, UserId = 7, Quantity = 1 };
        var repository = new Mock<IInventoryRepository>();
        repository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(item);
        var service = new InventoryService(repository.Object);

        var result = await service.UpdateInventoryItemQuantityAsync(2, 5, 7);

        Assert.Same(item, result);
        Assert.Equal(5, item.Quantity);
        repository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateInventoryItemQuantityAsync_DeniesOtherUser()
    {
        var item = new InventoryItem { Id = 2, UserId = 7, Quantity = 1 };
        var repository = new Mock<IInventoryRepository>();
        repository.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(item);
        var service = new InventoryService(repository.Object);

        var result = await service.UpdateInventoryItemQuantityAsync(2, 5, 8);

        Assert.Null(result);
        Assert.Equal(1, item.Quantity);
        repository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}