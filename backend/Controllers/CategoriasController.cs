using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceiroApi.Data;
using FinanceiroApi.DTOs;
using FinanceiroApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FinanceiroApi.Controllers;

[ApiController]
[Route("api/categorias")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _db;
    public CategoriasController(AppDbContext db) => _db = db;

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private static CategoriaResponse ToDto(Categoria c) =>
        new(c.Id, c.UsuarioId, c.Nome, c.Tipo, c.Cor, c.Icone);

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? tipo)
    {
        var uid = GetUserId();
        var query = _db.Categorias.AsNoTracking().Where(c => c.UsuarioId == uid);
        if (!string.IsNullOrEmpty(tipo))
            query = query.Where(c => c.Tipo == tipo || c.Tipo == "Ambos");

        return Ok(await query.Select(c => ToDto(c)).ToListAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var uid = GetUserId();
        var c = await _db.Categorias.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == uid);
        return c is null ? NotFound() : Ok(ToDto(c));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CategoriaRequest req)
    {
        var uid = GetUserId();
        var cat = new Categoria { UsuarioId = uid, Nome = req.Nome, Tipo = req.Tipo, Cor = req.Cor, Icone = req.Icone };
        _db.Categorias.Add(cat);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obter), new { id = cat.Id }, ToDto(cat));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] CategoriaRequest req)
    {
        var uid = GetUserId();
        var cat = await _db.Categorias.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == uid);
        if (cat is null) return NotFound();

        cat.Nome = req.Nome;
        cat.Tipo = req.Tipo;
        cat.Cor = req.Cor;
        cat.Icone = req.Icone;
        await _db.SaveChangesAsync();
        return Ok(ToDto(cat));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var uid = GetUserId();
        var cat = await _db.Categorias.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == uid);
        if (cat is null) return NotFound();
        _db.Categorias.Remove(cat);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
