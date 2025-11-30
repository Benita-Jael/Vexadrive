using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VexaDriveAPI.JWT
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Generate token from claims (roles included)
        public string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenExpiry = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: tokenExpiry,
                signingCredentials: creds,
                claims: claims
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Helper: Generate token directly from IdentityUser + roles
        public async Task<string> GenerateTokenForUserAsync(IdentityUser user, UserManager<IdentityUser> userManager)
        {
            // Retrieve the actual roles from UserManager
            var roles = await userManager.GetRolesAsync(user);

            var userClaims = new List<Claim>
            {
                // Include both the standard JWT "sub" claim and the ASP.NET identity name identifier
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add role claims for each role the user has
            foreach (var role in roles)
            {
                userClaims.Add(new Claim(System.Security.Claims.ClaimTypes.Role, role));
                userClaims.Add(new Claim("role", role));
            }

            return GenerateJwtToken(userClaims);
        }
    }
}
