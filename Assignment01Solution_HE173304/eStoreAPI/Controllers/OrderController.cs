using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace eStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public OrderController(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        // GET: api/Order
        [HttpGet]
        public IActionResult GetAll()
        {
            var orderRepository = _repositoryFactory.GetOrderRepository();
            var orders = orderRepository.GetAll();
            return Ok(orders);
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var orderRepository = _repositoryFactory.GetOrderRepository();
            var order = orderRepository.GetById(id);
            
            if (order == null)
            {
                return NotFound();
            }
            
            return Ok(order);
        }

        // GET: api/Order/member/5
        [HttpGet("member/{memberId}")]
        public IActionResult GetOrdersByMember(int memberId)
        {
            var orderRepository = _repositoryFactory.GetOrderRepository();
            var orders = orderRepository.GetOrdersByMember(memberId);
            return Ok(orders);
        }

        // GET: api/Order/daterange?startDate=2025-05-01&endDate=2025-05-31
        [HttpGet("daterange")]
        public IActionResult GetOrdersByDateRange(string startDate, string endDate)
        {
            if (!DateOnly.TryParse(startDate, out var start) || !DateOnly.TryParse(endDate, out var end))
            {
                return BadRequest("Invalid date format. Use YYYY-MM-DD format.");
            }
            
            var orderRepository = _repositoryFactory.GetOrderRepository();
            var orders = orderRepository.GetOrdersByDateRange(start, end);
            return Ok(orders);
        }

        // GET: api/Order/5/details
        [HttpGet("{id}/details")]
        public IActionResult GetOrderDetails(int id)
        {
            var orderRepository = _repositoryFactory.GetOrderRepository();
            var order = orderRepository.GetById(id);
            
            if (order == null)
            {
                return NotFound();
            }
            
            var orderDetailRepository = _repositoryFactory.GetOrderDetailRepository();
            var details = orderDetailRepository.GetOrderDetailsByOrder(id);
            
            return Ok(details);
        }

        // POST: api/Order
        [HttpPost]
        public IActionResult Create(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var orderRepository = _repositoryFactory.GetOrderRepository();
                
                // Set the order date if not provided
                if (order.OrderDate == null)
                {
                    order.OrderDate = DateOnly.FromDateTime(DateTime.Today);
                }
                
                // Get the next available order ID if not provided
                if (order.OrderId <= 0)
                {
                    int nextOrderId = orderRepository.GetAll().Any() 
                        ? orderRepository.GetAll().Max(o => o.OrderId) + 1 
                        : 1;
                    order.OrderId = nextOrderId;
                }
                
                orderRepository.Add(order);
                return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Order/5/details
        [HttpPost("{orderId}/details")]
        public IActionResult AddOrderDetail(int orderId, OrderDetail orderDetail)
        {
            if (orderId != orderDetail.OrderId)
            {
                return BadRequest("Order ID mismatch");
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var orderRepository = _repositoryFactory.GetOrderRepository();
                if (!orderRepository.Exists(orderId))
                {
                    return NotFound("Order not found");
                }
                
                var productRepository = _repositoryFactory.GetProductRepository();
                var product = productRepository.GetById(orderDetail.ProductId);
                
                if (product == null)
                {
                    return NotFound("Product not found");
                }
                
                // Check if there's enough stock
                if (product.UnitsInStock < orderDetail.Quantity)
                {
                    return BadRequest($"Product {product.ProductName} only has {product.UnitsInStock} units in stock");
                }
                
                var orderDetailRepository = _repositoryFactory.GetOrderDetailRepository();
                orderDetailRepository.Add(orderDetail);
                
                // Update product stock
                product.UnitsInStock -= orderDetail.Quantity;
                productRepository.Update(product);
                
                return CreatedAtAction(nameof(GetOrderDetails), new { id = orderDetail.OrderId }, orderDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest("ID mismatch");
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var orderRepository = _repositoryFactory.GetOrderRepository();
                
                if (!orderRepository.Exists(id))
                {
                    return NotFound();
                }
                
                orderRepository.Update(order);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var orderRepository = _repositoryFactory.GetOrderRepository();
                
                if (!orderRepository.Exists(id))
                {
                    return NotFound();
                }
                
                var orderDetailRepository = _repositoryFactory.GetOrderDetailRepository();
                var details = orderDetailRepository.GetOrderDetailsByOrder(id).ToList();
                
                // Return products to inventory
                var productRepository = _repositoryFactory.GetProductRepository();
                foreach (var detail in details)
                {
                    var product = productRepository.GetById(detail.ProductId);
                    if (product != null)
                    {
                        product.UnitsInStock += detail.Quantity;
                        productRepository.Update(product);
                    }
                }
                
                // Remove order (cascade will handle order details)
                orderRepository.Remove(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}