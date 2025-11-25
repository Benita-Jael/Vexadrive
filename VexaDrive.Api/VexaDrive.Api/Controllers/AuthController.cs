using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
 
        public VexaDriveAuthController(UserManager<IdentityUser> userManager, JwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
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
 
            return Ok(new { Status = "Success", Message = "User created successfully" });
        }
 
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
 
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var token = _jwtTokenGenerator.GenerateJWtToken(user);
                return Ok(new { Token = token });
            }
 
            return Unauthorized(new { Status = "Error", Message = "Invalid credentials. Please try again." });
        }
    }
}