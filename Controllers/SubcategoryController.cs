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

        public SubcategoryController(ISubcategoryRepository subcategoryRepository)
        {
            _subcategoryRepository = subcategoryRepository;
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

            var baseUrl = $"{Request.Scheme}://{Request.Host}/SubCategoryImages/";

            var categoryDtos = subcategories.Select(category => new
            {
                SubCategoryId = category.SubCategoryId,
                SubCategoryName = category.SubCategoryName,
                SubCategoryImageUrl = string.IsNullOrEmpty(category.SubCategoryImage)
                    ? null
                    : baseUrl + Path.GetFileName(category.SubCategoryImage)
            }).ToList();

            return Ok(categoryDtos);

        }

        // GET: api/Subcategory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubCategory>> GetSubcategoryById(int id)
        {
            var category = await _subcategoryRepository.GetSubcategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { message = "No subcategories found" });
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}/SubCategoryImages/";

            var categoryDtos = new
            {
                SubCategoryId = category.SubCategoryId,
                SubCategoryName = category.SubCategoryName,
                SubCategoryImageUrl = string.IsNullOrEmpty(category.SubCategoryImage)
                    ? null
                    : baseUrl + Path.GetFileName(category.SubCategoryImage)


            };

            return Ok(categoryDtos);
        }

        [HttpPost]
        public async Task<ActionResult<SubCategory>> CreateSubcategory(SubCategory subcategory)
        {

            if (subcategory == null)
            {
                return BadRequest(new { message = "Subcategory cannot be null" });
            }

            if (subcategory.ImageFile != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SubCategoryImages");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(subcategory.ImageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await subcategory.ImageFile.CopyToAsync(stream);
                }
                subcategory.SubCategoryImage = "/SubCategoryImages/" + fileName;
            }

            var result = await _subcategoryRepository.CreateSubcategoryAsync(subcategory);
            if (result)
            {
                return CreatedAtAction(nameof(GetSubcategoryById), new { id = subcategory.SubCategoryId }, subcategory);
            }
            return BadRequest("Failed to create subcategory.");
        }

        // PUT: api/Subcategory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubcategory(int id, SubCategory subcategory)
        {
            if (id != subcategory.SubCategoryId)
            {
                return BadRequest("Subcategory ID mismatch.");
            }

            if (subcategory.ImageFile != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SubCategoryImages");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(subcategory.ImageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await subcategory.ImageFile.CopyToAsync(stream);
                }
                subcategory.SubCategoryImage = "/SubCategoryImages/" + fileName;
            }

            var result = await _subcategoryRepository.UpdateSubcategoryAsync(subcategory);
            if (result)
            {
                return NoContent();
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
                return NoContent();
            }
            return NotFound();
        }
    }
}
