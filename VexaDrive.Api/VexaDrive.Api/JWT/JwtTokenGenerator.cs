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
 
        public string GenerateJWtToken(IdentityUser user)

        {

            // Secret key from appsettings

            var key = new SymmetricSecurityKey(

                Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"])

            );
 
            // Signing credentials

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
 
            // Token expiry

            var tokenExpiry = DateTime.Now.AddMinutes(

                Convert.ToDouble(_configuration["JwtSettings:Expiry"])

            );
 
            // Claims

            var claims = new List<Claim>

            {

                new Claim(ClaimTypes.Role, "Admin"), // You can later add role dynamically

                new Claim(ClaimTypes.Email, user.Email)

            };
 
            // Create token

            var token = new JwtSecurityToken(

                issuer: _configuration["JwtSettings:Issuer"],

                audience: _configuration["JwtSettings:Audience"],

                expires: tokenExpiry,

                signingCredentials: creds,

                claims: claims

            );
 
            // Return serialized token

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

    }

}

 