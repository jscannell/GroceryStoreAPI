using GroceryStoreAPI.Entities;
using GroceryStoreAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Controllers
{
    [Route("customers")]
    public class CustomerController : GroceryStoreController
    {
        readonly ICustomerRepository _customers;

        public CustomerController(ICustomerRepository customers)
        {
            _customers = customers;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Customer>))]
        public IActionResult Index()
        {
            return Ok(_customers.LoadAll());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Customer))]
        public IActionResult Get(int id)
        {
            var customer = _customers.LoadById(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Customer))]
        public async Task<IActionResult> Update(int id, [FromBody] Customer customer)
        {
            if (id < 1)
                return BadRequest();

            if (customer == null)
                return BadRequest();

            if (id != customer.Id)
                return BadRequest();

            await _customers.SaveAsync(customer);

            return Ok(customer);
        }
    }
}
