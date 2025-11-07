using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuApi.Data;
using MottuApi.Models;

namespace MottuApi.Controllers
{
    /// <summary>CRUD de Motos.</summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/motos")]
    public class MotosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MotosController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _db.Motos
                                 .AsNoTracking()
                                 .Include(m => m.Patio)   // se existir a navegação
                                 .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var moto = await _db.Motos
                                .Include(m => m.Patio)
                                .FirstOrDefaultAsync(m => m.Id == id);

            if (moto == null) return NotFound();
            return Ok(moto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Moto model)
        {
            _db.Motos.Add(model);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = model.Id, version = "1.0" },
                model
            );
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] Moto model)
        {
            var exists = await _db.Motos.AnyAsync(m => m.Id == id);
            if (!exists) return NotFound();

            model.Id = id;
            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var moto = await _db.Motos.FindAsync(id);
            if (moto == null) return NotFound();

            _db.Motos.Remove(moto);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
