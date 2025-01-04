using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories.Declarations;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubcategoryController : ControllerBase
    {
        private readonly ISubcategoryRepository _subcategoryRepository;

        public ICloudinaryService CloudinaryService { get; }

        public SubcategoryController(ISubcategoryRepository subcategoryRepository, ICloudinaryService cloudinaryService)
        {
            _subcategoryRepository = subcategoryRepository;
            CloudinaryService = cloudinaryService;
        }

        // GET: api/Subcategory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubCategory>>> GetAllSubcategories()
        {

            var subcategories = await _subcategoryRepository.GetAllSubcategoriesAsync();

            if (subcategories == null || subcategories.Count == 0)
            {
                return NotFound(new { message = "No subcategories found" });
            }

            

            return Ok(subcategories);

        }

        // GET: api/Subcategory/5
        [HttpGet("GetSubCatgoryId")]
        public async Task<ActionResult<SubCategory>> GetSubcategoryById(int id)
        {
            var category = await _subcategoryRepository.GetSubcategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { message = "No subcategories found" });
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<SubCategory>> CreateSubcategory([FromForm]SubCategory subcategory)
        {

            if (subcategory == null)
            {
                return BadRequest(new { message = "Subcategory cannot be null" });
            }

            var imageUrl = await CloudinaryService.UploadImageAsync(subcategory.ImageFile);
            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest(new { message = "Image upload failed." });
            }

            subcategory.SubCategoryImage = imageUrl;

            var result = await _subcategoryRepository.CreateSubcategoryAsync(subcategory);
            if (result)
            {
                return StatusCode(201, new {Status=200 ,message="Sub Category Successfully" });
            }
            return BadRequest("Failed to create subcategory.");
        }

        // PUT: api/Subcategory/5
        [HttpPut("Update SubCategory")]
        public async Task<IActionResult> UpdateSubcategory([FromForm]SubCategory subcategory)
        {

            var imageUrl = await CloudinaryService.UploadImageAsync(subcategory.ImageFile);
            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest(new { message = "Image upload failed." });
            }

            subcategory.SubCategoryImage = imageUrl;

            var result = await _subcategoryRepository.UpdateSubcategoryAsync(subcategory);
            if (result)
            {
                return StatusCode(200, new { Status = 200, message = "Updated Successfully" });
            }
            return NotFound();
        }

        // DELETE: api/Subcategory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubcategory(int id)
        {
            var result = await _subcategoryRepository.DeleteSubcategoryAsync(id);
            if (result)
            {
                return StatusCode(200, new { Status = 200, message = "Deleted Successfully" });
            }
            return NotFound();
        }
    }
}
