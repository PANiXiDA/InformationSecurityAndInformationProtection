namespace BL.Interfaces
{
    public interface ITokensBL
    {
        string GenerateAccessToken(string username);
        string GenerateRefreshToken(string username);
        (string AccessToken, string RefreshToken)? RefreshTokens(string refreshToken);
    }
}
