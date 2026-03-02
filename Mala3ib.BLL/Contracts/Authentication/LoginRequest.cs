namespace Mala3ib.BLL.Contracts.Authentication
{
    public record LoginRequest (
        string Email, 
        string Password
    );
}
