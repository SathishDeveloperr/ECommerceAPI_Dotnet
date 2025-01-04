using ECommerceAPI.Models;
using ECommerceAPI.Repositories.Declarations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.IO;
using System.Net;


namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IConfiguration configuration;

        public UsersController(IUserRepository userRepository, IConfiguration configuration)
        {
            this.userRepository = userRepository;
            this.configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Users user)
        {
            try
            {
                var result = await userRepository.CreateUserAsync(user);
                return Content("User Created Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm]Users user)
        {
            try
            {
                var userIdCookie = HttpContext.Request.Cookies["AuthToken"];
                if (string.IsNullOrEmpty(userIdCookie))
                {
                    return BadRequest(new { message = "User ID cookie is missing" });
                }

                var userId = GetUserIdFromJwtToken(userIdCookie);

                if (userId == null)
                {
                    return BadRequest(new { message = "Invalid JWT token or UserId" });
                }

                if (user.ImageFile != null)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UserImages");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(user.ImageFile.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await user.ImageFile.CopyToAsync(stream);
                    }

                    user.ProfilePicture = "/UserImages/" + fileName;
                }


                int i = Convert.ToInt32(userId);
                user.UserId = i;
                var result = await userRepository.UpdateUserAsync(user);
                return StatusCode(200, new { message = "Updated Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete()
        {
            try
            {
                var userIdCookie = HttpContext.Request.Cookies["AuthToken"];
                if (string.IsNullOrEmpty(userIdCookie))
                {
                    return BadRequest(new { message = "User ID cookie is missing" });
                }

                var userId = GetUserIdFromJwtToken(userIdCookie);

                if (userId == null)
                {
                    return BadRequest(new { message = "Invalid JWT token or UserId" });
                }
                int i = Convert.ToInt32(userId);

                var success = await userRepository.DeleteUserAsync(i);
                if (!success) return NotFound(new { message = "User not found" });
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("get")]
        public async Task<IActionResult> GetUserByIdFromCookie()
        {
            try
            {
                var userIdCookie = HttpContext.Request.Cookies["AuthToken"];

                if (string.IsNullOrEmpty(userIdCookie))
                {
                    return BadRequest(new { message = "User ID cookie is missing" });
                }

                var userId = GetUserIdFromJwtToken(userIdCookie);

                if (userId == null)
                {
                    return BadRequest(new { message = "Invalid JWT token or UserId" });
                }

                var user = await userRepository.GetUserByIdAsync(userId.Value);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}/UserImages/";

                var UserDto = new
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    EmailId = user.EmailId,
                    Address = user.Address,
                    profilePicture = string.IsNullOrEmpty(user.ProfilePicture)
                        ? null
                        : baseUrl + Path.GetFileName(user.ProfilePicture)
                };


                return Ok(UserDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Response.Cookies.Append("AuthToken", "", new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(-1), // Expire the cookie
                    HttpOnly = true, // Prevent client-side access to the cookie
                    Secure = true, // Ensure the cookie is sent over HTTPS
                    SameSite = SameSiteMode.Strict
                });

                return Ok(new { message = "Successfully logged out" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(string EmailId, string Password)
        {
            try
            {
                var user = await userRepository.ValidateUserLoginAsync(EmailId, Password);

                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                var token = GenerateJwtToken(user);

                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1)
                });

                return Ok(new { message = "Login successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        private string GenerateJwtToken(ECommerceAPI.Models.Users user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.EmailId),
        new Claim("UserName", user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int? GetUserIdFromJwtToken(string jwtToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

                if (jsonToken == null)
                {
                    return null;
                }

                var userIdClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "sub");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }


    }
}

