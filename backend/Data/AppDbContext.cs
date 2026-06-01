using Microsoft.EntityFrameworkCore;
using FinanceiroApi.Models;

namespace FinanceiroApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Conta> Contas => Set<Conta>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();
    public DbSet<Orcamento> Orcamentos => Set<Orcamento>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        foreach (var prop in mb.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType == typeof(decimal)))
        {
            prop.SetColumnType("TEXT"); 
        }

        mb.Entity<Transacao>()
            .HasOne(t => t.Conta)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.ContaId)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Transacao>()
            .HasOne(t => t.Categoria)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Orcamento>()
            .HasOne(o => o.Conta)
            .WithMany(c => c.Orcamentos)
            .HasForeignKey(o => o.ContaId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<Orcamento>()
            .HasOne(o => o.Categoria)
            .WithMany(c => c.Orcamentos)
            .HasForeignKey(o => o.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed data moved to Program.cs to handle BCrypt hashing properly
    }
}
