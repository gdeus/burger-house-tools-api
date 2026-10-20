using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using burger_house_tools_api.Models;
using burger_house_tools_api.DTOs;

namespace burger_house_tools_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var users = await _context.Users.FindAsync(id);
        
        if (users is null) 
            return NotFound(new { message = "Usuario não encontrado." });
            
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserInputDto input)
    {
        // Converte o DTO para a Entidade do banco
        var user = new User
        {
            Name = input.Name,
            Login = input.Login,
            Password = input.Password,
            UserRole = input.UserRole
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Retorna o status 201 Created apontando para a rota de GetById
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserInputDto input)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user is null) 
            return NotFound(new { message = "Usuario não encontrado." });

        user.Name = input.Name;
        user.Login = input.Login;
        user.Password = input.Password;
        user.UserRole = input.UserRole;

        await _context.SaveChangesAsync();
        
        return NoContent(); // Status 204 para atualizações com sucesso
    }

    [HttpDelete("{id}")]
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