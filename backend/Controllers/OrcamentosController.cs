using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceiroApi.Data;
using FinanceiroApi.DTOs;
using FinanceiroApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FinanceiroApi.Controllers;

[ApiController]
[Route("api/orcamentos")]
[Authorize]
public class OrcamentosController : ControllerBase
{
    private readonly AppDbContext _db;
    public OrcamentosController(AppDbContext db) => _db = db;

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private static OrcamentoResponse ToDto(Orcamento o) => new(
        o.Id, o.ContaId, o.CategoriaId,
        o.Categoria is null ? null
            : new CategoriaResponse(o.Categoria.Id, o.Categoria.UsuarioId, o.Categoria.Nome, o.Categoria.Tipo, o.Categoria.Cor, o.Categoria.Icone),
        o.ValorLimite, o.MesReferencia, o.ValorGasto, o.PercentualUsado
    );

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? mes)
    {
        var uid = GetUserId();
        var query = _db.Orcamentos
            .AsNoTracking()
            .Include(o => o.Categoria)
            .Include(o => o.Conta)
            .Where(o => o.Conta.UsuarioId == uid);

        if (!string.IsNullOrEmpty(mes))
            query = query.Where(o => o.MesReferencia == mes);

        return Ok(await query.Select(o => ToDto(o)).ToListAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var uid = GetUserId();
        var o = await _db.Orcamentos
            .Include(o => o.Categoria)
            .Include(o => o.Conta)
            .FirstOrDefaultAsync(o => o.Id == id && o.Conta.UsuarioId == uid);
        return o is null ? NotFound() : Ok(ToDto(o));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] OrcamentoRequest req)
    {
        var uid = GetUserId();
        var jaGasto = await _db.Transacoes
            .Where(t => t.CategoriaId == req.CategoriaId
                     && t.Tipo == "Despesa"
                     && t.Status == "Confirmada"
                     && t.Conta.UsuarioId == uid
                     && t.Data.ToString().StartsWith(req.MesReferencia))
            .SumAsync(t => (decimal?)t.Valor) ?? 0m;

        var orc = new Orcamento
        {
            ContaId        = req.ContaId,
            CategoriaId    = req.CategoriaId,
            ValorLimite    = req.ValorLimite,
            MesReferencia  = req.MesReferencia,
            ValorGasto     = jaGasto,
        };

        _db.Orcamentos.Add(orc);
        await _db.SaveChangesAsync();
        await _db.Entry(orc).Reference(o => o.Categoria).LoadAsync();

        return CreatedAtAction(nameof(Obter), new { id = orc.Id }, ToDto(orc));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] OrcamentoRequest req)
    {
        var uid = GetUserId();
        var orc = await _db.Orcamentos.Include(o => o.Categoria).Include(o => o.Conta).FirstOrDefaultAsync(o => o.Id == id && o.Conta.UsuarioId == uid);
        if (orc is null) return NotFound();

        orc.ValorLimite   = req.ValorLimite;
        orc.MesReferencia = req.MesReferencia;
        await _db.SaveChangesAsync();
        return Ok(ToDto(orc));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var uid = GetUserId();
        var orc = await _db.Orcamentos.Include(o => o.Conta).FirstOrDefaultAsync(o => o.Id == id && o.Conta.UsuarioId == uid);
        if (orc is null) return NotFound();
        _db.Orcamentos.Remove(orc);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
