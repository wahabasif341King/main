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
   // [Authorize]
    public class WarehouseController : ControllerBase
    {
        private readonly WarehouseService _service;

        public WarehouseController(WarehouseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var warehouses = await _service.GetAllAsync();
            return Ok(warehouses);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Warehouse name is required.");

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = created.WarehouseId }, created);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _service;

        public InventoryController(InventoryService service)
        {
            _service = service;
        }

        // GET /api/inventory                -> all stock
        // GET /api/inventory?warehouseId=... -> stock for one warehouse
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? warehouseId)
        {
            var inventory = await _service.GetAllAsync(warehouseId);
            return Ok(inventory);
        }

        // Used both for initial opening stock and for periodic physical-count
        // reconciliation. Goes through StockAdjustment + StockMovement internally.
        [HttpPost("adjust")]
        public async Task<IActionResult> AdjustStock([FromBody] StockAdjustmentRequestDto dto)
        {
            if (dto.PhysicalQuantity < 0)
                return BadRequest("Physical quantity cannot be negative.");

            var result = await _service.AdjustStockAsync(dto);
            return Ok(result);
        }
    }
}