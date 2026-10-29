using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using burger_house_tools_api.Models;
using burger_house_tools_api.DTOs;
using burger_house_tools_api.Services;

namespace burger_house_tools_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly KeycloakService _keycloakService;

    public UsersController(AppDbContext context, KeycloakService keycloakService)
    {
        _context = context;
        _keycloakService = keycloakService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Caixa,Garcom")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Caixa,Garcom")]
    public async Task<IActionResult> GetById(int id)
    {
        var users = await _context.Users.FindAsync(id);
        
        if (users is null) 
            return NotFound(new { message = "Usuario não encontrado." });
            
        return Ok(users);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] UserInputDto input)
    {
        // 1. Cria o usuário no Keycloak (login, senha e role)
        string keycloakId;
        try
        {
            keycloakId = await _keycloakService.CreateUserAsync(
                input.Login, input.Password, input.UserRole.ToString());
        }
        catch (HttpRequestException ex)
        {
            return BadRequest(new { message = "Erro ao criar usuário no Keycloak.", details = ex.Message });
        }

        // 2. Salva os dados de negócio na tabela local
        var user = new User
        {
            KeycloakId = keycloakId,
            Name = input.Name,
            Login = input.Login,
            Password = input.Password,
            UserRole = input.UserRole,
            DailyRate = input.DailyRate,
            Pix = input.Pix
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UserInputDto input)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user is null) 
            return NotFound(new { message = "Usuario não encontrado." });

        user.Name = input.Name;
        user.Login = input.Login;
        user.Password = input.Password;
        user.UserRole = input.UserRole;
        user.DailyRate = input.DailyRate;
        user.Pix = input.Pix;

        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user is null) 
            return NotFound(new { message = "Usuário não encontrado." });

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}
