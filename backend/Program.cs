using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FinanceiroApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")
        ?? "Data Source=financeiro.db"));

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Financeiro API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p
        .WithOrigins("http://localhost:3000", "http://localhost:3001")
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Seed default user if none exists
    if (!db.Usuarios.Any())
    {
        var uid = Guid.Parse("11111111-0000-0000-0000-000000000001");
        var cid = Guid.Parse("22222222-0000-0000-0000-000000000001");
        var cat1 = Guid.Parse("33333333-0000-0000-0000-000000000001");
        var cat2 = Guid.Parse("33333333-0000-0000-0000-000000000002");
        var cat3 = Guid.Parse("33333333-0000-0000-0000-000000000003");

        db.Usuarios.Add(new FinanceiroApi.Models.Usuario
        {
            Id = uid, Nome = "João Silva", Email = "joao@email.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            CriadoEm = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        db.Contas.Add(new FinanceiroApi.Models.Conta
        {
            Id = cid, UsuarioId = uid, Nome = "Conta Corrente", Tipo = "Corrente",
            SaldoInicial = 1000m, SaldoAtual = 5920m, Moeda = "BRL", Ativa = true
        });

        db.Categorias.AddRange(
            new FinanceiroApi.Models.Categoria { Id = cat1, UsuarioId = uid, Nome = "Salário", Tipo = "Receita", Cor = "#2E7D32", Icone = "mdi-briefcase" },
            new FinanceiroApi.Models.Categoria { Id = cat2, UsuarioId = uid, Nome = "Moradia", Tipo = "Despesa", Cor = "#C62828", Icone = "mdi-home" },
            new FinanceiroApi.Models.Categoria { Id = cat3, UsuarioId = uid, Nome = "Alimentação", Tipo = "Despesa", Cor = "#F57F17", Icone = "mdi-food" }
        );

        db.Transacoes.AddRange(
            new FinanceiroApi.Models.Transacao { ContaId = cid, CategoriaId = cat1, Valor = 6800m, Tipo = "Receita", Descricao = "Salário junho", Data = new DateTime(2025, 6, 1), Status = "Confirmada", CriadoEm = DateTime.UtcNow },
            new FinanceiroApi.Models.Transacao { ContaId = cid, CategoriaId = cat2, Valor = 1800m, Tipo = "Despesa", Descricao = "Aluguel junho", Data = new DateTime(2025, 6, 5), Status = "Confirmada", CriadoEm = DateTime.UtcNow },
            new FinanceiroApi.Models.Transacao { ContaId = cid, CategoriaId = cat3, Valor = 320m, Tipo = "Despesa", Descricao = "Mercado Extra", Data = new DateTime(2025, 6, 8), Status = "Confirmada", CriadoEm = DateTime.UtcNow }
        );

        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
