namespace FinanceiroApi.Models;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string SenhaHash { get; set; } = "";
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Conta> Contas { get; set; } = new List<Conta>();
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
}

public class Conta
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string Nome { get; set; } = "";
    public string Tipo { get; set; } = "Corrente"; 
    public decimal SaldoInicial { get; set; }
    public decimal SaldoAtual { get; set; }
    public string Moeda { get; set; } = "BRL";
    public bool Ativa { get; set; } = true;

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    public ICollection<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();
}

public class Categoria
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string Nome { get; set; } = "";
    public string Tipo { get; set; } = "Despesa"; 
    public string Cor { get; set; } = "#1976D2";
    public string Icone { get; set; } = "mdi-tag";

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    public ICollection<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();
}

public class Transacao
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ContaId { get; set; }
    public Conta? Conta { get; set; }
    public Guid CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = "Despesa";
    public string Descricao { get; set; } = "";
    public DateTime Data { get; set; }
    public string Status { get; set; } = "Confirmada";
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}

public class Orcamento
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ContaId { get; set; }
    public Conta? Conta { get; set; }
    public Guid CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public decimal ValorLimite { get; set; }
    public string MesReferencia { get; set; } = "";
    public decimal ValorGasto { get; set; }

    public decimal PercentualUsado => ValorLimite > 0
        ? Math.Round(ValorGasto / ValorLimite * 100, 2)
        : 0;
}
