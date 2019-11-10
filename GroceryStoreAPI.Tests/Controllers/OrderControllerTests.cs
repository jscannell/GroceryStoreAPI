using GroceryStoreAPI.Entities;
using GroceryStoreAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace GroceryStoreAPI.Controllers.Tests
{
    public class OrderControllerTests
    {
        readonly IOrderRepository repository;
        readonly OrderController controller;

        public OrderControllerTests()
        {
            repository = Substitute.For<IOrderRepository>();
            controller = new OrderController(repository);
        }

        [Fact]
        public void Index()
        {
            var expected = new List<Order>();

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
            var expected = new Order { Id = 123 };

            repository.LoadById(expected.Id).Returns(expected);

            // Act
            var result = (OkObjectResult)controller.Get(expected.Id);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void FindByDate_InvalidDate_ReturnsBadRequest()
        {
            // Act
            var result = controller.FindByDate(0, 0, 0);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void FindByDate()
        {
            var expected = new List<Order>();
            var date = new DateTime(2019, 11, 9);

            repository.LoadByDate(date).Returns(expected);

            // Act
            var result = (OkObjectResult)controller.FindByDate(date.Year, date.Month, date.Day);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void FindByCustomer()
        {
            var expected = new List<Order>();
            var customerId = 123;

            repository.LoadByCustomer(customerId).Returns(expected);

            // Act
            var result = (OkObjectResult)controller.FindByCustomer(customerId);

            // Assert
            Assert.Equal(expected, result.Value);
        }
    }
}
