using ECommerceAPI.Models;
using System.Threading.Tasks;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Repositories.Declarations;
using System;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository productRepository;

        public IWebHostEnvironment HostEnvironment { get; }

        public ProductController(IProductRepository productRepository, IWebHostEnvironment hostEnvironment)
        {
            this.productRepository = productRepository;
            HostEnvironment = hostEnvironment;
        }


        [HttpPost("Add Products")]
        public async Task<IActionResult> CreateProduct(IFormFileCollection formcollects, Product product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest(new { message = "Product cannot be null" });
                }

                string FilePath = GetFilePath("Product");
                if (!System.IO.Directory.Exists(FilePath))
                {
                    System.IO.Directory.CreateDirectory(FilePath);
                }

                List<ProductImage> productImages = new List<ProductImage>(); // To hold ProductImage objects

                foreach (var item in formcollects)
                {
                    string ImagePath = Path.Combine(FilePath, item.FileName);

                    // Delete the existing file if it already exists
                    if (System.IO.File.Exists(ImagePath))
                    {
                        System.IO.File.Delete(ImagePath);
                    }

                    // Save the image file to disk
                    using (FileStream stream = System.IO.File.Create(ImagePath))
                    {
                        await item.CopyToAsync(stream);
                    }

                    // Create a ProductImage object for the saved image
                    var productImage = new ProductImage
                    {
                        ImageUrl = ImagePath
                    };

                    // Add the ProductImage object to the list
                    productImages.Add(productImage);
                }

                // Set the ProductImages list to the Product object
                product.ProductImages = productImages;

                var result = await productRepository.CreateProductAsync(product);

                if (!result)
                {
                    return BadRequest(new { message = "Product creation failed" });
                }

                return StatusCode(201, new { message = "Product Created Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        [NonAction]
        public string GetFilePath(string Product)
        {
            return this.HostEnvironment.WebRootPath + "\\Products" + Product;
        }


        [HttpDelete("Remove Product")]
        public async Task<IActionResult> RemoveProduct(int ProductId)
        {
            if (ProductId == 0)
            {
                return BadRequest(new { message = "product cannot be null" });
            }

            var result = await productRepository.DeleteProductAsync(ProductId);
            if (result)
            {
                return Ok(new { message = "Product Deleted Successfully" });
            }
            return StatusCode(500, new { message="Internal Server Error"});
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest(new { message = "product cannot be null" });
                }

                var updatedproduct = await productRepository.UpdateProductAsync(product);

                if (!updatedproduct)
                {
                    return NotFound(new { message = "Product not found" });
                }

                return Ok(new { message = "Product Updated Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var Products = await productRepository.GetAllProductsAsync();

                if (Products == null || Products.Count == 0)
                {
                    return NotFound(new { message = "No Products found" });
                }

                return Ok(Products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetProduct/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var Product = await productRepository.GetProductByIdAsync(id);

                if (Product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                return Ok(Product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}
