using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceiroApi.Data;
using FinanceiroApi.DTOs;
using FinanceiroApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FinanceiroApi.Controllers;

[ApiController]
[Route("api/transacoes")]
[Authorize]
public class TransacoesController : ControllerBase
{
    private readonly AppDbContext _db;
    public TransacoesController(AppDbContext db) => _db = db;

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private static TransacaoResponse ToDto(Transacao t) => new(
        t.Id, t.ContaId,
        t.Conta is null ? null : new ContaResponse(t.Conta.Id, t.Conta.Nome, t.Conta.Tipo, t.Conta.SaldoInicial, t.Conta.SaldoAtual, t.Conta.Moeda, t.Conta.Ativa),
        t.CategoriaId,
        t.Categoria is null ? null : new CategoriaResponse(t.Categoria.Id, t.Categoria.UsuarioId, t.Categoria.Nome, t.Categoria.Tipo, t.Categoria.Cor, t.Categoria.Icone),
        t.Valor, t.Tipo, t.Descricao, t.Data, t.Status, t.CriadoEm
    );

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? tipo,
        [FromQuery] Guid? categoriaId,
        [FromQuery] Guid? contaId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] string? busca)
    {
        var uid = GetUserId();
        var query = _db.Transacoes
            .AsNoTracking()
            .Include(t => t.Conta)
            .Include(t => t.Categoria)
            .Where(t => t.Conta.UsuarioId == uid);

        if (!string.IsNullOrEmpty(tipo))         query = query.Where(t => t.Tipo == tipo);
        if (categoriaId.HasValue)                query = query.Where(t => t.CategoriaId == categoriaId);
        if (contaId.HasValue)                    query = query.Where(t => t.ContaId == contaId);
        if (dataInicio.HasValue)                 query = query.Where(t => t.Data >= dataInicio.Value);
        if (dataFim.HasValue)                    query = query.Where(t => t.Data <= dataFim.Value);
        if (!string.IsNullOrEmpty(busca))        query = query.Where(t => t.Descricao.Contains(busca));

        var resultado = await query
            .OrderByDescending(t => t.Data)
            .ThenByDescending(t => t.CriadoEm)
            .Select(t => ToDto(t))
            .ToListAsync();

        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var uid = GetUserId();
        var t = await _db.Transacoes
            .Include(t => t.Conta)
            .Include(t => t.Categoria)
            .FirstOrDefaultAsync(t => t.Id == id && t.Conta.UsuarioId == uid);

        return t is null ? NotFound() : Ok(ToDto(t));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] TransacaoRequest req)
    {
        var uid = GetUserId();
        var conta = await _db.Contas.FirstOrDefaultAsync(c => c.Id == req.ContaId && c.UsuarioId == uid);
        if (conta is null) return BadRequest("Conta não encontrada.");

        var transacao = new Transacao
        {
            ContaId     = req.ContaId,
            CategoriaId = req.CategoriaId,
            Valor       = req.Valor,
            Tipo        = req.Tipo,
            Descricao   = req.Descricao,
            Data        = req.Data,
            Status      = req.Status,
        };

        if (req.Status == "Confirmada")
        {
            conta.SaldoAtual += req.Tipo == "Receita" ? req.Valor : -req.Valor;
        }

        if (req.Tipo == "Despesa" && req.Status == "Confirmada")
        {
            var mesRef = req.Data.ToString("yyyy-MM");
            var orc = await _db.Orcamentos
                .Include(o => o.Conta)
                .FirstOrDefaultAsync(o => o.CategoriaId == req.CategoriaId && o.MesReferencia == mesRef && o.Conta.UsuarioId == uid);
            if (orc is not null) orc.ValorGasto += req.Valor;
        }

        _db.Transacoes.Add(transacao);
        await _db.SaveChangesAsync();

        await _db.Entry(transacao).Reference(t => t.Conta).LoadAsync();
        await _db.Entry(transacao).Reference(t => t.Categoria).LoadAsync();

        return CreatedAtAction(nameof(Obter), new { id = transacao.Id }, ToDto(transacao));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] TransacaoRequest req)
    {
        var uid = GetUserId();
        var transacao = await _db.Transacoes
            .Include(t => t.Conta)
            .FirstOrDefaultAsync(t => t.Id == id && t.Conta.UsuarioId == uid);
        if (transacao is null) return NotFound();

        if (transacao.Status == "Confirmada" && transacao.Conta is not null)
        {
            transacao.Conta.SaldoAtual -= transacao.Tipo == "Receita" ? transacao.Valor : -transacao.Valor;
        }

        transacao.ContaId     = req.ContaId;
        transacao.CategoriaId = req.CategoriaId;
        transacao.Valor       = req.Valor;
        transacao.Tipo        = req.Tipo;
        transacao.Descricao   = req.Descricao;
        transacao.Data        = req.Data;
        transacao.Status      = req.Status;

        var conta = await _db.Contas.FirstOrDefaultAsync(c => c.Id == req.ContaId && c.UsuarioId == uid);
        if (conta is not null && req.Status == "Confirmada")
        {
            conta.SaldoAtual += req.Tipo == "Receita" ? req.Valor : -req.Valor;
        }

        await _db.SaveChangesAsync();
        await _db.Entry(transacao).Reference(t => t.Categoria).LoadAsync();

        return Ok(ToDto(transacao));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var uid = GetUserId();
        var transacao = await _db.Transacoes
            .Include(t => t.Conta)
            .FirstOrDefaultAsync(t => t.Id == id && t.Conta.UsuarioId == uid);
        if (transacao is null) return NotFound();

        if (transacao.Status == "Confirmada" && transacao.Conta is not null)
        {
            transacao.Conta.SaldoAtual -= transacao.Tipo == "Receita" ? transacao.Valor : -transacao.Valor;
        }

        _db.Transacoes.Remove(transacao);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
