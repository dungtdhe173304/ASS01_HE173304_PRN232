using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace eStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public ProductController(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        // GET: api/Product
        [HttpGet]
        public IActionResult GetAll()
        {
            var productRepository = _repositoryFactory.GetProductRepository();
            var products = productRepository.GetAll();
            return Ok(products);
        }

        // GET: api/Product/instock
        [HttpGet("instock")]
        public IActionResult GetProductsInStock()
        {
            var productRepository = _repositoryFactory.GetProductRepository();
            var products = productRepository.GetProductsInStock();
            return Ok(products);
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var productRepository = _repositoryFactory.GetProductRepository();
            var product = productRepository.GetById(id);
            
            if (product == null)
            {
                return NotFound();
            }
            
            return Ok(product);
        }

        // GET: api/Product/category/5
        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            var productRepository = _repositoryFactory.GetProductRepository();
            var products = productRepository.GetProductsByCategory(categoryId);
            return Ok(products);
        }

        // GET: api/Product/search?term=keyword
        [HttpGet("search")]
        public IActionResult Search(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return BadRequest("Search term is required");
            }

            var productRepository = _repositoryFactory.GetProductRepository();
            var products = productRepository.SearchProducts(term);
            return Ok(products);
        }

        // GET: api/Product/lowstock
        [HttpGet("lowstock")]
        public IActionResult GetLowStockProducts()
        {
            var productRepository = _repositoryFactory.GetProductRepository();
            var products = productRepository.GetAll()
                .Where(p => p.UnitsInStock < 10)
                .OrderBy(p => p.UnitsInStock);
            return Ok(products);
        }

        // POST: api/Product
        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var productRepository = _repositoryFactory.GetProductRepository();
                
                if (productRepository.Exists(product.ProductId))
                {
                    return Conflict("A product with this ID already exists");
                }
                
                productRepository.Add(product);
                return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest("ID mismatch");
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var productRepository = _repositoryFactory.GetProductRepository();
                
                if (!productRepository.Exists(id))
                {
                    return NotFound();
                }
                
                productRepository.Update(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var productRepository = _repositoryFactory.GetProductRepository();
                var orderDetailRepository = _repositoryFactory.GetOrderDetailRepository();
                
                if (!productRepository.Exists(id))
                {
                    return NotFound();
                }
                
                // Check if product is used in any orders
                var orderDetails = orderDetailRepository.GetOrderDetailsByProduct(id);
                
                if (orderDetails.Any())
                {
                    return BadRequest("This product cannot be deleted because it is referenced by existing orders");
                }
                
                productRepository.Remove(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}