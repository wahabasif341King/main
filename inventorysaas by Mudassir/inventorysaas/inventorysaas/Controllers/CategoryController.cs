using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventorySaaS_Application.Application.DTOs;
using InventorySaaS_Application.Application.Services;

namespace InventorySaaS_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize] // requires a valid JWT once your mate's auth is wired in
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _service;

        public CategoryController(CategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _service.GetAllAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Category name is required.");

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = created.CategoryId }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.SKU))
                return BadRequest("Name and SKU are required.");

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
            }
            catch (InvalidOperationException ex)
            {
                // e.g. duplicate SKU
                return Conflict(ex.Message);
            }
        }
    }
}