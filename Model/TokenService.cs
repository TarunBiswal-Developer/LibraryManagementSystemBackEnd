using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService ( IConfiguration configuration )
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken ( IdentityUser user, IList<string> roles )
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName)
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration ["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        var currentUtcTime = DateTime.UtcNow;
        var expiryTime = TimeZoneInfo.ConvertTimeFromUtc(currentUtcTime, istTimeZone).AddHours(1);

        var token = new JwtSecurityToken(
            _configuration ["Jwt:Issuer"],
             _configuration ["Jwt:Issuer"],
            claims,
            expires: expiryTime,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

 