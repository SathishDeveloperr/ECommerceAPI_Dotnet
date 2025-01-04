using ECommerceAPI.Models;
using ECommerceAPI.Repositories.Declarations;
using System.Threading.Tasks;
using System;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromForm] Category category)
        {
            try
            {
                if (category == null)
                {
                    return BadRequest(new { message = "Category cannot be null" });
                }

                if (category.ImageFile != null)
                {
                    // Use Render's persistent storage location
                    var folderPath = Path.Combine("/mnt/data", "CategoryImages");

                    // Ensure the directory exists
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(category.ImageFile.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await category.ImageFile.CopyToAsync(stream);
                    }

                    // Set the image URL to access the file
                    category.CategoryImage = "/files/CategoryImages/" + fileName;
                }

                var result = await categoryRepository.CreateCategoryAsync(category);

                if (!result)
                {
                    return BadRequest(new { message = "Category creation failed" });
                }

                return StatusCode(201, new { message = "Category Created Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromForm]Category category)
        {
            try
            {
                if (category == null)
                {
                    return BadRequest(new { message = "Category cannot be null" });
                }

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CategoryImages");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(category.ImageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await category.ImageFile.CopyToAsync(stream);
                }

                category.CategoryImage = "/CategoryImages/" + fileName;


                var updatedCategory = await categoryRepository.UpdateCategoryAsync(category);

                if (!updatedCategory)
                {
                    return NotFound(new { message = "Category not found" });
                }

                return Ok(new { message = "Category Updated Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await categoryRepository.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                await categoryRepository.DeleteCategoryAsync(id);
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
                var categories = await categoryRepository.GetAllCategoriesAsync();

                if (categories == null || categories.Count == 0)
                {
                    return NotFound(new { message = "No categories found" });
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}/CategoryImages/";

                var categoryDtos = categories.Select(category => new
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    CategoryImageUrl = string.IsNullOrEmpty(category.CategoryImage)
                        ? null
                        : baseUrl + Path.GetFileName(category.CategoryImage)
                }).ToList();

                return Ok(categoryDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await categoryRepository.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    return NotFound(new { message = "No category found" });
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}/CategoryImages/";

                var categoryDto = new
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    CategoryImageUrl = string.IsNullOrEmpty(category.CategoryImage)
                        ? null
                        : baseUrl + Path.GetFileName(category.CategoryImage)
                };

                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
