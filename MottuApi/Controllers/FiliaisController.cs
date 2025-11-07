using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using MottuApi.Data;
using MottuApi.Models;
using MottuApi.Utils;

namespace MottuApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/filiais")]
public class FiliaisController : ControllerBase
{
    private readonly AppDbContext _db;
    public FiliaisController(AppDbContext db) => _db = db;

    // GET /api/v1/filiais?pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = _db.Filiais.AsNoTracking().OrderBy(f => f.Id);

        var total = await query.CountAsync();
        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // cada item com HATEOAS
        var dataWithLinks = data.Select(f => new
        {
            item = f,
            _links = new List<Link>
            {
                new("self",   Url.Action(nameof(GetById), values: new { version = "1", id = f.Id })!, "GET"),
                new("update", Url.Action(nameof(Update),  values: new { version = "1", id = f.Id })!, "PUT"),
                new("delete", Url.Action(nameof(Delete),  values: new { version = "1", id = f.Id })!, "DELETE")
            }
        });

        var links = BuildCollectionLinks(pageNumber, pageSize, total);

        var result = new
        {
            data = dataWithLinks,
            total,
            pageNumber,
            pageSize,
            _links = links
        };

        // header de paginação (não é obrigatório pro teste, mas é bonito 😎)
        Response.Headers["X-Pagination"] =
            System.Text.Json.JsonSerializer.Serialize(new { total, pageNumber, pageSize });

        return Ok(result);
    }

    // GET /api/v1/filiais/3
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var filial = await _db.Filiais.FindAsync(id);
        if (filial is null)
            return NotFound();

        return Ok(new
        {
            item = filial,
            _links = new List<Link>
            {
                new("self",   Url.Action(nameof(GetById), values: new { version = "1", id })!, "GET"),
                new("update", Url.Action(nameof(Update),  values: new { version = "1", id })!, "PUT"),
                new("delete", Url.Action(nameof(Delete),  values: new { version = "1", id })!, "DELETE")
            }
        });
    }

    // POST /api/v1/filiais
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Filial model)
    {
        _db.Filiais.Add(model);
        await _db.SaveChangesAsync();

        var response = new
        {
            item = model,
            _links = new List<Link>
            {
                new("self",   Url.Action(nameof(GetById), values: new { version = "1", id = model.Id })!, "GET"),
                new("update", Url.Action(nameof(Update),  values: new { version = "1", id = model.Id })!, "PUT"),
                new("delete", Url.Action(nameof(Delete),  values: new { version = "1", id = model.Id })!, "DELETE")
            }
        };

        return CreatedAtAction(nameof(GetById), new { id = model.Id, version = "1" }, response);
    }

    // PUT /api/v1/filiais/3
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] Filial model)
    {
        var exists = await _db.Filiais.AnyAsync(x => x.Id == id);
        if (!exists)
            return NotFound();

        model.Id = id;
        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/v1/filiais/3
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var filial = await _db.Filiais.FindAsync(id);
        if (filial is null)
            return NotFound();

        _db.Filiais.Remove(filial);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private IEnumerable<Link> BuildCollectionLinks(int pageNumber, int pageSize, int total)
    {
        var list = new List<Link>
        {
            new("self",   Url.Action(nameof(GetAll), values: new { version = "1", pageNumber, pageSize })!, "GET"),
            new("create", Url.Action(nameof(Create),  values: new { version = "1" })!, "POST")
        };

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        if (pageNumber > 1)
        {
            list.Add(new("prev", Url.Action(nameof(GetAll), values: new { version = "1", pageNumber = pageNumber - 1, pageSize })!, "GET"));
        }

        if (pageNumber < totalPages)
        {
            list.Add(new("next", Url.Action(nameof(GetAll), values: new { version = "1", pageNumber = pageNumber + 1, pageSize })!, "GET"));
        }

        return list;
    }
}
