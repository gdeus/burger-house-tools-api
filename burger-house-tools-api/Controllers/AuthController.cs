using burger_house_tools_api.DTOs;
using burger_house_tools_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace burger_house_tools_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly KeycloakService _keycloakService;

    public AuthController(KeycloakService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    /// <summary>
    /// Realiza a autenticação do usuário retornando o JWT Bearer Token gerado pelo Keycloak.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginInputDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Login) || string.IsNullOrWhiteSpace(input.Password))
        {
            return BadRequest(new { message = "Login e senha são obrigatórios." });
        }

        var result = await _keycloakService.LoginAsync(input.Login, input.Password);

        if (result is null)
        {
            return Unauthorized(new { message = "Usuário ou senha inválidos." });
        }

        return Ok(result);
    }
}
