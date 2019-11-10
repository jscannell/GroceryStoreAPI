using GroceryStoreAPI.Entities;
using GroceryStoreAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GroceryStoreAPI.Controllers.Tests
{
    public class ProductControllerTests
    {
        readonly IProductRepository repository;
        readonly ProductController controller;

        public ProductControllerTests()
        {
            repository = Substitute.For<IProductRepository>();
            controller = new ProductController(repository);
        }

        [Fact]
        public void Index()
        {
            var expected = new List<Product>();

            repository.LoadAll().Returns(expected);

            // Act
            var result = (OkObjectResult)controller.Index();

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void Get_NotFound_ReturnsNotFoundResult()
        {
            // Act
            var result = controller.Get(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Get()
        {
            var expected = new Product { Id = 123 };

            repository.LoadById(expected.Id).Returns(expected);

            // Act
            var result = (OkObjectResult)controller.Get(expected.Id);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public async Task Update_IdLessThanOne_ReturnsBadRequest()
        {
            // Act
            var result = await controller.Update(0, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_ProductIsNull_ReturnsBadRequest()
        {
            // Act
            var result = await controller.Update(1, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_IdMismatch_ReturnsBadRequest()
        {
            var expected = new Product { Id = 123 };

            // Act
            var result = await controller.Update(expected.Id + 1, expected);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_SavesProduct()
        {
            var expected = new Product { Id = 123 };

            // Act
            _ = await controller.Update(expected.Id, expected);

            // Assert
            await repository.Received().SaveAsync(expected);
        }

        [Fact]
        public async Task Update_ReturnsProduct()
        {
            var expected = new Product { Id = 123 };

            // Act
            var result = (OkObjectResult)await controller.Update(expected.Id, expected);

            // Assert
            Assert.Equal(expected, result.Value);
        }
    }
}
