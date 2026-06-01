using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceiroApi.Data;
using FinanceiroApi.DTOs;
using FinanceiroApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FinanceiroApi.Controllers;

[ApiController]
[Route("api/contas")]
[Authorize]
public class ContasController : ControllerBase
{
    private readonly AppDbContext _db;
    public ContasController(AppDbContext db) => _db = db;

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private static ContaResponse ToDto(Conta c) =>
        new(c.Id, c.Nome, c.Tipo, c.SaldoInicial, c.SaldoAtual, c.Moeda, c.Ativa);

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var uid = GetUserId();
        var contas = await _db.Contas
            .AsNoTracking()
            .Where(c => c.UsuarioId == uid)
            .Select(c => ToDto(c))
            .ToListAsync();
        return Ok(contas);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var uid = GetUserId();
        var conta = await _db.Contas.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == uid);
        return conta is null ? NotFound() : Ok(ToDto(conta));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ContaRequest req)
    {
        var uid = GetUserId();
        var conta = new Conta
        {
            UsuarioId = uid,
            Nome = req.Nome,
            Tipo = req.Tipo,
            SaldoInicial = req.SaldoInicial,
            SaldoAtual = req.SaldoInicial,
            Moeda = req.Moeda,
            Ativa = true
        };
        _db.Contas.Add(conta);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obter), new { id = conta.Id }, ToDto(conta));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] ContaRequest req)
    {
        var uid = GetUserId();
        var conta = await _db.Contas.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == uid);
        if (conta is null) return NotFound();

        conta.Nome = req.Nome;
        conta.Tipo = req.Tipo;
        conta.Moeda = req.Moeda;

        await _db.SaveChangesAsync();
        return Ok(ToDto(conta));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var uid = GetUserId();
        var conta = await _db.Contas.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == uid);
        if (conta is null) return NotFound();

        conta.Ativa = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
