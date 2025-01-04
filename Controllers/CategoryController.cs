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

        public ICloudinaryService CloudinaryService { get; }

        public CategoryController(ICategoryRepository categoryRepository,ICloudinaryService cloudinaryService)
        {
            this.categoryRepository = categoryRepository;
            CloudinaryService = cloudinaryService;
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

                var imageUrl = await CloudinaryService.UploadImageAsync(category.ImageFile);
                category.CategoryImage = imageUrl;

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
        public async Task<IActionResult> UpdateCategory([FromForm] Category category)
        {
            try
            {
                if (category == null)
                {
                    return BadRequest(new { message = "Category cannot be null" });
                }

                if (category.ImageFile == null || category.ImageFile.Length == 0)
                {
                    return BadRequest(new { message = "Invalid image file" });
                }

                var imageUrl = await CloudinaryService.UploadImageAsync(category.ImageFile);

                category.CategoryImage = imageUrl;

                var updatedCategory = await categoryRepository.UpdateCategoryAsync(category);

                if (!updatedCategory)
                {
                    return NotFound(new { message = "Category not found" });
                }

                return Ok(new { message = "Category Updated Successfully", imageUrl });
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

               

                return Ok(categories);
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
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
