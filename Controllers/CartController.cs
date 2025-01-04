using System;
using System.Threading.Tasks;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories.Declarations;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
        }

        [HttpPost("AddToCart")]
        public async Task<ActionResult> AddCart(Cart cart)
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
                int i = Convert.ToInt32(userId);
                cart.UserId = i;

                if (cart == null)
                {
                    return BadRequest(new { message = "Cart cannot be null." });
                }
 
                var result = await cartRepository.AddToCartAsync(cart);

                if (result)
                {
                    return Ok(new { message = "Item added to cart successfully." });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to add item to cart." });
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {

                if (id == 0)
                {
                    return BadRequest(new { message = "Incorrect ProductId" });
                }

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
                int userid = Convert.ToInt32(userId);

                var category =  await cartRepository.RemoveFromCartAsync(id,userid);

                return Ok(new { message = "Category Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCartList()
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
                int userid = Convert.ToInt32(userId);

                var categories = await cartRepository.GetCartsByUserIdAsync(userid);


                if (categories == null || categories.Count == 0)
                {
                    return NotFound(new { message = "No categories found" });
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}
