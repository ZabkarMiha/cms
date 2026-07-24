using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Core.Services
{
    public interface IJWTSecurityTokenService
    {
        JwtSecurityToken GetToken(List<Claim> authClaims);
    }

    public class JWTSecurityTokenService : IJWTSecurityTokenService
    {
        private readonly JWT _jwtOptions;

        public JWTSecurityTokenService(IOptions<JWT> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.ValidIssuer,
                audience: _jwtOptions.ValidAudience,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(
                    authSigningKey,
                    SecurityAlgorithms.HmacSha256
                )
            );

            return token;
        }
    }
}
