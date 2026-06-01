namespace FinanceiroApi.DTOs;

public record ContaRequest(
    string Nome,
    string Tipo,
    decimal SaldoInicial,
    string Moeda
);

public record ContaResponse(
    Guid Id,
    string Nome,
    string Tipo,
    decimal SaldoInicial,
    decimal SaldoAtual,
    string Moeda,
    bool Ativa
);

public record CategoriaRequest(
    string Nome,
    string Tipo,
    string Cor,
    string Icone
);

public record CategoriaResponse(
    Guid Id,
    Guid UsuarioId,
    string Nome,
    string Tipo,
    string Cor,
    string Icone
);

public record TransacaoRequest(
    Guid ContaId,
    Guid CategoriaId,
    decimal Valor,
    string Tipo,
    string Descricao,
    DateTime Data,
    string Status
);

public record TransacaoResponse(
    Guid Id,
    Guid ContaId,
    ContaResponse? Conta,
    Guid CategoriaId,
    CategoriaResponse? Categoria,
    decimal Valor,
    string Tipo,
    string Descricao,
    DateTime Data,
    string Status,
    DateTime CriadoEm
);

public record OrcamentoRequest(
    Guid ContaId,
    Guid CategoriaId,
    decimal ValorLimite,
    string MesReferencia
);

public record OrcamentoResponse(
    Guid Id,
    Guid ContaId,
    Guid CategoriaId,
    CategoriaResponse? Categoria,
    decimal ValorLimite,
    string MesReferencia,
    decimal ValorGasto,
    decimal PercentualUsado
);

public record ResumoMensalResponse(
    string Mes,
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal Saldo
);

public record GastoPorCategoriaResponse(
    Guid CategoriaId,
    string CategoriaNome,
    string CategoriaCor,
    decimal Total,
    decimal Percentual
);
