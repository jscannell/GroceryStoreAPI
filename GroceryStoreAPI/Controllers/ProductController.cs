using GroceryStoreAPI.Entities;
using GroceryStoreAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Controllers
{
    [Route("products")]
    [ApiController]
    public class ProductController : GroceryStoreController
    {
        readonly IProductRepository _products;

        public ProductController(IProductRepository products)
        {
            _products = products;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        public IActionResult Index()
        {
            return Ok(_products.LoadAll());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        public IActionResult Get(int id)
        {
            var product = _products.LoadById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            if (id < 1)
                return BadRequest();

            if (product == null)
                return BadRequest();

            if (id != product.Id)
                return BadRequest();

            await _products.SaveAsync(product);

            return Ok(product);
        }
    }
}
