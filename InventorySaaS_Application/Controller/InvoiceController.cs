using InventorySaaS_Application.Application.DTOs;
using InventorySaaS_Application.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InventorySaaS_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _service;

        public InvoiceController(InvoiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var invoice = await _service.GetByIdAsync(id);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        // Existing Sales Order se invoice generate karta hai
        [HttpPost("from-sales-order")]
        public async Task<IActionResult> CreateFromSalesOrder([FromBody] CreateInvoiceFromSalesOrderDto dto)
        {
            try
            {
                var created = await _service.CreateFromSalesOrderAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.InvoiceId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}