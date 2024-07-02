using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateToken(User user)
		{
			string tokenKey = _configuration["TokenKey"] ?? throw new Exception("Can`t access the token from appconfig");
			if (tokenKey.Length < 64) throw new Exception("The given token must be longer");

			SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(tokenKey));

			List<Claim> claim = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, user.Name)
			};

			SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512Signature);

			SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
			{
				Subject = new ClaimsIdentity(claim),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = creds
			};

			JwtSecurityTokenHandler tokenHandler = new();
			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}
	}
}
