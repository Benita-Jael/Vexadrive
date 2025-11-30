using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VexaDriveAPI.DTO.AuthDTO;
using VexaDriveAPI.JWT;

namespace VexaDriveAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
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
            // Check for existing user by email
            var userExists = await _userManager.FindByEmailAsync(registerModel.Email);
            if (userExists != null)
            {
                return Conflict(new { message = "User already exists" });
            }

            var user = new IdentityUser
            {
                UserName = registerModel.Name,
                Email = registerModel.Email,
                EmailConfirmed = true, // Optional: confirm email for demo/local usage
                PhoneNumber = registerModel.ContactNumber
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                // Return detailed errors so frontend can display them
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return BadRequest(new { message = "User creation failed", errors });
            }

            // Ensure Customer role exists
            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            // Assign Customer role to new user
            var addRoleResult = await _userManager.AddToRoleAsync(user, "Customer");
            if (!addRoleResult.Succeeded)
            {
                var roleErrors = addRoleResult.Errors.Select(e => e.Description).ToArray();
                return StatusCode(500, new { message = "Failed to assign role to user", errors = roleErrors });
            }

            return Ok(new { message = "Customer registered successfully" });
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
                    // include both sub and the identity name identifier so downstream helpers can resolve the user id
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in roles)
                {
                    // add both role claim types for compatibility
                    claims.Add(new Claim(System.Security.Claims.ClaimTypes.Role, role));
                    claims.Add(new Claim("role", role));
                }

                // debug: write roles and claims to console to assist troubleshooting in dev
                Console.WriteLine($"[AuthController] User '{user.Email}' roles: {string.Join(',', roles)}");
                foreach (var c in claims)
                {
                    Console.WriteLine($"[AuthController] Claim: {c.Type} = {c.Value}");
                }

                var token = _jwtTokenGenerator.GenerateJwtToken(claims);

                // Return camelCase properties to match frontend models
                return Ok(new { token = token, roles = roles });
            }

            return Unauthorized(new { message = "Invalid credentials. Please try again." });
        }

    }
}
