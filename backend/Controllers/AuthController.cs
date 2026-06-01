using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceiroApi.Data;
using FinanceiroApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FinanceiroApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (await _db.Usuarios.AnyAsync(u => u.Email == req.Email))
            return BadRequest("Email já cadastrado.");

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(req.Senha);

        var usuario = new Usuario
        {
            Nome = req.Nome,
            Email = req.Email,
            SenhaHash = senhaHash,
            CriadoEm = DateTime.UtcNow
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        var token = GerarToken(usuario);

        return Ok(new AuthResponse(usuario.Id, usuario.Nome, usuario.Email, token));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(req.Senha, usuario.SenhaHash))
            return Unauthorized("Email ou senha inválidos.");

        var token = GerarToken(usuario);

        return Ok(new AuthResponse(usuario.Id, usuario.Nome, usuario.Email, token));
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id)) return Unauthorized();

        var usuario = await _db.Usuarios.FindAsync(Guid.Parse(id));
        if (usuario is null) return NotFound();

        return Ok(new { usuario.Id, usuario.Nome, usuario.Email });
    }

    private string GerarToken(Usuario usuario)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record RegisterRequest(string Nome, string Email, string Senha);
public record LoginRequest(string Email, string Senha);
public record AuthResponse(Guid Id, string Nome, string Email, string Token);
