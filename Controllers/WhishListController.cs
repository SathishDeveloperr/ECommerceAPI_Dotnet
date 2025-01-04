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
    public class WhishListController : ControllerBase
    {
        private readonly IWhishlistRepository whishListRepository;

        public WhishListController(IWhishlistRepository whishListRepository)
        {
            this.whishListRepository = whishListRepository;
        }

        [HttpPost("AddToWhishList")]
        public async Task<ActionResult> AddToWhishList(WhishList whishList)
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
                whishList.UserId = i;

                if (whishList == null)
                {
                    return BadRequest(new { message = "Wishlist item cannot be null." });
                }

                var result = await whishListRepository.AddToWhishListAsync(whishList);

                if (result)
                {
                    return Ok(new { message = "Item added to wishlist successfully." });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to add item to wishlist." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpDelete("DeleteWhishList/{id}")]
        public async Task<IActionResult> DeleteWhishList(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest(new { message = "Incorrect WishlistId" });
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

                var result = await whishListRepository.RemoveFromWhishListAsync(id, userid);

                if (result)
                {
                    return Ok(new { message = "Wishlist item removed successfully." });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to remove wishlist item." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetWhishList")]
        public async Task<IActionResult> GetWhishList()
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

                var items = await whishListRepository.GetWhishListsByUserIdAsync(userid);

                if (items == null || items.Count == 0)
                {
                    return NotFound(new { message = "No wishlist items found" });
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
