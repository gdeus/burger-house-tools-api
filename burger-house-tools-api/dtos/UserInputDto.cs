using burger_house_tools_api.Models.Enums;

namespace burger_house_tools_api.DTOs;

public class UserInputDto
{
    public string Name { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public UserRole UserRole { get; set; }
}