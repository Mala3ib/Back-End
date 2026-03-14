namespace Mala3ib.BLL.Contracts.Player
{
    public record PlayerProfileDto (
        string Email, 
        string FirstName,
        string LastName,
        string PhoneNumber, 
        DateOnly DateOfBirth
    );
}
