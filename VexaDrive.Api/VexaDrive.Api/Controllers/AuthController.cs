using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VexaDriveAPI.DTO.AuthDTO;
using VexaDriveAPI.JWT;

namespace VexaDriveAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VexaDriveAuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly RoleManager<IdentityRole> _roleManager;

        public VexaDriveAuthController(
            UserManager<IdentityUser> userManager,
            JwtTokenGenerator jwtTokenGenerator,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerModel)
        {
            var userExists = await _userManager.FindByEmailAsync(registerModel.Email);
            if (userExists != null)
                return Conflict(new { Status = "Error", Message = "User already exists" });

            var user = new IdentityUser
            {
                UserName = registerModel.Name,
                Email = registerModel.Email
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
                return StatusCode(500, new { Status = "Error", Message = "User creation failed! Please try again." });

            // Ensure Customer role exists
            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            // Assign Customer role to new user
            await _userManager.AddToRoleAsync(user, "Customer");

            return Ok(new { Status = "Success", Message = "Customer registered successfully" });
        }

        [HttpPost("login")]
public async Task<IActionResult> Login(LoginDTO loginModel)
{
    var user = await _userManager.FindByEmailAsync(loginModel.Email);
    if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = _jwtTokenGenerator.GenerateJwtToken(claims);

        return Ok(new { Token = token, Roles = roles });
    }

    return Unauthorized(new { Status = "Error", Message = "Invalid credentials. Please try again." });
}

    }
}
