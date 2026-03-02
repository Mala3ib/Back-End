namespace Mala3ib.BLL.Contracts.Authentication
{
    public record RefreshTokenRequest(
        string Token,
        string RefreshToken
    );
}
