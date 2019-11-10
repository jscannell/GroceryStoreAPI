using GroceryStoreAPI.Entities;
using GroceryStoreAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace GroceryStoreAPI.Controllers
{
    [Produces("application/json")]
    [Route("orders")]
    [ApiController]
    public class OrderController : GroceryStoreController
    {
        readonly IOrderRepository _orders;

        public OrderController(IOrderRepository orders)
        {
            _orders = orders;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Order>))]
        public IActionResult Index()
        {
            return Ok(_orders.LoadAll());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
        public IActionResult Get(int id)
        {
            var order = _orders.LoadById(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet("{year}/{month}/{day}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Order>))]
        public IActionResult FindByDate(int year, int month, int day)
        {
            DateTime date;

            try
            {
                date = new DateTime(year, month, day);
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest();
            }

            var orders = _orders.LoadByDate(date);

            return Ok(orders);
        }
        
        [Route("/api/customers/{id}/orders")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Order>))]
        public IActionResult FindByCustomer(int id)
        {
            var orders = _orders.LoadByCustomer(id);

            return Ok(orders);
        }
    }
}
