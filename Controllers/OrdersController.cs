using ECommerceAPI.Models;
using System.Threading.Tasks;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Repositories.Declarations;
using System;
using ECommerceAPI.Helpers;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersRepository ordersRepository;

        public OrdersController(IOrdersRepository ordersRepository)
        {
            this.ordersRepository = ordersRepository;
        }

        [HttpPost("CreateOrders")]
        public async Task<ActionResult> CreateOrdersAsync(Orders Orders)
        {
            try
            {
                if (Orders == null)
                {
                    return BadRequest(new { message = "Order cannot be null" });
                }

                var result = await ordersRepository.CreateOrdersAsync(Orders);

                if (!result)
                {
                    return BadRequest(new { message = "Orders creation failed" });
                }

                return StatusCode(201, new { message = "Orders Created Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteOrder/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                
                await ordersRepository.DeleteOrderAsync(id);
                return Ok(new { message = "Category Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetCategoryList()
        {
            try
            {
                var orders = await ordersRepository.GetAllOrdersAsync();

                if (orders == null || orders.Count == 0)
                {
                    return NotFound(new { message = "No categories found" });
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetOrderId")]
        public async Task<IActionResult> GetCategoryById()
        {
            try
            {
                var userIdCookie = HttpContext.Request.Cookies["AuthToken"];
                if (string.IsNullOrEmpty(userIdCookie))
                {
                    return BadRequest(new { message = "User ID cookie is missing" });
                }

                var userId = GetUserId.GetUserIdFromJwtToken(userIdCookie);

                if (userId == null)
                {
                    return BadRequest(new { message = "Invalid JWT token or UserId" });
                }
                int useriid = Convert.ToInt32(userId);

                var order = await ordersRepository.GetOrdersByIdAsync(useriid);

                if (order == null)
                {
                    return NotFound(new { message = "order not found" });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
