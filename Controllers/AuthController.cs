using Employee_Self_Service.DAL;
using Employee_Self_Service.Modals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Employee_Self_Service.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Route("api/Auth/Register")]
        public async Task<IActionResult> Register(RegisterUser model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByNameAsync(model.UserName);

            if (existingUser != null)
                return BadRequest("Username already exists.");

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "Employee");

            return Ok(new
            {
                message = "User created successfully."
            });
        }

        // ============================ // Get All Users // ============================
        /*[Authorize(Roles = "Admin")] */
        /*[Route("api/Auth/GetAllUsers")] 
        [HttpGet] 
        public async Task<IActionResult> GetAllUsers() 
        { 
            var users = _userManager.Users.ToList(); 
            var userList = new List<object>(); 
            foreach (var user in users) 
            { 
                var roles = await _userManager.GetRolesAsync(user); 
                userList.Add(new { user.Id, user.UserName, user.Email, user.FullName, Roles = roles }); 
            } 
            
            return Ok(userList); 
        }*/

        [Route("api/Auth/login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUser model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
                return Unauthorized("Invalid username or password.");

            bool validPassword =
                await _userManager.CheckPasswordAsync(user, model.Password);

            if (!validPassword)
                return Unauthorized("Invalid username or password.");

            var token = await GenerateJwtToken(user);

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                token,
                expiresIn = 3600,
                roles,
                user = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FullName
                }   
            });
        }

        [Authorize(Roles = "Admin")]
        [Route("api/Auth/AssignRole")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(AssignRole dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user == null)
                return NotFound("User not found.");

            if (!await _roleManager.RoleExistsAsync(dto.RoleName))
                return BadRequest("Role does not exist.");

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains(dto.RoleName))
                return BadRequest("User already has this role.");

            var result =
                await _userManager.AddToRoleAsync(user, dto.RoleName);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Role assigned successfully.");
        }
    }
}