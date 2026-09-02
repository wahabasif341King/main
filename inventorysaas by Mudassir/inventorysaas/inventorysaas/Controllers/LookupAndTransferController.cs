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
    // [Authorize]  // uncomment once JWT auth is wired in
    public class BrandController : ControllerBase
    {
        private readonly BrandService _service;
        public BrandController(BrandService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBrandDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Brand name is required.");
            return Ok(await _service.CreateAsync(dto));
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class TaxController : ControllerBase
    {
        private readonly TaxService _service;
        public TaxController(TaxService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaxDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Tax name is required.");
            return Ok(await _service.CreateAsync(dto));
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class ProductVariantController : ControllerBase
    {
        private readonly ProductVariantService _service;
        public ProductVariantController(ProductVariantService service) => _service = service;

        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId) =>
            Ok(await _service.GetByProductAsync(productId));

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
    }

    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
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