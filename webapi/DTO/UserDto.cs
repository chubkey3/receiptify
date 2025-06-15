namespace webapi.DTO;

public class CreateUserDto
{
    public string Uid { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;
}