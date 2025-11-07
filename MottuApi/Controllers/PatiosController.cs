using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuApi.Data;
using MottuApi.Models;

namespace MottuApi.Controllers
{
    /// <summary>CRUD de Pátios.</summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/patios")]
    public class PatiosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PatiosController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _db.Patios
                                 .AsNoTracking()
                                 .Include(p => p.Filial)   // se tiver navegação
                                 .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var patio = await _db.Patios
                                 .Include(p => p.Filial)
                                 .FirstOrDefaultAsync(p => p.Id == id);

            if (patio == null) return NotFound();
            return Ok(patio);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Patio model)
        {
            _db.Patios.Add(model);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = model.Id, version = "1.0" },
                model
            );
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] Patio model)
        {
            var exists = await _db.Patios.AnyAsync(p => p.Id == id);
            if (!exists) return NotFound();

            model.Id = id;
            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var patio = await _db.Patios.FindAsync(id);
            if (patio == null) return NotFound();

            _db.Patios.Remove(patio);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
