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
    [Authorize]
    public class BrandController : ControllerBase
    {
        private readonly BrandService _service;
        public BrandController(BrandService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var brand = await _service.GetByIdAsync(id);
            if (brand == null) return NotFound();
            return Ok(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBrandDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Brand name is required.");
            return Ok(await _service.CreateAsync(dto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateBrandDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Brand name is required.");
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
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
    public class TaxController : ControllerBase
    {
        private readonly TaxService _service;
        public TaxController(TaxService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tax = await _service.GetByIdAsync(id);
            if (tax == null) return NotFound();
            return Ok(tax);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaxDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Tax name is required.");
            return Ok(await _service.CreateAsync(dto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateTaxDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Tax name is required.");
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                // Tax still assigned to a product
                return Conflict(ex.Message);
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductVariantController : ControllerBase
    {
        private readonly ProductVariantService _service;
        public ProductVariantController(ProductVariantService service) => _service = service;

        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId) =>
            Ok(await _service.GetByProductAsync(productId));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var variant = await _service.GetByIdAsync(id);
            if (variant == null) return NotFound();
            return Ok(variant);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductVariantDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SKU)) return BadRequest("Variant SKU is required.");
            try
            {
                return Ok(await _service.CreateAsync(dto));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateProductVariantDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SKU)) return BadRequest("Variant SKU is required.");
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
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
    public class StockTransferController : ControllerBase
    {
        private readonly StockTransferService _service;
        public StockTransferController(StockTransferService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockTransferDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest("Transfer must include at least one item.");

            try
            {
                return Ok(await _service.CreateAsync(dto));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string newStatus)
        {
            try
            {
                return Ok(await _service.UpdateStatusAsync(id, newStatus));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}