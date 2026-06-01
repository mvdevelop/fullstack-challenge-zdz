using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceiroApi.Data;
using FinanceiroApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FinanceiroApi.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize]
public class RelatoriosController : ControllerBase
{
    private readonly AppDbContext _db;
    public RelatoriosController(AppDbContext db) => _db = db;

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet("mensal")]
    public async Task<IActionResult> Mensal([FromQuery] int meses = 6)
    {
        var uid = GetUserId();
        var inicio = DateTime.UtcNow.AddMonths(-meses + 1);
        var inicio1 = new DateTime(inicio.Year, inicio.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var transacoes = await _db.Transacoes
            .AsNoTracking()
            .Include(t => t.Conta)
            .Where(t => t.Data >= inicio1 && t.Status == "Confirmada" && t.Conta.UsuarioId == uid)
            .Select(t => new { t.Data, t.Tipo, t.Valor })
            .ToListAsync();

        var resultado = transacoes
            .GroupBy(t => t.Data.ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .Select(g => new ResumoMensalResponse(
                Mes: g.Key,
                TotalReceitas: g.Where(t => t.Tipo == "Receita").Sum(t => t.Valor),
                TotalDespesas: g.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor),
                Saldo: g.Where(t => t.Tipo == "Receita").Sum(t => t.Valor)
                       - g.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor)
            ))
            .ToList();

        return Ok(resultado);
    }

    [HttpGet("por-categoria")]
    public async Task<IActionResult> PorCategoria([FromQuery] string? mes)
    {
        var uid = GetUserId();
        var mesRef = mes ?? DateTime.UtcNow.ToString("yyyy-MM");
        var inicio = DateTime.Parse($"{mesRef}-01").ToUniversalTime();
        var fim    = inicio.AddMonths(1).AddSeconds(-1);

        var transacoes = await _db.Transacoes
            .AsNoTracking()
            .Include(t => t.Categoria)
            .Include(t => t.Conta)
            .Where(t => t.Tipo == "Despesa"
                     && t.Status == "Confirmada"
                     && t.Data >= inicio
                     && t.Data <= fim
                     && t.Categoria != null
                     && t.Conta.UsuarioId == uid)
            .Select(t => new { t.CategoriaId, t.Categoria!.Nome, t.Categoria.Cor, t.Valor })
            .ToListAsync();

        var totalGeral = transacoes.Sum(t => t.Valor);

        var resultado = transacoes
            .GroupBy(t => new { t.CategoriaId, t.Nome, t.Cor })
            .Select(g => new GastoPorCategoriaResponse(
                CategoriaId:    g.Key.CategoriaId,
                CategoriaNome:  g.Key.Nome,
                CategoriaCor:   g.Key.Cor,
                Total:          g.Sum(t => t.Valor),
                Percentual:     totalGeral > 0
                                    ? Math.Round(g.Sum(t => t.Valor) / totalGeral * 100, 2)
                                    : 0
            ))
            .OrderByDescending(r => r.Total)
            .ToList();

        return Ok(resultado);
    }
}
